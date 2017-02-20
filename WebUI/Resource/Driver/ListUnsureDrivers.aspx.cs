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
	/// Summary description for ListUnsureDrivers.
	/// </summary>
	public partial class ListUnsureDrivers : Orchestrator.Base.BasePage
	{
		#region Form Elements


		#endregion

		#region Page Load/Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
				LoadData();

			lblError.Visible = false;
		}

		private void ListUnsureDrivers_Init(object sender, EventArgs e)
		{
			dgDriversAwaitingInstruction.ItemDataBound += new DataGridItemEventHandler(dgDriversAwaitingInstruction_ItemDataBound);
		}

		#endregion

		private void LoadData()
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

			using (Facade.IDriver facDriver = new Facade.Resource())
				dgDriversAwaitingInstruction.DataSource = facDriver.GetDriversAwaitingInstruction();
			dgDriversAwaitingInstruction.DataBind();
		}

		#region DataGrid Events

		private void dgDriversAwaitingInstruction_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				DataRowView dv = (DataRowView) e.Item.DataItem;
				if (dv["JobId"] != DBNull.Value && ((int) dv["JobId"]) == -1)
					e.Item.Cells[5].Controls.Clear();
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
			this.Init += new EventHandler(ListUnsureDrivers_Init);
		}
		#endregion
	}
}
