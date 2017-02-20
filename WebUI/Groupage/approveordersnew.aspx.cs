using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using System.Data;
using Telerik.Web.UI;
using System.Globalization;

namespace Orchestrator.WebUI.Groupage
{
    public partial class approveordersnew : Orchestrator.Base.BasePage
    {

        #region Page load/init

        protected void Page_Load(object sender, EventArgs e)
        {
            Security.Authorise.EnforceAuthorisation(eSystemPortion.ApproveOrder);

            string args = Request.Params["__EVENTARGUMENT"];

            if (this.IsPostBack && args != null && args.ToLower() == "refresh")
            {
                Server.Transfer(this.Request.RawUrl);
            }

            if (!IsPostBack)
            {
                // Wire up the behaviours for the booked in date time options
                rimApproveOrder.GetSettingByBehaviorID("DateInputBehaviour").TargetControls.Add(new TargetInput(txtBookInByFromDate.UniqueID, true));
                rimApproveOrder.GetSettingByBehaviorID("DateInputBehaviour").TargetControls.Add(new TargetInput(txtBookInFromDate.UniqueID, true));
                rimApproveOrder.GetSettingByBehaviorID("TimeInputBehaviour").TargetControls.Add(new TargetInput(txtBookInByFromTime.UniqueID, true));
                rimApproveOrder.GetSettingByBehaviorID("TimeInputBehaviour").TargetControls.Add(new TargetInput(txtBookInFromTime.UniqueID, true));

                txtBookInFromDate.Text = DateTime.Today.AddDays(1).ToString("dd/MM/yy");
                txtBookInFromTime.Text = "08:00";
                txtBookInByFromDate.Text = DateTime.Today.AddDays(1).ToString("dd/MM/yy");
                txtBookInByFromTime.Text = "17:00";

                //If a clientId has been passed preload the client combo with it
                int? identityID = Utilities.ParseNullable<int>(Request.QueryString["iId"]);
                if (identityID.HasValue)
                {
                    var organisation = EF.DataContext.Current.OrganisationSet.FirstOrDefault(o => o.IdentityId == identityID.Value);
                    if (organisation != null)
                    {
                        cboClient.SelectedValue = organisation.IdentityId.ToString();
                        cboClient.Text = organisation.OrganisationName;
                    }
                }

                var rejectionReasons = EF.DataContext.Current.RejectionReasonSet.Select(rr => rr.Description);
                bool reasonListEmpty = !rejectionReasons.Any();

                cboRejectionReason.Visible = !reasonListEmpty;
                cboRejectionReason.DataSource = rejectionReasons;
                cboRejectionReason.DataBind();

                cvRejectionReason.ErrorMessage =
                    reasonListEmpty ?
                        "Please supply a rejection reason" :
                        "Please either select a rejection reason from the list or type a reason";
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.grdOrders.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdOrders_NeedDataSource);
            this.grdOrders.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(grdOrders_ItemDataBound);

            this.btnSearch.Click += new EventHandler(btnSearch_Click);
            this.btnSaveChanges.Click += new EventHandler(btnSaveChanges_Click);
            this.btnConfirmOrders.Click += new EventHandler(btnConfirmOrders_Click);
            this.btnHiddenRejectOrders.Click += new EventHandler(btnHiddenRejectOrders_Click);

            this.cvRejectionReason.ServerValidate += new ServerValidateEventHandler(cvRejectionReason_ServerValidate);
        }

        private void cvRejectionReason_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = !string.IsNullOrEmpty(cboRejectionReason.SelectedValue) || !string.IsNullOrEmpty(txtRejectionReason.Text);
        }

        void btnSearch_Click(object sender, EventArgs e)
        {
            this.grdOrders.Rebind();
        }

        void btnHiddenRejectOrders_Click(object sender, EventArgs e)
        {
            string ordersAsText = hidApproveOrderIDs.Value;
            var orderIDs = hidApproveOrderIDs.Value
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.Parse(s));

            Facade.IOrder facOrder = new Facade.Order();
            string rejectionReason = Entities.Utilities.MergeStrings(" - ", cboRejectionReason.SelectedValue, txtRejectionReason.Text);
            facOrder.Reject(orderIDs, rejectionReason, this.Page.User.Identity.Name);

