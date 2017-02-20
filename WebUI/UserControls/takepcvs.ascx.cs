using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using P1TP.Components.Web.Validation;
using Orchestrator.Globals;
using Orchestrator.WebUI.Controls;

//using Globals;
//using WebUI.Controls;

namespace Orchestrator.WebUI.UserControls
{
	/// <summary>
	///		Summary description for TakePCVs.
	/// </summary>
	public partial class TakePCVs : System.Web.UI.UserControl
	{
		#region Constants

		private const string C_JOB_ID_VS = "C_JOB_ID_VS";
        private const string C_DELIVERY_POINT_ID_VS = "C_DELIVERY_POINT_ID_VS";

		#endregion

		#region Form Elements

		#endregion

		#region Page Variables

		private int					m_jobId = 0;
        private int                 m_deliveryPointId = 0;
        private int                 m_PointId = 0;
		private Entities.Job		m_job = null;

        private int m_palletCount = 0;

		#endregion

		#region Property Interfaces

		private string JobSortCriteria
		{
			get { return (string) ViewState["C_JOB_SORT_CRITERIA_VS"]; }
			set { ViewState["C_JOB_SORT_CRITERIA_VS"] = value; }
		}

		private string JobSortDirection
		{
			get { return (string) ViewState["C_JOB_SORT_DIRECTION_VS"]; }
			set { ViewState["C_JOB_SORT_DIRECTION_VS"] = value; }
		}

		private string PCVSortCriteria
		{
			get { return (string) ViewState["C_PCV_SORT_CRITERIA_VS"]; }
			set { ViewState["C_PCV_SORT_CRITERIA_VS"] = value; }
		}

		private string PCVSortDirection
		{
			get { return (string) ViewState["C_PCV_SORT_DIRECTION_VS"]; }
			set { ViewState["C_PCV_SORT_DIRECTION_VS"] = value; }
		}

		#endregion

		#region Page Load/Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Retrieve the job we're working on
			try
			{
                if (Request.QueryString["pointID"] != null)
                    int.TryParse(Request.QueryString["pointID"], out m_PointId);

				m_jobId = Convert.ToInt32(ViewState[C_JOB_ID_VS]);
                m_deliveryPointId = Convert.ToInt32(ViewState[C_DELIVERY_POINT_ID_VS]);

				Facade.IJob facJob = new Facade.Job();
				m_job = facJob.GetJob(m_jobId);

				Facade.IPCV facPCV = new Facade.PCV();
				m_job.PCVs = facPCV.GetForJobId(m_jobId, m_deliveryPointId);
			}
			catch {}

			if (!IsPostBack)
			{
				PopulateStaticControls();
				LoadJobs();
			}
		}

		private void TakePCVs_Init(object sender, EventArgs e)
		{
			cboControlAreas.SelectedIndexChanged += new EventHandler(cboControlAreas_SelectedIndexChanged);

			dgJobs.ItemCommand += new DataGridCommandEventHandler(dgJobs_ItemCommand);
			dgJobs.SortCommand += new DataGridSortCommandEventHandler(dgJobs_SortCommand);
            dgJobs.ItemDataBound += new DataGridItemEventHandler(dgJobs_ItemDataBound);

			dgPCVs.ItemDataBound += new DataGridItemEventHandler(dgPCVs_ItemDataBound);
			dgPCVs.SortCommand += new DataGridSortCommandEventHandler(dgPCVs_SortCommand);

			btnTakeOnJob.Click += new EventHandler(btnTakeOnJob_Click);
			btnGenerateRedemptionForm.Click += new EventHandler(btnGenerateRedemptionForm_Click);

            btnRefresh.Click += new EventHandler(btnRefresh_Click);
        }
        
		#endregion

		#region Page Methods

