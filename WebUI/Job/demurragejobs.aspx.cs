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

namespace Orchestrator.WebUI.Job
{
	/// <summary>
	/// Displays a list of all jobs that have demurrage but have not been invoiced.
	/// </summary>
	public partial class demurragejobs : Orchestrator.Base.BasePage
	{
		#region Form Elements


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


		private void demurragejobs_Init(object sender, EventArgs e)
		{
			btnRefresh.Click += new EventHandler(btnRefresh_Click);

			dgJobs.ItemDataBound += new DataGridItemEventHandler(dgJobs_ItemDataBound);
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
				m_dsJobsData = facJob.GetDemurrageJobs(((Entities.CustomPrincipal) Page.User).IdentityId);

			dgJobs.DataSource = m_dsJobsData;
			dgJobs.DataBind();
		}

		#region Button Event Handlers

		private void btnRefresh_Click(object sender, EventArgs e)
		{
			LoadData();
		}

		#endregion

		#region DataGrid Event Handlers

		private void dgJobs_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			ListItemType itemType = e.Item.ItemType;

			if (itemType == ListItemType.Item || itemType == ListItemType.AlternatingItem || itemType == ListItemType.SelectedItem)
			{
				string jobIdText = ((HtmlInputHidden) e.Item.FindControl("hidJobId")).Value;

				try
				{
					int jobId = Convert.ToInt32(jobIdText);

					if (jobId > 0)
					{
						DataRowView dataItem = (DataRowView) e.Item.DataItem;
						eJobState jobState = (eJobState) Enum.Parse(typeof(eJobState), e.Item.Cells[2].Text.Replace(" ", ""), true);
						eJobType jobType = (eJobType) Enum.Parse(typeof(eJobType), e.Item.Cells[3].Text.Replace(", ", ""), true);

						e.Item.BackColor = Utilities.GetJobStateColour(jobState);

						if (jobState == eJobState.Booked || jobState == eJobState.Planned || jobState == eJobState.InProgress)
						{
							Facade.IJob facJob = new Facade.Job();
							((HtmlImage) e.Item.FindControl("imgRequiresCallIn")).Visible = facJob.RequiresCallIn(jobId);
						}
						else
							((HtmlImage) e.Item.FindControl("imgRequiresCallIn")).Visible = false;

						HtmlAnchor lnkEditJob = (HtmlAnchor) e.Item.FindControl("lnkEditJob");
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

						// Configure the toggle ownership button
						CheckBox chkOwnership = (CheckBox) e.Item.FindControl("chkOwnership");
						bool isMyJob = ((int) dataItem.Row["DoesOwn"] == 1);
						chkOwnership.Checked = isMyJob;

						HtmlImage imgHasRequests = (HtmlImage) e.Item.FindControl("imgHasRequests");
						if (((int) dataItem.Row["Requests"]) == 0)
							imgHasRequests.Visible = false;
						else
						{
							imgHasRequests.Visible = true;
							imgHasRequests.Attributes.Add("onClick", "javascript:ShowPlannerRequests('" + jobId.ToString() + "');");
						}

						Table tblCollections = (Table) e.Item.FindControl("tblCollections");

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

						Table tblDeliveries = (Table) e.Item.FindControl("tblDeliveries");

						DataView dvDeliveries = new DataView(m_dsJobsData.Tables[2]);
						foreach (DataRow delivery in dvDeliveries.Table.Rows)
						{
							if ((int) delivery["JobId"] == jobId)
							{
								// This is a collection for the current job
								ArrayList rows = CreateTableRows(delivery);
								foreach (TableRow row in rows)
									tblDeliveries.Rows.Add(row);
							}
						}
					}
				}
				catch (Exception exc)
				{
					string error = exc.Message;
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
			this.Init += new EventHandler(demurragejobs_Init);
		}
		#endregion
	}
}
