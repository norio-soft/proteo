using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

using PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using PdfSharp.Drawing;
using System.Collections.Specialized;
using System.Data;
using DataDynamics.ActiveReports.Export.Pdf;
using Orchestrator.Reports;
using Orchestrator.Logging;

namespace Orchestrator.WebUI.manifest
{
    public partial class printmultipleManifests : System.Web.UI.Page
    {

     

        private List<int> manifestIDs = null;
        public List<int> ManifestIDs
        {
            get
            {
                if (manifestIDs == null)
                {
                    // Get from the querystring and parse into a list;
                    string input = Request.QueryString["i"];
                    List<string> ids = input.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries).ToList();
                    manifestIDs = ids.ConvertAll<int>((s) => int.Parse(s));
                }
                return manifestIDs;
            }
        }

        public string GetURLForPrint
        {
            get
            {
                    return Request.Url.ToString() + "&generate";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString.ToString().Contains("generate"))
            {
                CombinePDFs();
            }
            
        }

        public MemoryStream ExecuteReport1()
        {
            //Response.Buffer = true;
            Orchestrator.Reports.ReportBase rpt = GetManifestReport();
            if (rpt == null)
            {
                Response.Clear();
                Response.ClearContent();
                Response.Write("<br /><br />The report failed to generate, please try again. If this problem continues please contact support.");
                if (Utilities.LastError != null)
                {
                    Response.Write("<br />" + Utilities.LastError.Message);
                    Response.Write("<br />" + Utilities.LastError.StackTrace);
                }
                else
                    Response.Write("<br />No error stored");
                    Response.End();

                return null;
            }


            MemoryStream memStream = new MemoryStream();
            // Create the PDF export object
            using (PdfExport pdf = new PdfExport())
            {                
               // pdf.Export(rpt.Document, Path.Combine(PDFPath, System.Environment.TickCount.ToString().Substring(0, 4) + ".pdf"));
                pdf.Export(rpt.Document, memStream);
            }

            return memStream;
           
        }

