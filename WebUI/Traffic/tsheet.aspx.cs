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
using System.Xml;
using System.Xml.Xsl;

using System.Collections.Generic;
using System.Xml.Serialization;


using P1TP.Components.Web.UI;

using System.Xml.Serialization;

namespace Orchestrator.WebUI.Traffic
{
	/// <summary>
	/// Summary description for tsheet.
	/// </summary>
	public partial class tsheet : Orchestrator.Base.BasePage
	{
		#region Constants & Enums

        private readonly string C_TRAFFIC_SHEET_XML = "TrafficSheetFilterXML";

		private const string	C_JOB_VS	= "C_JOB_VS";
		private const string	C_FILTER_VS = "C_FILTER_VS";

		private enum eJobSheetMode {Normal, Planning, Completed};

		#endregion

		#region Page Variables

		private bool			m_isJobSheet = false;
		private eJobSheetMode	m_jobSheetMode = eJobSheetMode.Normal;

		private DataSet			m_dsJobsData;

		protected	int			m_clientId = 0;
		protected	int			m_townId = 0;
		protected	int			m_pointId = 0;

		protected	int			m_selectPointClientId = 0;
		protected	int			m_selectPointTownId = 0;
		protected	string		m_selectPointTownName = string.Empty;
		protected	int			m_selectPointPointId = 0;

		//private		bool		m_foundFirstIncompleteLeg = false;

		private		int			m_myJobCount = 0;
		private		ArrayList	m_pencilledInDrivers = new ArrayList();
		private		ArrayList	m_pencilledInVehicles = new ArrayList();
		private		ArrayList	m_pencilledInTrailers = new ArrayList();

        protected Entities.TrafficSheetFilter m_trafficSheetFilter = null;
       
        private eTrafficSheetGrouping m_Grouping = eTrafficSheetGrouping.Organisation;
       
		#endregion

        protected string TrafficAreas
        {
            get { return Entities.Utilities.CommaSeparatedIDs(m_trafficSheetFilter.TrafficAreaIDs); }
        }

		#region Page Load/Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

			#region Configure the job sheet mode if applicable

			if (Request.QueryString["isJobSheet"] != null)
			{
				m_isJobSheet = Request.QueryString["isJobSheet"].ToLower() == "true";
				if (m_isJobSheet && Request.QueryString["jobSheetMode"] != null)
				{
					try
					{
						m_jobSheetMode = (eJobSheetMode) Enum.Parse(typeof(eJobSheetMode), Request.QueryString["jobSheetMode"], true);
					}
					catch {}
				}
			}

			#endregion

			if (!IsPostBack)
				LoadDefaultFilter();

			lblError.Visible = false;

			if (m_isJobSheet)
			{
                Page.Title = "Haulier Enterprise";
                /*
				switch (m_jobSheetMode)
				{
					case eJobSheetMode.Completed:
                        Page.Title = "Completed Job Report";
						break;
					case eJobSheetMode.Normal:
                        Page.Title = "Run Sheet";
						break;
					case eJobSheetMode.Planning:
                        Page.Title = "Planning Report";
						break;
				}*/
			}
			else
                Page.Title = "Daily Traffic Sheet";

		}

		private void tsheet_Init(object sender, EventArgs e)
		{
            dgBasicJobs.ItemContentCreated += new ComponentArt.Web.UI.Grid.ItemContentCreatedEventHandler(dgBasicJobs_ItemContentCreated);
			btnAddPlannerRequest.Click += new EventHandler(btnAddPlannerRequest_Click);
			btnUpdateOwnership.Click += new EventHandler(btnUpdateOwnership_Click);
            this.imbRefresh.Click += new ImageClickEventHandler(imbRefresh_Click);
            this.dlgFilter.DialogCallBack += new EventHandler(dlgFilter_DialogCallBack);
		}

        void dlgFilter_DialogCallBack(object sender, EventArgs e)
        {
            LoadDefaultFilter();
            m_trafficSheetFilter = Utilities.GetFilterFromCookie(this.CookieSessionID, Request);
            BindData(m_trafficSheetFilter);
        }

        void imbRefresh_Click(object sender, ImageClickEventArgs e)
        {
            LoadDefaultFilter();   
        }

    	#endregion

        #region Viewstate Handling (keep it on the server)

      

        #endregion

        #region Filter Handling

       
       
        #endregion

        #region Table Binding

        private void BindData(Entities.TrafficSheetFilter filter)
        {
            BindJobData(filter.TrafficSheetGrouping);
        }

