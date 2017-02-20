using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Orchestrator.Globals;
using Orchestrator.WebUI.UserControls;
using P1TP.Components.Web.Validation;



namespace Orchestrator.WebUI.PCV
{
	/// <summary>
	/// Summary description for PCVRedemptionReport.
	/// </summary>
	public partial class PCVRedemptionReport : Orchestrator.Base.BasePage
	{

		#region Form Elements

		protected DropDownList		cboReportType;
		#endregion

		#region Page Variables

		private string m_reportType;
		protected	int m_pcvId = 0;

		#endregion

		#region Page Load/Init/Error

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

			m_reportType = Convert.ToString(Request.QueryString["reportType"]);
			m_pcvId = Convert.ToInt32(Request.QueryString["PCVId"]);

			btnGenerateReport.Attributes.Add("onclick", "ClearQueryString()");
						
			switch (m_reportType)
			{
				case "beingRedeemed":
					h1Text.Text = "PCVs Being Redeemed.";
                    h2Text.Text = "A list of PCVs being taken for redemption on runs during the specified period is displayed below.";
					break;
				case "outstanding":
                    h1Text.Text = "PCVs Awaiting Redemption";
                    h2Text.Text = "A list of PCVs which have not been Redeemed or Posted to B&M For Dehire is displayed below";
					break;
			}

			if (!IsPostBack && m_reportType != null) 
			{

				// If a different report type, clear the session variables used for sorting.
				if (Convert.ToString(Session["ReportType"]) != m_reportType)
				{
					ClearSessionVariables();
				}

				if (Session["ClientId"] != null || (Session["DateFrom"] != null && Session["DateTo"] != null))
				{
					if (Session["ClientId"] != null)
					{
						string clientId = Convert.ToString(Session["ClientId"]);
						Facade.IOrganisation facOrganisation = new Facade.Organisation();
						string clientName = facOrganisation.GetNameForIdentityId(int.Parse(clientId));
						cboClient.SelectedValue = clientId;
						cboClient.Text = clientName;
					}
                    dteDateFrom.SelectedDate = Session["DateFrom"] != null ? (DateTime?)Session["DateFrom"] : (DateTime?)null ;
                    dteDateTo.SelectedDate = Session["DateTo"] != null ? (DateTime?)Session["DateTo"] : (DateTime?)null;
					LoadReport();
				}
				else
				{
					// Default period from yesterday to today
                    dteDateFrom.SelectedDate = DateTime.Today - new TimeSpan(1, 0, 0, 0);
                    dteDateTo.SelectedDate = DateTime.Today;
				}
				if (Session["Sort"] != null)
				{
					// Load the report without user intervention, since sorting a current
					// report.
					LoadReport();
				}

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
			this.Init +=new EventHandler(PCVRedemptionReport_Init);
		}
		#endregion
	
		#region DBCombo's Server Methods and Initialisation

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

		#region ActiveReport 

