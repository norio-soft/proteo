using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Telerik.Web.UI;
using System.Web.UI.HtmlControls;
using System.Globalization;
using System.Data.SqlClient;

namespace Orchestrator.WebUI.SubConPortal
{
    public partial class ManageCallIns : System.Web.UI.Page
    {
        //--------------------------------------------------------------------------------------------------------------------------

        private const string vs_usersSubCon = "vs_userParentOrg";

        //--------------------------------------------------------------------------------------------------------------------------

        protected int UserParentIdentityId
        {
            get
            {
                int retVal = -1;
                if (ViewState[vs_usersSubCon] == null)
                {
                    Facade.IUser facUser = new Facade.User();
                    DataSet dsOrg = facUser.GetOrganisationForUser(this.Page.User.Identity.Name);

                    if (dsOrg.Tables[0].Rows.Count > 0)
                        ViewState[vs_usersSubCon] = dsOrg.Tables[0].Rows[0]["RelatedIdentityId"].ToString();
                }

                retVal = int.Parse(ViewState[vs_usersSubCon].ToString());
                return retVal;
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtSearch.Focus();
                LoadGoodsTypes();
                LoadServiceLevels();

                cblOrderStatus.DataSource = new string[] { eOrderStatus.Approved.ToString(), eOrderStatus.Delivered.ToString(), eOrderStatus.Invoiced.ToString() };
                cblOrderStatus.DataBind();

                cblOrderStatus.Items.FindByValue(eOrderStatus.Approved.ToString()).Selected = true;

                // Date filters should be blank by default (as per Freight Force customisation requirement)
                dteStartDate.SelectedDate = null;
                dteEndDate.SelectedDate = null;

                hidStartDate.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01).ToString("dd/MM/yy");
                hidEndDate.Value = DateTime.Today.ToString("dd/MM/yy");

                #region // Load the Business Types
                LoadBusinessTypes();
                #endregion

                if (!string.IsNullOrEmpty(Request.QueryString["ubt"]))
                {
                    this.grdOrders.Rebind();
                }

                if (!string.IsNullOrEmpty(Request.QueryString["ss"]))
                {
                    // We can populate the search box and have a go
                    txtSearch.Text = Request.QueryString["ss"];
                    grdOrders.Rebind();
                }

                var rejectionReasons = EF.DataContext.Current.RejectionReasonSet.Select(rr => rr.Description);
                bool reasonListEmpty = !rejectionReasons.Any();

            }

            GridColumn customerOrderNumber = this.grdOrders.Columns.FindByUniqueName("CustomerOrderNumber");
            customerOrderNumber.HeaderText = Globals.Configuration.SystemLoadNumberText;
            GridColumn deliveryrOrderNumber = this.grdOrders.Columns.FindByUniqueName("DeliveryOrderNumber");
            deliveryrOrderNumber.HeaderText = Globals.Configuration.SystemDocketNumberText;

        }

