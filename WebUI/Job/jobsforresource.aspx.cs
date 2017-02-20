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
    public partial class Job_jobsforresource : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.odsJobs.Selecting += new ObjectDataSourceSelectingEventHandler(odsJobs_Selecting);
            this.btnRefresh.Click += new EventHandler(btnRefresh_Click);
            this.grdJobs.DetailTableDataBind +=new Telerik.Web.UI.GridDetailTableDataBindEventHandler(grdJobs_DetailTableDataBind);
            cboSubContractor.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboSubContractor_ItemsRequested);
        }

        void grdJobs_DetailTableDataBind(object source, Telerik.Web.UI.GridDetailTableDataBindEventArgs e)
        {
            Telerik.Web.UI.GridDataItem dataItem = e.DetailTableView.ParentItem as Telerik.Web.UI.GridDataItem;

            if (e.DetailTableView.Name == "Orders")
            {
                int jobID = int.Parse(((HyperLink)dataItem["JobId"].Controls[0]).Text);
                Facade.IOrder facOrder = new Facade.Order();

                e.DetailTableView.DataSource = facOrder.GetOrdersForJob(jobID);
            }
        }

        void btnRefresh_Click(object sender, EventArgs e)
        {
            grdJobs.Rebind();
        }

        void odsJobs_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            if (cboSubContractor.SelectedValue.Length == 0)
                e.Cancel = true;
        }

        void cboSubContractor_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboSubContractor.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllDriversFiltered(e.Text);

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


    }
}