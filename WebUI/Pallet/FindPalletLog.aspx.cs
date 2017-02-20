using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;


using Orchestrator.Globals;
using P1TP.Components.Web.Validation;

namespace Orchestrator.WebUI.Pallet
{
	/// <summary>
	/// Summary description for FindPalletLog.
	/// </summary>
	public partial class FindPalletLog : Orchestrator.Base.BasePage
	{
		#region Constants & Enums
		private const int C_HOURSPREVIOUS = 4;	// Past hours to show
		private const int C_HOURSFOLLOWING = 4;	// Following hours to show
		private const int C_HOURSTOTAL = 36;	// Total hours to show
		private const int C_HOURSMOVE = 6;		// Hours to move when going back / forward
		
		private const string C_HOURS_PREVIOUS_VS = "C_HOURS_PREVIOUS_VS";
		private const string C_HOURS_FOLLOWING_VS = "C_HOURS_FOLLOWING_VS";

		private static readonly TimeSpan C_TIMESPAN = new TimeSpan(0, 0, 30, 0, 0);
		private static readonly TimeSpan C_MOVEONREFRESH = C_TIMESPAN;
		#endregion

		#region Page Variables


		#endregion

		#region Property Interfaces
		private int HoursPrevious
		{
			get
			{
				if (ViewState[C_HOURS_PREVIOUS_VS] == null)
				{
					HoursPrevious = 6;
					return 6;
				}
				else
					return (int) ViewState[C_HOURS_PREVIOUS_VS];
			}
			set
			{
				ViewState[C_HOURS_PREVIOUS_VS] = value;
			}
		}

		private int HoursFollowing
		{
			get
			{
				if (ViewState[C_HOURS_FOLLOWING_VS] == null)
				{
					HoursFollowing = 3;
					return 3;
				}
				else
					return (int) ViewState[C_HOURS_FOLLOWING_VS];
			}
			set
			{
				ViewState[C_HOURS_FOLLOWING_VS] = value;
			}
		}

		#endregion

		#region Page Elements
		#endregion
	
		#region Page/Load/Init/Error
		///	<summary> 
		///	Page Load
		///	</summary>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

			if (!IsPostBack)
				PopulateStaticControls();
		}
		#endregion

		#region DBCombo's Server Methods and Initialisation 

		void cboClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
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

		
		#endregion

		#region Populate Static Controls
		///	<summary> 
		///	Populate Static Fields
		///	</summary>
		private void PopulateStaticControls()
		{
			DateTime startWeek = DateTime.UtcNow;
			TimeSpan toBegin = new TimeSpan(((int) startWeek.DayOfWeek) + 6, 0,0,0);
			startWeek = startWeek.Subtract(toBegin);
			
			//startWeek = startWeek.Subtract(startWeek.TimeOfDay);
            dteStartDate.SelectedDate = startWeek; 
			
			// Add default 7 days to end date
			dteEndDate.SelectedDate = startWeek.AddDays(6);

		}
		#endregion
  		
		#region Methods and Functions
		protected void btnSearch_Click(object sender, System.EventArgs e)
		{
			btnSearch.DisableServerSideValidation();

			if (Page.IsValid)
				performSearch();
			else
				reportViewer.Visible = false;
		}        
		
		private void btnClear_Click(object sender, EventArgs e)
		{
			cboClient.Text = String.Empty;
			cboClient.SelectedValue = String.Empty;
				chkPCVRequired.Checked = false;

			PopulateStaticControls();

			reportViewer.Visible = false;
		}

		private void performSearch()
		{
			// Configure the report settings collection
			NameValueCollection reportParams = new NameValueCollection();
			
			int clientId = 0;
			
			if (cboClient.Text != "")
			{
				clientId = Convert.ToInt32 (cboClient.SelectedValue);
				reportParams.Add("Client", cboClient.Text);
				reportParams.Add("IdentityId", clientId.ToString ());
			}

			reportParams.Add("startDate", dteStartDate.SelectedDate.Value.ToString("dd/MM/yy"));
			
			reportParams.Add("endDate", dteEndDate.SelectedDate.Value.ToString("dd/MM/yy"));
	
			DataSet dsPallets; 

			DateTime start = dteStartDate.SelectedDate.Value;
			start = start.Subtract(start.TimeOfDay);

			DateTime end = dteEndDate.SelectedDate.Value;
			end = end.Subtract(end.TimeOfDay).Add(new TimeSpan(23, 59, 59));

			bool PCVRequired = false;

			if (chkPCVRequired.Checked)
				PCVRequired = true;
			else
				PCVRequired = false;

			Facade.IPallet facPallet = new Facade.Pallet();
			
			dsPallets = facPallet.GetPalletLogforIdentityId(clientId, start, end, PCVRequired); 
			
			// Configure the Session variables used to pass data to the report
			Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.PalletLog;
			Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsPallets;
			Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
			Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";
			Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;

			reportViewer.IdentityId = clientId;
			// Show the user control
			reportViewer.Visible = true;
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
			this.btnClear.Click += new EventHandler(btnClear_Click);
            this.cboClient.ItemsRequested +=new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);

		}
		#endregion
	}
}
