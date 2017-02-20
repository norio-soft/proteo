using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;

using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Text;



namespace Orchestrator.WebUI.manifest
{
    public partial class drivermanifest : Orchestrator.Base.BasePage
    {

        #region Page Properties
        protected int JobID
        {
            get
            {
                return (int.Parse(Request.QueryString["jID"]));
            }
        }

        protected int DriverResourceID
        {
            get
            {
                return (int.Parse(Request.QueryString["drID"]));
            }
        }

        protected DateTime StartDate
        {
            get
            {
                return (DateTime.Parse(Request.QueryString["sd"]));
            }
        }

        protected DateTime EndDate
        {
            get
            {
                return (DateTime.Parse(Request.QueryString["ed"]));
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.grdInstructions.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdInstructions_NeedDataSource);
        }

        void grdInstructions_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            this.grdInstructions.DataSource = GetData();
        }

        #region Get the Data for the Display

        private DataSet GetData()
        {
            DataAccess.Manifest DAL = new DataAccess.Manifest();

            DataSet dsInstructions = DAL.GetInstructionsForDriver(this.DriverResourceID, this.JobID);
            DataSet retVal = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Action", typeof(string)));
            dt.Columns.Add(new DataColumn("Point", typeof(string)));
            dt.Columns.Add(new DataColumn("ActionDateTime", typeof(DateTime)));
            dt.Columns.Add(new DataColumn("Vehicle", typeof(string)));
            dt.Columns.Add(new DataColumn("Trailer", typeof(string)));
            dt.Columns.Add(new DataColumn("Notes", typeof(string)));
            dt.Columns.Add(new DataColumn("Orders", typeof(string)));
            retVal.Tables.Add(dt);

            string action = string.Empty;
            DataRow row = null;

            string previousTrailer = string.Empty;
            List<miniOrder> orders = new List<miniOrder>();

            string pickUpPoint = string.Empty;
            string trailerRef = string.Empty;   
            DateTime actionDateTime = DateTime.MinValue;
            string vehicle = string.Empty;
            int lastInstructiontypeID = -1;
            foreach (DataRow instruction in dsInstructions.Tables[0].Rows)
            {
                string driversVehicle = 
                    (instruction["RegNo"] == DBNull.Value) ? String.Empty : instruction["RegNo"].ToString();

                switch((int)instruction["InstructionTypeID"])
                {
                    case 1:
                    // this is a collection so all is well
                    action = "Collect From";
                    row = dt.NewRow();
                    row["Action"] = action;
                    row["Point"] = instruction["Point"];
                    row["ActionDateTime"] = instruction["PlannedArrivalDateTime"];
                    row["Vehicle"] = instruction["RegNo"];
                    row["Trailer"] = instruction["TrailerRef"];
                    row["Orders"] = instruction["Orders"];
                    previousTrailer = instruction["TrailerRef"].ToString();
                    dt.Rows.Add(row);
                    break;

                    case 2: // Drop
                        #region We have to pick up the trailer from the trailers last point before the current instruction order.
                        foreach (DataRow r in dsInstructions.Tables[1].Rows)
                        {
                            // Loop through the previous instructions on this job to determine what is on trailer
                            if ((int)r["InstructionOrder"] < (int)instruction["InstructionOrder"])
                            {
                                if (r["InstructionTypeID"].ToString() == "1")
                                {
                                    // Add these orders to the Orders collection
                                    orders.Add(new miniOrder((int)r["OrderID"], (int)r["CustomerIdentityID"], r["Customer"].ToString(), r["CustomerOrderNumber"].ToString(), r["DeliveryOrderNumber"].ToString(), (int)r["NoPallets"], (decimal)r["Weight"]));
                                }
                                if (r["InstructionTypeID"].ToString() == "2")
                                {
                                    // for each Order in this instruction remove this from the list of Orders we start with 
                                    for (int i = 0; i < orders.Count; i++)
                                        if (orders[i].OrderID == (int)r["OrderID"])
                                            orders.Remove(orders[i]);
                                }
                                if (r["InstructionTypeID"].ToString() == "7")
                                {
                                    if (r["OrderActionID"] != DBNull.Value)
                                    {
                                        // If Order is Cross Docked or Transhipped please remove from orders collection
                                        if ((int)r["OrderActionID"] == 3 || (int)r["OrderActionID"] == 2)
                                        {
                                            for (int i = 0; i < orders.Count; i++)
                                                if (orders[i].OrderID == (int)r["OrderID"])
                                                    orders.Remove(orders[i]);
                                        }
                                    }
                                }
                            }

                            if ((int)r["InstructionOrder"] == ((int)instruction["InstructionOrder"]) - 1)
                            {
                                pickUpPoint = r["Point"].ToString();
                                trailerRef = r["TrailerRef"].ToString();

                                // use the drivers vehicle if he has one
                                vehicle = driversVehicle;
                                actionDateTime = (DateTime)r["PlannedDepartureDateTime"];

                                // only add the pick up if the current driver did do the previous instruction
                                // or the trailer is different
                                if (
                                        (r["DriverResourceId"].GetType() == typeof(System.DBNull))
                                        ||
                                        ((int)r["DriverResourceId"] != (int)instruction["DriverResourceId"])
                                        ||
                                        trailerRef != (string)instruction["TrailerRef"]
                                    )
                                {
                                    // Add a new row to the output table for this instruction
                                    row = dt.NewRow();
                                    row["Action"] = "Pick Up Trailer";
                                    row["Point"] = pickUpPoint;
                                    row["Trailer"] = trailerRef;
                                    row["Vehicle"] = vehicle;
                                    row["ActionDateTime"] = actionDateTime;
                                    row["Orders"] = ConvertOrdersToCSV(orders);
                                    dt.Rows.Add(row);
                                    orders = new List<miniOrder>();

                                    break;
                                }
                            }
                        }
                        #endregion

                        #region This is a simple load and go job but spread over more than 1 day.
                        if (dsInstructions.Tables[1].Rows.Count == 1)
                            if (trailerRef != (string)instruction["TrailerRef"])
                            {
                                row = dt.NewRow();
                                row["Action"] = "Pick Up Trailer";
                                row["Point"] = pickUpPoint;
                                row["Trailer"] = trailerRef;
                                row["Vehicle"] = vehicle;
                                row["ActionDateTime"] = actionDateTime;
                                row["Orders"] = ConvertOrdersToCSV(orders);
                                dt.Rows.Add(row);
                                orders = new List<miniOrder>();
                            }
                        #endregion

                    // Get the Instruction details
                    action = "Deliver to";
                    row = dt.NewRow();
                    row["Action"] = action;
                    row["Point"] = instruction["Point"];
                    row["ActionDateTime"] = instruction["PlannedArrivalDateTime"];
                    row["Vehicle"] = instruction["RegNo"];
                    row["Trailer"] = instruction["TrailerRef"];
                    row["Orders"] = instruction["Orders"];
                    previousTrailer = instruction["TrailerRef"].ToString();
                    dt.Rows.Add(row);

                    break;
                    case 7: // Trunk
                        #region We have to pick up the trailer from the trailers last point before the current instruction order.
                        foreach (DataRow r in dsInstructions.Tables[1].Rows)
                        {
                            lastInstructiontypeID = (int)r["InstructionTypeID"];
                            // Loop through the previous instructions on this job to determine what is on trailer

                            if ((int)r["InstructionOrder"] < (int)instruction["InstructionOrder"])
                            {
                                if (r["InstructionTypeID"].ToString() == "1")
                                {
                                    // Add these orders to the Orders collection
                                    orders.Add(new miniOrder((int)r["OrderID"], (int)r["CustomerIdentityID"], r["Customer"].ToString(), r["CustomerOrderNumber"].ToString(), r["DeliveryOrderNumber"].ToString(), (int)r["NoPallets"], (decimal)r["Weight"]));
                                }
                                if (r["InstructionTypeID"].ToString() == "2")
                                {
                                    // for each Order in this instruction remove this from the list of Orders we start with 
                                    for (int i = 0; i < orders.Count; i++)
                                        if (orders[i].OrderID == (int)r["OrderID"])
                                            orders.Remove(orders[i]);
                                }
                                if (r["InstructionTypeID"].ToString() == "7")
                                {
                                    if (r["OrderActionID"] != DBNull.Value)
                                    {
                                        // If Order is Cross Docked or Transhipped please remove from orders collection
                                        if ((int)r["OrderActionID"] == 3 || (int)r["OrderActionID"] == 2)
                                        {
                                            for (int i = 0; i < orders.Count; i++)
                                                if (orders[i].OrderID == (int)r["OrderID"])
                                                    orders.Remove(orders[i]);
                                        }
                                    }
                                }
                            }
                            if ((int)r["InstructionOrder"] == ((int)instruction["InstructionOrder"]) - 1)
                            {
                                pickUpPoint = r["Point"].ToString();
                                trailerRef = r["TrailerRef"].ToString();
                                // use the drivers vehicle if he has one
                                vehicle = driversVehicle;
                                actionDateTime = (DateTime)r["PlannedDepartureDateTime"];

                                // Add a new row to the output table for this instruction 
                                if (
                                    (r["DriverResourceId"].GetType() == typeof(System.DBNull))
                                    ||
                                    ((int)r["DriverResourceId"] != (int)instruction["DriverResourceId"])
                                    ||
                                    trailerRef != (string)instruction["TrailerRef"]
                                   )
                                {
                                    // Add a new row to the output table for this instruction
                                    row = dt.NewRow();
                                    row["Action"] = "Pick Up Trailer";
                                    row["Point"] = pickUpPoint;
                                    row["Trailer"] = trailerRef;
                                    row["Vehicle"] = vehicle;
                                    row["ActionDateTime"] = actionDateTime;
                                    row["Orders"] = ConvertOrdersToCSV(orders);
                                    dt.Rows.Add(row);
                                    orders = new List<miniOrder>();
                               
                                    break;
                                }
                            }
                        }
                        #endregion

                        #region This is a simple load and go job but spread over more than 1 day.
                        if (dsInstructions.Tables[1].Rows.Count == 1)
                            if (trailerRef != (string)instruction["TrailerRef"])
                            {
                                row = dt.NewRow();
                                row["Action"] = "Pick Up Trailer";
                                row["Point"] = pickUpPoint;
                                row["Trailer"] = trailerRef;
                                row["Vehicle"] = vehicle;
                                row["ActionDateTime"] = actionDateTime;
                                row["Orders"] = ConvertOrdersToCSV(orders);
                                dt.Rows.Add(row);
                                orders = new List<miniOrder>();
                            }
                        #endregion

                    // Get the Instruction details
                    action = "Trunk to";
                    row = dt.NewRow();
                    row["Action"] = action;
                    row["Point"] = instruction["Point"];
                    row["ActionDateTime"] = instruction["PlannedArrivalDateTime"];
                    row["Vehicle"] = instruction["RegNo"];
                    row["Trailer"] = instruction["TrailerRef"];
                    row["Orders"] = instruction["Orders"];
                    previousTrailer = instruction["TrailerRef"].ToString();
                    dt.Rows.Add(row);
                     
                    break;
                }
                
            }

            return retVal;
        }

