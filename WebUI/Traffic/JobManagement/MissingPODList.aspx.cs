using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;


using P1TP.Components.Web.Validation;
using Orchestrator.Globals;

namespace Orchestrator.WebUI.Traffic.JobManagement
{
	/// <summary>
	/// Summary description for MissingPODList.
	/// </summary>
	public partial class MissingPODList : Orchestrator.Base.BasePage
	{
        protected override PageStatePersister PageStatePersister
        {
            get
            {
                return new HiddenFieldPageStatePersister(this);
            }
        }

		#region Form Elements

		#endregion

		#region Page Load/Init/Error

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

            if (!IsPostBack)
            {
                CurrentSortExpression = "JobId";
                CurrentSortDirection = SortDirection.Ascending;
            }
		}

		#endregion

		private void btnReport_Click(object sender, EventArgs e)
		{
			btnReport.DisableServerSideValidation();

			if (Page.IsValid)
			{
				// Retrieve the report data and place it in the session variables
				GetMissingPODReportData();
			}
			else
			{
				// Hide the report
				reportViewer.Visible = false;
                gvMissingPOD.Visible = false;
			}
		}

        private DataSet GetData()
        {
            DateTime startDate = dteStartDate.SelectedDate.Value;
            startDate = startDate.Subtract(startDate.TimeOfDay);
            DateTime endDate = dteEndDate.SelectedDate.Value;
            endDate = endDate.Subtract(endDate.TimeOfDay);
            endDate = endDate.Add(new TimeSpan(23, 59, 59));

            Facade.IPOD facPOD = new Facade.POD();
            return facPOD.GetMissingPODs(Int32.Parse(cboClient.SelectedValue), startDate, endDate);
        }

		private void GetMissingPODReportData()
		{
            DataSet dsMissingPODs = GetData();

			if (dsMissingPODs.Tables[0].Rows.Count > 0)
			{
				// Configure the report settings collection
				NameValueCollection reportParams = new NameValueCollection();
				reportParams.Add("Client", cboClient.Text);
				reportParams.Add("IdentityId", cboClient.SelectedValue);

				// Configure the Session variables used to pass data to the report
				Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.MissingPODList;
				Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsMissingPODs;
				Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
				Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";
				Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;

                lblReportError.Visible = false;
				reportViewer.IdentityId = int.Parse(cboClient.SelectedValue);
				// Show the user control
				reportViewer.Visible = true;

                gvMissingPOD.DataSource = dsMissingPODs;
                gvMissingPOD.DataBind();
                gvMissingPOD.Visible = true;
			}
			else
			{
				lblReportError.Text = "No missing PODs found for client " + cboClient.Text;
				lblReportError.Visible = true;
                gvMissingPOD.Visible = false;
			}
		}

        private string CurrentSortExpression
        {
            get { return (string)ViewState["CurrentSortExpression"]; }
            set { ViewState["CurrentSortExpression"] = value; }
        }

        private SortDirection CurrentSortDirection
        {
            get { return (SortDirection)ViewState["CurrentSortDirection"]; }
            set { ViewState["CurrentSortDirection"] = value; }
        }

        void gvMissingPOD_Sorting(object sender, GridViewSortEventArgs e)
        {
            DataSet dsMissingPODs = GetData();

            string sortExpression = e.SortExpression + " ";
            if (e.SortExpression == CurrentSortExpression)
            {
                if (CurrentSortDirection == SortDirection.Ascending)
                {
                    sortExpression += "DESC";
                    CurrentSortDirection = SortDirection.Descending;
                }
                else
                {
                    sortExpression += "ASC";
                    CurrentSortDirection = SortDirection.Ascending;
                }
            }
            else
            {
                sortExpression += "ASC";
                CurrentSortDirection = SortDirection.Ascending;
            }
            CurrentSortExpression = e.SortExpression;
            dsMissingPODs.Tables[0].DefaultView.Sort = sortExpression;

            if (dsMissingPODs.Tables[0].Rows.Count > 0)
            {
                // Configure the report settings collection
                NameValueCollection reportParams = new NameValueCollection();
                reportParams.Add("Client", cboClient.Text);
                reportParams.Add("IdentityId", cboClient.SelectedValue);

                // Configure the Session variables used to pass data to the report
                Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.MissingPODList;
                Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsMissingPODs.Tables[0].DefaultView;
                Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
                Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";
                Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;

                lblReportError.Visible = false;
                reportViewer.IdentityId = int.Parse(cboClient.SelectedValue);
                // Show the user control
                reportViewer.Visible = true;

                gvMissingPOD.DataSource = dsMissingPODs.Tables[0].DefaultView;
                gvMissingPOD.DataBind();
                gvMissingPOD.Visible = true;
            }
            else
            {
                lblReportError.Text = "No missing PODs found for client " + cboClient.Text;
                lblReportError.Visible = true;
                gvMissingPOD.Visible = false;
            }
        }

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			btnReport.Click += new EventHandler(btnReport_Click);
            cboClient.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
            gvMissingPOD.Sorting += new GridViewSortEventHandler(gvMissingPOD_Sorting);
		}

        void cboClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboClient.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllClientsFiltered(e.Text);

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

        #endregion
	}
}
