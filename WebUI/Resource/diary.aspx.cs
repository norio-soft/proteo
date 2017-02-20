using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Orchestrator.WebUI.Resource
{
    public partial class diary : System.Web.UI.Page
    {

        #region Page Load / Init
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DateTime startDate = DateTime.Today;
                if (startDate.DayOfWeek != DayOfWeek.Monday)
                {
                    if ((int)startDate.DayOfWeek > 1)
                        startDate = startDate.AddDays(-((int)startDate.DayOfWeek - 1));
                }
                dteStartDate.SelectedDate = startDate;
                dteEndDate.SelectedDate = DateTime.Today;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnGetdata.Click += new EventHandler(btnGetdata_Click);
            this.grdDiary.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdDiary_NeedDataSource);
            this.grdDiary.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(grdDiary_ItemDataBound);
        }

        void btnGetdata_Click(object sender, EventArgs e)
        {
            grdDiary.Rebind();
        }

        

        #endregion

        #region Data Loading
        void grdDiary_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {

           
            
        }

        void grdDiary_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            this.grdDiary.DataSource = GetData();
        }

        private DataTable GetData()
        {
            string orderDisplay = "<div><span class='spOrderID' >{4} <span style='font-weight:bold;'>Order ID:</span> <a href='javascript:viewOrder({0})'>{0}</a></span><span> <span style='font-weight:bold;'>Collect from: </span> {1}</span><span> <span style='font-weight:bold;'>Deliver to:</span> {2}</span><span> <span style='font-weight:bold;'>Refs : </span>{3}<span><br/></div>";
            string orderDetails = string.Empty;

            // Get the data for the diry view for the resource type.
            DataSet dsDiary = Facade.Resource.GetDiaryView(eResourceType.Driver, dteStartDate.SelectedDate.Value, dteEndDate.SelectedDate.Value.Subtract(dteEndDate.SelectedDate.Value.TimeOfDay).Add(new TimeSpan(23,59,59)));
            this.grdDiary.DataSource = dsDiary;
            
            DataTable dtResource = dsDiary.Tables[0];
            dtResource.Columns.Add("OrderDetails", typeof(string));
            dtResource.AcceptChanges();

            // Now group together the order informtion and poopulate the order details
            // this is initially sorted by driver resource id 
            foreach (DataRow row in dtResource.Rows)
            {
                
                DataView vwOrders = dsDiary.Tables[1].DefaultView;
                vwOrders.RowFilter = "DriverResourceId = " + row["ResourceID"].ToString();
                if (vwOrders.Count > 0)
                {
                    string orderSummary = "<div class='summaryOrder'>Number of Orders {0}&#160;<img src='/images/expand.jpg' alt=''/></div>";
                    orderSummary = string.Format(orderSummary, vwOrders.Count);
                    // New resource record.
                    orderDetails = orderSummary += "<div class='orders' style='display:none;'>";
                        
                    foreach (DataRow order in vwOrders.ToTable().Rows)
                    {


                        orderDetails += string.Format(orderDisplay, order["OrderID"], order["CollectionPoint"], order["DeliveryPoint"], order["CustomerOrderNumber"] + " / " + order["DeliveryOrderNumber"], order["ResourceManifestId"] == DBNull.Value ? "" : "<a href='javascript:viewManifest(" + order["ResourceManifestId"].ToString() + ");'>View Manifest</a>");
                    }
                    orderDetails += "</div>";
                    row["OrderDetails"] = orderDetails;
                    dtResource.AcceptChanges();
                }
               
            }
            dtResource.DefaultView.Sort = "FirstNames";
            return dtResource;
        }
        #endregion
    }
}
