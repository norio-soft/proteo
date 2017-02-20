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

namespace Orchestrator.WebUI.Resource
{
	/// <summary>
	/// Summary description for DeletedResources.
	/// </summary>
	public partial class DeletedResources : Orchestrator.Base.BasePage
	{
		#region Form Elements


		#endregion

		#region Page Load/Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditResource);

			if (!IsPostBack)
				BindResources();
		}
		
		private void DeletedResources_Init(object sender, EventArgs e)
		{
			dgResources.ItemCommand += new DataGridCommandEventHandler(dgResources_ItemCommand);
			dgResources.SortCommand += new DataGridSortCommandEventHandler(dgResources_SortCommand);
		}

		#endregion

		#region Property Interfaces

		private string SortCriteria
		{
			get	{ return (string) ViewState["C_SORT_CRITERIA"]; }
			set { ViewState["C_SORT_CRITERIA"] = value; }
		}

		private string SortDirection
		{
			get	{ return (string) ViewState["C_SORT_DIRECTION"]; }
			set { ViewState["C_SORT_DIRECTION"] = value; }
		}

		#endregion

		private void BindResources()
		{
			using (Facade.IResource facResource = new Facade.Resource())
			{
				DataView dv = new DataView(facResource.GetDeletedResources().Tables[0]);
				string sorting = SortCriteria + " " + SortDirection;
				dv.Sort = sorting.Trim();
				dgResources.DataSource = dv;
				dgResources.DataBind();
			}
		}

		#region DataGrid Event Handlers

		private void dgResources_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			switch (e.CommandName.ToLower())
			{
				case "restore":
					int resourceId = Convert.ToInt32(e.Item.Cells[1].Text);
					int resourceTypeId = Convert.ToInt32(e.Item.Cells[2].Text);
					using (Facade.IResource facResource = new Facade.Resource())
					{
						facResource.UpdateResourceStatus(resourceId, (eResourceType) resourceTypeId, eResourceStatus.Active, ((Entities.CustomPrincipal) Page.User).UserName);
						BindResources();
					}
					break;
			}
		}
		
		private void dgResources_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			if (e.SortExpression == SortCriteria)
			{
				if (SortDirection == "DESC")
					SortDirection = "ASC";
				else
					SortDirection = "DESC";
			}
			else
			{
				SortCriteria = e.SortExpression;
				SortDirection = "DESC";
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
			this.Init += new EventHandler(DeletedResources_Init);
		}
		#endregion
	}
}