		private void LoadJobs()
		{
			Facade.IJob facJob = new Facade.Job();
            DataSet ds = null;
            int controlAreaId = 0;

            ConfigureDisplay();

            if (Orchestrator.Globals.Configuration.UseControlAreaToFilterPCVsForReport && int.TryParse(cboControlAreas.SelectedValue, out controlAreaId))
                ds = facJob.GetJobsThatCanTakePCVs(controlAreaId);
            else if (!Orchestrator.Globals.Configuration.UseControlAreaToFilterPCVsForReport)
                ds = facJob.GetJobsThatCanTakePCVs(null);
            
            if (ds != null && ds.Tables.Count > 0)
            {
                List<DataRow> jobDetails = null;
                DataView dv = new DataView(ds.Tables[0]);

                string sortExpression = JobSortCriteria + " " + JobSortDirection;
                dv.Sort = sortExpression.Trim();

                if (m_jobId > 0)
                {
                    //dgJobs.Columns[7].Visible = false;
                    //dv.RowFilter = "JobId = " + m_jobId.ToString() + " AND DeliveryPointId = " + m_deliveryPointId.ToString();

                    jobDetails = ds.Tables[0].Rows.Cast<DataRow>().Where(dr => (int)dr["JobID"] == m_jobId && (int)dr["DeliveryPointID"] == m_deliveryPointId).ToList();
                    LoadJobDetails(jobDetails);
                    LoadReport(m_jobId);

                    dgJobs.Visible = false;
                }
                else
                {
                    
                    if (m_PointId > 0)
                    {
                        dv.RowFilter = "DeliveryPointId = " + m_PointId.ToString();
                    }
                    else
                        dgJobs.Columns[7].Visible = true;

                    dgJobs.Visible = true;
                    dgJobs.DataSource = dv;
                    dgJobs.DataBind();
                }

                dgPCVs.Visible = m_jobId > 0;
                reportViewer.Visible = m_jobId > 0 && m_job.PCVs.Count > 0;
                btnTakeOnJob.Enabled = m_jobId > 0;
                btnGenerateRedemptionForm.Enabled = m_jobId > 0;
            }
            else
                PopulateStaticControls();
        }

		private void LoadPCVs(int jobId)
		{
			Facade.IPCV facPCV = new Facade.PCV();
			DataSet ds = facPCV.GetPCVsThatCanBeTaken(jobId);
			
			DataView dv = new DataView(ds.Tables[0]);

			string sortExpression = PCVSortCriteria + " " + PCVSortDirection;

			dv.Sort = sortExpression.Trim();
            dv.RowFilter = "PointId = " + m_deliveryPointId.ToString();
            m_palletCount = 0;
			dgPCVs.DataSource = dv;
			dgPCVs.DataBind();
            lblPalletCount.Text = m_palletCount.ToString();

			dgPCVs.Visible = dv.Table.Rows.Count > 0;
			btnTakeOnJob.Enabled = dv.Table.Rows.Count > 0;
			btnGenerateRedemptionForm.Enabled = dv.Table.Rows.Count > 0;
		}

        private void LoadPCVAgreements(int jobID)
        {
            Facade.IPCV facPCV = new Facade.PCV();
            lvPCVRedemptionAgreed.DataSource = facPCV.GetPCVAgreementsForJobID(jobID);
            lvPCVRedemptionAgreed.DataBind();
        }

