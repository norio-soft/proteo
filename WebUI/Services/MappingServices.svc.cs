using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Data;
using Orchestrator.WebUI.ws;
using System.ServiceModel.Activation;
using Microsoft.SqlServer.Types;
using Orchestrator.EF;
using System.Threading;
using System.Data.SqlClient;


namespace Orchestrator.WebUI.Services
{
    // NOTE: If you change the class name "MappingServices" here, you must also update the reference to "MappingServices" in Web.config.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)] // Please note this this is required for now.
   
    public class MappingServices : IMappingServices
    {
        public Orchestrator.WebUI.ws.MappingServices.DataPoint GetCurrentGPSPosition(int resourceId)
        {
            DataSet ds = Facade.Resource.GetCurrentGPSPosition(resourceId);
            var currentPosition = (from v in ds.Tables[0].AsEnumerable()
                         orderby v.Field<DateTime>("DateStamp") descending
                                   select new Orchestrator.WebUI.ws.MappingServices.DataPoint()
                         {
                             RegNo = v.Field<string>("RegNo"),
                             TrailerRef = v.Field<string>("TrailerRef"),
                             //Driver = v.Field<string>("FirstNames") + " " + v.Field<string>("LastName"),
                             Latitude = v.Field<double>("Latitude"),
                             Longitude = v.Field<double>("Longitude"),
                             Gazetteer = v.Field<string>("Gazetteer"),
                             Reason = v.Field<string>("Reason").Trim(),
                             JobID = v.Field<string>("Jobs").Length > 0 ? int.Parse(v.Field<string>("Jobs").Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)[0]) : 0,
                             Direction = v.Field<string>("Direction"),
                             DateStamp = v.Field<DateTime>("DateStamp"),
                             Speed = v.Field<double>("Speed"),
                             GpsUnitID = v.Field<string>("GPSUnitId"),
                             ResourceID = v.Field<int>("ResourceId"),
                         }).FirstOrDefault();
            return currentPosition;
        }

