﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Orchestrator.WebUI.UserControls.WebParts.FleetMetrik
{
    public partial class wpDriverBrakingGraph : System.Web.UI.UserControl, IWebPart
    {

        #region Public Properties
        public string DataUrl { get; set; }
        public string ReportUrl { get; set; }
        public TimePeriods timePeriod { get; set; }

        public string Title { get; set; }

        public string CatalogIconImageUrl { get; set; }
        public string Description { get; set; }
        public string Subtitle { get; set; }
        public string TitleIconImageUrl { get; set; }
        public string TitleUrl { get; set; }
        #endregion

        public wpDriverBrakingGraph()
        {
            timePeriod = new TimePeriods();
            timePeriod.SetWeekDays();
            this.Title = "Worst Driver Braking - " + timePeriod.StartDate + " to " + timePeriod.EndDate;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.DataUrl = string.Format("{0}/api/driver/driverbraking?dateFrom={1:s}&dateTo={2:s}",
            ConfigurationManager.AppSettings["ApiBaseUrl"],
            timePeriod.StartDate,
            timePeriod.EndDate);

            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
            this.ReportUrl = string.Format("{0}/Reports/ReportViewer2.aspx?rn=FleetMetrik.DriverOverview&StartDate={1:s}&EndDate={2:s}&DriverIds=", baseUrl, timePeriod.StartDate, timePeriod.EndDate);
        }
    }
}