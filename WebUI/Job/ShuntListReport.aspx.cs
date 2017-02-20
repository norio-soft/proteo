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


namespace Orchestrator.WebUI.Job
{
	/// <summary>
	/// Summary description for ShuntListReport.
	/// </summary>
	public partial class ShuntListReport : Orchestrator.Base.BasePage
	{
		#region Form Elements


		#endregion

		#region Page Load

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

			if (!IsPostBack)
			{
				DateTime today = DateTime.UtcNow;
				today = today.Subtract(today.TimeOfDay);

                dteShuntListDate.SelectedDate = today;
			}
		}

        void ShuntListReport_Init(object sender, EventArgs e)
        {
            this.cboClient.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
            cfvClient.ServerValidate += new ServerValidateEventHandler(cfvClient_ServerValidate);
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

		#region Event Handlers

        void cfvClient_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(cboClient.SelectedValue, 1, true);
        }
        
		private void btnReport_Click(object sender, EventArgs e)
		{
			btnReport.DisableServerSideValidation();

			if (Page.IsValid)
			{
				// Retrieve the report data and place it in the session variables
				GetShuntListData();
			}
			else
			{
				// Hide the report
				reportViewer.Visible = false;
			}
		}
		
		private void btnReset_Click(object sender, EventArgs e)
		{
			// Reset the dbcombo
			cboClient.Text = String.Empty;
			cboClient.SelectedValue = String.Empty;

			// Reset the report viewer
			reportViewer.Visible = false;
		}

		#endregion

		#region Active Report

		private void GetShuntListData()
		{
			DateTime shuntListDate = dteShuntListDate.SelectedDate.Value;
            shuntListDate = shuntListDate.Subtract(shuntListDate.TimeOfDay);

			Facade.Job facJob = new Facade.Job();

			int identityId = Convert.ToInt32(cboClient.SelectedValue);
			DataSet dsShuntList = facJob.GetShuntListForClient(identityId, shuntListDate);

			if (dsShuntList.Tables[0].Rows.Count > 0)
			{
				// Configure the report settings collection
				NameValueCollection reportParams = new NameValueCollection();
				reportParams.Add("Client", cboClient.Text);
				reportParams.Add("IdentityId", cboClient.SelectedValue);
				reportParams.Add("Date", shuntListDate.ToString("dd/MM/yy"));
				
				// Configure the Session variables used to pass data to the report
				Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.ShuntList;
				Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsShuntList;
				Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
				Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";
				Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;

				// Set the identity ID property of the report viewer.
				reportViewer.IdentityId = identityId;

				// Show the user control
				reportViewer.Visible = true;
			}
			else
			{
				lblError.Text = "No jobs found for client " + cboClient.Text;
				lblError.Visible = true;
			}
		}

		#endregion

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
			btnReset.Click += new EventHandler(btnReset_Click);
            this.Init +=new EventHandler(ShuntListReport_Init);
		}
		#endregion

		
	}
}
