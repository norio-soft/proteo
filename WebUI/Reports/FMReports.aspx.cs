using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Orchestrator.WebUI.Reports
{
    public partial class FMReports : System.Web.UI.Page
    {

       
         protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //DateTime StartDate = DateTime.Today.AddYears(-1);
                //DateTime EndDate = DateTime.Today;

                //string _startdate = DateTime.Now.Subtract(new DateTime(1970, 1,1)).TotalMilliseconds
                //lnkWeeklyDriverGrading.NavigateUrl = string.Format("javascript:popup('/Reports/ReportViewer2.aspx?rn=FleetMetrik.DriverGradingByWeek&StartDate={0}&EndDate={1}', 1200, 750)", StartDate, EndDate);
                //lnkMonthlyDriverGrading.NavigateUrl = string.Format("javascript:popup('/Reports/ReportViewer2.aspx?rn=FleetMetrik.DriverGradingByMonth&StartDate={0}&EndDate={1}', 1200, 750)", StartDate, EndDate);

                //lnkWeeklySpeedingTrend.NavigateUrl = string.Format("javascript:popup('/Reports/ReportViewer2.aspx?rn=FleetMetrik.SpeedingTrendByWeek&StartDate={0}&EndDate={1}', 1200, 750)", StartDate, EndDate);
                //lnkMonthlySpeedingTrend.NavigateUrl = string.Format("javascript:popup('/Reports/ReportViewer2.aspx?rn=FleetMetrik.SpeedingTrendByMonth&StartDate={0}&EndDate={1}', 1200, 750)", StartDate, EndDate);
                //lnkWeeklyHarshBrakingTrend.NavigateUrl = string.Format("javascript:popup('/Reports/ReportViewer2.aspx?rn=FleetMetrik.HarshBrakingTrendByWeek&StartDate={0}&EndDate={1}', 1200, 750)", StartDate, EndDate);
                //lnkMonthlyHarshBrakingTrend.NavigateUrl = string.Format("javascript:popup('/Reports/ReportViewer2.aspx?rn=FleetMetrik.HarshBrakingTrendByMonth&StartDate={0}&EndDate={1}', 1200, 750)", StartDate, EndDate);

                //lnkWeeklyOverRevvingTrend.NavigateUrl = string.Format("javascript:popup('/Reports/ReportViewer2.aspx?rn=FleetMetrik.OverRevvingTrendByWeek&StartDate={0}&EndDate={1}', 1200, 750)", StartDate, EndDate);
                //lnkMonthlyOverRevvingTrend.NavigateUrl = string.Format("javascript:popup('/Reports/ReportViewer2.aspx?rn=FleetMetrik.OverRevvingTrendByMonth&StartDate={0}&EndDate={1}', 1200, 750)", StartDate, EndDate);

                //lnkWeeklyIdlingTrend.NavigateUrl = string.Format("javascript:popup('/Reports/ReportViewer2.aspx?rn=FleetMetrik.IdlingTrendByWeek&StartDate={0}&EndDate={1}', 1200, 750)", StartDate, EndDate);
                //lnkMonthlyIdlingTrend.NavigateUrl = string.Format("javascript:popup('/Reports/ReportViewer2.aspx?rn=FleetMetrik.IdlingTrendByMonth&StartDate={0}&EndDate={1}', 1200, 750)", StartDate, EndDate);

                //DateTime overviewStartDate = new DateTime(DateTime.Today.Year, DateTime.Today.AddMonths(-1).Month, 01);
                //DateTime overviewEndDate = overviewStartDate.AddMonths(1);

                //lnkDriverOverview.NavigateUrl = string.Format("javascript:popup('/Reports/ReportViewer2.aspx?rn=FleetMetrik.DriverOverview&StartDate={0}&EndDate={1}', 1200, 750)", overviewStartDate, overviewEndDate);
            }
        }
    }
}