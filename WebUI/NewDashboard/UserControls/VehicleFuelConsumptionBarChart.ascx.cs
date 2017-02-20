using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Orchestrator.WebUI.Services;

namespace Orchestrator.WebUI.NewDashboard.UserControls
{
    public partial class VehicleFuelConsumptionBarChart : System.Web.UI.UserControl
    {
        public List<FuelConsumption> fuelConsumptionList;
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

                CanDashboardServices canDasboardServices = new CanDashboardServices();
                fuelConsumptionList = canDasboardServices.GetVehicleFuelConsumption(timePeriod.StartDate, timePeriod.EndDate, null);
                fuelConsumptionList.Sort(delegate(FuelConsumption fc2, FuelConsumption fc1) { return fc2.MPG.CompareTo(fc1.MPG); });

            }
        }
    }
}