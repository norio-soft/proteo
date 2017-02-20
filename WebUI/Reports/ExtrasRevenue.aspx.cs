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

using System.Collections.Specialized;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Reports
{
    public partial class ExtrasRevenue : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["rcbID"])) return;

            if (!IsPostBack)
                ConfigureDisplay();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.cboClient.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
            this.btnDisplayReport.Click += new EventHandler(btnDisplayReport_Click);
        }

        private void ConfigureDisplay()
        {
            rdiStartDate.SelectedDate = DateTime.Today;
            rdiEndDate.SelectedDate = DateTime.Today.AddMonths(1);
        }

        void cboClient_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            cboClient.Items.Clear();

            Facade.IReferenceData facReferenceData = new Facade.ReferenceData();
            DataSet ds;
            DataTable dt;
            RadComboBoxItem rcItem;
            int itemsPerRequest = 20;
            int itemOffset;
            int endOffset;

            ds = facReferenceData.GetAllClientsFiltered(e.Text);

            itemOffset = e.NumberOfItems;
            endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            dt = ds.Tables[0];
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new RadComboBoxItem();
                rcItem.Text = dt.Rows[i]["OrganisationName"].ToString();
                rcItem.Value = dt.Rows[i]["IdentityID"].ToString();
                cboClient.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset, dt.Rows.Count);
            }
        }

        void btnDisplayReport_Click(object sender, EventArgs e)
        {
            int clientID = 0;
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            DataSet ds = null;

            Facade.IReferenceData facReferenceData = new Facade.ReferenceData();

            int.TryParse(cboClient.SelectedValue, out clientID);
            startDate = rdiStartDate.SelectedDate.Value;
            endDate = rdiEndDate.SelectedDate.Value;

            ds = facReferenceData.GetExtrasRevenueReport(clientID, startDate, endDate);

            if (ds.Tables[1].Rows.Count == 0)
            {
                if(clientID > 0)
                    lblError.Text = "No extras for client " + cboClient.Text + " for period " + startDate.ToString("dd/MM/yy") + " to " + endDate.ToString("dd/MM/yy");
                else
                    lblError.Text = "No extras for period " + startDate.ToString("dd/MM/yy") + " to " + endDate.ToString("dd/MM/yy");

                lblError.Visible = true;
                reportViewer.Visible = false;
            }
            else
            {
                lblError.Visible = false;

                NameValueCollection reportParams = new NameValueCollection();

                reportParams.Add("IdentityID", clientID.ToString());
                reportParams.Add("StartDate", startDate.ToString("dd/MM/yy"));
                reportParams.Add("EndDate", endDate.ToString("dd/MM/yy"));

                // Configure the Session variables used to pass data to the report
                Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.ExtrasRevenue;
                Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = ds;
                Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
                Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table1";
                Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;

                if (cboClient.SelectedValue != "")
                    reportViewer.IdentityId = int.Parse(cboClient.SelectedValue);
                // Show the user control
                reportViewer.Visible = true;
            }

        }
    }
}
