using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using P1TP.Components.Web.UI;

namespace Orchestrator.WebUI.Traffic
{
	/// <summary>
	/// Summary description for ListPlannerRequests.
	/// </summary>
	public partial class ListPlannerRequests : Orchestrator.Base.BasePage
	{
		#region Form Elements

		




		#endregion

		#region Page Load/Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

			if (!IsPostBack)
			{
				PopulateStaticControls();
				BindData();
			}

			lblConfirmation.Visible = false;
		}

		private void ListPlannerRequests_Init(object sender, EventArgs e)
		{
			btnFilter.Click += new EventHandler(btnFilter_Click);
			dgRequests.ItemCommand += new DataGridCommandEventHandler(dgRequests_ItemCommand);
			btnAddRequest.Click += new EventHandler(btnAddRequest_Click);
		}

		#endregion

		private void BindData()
		{
			// Get the users we're interested in
			StringBuilder sb = new StringBuilder();
			foreach (ListItem item in chkPlanners.Items)
			{
				if (item.Selected)
				{
					if (sb.Length > 0)
						sb.Append(",");
					sb.Append(item.Value);
				}
			}

			// Get the job id
			int jobId = 0;
			try
			{
				jobId = Convert.ToInt32(txtJobId.Text);
			}
			catch {}

			using (Facade.IPlannerRequest facPlannerRequest = new Facade.PlannerRequest())
			{
				dgRequests.DataSource = facPlannerRequest.GetPlannerRequests(sb.ToString(), jobId);
				dgRequests.DataBind();
			
				if (((DataTable) dgRequests.DataSource).Rows.Count == 0)
					dgRequests.Visible = false;
				else
					dgRequests.Visible = true;
			}
		}

		private void PopulateStaticControls()
		{
			using (Facade.IUser facUser = new Facade.User())
			{
				chkPlanners.DataSource = facUser.GetAllUsersInRole(eUserRole.Planner);
			}
			chkPlanners.DataValueField = "UserName";
			chkPlanners.DataTextField = "FullName";
			chkPlanners.DataBind();
		}

		#region Button Events

		private void btnFilter_Click(object sender, EventArgs e)
		{
			BindData();
		}

		private void btnAddRequest_Click(object sender, EventArgs e)
		{
			Response.Redirect("AddUpdatePlannerRequest.aspx");
		}

		#endregion

		#region Pretty Data Grid Events

		private void dgRequests_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			switch (e.CommandName.ToLower())
			{
				case "edit":
					Response.Redirect("AddUpdatePlannerRequest.aspx?requestId=" + e.Item.Cells[0].Text);
					break;
				case "delete":
					Facade.IPlannerRequest facPlannerRequest = new Facade.PlannerRequest();
					bool success = facPlannerRequest.Delete(Convert.ToInt32(e.Item.Cells[0].Text));

					if (success)
						BindData();

					lblConfirmation.Text = "The planner request was " + (success ? "" : "not ") + "deleted.";
					lblConfirmation.Visible = true;
					break;
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
			this.Init += new EventHandler(ListPlannerRequests_Init);
		}
		#endregion
	}
}
