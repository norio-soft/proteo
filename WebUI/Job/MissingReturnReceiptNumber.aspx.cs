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

namespace Orchestrator.WebUI.Job
{
	/// <summary>
	/// Summary description for MissingReturnReceiptNumber.
	/// </summary>
	public partial class MissingReturnReceiptNumber : Orchestrator.Base.BasePage
	{
		#region Constants

		private const string C_SORT_CRITERIA_VS = "C_SORT_CRITERIA_VS";
		private const string C_SORT_DIRECTION_VS = "C_SORT_DIRECTION_VS";

		#endregion

		#region Form Elements


		#endregion

		#region Property Interfaces

		private string SortCriteria
		{
			get { return (string) ViewState[C_SORT_CRITERIA_VS]; }
			set { ViewState[C_SORT_CRITERIA_VS] = value; }
		}

		private string SortDirection
		{
			get { return (string) ViewState[C_SORT_DIRECTION_VS]; }
			set { ViewState[C_SORT_DIRECTION_VS] = value; }
		}

		#endregion

		#region Page Load/Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

			if (!IsPostBack)
			{
				PopulateJobs();
			}
		}

		private void MissingReturnReceiptNumber_Init(object sender, EventArgs e)
		{
			dgJobs.ItemDataBound += new DataGridItemEventHandler(dgJobs_ItemDataBound);
			dgJobs.PageIndexChanged += new DataGridPageChangedEventHandler(dgJobs_PageIndexChanged);
			dgJobs.SortCommand += new DataGridSortCommandEventHandler(dgJobs_SortCommand);

			dgJobs.CancelCommand += new DataGridCommandEventHandler(dgJobs_CancelCommand);
			dgJobs.EditCommand += new DataGridCommandEventHandler(dgJobs_EditCommand);
			dgJobs.UpdateCommand += new DataGridCommandEventHandler(dgJobs_UpdateCommand);
		}

		#endregion

		private void PopulateJobs()
		{
			Facade.IJob facJob = new Facade.Job();
			DataSet ds = facJob.GetGoodsReturnJobsMissingReturnReceipts();

			DataView dv = new DataView(ds.Tables[0]);
			string sortExpression = SortCriteria + " " + SortDirection;
			dv.Sort = sortExpression.Trim();

			// Stop the page throwing an error if we're on the last page (with on entry) that is then removed on update.
			if (dv.Table.Rows.Count % dgJobs.PageSize == 0 && dv.Table.Rows.Count / dgJobs.PageSize == dgJobs.CurrentPageIndex && dgJobs.CurrentPageIndex > 1)
				dgJobs.CurrentPageIndex--;

			dgJobs.DataSource = dv;
			dgJobs.DataBind();
		}

		#region DataGrid Events

		private void dgJobs_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.EditItem)
			{
				DataRowView row = (DataRowView) e.Item.DataItem;
				Label lblDeliveryDate = (Label) e.Item.FindControl("lblDeliveryDate");

				if ((bool) row["IsAnytime"])
					lblDeliveryDate.Text = ((DateTime) row["BookedDateTime"]).ToString("dd/MM/yy") + " AnyTime";
				else
					lblDeliveryDate.Text = ((DateTime) row["BookedDateTime"]).ToString("dd/MM/yy HH:mm");
			}
		}

		private void dgJobs_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			dgJobs.EditItemIndex = -1;
			if (SortCriteria == e.SortExpression)
			{
				if (SortDirection == "ASC")
					SortDirection = "DESC";
				else
					SortDirection = "ASC";
			}
			else
			{
				SortCriteria = e.SortExpression;
				SortDirection = "DESC";
			}

			PopulateJobs();
		}

		private void dgJobs_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			dgJobs.EditItemIndex = -1;
			dgJobs.CurrentPageIndex = e.NewPageIndex;
			PopulateJobs();
		}

		private void dgJobs_EditCommand(object source, DataGridCommandEventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditJob, eSystemPortion.TakeCallIn);
			dgJobs.EditItemIndex = e.Item.ItemIndex;
			PopulateJobs();
		}

		private void dgJobs_UpdateCommand(object source, DataGridCommandEventArgs e)
		{
			int jobId = Convert.ToInt32(e.Item.Cells[0].Text);
			string receiptNumber = ((TextBox) e.Item.Cells[2].Controls[0]).Text;

			Facade.IJob facJob = new Facade.Job();
			Entities.Job job = facJob.GetJob(jobId);

			job.ReturnReceiptNumber = receiptNumber;
			facJob.Update(job, ((Entities.CustomPrincipal) Page.User).UserName);

			dgJobs.EditItemIndex = -1;
			PopulateJobs();
		}

		private void dgJobs_CancelCommand(object source, DataGridCommandEventArgs e)
		{
			dgJobs.EditItemIndex = -1;
			PopulateJobs();
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
			this.Init += new EventHandler(MissingReturnReceiptNumber_Init);
		}
		#endregion
	}
}
