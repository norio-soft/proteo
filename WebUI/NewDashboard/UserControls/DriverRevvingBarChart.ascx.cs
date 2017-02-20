using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Orchestrator.WebUI.Services;

namespace Orchestrator.WebUI.NewDashboard.UserControls
{
    public partial class DriverRevvingBarChart : System.Web.UI.UserControl
    {
        public List<InfringementCount> infringementList;
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
                infringementList = canDasboardServices.GetDriverOverReving(timePeriod.StartDate, timePeriod.EndDate, null);
                infringementList.Sort(delegate(InfringementCount ic1, InfringementCount ic2) { return ic2.Count.CompareTo(ic1.Count); });

            }
        }
    }
}