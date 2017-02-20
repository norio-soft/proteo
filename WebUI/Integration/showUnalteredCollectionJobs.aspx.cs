using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;


using System.Text;

namespace Orchestrator.WebUI.Integration
{
    public partial class showUnalteredCollectionJobs : Orchestrator.Base.BasePage
    {
		#region Page Variables

		private		DataSet				m_dsJobsData;

		#endregion

		#region Page Load/Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

			if (!IsPostBack)
			{
                // Display the job state legend
                foreach (HtmlTableRow row in tblLegend.Rows)
                    foreach (HtmlTableCell cell in row.Cells)
                    {
                        try
                        {
                            eJobState jobState = (eJobState)Enum.Parse(typeof(eJobState), cell.InnerText.Replace(" ", ""), true);
                            cell.BgColor = Utilities.GetJobStateColour(jobState).Name;
                        }
                        catch { }
                    } 
                
                BindJobData();
            }
		}

		private void PopulateStaticControls()
		{
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
		}

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnRefresh.Click += new EventHandler(btnSearch_Click);
            dgJobs.ItemContentCreated += new ComponentArt.Web.UI.Grid.ItemContentCreatedEventHandler(dgJobs_ItemContentCreated);
            MyClientSideAnchor.OnWindowClose += new Codesummit.WebModalAnchor.OnWindowCloseEventHandler(MyClientSideAnchor_OnWindowClose);
        }

        void MyClientSideAnchor_OnWindowClose(Codesummit.WebModalAnchor sender)
        {
            BindJobData();
        }

		#endregion

		private void BindJobData()
		{
            Facade.IJob facJob = new Facade.Job();
            m_dsJobsData = facJob.GetJobsWithoutCollectionDateChanges("1543,1547", ((Entities.CustomPrincipal)Page.User).IdentityId);

            if (m_dsJobsData != null && m_dsJobsData.Tables.Count != 0)
            {
                dgJobs.DataSource = m_dsJobsData;
                dgJobs.UnSelectAll();
                dgJobs.GroupBy = "JobState";
                dgJobs.DataBind();
            }
		}

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
			location.Text += "</span><br><b>" + (string) data["Dockets"] + "</b>";
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

		#region Button Event Handlers

		private void btnSearch_Click(object sender, EventArgs e)
		{
			BindJobData();
		}

		#endregion

		#region DataGrid Event Handlers

        void dgJobs_ItemContentCreated(object sender, ComponentArt.Web.UI.GridItemContentCreatedEventArgs e)
        {
            if (m_dsJobsData != null)
            {
                int jobId = Convert.ToInt32(e.Item["JobId"]);
                
                HtmlAnchor lnkManageJob = (HtmlAnchor)e.Content.FindControl("lnkManageJob");
                Table tblCollections = (Table)e.Content.FindControl("tblCollections");
                Table tblDeliveries = (Table)e.Content.FindControl("tblDeliveries");

                if (lnkManageJob != null)
                {
                    HtmlImage imgRequiresCallIn = (HtmlImage)e.Content.FindControl("imgRequiresCallIn");
                    HtmlImage imgHasRequests = (HtmlImage)e.Content.FindControl("imgHasRequests");
                    HtmlImage imgHasNewPCVs = (HtmlImage)e.Content.FindControl("imgHasNewPCVs");

                    // Set the manage job link.
                    lnkManageJob.HRef = "javascript:openDialogWithScrollbars('../traffic/JobManagement.aspx?wiz=true&jobId=" + jobId.ToString() + "'+ getCSID(),'600','400');";
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
                }

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
                                tblDeliveries.Rows.Add(row);
                        }
                    }
                }
            }
        }
        
		#endregion
    }
}