        public List<Orchestrator.WebUI.ws.MappingServices.RunLeg> GetLegsForRun(int runID, bool includeDataPoints)
        {
            List<Orchestrator.WebUI.ws.MappingServices.RunLeg> runLegs = new List<Orchestrator.WebUI.ws.MappingServices.RunLeg>();

            try
            {
                Orchestrator.Facade.IJob facJob = new Orchestrator.Facade.Job();
                Orchestrator.Facade.IInstruction facInstruction = new Orchestrator.Facade.Instruction();
                Orchestrator.Facade.IGPS facGPS = new Orchestrator.Facade.GPS();
                Orchestrator.Entities.InstructionCollection instructions = facInstruction.GetForJobId(runID);
                Orchestrator.Entities.LegPlan lp = new Orchestrator.Facade.Instruction().GetLegPlan(instructions, true);

                DataTable ds = facGPS.GetLegTable(lp);
                runLegs = (from v in ds.AsEnumerable()
                           select new Orchestrator.WebUI.ws.MappingServices.RunLeg()
                           {
                               JobID = v.Field<int>("JobID"),
                               StartInstructionID = v.Field<int>("StartInstructionID"),
                               EndInstructionID = v.Field<int>("EndInstructionID"),
                               ActualDistance = (v.IsNull("ActualDistance")) ? 0 : v.Field<int>("ActualDistance"),
                               ActualDuration = (v.IsNull("ActualDuration")) ? 0 : v.Field<int>("ActualDuration"),
                               EstimatedDistance = (v.IsNull("EstimatedDistance")) ? 0 : v.Field<int>("EstimatedDistance"),
                               EstimatedDuration = (v.IsNull("EstimatedDuration")) ? 0 : v.Field<int>("EstimatedDuration"),

                               StartInstructionTypeID = v.Field<int>("StartInstructionTypeId"),
                               PlannedStartDate = v.Field<DateTime>("PlannedStartDateTime"),
                               StartActualArrivalDate = v.IsNull("StartActualArrivalDateTime") ? new DateTime?() : v.Field<DateTime>("StartActualArrivalDateTime"),
                               StartActualDepartureDate = v.IsNull("StartActualDepartureDateTime") ? new DateTime?() : v.Field<DateTime>("StartActualDepartureDateTime"),
                               StartPointID = v.Field<int>("StartPointID"),
                               StartPointDescription = v.Field<string>("StartPointDisplay"),
                               StartPointLatitude = Convert.ToDouble(v.Field<decimal>("StartLatitude")),
                               StartPointLongitude = Convert.ToDouble(v.Field<decimal>("StartLongitude")),

                               EndInstructionTypeID = v.Field<int>("EndInstructionTypeId"),
                               PlannedEndDate = v.Field<DateTime>("PlannedEndDateTime"),
                               EndActualArrivalDate = v.IsNull("EndActualArrivalDateTime") ? new DateTime?() : v.Field<DateTime>("EndActualArrivalDateTime"),
                               EndActualDepartureDate = v.IsNull("EndActualDepartureDateTime") ? new DateTime?() : v.Field<DateTime>("EndActualDepartureDateTime"),
                               EndPointID = v.Field<int>("EndPointID"),
                               EndPointDescription = v.Field<string>("EndPointDisplay"),
                               EndPointLatitude = Convert.ToDouble(v.Field<decimal>("EndLatitude")),
                               EndPointLongitude = Convert.ToDouble(v.Field<decimal>("EndLongitude")),

                               RegNo = v.Field<string>("RegNo"),
                               VehicleResourceID = v.Field<int>("VehicleResourceID"),
                               DriverName = v.Field<string>("FullName"),
                               TrailerRef = v.Field<string>("TrailerRef"),
                               JobSubContractID = v.Field<int>("JobSubContractID"),
                               SubContractorIdentityID = v.Field<int>("SubContractor"),
                               SubContractorName = v.Field<string>("DrivingDisplayName"),
                               ETA = String.Empty,
                               InstructionStateID = v.Field<int>("InstructionStateID"),
                           }).ToList();

                if (includeDataPoints)
                    foreach (Orchestrator.WebUI.ws.MappingServices.RunLeg runLeg in runLegs)
                        runLeg.DataPoints = GetPositionHistoryForRunLeg(runLeg);
            }
            catch (Exception ex)
            {
                throw;
            }
            return runLegs;
        }