            grdOrders.Rebind();
        }

        void btnConfirmOrders_Click(object sender, EventArgs e)
        {
            // Check to see if there are any orders to save the changs for before approving
            if (!string.IsNullOrEmpty(hidOrderIDs.Value) && hidOrderIDs.Value.Length > 0)
                SaveChanges();

            var orderIDs = hidApproveOrderIDs.Value
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.Parse(s));

            //Attempt to find a customer specific implentation to approve the orders
            //if not use the default implementation
            Orchestrator.Application.IApproveOrder approveOrder = Orchestrator.Application.GetSpecificImplementation<Orchestrator.Application.IApproveOrder>();

            if (approveOrder != null)
                approveOrder.ApproveOrders(orderIDs.ToList(), this.Page.User.Identity.Name);
            else
            {
                Facade.IOrder facOrder = new Facade.Order();
                facOrder.Approve(orderIDs, this.Page.User.Identity.Name);
            }

            grdOrders.Rebind();
        }

        void btnSaveChanges_Click(object sender, EventArgs e)
        {
            SaveChanges();

            grdOrders.Rebind();
        }

        private void SaveChanges()
        {
            // get the orders that have been amended.
            string ordersAsText = hidOrderIDs.Value;
            string[] orderIDs = ordersAsText.Split(',');
            List<string> orders = orderIDs.ToList();

            foreach (Telerik.Web.UI.GridItem item in grdOrders.Items)
            {
                if (item is GridDataItem)
                {
                    if (orders.Contains(item.Attributes["orderid"]))
                    {

                        int orderID = int.Parse(item.Attributes["orderid"]);
                        // get the information required for the update;
                        DateTime collectFromDate = DateTime.Parse(((TextBox)item.FindControl("txtCollectAtDate")).Text);
                        DateTime? collectFromTime = null;
                        if (((TextBox)item.FindControl("txtCollectAtTime")).Text != "")
                            collectFromTime = DateTime.Parse(((TextBox)item.FindControl("txtCollectAtTime")).Text);

                        DateTime collectByDate = DateTime.Parse(((TextBox)item.FindControl("txtCollectByDate")).Text);
                        DateTime? collectByTime = null;
                        if (((TextBox)item.FindControl("txtCollectByTime")).Text != "")
                            collectByTime = DateTime.Parse(((TextBox)item.FindControl("txtCollectByTime")).Text);

                        RadioButtonList rblCollectionDateType = item.FindControl("rblCollectionTimeOptions") as RadioButtonList;

                        if (int.Parse(rblCollectionDateType.SelectedValue) == 2)
                        {
                            collectFromTime = new DateTime(collectByTime.Value.Year, collectByTime.Value.Month,
                                collectByTime.Value.Day, 0, 0, 0);

                            collectByTime = new DateTime(collectByTime.Value.Year, collectByTime.Value.Month,
                                collectByTime.Value.Day, 23, 59, 0);
                        }

                        DateTime deliverAtFromDate = DateTime.Parse(((TextBox)item.FindControl("txtDeliverAtFromDate")).Text);
                        DateTime? deliverAtFromTime = null;
                        if (((TextBox)item.FindControl("txtDeliverAtFromTime")).Text != "")
                            deliverAtFromTime = DateTime.Parse(((TextBox)item.FindControl("txtDeliverAtFromTime")).Text);

                        DateTime deliverByDate = DateTime.Parse(((TextBox)item.FindControl("txtDeliverByFromDate")).Text);
                        DateTime? deliverByTime = null;
                        if (((TextBox)item.FindControl("txtDeliverByFromTime")).Text != "")
                            deliverByTime = DateTime.Parse(((TextBox)item.FindControl("txtDeliverByFromTime")).Text);

                        RadioButtonList rblDeliveryDateType = item.FindControl("rblDeliveryTimeOptions") as RadioButtonList;

                        decimal rate = string.IsNullOrEmpty(((TextBox)item.FindControl("txtRate")).Text) ? 0M : decimal.Parse(((TextBox)item.FindControl("txtRate")).Text, NumberStyles.Any);

                        string deliveryNotes = ((TextBox)item.FindControl("txtDeliveryNotes")).Text;

                        if (int.Parse(rblDeliveryDateType.SelectedValue) == 2)
                        {
                            deliverAtFromTime = new DateTime(deliverByTime.Value.Year, deliverByTime.Value.Month,
                                deliverByTime.Value.Day, 0, 0, 0);

                            deliverByTime = new DateTime(deliverByTime.Value.Year, deliverByTime.Value.Month,
                                deliverByTime.Value.Day, 23, 59, 0);
                        }

                        UpdateOrder(orderID, collectFromDate, collectFromTime, collectByDate, collectByTime, int.Parse(rblCollectionDateType.SelectedValue), deliverAtFromDate, deliverAtFromTime, deliverByDate, deliverByTime, int.Parse(rblDeliveryDateType.SelectedValue), rate, deliveryNotes, Page.User.Identity.Name);

                    }
                }
            }
        }
        #endregion

        #region Grid Events
        private const string unApprovedPoint = @"<a href=""#"" onclick=""{0}; return false;"" target=""_blank"">{1}</a>";
        int numberofJobs = 0;
        double palletSpaces = 0;

        void grdOrders_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {

                HtmlImage imgOrderBookedIn = (HtmlImage)e.Item.FindControl("imgOrderBookedIn");
                HiddenField hidOrderID = e.Item.FindControl("hidOrderChanged") as HiddenField;
                HyperLink lnkSetBookIn = e.Item.FindControl("lnkSetRequireBookIn") as HyperLink;
                DataRowView drv = (DataRowView)e.Item.DataItem;
                ePointState collectionPointState = (ePointState)Convert.ToInt32(((System.Data.DataRowView)e.Item.DataItem)["CollectionPointStateId"].ToString());
                ePointState deliveryPointState = (ePointState)Convert.ToInt32(((System.Data.DataRowView)e.Item.DataItem)["deliveryPointStateId"].ToString());

                eIdentityStatus collectionOrganisationState = (eIdentityStatus)Convert.ToInt32(((System.Data.DataRowView)e.Item.DataItem)["CollectionOrganisationStateID"].ToString());
                eIdentityStatus deliveryOrganisationState = (eIdentityStatus)Convert.ToInt32(((System.Data.DataRowView)e.Item.DataItem)["DeliveryOrganisationStateID"].ToString());

                int collectionPointId = (int)drv["CollectionPointId"];
                int deliveryPointId = (int)drv["DeliveryPointId"];
                int orderID = (int)drv["OrderID"];

                palletSpaces += double.Parse(drv["NoPallets"].ToString());

                // add the order id to the row as this is the simplest place to put it.
                e.Item.Attributes.Add("orderid", orderID.ToString());
                e.Item.Attributes.Add("palletspaces", drv["NoPallets"].ToString());

                CheckBox chkSelectOrder = e.Item.FindControl("chkSelectOrder") as CheckBox;
                string orderGroupId = ((DataRowView)e.Item.DataItem)["OrderGroupID"].ToString();

                //if the order requires booking show the requires book in icon instead and hook up the method call
                if (drv["BookedInStateID"] != DBNull.Value && (int)drv["BookedInStateID"] == (int)eBookedInState.Required)
                {
                    imgOrderBookedIn.Src = "/images/star.gif";
                    imgOrderBookedIn.Attributes.Add("onclick", "ShowBookIn(this," + drv["OrderID"].ToString() + ");");
                    imgOrderBookedIn.Alt = "Please click to mark as booked in.";
                    imgOrderBookedIn.Visible = true;
                    lnkSetBookIn.Text = "Del";
                    lnkSetBookIn.ToolTip = "Remove the Requires Book In Setting";
                    lnkSetBookIn.Attributes.Add("onclick", string.Format("RemoveBookInRequired(this,{0}); return false;", orderID));
                }

                #region Requires Book In Control
                if (drv["BookedInStateID"] != DBNull.Value && (int)drv["BookedInStateID"] == (int)eBookedInState.NotRequired)
                {
                    lnkSetBookIn.Text = "Set";
                    lnkSetBookIn.ToolTip = "Set that this job needs to be Booked In.";
                    lnkSetBookIn.Attributes.Add("onclick", string.Format("SetBookInRequired(this,{0}); return false;", orderID));
                }

                if (drv["BookedInStateID"] != DBNull.Value && (int)drv["BookedInStateID"] == (int)eBookedInState.BookedIn)
                {
                    lnkSetBookIn.Visible = false;
                    imgOrderBookedIn.Src = "/images/tick.gif";
                    imgOrderBookedIn.Visible = true;
                }

                #endregion

                #region rate Details
                int lcid = (int)drv["LCID"];
                decimal rate = (decimal)drv["ForeignRate"];
                TextBox txtRate = e.Item.FindControl("txtRate") as TextBox;
                CultureInfo culture = new CultureInfo(lcid);
                txtRate.Attributes.Add("symbol", culture.NumberFormat.CurrencySymbol);
                txtRate.Text = rate.ToString();
                #endregion

                #region Wire up the Input Manager controls
                TextBox txtCollectAt = e.Item.FindControl("txtCollectAtDate") as TextBox;
                TextBox txtCollectAtTime = e.Item.FindControl("txtCollectAtTime") as TextBox;
                TextBox txtDeliverAtFrom = e.Item.FindControl("txtDeliverAtFromDate") as TextBox;
                TextBox txtDeliverAtFromTime = e.Item.FindControl("txtDeliverAtFromTime") as TextBox;
                TextBox txtDeliverAtBy = e.Item.FindControl("txtDeliverByFromDate") as TextBox;
                TextBox txtDeliverAtByTime = e.Item.FindControl("txtDeliverByFromTime") as TextBox;
                TextBox txtCollectBy = e.Item.FindControl("txtCollectByDate") as TextBox;
                TextBox txtCollectByTime = e.Item.FindControl("txtCollectByTime") as TextBox;

                rimApproveOrder.GetSettingByBehaviorID("DateInputBehaviour").TargetControls.Add(new TargetInput(txtCollectAt.UniqueID, true));
                rimApproveOrder.GetSettingByBehaviorID("DateInputBehaviour").TargetControls.Add(new TargetInput(txtDeliverAtFrom.UniqueID, true));
                rimApproveOrder.GetSettingByBehaviorID("DateInputBehaviour").TargetControls.Add(new TargetInput(txtDeliverAtBy.UniqueID, true));
                rimApproveOrder.GetSettingByBehaviorID("DateInputBehaviour").TargetControls.Add(new TargetInput(txtCollectBy.UniqueID, true));

                rimApproveOrder.GetSettingByBehaviorID("TimeInputBehaviour").TargetControls.Add(new TargetInput(txtCollectAtTime.UniqueID, true));
                rimApproveOrder.GetSettingByBehaviorID("TimeInputBehaviour").TargetControls.Add(new TargetInput(txtDeliverAtFromTime.UniqueID, true));
                rimApproveOrder.GetSettingByBehaviorID("TimeInputBehaviour").TargetControls.Add(new TargetInput(txtDeliverAtByTime.UniqueID, true));
                rimApproveOrder.GetSettingByBehaviorID("TimeInputBehaviour").TargetControls.Add(new TargetInput(txtCollectByTime.UniqueID, true));

                rimApproveOrder.GetSettingByBehaviorID("NumericInputBehaviour").TargetControls.Add(new TargetInput(txtRate.UniqueID, true));

                #endregion

                #region Handle the Unaproved point
                bool itemHasPointsOrOrganisationsThatRequireApproval = false;

                PlaceHolder litViewCollectionOrganisation = e.Item.FindControl("litViewCollectionOrganisation") as PlaceHolder;
                Label lblCollectFromOrganisation = (Label)e.Item.FindControl("lblCollectFromOrganisation");

                String convertedName = string.IsNullOrWhiteSpace(drv["CollectionOrganisationName"].ToString()) ? "Unnamed" : drv["CollectionOrganisationName"].ToString();

                if (collectionOrganisationState != eIdentityStatus.Active)
                {
                    itemHasPointsOrOrganisationsThatRequireApproval = true;
                    litViewCollectionOrganisation.Controls.Add(new LiteralControl(string.Format(unApprovedPoint, string.Format("javascript:openDialogWithScrollbars('ApproveOrderOrganisation.aspx?OrganisationID={0}',700,700);", drv["CollectionOrganisationID"].ToString()), convertedName)));
                    litViewCollectionOrganisation.Controls.Add(new LiteralControl("<br/><span style='color:red'>" + collectionOrganisationState.ToString() + "</span>"));
                }
                else
                    litViewCollectionOrganisation.Controls.Add(new LiteralControl(convertedName));

                PlaceHolder litViewCollectionPoint = e.Item.FindControl("litViewCollectionPoint") as PlaceHolder;
                if (collectionPointState == ePointState.Unapproved)
                {
                    itemHasPointsOrOrganisationsThatRequireApproval = true;
                    litViewCollectionPoint.Controls.Add(new LiteralControl(string.Format(unApprovedPoint, string.Format("javascript:openDialogWithScrollbars('ApproveOrderPoint.aspx?PointId={0}&OrderId={1}&Ptype={2}',400,700);", collectionPointId, orderID, "C"), drv["CollectionPointDescription"].ToString())));
                    litViewCollectionPoint.Controls.Add(new LiteralControl("<br/><span style='color:red'>to be approved</span>"));
                }
                else
                {
                    if (Globals.Configuration.GPSRealtime)
                    {
                        Label lblCollectFromPoint = (Label)e.Item.FindControl("lblCollectFromPoaint");
                        //lblCollectFromPoint.Text = drv["CollectionPointDescription"].ToString(); ;

                        HyperLink hl = new HyperLink();
                        if (drv["CollectionLatitude"] != DBNull.Value && double.Parse(drv["CollectionLatitude"].ToString()) != 0)
                        {
                            hl.NavigateUrl = string.Format("javascript:ViewPoint({0}, {1}, '{2}');", drv["CollectionLatitude"].ToString(), drv["CollectionLongitude"].ToString(), Server.HtmlEncode(drv["CollectionPointDescription"].ToString()));
                            hl.Text = Server.HtmlEncode(drv["CollectionPointDescription"].ToString());
                            litViewCollectionPoint.Controls.Add(hl);
                        }
                        else
                        {
                            litViewCollectionPoint.Controls.Add(new LiteralControl(drv["CollectionPointDescription"].ToString()));
                        }
                    }
                    else
                    {
                        litViewCollectionPoint.Controls.Add(new LiteralControl(drv["CollectionPointDescription"].ToString()));
                    }


                }

                PlaceHolder litViewDeliveryOrganisation = e.Item.FindControl("litViewDeliveryOrganisation") as PlaceHolder;
                Label lblDeliverToOrganisation = (Label)e.Item.FindControl("lblDeliverToOrganisation");

                convertedName = string.IsNullOrWhiteSpace(drv["DeliveryOrganisationName"].ToString()) ? "Unnamed" : drv["DeliveryOrganisationName"].ToString();

                if (deliveryOrganisationState != eIdentityStatus.Active)
                {
                    itemHasPointsOrOrganisationsThatRequireApproval = true;
                    litViewDeliveryOrganisation.Controls.Add(new LiteralControl(string.Format(unApprovedPoint, string.Format("javascript:openDialogWithScrollbars('ApproveOrderOrganisation.aspx?OrganisationID={0}',700,700);", drv["DeliveryOrganisationID"].ToString()), convertedName)));
                    litViewDeliveryOrganisation.Controls.Add(new LiteralControl("<br/><span style='color:red'>" + deliveryOrganisationState.ToString() + "</span>"));
                }
                else
                    litViewDeliveryOrganisation.Controls.Add(new LiteralControl(convertedName));

                PlaceHolder litViewDeliveryPoint = e.Item.FindControl("litViewDeliveryPoint") as PlaceHolder;
                if (deliveryPointState == ePointState.Unapproved)
                {
                    itemHasPointsOrOrganisationsThatRequireApproval = true;
                    litViewDeliveryPoint.Controls.Add(new LiteralControl(string.Format(unApprovedPoint, string.Format("javascript:openDialogWithScrollbars('ApproveOrderPoint.aspx?PointId={0}&OrderId={1}&Ptype={2}',400,700);", deliveryPointId, orderID, "D"), drv["DeliveryPointDescription"].ToString())));
                    litViewDeliveryPoint.Controls.Add(new LiteralControl("<br/><span style='color:red'>to be approved</span>"));
                }
                else
                {
                    if (Globals.Configuration.GPSRealtime)
                    {
                        if (drv["DeliveryLatitude"] != DBNull.Value && double.Parse(drv["DeliveryLatitude"].ToString()) != 0)
                        {
                            HyperLink hl = new HyperLink();
                            hl.Text = drv["DeliveryPointDescription"].ToString();
                            hl.NavigateUrl = string.Format("javascript:ViewPoint({0}, {1}, '{2}');", drv["DeliveryLatitude"].ToString(), drv["DeliveryLongitude"].ToString(), Server.HtmlEncode(drv["DeliveryPointDescription"].ToString()));
                            if (deliveryPointState == ePointState.Unapproved)
                                hl.Text = "show";
                            else
                                hl.Text = Server.HtmlEncode(drv["DeliveryPointDescription"].ToString());

                            litViewDeliveryPoint.Controls.Add(hl);
                        }
                        else
                        {
                            litViewDeliveryPoint.Controls.Add(new LiteralControl(drv["DeliveryPointDescription"].ToString()));
                        }
                    }
                    else
                        litViewDeliveryPoint.Controls.Add(new LiteralControl(drv["DeliveryPointDescription"].ToString()));
                }





                if (itemHasPointsOrOrganisationsThatRequireApproval)
                {
                    chkSelectOrder.Enabled = false;
                }
                else
                {
                    chkSelectOrder.Attributes.Add("onclick", "javascript:ChangeList(event,this);orderCheckChange('" + orderGroupId.ToString() + "', this);");
                }
                #endregion

                chkSelectOrder.Attributes.Add("orderid", orderID.ToString());

                #region display the current time options for the collection and delivery

                txtCollectAt.Text = ((DateTime)drv["CollectionDateTime"]).ToString("dd/MM/yy");
                txtCollectAtTime.Text = ((DateTime)drv["CollectionDateTime"]).ToString("HH:mm");
                txtCollectBy.Text = ((DateTime)drv["CollectionByDateTime"]).ToString("dd/MM/yy");
                txtCollectByTime.Text = ((DateTime)drv["CollectionByDateTime"]).ToString("HH:mm");

                txtDeliverAtFrom.Text = ((DateTime)drv["DeliveryFromDateTime"]).ToString("dd/MM/yy");
                txtDeliverAtFromTime.Text = ((DateTime)drv["DeliveryFromDateTime"]).ToString("HH:mm");
                txtDeliverAtBy.Text = ((DateTime)drv["DeliveryDateTime"]).ToString("dd/MM/yy");
                txtDeliverAtByTime.Text = ((DateTime)drv["DeliveryDateTime"]).ToString("HH:mm");

                RadioButtonList rblCollectionTimeOptions = e.Item.FindControl("rblCollectionTimeOptions") as RadioButtonList;
                if (txtCollectAt.Text == txtCollectBy.Text && txtCollectAtTime.Text == "00:00"
                    && txtCollectByTime.Text == "23:59")
                {
                    // Any Time
                    rblCollectionTimeOptions.Items.FindByValue("2").Selected = true;
                    txtCollectBy.Style.Add("display", "none");
                    txtCollectByTime.Style.Add("display", "none");
                    txtCollectAtTime.Text = string.Empty;
                }
                else if (txtCollectAt.Text == txtCollectBy.Text && txtCollectAtTime.Text == txtCollectByTime.Text)
                {
                    // time booking
                    rblCollectionTimeOptions.Items.FindByValue("1").Selected = true;
                    txtCollectBy.Style.Add("display", "none");
                    txtCollectByTime.Style.Add("display", "none");
                }
                else
                {
                    // Booking Window
                    rblCollectionTimeOptions.Items.FindByValue("0").Selected = true;
                    txtCollectBy.Style.Add("display", "");
                    txtCollectByTime.Style.Add("display", "");
                }

                RadioButtonList rblDeliveryTimeOptions = e.Item.FindControl("rblDeliveryTimeOptions") as RadioButtonList;
                if (txtDeliverAtFrom.Text == txtDeliverAtBy.Text && txtDeliverAtFromTime.Text == "00:00"
                    && txtDeliverAtByTime.Text == "23:59")
                {
                    // Any Time
                    rblDeliveryTimeOptions.Items.FindByValue("2").Selected = true;
                    txtDeliverAtBy.Style.Add("display", "none");
                    txtDeliverAtByTime.Style.Add("display", "none");
                    txtDeliverAtFromTime.Text = string.Empty;
                }
                else if (txtDeliverAtFrom.Text == txtDeliverAtBy.Text && txtDeliverAtFromTime.Text == txtDeliverAtByTime.Text)
                {
                    // time booking
                    rblDeliveryTimeOptions.Items.FindByValue("1").Selected = true;
                    txtDeliverAtBy.Style.Add("display", "none");
                    txtDeliverAtByTime.Style.Add("display", "none");
                }
                else
                {
                    // Booking Window
                    rblDeliveryTimeOptions.Items.FindByValue("0").Selected = true;
                    txtDeliverAtBy.Style.Add("display", "");
                    txtDeliverAtByTime.Style.Add("display", "");
                }
                #endregion

            }
        }

        void grdOrders_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {

            this.grdOrders.DataSource = null;

            if (this.IsPostBack)
            {
                int clientId = 0;
                int.TryParse(cboClient.SelectedValue, out clientId);

                //Get the orders.
                Facade.IOrder facOrder = new Facade.Order();
                DataSet orderData = null;
                if (clientId > 0)
                    orderData = facOrder.GetOrdersForClientAndStatus(clientId, eOrderStatus.Awaiting_Approval);
                else
                    orderData = facOrder.GetOrders(eOrderStatus.Awaiting_Approval);

                //Build a filter query from the dates entered
                string query = string.Empty;
                if (dteStartDate.SelectedDate.HasValue)
                    query += string.Format("CollectionDateTime >= #{0:MM/dd/yyyy}#", dteStartDate.SelectedDate.Value);

                if (dteEndDate.SelectedDate.HasValue)
                {
                    if (query.Length > 0)
                        query += " AND ";
                    query += string.Format("CollectionDateTime < #{0:MM/dd/yyyy}#", dteEndDate.SelectedDate.Value.AddDays(1));
                }

                //Filter the results and update the grid
                DataView dv = new DataView(orderData.Tables[0], query, "CollectionDateTime", DataViewRowState.CurrentRows);
                this.grdOrders.DataSource = dv;

                //Calculate and display the total number of pallets and orders
                object o = orderData.Tables[0].Compute("SUM(NoPallets)", query);
                long NoPalletSpaces = o == DBNull.Value ? 0 : (long)o;

                lblPalletCount.Text = NoPalletSpaces.ToString();
                lblOrderCount.Text = dv.Count.ToString();

                // hide the filter display
                this.ClientScript.RegisterStartupScript(this.GetType(), "hideFilters", "FilterOptionsDisplayHide();", true);
            }

        }

        #endregion

        #region Page methods for Booking In

        [System.Web.Services.WebMethod]
        public static bool BookIn(int orderID, string bookedInBy, string bookedInWith, string bookedInReferences, string dateOption, string datefromDate, string dateFromTime, string dateFromByDate, string dateFromByTime)
        {
            Orchestrator.EF.DataContext context = EF.DataContext.Current;
            Orchestrator.EF.Order order = context.OrderSet.FirstOrDefault(o => o.OrderId == orderID);

            Facade.IOrder facOrder = new Facade.Order();
            Entities.Order orderOriginal = facOrder.GetForOrderID(orderID);

            order.BookedIn = true;
            order.BookedInByUserName = bookedInBy;
            order.BookedInDateTime = DateTime.Now;
            order.BookedInStateId = (int)eBookedInState.BookedIn;
            order.BookedInWith = bookedInWith;
            order.BookedInReferences = bookedInReferences;

            // Set the date and time for the Delivery based on the book in etails
            if (int.Parse(dateOption) == 0) //window
            {
                order.DeliveryFromDateTime = DateTime.Parse(datefromDate).Add(TimeSpan.Parse(dateFromTime));
                order.DeliveryDateTime = DateTime.Parse(dateFromByDate).Add(TimeSpan.Parse(dateFromByTime));
            }
            else
            {
                order.DeliveryFromDateTime = DateTime.Parse(datefromDate).Add(TimeSpan.Parse(dateFromTime));
                order.DeliveryDateTime = DateTime.Parse(datefromDate).Add(TimeSpan.Parse(dateFromTime));
            }

            order.LastUpdateDate = DateTime.Now;
            order.LastUpdateUserID = bookedInBy;
            context.SaveChanges(true);

            Entities.Order orderNew = facOrder.GetForOrderID(orderID);

            var exports = Orchestrator.Application.GetSpecificImplementations<Application.IExportOrder>();

            if (exports.Any())
            {
                foreach (var export in exports)
                {
                    export.Update(orderNew, orderOriginal, bookedInBy);
                }
            }

            return true;
        }


        // lnkSetBookIn.Attributes.Add("onclick", string.Format("RemoveBookInRequired(this,{0}); return false;", orderID));
        //}

        //#region Requires Book In Control
        //if (drv["BookedInStateID"] != DBNull.Value && (int)drv["BookedInStateID"] == (int)eBookedInState.NotRequired)
        //{
        //    lnkSetBookIn.Text = "Set";
        //    lnkSetBookIn.ToolTip = "Set that this job needs to be Booked In.";
        //    lnkSetBookIn.Attributes.Add("onclick", string.Format("SetBookInRequired(this,{0}); return false;", orderID));


        [System.Web.Services.WebMethod]
        public static List<string> SetRequiresBookingIn(int orderID, string linkClientID, string userName)
        {
            Orchestrator.EF.DataContext context = EF.DataContext.Current;
            Orchestrator.EF.Order order = context.OrderSet.FirstOrDefault(o => o.OrderId == orderID);
            order.BookedIn = false;
            order.BookedInByUserName = string.Empty;
            order.BookedInDateTime = null; ;
            order.BookedInStateId = (int)eBookedInState.Required;
            order.LastUpdateDate = DateTime.Now;
            order.LastUpdateUserID = userName;
            context.SaveChanges(true);

            List<string> result = new List<string>();
            result.Add(true.ToString().ToLower());
            result.Add(linkClientID);
            result.Add(string.Format("RemoveBookInRequired(this,{0}); return false;", orderID));
            return result;
        }

        [System.Web.Services.WebMethod]
        public static List<string> RemoveRequiresBookingIn(int orderID, string linkClientID, string userName)
        {
            Orchestrator.EF.DataContext context = EF.DataContext.Current;
            Orchestrator.EF.Order order = context.OrderSet.FirstOrDefault(o => o.OrderId == orderID);
            order.BookedIn = false;
            order.BookedInByUserName = string.Empty;
            order.BookedInDateTime = null; ;
            order.BookedInStateId = (int)eBookedInState.NotRequired;
            order.LastUpdateDate = DateTime.Now;
            order.LastUpdateUserID = userName;
            context.SaveChanges(true);

            List<string> result = new List<string>();
            result.Add(true.ToString().ToLower());
            result.Add(linkClientID);
            result.Add(string.Format("SetBookInRequired(this,{0}); return false;", orderID));
            return result;
        }
        #endregion

        #region Page Methods for Storing changes
        [System.Web.Services.WebMethod]
        public static List<string> UpdateOrder(int orderID, DateTime collectFromDate, DateTime? collectFromTime, DateTime collectByDate, DateTime? collectByTime, int collectionTimeType,
                                                    DateTime deliverFromDate, DateTime? deliverFromTime, DateTime deliverToDate, DateTime? deliverToTime, int deliveryTimeType,
                                                    decimal rate, string deliveryNotes, string userID)
        {
            List<string> result = new List<string>();

            result.Add(true.ToString());

            Orchestrator.EF.DataContext data = EF.DataContext.Current;
            Orchestrator.EF.Order order = data.OrderSet.First(o => o.OrderId == orderID);

            //determine the date(s) to use

            order.CollectionDateTime = collectFromDate.Add(collectFromTime.HasValue ? new TimeSpan(collectFromTime.Value.Hour, collectFromTime.Value.Minute, 0) : new TimeSpan(23, 59, 0));
            if (collectionTimeType == 1)
                order.CollectionByDateTime = order.CollectionDateTime;
            else
                order.CollectionByDateTime = collectByDate.Add(collectByTime.HasValue ? new TimeSpan(collectByTime.Value.Hour, collectByTime.Value.Minute, 0) : new TimeSpan(23, 59, 0));

            order.DeliveryFromDateTime = deliverFromDate.Add(deliverFromTime.HasValue ? new TimeSpan(deliverFromTime.Value.Hour, deliverFromTime.Value.Minute, 0) : new TimeSpan(23, 59, 0));
            if (deliveryTimeType == 1)
                order.DeliveryDateTime = order.DeliveryFromDateTime.Value;
            else
                order.DeliveryDateTime = deliverToDate.Add(deliverToTime.HasValue ? new TimeSpan(deliverToTime.Value.Hour, deliverToTime.Value.Minute, 0) : new TimeSpan(23, 59, 0));

            order.LastUpdateDate = DateTime.Now;
            order.LastUpdateUserID = userID;

            order.DeliveryNotes = deliveryNotes;
            order.ForeignRate = rate;

            Facade.IExchangeRates facER = new Facade.ExchangeRates();
            CultureInfo nativeCulture = new CultureInfo(Orchestrator.Globals.Configuration.NativeCulture);

            if (order.LCID != nativeCulture.LCID)
            {
                order.ExchangeRateID = facER.GetCurrentExchangeRateID(Facade.Culture.GetCurrencySymbol(order.LCID.Value), order.CollectionDateTime);
                order.Rate = facER.GetConvertedRate(order.ExchangeRateID.Value, order.ForeignRate.Value);
            }
            else
                order.Rate = decimal.Round(order.ForeignRate.Value, 4, MidpointRounding.AwayFromZero);

            data.SaveChanges();

            result.Add(orderID.ToString());


            return result;
        }
        #endregion

        #region Page Methods for checking order rejection

        [System.Web.Services.WebMethod]
        public static object CanRejectOrders(string orderIdList)
        {
            var orderIDs = orderIdList
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.Parse(s));

            bool canReject = true;
            string message = null;

            var cannotCancelReasons = new Dictionary<int, eCannotCancelOrderReason>();
            var ordersToReject = Enumerable.Empty<int>();

            Facade.IOrder facOrder = new Facade.Order();
            foreach (int orderID in orderIDs)
            {
                var order = facOrder.GetForOrderID(orderID);

                IEnumerable<int> ordersToRejectHere;
                eCannotCancelOrderReason? reason;
                canReject &= facOrder.CanRejectOrCancel(order, out ordersToRejectHere, out reason);
                ordersToReject = ordersToReject.Union(ordersToRejectHere);

                if (!canReject)
                    cannotCancelReasons.Add(orderID, reason.Value);
            }

            if (canReject)
            {
                var otherOrders = ordersToReject.Where(o => !orderIDs.Contains(o));
                if (otherOrders.Any())
                {
                    message = string.Format(
                        "One or more of the selected orders are part of a group which may only be rejected as a whole.  As a result, if you continue, order{0} {1} will also be rejected or cancelled.",
                        otherOrders.Count() == 1 ? string.Empty : "s",
                        Entities.Utilities.SentenceMerge(otherOrders.Select(oid => oid.ToString())));
                }
            }
            else
            {
                var messages = new Dictionary<eCannotCancelOrderReason, string>
                    {
                        { eCannotCancelOrderReason.Already_Rejected, "Cannot reject order {0}: the order has already been rejected." },
                        { eCannotCancelOrderReason.Already_Cancelled, "Cannot reject order {0}: the order has already been cancelled." },
                        { eCannotCancelOrderReason.Invoiced, "Cannot reject order {0}: this order has been invoiced." },
                        { eCannotCancelOrderReason.Grouped_With_Delivered, "Order {0} cannot be rejected because it is part of a group which may only be rejected as a whole and which contains one or more orders which have already been delivered." },
                        { eCannotCancelOrderReason.Grouped_With_On_Run, "Order {0} order cannot be rejected because it is part of a group which may only be rejected as a whole and which contains one or more orders which are on a run.  All orders in the group must first be removed from any run they are on." },
                    };

                var reasons = cannotCancelReasons.Select(ccr => string.Format(messages[ccr.Value], ccr.Key));
                message = string.Concat(
                    "Could not reject order(s):\n\n",
                    string.Join("\n", reasons.ToArray()));
            }

            return new
            {
                CanReject = canReject,
                OrdersToReject = string.Join(",", ordersToReject.Select(o => o.ToString()).ToArray()),
                Message = message,
            };
        }

        #endregion Page Methods for checking order rejection

    }
}