        private string ConvertOrdersToCSV(List<miniOrder> orders)
        {
            StringBuilder retVal = new StringBuilder();
            foreach (miniOrder o in orders)
            {
                if (retVal.Length > 0) retVal.Append("^^");
                retVal.Append(o.OrderID.ToString());
                retVal.Append("|");
                retVal.Append(o.CustomerIdentityID.ToString());
                retVal.Append("|");
                retVal.Append(o.Customer);
                retVal.Append("|");
                retVal.Append(o.CustomerOrderNumber);
                retVal.Append("|");
                retVal.Append(o.DeliveryOrderNumber);
                retVal.Append("|");
                retVal.Append(o.NoPallets.ToString());
                retVal.Append("|");
                retVal.Append(o.Weight.ToString());
            }

            return retVal.ToString();
        }
        #endregion

    }

    public struct miniOrder
    {
        public int     OrderID;
        public int CustomerIdentityID;
        public string Customer;
        public string CustomerOrderNumber;
        public string DeliveryOrderNumber;
        public int NoPallets;
        public decimal Weight;

        public miniOrder(int orderID, int customerIdentityID, string customer, string customerOrderNumber, string deliveryOrderNumber, int noPallets, decimal weight)
        {
            OrderID = orderID;
            Customer = customer;
            CustomerIdentityID = customerIdentityID;
            CustomerOrderNumber = customerOrderNumber;
            DeliveryOrderNumber = deliveryOrderNumber;
            NoPallets = noPallets;
            Weight = weight;
        }
    }
}
