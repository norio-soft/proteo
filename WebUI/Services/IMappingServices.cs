using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Orchestrator.WebUI.ws;
using System.ServiceModel.Activation;
using System.ComponentModel.DataAnnotations;


namespace Orchestrator.WebUI.Services
{
    // NOTE: If you change the interface name "IMappingServices" here, you must also update the reference to "IMappingServices" in Web.config.
    [ServiceContract]
    public interface IMappingServices
    {
        [OperationContract]
        List<Orchestrator.WebUI.ws.MappingServices.RunLeg> GetLegsForRun(int runID, bool includeDataPoints);

        [OperationContract]
        List<Orchestrator.WebUI.ws.MappingServices.EmptyRunningLeg> GetEmptyRunningLegsForRun(int runID);

        [OperationContract]
        List<Orchestrator.WebUI.ws.MappingServices.DataPoint> GetPositionHistoryForRunLeg(Orchestrator.WebUI.ws.MappingServices.RunLeg runLeg);

        [OperationContract]
        List<Orchestrator.WebUI.ws.MappingServices.DataPoint> GetPositionHistoryForResourceId(int ResourceId, DateTime startDate, DateTime endDate);

        [OperationContract]
        List<Orchestrator.WebUI.ws.MappingServices.DataPoint> GetPositionHistory(string gpsUnitID, DateTime startDate, DateTime endDate);

        [OperationContract]
        Orchestrator.WebUI.ws.MappingServices.DataPoint GetCurrentGPSPosition(int resourceId);

        [OperationContract]
        Orchestrator.WebUI.ws.MappingServices.Point GetPoint(int pointID);

        [OperationContract]
        List<Point> GetPointsPenetrated(List<LatLong> latLongs);

    }

    [DataContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class FleetViewTreeViewNode
    {
        [DataMember]
        public int Id;

        [DataMember]
        public int ParentId;

        [DataMember]
        public string Description;

        [DataMember]
        public DataNodeType NodeType;

        public override string ToString()
        {
            return Description;
        }

        public enum DataNodeType
        {
            OrgUnit,
            Vehicle,
            Driver
        }

        [DataMember]
        public string GpsUnitId;

        [DataMember]
        public List<FleetViewTreeViewNode> Children;
    }

    [DataContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class LatLong
    {
        [DataMember]
        public double Latitude;
        [DataMember]
        public double Longitude;
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
        public double Latitude { get; set; }

        [DataMember]
        public double Longitude { get; set; }
        [DataMember]
        public List<LatLong> GeofencePoints { get; set; }
        [DataMember]
        public int Radius { get; set; }
        [DataMember]
        public int HasActiveAlert { get; set; }
        [DataMember]
        public int ClosestTownID{ get; set;}
        [DataMember]
        public String ClosestTown{ get;set;}
        [DataMember]
        public int IdentityID {get;set;}
        [DataMember]
        public string OrganisationName{get;set;}
        [DataMember]
        public string PointNotes{ get;set;}
        [DataMember]
        public string PointCode {get;set;}
        [DataMember]
        public string PhoneNumber { get; set; }
        [DataMember]
        public int AddressID
        {
            get;
            set;
        }
        [DataMember]
        public string AddressLine1
        {
            get;
            set;
        }
        [DataMember]
        public string AddressLine2
        {
            get;
            set;
        }
        [DataMember]
        public string AddressLine3
        {
            get;
            set;
        }
        [DataMember]
        public string PostTown
        {
            get;
            set;
        }
        [DataMember]
        public string County
        {
            get;
            set;
        }
        [DataMember]
        public string PostCode
        {
            get;
            set;
        }
        [DataMember]
        public int CountryID
        {
            get;
            set;
        }
        [DataMember]
        public string CountryDescription
        {
            get;
            set;
        }
        [DataMember]
        public string ErrorMeesage
        {
            get;
            set;
        }
    }

    [DataContract]
    public class VehicleCacheItem
    {
        [DataMember]
        public string GpsUnitId { get; set; }
        [DataMember]
        public string Reg { get; set; }
    }

    [DataContract]
    public class DriverCacheItem
    {
        [DataMember]
        public string TachoId { get; set; }
        [DataMember]
        public string DriverName { get; set; }
    }

    [DataContract]
    public class VehicleNearLocationItem
    {
        [DataMember]
        public string GPSUnitId { get; set; }

        [DataMember]
        public string ResourceRef { get; set; }

        [DataMember]
        public int GPSPositionHistoryId { get; set; }

        [DataMember]
        public double DistanceFromPoint { get; set; }

        [DataMember]
        public DateTime LastResponseTime { get; set; }

        [DataMember]
        public double Latitude { get; set; }

        [DataMember]
        public double Longitude { get; set; }
    }
}
