using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Orchestrator.WebUI.Invoicing
{
    public partial class UninvoicedValueForDateRange : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnExportToCSV.Click += new EventHandler(btnExportToCSV_Click);
            btnReport.Click += new EventHandler(btnReport_Click);
        }

        private DataSet GetData()
        {
            DateTime start = dteStartDate.SelectedDate.Value;
            start = start.Subtract(start.TimeOfDay);

            DateTime end = dteEndDate.SelectedDate.Value;
            end = end.Subtract(end.TimeOfDay);
            end = end.Add(new TimeSpan(23, 59, 59));

            Facade.IReferenceData facReferenceData = new Facade.ReferenceData();
            return facReferenceData.GetUnvoicedValueForDateRange(start, end);
        }

        #region Button Event Handlers

        void btnExportToCSV_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                Session["__ExportDS"] = GetData().Tables[0];
                Server.Transfer("../Reports/csvexport.aspx?filename=DailyLogExport.csv");
            }
        }

        void btnReport_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                gvResults.DataSource = GetData();
                gvResults.DataBind();
            }
        }

        #endregion
    }
}