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

namespace Orchestrator.WebUI.Job
{
	/// <summary>
	/// Summary description for jobSearch.
	/// </summary>
	public partial class jobSearch : Orchestrator.Base.BasePage
	{
		#region Form Elements


		#endregion

		#region Page Variables

		private		ArrayList			m_jobStates;
		private		string				m_searchString = String.Empty;
		private		DataSet				m_dsJobsData;
		private		string				C_INVALID_SEARCH_STRING_VS = ".";
		private		eJobSearchField		m_jobSearchField;

		protected	int					m_singleJobId = 0;

		#endregion

		#region Property Interfaces

		private string SortCriteria
		{
			get { return (string) ViewState["sortCriteria"]; }
			set { ViewState["sortCriteria"] = value; }
		}

		private string SortDirection
		{
			get { return (string) ViewState["sortDirection"]; }
			set { ViewState["sortDirection"] = value; }
		}

		#endregion

		#region Page Load/Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

            // Search Field
            m_searchString = Request.QueryString["searchString"];
            txtSearchFor.Text = m_searchString;
            lblSearchExpression.Text = m_searchString;



            // Job State Check Boxes
            GenerateStates(Request.QueryString["state"]);

            // Search Field
            m_jobSearchField = eJobSearchField.All;
            try
            {
                if (Request.QueryString["field"] != null)
                    m_jobSearchField = (eJobSearchField)int.Parse(Request.QueryString["field"]);
            }
            catch { }
            PopulateStaticControls();

            // Start Date
            dteStartDate.DateInput.Text = string.Empty;
            if (Request.QueryString["from"] != null)
            {
                dteStartDate.SelectedDate = System.DateTime.Parse(Request.QueryString["from"]);
            }
                

            // End Date
            dteEndDate.DateInput.Text = string.Empty;
            if (Request.QueryString["to"] != null)
                dteEndDate.SelectedDate = System.DateTime.Parse(Request.QueryString["to"]);

            // User is searching from the "fast-search" (top-right of screen).
            if (Request.QueryString["filterDates"] == "on") // Show this weeks.
            {
                dteStartDate.SelectedDate = (DateTime.Now.AddDays(-((int)DateTime.Today.DayOfWeek - (int)DayOfWeek.Sunday)));
                dteEndDate.SelectedDate = DateTime.Now.AddDays((int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Saturday);
            }
            if (Request.QueryString["filterDates"] == "undefined") // Show the last month.
            {
                dteStartDate.SelectedDate = DateTime.Today.AddDays(-90);
                dteEndDate.SelectedDate = DateTime.Now.AddDays((int)DateTime.Today.DayOfWeek + (int)DayOfWeek.Saturday);
            }
            dgJobs.GroupBy = "JobState ASC";

            BindJobData();
            dgJobs.DataBind();
		}

