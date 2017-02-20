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
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Traffic.Filters
{
	/// <summary>
	/// Summary description for ListFilters.
	/// </summary>
	public partial class ListFilters : Orchestrator.Base.BasePage
	{
		#region Form Elements


		#endregion

		#region Page Load/Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

		}
		
		private void ListFilters_Init(object sender, EventArgs e)
		{
			grdFilters.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdFilters_NeedDataSource);
            grdFilters.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(grdFilters_ItemDataBound);
            grdFilters.ItemCommand += new GridCommandEventHandler(grdFilters_ItemCommand);
			chkShowDeactivated.CheckedChanged += new EventHandler(chkShowDeactivated_CheckedChanged);
		}

        void grdFilters_ItemCommand(object source, GridCommandEventArgs e)
        {
            int filterID = int.Parse(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["FilterId"].ToString());
            Entities.CustomPrincipal principal = (Entities.CustomPrincipal)Page.User;

            switch (e.CommandName.ToLower())
            {
                case "setdefault":
                    using (Facade.ITrafficSheetFilter facTrafficSheetFilter = new Facade.Traffic())
                        facTrafficSheetFilter.SetDefault(filterID, principal.IdentityId, principal.UserName);
                    grdFilters.Rebind();
                    break;
                case "switchactivity":
                    GridDataItem gdi = e.Item as GridDataItem;
                    bool isActive  = (gdi["IsActive"] as TableCell).Text == "Yes" ? true : false;
                    
                    using (Facade.ITrafficSheetFilter facTrafficSheetFilter = new Facade.Traffic())
                        if (isActive)
                            facTrafficSheetFilter.Deactivate(filterID, principal.UserName);
                        else if (e.Item.Cells[5].Text.ToLower() == "no")
                            facTrafficSheetFilter.Activate(filterID, principal.UserName);
                    grdFilters.Rebind();

                    break;
            }
        }

        void grdFilters_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {

            if (e.Item is GridDataItem)
        {
            GridDataItem gdi = e.Item as GridDataItem;
            DataRowView drv = (DataRowView)gdi.DataItem;
                
                // Set the default button
                bool isDefault = ( (string)drv["IsDefault"]) == "Yes";
                (gdi["btnDefaults"] as TableCell).Controls[0].Visible = !isDefault;
                if(isDefault)
                    (gdi["btnDefaults"] as TableCell).Text = "&#160;";

                
                // Set the set activity button text
                bool isActive = ((string)drv["IsActive"]) == "Yes";
                Button btnSetActivity = (Button)(gdi["btnActivate"] as TableCell).Controls[0];
                btnSetActivity.CommandArgument = isActive.ToString();
                if (isActive)
                    btnSetActivity.Text = "Deactivate";
                else
                    btnSetActivity.Text = "Activate";
            }
        }

        void grdFilters_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            using (Facade.ITrafficSheetFilter facTrafficSheetFilter = new Facade.Traffic())
            {
                grdFilters.DataSource = facTrafficSheetFilter.GetForUser(((Entities.CustomPrincipal)Page.User).IdentityId, chkShowDeactivated.Checked);
            }
        }

		#endregion

	

		#region CheckBox Events

		private void chkShowDeactivated_CheckedChanged(object sender, EventArgs e)
		{
            grdFilters.Rebind();
		}

		#endregion

		#region DataGrid Events
        protected string GetYesNo(bool truefalse)
        {
            string retVal = truefalse == true ? "Yes" : "No";
            return retVal;
        }

        protected string GetTrafficAreas(string trafficAreas)
        {
            return GetWrappedStringFromCSV(trafficAreas);
        }

        protected string GetJobStates(string jobStates)
        {
            return GetWrappedStringFromCSV(jobStates);
        }

        protected string GetJobTypes(string jobTypes)
        {
            string retVal = string.Empty;
            if (jobTypes.Length == 0)
                return "&#160;";

            foreach (string s in jobTypes.Split(','))
            {
                retVal += Utilities.UnCamelCase(((eJobType)int.Parse(s)).ToString()) + "<br/>";
            }

            return retVal;
        }

        private static string GetWrappedStringFromCSV(string jobStates)
        {
            string retVal = string.Empty;

            foreach (string s in jobStates.Split(','))
                retVal += s + "<br/>";

            return retVal;
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
			this.Init += new EventHandler(ListFilters_Init);
		}
		#endregion
	}
}
