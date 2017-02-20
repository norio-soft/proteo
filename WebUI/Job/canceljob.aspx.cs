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

using Orchestrator.WebUI.Security;

namespace Orchestrator.WebUI.Job
{	 		
	/// <summary>
	/// Summary description for jobDetails.
	/// </summary>
	public partial class CancelJob : Orchestrator.Base.BasePage
	{

		#region Page Variables

		#endregion
		
       	#region Form Elements


		
		protected System.Web.UI.WebControls.Button btnBack;


		#endregion	
		
		#region Page Load/Init/Error
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.ProcessCancellations);


		 	// if (Request.QueryString["jobId"] != string.Empty)
			// m_jobId = Convert.ToInt32(Request.QueryString["jobId"]);
	
			if (!IsPostBack)
			{
                if (Request.QueryString["rcbID"] == null)
                {
                    PopulateStaticControls();

                    if (Request.QueryString["ca"] != null)
                    {
                        cboTrafficeArea.ClearSelection();
                        cboTrafficeArea.Items.FindByValue(Request.QueryString["ca"]).Selected = true;
                    }

                    ApplyFilter();
                }
			}
            else
                ApplyFilter();
		}

        protected void Page_Init(object sender, EventArgs e)
        {
            this.cboClient.ItemsRequested +=new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
            this.dgJobs.ItemContentCreated += new ComponentArt.Web.UI.Grid.ItemContentCreatedEventHandler(dgJobs_ItemContentCreated);
            this.dgJobs.ItemCommand += new ComponentArt.Web.UI.Grid.ItemCommandEventHandler(dgJobs_ItemCommand);
            
        }

        void dgJobs_ItemCommand(object sender, ComponentArt.Web.UI.GridItemCommandEventArgs e)
        {
            Facade.Job facJob = new Orchestrator.Facade.Job();

            string userId = ((Entities.CustomPrincipal)Page.User).UserName;
            int jobId;
            bool success = false;

            switch (((Button)e.Control).CommandName.ToLower())
            {
                case "reinstate":
                    jobId = Convert.ToInt32(e.Item["JobId"]);
                    success = facJob.UpdateForCancellation(jobId, false, "", userId);

                    if (success == true)
                    {
                        pnlConfirmation.Visible = true;
                        imgIcon.ImageUrl = "~/images/ico_info.gif";
                        lblNote.Text = "The Job " + jobId + " has been reinstated successfully.";
                    }
                    else
                    {
                        pnlConfirmation.Visible = true;
                        imgIcon.ImageUrl = "~/images/ico_critical.gif";
                        lblNote.Text = "The application failed while reinstating the job, please try again.";
                    }

                    break;
                case "cancel":
                    jobId = Convert.ToInt32(e.Item["JobId"]);

                    Facade.IInstruction facInstruction = new Facade.Instruction();
                    Entities.InstructionCollection insCol = facInstruction.GetForJobId(jobId);

                    if (insCol.Count == 0 || insCol[0].InstructionActuals == null || insCol[0].InstructionActuals.Count == 0)
                    {
                        success =
                            facJob.UpdateState(jobId, eJobState.Cancelled,
                                               ((Entities.CustomPrincipal) Page.User).UserName);

                        if (success)
                        {
                            pnlConfirmation.Visible = true;
                            imgIcon.ImageUrl = "../images/ico_info.gif";
                            lblNote.Text = "The Job " + jobId + " has been cancelled successfully.";
                        }
                        else
                        {
                            pnlConfirmation.Visible = true;
                            imgIcon.ImageUrl = "../images/ico_critical.gif";
                            lblNote.Text = "The application failed while cancelling the job, please try again.";
                        }
                    }
                    else
                    {
                        pnlConfirmation.Visible = true;
                        imgIcon.ImageUrl = "../images/ico_critical.gif";
                        lblNote.Text = "This job can not be cancelled at this time as it has at least one call-in.";
                    }

                    break;
            }

            ApplyFilter();
        }


        void cboClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {

            cboClient.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllOrganisationsFiltered(e.Text);

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
                rcItem.Text = dt.Rows[i]["OrganisationName"].ToString();
                rcItem.Value = dt.Rows[i]["IdentityId"].ToString();
                cboClient.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }

        }


		#region DBCombo's Server Methods and Initialisation



        #endregion
        #endregion


        #region Populate and Display Controls/Elements

        public void PopulateStaticControls()
		{
			Facade.IReferenceData facRef = new Facade.ReferenceData();
			DataSet dsControlAreas = facRef.GetAllControlAreas();
			cboTrafficeArea.DataSource = dsControlAreas;
			cboTrafficeArea.DataValueField = "ControlAreaId";
			cboTrafficeArea.DataTextField = "Description";
		
			cboTrafficeArea.DataBind(); 
		}

		#endregion
 	
		#region Page Elements Changes

		protected void btnFilter_Click(object sender, System.EventArgs e)
		{
			ApplyFilter();
            pnlConfirmation.Visible = false;
        }

		#endregion
		
		#region Methods and Functions
        protected DataSet m_dsJobsData = null;
		public void ApplyFilter()
		{
			string filter = string.Empty;
			
			Facade.Job facJob = new Facade.Job();
			DataSet ds = null;
			
			// Build Filter String
			// Client
			if (cboClient.SelectedValue != string.Empty)
			{
				filter = "IdentityId = " + cboClient.SelectedValue; 
			}

			// Control Area
			if (cboTrafficeArea.SelectedValue != string.Empty)
			{
				if (filter != string.Empty)
				{
                    filter += " AND CancellationControlArea = " + cboTrafficeArea.SelectedValue; 
				}
				else
				{
                    filter = "CancellationControlArea = " + cboTrafficeArea.SelectedValue; 
				}
			}

            if (dteStartDate.SelectedDate == DateTime.MinValue || dteEndDate.SelectedDate == DateTime.MinValue)
			{
				ds = facJob.GetMarkedForCancellation();
			}
			else
			{
				DateTime start = dteStartDate.SelectedDate.Value;
				start = start.Subtract(start.TimeOfDay);
				DateTime end = dteEndDate.SelectedDate.Value;
				end = end.Subtract(end.TimeOfDay);
				end = end.Add(new TimeSpan(0, 23, 59, 59));

				ds = facJob.GetMarkedForCancellationByDate(start, end);
			}
						
			DataView dv = new DataView(ds.Tables[0], "", "", DataViewRowState.CurrentRows); 
			
			// Apply dataview filter
			dv.RowFilter = filter;

            m_dsJobsData = ds;

            //dgJobs.GroupBy = "OrganisationName";
            dgJobs.DataSource = dv;
            dgJobs.DataBind();

            if (dgJobs.Items.Count != 0)
                dgJobs.Visible = true;
    		else
			{
                dgJobs.Visible = false;
				pnlConfirmation.Visible = true;
                imgIcon.ImageUrl = "~/images/ico_warning.gif";
				lblNote.Text = "No results for given criteria.";
				lblNote.ForeColor = Color.Red; 
			}
		}


		protected void btnReset_Click(object sender, EventArgs e)
		{
			// Clear the dbcombo used for the client
			cboClient.SelectedValue = "";
			cboClient.Text = "";
			

			// Clear the traffic area selector
			cboTrafficeArea.ClearSelection();

			// Clear the date ranges
            dteStartDate.SelectedDate = (DateTime?)null;
            dteEndDate.SelectedDate = (DateTime?)null;

			ApplyFilter();
		}


		#endregion

        #region DataGrid Event Handlers
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
            location.Text += "<span onMouseOver=\"ShowPoint('" + pointAddressUri.ToString() + "', '" + ((int)data["PointId"]).ToString() + "')\" onMouseOut=\"HidePoint()\">";
            location.Text += (string)data["Location"];
            location.Text += "</span><br><b>" + (string)data["Dockets"] + "</b>";
            location.Width = new Unit("100px");
            row.Cells.Add(location);

            // Timing
            TableCell timing = new TableCell();
            if ((bool)data["Anytime"])
                timing.Text = ((DateTime)data["By"]).ToString("dd/MM/yy") + " Anytime";
            else
                timing.Text = ((DateTime)data["By"]).ToString("dd/MM/yy HH:mm");
            timing.Width = new Unit("100px");
            row.Cells.Add(timing);

            // Resources
            TableCell resources = new TableCell();
            if (data["FullName"] != DBNull.Value)
                resources.Text = "<b>" + (string)data["FullName"] + "</b><br>";
            if (data["RegNo"] != DBNull.Value)
                resources.Text += "<b>" + (string)data["RegNo"] + "</b><br>";
            if (data["TrailerRef"] != DBNull.Value)
                resources.Text += "<b>" + (string)data["TrailerRef"] + "</b><br>";
            if (resources.Text.Length == 0)
                resources.Text = "&nbsp;";
            resources.Width = new Unit("150px");
            row.Cells.Add(resources);

            foreach (TableCell cell in row.Cells)
            {
                cell.VerticalAlign = VerticalAlign.Top;
                cell.HorizontalAlign = HorizontalAlign.Left;
            }

            //// Pallets (delivery only)
            //if (data.Table.Columns.Contains("Pallets"))
            //{
            //    TableCell pallets = new TableCell();
            //    if (data["Pallets"] != DBNull.Value)
            //        pallets.Text = ((int)data["Pallets"]).ToString("F0");
            //    else
            //        pallets.Text = "0";
            //    pallets.Width = new Unit("30px");
            //    pallets.VerticalAlign = VerticalAlign.Top;
            //    pallets.HorizontalAlign = HorizontalAlign.Right;
            //    row.Cells.Add(pallets);
            //}

            //// Weight (delivery only)
            //if (data.Table.Columns.Contains("Weight"))
            //{
            //    TableCell weight = new TableCell();
            //    if (data["Weight"] != DBNull.Value)
            //        weight.Text = ((decimal)data["Weight"]).ToString("F3");
            //    else
            //        weight.Text = "0.000";
            //    weight.Width = new Unit("30px");
            //    weight.VerticalAlign = VerticalAlign.Top;
            //    weight.HorizontalAlign = HorizontalAlign.Right;
            //    row.Cells.Add(weight);
            //}

            #endregion

            #region Notes

            if (data["Note"] != DBNull.Value && ((string)data["Note"]).Length > 0)
            {
                TableRow notes = new TableRow();
                rows.Add(notes);

                // Note
                TableCell space = new TableCell();
                space.Text = "&nbsp;";
                notes.Cells.Add(space);
                TableCell note = new TableCell();
                note.Text = (string)data["Note"];
                note.ColumnSpan = row.Cells.Count - 1;
                notes.Cells.Add(note);
            }

            #endregion

            return rows;
        }

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

            if (e.Column.DataCellServerTemplateId == "ReinstateJobTemplate")
            {
                Button btn = (Button)e.Content.FindControl("btnReinstate");
                btn.Attributes.Add("onclick", "javascript:" + btn.ClientID + ".disabled=true;" + this.GetPostBackEventReference(btn));
            }

            if (e.Column.DataCellServerTemplateId == "CancelJobTemplate")
            {
                Button btn = (Button)e.Content.FindControl("btnCancel");
                btn.Attributes.Add("onclick", "javascript:" + btn.ClientID + ".disabled=true;" + this.GetPostBackEventReference(btn));
            }

        }

        #endregion

		#region Cancelled Job Data Grid Events

	
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
		
		}
		#endregion
	}
}
