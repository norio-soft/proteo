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
using System.Collections.Generic;
using Orchestrator.Entities;

namespace Orchestrator.WebUI.Job
{
    public partial class SubContractedHistory : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            cboSubContractor.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboSubContractor_ItemsRequested);

            btnReset.Click += new EventHandler(btnReset_Click);
            btnReport.Click += new EventHandler(btnReport_Click);
            btnExportToCSV.Click += new EventHandler(btnExportToCSV_Click);
        }

        #region Event Handlers

        void cboSubContractor_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboSubContractor.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllOrganisationsFiltered(e.Text, eOrganisationType.SubContractor, false);

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
                cboSubContractor.Items.Add(rcItem);
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
                int identityID = 0;
                int.TryParse(cboSubContractor.SelectedValue, out identityID);
                DateTime startDate = dteStartDate.SelectedDate.Value;
                startDate = startDate.Subtract(startDate.TimeOfDay);
                DateTime endDate = dteEndDate.SelectedDate.Value;
                endDate = endDate.Subtract(endDate.TimeOfDay);
                endDate = endDate.Add(new TimeSpan(23, 59, 59));

                Facade.IJobSubContractor facJobSubcontractor = new Facade.Job();
                DataSet dsSubContractedHistory;

                    dsSubContractedHistory = facJobSubcontractor.GetSubContractedHistory(identityID, startDate, endDate);
               
                if (dsSubContractedHistory.Tables[0].Rows.Count > 0)
                {
                    // Configure the report settings collection
                    NameValueCollection reportParams = new NameValueCollection();
                    reportParams.Add("Start Date", startDate.ToString("dd/MM/yy"));
                    reportParams.Add("End Date", endDate.ToString("dd/MM/yy"));

                    // Configure the Session variables used to pass data to the report
                    Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.SubContractedHistory;
                    Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsSubContractedHistory;
                    Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
                    Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";
                    Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;

                    // Show the user control
                    reportViewer.Visible = true;
                }
                else
                {
                    lblError.Text = "&nbsp;No runs found from " + startDate.ToString("dd/MM/yy") + " to " + endDate.ToString("dd/MM/yy") + ".";
                    lblError.Visible = true;
                }
            }
            else
                reportViewer.Visible = false;
        }

        void btnReset_Click(object sender, EventArgs e)
        {
            cboSubContractor.Text = string.Empty;
            cboSubContractor.SelectedValue = string.Empty;
            dteStartDate.Clear();
            dteEndDate.Clear();

            reportViewer.Visible = false;
        }


        void btnExportToCSV_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                int identityID = 0;
                int.TryParse(cboSubContractor.SelectedValue, out identityID);
                DateTime startDate = dteStartDate.SelectedDate.Value;
                startDate = startDate.Subtract(startDate.TimeOfDay);
                DateTime endDate = dteEndDate.SelectedDate.Value;
                endDate = endDate.Subtract(endDate.TimeOfDay);
                endDate = endDate.Add(new TimeSpan(23, 59, 59));

                Facade.IJobSubContractor facJobSubcontractor = new Facade.Job();
                DataSet dsSubContractedHistory;

                dsSubContractedHistory = facJobSubcontractor.GetSubContractedHistoryCSV(identityID, startDate, endDate);
              
                Session["__ExportDS"] = dsSubContractedHistory.Tables[0];

                Server.Transfer("../Reports/csvexport.aspx?filename=SubContractedHistoryExport.csv");
                
            }

            
        }
        #endregion
    }
}
