using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Orchestrator.WebUI.Pallet
{
    public partial class AddUnattachedPCV : Orchestrator.Base.BasePage
    {
        private const string injectScript = @"<script>loadPCVCreatedWindow({0}, {1});</script>";

        #region Page

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ConfigureDisplay();
                LoadPCVList();
            }
        }

        protected override void OnInit(System.EventArgs e)
        {
 	        base.OnInit(e);

            btnAdd.Click += new System.EventHandler(btnAdd_Click);
            btnAddAndReset.Click += new System.EventHandler(btnAdd_Click);

            rcbClient.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(rcbClient_ItemsRequested);
            rcbClient.SelectedIndexChanged += new Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventHandler(rcbClient_SelectedIndexChanged);

            dlgPCVAdded.DialogCallBack += new EventHandler(dlgPCVAdded_DialogCallBack);
            dlgScanDocument.DialogCallBack += new EventHandler(dlgScanDocument_DialogCallBack);
        }

        #endregion

        #region Private Functions

        private void ConfigureDisplay()
        {
            rcbClient.ClearSelection();
            rcbReasonForIssue.ClearSelection();
            rcbPalletType.ClearSelection();

            rcbClient.Text = string.Empty;
            rcbReasonForIssue.Text = string.Empty;
            rcbPalletType.Text = string.Empty;

            rcbClient.Items.Clear();
            rcbReasonForIssue.Items.Clear();
            rcbPalletType.Items.Clear();

            rcbNoOfSignings.SelectedIndex = 0;
            rcbPalletType.Items.Add(new Telerik.Web.UI.RadComboBoxItem("Please select a client.", "-1"));

            ucIssuedPoint.Reset();

            rdiDate.SelectedDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0);

            rntVoucherNo.Value = null;
            rntNoOfPallets.Value = 0;

            rcbReasonForIssue.Items.Clear();
            rcbReasonForIssue.DataSource = Utilities.GetEnumPairs<ePCVReasonForIssuing>();
            rcbReasonForIssue.DataBind();
        }

        private void LoadPCVList()
        {
            Facade.IPCV facPCV = new Facade.PCV();
            lvLastPCVs.DataSource = facPCV.GetLast10UnattachedCreated();
            lvLastPCVs.DataBind();
        }

        private void ShowConfirmationWindow(int sfID, int pcvID)
        {
            litInjectScript.Text = string.Format(injectScript, sfID.ToString(), pcvID.ToString());
        }

        private Entities.PCV LoadPCV()
        {
            Entities.PCV newPCV = new Entities.PCV();

            int clientID = -1, palletTypeID = -1;
            int.TryParse(rcbClient.SelectedValue, out clientID);
            int.TryParse(rcbPalletType.SelectedValue, out palletTypeID);

            //Add PCV here
            newPCV.ClientID = clientID;
            newPCV.DateOfIssue = rdiDate.SelectedDate.Value;
            newPCV.DeliverPointId = ucIssuedPoint.PointID;
            newPCV.FormTypeId = eFormTypeId.PCV;
            newPCV.NoOfPallets = (int)rntNoOfPallets.Value;
            newPCV.NoOfPalletsReceived = 0;
            newPCV.NoOfPalletsReturned = 0;
            newPCV.NoOfSignings = int.Parse(rcbNoOfSignings.SelectedValue);
            newPCV.PalletTypeID = palletTypeID;
            newPCV.PCVIssued = true;
            newPCV.PCVReasonForIssuingId = (ePCVReasonForIssuing)int.Parse(rcbReasonForIssue.SelectedValue);
            newPCV.PCVRedemptionStatusId = ePCVRedemptionStatus.ToBeRedeemed;
            newPCV.PCVStatusId = ePCVStatus.Received;
            newPCV.VoucherNo = (int)rntVoucherNo.Value;
            newPCV.RequiresScan = true;

            return newPCV;
        }

        #endregion

        #region Events

        #region Dialogs

        void dlgPCVAdded_DialogCallBack(object sender, EventArgs e)
        {
            LoadPCVList();
        }

        void dlgScanDocument_DialogCallBack(object sender, EventArgs e)
        {
            LoadPCVList();
        }

        #endregion

        #region Drop Down List

        void rcbClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            rcbClient.Items.Clear();
            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllClientsFiltered("%" + e.Text, true);
            DataTable dt = ds.Tables[0];
            DataTable boundResults = dt.Clone();

            int itemsPerRequest = 15;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > dt.Rows.Count)
                endOffset = dt.Rows.Count;

            for (int i = itemOffset; i < endOffset; i++)
                boundResults.ImportRow(dt.Rows[i]);

            rcbClient.DataSource = boundResults;
            rcbClient.DataBind();

            if (dt.Rows.Count > 0)
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
        }

        void rcbClient_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            int clientID;
            int.TryParse(e.Value, out clientID);

            if (clientID > 0)
            {
                if (rcbPalletType.Items.Count > 0)
                    rcbPalletType.Items.Clear();

                DataSet dsPalletTypes = Orchestrator.Facade.PalletType.GetAllPalletTypes(clientID);

                IEnumerable<DataRow> activeRows;
                var queryPallets = dsPalletTypes.Tables[0].Rows.Cast<DataRow>().AsEnumerable();

                // Is the pallet type active?
                Func<DataRow, bool> isActive = dr => dr.Field<bool>("IsActive");
                Func<DataRow, bool> isDefault = dr => dr.Field<bool>("IsDefault");

                if (queryPallets.Any(isActive))
                    activeRows = queryPallets.Where(isActive);
                else
                    activeRows = queryPallets;

                rcbPalletType.DataSource = from ar in activeRows select new { Description = ar.Field<string>("Description"), PalletTypeID = ar.Field<int>("PalletTypeID") };
                rcbPalletType.DataBind();

                // Note there is an assumption that if "IsDefault" is true then so is "IsActive"
                DataRow defaultRow = activeRows.FirstOrDefault(isDefault);
                if (defaultRow != null)
                    rcbPalletType.SelectedIndex = rcbPalletType.FindItemIndexByValue(defaultRow["PalletTypeID"].ToString());
            }
            else
                rcbPalletType.Items.Add(new Telerik.Web.UI.RadComboBoxItem("No Pallet types Active", "-1"));
        }

        #endregion

        #region Button

        void btnAdd_Click(object sender, System.EventArgs e)
        {
            if (Page.IsValid)
            {
                Button currentButton = sender as Button;

                Facade.IPCV facPCV = new Facade.PCV();
                Facade.ITrafficArea facTrafficArea = new Facade.Traffic();

                Entities.PCV newPCV = LoadPCV();

                int pcvID = facPCV.Create(newPCV, ((Entities.CustomPrincipal)Page.User).UserName);

                //Do reset if specified
                switch (currentButton.CommandArgument.ToLower())
                {
                    case "add_reset":
                        ConfigureDisplay();
                        break;
                }

                //Popup PCV Details
                if (pcvID > 0)
                    ShowConfirmationWindow(newPCV.ScannedFormId, pcvID);

                LoadPCVList();
            }
        }

        #endregion

        #endregion
    }
}