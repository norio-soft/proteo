using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Orchestrator.Entities;

namespace Orchestrator.WebUI.Groupage
{
    public partial class cancelOrder : Orchestrator.Base.BasePage
    {
        private int[] OrdersToCancel
        {
            get { return (int[])(ViewState["OrdersToCancel"] ?? new int[0]); }
            set { ViewState["OrdersToCancel"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Master.WizardTitle = "Cancelling an Order";
                PopulateStaticFields();
                LoadOrder();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            btnConfirm.Click += new EventHandler(btnConfirm_Click);
            cvCancellationReason.ServerValidate += new ServerValidateEventHandler(cvCancellationReason_ServerValidate);
        }

        void cvCancellationReason_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = !string.IsNullOrEmpty(cboCancellationReason.SelectedValue) || !string.IsNullOrEmpty(txtCancellationReason.Text);
        }

        private void PopulateStaticFields()
        {
            var cancellationReasons = EF.DataContext.Current.CancellationReasonSet.Select(cr => cr.Description);
            bool reasonListEmpty = !cancellationReasons.Any();

            cboCancellationReason.Visible = !reasonListEmpty;
            cboCancellationReason.DataSource = cancellationReasons;
            cboCancellationReason.DataBind();

            cvCancellationReason.ErrorMessage =
                reasonListEmpty ?
                    "Please supply a cancellation reason" :
                    "Please either select a cancellation reason from the list or type a reason";
        }

        private void LoadOrder()
        {
            int orderID = Utilities.ParseNullable<int>(Request.QueryString["orderID"]) ?? 0;

            Facade.IOrder facOrder = new Facade.Order();
            var order = facOrder.GetForOrderID(orderID);

            string msg = null;
            bool canCancel = false;

            if (order == null)
                msg = "Error: order cannot be found";
            else
            {
                eCannotCancelOrderReason? reason;
                IEnumerable<int> ordersToCancel;
                canCancel = facOrder.CanRejectOrCancel(order, out ordersToCancel, out reason);

                if (canCancel)
                {
                    this.OrdersToCancel = ordersToCancel.ToArray();
                    var otherOrders = ordersToCancel.Where(oid => oid != orderID);

                    if (otherOrders.Any())
                    {
                        // Display a warning message... note that canCancel is true so this will be displayed but still allow the user to proceed with cancelling the orders.
                        msg = string.Format(
                            "This order is part of a group which may only be cancelled as a whole.  As a result, if you continue, order{0} {1} will also be cancelled.",
                            otherOrders.Count() == 1 ? string.Empty : "s",
                            Entities.Utilities.SentenceMerge(otherOrders.Select(oid => oid.ToString())));
                    }
                }
                else
                {
                    var messages = new Dictionary<eCannotCancelOrderReason, string>
                    {
                        { eCannotCancelOrderReason.Already_Rejected, "Cannot cancel: the order has already been rejected." },
                        { eCannotCancelOrderReason.Already_Cancelled, "Cannot cancel: the order has already been cancelled." },
                        { eCannotCancelOrderReason.Invoiced, "Cannot cancel: this order has been invoiced." },
                        { eCannotCancelOrderReason.Grouped_With_Delivered, "This order cannot be cancelled because it is part of a group which may only be cancelled as a whole and which contains one or more orders which have already been delivered." },
                        { eCannotCancelOrderReason.Grouped_With_On_Run, "This order cannot be cancelled because it is part of a group which may only be cancelled as a whole and which contains one or more orders which are on a run.  All orders in the group must first be removed from any run they are on." },
                    };

                    msg = messages[reason.Value];
                }
            }

            lblCancellationMessage.Text = msg;
            pnlCancellationMessage.Visible = !string.IsNullOrEmpty(msg);

            txtCancellationReason.Text = order.CancellationReason;
            pnlCancellationReason.Visible = canCancel || txtCancellationReason.Text.Length > 0;

            btnConfirm.Visible = canCancel;
        }

        void btnConfirm_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                string cancellationReason = Entities.Utilities.MergeStrings(" - ", cboCancellationReason.SelectedValue, txtCancellationReason.Text);

                Facade.IOrder facOrder = new Facade.Order();
                if (facOrder.Cancel(this.OrdersToCancel, cancellationReason, User.Identity.Name))
                {
                    btnConfirm.Visible = false;
                    lblCancellationMessage.Text = this.OrdersToCancel.Count() == 1 ? "The order has been cancelled" : "The orders have been cancelled";
                    lblCancellationMessage.Visible = true;
                }
            }
        }
    }
}