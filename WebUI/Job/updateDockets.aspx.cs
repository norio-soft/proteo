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

namespace Orchestrator.WebUI.Job
{
    public partial class UpdateDockets : Orchestrator.Base.BasePage
    {
        #region Properties

        private int _instructionId = 0;

        private int _jobID = -1;
        public int JobID 
        {
            get
            {
                if (_jobID < 0)
                    _jobID = int.Parse(Request.QueryString["jid"]);

                return _jobID;
            }
        }

        #endregion

        #region Page Loading/Setup/Init

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Panel1.Visible = false;
            if (!IsPostBack)
            {
                BindCollectDrops();
            }
        }

        #endregion

        #region Data Loading

        private void BindCollectDrops()
        {
            Facade.IOrder facOrder = new Facade.Order();
            DataSet dsOrders = facOrder.GetForJob(this.JobID);
            gvDockets.DataSource = dsOrders;
            gvDockets.DataBind();
        }

        #endregion


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            gvDockets.RowEditing += new GridViewEditEventHandler(gvDockets_RowEditing);
            gvDockets.RowUpdating += new GridViewUpdateEventHandler(gvDockets_RowUpdating);
            gvDockets.RowCancelingEdit += new GridViewCancelEditEventHandler(gvDockets_RowCancelingEdit);
            btnClose.Click += new EventHandler(btnClose_Click);
            this.btnUpdateLoadNo.Click += new EventHandler(btnUpdateLoadNo_Click);
        }

        void btnUpdateLoadNo_Click(object sender, EventArgs e)
        {
            string orderIDs = string.Empty;
            Facade.IOrder facOrder = new Facade.Order();
            DataSet dsOrders = facOrder.GetForJob(this.JobID);

            foreach (DataRow row in dsOrders.Tables[0].Rows)
            {
                if (orderIDs.Length > 0)
                    orderIDs += ",";
                orderIDs += row["OrderID"].ToString();
            }

            facOrder.UpdateMultiplOrderReferences(orderIDs, txtLoadNo.Text, string.Empty, Page.User.Identity.Name);
            BindCollectDrops();
        }

        void btnClose_Click(object sender, EventArgs e)
        {
            this.ReturnValue = "Refresh";
            this.Close();
        }

        void gvDockets_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvDockets.ShowFooter = true;
            gvDockets.EditIndex = -1;
        }

      
        void gvDockets_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {

            //Update
            int orderID = int.Parse(((HiddenField)gvDockets.Rows[e.RowIndex].FindControl("hidOrderID")).Value);
            string deliveryOrderNumber = ((TextBox)gvDockets.Rows[e.RowIndex].FindControl("txtDocketNo")).Text;
            string loadNo = ((TextBox)gvDockets.Rows[e.RowIndex].FindControl("txtLoadNo")).Text;
            
            // This has been amended to update the information on the Orders 
            Facade.IOrder facOrder = new Facade.Order();
            facOrder.UpdateMultiplOrderReferences(orderID.ToString(), loadNo , deliveryOrderNumber, ((Entities.CustomPrincipal)Page.User).UserName);

            gvDockets.EditIndex = -1; 
            gvDockets.ShowFooter = true; 
            ViewState["__refresh"] = true;
            BindCollectDrops();
        }

        void gvDockets_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvDockets.ShowFooter = false;
            gvDockets.EditIndex = e.NewEditIndex;
            BindCollectDrops();
        }

            //void gvDockets_DataBound(object sender, EventArgs e)
            //{
            //    Facade.IJob facJob = new Facade.Job();
            //    DropDownList cboGoodsType;

            //    Entities.Instruction instruction = ((Entities.Instruction)ViewState["__instruction"]);
            //    int identityId = facJob.GetJob(instruction.JobId).IdentityId;

            //    foreach (GridViewRow row in gvDockets.Rows)
            //        if (row.RowIndex == gvDockets.EditIndex)
            //        {
            //            Entities.CollectDrop cd = instruction.CollectDrops[row.RowIndex];

            //            cboGoodsType = (DropDownList)row.FindControl("cboGoodsType");
            //            cboGoodsType.DataSource = Facade.GoodsType.GetGoodsTypesForClientAndCollectDrop(identityId, cd.CollectDropId);
            //            cboGoodsType.DataBind();
            //            cboGoodsType.ClearSelection();
            //            cboGoodsType.Items.FindByValue(((Entities.CollectDropCollection)gvDockets.DataSource)[row.RowIndex].GoodsTypeId.ToString()).Selected = true;
            //        }

            //    cboGoodsType = (DropDownList)gvDockets.FooterRow.FindControl("cboGoodsType");
            //    cboGoodsType.DataSource = Facade.GoodsType.GetGoodsTypesForClient(identityId);
            //    cboGoodsType.DataBind();
            //}
    }
}