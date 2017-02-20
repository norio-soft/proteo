using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Transactions;

namespace Orchestrator.WebUI.Groupage
{
    public partial class ApproveOrderPoint : Orchestrator.Base.BasePage
    {
        public Entities.Point Point { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                int orderId = Convert.ToInt32(Request.QueryString["OrderId"].ToString());
                int pointId = Convert.ToInt32(Request.QueryString["PointId"].ToString());
                
                string pointType = Request.QueryString["PType"].ToString();
                if (pointType.ToUpper() == "D")
                    pointType = "delivery";
                else if (pointType.ToUpper() == "C")
                    pointType = "collection";

                // Set the page title.
                this.Master.WizardTitle = string.Format("Approve {0} point for order {1}", pointType, orderId); ;
                
                lblProposedPoint.Text = string.Format("Please approve the proposed {0} point for order {1}", pointType, orderId);
                lblProposedPoint.Font.Bold = true;

                lblAlternativePoint.Text = string.Format("... or choose an alternative {0} point for order {1}", pointType, orderId);
                lblAlternativePoint.Font.Bold = true;

                Facade.Point facPoint = new Orchestrator.Facade.Point();
                this.Point = facPoint.GetPointForPointId(pointId);

                string fullAddress = this.Point.Address.ToString();
                fullAddress = fullAddress.Replace("\n", "");
                fullAddress = fullAddress.Replace("\r", "<br>");

                lblFullAddress.Text = this.Point.OrganisationName + "<br />" + fullAddress;

                string jsInjection = @"
                try { resizeTo(630, 730); }
                catch (err) { }
                window.focus();
                moveTo(30, 20);";

                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "ApproveOnly", jsInjection, true);
            }
        }

        protected void btnApprovePoint_Click(object sender, EventArgs e)
        {
            int orderId = Convert.ToInt32(Request.QueryString["OrderId"].ToString());
            int pointId = Convert.ToInt32(Request.QueryString["PointId"].ToString());

            Facade.Point facPoint = new Orchestrator.Facade.Point();
            Entities.Point point = facPoint.GetPointForPointId(pointId);

            Facade.IOrganisation facOrg = new Facade.Organisation();
            Entities.Organisation org = facOrg.GetForName(point.OrganisationName);

            Entities.FacadeResult res = null;

            using (TransactionScope ts = new TransactionScope())
            {
                if (org.IdentityStatus == eIdentityStatus.Unapproved)
                {
                    // Set the identityStatus to approved
                    org.IdentityStatus = eIdentityStatus.Active;
                    facOrg.Update(org, this.Page.User.Identity.Name);
                }

                point.PointStateId = ePointState.Approved;
                res = facPoint.Update(point, this.Page.User.Identity.Name);

                ts.Complete();
            }

            if (res.Success)
            {
                this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ApprovePoint", "window.opener.__doPostBack('','Refresh');window.close();", true);
            }
            else
            {
                idErrors.Infringements = res.Infringements;
                idErrors.DisplayInfringments();
                idErrors.Visible = true;
            }
        }

        protected void btnCloseForm_Click(object sender, EventArgs e)
        {
            this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "CancelApprovePoint", "window.close();", true);
        }

        protected void btnSubmitAlternative_Click(object sender, EventArgs e)
        {
            if (ucPoint.PointID > 0)
            {
                int orderId = Convert.ToInt32(Request.QueryString["OrderId"].ToString());
                int pointId = Convert.ToInt32(Request.QueryString["PointId"].ToString());

                string pointType = Request.QueryString["PType"].ToString();
                if (pointType.ToUpper() == "D")
                    pointType = "delivery";
                else if (pointType.ToUpper() == "C")
                    pointType = "collection";

                Facade.Point facPoint = new Orchestrator.Facade.Point();
                this.Point = facPoint.GetPointForPointId(pointId);

                this.Point.PointStateId = ePointState.Rejected;

                Facade.IOrder facOrder = new Orchestrator.Facade.Order();
                Entities.Order order = facOrder.GetForOrderID(orderId);

                switch (pointType)
                {
                    case "delivery":
                        order.DeliveryPointID = ucPoint.PointID;
                        break;

                    case "collection":
                        order.CollectionPointID = ucPoint.PointID;
                        break;

                    default:
                        break;
                }

                bool failTheTransaction = false;
                using (TransactionScope ts = new TransactionScope())
                {
                    // Do not set the point to rejected when submitting an alternative, doing so 
                    // may causse problems for other unapproved invoices that use the same point.

                    if (!failTheTransaction)
                    {
                        // Update the orders collection or delivery point to the alternative point.
                        bool orderUpdated = false;
                        orderUpdated = facOrder.Update(order, this.User.Identity.Name);

                        if (!orderUpdated) { failTheTransaction = true; }
                    }

                    if (!failTheTransaction)
                    {
                        ts.Complete();
                        this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "SubmitAlternativePoint", "window.opener.__doPostBack('','Refresh');window.close();", true);
                    }
                }
            }
            else
            {
                // User must first select an alternative point.
            }
        }
    }
}