        private void BindData()
        {

            eTrafficSheetGrouping grouping = eTrafficSheetGrouping.Organisation;

            BindJobData(grouping);
        }

		private void BindJobData()
		{
           	BindJobData(eTrafficSheetGrouping.None);
		}

        private void UseDefaultFilter()
        {
            Facade.ITrafficSheetFilter facTrafficSheetFilter = new Facade.Traffic();
            m_trafficSheetFilter = facTrafficSheetFilter.GetDefault(((Entities.CustomPrincipal)Page.User).IdentityId);

            
            // Configure the default dates.
            // Default dates are from the start of today until:
            //   1) On a Saturday, until the end of Monday.
            //   2) On any other day, until the end of tomorrow.
            DateTime startOfToday = DateTime.Now;
            startOfToday = startOfToday.Subtract(startOfToday.TimeOfDay);
            DateTime endOfTomorrow = startOfToday.Add(new TimeSpan(1, 23, 59, 59));

            m_trafficSheetFilter.FilterStartDate = startOfToday;
            if (startOfToday.DayOfWeek == DayOfWeek.Saturday)
            {
                DateTime endOfMonday = startOfToday.Add(new TimeSpan(2, 23, 59, 59));
                m_trafficSheetFilter.FilterEnddate = endOfMonday;
            }
            else
                m_trafficSheetFilter.FilterEnddate = endOfTomorrow;

            BindJobData();
        }

		private void BindJobData(eTrafficSheetGrouping grouping)
		{
            var trafficAreaIDs = m_trafficSheetFilter.TrafficAreaIDs.ToArray();
            
            var jobStates = new int[m_trafficSheetFilter.JobStates.Count];
            for (int i = 0; i < m_trafficSheetFilter.JobStates.Count; i++)
            {
                if (m_trafficSheetFilter.JobStates[i] is eJobState) 
                    jobStates[i] = (int)(eJobState) m_trafficSheetFilter.JobStates[i];
                else
                    jobStates[i] = (int) (long) m_trafficSheetFilter.JobStates[i];
            }

            // Refilter the jobs normally
			using (Facade.IJob facJob = new Facade.Job())
				m_dsJobsData = facJob.GetJobsForControlAreaPeriod(m_trafficSheetFilter.ControlAreaId, trafficAreaIDs, jobStates, false, m_trafficSheetFilter.OnlyShowMyJobs, m_trafficSheetFilter.OnlyShowJobsWithPCVs, m_trafficSheetFilter.OnlyShowJobsWithDemurrage, m_trafficSheetFilter.OnlyShowJobsWithDemurrageAwaitingAcceptance, ((Entities.CustomPrincipal) Page.User).IdentityId, m_trafficSheetFilter.FilterStartDate, m_trafficSheetFilter.FilterEnddate, m_trafficSheetFilter.BusinessTypes);

			// Sort and Bind
			DataView dvJobsData = new DataView(m_dsJobsData.Tables[0]);
			
			switch (grouping)
			{
				case eTrafficSheetGrouping.Organisation:
					dgBasicJobs.GroupBy ="OrganisationName";
					break;
				case eTrafficSheetGrouping.DepotCode:
					dgBasicJobs.GroupBy = "DepotCode";
					break;
                default:
                    dgBasicJobs.GroupBy = "OrganisationName";
                    break;
			}

			//if (((SortCriteria + " " + SortDirection).Trim().Length > 0) && (SortCriteria.StartsWith(dgBasicJobs.GroupingColumn)))
//				dvJobsData.Sort = SortCriteria + " " + SortDirection;
//			else
//				dvJobsData.Sort = dgBasicJobs.GroupingColumn + ", JobStartsAt";

			dgBasicJobs.DataSource = dvJobsData;
			dgBasicJobs.DataBind();

			// Display job count
			lblJobCount.Visible = true;
			lblJobCount.Text = "You have <b>" + dvJobsData.Table.Rows.Count + "</b> run" + (dvJobsData.Table.Rows.Count == 1 ? "" : "s") + " displayed.";
        }
        #endregion

        private string CellOutputFromArrayList(ArrayList list)
		{
			StringBuilder sb = new StringBuilder();
			
			foreach (string val in list)
			{
				if (sb.Length > 0)
					sb.Append("<br>");
				sb.Append("<li>");
				sb.Append(val);
			}

			return sb.ToString();
		}

