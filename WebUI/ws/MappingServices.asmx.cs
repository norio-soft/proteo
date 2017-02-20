using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Runtime.Serialization;
using Microsoft.SqlServer.Types;
using Orchestrator.WebUI.Services;
using System.Data.SqlClient;
using Orchestrator.Repositories;

namespace Orchestrator.WebUI.ws
{
    /// <summary>
    /// Summary description for MappingServices1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class MappingServices : System.Web.Services.WebService
    {
        #region Fleetview Methods
        int VEHICLE_RESOURCE_TYPE = 1;

        [WebMethod]
        public string GetBluesphereCustomerId()
        {
            return Globals.Configuration.BlueSphereCustomerId;    
        }

        [WebMethod]
        public List<VehicleCacheItem> RefreshVehicleCache()
        {
            return RefreshVehicleCacheForClient(null);
        }

        [WebMethod]
        public List<VehicleCacheItem> RefreshVehicleCacheForClient(int? clientIdentityID)
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<IVehicleRepository>(uow);
                var vehicles = repo.GetAll();

                if (clientIdentityID.HasValue)
                    vehicles = vehicles.Where(v => v.DedicatedToClientIdentityID == clientIdentityID.Value);

                var vehicleCache =
                    from v in vehicles
                    let r = v.Resource
                    where r.ResourceStatus == eResourceStatus.Active
                    where !string.IsNullOrEmpty(r.GPSUnitID)
                    select new VehicleCacheItem
                    {
                        GpsUnitId = r.GPSUnitID,
                        Reg = v.RegNo,
                    };

                return vehicleCache.ToList();
            }
        }

        [WebMethod]
        public List<DriverCacheItem> RefreshDriverCache()
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<IDriverRepository>(uow);
                var drivers = repo.GetAll();

                var driverCache =
                    from d in drivers
                    where !string.IsNullOrEmpty(d.DigitalTachoCardID)
                    let r = d.Resource
                    where r.ResourceStatus == eResourceStatus.Active
                    let i = d.Individual
                    select new DriverCacheItem
                    {
                        TachoId = d.DigitalTachoCardID,
                        DriverName = i.FirstNames + " " + i.LastName,
                    };

                return driverCache.ToList();
            }
        }

        [WebMethod]
        public List<Orchestrator.WebUI.Services.FleetViewTreeViewNode> GetResourceViewsForVehicle(int? clientIdentityID = null)
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<IOrgUnitRepository>(uow);
                var orgUnits = repo.GetNested(true, false, false, clientIdentityID);
                var retVal = orgUnits.Select(ou => Services.MappingServices.OrgUnitToTreeViewNode(ou, null));
                return retVal.ToList();
            }
        }

        [WebMethod]
        public List<Orchestrator.WebUI.Services.Point> GetPointsPenetrated(List<LatLong> latLongs)
        {
            List<Orchestrator.WebUI.Services.Point> points = new List<Orchestrator.WebUI.Services.Point>();

            if (latLongs.Count > 0)
            {
                SqlGeographyBuilder geogBuilder = new SqlGeographyBuilder();
                geogBuilder.SetSrid(4326);
                geogBuilder.BeginGeography(OpenGisGeographyType.LineString);

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

                //geogBuilder.AddLine(firstLatLong.Latitude, firstLatLong.Longitude); //Note: Last Point same as First 
                geogBuilder.EndFigure();

                geogBuilder.EndGeography();
                SqlGeography rectangle = null;

                try
                {
                    rectangle = geogBuilder.ConstructedGeography;
                }
                catch (Exception ex)
                {
                    //SqlGeometryBuilder gb = new SqlGeometryBuilder();
                    //gb.SetSrid(4326);
                    //gb.BeginGeometry(OpenGisGeometryType.Polygon);

                    //firstLatLong = null;
                    //firstLatLongStored = false;
                    //foreach (LatLong latLong in latLongs)
                    //{
                    //    if (!firstLatLongStored)
                    //    {
                    //        firstLatLong = latLong;
                    //        gb.BeginFigure(firstLatLong.Latitude, firstLatLong.Longitude);
                    //        firstLatLongStored = true;
                    //    }
                    //    else
                    //        gb.AddLine(latLong.Latitude, latLong.Longitude);
                    //}

                    //gb.AddLine(firstLatLong.Latitude, firstLatLong.Longitude); //Note: Last Point same as First 
                    //gb.EndFigure();

                    //gb.EndGeometry();

                    //SqlGeometry geom = null;
                    //geom = gb.ConstructedGeometry.MakeValid();

                    ////geom = geom.MakeValid().STUnion(geom.STStartPoint());

                    //rectangle = SqlGeography.STPolyFromText(geom.STAsText(), 4326);
                }

                SqlDataReader dr = null;
                try
                {
                    BusinessLogicLayer.IPoint busPoint = new BusinessLogicLayer.Point();

                    dr = busPoint.GetPointsIntersected(rectangle);

                    while (dr.Read())
                    {
                        Orchestrator.WebUI.Services.Point point = new Orchestrator.WebUI.Services.Point();
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
            }

            return points;
        }

        [WebMethod]
        public List<DataPoint> GetFleet()
        {
            DataSet ds = Facade.Resource.GetFleetView();
            var fleet = (from v in ds.Tables[0].AsEnumerable()
                         orderby v.Field<DateTime>("DateStamp") descending
                         select new DataPoint()
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
                             GpsUnitID = v.Field<string>("GPSUnitId")
                         }).ToList();
            return fleet;
        }

        [WebMethod]
        public List<DataPoint> GetNoticeboard(string deviceIdentifiers)
        {
            List<DataPoint> retVal = new List<DataPoint>();
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<IGPSPositionRepository>(uow);
                var data = repo.GetNoticeboard(deviceIdentifiers);

                foreach(var gps in data)
                {
                    retVal.Add(new DataPoint()
                    {
                        RegNo = gps.RegNo,
                        DateStamp = gps.DateStamp,
                        Description = gps.Reason,
                        Direction = gps.Direction.ToString(),
                        Driver = gps.FirstNames + ' ' + gps.LastName,
                        Gazetteer = gps.LocationString,
                        GpsUnitID = gps.GPSUnitID,
                        Latitude = gps.Latitude,
                        Longitude = gps.Longitude,
                        Reason = gps.Reason,
                        ResourceID = gps.ResourceID,
                        Speed = gps.Speed,
                        DigitalTachoCardId = gps.DigitalTachoCardId

                    });
                }

            }
            return retVal;
        }

        [WebMethod]
        public List<DataPoint> GetPositionHistory(string gpsUnitID, DateTime startDate, DateTime endDate)
        {
            List<DataPoint> fleet = new List<DataPoint>();
            try
            {
                DataSet ds = Facade.Resource.GetPositionHistory(gpsUnitID, startDate, endDate);
                fleet = (from v in ds.Tables[0].AsEnumerable()
                         orderby v.Field<DateTime>("DateStamp") descending
                         select new DataPoint()
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
                             Speed = v.Field<double>("Speed"),
                             GpsUnitID = v.Field<string>("GPSUnitID")
                         }).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex.Message);
            }
            return fleet;
        }

        [WebMethod]
        public List<DataPoint> GetPositionHistoryForResourceId(int ResourceId, DateTime startDate, DateTime endDate)
        {
            List<DataPoint> fleet = new List<DataPoint>();
            try
            {
                DataSet ds = Facade.Resource.GetPositionHistoryForeResourceID(ResourceId, startDate, endDate);
                fleet = (from v in ds.Tables[0].AsEnumerable()
                         orderby v.Field<DateTime>("DateStamp") descending
                         select new DataPoint()
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

        [WebMethod]
        public List<RunLeg> GetLegsForRun(int runID, bool includeDataPoints)
        {
            List<RunLeg> runLegs = new List<RunLeg>();

            try
            {
                Orchestrator.Facade.IJob facJob = new Orchestrator.Facade.Job();
                Orchestrator.Facade.IInstruction facInstruction = new Orchestrator.Facade.Instruction();
                Orchestrator.Facade.IGPS facGPS = new Orchestrator.Facade.GPS();
                Orchestrator.Entities.InstructionCollection instructions = facInstruction.GetForJobId(runID);
                Orchestrator.Entities.LegPlan lp = new Orchestrator.Facade.Instruction().GetLegPlan(instructions, true);

                DataTable ds = facGPS.GetLegTable(lp);
                runLegs = (from v in ds.AsEnumerable()
                           select new RunLeg()
                                    {
                                        JobID = v.Field<int>("JobID"),
                                        StartInstructionID = v.Field<int>("StartInstructionID"),
                                        EndInstructionID = v.Field<int>("EndInstructionID"),
                                        ActualDistance = (v.IsNull("ActualDistance")) ? 0 : v.Field<int>("ActualDistance"),
                                        ActualDuration = (v.IsNull("ActualDuration")) ? 0 : v.Field<int>("ActualDuration"),
                                        EstimatedDistance = (v.IsNull("EstimatedDistance")) ? 0 : v.Field<int>("EstimatedDistance"),
                                        EstimatedDuration = (v.IsNull("EstimatedDuration")) ? 0 : v.Field<int>("EstimatedDuration"),

                                        PlannedStartDate = v.Field<DateTime>("PlannedStartDateTime"),
                                        StartActualArrivalDate = v.IsNull("StartActualArrivalDateTime") ? new DateTime?() : v.Field<DateTime>("StartActualArrivalDateTime"),
                                        StartActualDepartureDate = v.IsNull("StartActualDepartureDateTime") ? new DateTime?() : v.Field<DateTime>("StartActualDepartureDateTime"),
                                        StartPointID = v.Field<int>("StartPointID"),
                                        StartPointDescription = v.Field<string>("StartPointDisplay"),
                                        StartPointLatitude = Convert.ToDouble(v.Field<decimal>("StartLatitude")),
                                        StartPointLongitude = Convert.ToDouble(v.Field<decimal>("StartLongitude")),

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
                                        InstructionStateID = v.Field<int>("InstructionStateId"),
                                    }).ToList();

                if (includeDataPoints)
                    foreach (RunLeg runLeg in runLegs)
                        runLeg.DataPoints = GetPositionHistoryForRunLeg(runLeg);
            }
            catch (Exception ex)
            {
                throw;
            }
            return runLegs;
        }

        [WebMethod]
        public List<DataPoint> GetPositionHistoryForRunLeg(RunLeg runLeg)
        {
            List<DataPoint> dataPoints = new List<DataPoint>();

            try
            {
                if (runLeg.StartActualArrivalDate.HasValue)
                    dataPoints = GetPositionHistoryForResourceId(runLeg.VehicleResourceID, runLeg.StartActualArrivalDate.Value,
                        (runLeg.EndActualDepartureDate.HasValue) ? runLeg.EndActualDepartureDate.Value : DateTime.Now);
            }
            catch (Exception ex)
            {
                throw;
            }
            return dataPoints;
        }

        [WebMethod]
        public List<EmptyRunningLeg> GetEmptyRunningLegsForRun(int runID)
        {
            List<EmptyRunningLeg> emptyRunningLegs = new List<EmptyRunningLeg>();
            try
            {
                Facade.EmptyRunning facEmptyRunning = new Orchestrator.Facade.EmptyRunning();
                DataSet ds = facEmptyRunning.GetEmptyRunningForJobId(runID);
                emptyRunningLegs = (from v in ds.Tables[0].AsEnumerable()
                                    orderby v.Field<DateTime>("StartDate") descending
                                    select new EmptyRunningLeg()
                         {
                             EmptyRunningID = v.Field<int>("EmptyRunningID"),
                             EmptyRunningResourceID = v.Field<int>("EmptyRunningResourceID"),
                             EmptyRunningResourceRef = v.Field<string>("EmptyRunningResourceRef"),
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

                             StartGPSUnitID = v.Field<string>("StartGPSUnitID"),
                             EndGPSUnitID = v.Field<string>("EndGPSUnitID"),
                             StartRegNo = v.Field<string>("StartRegNo"),
                             EndRegNo = v.Field<string>("EndRegNo"),
                             EndVehicleResourceID = v.Field<int>("EndVehicleResourceID"),
                             StartVehicleResourceID = v.Field<int>("StartVehicleResourceID"),

                             StartDate = v.Field<DateTime>("StartDate"),
                             EndDate = v.Field<DateTime>("EndDate"),


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

        [WebMethod]
        public List<Orchestrator.WebUI.Services.Point> GetConcentricCirclesForLatLong(Orchestrator.WebUI.Services.Point selectedPoint)
        {
            List<Orchestrator.WebUI.Services.Point> points = new List<Orchestrator.WebUI.Services.Point>();

            points.Add(selectedPoint);

            try
            {
                SqlGeography position = this.BuildGeography(new List<LatLong>() { new LatLong() { Latitude = selectedPoint.Latitude, Longitude = selectedPoint.Longitude } }, OpenGisGeographyType.Point);

                List<KeyValuePair<int, double>> distances = new List<KeyValuePair<int, double>>();

                double metresPerMile = 1609.344;
                KeyValuePair<int, double> cc = new KeyValuePair<int, double>(1, (metresPerMile / 2));
                distances.Add(cc);

                for (int i = 1; i <= 2; i++)
                {
                    KeyValuePair<int, double> tc = new KeyValuePair<int, double>(Convert.ToInt32(5 * Math.Pow(2, i - 1)), (5 * Math.Pow(2, i - 1)) * (metresPerMile));// / 2));
                    distances.Add(tc);
                }

                foreach (KeyValuePair<int, double> kvp in distances)
                {
                    SqlGeography sqlGeofence = position.STBuffer(kvp.Value);

                    Orchestrator.WebUI.Services.Point point = new Orchestrator.WebUI.Services.Point();
                    point.GeofencePoints = new List<LatLong>();
                    point.Description = kvp.Key.ToString();
                    point.Latitude = selectedPoint.Latitude;
                    point.Longitude = selectedPoint.Longitude;

                    for (int i = 0; i < sqlGeofence.STNumPoints(); i++)
                    {
                        SqlGeography p = sqlGeofence.STPointN(i + 1);
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

            return points;
        }

        [WebMethod]
        public List<VehicleNearLocationItem> GetVehicleIntersectionsForDateTimeAndRadius(DateTime startDateTime, DateTime endDateTime, int radius, double latitude, double longitude, int? clientIdentityID = null)
        {
            BusinessLogicLayer.IPoint busPoint = new BusinessLogicLayer.Point();
            List<VehicleNearLocationItem> vehicleItems = new List<VehicleNearLocationItem>();
            DataSet ds = null;
            double metresPerMile = 1609.344;

            try
            {
                SqlGeography position = this.BuildGeography(new List<LatLong>() { new LatLong() { Latitude = latitude, Longitude = longitude } }, OpenGisGeographyType.Point);

                SqlGeography sqlInnerPoint = position.STBuffer(0.1);
                SqlGeography sqlGeofence = position.STBuffer(radius * metresPerMile);

                ds = busPoint.GetVehicleIntersectionsForDateTimeAndGeofence(startDateTime, endDateTime, sqlGeofence, sqlInnerPoint, clientIdentityID);

                if (ds != null && ds.Tables.Count > 0)
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        VehicleNearLocationItem vnl = new VehicleNearLocationItem();
                        vnl.GPSPositionHistoryId = dr.Field<int>("GPSPositionHistoryId");
                        vnl.GPSUnitId = dr.Field<string>("GPSUnitId");
                        vnl.ResourceRef = dr.Field<string>("ResourceRef");
                        vnl.DistanceFromPoint = dr.Field<double>("DistanceFromPoint");
                        vnl.LastResponseTime = dr.Field<DateTime>("LastResponseTime");
                        vnl.Latitude = dr.Field<double>("Latitude");
                        vnl.Longitude = dr.Field<double>("Longitude");
                        vehicleItems.Add(vnl);
                    }
            }
            catch (Exception ex)
            {
                throw;
            }

            return vehicleItems;
        }

        #endregion

        #region private functions

        public SqlGeography BuildGeography(List<LatLong> points, OpenGisGeographyType type)
        {
            SqlGeography geog = null;

            SqlGeographyBuilder geogBuilder = new SqlGeographyBuilder();
            geogBuilder.SetSrid(4326);
            geogBuilder.BeginGeography(type);

            LatLong firstLatLong = null;
            bool firstLatLongStored = false;

            foreach (LatLong latLong in points)
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

            if (type == OpenGisGeographyType.Polygon)
                if (firstLatLong.Latitude != points[points.Count - 1].Latitude && firstLatLong.Longitude != points[points.Count - 1].Longitude)
                    geogBuilder.AddLine(firstLatLong.Latitude, firstLatLong.Longitude);

            geogBuilder.EndFigure();
            geogBuilder.EndGeography();

            try
            {
                geog = geogBuilder.ConstructedGeography;
            }
            catch (Exception ex)
            {
                throw;
            }

            return geog;
        }

        #endregion

        #region Data Objects

        [DataContract]
        public class DataPoint
        {
            [DataMember]
            public int ResourceID { get; set; }
            [DataMember]
            public int? JobID { get; set; }
            [DataMember]
            public double Speed { get; set; }
            [DataMember]
            public string GpsUnitID { get; set; }
            [DataMember]
            public string RegNo { get; set; }
            [DataMember]
            public string TrailerRef { get; set; }
            [DataMember]
            public string Driver { get; set; }
            [DataMember]
            public DateTime DateStamp { get; set; }
            [DataMember]
            public double Latitude { get; set; }
            [DataMember]
            public double Longitude { get; set; }
            [DataMember]
            public string Gazetteer { get; set; }
            [DataMember]
            public string Direction { get; set; }
            [DataMember]
            public string Reason { get; set; }
            [DataMember]
            public int Rotation { get; set; }
            [DataMember]
            public string Description
            {
                get
                {
                    string description = this.RegNo.Length == 0 || this.RegNo == null ? this.TrailerRef : this.RegNo;
                    return description;
                }
                set { }
            }

            public string GetDescription()
            {
                string description = this.RegNo.Length == 0 || this.RegNo == null ? this.TrailerRef : this.RegNo;
                return description;
            }

            public string DigitalTachoCardId { get; set; }
        }

        [DataContract]
        public class EmptyRunningLeg
        {
            [DataMember]
            public int EmptyRunningID { get; set; }
            [DataMember]
            public int EmptyRunningResourceID { get; set; }
            [DataMember]
            public string EmptyRunningResourceRef { get; set; }
            [DataMember]
            public int StartJobID { get; set; }
            [DataMember]
            public int EndJobID { get; set; }
            [DataMember]
            public int ActualDistance { get; set; }
            [DataMember]
            public int ActualDuration { get; set; }
            [DataMember]
            public int EstimatedDistance { get; set; }
            [DataMember]
            public int EstimatedDuration { get; set; }
            [DataMember]
            public string StartPointDescription { get; set; }
            [DataMember]
            public int StartPointID { get; set; }
            [DataMember]
            public double StartPointLatitude { get; set; }
            [DataMember]
            public double StartPointLongitude { get; set; }
            [DataMember]
            public string EndPointDescription { get; set; }
            [DataMember]
            public int EndPointID { get; set; }
            [DataMember]
            public double EndPointLatitude { get; set; }
            [DataMember]
            public double EndPointLongitude { get; set; }
            [DataMember]
            public DateTime StartDate { get; set; }
            [DataMember]
            public DateTime EndDate { get; set; }
            [DataMember]
            public string EndRegNo { get; set; }
            [DataMember]
            public string StartRegNo { get; set; }
            [DataMember]
            public string EndGPSUnitID { get; set; }
            [DataMember]
            public string StartGPSUnitID { get; set; }
            [DataMember]
            public int EndVehicleResourceID { get; set; }
            [DataMember]
            public int StartVehicleResourceID { get; set; }
            [DataMember]
            public string StartDriver { get; set; }
            [DataMember]
            public string EndDriver { get; set; }
            [DataMember]
            public List<DataPoint> DataPoints { get; set; }
        }

        [DataContract]
        public class RunLeg
        {
            [DataMember]
            public int JobID { get; set; }

            [DataMember]
            public DateTime PlannedStartDate { get; set; }
            [DataMember]
            public DateTime? StartActualArrivalDate { get; set; }
            [DataMember]
            public DateTime? StartActualDepartureDate { get; set; }
            [DataMember]
            public DateTime PlannedEndDate { get; set; }
            [DataMember]
            public int ActualDistance { get; set; }
            [DataMember]
            public int ActualDuration { get; set; }
            [DataMember]
            public int EstimatedDistance { get; set; }
            [DataMember]
            public int EstimatedDuration { get; set; }
            [DataMember]
            public int StartInstructionTypeID { get; set; }
            [DataMember]
            public int InstructionStateID { get; set; }
            [DataMember]
            public int StartInstructionID { get; set; }
            [DataMember]
            public string StartPointDescription { get; set; }
            [DataMember]
            public int StartPointID { get; set; }
            [DataMember]
            public double StartPointLatitude { get; set; }
            [DataMember]
            public double StartPointLongitude { get; set; }
            [DataMember]
            public DateTime? EndActualArrivalDate { get; set; }
            [DataMember]
            public DateTime? EndActualDepartureDate { get; set; }
            [DataMember]
            public int EndInstructionTypeID { get; set; }
            [DataMember]
            public int EndInstructionID { get; set; }
            [DataMember]
            public string EndPointDescription { get; set; }
            [DataMember]
            public int EndPointID { get; set; }
            [DataMember]
            public double EndPointLatitude { get; set; }
            [DataMember]
            public double EndPointLongitude { get; set; }
            [DataMember]
            public string RegNo { get; set; }
            [DataMember]
            public int VehicleResourceID { get; set; }
            [DataMember]
            public string DriverName { get; set; }
            [DataMember]
            public string TrailerRef { get; set; }
            [DataMember]
            public int TrailerResourceID { get; set; }
            [DataMember]
            public int JobSubContractID { get; set; }
            [DataMember]
            public int SubContractorIdentityID { get; set; }
            [DataMember]
            public string SubContractorName { get; set; }
            [DataMember]
            public List<DataPoint> DataPoints { get; set; }
            [DataMember]
            public string ETA { get; set; }
        }

        [DataContract]
        public class Load
        {
            [DataMember]
            public int OrderID { get; set; }
            [DataMember]
            public int Sequence { get; set; }
            [DataMember]
            public double CollectionLatitude { get; set; }
            [DataMember]
            public double CollectionLongitude { get; set; }
            [DataMember]
            public double DeliveryLatitude { get; set; }
            [DataMember]
            public double DeliveryLongitude { get; set; }
            [DataMember]
            public string CollectionPointDescription { get; set; }
            [DataMember]
            public string DeliveryPointDescription { get; set; }
        }

        [DataContract]
        public class Point
        {
            [DataMember]
            public int PointID { get; set; }
            [DataMember]
            public string Description { get; set; }
            [DataMember]
            public double Latitide { get; set; }
            [DataMember]
            public double Longitude { get; set; }
            [DataMember]
            public List<double[]> GeofencePoints { get; set; }

        }
        #endregion
    }
}