        public List<Orchestrator.WebUI.ws.MappingServices.DataPoint> GetPositionHistoryForRunLeg(Orchestrator.WebUI.ws.MappingServices.RunLeg runLeg)
        {
            var dataPoints = new List<Orchestrator.WebUI.ws.MappingServices.DataPoint>();

            try
            {
                if (runLeg.StartActualArrivalDate.HasValue)
                {
                    var startDateTime = runLeg.StartActualArrivalDate.Value;
                    var endDateTime = runLeg.EndActualDepartureDate ?? DateTime.Now;

                    // For trunks we don't really have any data to indicate when the subsequent leg started...
                    // the StartActualArrivalDate and StartActualDepartureDate both refer to the goods being left, which can be a long time before they are picked up again.
                    // As a result if we use this date we can end up including position history that doesn't really relate to this run at all.
                    // Therefore, if the planned start date of the next leg is later in time than the StartActualArrivalDate of the trunk then we should use that instead.
                    if (runLeg.StartInstructionTypeID == (int)eInstructionType.Trunk && runLeg.PlannedStartDate > startDateTime)
                        startDateTime = runLeg.PlannedStartDate;

                    dataPoints = GetPositionHistoryForResourceId(runLeg.VehicleResourceID, startDateTime, endDateTime);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return dataPoints;
        }

        public List<Orchestrator.WebUI.ws.MappingServices.DataPoint> GetPositionHistoryForResourceId(int ResourceId, DateTime startDate, DateTime endDate)
        {
            List<Orchestrator.WebUI.ws.MappingServices.DataPoint> fleet = new List<Orchestrator.WebUI.ws.MappingServices.DataPoint>();
            try
            {
                DataSet ds = Facade.Resource.GetPositionHistoryForeResourceID(ResourceId, startDate, endDate);
                fleet = (from v in ds.Tables[0].AsEnumerable()
                         orderby v.Field<DateTime>("DateStamp") ascending
                         select new Orchestrator.WebUI.ws.MappingServices.DataPoint()
                         {
                             RegNo = v.Field<string>("RegNo"),
                             //TrailerRef = v.Field<string>("TrailerRef"),
                             //Driver = v.Field<string>("FirstNames") + " " + v.Field<string>("LastName"),
                             Latitude = v.Field<double>("Latitude"),
                             Longitude = v.Field<double>("Longitude"),
                             Gazetteer = v.Field<string>("Gazetteer"),
                             Reason = v.Field<string>("Reason").Trim(),
                             //JobID = v.Field<string>("Jobs").Length > 0 ? int.Parse(v.Field<string>("Jobs").Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)[0]) : 0,
                             Direction = v.Field<string>("Direction"),
                             DateStamp = v.Field<DateTime>("DateStamp"),
                             Speed = v.Field<double>("Speed")
                         }).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.Message);
            }
            return fleet;
        }

         public List<Orchestrator.WebUI.ws.MappingServices.DataPoint> GetPositionHistory(string gpsUnitID, DateTime startDate, DateTime endDate)
        {
            List<Orchestrator.WebUI.ws.MappingServices.DataPoint> fleet = new List<Orchestrator.WebUI.ws.MappingServices.DataPoint>();
            try
            {
                DataSet ds = Facade.Resource.GetPositionHistory(gpsUnitID, startDate, endDate);
                fleet = (from v in ds.Tables[0].AsEnumerable()
                         orderby v.Field<DateTime>("DateStamp") descending
                         select new Orchestrator.WebUI.ws.MappingServices.DataPoint()
                         {
                             RegNo = v.Field<string>("RegNo"),
                             //TrailerRef = v.Field<string>("TrailerRef"),
                             //Driver = v.Field<string>("FirstNames") + " " + v.Field<string>("LastName"),
                             Latitude = v.Field<double>("Latitude"),
                             Longitude = v.Field<double>("Longitude"),
                             Gazetteer = v.Field<string>("Gazetteer"),
                             Reason = v.Field<string>("Reason").Trim(),
                             //JobID = v.Field<string>("Jobs").Length > 0 ? int.Parse(v.Field<string>("Jobs").Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries)[0]) : 0,
                             Direction = v.Field<string>("Direction"),
                             DateStamp = v.Field<DateTime>("DateStamp"),
                             Speed = v.Field<double>("Speed")
                         }).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.Message);
            }
            return fleet;
        }

