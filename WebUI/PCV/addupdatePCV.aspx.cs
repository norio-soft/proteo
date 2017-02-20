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

namespace Orchestrator.WebUI.Resource.PCV
{
	/// <summary>
	/// Summary description for addupdatepcv.
	/// </summary>
	public partial class addupdatePCV : Orchestrator.Base.BasePage
	{	
		#region Page Variables
		private bool			m_isUpdate = false;
		private int				m_jobId = 0;
		private int				m_PCVId = 0;
		private Entities.PCV	pcv;
		#endregion 

		#region Form Elements
		#endregion
	
		#region Page/Load/Init/Error
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage, eSystemPortion.TakeCallIn);

			// New PCV
			m_jobId = Convert.ToInt32(Request.QueryString["jobId"]);
			if (m_jobId > 0 )
				m_isUpdate = false;

			// Update PCV
			m_PCVId = Convert.ToInt32(Request.QueryString["PCVId"]);   		
			
			if (m_PCVId > 0 )
				m_isUpdate = true;
		
			if (!IsPostBack)
			{
				PopulateStaticControls();
				if (m_isUpdate)
					LoadPCV();
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

		}
		#endregion
		
		#region Populate Static Controls
		private void SetupTableDeliveryPoint()
		{
			// Load the delivery points depending on the m_JobId
			Facade.IPoint facPoints = new Facade.Point();
			DataSet dsD = facPoints.GetDeliveryPointsForJobId(m_jobId);

			dgDeliveryPoint.DataSource = dsD;
			dgDeliveryPoint.DataBind();
		}

		///	<summary> 
		/// Populate Static Controls
		///	</summary>
		private void PopulateStaticControls()
		{
			//-----------------------------------------------------------------------------
			// New Section
			//-----------------------------------------------------------------------------
			SetupTableDeliveryPoint();
			
			string[] pcvStatusNames = Enum.GetNames(typeof(ePCVStatus));
			cboPCVStatus.DataSource = pcvStatusNames;
			cboPCVStatus.DataBind();

			string[] pcvRedemptionStatusNames = Enum.GetNames(typeof(ePCVRedemptionStatus));
			cboPCVRedemptionStatus.DataSource = pcvRedemptionStatusNames;
			cboPCVRedemptionStatus.DataBind();
			
			txtJobPCVId.Text = m_jobId.ToString();
		
			//-----------------------------------------------------------------------------
			// Update Section
			//-----------------------------------------------------------------------------
			if (m_isUpdate) 
			{
				lblJobPCV.Text = "PCV No";
				txtJobPCVId.Text = m_PCVId.ToString();
				btnAdd.Text = "Update PCV";
				
				// Voucher Number and Delivery Point Cannot Be Edited
				txtVoucherNo.Enabled = false;
		        dgDeliveryPoint.Enabled = false;
			}
		}
		
  		#endregion 

