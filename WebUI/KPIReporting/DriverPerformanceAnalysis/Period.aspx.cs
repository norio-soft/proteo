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

using P1TP.Components.Web.Validation;
using Orchestrator.Globals;

namespace Orchestrator.WebUI.KPIReporting.DriverPerformanceAnalysis
{
	/// <summary>
	/// Summary description for Period.
	/// </summary>
	public partial class Period : Orchestrator.Base.BasePage
	{
		#region Form Elements






		#endregion

		#region Page Load/Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Authorise.EnforceAuthorisation(eSystemPortion.KPI);
		}

		private void Period_Init(object sender, EventArgs e)
		{
			cfvStartDate.ServerValidate += new ServerValidateEventHandler(cfvStartDate_ServerValidate);
			btnViewReport.Click += new EventHandler(btnViewReport_Click);
            this.cboDriver.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboDriver_ItemsRequested);
		}

        void cboDriver_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
        
            cboDriver.Items.Clear();

            Facade.IResource facResource = new Facade.Resource();
            DataSet ds = facResource.GetAllResourcesFiltered(e.Text, eResourceType.Driver, true);

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
                rcItem.Text = dt.Rows[i]["Description"].ToString();
                rcItem.Value = dt.Rows[i]["ResourceId"].ToString();
                cboDriver.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

		#endregion

		#region Button Events

		private void btnViewReport_Click(object sender, EventArgs e)
		{
			btnViewReport.DisableServerSideValidation();

			if (Page.IsValid)
			{
				// Retrieve the values to report on
				int driverId = Convert.ToInt32(cboDriver.SelectedValue);
				DateTime startDate = dteStartDate.SelectedDate.Value.Subtract(dteStartDate.SelectedDate.Value.TimeOfDay);
				DateTime endDate = dteEndDate.SelectedDate.Value.Subtract(dteEndDate.SelectedDate.Value.TimeOfDay).Add(new TimeSpan(23, 59, 59));

				// Get the data needed to run the report
				Facade.IKPI facKPI = new Facade.KPI();
				DataSet ds = facKPI.GetDriverPerformanceForPeriod(driverId, startDate, endDate);

				Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.DriverPerformanceAnalysisPeriod;
				Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = ds;
				Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
				Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";

				NameValueCollection nvc = new NameValueCollection();
				nvc.Add("ReportTitle", "KPI Performance (Period)");
				nvc.Add("Driver", cboDriver.Text);
                nvc.Add("StartDate", startDate.ToString("dd/MM/yy"));
                nvc.Add("EndDate", endDate.ToString("dd/MM/yy"));

				Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = nvc;

				reportViewer1.Visible = true;
			}
		}

		#endregion

		#region Validation

		private void cfvStartDate_ServerValidate(object source, ServerValidateEventArgs args)
		{
			args.IsValid = false;

			if (rfvEndDate.IsValid)
			{
				if (dteStartDate.SelectedDate <= dteEndDate.SelectedDate)
					args.IsValid = true;
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
			this.Init += new EventHandler(Period_Init);
		}
		#endregion
	}
}
