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

namespace Orchestrator.WebUI.Resource
{
	/// <summary>
	/// Summary description for Future.
	/// </summary>
	public partial class Future : Orchestrator.Base.BasePage
	{
		#region Form Elements




		#endregion

		#region Page Variables

		private		int				m_resourceId;
		private		eResourceType	m_resourceType;
		private		DateTime		m_startDate;

		private		DataSet			m_dsFutureJobs;

		#endregion

		#region Page Load/Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

			m_resourceId = Convert.ToInt32(Request.QueryString["resourceId"]);
			m_resourceType = (eResourceType) Convert.ToInt32(Request.QueryString["resourceTypeId"]);
			string date = Request.QueryString["fromDateTime"];
			m_startDate = new DateTime(Convert.ToInt32(date.Substring(4, 4)), Convert.ToInt32(date.Substring(2, 2)), Convert.ToInt32(date.Substring(0, 2)), Convert.ToInt32(date.Substring(8, 2)), Convert.ToInt32(date.Substring(10, 2)), 0);

			if (!IsPostBack)
			{
				switch (m_resourceType)
				{
					case eResourceType.Driver:
						Facade.IDriver facDriver = new Facade.Resource();
						lblResource.Text = facDriver.GetDriverForResourceId(m_resourceId).Individual.FullName;
						break;
					case eResourceType.Vehicle:
						Facade.IVehicle facVehicle = new Facade.Resource();
						lblResource.Text = facVehicle.GetForVehicleId(m_resourceId).RegNo;
						break;
					case eResourceType.Trailer:
						Facade.ITrailer facTrailer = new Facade.Resource();
						lblResource.Text = facTrailer.GetForTrailerId(m_resourceId).TrailerRef;
						break;
				}

				Facade.IJob facJob = new Facade.Job();
				m_dsFutureJobs = facJob.GetFutureJobs(m_resourceId, ((Entities.CustomPrincipal) Page.User).IdentityId);
				dgBasicJobs.DataSource = m_dsFutureJobs;
				dgBasicJobs.DataBind();
			}
		}

		private void DriverFuture_Init(object sender, EventArgs e)
		{
			dgBasicJobs.ItemDataBound += new DataGridItemEventHandler(dgBasicJobs_ItemDataBound);
            btnCancel.Click += new EventHandler(btnCancel_Click);
		}

        void btnCancel_Click(object sender, EventArgs e)
        {
            mwhelper.CausePostBack = false;
            mwhelper.CloseForm = true;
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
			location.Text += "<span onMouseOver=\"ShowPoint('" + pointAddressUri.ToString() + "', '" + ((int) data["PointId"]).ToString() + "');\" onMouseOut=\"HidePoint();\">";
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

		#region DataGrid Event Handlers

		private void dgBasicJobs_ItemDataBound(object sender, DataGridItemEventArgs e)
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

						e.Item.BackColor = Utilities.GetJobStateColour(jobState);

						if (jobState == eJobState.Booked || jobState == eJobState.Planned || jobState == eJobState.InProgress)
						{
							Facade.IJob facJob = new Facade.Job();
							((HtmlImage) e.Item.FindControl("imgRequiresCallIn")).Visible = facJob.RequiresCallIn(jobId);
						}
						else
							((HtmlImage) e.Item.FindControl("imgRequiresCallIn")).Visible = false;

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

						DataView dvCollections  = new DataView(m_dsFutureJobs.Tables[1]);
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

						DataView dvDeliveries = new DataView(m_dsFutureJobs.Tables[2]);
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
			this.Init += new EventHandler(DriverFuture_Init);
		}
		#endregion
	}
}
