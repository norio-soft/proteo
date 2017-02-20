using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace Orchestrator.WebUI.ws
{
    /// <summary>
    /// Summary description for OrderService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class OrderService : System.Web.Services.WebService
    {

        [WebMethod]
        public void SetBookedInState(int orderID, eBookedInState bookedInState, string userName)
        {
            Orchestrator.EF.DataContext context = new Orchestrator.EF.DataContext();
            Orchestrator.EF.Order order = context.OrderSet.FirstOrDefault(o => o.OrderId == orderID);
            order.BookedIn = false;
            order.BookedInByUserName = string.Empty;
            order.BookedInDateTime = null; ;
            order.BookedInStateId = (int)bookedInState;
            order.LastUpdateDate = DateTime.Now;
            order.LastUpdateUserID = userName;
            context.SaveChanges(true);
            
        }

        [WebMethod]
        public bool UpdateOrder(int orderID, DateTime collectFromDate, DateTime? collectFromTime, DateTime collectByDate, DateTime? collectByTime, int collectionTimeType,
                                                   DateTime deliverFromDate, DateTime? deliverFromTime, DateTime deliverToDate, DateTime? deliverToTime, int deliveryTimeType,
                                                   decimal rate, string deliveryNotes, string userID)
        {
            bool result = true;

            Orchestrator.EF.DataContext data = new Orchestrator.EF.DataContext();
            Orchestrator.EF.Order order = data.OrderSet.First(o => o.OrderId == orderID);

            //determine the date(s) to use

            order.CollectionDateTime = collectFromDate.Add(collectFromTime.HasValue ? new TimeSpan(collectFromTime.Value.Hour, collectFromTime.Value.Minute, 0) : new TimeSpan(23, 59, 0));
            if (collectionTimeType == 1 || collectionTimeType == 2)
                order.CollectionByDateTime = order.CollectionDateTime;
            else
                order.CollectionByDateTime = collectByDate.Add(collectByTime.HasValue ? new TimeSpan(collectByTime.Value.Hour, collectByTime.Value.Minute, 0) : new TimeSpan(23, 59, 0));

            order.DeliveryFromDateTime = deliverFromDate.Add(deliverFromTime.HasValue ? new TimeSpan(deliverFromTime.Value.Hour, deliverFromTime.Value.Minute, 0) : new TimeSpan(23, 59, 0));
            if (deliveryTimeType == 1 || deliveryTimeType == 2)
                order.DeliveryDateTime = order.DeliveryFromDateTime.Value;
            else
                order.DeliveryDateTime = deliverToDate.Add(deliverToTime.HasValue ? new TimeSpan(deliverToTime.Value.Hour, deliverToTime.Value.Minute, 0) : new TimeSpan(23, 59, 0));

            order.LastUpdateDate = DateTime.Now;
            order.LastUpdateUserID = userID;

            order.DeliveryNotes = deliveryNotes;
            order.ForeignRate = rate;

            data.SaveChanges();

            return result;
        }

        [WebMethod]
        public bool BookIn(int orderID, string bookedInBy, string bookedInWith, string bookedInReferences, string dateOption, string datefromDate, string dateFromTime, string dateFromByDate, string dateFromByTime)
        {
            Orchestrator.EF.DataContext context = new Orchestrator.EF.DataContext();
            Orchestrator.EF.Order order = context.OrderSet.FirstOrDefault(o => o.OrderId == orderID);
            order.BookedIn = true;
            order.BookedInByUserName = bookedInBy;
            order.BookedInDateTime = DateTime.Now;
            order.BookedInStateId = (int)eBookedInState.BookedIn;
            order.BookedInWith = bookedInWith;
            order.BookedInReferences = bookedInReferences;

            // Set the date and time for the Delivery based on the book in details
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

            return true;
        }

    }
}
