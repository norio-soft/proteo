using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Orchestrator.WebUI.Reports
{

    public class EmailedReport : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                var encryptedScannedFormID = context.Request.QueryString["esfid"];
                if (string.IsNullOrEmpty(encryptedScannedFormID))
                    throw new ApplicationException("An encrypted ScannedFormID must be passed in the query string.");

                EF.ScannedForm scannedForm = null;

                try
                {
                    var decryptedScannedFormID = Orchestrator.SystemFramework.EncryptionProvider.DecryptText(encryptedScannedFormID);
                    var scannedFormID = int.Parse(decryptedScannedFormID);
                    scannedForm = EF.DataContext.Current.ScannedForms.First(sf => sf.ScannedFormID == scannedFormID);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("The value supplied in the querystring is not a valid encrypted ScannedFormID.", ex);
                }

                var bytes = LoadScannedFormPdf(scannedForm);

                this.AuditEmailView(scannedForm.ScannedFormID);

                context.Response.ContentType = "application/pdf";
                context.Response.AddHeader("content-disposition", "inline; filename=report.pdf");
                context.Response.BinaryWrite(bytes);
                context.Response.Flush();
            }
            catch (Exception ex)
            {
                try
                {
                    Utilities.LastError = ex;
                    WebUI.Global.UnhandledException(ex);
                }
                finally
                {
                    context.Response.Redirect("~/reports/emailedreportunavailable.aspx");
                }
            }
        }

        private static byte[] LoadScannedFormPdf(EF.ScannedForm scannedForm)
        {
            byte[] retVal;

            if (scannedForm.IsUploaded ?? false)
            {
                // Load the pdf from the uploaded url into a byte array
                byte[] buffer = new byte[4096];

                try
                {
                    var webRequest = System.Net.WebRequest.Create(scannedForm.ScannedFormPDF);

                    using (var response = webRequest.GetResponse())
                    using (var responseStream = response.GetResponseStream())
                    using (var memoryStream = new MemoryStream())
                    {
                        int count = 0;
                        do
                        {
                            count = responseStream.Read(buffer, 0, buffer.Length);
                            memoryStream.Write(buffer, 0, count);

                        } while (count > 0);

                        retVal = memoryStream.ToArray();
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException("The report PDF has been uploaded but cannot be retrieved from the stored url.", ex);
                }
            }
            else
            {
                // The pdf has not yet been uploaded and should therefore be in the Orchestrator server's uploader PDFs folder, so Load from there into a byte array
                var uploaderPath = Globals.Configuration.OrchestratorServerUploaderPdfsPath;
                if (string.IsNullOrEmpty(uploaderPath))
                    throw new ApplicationException("Cannot load report: the OrchestratorServerUploaderPdfsPath setting has not been configured.");

                var reportFilePath = Path.Combine(uploaderPath, scannedForm.ScannedFormPDF);

                if (!File.Exists(reportFilePath))
                {
                    // If it's not in the root folder it may be in the sendnow folder, so check there.
                    var sendNowPath = Path.Combine(uploaderPath, "sendnow");
                    reportFilePath = Path.Combine(sendNowPath, scannedForm.ScannedFormPDF);
                }

                if (!File.Exists(reportFilePath))
                    throw new ApplicationException("The report PDF has not yet been uploaded but cannot be found in the uploader PDFs folder or the Send Now folder on the Orchestrator server.");

                retVal = File.ReadAllBytes(reportFilePath);
            }

            return retVal;
        }

        private void AuditEmailView(int scannedFormID)
        {
            Facade.IAudit facAudit = new Facade.Audit();
            facAudit.AuditEmailedReportView(scannedFormID);
        }

        public bool IsReusable
        {
            get { return false; }
        }

    }

}