using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;


namespace Orchestrator.WebUI
{
    public class ManifestGeneration
    {
        public ManifestGeneration() { }

        /// <summary>
        /// Resource Manifest for Drivers.
        /// </summary>
        /// <remarks>
        /// Please note that if you specify to use Instruction order the manifest will use the Orders Date Time and will igonore the usePlannedTimes
        /// </remarks>
        public static DataTable GetDriverManifest(int resourceManifestID, int resourceID, bool usePlannedTimes, bool excludeFirstRow, bool showFullAddress, bool useInstructionOrder)
        {
            var facResourceManifest = new Orchestrator.Facade.ResourceManifest();
            var dsInstructions = facResourceManifest.GetResourceManifestJobForReport(resourceManifestID);

            return GetManifest(dsInstructions, resourceID, usePlannedTimes, excludeFirstRow, showFullAddress, useInstructionOrder);
        }

        private static DataTable GetManifest(DataSet dsInstructions, int resourceID, bool usePlannedTimes, bool excludeFirstRow, bool showFullAddress, bool useInstructionOrder)
        {
            DataTable finalData = new DataTable();
            DataTable dt = new DataTable("Table");
            dt.Columns.Add(new DataColumn("DriverName", typeof(string)));
            dt.Columns.Add(new DataColumn("JobId", typeof(int)));
            dt.Columns.Add(new DataColumn("Action", typeof(string)));
            dt.Columns.Add(new DataColumn("Point", typeof(string)));
            dt.Columns.Add(new DataColumn("ActionDateTime", typeof(DateTime)));
            dt.Columns.Add(new DataColumn("Vehicle", typeof(string)));
            dt.Columns.Add(new DataColumn("Trailer", typeof(string)));
            dt.Columns.Add(new DataColumn("Notes", typeof(string)));
            dt.Columns.Add(new DataColumn("Orders", typeof(string)));
            dt.Columns.Add(new DataColumn("InstructionOrder", typeof(string)));
            dt.Columns.Add(new DataColumn("EarliestDateTime", typeof(DateTime)));
            dt.Columns.Add(new DataColumn("ResourceId", typeof(int)));
            dt.Columns.Add(new DataColumn("InstructionTypeID", typeof(int)));
            dt.Columns.Add(new DataColumn("InstructionId", typeof(int)));
            dt.Columns.Add(new DataColumn("TotalOrders", typeof(int)));
            dt.Columns.Add(new DataColumn("TotalWeight", typeof(decimal)));
            dt.Columns.Add(new DataColumn("TotalPallets", typeof(int)));
            dt.Columns.Add(new DataColumn("TotalPalletSpaces", typeof(decimal)));
            dt.Columns.Add(new DataColumn("IsTimed", typeof(bool)));
            dt.Columns.Add(new DataColumn("OrderIsAnyTime", typeof(bool)));
            dt.Columns.Add(new DataColumn("Surcharges", typeof(string)));
            dt.Columns.Add(new DataColumn("JobOrder", typeof(int)));

            string action = string.Empty;
            string previousTrailer = string.Empty;
            List<miniOrder> orders = new List<miniOrder>();
            //Changed the way earliest date time is handled so that the Time part is set to zero rather than the actual maximum time
            DateTime earliestDateTime = new DateTime(9999,12,31,00,00,00);

            string pickUpPoint = string.Empty;
            string trailerRef = string.Empty;
            DateTime actionDateTime = DateTime.MinValue;
            string vehicle = string.Empty;
            int lastInstructiontypeID = -1;
            foreach (DataRow instruction in dsInstructions.Tables[0].Rows)
            {
                DataRow row = null;
                string driversVehicle = (instruction["RegNo"] == DBNull.Value) ? String.Empty : instruction["RegNo"].ToString();
                
                //Old method to work out the plannedArrivalDateTime which does not work as intended
                //DateTime plannedArrivalDateTime = useInstructionOrder ? Convert.ToDateTime(instruction["OrderDateTime"]) : usePlannedTimes ? Convert.ToDateTime(instruction["PlannedArrivalDateTime"]) : Convert.ToDateTime(instruction["OrderDateTime"]);
                DateTime plannedArrivalDateTime;
                if (usePlannedTimes == true)
                {
                    plannedArrivalDateTime = Convert.ToDateTime(instruction["PlannedArrivalDateTime"]);
                }
                else
                {
                    plannedArrivalDateTime = Convert.ToDateTime(instruction["OrderDateTime"]);
                }
                
                
                string point = showFullAddress ? instruction["FullAddress"].ToString() : instruction["Point"].ToString();


                // if InstructionTypeId is null then the driver has been removed from the job in question.
                if (instruction["InstructionTypeID"] == DBNull.Value)
                    break;

                switch ((int)instruction["InstructionTypeID"])
                {
                    case 1: //Load
                        #region Load

                        // this is a collection so all is well
                        action = "Collect From";
                        row = dt.NewRow();

                        row["DriverName"] = instruction["FullName"];
                        row["JobId"] = instruction["JobId"];
                        row["Action"] = action + Environment.NewLine + point;
                        row["Point"] = point;
                        row["ActionDateTime"] = plannedArrivalDateTime;
                        row["Vehicle"] = instruction["RegNo"];
                        row["Trailer"] = instruction["TrailerRef"];
                        row["Orders"] = instruction["Orders"];
                        row["Notes"] = GetOrderNotes(instruction);
                        row["ResourceId"] = resourceID;
                        row["InstructionOrder"] = instruction["InstructionOrder"];
                        row["InstructionId"] = instruction["InstructionId"];
                        row["InstructionTypeID"] = instruction["InstructionTypeID"];
                        row["IsTimed"] = instruction["IsTimed"];
                        row["OrderIsAnyTime"] = instruction["OrderIsAnyTime"];
                        row["Surcharges"] = instruction["Surcharges"];
                        row["JobOrder"] = instruction["JobOrder"];

                        previousTrailer = instruction["TrailerRef"].ToString();
                        earliestDateTime = earliestDateTime > plannedArrivalDateTime ? plannedArrivalDateTime : earliestDateTime;

                        #endregion
                        break;
                    case 2: // Drop
                        #region Drop

                        #region We have to pick up the trailer from the trailers last point before the current instruction order.
                        foreach (DataRow r in dsInstructions.Tables[1].Rows)
                        {
                            // Loop through the previous instructions on this job to determine what is on trailer
                            if ((int)r["InstructionOrder"] < (int)instruction["InstructionOrder"])
                            {
                                if (r["InstructionTypeID"].ToString() == "1")
                                {
                                    // Add these orders to the Orders collection
                                    if ((int)r["JobId"] == (int)instruction["JobId"])
                                        orders.Add(new miniOrder((int)r["OrderID"], (int)r["CustomerIdentityID"], r["Customer"].ToString(), r["CustomerOrderNumber"].ToString(), r["DeliveryOrderNumber"].ToString(), (int)r["NoPallets"], (decimal)r["Weight"], r["Surcharges"].ToString()));
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

                            // This is the previous instruction
                            if ((int)r["InstructionOrder"] == ((int)instruction["InstructionOrder"]) - 1 && (int)r["JobId"] == (int)instruction["JobId"])
                            {
                                pickUpPoint = showFullAddress ? r["FullAddress"].ToString() : r["Point"].ToString();
                                trailerRef = r["TrailerRef"].ToString();
                                // use the drivers vehicle if he has one
                                vehicle = driversVehicle;
                                actionDateTime = usePlannedTimes ? Convert.ToDateTime(r["PlannedDepartureDateTime"]) : Convert.ToDateTime(r["OrderDateTime"]);

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

                                    row["DriverName"] = instruction["FullName"];
                                    row["JobId"] = instruction["JobId"];
                                    row["Action"] = "Pick Up Trailer" + Environment.NewLine + pickUpPoint;
                                    row["Point"] = pickUpPoint;
                                    row["Trailer"] = trailerRef;
                                    row["Vehicle"] = vehicle;
                                    row["ActionDateTime"] = actionDateTime;
                                    row["Orders"] = ConvertOrdersToCSV(orders);
                                    row["Notes"] = GetOrderNotes(instruction);
                                    row["ResourceId"] = resourceID;
                                    row["InstructionId"] = instruction["InstructionId"];
                                    row["InstructionTypeID"] = instruction["InstructionTypeID"];
                                    row["InstructionOrder"] = instruction["InstructionOrder"];
                                    row["IsTimed"] = instruction["IsTimed"];
                                    row["OrderIsAnyTime"] = instruction["OrderIsAnyTime"];
                                    row["Surcharges"] = instruction["Surcharges"];
                                    row["JobOrder"] = instruction["JobOrder"];

                                    dt.Rows.Add(row);
                                    earliestDateTime = earliestDateTime > actionDateTime ? actionDateTime : earliestDateTime;
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

                                row["DriverName"] = instruction["FullName"];
                                row["JobId"] = instruction["JobId"];
                                row["Action"] = "Pick Up Trailer" + Environment.NewLine + pickUpPoint;
                                row["Point"] = pickUpPoint;
                                row["Trailer"] = trailerRef;
                                row["Vehicle"] = vehicle;
                                row["ActionDateTime"] = actionDateTime;
                                row["Orders"] = ConvertOrdersToCSV(orders);
                                row["Notes"] = GetOrderNotes(instruction);
                                row["ResourceId"] = resourceID;
                                row["InstructionOrder"] = instruction["InstructionOrder"];
                                row["InstructionId"] = instruction["InstructionId"];
                                row["InstructionTypeID"] = instruction["InstructionTypeID"];
                                row["IsTimed"] = instruction["IsTimed"];
                                row["OrderIsAnyTime"] = instruction["OrderIsAnyTime"];
                                row["Surcharges"] = instruction["Surcharges"];
                                row["JobOrder"] = instruction["JobOrder"];

                                dt.Rows.Add(row);
                                earliestDateTime = earliestDateTime > actionDateTime ? actionDateTime : earliestDateTime;
                                orders = new List<miniOrder>();
                            }

                        #endregion

                        // Get the Instruction details
                        action = "Deliver to";
                        row = dt.NewRow();

                        row["DriverName"] = instruction["FullName"];
                        row["JobId"] = instruction["JobId"];
                        row["Action"] = action + Environment.NewLine + point;
                        row["Point"] = point;
                        row["ActionDateTime"] = plannedArrivalDateTime;
                        row["Vehicle"] = instruction["RegNo"];
                        row["Trailer"] = instruction["TrailerRef"];
                        row["Orders"] = instruction["Orders"];
                        row["Notes"] = GetOrderNotes(instruction);
                        row["ResourceId"] = resourceID;
                        row["InstructionOrder"] = instruction["InstructionOrder"];
                        row["InstructionId"] = instruction["InstructionId"];
                        row["InstructionTypeID"] = instruction["InstructionTypeID"];
                        row["IsTimed"] = instruction["IsTimed"];
                        row["OrderIsAnyTime"] = instruction["OrderIsAnyTime"];
                        row["Surcharges"] = instruction["Surcharges"];
                        row["JobOrder"] = instruction["JobOrder"];

                        previousTrailer = instruction["TrailerRef"].ToString();
                        earliestDateTime = earliestDateTime > plannedArrivalDateTime ? plannedArrivalDateTime : earliestDateTime;

                        #endregion
                        break;
                    case 7: // Trunk
                        #region Trunk

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
                                    if ((int)r["JobId"] == (int)instruction["JobId"])
                                        orders.Add(new miniOrder((int)r["OrderID"], (int)r["CustomerIdentityID"], r["Customer"].ToString(), r["CustomerOrderNumber"].ToString(), r["DeliveryOrderNumber"].ToString(), (int)r["NoPallets"], (decimal)r["Weight"], r["Surcharges"].ToString()));
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

                            // This is the previous instruction
                            if ((int)r["InstructionOrder"] == ((int)instruction["InstructionOrder"]) - 1 && (int)r["JobId"] == (int)instruction["JobId"])
                            {
                                pickUpPoint = showFullAddress ? r["FullAddress"].ToString() : r["Point"].ToString();
                                trailerRef = r["TrailerRef"].ToString();
                                // use the drivers vehicle if he has one
                                vehicle = driversVehicle;
                                actionDateTime = usePlannedTimes ? Convert.ToDateTime(r["PlannedDepartureDateTime"]) : Convert.ToDateTime(r["OrderDateTime"]);

                                // Add a new row to the output table for this instruction 
                                if (
                                    (r["DriverResourceId"].GetType() == typeof(System.DBNull))
                                    ||
                                    ((int)r["DriverResourceId"] != (int)instruction["DriverResourceId"])
                                    ||
                                    trailerRef != (string)instruction["TrailerRef"]
                                   )
                                {
                                    row = dt.NewRow();

                                    row["DriverName"] = instruction["FullName"];
                                    row["JobId"] = instruction["JobId"];
                                    row["Action"] = "Pick Up Trailer" + Environment.NewLine + pickUpPoint;
                                    row["Point"] = pickUpPoint;
                                    row["Trailer"] = trailerRef;
                                    row["Vehicle"] = vehicle;
                                    row["ActionDateTime"] = actionDateTime;
                                    row["Orders"] = ConvertOrdersToCSV(orders);

                                    // Do not show the order notes for trunk instructions.
                                    row["Notes"] = ""; // GetOrderNotes(instruction);

                                    row["ResourceId"] = resourceID;
                                    row["InstructionOrder"] = instruction["InstructionOrder"];
                                    row["InstructionId"] = instruction["InstructionId"];
                                    row["InstructionTypeID"] = instruction["InstructionTypeID"];
                                    row["IsTimed"] = instruction["IsTimed"];
                                    row["OrderIsAnyTime"] = instruction["OrderIsAnyTime"];
                                    row["Surcharges"] = instruction["Surcharges"];
                                    row["JobOrder"] = instruction["JobOrder"];

                                    dt.Rows.Add(row);
                                    earliestDateTime = earliestDateTime > actionDateTime ? actionDateTime : earliestDateTime;
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

                                row["DriverName"] = instruction["FullName"];
                                row["JobId"] = instruction["JobId"];
                                row["Action"] = "Pick Up Trailer" + Environment.NewLine + pickUpPoint;
                                row["Point"] = pickUpPoint;
                                row["Trailer"] = trailerRef;
                                row["Vehicle"] = vehicle;
                                row["ActionDateTime"] = actionDateTime;
                                row["Orders"] = ConvertOrdersToCSV(orders);

                                // Do not show the order notes for trunk instructions.
                                row["Notes"] = ""; // GetOrderNotes(instruction);

                                row["ResourceId"] = resourceID;
                                row["InstructionOrder"] = instruction["InstructionOrder"];
                                row["InstructionId"] = instruction["InstructionId"];
                                row["InstructionTypeID"] = instruction["InstructionTypeID"];
                                row["IsTimed"] = instruction["IsTimed"];
                                row["OrderIsAnyTime"] = instruction["OrderIsAnyTime"];
                                row["Surcharges"] = instruction["Surcharges"];
                                row["JobOrder"] = instruction["JobOrder"];

                                dt.Rows.Add(row);
                                earliestDateTime = earliestDateTime > actionDateTime ? actionDateTime : earliestDateTime;

                                orders = new List<miniOrder>();
                            }
                        #endregion

                        // Get the Instruction details
                        action = "Trunk to";
                        row = dt.NewRow();

                        row["DriverName"] = instruction["FullName"];
                        row["JobId"] = instruction["JobId"];
                        row["Action"] = action + Environment.NewLine + point;
                        row["Point"] = point;
                        row["ActionDateTime"] = plannedArrivalDateTime;
                        row["Vehicle"] = instruction["RegNo"];
                        row["Trailer"] = instruction["TrailerRef"];
                        row["Orders"] = instruction["Orders"];
                        row["InstructionOrder"] = instruction["InstructionOrder"];
                        row["InstructionId"] = instruction["InstructionId"];
                        row["InstructionTypeID"] = instruction["InstructionTypeID"];
                        row["IsTimed"] = instruction["IsTimed"];
                        row["OrderIsAnyTime"] = instruction["OrderIsAnyTime"];
                        row["Surcharges"] = instruction["Surcharges"];
                        row["JobOrder"] = instruction["JobOrder"];

                        previousTrailer = instruction["TrailerRef"].ToString();

                        // Do not show the order notes for trunk instructions.
                        row["Notes"] = ""; // GetOrderNotes(instruction);
                        row["ResourceId"] = resourceID;
                        earliestDateTime = earliestDateTime > plannedArrivalDateTime ? plannedArrivalDateTime : earliestDateTime;

                        #endregion
                        break;
                }

                if (row != null)
                    dt.Rows.Add(row);
            }

            if (dsInstructions.Tables.Count > 2)
                foreach (DataRow sdr in dsInstructions.Tables[2].Rows)
                    foreach (DataRow dr in dt.Rows)
                        if (Convert.ToInt32(dr["JobId"]) == Convert.ToInt32(sdr["JobID"]) && Convert.ToInt32(dr["ResourceId"]) == resourceID)
                            dr["EarliestDateTime"] = earliestDateTime;

            if (excludeFirstRow)
            {
                // Remove the first row as indicated by the user
                dt.Rows.RemoveAt(0);

                // Remove the 1st row from the summary table
                if (dsInstructions.Tables.Count > 2)
                {
                    int baseInstructionOrder = int.Parse(dsInstructions.Tables[2].Rows[0]["InstructionOrder"].ToString());
                    List<DataRow> removeSummaryRows = dsInstructions.Tables[2].Rows.Cast<DataRow>().Where(dr => (int)dr["InstructionOrder"] == baseInstructionOrder).ToList();

                    foreach (DataRow dr in removeSummaryRows)
                        dsInstructions.Tables[2].Rows.Remove(dr);
                }
            }

            #region Summary Details
            if (dsInstructions.Tables.Count > 3)
            {
                foreach (DataRow drJob in dsInstructions.Tables[3].Rows)
                {
                    int totalOrders = 0, totalPallets = 0;
                    decimal totalWeight = 0m, totalPalletSpaces = 0m;

                    if (dsInstructions.Tables[2].Rows.Cast<DataRow>().Any(dr => (int)dr["JobID"] == (int)drJob["JobID"] && (int)dr["InstructionTypeID"] != (int)eInstructionType.Load))
                    {
                        List<DataRow> drs = dsInstructions.Tables[2].Rows.Cast<DataRow>().Where(dr => (int)dr["JobID"] == (int)drJob["JobID"] && (int)dr["InstructionTypeID"] != (int)eInstructionType.Load).ToList();

                        foreach (DataRow summarydr in drs)
                        {
                            totalOrders++;
                            totalWeight += summarydr.Field<decimal>("Weight");
                            totalPallets += summarydr.Field<int>("NoPallets");
                            totalPalletSpaces += summarydr.Field<decimal>("PalletSpaces");
                        }

                        List<DataRow> mdr = dt.Rows.Cast<DataRow>().Where(dr => (int)dr["JobID"] == (int)drJob["JobID"]).ToList();

                        foreach (DataRow itemdr in mdr)
                        {
                            itemdr["TotalOrders"] = totalOrders;
                            itemdr["TotalWeight"] = totalWeight;
                            itemdr["TotalPallets"] = totalPallets;
                            itemdr["TotalPalletSpaces"] = totalPalletSpaces;
                        }
                    }
                }
            }
            #endregion

            finalData = dt.Clone();
            List<DataRow> rows = null;
            if (useInstructionOrder)
                rows = (from row in dt.AsEnumerable()
                        orderby
                        row.Field<int>("JobOrder") ascending,
                        int.Parse(row.Field<string>("InstructionOrder")) ascending
                        select row).ToList();

            else
                rows = (from row in dt.Rows.Cast<DataRow>()
                        orderby
                        row.Field<int>("JobOrder") ascending,
                        (DateTime)row["ActionDateTime"] ascending
                        select row).ToList();

            foreach (var dr in rows)
                finalData.ImportRow(dr);

            return finalData;
        }

        // Resource Manifest for Sub contractors.
        public static DataTable GetSubbyManifest(int resourceManifestID, int? subContractorID, bool usePlannedTimes, bool excludeFirstRow, bool showFullAddress, bool useInstructionOrder)
        {
            Facade.ResourceManifest facResourceManifest = new Orchestrator.Facade.ResourceManifest();

            DataTable finalData = new DataTable();
            DataTable dt = new DataTable("Table");
            dt.Columns.Add(new DataColumn("OrganisationName", typeof(string)));
            dt.Columns.Add(new DataColumn("JobId", typeof(int)));
            dt.Columns.Add(new DataColumn("Action", typeof(string)));
            dt.Columns.Add(new DataColumn("Point", typeof(string)));
            dt.Columns.Add(new DataColumn("ActionDateTime", typeof(DateTime)));
            dt.Columns.Add(new DataColumn("Vehicle", typeof(string)));
            dt.Columns.Add(new DataColumn("Trailer", typeof(string)));
            dt.Columns.Add(new DataColumn("Notes", typeof(string)));
            dt.Columns.Add(new DataColumn("Orders", typeof(string)));
            dt.Columns.Add(new DataColumn("InstructionOrder", typeof(string)));
            dt.Columns.Add(new DataColumn("EarliestDateTime", typeof(DateTime)));
            dt.Columns.Add(new DataColumn("SubContractorIdentityID", typeof(int)));
            dt.Columns.Add(new DataColumn("InstructionTypeID", typeof(int)));
            dt.Columns.Add(new DataColumn("InstructionId", typeof(int)));
            dt.Columns.Add(new DataColumn("TotalOrders", typeof(int)));
            dt.Columns.Add(new DataColumn("TotalWeight", typeof(decimal)));
            dt.Columns.Add(new DataColumn("TotalPallets", typeof(int)));
            dt.Columns.Add(new DataColumn("TotalPalletSpaces", typeof(decimal)));
            dt.Columns.Add(new DataColumn("IsTimed", typeof(bool)));
            dt.Columns.Add(new DataColumn("OrderIsAnyTime", typeof(bool)));
            dt.Columns.Add(new DataColumn("Surcharges", typeof(string)));
            dt.Columns.Add(new DataColumn("JobOrder", typeof(int)));

            DataSet dsInstructions = facResourceManifest.GetResourceManifestJobForReport(resourceManifestID);

            string action = string.Empty;
            string previousTrailer = string.Empty;
            List<miniOrder> orders = new List<miniOrder>();
            DateTime earliestDateTime = DateTime.MaxValue;

            string pickUpPoint = string.Empty;
            string trailerRef = string.Empty;
            DateTime actionDateTime = DateTime.MinValue;
            string vehicle = string.Empty;
            int lastInstructiontypeID = -1;
            foreach (DataRow instruction in dsInstructions.Tables[0].Rows)
            {
                DataRow row = null;
                string driversVehicle = (instruction["RegNo"] == DBNull.Value) ? String.Empty : instruction["RegNo"].ToString();
                DateTime plannedArrivalDateTime = usePlannedTimes ? Convert.ToDateTime(instruction["PlannedArrivalDateTime"]) : Convert.ToDateTime(instruction["OrderDateTime"]);

                string point = showFullAddress ? instruction["FullAddress"].ToString() : instruction["Point"].ToString();

                switch ((int)instruction["InstructionTypeID"])
                {
                    case 1: // this is a collection so all is well
                        #region Load

                        action = "Collect From";
                        row = dt.NewRow();

                        row["OrganisationName"] = instruction["OrganisationName"];
                        row["JobId"] = instruction["JobId"];
                        row["Action"] = action + Environment.NewLine + point;
                        row["Point"] = point;
                        row["ActionDateTime"] = plannedArrivalDateTime;
                        row["Vehicle"] = instruction["RegNo"];
                        row["Trailer"] = instruction["TrailerRef"];
                        row["Orders"] = instruction["Orders"];
                        row["Notes"] = GetOrderNotes(instruction);
                        row["SubcontractorIdentityID"] = subContractorID;
                        row["InstructionOrder"] = instruction["InstructionOrder"];
                        row["InstructionId"] = instruction["InstructionId"];
                        row["InstructionTypeID"] = instruction["InstructionTypeID"];
                        row["IsTimed"] = instruction["IsTimed"];
                        row["OrderIsAnyTime"] = instruction["OrderIsAnyTime"];
                        row["Surcharges"] = instruction["Surcharges"];
                        row["JobOrder"] = instruction["JobOrder"];

                        previousTrailer = instruction["TrailerRef"].ToString();
                        earliestDateTime = earliestDateTime > plannedArrivalDateTime ? plannedArrivalDateTime : earliestDateTime;

                        #endregion
                        break;
                    case 2: // Drop
                        #region Drop

                        #region We have to pick up the trailer from the trailers last point before the current instruction order.
                        foreach (DataRow r in dsInstructions.Tables[1].Rows)
                        {
                            // Loop through the previous instructions on this job to determine what is on trailer
                            if ((int)r["InstructionOrder"] < (int)instruction["InstructionOrder"])
                            {
                                if (r["InstructionTypeID"].ToString() == "1")
                                {
                                    // Add these orders to the Orders collection
                                    if ((int)r["JobId"] == (int)instruction["JobId"])
                                        orders.Add(new miniOrder((int)r["OrderID"], (int)r["CustomerIdentityID"], r["Customer"].ToString(), r["CustomerOrderNumber"].ToString(), r["DeliveryOrderNumber"].ToString(), (int)r["NoPallets"], (decimal)r["Weight"], r["Surcharges"].ToString()));
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

                            // This is the previous instruction
                            if ((int)r["InstructionOrder"] == ((int)instruction["InstructionOrder"]) - 1 && (int)r["JobId"] == (int)instruction["JobId"])
                            {
                                pickUpPoint = showFullAddress ? r["FullAddress"].ToString() : r["Point"].ToString();
                                trailerRef = r["TrailerRef"].ToString();
                                // use the drivers vehicle if he has one
                                vehicle = driversVehicle;
                                actionDateTime = usePlannedTimes ? Convert.ToDateTime(r["PlannedDepartureDateTime"]) : Convert.ToDateTime(r["OrderDateTime"]);

                                // only add the pick up if the current driver did do the previous instruction
                                // or the trailer is different
                                if ((r["SubcontractorIdentityID"].GetType() == typeof(System.DBNull))
                                    || ((int)r["SubcontractorIdentityID"] != (int)instruction["SubcontractorIdentityID"])
                                    || trailerRef != (string)instruction["TrailerRef"]
                                    )
                                {
                                    // Add a new row to the output table for this instruction
                                    row = dt.NewRow();

                                    row["OrganisationName"] = instruction["OrganisationName"];
                                    row["JobId"] = instruction["JobId"];
                                    row["Action"] = "Pick Up Trailer" + Environment.NewLine + pickUpPoint;
                                    row["Point"] = pickUpPoint;
                                    row["Trailer"] = trailerRef;
                                    row["Vehicle"] = vehicle;
                                    row["ActionDateTime"] = actionDateTime;
                                    row["Orders"] = ConvertOrdersToCSV(orders);
                                    row["Notes"] = GetOrderNotes(instruction);
                                    row["SubcontractorIdentityID"] = subContractorID;
                                    row["InstructionTypeID"] = instruction["InstructionTypeID"];
                                    row["InstructionId"] = instruction["InstructionId"];
                                    row["InstructionOrder"] = instruction["InstructionOrder"];
                                    row["IsTimed"] = instruction["IsTimed"];
                                    row["OrderIsAnyTime"] = instruction["OrderIsAnyTime"];
                                    row["Surcharges"] = instruction["Surcharges"];
                                    row["JobOrder"] = instruction["JobOrder"];

                                    dt.Rows.Add(row);
                                    earliestDateTime = earliestDateTime > actionDateTime ? actionDateTime : earliestDateTime;
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

                                row["OrganisationName"] = instruction["OrganisationName"];
                                row["JobId"] = instruction["JobId"];
                                row["Action"] = "Pick Up Trailer" + Environment.NewLine + pickUpPoint;
                                row["Point"] = pickUpPoint;
                                row["Trailer"] = trailerRef;
                                row["Vehicle"] = vehicle;
                                row["ActionDateTime"] = actionDateTime;
                                row["Orders"] = ConvertOrdersToCSV(orders);
                                row["Notes"] = GetOrderNotes(instruction);
                                row["SubcontractorIdentityID"] = subContractorID;
                                row["InstructionOrder"] = instruction["SubcontractorIdentityID"];
                                row["InstructionTypeID"] = instruction["InstructionTypeID"];
                                row["InstructionId"] = instruction["InstructionId"];
                                row["IsTimed"] = instruction["IsTimed"];
                                row["OrderIsAnyTime"] = instruction["OrderIsAnyTime"];
                                row["Surcharges"] = instruction["Surcharges"];
                                row["JobOrder"] = instruction["JobOrder"];

                                dt.Rows.Add(row);
                                earliestDateTime = earliestDateTime > actionDateTime ? actionDateTime : earliestDateTime;
                                orders = new List<miniOrder>();
                            }

                        #endregion

                        // Get the Instruction details
                        action = "Deliver to";
                        row = dt.NewRow();

                        row["OrganisationName"] = instruction["OrganisationName"];
                        row["JobId"] = instruction["JobId"];
                        row["Action"] = action + Environment.NewLine + point;
                        row["Point"] = point;
                        row["ActionDateTime"] = plannedArrivalDateTime;
                        row["Vehicle"] = instruction["RegNo"];
                        row["Trailer"] = instruction["TrailerRef"];
                        row["Orders"] = instruction["Orders"];
                        row["Notes"] = GetOrderNotes(instruction);
                        row["SubcontractorIdentityID"] = subContractorID;
                        row["InstructionOrder"] = instruction["InstructionOrder"];
                        row["InstructionTypeID"] = instruction["InstructionTypeID"];
                        row["InstructionId"] = instruction["InstructionId"];
                        row["IsTimed"] = instruction["IsTimed"];
                        row["OrderIsAnyTime"] = instruction["OrderIsAnyTime"];
                        row["Surcharges"] = instruction["Surcharges"];
                        row["JobOrder"] = instruction["JobOrder"];

                        previousTrailer = instruction["TrailerRef"].ToString();
                        earliestDateTime = earliestDateTime > plannedArrivalDateTime ? plannedArrivalDateTime : earliestDateTime;

                        #endregion
                        break;
                    case 7: // Trunk
                        #region Trunk

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
                                    if ((int)r["JobId"] == (int)instruction["JobId"])
                                        orders.Add(new miniOrder((int)r["OrderID"], (int)r["CustomerIdentityID"], r["Customer"].ToString(), r["CustomerOrderNumber"].ToString(), r["DeliveryOrderNumber"].ToString(), (int)r["NoPallets"], (decimal)r["Weight"], r["Surcharges"].ToString()));
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

                            // This is the previous instruction
                            if ((int)r["InstructionOrder"] == ((int)instruction["InstructionOrder"]) - 1 && (int)r["JobId"] == (int)instruction["JobId"])
                            {
                                pickUpPoint = showFullAddress ? r["FullAddress"].ToString() : r["Point"].ToString();
                                trailerRef = r["TrailerRef"].ToString();
                                // use the drivers vehicle if he has one
                                vehicle = driversVehicle;
                                actionDateTime = usePlannedTimes ? Convert.ToDateTime(r["PlannedDepartureDateTime"]) : Convert.ToDateTime(r["OrderDateTime"]);

                                // Add a new row to the output table for this instruction 
                                if (
                                    (r["SubcontractorIdentityID"].GetType() == typeof(System.DBNull))
                                    ||
                                    ((int)r["SubcontractorIdentityID"] != (int)instruction["SubcontractorIdentityID"])
                                    ||
                                    trailerRef != (string)instruction["TrailerRef"]
                                   )
                                {
                                    row = dt.NewRow();

                                    row["OrganisationName"] = instruction["OrganisationName"];
                                    row["JobId"] = instruction["JobId"];
                                    row["Action"] = "Pick Up Trailer" + Environment.NewLine + pickUpPoint;
                                    row["Point"] = pickUpPoint;
                                    row["Trailer"] = trailerRef;
                                    row["Vehicle"] = vehicle;
                                    row["ActionDateTime"] = actionDateTime;
                                    row["Orders"] = ConvertOrdersToCSV(orders);

                                    // Do not show order notes for trunk instructions.
                                    row["Notes"] = ""; // GetOrderNotes(instruction);

                                    row["SubcontractorIdentityID"] = subContractorID;
                                    row["InstructionOrder"] = instruction["InstructionOrder"];
                                    row["InstructionTypeID"] = instruction["InstructionTypeID"];
                                    row["InstructionId"] = instruction["InstructionId"];
                                    row["IsTimed"] = instruction["IsTimed"];
                                    row["OrderIsAnyTime"] = instruction["OrderIsAnyTime"];
                                    row["Surcharges"] = instruction["Surcharges"];
                                    row["JobOrder"] = instruction["JobOrder"];

                                    dt.Rows.Add(row);
                                    earliestDateTime = earliestDateTime > actionDateTime ? actionDateTime : earliestDateTime;
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

                                row["OrganisationName"] = instruction["OrganisationName"];
                                row["JobId"] = instruction["JobId"];
                                row["Action"] = "Pick Up Trailer" + Environment.NewLine + pickUpPoint;
                                row["Point"] = pickUpPoint;
                                row["Trailer"] = trailerRef;
                                row["Vehicle"] = vehicle;
                                row["ActionDateTime"] = actionDateTime;
                                row["Orders"] = ConvertOrdersToCSV(orders);

                                // Do not show order notes for trunk instructions.
                                row["Notes"] = ""; // GetOrderNotes(instruction);

                                row["SubcontractorIdentityID"] = subContractorID;
                                row["InstructionOrder"] = instruction["InstructionOrder"];
                                row["InstructionTypeID"] = instruction["InstructionTypeID"];
                                row["InstructionId"] = instruction["InstructionId"];
                                row["IsTimed"] = instruction["IsTimed"];
                                row["OrderIsAnyTime"] = instruction["OrderIsAnyTime"];
                                row["Surcharges"] = instruction["Surcharges"];
                                row["JobOrder"] = instruction["JobOrder"];

                                dt.Rows.Add(row);
                                earliestDateTime = earliestDateTime > actionDateTime ? actionDateTime : earliestDateTime;

                                orders = new List<miniOrder>();
                            }
                        #endregion

                        // Get the Instruction details
                        action = "Trunk to";
                        row = dt.NewRow();

                        row["OrganisationName"] = instruction["OrganisationName"];
                        row["JobId"] = instruction["JobId"];
                        row["Action"] = action + Environment.NewLine + point;
                        row["Point"] = point;
                        row["ActionDateTime"] = plannedArrivalDateTime;
                        row["Vehicle"] = instruction["RegNo"];
                        row["Trailer"] = instruction["TrailerRef"];
                        row["Orders"] = instruction["Orders"];
                        row["InstructionOrder"] = instruction["InstructionOrder"];
                        row["InstructionTypeID"] = instruction["InstructionTypeID"];
                        row["InstructionId"] = instruction["InstructionId"];
                        row["IsTimed"] = instruction["IsTimed"];
                        row["OrderIsAnyTime"] = instruction["OrderIsAnyTime"];
                        row["Surcharges"] = instruction["Surcharges"];
                        row["JobOrder"] = instruction["JobOrder"];

                        previousTrailer = instruction["TrailerRef"].ToString();

                        // Do not show order notes for trunk instructions.
                        row["Notes"] = ""; // GetOrderNotes(instruction);

                        row["SubcontractorIdentityID"] = subContractorID;
                        earliestDateTime = earliestDateTime > plannedArrivalDateTime ? plannedArrivalDateTime : earliestDateTime;

                        #endregion
                        break;
                }

                if (row != null)
                    dt.Rows.Add(row);
            }

            if (dsInstructions.Tables.Count > 2)
                foreach (DataRow sdr in dsInstructions.Tables[2].Rows)
                    foreach (DataRow dr in dt.Rows)
                        if (Convert.ToInt32(dr["JobId"]) == Convert.ToInt32(sdr["JobID"]) && Convert.ToInt32(dr["SubcontractorIdentityID"]) == subContractorID)
                            dr["EarliestDateTime"] = earliestDateTime;


            if (excludeFirstRow)
            {
                // Remove the first row as indicated by the user
                dt.Rows.RemoveAt(0);

                // Remove the 1st row from the summary table
                if (dsInstructions.Tables.Count > 2)
                {
                    int baseInstructionOrder = int.Parse(dsInstructions.Tables[2].Rows[0]["InstructionOrder"].ToString());
                    List<DataRow> removeSummaryRows = dsInstructions.Tables[2].Rows.Cast<DataRow>().Where(dr => (int)dr["InstructionOrder"] == baseInstructionOrder).ToList();

                    foreach (DataRow dr in removeSummaryRows)
                        dsInstructions.Tables[2].Rows.Remove(dr);
                }
            }

            #region Summary Details
            if (dsInstructions.Tables.Count > 3)
            {
                foreach (DataRow drJob in dsInstructions.Tables[3].Rows)
                {
                    int totalOrders = 0, totalPallets = 0;
                    decimal totalWeight = 0m, totalPalletSpaces = 0m;

                    if (dsInstructions.Tables[2].Rows.Cast<DataRow>().Any(dr => (int)dr["JobID"] == (int)drJob["JobID"] && (int)dr["InstructionTypeID"] != (int)eInstructionType.Load))
                    {
                        List<DataRow> drs = dsInstructions.Tables[2].Rows.Cast<DataRow>().Where(dr => (int)dr["JobID"] == (int)drJob["JobID"] && (int)dr["InstructionTypeID"] != (int)eInstructionType.Load).ToList();

                        foreach (DataRow summarydr in drs)
                        {
                            totalOrders++;
                            totalWeight += summarydr.Field<decimal>("Weight");
                            totalPallets += summarydr.Field<int>("NoPallets");
                            totalPalletSpaces += summarydr.Field<decimal>("PalletSpaces");
                        }

                        List<DataRow> mdr = dt.Rows.Cast<DataRow>().Where(dr => (int)dr["JobID"] == (int)drJob["JobID"]).ToList();

                        foreach (DataRow itemdr in mdr)
                        {
                            itemdr["TotalOrders"] = totalOrders;
                            itemdr["TotalWeight"] = totalWeight;
                            itemdr["TotalPallets"] = totalPallets;
                            itemdr["TotalPalletSpaces"] = totalPalletSpaces;
                        }
                    }
                }
            }
            #endregion

            finalData = dt.Clone();
            List<DataRow> rows = null;

            //-----------------
            if (useInstructionOrder)
                rows = (from row in dt.AsEnumerable()
                        orderby
                        row.Field<int>("JobOrder") ascending,
                        int.Parse(row.Field<string>("InstructionOrder")) ascending
                        select row).ToList();

            else
                rows = (from row in dt.Rows.Cast<DataRow>()
                        orderby
                        row.Field<int>("JobOrder") ascending,
                        (DateTime)row["ActionDateTime"] ascending
                        select row).ToList();

            foreach (var dr in rows)
                finalData.ImportRow(dr);

            return finalData;

        }

        public static DataTable GetClientManifestReport(int customerIdentityID, DateTime dateFrom, DateTime dateTo, bool usePlannedTimes, bool excludeFirstRow, bool showFullAddress, bool useInstructionOrder)
        {
            var facResourceManifest = new Orchestrator.Facade.ResourceManifest();
            var dsInstructions = facResourceManifest.GetDataForClientManifestReport(customerIdentityID, dateFrom, dateTo);

            return GetManifest(dsInstructions, 0, usePlannedTimes, excludeFirstRow, showFullAddress, useInstructionOrder);
        }

        #region Private Members

        private struct miniOrder
        {
            public int OrderID;
            public int CustomerIdentityID;
            public string Customer;
            public string CustomerOrderNumber;
            public string DeliveryOrderNumber;
            public int NoPallets;
            public decimal Weight;
            public string Surcharges;

            public miniOrder(int orderID, int customerIdentityID, string customer, string customerOrderNumber, string deliveryOrderNumber, int noPallets, decimal weight, string surcharges)
            {
                OrderID = orderID;
                Customer = customer;
                CustomerIdentityID = customerIdentityID;
                CustomerOrderNumber = customerOrderNumber;
                DeliveryOrderNumber = deliveryOrderNumber;
                NoPallets = noPallets;
                Weight = weight;
                Surcharges = surcharges;
            }
        }

        #endregion

        #region private functions

        private static string ConvertOrdersToCSV(List<miniOrder> orders)
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
                retVal.Append("|");
                retVal.Append(" |"); // In place of the Reg No
            }

            return retVal.ToString();
        }

        private static string GetOrderNotes(DataRow row)
        {
            string orderNotes = string.Empty;

            string[] Orders;
            string[] OrderDetails;
            Orders = row["Orders"].ToString().Split(new string[] { "^^" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string order in Orders)
            {
                OrderDetails = order.Split('|');

                if (OrderDetails.Length > 1)
                {
                    if (OrderDetails[7].Length > 0)
                        orderNotes += string.Format("[{0}]{1}\n", OrderDetails[0], OrderDetails[7]);
                }
            }

            return orderNotes;
        }

        #endregion
    }
}
