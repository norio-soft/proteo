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

using Orchestrator.WebUI.Controls;

using Orchestrator.WebUI.UserControls;
using Orchestrator.WebUI.Security;

namespace Orchestrator.WebUI.Traffic.JobManagement
{
	/// <summary>
	/// Summary description for bookingInPODs.
	/// </summary>
	public partial class bookingInPODs : Orchestrator.Base.BasePage
	{
		#region Private Fields

		protected Entities.Job	m_job;
		private DataSet			m_dsJob;
		private Facade.IJob		m_facJob;
		private int				m_jobId;
		private int				m_assignedPodCount;
		private bool			m_canEdit = false;
        private int             m_orderID;

		#endregion 

		#region Form Elements
		
		protected System.Web.UI.WebControls.Button		btnDeleteFromJob;
		

		protected System.Web.UI.WebControls.CheckBox	chkUnassignPODs;

		
		#endregion

		#region Public Properties

		public int JobId 
		{
			get { return m_jobId; }
		}

		#endregion

		#region Page Load

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage, eSystemPortion.AttachPODs);
			m_canEdit = Orchestrator.WebUI.Security.Authorise.CanAccess(eSystemPortion.AttachPODs);

			ViewState["AssignedPODCount"] = m_assignedPodCount;
			m_jobId = Convert.ToInt32(Request.QueryString["jobId"]);
		
			m_facJob = new Facade.Job();
			if(m_jobId > 0) 
			{
				m_job = m_facJob.GetJob(m_jobId);

				if (m_job.JobState == eJobState.Cancelled)
                    Response.Redirect("../../Job/job.aspx?wiz=true&jobId=" + m_job.JobId.ToString() + "&csid=" + this.CookieSessionID);

				if (m_job.JobState == eJobState.Booked || m_job.JobState == eJobState.Planned || m_job.JobState == eJobState.InProgress)
                    Response.Redirect("../jobManagement.aspx?jobId=" + m_jobId.ToString() + "&csid=" + this.CookieSessionID);
			}

			if(!IsPostBack) 
			{
			   PopulatePODs();

               PopulatePCVS();
               PopulateRefusals();

                //PODs can only be booked in if the job state is of Completed or BookingInIncomplete.
				if (m_jobId > 0 && m_job.JobState == eJobState.BookingInComplete || m_job.JobState == eJobState.ReadyToInvoice || m_job.JobState == eJobState.Invoiced)
				{
					chkBookingInComplete.Checked = true;
                    if (m_job.JobState == eJobState.Invoiced)
                    {
                        chkBookingInComplete.Enabled = false;
                        pnlUnassignedPODs.Enabled = false;
                        pnlUpdateJob.Enabled = false;
                        btnUnassignPODs.Enabled = false;
                    }
                    else
                    {
                        btnUnassignPODs.Enabled = m_canEdit;
                        pnlUnassignedPODs.Enabled = m_canEdit;
                        pnlUpdateJob.Enabled = m_canEdit;
                    }
				}
			}

			if (chkBookingInComplete.Checked)
				DisplayProgress();

