using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Orchestrator.WebUI
{
    public partial class ManageBusinessType : Orchestrator.Base.BasePage
    {
        #region Page Load/Init
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                PopulateStaticControls();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.cboDefaultNominalCode.ItemDataBound +=new Telerik.Web.UI.RadComboBoxItemEventHandler(cboDefaultNominalCode_ItemDataBound);
            this.cboSubContractNominalCode.ItemDataBound += new Telerik.Web.UI.RadComboBoxItemEventHandler(cboDefaultNominalCode_ItemDataBound);
            this.cboSubContractSelfBillNominalCode.ItemDataBound += new Telerik.Web.UI.RadComboBoxItemEventHandler(cboDefaultNominalCode_ItemDataBound);

            this.btnUpdate.Click +=new EventHandler(btnUpdate_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("businesstypes.aspx");
        }

        #endregion

        #region Private Properties
        private Entities.BusinessType businessType
        {
            get { return (Entities.BusinessType)this.ViewState["_businessType"]; }
            set { this.ViewState["_businessType"] = value; }
        }

        private int BusinessTypeID
        {
            get
            {
                if (!string.IsNullOrEmpty(Request.QueryString["BTID"]))
                {
                    return int.Parse(Request.QueryString["BTID"]);
                }
                else
                    return -1;
            }
        }

        #endregion

        #region Populate Static Controls

        private void PopulateStaticControls()
        {
            trConsortium.Visible = Utilities.IsAllocationEnabled();
            
            PopulateNominalCodes();

            if (this.BusinessTypeID > 0)
                LoadBusinessType();
        }


        private void PopulateNominalCodes()
        {
            Facade.INominalCode facNominalCode = new Facade.NominalCode();
            DataSet dsNominalCode = facNominalCode.GetAllActive();

            DataTable dt = dsNominalCode.Tables[0];
            DataRow dr = dt.NewRow();
            dr["NominalCode"] = "";
            dr["Description"] = "Please Select a Nominal Code";
            dt.Rows.InsertAt(dr, 0);
            
            cboDefaultNominalCode.DataSource = dt;
            cboDefaultNominalCode.DataValueField = "NominalCodeID";
            cboSubContractNominalCode.DataSource = dt;
            cboSubContractNominalCode.DataValueField = "NominalCodeID";
            cboSubContractSelfBillNominalCode.DataSource = dt;
            cboSubContractSelfBillNominalCode.DataValueField = "NominalCodeID";

            cboDefaultNominalCode.DataBind();
            cboSubContractNominalCode.DataBind();
            cboSubContractSelfBillNominalCode.DataBind();
           
            cboDefaultNominalCode.Items[0].Selected = true;
            cboSubContractNominalCode.Items[0].Selected = true;
            cboSubContractNominalCode.Items[0].Selected = true;
        }

        private void LoadBusinessType()
        {
            Facade.IBusinessType facBT = new Facade.BusinessType();
            this.businessType = facBT.GetForBusinessTypeID(this.BusinessTypeID);

            txtDescription.Text = this.businessType.Description;
            rblDeBreifRequired.ClearSelection();
            rblDeBreifRequired.Items[0].Selected = this.businessType.RequireDeBreif;
            rblDeBreifRequired.Items[1].Selected = !this.businessType.RequireDeBreif;
            rblShowCreateJob.Items[0].Selected = this.businessType.ShowCreateJob;
            rblShowCreateJob.Items[1].Selected = !this.businessType.ShowCreateJob;
            rblCreateJobChecked.Items[0].Selected = this.businessType.CreateJobChecked;
            rblCreateJobChecked.Items[1].Selected = !this.businessType.CreateJobChecked;
            rblExcludeFromConsortiumMemberFileExport.SelectedIndex = this.businessType.ExcludeFromConsortiumMemberFileExport ? 0 : 1;
            rblIsBarcodeScannedOnDelivery.SelectedIndex = this.businessType.IsBarcodeScannedOnDelivery ? 0 : 1;
            rblIsCustomerNameCapturedOnCollection.SelectedIndex = this.businessType.IsCustomerNameCapturedOnCollection ? 0 : 1;
            rblIsCustomerNameCapturedOnDelivery.SelectedIndex = this.businessType.IsCustomerNameCapturedOnDelivery ? 0 : 1;
            rblIsCustomerSignatureCapturedOnCollection.SelectedIndex = this.businessType.IsCustomerSignatureCapturedOnCollection ? 0 : 1;
            rblIsCustomerSignatureCapturedOnDelivery.SelectedIndex = this.businessType.IsCustomerSignatureCapturedOnDelivery ? 0 : 1;
            rblMwfBypassCleanClausedScreen.SelectedIndex = this.businessType.MwfBypassCleanClausedScreen ? 0 : 1;
            rblMwfBypassCommentsScreen.SelectedIndex = this.businessType.MwfBypassCommentsScreen ? 0 : 1;
            rblMwfConfirmCallIn.SelectedIndex = this.businessType.MwfConfirmCallIn ? 0 : 1;

            txtPalletThresholdMin.Text = this.businessType.PalletThresholdMin.ToString();
            txtPalletThresholdMax.Text = this.businessType.PalletThresholdMax.ToString();

            chkPalletNetwork.Checked = this.businessType.IsPalletNetwork;
            txtPalletNetworkExportDepotCode.Text = this.businessType.PalletNetworkExportDepotCode;

            if (this.businessType.NominalCode.NominalCodeID > 0)
            {
                cboDefaultNominalCode.ClearSelection();
                cboDefaultNominalCode.FindItemByValue(this.businessType.NominalCode.NominalCodeID.ToString()).Selected = true;
            }

            if (this.businessType.SubContractNominalCode.NominalCodeID > 0)
            {
                cboSubContractNominalCode.ClearSelection();
                cboSubContractNominalCode.FindItemByValue(this.businessType.SubContractNominalCode.NominalCodeID.ToString()).Selected = true;
            }

            if (this.businessType.SubContractSelfBillNominalCode.NominalCodeID > 0)
            {
                cboSubContractSelfBillNominalCode.ClearSelection();
                cboSubContractSelfBillNominalCode.FindItemByValue(this.businessType.SubContractSelfBillNominalCode.NominalCodeID.ToString()).Selected = true;
            }

            btnUpdate.Text = "Update";
        }
        #endregion

        #region Combo Box Events

        void cboDefaultNominalCode_ItemDataBound(object o, Telerik.Web.UI.RadComboBoxItemEventArgs e)
        {
            e.Item.Text = (e.Item.DataItem as DataRowView)["Description"].ToString();
        }
        #endregion

        #region Button Events
        void btnUpdate_Click(object sender, EventArgs e)
        {
            UpdateBusinessType();          

        }

        private void UpdateBusinessType()
        {
            // Populate the BusinessType
            PopulateBusinessType();

            Facade.IBusinessType facBusinessType = new Facade.BusinessType();
            Entities.FacadeResult retVal  = new Orchestrator.Entities.FacadeResult();
            bool AllowNominalCodeReUseForBusinessTypes = Orchestrator.Globals.Configuration.AllowNominalCodeReUseForBusinessTypes;
            if (this.BusinessTypeID > 0)
                retVal = facBusinessType.Update(this.businessType, this.Page.User.Identity.Name, AllowNominalCodeReUseForBusinessTypes);
            else
                retVal = facBusinessType.Create(this.businessType, this.Page.User.Identity.Name, AllowNominalCodeReUseForBusinessTypes);
            if (retVal.Success)
            {
                Response.Redirect("businesstypes.aspx");
            }
            else
            {
                ucInfringements.Infringements = retVal.Infringements;
                ucInfringements.DisplayInfringments();
            }
        }

        
        #endregion

        #region Entity Handling

        private void PopulateBusinessType()
        {

            if (this.businessType == null)
                this.businessType = new Orchestrator.Entities.BusinessType();

            this.businessType.Description = txtDescription.Text;
            this.businessType.PalletThresholdMin = int.Parse(txtPalletThresholdMin.Text);
            this.businessType.PalletThresholdMax = int.Parse(txtPalletThresholdMax.Text);

            this.businessType.IsPalletNetwork = chkPalletNetwork.Checked;
            this.businessType.PalletNetworkExportDepotCode = txtPalletNetworkExportDepotCode.Text;

            if (cboDefaultNominalCode.SelectedValue != "")
                this.businessType.NominalCode.NominalCodeID = int.Parse(cboDefaultNominalCode.SelectedValue);
            else
                this.businessType.NominalCode.NominalCodeID = 0;

            if (cboSubContractNominalCode.SelectedValue != "")
                this.businessType.SubContractNominalCode.NominalCodeID = int.Parse(cboSubContractNominalCode.SelectedValue);
            else
                this.businessType.SubContractNominalCode.NominalCodeID = 0;

            if (cboSubContractSelfBillNominalCode.SelectedValue != "")
                this.businessType.SubContractSelfBillNominalCode.NominalCodeID = int.Parse(cboSubContractSelfBillNominalCode.SelectedValue);
            else
                this.businessType.SubContractSelfBillNominalCode.NominalCodeID = 0;

            this.businessType.RequireDeBreif = bool.Parse(rblDeBreifRequired.SelectedValue);
            this.businessType.ShowCreateJob = bool.Parse(rblShowCreateJob.SelectedValue);
            this.businessType.CreateJobChecked = bool.Parse(rblCreateJobChecked.SelectedValue);
            this.businessType.ExcludeFromConsortiumMemberFileExport = bool.Parse(rblExcludeFromConsortiumMemberFileExport.SelectedValue);
            this.businessType.IsBarcodeScannedOnDelivery = bool.Parse(rblIsBarcodeScannedOnDelivery.SelectedValue);
            this.businessType.IsCustomerNameCapturedOnCollection = bool.Parse(rblIsCustomerNameCapturedOnCollection.SelectedValue);
            this.businessType.IsCustomerNameCapturedOnDelivery = bool.Parse(rblIsCustomerNameCapturedOnDelivery.SelectedValue);
            this.businessType.IsCustomerSignatureCapturedOnCollection = bool.Parse(rblIsCustomerSignatureCapturedOnCollection.SelectedValue);
            this.businessType.IsCustomerSignatureCapturedOnDelivery = bool.Parse(rblIsCustomerSignatureCapturedOnDelivery.SelectedValue);
            this.businessType.MwfBypassCleanClausedScreen = bool.Parse(rblMwfBypassCleanClausedScreen.SelectedValue);
            this.businessType.MwfBypassCommentsScreen = bool.Parse(rblMwfBypassCommentsScreen.SelectedValue);
            this.businessType.MwfConfirmCallIn = bool.Parse(rblMwfConfirmCallIn.SelectedValue);
        }

        #endregion

    }
}