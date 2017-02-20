using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;

using Telerik.Web.UI;
using System.Globalization;

namespace Orchestrator.WebUI.Pallet
{
    public partial class PalletHandlingWizard : Orchestrator.Base.BasePage
    {
        protected struct PalletDelivery
        {
            public string Destination;
            public string PalletType;
            public decimal Charge;
            public eInstructionType PalletAction;
            public Entities.Order PalletOrder;

            public PalletDelivery(Entities.Order palletOrder, eInstructionType palletAction, decimal charge, string destination, string pallettype)
            {
                PalletOrder = palletOrder;
                PalletAction = palletAction;
                Charge = charge;
                Destination = destination;
                PalletType = pallettype;
            }

            public string PalletActionDescription
            {
                get
                {
                    return Utilities.UnCamelCase(PalletAction.ToString());
                }
            }

            public string DeliveryDateFormatted
            {
                get
                {
                    string retVal = string.Empty;

                    if (PalletOrder.DeliveryIsAnytime)
                        retVal = string.Format("{0} Anytime", PalletOrder.DeliveryDateTime.ToShortDateString());
                    else if (PalletOrder.DeliveryDateTime == PalletOrder.DeliveryFromDateTime)
                        retVal = string.Format("{0} at {1}", PalletOrder.DeliveryDateTime.ToShortDateString(), PalletOrder.DeliveryDateTime.ToShortTimeString());
                    else
                        retVal = string.Format("From {0} {1} To {2} {3}", PalletOrder.DeliveryFromDateTime.ToShortDateString(), PalletOrder.DeliveryFromDateTime.ToShortTimeString(), PalletOrder.DeliveryDateTime.ToShortDateString(), PalletOrder.DeliveryDateTime.ToShortTimeString());

                    return retVal;
                }
            }
        }

        private const string s_selectedPalletTypeCounts = "s_selectedPalletTypeCounts";
        private List<Entities.PalletTypeCount> _selectedPalletTypeCounts = null;
        public List<Entities.PalletTypeCount> SelectedPalletTypeCounts
        {
            get 
            {
                if (_selectedPalletTypeCounts == null)
                    _selectedPalletTypeCounts = (List<Entities.PalletTypeCount>)Session[s_selectedPalletTypeCounts];
    
                return _selectedPalletTypeCounts; 
            }
        }

        private const string s_palletDeliveries = "s_palletDeliveries";
        protected List<PalletDelivery> PalletDeliveries
        {
            get 
            {
                if (Session[s_palletDeliveries] == null)
                    Session[s_palletDeliveries] = new List<PalletDelivery>();

                return (List<PalletDelivery>)Session[s_palletDeliveries]; 
            }
            set { Session[s_palletDeliveries] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.PalletBalanceManagement);

            if (!IsPostBack)
                ConfigureDisplay();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnClose.Click += new EventHandler(btnClose_Click);
            btnCreatePalletDelivery.Click += new EventHandler(btnCreatePalletDelivery_Click);

            rcbDeHireOrganisation.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(rcbDeHireOrganisation_ItemsRequested);
        }

        private void ConfigureDisplay()
        {
            Rebind();

            // Bind the handle method
            rcbPalletHandlingAction.Items.Clear();
            rcbPalletHandlingAction.Items.Add(new RadComboBoxItem(Utilities.UnCamelCase(eInstructionType.LeavePallets.ToString()), ((int)eInstructionType.LeavePallets).ToString()));
            rcbPalletHandlingAction.Items.Add(new RadComboBoxItem(Utilities.UnCamelCase(eInstructionType.DeHirePallets.ToString()), ((int)eInstructionType.DeHirePallets).ToString()));
            rcbPalletHandlingAction.ClearSelection();

            // Setting currency culture
            CultureInfo systemCulture = new CultureInfo(Orchestrator.Globals.Configuration.NativeCulture);
            rntPalletDeliveryCharge.Culture = systemCulture;
        }

