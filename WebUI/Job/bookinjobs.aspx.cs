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

using ComponentArt.Web.UI;
using P1TP.Components.Web.UI;

namespace Orchestrator.WebUI.Job
{
	/// <summary>
	/// Displays a list of jobs which have not yet been marked as booking in or have not yet bee marked as "Booking In Complete"
	/// </summary>
	public partial class bookinjobs : Orchestrator.Base.BasePage
	{
		#region Form Elements

		//protected	PrettyDataGrid	dgJobs;

		#endregion

		#region Page Variables

		private DataSet		m_dsJobsData;

		#endregion

		#region Page Load/Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

			if (!IsPostBack)
				LoadData();
		}

		private void bookinjobs_Init(object sender, EventArgs e)
		{
			btnRefresh.Click += new EventHandler(btnRefresh_Click);

			//dgJobsToBookIn.ItemDataBound += new ComponentArt.Web.UI.Grid.ItemDataBoundEventHandler(dgJobsToBookIn_ItemDataBound);
		}

		#endregion

		private ArrayList CreateTableRows(DataRow data)
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
			location.Text += "<br><b>" + (string) data["Dockets"] + "</b>";
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
				resources.Text = (string) data["FullName"] + "<br>";
			if (data["RegNo"] != DBNull.Value)
				resources.Text += (string) data["RegNo"] + "<br>";
			if (data["TrailerRef"] != DBNull.Value)
				resources.Text += (string) data["TrailerRef"] + "<br>";
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

		private void LoadData()
		{
            using (Facade.IJob facJob = new Facade.Job())
                m_dsJobsData = facJob.GetBookInJobs(((Entities.CustomPrincipal) Page.User).IdentityId);

            dgJobsToBookIn.DataSource = m_dsJobsData;
            dgJobsToBookIn.PageSize = m_dsJobsData.Tables[0].Rows.Count;
            dgJobsToBookIn.DataBind();
		}

		#region Button Event Handlers

		private void btnRefresh_Click(object sender, EventArgs e)
		{
			LoadData();
		}

		#endregion

		#region DataGrid Event Handlers

        private void dgJobsToBookIn_ItemContentCreated(object sender, GridItemContentCreatedEventArgs e)
		{
			try
			{
				int jobId = Convert.ToInt32(((HtmlInputHidden) e.Content.FindControl("hidJobId")).Value);

				if (jobId > 0)
				{
					DataView dvJobRow = new DataView(m_dsJobsData.Tables[0]);
					dvJobRow.RowFilter = "JobId = " + jobId.ToString();
					eJobState jobState = (eJobState) Enum.Parse(typeof(eJobState), ((string) dvJobRow.Table.Rows[0]["JobState"]).Replace(" ", ""), true);

					HtmlInputHidden hidJobState = (HtmlInputHidden) e.Content.FindControl("hidJobState");
					HtmlInputHidden hidJobType = (HtmlInputHidden) e.Content.FindControl("hidJobType");

					if (hidJobState != null && hidJobType != null)
					{
						eJobType jobType = (eJobType) Enum.Parse(typeof(eJobType), ((string) dvJobRow.Table.Rows[0]["JobType"]).Replace(" ", ""), true);

						GridServerTemplateContainer container = (GridServerTemplateContainer) e.Content;

						if (jobState == eJobState.Booked || jobState == eJobState.Planned || jobState == eJobState.InProgress)
						{
							Facade.IJob facJob = new Facade.Job();
							((HtmlImage) e.Content.FindControl("imgRequiresCallIn")).Visible = facJob.RequiresCallIn(jobId);
						}
						else
							((HtmlImage) e.Content.FindControl("imgRequiresCallIn")).Visible = false;

						HtmlImage imgHasRequests = (HtmlImage) e.Content.FindControl("imgHasRequests");
						if (((int) dvJobRow[0]["Requests"]) == 0)
							imgHasRequests.Visible = false;
						else
						{
							imgHasRequests.Visible = true;
							imgHasRequests.Attributes.Add("onClick", "javascript:ShowPlannerRequests('" + jobId.ToString() + "');");
						}

						HtmlAnchor lnkEditJob = (HtmlAnchor) e.Content.FindControl("lnkEditJob");
						switch (jobType)
						{
							case eJobType.Normal:
								lnkEditJob.HRef = "javascript:openResizableDialogWithScrollbars('../job/wizard/wizard.aspx?jobId=" + jobId.ToString() + "', '623', '508');";
								break;
							case eJobType.PalletReturn:
								lnkEditJob.HRef = "../job/addupdatepalletreturnjob.aspx?jobId=" + jobId.ToString();
								break;
							case eJobType.Return:
								lnkEditJob.HRef = "../job/addupdategoodsreturnjob.aspx?jobId=" + jobId.ToString();
								break;
							default:
								lnkEditJob.Visible = false;
								break;
						}				
					}

					Table tblCollections = (Table) e.Content.FindControl("tblCollections");
					if (tblCollections != null)
					{
						tblCollections.BackColor = Utilities.GetJobStateColour(jobState);
						DataView dvCollections  = new DataView(m_dsJobsData.Tables[1]);
						foreach (DataRow collection in dvCollections.Table.Rows)
						{
							if ((int) collection["JobId"] == jobId)
							{
								// This is a collection for the current job
								ArrayList rows = CreateTableRows(collection);
								foreach (TableRow row in rows)
									tblCollections.Rows.Add(row);
							}
						}
					}

					Table tblDeliveries = (Table) e.Content.FindControl("tblDeliveries");
					if (tblDeliveries != null)
					{
						tblDeliveries.BackColor = Utilities.GetJobStateColour(jobState);
						DataView dvDeliveries = new DataView(m_dsJobsData.Tables[2]);
						foreach (DataRow delivery in dvDeliveries.Table.Rows)
						{
							if ((int) delivery["JobId"] == jobId)
							{
								// This is a delivery for the current job
								ArrayList rows = CreateTableRows(delivery);
								foreach (TableRow row in rows)
									tblDeliveries.Rows.Add(row);
							}
						}
					}
				}
			}
			catch (Exception exc)
			{
				string error = exc.Message;
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
        /// 
		private void InitializeComponent()
		{    
			this.Init += new EventHandler(bookinjobs_Init);
            this.dgJobsToBookIn.ItemContentCreated += new ComponentArt.Web.UI.Grid.ItemContentCreatedEventHandler(dgJobsToBookIn_ItemContentCreated);// +=new ComponentArt.Web.UI.Grid.ItemDataBoundEventHandler(dgJobsToBookIn_ItemDataBound);
		}

		#endregion
	}
}
