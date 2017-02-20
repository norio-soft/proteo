using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Collections.Generic;
using Orchestrator;
using System.Data;
using System.Data.SqlClient;

//Please note that this is used for the FM Dashboard project which has now become the FleetMetrik Solution
namespace Orchestrator.WebUI.Services
{
    #region DTO
    public class IdlingTime
    {
        public int? Id {get; set;}
        public string Name {get; set;}
        public int IdlingSeconds { get; set; }
        public int DurationSeconds { get; set; }
        public decimal Percentage { get; set; }
    }

    public class FuelConsumption
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public double FuelLitres { get; set; }
        public int DistanceMetres { get; set; }
        public double MPG { get; set; }
    }   

    public class InfringementCount
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
    }

    public class FuelPrice
    {
        public DateTime StartOfWeek { get; set; }
        public decimal Price { get; set; }
        public DateTime CreateDateTime { get; set; }
    }

    public class TripSummary
    {
        public string GPSUnitID { get; set; }
        public DateTime Date { get; set; }
        public int Trips { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public double Miles { get; set; }
        public string TachoID { get; set; }
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
    }

    public class Driver
    {
        public int ResourceID { get; set; }
        public int IdentityID { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", this.FirstName, this.LastName);
            }
        }
        public string TachoCardID { get; set; }

        public string HomePhone { get; set; }
        public string MobilePhone { get; set; }
        public string PersonalMobile { get; set; }

        public Address Address { get; set; }

        public string CurrentLocation { get; set; }
        public string UsualVehicleRegistration { get; set; }

        public bool IsAvaialable { get; set; }
        public bool IsAgencyDriver{ get; set; }

        public Driver() { }
        public Driver(DataRow driverRow)
        {
            this.Address = new Address();
            this.IdentityID = (int)driverRow["IdentityId"];
            this.ResourceID = (int)driverRow["ResourceId"];
            this.FirstName = (string)driverRow["FirstNames"];
            this.LastName = (string)driverRow["LastName"];
            this.HomePhone = driverRow["HomePhone"] == DBNull.Value ? string.Empty : (string)driverRow["HomePhone"];
            this.MobilePhone= driverRow["MobilePhone"] == DBNull.Value ? string.Empty : (string)driverRow["MobilePhone"];
            this.PersonalMobile = driverRow["PersonalMobile"] ==DBNull.Value ? string.Empty :  (string)driverRow["PersonalMobile"];
            this.Address.AddressLine1 = (string)driverRow["AddressLine1"];
            this.Address.PostCode = (string)driverRow["PostCode"];
            this.UsualVehicleRegistration = driverRow["RegNo"] == DBNull.Value ? string.Empty : (string)driverRow["RegNo"];
            this.TachoCardID = driverRow["DigitaltachoCardID"] == DBNull.Value ? string.Empty : (string)driverRow["DigitaltachoCardID"];
            this.CurrentLocation = driverRow["CurrentLocation"] == DBNull.Value ? string.Empty : (string)driverRow["CurrentLocation"];
        }

    }

    public class Address
    {
        public int AddressID { get; set; }
        public int CountryID { get; set; }

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string PostTown { get; set; }
        public string County { get; set; }
        public string PostCode { get; set; }

    }

    public class Vehicle
    {
        public int ResourceID { get; set; }
        public int ManufacturerID { get; set; }
        public int VehicleClassID { get; set; }

        public string GPSUnitID { get; set; }
        public string RegistrationNo { get; set; }
        public string Manufacturer { get; set; }
        public string VehicleClass { get; set; }
        public string RegularDriver { get; set; }
        public string PhoneNumber { get; set; }
        
        public string CurrentLocation { get; set; }
        public DateTime GPSTimeStamp { get; set; }
        public string GPSReason { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public DateTime MOTExpiryDate { get; set; }

        public Vehicle() { }
        public Vehicle(DataRow vehicleRow)
        {
            this.Manufacturer = (string)vehicleRow["VehicleManufacturer"];
            this.RegistrationNo = (string)vehicleRow["RegNo"];
            this.GPSUnitID = vehicleRow["GPSUnitID"] == DBNull.Value ? string.Empty : (string)vehicleRow["GPSUnitID"];
            
            //this.PhoneNumber = vehicleRow["TelephoneNumber"] == DBNull.Value ? string.Empty : (string)vehicleRow["TelephoneNumber"];
            this.CurrentLocation = vehicleRow["Gazetteer"] == DBNull.Value ? string.Empty : (string)vehicleRow["Gazetteer"];
            this.GPSReason = vehicleRow["Reason"] == DBNull.Value ? string.Empty : (string)vehicleRow["Reason"];
            this.GPSTimeStamp = vehicleRow["DateStamp"] == DBNull.Value ? DateTime.MinValue : (DateTime)vehicleRow["DateStamp"];
            this.Latitude = vehicleRow["Latitude"] == DBNull.Value ? -1 : (double)vehicleRow["Latitude"];
            this.Longitude = vehicleRow["Longitude"] == DBNull.Value ? -1 : (double)vehicleRow["Longitude"];
            
        }
       

    }

    #endregion


    [ServiceContract(Namespace = "www.orchestrator.co.uk/CanDashboardServices")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class CanDashboardServices
    {
        //[OperationContract]
        //public void GetFleetIdlingTime(DateTime FromDate, DateTime ToDate, ref int IdlingSeconds, ref int DurationSeconds)
        //{
        //    IdlingSeconds = 0;
        //    DurationSeconds = 0;

        //    EF.DataContext.Current.CAN_GetFleetIdlingTime(FromDate, ToDate, out IdlingSeconds, out DurationSeconds);

        //}

        #region Dashboard Service Methods

        [OperationContract]
        public List<IdlingTime> GetDriverIdlingTime(DateTime FromDate, DateTime ToDate, int? OrgUnitId)
        {
            SqlConnection conn = null;
            SqlDataReader reader = null;
            List<IdlingTime> idlingTimes = new List<IdlingTime>();

            try
            {
                conn = new SqlConnection(Globals.Configuration.ConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("spCAN_GetDriverIdlingTimes", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("FromDate", FromDate);
                cmd.Parameters.AddWithValue("ToDate", ToDate);
                cmd.Parameters.AddWithValue("OrgUnitId", OrgUnitId ?? (object)DBNull.Value);
                reader = cmd.ExecuteReader();
                
                while (reader.Read())
                {
                    IdlingTime idlingTime = new IdlingTime()
                    {
                        Id = reader.GetValue(0) == DBNull.Value ? null : (int?)reader.GetValue(0),
                        Name = reader.GetValue(1) == DBNull.Value ? string.Empty : (string)reader.GetValue(1),
                        IdlingSeconds = reader.GetInt32(2),
                        DurationSeconds = reader.GetInt32(3),
                        Percentage = reader.GetDecimal(4)
                    };
                    idlingTimes.Add(idlingTime);
                }
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (conn != null)
                    conn.Close();
            }

            return idlingTimes;
        }

        [OperationContract]
        public List<IdlingTime> GetVehicleIdlingTime(DateTime FromDate, DateTime ToDate, int? OrgUnitId)
        {
            SqlConnection conn = null;
            SqlDataReader reader = null;
            List<IdlingTime> idlingTimes = new List<IdlingTime>();

            try
            {
                conn = new SqlConnection(Globals.Configuration.ConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("spCAN_GetVehicleIdlingTimes", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("FromDate", FromDate);
                cmd.Parameters.AddWithValue("ToDate", ToDate);
                cmd.Parameters.AddWithValue("OrgUnitId", OrgUnitId ?? (object)DBNull.Value);
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    IdlingTime idlingTime = new IdlingTime()
                    {
                        Id = reader.GetValue(0) == DBNull.Value ? null : (int?)reader.GetValue(0),
                        Name = reader.GetValue(1) == DBNull.Value ? string.Empty : (string)reader.GetValue(1),
                        IdlingSeconds = reader.GetInt32(2),
                        DurationSeconds = reader.GetInt32(3),
                        Percentage = reader.GetDecimal(4)
                    };
                    idlingTimes.Add(idlingTime);
                }
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (conn != null)
                    conn.Close();
            }

            return idlingTimes;
        }


        [OperationContract]
        public List<FuelConsumption> GetVehicleFuelConsumption(DateTime FromDate, DateTime ToDate, int? OrgUnitId)
        {
            SqlConnection conn = null;
            SqlDataReader reader = null;
            List<FuelConsumption> fuelConsumptions = new List<FuelConsumption>();

            try
            {
                conn = new SqlConnection(Globals.Configuration.ConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("spCAN_GetVehicleFuelConsumptions", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("FromDate", FromDate);
                cmd.Parameters.AddWithValue("ToDate", ToDate);
                cmd.Parameters.AddWithValue("OrgUnitId", OrgUnitId ?? (object)DBNull.Value);
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    FuelConsumption fuelConsumption = new FuelConsumption()
                    {
                        Id = reader.GetValue(0) == DBNull.Value ? null : (int?)reader.GetValue(0),
                        Name = reader.GetValue(1) == DBNull.Value ? string.Empty : (string)reader.GetValue(1),
                        FuelLitres= (double)reader.GetDecimal(2),
                        DistanceMetres = reader.GetInt32(3),
                        MPG = reader.GetDouble(4)
                    };
                    fuelConsumptions.Add(fuelConsumption);
                }
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (conn != null)
                    conn.Close();
            }

            return fuelConsumptions;
        }

        [OperationContract]
        public List<FuelConsumption> GetDriverFuelConsumption(DateTime FromDate, DateTime ToDate, int? OrgUnitId)
        {
            SqlConnection conn = null;
            SqlDataReader reader = null;
            List<FuelConsumption> fuelConsumptions = new List<FuelConsumption>();

            try
            {
                conn = new SqlConnection(Globals.Configuration.ConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("spCAN_GetDriverFuelConsumptions", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("FromDate", FromDate);
                cmd.Parameters.AddWithValue("ToDate", ToDate);
                cmd.Parameters.AddWithValue("OrgUnitId", OrgUnitId ?? (object)DBNull.Value);
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    FuelConsumption fuelConsumption = new FuelConsumption()
                    {
                        Id = reader.GetValue(0) == DBNull.Value ? null : (int?)reader.GetValue(0),
                        Name = reader.GetValue(1) == DBNull.Value ? string.Empty : (string)reader.GetValue(1),
                        FuelLitres = (double)reader.GetDecimal(2),
                        DistanceMetres = reader.GetInt32(3),
                        MPG = reader.GetDouble(4)
                    };
                    fuelConsumptions.Add(fuelConsumption);
                }
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (conn != null)
                    conn.Close();
            }

            return fuelConsumptions;
        }

        [OperationContract]
        public List<InfringementCount> GetDriverSpeeding(DateTime FromDate, DateTime ToDate, int? OrgUnitId)
        {
            SqlConnection conn = null;
            SqlDataReader reader = null;
            List<InfringementCount> infringementCounts = new List<InfringementCount>();

            try
            {
                conn = new SqlConnection(Globals.Configuration.ConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("spCAN_GetDriverSpeedingCounts", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("FromDate", FromDate);
                cmd.Parameters.AddWithValue("ToDate", ToDate);
                cmd.Parameters.AddWithValue("OrgUnitId", OrgUnitId ?? (object)DBNull.Value);
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    InfringementCount infringementCount = new InfringementCount()
                    {
                        Id = reader.GetValue(0) == DBNull.Value ? null : (int?)reader.GetValue(0),
                        Name = reader.GetValue(1) == DBNull.Value ? string.Empty : (string)reader.GetValue(1),
                        Count = reader.GetInt32(2)
                    };
                    infringementCounts.Add(infringementCount);
                }
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (conn != null)
                    conn.Close();
            }

            return infringementCounts;
        }

        [OperationContract]
        public List<InfringementCount> GetDriverOverReving(DateTime FromDate, DateTime ToDate, int? OrgUnitId)
        {
            SqlConnection conn = null;
            SqlDataReader reader = null;
            List<InfringementCount> infringementCounts = new List<InfringementCount>();

            try
            {
                conn = new SqlConnection(Globals.Configuration.ConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("spCAN_GetDriverOverRevingCounts", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("FromDate", FromDate);
                cmd.Parameters.AddWithValue("ToDate", ToDate);
                cmd.Parameters.AddWithValue("OrgUnitId", OrgUnitId ?? (object)DBNull.Value);
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    InfringementCount infringementCount = new InfringementCount()
                    {
                        Id = reader.GetValue(0) == DBNull.Value ? null : (int?)reader.GetValue(0),
                        Name = reader.GetValue(1) == DBNull.Value ? string.Empty : (string)reader.GetValue(1),
                        Count = reader.GetInt32(2)
                    };
                    infringementCounts.Add(infringementCount);
                }
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (conn != null)
                    conn.Close();
            }

            return infringementCounts;
        }
        [OperationContract]
        public List<InfringementCount> GetDriverBraking(DateTime FromDate, DateTime ToDate, int? OrgUnitId)
        {
            SqlConnection conn = null;
            SqlDataReader reader = null;
            List<InfringementCount> infringementCounts = new List<InfringementCount>();

            try
            {
                conn = new SqlConnection(Globals.Configuration.ConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("spCAN_GetDriverBrakingCounts", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("FromDate", FromDate);
                cmd.Parameters.AddWithValue("ToDate", ToDate);
                cmd.Parameters.AddWithValue("OrgUnitId", OrgUnitId ?? (object)DBNull.Value);
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    InfringementCount infringementCount = new InfringementCount()
                    {
                        Id = reader.GetValue(0) == DBNull.Value ? null : (int?)reader.GetValue(0),
                        Name = reader.GetValue(1) == DBNull.Value ? string.Empty : (string)reader.GetValue(1),
                        Count = reader.GetInt32(2)
                    };
                    infringementCounts.Add(infringementCount);
                }
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (conn != null)
                    conn.Close();
            }
            
            return infringementCounts;
        }

        [OperationContract]
        public List<InfringementCount> GetVehicleSpeeding(DateTime FromDate, DateTime ToDate, int? OrgUnitId)
        {
            SqlConnection conn = null;
            SqlDataReader reader = null;
            List<InfringementCount> infringementCounts = new List<InfringementCount>();

            try
            {
                conn = new SqlConnection(Globals.Configuration.ConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("spCAN_GetVehicleSpeedingCounts", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("FromDate", FromDate);
                cmd.Parameters.AddWithValue("ToDate", ToDate);
                cmd.Parameters.AddWithValue("OrgUnitId", OrgUnitId ?? (object)DBNull.Value);
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    InfringementCount infringementCount = new InfringementCount()
                    {
                        Id = reader.GetValue(0) == DBNull.Value ? null : (int?)reader.GetValue(0),
                        Name = reader.GetValue(1) == DBNull.Value ? string.Empty : (string)reader.GetValue(1),
                        Count = reader.GetInt32(2)
                    };
                    infringementCounts.Add(infringementCount);
                }
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (conn != null)
                    conn.Close();
            }

            return infringementCounts;
        }


        [OperationContract]
        public List<InfringementCount> GetVehicleBraking(DateTime FromDate, DateTime ToDate, int? OrgUnitId)
        {
            SqlConnection conn = null;
            SqlDataReader reader = null;
            List<InfringementCount> infringementCounts = new List<InfringementCount>();

            try
            {
                conn = new SqlConnection(Globals.Configuration.ConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("spCAN_GetVehicleBrakingCounts", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("FromDate", FromDate);
                cmd.Parameters.AddWithValue("ToDate", ToDate);
                cmd.Parameters.AddWithValue("OrgUnitId", OrgUnitId ?? (object)DBNull.Value);
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    InfringementCount infringementCount = new InfringementCount()
                    {
                        Id = reader.GetValue(0) == DBNull.Value ? null : (int?)reader.GetValue(0),
                        Name = reader.GetValue(1) == DBNull.Value ? string.Empty : (string)reader.GetValue(1),
                        Count = reader.GetInt32(2)
                    };
                    infringementCounts.Add(infringementCount);
                }
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (conn != null)
                    conn.Close();
            }

            return infringementCounts;
        }


        [OperationContract]
        public List<EF.CANBenchmark> GetCanBenchMarks()
        {
            var ctx = EF.DataContext.Current;

            var benchmarks = from b in EF.DataContext.Current.CANBenchmarks
                             where b.IsEnabled == true
                             select b;

            return benchmarks.ToList();
                                
        }


        [OperationContract]
        public FuelPrice GetCurrentFuelPrice(DateTime date)
        {
            SqlConnection conn = null;
            SqlDataReader reader = null;
            FuelPrice price = new FuelPrice();

            try
            {
                conn = new SqlConnection(Globals.Configuration.ConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("spFuelPrice_GetForDate", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("Date", date);
               
                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    price = new FuelPrice()
                    {
                        StartOfWeek = reader.GetValue(0) == DBNull.Value ? DateTime.MinValue : reader.GetDateTime(0),
                        Price = reader.GetValue(1) == DBNull.Value ? 0.00M : reader.GetDecimal(1),
                        CreateDateTime = reader.GetValue(2) == DBNull.Value ? DateTime.MinValue : reader.GetDateTime(2)
                    };
                    
                }
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (conn != null)
                    conn.Close();
            }

            return price;
        }

        [OperationContract]
        public List<TripSummary> GetTripSummary(string GPSUnitID, DateTime FromDate, DateTime ToDate)
        {
            SqlConnection conn = null;
            SqlDataReader reader = null;
            List<TripSummary> trips = new List<TripSummary>();

            try
            {
                conn = new SqlConnection(Globals.Configuration.ConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("spFM_GetTripSummaryForDevice", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("GPSUnitID", GPSUnitID);
                cmd.Parameters.AddWithValue("DateFrom", FromDate);
                cmd.Parameters.AddWithValue("DateTo", ToDate);
                reader = cmd.ExecuteReader();

                
                while (reader.Read())
                {
                    TripSummary trip= new TripSummary()
                    {
                        GPSUnitID = reader.GetString(0),
                        Date = reader.GetDateTime(1),
                        Trips = reader.GetInt32(2),
                        StartTime = reader.GetDateTime(3),
                        EndTime = reader.GetDateTime(4),
                        Miles = (double)reader.GetDecimal(5),
                        TachoID = reader.GetString(7)
                    };
                    trips.Add(trip);
                }
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (conn != null)
                    conn.Close();
            }

            return trips;
        }

        [OperationContract]
        public List<TripSummary> GetTripDetails(string GPSUnitID, DateTime FromDate, DateTime ToDate)
        {
            SqlConnection conn = null;
            SqlDataReader reader = null;
            List<TripSummary> trips = new List<TripSummary>();

            try
            {
                conn = new SqlConnection(Globals.Configuration.ConnectionString);
                conn.Open();
                SqlCommand cmd = new SqlCommand("spFM_GetTripDetailsForDevice", conn);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("GPSUnitID", GPSUnitID);
                cmd.Parameters.AddWithValue("DateFrom", FromDate);
                cmd.Parameters.AddWithValue("DateTo", ToDate);
                reader = cmd.ExecuteReader();


                while (reader.Read())
                {
                    TripSummary trip = new TripSummary()
                    {
                        GPSUnitID = reader.GetString(0),
                        Date = reader.GetDateTime(1),
                        StartTime = reader.GetDateTime(2),
                        EndTime = reader.GetDateTime(3),
                        Miles = (double)reader.GetDecimal(4),
                        TachoID = reader.GetString(6),
                        StartLocation = reader.GetString(7),
                        EndLocation = reader.GetString(8)
                    };
                    trips.Add(trip);
                }
            }
            finally
            {
                if (reader != null)
                    reader.Close();
                if (conn != null)
                    conn.Close();
            }

            return trips;
        }

        [OperationContract]
        public bool HasTachoLink()
        {
            return Orchestrator.Globals.Configuration.HasTachoLink;
        }
        #endregion

        #region Resource Service Methods
        [OperationContract]
        public List<Driver> GetDriverData()
        {
            Facade.IDriver facResource = new Facade.Resource();
            DataSet dsDrivers = null;

            // get all of the drivers and convert to a Driver Resource
            dsDrivers = facResource.GetForNames(string.Empty, string.Empty);
            List<Driver> drivers = new List<Driver>();
            foreach(DataRow row in dsDrivers.Tables[0].Rows)
                drivers.Add(new Driver(row));
            
            return drivers;
        }

        [OperationContract]
        public List<Vehicle> GetVehicleData()
        {
            Facade.IVehicle facResource = new Facade.Resource();
            DataSet dsVehicles= null;

            // get all of the drivers and convert to a Driver Resource
            dsVehicles = facResource.GetAllWithGPS();
            List<Vehicle> vehicles = new List<Vehicle>();
            foreach (DataRow row in dsVehicles.Tables[0].Rows)
                vehicles.Add(new Vehicle(row));

            return vehicles;
        }
        #endregion

    }
}
