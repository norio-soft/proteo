using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Orchestrator.WebUI
{
    public partial class Groupage_findorder : Orchestrator.Base.BasePage
    {
        #region Private Fields
        
        int orderCount = 0;
        double totalWeight = 0;
        double totalPalletCount = 0;
        double totalPalletSpaces = 0;


        private bool _updateGridSettings = false;

        #endregion

        #region Page Load/Init;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtSearch.Focus();
                LoadGoodsTypes();
                LoadServiceLevels();
                LoadColumnConfiguration();

                cblOrderStatus.DataSource = Enum.GetNames(typeof(eOrderStatus));
                cblOrderStatus.DataBind();

                // Task 15629 - T.Farrow: Enable all filter checkboxes except for "cancelled"
                CheckAllExceptCancelled();

                cblOrderStatus.Items.FindByValue(eOrderStatus.Approved.ToString()).Selected = true;

                // Date filters should be blank by default (as per Freight Force customisation requirement)
                dteStartDate.SelectedDate = null;
                dteEndDate.SelectedDate = null;

                this.businessTypeCheckList.AllBusinessTypesSelected = true;

                ConfigureContextMenu();

                trFilterAllocation.Visible = Utilities.IsAllocationEnabled();

                this.ConfigureOrganisationReferences();

                this.LoadGridSettings();

                if (!string.IsNullOrEmpty(Request.QueryString["ubt"]))
                    this.grdOrders.Rebind();

                if (!string.IsNullOrEmpty(Request.QueryString["ss"]))
                {
                    // We can populate the search box and have a go
                    txtSearch.Text = Request.QueryString["ss"];
                    grdOrders.Rebind();
                }

                int? unallocatedOrdersIdentityID = Utilities.ParseNullable<int>(Request.QueryString["uoiid"]);
                if (unallocatedOrdersIdentityID.HasValue)
                {
                    // Requested search for unallocated orders for client identity id
                    rblAllocation.SelectedIndex = 1; // Unallocated only

                    var organisation = EF.DataContext.Current.OrganisationSet.FirstOrDefault(o => o.IdentityId == unallocatedOrdersIdentityID.Value);
                    cboClient.SelectedValue = unallocatedOrdersIdentityID.Value.ToString();
                    cboClient.Text = organisation.OrganisationName;

                    foreach (ListItem li in cblOrderStatus.Items)
                    {
                        // Only include approved orders to match results of Unallocated Orders web part
                        li.Selected = li.Text == "Approved";
                    }

                    rblAllocation.SelectedIndex = 1; // Unallocated only
                    cboClient.SelectedValue = unallocatedOrdersIdentityID.Value.ToString();

                    this.grdOrders.Rebind();
                }
            }
        }

        protected override PageStatePersister PageStatePersister
        {
            get
            {
                return new HiddenFieldPageStatePersister(this);
            }
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            
            this.grdOrders.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdOrders_NeedDataSource);
            this.grdOrders.ItemCommand += new GridCommandEventHandler(grdOrders_ItemCommand);
            this.grdOrders.ItemDataBound += new GridItemEventHandler(grdOrders_ItemDataBound);
            this.grdOrders.ItemCreated += new GridItemEventHandler(grdOrders_ItemCreated);

            this.btnSearch.Click += new EventHandler(btnSearch_Click);
            this.btnChangeColumns.Click += new EventHandler(btnChangeColumns_Click);

            this.cboResource.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboResource_ItemsRequested);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (_updateGridSettings)
            {
                SaveGridSettings(false);
                _updateGridSettings = false;
            }
        }

        #endregion

        #region Data Loading

        void grdOrders_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            if ((IsPostBack || !string.IsNullOrEmpty(Request.QueryString["ss"]) || !string.IsNullOrEmpty(Request.QueryString["uoiid"])) && string.IsNullOrEmpty(Request.QueryString["rcbID"]))
            {
                var orderStatusItems = cblOrderStatus.Items.Cast<ListItem>();
                var allOrderStatusesSelected = !orderStatusItems.Any(i => !i.Selected);
                // Specifically code for "all statuses except Cancelled" because this is the default filter and will allow for a better performing query
                var allOrderStatusesExceptCancelledSelected = !allOrderStatusesSelected && !orderStatusItems.Any(i => !i.Selected && i.Value != eOrderStatus.Cancelled.ToString());
                var orderStatuses = allOrderStatusesSelected || allOrderStatusesExceptCancelledSelected || orderStatusItems.Any(i => i.Selected && i.Value == eOrderStatus.All.ToString()) ? null : orderStatusItems.Where(i => i.Selected).Select(i => (eOrderStatus)Enum.Parse(typeof(eOrderStatus), i.Value));

                var allBusinessTypesSelected = this.businessTypeCheckList.AllBusinessTypesSelected;
                var businessTypeIDs = allBusinessTypesSelected ? null : this.businessTypeCheckList.SelectedBusinessTypeIDs;

                int? collectionPointID = null;
                var collectionIdentityIDWithPointID = hidCollectionPointID.Value.ToString();

                if (collectionIdentityIDWithPointID.Length > 0)
                {
                    var ids = collectionIdentityIDWithPointID.Split(',');
                    collectionPointID = Utilities.ParseNullable<int>(ids[1]);
                }

                int? deliveryPointID = null;
                string deliveryIdentityIDWithPointID = hidDeliveryPointID.Value.ToString();

                if (deliveryIdentityIDWithPointID.Length > 0)
                {
                    var ids = deliveryIdentityIDWithPointID.Split(',');
                    deliveryPointID = Utilities.ParseNullable<int>(ids[1]);;
                }

                var isAllocationEnabled = Utilities.IsAllocationEnabled();
                var unallocatedOnly = isAllocationEnabled && rblAllocation.SelectedIndex != 0;
                var previouslyAllocatedOnly = isAllocationEnabled && rblAllocation.SelectedIndex == 2;

                var customReferenceFieldNames = from li in this.cblOrganisationReferenceColumns.Items.Cast<ListItem>() where li.Selected select li.Text;
                var isDeliveringResourceVisible = grdOrders.Columns.FindByUniqueName("DeliveringResource").Visible;
                var isSurchargesVisible = grdOrders.Columns.FindByUniqueName("Surcharges").Visible;

                using (var uow = DIContainer.CreateUnitOfWork())
                {
                    uow.SetCommandTimeout(60);
                    var repo = DIContainer.CreateRepository<Repositories.IOrderRepository>(uow);

                    try
                    {
                        var orders = repo.GetForFindOrder(
                            txtSearch.Text.Trim(),
                            cblSearchFor.Items.FindByValue("LOADNO").Selected,
                            cblSearchFor.Items.FindByValue("DELNO").Selected,
                            cblSearchFor.Items.FindByValue("ORDER").Selected,
                            cblSearchFor.Items.FindByValue("RUNID").Selected,
                            cblSearchFor.Items.FindByValue("CUSTOM").Selected,
                            dteStartDate.SelectedDate,
                            dteEndDate.SelectedDate,
                            cboSearchAgainstDate.Items.FindByValue("COL").Selected || cboSearchAgainstDate.Items.FindByValue("BOTH").Selected,
                            cboSearchAgainstDate.Items.FindByValue("DEL").Selected || cboSearchAgainstDate.Items.FindByValue("BOTH").Selected,
                            cboSearchAgainstDate.Items.FindByValue("TRUNK").Selected,
                            cboSearchAgainstDate.Items.FindByValue("EXPORT").Selected,
                            allOrderStatusesSelected,
                            allOrderStatusesExceptCancelledSelected,
                            orderStatuses,
                            allBusinessTypesSelected,
                            businessTypeIDs,
                            Utilities.ParseNullable<int>(cboService.SelectedValue),
                            Utilities.ParseNullable<int>(cboClient.SelectedValue),
                            Utilities.ParseNullable<int>(cboResource.SelectedValue),
                            Utilities.ParseNullable<int>(cboSubContractor.SelectedValue),
                            txtTrackingNumber.Text.Trim(),
                            collectionPointID,
                            deliveryPointID,
                            Utilities.ParseNullable<int>(cboGoodsType.SelectedValue),
                            unallocatedOnly,
                            previouslyAllocatedOnly,
                            customReferenceFieldNames,
                            isDeliveringResourceVisible,
                            isSurchargesVisible);

                        orderCount = orders.Count();
                        totalPalletCount = orders.Sum(o => o.NoPallets);
                        totalPalletSpaces = (double)orders.Sum(o => o.PalletSpaces);
                        totalWeight = (double)orders.Sum(o => o.Weight);

                        grdOrders.DataSource = orders.ToList();
                    }
                    catch (Exception exc)
                    {
                        while (!(exc is SqlException) && exc.InnerException != null)
                        {
                            exc = exc.InnerException;
                        }

                        if (exc.Message.StartsWith("Timeout expired."))
                        {
                            // A timeout exception has been encountered, instead of throwing the error page, instruct the user to refine their search.
                            lblNote.Text = "Your query is not precise enough, please provide additional information or narrow the date / order state range.";
                            pnlConfirmation.Visible = true;
                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                // hide the filter display
                this.ClientScript.RegisterStartupScript(this.GetType(), "hideFilters", "FilterOptionsDisplayHide();", true);
            }
            else
            {
                // This is needed as there is no data to show on the page load therefore is no grid to show..
                grdOrders.DataSource = Enumerable.Empty<Repositories.DTOs.FindOrderRow>();
            }
        }

        #endregion

        #region Private Methods

        private void ConfigureOrganisationReferences()
        {
            // Add any custom organisation reference fields
            Facade.IOrganisation facOrganisation = new Facade.Organisation();
            var organisationReferences = facOrganisation.GetAllOrganisationReferences().Tables[0].AsEnumerable();

            if (organisationReferences.Any())
            {
                var groupedReferences = organisationReferences.GroupBy(or => or.Field<string>("Description"));

                foreach (var gr in groupedReferences)
                {
                    var columnDescription = gr.Key;
                    var uniqueName = OrganisationReferenceColumnUniqueName(columnDescription);

                    // Add a check box to allow the user to toggle visibility of this column
                    var listItem = new ListItem(columnDescription, uniqueName);
                    listItem.Attributes["title"] = gr.Count() > 10 ? string.Format("{0} clients", gr.Count()) : Entities.Utilities.MergeStrings("\n", gr.Select(or => or.Field<string>("OrganisationName")));
                    this.cblOrganisationReferenceColumns.Items.Add(listItem);

                    // Add the column to the grid
                    var column = new GridTemplateColumn();
                    this.grdOrders.MasterTableView.Columns.Add(column);
                    column.UniqueName = uniqueName;
                    column.HeaderText = columnDescription;
                    column.Visible = false;
                }
            }
        }

        private void CheckAllExceptCancelled()
        {
            foreach (ListItem li in cblOrderStatus.Items)
            {
                if (li.Text != "Cancelled")
                {
                    li.Selected = true;
                }
            }
        }

        private void LoadGoodsTypes()
        {
            DataSet dsGoodsTypes = Orchestrator.Facade.GoodsType.GetAllActiveGoodsTypes();
            cboGoodsType.DataSource = dsGoodsTypes;
            cboGoodsType.DataTextField = "Description";
            cboGoodsType.DataValueField = "GoodsTypeID";
            cboGoodsType.DataBind();

            // The first value should be blank.
            cboGoodsType.Items.Insert(0, new RadComboBoxItem(""));
        }

        private void LoadServiceLevels()
        {
            Orchestrator.Facade.IOrderServiceLevel facOrder = new Orchestrator.Facade.Order();
            DataSet dsServices = facOrder.GetAll();

            cboService.DataSource = dsServices;
            cboService.DataTextField = "Description";
            cboService.DataValueField = "OrderServiceLevelID";
            cboService.DataBind();

            // The first value should be blank.
            cboService.Items.Insert(0, new RadComboBoxItem(""));
        }

        private void ConfigureContextMenu()
        {
            Facade.IBusinessType facBusinessType = new Facade.BusinessType();
            DataSet dsBusinessTypes = facBusinessType.GetAll();

            //Top level context menu items.
            RadMenuItem cbt = new RadMenuItem("Change Business Type To");
            RadMenuItem cdn = new RadMenuItem("Create Delivery Note");
            cdn.Value = "-1";

            //Create business type menu items.
            foreach (DataRow row in dsBusinessTypes.Tables[0].Rows)
            {
                RadMenuItem rmi = new RadMenuItem();
                rmi.Text = row["Description"].ToString();
                rmi.Value = row["BusinessTypeID"].ToString();
                cbt.Items.Add(rmi);
            }

            //Add items to context menu.
            RadMenu1.Items.Add(cbt);
            RadMenu1.Items.Add(cdn);
        }

        private void LoadColumnConfiguration()
        {
            if (!Globals.Configuration.FindOrderShowSubcontractorRate)
            {
                var checkBox = cblGridColumns.Items.FindByValue("SubcontractRate");
                if (checkBox != null)
                    cblGridColumns.Items.Remove(checkBox);
            }
        }

        private static string OrganisationReferenceColumnUniqueName(string columnDescription)
        {
            return string.Concat("_OrgRef_", columnDescription.Replace(' ', '_'));
        }

        #endregion Private Methods

        #region Combobox Event Handlers

        void cboClient_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            cboClient.Items.Clear();

            Facade.IReferenceData facReferenceData = new Facade.ReferenceData();
            DataSet ds = facReferenceData.GetAllClientsFiltered(e.Text);

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
                rcItem.Value = dt.Rows[i]["IdentityID"].ToString();
                cboClient.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

        void cboResource_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboResource.Items.Clear();

            Facade.IResource facResource = new Facade.Resource();
            DataSet ds = facResource.GetAllResourcesFiltered(e.Text, false, true);

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
                rcItem.Text = dt.Rows[i]["Description"].ToString();
                rcItem.Value = dt.Rows[i]["ResourceId"].ToString();
                cboResource.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

        void cboSubContractor_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboSubContractor.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllSubContractorsFiltered(e.Text, false);

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
                cboSubContractor.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

        #endregion

        #region Button Event Handlers

        public void btnSearch_Click(object sender, EventArgs e)
        {
            LoadGridSettings();
            this.grdOrders.Rebind();
        }

        void btnChangeColumns_Click(object sender, EventArgs e)
        {
            SaveGridSettings(true);
        }

        public void btnExport_Click(object sender, EventArgs e)
        {
            this.grdOrders.ExportSettings.ExportOnlyData = true;
            this.grdOrders.ExportSettings.FileName = "OrderExport";
            this.grdOrders.MasterTableView.ExportToExcel();
        }

        #endregion

        #region Grid View Events

        protected string GetDate(DateTime date, bool anytime)
        {
            string retVal = string.Empty;

            if (anytime)
                retVal = date.ToString("dd/MM/yy") + " AnyTime";
            else
                retVal = date.ToString("dd/MM/yy HH:mm");

            return retVal;
        }

        void grdOrders_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridCommandItem)
            {
                ((Label)e.Item.FindControl("lblPalletsTotal")).Text = totalPalletCount.ToString();
                ((Label)e.Item.FindControl("lblSpacesTotal")).Text = totalPalletSpaces.ToString();
                ((Label)e.Item.FindControl("lblWeightTotal")).Text = totalWeight.ToString();
                ((Label)e.Item.FindControl("lblOrderCount")).Text = orderCount.ToString();
            }
        }

        void grdOrders_ItemCommand(object source, GridCommandEventArgs e)
        {
            if (e.CommandName.ToLower() == "sort")
            {
                _updateGridSettings = true;
            }
        }

        void grdOrders_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                var row = (Repositories.DTOs.FindOrderRow)e.Item.DataItem;
                var orderID = row.OrderID;

                var hypUpdateOrder = e.Item.FindControl("hypUpdateOrder") as HyperLink;
                hypUpdateOrder.Text = orderID.ToString();

                var hypJobID = e.Item.FindControl("hypJobId") as HyperLink;

                HyperLink lnkPOD = (HyperLink)e.Item.FindControl("lnkPOD");

                if (row.HasPod)
                {
                    lnkPOD.ForeColor = System.Drawing.Color.Blue;
                    lnkPOD.NavigateUrl = row.ScannedFormPDF.Trim();
                    lnkPOD.Text = "Yes";
                }
                else
                {
                    if (row.OrderStatus == eOrderStatus.Delivered || row.OrderStatus == eOrderStatus.Invoiced)
                    {
                        lnkPOD.Text = "No";
                        lnkPOD.ForeColor = System.Drawing.Color.Blue;
                        lnkPOD.NavigateUrl = @"javascript:OpenPODWindow(" + row.OrderID + ")";
                    }
                    else
                    {
                        lnkPOD.Text = "N/A";
                        lnkPOD.ToolTip = "Not Delivered";
                        lnkPOD.Style.Add("text-decoration", "none");
                    }
                }
                    
                if (row.JobID.HasValue)
                {
                    hypJobID.Text = row.JobID.ToString();
                    hypJobID.NavigateUrl = string.Format("javascript:openResizableDialogWithScrollbars('/job/job.aspx?jobId={0}' + getCSID(), '1220', '870');", row.JobID);
                }

                var openOrderJS = dlgOrderShuffler.GetOpenDialogScript();
                hypUpdateOrder.NavigateUrl = string.Format("javascript:setHiddenRecordIds(); currentRowId = {0}; {1}", orderID, openOrderJS);

                // Colour the row light pink if the order has a pending row (or retry row) in tblExportMessage.
                // Colour the row dark pink if the order has a processed row in tblExportMessage, i.e. if it has been exported.

                if (row.MessageStateID.HasValue)
                {
                    switch ((eMessageState)row.MessageStateID.Value)
                    {
                        case eMessageState.Unprocessed:
                            e.Item.BackColor = System.Drawing.Color.Pink;
                            break;

                        case eMessageState.Processed:
                        case eMessageState.NotProcessed:
                            e.Item.BackColor = System.Drawing.Color.Violet;
                            break;

                        case eMessageState.Error:
                            e.Item.BackColor = System.Drawing.Color.Red;
                            break;
                    }
                }

                // Determine the culture for rate display
                var lcid = row.LCID ?? 2057;
                var culture = new CultureInfo(lcid);

                // Set the rate
                var lblRate = e.Item.FindControl("lblRate") as Label;

                if (lblRate != null)
                {
                    var rate = row.ForeignRate ?? 0M;
                    lblRate.Text = rate.ToString("C", culture);
                }

                // Set the extras total
                var lblExtras = e.Item.FindControl("lblExtras") as Label;

                if (lblExtras != null)
                {
                    var extras = row.ExtrasForeignTotal ?? 0M;
                    lblExtras.Text = extras.ToString("C", culture);
                }

                // Set the subcontract rate
                var lblSubcontractRate = e.Item.FindControl("lblSubcontractRate") as Label;

                if (lblSubcontractRate != null)
                {
                    if (row.SubcontractRate.HasValue)
                    {
                        var subcontractLCID = row.SubcontractLCID ?? 2057;
                        var subcontractCulture = new CultureInfo(subcontractLCID);
                        lblSubcontractRate.Text = row.SubcontractRate.Value.ToString("C", subcontractCulture);
                    }
                    else
                        lblSubcontractRate.Text = string.Empty;
                }

                // Populate any custom organisation reference fields
                if (row.CustomReferences.Any())
                {
                    foreach (var customReference in row.CustomReferences)
                    {
                        var uniqueName = OrganisationReferenceColumnUniqueName(customReference.FieldName);
                        var column = e.Item.OwnerTableView.Columns.FindByUniqueNameSafe(uniqueName);

                        if (column != null)
                            ((GridDataItem)e.Item)[column].Text = customReference.Value;
                    }
                }
            }
        }

        #endregion

        #region Grid Saving

        public void btnSaveGridSettings_Click(object sender, EventArgs e)
        {
            SaveGridSettings(false);
        }

        private void SaveGridSettings(bool refreshGrid)
        {
            // get any columns that need to be hidden
            foreach (ListItem li in this.cblGridColumns.Items)
            {
                this.grdOrders.Columns.FindByUniqueName(li.Value).Visible = li.Selected;
            }

            foreach (ListItem li in this.cblOrganisationReferenceColumns.Items)
            {
                this.grdOrders.Columns.FindByUniqueName(li.Value).Visible = li.Selected;
            }

            Utilities.SaveGridSettings(this.grdOrders, eGrid.FindOrder, Page.User.Identity.Name);

            // forces the grid to pick up the column settings if changed
            if (refreshGrid)
            {
                LoadGridSettings();
                grdOrders.Rebind();
            }
        }

        private void LoadGridSettings()
        {
            LoadColumnConfiguration();

            IEnumerable<string> columnsToHide;
            Utilities.LoadSettings(this.grdOrders, eGrid.FindOrder, out columnsToHide, Page.User.Identity.Name);

            foreach (ListItem li in this.cblGridColumns.Items)
            {
                li.Selected = !columnsToHide.Any(s => s == li.Value);
            }

            foreach (ListItem li in this.cblOrganisationReferenceColumns.Items)
            {
                li.Selected = !columnsToHide.Any(s => s == li.Value);
            }

            // Subcontractor Rate column display is based on value of system setting
            if (!Globals.Configuration.FindOrderShowSubcontractorRate)
            {
                var subcontractorRateColumn = grdOrders.Columns.FindByUniqueNameSafe("SubcontractRate");
                if (subcontractorRateColumn != null)
                    subcontractorRateColumn.Visible = false;
            }
        }

        #endregion Grid Saving

        #region Page Method for Updating Order Business Types

        [System.Web.Services.WebMethod]
        public static bool UpdateBusinessType(string orderIDCSV, int businessTypeID, string userName)
        {
            Facade.IOrder facOrder = new Facade.Order();
            return facOrder.UpdateOrdersBusinessType(orderIDCSV, businessTypeID, userName);
        }

        #endregion
    }
}