		#region Add/Update/Load/Populate PCV
		///	<summary> 
		/// Load PCV
		///	</summary>
		private void LoadPCV()
		{
			// May be required to be  shown if PCV's are allowed to be deleted once created
			//chkDelete.Visible = true;
			//pnlPCVDeleted.Visible = true;

			if (ViewState["pcv"]==null)
			{
				Facade.IPCV facPCV = new Facade.PCV();
				pcv = facPCV.GetForPCVId(m_PCVId);
				
				ViewState["pcv"] = pcv;
			}
			else
				pcv = (Entities.PCV)ViewState["pcv"];


			if (m_isUpdate)
				txtJobPCVId.Text = pcv.PCVId.ToString(); 
			else
				txtJobPCVId.Text = m_jobId.ToString();
		
			txtVoucherNo.Text = pcv.VoucherNo.ToString();
           	txtNoOfPallets.Text = pcv.NoOfPalletsReceived.ToString();
            dteDateOfIssue.SelectedDate = pcv.DateOfIssue;
			txtDepotId.Text = pcv.DepotId.ToString(); 
          	cboPCVStatus.Items.FindByValue(pcv.PCVStatusId.ToString()).Selected = true;
			cboPCVRedemptionStatus.Items.FindByValue(pcv.PCVRedemptionStatusId.ToString()).Selected = true;  

			// Load dgDeliveryPoints with the relevant fields and markers
			int deliveryPointId = pcv.DeliverPointId;
			
			// Get point and mark it checked in the grid
			Facade.IPoint facPoint = new Facade.Point();
			DataSet ds = facPoint.GetPointForPointId(deliveryPointId, "DataSet");
			dgDeliveryPoint.DataSource = ds;
			dgDeliveryPoint.DataBind(); 
			
			foreach(DataGridItem dgItem in dgDeliveryPoint.Items)
			{
				// Get RdoBtnGrouper object
				RdoBtnGrouper selectRadioButton = dgItem.FindControl("selectRadioButton") as RdoBtnGrouper;
				
				// Make the row highlight the radiobutton 
				selectRadioButton.Checked = true;  
			}

			txtSignings.Text = pcv.NoOfSignings.ToString();  

			//if (pcv.PCVStatus == ePCVStatus.NotApplicable)
			//	chkDelete.Checked = true;

			Header1.Title = "Update PCV";
			Header1.subTitle = "Please make any changes neccessary.";
			btnAdd.Text = "Update";
		}
		
		
		///	<summary> 
		/// Populate PCV
		///	</summary>
		private void populatePCV()
		{
	        if (ViewState["pcv"] ==null)
				pcv = new Entities.PCV();
			else
				pcv = (Entities.PCV)ViewState["pcv"];
			
			pcv.PCVId = Convert.ToInt32(txtJobPCVId.Text);
			pcv.VoucherNo = Convert.ToInt32(txtVoucherNo.Text);
			pcv.NoOfPalletsReceived = Convert.ToInt32(txtNoOfPallets.Text);
			pcv.DateOfIssue = dteDateOfIssue.SelectedDate.Value;
			pcv.PCVStatusId =  (ePCVStatus)Enum.Parse(typeof(ePCVStatus),(cboPCVStatus.Items[cboPCVStatus.SelectedIndex].Value));
			pcv.PCVRedemptionStatusId = (ePCVRedemptionStatus)Enum.Parse(typeof(ePCVRedemptionStatus),(cboPCVRedemptionStatus.Items[cboPCVRedemptionStatus.SelectedIndex].Value));
			pcv.DeliverPointId = FindSelectedPoint(); 
			pcv.DepotId = Convert.ToInt32(txtDepotId.Text);
			pcv.NoOfSignings =  Convert.ToInt32(txtSignings.Text); 

			// Deleted checked not required until the PCV's are allowed to be deleted
//			if (chkDelete.Checked)
//				pcv.PCVStatus = ePCVStatus.NotApplicable;
//			else
//				pcv.PCVStatus = ePCVStatus.Received;
		}
   		
	
		///	<summary> 
		/// Update PCV
		///	</summary>
		private bool UpdatePCV()
		{
			Facade.IPCV facPCV = new Facade.PCV();
			bool retVal = false;
			string userName = ((Entities.CustomPrincipal)Page.User).UserName;
			dgDeliveryPoint.Enabled = true;
			retVal = facPCV.Update(pcv, userName);
			dgDeliveryPoint.Enabled = false;
			return retVal;
		}
     		
	
		///	<summary> 
		/// Add PCV
		///	</summary>
		private bool AddPCV ()
		{
			int PCVId = 0;
			Facade.IPCV facPCV = new Facade.PCV();
            bool retVal = false;
			string userName = ((Entities.CustomPrincipal)Page.User).UserName;
            int deliveryPointId = FindSelectedPoint();
			
			if (deliveryPointId == 0)
			{
				lblConfirmation.Text = "Please select a Delivery Point and then try again, please.";
				lblConfirmation.Visible = true;
				lblConfirmation.ForeColor = Color.Red;
				return retVal;
			}
			else
			{
				pcv.DeliverPointId = deliveryPointId;
				PCVId = facPCV.Create(pcv, m_jobId, userName);
			}
            
			if (PCVId == 0)
			{
				lblConfirmation.Text = "There was an error adding the PCV, please try again.";
				lblConfirmation.Visible = true;
				lblConfirmation.ForeColor = Color.Red;
				retVal = false;
			}
			else
				lblJobPCV.Text = "PCV Id";
				txtJobPCVId.Text = PCVId.ToString(); 
				btnAdd.Text = "Update PCV"; 
				retVal = true;	
			
			return retVal;
		}


		private int FindSelectedPoint()
		{
			foreach(DataGridItem dgItem in dgDeliveryPoint.Items)
			{
				// Get RdoBtnGrouper object
				RdoBtnGrouper selectRadioButton = dgItem.FindControl("selectRadioButton") as RdoBtnGrouper;

				if(selectRadioButton != null && selectRadioButton.Checked)
				{
					int pointId = Convert.ToInt32(selectRadioButton.Attributes["Value"]);
                    return pointId;
				}
			}

			// There are no buttons selected
			return 0;
		}
 		#endregion

		#region Methods and Functions
		///	<summary> 
		/// Button Add_Click
		///	</summary>
		protected void btnAdd_Click(object sender, System.EventArgs e)
		{
			bool retVal = false;

			if (pcv==null)
				populatePCV();

			if (m_isUpdate) 
				retVal = UpdatePCV();
			else
				retVal = AddPCV();

			if (m_isUpdate) 
				lblConfirmation.Text = "The PCV has been updated successfully.";
			else
				lblConfirmation.Text = "The PCV has been added successfully.";
            			
			lblConfirmation.Visible=true;
		}
       #endregion
     }
}
