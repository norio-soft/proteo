using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Telerik.Web.UI;

namespace Orchestrator.WebUI.Pallet
{
	/// <summary>
	/// Summary description for AlterPalletBalance.
	/// </summary>
	public partial class AlterPalletBalance : Orchestrator.Base.BasePage
    {
        #region Properties

        private const string vs_jobId = "vs_jobId";
        public int JobId
        {
            get { return ViewState[vs_jobId] == null ? -1 : (int)ViewState[vs_jobId]; }
            set { ViewState[vs_jobId] = value; }
        }

        private const string vs_resourceId = "vs_resourceId";
        public int ResourceId
        {
            get { return ViewState[vs_resourceId] == null ? -1 : (int)ViewState[vs_resourceId]; }
            set { ViewState[vs_resourceId] = value; }
        }

        private const string vs_palletTypeId = "vs_palletTypeId";
        public int PalletTypeId
        {
            get { return ViewState[vs_palletTypeId] == null ? -1 : (int)ViewState[vs_palletTypeId]; }
            set { ViewState[vs_palletTypeId] = value; }
        }

        private const string vs_noOfPallets = "vs_noOfPallets";
        public int NoOfPallets
        {
            get { return ViewState[vs_noOfPallets] == null ? -1 : (int)ViewState[vs_noOfPallets]; }
            set { ViewState[vs_noOfPallets] = value; }
        }

        private const string vs_noOfPalletsUpdated = "vs_noOfPalletsUpdated";
        public bool NoOfPalletsUpdated
        {
            get { return ViewState[vs_noOfPalletsUpdated] == null ? false : (bool)ViewState[vs_noOfPalletsUpdated]; }
            set { ViewState[vs_noOfPalletsUpdated] = value; }
        }

        #endregion

        protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.PalletBalanceManagement);

