using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchestrator.WebUI.UserControls.WebParts.FleetMetrik
{
    public class TimePeriods
    {
        public DateTime StartDate {get; set;}


        public DateTime EndDate { get; set; }


        public void SetWeekDays()
        {
           
            StartDate = DateTime.Now.AddDays(-(((int)DateTime.Now.DayOfWeek)) - (int)DayOfWeek.Sunday).Date;
            StartDate = StartDate.Add(new TimeSpan(6, 0, 0));
            EndDate = StartDate.AddDays(7).Date;
            EndDate = EndDate.Add(new TimeSpan(5, 59, 59));
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
