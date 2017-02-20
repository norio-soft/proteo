using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using DataDynamics.ActiveReports.Export.Pdf;
using DataDynamics.ActiveReports.Export.Xls;
using Orchestrator.Globals;
using Orchestrator.Logging;
using Orchestrator.Reports;

namespace Orchestrator.WebUI.Reports
{

    public class ActiveReportExporter
    {

        private const string C_MODULE = "Orchestrator.WebUI.Reports.ReportRunner.";

        private ReportBase GetCurrentActiveReport(out eReportType reportType)
        {
            var session = HttpContext.Current.Session;

            ReportBase activeReport = null;
            bool enforceDataExistence;
            reportType = (eReportType)session[Orchestrator.Globals.Constants.ReportTypeSessionVariable];
            activeReport = Orchestrator.Reports.Utilities.GetActiveReport(reportType, string.Empty, out enforceDataExistence);

            if (activeReport != null)
            {
                DataView view = null;
                DataSet ds = null;

                var dse = session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable];

                if (dse is DataView)
                    activeReport.DataSource = dse;
                else
                    ds = (DataSet)session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable];

                var sortExpression = (string)session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable];

                if (sortExpression.Length > 0 && dse is DataSet)
                {
                    view = new DataView(ds.Tables[0]);
                    view.Sort = sortExpression;
                    activeReport.DataSource = view;
                }
                else if (dse is DataSet)
                    activeReport.DataSource = ds;

