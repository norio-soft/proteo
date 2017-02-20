using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Orchestrator.WebUI.Report
{
    public partial class GetAllForClientAndLoad : Orchestrator.Base.BasePage
    {
        protected CultureInfo GetCulture(int lcid)
        {
            return new CultureInfo(lcid);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnExport.Click += new EventHandler(btnExport_Click);
            this.btnGetJobs.Click += new EventHandler(btnGetJobs_Click);
            this.cboClient.ItemsRequested +=new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
        }

        void btnGetJobs_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                //DataSet dsJobs = GetReportData();
                //dgJobs.DataSource = dsJobs;
                //dgJobs.DataBind();
                
                //pnlGrid.Visible = true;

                //if (cboClient.SelectedValue == string.Empty)
                //    dgJobs.Levels[0].Columns[0].Visible = true;

                ActionReportData();
            }
        }

        private DataSet GetReportData()
        {
            int identityId = 0;

            if (cboClient.SelectedValue != string.Empty)
                identityId =  Convert.ToInt32(cboClient.SelectedValue);

            //DateTime start = dteStartDate.SelectedDate.Value;
            DateTime start = rdiStartDate.SelectedDate.Value;

            start = start.Subtract(start.TimeOfDay);

            //DateTime end = dteEndDate.SelectedDate.Value;
            DateTime end = rdiEndDate.SelectedDate.Value;

            end = end.Subtract(end.TimeOfDay).Add(new TimeSpan(23, 59, 59));

            Facade.IJob facJob = new Facade.Job();
            DataSet dsCallInSheet = null;

            dsCallInSheet = facJob.GetAllForIdentityAndLoadDate(identityId, start, end, chkUserPlannedTimes.Checked);

            return dsCallInSheet;

        }

        private void ActionReportData()
        {
            int identityId = 0;

            if (cboClient.SelectedValue != string.Empty)
                identityId = Convert.ToInt32(cboClient.SelectedValue);

            DateTime start = rdiStartDate.SelectedDate.Value;
            start = start.Subtract(start.TimeOfDay);

            DateTime end = rdiEndDate.SelectedDate.Value;
            end = end.Subtract(end.TimeOfDay).Add(new TimeSpan(23, 59, 59));

            Facade.IJob facJob = new Facade.Job();
            DataSet dsCallInSheet = facJob.GetAllForIdentityAndLoadDate(identityId, start, end, chkUserPlannedTimes.Checked);

            if (dsCallInSheet.Tables.Count > 0)
            {
                var returnedOrders = from row in dsCallInSheet.Tables[0].Rows.Cast<DataRow>()
                                     group row by
                                     new { Client = row["Client"], Description = row["Description"], PostTown = row["PostTown"] } into g
                                     orderby g.Key.Client, g.Key.Description
                                     select new
                                     {
                                         Destination = g.Key,
                                         Orders = g.Count(),
                                         Items = g
                                     };
                                     

                lvReturnedOrders.DataSource = returnedOrders;
                lvReturnedOrders.DataBind();
            }
            else
            {
                lvReturnedOrders.DataSource = dsCallInSheet;
                lvReturnedOrders.DataBind();
            }
        }

        void btnExport_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                DataSet dsCallInSheet = GetReportData();

                Session["__ExportDS"] = dsCallInSheet.Tables[0];

                Server.Transfer("../Reports/csvexport.aspx?filename=DailyLogExport.csv");
            }
        }

        void cboClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboClient.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllClientsFiltered(e.Text);

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
                cboClient.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }
    }
}