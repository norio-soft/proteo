using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Runtime.Serialization;
using Orchestrator.EF;
using System.ComponentModel;


namespace Orchestrator.WebUI.ws
{
    /// <summary>
    /// Summary description for GridDataService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class GridDataService : System.Web.Services.WebService
    {

        [WebMethod]
        public List<SLOrder> GetOrdersToApprove(int? clientIdentityID)
        {
            Facade.IOrder facOrder = new Facade.Order();
            System.Data.DataSet dsOrders = null;
            List<SLOrder> ret = new List<SLOrder>();
            if (clientIdentityID.HasValue)
                dsOrders = facOrder.GetOrdersForClientAndStatus((int)clientIdentityID, eOrderStatus.Awaiting_Approval);
            else
                dsOrders = facOrder.GetOrders(eOrderStatus.Awaiting_Approval);
            try
            {
                // Make sure that all of the fields we want are available as LINQ throws no helpful error to advise what is missing.

                ret = (from r in dsOrders.Tables[0].AsEnumerable()
                           select new SLOrder()
                           {
                               OrderID = r.Field<int>("OrderID"),
                               CollectionPointStateID = r.Field<int>("CollectionPointStateId"),
                               DeliveryPointStateID = r.Field<int>("DeliveryPointStateId"),
                               CollectionPointID = r.Field<int>("CollectionPointId"),
                               DeliveryPointID = r.Field<int>("DeliveryPointId"),
                               BookedInStateID = r.Field<int>("BookedInStateID"),

                               NoPallets = r.IsNull("NoPallets") ? 0.0M :  decimal.Parse(r.Field<int>("NoPallets").ToString()),
                               Weight = r.IsNull("Weight") ? 0 :  r.Field<decimal>("Weight"),
                               ForeignRate = r.IsNull("ForeignRate") ? 0 : r.Field<decimal>("ForeignRate"),

                               CustomerOrganisationName = r.Field<string>("CustomerOrganisationName"),
                               CustomerOrderNumber = r.IsNull("CustomerOrderNumber") ? string.Empty :  r.Field<string>("CustomerOrderNumber"),
                               DeliveryOrderNumber = r.IsNull("DeliveryOrderNumber") ? string.Empty :  r.Field<string>("DeliveryOrderNumber"),
                               DeliveryNotes = r.IsNull("DeliveryNotes") ? string.Empty : r.Field<string>("DeliveryNotes"),
                               //RequestingDepot = r.Field<string>("RequestingDepot"),
                               OrderLevelShortCode = r.IsNull("OrderServiceLevelShortCode") ? string.Empty : r.Field<string>("OrderServiceLevelShortCode"),
                               Surcharges = r.IsNull("Surcharges") ? string.Empty :  r.Field<string>("Surcharges"),

                               CollectionDateTime = r.Field<DateTime>("CollectionDateTime"),
                               CollectionByDateTime = r.Field<DateTime>("CollectionByDateTime"),
                               DeliveryDateTime = r.Field<DateTime>("DeliveryDateTime"),
                               DeliverFromDateTime = r.Field<DateTime>("DeliveryFromDateTime"),

                               CollectionLatitude = r.IsNull("CollectionLatitude") ? 0.0M : r.Field<decimal>("CollectionLatitude"),
                               CollectionLongitude = r.IsNull("CollectionLongitude") ? 0.0M : r.Field<decimal>("CollectionLongitude"),
                               DeliveryLatitude = r.IsNull("DeliveryLatitude") ? 0.0M : r.Field<decimal>("DeliveryLatitude"),
                               DeliverLongitude = r.IsNull("DeliveryLongitude") ? 0.0M : r.Field<decimal>("DeliveryLongitude"),

                               CollectionPointDescription = r.IsNull("CollectionPointDescription") ? string.Empty : r.Field<string>("CollectionPointDescription"),
                               DeliveryPointDescription = r.IsNull("DeliveryPointDescription") ? string.Empty :  r.Field<string>("DeliveryPointDescription")
                           }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ret;

        }

       

        

      
    }

        

   




    public class SLOrder
    {
        public int OrderID { get; set; }
        public int CollectionPointStateID { get; set; }
        public int DeliveryPointStateID { get; set; }
        public int CollectionPointID { get; set; }
        public int DeliveryPointID { get; set; }
        public int BookedInStateID { get; set; }

        public decimal NoPallets { get; set; }
        public decimal Weight { get; set; }
        public decimal ForeignRate { get; set; }
        public decimal CollectionLatitude { get; set; }
        public decimal CollectionLongitude { get; set; }
        public decimal DeliveryLatitude { get; set; }
        public decimal DeliverLongitude { get; set; }

        public string CustomerOrganisationName { get; set; }
        public string CustomerOrderNumber { get; set; }
        public string DeliveryOrderNumber { get; set; }
        public string DeliveryNotes { get; set; }

        public string RequestingDepot { get; set; }
        public string OrderLevelShortCode { get; set; }
        public string Surcharges { get; set; }

        public string CollectionPointDescription { get; set; }
        public string DeliveryPointDescription { get; set; }

        public DateTime CollectionDateTime { get; set; }
        public DateTime CollectionByDateTime { get; set; }
        public DateTime DeliverFromDateTime { get; set; }
        public DateTime DeliveryDateTime { get; set; }

        public SLVigoOrder VigorOrder { get; set; }

    }

    public class SLVigoOrder
    {
        public int OrderId { get; set; }

        public int QtrPallets { get; set; }
        public int HalfPallets { get; set; }
        public int FullPallets { get; set; }
        public int OverPallets { get; set; }

        public string Note1 { get; set; }
        public string Note2 { get; set; }
        public string Note3 { get; set; }
        public string Note4 { get; set; }

        public string RequestingDepot { get; set; }
        public string CollectionDepot { get; set; }
        public string DeliveryDepot { get; set; }
    }
}
