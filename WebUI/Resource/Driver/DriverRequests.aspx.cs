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

namespace Orchestrator.WebUI.Resource.Driver
{
	/// <summary>
	/// Summary description for DriverRequests.
	/// </summary>
	public partial class DriverRequests : Orchestrator.Base.BasePage
	{
		#region Form Elements


		

		#endregion

		#region Page Variables

		private		int				m_resourceId;
		private		DateTime		m_startDate;

		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

			m_resourceId = Convert.ToInt32(Request.QueryString["resourceId"]);
			string date = Request.QueryString["fromDateTime"];
			m_startDate = new DateTime(Convert.ToInt32(date.Substring(4, 4)), Convert.ToInt32(date.Substring(2, 2)), Convert.ToInt32(date.Substring(0, 2)), Convert.ToInt32(date.Substring(8, 2)), Convert.ToInt32(date.Substring(10, 2)), 0);

			if (!IsPostBack)
			{
				Facade.IDriver facDriver = new Facade.Resource();
				lblDriver.Text = facDriver.GetDriverForResourceId(m_resourceId).Individual.FullName;

				lblStartDate.Text = m_startDate.ToString("dd/MM/yy");

				FilterRequests();
			}		

			lblConfirmation.Visible = false;
		}

		private void FilterRequests()
		{
			Facade.IDriver facDriver = new Facade.Resource();
			dgRequests.DataSource = facDriver.GetDriverRequests(m_startDate, DateTime.MaxValue, m_resourceId, ((Entities.CustomPrincipal) Page.User).IdentityId);
			dgRequests.DataBind();
		}

		#region Pretty Data Grid Events
		
		private void dgRequests_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			HtmlInputHidden hidRequestId = (HtmlInputHidden) e.Item.FindControl("hidRequestId");

			if (hidRequestId != null)
			{
				int requestId = Convert.ToInt32(hidRequestId.Value);

				switch (e.CommandName.ToLower())
				{
					case "edit":
						Response.Redirect("AddUpdateDriverRequest.aspx?requestId=" + requestId.ToString());
						break;
					case "delete":
						Facade.IDriverRequest facDriverRequest = new Facade.Resource();
						if (facDriverRequest.Delete(requestId, ((Entities.CustomPrincipal) Page.User).UserName))
							lblConfirmation.Text = "The request has been deleted.";
						else
							lblConfirmation.Text = "The request has not been deleted.";
						lblConfirmation.Visible = true;

						FilterRequests();
						break;
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
			dgRequests.ItemCommand += new DataGridCommandEventHandler(dgRequests_ItemCommand);
		}
		#endregion
	}
}
