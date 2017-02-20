using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Orchestrator.WebUI.Services;

namespace Orchestrator.WebUI.NewDashboard.UserControls
{
    public partial class FleetFuelConsumptionGuage : System.Web.UI.UserControl
    {
        public List<FuelConsumption> fuelConsumptionList;
        public double fleetFuelConsumption;
        public double fleetFuelLitres;
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
                double mpgTotal = 0;
                double litres = 0;
                foreach (FuelConsumption fuelConsumption in fuelConsumptionList)
                {
                    mpgTotal += fuelConsumption.MPG;
                    litres += fuelConsumption.FuelLitres;
                }
                if (fuelConsumptionList.Capacity == 0)
                {
                    fleetFuelConsumption = 0;
                    litres = 0;
                }
                else
                {
                    fleetFuelConsumption = mpgTotal / fuelConsumptionList.Capacity;
                    if (fleetFuelConsumption > 20)
                        fleetFuelConsumption = 20;
                    fleetFuelLitres = litres / fuelConsumptionList.Capacity;
                }
                
            }
        }
    }
}