		private string CellOutputFromArrayList(ArrayList drivers, ArrayList vehicles, ArrayList trailers)
		{
			StringBuilder sb = new StringBuilder();

			foreach (string val in drivers)
			{
				if (sb.Length > 0)
					sb.Append(", ");
				sb.Append(val);
			}

			foreach (string val in vehicles)
			{
				if (sb.Length > 0)
					sb.Append(", ");
				sb.Append(val);
			}

			foreach (string val in trailers)
			{
				if (sb.Length > 0)
					sb.Append(", ");
				sb.Append(val);
			}

			return sb.ToString();
		}

		private void ConfigureDefaultFilter()
		{

            Facade.ITrafficSheetFilter facFilters = new Facade.Traffic();
            m_trafficSheetFilter = facFilters.GetDefault(((Entities.CustomPrincipal)Page.User).IdentityId);

			if (m_isJobSheet && m_jobSheetMode != eJobSheetMode.Normal)
			{
				switch (m_jobSheetMode)
				{
					case eJobSheetMode.Planning:
                        m_trafficSheetFilter.JobStates.Add(eJobState.Booked);
						break;
					case eJobSheetMode.Completed:
                        m_trafficSheetFilter.JobStates.Add(eJobState.Completed);
                        m_trafficSheetFilter.JobStates.Add(eJobState.BookingInComplete);
                        m_trafficSheetFilter.JobStates.Add(eJobState.BookingInIncomplete);
                        m_trafficSheetFilter.JobStates.Add(eJobState.ReadyToInvoice);
                        m_trafficSheetFilter.JobStates.Add(eJobState.Invoiced);
                        break;
				}
			}
			else
			{
                m_trafficSheetFilter.JobStates.Add(eJobState.Booked);
                m_trafficSheetFilter.JobStates.Add(eJobState.Planned);
                m_trafficSheetFilter.JobStates.Add(eJobState.InProgress);
			}

			BindData();
		}

		private ArrayList CreateTableRows(DataRow data)
		{
			ArrayList rows = new ArrayList();

			#region Top row (Location, Timings, Resources, Pallets, Weight)

			TableRow row = new TableRow();

			rows.Add(row);
            if (data.Table.Columns.Contains("HasExtra") &&  (int)data["HasExtra"] == 1)
                row.Attributes.Add("HasExtra", "true");

			// Location
			TableCell location = new TableCell();
			if (data["InstructionActualId"] != DBNull.Value)
				location.Text = "<img src=\"..\\images\\tick.gif\" alt=\"Completed\" style=\"VERTICAL-ALIGN: -3px;\">&nbsp;";
			Uri pointAddressUri = new Uri(Request.Url, "../Point/GetPointAddressHtml.aspx");
			location.Text += "<span onMouseOver=\"ShowPoint('" + pointAddressUri.ToString() + "', '" + ((int) data["PointId"]).ToString() + "')\" onMouseOut=\"HidePoint()\">";
			location.Text += (string) data["Location"];
			location.Text += "</span>";
			location.Text += "<br><b>" + (string) data["Dockets"] + "</b>";
            location.Width = Unit.Pixel(250);
            location.Wrap = false;
			row.Cells.Add(location);

			// Timing
			TableCell timing = new TableCell();
			if ((bool) data["Anytime"])
				timing.Text = ((DateTime) data["By"]).ToString("dd/MM/yy") + " Anytime";
			else
				timing.Text = ((DateTime) data["By"]).ToString("dd/MM/yy HH:mm");
			timing.Width = new Unit("100px");
			row.Cells.Add(timing);

			// Resources
			TableCell resources = new TableCell();
            if (data["SubContractor"] != DBNull.Value)
                resources.Text = "<b>" + (string)data["SubContractor"] + "</b><br>";
			if (data["FullName"] != DBNull.Value)
				resources.Text = "<b>" + (string) data["FullName"] + "</b><br>";
			if (data["RegNo"] != DBNull.Value)
				resources.Text += "<b>" + (string) data["RegNo"] + "</b><br>";
			if (data["TrailerRef"] != DBNull.Value)
				resources.Text += "<b>" + (string) data["TrailerRef"] + "</b><br>";
			if (resources.Text.Length == 0)
				resources.Text = "&nbsp;";
			resources.Width = new Unit("150px");
			row.Cells.Add(resources);

			foreach (TableCell cell in row.Cells)
			{
				cell.VerticalAlign = VerticalAlign.Top;
				cell.HorizontalAlign = HorizontalAlign.Left;
			}

			// Pallets (delivery only)
			if (data.Table.Columns.Contains("Pallets"))
			{
				TableCell pallets = new TableCell();
				if (data["Pallets"] != DBNull.Value)
					pallets.Text = ((int) data["Pallets"]).ToString("F0");
				else
					pallets.Text = "0";
				pallets.Width = new Unit("30px");
				pallets.VerticalAlign = VerticalAlign.Top;
				pallets.HorizontalAlign = HorizontalAlign.Right;
				row.Cells.Add(pallets);
			}

			// Weight (delivery only)
			if (data.Table.Columns.Contains("Weight"))
			{
				TableCell weight = new TableCell();
				if (data["Weight"] != DBNull.Value)
                    weight.Text = ((decimal) data["Weight"]).ToString("F0");
				else
					weight.Text = "0";
				weight.Width = new Unit("30px");
				weight.VerticalAlign = VerticalAlign.Top;
				weight.HorizontalAlign = HorizontalAlign.Right;
				row.Cells.Add(weight);
			}

			#endregion

			#region Notes
			
			if (data["Note"] != DBNull.Value && ((string) data["Note"]).Length > 0)
			{
				TableRow notes = new TableRow();
				rows.Add(notes);

				// Note
				TableCell space = new TableCell();
				space.Text = "&nbsp;";
				notes.Cells.Add(space);
				TableCell note = new TableCell();
				note.Text = (string) data["Note"];
				note.ColumnSpan = row.Cells.Count - 1;
				notes.Cells.Add(note);
			}

			#endregion

			return rows;
		}

