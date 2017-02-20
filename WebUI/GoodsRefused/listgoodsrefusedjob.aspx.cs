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

using Orchestrator.WebUI.Security;
using Orchestrator.WebUI.UserControls;
using Orchestrator.Globals;

using P1TP.Components.Web.Validation;

namespace Orchestrator.WebUI.GoodRefused
{
	/// <summary>
	/// Summary description for List Goods Refused Job.
	/// </summary>

	public partial class ListGoodsRefusedJob: Orchestrator.Base.BasePage
	{	
		#region Constant & Enums

        private const string C_REFUSALDS_VS = "RefusalDS";

		#endregion
		
		#region Page Variables

		protected	int		m_StoreIdentityId	= 0;
		protected	string	m_StoreTown			= String.Empty;
		protected	int		m_StoreTownId		= 0;
		protected	int		m_StorePointId		= 0;

		#endregion

		#region Form Elements

		protected System.Web.UI.WebControls.RequiredFieldValidator	rfvJobStatus;

		#endregion

		#region Properties

		#endregion

		#region Page Load/Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

			if (!IsPostBack)
			{
				PopulateStaticControls();

				rfvClientReceipt.Visible = true;
				lblClientReceipt.Visible = true;
				rdoClientReceipt.Visible = true;
			}

			if ((eGoodsRefusedFilterType) Enum.Parse(typeof(eGoodsRefusedFilterType), rdoGoodsRefusedFilterType.SelectedValue.ToString().Replace(" ", "")) == eGoodsRefusedFilterType.GoodsToReturn ) 
				rfvClientReceipt.Visible = false;
			else
				rfvClientReceipt.Visible = true;
		}