        private void Rebind()
        {
            var groupPalletTypes = from grouped in SelectedPalletTypeCounts
                                   group grouped by grouped.PalletTypeID into g
                                   select new
                                   {
                                       PalletTypeID = g.Key,
                                       PalletDescription = g.First(r => r.PalletTypeID == g.Key).PalletTypeDescription,
                                       PBalance = g.Sum(r => r.SelectedBalance),
                                       PlannedBalance = (from created in PalletDeliveries
                                                         where created.PalletOrder.PalletTypeID == g.Key
                                                         select created.PalletOrder.NoPallets).Sum()
                                   };

            lvSelectedPalletDetails.DataSource = groupPalletTypes;
            lvSelectedPalletDetails.DataBind();

            lvPalletDeliveries.DataSource = PalletDeliveries;
            lvPalletDeliveries.DataBind();

            // Only allow pallet types that have a balance remaining.
            var allowPalletTypes = groupPalletTypes.Where(gpt => gpt.PlannedBalance != gpt.PBalance);
            rcbSelectedPalletType.ClearSelection();
            rcbSelectedPalletType.Items.Clear();
            rcbSelectedPalletType.DataSource = allowPalletTypes;
            rcbSelectedPalletType.DataBind();

            rcbSelectedPalletType.SelectedIndex = 0;
            int palletTypeId = 0;

            if (allowPalletTypes.Count() > 0)
            {
                palletTypeId = int.Parse(rcbSelectedPalletType.SelectedValue);
                var palletBalanceForType = groupPalletTypes.FirstOrDefault(gpt => gpt.PalletTypeID == palletTypeId);

                if (palletBalanceForType != null)
                    rntSelectedPallets.MaxValue = palletBalanceForType.PBalance - palletBalanceForType.PlannedBalance;
            }
        }

        private void ClearSelection()
        {
            rntSelectedPallets.Value = null;
            rntPalletDeliveryCharge.Value = null;
            rcbDeHireOrganisation.ClearSelection();
            ucDeliveryPoint.Reset();
            dteCollectionFromDate.SelectedDate = null;
            dteCollectionFromTime.SelectedDate = null;
            dteCollectionByDate.SelectedDate = null;
            dteCollectionByTime.SelectedDate = null;
            dteDeliveryFromDate.SelectedDate = null;
            dteDeliveryFromTime.SelectedDate = null;
            dteDeliveryByDate.SelectedDate = null;
            dteDeliveryByTime.SelectedDate = null;
        }

        #region Events

        #region  Buttons

        void btnClose_Click(object sender, EventArgs e)
        {
            Session[s_selectedPalletTypeCounts] = null;
            PalletDeliveries = null;

            this.Close();
        }