		private void PopulateStaticControls()
		{
			cboJobSearchField.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eJobSearchField)));
			cboJobSearchField.DataBind();
			cboJobSearchField.ClearSelection();
			cboJobSearchField.Items.FindByText(Utilities.UnCamelCase(m_jobSearchField.ToString())).Selected = true;

			// Display the job state legend
			foreach (HtmlTableRow row in tblLegend.Rows)
				foreach (HtmlTableCell cell in row.Cells)
				{
					try
					{
						eJobState jobState = (eJobState) Enum.Parse(typeof(eJobState), cell.InnerText.Replace(" ", ""), true);
						cell.BgColor = Utilities.GetJobStateColour(jobState).Name;
					}
					catch {}
				}

            txtSearchFor.Text = m_searchString;
		}

		private void jobSearch_Init(object sender, EventArgs e)
		{
            dgJobs.ItemContentCreated += new ComponentArt.Web.UI.Grid.ItemContentCreatedEventHandler(dgJobs_ItemContentCreated);
            dlgAddUpdateDriver.DialogCallBack += new EventHandler(dlgAddUpdateDriver_DialogCallBack);
            dgJobs.GroupCommand += new ComponentArt.Web.UI.Grid.GroupCommandEventHandler(dgJobs_GroupCommand);
            dgJobs.NeedDataSource += new ComponentArt.Web.UI.Grid.NeedDataSourceEventHandler(dgJobs_NeedDataSource);
            dgJobs.NeedRebind += new ComponentArt.Web.UI.Grid.NeedRebindEventHandler(dgJobs_NeedRebind);
            dgJobs.SortCommand += new ComponentArt.Web.UI.Grid.SortCommandEventHandler(dgJobs_SortCommand);
            dgJobs.PageIndexChanged += new ComponentArt.Web.UI.Grid.PageIndexChangedEventHandler(dgJobs_PageIndexChanged);
            this.btnSearch.OnClientClick = "return Search();"; 
		}

        void dlgAddUpdateDriver_DialogCallBack(object sender, EventArgs e)
        {
            BindJobData(); 
        }

        void dgJobs_PageIndexChanged(object sender, ComponentArt.Web.UI.GridPageIndexChangedEventArgs e)
        {
            dgJobs.CurrentPageIndex = e.NewIndex;
        }

        void dgJobs_NeedRebind(object sender, EventArgs e)
        {
            dgJobs.DataBind();
        }

        void dgJobs_SortCommand(object sender, ComponentArt.Web.UI.GridSortCommandEventArgs e)
        {
            dgJobs.Sort = e.SortExpression;
        }

        void dgJobs_NeedDataSource(object sender, EventArgs e)
        {
            BindJobData();
            dgJobs.DataSource = m_dsJobsData;
        }

        void dgJobs_GroupCommand(object sender, ComponentArt.Web.UI.GridGroupCommandEventArgs e)
        {
            dgJobs.GroupBy = e.GroupExpression;
        }

		#endregion

        #region Data Binding
		private void BindJobData()
		{
			DateTime start = DateTime.MinValue;
			DateTime end = DateTime.MinValue;

            if ((!string.IsNullOrEmpty(Request.QueryString["chkFilterDates"]) && Request.QueryString["chkFilterDates"].ToLower() != "false") || !dteStartDate.IsEmpty || !dteEndDate.IsEmpty)
            {
                // We should be using the dates specified (or default dates).
                if (!dteStartDate.IsEmpty)
                {
                    // Use the date supplied
                    start = dteStartDate.SelectedDate.Value;
                    start = start.Subtract(start.TimeOfDay);
                }

                if (!dteEndDate.IsEmpty)
                {
                    // Use the date supplied
                    end = dteEndDate.SelectedDate.Value;
                    end = end.Subtract(end.TimeOfDay);
                }

                if (start != DateTime.MinValue && end == DateTime.MinValue)
                {
                    // If we've only specified a start date, make the end date the end of that day.
                    end = start.Subtract(start.TimeOfDay);
                    end = end.Add(new TimeSpan(0, 23, 59, 59));
                }
                else
                    if (start == DateTime.MinValue && end == DateTime.MinValue)
                    {
                        // If we haven't specified any dates set to the last week.
                        end = DateTime.UtcNow;
                        end = end.Subtract(end.TimeOfDay);
                        end = end.Add(new TimeSpan(0, 23, 59, 59));

                        start = end.Subtract(new TimeSpan(7, 23, 59, 59));
                    }
                    else
                    {
                        end = end.Add(new TimeSpan(0, 23, 59, 59));
                    }
            }
            else
            {
                start = DateTime.MinValue;
                end = DateTime.MaxValue;
            }

            if (start != DateTime.MinValue)
            {
                dteStartDate.SelectedDate = start;
                dteEndDate.SelectedDate = end;
            }

			BindJobData(start, end);
		}

		private void BindJobData(DateTime startDate, DateTime endDate)
		{
			StringBuilder sb = new StringBuilder();
			foreach (eJobState jobState in m_jobStates)
			{
				if (sb.Length > 0)
					sb.Append(",");
				sb.Append(((int) jobState).ToString());
			}
			string jobStatesCSV = sb.ToString();

			// Refilter the jobs.
			
			if (m_searchString == null || m_searchString == string.Empty || m_searchString == C_INVALID_SEARCH_STRING_VS)
			{
				lblResultCount.Text = "0";
			}
			else
			{
				Facade.IJob facJob = new Facade.Job();
                switch (m_searchString.ToUpper().Substring(0, 1))
                {
                    case "T":
                        // Trailer only search.
                        string trailerReference = m_searchString.Substring(1);
                        try
                        {
                            // The rest of the text is a number.
                            int trailerId = int.Parse(trailerReference);

                            m_jobSearchField = eJobSearchField.TrailerReference;
                            cboJobSearchField.ClearSelection();
                            cboJobSearchField.Items.FindByText(Utilities.UnCamelCase(Enum.GetName(typeof(eJobSearchField), eJobSearchField.TrailerReference))).Selected = true;
                            // Remove the preceeding T.
                            m_searchString = m_searchString.Substring(1);
                        }
                        catch { }
                        break;
                }

                // Support searching for orders
                if (m_searchString.Length > 2 && m_searchString.Substring(0, 2).ToLower() == "o:")
                {
                    m_jobSearchField = eJobSearchField.OrderID;
                    m_searchString = m_searchString.Substring(2);
                }

                // Support searching for Dehire Note Numbers (Receipt Numbers)
                if (m_searchString.Length > 2 && m_searchString.Substring(0, 2).ToLower() == "d:")
                {
                    m_jobSearchField = eJobSearchField.DehireReceiptNumber;
                    m_searchString = m_searchString.Substring(2);
                }

                m_dsJobsData = facJob.FindJob(m_searchString, m_jobSearchField, jobStatesCSV, startDate, endDate, ((Entities.CustomPrincipal) Page.User).IdentityId);

				if (m_dsJobsData != null && m_dsJobsData.Tables.Count != 0)
				{
					lblResultCount.Text = m_dsJobsData.Tables[0].Rows.Count.ToString();

                    dgJobs.DataSource = m_dsJobsData;
                    dgJobs.UnSelectAll();
                    //dgJobs.GroupBy = "JobState"; 
                    if (m_dsJobsData.Tables[0].Rows.Count == 1)
                    {
                        m_singleJobId = (int)m_dsJobsData.Tables[0].Rows[0]["JobId"];
                        dgJobs.PreExpandOnGroup = true;
                    }
                    else
                        dgJobs.PreExpandOnGroup = false;
                }
				else
				{
					lblResultCount.Text = "0";
					dgJobs.Visible = false;
				}

                // hide the filter display
                this.ClientScript.RegisterStartupScript(this.GetType(), "hideFilters", "FilterOptionsDisplayHide();", true);
			}
        }