		private void LoadReport()
		{
			NameValueCollection reportParams = new NameValueCollection();

			string redemptionStatusCSV = null;

			switch (m_reportType)
			{
				case "beingRedeemed":
					reportParams.Add("ReportType", "beingRedeemed");
					redemptionStatusCSV = Convert.ToString((int)ePCVRedemptionStatus.ToBeRedeemed);
					break;
				case "outstanding":
					reportParams.Add("ReportType", "Awaiting Redemption");
					redemptionStatusCSV = Convert.ToString((int)ePCVRedemptionStatus.ToBeRedeemed) + "," + Convert.ToString((int)ePCVRedemptionStatus.RequiresDeHire);
					break;
			}

			int clientId = cboClient.SelectedValue == "" ? 0 : int.Parse(cboClient.SelectedValue);
			
			if (clientId > 0)
				reportParams.Add("Client", cboClient.Text);

			DateTime dateFrom;
			DateTime dateTo;

			// Check the user hasn't cleared the WebDateTimeEdit fields. If so, 
			// use defaults.
			if (dteDateFrom.SelectedDate.Value == DateTime.MinValue)
			{
				dateFrom = DateTime.Today - new TimeSpan(1, 0, 0, 0);
			}
			else
			{
				dateFrom =dteDateFrom.SelectedDate.Value;
			}
			if (dteDateTo.SelectedDate.Value == DateTime.MinValue)
			{

				dateTo = DateTime.Today;
			}
			else
			{
				dateTo = dteDateTo.SelectedDate.Value;
			}

			Facade.IPCV facPCV = new Facade.PCV();
			DataSet dsPCVs = facPCV.GetWithJobByRedemptionStatus(clientId, dateFrom, dateTo, redemptionStatusCSV);
			
			if (dsPCVs.Tables[0].Rows.Count == 0)
			{
				lblError.Text = "No PCVs found for period " + dteDateFrom.SelectedDate.Value.ToString("dd/MM/yy") + " " + " to " + dteDateTo.SelectedDate.Value.ToString("dd/MM/yy");
				lblError.Visible = true;
			}
			else
			{
				//-------------------------------------------------------------------------------------	
				//									Load Report Section 
				//-------------------------------------------------------------------------------------	
				Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.PCV;
				Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;

				reportParams.Add("DateFrom", dteDateFrom.SelectedDate.Value.ToString("dd/MM/yy"));
				reportParams.Add("DateTo", dteDateTo.SelectedDate.Value.ToString("dd/MM/yy"));
				
				if (clientId > 0)
					reportParams.Add("ClientId", cboClient.SelectedValue);

				if (Request.QueryString["sort"] != null)
				{
					reportParams.Add("sort", "true");
					reportParams.Add("sortDirection", Convert.ToString(Request.QueryString["sortDirection"]));
				
					DataView dvPCVs = new DataView(dsPCVs.Tables[0]);

					string sortBy = Convert.ToString(Request.QueryString["sortBy"]);
					string sortDirection = Convert.ToString(Request.QueryString["sortDirection"]);

					// Check query string has not been altered by user. If it has, set to a 
					// default value
					if (sortBy != "PCVId" && sortBy != "RedemptionStatus")
						sortBy = "PCVId";

					if (sortDirection != "ASC" && sortDirection != "DESC")
						sortDirection = "ASC";

					if (sortBy == "PCVId")
						sortBy = "VoucherNo";
			
					Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsPCVs;
					Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = sortBy + " " + sortDirection;

					// Since a DataView, no DataMember as underlying DataTable object does not change and
					// setting it to "Table" would not reflect the sort being applied.
					Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "";
				}
				else
				{
					reportParams.Add("sort", "false");
					Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsPCVs;
					Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
					Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";
				}

				if (cboClient.SelectedValue != "")
					reportViewer.IdentityId = int.Parse(cboClient.SelectedValue);
				reportViewer.Visible = true;
			}
		}

		#endregion

		#region Event Handlers

		private void btnGenerateReport_Click(object sender, EventArgs e)
		{
			ClearSessionVariables();

			if (cboClient.Text != "")
				rfvClient.Enabled = true;
			else
				rfvClient.Enabled = false;

			if (Page.IsValid)
				LoadReport();
		}

		private void PCVRedemptionReport_Init(object sender, EventArgs e)
		{
			btnGenerateReport.Click +=new EventHandler(btnGenerateReport_Click);
			btnReset.Click +=new EventHandler(btnReset_Click);
            this.cboClient.ItemsRequested +=new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
		}

		private void btnReset_Click(object sender, EventArgs e)
		{
			ClearSessionVariables();

			cboClient.SelectedValue = "";
			cboClient.Text = "";

			// Default period from yesterday to today
            dteDateFrom.SelectedDate = DateTime.Today - new TimeSpan(1, 0, 0, 0);
            dteDateTo.SelectedDate = DateTime.Today;

			reportViewer.Visible = false;
		}

		#endregion

		#region Validation

		protected void cfvStartDate_ServerValidate(object source, ServerValidateEventArgs args)
		{
			args.IsValid = false;

            if (dteDateFrom.SelectedDate <= dteDateTo.SelectedDate)
				args.IsValid = true;
		}

		#endregion

		private void ClearSessionVariables()
		{
			Session["Sort"] = null;
			Session["DateFrom"] = null;
			Session["DateTo"] = null;
			Session["ClientId"] = null;
		}
	}
}