			lblConfirmation.Visible = false;
			lblAddToJobError.Visible = false;
		}

		#endregion

		#region DataGrid Event Handlers

		protected void dgUnassignedPODs_Page(Object sender, DataGridPageChangedEventArgs e) 
		{
			dgUnassignedPODs.CurrentPageIndex = e.NewPageIndex;   
			PopulatePODs();
		}	

		protected void dgUnassignedPODs_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem) 
			{
				Facade.IPOD facPOD = new Facade.POD();
				
				Entities.POD itemPOD = facPOD.GetForPODId(int.Parse(e.Item.Cells[0].Text));
				
				string podId = e.Item.Cells[0].Text;
				
				HyperLink lnkViewPOD = (HyperLink)e.Item.FindControl("lnkViewPOD");
				if (itemPOD.ScannedFormId > 0)
				{
                    lnkViewPOD.Target = "_blank";
                    lnkViewPOD.NavigateUrl = itemPOD.ScannedFormPDF;
				}
				else
				{
					lnkViewPOD.Visible = false;
				}
			}
		}

		protected void dgCollectionDrop_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			m_assignedPodCount = (int) ViewState["AssignedPODCount"];

			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem) 
			{	
				Orchestrator.Facade.POD facPOD = new Orchestrator.Facade.POD();
				int collectDropId = int.Parse(e.Item.Cells[0].Text);
				DataSet dsPOD = facPOD.GetForCollectDropID(collectDropId);
				Entities.POD itemPOD = new Entities.POD();
				string ticketNo = null;
				if(dsPOD.Tables[0].Rows.Count > 0) 
				{
					itemPOD = facPOD.GetForPODId(Convert.ToInt32(dsPOD.Tables[0].Rows[0]["PODId"]));
					ticketNo = (string)dsPOD.Tables[0].Rows[0]["TicketNo"];
				}
				if (ticketNo == null) 
				{
					((CheckBox) e.Item.FindControl("chkUnassignPOD")).Visible = false;
					((HyperLink) e.Item.FindControl("lnkPODScanning")).Text = "Scan POD";
					((HyperLink) e.Item.FindControl("lnkPODScanning")).NavigateUrl = @"javascript:OpenPODWindow(" + JobId + "," + collectDropId + ");";
				}
				else
				{
                    if (itemPOD != null && itemPOD.ScannedFormId > 0) 
					{
						((HyperLink) e.Item.FindControl("lnkPODView")).Visible = true;
                        ((HyperLink)e.Item.FindControl("lnkPODView")).Target = "_blank";
                        ((HyperLink)e.Item.FindControl("lnkPODView")).NavigateUrl = itemPOD.ScannedFormPDF.Trim();
					}
					((RdoBtnGrouper) e.Item.FindControl("rbgCollectionDrop")).Visible = false;
					((HyperLink) e.Item.FindControl("lnkPODScanning")).Text = "Ticket number " + ticketNo;
                    ((HyperLink)e.Item.FindControl("lnkPODScanning")).NavigateUrl = @"javascript:OpenPODWindowForEdit(" + ((int)dsPOD.Tables[0].Rows[0]["ScannedFormID"]).ToString() + "," + JobId + ");";

					m_assignedPodCount++;
				}
			}

			ViewState["AssignedPODCount"] = m_assignedPodCount;
		}

		protected void dgCollectionDropSummary_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem) 
			{
				DataGrid dgCollectionDrop = (DataGrid)e.Item.FindControl("dgCollectionDrop");

				// Get the default DataView of the Collection Drop
				DataView dvCollectDrop = m_dsJob.Tables["CollectionDrop"].DefaultView;

				// Filter by the Collect Drop Summary ID of the current DataGridItem 
				dvCollectDrop.RowFilter = "InstructionID=" + e.Item.Cells[0].Text;

				dgCollectionDrop.DataSource = dvCollectDrop;
                if (m_job.JobType == eJobType.Return)
                {
                    dgCollectionDrop.Columns[2].Visible = false;
                    dgCollectionDrop.Columns[3].Visible = false;
                    dgCollectionDrop.Columns[4].Visible = false;
                    dgCollectionDrop.Columns[5].Visible = true;
                    dgCollectionDrop.Columns[6].Visible = true;
                    //dgCollectionDrop.Columns[7].Visible = true;
                }
				dgCollectionDrop.DataBind();
			}		
		}

		protected void dgCollectionDrop_PreRender(object sender, EventArgs e)
		{
			bool hasCheckboxForUnassign = false;
			bool hasRadioButtonForAssign = false;

			foreach (DataGridItem item in ((DataGrid) sender).Items)
			{
				if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
				{
					if (!hasCheckboxForUnassign && ((CheckBox) item.FindControl("chkUnassignPOD")).Visible)
						hasCheckboxForUnassign = true;
					if (!hasRadioButtonForAssign && ((RdoBtnGrouper) item.FindControl("rbgCollectionDrop")).Visible)
						hasRadioButtonForAssign = true;
				}
			}

            // Commented out as the grid appears to handle everything correctly.
            // Stephen Newman 07/02/06
            //if (!hasCheckboxForUnassign)
            //    ((DataGrid)sender).Columns[7].Visible = false;
            //if (!hasRadioButtonForAssign || !dgUnassignedPODs.Visible)
            //    ((DataGrid)sender).Columns[8].Visible = false;
		}

        protected void dgUnattachedDeHire_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            m_assignedPodCount = (int)ViewState["AssignedPODCount"];

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Orchestrator.Facade.POD facPOD = new Orchestrator.Facade.POD();
                Orchestrator.Facade.Form facForm = new Orchestrator.Facade.Form();

                int orderID = int.Parse(e.Item.Cells[0].Text);
                int scannedFormId = -1;
                int dehireReceiptId = int.Parse(e.Item.Cells[1].Text);
                string receiptNumber = e.Item.Cells[2].Text;

                Entities.Scan dehireReceiptScan = null;
                
                if(int.TryParse(e.Item.Cells[3].Text, out scannedFormId) && scannedFormId > 0)
                    dehireReceiptScan = facForm.GetForScannedFormId(scannedFormId);

                if (dehireReceiptScan == null)
                {
                    string date = e.Item.Cells[5].Text;
                    string truncatedDate = date.Substring(0, date.LastIndexOf(' '));
                    string urlEncodedDate = Server.UrlEncode(truncatedDate);

                    ((CheckBox)e.Item.FindControl("chkUnassignPOD")).Visible = false;
                    ((HyperLink)e.Item.FindControl("lnkPODScanning")).Text = "Scan Dehire Receipt";
                    ((HyperLink)e.Item.FindControl("lnkPODScanning")).NavigateUrl = @"javascript:OpenDehireWindow(" + JobId + "," + orderID + "," + dehireReceiptId + ",'" + receiptNumber + "');";
                }
                else
                {
                    if (dehireReceiptScan.ScannedFormId > 0)
                    {
                        ((HyperLink)e.Item.FindControl("lnkPODView")).Visible = true;
                        ((HyperLink)e.Item.FindControl("lnkPODView")).Target = "_blank";
                        ((HyperLink)e.Item.FindControl("lnkPODView")).NavigateUrl = dehireReceiptScan.ScannedFormPDF.Trim();
                    }

                    ((RdoBtnGrouper)e.Item.FindControl("rbgCollectionDrop")).Visible = false;
                    ((HyperLink)e.Item.FindControl("lnkPODScanning")).Text = "Dehire Receipt Number " + receiptNumber;
                    ((HyperLink)e.Item.FindControl("lnkPODScanning")).NavigateUrl = @"javascript:OpenDehireWindowForEdit(" + dehireReceiptScan.ScannedFormId.ToString() + "," + JobId + "," + orderID + "," + dehireReceiptId + ",'" + receiptNumber + "');";

                    m_assignedPodCount++;
                }
            }

            ViewState["AssignedPODCount"] = m_assignedPodCount;
        }

		#endregion

		#region Populate Page's DataGrids

        private void PopulatePCVS()
        {

            Facade.IPCV facPCV = new Facade.PCV();
            DataSet ds = facPCV.GetForJobIdDataSet(m_jobId);
            dgPCVs.DataSource = ds;
            dgPCVs.DataBind();

            if (ds.Tables[0].Rows.Count > 0)
                pnlPCVs.Visible = true;
        }

        private void PopulateRefusals()
        {

            Facade.IGoodsRefusal facRefusals = new Facade.GoodsRefusal();
            DataSet ds = facRefusals.GetGoodsRefusedForJob(JobId);
            dgRefusals.DataSource = ds;
            dgRefusals.DataBind();

            if (ds.Tables[0].Rows.Count > 0)
                pnlRefusals.Visible = true;
        }

		private void PopulatePODs()
		{
			ViewState["AssignedPODCount"] = 0;

            // Only show the centrally scanned pods if the option is set in tblSystem.
            bool centrallyScannedPODsActive = Facade.SystemSettings.CentrallyScannedPODsActive();

            Facade.IPOD facPOD = new Facade.POD();
            m_dsJob = facPOD.GetJobDetails(m_jobId);

            dgCollectionDropSummary.DataSource = facPOD.GetJobDetails(m_jobId);
            dgCollectionDropSummary.DataMember = "CollectionDropSummary";
            dgCollectionDropSummary.DataBind();

            DataSet ds = facPOD.GetJobDetailsWithOutLeg(m_jobId);
            dgUnattachedDeHire.DataSource = ds;
            dgUnattachedDeHire.DataBind();

            if (m_jobId > 0)
            {
                DataSet dsUnassignedPODs = facPOD.GetUnassignedToJobByClientId(m_job.IdentityId);
                if (dsUnassignedPODs.Tables[0].Rows.Count > 0)
                {
                    pnlUnassignedPODs.Visible = true && centrallyScannedPODsActive;
                    dgUnassignedPODs.DataSource = dsUnassignedPODs;
                }
                else
                    pnlUnassignedPODs.Visible = false && centrallyScannedPODsActive;
            }
            else
                dgUnassignedPODs.DataSource = facPOD.GetUnassignedToJob();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                pnlUnattachedDeHire.Visible = true;
            else
                pnlUnattachedDeHire.Visible = false;

            try
            {
                dgUnassignedPODs.DataBind();
            }
            catch (Exception)
            {
                dgUnassignedPODs.CurrentPageIndex = 0;
                dgUnassignedPODs.DataBind();
            }

            if ((int)ViewState["AssignedPODCount"] > 0)
                btnUnassignPODs.Visible = true;
            else
                btnUnassignPODs.Visible = false;
		}

		#endregion

		#region Event Handlers

		protected void btnAddToJob_Click(object sender, System.EventArgs e)
		{
			int collectDropId = 0;
			int PODId = 0;
			Entities.POD podToAssign = null;

			// Find the Collect Drop ID for the selected collect drop portion
			foreach(DataGridItem cdsItem in dgCollectionDropSummary.Items) 
			{
				DataGrid dgCollectionDrop = (DataGrid) cdsItem.FindControl("dgCollectionDrop");
				foreach(DataGridItem cdItem in dgCollectionDrop.Items) 
				{
					RdoBtnGrouper rbgCollectionDrop = (RdoBtnGrouper) cdItem.FindControl("rbgCollectionDrop");
					if(rbgCollectionDrop.Checked) 
						collectDropId = int.Parse(cdItem.Cells[0].Text);
				}
			}

			// Find the POD ID of the selected POD
			foreach(DataGridItem PODItem in dgUnassignedPODs.Items)
			{
				RdoBtnGrouper rbgPODId = (RdoBtnGrouper) PODItem.FindControl("rbgPODId");
				if(rbgPODId.Checked) 
				{
					PODId = int.Parse(PODItem.Cells[0].Text);
					podToAssign = new Entities.POD(PODId, PODItem.Cells[1].Text, DateTime.Parse(PODItem.Cells[2].Text), m_jobId, collectDropId);
				}
			}
	
			if(podToAssign != null && collectDropId > 0 && PODId > 0) 
			{
				Facade.IPOD facPOD = new Facade.POD();
				facPOD.AssignToJob(podToAssign, ((Entities.CustomPrincipal) Page.User).UserName);
			}
			else
			{
				lblAddToJobError.Text = "Select POD and drop to which to assign.";
				lblAddToJobError.Visible = true;
			}
			PopulatePODs();
		}

		protected void btnUnassignPODs_Click(object sender, System.EventArgs e)
		{
			Facade.IPOD facPOD = new Facade.POD();
			
			ArrayList arrayListPODIds = new ArrayList();
	
			foreach(DataGridItem cdsItem in dgCollectionDropSummary.Items) 
			{
				DataGrid dgCollectionDrop = (DataGrid) cdsItem.FindControl("dgCollectionDrop");
				foreach(DataGridItem cdItem in dgCollectionDrop.Items)
				{	
					if(((CheckBox) cdItem.FindControl("chkUnassignPOD")).Checked) 
					{
						DataSet dsPOD = facPOD.GetForCollectDropID(int.Parse(cdItem.Cells[0].Text));
						arrayListPODIds.Add((int)dsPOD.Tables[0].Rows[0]["PODId"]);
					}
				}
			}

			int[] toUnassignPODIds = new int[arrayListPODIds.Count];

			for (int counter=0; counter<arrayListPODIds.Count; counter++)
			{
				toUnassignPODIds[counter] = (int)arrayListPODIds[counter];
			}

			if (arrayListPODIds.Count > 0 && ((int) arrayListPODIds[0]) > 0)
			{
				lblUnassignPODsError.Visible = false;
				facPOD.UnassignFromJob(toUnassignPODIds, ((Entities.CustomPrincipal) Page.User).UserName);
			}
			else
			{
				lblUnassignPODsError.Text = "Please mark POD(s) to unassign";
				lblUnassignPODsError.Visible = true;
			}
			PopulatePODs();
		}

		private void DisplayProgress()
		{
			if (!m_job.HasBeenPosted)
				lblProgress.Text = "This job has been invoiced.";
			else
				lblProgress.Text = "This job is now ready for pricing.";
			hlProgress.Text = "Price this job";
			hlProgress.NavigateUrl = "Pricing2.aspx?jobId=" + m_jobId + "&csid=xx";
		}
	
		protected void btnUpdateJobState_Click(object sender, System.EventArgs e)
		{
			bool retVal = false;

			Facade.IJob facJob = new Facade.Job();

		    string userName = ((Entities.CustomPrincipal) Page.User).UserName;
		    if (m_job.JobType == eJobType.Groupage)
            {
                m_job.IsPriced = chkBookingInComplete.Checked;
                facJob.Update(m_job, userName);
            }

			if (chkBookingInComplete.Checked)
			{
				if (m_job.IsPriced) 
					retVal = facJob.UpdateState(m_job.JobId, eJobState.ReadyToInvoice, userName);
				else
                    retVal = facJob.UpdateState(m_job.JobId, eJobState.BookingInComplete, userName);
				chkBookingInComplete.Checked = true;
				pnlUnassignedPODs.Enabled = false;
				pnlUpdateJob.Enabled = true;
				DisplayProgress();
			}
			else if (!chkBookingInComplete.Checked)
			{
				m_job.JobState = eJobState.BookingInIncomplete;

                retVal = facJob.UpdateState(m_job.JobId, eJobState.BookingInIncomplete, userName);

				chkBookingInComplete.Checked = false;
				pnlUnassignedPODs.Enabled = true;
				pnlUpdateJob.Enabled = true;
				pnlProgress.Visible = false;
			}

			if (retVal)
				lblConfirmation.Text = "The job has been updated successfully.";
			else
				lblConfirmation.Text = "The job has not been successfully updated.";

			lblConfirmation.Visible = true;
		}

        private void dgPCVs_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                int pcvId = Convert.ToInt32(e.Item.Cells[0].Text);
                int palletCount = Convert.ToInt32(e.Item.Cells[4].Text);
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
            dgPCVs.ItemDataBound += new DataGridItemEventHandler(dgPCVs_ItemDataBound);
            this.dlgDocumentWizard.DialogCallBack += (o, ev) => { this.Response.Redirect(this.Request.Url.ToString()); };
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
