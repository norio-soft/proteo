using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Telerik.Web.UI;
using P1TP.Components.Web.UI;

namespace Orchestrator.WebUI.Pallet
{
	/// <summary>
	/// Summary description for ViewPalletBalances.
	/// </summary>
	public partial class ViewPalletBalances : Orchestrator.Base.BasePage
	{
		#region Form Elements

        protected		Uri				m_pointAddressUri;

		#endregion

        #region Properties

        private const string s_palletDeliveries = "s_palletDeliveries";

        private const string vs_updatePalletBalances = "vs_updatePalletBalances";
        protected bool UpdatePalletBalances
        {
            get 
            { 
                if(ViewState[vs_updatePalletBalances] == null)
                    ViewState[vs_updatePalletBalances] = Orchestrator.WebUI.Security.Authorise.CanAccess(eSystemPortion.PalletBalanceManagement);

                return (bool)ViewState[vs_updatePalletBalances];
            }
        }

        private const string s_selectedPalletTypeCounts = "s_selectedPalletTypeCounts";
        public List<ArrayList> SelectedPalletTypeCounts
        {
            get { return Session[s_selectedPalletTypeCounts] == null ? null : (List<ArrayList>)Session[s_selectedPalletTypeCounts]; }
            set { Session[s_selectedPalletTypeCounts] = value; }
        }

        #endregion

        #region Page Load/Init

        protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);
		}

        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            base.OnInit(e);

            this.btnRefresh.Click += new EventHandler(btnRefresh_Click);

            this.grdClient.NeedDataSource += new GridNeedDataSourceEventHandler(grdClient_NeedDataSource);
            this.grdClient.UpdateCommand += new GridCommandEventHandler(grdClient_UpdateCommand);

            this.grdPoints.NeedDataSource += new GridNeedDataSourceEventHandler(grdPoints_NeedDataSource);
            this.grdPoints.ItemDataBound += new GridItemEventHandler(grdPoints_ItemDataBound);
            this.grdPoints.UpdateCommand += new GridCommandEventHandler(grdPoints_UpdateCommand);
            this.grdPoints.PreRender += new EventHandler(grdPoints_PreRender);

            this.grdTrailers.NeedDataSource += new GridNeedDataSourceEventHandler(grdTrailers_NeedDataSource);
        }

        #endregion

        #region Point Pallet balances

        void grdPoints_UpdateCommand(object source, GridCommandEventArgs e)
        {
            GridEditableItem editiedItem = e.Item as GridEditableItem;
            RadNumericTextBox rntPointPalletBalance = e.Item.FindControl("rntPointPalletBalance") as RadNumericTextBox;

            // Update the Pallet balance for the Client
            int pointID = int.Parse(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["PointID"].ToString());
            int palletTypeID = int.Parse(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["PalletTypeId"].ToString());
            int newBalance = Convert.ToInt32(rntPointPalletBalance.Value);

            int oldBalance = 0;

            using (Facade.IPalletBalance facPalletBalance = new Facade.Pallet())
                 oldBalance = facPalletBalance.GetPalletBalanceAtPointId(pointID, palletTypeID);

            int interimBalance = newBalance - oldBalance;
            newBalance = interimBalance;

            using (Facade.IPalletBalance facPalletBalance = new Facade.Pallet())
                facPalletBalance.UpdatePointPalletBalance(null, null, null, pointID, palletTypeID, newBalance, ((Entities.CustomPrincipal)Page.User).UserName);

            grdPoints.Rebind();
        }

        void grdPoints_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            GridColumn alterPointPallets = grdPoints.Columns.FindByUniqueNameSafe("AlterPointPallets");

            if (alterPointPallets != null)
                alterPointPallets.Visible = UpdatePalletBalances;

            using (Facade.IPalletBalance facPalletBalance = new Facade.Pallet())
                grdPoints.DataSource = facPalletBalance.GetPointBalances();
        }

        void grdPoints_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                RadNumericTextBox rntPalletCount = e.Item.FindControl("rntPalletCount") as RadNumericTextBox;
                HiddenField hdnPalletBalance = e.Item.FindControl("hdnPalletBalance") as HiddenField;

                if (rntPalletCount != null && hdnPalletBalance != null)
                {
                    int maxPalletCount = int.Parse(hdnPalletBalance.Value);
                    
                    if (maxPalletCount < 1) // This is so it is greater than the minvalue set on the control.
                        maxPalletCount = 1;

                    rntPalletCount.MaxValue = maxPalletCount;
                    rntPalletCount.TabIndex = (short)(e.Item.RowIndex - 1);
                }
            }
        }

        void grdPoints_PreRender(object sender, EventArgs e)
        {
            btnCreatePalletReturn.TabIndex = (short)(grdPoints.Items.Count + 1);
        }

        #endregion

        #region Client Pallet Balances

        void grdClient_UpdateCommand(object source, GridCommandEventArgs e)
        {
            if (Page.IsValid)
            {
                GridEditableItem editiedItem = e.Item as GridEditableItem;
                RadNumericTextBox rntClientPalletBalance = e.Item.FindControl("rntClientPalletBalance") as RadNumericTextBox;
                HiddenField hidWoodenPallets = e.Item.FindControl("hidWoodenPallets") as HiddenField;

                // Update the Pallet balance for the Client
                int identityID = int.Parse(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["IdentityId"].ToString());
                int palletTypeID = int.Parse(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["PalletTypeId"].ToString());
                int balance = int.Parse(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["Balance"].ToString());
                int PaperPallets = int.Parse(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["PaperPallets"].ToString());

                int oldWoodenBalance = int.Parse(hidWoodenPallets.Value);
                int newWoodenBalance = Convert.ToInt32(rntClientPalletBalance.Value);

                int oldBalance = oldWoodenBalance + PaperPallets;
                int newBalance = newWoodenBalance + PaperPallets;
                int difference = oldBalance - newBalance;

                using (Facade.IPalletBalance facPalletBalance = new Facade.Pallet())
                {
                    facPalletBalance.UpdateClientPalletBalance(null, null, null, identityID, palletTypeID, difference, ((Entities.CustomPrincipal)Page.User).UserName);
                }

                grdClient.Rebind();
            }
        }
      
        void grdClient_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            GridColumn alterClientPallets = grdClient.Columns.FindByUniqueNameSafe("AlterClientPallets");

            if (alterClientPallets != null)
                alterClientPallets.Visible = UpdatePalletBalances;

            using (Facade.IPalletBalance facPalletBalance = new Facade.Pallet())
                grdClient.DataSource = facPalletBalance.GetClientBalances();
        }

		#endregion

        #region Trailer Pallet balances

        void grdTrailers_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            GridColumn alterTrailerPallets = grdTrailers.Columns.FindByUniqueNameSafe("AlterTrailerPallets");

            if (alterTrailerPallets != null)
                alterTrailerPallets.Visible = false; // UpdatePalletBalances;

            using (Facade.IPalletBalance facPalletBalance = new Facade.Pallet())
                grdTrailers.DataSource = facPalletBalance.GetTrailerBalances();
        }

        #endregion

        #region Button

        void btnRefresh_Click(object sender, EventArgs e)
        {
            grdClient.Rebind();
            grdPoints.Rebind();
            grdTrailers.Rebind();
        }

		#endregion

        #region Web Methods

        [System.Web.Services.WebMethod]
        public static string GetSelectedPallets(string palletTypeCountCSV)
        {
            System.Web.SessionState.HttpSessionState currentSession = HttpContext.Current.Session;
            currentSession[s_selectedPalletTypeCounts] = null; // Reset Session Variable, as new call.
            currentSession[s_palletDeliveries] = null; // Reset Session Variable, as new call.

            string retVal = string.Empty;
            List<Entities.PalletTypeCount> selectedPalletTypeCounts = new List<Entities.PalletTypeCount>();

            try
            {
                if (palletTypeCountCSV.Length == 0)
                    throw new Exception();

                string[] selectedRows = palletTypeCountCSV.Split('|');

                foreach (string s in selectedRows)
                {
                    string[] rowItems = s.Split(',');

                    Entities.PalletTypeCount palletTypeDetails = new Entities.PalletTypeCount(int.Parse(rowItems[0]), int.Parse(rowItems[1]), rowItems[2], int.Parse(rowItems[3]));
                    selectedPalletTypeCounts.Add(palletTypeDetails);
                }

                currentSession[s_selectedPalletTypeCounts] = selectedPalletTypeCounts;
                retVal = "complete";
            }
            catch (Exception ex)
            {
                retVal = "There was an error retrieving your selection, please reload the page and try again.";
            }

            return retVal;
        }

        #endregion
    }
}