		private void LoadDefaultFilter()
		{
			int identityId = ((Entities.CustomPrincipal) Page.User).IdentityId;

			Facade.ITrafficSheetFilter facTrafficSheetFilter = new Facade.Traffic();
            m_trafficSheetFilter = Utilities.GetFilterFromCookie(this.CookieSessionID, Request);
            if (m_trafficSheetFilter == null)
            {
                m_trafficSheetFilter = facTrafficSheetFilter.GetDefault(identityId);
            }

            if (m_trafficSheetFilter.LastUpdated == DateTime.MinValue || DateTime.Today.Subtract(m_trafficSheetFilter.LastUpdated).Days >= 1)
            {
                // Configure the default dates.
                // Default dates are from the start of today until:
                //   1) On a Saturday, until the end of Monday.
                //   2) On any other day, until the end of tomorrow.
                DateTime startOfToday = DateTime.Now;
                startOfToday = startOfToday.Subtract(startOfToday.TimeOfDay);
                DateTime endOfTomorrow = startOfToday.Add(new TimeSpan(1, 23, 59, 59));

                m_trafficSheetFilter.FilterStartDate = startOfToday;
                if (startOfToday.DayOfWeek == DayOfWeek.Saturday)
                {
                    DateTime endOfMonday = startOfToday.Add(new TimeSpan(2, 23, 59, 59));
                    m_trafficSheetFilter.FilterEnddate = endOfMonday;
                }
                else
                    m_trafficSheetFilter.FilterEnddate = endOfTomorrow;

                m_trafficSheetFilter.LastUpdated = DateTime.Now;
            }
            
            if (m_jobSheetMode == eJobSheetMode.Completed || m_jobSheetMode == eJobSheetMode.Planning)
                ConfigureDefaultFilter();
          
            BindData(m_trafficSheetFilter);
		}
		
        #region Button Event Handlers