        void btnCreatePalletDelivery_Click(object sender, EventArgs e)
        {
            eInstructionType selectedPalletAction;

            Entities.Order palletOrder = new Entities.Order();

            // Setting default values for the Order.
            palletOrder.Cases = 0;
            palletOrder.Weight = 0;
            palletOrder.GoodsTypeID = 1; // This REALLY needs to be Pallets ( hint hint system definition ).
            palletOrder.PalletSpaceID = (int)ePalletSize.Normal;
            palletOrder.CustomerOrderNumber = "palletHandling";
            palletOrder.DeliveryOrderNumber = "palletHandling";
            palletOrder.OrderStatus = eOrderStatus.Approved;
            palletOrder.OrderType = eOrderType.PalletHandling;
            palletOrder.OrderInstructionID = 3; // Find out what this is?
            
            if (rcbDeHireOrganisation.SelectedValue != string.Empty)
                palletOrder.CustomerIdentityID = int.Parse(rcbDeHireOrganisation.SelectedValue);
            else
                palletOrder.CustomerIdentityID = Orchestrator.Globals.Configuration.IdentityId;

            palletOrder.PalletTypeID = int.Parse(rcbSelectedPalletType.SelectedValue);
            palletOrder.NoPallets = rntSelectedPallets.Value.HasValue ? (int)rntSelectedPallets.Value : 0;
            palletOrder.PalletSpaces = palletOrder.NoPallets;
            palletOrder.DeliveryPointID = ucDeliveryPoint.SelectedPoint.PointId;
            palletOrder.ForeignRate = palletOrder.Rate = rntPalletDeliveryCharge.Value.HasValue ? (decimal)rntPalletDeliveryCharge.Value : 0m;

            palletOrder.CollectionDateTime = new DateTime(dteCollectionFromDate.SelectedDate.Value.Year, dteCollectionFromDate.SelectedDate.Value.Month, dteCollectionFromDate.SelectedDate.Value.Day, dteCollectionFromTime.SelectedDate.Value.Hour, dteCollectionFromTime.SelectedDate.Value.Minute, 0);
            palletOrder.CollectionByDateTime = new DateTime(dteCollectionByDate.SelectedDate.Value.Year, dteCollectionByDate.SelectedDate.Value.Month, dteCollectionByDate.SelectedDate.Value.Day, dteCollectionByTime.SelectedDate.Value.Hour, dteCollectionByTime.SelectedDate.Value.Minute, 0);
            palletOrder.CollectionIsAnytime = rdCollectionIsAnytime.Checked;

            palletOrder.DeliveryFromDateTime = new DateTime(dteDeliveryFromDate.SelectedDate.Value.Year, dteDeliveryFromDate.SelectedDate.Value.Month, dteDeliveryFromDate.SelectedDate.Value.Day, dteDeliveryFromTime.SelectedDate.Value.Hour, dteDeliveryFromTime.SelectedDate.Value.Minute, 0);
            palletOrder.DeliveryDateTime = new DateTime(dteDeliveryByDate.SelectedDate.Value.Year, dteDeliveryByDate.SelectedDate.Value.Month, dteDeliveryByDate.SelectedDate.Value.Day, dteDeliveryByTime.SelectedDate.Value.Hour, dteDeliveryByTime.SelectedDate.Value.Minute, 0);
            palletOrder.DeliveryIsAnytime = rdDeliveryIsAnytime.Checked;

            palletOrder.FuelSurchargePercentage = 0m;
            palletOrder.FuelSurchargeAmount = palletOrder.FuelSurchargeForeignAmount = 0m;

            selectedPalletAction = (eInstructionType)int.Parse(rcbPalletHandlingAction.SelectedValue);

            PalletDelivery newDelivery = new PalletDelivery(palletOrder, selectedPalletAction, palletOrder.Rate, ucDeliveryPoint.SelectedPoint.Description, rcbSelectedPalletType.SelectedItem.Text);
            PalletDeliveries.Add(newDelivery);

            ClearSelection();
            Rebind();
        }

        #endregion

        #region Combobox

        void rcbDeHireOrganisation_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllClientsFiltered(e.Text);
            DataTable dt = ds.Tables[0];
            DataTable boundResults = dt.Clone();

            int itemsPerRequest = 20;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            for (int i = itemOffset; i < endOffset; i++)
                boundResults.ImportRow(dt.Rows[i]);

            rcbDeHireOrganisation.Items.Clear();
            rcbDeHireOrganisation.DataSource = boundResults;
            rcbDeHireOrganisation.DataBind();

            if (dt.Rows.Count > 0)
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
        }

        #endregion

        #region WebMethods

        [System.Web.Services.WebMethod]
        public static string CreatePalletDeliveryRun(string userID)
        {
            System.Web.SessionState.HttpSessionState currentSession = HttpContext.Current.Session;
            string retVal = string.Empty;

            List<Entities.PalletTypeCount> palletTypeCountCollections = (List<Entities.PalletTypeCount>)currentSession[s_selectedPalletTypeCounts];
            List<PalletDelivery> palletDeliveries = (List<PalletDelivery>)currentSession[s_palletDeliveries];
            List<KeyValuePair<Entities.Order, eInstructionType>> specifiedDeliveries = new List<KeyValuePair<Entities.Order,eInstructionType>>();

            foreach(PalletDelivery pd in palletDeliveries)
                specifiedDeliveries.Add(new KeyValuePair<Entities.Order, eInstructionType>(pd.PalletOrder, pd.PalletAction));

            try
            {
                Facade.IPalletReturn facPalletReturn = new Facade.Pallet();
                int runId = facPalletReturn.Create(palletTypeCountCollections, specifiedDeliveries, userID);
                retVal = string.Format("{0}", runId);
            }
            catch (Exception ex)
            {
                WebUI.Global.UnhandledException(ex);

                string sensibleErrorMessage = ex.Message;
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    sensibleErrorMessage += "\n" + ex.Message;
                }

                throw new ApplicationException(sensibleErrorMessage);
            }

            return retVal;
        }

        #endregion

        #endregion
    }
}