		private void LoadReport(int jobId)
		{
			Facade.IPCV facPCV = new Facade.PCV();

			DataSet dsPoints = facPCV.GetPointsWithPCVsForJobId(jobId);
			
			NameValueCollection reportParams = new NameValueCollection();
			reportParams.Add("JobId", jobId.ToString());

			//-------------------------------------------------------------------------------------	
			//									Load Report Section 
			//-------------------------------------------------------------------------------------	
			Session[Globals.Constants.ReportTypeSessionVariable] = eReportType.PCVRedemptionForm;
			Session[Globals.Constants.ReportDataSessionTableVariable] = dsPoints;
			Session[Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
			Session[Globals.Constants.ReportDataMemberSessionVariable] = "Table";
			Session[Globals.Constants.ReportParamsSessionVariable] = reportParams;

			// Show the user control
			reportViewer.Visible = true;
		}

        private void LoadJobDetails(List<DataRow> jobDetails)
        {
            if (jobDetails.Count == 1)
                foreach(DataRow dr in jobDetails)
                {
                    lblJobID.Text = dr.Field<int>("JobID").ToString();
                    lblLoadNo.Text = dr.Field<string>("LoadNo").ToString();
                    lblDockets.Text = dr.Field<string>("Dockets");
                    lblClient.Text = dr.Field<string>("Client");
                    lblDeliveryPoint.Text = dr.Field<string>("DeliveryPoint");

                    if (dr.Field<bool>("IsAnytime"))
                    {
                        lblBookedDateTime.Text = dr.Field<DateTime>("BookedDateTime").ToString("dd/MM/yy") + " Anytime";
                        //rdiSlotDate.SelectedDate = dr.Field<DateTime>("BookedDateTime");
                    }
                    else
                    {
                        lblBookedDateTime.Text = dr.Field<DateTime>("BookedDateTime").ToString("dd/MM/yy HH:mm");
                        //rdiSlotDate.SelectedDate = dr.Field<DateTime>("BookedDateTime");
                        //rdiSlotTime.SelectedDate = dr.Field<DateTime>("BookedDateTime");
                    }
                }
        }

		private void PopulateStaticControls()
		{
            if (Orchestrator.Globals.Configuration.UseControlAreaToFilterPCVsForReport)
            {
                Facade.ITrafficSheetFilter facTrafficSheetFilter = new Facade.Traffic();
                Entities.TrafficSheetFilter defaultFilter = facTrafficSheetFilter.GetDefault(((Entities.CustomPrincipal)Page.User).IdentityId);

                Facade.IControlArea facControlArea = new Facade.Traffic();
                cboControlAreas.DataSource = facControlArea.GetAll();
                cboControlAreas.DataBind();

                if (defaultFilter == null)
                    cboControlAreas.SelectedIndex = 0;
                else
                    cboControlAreas.SelectedValue = defaultFilter.ControlAreaId.ToString();
            }

            //rdiSlotDate.SelectedDate = DateTime.Today;
            //rdiSlotTime.SelectedDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 23, 59, 0);
            rdiSlotDate.SelectedDate = null;
            rdiSlotTime.SelectedDate = null;
		}
	
		private void Reset()
		{
			LoadJobs();
		}

        private void ConfigureDisplay()
        {
            if (m_jobId > 0)
            {
                LoadPCVAgreements(m_jobId);
                pnlAgreedPCVRedemptions.Visible = pnlPCVAvailable.Visible = true;
                lblJobs.Visible = false;
                btnRefresh.Text = "Back";
            }
            else
            {
                pnlAgreedPCVRedemptions.Visible = pnlPCVAvailable.Visible = false;
                lblJobs.Visible = true;
                btnRefresh.Text = "Refresh";
            }

            pnlControlArea.Visible = Orchestrator.Globals.Configuration.UseControlAreaToFilterPCVsForReport;
        }

		#endregion

		#region Button Event Handlers

        void btnRefresh_Click(object sender, EventArgs e)
        {
            lblConfirmation.Text = string.Empty;
            lblConfirmation.Visible = false;

            JobSortCriteria = "";
            JobSortDirection = "";

            PCVSortCriteria = "";
            PCVSortDirection = "";

            m_jobId = 0;
            ViewState[C_JOB_ID_VS] = m_jobId;

            Reset();
        }

		private void btnTakeOnJob_Click(object sender, EventArgs e)
		{
			reportViewer.Visible = false;
			Facade.IPCV facPCV = new Facade.PCV();
			Entities.PCVCollection pcvs = new Entities.PCVCollection();
            List<int> updatePCVIDs = new List<int>();
			int pcvId;
            DateTime AgreedDateTime;
            string ClientContact = string.Empty;

            m_palletCount = 0;
			foreach (DataGridItem item in dgPCVs.Items)
			{
				CheckBox takePCV = (CheckBox) item.FindControl("chkTakePCV");
                CheckBox updatePCV = item.FindControl("chkUpdatePCV") as CheckBox;

				if (takePCV.Checked)
				{
					pcvId = Convert.ToInt32(item.Cells[0].Text);
                    m_palletCount += Convert.ToInt32(item.Cells[4].Text);

					Entities.PCV pcv = facPCV.GetForPCVId(pcvId);
					pcvs.Add(pcv);
				}

                if (updatePCV.Checked)
                    updatePCVIDs.Add(int.Parse(item.Cells[0].Text));
			}

            lblPalletCount.Text = m_palletCount.ToString();

            if (rdiSlotDate.SelectedDate.HasValue && rdiSlotTime.SelectedDate.HasValue)
                AgreedDateTime = new DateTime(rdiSlotDate.SelectedDate.Value.Year, rdiSlotDate.SelectedDate.Value.Month, rdiSlotDate.SelectedDate.Value.Day, rdiSlotTime.SelectedDate.Value.Hour, rdiSlotTime.SelectedDate.Value.Minute, 0);
            else
                AgreedDateTime = DateTime.Today;

            // If the client contact is empty, the agreed redemption details will not be added. 
            ClientContact = txtClientContact.Text;

			Facade.IJob facJob = new Facade.Job();
            bool success = facJob.TakePCVs(m_jobId, m_deliveryPointId, pcvs, updatePCVIDs, AgreedDateTime, ClientContact, ((Entities.CustomPrincipal)Page.User).Name);

			if (success)
			{
                LoadPCVAgreements(m_jobId);

                m_job.PCVs = facPCV.GetForJobId(m_jobId, m_deliveryPointId);
                LoadPCVs(m_jobId);

                if (m_job.PCVs.Count > 0)
                    LoadReport(m_jobId);
                else
                    reportViewer.Visible = false;
			}
			else
			{
				lblConfirmation.Visible = true;
				lblConfirmation.Text = "The specified PCVs will not be taken on the specified job as an error occurred, please try again.";
			}
		}

		private void btnGenerateRedemptionForm_Click(object sender, EventArgs e)
		{
			LoadReport(m_jobId);
		}

		#endregion

		#region Drop Down Event Handlers

		private void cboControlAreas_SelectedIndexChanged(object sender, EventArgs e)
		{
			LoadJobs();
		}

		#endregion

		#region Data Grid Event Handlers

		#region Job Data Grid Events

		private void dgJobs_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			switch (e.CommandName)
			{
				case "AttachPCVs":
					m_jobId = Convert.ToInt32(e.Item.Cells[0].Text);
                    m_deliveryPointId = Convert.ToInt32(e.Item.Cells[5].Text);

					ViewState[C_JOB_ID_VS] = m_jobId;
                    ViewState[C_DELIVERY_POINT_ID_VS] = m_deliveryPointId;

					Facade.IJob facJob = new Facade.Job();
					m_job = facJob.GetJob(m_jobId);
					Facade.IPCV facPCV = new Facade.PCV();
					m_job.PCVs = facPCV.GetForJobId(m_jobId, m_deliveryPointId);

					LoadJobs();
					LoadPCVs(m_jobId);
					break;
			}
		}

