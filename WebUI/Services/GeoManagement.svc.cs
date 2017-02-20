using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Microsoft.SqlServer.Types;
using System.ServiceModel.Activation;
using Orchestrator.EF;
using System.Data;
using Orchestrator.Entities;
using Orchestrator.Repositories;

namespace Orchestrator.WebUI.Services
{
    //-----------------------------------------------------------------------------------------------------------

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "GeoManagement" in code, svc and config file together.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)] // Please note this this is required for now.
    [ServiceContract]
    public class GeoManagement 
    {
        int VEHICLE_RESOURCE_TYPE = 1;
        //-----------------------------------------------------------------------------------------------------------

        [OperationContract]
        public Orchestrator.WebUI.Services.Point GetPoint(int pointID)
        {
            Orchestrator.WebUI.Services.Point p = this.GetPointForWebService(pointID);

            return p;
        }

        //-----------------------------------------------------------------------------------------------------------

        [OperationContract]
        public Orchestrator.WebUI.Services.PointGeofence GetPointGeofence(int pointID)
        {
            return GetGeofenceForWebService(pointID);
        }

        //-----------------------------------------------------------------------------------------------------------

        [OperationContract]
        public List<EF.PointGeofenceNotification> GetPointGeofenceNotifications(int pointID)
        {
            List<EF.PointGeofenceNotification> pgns = new List<PointGeofenceNotification>();

            if ((from r in DataContext.Current.PointGeofenceNotificationSet.Include("PointGeofence").Include("WeekActivePeriod")
                    where r.PointGeofence.PointId == pointID
                    select r).Any())
            {
                try
                {
                    pgns = (from r in DataContext.Current.PointGeofenceNotificationSet.Include("PointGeofence").Include("WeekActivePeriod")
                            where r.PointGeofence.PointId == pointID
                            select r).ToList();
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return pgns;
        }

        //-----------------------------------------------------------------------------------------------------------

        [OperationContract]
        public List<UserLight> GetPointGeofenceNotificationUsers(int notificationId)
        {
            List<UserLight> notificationUsers = new List<UserLight>();
            Facade.IUser facUser = new Facade.User();
            DataSet dsUsers = facUser.GetPointGeofenceNotificationUsers(notificationId);

            try
            {
                foreach (DataRow dr in dsUsers.Tables[0].Rows)
                    notificationUsers.Add(new UserLight()
                    {
                        NotificationId = (int?)dr["NotificationId"],
                        IdentityId = dr["IdentityId"] == DBNull.Value ? new int?() : (int)dr["IdentityId"],
                        UserName = dr["UserName"] == DBNull.Value ? String.Empty : dr["UserName"].ToString(),
                        TypeId = Convert.ToInt32(dr["TypeId"]),
                        Recipient = dr["Recipient"] == DBNull.Value ? String.Empty : dr["Recipient"].ToString()
                    });
            }
            catch (Exception ex)
            { 
                
            }

            return notificationUsers;
        }

        //-----------------------------------------------------------------------------------------------------------

        [OperationContract]
        public EF.WeekActivePeriod GetWeekActivePeriod(Guid weekActivePeriodId)
        {
            var weekActivePeriod = (from wap in EF.DataContext.Current.WeekActivePeriodSet
                    where wap.WeekActivePeriodID == weekActivePeriodId
                    select wap).FirstOrDefault();

            return weekActivePeriod;
        }

        //-----------------------------------------------------------------------------------------------------------

        [OperationContract]
        public EF.WeekActivePeriod UpdateWeekActivePeriod(EF.WeekActivePeriod weekActivePeriod)
        {
                try
                {
                    EF.WeekActivePeriod periodToSave = (from wap in EF.DataContext.Current.WeekActivePeriodSet
                                                        where wap.WeekActivePeriodID == weekActivePeriod.WeekActivePeriodID
                                                        select wap).FirstOrDefault();

                    periodToSave.Monday = weekActivePeriod.Monday;
                    periodToSave.Monday2 = weekActivePeriod.Monday2;
                    periodToSave.Tuesday = weekActivePeriod.Tuesday;
                    periodToSave.Tuesday2 = weekActivePeriod.Tuesday2;
                    periodToSave.Wednesday = weekActivePeriod.Wednesday;
                    periodToSave.Wednesday2 = weekActivePeriod.Wednesday2;
                    periodToSave.Thursday = weekActivePeriod.Thursday;
                    periodToSave.Thursday2 = weekActivePeriod.Thursday2;
                    periodToSave.Friday = weekActivePeriod.Friday;
                    periodToSave.Friday2 = weekActivePeriod.Friday2;
                    periodToSave.Saturday = weekActivePeriod.Saturday;
                    periodToSave.Saturday2 = weekActivePeriod.Saturday2;
                    periodToSave.Sunday = weekActivePeriod.Sunday;
                    periodToSave.Sunday2 = weekActivePeriod.Sunday2;

                    EF.DataContext.Current.SaveChanges();
                }
                catch (Exception ex)
                { 
                
                }

                EF.WeekActivePeriod weekActivePeriodToReturn = null;

                try
                {
                    weekActivePeriodToReturn = (from wap in EF.DataContext.Current.WeekActivePeriodSet
                                                where wap.WeekActivePeriodID == weekActivePeriod.WeekActivePeriodID
                                                select wap).FirstOrDefault();
                }
                catch (Exception ex)
                { 
                    
                }

                 return weekActivePeriodToReturn;
        }

        //-----------------------------------------------------------------------------------------------------------

        [OperationContract]
        public EF.WeekActivePeriod AddWeekActivePeriod(EF.WeekActivePeriod weekActivePeriod)
        {
            try
            {
                EF.WeekActivePeriod periodToSave = new EF.WeekActivePeriod();
                periodToSave.WeekActivePeriodID = weekActivePeriod.WeekActivePeriodID;
                periodToSave.Monday = weekActivePeriod.Monday;
                periodToSave.Monday2 = weekActivePeriod.Monday2;
                periodToSave.Tuesday = weekActivePeriod.Tuesday;
                periodToSave.Tuesday2 = weekActivePeriod.Tuesday2;
                periodToSave.Wednesday = weekActivePeriod.Wednesday;
                periodToSave.Wednesday2 = weekActivePeriod.Wednesday2;
                periodToSave.Thursday = weekActivePeriod.Thursday;
                periodToSave.Thursday2 = weekActivePeriod.Thursday2;
                periodToSave.Friday = weekActivePeriod.Friday;
                periodToSave.Friday2 = weekActivePeriod.Friday2;
                periodToSave.Saturday = weekActivePeriod.Saturday;
                periodToSave.Saturday2 = weekActivePeriod.Saturday2;
                periodToSave.Sunday = weekActivePeriod.Sunday;
                periodToSave.Sunday2 = weekActivePeriod.Sunday2;

                EF.DataContext.Current.AddToWeekActivePeriodSet(periodToSave);
                EF.DataContext.Current.SaveChanges();
            }
            catch (Exception ex)
            { 
            
            }
            EF.WeekActivePeriod weekPeriod = null;
            try
            {
                weekPeriod = EF.DataContext.Current.WeekActivePeriodSet.FirstOrDefault(w => w.WeekActivePeriodID == weekActivePeriod.WeekActivePeriodID);
            }
            catch (Exception ex)
            { 
                
            }

            return weekPeriod;
        }

        //-----------------------------------------------------------------------------------------------------------

        [OperationContract]
        public List<Orchestrator.WebUI.Services.FleetViewTreeViewNode> GetVehiclesForPointGeofence(int pointId)
        {

            List<FleetViewTreeViewNode> returnCollection = new List<FleetViewTreeViewNode>();
            try
            {
                List<EF.PointGeofenceVehicle> pointGeofenceVehicles = (from pgv in EF.DataContext.Current.PointGeofenceVehicleSet.Include("Resource.Vehicle").Include("Resource.ResourceType")
                                             where pgv.PointGeofence.PointId == pointId
                                                && pgv.Resource.ResourceType.ResourceTypeId == VEHICLE_RESOURCE_TYPE && pgv.Resource.Vehicle != null
                                                && pgv.Resource.ResourceStatus.ResourceStatusId == 1
                                             orderby pgv.Resource.Vehicle.RegNo
                                             select pgv).ToList();

                foreach (EF.PointGeofenceVehicle pointGeofenceVehicle in pointGeofenceVehicles)
                    returnCollection.Add(new FleetViewTreeViewNode()
                    {
                        Id = pointGeofenceVehicle.Resource.ResourceId,
                        Description = pointGeofenceVehicle.Resource.Vehicle.RegNo,
                        GpsUnitId = pointGeofenceVehicle.Resource.GPSUnitID,
                        NodeType = FleetViewTreeViewNode.DataNodeType.Vehicle,
                        ParentId = pointId
                    });
            }
            catch (Exception ex)
            {
                throw;
            }

            return returnCollection;
        }

        //-----------------------------------------------------------------------------------------------------------

        [OperationContract]
        public List<Orchestrator.WebUI.Services.FleetViewTreeViewNode> UpdatePointGeofenceVehicles(int pointId, List<Orchestrator.WebUI.Services.FleetViewTreeViewNode> pointGeofenceVehicles)
        {
            
            List<FleetViewTreeViewNode> returnCollection = new List<FleetViewTreeViewNode>();
                try
                {
                    EF.PointGeofence geofence = (from pg in EF.DataContext.Current.PointGeofenceSet.Include("PointGeofenceVehicles")
                                                                           where pg.PointId == pointId
                                                                           select pg).FirstOrDefault();

                    for (int i = geofence.PointGeofenceVehicles.Count - 1; i >= 0; i--)
                        EF.DataContext.Current.DeleteObject(geofence.PointGeofenceVehicles.ElementAt(i));

                    foreach (Orchestrator.WebUI.Services.FleetViewTreeViewNode pointGeofenceVehicle in pointGeofenceVehicles)
                    {
                        EF.PointGeofenceVehicle vehicle = new PointGeofenceVehicle(){GPSUnitId = pointGeofenceVehicle.GpsUnitId};
                        vehicle.Resource = EF.DataContext.Current.Resources.FirstOrDefault(r => r.ResourceId == pointGeofenceVehicle.Id);
                        vehicle.PointGeofence = geofence;
                    }

                    EF.DataContext.Current.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw;
                }

            return returnCollection;
        }

        //-----------------------------------------------------------------------------------------------------------

        [OperationContract]
        public List<Orchestrator.WebUI.Services.FleetViewTreeViewNode> GetResourceViewsForVehicle()
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<IOrgUnitRepository>(uow);
                var orgUnits = repo.GetNested(true, false, false, null);
                var retVal = orgUnits.Select(ou => MappingServices.OrgUnitToTreeViewNode(ou, null));
                return retVal.ToList();
            }
        }

        //-----------------------------------------------------------------------------------------------------------

        [OperationContract]
        public List<UserLight> GetAllUsers()
        {
            List<UserLight> users = new List<UserLight>();

            Facade.IUser facUser = new Facade.User();
            DataSet dsUSers = facUser.GetAllUserLight();

            foreach (DataRow dr in dsUSers.Tables[0].Rows)
                users.Add(new UserLight()
                {
                    NotificationId = 0,
                    IdentityId = dr["IdentityId"] == DBNull.Value ? new int?() : (int)dr["IdentityId"], 
                    UserName = dr["UserName"] == DBNull.Value ? String.Empty : dr["UserName"].ToString(), 
                    TypeId = (int)dr["TypeId"],
                    Recipient = dr["Recipient"] == DBNull.Value ? String.Empty : dr["Recipient"].ToString()
                });

            return users;
        }

        //-----------------------------------------------------------------------------------------------------------

        [OperationContract]
        public EF.PointGeofenceNotification GetPointGeofenceNotification(int notificationId)
        {
            return (from r in EF.DataContext.Current.PointGeofenceNotificationSet.Include("PointGeofence").Include("WeekActivePeriod")
                    where r.NotificationId == notificationId
                    select r).FirstOrDefault();
        }

        [OperationContract]
        public EF.PointGeofenceNotification GetPointGeofenceNotificationWithUsers(int notificationId)
        {
            return (from r in EF.DataContext.Current.PointGeofenceNotificationSet.Include("PointGeofence").Include("WeekActivePeriod").Include("PointGeofenceNotificationUser")
                    where r.NotificationId == notificationId
                    select r).FirstOrDefault();
        }

        //-----------------------------------------------------------------------------------------------------------

        private EF.PointGeofenceNotification GetPointGeofenceNotificationForWebService(int pointID)
        {
            return (from r in EF.DataContext.Current.PointGeofenceNotificationSet.Include("PointGeofence").Include("WeekActivePeriod")
                    where r.PointGeofence.PointId == pointID
                    select r).FirstOrDefault();
        }

        //-----------------------------------------------------------------------------------------------------------

        [OperationContract]
        public EF.PointGeofenceNotification AddPointGeofenceNotification(EF.PointGeofenceNotification pointGeofenceNotification, int pointId, List<UserLight> notificationUsers)
        {
                try
                {
                    //EF.DataContext.Current.AttachTo("PointGeofenceNotificationSet", pointGeofenceNotification);
                        Guid weekActivePeriodId = pointGeofenceNotification.WeekActivePeriod.WeekActivePeriodID;
                        EF.WeekActivePeriod week = EF.DataContext.Current.WeekActivePeriodSet.First(w => w.WeekActivePeriodID == weekActivePeriodId);
                        pointGeofenceNotification.WeekActivePeriod = week;

                        EF.DataContext.Current.AddToPointGeofenceNotificationSet(pointGeofenceNotification);

                        //dataContext1.AttachTo("PointGeofenceNotificationSet", pointGeofenceNotification);

                        foreach (UserLight user in notificationUsers)
                            pointGeofenceNotification.PointGeofenceNotificationUsers.Add(new EF.PointGeofenceNotificationUser() { Recipient = user.Recipient, UserName = user.UserName, TypeId = user.TypeId });

                        EF.PointGeofence geofence = EF.DataContext.Current.PointGeofenceSet.FirstOrDefault(pg => pg.PointId == pointId);
                        pointGeofenceNotification.PointGeofence = geofence;

                        EF.DataContext.Current.SaveChanges();
                }
                catch (Exception ex)
                {
         
                }
 
                return this.GetPointGeofenceNotificationForWebService(pointId);           
        }

       
        //-----------------------------------------------------------------------------------------------------------

        [OperationContract]
        public EF.PointGeofenceNotification UpdatePointGeofenceNotification(int notificationID, string description, bool IsEnabled, bool IsIncoming, bool IsOutgoing, List<UserLight> notificationUsers)
        {
            EF.PointGeofenceNotification pn = null;
            try
            {
                pn = (from r in EF.DataContext.Current.PointGeofenceNotificationSet.Include("PointGeofence").Include("WeekActivePeriod")
                          where r.NotificationId == notificationID
                          select r).FirstOrDefault();

                    pn.Description = description;
                    pn.IsEnabled = IsEnabled;
                    pn.Incoming = IsIncoming;
                    pn.Outgoing = IsOutgoing;
                

                    Guid weekActivePeriodId = pn.WeekActivePeriod.WeekActivePeriodID;
                    //EF.WeekActivePeriod week = dataContext1.WeekActivePeriodSet.First(w => w.WeekActivePeriodID == weekActivePeriodId);
                    //pointGeofenceNotification.WeekActivePeriod = week;

                    pn.PointGeofenceNotificationUsers.Load();

                    for (int i = pn.PointGeofenceNotificationUsers.Count-1; i >= 0; i--)
                        EF.DataContext.Current.DeleteObject(pn.PointGeofenceNotificationUsers.ElementAt(i));

                    foreach (UserLight user in notificationUsers)
                        pn.PointGeofenceNotificationUsers.Add(new EF.PointGeofenceNotificationUser() { Recipient = user.Recipient, UserName = user.UserName, TypeId = user.TypeId });

                    EF.DataContext.Current.SaveChanges();
            }
            catch (Exception ex)
            {

            }

            return pn;
        }

        //-----------------------------------------------------------------------------------------------------------

        [OperationContract]
        public void DeletePointGeofenceNotification(int notificationId)
        {
            try
            {
                //EF.DataContext.Current.AttachTo("PointGeofenceNotificationSet", pointGeofenceNotification);
                EF.PointGeofenceNotification pgn = EF.DataContext.Current.PointGeofenceNotificationSet.FirstOrDefault(p => p.NotificationId == notificationId);
                if (pgn != null)
                    EF.DataContext.Current.DeleteObject(pgn);

                EF.DataContext.Current.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        //-----------------------------------------------------------------------------------------------------------

        private Orchestrator.WebUI.Services.PointGeofence GetGeofenceForWebService(int pointID)
        {
            Orchestrator.Facade.IPoint facPoint = new Orchestrator.Facade.Point();
            Orchestrator.WebUI.Services.PointGeofence pg = new PointGeofence();
            DataSet dsPointGeofence = facPoint.GetPointGeofenceInfo(pointID);

            if (dsPointGeofence.Tables[0].Rows.Count > 0)
            {
                DataRow dr = dsPointGeofence.Tables[0].Rows[0];
                pg.PointID = Convert.ToInt32(dr["PointId"]);
                pg.Description = dr["Description"].ToString();
                pg.Latitude = dr["Latitude"] == DBNull.Value ? 0 : Convert.ToDouble(dr["Latitude"]);
                pg.Longitude = dr["Longitude"] == DBNull.Value ? 0 : Convert.ToDouble(dr["Longitude"]);
                pg.Radius = dr["Radius"] == DBNull.Value ? new int?() : (int)dr["Radius"];

                pg.GeofencePoints = new List<LatLong>();

                SqlGeography geofence = (SqlGeography)dr["Geofence"];
                string points = string.Empty;
                for (int i = 0; i < geofence.STNumPoints(); i++)
                {
                    SqlGeography point = geofence.STPointN(i + 1);
                    LatLong latLon = new LatLong() { Latitude = (double)point.Lat, Longitude = (double)point.Long };
                    pg.GeofencePoints.Add(latLon);
                }
            }

            return pg;
        }

        //-----------------------------------------------------------------------------------------------------------

        [OperationContract]
        public Orchestrator.WebUI.Services.PointGeofence UpdatePointGeofence(Orchestrator.WebUI.Services.PointGeofence geofence, string user)
        {
            Orchestrator.Facade.IPoint facPoint = new Orchestrator.Facade.Point();

            SqlGeography position =  this.BuildGeography(new List<LatLong>() {new LatLong() {Latitude = geofence.Latitude, Longitude=geofence.Longitude}}, OpenGisGeographyType.Point);

            SqlGeography sqlGeofence = null;
            if (geofence.GeofencePoints != null && geofence.GeofencePoints.Count > 0)
            {
                try
                {
                    sqlGeofence = this.BuildGeography(geofence.GeofencePoints, OpenGisGeographyType.Polygon);
                }
                catch (Exception ex)
                {
                    geofence.GeofencePoints.Reverse();
                    sqlGeofence = this.BuildGeography(geofence.GeofencePoints, OpenGisGeographyType.Polygon);
                }
            }

           facPoint.UpdatePointGeofence(geofence.PointID, Convert.ToDecimal(geofence.Latitude), Convert.ToDecimal(geofence.Longitude),position, 
                sqlGeofence,geofence.Radius,user);

            return GetGeofenceForWebService(geofence.PointID);
        }

        //-----------------------------------------------------------------------------------------------------------

        [OperationContract]
        public Orchestrator.WebUI.Services.PointGeofence AddPointGeofence(Orchestrator.WebUI.Services.PointGeofence geofence, string user)
        {
            Orchestrator.Facade.IPoint facPoint = new Orchestrator.Facade.Point();

            SqlGeography position = this.BuildGeography(new List<LatLong>() { new LatLong() { Latitude = geofence.Latitude, Longitude = geofence.Longitude } }, OpenGisGeographyType.Point);
            SqlGeography sqlGeofence = null;
            try
            {
                sqlGeofence = this.BuildGeography(geofence.GeofencePoints, OpenGisGeographyType.Polygon);
            }
            catch (Exception ex)
            {
                geofence.GeofencePoints.Reverse();
                sqlGeofence = this.BuildGeography(geofence.GeofencePoints, OpenGisGeographyType.Polygon);
            }

            facPoint.AddPointGeofence(geofence.PointID, Convert.ToDecimal(geofence.Latitude), Convert.ToDecimal(geofence.Longitude), position,
                sqlGeofence, geofence.Radius, user);

            return GetGeofenceForWebService(geofence.PointID);
        }


        ////-----------------------------------------------------------------------------------------------------------

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

            if(type == OpenGisGeographyType.Polygon)
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

            var invertedGeogrpahy = geog.ReorientObject();
            if(geog.STArea() > invertedGeogrpahy.STArea())
            {
                geog = invertedGeogrpahy;
            }

            return geog;
        }

        //-----------------------------------------------------------------------------------------------------------

        private Orchestrator.WebUI.Services.Point GetPointForWebService(int pointID)
        {
            Orchestrator.Facade.IPoint facPoint = new Orchestrator.Facade.Point();
            Orchestrator.Entities.Point selectedPoint = facPoint.GetPointForPointId(pointID);
            Orchestrator.WebUI.Services.Point p = new Orchestrator.WebUI.Services.Point();

            p.PointID = selectedPoint.PointId;
            p.Description = selectedPoint.Description;
            p.Latitide = (double)selectedPoint.Latitude;
            p.Latitude = (double)selectedPoint.Latitude; // added a correctly spelt version but left the old one to prevent having to change everything.

            p.Longitude = (double)selectedPoint.Longitude;
            p.ClosestTownID = selectedPoint.PostTown.TownId;
            p.ClosestTown = selectedPoint.PostTown.TownName;
            p.IdentityID = selectedPoint.IdentityId;
            p.OrganisationName = selectedPoint.OrganisationName;
            p.PointNotes = selectedPoint.PointNotes;
            p.PointCode = selectedPoint.PointCode;
            p.PhoneNumber = selectedPoint.PhoneNumber;
            p.AddressID = selectedPoint.Address.AddressId;
            p.AddressLine1 = selectedPoint.Address.AddressLine1;
            p.AddressLine2 = selectedPoint.Address.AddressLine2;
            p.AddressLine3 = selectedPoint.Address.AddressLine3;
            p.PostTown = selectedPoint.Address.PostTown;
            p.County = selectedPoint.Address.County;
            p.PostCode = selectedPoint.Address.PostCode;
            p.CountryID = selectedPoint.Address.CountryId;
            p.CountryDescription = selectedPoint.Address.CountryDescription;

            p.GeofencePoints = new List<LatLong>();

            string points = string.Empty;
            for (int i = 0; i < selectedPoint.Geofence.STNumPoints(); i++)
            {
                SqlGeography point = selectedPoint.Geofence.STPointN(i + 1);
                LatLong latLon = new LatLong() { Latitude = (double)point.Lat, Longitude = (double)point.Long };
                p.GeofencePoints.Add(latLon);
            }

            return p;
        }

        //-----------------------------------------------------------------------------------------------------------

        [OperationContract]
        public Orchestrator.WebUI.Services.Point UpdatePoint(Orchestrator.WebUI.Services.Point p, string userId)
        {
            Orchestrator.WebUI.Services.Point returnPoint = null;
            Orchestrator.Facade.IPoint facPoint = new Orchestrator.Facade.Point();
            Orchestrator.Entities.Point selectedPoint = facPoint.GetPointForPointId(p.PointID);
            selectedPoint.Description = p.Description;
            selectedPoint.Latitude = (decimal)p.Latitide;
            selectedPoint.Longitude = (decimal)p.Longitude;
            selectedPoint.PostTown.TownId = p.ClosestTownID;
            selectedPoint.PostTown.TownName = p.ClosestTown;
            selectedPoint.PointNotes = p.PointNotes;
            selectedPoint.PointCode = p.PointCode;
            selectedPoint.PhoneNumber = p.PhoneNumber;
            selectedPoint.Address.AddressLine1 = p.AddressLine1;
            selectedPoint.Address.AddressLine2 = p.AddressLine2;
            selectedPoint.Address.AddressLine3 = p.AddressLine3;
            selectedPoint.Address.PostTown = p.PostTown;
            selectedPoint.Address.County = p.County;
            selectedPoint.Address.PostCode = p.PostCode;
            selectedPoint.Address.CountryId = p.CountryID;
            selectedPoint.Address.CountryDescription = p.CountryDescription;

            Entities.FacadeResult facResult = facPoint.Update(selectedPoint, userId);

            if (facResult.Success)
                returnPoint = this.GetPointForWebService(selectedPoint.PointId);
            else
            {
                foreach (BusinessRuleInfringement i in facResult.Infringements)
                    p.ErrorMeesage += String.Format("{0} : {1}{2}", i.Key, i.Description,Environment.NewLine);

                returnPoint = p;
            }

            return returnPoint;
        }

        //-----------------------------------------------------------------------------------------------------------

        [OperationContract]
        public List<Orchestrator.WebUI.Services.Country> GetCountries(string filter)
        {
            List<Orchestrator.WebUI.Services.Country> countries = new List<Orchestrator.WebUI.Services.Country>();
            try
            {
                Orchestrator.Facade.IReferenceData facRef = new Orchestrator.Facade.ReferenceData();
                DataSet dsCountries = facRef.GetAllCountries();

                foreach (DataRow dr in dsCountries.Tables[0].Rows)
                    countries.Add(new Country() { ID = Convert.ToInt32(dr["CountryId"].ToString()), CountryName = dr["CountryDescription"].ToString() });
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return countries;
        }

        //-----------------------------------------------------------------------------------------------------------

        [OperationContract]
        public List<Orchestrator.WebUI.Services.PostTown> GetTowns(string filter, int maxCount)
        {
            List<Orchestrator.WebUI.Services.PostTown> towns = new List<Orchestrator.WebUI.Services.PostTown>();
            try
            {
                Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
                DataSet ds = facRefData.GetTownForTownName(filter);
                DataTable dt = ds.Tables[0];

                foreach (DataRow dr in dt.Rows)
                    if (towns.Count <= maxCount)
                        towns.Add(new PostTown() { ID = Convert.ToInt32(dr["TownId"].ToString()), TownName = dr["Description"].ToString() });
                    else
                        break;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return towns;
        }

        //-----------------------------------------------------------------------------------------------------------

        [OperationContract]
        public List<Orchestrator.WebUI.Services.PostTown> GetTownsForCountry(string filter, int countryId, int maxCount)
        {
            List<Orchestrator.WebUI.Services.PostTown> towns = new List<Orchestrator.WebUI.Services.PostTown>();
            try
            {
                Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
                DataSet ds = facRefData.GetTownForTownNameAndCountry(filter, countryId); ;
                DataTable dt = ds.Tables[0];

                foreach (DataRow dr in dt.Rows)
                    if (towns.Count == maxCount)
                        break;
                    else
                        towns.Add(new PostTown() { ID = Convert.ToInt32(dr["TownId"].ToString()), TownName = dr["Description"].ToString() });
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return towns;
        }

        //-----------------------------------------------------------------------------------------------------------

    }

    //-----------------------------------------------------------------------------------------------------------

    [DataContract]
    public class WeekActivePeriod
    {
        [DataMember]
        public Guid WeekActivePeriodId { get; set; }
        [DataMember]
        public bool IsDeleted { get; set; }
        [DataMember]
        public int Monday { get; set; }
        [DataMember]
        public int Tuesday { get; set; }
        [DataMember]
        public int Wednesday { get; set; }
        [DataMember]
        public int Thursday { get; set; }
        [DataMember]
        public int Friday { get; set; }
        [DataMember]
        public int Saturday { get; set; }
        [DataMember]
        public int Sunday { get; set; }
        [DataMember]
        public int MondaySecond { get; set; }
        [DataMember]
        public int TuesdaySecond { get; set; }
        [DataMember]
        public int WednesdaySecond { get; set; }
        [DataMember]
        public int ThursdaySecond { get; set; }
        [DataMember]
        public int FridaySecond { get; set; }
        [DataMember]
        public int SaturdaySecond { get; set; }
        [DataMember]
        public int SundaySecond { get; set; }

        public bool IsActiveForTime(DateTime time)
        {

            bool secondHalf = time.Minute >= 30;

            switch (time.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return HourEnabled(secondHalf ? this.MondaySecond : this.Monday, time.Hour);


                case DayOfWeek.Tuesday:
                    return HourEnabled(secondHalf ? this.TuesdaySecond : this.Tuesday, time.Hour);


                case DayOfWeek.Wednesday:
                    return HourEnabled(secondHalf ? this.WednesdaySecond : this.Wednesday, time.Hour);


                case DayOfWeek.Thursday:
                    return HourEnabled(secondHalf ? this.ThursdaySecond : this.Thursday, time.Hour);


                case DayOfWeek.Friday:
                    return HourEnabled(secondHalf ? this.FridaySecond : this.Friday, time.Hour);


                case DayOfWeek.Saturday:
                    return HourEnabled(secondHalf ? this.SaturdaySecond : this.Saturday, time.Hour);

                case DayOfWeek.Sunday:
                    return HourEnabled(secondHalf ? this.SundaySecond : this.Sunday, time.Hour);

            }


            return false;
        }

        private bool HourEnabled(int bitmasked, int hour)
        {
            return ((bitmasked & GetBitForTime(hour)) > 0);
        }

        private int GetBitForTime(int hour)
        {
            //0,1,2,4,8,16,32,64,128....16777216

            return (int)System.Math.Pow((double)2, (double)hour);
        }
    }

    [DataContract]
    public class UserLight
    {
        [DataMember]
        public int? NotificationId { get; set; }
        [DataMember]
        public int? IdentityId { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string Recipient { get; set; }
        [DataMember]
        public int TypeId { get; set; }

        public enum eNotificationContactType { Email = 1, SMS = 2 };

        [DataMember]
        public string ContactType
        {
            get
            {
                return ((eNotificationContactType)this.TypeId).ToString();
            }
        }
    }

    //---------------------------------------------------------------------------------------------------------

    [DataContract]
    public class PointGeofence
    {
        [DataMember]
        public int PointID { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public double Latitude { get; set; }
        [DataMember]
        public double Longitude { get; set; }
        [DataMember]
        public List<LatLong> GeofencePoints { get; set; }
        [DataMember]
        public int? Radius { get; set; }
    }

    //---------------------------------------------------------------------------------------------------------

    [DataContract]
    public class PostTown
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string TownName { get; set; }
    }

    //-----------------------------------------------------------------------------------------------------------

    [DataContract]
    public class Country
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string CountryName { get; set; }
    }

    //-----------------------------------------------------------------------------------------------------------

}

