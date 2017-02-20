using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.UI.WebControls.WebParts;

namespace Orchestrator.WebUI.WebParts
{

    public partial class wpOrdersByCreator : System.Web.UI.UserControl, IWebPart
    {

        public enum eDisplayOption { Last_30_Days, This_Week, This_Month, This_Year };

        public wpOrdersByCreator()
        {
            this.Title = "Orders By Creator";
            this.Display = eDisplayOption.This_Week;
        }

        #region Public Properties

        [Personalizable(PersonalizationScope.User), WebBrowsable(true)]
        public eDisplayOption Display { get; set; }

        public string CatalogIconImageUrl { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string TitleIconImageUrl { get; set; }
        public string TitleUrl { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string DataUrl { get; set; }

        #endregion Public Properties

        protected override void OnPreRender(EventArgs e)
        {
            switch (this.Display)
            {
                case eDisplayOption.This_Week:
                    SetWeekDays();
                    break;
                case eDisplayOption.This_Month:
                    SetMonthDays();
                    break;
                case eDisplayOption.This_Year:
                    StartDate = new DateTime(DateTime.Today.Year, 01, 01, 00, 00, 00);
                    EndDate = new DateTime(DateTime.Today.Year, 12, 31, 23, 59, 59);
                    break;
                case eDisplayOption.Last_30_Days:
                    SetLast30Days();
                    break;
            }

            this.DataUrl = string.Format(
                "{0}/api/order/statisticsbycreator?dateFrom={1:s}&dateTo={2:s}",
                ConfigurationManager.AppSettings["ApiBaseUrl"],
                StartDate,
                EndDate);
        }

        private void SetWeekDays()
        {
            var currentDate = DateTime.Today;

            while (currentDate.DayOfWeek != DayOfWeek.Monday)
            {
                currentDate = currentDate.AddDays(-1);
            }

            StartDate = currentDate;
            EndDate = currentDate.AddDays(6);
        }

        private void SetMonthDays()
        {
            StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01);
            EndDate = new DateTime(StartDate.Year, StartDate.Month, new System.Globalization.GregorianCalendar().GetDaysInMonth(StartDate.Year, StartDate.Month));
        }

        private void SetLast30Days()
        {
            StartDate = DateTime.Today.AddDays(-30);
            EndDate = DateTime.Today;
        }

    }

}