         public List<Orchestrator.WebUI.ws.MappingServices.EmptyRunningLeg> GetEmptyRunningLegsForRun(int runID)
        {
            List<Orchestrator.WebUI.ws.MappingServices.EmptyRunningLeg> emptyRunningLegs = new List<Orchestrator.WebUI.ws.MappingServices.EmptyRunningLeg>();
            try
            {
                Facade.EmptyRunning facEmptyRunning = new Orchestrator.Facade.EmptyRunning();
                DataSet ds = facEmptyRunning.GetEmptyRunningForJobId(runID);
                emptyRunningLegs = (from v in ds.Tables[0].AsEnumerable()
                                    orderby v.Field<DateTime>("StartDate") descending
                                    select new Orchestrator.WebUI.ws.MappingServices.EmptyRunningLeg()
                                    {
                                        EmptyRunningID = v.Field<int>("EmptyRunningId"),
                                        StartJobID = v.Field<int>("StartJobID"),
                                        EndJobID = v.Field<int>("EndJobID"),
                                        ActualDistance = v.Field<int>("ActualDistance"),
                                        ActualDuration = v.Field<int>("ActualDuration"),
                                        EstimatedDistance = v.Field<int>("EstimatedDistance"),
                                        EstimatedDuration = v.Field<int>("EstimatedDuration"),

                                        StartPointID = v.Field<int>("StartPointID"),
                                        StartPointDescription = v.Field<string>("StartPointDescription"),

                                        EndPointID = v.Field<int>("EndPointID"),
                                        EndPointDescription = v.Field<string>("EndPointDescription"),

                                        StartPointLatitude = Convert.ToDouble(v.Field<decimal>("StartLatitude")),
                                        StartPointLongitude = Convert.ToDouble(v.Field<decimal>("StartLongitude")),

                                        EndPointLatitude = Convert.ToDouble(v.Field<decimal>("EndLatitude")),
                                        EndPointLongitude = Convert.ToDouble(v.Field<decimal>("EndLongitude")),

                                        StartDate = v.Field<DateTime>("StartDate"),
                                        EndDate = v.Field<DateTime>("EndDate"),
                                        EmptyRunningResourceID = v.Field<int>("EmptyRunningResourceID"),
                                        EmptyRunningResourceRef = v.Field<string>("EmptyRunningResourceRef"),
                                        StartGPSUnitID = v.Field<string>("StartGPSUnitID"),
                                        EndGPSUnitID = v.Field<string>("EndGPSUnitID"),
                                        StartRegNo = v.Field<string>("StartRegNo"),
                                        EndRegNo = v.Field<string>("EndRegNo"),
                                        EndVehicleResourceID = v.Field<int>("EndVehicleResourceID"),
                                        StartVehicleResourceID = v.Field<int>("StartVehicleResourceID"),
                                        StartDriver = v.Field<string>("StartDriver"),
                                        EndDriver = v.Field<string>("EndDriver"),
                                        DataPoints = GetPositionHistory(v.Field<string>("StartGPSUnitID"), v.Field<DateTime>("StartDate"), v.Field<DateTime>("EndDate"))
                                    }).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
            return emptyRunningLegs;
        }

         public List<Point> GetPointsPenetrated(List<LatLong> latLongs)
         {
             List<Point> points = new List<Point>();

             SqlGeographyBuilder geogBuilder = new SqlGeographyBuilder();
             geogBuilder.SetSrid(4326);
             geogBuilder.BeginGeography(OpenGisGeographyType.Polygon);

                 LatLong firstLatLong = null;
                 bool firstLatLongStored = false;
                 foreach (LatLong latLong in latLongs)
                 {
                     if (!firstLatLongStored)
                     {
                         firstLatLong = latLong;
                         geogBuilder.BeginFigure(firstLatLong.Latitude, firstLatLong.Longitude);
                         firstLatLongStored = true;
                     }
                     else
                         geogBuilder.AddLine(latLong.Latitude, latLong.Longitude);
                 }

                 geogBuilder.AddLine(firstLatLong.Latitude, firstLatLong.Longitude); //Note: Last Point same as First 
                 geogBuilder.EndFigure();

             geogBuilder.EndGeography();
             SqlGeography rectangle = null;

             try
             {
                 rectangle = geogBuilder.ConstructedGeography;
             }
             catch (Exception ex)
             {
                 SqlGeometryBuilder gb = new SqlGeometryBuilder();
                 gb.SetSrid(4326);
                 gb.BeginGeometry(OpenGisGeometryType.Polygon);

                 firstLatLong = null;
                 firstLatLongStored = false;
                 foreach (LatLong latLong in latLongs)
                 {
                     if (!firstLatLongStored)
                     {
                         firstLatLong = latLong;
                         gb.BeginFigure(firstLatLong.Latitude, firstLatLong.Longitude);
                         firstLatLongStored = true;
                     }
                     else
                         gb.AddLine(latLong.Latitude, latLong.Longitude);
                 }

                 gb.AddLine(firstLatLong.Latitude, firstLatLong.Longitude); //Note: Last Point same as First 
                 gb.EndFigure();

                 gb.EndGeometry();

                 SqlGeometry geom = null;
                 geom = gb.ConstructedGeometry.MakeValid();

                 //geom = geom.MakeValid().STUnion(geom.STStartPoint());

                 rectangle = SqlGeography.STPolyFromText(geom.STAsText(), 4326);
             }

             SqlDataReader dr = null;
             try
             {
                 BusinessLogicLayer.IPoint busPoint = new BusinessLogicLayer.Point();

                 dr = busPoint.GetPointsIntersected(rectangle);

                 while (dr.Read())
                 {
                     Point point = new Point();
                     point.GeofencePoints = new List<LatLong>();
                     point.Description = dr["PointName"].ToString();
                     point.Latitide = dr["WGS84Latitude"] != DBNull.Value ? Convert.ToDouble(dr["WGS84Latitude"]) : 0;
                     point.Longitude = dr["WGS84Longitude"] != DBNull.Value ? Convert.ToDouble(dr["WGS84Longitude"]) : 0;
                     point.PointID = int.Parse(dr["PointId"].ToString());
                     SqlGeography geofence = (SqlGeography)dr["Geofence"];

                     for (int i = 0; i < geofence.STNumPoints(); i++)
                     {
                         SqlGeography p = geofence.STPointN(i + 1);
                         LatLong latLong = new LatLong();
                         latLong.Latitude = (double)p.Lat;
                         latLong.Longitude = (double)p.Long;
                         point.GeofencePoints.Add(latLong);
                     }
                     points.Add(point);
                 }
             }
             catch (Exception ex)
             {
                 throw;
             }
             finally
             {
                 dr.Close();
             }
                 
             return points;
         }

         public Orchestrator.WebUI.ws.MappingServices.Point GetPoint(int pointID)
         {
             Orchestrator.Facade.IPoint facPoint = new Orchestrator.Facade.Point();
             Orchestrator.Entities.Point selectedPoint = facPoint.GetPointForPointId(pointID);
             Orchestrator.WebUI.ws.MappingServices.Point p = new Orchestrator.WebUI.ws.MappingServices.Point();
             p.PointID = selectedPoint.PointId;
             p.Description = selectedPoint.Description;
             p.Latitide = (double)selectedPoint.Latitude;
             p.Longitude = (double)selectedPoint.Longitude;


             p.GeofencePoints = new List<double[]>();

             string points = string.Empty;
             for (int i = 0; i < selectedPoint.Geofence.STNumPoints(); i++)
             {
                 SqlGeography point = selectedPoint.Geofence.STPointN(i + 1);
                 double[] latLon = new double[2] { (double)point.Lat, (double)point.Long };
                 p.GeofencePoints.Add(latLon);

                 //points = point.Lat.ToString() + ',' + point.Long.ToString() + '|';
             }

             return p;
         }

         internal static FleetViewTreeViewNode OrgUnitToTreeViewNode(Repositories.DTOs.OrgUnit orgUnit, int? parentID)
         {
             var orgUnitID = orgUnit.OrgUnitID;

             var node = new FleetViewTreeViewNode
             {
                 Id = orgUnitID,
                 ParentId = parentID ?? 0,
                 Description = orgUnit.Name,
                 NodeType = FleetViewTreeViewNode.DataNodeType.OrgUnit,
             };

             node.Children = orgUnit.Children.Select(c => OrgUnitToTreeViewNode(c, orgUnitID)).ToList();

             if (orgUnit.Vehicles.Any())
             {
                 node.Children.AddRange(orgUnit.Vehicles.Select(v => new FleetViewTreeViewNode
                 {
                     Id = v.ResourceID,
                     ParentId = orgUnitID,
                     Description = v.RegistrationNumber,
                     NodeType = FleetViewTreeViewNode.DataNodeType.Vehicle,
                     GpsUnitId = v.GpsUnitID,
                 }));
             }

             return node;
         }

    }

}
