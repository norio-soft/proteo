using System;
using System.Data;
using System.Web.UI.WebControls;

using Telerik.Web.UI;
using P1TP.Components.Web.Validation;

namespace Orchestrator.WebUI.Organisation
{
    public partial class ChangeReference : Orchestrator.Base.BasePage
    {
        private const string c_AllSelectedWorkForOrganisation_VS = "c_AllSelectedWorkForOrganisation_VS";
        private const string c_GetAllReferences_VS = "c_GetAllReferences_VS";
        private const string C_OrderList_VS = "C_OrderList_VS";
        private const string C_JobList_VS = "C_JobList_VS";
        private const string orderIDs = "vs_orderIDs";
        private const string jobIDs = "vs_jobIDs";
        private const string identityID = "identityID";
        private const string validator = @"";
        DataSet updatedOrders = null, updatedJobs = null;

        #region properties

        public DataSet AllSelectedWorkForOrganisation
        {
            get
            {
                if (ViewState[c_AllSelectedWorkForOrganisation_VS] == null)
                {
                    Facade.IOrganisation facOrg = new Facade.Organisation();

                    if (IdentityID > 0)
                        ViewState[c_AllSelectedWorkForOrganisation_VS] = facOrg.GetAllSelectedWorkForOrganisation(IdentityID, JobIDs, OrderIDs);
                    else
                        ViewState[c_AllSelectedWorkForOrganisation_VS] = null;
                }

                return (DataSet)ViewState[c_AllSelectedWorkForOrganisation_VS];
            }
            set { ViewState[c_AllSelectedWorkForOrganisation_VS] = null; }
        }

        public DataSet GetAllReferences
        {
            get
            {
                if (ViewState[c_GetAllReferences_VS] == null)
                {
                    Facade.IReferenceData facRef = new Facade.ReferenceData();

                    if (IdentityID > 0)
                        ViewState[c_GetAllReferences_VS] = facRef.GetAllReferences(OrderIDs, JobIDs, IdentityID);
                    else
                        ViewState[c_GetAllReferences_VS] = null;
                }

                return (DataSet)ViewState[c_GetAllReferences_VS];
            }
        }

        public string OrderIDs
        {
            get { return ViewState[orderIDs] == null ? string.Empty : ViewState[orderIDs].ToString(); }
            set { ViewState[orderIDs] = value; }
        }

        public string JobIDs
        {
            get { return ViewState[jobIDs] == null ? string.Empty : ViewState[jobIDs].ToString(); }
            set { ViewState[jobIDs] = value; }
        }

        public int IdentityID
        {
            get { return ViewState[identityID] == null ? -1 : int.Parse(ViewState[identityID].ToString()); }
            set { ViewState[identityID] = value; }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                ConfigureDisplay();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.btnUpdate.Click += new EventHandler(btnUpdate_Click);
            this.btnClose.Click += new EventHandler(btnClose_Click);
            
            this.cboReference.SelectedIndexChanged += new Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventHandler(cboReference_SelectedIndexChanged);
            
            this.grdJobs.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(grdJobs_ItemDataBound);
            this.grdOrders.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(grdOrders_ItemDataBound);

            this.cfvTxtValueDecimal.ServerValidate += new ServerValidateEventHandler(validatorControl_ServerValidateDecimal);
            this.cfvTxtValueInteger.ServerValidate += new ServerValidateEventHandler(validatorControl_ServerValidateInteger);
        }

        #region Private functions

        private void ConfigureDisplay()
        {
            if (Request.QueryString["clientID"] != null && IdentityID < 0)
                IdentityID = int.Parse(Request.QueryString["clientID"].ToString());

            if (Session[C_JobList_VS] != null && Session[C_OrderList_VS] != null)
            {
                JobIDs = Session[C_JobList_VS].ToString();
                OrderIDs = Session[C_OrderList_VS].ToString();
            }

            cboReference.Items.Clear();
            cboReference.DataSource = GetAllReferences;
            cboReference.DataBind();

            if (JobIDs.Length < 1)
            {
                cboReference.Items.Insert(0, new Telerik.Web.UI.RadComboBoxItem("Delivery Order Number", "0"));
                cboReference.Items.Insert(1, new Telerik.Web.UI.RadComboBoxItem("Customer Order Number", "0"));
            }

            if (cboReference.Items.Count < 1)
            {
                cboReference.Enabled = false;
                txtValue.Enabled = false;
                btnUpdate.Enabled = false;

                txtValue.BackColor = System.Drawing.Color.LightGray;
            }
            else
                txtValue.BackColor = System.Drawing.Color.White;


            BindGrids();
        }

        private void BindGrids()
        {
            if (AllSelectedWorkForOrganisation != null)
            {
                AllSelectedWorkForOrganisation.Tables[0].DefaultView.RowFilter = "OrderID IS NULL";
                if (AllSelectedWorkForOrganisation.Tables[0].DefaultView.Count > 0)
                {
                    grdJobs.DataSource = AllSelectedWorkForOrganisation.Tables[0].DefaultView;
                    grdJobs.DataBind();
                }
                else
                    grdJobs.Visible = false;

                AllSelectedWorkForOrganisation.Tables[0].DefaultView.RowFilter = "OrderID IS NOT NULL";
                if (AllSelectedWorkForOrganisation.Tables[0].DefaultView.Count > 0)
                {
                    grdOrders.DataSource = AllSelectedWorkForOrganisation.Tables[0].DefaultView;
                    grdOrders.DataBind();
                }
                else
                    grdOrders.Visible = false;
            }
            else
            {
                grdJobs.Visible = false;
                grdOrders.Visible = false;
            }
        }

