using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchestrator.WebUI.Services;


namespace Orchestrator.WebUI.NewDashboard
{
    public class WidgetMethods
    {
        public static String secondsToTimePeriodString(int seconds)
        {
            int days = 0;
            int hours = 0;
            int mins = 0;

            days = seconds / 86400;
            hours = (seconds - (days * 86400)) / 3600;
            mins = (seconds - (days * 86400) - (hours * 3600)) / 60;
            return days.ToString() + "d " + hours.ToString() + "h " + mins.ToString() + "m";
        }

        public static double secondsToHours(int seconds)
        {
            return seconds / 3600;
        }
    }
}