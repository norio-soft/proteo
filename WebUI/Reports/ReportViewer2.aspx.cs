using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Reporting;

namespace Orchestrator.WebUI.Reports
{
    public partial class ReportViewer2 : System.Web.UI.Page
    {
        const string REPORTS_ASSEMBLY = "Orchestrator.Reports.{0}, Orchestrator.Reports";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                
                AssignParameters();
                //RadToolBar1.Width = ReportViewer.Width;
            }
        }

        private void AssignParameters()
        {
            string reportName = Server.UrlDecode(this.Request.QueryString["rn"]);

            if (!string.IsNullOrEmpty(reportName))
            {
                string typeName = string.Format(REPORTS_ASSEMBLY, reportName);
                Type reportType = Type.GetType(typeName);

                if (reportType == null)
                    throw new ApplicationException(string.Format("Report {0} could not be found.", reportName));

                IReportDocument report = (IReportDocument)Activator.CreateInstance(reportType);

                foreach (var param in report.ReportParameters)
                {
                    string qsValue = this.Request.QueryString[param.Name];

                    try
                    {
                        if (!string.IsNullOrEmpty(qsValue))
                        {
                            switch (param.Type)
                            {
                                case ReportParameterType.Boolean:
                                    param.Value = bool.Parse(qsValue);
                                    break;

                                case ReportParameterType.DateTime:
                                    param.Value = DateTime.Parse(qsValue);
                                    break;
                                case ReportParameterType.Float:
                                    param.Value = float.Parse(qsValue);
                                    break;

                                case ReportParameterType.Integer:
                                    param.Value = long.Parse(qsValue);
                                    break;

                                case ReportParameterType.String:
                                    param.Value = qsValue;
                                    break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new ApplicationException(string.Format("Couldn't parse value {0} for parameter {1}.", qsValue, param.Name));
                    }
                }

                var reportSource = new InstanceReportSource { ReportDocument = report };
                this.ReportViewer.ReportSource = reportSource;
                this.Page.Title = "Report Viewer - " + reportType.Name;
            }
        }

        protected void RadToolBar1_ButtonClick(object sender, Telerik.Web.UI.RadToolBarEventArgs e)
        {
            switch (e.Item.Value)
            {
                case "SendEmailAttachment":
                    SendEmailAttachment();
                    break;

                case "ExportData":
                    ExportData();
                    break;

                default:
                    break;
            }
        }

        private void SendEmailAttachment()
        {
            //System.Web.UI.WebControls.TextBox txt = (System.Web.UI.WebControls.TextBox)RadToolBar1.Items[12].FindControl("EmailTextBox");
            //MailReport((Telerik.Reporting.Report)ReportViewer.Report, "myemail@mydomain.com", txt.Text, "Finance report", "Report is attached");
        }

        private void ExportData()
        {
            ReportExport reportExport = new ReportExport();

            string reportName = Request.QueryString["rn"];


            Session["__ExportDS"] = reportExport.RetieveExportDataForReport((Telerik.Reporting.InstanceReportSource)ReportViewer.ReportSource, reportName);

            if (Session["__ExportDS"] != null)
                Server.Transfer("../Reports/csvexport.aspx?filename=" + reportName + ".csv");
        }

        protected void DropDownList3_SelectedIndexChanged(object sender, EventArgs e)
        {
            RadToolBar1.Skin = (sender as DropDownList).SelectedValue;
        }
    }
}