        private Orchestrator.Reports.ReportBase GetManifestReport()
        {
            Orchestrator.Reports.ReportBase activeReport = null;
            Orchestrator.Reports.IDriverRunSheet report = Orchestrator.Application.GetSpecificImplementation<IDriverRunSheet>();

            if (report != null)
            {
                activeReport = (Orchestrator.Reports.ReportBase)report;
            }
            else
            {
                activeReport = new Orchestrator.Reports.rptDriverRunSheet();
            }

            activeReport.Document.Printer.PrinterName = string.Empty;
            activeReport.Document.Printer.PaperKind = System.Drawing.Printing.PaperKind.A4;

            if (activeReport != null)
            {
                try
                {

                    ApplicationLog.WriteTrace("Getting Data From Session Variable");
                    DataView view = null;
                    string sortExpression = string.Empty;
                    object ds = null;

                    //if (useReportParametersKey)
                    //{
                    //    if (htReportInformation[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] is DataSet)
                    //        ds = (DataSet)htReportInformation[Orchestrator.Globals.Constants.ReportDataSessionTableVariable];
                    //    else
                    //        ds = htReportInformation[Orchestrator.Globals.Constants.ReportDataSessionTableVariable];

                    //    sortExpression = (string)htReportInformation[Orchestrator.Globals.Constants.ReportDataSessionSortVariable];
                    //}
                    //else
                    //{
                        if (Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] is DataSet)
                            ds = (DataSet)Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable];
                        else
                            ds = Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable];
                        sortExpression = (string)Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable];
                    //}


                    ApplicationLog.WriteTrace("sortExpression ::" + sortExpression);
                    try
                    {
                        if (sortExpression.Length > 0)
                        {
                            if (ds is DataSet)
                                view = new DataView(((DataSet)ds).Tables[0]);
                            view.Sort = sortExpression;
                            activeReport.DataSource = view;
                        }
                        else
                            activeReport.DataSource = ds;

                        //if (enforceDataExistence && ds != null)
                        //{
                        //    if (ds is DataSet)
                        //        if (((DataSet)ds).Tables.Count > 0 && ((DataSet)ds).Tables[0].Rows.Count == 0)
                        //        {
                        //            // There is no data to display so redirect to a page.
                        //            Response.Clear();
                        //            Server.Transfer("blankReport.aspx");
                        //        }
                        //}

                        //if (useReportParametersKey)
                        //{
                        //    activeReport.DataMember = (string)htReportInformation[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable];
                        //}
                        //else
                        //{
                            activeReport.DataMember = (string)Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable];
                        //}
                    }
                    catch (Exception eX)
                    {
                        Utilities.LastError = eX;
                        this.Trace.Write("Err1:" + eX.Message);
                        ApplicationLog.WriteTrace("GetReport :: Err1:" + eX.Message);
                    }
                    activeReport.SetReportCulture();
                    activeReport.Run(false);

                }
                catch (Exception e)
                {
                    Utilities.LastError = e;
                    return null;
                }
            }
            this.Trace.Write("Returning Report");
            ApplicationLog.WriteTrace("GetReport :: Returning Report");
           
            return activeReport;
        }

        private void CombinePDFs()
        {

            Response.Buffer = true;
            Facade.ResourceManifest facMan = new Facade.ResourceManifest();
            PdfDocument combinedPDF = new PdfDocument();
            


            foreach (int resourceManifestID in ManifestIDs)
            {
                // Retrieve the resource manifest 
                NameValueCollection reportParams = new NameValueCollection();

                var manifest = facMan.GetResourceManifest(resourceManifestID);
                DataSet manifests = new DataSet();

                if(manifest.SubcontractorId == null)
                    manifests.Tables.Add(ManifestGeneration.GetDriverManifest(resourceManifestID, manifest.ResourceId, true, false, false, true));
                else
                    manifests.Tables.Add(ManifestGeneration.GetSubbyManifest(resourceManifestID, manifest.ResourceId, true, false, false, true));

                if (manifests.Tables[0].Rows.Count > 0)
                {
                    // Add blank rows if applicable
                    int extraRows = 0;
                    if (extraRows > 0)
                    {
                        for (int i = 0; i < extraRows; i++)
                        {
                            DataRow newRow = manifests.Tables[0].NewRow();
                            manifests.Tables[0].Rows.Add(newRow);
                        }
                    }

                    //-------------------------------------------------------------------------------------	
                    //									Load Report Section 
                    //-------------------------------------------------------------------------------------	
                    reportParams.Add("ManifestName", manifest.Description);
                    reportParams.Add("ManifestID", manifest.ResourceManifestId.ToString());
                    reportParams.Add("UsePlannedTimes", "false");

                    Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.RunSheet;
                    Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;
                    Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = manifests;
                    Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = "";
                    Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";

                   MemoryStream mem = ExecuteReport1();
                   PdfDocument pdf = PdfReader.Open(mem, PdfDocumentOpenMode.Import);
                   foreach (PdfPage page in pdf.Pages)
                       combinedPDF.AddPage(page);

                }
            }

            
            //string[] pdfs = Directory.GetFiles(PDFPath);
            //foreach (string file in pdfs)
            //{
            //    PdfDocument pdf = PdfReader.Open(file, PdfDocumentOpenMode.Import);
  
            //    foreach (PdfPage page in pdf.Pages)
            //        combinedPDF.AddPage(page);
            //}
            

            // Show the user control
            // Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", "<script language=\"javascript\">window.open('" + Page.ResolveClientUrl("../reports/reportviewer.aspx?wiz=true") + "');</script>");
            
            

            // Combine the PDFS into a single document
            //foreach (var invoice in invoices)
            //{
            //    using (FileStream fs = File.OpenRead(Server.MapPath(invoice.PDFLocation)))
            //    {
            //        pdf = PdfReader.Open(fs, PdfDocumentOpenMode.Import);
            //    }
            //}
            
            // Stream this to the IFrame
            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.Buffer = true;
            Response.ContentType = "application/pdf";

            // this makes a new window appear: 
            //Response.AddHeader("content-disposition","attachment; filename=MyPDF.PDF");
            Response.AddHeader("content-disposition", "inline; filename=Manifests.PDF");

            MemoryStream binaryData = new MemoryStream();
            combinedPDF.Save(binaryData);

            Response.BinaryWrite(binaryData.ToArray());

            //Directory.Delete(PDFPath);
        }
    }
}