            if (!IsPostBack)
                ConfigureDisplay();
		}

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            //ramAlterPalletBalance.AjaxRequest += new RadAjaxControl.AjaxRequestDelegate(ramAlterPalletBalance_AjaxRequest);

            rcbDeHireOrganisation.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(rcbDeHireOrganisation_ItemsRequested);

            btnLeavePallets.Click += new EventHandler(btnLeavePallets_Click);
            btnDeHirePallets.Click += new EventHandler(btnDeHirePallets_Click);

            btnClose.Click += new EventHandler(btnClose_Click);
        }

        private void ConfigureDisplay()
        {
            int l_jobId, l_resourceId, l_palletTypeId, l_noOfPallets;

            if (int.TryParse(Request.QueryString["JobId"], out l_jobId))
                JobId = l_jobId;

            if (int.TryParse(Request.QueryString["rID"], out l_resourceId))
                ResourceId = l_resourceId;

            if (int.TryParse(Request.QueryString["PTId"], out l_palletTypeId))
                PalletTypeId = l_palletTypeId;

            if (int.TryParse(Request.QueryString["noP"], out l_noOfPallets))
                NoOfPallets = l_noOfPallets;

            if (JobId == -1 || PalletTypeId == -1 || NoOfPallets == -1 || ResourceId == -1)
            {
                divNoDetails.Style.Add("display", "");
                divPalletHandling.Style.Add("display", "none");
            }
            else
            {
                DisplayControls();
            }
        }

        private void DisplayControls()
        {
            var PalletTypes = Facade.PalletType.GetAllPalletTypes().Tables[0].Rows.Cast<DataRow>().AsEnumerable();
            var selectedPalletType = PalletTypes.FirstOrDefault(pts => pts.Field<int>("PalletTypeId") == PalletTypeId);

            lblRunID.Text = JobId.ToString();
            lblPalletType.Text = selectedPalletType.Field<string>("Description");

            rcbDehireRecieptType.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eDehiringReceiptType)));
            rcbDehireRecieptType.DataBind();

            // Setting currency culture
            CultureInfo systemCulture = new CultureInfo(Orchestrator.Globals.Configuration.NativeCulture);
            rntPalletDeliveryCharge.Culture = systemCulture;

            ResetControls();

            divNoDetails.Style.Add("display", "none");
            divPalletHandling.Style.Add("display", "");
        }

        #region Events

        #region AjaxManger

        void ramAlterPalletBalance_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            string retVal = string.Empty;

            int palletAction = -1;
            if (int.TryParse(e.Argument, out palletAction))
            {
                switch((eInstructionType)palletAction)
                {
                    case eInstructionType.LeavePallets:
                        LeavePallets();
                        break;

                    case eInstructionType.DeHirePallets:
                        DehirePallets();
                        break;
                }
            }
        }

        #endregion

        #region Buttons

        void btnDeHirePallets_Click(object sender, EventArgs e)
        {
            DehirePallets();
        }

        void btnLeavePallets_Click(object sender, EventArgs e)
        {
            LeavePallets();
        }

        void btnClose_Click(object sender, EventArgs e)
        {
            if (NoOfPalletsUpdated)
                this.ReturnValue = "Refresh_Pallets";

            this.Close();
        }

        #endregion

        #region ComboBox

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

        #endregion

        #region Private Functions

        private void ResetControls()
        {
            lblNoOfPallets.Text = NoOfPallets.ToString();

            #region Leave Pallets

            ucLeavePoint.Reset();
            rntNoLeavePallets.Value = null;
            rntNoLeavePallets.MaxValue = NoOfPallets;
            rntNoLeavePallets.MinValue = NoOfPallets < 1 ? -1 : 0;

            #endregion

            #region De-hire Pallets

            rcbDeHireOrganisation.ClearSelection();
            ucDeliveryPoint.Reset();
            txtDeHireReceipt.Text = string.Empty;
            rntNoDehirePallets.Value = null;
            rntNoDehirePallets.MaxValue = NoOfPallets;
            rntNoDehirePallets.MinValue = NoOfPallets < 1 ? -1 : 0;
            rntPalletDeliveryCharge.Value = null;

            #endregion
        }

        private void LeavePallets()
        {
            int pointId, noOfPallets;
            string userID = ((Page.User) as Orchestrator.Entities.CustomPrincipal).UserName;

            Entities.Order leaveOrder = new Entities.Order();

            pointId = ucLeavePoint.SelectedPoint.PointId;
            noOfPallets = (int)rntNoLeavePallets.Value.Value;

            #region Generate Leave Order

            // Setting default values for the Order.
            leaveOrder.Cases = 0;
            leaveOrder.Weight = 0;
            leaveOrder.GoodsTypeID = 1; // This REALLY needs to be Pallets ( hint hint system definition ).
            leaveOrder.PalletSpaceID = (int)ePalletSize.Normal;
            leaveOrder.CustomerOrderNumber = "palletHandling";
            leaveOrder.DeliveryOrderNumber = "palletHandling";
            leaveOrder.OrderStatus = eOrderStatus.Delivered; //This is because the order will not be on a run so it is assumed its at the dehire point at time of creation.
            leaveOrder.OrderType = eOrderType.PalletHandling;
            leaveOrder.OrderInstructionID = 3; // Find out what this is?
            leaveOrder.CustomerIdentityID = Orchestrator.Globals.Configuration.IdentityId;
            leaveOrder.PalletTypeID = PalletTypeId;
            leaveOrder.NoPallets = noOfPallets;
            leaveOrder.PalletSpaces = leaveOrder.NoPallets;
            leaveOrder.CollectionPointID = leaveOrder.DeliveryPointID = pointId;

            leaveOrder.ForeignRate = leaveOrder.Rate = 0m;

            leaveOrder.CollectionByDateTime = leaveOrder.DeliveryDateTime = leaveOrder.CollectionDateTime = leaveOrder.DeliveryFromDateTime = DateTime.Now;
            leaveOrder.CollectionIsAnytime = leaveOrder.DeliveryIsAnytime = false;

            leaveOrder.FuelSurchargePercentage = 0m;
            leaveOrder.FuelSurchargeAmount = leaveOrder.FuelSurchargeForeignAmount = 0m;

            leaveOrder.LastRunId = JobId;

            #endregion

            Facade.IPalletBalance facPalletBalance = new Facade.Pallet();
            NoOfPalletsUpdated = facPalletBalance.LeavePalletsAtPointWithNoLeg(leaveOrder, ResourceId, userID);

            int currentBalance = NoOfPallets;
                       
            // Have any pallets been handled ?
            NoOfPalletsUpdated = (currentBalance - noOfPallets) < NoOfPallets;

            if (NoOfPalletsUpdated)
                NoOfPallets = (currentBalance - noOfPallets);            

            if (NoOfPallets == 0)
                DisplayControls();
            else
                ResetControls();
        }

        private void DehirePallets()
        {
            int noOfPallets = (int)rntNoDehirePallets.Value;
            string userID = ((Page.User) as Orchestrator.Entities.CustomPrincipal).UserName;
            Entities.Order dehireOrder = new Entities.Order();
            Entities.DehiringReceipt dehireReceipt = new Entities.DehiringReceipt();

            #region Generate De-hire Order

            // Setting default values for the Order.
            dehireOrder.Cases = 0;
            dehireOrder.Weight = 0;
            dehireOrder.GoodsTypeID = 1; // This REALLY needs to be Pallets ( hint hint system definition ).
            dehireOrder.PalletSpaceID = (int)ePalletSize.Normal;
            dehireOrder.CustomerOrderNumber = "palletHandling";
            dehireOrder.DeliveryOrderNumber = "palletHandling";
            dehireOrder.OrderStatus = eOrderStatus.Delivered; //This is because the order will not be on a run so it is assumed its at the dehire point at time of creation.
            dehireOrder.OrderType = eOrderType.PalletHandling;
            dehireOrder.OrderInstructionID = 3; // Find out what this is?
            dehireOrder.CustomerIdentityID = int.Parse(rcbDeHireOrganisation.SelectedValue);
            dehireOrder.PalletTypeID = PalletTypeId;
            dehireOrder.NoPallets = noOfPallets;
            dehireOrder.PalletSpaces = dehireOrder.NoPallets;
            dehireOrder.CollectionPointID = dehireOrder.DeliveryPointID = ucDeliveryPoint.SelectedPoint.PointId;
            
            dehireOrder.ForeignRate = dehireOrder.Rate = rntPalletDeliveryCharge.Value.HasValue ? (decimal)rntPalletDeliveryCharge.Value : 0m;

            dehireOrder.CollectionDateTime = dehireOrder.DeliveryFromDateTime = new DateTime(dteDeliveryFromDate.SelectedDate.Value.Year, dteDeliveryFromDate.SelectedDate.Value.Month, dteDeliveryFromDate.SelectedDate.Value.Day, dteDeliveryFromTime.SelectedDate.Value.Hour, dteDeliveryFromTime.SelectedDate.Value.Minute, 0);
            dehireOrder.CollectionByDateTime = dehireOrder.DeliveryDateTime = new DateTime(dteDeliveryByDate.SelectedDate.Value.Year, dteDeliveryByDate.SelectedDate.Value.Month, dteDeliveryByDate.SelectedDate.Value.Day, dteDeliveryByTime.SelectedDate.Value.Hour, dteDeliveryByTime.SelectedDate.Value.Minute, 0);
            dehireOrder.CollectionIsAnytime = dehireOrder.DeliveryIsAnytime = rdDeliveryIsAnytime.Checked;

            dehireOrder.FuelSurchargePercentage = 0m;
            dehireOrder.FuelSurchargeAmount = dehireOrder.FuelSurchargeForeignAmount = 0m;

            dehireOrder.LastRunId = JobId;

            #endregion

            #region Generation De-hire Receipt

            eDehiringReceiptType dehireReceiptType = (eDehiringReceiptType)Enum.Parse(typeof(eDehiringReceiptType), rcbDehireRecieptType.SelectedValue.Replace(" ", ""));

            dehireReceipt.JobID = JobId;
            dehireReceipt.ReceiptNumber = txtDeHireReceipt.Text;
            dehireReceipt.ReceiptType = dehireReceiptType;
            dehireReceipt.NumberOfPallets = noOfPallets;
            dehireReceipt.DateIssued = dehireOrder.DeliveryFromDateTime;

            #endregion

            Facade.IPalletBalance facPalletBalance = new Facade.Pallet();
            facPalletBalance.DehirePalletsWithNoLeg(dehireOrder, dehireReceipt, ResourceId, userID);

            int currentBalance = NoOfPallets;

            // Have any pallets been handled ?
            NoOfPalletsUpdated = (currentBalance - noOfPallets) < NoOfPallets;

            if (NoOfPalletsUpdated)
                NoOfPallets = (currentBalance - noOfPallets);

            if (NoOfPallets == 0)
                DisplayControls();
            else
                ResetControls();
        }

        #endregion

    }
}
