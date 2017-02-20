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
	/// Summary description for Trend.
	/// </summary>
	public partial class Trend : Orchestrator.Base.BasePage
	{
		#region Constants

		private const string	C_DRIVER_COLLECTION = "C_DRIVER_COLLECTION";

		#endregion

		#region Form Elements







		#endregion

		#region Page Variables

		private		Entities.DriverCollection	m_drivers;

		#endregion

		#region Page Load/Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Authorise.EnforceAuthorisation(eSystemPortion.KPI);

            if (!IsPostBack)
            {
                m_drivers = new Entities.DriverCollection();
                ViewState[C_DRIVER_COLLECTION] = m_drivers;
                BindDrivers();

                cboTrendPeriod.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eKPITrendPeriod)));
                cboTrendPeriod.DataBind();
            }
            else
            {
                m_drivers = (Entities.DriverCollection)ViewState[C_DRIVER_COLLECTION];
            }
		}

		protected void Trend_Init(object sender, EventArgs e)
		{
			cfvStartDate.ServerValidate += new ServerValidateEventHandler(cfvStartDate_ServerValidate);
			btnViewReport.Click += new EventHandler(btnViewReport_Click);
			btnSelectDriver.Click += new EventHandler(btnSelectDriver_Click);
			dgDrivers.ItemCommand += new DataGridCommandEventHandler(dgDrivers_ItemCommand);
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

		#region Events & Methods
		private void BindDrivers()
		{
			dgDrivers.DataSource = m_drivers;
			dgDrivers.DataBind();
			
			if (dgDrivers.Items.Count != 0)
			{
				dgDrivers.Visible = true;
				lblNote.Visible = false;
			}
			else
			{
				dgDrivers.Visible = false;
				lblNote.Text = "No Drivers have been selected.";
				lblNote.Visible = true;
				lblNote.ForeColor = Color.Red;
			}

			// Only allow the user to view the report once drivers have been specified
			btnViewReport.Enabled = m_drivers.Count > 0;
		}

		private void btnSelectDriver_Click(object sender, EventArgs e)
		{
			btnSelectDriver.DisableServerSideValidation();

			if (Page.IsValid)
			{
				int resourceId = Convert.ToInt32(cboDriver.SelectedValue);
				bool alreadySelected = false;

				// Make sure each driver is only present once
				foreach (Entities.Driver thisDriver in m_drivers)
				{
					if (thisDriver.ResourceId == resourceId)
						alreadySelected = true;
				}

				if (!alreadySelected)
				{
					// Retrieve the driver
					Facade.IDriver facDriver = new Facade.Resource();
					Entities.Driver driver = facDriver.GetDriverForResourceId(resourceId);

					// Add the driver to the collection and refresh the viewstate
					m_drivers.Add(driver);
					ViewState[C_DRIVER_COLLECTION] = m_drivers;
				}

				// Reset the dbcombo
				cboDriver.Text = "";
				cboDriver.SelectedValue = "";

				// Rebind the drivers
				BindDrivers();
			}
		}

		private void btnViewReport_Click(object sender, EventArgs e)
		{
			btnViewReport.DisableServerSideValidation();

			if (Page.IsValid)
			{
				// Retrieve the values to report on
				int[] resources = new int[m_drivers.Count];
				for (int driverIndex = 0; driverIndex < m_drivers.Count; driverIndex++)
					resources[driverIndex] = m_drivers[driverIndex].ResourceId;
                DateTime startDate = dteStartDate.SelectedDate.Value.Subtract(dteStartDate.SelectedDate.Value.TimeOfDay);
                DateTime endDate = dteEndDate.SelectedDate.Value.Subtract(dteEndDate.SelectedDate.Value.TimeOfDay).Add(new TimeSpan(23, 59, 59));

				// Get the data needed to run the report
				Facade.IKPI facKPI = new Facade.KPI();
				eKPITrendPeriod selectedPeriod = (eKPITrendPeriod) Enum.Parse(typeof(eKPITrendPeriod), cboTrendPeriod.SelectedValue.Replace(" ", ""));
				DataSet ds = facKPI.GetDriverPerformanceTrend(resources, startDate, endDate, selectedPeriod);

				Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.DriverPerformanceAnalysisTrend;
				Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = ds;
				Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
				Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";

				NameValueCollection nvc = new NameValueCollection();
				nvc.Add("ReportTitle", "KPI Performance (Trend)");
				nvc.Add("NumberOfDrivers", m_drivers.Count.ToString());
				nvc.Add("TrendSplit", selectedPeriod.ToString());
				nvc.Add("StartDate", dteStartDate.SelectedDate.Value.ToString("dd/MM/yy"));
				nvc.Add("EndDate", dteEndDate.SelectedDate.Value.ToString("dd/MM/yy"));

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

		#region DataGrid Events

		private void dgDrivers_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			switch (e.CommandName.ToLower())
			{
				case "removedriver":
					int resourceId = Convert.ToInt32(e.Item.Cells[0].Text);

					for (int driverIndex = 0; driverIndex < m_drivers.Count; driverIndex++)
					{
						Entities.Driver thisDriver = m_drivers[driverIndex];

						if (thisDriver.ResourceId == resourceId)
						{
							m_drivers.RemoveAt(driverIndex);
							ViewState[C_DRIVER_COLLECTION] = m_drivers;
							BindDrivers();
						}
					}
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
			this.Init += new System.EventHandler(this.Trend_Init);

		}
		#endregion
	}
}
