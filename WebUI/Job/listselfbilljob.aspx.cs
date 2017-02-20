using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Orchestrator.WebUI.Security;

namespace Orchestrator.WebUI.Job
{
	/// <summary>
	/// Summary description for list self bill job.
	/// </summary>
	public partial class listselfbilljob: Orchestrator.Base.BasePage
	{	
		#region Constants & Enums

		private const string C_JobDATA_VS = "JobData"; 
		
		#endregion

		#region Page Variables
		
		private DataSet dsJob; 
		private string sortDirection = "asc";

		#endregion

		#region Form Elements
		#endregion

		#region Page Load/Init
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

			if (!IsPostBack)
			{
				PopulateStaticControls();

				performSearch();
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
		
		protected void ListInvoice_Init(object sender, EventArgs e)
		{
			this.btnDates.Click += new System.EventHandler(this.btnDates_Click);
			this.dgJob.SortCommand += new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.dgJob_SortCommand);
			this.dgJob.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dgJob_ItemDataBound);
			this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            this.cboClient.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
		}

        

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.dgJob.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgJob_CancelCommand);
			this.dgJob.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgJob_EditCommand);
			this.dgJob.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgJob_UpdateCommand);
			this.Init += new System.EventHandler(this.ListInvoice_Init);

		}
		#endregion

		#region Populate Static Controls 
		///	<summary> 
		///	Populate Static Controls
		///	</summary>
		private void PopulateStaticControls()
		{

		}


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

		#endregion
		
		#region Methods & Events 
		///	<summary> 
		///	Button Search Click
		///	</summary>
		private void btnSearch_Click(object sender, System.EventArgs e)
		{
			performSearch();
		}
		
		private void btnDates_Click(object sender, System.EventArgs e)
		{
			if ((Convert.ToDateTime(dteStartDate.SelectedDate.Value) != DateTime.MinValue) || (Convert.ToDateTime(dteEndDate.SelectedDate.Value) != DateTime.MinValue))
			{
				dteStartDate.SelectedDate= (DateTime?)null;
				dteEndDate.SelectedDate = (DateTime?)null;
				return;
			}

			DateTime startWeek = DateTime.UtcNow;
			TimeSpan toBegin = new TimeSpan((int) startWeek.DayOfWeek, 0,0,0);
			startWeek = startWeek.Subtract(toBegin);
			
			//startWeek = startWeek.Subtract(startWeek.TimeOfDay);
            dteStartDate.SelectedDate = startWeek; 
			
			// Add default 7 days to end date
            dteEndDate.SelectedDate = startWeek;
		}


		private void performSearch()
		{
			Facade.IJob facJob = new Facade.Job();

			int clientId = cboClient.SelectedValue == "" ? 0 : Convert.ToInt32(cboClient.SelectedValue);
			
			DateTime startDate = dteStartDate.SelectedDate.Value; 
			
			DateTime endDate = dteEndDate.SelectedDate.Value;
			
			if (startDate != DateTime.MinValue || endDate != DateTime.MinValue)
				dsJob = facJob.GetSelfBillJobswithParamsAndDate (clientId, startDate, endDate);   
			else
			{
				if (clientId == 0)
					dsJob = facJob.GetSelfBillJobs();
				else
					dsJob = facJob.GetSelfBillJobswithParams(clientId);   
			}  
	
			dgJob.DataSource = dsJob;
		
			ViewState[C_JobDATA_VS] = dsJob; 

			dgJob.DataBind();
		}
		
		protected string[] SelfBillJobState
		{
			get { return Enum.GetNames(typeof(eSelfBillStatus)); }
		}

		private void PopulateJob()
		{
			DataView dvJob  = new DataView(GetSelfBillJobData().Tables[0]);
			
			dgJob.DataSource = dvJob;
			dgJob.DataBind();
		}

		private DataSet GetSelfBillJobData()
		{
			Facade.IJob facJob = new Facade.Job();
			
			int clientId = cboClient.SelectedValue == "" ? 0 : Convert.ToInt32(cboClient.SelectedValue);
			
			DateTime startDate = Convert.ToDateTime(dteStartDate.SelectedDate.Value); 
			
			DateTime endDate = Convert.ToDateTime(dteEndDate.SelectedDate.Value);

			if (startDate != DateTime.MinValue || endDate != DateTime.MinValue)
				dsJob = facJob.GetSelfBillJobswithParamsAndDate (clientId, startDate, endDate);   
			else
			{
				if (clientId == 0)
					dsJob = facJob.GetSelfBillJobs();
				else
					dsJob = facJob.GetSelfBillJobswithParams(clientId);   
			}  

			return dsJob;
		}
		#endregion
		
		#region Grid 
		protected void dgJob_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			// TODO eItem 
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				eSelfBillStatus state = (eSelfBillStatus) Enum.Parse(typeof(eSelfBillStatus), (string) (((DataRowView) e.Item.DataItem)["Description"]));
				
				if (state == eSelfBillStatus.Accepted || state == eSelfBillStatus.Invoiced)
					e.Item.Cells[8].Visible = false;
			}
			else if (e.Item.ItemType == ListItemType.EditItem || e.Item.ItemType == ListItemType.SelectedItem)
			{
				DropDownList state = (DropDownList) e.Item.FindControl("cboJobSelfBillState");
				if ((eSelfBillStatus) Enum.Parse (typeof(eSelfBillStatus), state.SelectedValue) == eSelfBillStatus.Invoiced)   
					e.Item.Cells[8].Visible = false;
			}
		}
		protected void dgJob_Page(Object sender, DataGridPageChangedEventArgs e) 
		{
			dgJob.CurrentPageIndex = e.NewPageIndex;   
		}

		private void dgJob_SortCommand(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{
			DataView dvJob = new DataView(dsJob.Tables[0]);

			if (sortDirection == "desc")
			{
				dvJob.Sort = e.SortExpression + " desc "; 
				sortDirection = (string)ViewState["asc"];
			}
			else
			{
				dvJob.Sort = e.SortExpression; 
				sortDirection = (string)ViewState["desc"];
			}
			dgJob.DataSource = dvJob; 
			dgJob.DataBind(); 
		}
		private void dgJob_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			dgJob.EditItemIndex = e.Item.ItemIndex;
			PopulateJob();
		}

		private void dgJob_CancelCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			dgJob.EditItemIndex = -1;
			PopulateJob();
		}

		private void dgJob_UpdateCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			DataGridItem updatingItem = dgJob.Items[e.Item.ItemIndex];
			
			Facade.IJob facJob = new Facade.Job();

			int invoiceId = Convert.ToInt32(((Label) updatingItem.FindControl("lblInvoiceId")).Text);
			int jobId = Convert.ToInt32(((Label) updatingItem.FindControl("lblJobId")).Text);
			string whom = ((TextBox) updatingItem.FindControl("txtWhom")).Text;
			decimal amount = Convert.ToDecimal(((TextBox) updatingItem.FindControl("txtChargeAmount")).Text);
			DropDownList cboJobSelfBillState = (DropDownList) updatingItem.FindControl("cboJobSelfBillState");

			eSelfBillStatus status = (eSelfBillStatus) Enum.Parse (typeof(eSelfBillStatus), cboJobSelfBillState.SelectedValue, true);  
			
			// TODO: May need to check that the State is not awaiting but has someone in the Whom section

			string userId = ((Entities.CustomPrincipal) Page.User).UserName;
			bool success = facJob.Update(invoiceId, jobId, whom, status, amount, userId);

			if (success)
			{
				lblConfirmation.Text = "The Self Bill Job was updated successfully.";
				lblConfirmation.Visible = true;
				dgJob.EditItemIndex = -1;

				// TODO: Update the job in viewstate
				// ViewState[C_JOB_VS] = m_job;

				// Rebind the datagrid
				PopulateJob();
			}
			else
			{
				lblConfirmation.Text = "The Self Bill Job was not updated successfully.";
				lblConfirmation.Visible = true;
			}
		}

		#endregion
	}
}

