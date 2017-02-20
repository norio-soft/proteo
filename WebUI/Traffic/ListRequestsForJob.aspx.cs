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

using P1TP.Components.Web.UI;

namespace Orchestrator.WebUI.Traffic
{
	/// <summary>
	/// Summary description for ListRequestsForJob.
	/// </summary>
	public partial class ListRequestsForJob : Orchestrator.Base.BasePage
	{
		#region Form Elements

		


		#endregion

		#region Page Variables

		private		int				m_jobId = 0;

		#endregion

		#region Page Load/Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

			m_jobId = Convert.ToInt32(Request.QueryString["JobId"]);
			
			if (!IsPostBack)
				BindData();

			lblConfirmation.Visible = false;
		}

		private void ListRequestsForJob_Init(object sender, EventArgs e)
		{
			dgRequests.ItemCommand += new DataGridCommandEventHandler(dgRequests_ItemCommand);
		}

		#endregion

		private void BindData()
		{
			if (m_jobId > 0)
			{
				Facade.IPlannerRequest facPlannerRequest = new Facade.PlannerRequest();
				dgRequests.DataSource = facPlannerRequest.GetPlannerRequestsForJobId(m_jobId);
				dgRequests.DataBind();

				lblJobId.Text = m_jobId.ToString();
			}
		}

		#region Pretty Data Grid Events

		private void dgRequests_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			switch (e.CommandName.ToLower())
			{
				case "delete":
					int requestId = Convert.ToInt32(e.Item.Cells[0].Text);
					Facade.IPlannerRequest facPlannerRequest = new Facade.PlannerRequest();
					bool success = facPlannerRequest.Delete(requestId);

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
			this.Init += new EventHandler(ListRequestsForJob_Init);
		}
		#endregion
	}
}