        //--------------------------------------------------------------------------------------------------------------------------

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.grdOrders.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdOrders_NeedDataSource);
            this.grdOrders.ItemDataBound += new GridItemEventHandler(grdOrders_ItemDataBound);


            this.dlgCallIn.DialogCallBack += new EventHandler(dlgCallIn_DialogCallBack);
            this.dlgSubbyOrderProfile.DialogCallBack += new EventHandler(dlgSubbyOrderProfile_DialogCallBack);

            this.btnSearch.Click += new EventHandler(btnSearch_Click);
        }

        //--------------------------------------------------------------------------------------------------------------------------

        protected void dlgSubbyOrderProfile_DialogCallBack(object sender, EventArgs e)
        {
            this.grdOrders.Rebind();
        }

        //--------------------------------------------------------------------------------------------------------------------------

        protected void dlgCallIn_DialogCallBack(object sender, EventArgs e)
        {
            this.grdOrders.Rebind();
        }

        //--------------------------------------------------------------------------------------------------------------------------

        protected void grdOrders_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            if ((IsPostBack || !string.IsNullOrEmpty(Request.QueryString["ss"]) || !string.IsNullOrEmpty(Request.QueryString["uoiid"])) && string.IsNullOrEmpty(Request.QueryString["rcbID"]))
            {
                // Search for orders based on Date Range Status and text
                // Determine the parameters
                List<int> orderStatusIDs = new List<int>();
                foreach (ListItem li in cblOrderStatus.Items)
                {
                    if (li.Selected)
                        orderStatusIDs.Add((int)((eOrderStatus)Enum.Parse(typeof(eOrderStatus), li.Value)));
                }

                // Set the Business Types
                List<int> BusinessTypes = new List<int>();
                foreach (ListItem li in cblBusinessType.Items)
                    if (li.Selected)
                        BusinessTypes.Add(int.Parse(li.Value));

                if (BusinessTypes.Count == 0)
                {
                    // none selected so assume to select all
                    foreach (ListItem li in cblBusinessType.Items)
                        BusinessTypes.Add(int.Parse(li.Value));
                }

                // Retrieve the client id, resource id, and sub-contractor identity id.
                int clientID = 0;
                int.TryParse(cboClient.SelectedValue, out clientID);
                int resourceID = 0;
                int subContractorIdentityID = this.UserParentIdentityId; // the subb of the user logged in.
                int collectionPointId = 0;
                int deliveryPointId = 0;

                string collectionOrgIdentityIdWithPointId = cboCollectionPointFilter.SelectedValue;
                if (collectionOrgIdentityIdWithPointId.Length > 0)
                {
                    string[] ids = collectionOrgIdentityIdWithPointId.Split(",".ToCharArray());
                    int.TryParse(ids[1], out collectionPointId);
                }

                string deliveryOrgIdentityIdWithPointId = cboDeliveryPointFilter.SelectedValue;
                if (deliveryOrgIdentityIdWithPointId.Length > 0)
                {
                    string[] ids = deliveryOrgIdentityIdWithPointId.Split(",".ToCharArray());
                    int.TryParse(ids[1], out deliveryPointId);
                }

                int goodsType = 0;
                int.TryParse(cboGoodsType.SelectedValue, out goodsType);

                int serviceLevelId = 0;
                int.TryParse(this.cboService.SelectedValue, out serviceLevelId);

                // Find the orders.
                Facade.IOrder facOrder = new Facade.Order();
                DataSet orderData = null;
                try
                {
                    orderData =
                        facOrder.GetCallinsForSubbyPortal(orderStatusIDs, dteStartDate.SelectedDate, dteEndDate.SelectedDate, txtSearch.Text,
                                        cboSearchAgainstDate.Items[0].Selected || cboSearchAgainstDate.Items[2].Selected,
                                        cboSearchAgainstDate.Items[1].Selected || cboSearchAgainstDate.Items[2].Selected,
                                        clientID, subContractorIdentityID, BusinessTypes, collectionPointId, deliveryPointId, goodsType,
                                        (rblSearchFor.Items[0].Selected || rblSearchFor.Items[1].Selected),
                                        (rblSearchFor.Items[0].Selected || rblSearchFor.Items[2].Selected),
                                        (rblSearchFor.Items[0].Selected || rblSearchFor.Items[3].Selected),
                                        (rblSearchFor.Items[0].Selected || rblSearchFor.Items[4].Selected), serviceLevelId,
                                        chkOutstandingOnly.Checked,this.txtLastUpdatedUser.Text.Trim());
                }
                catch (SqlException exc)
                {
                    if (exc.Message.StartsWith("Timeout expired."))
                    {
                        // A timeout exception has been encountered, instead of throwing the error page, instruct the user to refine their search.
                        lblNote.Text = "Your query is not precise enough, please provide additional information or narrow the date/order state range.";
                        pnlConfirmation.Visible = true;

                        // Communicate the details of the exception to support.
                        string methodCall = "Facade.IOrder.Search('{0}', {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}); encountered by {9}";
                        Utilities.SendSupportEmailHelper(
                            string.Format(methodCall,
                                          Entities.Utilities.GetCSV(orderStatusIDs),
                                          dteStartDate.SelectedDate,
                                          dteEndDate.SelectedDate,
                                          txtSearch.Text,
                                          cboSearchAgainstDate.Items[0].Selected || cboSearchAgainstDate.Items[2].Selected,
                                          cboSearchAgainstDate.Items[1].Selected || cboSearchAgainstDate.Items[2].Selected,
                                          clientID,
                                          resourceID,
                                          subContractorIdentityID,
                                          ((Entities.CustomPrincipal)Page.User).UserName)
                            , exc);

                        orderData = null;
                    }
                    else
                        throw;
                }

                // sort out the rate to display, when none of the orders have a rate and only the order group has a rate.
                // we only want to display the rate for one order in the group.
                List<int> orderGroupIds = new List<int>();

                if (orderData != null)
                    foreach (DataRow row in orderData.Tables[0].Rows)
                        if (row["OrderGroupId"] != System.DBNull.Value && Convert.ToDouble(row["ForeignRate"].ToString()) > 0.00)
                            if (orderGroupIds.Contains(int.Parse(row["OrderGroupId"].ToString())))
                                row["ForeignRate"] = 0.00;
                            else
                            {
                                row["ForeignRate"] = row["GroupRate"];
                                orderGroupIds.Add(int.Parse(row["OrderGroupId"].ToString()));
                            }

                grdOrders.DataSource = orderData;

                // hide the filter display
                this.ClientScript.RegisterStartupScript(this.GetType(), "hideFilters", "FilterOptionsDisplayHide();", true);
            }
            else
            {
                // This is needed as there is no data to show on the page load therefore is no grid to show..
                grdOrders.DataSource = new DataTable();
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------

        public void btnSearch_Click(object sender, EventArgs e)
        {
            this.grdOrders.Rebind();
        }

        //--------------------------------------------------------------------------------------------------------------------------

        protected string GetDate(DateTime date, bool anytime)
        {
            string retVal = string.Empty;

            if (anytime)
                retVal = date.ToString("dd/MM/yy") + " AnyTime";
            else
                retVal = date.ToString("dd/MM/yy HH:mm");

            return retVal;
        }

        //--------------------------------------------------------------------------------------------------------------------------

        protected void grdOrders_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;
                string orderId = row["OrderID"].ToString();

                HyperLink hypUpdateOrder = e.Item.FindControl("hypUpdateOrder") as HyperLink;
                HtmlAnchor hypCallIn = e.Item.FindControl("hypCallIn") as HtmlAnchor;
                Label lblCreateUserId = e.Item.FindControl("lblCreateUserId") as Label;
                Label lblLastUpdateUserId = e.Item.FindControl("lblLastUpdateUserId") as Label;

                if (row["CollectDropActualId"] == System.DBNull.Value)
                {
                    hypCallIn.InnerHtml = "Add";

                    if (((int)row["InstructionStateId"]) == (int)eInstructionState.InProgress)
                        hypCallIn.HRef = "javascript:CallInThis(" + row["JobId"].ToString() + "," + row["InstructionId"].ToString() + ");";
                    else
                        hypCallIn.InnerHtml = String.Empty;
                }
                else if (row["CollectDropActualId"] != System.DBNull.Value)
                {
                    hypCallIn.InnerHtml = "Update";
                    hypCallIn.HRef = "javascript:CallInThis(" + row["JobId"].ToString() + "," + row["InstructionId"].ToString() + ");";
                    lblCreateUserId.Text = (row["CreateUserId"] == DBNull.Value) ? "" : row["CreateUserId"].ToString();
                    lblLastUpdateUserId.Text = (row["LastUpdateUserId"] == DBNull.Value) ? "" : row["LastUpdateUserId"].ToString();
                }

                hypUpdateOrder.NavigateUrl = "javascript:ViewOrder(" + orderId + ");";
                hypUpdateOrder.Text = orderId.ToString();

                // Set the rate
                Label lblRate = e.Item.FindControl("lblRate") as Label;

                if (lblRate != null)
                {
                    int lcid = 2057;
                    decimal rate = (decimal)row["SubbyRate"];

                    if (row["SubbyLCID"] != DBNull.Value)
                        lcid = Convert.ToInt32(row["SubbyLCID"]);

                    CultureInfo culture = new CultureInfo(lcid);
                    lblRate.Text = rate.ToString("C", culture);
                }
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------

        protected string GetInstructionTypeImage(int instructionTypeID)
        {
            if (instructionTypeID == 1 || instructionTypeID == 5)
                return "loadfinal.png";
            else if (instructionTypeID == 7)
                return "trunk.gif";
            else
                return "dropfinal.png";
        }

        //--------------------------------------------------------------------------------------------------------------------------

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

        //--------------------------------------------------------------------------------------------------------------------------

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

        //--------------------------------------------------------------------------------------------------------------------------

        private void LoadBusinessTypes()
        {
            Facade.IBusinessType facBusinessType = new Facade.BusinessType();
            DataSet dsBusinessTypes = facBusinessType.GetAll();

            foreach (DataRow row in dsBusinessTypes.Tables[0].Rows)
            {
                ListItem li = new ListItem(row["Description"].ToString(), row["BusinessTypeID"].ToString());
                // Select all the business types by default.
                li.Selected = true;
                cblBusinessType.Items.Add(li);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------

        protected void cboClient_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
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
    }
}
