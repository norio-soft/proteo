using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;

using Orchestrator;
using System.IO;
using System.Data;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;

using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;

using Orchestrator.BulkScan.Types;

namespace Orchestrator.WebServices
{
    //Generic method call delegates for making async calls
    public delegate TOutput MethodCall<TOutput>();
    public delegate TOutput MethodCall<TOutput, TInput>(TInput input);

    /// <summary>
    /// Summary description for ScanUploaded
    /// </summary>
    [WebService(Namespace = "http://scanuploaded.orchestrator.co.uk/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class ScanUploaded : System.Web.Services.WebService
    {

        //---------------------------------------------------------------------------------------

        public ScanUploaded() { }

        //---------------------------------------------------------------------------------------

        [WebMethod]
        public List<Orchestrator.BulkScan.Types.ScanResult> GetScanUploadDefaultInformationAndExistingScans(string barcodes)
        {
            if (string.IsNullOrEmpty(barcodes))
                return new List<ScanResult>();

            List<ScanResult> result = new List<ScanResult>();
            Facade.Form facForm = new Orchestrator.Facade.Form();
            Facade.ResourceManifest facRM = new Orchestrator.Facade.ResourceManifest();
            Facade.IOrder facOrder = new Orchestrator.Facade.Order();
            Entities.Scan scan = null;
            ScanResult sr = null;

            foreach (string barcode in barcodes.Split(":".ToCharArray()))
            {
                try
                {
                    // The 3rd and 4th chars of the barcode indicate the form type
                    switch (barcode.Substring(2, 2))
                    {
                        case "02": // Resource Manifests

                            string extractedResourceManifestId = barcode.Substring(4, 8);
                            for (int i = 0; i < 8; i++)
                                if (extractedResourceManifestId.Length > 0 && extractedResourceManifestId.Substring(0, 1) == "0")
                                    extractedResourceManifestId = extractedResourceManifestId.Substring(1);
                                else
                                    break;

                            int resourceManifestId = Convert.ToInt32(extractedResourceManifestId);

                            Entities.ResourceManifest rm = facRM.GetResourceManifest(resourceManifestId);

                            if (rm != null)
                            {
                                sr = new ScanResult();
                                sr.Barcode = barcode;
                                sr.FormType = "02";

                                if (rm.ScannedFormId > 0)
                                {
                                    scan = facForm.GetForScannedFormId(rm.ScannedFormId);
                                    if (scan != null)
                                    {
                                        sr.PDFPath = scan.ScannedFormPDF;
                                        sr.Replace = false;
                                    }
                                }

                                result.Add(sr);
                            }

                            break;

                        //case "01": // Delivery Note

                        //    int orderId = Convert.ToInt32(barcode.Substring(3));
                        //    Entities.Order order = facOrder.GetForOrderID(orderId);

                        //    if (order != null)
                        //    {
                        //        sr = new ScanResult();
                        //        sr.Barcode = barcode;
                        //        sr.FormType = "01";

                        //        if (order.DeliveryNoteScannedFormId.HasValue)
                        //        {
                        //            scan = facForm.GetForScannedFormId(order.DeliveryNoteScannedFormId.Value);
                   
                        //            if (scan != null)
                        //            {
                        //                sr.PDFPath = scan.ScannedFormPDF;
                        //                sr.Replace = false;
                        //            }
                        //        }

                        //        result.Add(sr);
                        //    }

                        //    break;

                        case "01": // PODS

                            string extratedOrderId = barcode.Substring(4, 8);
                            for (int i = 0; i < 8; i++)
                                if (extratedOrderId.Length > 0 && extratedOrderId.Substring(0, 1) == "0")
                                    extratedOrderId = extratedOrderId.Substring(1);
                                else
                                    break;
                            int orderId2 = Convert.ToInt32(extratedOrderId);

                            Entities.Order order2 = facOrder.GetForOrderID(orderId2);
                            if (order2 != null)
                            {
                                Facade.POD facPod = new Orchestrator.Facade.POD();
                                Entities.POD pod = facPod.GetForOrderID(orderId2);

                                BatchItemPodInfo podInfo = new BatchItemPodInfo();

                                sr = new ScanResult();
                                sr.Barcode = barcode;
                                sr.FormType = "01";

                                if (pod != null)
                                {
                                    scan = facForm.GetForScannedFormId(pod.ScannedFormId);
                                    if (scan != null)
                                    {
                                        sr.PDFPath = scan.ScannedFormPDF;
                                        sr.Replace = false;
                                    }
                                }

                                int collectDropId = facOrder.GetDeliveryCollectDropIDForPODScanner(orderId2);
                                int JobId = facOrder.GetDeliveryJobIDForOrderID(orderId2);

                                DataSet dsJobDetails = facPod.GetJobDetails(JobId);

                                bool foundCollectDrop = false;
                                foreach (DataRow row in dsJobDetails.Tables["CollectionDrop"].Rows)
                                {
                                    if ((int)row["CollectDropId"] == collectDropId)
                                    {
                                        podInfo.TicketNumber = row["ClientsCustomerReference"].ToString();
                                        podInfo.SignatureDate = (DateTime)row["CollectDropDateTime"];
                                        foundCollectDrop = true;
                                        break;
                                    }
                                }

                                // If we don't find pod info, default it to something sensible.
                                if (!foundCollectDrop)
                                {
                                    podInfo.TicketNumber = "";
                                    podInfo.SignatureDate = DateTime.Now;
                                }

                                sr.BatchItemInfo = SerializeBatchItemInfoToString(sr.FormType, podInfo);

                                result.Add(sr);
                            }

                            break;

                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ScanResult srError = new ScanResult();
                    srError.Barcode = barcode;
                    srError.ErrorOccurred = true;
                    srError.ErrorText = ex.Message;
                    result.Add(srError);
                }
            }

            return result;
        }

        private string SerializeBatchItemInfoToString(string formType, Orchestrator.BulkScan.Types.BatchItemInfo batchItemInfo)
        {
            try
            {
                XmlSerializer serializer = null;

                switch (formType)
                {
                    case "02": // Resource Manifests
                        serializer = new XmlSerializer(typeof(BatchItemResourceManifestInfo));
                        break;

                    //case "01":
                    //    serializer = new XmlSerializer(typeof(BatchItemDeliveryNoteInfo));
                    //    break;

                    case "01": // PODs
                        serializer = new XmlSerializer(typeof(BatchItemPodInfo));
                        break;

                    default:
                        break;
                }

                using (StringWriter writer = new StringWriter())
                {
                    serializer.Serialize(writer, batchItemInfo);
                    return writer.ToString();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private BatchItemInfo DeserializeBatchItemInfoFromString(ScanPreparation sp)
        {
            string batchItemInfo = sp.BatchItemInfo;

            if (!string.IsNullOrEmpty(batchItemInfo))
            {
                XmlSerializer deserializer = null;

                switch (sp.FormType)
                {
                    case "02": // Resource Manifests
                        deserializer = new XmlSerializer(typeof(BatchItemResourceManifestInfo));
                        break;

                    //case "01": // DEL NOTES
                    //    deserializer = new XmlSerializer(typeof(BatchItemDeliveryNoteInfo));
                    //    break;

                    case "01": // PODS
                        deserializer = new XmlSerializer(typeof(BatchItemPodInfo));
                        break;

                    default:
                        break;
                }
                return (BatchItemInfo)deserializer.Deserialize(new StringReader(batchItemInfo));
            }
            else
            {
                return null;
            }
        }

        [WebMethod]
        public bool PrepareForBulkScanUpload(List<ScanPreparation> scanPreps, string userId)
        {
            bool result = false;

            foreach (ScanPreparation sp in scanPreps)
            {
                if (sp.FormType == "02") // Resource Manifest
                {
                    Entities.Scan manifest = null;

                    // Resource Manifest
                    if (Convert.ToInt32(sp.RecordId) > 0)
                    {
                        Facade.Form facForm = new Facade.Form();

                        manifest = new Entities.Scan();

                        // If this is not a replace (ie its an 'append'), then the previousScannedFormId value must be set.
                        // The previousScannedFormId should also be set when a placeholder row (with NoDocumentScanned.pdf) is present.

                        Facade.ResourceManifest facResourceManifest = new Orchestrator.Facade.ResourceManifest();
                        Entities.ResourceManifest rm = facResourceManifest.GetResourceManifest(int.Parse(sp.RecordId));
                        if (rm.ScannedFormId > 0)
                            manifest.PreviousScannedFormId = rm.ScannedFormId;

                        manifest.ScannedDateTime = DateTime.Today;
                        manifest.FormTypeId = eFormTypeId.Manifest;
                        manifest.ScannedFormPDF = sp.PDFFileName;
                        manifest.IsAppend = !sp.Replace;
                        manifest.IsUploaded = false;

                        int scanFormId = facForm.CreateNew(manifest, userId);

                        if (scanFormId > 0)
                        {
                            manifest.ScannedFormId = scanFormId;

                            // Update manifest record here
                            facResourceManifest.UpdateResourceManifestScan(int.Parse(sp.RecordId), scanFormId, userId);
                        }
                    }

                    result = true;
                }

                if (sp.FormType == "01") // POD
                {
                    Facade.IOrder facOrder = new Facade.Order();
                    Facade.POD facPod = new Orchestrator.Facade.POD();

                    Entities.POD pod = null;

                    int collectDropId = facOrder.GetDeliveryCollectDropIDForPODScanner(int.Parse(sp.RecordId));
                    int JobId = facOrder.GetDeliveryJobIDForOrderID(int.Parse(sp.RecordId));

                    pod = facPod.GetForOrderID(int.Parse(sp.RecordId));
                    if (pod == null)
                        pod = new Entities.POD();

                    BatchItemPodInfo podInfo = (BatchItemPodInfo)DeserializeBatchItemInfoFromString(sp);
                    if (podInfo != null)
                    {
                        pod.TicketNo = podInfo.TicketNumber;
                        pod.SignatureDate = podInfo.SignatureDate;
                    }
                    else
                    {
                        pod.TicketNo = string.Empty;
                        pod.SignatureDate = DateTime.Now;
                    }
                    
                    //pod.PreviousScannedFormId

                    pod.ScannedDateTime = DateTime.Today;
                    pod.JobId = JobId;
                    pod.CollectDropId = collectDropId;
                    pod.OrganisationId = 0;

                    if (pod.ScannedFormPDF != null && pod.ScannedFormPDF.ToLower().Contains(Globals.Constants.NO_DOCUMENT_AVAILABLE.ToLower()))
                        pod.IsAppend = false;
                    else
                        pod.IsAppend = !sp.Replace;

                    pod.ScannedFormPDF = sp.PDFFileName;

                    if (pod.ScannedFormId > 0)
                    {
                        facPod.Update(pod, userId);
                        // re get the pod entry as the scannedFormId will have changed.
                        pod = facPod.GetForPODId(pod.PODId);
                    }
                    else
                    {
                        int podId = facPod.Create(pod, JobId, 0, collectDropId, userId);

                        // get the newly created pod.
                        pod = facPod.GetForPODId(podId);
                    }

                    result = true;
                }

                //if (sp.FormType == "01") // delivery notes
                //{
                //    Entities.Scan deliveryNote = null;

                //    // delivery note
                //    if (Convert.ToInt32(sp.RecordId) > 0)
                //    {
                //        Facade.IOrder facOrder = new Facade.Order();
                //        Facade.Form facForm = new Facade.Form();

                //        deliveryNote = new Entities.Scan();

                //        // If this is not a replace (ie its an 'append'), then the previousScannedFormId value must be set.
                //        // The previousScannedFormId should also be set when a placeholder row (with NoDocumentScanned.pdf) is present.

                //        Entities.Order order = facOrder.GetForOrderID(int.Parse(sp.RecordId));
                //        if (order.DeliveryNoteScannedFormId > 0)
                //            deliveryNote.PreviousScannedFormId = order.DeliveryNoteScannedFormId;

                //        deliveryNote.ScannedDateTime = DateTime.Today;
                //        deliveryNote.FormTypeId = eFormTypeId.DeliveryNote;
                //        deliveryNote.ScannedFormPDF = sp.PDFFileName;
                //        deliveryNote.IsAppend = !sp.Replace;
                //        deliveryNote.IsUploaded = false;

                //        int scanFormId = facForm.CreateNew(deliveryNote, userId);

                //        if (scanFormId > 0)
                //        {
                //            deliveryNote.ScannedFormId = scanFormId;

                //            // Update manifest record here
                //            facOrder.UpdateDeliveryNoteScannedFormId(int.Parse(sp.RecordId), scanFormId, userId);
                //        }
                //    }

                //    result = true;
                //}
            }

            return result;
        }

        /// <summary>
        /// Called to indicate that a scan has been uploaded to the LOCAL server. This will be obselete and replaced with ScanHasBeenUploadedTo
        /// </summary>
        /// <param name="fileNameAndFTPDirectory">System file path and name</param>
        [WebMethod]
        public string ScanHasBeenUploaded(string fileNameAndFTPDirectory)
        {
            if (fileNameAndFTPDirectory.StartsWith("\\"))
                fileNameAndFTPDirectory = fileNameAndFTPDirectory.Substring(1, fileNameAndFTPDirectory.Length - 1);

            string pdfLocation = Server.MapPath("~/PDFS");
            string methodStatus = String.Empty;
            string newFileName = Path.GetFileName(fileNameAndFTPDirectory);
            string newFileLocation = Path.Combine(pdfLocation, fileNameAndFTPDirectory.Replace(newFileName, ""));
            string newFileLocationAndName = Path.Combine(newFileLocation, newFileName);
            Facade.Form facScanForm = new Orchestrator.Facade.Form();
            Entities.Scan scan = facScanForm.GetForScannedFormForScannedFormPDF(newFileName);

            if (scan.IsAppend && scan.PreviousScannedFormId != null)
            {
                Entities.Scan existingScan = null;

                // get the previous scannedForm Record
                existingScan = facScanForm.GetForScannedFormId((int)scan.PreviousScannedFormId);

                if (existingScan != null)
                {
                    PdfDocument previousDocument;
                    //string existingFileNameAndPath = Path.Combine(pdfLocation, existingScan.ScannedFormPDF);
                    string existingFileNameAndPath = Server.MapPath(existingScan.ScannedFormPDF);

                    if (File.Exists(existingFileNameAndPath))
                    {
                        previousDocument = PdfReader.Open(existingFileNameAndPath,PdfDocumentOpenMode.Import);

                        try
                        {
                            // should always exist
                            if (!Directory.Exists(newFileLocation))
                                Directory.CreateDirectory(newFileLocation);
                        }
                        catch (Exception ex)
                        {
                            throw new ApplicationException("ScanHasBeenUploaded - Error Creating directory - "
                                + newFileLocation, ex);
                        }

                        try
                        {
                            // read the exisiting but newly created pdf
                            PdfDocument newPdfDoc = PdfReader.Open(newFileLocationAndName,PdfDocumentOpenMode.Import);

                            PdfDocument finalDoc = new PdfDocument();
                            // Append the previous document to our new document
                            foreach(PdfPage page in previousDocument.Pages)
                                finalDoc.AddPage(page);
                            // Append the new document to the previous document
                            foreach(PdfPage page in newPdfDoc.Pages)
                                finalDoc.AddPage(page);

                            // attempt to save the new doc
                            finalDoc.Save(newFileLocationAndName);

                            methodStatus = "Appended " + newFileLocationAndName
                                + " to " + existingFileNameAndPath;
                        }
                        catch (Exception ex)
                        {
                            throw new ApplicationException("ScanHasBeenUploaded - Error appending document - "
                                + fileNameAndFTPDirectory, ex);
                        }
                    }
                    else
                        throw new ApplicationException("ExistingScanForm file could not be found - Path - "
                            + existingFileNameAndPath + " - New FileLocationAndName: " + newFileLocationAndName);
                }
                else
                    throw new ApplicationException("ExistingScanForm entity could not be found.");
            }
            else
                methodStatus = "Append = false or no previousScanneDformId to append too." + Environment.NewLine;

            scan.IsUploaded = true;
            
            //Use full URL instead
            //scan.ScannedFormPDF = fileNameAndFTPDirectory;
            string URL = this.Context.Request.Url.GetLeftPart(UriPartial.Authority) + "/PDFS/" + fileNameAndFTPDirectory.Replace('\\', '/');
            scan.ScannedFormPDF = URL;

            try
            {
                // update the IsUploaded flag on the scannedForm
                facScanForm.Update(scan, scan.CreateUserID);

                Facade.IPOD facPod = new Facade.POD();
                methodStatus += facPod.SendClientPod(scan.ScannedFormId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("ScanHasBeenUploaded - Update ScanForm failed (ScanForm ID:"+scan.ScannedFormId+")", ex);
            }

            return methodStatus;
        }
        
        /// <summary>
        /// Called to indicate that a scan has veen uploaded to ANY server. This will replace ScanHasBeenUploaded
        /// </summary>
        /// <param name="URL">Where the document was uploaded to</param>
        [WebMethod]
        public string ScanHasBeenUploadedTo(string URL)
        {
            string methodStatus = String.Empty;

            Uri URI = new Uri(URL);
            string newFileName = URI.Segments[URI.Segments.Length - 1];

            Facade.Form facScanForm = new Orchestrator.Facade.Form();
            Entities.Scan scan = facScanForm.GetForScannedFormForScannedFormPDF(newFileName);

            if (scan == null)
                throw new ApplicationException("ScanHasBeenUploadedTo - Scan not found for filename " + newFileName);

            scan.IsUploaded = true;
            scan.ScannedFormPDF = URL;

            try
            {
                // update the IsUploaded flag on the scannedForm
                facScanForm.Update(scan, scan.CreateUserID);

            }
            catch (Exception ex)
            {
                throw new ApplicationException("ScanHasBeenUploadedTo - Update ScanForm failed", ex);
            }

            //If the scan is a POD email it
            if (scan.FormTypeId == eFormTypeId.POD)
            {
                //Email the POD to the client
                Facade.IPOD facPod = new Facade.POD();

                //Make the call to send the email async as it will
                //have to download the attachment 
                MethodCall<string, int> asyncSendClientPod = facPod.SendClientPod;
                asyncSendClientPod.BeginInvoke(scan.ScannedFormId, r =>
                    {
                        try { asyncSendClientPod.EndInvoke(r); }
                        catch (Exception ex) { WebUI.Global.UnhandledException(ex); }
                    }, null);
            }

            return methodStatus;
        }

        /// <summary>
        /// Called to indicate that a scan has been uploaded.
        /// </summary>
        /// <param name="URL">Where the document was uploaded to</param>
        /// <param name="scannedFormId">The ScannedFormId of the uploaded document</param>
        [WebMethod]
        public string ScanHasBeenUploadedToWithId(string URL, int scannedFormId)
        {
            var methodStatus = String.Empty;

            var facScanForm = new Facade.Form();
            var scan = facScanForm.GetForScannedFormId(scannedFormId);

            if (scan == null)
            {
                // If scan is null try and use the old method based on file name.
                ScanHasBeenUploadedTo(URL);
                return methodStatus;
            }

            scan.IsUploaded = true;
            scan.ScannedFormPDF = URL;

            try
            {
                // update the IsUploaded flag on the scannedForm
                facScanForm.Update(scan, scan.CreateUserID);

            }
            catch (Exception ex)
            {
                throw new ApplicationException("ScanHasBeenUploadedTo - Update ScanForm failed", ex);
            }

            //If the scan is a POD email it
            if (scan.FormTypeId == eFormTypeId.POD)
            {
                //Email the POD to the client
                Facade.IPOD facPod = new Facade.POD();

                //Make the call to send the email async as it will
                //have to download the attachment 
                MethodCall<string, int> asyncSendClientPod = facPod.SendClientPod;
                asyncSendClientPod.BeginInvoke(scan.ScannedFormId, r =>
                {
                    try { asyncSendClientPod.EndInvoke(r); }
                    catch (Exception ex) { WebUI.Global.UnhandledException(ex); }
                }, null);
            }

            return methodStatus;
        }

        //---------------------------------------------------------------------------------------

        /// <summary>
        /// Determines whether there is a ScannedForm row. This will become oselete and replaced with GetScannedForm
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [WebMethod]
        public bool DoesScannedFormRowExist(string fileName)
        {
            bool doesScanFormRowExist = false;

            Facade.Form facScanForm = new Orchestrator.Facade.Form();
            Entities.Scan scan = facScanForm.GetForScannedFormForScannedFormPDF(fileName);

            if (scan.ScannedFormId > 0)
                doesScanFormRowExist = true;

            return doesScanFormRowExist;
        }

        //---------------------------------------------------------------------------------------

        /// <summary>
        /// Returns a ScannedForm row for the FileName
        /// </summary>
        /// <param name="FileName">The filname to use for looking up</param>
        /// <returns>A Scan object</returns>
        [WebMethod]
        public Entities.Scan GetScannedForm(string FileName, out string PreviousScanURL)
        {
            Facade.Form facScanForm = new Orchestrator.Facade.Form();
            Entities.Scan scan = facScanForm.GetForScannedFormForScannedFormPDF(FileName);

            PreviousScanURL = string.Empty;
            if (scan != null && scan.PreviousScannedFormId.HasValue)
            {
                Entities.Scan previousScan = facScanForm.GetForScannedFormId(scan.PreviousScannedFormId.Value);
                if (previousScan != null)
                    PreviousScanURL = previousScan.ScannedFormPDF;
            }

            if (scan == null || scan.ScannedFormId < 1)
                return null;
            else
                return scan;
        }

        //---------------------------------------------------------------------------------------

        /// <summary>
        /// Gets the orchestrator settings
        /// </summary>
        /// <returns>A List of Setting objects</returns>
        [WebMethod]
        public List<EF.Setting> GetSettings()
        {
            var settings = EF.DataContext.Current.Settings.ToList();
            return settings;
        }

    }



}
