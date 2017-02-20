using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.IO;

using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;

namespace Orchestrator.WebUI.scan.wizard
{
    public partial class uploadDocument : Orchestrator.Base.BasePage
    {
        //--------------------------------------------------------------------------

        protected int ScannedFormTypeId
        {
            get
            {
                int formTypeId = -1;
                if (Request.QueryString["ScannedFormTypeId"] != null)
                    formTypeId = int.Parse(Request.QueryString["ScannedFormTypeId"].ToString());

                return formTypeId;
            }
        }

        //--------------------------------------------------------------------------

        protected string FileName
        {
            get
            {
                string uploadedFilePath = String.Empty;
                if (ViewState["FileName"] != null)
                    uploadedFilePath = ViewState["FileName"].ToString();

                return uploadedFilePath;
            }
            set 
            {
                ViewState["FileName"] = value;   
            }
        }

        //--------------------------------------------------------------------------

        protected int ScannedFormId
        {
            get
            {
                int formId = -1;
                if (Request.QueryString["ScannedFormId"] != null)
                    formId = int.Parse(Request.QueryString["ScannedFormId"].ToString());

                return formId;
            }
        }

        //--------------------------------------------------------------------------

        protected int JobId
        {
            get
            {
                int jobId = -1;
                if (Request.QueryString["JobId"] != null)
                    jobId = int.Parse(Request.QueryString["JobId"].ToString());

                return jobId;
            }
        }

        //--------------------------------------------------------------------------

        protected int CollectDropId
        {
            get
            {
                int collectDropId = -1;
                if (Request.QueryString["CollectDropId"] != null)
                    collectDropId = int.Parse(Request.QueryString["CollectDropId"].ToString());

                return collectDropId;
            }
        }

        //--------------------------------------------------------------------------

        protected int ManifestId
        {
            get
            {
                int manifestId = -1;
                if (Request.QueryString["ManifestId"] != null)
                    manifestId = int.Parse(Request.QueryString["ManifestId"].ToString());

                return manifestId;
            }
        }

        //---------------------------------------------------------------------------------------

        protected int PointId
        {
            get
            {
                int pointId = 0;
                if (Request.QueryString["PointId"] != null)
                    pointId = int.Parse(Request.QueryString["PointId"].ToString());

                return pointId;
            }
        }

        //--------------------------------------------------------------------------

        protected int OrderId
        {
            get
            {
                int orderId = -1;
                if (Request.QueryString["OrderId"] != null)
                    orderId = int.Parse(Request.QueryString["OrderId"].ToString());

                return orderId;
            }
        }

        //--------------------------------------------------------------------------

        protected int OrganisationIdentityId
        {
            get
            {
                int orgId = -1;
                if (Request.QueryString["OrgId"] != null)
                    orgId = int.Parse(Request.QueryString["OrgId"].ToString());

                return orgId;
            }
        }

        //--------------------------------------------------------------------------

        protected int OrganisationDocumentId
        {
            get
            {
                int orgDocId = -1;
                if (Request.QueryString["OrgDocId"] != null)
                    orgDocId = int.Parse(Request.QueryString["OrgDocId"].ToString());

                return orgDocId;
            }
        }

        //--------------------------------------------------------------------------

        protected int DehireReceiptId
        {
            get
            {
                int dehireReceiptId = -1;
                if (Request.QueryString["DeHireReceiptId"] != null)
                    dehireReceiptId = int.Parse(Request.QueryString["DeHireReceiptId"].ToString());

                return dehireReceiptId;
            }
        }

        //--------------------------------------------------------------------------

        protected string DehireReceiptNumber
        {
            get
            {
                string dehireReceiptNumber = string.Empty;
                if (Request.QueryString["DHNo"] != null)
                    dehireReceiptNumber = Request.QueryString["DHNo"].ToString();

                return dehireReceiptNumber;
            }
        }

        //--------------------------------------------------------------------------

        protected string AppendOrReplace
        {
            get
            {
                string appendOrReplace = "UR";
                if (Request.QueryString["AppendOrReplace"] != null)
                    appendOrReplace = Request.QueryString["AppendOrReplace"].ToString();

                return appendOrReplace;
            }
        }

        //--------------------------------------------------------------------------

        public string ImageSaveLocation
        {
            get
            {
                string location = Globals.Configuration.ScannedDocumentPath;

                // javascript needs the additional backslahes
                if (!String.IsNullOrEmpty(location))
                    if (!location.EndsWith("\\"))
                        location += "\\";

                return location.Replace(@"\", @"\\");
            }
        }

        //--------------------------------------------------------------------------

        protected string RandomKey
        {
            get
            {
                DateTime date = DateTime.Now;
                return String.Format("{0}{1}{2}{3}{4}{5}", date.Year, date.Month,
                    date.Day, date.Minute, date.Second, date.Millisecond);
            }
        }

        //--------------------------------------------------------------------------

        protected void Page_Load(object sender, EventArgs e)
        {
            ((WizardMasterPage)this.Master).WizardTitle = "Document Wizard";
        }

        //--------------------------------------------------------------------------

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnBack.Click += btnBack_Click;
            this.btnNext.Click += btnNext_Click;
            this.btnUpload.Click += btnUpload_Click;
        }

        //--------------------------------------------------------------------------