		protected void ListInvoice_Init(object sender, EventArgs e)
		{
            this.cboClient.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);

            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
			/*this.btnSearchBottom.Click +=new EventHandler(btnSearch_Click);*/
			this.btnReport.Click += new System.EventHandler(this.btnReport_Click);
			this.btnReportBottom.Click += new System.EventHandler(this.btnReport_Click);
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
			this.Init += new System.EventHandler(this.ListInvoice_Init);

		}
		#endregion

		#region Populate Static Controls 
		///	<summary> 
		///	Populate Static Controls
		///	</summary>
        private void PopulateStaticControls()
        {
            // Filter Options
            rdoGoodsRefusedFilterType.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eGoodsRefusedFilterType)));
            rdoGoodsRefusedFilterType.DataBind();

            // Client Receipts (Include/Exclude/Both)
            rdoClientReceipt.DataSource = Enum.GetNames(typeof(eWithOrWithout));
            rdoClientReceipt.DataBind();

            rdoGoodsRefusedFilterType.Items[1].Selected = true;
            rdoClientReceipt.Items[0].Selected = true;

            btnReport.Visible = btnReportBottom.Visible = false;
        }

        #endregion

        #region Combo's Server Methods

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
		
		#region Events

		///	<summary> 
		///	Button Search Click
		///	</summary>
		private void btnSearch_Click(object sender, System.EventArgs e)
		{
			btnSearch.DisableServerSideValidation();

			if (Page.IsValid)
				GetData();
		}

		private void btnReport_Click(object sender, System.EventArgs e)
		{
            if (dgReturnJobs.SelectedItems.Count != 0)
            {
                lblNote.Visible = false;
                LoadReturnJobReport();
            }
            else
            {
                lblNote.Text = "Please select a return job(s)";
                lblNote.Visible = true;
            }
		}
		
		private void ClearFields()
		{
			pnlReturnJobs.Visible = false;
			btnReport.Visible = btnReportBottom.Visible =  false;
			lblNote.Visible = false; 
			reportViewer.Visible = false;

			for(int i = 0; i < rdoClientReceipt.Items.Count; i++)
			{
				rdoClientReceipt.Items[i].Selected = false;
			}
		}

		private void cboGoodsRefusedState_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			ClearFields(); 
		}

		private void cboGoodsRefusedStatus_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			ClearFields();
		}
		
        #endregion

		#region Methods
	
        private void GetData()
        {
            lblNote.Visible = false;
            rfvClientReceipt.Visible = false;
            GoodsReturnJob();
        }

        private void GoodsReturnJob()
		{
			Facade.IGoodsRefusal facGoods = new Facade.GoodsRefusal ();

			DataSet ds = null;

			int clientId = cboClient.SelectedValue == "" ? 0 : Convert.ToInt32(cboClient.SelectedValue);
			
			DateTime startDate = Convert.ToDateTime(dteStartDate.SelectedDate.Value); 
			DateTime endDate = Convert.ToDateTime(dteEndDate.SelectedDate.Value);

			int includeReceipt = 0;

			includeReceipt = (int) Enum.Parse(typeof(eWithOrWithout), rdoClientReceipt.SelectedValue.ToString().Replace(" ", "")); 

			if ((eWithOrWithout) Enum.Parse(typeof(eWithOrWithout), rdoClientReceipt.SelectedValue.ToString().Replace(" ", "")) == eWithOrWithout.All)
				includeReceipt = 0;

			if (startDate != DateTime.MinValue || endDate != DateTime.MinValue)
				ds = facGoods.GetReturnJobswithDates(clientId, startDate, endDate, includeReceipt);
			else	
			{
				if (clientId == 0)
					ds = facGoods.GetReturnJobs(includeReceipt); 
				else
					ds = facGoods.GetReturnJobswithParams(clientId, includeReceipt);
			}
			
			if (ds.Tables[0].Rows.Count != 0)
			{
				pnlReturnJobs.Visible = true; 
				
                ds.Tables[0].Columns.Add("Include", typeof(Boolean)); 

				dgReturnJobs.Visible = true;
				dgReturnJobs.DataSource = ds;
				dgReturnJobs.DataBind();

                if (cboClient.SelectedValue == "")
                {
                    btnReport.Visible = btnReportBottom.Visible = false;
                    dgReturnJobs.Levels[0].Columns["Include"].Visible = false;
                }
                else
                {
                    dgReturnJobs.Levels[0].Columns["Include"].Visible = true;
                    btnReport.Visible = btnReportBottom.Visible = true;
                }
                
				ViewState[C_REFUSALDS_VS] = ds; 
			}
			else
			{
				pnlReturnJobs.Visible = false;
				
				lblNote.Text = "There are no return runs for given criteria.";
				lblNote.ForeColor = Color.Red;
				lblNote.Visible = true;

                btnReport.Visible = btnReportBottom.Visible = false;
			}
		}

		private void LoadReturnJobReport()
		{
			// Configure the Session variables used to pass data to the report
			NameValueCollection reportParams = new NameValueCollection();
			
			// Client Name & Id
			if (cboClient.SelectedValue != "")
			{
				reportParams.Add("Client", Convert.ToString (cboClient.Text));
				reportParams.Add("ClientId", Convert.ToString(cboClient.SelectedValue));
			}
			
			// Date Range
			if (Convert.ToDateTime(Session["StartDate"]).Date != DateTime.MinValue)
				reportParams.Add("startDate", Convert.ToDateTime(Session["StartDate"]).ToString("dd/MM/yy"));
			
			if (Convert.ToDateTime(Session["EndDate"]).Date != DateTime.MinValue)
				reportParams.Add("endDate", Convert.ToDateTime(Session["EndDate"]).ToString("dd/MM/yy"));

			string m_jobIdCSV = GetJobIdCSV();

			// Dataset 
			Facade.IGoodsRefusal facGoods = new Facade.GoodsRefusal();
			DataSet ds = facGoods.GetJobDetailsForJobIdCSV(m_jobIdCSV);

			//-------------------------------------------------------------------------------------	
			//									Load Report Section 
			//-------------------------------------------------------------------------------------	
            Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.GoodsReturnJob;
            Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = ds;
            Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
            Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";
            Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;
							
			if (cboClient.SelectedValue != "")
				reportViewer.IdentityId = int.Parse(cboClient.SelectedValue);

			// Show the user control
			reportViewer.Visible = true;
		}

		private string GetJobIdCSV()
		{
            return hidSelectedRefusalJobs.Value;
		}

        #endregion
	}
}

