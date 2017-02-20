using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using System.Data;

namespace Orchestrator.WebUI.manifest
{

    public partial class ManifestReportForClient : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            cboClient.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);

            btnReset.Click += new EventHandler(btnReset_Click);
            btnReport.Click += new EventHandler(btnReport_Click);
        }

        #region Event Handlers

        void cboClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboClient.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllOrganisationsFiltered(e.Text, eOrganisationType.Client, false);

            int itemsPerRequest = 20;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            DataTable dt = ds.Tables[0];
            Telerik.Web.UI.RadComboBoxItem rcItem = null;
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new Telerik.Web.UI.RadComboBoxItem();
                rcItem.Text = dt.Rows[i]["OrganisationName"].ToString();
                rcItem.Value = dt.Rows[i]["IdentityId"].ToString();
                cboClient.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

        void btnReport_Click(object sender, EventArgs e)
        {
            btnReport.DisableServerSideValidation();

            if (Page.IsValid)
            {
                var excludeFirstLine = false;
                var usePlannedTimes = true;
                var showFullAddress = false;
                var useInstructionOrder = false;

                int customerIdentityID;
                int.TryParse(cboClient.SelectedValue, out customerIdentityID);
                var startDate = dteStartDate.SelectedDate.Value.Date;
                var endDate = dteEndDate.SelectedDate.Value.Date;

                var ds = new DataSet();
                ds.Tables.Add(ManifestGeneration.GetClientManifestReport(customerIdentityID, startDate, endDate, usePlannedTimes, excludeFirstLine, showFullAddress, useInstructionOrder));

                if (ds.Tables[0].Rows.Count > 0)
                {
                    // Configure the report settings collection
                    var reportParams = new NameValueCollection
                    {
                        { "ManifestName", "" },
                        { "ManifestID", "0" },
                        { "UsePlannedTimes", usePlannedTimes.ToString() },
                    };

                    // Configure the Session variables used to pass data to the report
                    Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.RunSheet;
                    Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = ds;
                    Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = string.Empty;
                    Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";
                    Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;

                    // Show the user control
                    reportViewer.Visible = true;
                }
                else
                {
                    lblError.Text = "&nbsp;No instructions found from " + startDate.ToString("dd/MM/yy") + " to " + endDate.ToString("dd/MM/yy") + ".";
                    lblError.Visible = true;
                }
            }
            else
                reportViewer.Visible = false;
        }

        void btnReset_Click(object sender, EventArgs e)
        {
            cboClient.Text = string.Empty;
            cboClient.SelectedValue = string.Empty;
            dteStartDate.Text = string.Empty;
            dteEndDate.Text = string.Empty;

            reportViewer.Visible = false;
        }

        #endregion

    }

}