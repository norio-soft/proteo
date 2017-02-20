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

namespace Orchestrator.WebUI.Resource.Driver
{
	/// <summary>
	/// Summary description for ViewDriverStartTimes.
	/// </summary>
	public partial class ViewDriverStartTimes : Orchestrator.Base.BasePage
	{
		#region Form Elements


		#endregion

		#region Page Load/Init/Error

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);
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
			btnGet.Click +=new EventHandler(btnGet_Click);
			dgDrivers.PageIndexChanged +=new DataGridPageChangedEventHandler(dgDrivers_PageIndexChanged);
			dgDrivers.ItemDataBound += new DataGridItemEventHandler(dgDrivers_ItemDataBound);
		}
		#endregion

		#region Event Handlers

		private void btnGet_Click(object sender, EventArgs e)
		{
			Facade.IDriver facDriver = new Facade.Resource();
            dgDrivers.DataSource = GetStartTimesForDate(dteDate.SelectedDate.Value);
			dgDrivers.DataBind();
		}

		#endregion

		#region Private Methods

		private DataSet GetStartTimesForDate(DateTime date)
		{
			Facade.IDriver facDriver = new Facade.Resource();
			return facDriver.GetStartTimesForDate(date);
		}

		#endregion

		#region Property Interfaces

		public string StartDateString
		{
            get { return dteDate.SelectedDate.HasValue ? dteDate.SelectedDate.Value.ToString("dd/MM/yyyy") : string.Empty; }
		}

		#endregion

		#region DataGrid Event Handlers

		private void dgDrivers_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			dgDrivers.CurrentPageIndex = e.NewPageIndex;
            dgDrivers.DataSource = GetStartTimesForDate(dteDate.SelectedDate.Value);
			dgDrivers.DataBind();
		}

		private void dgDrivers_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				DataRowView dv = (DataRowView) e.Item.DataItem;

				HtmlImage imgClockIn = (HtmlImage) e.Item.FindControl("imgClockIn");

				if (dv["StartDateTime"] == DBNull.Value)
				{
					imgClockIn.Visible = true;
					imgClockIn.Attributes.Add("onClick", "javascript:ClockIn('" + dv["ResourceId"] + "');");
				}
				else
					imgClockIn.Visible = false;
			}
		}

		#endregion
	}
}
