using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;

namespace Orchestrator.WebUI.ws
{
    /// <summary>
    /// Summary description for ClientDashboardServices
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ClientDashboardServices : System.Web.Services.WebService
    {

        [WebMethod]
        public List<ChartDataPoint> GetNumberofOrdersThisWeek(int clientIdentityID)
        {
            try
            {
                DateTime startDate = DateTime.Today;
                if (startDate.DayOfWeek != DayOfWeek.Monday)
                {
                    startDate = startDate.AddDays(-((int)startDate.DayOfWeek - (int)DayOfWeek.Monday));
                }

                DateTime endDate = startDate.AddDays(7);

                List<ChartDataPoint> retVal = new List<ChartDataPoint>();

                DataSet ds = Facade.ClientDashboard.GetOrdersForClientAndDateRangeByDay(clientIdentityID, startDate, endDate);
                
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    retVal.Add(new ChartDataPoint() { Label = row[0].ToString(), Value = (int)row[1] });
                }

                return retVal;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [WebMethod]
        public List<PODSummaryRow> GetAgedOutstandingPOD(int clientIdentityID, int age)
        {
            try
            {
                int? _age = null;
                if (age > 0)
                    _age = age;

                DataAccess.POD DAL = new Orchestrator.DataAccess.POD();
                DataSet dsPOD = DAL.GetAgedOutstandingPOD(clientIdentityID, _age);
                var retVal = (from r in dsPOD.Tables[0].AsEnumerable()
                             select new PODSummaryRow()
                             {
                             Days4 = r.Field<int>("4Days"),
                             Days7 = r.Field<int>("7Days"),
                             Days15 = r.Field<int>("15Days")
                             }).ToList();

                

                return retVal;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [WebMethod]
        public List<OutstandingPOD> GetOutstandingPODForAge(int clientIdentityID, int age)
        {
            try
            {
                int? _age = null;
                if (age > 0)
                    _age = age;

                DataAccess.POD DAL = new Orchestrator.DataAccess.POD();
                DataSet dsPOD = DAL.GetAgedOutstandingPOD(clientIdentityID, _age);
                var retVal = (from r in dsPOD.Tables[0].AsEnumerable()
                              select new OutstandingPOD()
                              {
                                  OrderID = r.Field<int>("OrderID"),
                                   RunID = r.Field<int>("JobID"),
                                   DeliveryDateTime= r.Field<DateTime>("DeliveryDateTime"), 
                                   ArrivalDateTime= r.Field<DateTime>("ArrivalDateTime"),
                                   CustomerOrderNumber = r.Field<string>("CustomerOrderNumber")
                              }).ToList();



                return retVal;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [WebMethod]
        public List<ClientTurnover> GetClientTurnover(int identityID)
        {
            DataSet ds = new Facade.Organisation().GetClientRevenuePerMonthReport(DateTime.Today.AddMonths(-13), DateTime.Today, false, true, (dateTime) => string.Format("{0} {1}", dateTime.ToString("MMM"), dateTime.Year));

            DataRow[] row = ds.Tables[0].Select("IdentityID = " + identityID.ToString());

            try
            {
                List<ClientTurnover> retVal = new List<ClientTurnover>();
                if (row.Length > 0)
                {
                    foreach (DataColumn col in ds.Tables[0].Columns)
                    {
                        if (col.ColumnName == "IdentityID" || col.ColumnName == "Customer")
                            continue;

                        retVal.Add(new ClientTurnover() { Label = col.ColumnName.Replace("20",""), Value = row[0][col.ColumnName] == DBNull.Value ? 0 : double.Parse(row[0][col.ColumnName].ToString()) });
                    }
                }


                return retVal;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [WebMethod]
        public UninvoiceWork GetAllUninvoicedWork(int identityID)
        {
            Facade.IOrganisation facOrg = new Facade.Organisation();
            DateTime endDate = DateTime.Today.AddMonths(-1);

            // this has been amended to look up to the end of the previous month
            System.Globalization.Calendar cal = System.Globalization.CultureInfo.CurrentCulture.Calendar;
            endDate = new DateTime(endDate.Year, endDate.Month, cal.GetDaysInMonth(endDate.Year, endDate.Month));

            DataSet ds = facOrg.GetAllUninvoicedWorkForOrganisation(identityID, DateTime.Today.AddYears(-1), endDate, true, true, true);
            
            try
            {
                int orderCount = 0;
                decimal orderValue = 0;
                foreach (DataRow row in ds.Tables[1].Rows)
                {
                    orderCount+= (int)row["CountOfJobs"];
                    orderValue += (decimal)row["Total Charge Amount"];
                }

                UninvoiceWork retVal = new UninvoiceWork() { NumberOfOrders = orderCount, ValueOfOrders = orderValue };
                return retVal;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

    }
    public class UninvoiceWork
    {
        public int NumberOfOrders { get; set; }
        public decimal ValueOfOrders { get; set; }
    }
    public class ClientTurnover
    {
        public String Label { get; set; }
        public double Value { get; set; }

    }
    public class ChartDataPoint
    {
        public string Label { get; set; }
        public int Value { get; set; }
    }

    public class PODSummaryRow
    {
        public int Days4 { get; set; }
        public int Days7 { get; set; }
        public int Days15 { get; set; }
    }

    public class OutstandingPOD
    {
        public int OrderID { get; set; }
        public int RunID { get; set; }
        public string Driver { get; set; }
        public string CustomerOrderNumber { get; set; }
        public DateTime DeliveryDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }
    }

}