#endregion

        #region Template Helpers
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

		private ArrayList CreateTableRows(eJobType jobType, DataRow data)
		{
			ArrayList rows = new ArrayList();

			#region Top row (Location, Timings, Resources, Pallets, Weight)

			TableRow row = new TableRow();

			rows.Add(row);

			// Location
			TableCell location = new TableCell();
			if (data["InstructionActualId"] != DBNull.Value)
				location.Text = "<img src=\"..\\images\\tick.gif\" alt=\"Completed\" style=\"VERTICAL-ALIGN: -3px;\">&nbsp;";
			Uri pointAddressUri = new Uri(Request.Url, "../Point/GetPointAddressHtml.aspx");
			location.Text += "<span onMouseOver=\"ShowPoint('" + pointAddressUri.ToString() + "', '" + ((int) data["PointId"]).ToString() + "')\" onMouseOut=\"HidePoint()\">";
			location.Text += (string) data["Location"];
			location.Text += "</span>";
			location.Width = new Unit("100px");
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

            #region Dockets

            // Dockets row
            TableRow dockets = new TableRow();
            rows.Add(dockets);

            TableCell docketsCell = new TableCell();
            docketsCell.ColumnSpan = row.Cells.Count;
            dockets.Cells.Add(docketsCell);

            if (jobType == eJobType.Return)
            {
                // return jobs need better information than just the docket information.
                docketsCell.Attributes.Add("onMouseOver", "javascript:ReturnsForInstructionId(true, " + ((int)data["InstructionId"]).ToString() + ");");
                docketsCell.Attributes.Add("onMouseOut", "javascript:ReturnsForInstructionId(false, 0);");
                docketsCell.Text = "Mouseover for returns information";
                docketsCell.Style["cursor"] = "hand";
            }
            else
                docketsCell.Text = "<b>" + (string)data["Dockets"] + "</b>";

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
        #endregion

		private void GenerateStates(string stateQS)
		{
			m_jobStates = new ArrayList();
            if (stateQS == null || stateQS.Length == 0)
                stateQS = "1,2,3,4,5,6,7,8";

			string[] states = stateQS.Split(',');

			chkJobStates.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eJobState)));
			chkJobStates.DataBind();
			chkJobStates.ClearSelection();

			for (int index = 0; index < states.Length; index++)
			{
				try
				{
					int jobStateId = Convert.ToInt32(states[index]);

					if (jobStateId == 0)
					{
						foreach (ListItem item in chkJobStates.Items)
						{
							eJobState selectedState = (eJobState) Enum.Parse(typeof(eJobState), item.Text.Replace(" ", ""));
                            if (selectedState == eJobState.Booked || selectedState == eJobState.Planned || selectedState == eJobState.InProgress || selectedState == eJobState.Completed)
                            {
                                m_jobStates.Add(selectedState);

                                item.Selected = true;
                            }
						}
						break;
					}
					else
					{
						eJobState selectedState = (eJobState) jobStateId;
						m_jobStates.Add(selectedState);
					
						chkJobStates.Items.FindByValue(Utilities.UnCamelCase(selectedState.ToString())).Selected = true;
					}
				}
				catch {}
			}
		}

		#region DataGrid Event Handlers

        void dgJobs_ItemContentCreated(object sender, ComponentArt.Web.UI.GridItemContentCreatedEventArgs e)
        {
            
            if (m_dsJobsData != null)
            {
                int jobId = Convert.ToInt32(e.Item["JobId"]);
                eJobType jobType = (eJobType)Enum.Parse(typeof(eJobType), ((string)e.Item["JobType"]).Replace(" ", ""));
                HtmlAnchor lnkManageJob = (HtmlAnchor)e.Content.FindControl("lnkManageJob");
                Table tblCollections = (Table)e.Content.FindControl("tblCollections");
                Table tblDeliveries = (Table)e.Content.FindControl("tblDeliveries");

                if (lnkManageJob != null)
                {
                    HtmlImage imgRequiresCallIn = (HtmlImage)e.Content.FindControl("imgRequiresCallIn");
                    HtmlImage imgHasRequests = (HtmlImage)e.Content.FindControl("imgHasRequests");
                    HtmlImage imgHasNewPCVs = (HtmlImage)e.Content.FindControl("imgHasNewPCVs");
                    HtmlImage imgHasExtras = (HtmlImage)e.Content.FindControl("imgHasExtras");
                    HtmlImage imgMarkedForCancellation = (HtmlImage)e.Content.FindControl("imgMarkedForCancellation");
                    HtmlImage imgHasPCVAttached = (HtmlImage)e.Content.FindControl("imgHasPCVAttached");
                    HtmlImage imgHasDehireReceipt = (HtmlImage)e.Content.FindControl("imgHasDehireReceipt");

                    // Set the manage job link.
                    if (Orchestrator.Globals.Configuration.UseJobManagementLink)
                        lnkManageJob.HRef = "javascript:OpenJob(" + jobId.ToString() + ");";
                    else
                        lnkManageJob.HRef = "javascript:OpenJobDetails(" + jobId.ToString() + ");";

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

                    imgHasExtras.Visible = ((bool)e.Item["ExtraId"]);
                    imgHasPCVAttached.Visible = ((int)e.Item["HasPCVAttached"]) > 0 ? true : false;
                    imgHasDehireReceipt.Visible = ((int)e.Item["HasDehireReceipt"]) > 0 ? true : false;

                    // Display the Marked for Cancellation if required.
                    if ((bool)(e.Item["ForCancellation"]))
                    {
                        imgMarkedForCancellation.Visible = true;
                        imgMarkedForCancellation.Alt = "Marked for cancellation: " + (string)e.Item["ForCancellationReason"];
                    }
                    else
                        imgMarkedForCancellation.Visible = false;
                }

                if (tblCollections != null)
                {
                    DataView dvCollections = new DataView(m_dsJobsData.Tables[1]);
                    foreach (DataRow collection in dvCollections.Table.Rows)
                    {
                        if ((int)collection["JobId"] == jobId && ((int)collection["InstructionTypeID"] != (int)eInstructionType.Trunk || jobType == eJobType.Groupage))
                        {
                            // This is a collection for the current job
                            ArrayList rows = CreateTableRows(jobType, collection);
                            foreach (TableRow row in rows)
                                tblCollections.Rows.Add(row);
                        }
                    }
                }

                if (tblDeliveries != null)
                {
                    DataView dvDeliveries = new DataView(m_dsJobsData.Tables[2]);
                    foreach (DataRow delivery in dvDeliveries.Table.Rows)
                    {
                        if ((int)delivery["JobId"] == jobId && ((int)delivery["InstructionTypeID"] != (int)eInstructionType.Trunk || jobType == eJobType.Groupage))
                        {
                            // This is a delivery for the current job
                            ArrayList rows = CreateTableRows(jobType, delivery);
                            foreach (TableRow row in rows)
                                tblDeliveries.Rows.Add(row);
                        }
                    }
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
			this.Init += new EventHandler(jobSearch_Init);
		}
		#endregion
	}
}
