using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Orchestrator.WebUI.Services;
using Orchestrator.WebUI.NewDashboard;

namespace Orchestrator.WebUI.NewDashboard.UserControls
{
    public partial class FleetIdlingGauge : System.Web.UI.UserControl
    {
        public int fleetIdlingTime;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Attributes["mode"] != null)
            {
                int mode = int.Parse(this.Attributes["mode"]);
                TimePeriods timePeriod = new TimePeriods();
                switch (mode)
                {
                    case 1:
                        timePeriod.SetLast30Days();
                        break;
                    case 2:
                        timePeriod.SetWeekDays();
                        break;
                    case 3:
                        timePeriod.SetMonthDays();
                        break;
                    case 4:
                        timePeriod.SetYearDays();
                        break;
                }

                fleetIdlingTime = DashboardDataAccess.getFleetIdling(timePeriod.StartDate, timePeriod.EndDate);
                if (fleetIdlingTime > 20)
                    fleetIdlingTime = 20;

            }


        }
    }
}