		private void dgJobs_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			if (e.SortExpression == JobSortCriteria)
			{
				if (JobSortDirection == "DESC")
					JobSortDirection = "ASC";
				else
					JobSortDirection = "DESC";
			}
			else
			{
				JobSortCriteria = e.SortExpression;
				JobSortDirection = "DESC";
			}

			LoadJobs();
		}

        void dgJobs_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView dataItem = (DataRowView)e.Item.DataItem;
                if (((bool)dataItem["Manage"]).ToString() == bool.TrueString)
                {
                    Button btn = (Button)e.Item.Cells[7].Controls[0];
                    btn.Text = "Edit PCV's On Job";
                }
            }
        }

		#endregion 

		#region PCV Data Grid Events 
        
		private void dgPCVs_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
                bool pcvFound = false;
				int pcvId = Convert.ToInt32(e.Item.Cells[0].Text);
                int palletCount = Convert.ToInt32(e.Item.Cells[4].Text);

				CheckBox chkTakePCV = (CheckBox) e.Item.FindControl("chkTakePCV");
                CheckBox chkUpdatePCV = e.Item.FindControl("chkUpdatePCV") as CheckBox;

                chkTakePCV.Attributes.Add("onClick", "javascript:HandleCheckboxClick(this, " + palletCount.ToString() + ");");

				e.Item.Attributes.Add("onClick", "javascript:HighlightRow('" + e.Item.ClientID + "');");
				e.Item.Attributes.Add("id", e.Item.ClientID);

				if (m_job != null)
					foreach (Entities.PCV pcv in m_job.PCVs)
					{
						if (pcv.PCVId == pcvId)
						{
							pcvFound = chkTakePCV.Checked = true;
                            m_palletCount += palletCount;
							break;
						}
						else
							chkTakePCV.Checked = false;
					}

                if (!pcvFound)
                    chkUpdatePCV.Enabled = false;

			}
		}

		private void dgPCVs_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			if (PCVSortCriteria == e.SortExpression)
			{
				if (PCVSortDirection == "DESC")
					PCVSortDirection = "ASC";
				else
					PCVSortDirection = "DESC";
			}
			else
			{
				PCVSortCriteria = e.SortExpression;
				PCVSortDirection = "DESC";
			}

			LoadPCVs(m_jobId);
		}

		#endregion

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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Init += new EventHandler(TakePCVs_Init);
		}
		#endregion
	}
}
