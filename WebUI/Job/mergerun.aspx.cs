using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Job
{
    public partial class mergerun : Orchestrator.Base.BasePage
    {
        protected int JobID
        {
            get
            {
                int jobID = -1;
                int.TryParse(Request.QueryString["jid"], out jobID);
                return jobID;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["searchedJob"] != null)
                txtSearch.Text = Session["searchedJob"].ToString();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdOrders.ItemDataBound += GrdOrders_ItemDataBound;
            grdOrders.NeedDataSource += GrdOrders_NeedDataSource;
            grdOrders.ItemCommand += GrdOrders_ItemCommand;
            grdOrders.Visible = false;
            btnMerge.Visible = false;

            dlgAddOrder.DialogCallBack += (o, ev) => 
            {
                System.Diagnostics.Debug.WriteLine(dlgAddOrder.ReturnValue);
                //this.Response.Redirect(this.Request.Url.ToString());
                grdOrders.Rebind();
            };
        }

        private void GrdOrders_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                bool showButton = Convert.ToBoolean(item.GetDataKeyValue("CanAddThisOrder"));
                Control btn = item["actionCol"].Controls[0];
                btn.Visible = showButton;
            }
        }

        private void GrdOrders_ItemCommand(object sender, GridCommandEventArgs e)
        {
            switch (e.CommandName.ToLower())
            {
                case "addorder":
                    {
                        string orderId = ((GridDataItem)e.Item).GetDataKeyValue("OrderID").ToString();
                        var qs = "oid=" + orderId + "&jid=" + JobID.ToString();
                        string result = dlgAddOrder.Open(qs);
                        System.Diagnostics.Debug.WriteLine(result);
                        break;
                    }
            }
        }

        private void GrdOrders_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (Page.IsPostBack && e.RebindReason != GridRebindReason.InitialLoad)
                FindOrders();
            else
                grdOrders.DataSource = null;
        }

        protected void btnFindRun_Click(object sender, EventArgs e)
        {
            Session["searchedJob"] = txtSearch.Text;
            if (txtSearch.Text.Length == 0 || !Utilities.ValidateNumericValue(txtSearch.Text))
                lblRunInformation.Text = "The Run ID that you entered is not valid.";
            else
                grdOrders.Rebind();
        }        

        protected string GetDate(DateTime date, bool anytime)
        {
            string retVal = string.Empty;

            if (anytime)
                retVal = date.ToString("dd/MM/yy") + " AnyTime";
            else
                retVal = date.ToString("dd/MM/yy HH:mm");

            return retVal;
        }

        private void FindOrders()
        {
            int jobId = int.Parse(txtSearch.Text);
            Facade.IJob facJob = new Facade.Job();
            Entities.Job job = facJob.GetJob(jobId);
            if (job == null)
            {
                lblRunInformation.Text = "No run can be found for this Run ID please check and re-key.";
                return;
            }

            if(job.JobState >= eJobState.InProgress)
            {
                lblRunInformation.Text = "This run is already in progress and can not be merged.";
                return;
            }

            Facade.IOrder facOrder = new Facade.Order();
            List<Entities.Order> orders = facOrder.GetForJobID(this.JobID);
            //string filter = string.Empty;
            //foreach (Entities.Order o in orders)
            //{
            //    if (filter.Length > 0)
            //        filter += ",";
            //    filter += o.OrderID.ToString();
            //}

            var ds = facOrder.GetForJob(jobId);
            List<MergableRunOrder> mergableOrders = new List<MergableRunOrder>();
            for(int index = 0; index < ds.Tables[0].Rows.Count; index++)
            {
                var dr = ds.Tables[0].Rows[index];
                MergableRunOrder mergableOrder = new MergableRunOrder();
                mergableOrder.OrderID = int.Parse(dr["OrderID"].ToString());
                mergableOrder.BusinessType = dr["BusinessType"].ToString();
                mergableOrder.Customer = dr["Customer"].ToString();
                mergableOrder.CustomerOrderNumber = dr["CustomerOrderNumber"].ToString();
                //mergableOrder.DeliveringResource = dr["DeliveryDriver"].ToString();
                mergableOrder.DeliveryOrderNumber = dr["DeliveryOrderNumber"].ToString();
                Entities.Order foundOrder = orders.Find(o => o.OrderID == mergableOrder.OrderID);
                if(foundOrder == null)
                {
                    mergableOrder.CanAddThisOrder = true;
                    mergableOrder.Status = "";
                }
                else
                {
                    mergableOrder.CanAddThisOrder = false;
                    mergableOrder.Status = "This Order is already part of the target run";
                }
                mergableOrders.Add(mergableOrder);
            }


            //var dv = ds.Tables[0].DefaultView;
            //if (!string.IsNullOrEmpty(filter))
            //    dv.RowFilter = "OrderID NOT IN (" + filter + ")";

            //if(dv.Count == 0)
            //{
            //    lblRunInformation.Text = "This run contains no order(s) that can be merged to the current run.";
            //    return;
            //}
            grdOrders.DataSource = mergableOrders;
            grdOrders.Visible = true;
            btnMerge.Visible = true;
            lblRunInformation.Text = string.Empty;
        }

        private void cancelJob()
        {
            int jobId = int.Parse(txtSearch.Text);
            Facade.IJob facJob = new Facade.Job();
            facJob.CancelJob(jobId, false, "Job merged to " + JobID.ToString(), this.User.Identity.Name);
        }

        protected void btnMerge_Click(object sender, EventArgs e)
        {
            cancelJob();
            this.ClientScript.RegisterStartupScript(this.Page.GetType(), "OnLoad", "window.opener.RefreshPage(); window.close();", true);
        }        
    }
}