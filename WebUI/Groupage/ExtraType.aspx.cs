using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Telerik.Web.UI;

using Orchestrator;

namespace Orchestrator.WebUI.ExtraType
{
    public partial class ExtraType : Orchestrator.Base.BasePage
    {
        private DataSet _dsNominalCodes = null;
        private bool? _enableExtraTypeNominalCodes = null;

        public bool EnableExtraTypeNominalCodes
        {
            get
            {
                if (!_enableExtraTypeNominalCodes.HasValue)
                {
                    _enableExtraTypeNominalCodes = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["EnableExtraTypeNominalCodes"].ToString());
                }

                return (bool)_enableExtraTypeNominalCodes;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.grdExtraTypes.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdExtraTypes_NeedDataSource);
            this.grdExtraTypes.InsertCommand += new Telerik.Web.UI.GridCommandEventHandler(grdExtraTypes_InsertCommand);
            this.grdExtraTypes.UpdateCommand += new Telerik.Web.UI.GridCommandEventHandler(grdExtraTypes_UpdateCommand);
            this.grdExtraTypes.ItemCreated += new GridItemEventHandler(grdExtraTypes_ItemCreated);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Facade.ExtraType facExtraType = new Orchestrator.Facade.ExtraType();
                grdExtraTypes.DataSource = facExtraType.GetForIsEnabled(new bool?());
            }
        }

        protected void PopulateNominalCodes(object sender, EventArgs e)
        {
            DropDownList cboNominalCodes = sender as DropDownList;

            // populate the nominal codes
            if (cboNominalCodes != null && cboNominalCodes.Items.Count == 0)
            {
                if (_dsNominalCodes == null)
                {
                    Facade.INominalCode facNominalCode = new Facade.NominalCode();
                    _dsNominalCodes = facNominalCode.GetAllActive();
                }

                cboNominalCodes.Items.Insert(0, new ListItem("--- [ Please Select ] ---", "0"));

                foreach (DataRow row in _dsNominalCodes.Tables[0].Rows)
                {
                    ListItem li = new ListItem();
                    li.Value = row["NominalCodeId"].ToString();
                    li.Text = row["NominalCode"].ToString() + " - " + row["Description"].ToString();
                    cboNominalCodes.Items.Add(li);
                }
            }
        }

        void grdExtraTypes_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridEditFormItem && e.Item.IsInEditMode)
            {
                if (e.Item.OwnerTableView.IsItemInserted)
                {
                    //*******************************
                    //item is about to be inserted **
                    //*******************************
                }
                else
                {
                    //*******************************
                    //item is about to be edited   **
                    //*******************************
                    if (e.Item.DataItem != null && e.Item.DataItem.GetType() != typeof(GridInsertionObject) && e.Item.IsInEditMode && e.Item.DataItem.GetType() != typeof(GridEditFormInsertItem))
                    {
                        Entities.ExtraType et = (Entities.ExtraType)e.Item.DataItem;

                        GridEditableItem item = e.Item as GridEditableItem;
                        GridTemplateColumnEditor editor = (GridTemplateColumnEditor)item.EditManager.GetColumnEditor("NominalCode");
                        DropDownList cboNominalCodes = (DropDownList)editor.ContainerControl.FindControl("cboNominalCodes");
                        HtmlInputText txtDescription = (HtmlInputText)editor.ContainerControl.FindControl("txtDescription");
                        CheckBox chkIsEnabled = (CheckBox)editor.ContainerControl.FindControl("chkIsEnabled");
                        CheckBox chkFuelSurchargeApplies = (CheckBox)editor.ContainerControl.FindControl("chkFuelSurchargeApplies");

                        CheckBox chkIsTimeBased = (CheckBox)e.Item.FindControl("chkIsTimeBased");
                        RadTimePicker rdiStartTime = (RadTimePicker)e.Item.FindControl("rdiStartTime");
                        RadTimePicker rdiEndTime = (RadTimePicker)e.Item.FindControl("rdiEndTime");

                        CheckBox chkIsDayBased = (CheckBox)e.Item.FindControl("chkIsDayBased");
                        CheckBox chkMonday = (CheckBox)e.Item.FindControl("chkMonday");
                        CheckBox chkTuesday = (CheckBox)e.Item.FindControl("chkTuesday");
                        CheckBox chkWednesday = (CheckBox)e.Item.FindControl("chkWednesday");
                        CheckBox chkThursday = (CheckBox)e.Item.FindControl("chkThursday");
                        CheckBox chkFriday = (CheckBox)e.Item.FindControl("chkFriday");
                        CheckBox chkSaturday = (CheckBox)e.Item.FindControl("chkSaturday");
                        CheckBox chkSunday = (CheckBox)e.Item.FindControl("chkSunday");



                        if (et != null && cboNominalCodes != null)
                        {
                            txtDescription.Value = et.Description;
                            chkIsEnabled.Checked = et.IsEnabled;
                            chkFuelSurchargeApplies.Checked = et.FuelSurchargeApplies;
                            rdiStartTime.SelectedTime = et.ExtraTypeStartTime.HasValue ? TimeSpan.FromMinutes(et.ExtraTypeStartTime.Value) : new TimeSpan();
                            rdiEndTime.SelectedTime = et.ExtraTypeStartTime.HasValue ? TimeSpan.FromMinutes(et.ExtraTypeEndTime.Value) : new TimeSpan();

                            chkIsDayBased.Checked = et.IsDayBased;
                            chkMonday.Checked = et.Monday;
                            chkTuesday.Checked = et.Tuesday;
                            chkWednesday.Checked = et.Wednesday;
                            chkThursday.Checked = et.Thursday;
                            chkFriday.Checked = et.Friday;
                            chkSaturday.Checked = et.Saturday;
                            chkSunday.Checked = et.Sunday;

                            if (et.IsSystem == true)
                            {
                                txtDescription.Disabled = true;
                                chkIsEnabled.Enabled = false;
                            }

                            if (!EnableExtraTypeNominalCodes)
                            {
                                cboNominalCodes.Visible = false;
                            }

                            if(et.IsTimeBased)
                            {
                                chkIsTimeBased.Checked = true;
                            }

                            // populate the nominal codes
                            if (cboNominalCodes != null && EnableExtraTypeNominalCodes)
                            {
                                if (_dsNominalCodes == null)
                                {
                                    Facade.INominalCode facNominalCode = new Facade.NominalCode();
                                    _dsNominalCodes = facNominalCode.GetAllActive();
                                }

                                cboNominalCodes.Items.Insert(0, new ListItem("--- [ Please Select ] ---", "0"));

                                foreach (DataRow row in _dsNominalCodes.Tables[0].Rows)
                                {
                                    ListItem li = new ListItem();
                                    li.Value = row["NominalCodeId"].ToString();
                                    li.Text = row["NominalCode"].ToString() + " - " + row["Description"].ToString();
                                    cboNominalCodes.Items.Add(li);
                                }

                                // Set the currently selected nominal code.
                                if (e.Item.DataItem.GetType() == typeof(Entities.ExtraType))
                                {
                                    if (et != null && et.NominalCode.NominalCodeID > 0)
                                    {
                                        cboNominalCodes.Items.FindByValue(et.NominalCode.NominalCodeID.ToString()).Selected = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void cboNominalCodesValidator_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            // Only enforce nominal code if extra type nominal codes functionality is switched on.
            if (args.Value == 0.ToString() && EnableExtraTypeNominalCodes)
            {
                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }

        void grdExtraTypes_UpdateCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
        {
            GridEditableItem editedItem = e.Item as GridEditableItem;
            int extraTypeId = 0;

            if (!String.IsNullOrEmpty(editedItem.GetDataKeyValue("ExtraTypeId").ToString()))
                extraTypeId = Convert.ToInt32(editedItem.GetDataKeyValue("ExtraTypeId").ToString());

            if (extraTypeId > 0)
            {
                DropDownList cboNominalCodes = (DropDownList)e.Item.FindControl("cboNominalCodes");
                HtmlInputControl txtDescription = (HtmlInputControl)e.Item.FindControl("txtDescription");
                HtmlInputControl txtShortDescription = (HtmlInputControl)e.Item.FindControl("txtShortDescription");
                CheckBox chkFuelSurchargeApplies = (CheckBox)e.Item.FindControl("chkFuelSurchargeApplies");
                CheckBox chkIsEnabled = (CheckBox)e.Item.FindControl("chkIsEnabled");

                CheckBox chkIsAcceptanceRequired = (CheckBox)e.Item.FindControl("chkIsAcceptanceRequired");
                CheckBox chkIsDisplayedOnAddUpdateOrder = (CheckBox)e.Item.FindControl("chkIsDisplayedOnAddUpdateOrder");

                CheckBox chkIsTimeBased = (CheckBox)e.Item.FindControl("chkIsTimeBased");
                RadTimePicker rdiStartTime = (RadTimePicker)e.Item.FindControl("rdiStartTime");
                RadTimePicker rdiEndTime = (RadTimePicker)e.Item.FindControl("rdiEndTime");

                CheckBox chkIsDayBased = (CheckBox)e.Item.FindControl("chkIsDayBased");
                CheckBox chkMonday = (CheckBox)e.Item.FindControl("chkMonday");
                CheckBox chkTuesday = (CheckBox)e.Item.FindControl("chkTuesday");
                CheckBox chkWednesday = (CheckBox)e.Item.FindControl("chkWednesday");
                CheckBox chkThursday = (CheckBox)e.Item.FindControl("chkThursday");
                CheckBox chkFriday = (CheckBox)e.Item.FindControl("chkFriday");
                CheckBox chkSaturday = (CheckBox)e.Item.FindControl("chkSaturday");
                CheckBox chkSunday = (CheckBox)e.Item.FindControl("chkSunday");

                int? startTime = null;
                int? endTime = null;

                if(chkIsTimeBased.Checked)
                {
                    startTime = (int?)rdiStartTime.SelectedTime.Value.TotalMinutes;
                    endTime = (int?)rdiEndTime.SelectedTime.Value.TotalMinutes;
                }

                Facade.ExtraType facExtraType = new Facade.ExtraType();
                Entities.ExtraType extraType = new Entities.ExtraType();
                extraType = facExtraType.GetForExtraTypeID(extraTypeId);

                if (!String.IsNullOrEmpty(cboNominalCodes.SelectedValue) && Convert.ToInt32(cboNominalCodes.SelectedValue) > 0)
                {
                    Facade.INominalCode facNominal = new Facade.NominalCode();
                    Entities.NominalCode nominalCode = facNominal.GetForNominalCodeID(Convert.ToInt32(cboNominalCodes.SelectedValue));
                    extraType.NominalCode = nominalCode;
                }
                else
                {
                    Entities.NominalCode nominalCode = new Orchestrator.Entities.NominalCode();
                    extraType.NominalCode = nominalCode;
                }

                extraType.ExtraTypeId = extraTypeId;
                extraType.Description = txtDescription.Value;
                extraType.FuelSurchargeApplies = chkFuelSurchargeApplies.Checked;
                extraType.IsEnabled = chkIsEnabled.Checked;
                extraType.IsDisplayedOnAddUpdateOrder = chkIsDisplayedOnAddUpdateOrder.Checked;
                extraType.IsAcceptanceRequired = chkIsAcceptanceRequired.Checked;
                extraType.ShortDescription = txtShortDescription.Value;
                extraType.IsTimeBased = chkIsTimeBased.Checked;
                extraType.ExtraTypeStartTime = startTime;
                extraType.ExtraTypeEndTime = endTime;
                extraType.IsDayBased = chkIsDayBased.Checked;
                extraType.Monday = chkMonday.Checked;
                extraType.Tuesday = chkTuesday.Checked;
                extraType.Wednesday = chkWednesday.Checked;
                extraType.Thursday = chkThursday.Checked;
                extraType.Friday = chkFriday.Checked;
                extraType.Saturday = chkSaturday.Checked;
                extraType.Sunday = chkSunday.Checked;

                facExtraType.Update(extraType, ((Orchestrator.Entities.CustomPrincipal)Page.User).UserName.ToString());

                grdExtraTypes.Rebind();
            }
        }

        void grdExtraTypes_InsertCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
        {
            DropDownList cboNominalCodes = (DropDownList)e.Item.FindControl("cboNominalCodes");
            HtmlInputControl txtDescription = (HtmlInputControl)e.Item.FindControl("txtDescription");
            HtmlInputControl txtShortDescription = (HtmlInputControl)e.Item.FindControl("txtShortDescription");
            CheckBox chkFuelSurchargeApplies = (CheckBox)e.Item.FindControl("chkFuelSurchargeApplies");
            CheckBox chkIsEnabled = (CheckBox)e.Item.FindControl("chkIsEnabled");

            CheckBox chkIsAcceptanceRequired = (CheckBox)e.Item.FindControl("chkIsAcceptanceRequired");
            CheckBox chkIsDisplayedOnAddUpdateOrder = (CheckBox)e.Item.FindControl("chkIsDisplayedOnAddUpdateOrder");

            CheckBox chkIsTimeBased = (CheckBox)e.Item.FindControl("chkIsTimeBased");
            RadTimePicker rdiStartTime = (RadTimePicker)e.Item.FindControl("rdiStartTime");
            RadTimePicker rdiEndTime = (RadTimePicker)e.Item.FindControl("rdiEndTime");

            CheckBox chkIsDayBased = (CheckBox)e.Item.FindControl("chkIsDayBased");
            CheckBox chkMonday = (CheckBox)e.Item.FindControl("chkMonday");
            CheckBox chkTuesday = (CheckBox)e.Item.FindControl("chkTuesday");
            CheckBox chkWednesday = (CheckBox)e.Item.FindControl("chkWednesday");
            CheckBox chkThursday = (CheckBox)e.Item.FindControl("chkThursday");
            CheckBox chkFriday = (CheckBox)e.Item.FindControl("chkFriday");
            CheckBox chkSaturday = (CheckBox)e.Item.FindControl("chkSaturday");
            CheckBox chkSunday = (CheckBox)e.Item.FindControl("chkSunday");

            Facade.ExtraType facExtraType = new Facade.ExtraType();
            Entities.ExtraType extraType = new Entities.ExtraType();

            int? startTime = null;
            int? endTime = null;

            if (chkIsTimeBased.Checked)
            {
                startTime = (int?)rdiStartTime.SelectedTime.Value.TotalMinutes;
                endTime = (int?)rdiEndTime.SelectedTime.Value.TotalMinutes;
            }

            if (!string.IsNullOrEmpty(cboNominalCodes.SelectedValue) && Convert.ToInt32(cboNominalCodes.SelectedValue) > 0)
            {
                Facade.INominalCode facNominal = new Facade.NominalCode();
                Entities.NominalCode nominalCode = facNominal.GetForNominalCodeID(Convert.ToInt32(cboNominalCodes.SelectedValue));
                extraType.NominalCode = nominalCode;
            }

            extraType.Description = txtDescription.Value;
            extraType.FuelSurchargeApplies = chkFuelSurchargeApplies.Checked;
            extraType.IsEnabled = chkIsEnabled.Checked;
            extraType.IsDisplayedOnAddUpdateOrder = chkIsDisplayedOnAddUpdateOrder.Checked;
            extraType.IsAcceptanceRequired = chkIsAcceptanceRequired.Checked;
            extraType.IsTimeBased = chkIsTimeBased.Checked;
            extraType.ExtraTypeStartTime = startTime;
            extraType.ExtraTypeEndTime = endTime;
            extraType.IsDayBased = chkIsDayBased.Checked;
            extraType.Monday = chkMonday.Checked;
            extraType.Tuesday = chkTuesday.Checked;
            extraType.Wednesday = chkWednesday.Checked;
            extraType.Thursday = chkThursday.Checked;
            extraType.Friday = chkFriday.Checked;
            extraType.Saturday = chkSaturday.Checked;
            extraType.Sunday = chkSunday.Checked;

            extraType.ShortDescription = txtShortDescription.Value;

            facExtraType.Create(extraType, ((Orchestrator.Entities.CustomPrincipal)Page.User).UserName.ToString());

            grdExtraTypes.Rebind();
        }

        void grdExtraTypes_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            Facade.ExtraType facExtraType = new Orchestrator.Facade.ExtraType();
            grdExtraTypes.DataSource = facExtraType.GetForIsEnabled(new bool?());
        }
    }
}