        private void btnAddPlannerRequest_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddUpdatePlannerRequest.aspx");
        }

        private void btnUpdateOwnership_Click(object sender, EventArgs e)
        {
            StringBuilder sbPickUpJobs = new StringBuilder();
            StringBuilder sbDropJobs = new StringBuilder();

            foreach (DataGridItem item in dgBasicJobs.Items)
            {
                ListItemType itemType = item.ItemType;

                if (itemType == ListItemType.Item || itemType == ListItemType.AlternatingItem || itemType == ListItemType.SelectedItem)
                {
                    try
                    {
                        string jobIdText = ((HtmlInputHidden)item.FindControl("hidJobId")).Value;
                        int jobId = Convert.ToInt32(jobIdText);

                        CheckBox chkOwnership = (CheckBox)item.FindControl("chkOwnership");

                        if (chkOwnership.Checked)
                        {
                            if (sbPickUpJobs.Length > 0)
                                sbPickUpJobs.Append(",");
                            sbPickUpJobs.Append(jobId);
                        }
                        else
                        {
                            if (sbDropJobs.Length > 0)
                                sbDropJobs.Append(",");
                            sbDropJobs.Append(jobId);
                        }
                    }
                    catch { }
                }
            }

            if (sbPickUpJobs.Length > 0 || sbDropJobs.Length > 0)
            {
                Facade.IUser facUser = new Facade.User();
                facUser.ConfigureMyJobs(sbPickUpJobs.ToString(), sbDropJobs.ToString(), ((Entities.CustomPrincipal)Page.User).IdentityId);
            }

            BindData();
        }

        #endregion

		#region Pretty Data Grid Event Handlers

        void dgBasicJobs_ItemContentCreated(object sender, ComponentArt.Web.UI.GridItemContentCreatedEventArgs e)
        {
            if (m_dsJobsData != null)
            {
                int jobId = Convert.ToInt32(e.Item["JobId"]);
                HtmlAnchor lnkManageJob = (HtmlAnchor)e.Content.FindControl("lnkManageJob");
                Table tblCollections = (Table)e.Content.FindControl("tblCollections");
                Table tblDeliveries = (Table)e.Content.FindControl("tblDeliveries");

               

                if (tblCollections != null)
                {
                    DataView dvCollections = new DataView(m_dsJobsData.Tables[1]);
                    foreach (DataRow collection in dvCollections.Table.Rows)
                    {
                        if ((int)collection["JobId"] == jobId)
                        {
                            // This is a collection for the current job
                            ArrayList rows = CreateTableRows(collection);
                            foreach (TableRow row in rows)
                                tblCollections.Rows.Add(row);
                        }
                    }
                }

                // Now will include trunks if orders are attached.
                if (tblDeliveries != null)
                {
                    DataView dvDeliveries = new DataView(m_dsJobsData.Tables[2]);
                    foreach (DataRow delivery in dvDeliveries.Table.Rows)
                    {
                        if ((int)delivery["JobId"] == jobId)
                        {
                            // This is a delivery for the current job
                            ArrayList rows = CreateTableRows(delivery);
                            foreach (TableRow row in rows)
                            {
                                tblDeliveries.Rows.Add(row);
                            }
                            
                        }
                    }


                }

                if (lnkManageJob != null)
                {
                    HtmlImage imgRequiresCallIn = (HtmlImage)e.Content.FindControl("imgRequiresCallIn");
                    HtmlImage imgHasRequests = (HtmlImage)e.Content.FindControl("imgHasRequests");
                    HtmlImage imgHasNewPCVs = (HtmlImage)e.Content.FindControl("imgHasNewPCVs");
                    HtmlImage imgHasExtras = (HtmlImage)e.Content.FindControl("imgHasExtra");
                    HtmlImage imgHasPCVAttached = (HtmlImage)e.Content.FindControl("imgHasPCVAttached");
                    HtmlImage imgHasDehireReceipt = (HtmlImage)e.Content.FindControl("imgHasDehireReceipt");
                    HtmlImage imgHadPalletHandling = (HtmlImage)e.Content.FindControl("imgHadPalletHandling");
                    
                    // Set the manage job link.
                    if (Orchestrator.Globals.Configuration.UseJobManagementLink)
                        lnkManageJob.HRef = "javascript:openResizableDialogWithScrollbars('/traffic/jobmanagement.aspx?jobId=" + jobId.ToString() + "'+ getCSID(),'1220','870');";
                    else
                        lnkManageJob.HRef = "javascript:openResizableDialogWithScrollbars('/job/job.aspx?jobId=" + jobId.ToString() + "'+ getCSID(),'1220','870');";
                    lnkManageJob.InnerText = jobId.ToString();

                    // Display the Requires Call In icon if required.
                    imgRequiresCallIn.Visible = Convert.ToBoolean(e.Item["RequiresCallIn"]);

                    // Display the Has Request icon if required.
                    if (Convert.ToInt32(e.Item["Requests"]) == 0)
                        imgHasRequests.Visible = false;
                    else
                    {
                        imgHasRequests.Visible = true;
                        imgHasRequests.Attributes.Add("onClick", "javascript:ShowPlannerRequests('" + jobId.ToString() + "');");
                    }

                    // Display the Has New PCVs icon if required.
                    if (Convert.ToInt32(e.Item["IssuedPCVs"]) == 0)
                        imgHasNewPCVs.Visible = false;
                    else
                        imgHasNewPCVs.Visible = true;
                    

                    imgHasPCVAttached.Visible = ((int)e.Item["HasPCVAttached"]) > 0 ? true : false;
                    imgHasDehireReceipt.Visible = ((int)e.Item["HasDehireReceipt"]) > 0 ? true : false;
                    imgHasExtras.Visible = (bool)e.Item["HasExtra"];
                    imgHadPalletHandling.Visible = (bool)e.Item["HasPalletHandling"];
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
			this.Init += new EventHandler(tsheet_Init);
		}
		#endregion
	}
}
