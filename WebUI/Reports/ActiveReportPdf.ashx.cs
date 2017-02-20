using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using DataDynamics.ActiveReports.Export.Pdf;
using Orchestrator.Logging;
using Orchestrator.Reports;

namespace Orchestrator.WebUI.Reports
{

    /// <summary>
    /// Handler to generate an Active Report which is exported as a PDF to the Response
    /// </summary>
    /// <remarks>Code migrated from WebUI/Reports/reportrunner.aspx.cs which was the code-behind for a .pdfx handler which no longer works under Integrated Pipeline mode</remarks>
    public class ActiveReportPdf : IHttpHandler, IReadOnlySessionState
    {

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(HttpContext context)
        {
            var response = context.Response;

            response.Buffer = true;
            
            var reportSessionParametersKey = context.Request.QueryString["rpk"];
            var noData = false;
            var rpt = this.GetReport(context.Session, context.Trace, reportSessionParametersKey, out noData);

            if (noData)
            {
                // There is no data to display so redirect to a page.
                response.Clear();
                context.Server.Transfer("blankReport.aspx");
                return;
            }

            if (rpt == null)
            {
                response.Clear();
                response.ClearContent();
                response.Write("<br /><br />The report failed to generate, please try again. If this problem continues please contact support.");

                if (Utilities.LastError != null)
                {
                    response.Write("<br />" + Utilities.LastError.Message);
                    response.Write("<br />" + Utilities.LastError.StackTrace);
                }
                else
                    response.Write("<br />No error stored");

                response.End();
                return;
            }

            response.Clear();
            response.ClearContent();
            response.ClearHeaders();
            response.Cache.SetCacheability(HttpCacheability.NoCache);

            response.ContentType = "application/pdf";

            // IE & Acrobat seem to require "content-disposition" header being in the response.
            // If you don't add it, the doc still works most of the time, but not always.
            // this makes a new window appear: Response.AddHeader("content-disposition","attachment; filename=MyPDF.PDF");
            response.AddHeader("content-disposition", "inline; filename=MyPDF.PDF");

            // Create the PDF export object
            var pdf = new PdfExport();
            // Create a new memory stream that will hold the pdf output
            var memStream = new System.IO.MemoryStream();
            // Export the report to PDF:
            pdf.Export(rpt.Document, memStream);
            // Write the PDF stream out
            response.BinaryWrite(memStream.ToArray());
            // Send all buffered content to the client
            response.End();
            context.Response.ContentType = "text/plain";
            context.Response.Write("Hello World");
        }

        private ReportBase GetReport(HttpSessionState session, TraceContext trace, string reportParametersSessionKey, out bool noData)
        {
            ReportBase activeReport = null;
            noData = false;

            eReportType reportType = 0;
            bool enforceDataExistence = false;
            bool useReportParametersKey = !string.IsNullOrWhiteSpace(reportParametersSessionKey);
            Hashtable htReportInformation = null;

            try
            {
                if (useReportParametersKey)
                {
                    htReportInformation = (Hashtable)session[reportParametersSessionKey];
                    reportType = (eReportType)htReportInformation[Orchestrator.Globals.Constants.ReportTypeSessionVariable];
                }
                else
                {
                    reportType = (eReportType)session[Orchestrator.Globals.Constants.ReportTypeSessionVariable];
                }

                activeReport = Orchestrator.Reports.Utilities.GetActiveReport(reportType, reportParametersSessionKey, out enforceDataExistence);
            }
            catch (Exception ex)
            {
                Utilities.LastError = ex;
                return null;
            }

            if (activeReport != null)
            {
                try
                {
                    ApplicationLog.WriteTrace("Getting Data From Session Variable");
                    DataView view = null;
                    var sortExpression = string.Empty;
                    object ds = null;

                    if (useReportParametersKey)
                    {
                        if (htReportInformation[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] is DataSet)
                            ds = (DataSet)htReportInformation[Orchestrator.Globals.Constants.ReportDataSessionTableVariable];
                        else
                            ds = htReportInformation[Orchestrator.Globals.Constants.ReportDataSessionTableVariable];

                        sortExpression = (string)htReportInformation[Orchestrator.Globals.Constants.ReportDataSessionSortVariable];
                    }
                    else
                    {
                        if (session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] is DataSet)
                            ds = (DataSet)session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable];
                        else
                            ds = session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable];

                        sortExpression = (string)session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable];
                    }

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

                        if (enforceDataExistence && ds != null && ds is DataSet && ((DataSet)ds).Tables.Count > 0 && ((DataSet)ds).Tables[0].Rows.Count == 0)
                        {
                            noData = true;
                            return null;
                        }

                        if (useReportParametersKey)
                            activeReport.DataMember = (string)htReportInformation[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable];
                        else
                            activeReport.DataMember = (string)session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable];
                    }
                    catch (Exception eX)
                    {
                        Utilities.LastError = eX;
                        trace.Write("Err1:" + eX.Message);
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

            trace.Write("Returning Report");
            ApplicationLog.WriteTrace("GetReport :: Returning Report");
            return activeReport;
        }

    }

}
