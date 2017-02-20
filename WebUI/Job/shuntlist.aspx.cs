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

namespace Orchestrator.WebUI.Job
{	 		
	/// <summary>
	/// Summary description for jobDetails.
	/// </summary>
	public partial class ShuntList : Orchestrator.Base.BasePage
	{
		#region Page Variables

		#endregion
		
       	#region Form Elements

		#endregion	
		
		#region Page Load/Init/Error
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

			if (!IsPostBack)
			{
			}
		}

        void ShuntList_Init(object sender, EventArgs e)
        {
            this.cboClient.ItemsRequested += new Telerik.WebControls.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
        }

        
        void cboClient_ItemsRequested(object o, Telerik.WebControls.RadComboBoxItemsRequestedEventArgs e)
        {

            cboClient.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllClientsFiltered(e.Text);

            int itemsPerRequest = 20;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            DataTable dt = ds.Tables[0];
            Telerik.WebControls.RadComboBoxItem rcItem = null;
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new Telerik.WebControls.RadComboBoxItem();
                rcItem.Text = dt.Rows[i]["OrganisationName"].ToString();
                rcItem.Value = dt.Rows[i]["IdentityId"].ToString();
                cboClient.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }
		#endregion
       		
		#region Populate and Display Controls/Elements

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
			this.grdDailyShuntList.InitializeRow += new Infragistics.WebUI.UltraWebGrid.InitializeRowEventHandler(this.grdCancelledJobs_InitializeRow);

		}
		#endregion
 	
		#region Page Elements Changes
		private void grdCancelledJobs_InitializeRow(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
		{
			// Normally Used To Label Buttons
		}

		protected void btnFilter_Click(object sender, System.EventArgs e)
		{
			ApplyFilter();
		}
		protected void btnFax_Click(object sender, System.EventArgs e)
		{
			//TODO: Fax section of the shunt list
		}

		protected void btnEMail_Click(object sender, System.EventArgs e)
		{
			//TODO: EMail section of the shunt list		
		}

		#endregion

		#region Methods and Functions
		public void ApplyFilter()
		{
			// Build Filter String
			// Find Shunt List For Particular Client
			DateTime today = DateTime.UtcNow;
			today = today.Subtract(today.TimeOfDay);

			Facade.Job facJob = new Facade.Job();
			DataSet dsShuntList = facJob.GetShuntListForClient(Convert.ToInt32( cboClient.Value), today);

			grdDailyShuntList.DataSource = dsShuntList;
			grdDailyShuntList.DataBind(); 
		}

		#endregion 
 	}
}
