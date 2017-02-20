using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Orchestrator.WebUI.Services;

namespace Orchestrator.WebUI.NewDashboard.UserControls
{
        public partial class DriverIdlingTimeBarChart : System.Web.UI.UserControl
        {
            public List<IdlingTime> idlingTimeList;
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
                    idlingTimeList = canDasboardServices.GetDriverIdlingTime(timePeriod.StartDate, timePeriod.EndDate, null);
                    idlingTimeList.Sort(delegate(IdlingTime it1, IdlingTime it2) { return it2.IdlingSeconds.CompareTo(it1.IdlingSeconds); });

                }
            }

        }
}