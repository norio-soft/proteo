using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Orchestrator.WebUI.NewDashboard
{
    public class TimePeriods
    {
        private DateTime startDate;

        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; }
        }
        private DateTime endDate;

        public DateTime EndDate
        {
            get { return endDate; }
            set { endDate = value; }
        }

        public void SetWeekDays()
        {
            DateTime currentDate = DateTime.Today;

            while (currentDate.DayOfWeek != DayOfWeek.Monday)
            {
                currentDate = currentDate.AddDays(-1);
            }

            StartDate = currentDate;
            EndDate = currentDate.AddDays(6);
        }

        public void SetMonthDays()
        {
            StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01);
            EndDate = new DateTime(StartDate.Year, StartDate.Month, new System.Globalization.GregorianCalendar().GetDaysInMonth(StartDate.Year, StartDate.Month));
        }

        public void SetLast30Days()
        {
            StartDate = DateTime.Today.AddDays(-30);
            EndDate = DateTime.Today;
        }

        public void SetYearDays()
        {
            StartDate = new DateTime(DateTime.Today.Year, 01, 01, 00, 00, 00);
            EndDate = new DateTime(DateTime.Today.Year, 12, 31, 23, 59, 59);
        }
    }
}