        private void btnUpload_Click(object sender, EventArgs e)
        {
            string fileDestination = Server.MapPath("~/PDFUpload/sendnow");
            
            if (!fileDestination.EndsWith("\\"))
                fileDestination += "\\";

            DateTime date = DateTime.Now;
            //this.FileName = date.Year.ToString() + "\\" + date.Month.ToString() + "\\" + date.Day.ToString() + "\\";

            string uploadDirectory = fileDestination;

            if (radUpload.UploadedFiles.Count == 0)
                lblError.Text = "Please select a file to upload.";
            else
            {
                foreach (UploadedFile f in radUpload.UploadedFiles)
                {
                    // Determine the folder structure and ensure that this exists.
                    if (!Directory.Exists(Path.Combine(fileDestination, this.FileName)))
                        Directory.CreateDirectory(Path.Combine(fileDestination, this.FileName));

                    if (!fileDestination.EndsWith("\\"))
                        fileDestination += "\\";

                    // Set final destination and file name 
                    this.FileName += (((Entities.CustomPrincipal)this.Page.User).UserName + "_" + this.RandomKey + f.GetExtension());
                    fileDestination += this.FileName;
                    
           //    //     f.SaveAs(this.ImageSaveLocation + this.FileName, true);
                    f.SaveAs(fileDestination, true);


                    this.lblConfirmation.Visible = true;
                    this.lblConfirmation.Text = this.FileName + " has been uploaded";
                    this.lblError.Text = String.Empty;
                    this.radUpload.Enabled = false;
                    this.btnNext.Enabled = true;
                }

                //This only appears to deal with appends - which are NOT offered for Uploaded files anyway
                this.ScanHasBeenUploaded(this.FileName);

                //The full URL is now required so for file Uploads it has to be set.
                //For scans the FileName is temporarily left alone until the FTP Uploader sets it to the full path
                this.FileName = "http://" + Request.Url.Authority + "/PDFS/" + this.FileName.Replace('\\', '/');
               
            }
        }

        //--------------------------------------------------------------------------
        public string ScanHasBeenUploaded(string fileNameAndFTPDirectory)
        {
            //If the http post file upload is being used then the PODs must be on this server
            //so we can append etc.
            //Not sure why something sikmilar to this is done in scanreceive.aspx
            //TODO However this needs to be tested
            //This is no longer called - is it redundant?

            if (fileNameAndFTPDirectory.StartsWith("\\"))
                fileNameAndFTPDirectory = fileNameAndFTPDirectory.Substring(1, fileNameAndFTPDirectory.Length - 1);

            string methodStatus = String.Empty;
            string newFileName = Path.GetFileName(fileNameAndFTPDirectory);
            string newFileLocation = Path.Combine(Server.MapPath("~/PDFS"), fileNameAndFTPDirectory.Replace(newFileName, ""));
            string newFileLocationAndName = Path.Combine(newFileLocation, newFileName);
            Facade.Form facScanForm = new Orchestrator.Facade.Form();

            if (this.AppendOrReplace.Contains("A"))
            {
                Entities.Scan scan = facScanForm.GetForScannedFormId(this.ScannedFormId);

                if (scan != null)
                {
                    PdfDocument previousDocument;
                    string existingFileNameAndPath =  scan.ScannedFormPDF;

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
                            foreach (PdfPage page in previousDocument.Pages)
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
                            + existingFileNameAndPath);
                }
                else
                    throw new ApplicationException("ExistingScanForm entity could not be found.");
            }
            else
                methodStatus = "Append = false or no previousScanneDformId to append too.";

            return methodStatus;
        }

        //--------------------------------------------------------------------------

        private void btnBack_Click(object sender, EventArgs e)
        {
            // This is a temporary workaround for an issue caused by javascript history api not working in windows opened using showModalDialog.
            // The longer-term solution is to implement using a method other than showModalDialog which is deprecated in html5 anyway, and then
            // the Back button can simply call history.back() in javascript.
            if (Request.QueryString["WizardBackUrl"] != null)
                Response.Redirect(HttpUtility.UrlDecode(Request.QueryString["WizardBackUrl"]));
        }

        //--------------------------------------------------------------------------

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(this.FileName))
            {
                Orchestrator.Facade.ReferenceData facForm = new Orchestrator.Facade.ReferenceData();
                Entities.FormType formType = facForm.GetForFormTypeId(this.ScannedFormTypeId);

                string nextPage = formType.FormTypePage;

                nextPage += "?ScannedFormId=" + this.ScannedFormId.ToString() + "&";

                nextPage += "&ScannedFormTypeId=" + this.ScannedFormTypeId.ToString();
                nextPage += "&JobId=" + this.JobId;
                nextPage += "&OrderId=" + this.OrderId;
                nextPage += "&OrgId=" + this.OrganisationIdentityId;
                nextPage += "&OrgDocId=" + this.OrganisationDocumentId;
                nextPage += "&CollectDropId=" + this.CollectDropId;
                nextPage += "&PointId=" + this.PointId;
                nextPage += "&ManifestId=" + this.ManifestId;
                nextPage += "&DeHireReceiptId=" + this.DehireReceiptId;
                nextPage += "&DHNo=" + this.DehireReceiptNumber;
                nextPage += "&AppendOrReplace=" + this.AppendOrReplace;
                nextPage += "&FileName=" + this.FileName;

                if (this.Request.QueryString["dcb"] != null)
                    nextPage += "&dcb=" + this.Request.QueryString["dcb"].ToString();

                nextPage += "&WizardBackUrl=" + HttpUtility.UrlEncode(Request.Url.PathAndQuery);

                Response.Redirect(nextPage);
            }
            else
                this.lblError.Text = "No upload file path found.";
        }

        //--------------------------------------------------------------------------
    }
}
