using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Text;

namespace Orchestrator.WebUI
{
    public partial class changebusinesstype : Orchestrator.Base.BasePage
    {
        private DataSet _dsBusinessType = null;
        private DataSet BusinessTypeDataSet
        {
            get
            {
                if (_dsBusinessType == null)
                {
                    Facade.IBusinessType facBusinessType = new Facade.BusinessType();
                    _dsBusinessType = facBusinessType.GetAll();

                    //Set the Primary Key on the DataSet to allow Find to be used
                    _dsBusinessType.Tables[0].PrimaryKey = new DataColumn[] { _dsBusinessType.Tables[0].Columns[0] };
                }

                return _dsBusinessType;
            }
        }

        private DataSet _dsOrdColDelIDs = null;
        private DataSet OrderCollectionDeliveryDataSet
        {
            get
            {
                if (_dsOrdColDelIDs == null)
                {
                    Facade.IOrder facOrd = new Facade.Order();
                    _dsOrdColDelIDs = facOrd.GetCollectionAndDeliveryIDsForOrders(OrderIDCSV);

                    //Set the Primary Key on the DataSet to allow Find to be used
                    _dsOrdColDelIDs.Tables[0].PrimaryKey = new DataColumn[] { _dsOrdColDelIDs.Tables[0].Columns[0] };
                }

                return _dsOrdColDelIDs;
            }
        }

        private string OrderIDCSV = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            List<int> orderIDs = new List<int>();
            int businessTypeID = 0;
            string returnURL = string.Empty;

            if (!string.IsNullOrEmpty(Request.QueryString["oID"]))
            {
                string orderIDsCSV = Request.QueryString["oID"];
                string[] arrOrderIDs = orderIDsCSV.Split(',');
                foreach (string orderID in arrOrderIDs)
                {
                    int i = 0;
                    if (int.TryParse(orderID, out i) && !orderIDs.Contains(i))
                        orderIDs.Add(i);
                }
                OrderIDCSV = orderIDsCSV;
            }
                
            if (!string.IsNullOrEmpty(Request.QueryString["BT"]))
                int.TryParse(Request.QueryString["BT"], out businessTypeID);
            if (!string.IsNullOrEmpty(Request.QueryString["returnUrl"]))
                returnURL = Request.QueryString["returnUrl"];

            if (orderIDs.Count == 0 || businessTypeID == 0)
                return;

            //Change The Business Type
            Facade.IOrder facOrder = new Facade.Order();
            bool retVal = facOrder.ChangeBusinessType(orderIDs, businessTypeID, this.Page.User.Identity.Name);

            if (retVal)
            {
                DataRow drBusinessType = this.BusinessTypeDataSet.Tables[0].Rows.Find(businessTypeID);

                if ((bool)drBusinessType["IsPalletNetwork"])
                    UpdatePalletNetworkBusinessType(orderIDs, businessTypeID);

                Response.Redirect(returnURL);
            }
        }

        private void UpdatePalletNetworkBusinessType(List<int> orderIDs, int businessTypeID)
        {
            StringBuilder sb = new StringBuilder();

            foreach (int orderID in orderIDs)
            {
                EF.VigoOrder vigoOrder = EF.DataContext.Current.VigoOrderSet.Include("VigoOrderExtras").FirstOrDefault(v => v.OrderId == orderID);
                if (vigoOrder == null)
                {
                    //Get the BusinessType row for the selected BusinessType
                    DataRow drOrderDetails = this.OrderCollectionDeliveryDataSet.Tables[0].Rows.Find(orderID);

                    vigoOrder = new EF.VigoOrder();
                    EF.DataContext.Current.AddToVigoOrderSet(vigoOrder);

                    if(!string.IsNullOrEmpty((string)drOrderDetails["ShortDescription"]))
                        vigoOrder.OrderServiceLevelReference.EntityKey = new EntityKey("DataContext.OrderServiceLevelSet", "OrderServiceLevelId", (int)drOrderDetails["OrderServiceLevelID"]);
                    else
                        vigoOrder.OrderServiceLevelReference.EntityKey = new EntityKey("DataContext.OrderServiceLevelSet", "OrderServiceLevelId", 1); //Check with steele as to the best course of action for a default vigo service level if one isn't present.

                    vigoOrder.FullPallets = (int)drOrderDetails["NoOfPallets"];

                    if (sb.Length > 0)
                        sb.Append(",");

                    sb.Append(orderID.ToString());
                }
            }

            EF.DataContext.Current.SaveChanges();

            Facade.IOrder facOrd = new Facade.Order();
            facOrd.UpdateTrunkDateForOrderIDs(sb.ToString(), this.Page.User.Identity.Name);
        }
    }
}