        #endregion

        #region Events

        #region Buttons

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                int organisationReferenceId = int.Parse(cboReference.SelectedValue);
                string userId = ((Entities.CustomPrincipal)Page.User).UserName;
                Facade.IOrder facOrd = new Facade.Order();
                Facade.IReferenceData facRef = new Facade.ReferenceData();


                if (organisationReferenceId == 0)
                    switch (cboReference.SelectedItem.Text)
                    {
                        case "Delivery Order Number":
                            updatedOrders = facOrd.UpdateMultiplOrderReferences(OrderIDs, txtValue.Text, false, userId);
                            break;
                        case "Customer Order Number":
                            updatedOrders = facOrd.UpdateMultiplOrderReferences(OrderIDs, txtValue.Text, true, userId);
                            break;
                    }
                else
                {
                    if (OrderIDs.Length > 0)
                        updatedOrders = facRef.UpdateReferencesForOrderIDs(OrderIDs, txtValue.Text, organisationReferenceId, userId);

                    if (JobIDs.Length > 0)
                        updatedJobs = facRef.UpdateReferencesForJobIDs(JobIDs, txtValue.Text, organisationReferenceId, userId);
                }

                AllSelectedWorkForOrganisation = null;
                BindGrids();
                updatedOrders = updatedJobs = null;
            }
        }

        #endregion

        #region Grids

        void grdOrders_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem && updatedOrders != null)
                if (updatedOrders.Tables[0].Rows.Count > 0)
                {
                    DataRowView drv = (DataRowView)e.Item.DataItem;
                    updatedOrders.Tables[0].DefaultView.RowFilter = "OrderID = " + drv.Row["OrderID"].ToString();

                    if (updatedOrders.Tables[0].DefaultView.Count > 0)
                    {
                        bool isUpdatedOrder = false;

                        int referenceID = 0;
                        int.TryParse(cboReference.SelectedValue, out referenceID);

                        if (referenceID > 0)
                            isUpdatedOrder = (int)drv["OrganisationReferenceID"] == referenceID;
                        else
                            isUpdatedOrder = true;

                        if (isUpdatedOrder)
                            e.Item.BackColor = System.Drawing.Color.Coral;
                    }
                }
        }

        void grdJobs_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem && updatedJobs != null)
                if (updatedJobs.Tables[0].Rows.Count > 0)
                {
                    DataRowView drv = (DataRowView)e.Item.DataItem;
                    updatedJobs.Tables[0].DefaultView.RowFilter = "JobID = " + drv.Row["JobID"].ToString();

                    if (updatedJobs.Tables[0].DefaultView.Count > 0 && ((int)drv.Row["OrganisationReferenceID"] == Convert.ToInt32(cboReference.SelectedValue) || Convert.ToInt32(cboReference.SelectedValue) == 0))
                        e.Item.BackColor = System.Drawing.Color.Coral;
                }
        }

        #endregion

        #region Drop Down Lists

        void cboReference_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            // Configure the validator controls
            int dataType = 0;
            string dataTypeDescription = string.Empty;
            int organisationReferenceId = int.Parse(cboReference.SelectedValue);

            if (organisationReferenceId > 0)
            {
                GetAllReferences.Tables[0].DefaultView.RowFilter = "OrganisationReferenceID = " + organisationReferenceId;
                dataType = int.Parse(GetAllReferences.Tables[0].DefaultView[0].Row["DataTypeID"].ToString());
                dataTypeDescription = GetAllReferences.Tables[0].DefaultView[0].Row["DataTypeDescription"].ToString();
            }
            else
            {
                dataType = organisationReferenceId;
                dataTypeDescription = e.Text;
            }

            switch (dataType)
            {
                case (int)eOrganisationReferenceDataType.Decimal:
                    cfvTxtValueDecimal.Enabled = true;
                    cfvTxtValueInteger.Enabled = false;
                    break;
                case (int)eOrganisationReferenceDataType.FreeText:
                    cfvTxtValueDecimal.Enabled = false;
                    cfvTxtValueInteger.Enabled = false;
                    break;
                case (int)eOrganisationReferenceDataType.Integer:
                    cfvTxtValueDecimal.Enabled = false;
                    cfvTxtValueInteger.Enabled = true;
                    break;
                default:
                    break;
            }

            if (cfvTxtValueDecimal.Enabled || cfvTxtValueInteger.Enabled)
            {
                // Configure the validator properties
                cfvTxtValueDecimal.ErrorMessage = cfvTxtValueInteger.ErrorMessage = "Please supply a " + dataTypeDescription + ".";
                cfvTxtValueDecimal.Text = cfvTxtValueInteger.Text = "<img src=\"../images/ico_critical_small.gif\"  Title=\"Please supply a " + dataTypeDescription + ".\" />";
            }
        }

        #endregion

        #endregion

        #region Validators

        private void validatorControl_ServerValidateDecimal(object source, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(args.Value, 0, false);
        }

        private void validatorControl_ServerValidateInteger(object source, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(args.Value, 0, true);
        }

        #endregion
    }
}