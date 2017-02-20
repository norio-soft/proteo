using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace Orchestrator.WebUI
{
    public class ReportExport
    {
        #region public methods

        public ReportExport() { }

        //public DataTable RetieveExportDataForReport(Telerik.Reporting.ReportSource currentReport, string reportName)
        //{
        //    return GetData(currentReport, reportName);
        //}

        public DataTable RetieveExportDataForReport(Telerik.Reporting.InstanceReportSource reportInstance, string reportName)
        {
            return GetData(reportInstance, reportName);
        }
        #endregion

        #region private methods

        private DataTable GetData(Telerik.Reporting.InstanceReportSource currentReport, string reportName)
        {
            DataSet ds = null;
            DataTable dt = null;

            #region Report Selection

            switch (reportName)
            {
                case "Location.DriverLogon":
                    ds = DriverLogon(currentReport);
                    break;
                case "Location.LocationEvent":
                    ds = LocationEvent(currentReport);
                    break;
                case "Location.LocationIdling":
                    ds = LocationIdling(currentReport);
                    break;
                case "Location.LocationVehicleTrip":
                    ds = LocationVehicleTrip(currentReport);
                    break;
                case "Location.LocationSummary":
                    ds = LocationSummary(currentReport);
                    break;


                case "CAN.DriverGrading2":
                    ds = DriverGrading2(currentReport);
                    break;
                //case "CAN.DriverGradingDetail":
                //    ds = DriverGradingDetail(currentReport);
                //    break;
                //case "CAN.DriverOverview":
                    ds = DriverOverview(currentReport);
                    break;
                case "CAN.DriverFuelConsumption2":
                    ds = DriverFuelConsumption2(currentReport);
                    break;
                case "CAN.DriverIdling2":
                    ds = DriverIdling2(currentReport);
                    break;
                case "CAN.VehicleFuelConsumption2":
                    ds = VehicleFuelConsumption2(currentReport);
                    break;
                case "CAN.VehicleIdling2":
                    ds = VehicleIdling2(currentReport);
                    break;
                case "CAN.DriverIdlingTime":
                    ds = DriverIdlingTime(currentReport);
                    break;
                case "CAN.VehicleIdlingTime":
                    ds = VehicleIdlingTime(currentReport);
                    break;

                case "GeoFence.VisitsByVehicle":
                    ds = VisitsByVehicle(currentReport);
                    break;

                case "BinRound.RoundSummary":
                    ds = RoundSummary(currentReport);
                    break;
                case "BinRound.RoundArrangements":
                    ds = RoundArrangements(currentReport);
                    break;
                case "BinRound.RoundUncompletedArrangements":
                    ds = RoundUncompletedArrangements(currentReport);
                    break;
                case "BinRound.RoundIssues":
                    ds = RoundIssues(currentReport);
                    break;
                case "BinRound.RoundNonPresents":
                    ds = RoundNonPresents(currentReport);
                    break;
                case "BinRound.RoundSequence":
                    ds = RoundSequence(currentReport);
                    break;
                case "BinRound.RoundOverview":
                    ds = RoundOverview(currentReport);
                    break;
                case "FleetMetrik.DriverGradingByWeek":
                    ds = FMDriverGrading(currentReport, true);
                    break;
                case "FleetMetrik.DriverGradingByMonth":
                    ds = FMDriverGrading(currentReport, false);
                    break;
                case "FleetMetrik.OverRevvingTrendByWeek":
                case "FleetMetrik.OverRevvingTrendByMonth":
                case "FleetMetrik.HarshBrakingTrendByWeek":
                case "FleetMetrik.HarshBrakingTrendByMonth":
                case "FleetMetrik.IdlingTrendByWeek":
                case "FleetMetrik.IdlingTrendByMonth":
                case "FleetMetrik.SpeedingTrendByWeek":
                case "FleetMetrik.SpeedingTrendByMonth":
                case "CAN.DriverGradingDetail":
                    ds = FMTrendReport(currentReport, reportName);
                    break;
                default:
                    throw new NotImplementedException("Retrieving data for this extract type is not yet implemented.");
            }

            #endregion

            if (ds.Tables.Count > 0)
                dt = ds.Tables[0];

            return dt;
        }

        #region Location Functions

        private DataSet DriverLogon(Telerik.Reporting.ReportSource currentReport)
        {
            DataAccess.IGPS dacGPS = new DataAccess.GPS();

            DateTime fromDate = Convert.ToDateTime(currentReport.Parameters["FromDate"].Value);
            DateTime toDate = Convert.ToDateTime(currentReport.Parameters["ToDate"].Value);
            string driverResourceId = currentReport.Parameters["DriverId"].Value == null ? string.Empty : currentReport.Parameters["DriverId"].Value.ToString();
            string gpsReasonIds = "8,9";

            return dacGPS.GPSGetDriverHistoryForEventsCSVExport(driverResourceId, gpsReasonIds, fromDate, toDate);
        }

        private DataSet LocationEvent(Telerik.Reporting.ReportSource currentReport)
        {
            DataAccess.IGPS dacGPS = new DataAccess.GPS();

            DateTime fromDate = Convert.ToDateTime(currentReport.Parameters["FromDate"].Value);
            DateTime toDate = Convert.ToDateTime(currentReport.Parameters["ToDate"].Value);
            object[] vehicleIds = currentReport.Parameters["VehicleID"].Value as object[];
            object[] gpsEventIds = currentReport.Parameters["GPSEventIDs"].Value as object[];

            return dacGPS.GPSGetVehicleHistoryForEventsCSVExport(vehicleIds, gpsEventIds, fromDate, toDate);
        }

        private DataSet LocationIdling(Telerik.Reporting.ReportSource currentReport)
        {
            DataAccess.IGPS dacGPS = new DataAccess.GPS();

            DateTime fromDate = Convert.ToDateTime(currentReport.Parameters["FromDate"].Value);
            DateTime toDate = Convert.ToDateTime(currentReport.Parameters["ToDate"].Value);
            object[] vehicleIds = currentReport.Parameters["VehicleResourceIds"].Value as object[];

            return dacGPS.GPSGetLocationIdlingForVehicleCSVExport(vehicleIds, fromDate, toDate);
        }

        private DataSet LocationVehicleTrip(Telerik.Reporting.ReportSource currentReport)
        {
            DataAccess.IGPS dacGPS = new DataAccess.GPS();

            DateTime fromDate = Convert.ToDateTime(currentReport.Parameters["FromDate"].Value);
            DateTime toDate = Convert.ToDateTime(currentReport.Parameters["ToDate"].Value);
            object[] vehicleIds = currentReport.Parameters["VehicleIds"].Value as object[];

            return dacGPS.GPSGetLocationForVehicleTripCSVExport(vehicleIds, fromDate, toDate);
        }

        private DataSet LocationSummary(Telerik.Reporting.ReportSource currentReport)
        {
            DataAccess.IGPS dacGPS = new DataAccess.GPS();

            DateTime fromDate = Convert.ToDateTime(currentReport.Parameters["DateFrom"].Value);
            DateTime toDate = Convert.ToDateTime(currentReport.Parameters["DateTo"].Value);
            object[] vehicleIds = currentReport.Parameters["VehicleIds"].Value as object[];

            return dacGPS.GPSGetLocationSummary(vehicleIds, fromDate, toDate);
        }

        #endregion

        #region GeoFence Functions

        private DataSet VisitsByVehicle(Telerik.Reporting.ReportSource currentReport)
        {
            DataAccess.IGPS dacGPS = new DataAccess.GPS();

            DateTime fromDate = Convert.ToDateTime(currentReport.Parameters ["DateFrom"].Value);
            DateTime toDate = Convert.ToDateTime(currentReport.Parameters ["DateTo"].Value);
            object[] vehicleIds = currentReport.Parameters ["VehicleIds"].Value as object[];

            return dacGPS.GPSGetGeofenceVisitForVehicles(vehicleIds, fromDate, toDate);
        }

        #endregion

        #region CAN functions

        private DataSet DriverGrading2(Telerik.Reporting.ReportSource currentReport)
        {
            DataAccess.IGPS dacGPS = new DataAccess.GPS();

            DateTime fromDate = Convert.ToDateTime(currentReport.Parameters ["FromDate"].Value);
            DateTime toDate = Convert.ToDateTime(currentReport.Parameters ["ToDate"].Value);
            long orgUnitId = Convert.ToInt64(currentReport.Parameters ["OrgUnitId"].Value);

            return dacGPS.CANGetDriverGrading(orgUnitId, fromDate, toDate);
        }

        private DataSet DriverGradingDetail(Telerik.Reporting.Report currentReport)
        {
            DataAccess.IGPS dacGPS = new DataAccess.GPS();

            DateTime fromDate = Convert.ToDateTime(currentReport.ReportParameters["StartDate"].Value);
            DateTime toDate = Convert.ToDateTime(currentReport.ReportParameters["EndDate"].Value);
            Int32 orgUnitId = Convert.ToInt32(currentReport.ReportParameters["OrgUnitID"].Value);

            return dacGPS.CANGetDriverGradingDetail(orgUnitId, fromDate, toDate);
        }

        private DataSet DriverOverview(Telerik.Reporting.ReportSource currentReport)
        {
            DataAccess.IGPS dacGPS = new DataAccess.GPS();

            DateTime fromDate = Convert.ToDateTime(currentReport.Parameters ["FromDate"].Value);
            DateTime toDate = Convert.ToDateTime(currentReport.Parameters ["ToDate"].Value);
            Int32 orgUnitId = Convert.ToInt32(currentReport.Parameters ["DriverId"].Value);

            return dacGPS.CANGetDriverOverview(orgUnitId, fromDate, toDate);
        }

        private DataSet DriverFuelConsumption2(Telerik.Reporting.ReportSource currentReport)
        {
            DataAccess.IGPS dacGPS = new DataAccess.GPS();

            DateTime fromDate = Convert.ToDateTime(currentReport.Parameters ["FromDate"].Value);
            DateTime toDate = Convert.ToDateTime(currentReport.Parameters ["ToDate"].Value);
            long orgUnitId = Convert.ToInt64(currentReport.Parameters ["OrgUnitId"].Value);

            return dacGPS.CANGetDriverFuelConsumption(orgUnitId, fromDate, toDate);
        }

        private DataSet DriverIdling2(Telerik.Reporting.ReportSource currentReport)
        {
            DataAccess.IGPS dacGPS = new DataAccess.GPS();

            DateTime fromDate = Convert.ToDateTime(currentReport.Parameters ["FromDate"].Value);
            DateTime toDate = Convert.ToDateTime(currentReport.Parameters ["ToDate"].Value);
            long orgUnitId = Convert.ToInt64(currentReport.Parameters ["OrgUnitId"].Value);

            return dacGPS.CANGetDriverIdling(orgUnitId, fromDate, toDate);
        }

        private DataSet VehicleFuelConsumption2(Telerik.Reporting.ReportSource currentReport)
        {
            DataAccess.IGPS dacGPS = new DataAccess.GPS();

            DateTime fromDate = Convert.ToDateTime(currentReport.Parameters ["FromDate"].Value);
            DateTime toDate = Convert.ToDateTime(currentReport.Parameters ["ToDate"].Value);
            long orgUnitId = Convert.ToInt64(currentReport.Parameters ["OrgUnitId"].Value);

            return dacGPS.CANGetVehicleFuelConsumption(orgUnitId, fromDate, toDate);
        }

        private DataSet VehicleIdling2(Telerik.Reporting.ReportSource currentReport)
        {
            DataAccess.IGPS dacGPS = new DataAccess.GPS();

            DateTime fromDate = Convert.ToDateTime(currentReport.Parameters ["FromDate"].Value);
            DateTime toDate = Convert.ToDateTime(currentReport.Parameters ["ToDate"].Value);
            long orgUnitId = Convert.ToInt64(currentReport.Parameters ["OrgUnitId"].Value);

            return dacGPS.CANGetVehicleIdling(orgUnitId, fromDate, toDate);
        }

        private DataSet DriverIdlingTime(Telerik.Reporting.ReportSource currentReport)
        {
            DataAccess.IGPS dacGPS = new DataAccess.GPS();

            DateTime fromDate = Convert.ToDateTime(currentReport.Parameters ["FromDate"].Value);
            DateTime toDate = Convert.ToDateTime(currentReport.Parameters ["ToDate"].Value);
            long orgUnitId = Convert.ToInt64(currentReport.Parameters ["OrgUnitId"].Value);

            return dacGPS.CANGetDriverIdlingTime(orgUnitId, fromDate, toDate);
        }

        private DataSet VehicleIdlingTime(Telerik.Reporting.ReportSource currentReport)
        {
            DataAccess.IGPS dacGPS = new DataAccess.GPS();

            DateTime fromDate = Convert.ToDateTime(currentReport.Parameters ["FromDate"].Value);
            DateTime toDate = Convert.ToDateTime(currentReport.Parameters ["ToDate"].Value);
            long orgUnitId = Convert.ToInt64(currentReport.Parameters ["OrgUnitId"].Value);

            return dacGPS.CANGetVehicleIdlingTime(orgUnitId, fromDate, toDate);
        }

        #endregion

        #region BinRound Functions

        private DataSet RoundSummary(Telerik.Reporting.ReportSource currentReport)
        {
            DataAccess.IGPS dacGPS = new DataAccess.GPS();

            DateTime fromDate = Convert.ToDateTime(currentReport.Parameters ["FromDate"].Value);
            DateTime toDate = Convert.ToDateTime(currentReport.Parameters ["ToDate"].Value);

            return dacGPS.BR_GetRoundSummarysForDateRandAndCustomer(fromDate, toDate);
        }

        private DataSet RoundArrangements(Telerik.Reporting.ReportSource currentReport)
        {
            DataAccess.IGPS dacGPS = new DataAccess.GPS();

            DateTime fromDate = Convert.ToDateTime(currentReport.Parameters ["FromDate"].Value);
            DateTime toDate = Convert.ToDateTime(currentReport.Parameters ["ToDate"].Value);

            return dacGPS.BR_GetRoundArrangementsForDateRange(fromDate, toDate);
        }

        private DataSet RoundUncompletedArrangements(Telerik.Reporting.ReportSource currentReport)
        {
            DataAccess.IGPS dacGPS = new DataAccess.GPS();

            DateTime fromDate = Convert.ToDateTime(currentReport.Parameters ["FromDate"].Value);
            DateTime toDate = Convert.ToDateTime(currentReport.Parameters ["ToDate"].Value);

            return dacGPS.BR_GetRoundArrangementsUncompletedForDateRange(fromDate, toDate);
        }

        private DataSet RoundIssues(Telerik.Reporting.ReportSource currentReport)
        {
            DataAccess.IGPS dacGPS = new DataAccess.GPS();

            DateTime fromDate = Convert.ToDateTime(currentReport.Parameters ["FromDate"].Value);
            DateTime toDate = Convert.ToDateTime(currentReport.Parameters ["ToDate"].Value);

            return dacGPS.BR_GetRoundIssuesForDateRange(fromDate, toDate);
        }

        private DataSet RoundNonPresents(Telerik.Reporting.ReportSource currentReport)
        {
            DataAccess.IGPS dacGPS = new DataAccess.GPS();

            DateTime fromDate = Convert.ToDateTime(currentReport.Parameters ["FromDate"].Value);
            DateTime toDate = Convert.ToDateTime(currentReport.Parameters ["ToDate"].Value);

            return dacGPS.BR_GetRoundNonPresentsForDateRange(fromDate, toDate);
        }

        private DataSet RoundSequence(Telerik.Reporting.ReportSource currentReport)
        {
            DataAccess.IGPS dacGPS = new DataAccess.GPS();

            DateTime fromDate = Convert.ToDateTime(currentReport.Parameters ["FromDate"].Value);
            DateTime toDate = Convert.ToDateTime(currentReport.Parameters ["ToDate"].Value);

            return dacGPS.BR_GetRoundLegsInSequenceForDateRange(fromDate, toDate);
        }

        private DataSet RoundOverview(Telerik.Reporting.ReportSource currentReport)
        {
            DataAccess.IGPS dacGPS = new DataAccess.GPS();

            DateTime fromDate = Convert.ToDateTime(currentReport.Parameters ["DateFrom"].Value);
            DateTime toDate = Convert.ToDateTime(currentReport.Parameters ["DateTo"].Value);
            object[] driverIds = currentReport.Parameters ["Driver"].Value as object[];
            object[] vehicleIds = currentReport.Parameters ["Vehicle"].Value as object[];
            object[] roundIds = currentReport.Parameters ["Round"].Value as object[];

            return dacGPS.BR_GetRoundDetailsForDateRange(fromDate, toDate, driverIds, vehicleIds, roundIds);
        }

        #endregion

        #region FleetMetrik
        private DataSet FMDriverGrading(Telerik.Reporting.InstanceReportSource currentReport, bool byWeek)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            foreach (var p in currentReport.ReportDocument.ReportParameters)
                parameters.Add(p.Name, p.Value);

            DateTime startDate = Convert.ToDateTime(parameters["StartDate"]);
            DateTime endDate = Convert.ToDateTime(parameters["EndDate"]);

            DataAccess.IGPS dacGPS = new DataAccess.GPS();
            if (byWeek)
                return dacGPS.FMDriverGradingByWeek(startDate, endDate, null);
            else
                return dacGPS.FMDriverGradingByMonth(startDate, endDate, null);
        }
        
        private DataSet FMTrendReport(Telerik.Reporting.InstanceReportSource currentReport, string reportName)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            // Parameters can be passed in 2 ways depending if the report view is sending any.
            if (((Telerik.Reporting.ReportSource)currentReport).Parameters.Count > 0)
                foreach (var p in ((Telerik.Reporting.ReportSource)currentReport).Parameters)
                    parameters.Add(p.Name, p.Value);
            else if (currentReport.ReportDocument.ReportParameters.Count() > 0)
                foreach (var p in currentReport.ReportDocument.ReportParameters)
                    parameters.Add(p.Name, p.Value);

            DateTime startDate = Convert.ToDateTime(parameters["StartDate"]);
            DateTime endDate = Convert.ToDateTime(parameters["EndDate"]);
            if (endDate.TimeOfDay != new TimeSpan(23, 59, 59))
                endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);

            int tempVal;
            int? orgUnitId  = null;
            int? driverId = null;
            orgUnitId = Int32.TryParse((parameters.ContainsKey("OrgUnitID") && parameters["OrgUnitID"] != null) ? parameters["OrgUnitID"].ToString() : "", out tempVal) ? tempVal : (int?)null;
            driverId = Int32.TryParse(parameters.ContainsKey("DriverID") ? parameters["DriverID"].ToString() : "", out tempVal) ? tempVal : (int?)null;

            DataAccess.IGPS dacGPS = new DataAccess.GPS();
            DataSet ds = null;
            switch(reportName)
            {
                case "FleetMetrik.HarshBrakingTrendByMonth":
                    ds = dacGPS.FMHarshBrakingTrendByMonth(startDate, endDate, orgUnitId, driverId);
                    break;
                case "FleetMetrik.HarshBrakingTrendByWeek":
                    ds = dacGPS.FMHarshBrakingTrendByWeek(startDate, endDate, orgUnitId, driverId);
                    break;
                case "FleetMetrik.IdlingTrendByMonth":
                    ds = dacGPS.FMIdlingTrendByMonth(startDate, endDate, orgUnitId, driverId);
                    break;
                case "FleetMetrik.IdlingTrendByWeek":
                    ds = dacGPS.FMIdlingTrendByWeek(startDate, endDate, orgUnitId, driverId);
                    break;
                case "FleetMetrik.OverRevvingTrendByMonth":
                    ds = dacGPS.FMOverRevvingTrendByMonth(startDate, endDate, orgUnitId, driverId);
                    break;
                case "FleetMetrik.OverRevvingTrendByWeek":
                    ds = dacGPS.FMOverRevvingTrendByWeek(startDate, endDate, orgUnitId, driverId);
                    break;
                case "FleetMetrik.SpeedingTrendByMonth":
                    ds = dacGPS.FMSpeedingTrendByMonth(startDate, endDate, orgUnitId, driverId);
                    break;
                case "FleetMetrik.SpeedingTrendByWeek":
                    ds = dacGPS.FMSpeedingTrendByWeek(startDate, endDate, orgUnitId, driverId);
                    break;
                case "CAN.DriverGradingDetail":
                    ds = dacGPS.FMDriverOverviewExport(startDate, endDate, orgUnitId, "", "", "", 0);
                    break;
            }

            return ds;
        }
        #endregion

        #endregion
    }
}