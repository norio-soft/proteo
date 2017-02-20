using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;

using System.Data;

namespace Orchestrator.WebUI.GPS
{
    public partial class GetCurrentLocation : System.Web.UI.Page
    {

        public string HereMapsCoreJS
        {
            get { return Properties.Settings.Default.HereMapsJSCoreUrl; }
        }

        public string HereMapsServiceJS
        {
            get { return Properties.Settings.Default.HereMapsJSServiceUrl; }
        }

        public string HereMapsEventsJS
        {
            get { return Properties.Settings.Default.HereMapsJSMapEventsUrl; }
        }

        public string HereMapsUIJS
        {
            get { return Properties.Settings.Default.HereMapsJSUIUrl; }
        }

        public string HereMapsUICSS
        {
            get { return Properties.Settings.Default.HereMapsJSUICSSUrl; }
        }


        public string HereMapsApplicationCode
        {
            get { return Properties.Settings.Default.HereMapsApplicationCode; }
        }

        public string HereMapsApplicationId
        {
            get { return Properties.Settings.Default.HereMapsApplicationId; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
   
        [WebMethod()]
        public static InfoBoxData GetGPSPosition(string unitID)
        {
           
            if (string.IsNullOrEmpty(unitID))
                return null;
            Facade.IResource facResource = new Facade.Resource();
            DataSet dsPosition = facResource.GetCurrentGPSPosition(unitID);

            InfoBoxData retVal = new InfoBoxData();

            if (dsPosition.Tables[0].Rows.Count == 0)
                return retVal;

            DataRow row = dsPosition.Tables[0].Rows[0];

            #region get the Latitude and the Longitude
            
            decimal latitude = 0.0M;
            if (row["Latitude"] != DBNull.Value)
                latitude = decimal.Parse(row["Latitude"].ToString());
            else
            {

                decimal latitude_degree = decimal.Parse(row["Latitude_Degree"].ToString());
                decimal latitude_minute = decimal.Parse(row["Latitude_Minute"].ToString());
                decimal latitude_second = decimal.Parse(row["Latitude_Second"].ToString());
                latitude = latitude_degree + (latitude_minute / 60) + (latitude_second / 3600);


                if (row["Latitude_Indicator"].ToString() == "S")
                    latitude = -latitude;
            }
            retVal.Latitude = latitude;


            decimal longitude = 0.0M;

            if (row["Longitude"] != DBNull.Value)
                longitude = decimal.Parse(row["Longitude"].ToString());
            else
            {
                
                decimal longitude_degree = decimal.Parse(row["Longitude_Degree"].ToString());
                decimal longitude_minute = decimal.Parse(row["Longitude_Minute"].ToString());
                decimal longitude_second = decimal.Parse(row["Longitude_Second"].ToString());
                longitude = longitude_degree + (longitude_minute / 60) + (longitude_second / 3600);

                if (row["Longitude_Indicator"].ToString() == "W")
                    longitude = -longitude;
            }
            retVal.Longitude = longitude;

            #endregion

            #region Set the Resource Ref
            retVal.Title = row["ResourceRef"].ToString();
            retVal.Gazetteer = row["Gazetteer"].ToString();
            retVal.Reason = row["Reason"].ToString();
            
            DateTime gpsDate = Convert.ToDateTime(row["DateStamp"].ToString());
            retVal.dateStamp = gpsDate.ToShortDateString() + " " + gpsDate.ToShortTimeString();
            
            #endregion
            
            return retVal;
        }


        public class InfoBoxData
        {
            public string Title { get; set; }
            public decimal Latitude { get; set; }
            public decimal Longitude { get; set; }
            public string Gazetteer { get; set; }
            public string Reason { get; set; }
            public int? GeofenceRadius { get; set; }
            public string dateStamp {get; set;}
        }


    }
}
