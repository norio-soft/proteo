﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Orchestrator.Facade;
using System.Data;

namespace Orchestrator.WebUI.NewDashboard.UserControls
{
    public partial class EOTL : System.Web.UI.UserControl
    {
        public DataSet eotlDS;
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
                eotlDS = GetDataSimple(timePeriod.StartDate, timePeriod.EndDate);
            }
        }

        public DataSet GetDataSimple(DateTime startDate, DateTime endDate)
        {
            DataSet dsKPI = null;
            KPI kpi = new KPI();
            dsKPI = kpi.GetEOTL(startDate, endDate);
            return dsKPI;

        }
    }
}