                activeReport.DataMember = (string)session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable];
                activeReport.Document.Printer.PrinterName = string.Empty;
                activeReport.Document.Printer.PaperKind = System.Drawing.Printing.PaperKind.A4;
                activeReport.Run(false);
            }

            return activeReport;
        }

        public string FaxReport(string faxNumber)
        {
            string message = string.Empty;
            bool success = false;
            eReportType reportType;
            ReportBase activeReport = this.GetCurrentActiveReport(out reportType);

            if (activeReport != null)
            {
                // Use the override fax number if provided
                if (Configuration.InterFaxOverrideFaxNumber != String.Empty)
                    faxNumber = Configuration.InterFaxOverrideFaxNumber;

                // Strip the faxNumber into a pure telephone number (i.e. no non-numeric characters)
                Regex stripper = new Regex("[^0-9]*");
                faxNumber = stripper.Replace(faxNumber, String.Empty);

                // Create an international number for this UK number
                if (faxNumber.IndexOf("0") == 0)
                {
                    faxNumber = faxNumber.Substring(1);
                    faxNumber = "+44" + faxNumber;
                }

                // Create a memory stream to put the exported PDF into.
                PdfExport pdfExporter = new PdfExport();
                MemoryStream outputStream = new MemoryStream();

                // Use the pdf exporter to load the memory stream with the resulting PDF document
                pdfExporter.Export(activeReport.Document, outputStream);

                // Move the position back to the beginning of the stream.
                outputStream.Seek(0, SeekOrigin.Begin);

                // Create a byte array buffer to read the memory stream into.
                byte[] bytes = new byte[outputStream.Length];
                outputStream.Read(bytes, 0, (int)outputStream.Length);

                // Make the fax call (via WS)
                InterFax.InterFax myInterFax = new InterFax.InterFax();

                // Record the audit information
                long transactionId = myInterFax.Sendfax(Configuration.InterFaxUserName, Configuration.InterFaxPassword, faxNumber, bytes, "PDF");

                if (transactionId > 0)
                {
                    // The fax has been sent - record the event
                    Facade.IAudit facAudit = new Facade.Audit();
                    string userId = ((Entities.CustomPrincipal)HttpContext.Current.User).UserName;

                    // If faxing Client Delivery & Returns Log, LogId Session variable will be present.
                    if (HttpContext.Current.Session["LogId"] == null)
                        success = facAudit.FaxSent(reportType, transactionId, faxNumber, userId);
                    else
                        success = facAudit.LogFaxSent(transactionId, (int)HttpContext.Current.Session["LogId"], faxNumber, userId);

                    message = "Your report has been faxed.";
                }
                else
                {
                    message = "Your report has not been faxed. <!--(" + transactionId.ToString() + ")-->";

                    // An error occurred
                    if (Configuration.EventLogEnabled && Configuration.EventLogTraceLevel > 0)
                        ApplicationLog.WriteInfo("Orchestrator.ReportRunner.FaxReport", "Fax to " + faxNumber.ToString() + " Failed - Error reported as " + transactionId.ToString());
                }
            }

            return message;
        }

        public bool EmailReport(string emailToAddress)
        {
            bool success = false;
            eReportType reportType;
            ReportBase activeReport = GetCurrentActiveReport(out reportType);

            if (activeReport != null)
            {
                // Create a memory stream to put the exported PDF into.
                PdfExport pdfExporter = new PdfExport();
                MemoryStream outputStream = new MemoryStream();

                // Use the pdf exporter to load the memory stream with the resulting PDF document
                pdfExporter.Export(activeReport.Document, outputStream);

                // Move the position back to the beginning of the stream.
                outputStream.Seek(0, SeekOrigin.Begin);

                // Create a byte array buffer to read the memory stream into.
                byte[] bytes = new byte[outputStream.Length];
                outputStream.Read(bytes, 0, (int)outputStream.Length);
                outputStream.Seek(0, SeekOrigin.Begin);

                var scannedFormID = this.SaveReportPdf(reportType, bytes, HttpContext.Current.User.Identity.Name);

                var reportUrl = string.Format(
                    "http://{0}/reports/emailedreport.ashx?esfid={1}",
                    HttpContext.Current.Request.Url.Authority,
                    Orchestrator.SystemFramework.EncryptionProvider.EncryptText(scannedFormID.ToString()));

                MailMessage mailMessage = new MailMessage();

                mailMessage.From = new MailAddress(Orchestrator.Globals.Configuration.MailFromAddress,
                    Orchestrator.Globals.Configuration.MailFromName);

                string[] emailAddresses = emailToAddress.Split(';');
                foreach (string emailAddress in emailAddresses)
                    mailMessage.To.Add(emailAddress);
                mailMessage.Subject = "Your Proteo Enterprise Report";

                XmlDocument mydoc = new XmlDocument();
                mydoc.AppendChild(mydoc.CreateElement("root"));

                mailMessage.IsBodyHtml = true;
                mailMessage.Body = GenerateBodyText(mydoc, reportType, reportUrl);

                try
                {
                    SmtpClient smtp = new System.Net.Mail.SmtpClient(Globals.Configuration.MailServer, Globals.Configuration.MailPort);
                    smtp.Credentials = new NetworkCredential(Globals.Configuration.MailUsername, Globals.Configuration.MailPassword);
                    
                    SetMessageBody(mailMessage, mailMessage.Body, "Plain/HTML");

                    smtp.Send(mailMessage);
                    mailMessage.Dispose();

                    // The email has been sent - record the event
                    Facade.IAudit facAudit = new Facade.Audit();
                    string userId = ((Entities.CustomPrincipal)HttpContext.Current.User).UserName;

                    // If emailing Client Delivery & Returns Log, LogId Session variable will be present.
                    if (HttpContext.Current.Session["LogId"] == null)
                        success = facAudit.EmailSent(reportType, scannedFormID, emailToAddress, userId);
                    else
                        success = facAudit.LogEmailSent((int)HttpContext.Current.Session["LogId"], scannedFormID, emailToAddress, userId);
                }
                catch (Exception eX)
                {
                    ApplicationLog.WriteError("SendEmail", eX.Message);
                }
            }

            return success;
        }

        private static void SetMessageBody(MailMessage mailMessage, string body, string bodyType)
        {
            if (bodyType.ToLower().Contains("html"))
            {
                mailMessage.IsBodyHtml = true;
                body = body.Replace(";", "<br/>");

                // Check the template for embedded image placeholders 
                var htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                var regex = "[\\\",']cid:(.*?)?[\\\",']";

                var regEmbeddedImagePlaceholder = new Regex(regex, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                var matches = regEmbeddedImagePlaceholder.Matches(body);

                foreach (Match match in matches)
                {
                    var path = Orchestrator.Globals.Configuration.AppRoot + @"images\" + match.Groups[1].Value;
                    try
                    {
                        var logo = new LinkedResource(path);
                        logo.ContentId = match.Groups[1].Value;
                        //add the LinkedResource to the appropriate view
                        htmlView.LinkedResources.Add(logo);
                    }
                    catch (System.IO.FileNotFoundException exfnf)
                    { }
                    catch (System.IO.DirectoryNotFoundException exdnf)
                    { }
                }

                //add the views
                mailMessage.AlternateViews.Add(htmlView);
                mailMessage.Body = body;
            }
            else
            {
                mailMessage.IsBodyHtml = false;
                mailMessage.Body = body.Replace(";", Environment.NewLine);
            }
        }

        private int SaveReportPdf(eReportType reportType, byte[] bytes, string userName)
        {
            var uploaderPath = Globals.Configuration.OrchestratorServerUploaderPdfsPath;
            if (string.IsNullOrEmpty(uploaderPath))
                throw new ApplicationException("Cannot email report: the OrchestratorServerUploaderPdfsPath setting has not been configured.");

            if (!Directory.Exists(uploaderPath))
                Directory.CreateDirectory(uploaderPath);

            var sendNowPath = Path.Combine(uploaderPath, "sendnow");
            if (!Directory.Exists(sendNowPath))
                Directory.CreateDirectory(sendNowPath);

            var fileName = string.Format("EmailedReport_{0}_{1}_{2:yyyyMMddHHmmss}.pdf", Enum.GetName(typeof(eReportType), reportType), userName, DateTime.Now);
            var fileNameAndPath = Globals.Configuration.ScanFormSendNow ? Path.Combine(sendNowPath, fileName) : Path.Combine(uploaderPath, fileName);

            File.WriteAllBytes(fileNameAndPath, bytes);

            var facForm = new Facade.Form();

            var bookingForm = new Entities.Scan
            {
                ScannedDateTime = DateTime.Today,
                FormTypeId = eFormTypeId.EmailedReport,
                ScannedFormPDF = fileName,
                IsAppend = false,
                IsUploaded = false,
            };

            var scannedFormID = facForm.Create(bookingForm, userName);
            return scannedFormID;
        }

        private string GenerateBodyText(XmlDocument doc, eReportType reportType, string reportUrl)
        {
            string companyName = System.Configuration.ConfigurationManager.AppSettings["CompanyName"];
            string addressLine1 = System.Configuration.ConfigurationManager.AppSettings["Address1.Line1"];
            string addressLine2 = System.Configuration.ConfigurationManager.AppSettings["Address1.Line2"];
            string addressLine3 = System.Configuration.ConfigurationManager.AppSettings["Address1.Line3"];
            string companyLogoImageName = System.Configuration.ConfigurationManager.AppSettings["CompanyLogoImageName"];
            
            XslCompiledTransform transformer = new XslCompiledTransform();
            transformer.Load(HttpContext.Current.Server.MapPath(@"~/xsl/reportType.xsl"));
            XmlUrlResolver resolver = new XmlUrlResolver();
            XPathNavigator navigator = doc.CreateNavigator();

            XsltArgumentList args = new XsltArgumentList();
            args.AddParam("WebServer", "", Orchestrator.Globals.Configuration.WebServer);
            args.AddParam("ReportType", "", Utilities.UnCamelCase(Enum.GetName(typeof(eReportType), reportType)));
            args.AddParam("ReportUrl", "", reportUrl);
            args.AddParam("CompanyName", "", companyName);
            args.AddParam("AddressLine1", "", addressLine1);
            args.AddParam("AddressLine2", "", addressLine2);
            args.AddParam("AddressLine3", "", addressLine3);
            args.AddParam("CompanyLogoImageName", "", "cid:" + companyLogoImageName);

            StringWriter sw = new StringWriter();
            transformer.Transform(navigator, args, sw);

            string content = sw.GetStringBuilder().ToString();
            return content;
        }

        public void ExecuteReportExcel()
        {
            eReportType reportType;
            ReportBase activeReport = GetCurrentActiveReport(out reportType);

            if (activeReport != null)
            {
                string reportFilename = Enum.GetName(typeof(eReportType), reportType) + ".xls";

                // Create a memory stream to put the expored XLS into.
                XlsExport xlsExporter = new XlsExport();
                MemoryStream outputStream = new MemoryStream();

                // Use the XLS exporter to load the memory stream with the resulting XLS document.
                xlsExporter.Export(activeReport.Document, outputStream);

                // Move the position back to the beginning of the stream.
                outputStream.Seek(0, SeekOrigin.Begin);

                // Create a byte array buffer to read the memory stream into.
                byte[] bytes = new byte[outputStream.Length];
                // Fill the byte array buffer with the bytes from the memory stream.
                outputStream.Read(bytes, 0, (int)outputStream.Length);

                // Clear anything that might have been written by the aspx page.
                HttpContext.Current.Response.ClearContent();
                HttpContext.Current.Response.ClearHeaders();

                //Add the appropriate headers
                HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + reportFilename);
                //Add the right content type
                HttpContext.Current.Response.ContentType = "application/msexcel";

                // Write this report document byte array to the requestor:
                HttpContext.Current.Response.BinaryWrite(bytes);
                // End the response
                HttpContext.Current.Response.End();
            }
        }

    }



}
