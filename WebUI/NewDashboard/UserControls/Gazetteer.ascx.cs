using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Orchestrator.WebUI.Services;


namespace Orchestrator.WebUI.NewDashboard.UserControls
{
    public partial class Gazetteer : System.Web.UI.UserControl
    {
        public List<Vehicle> vehicleList;
        protected void Page_Load(object sender, EventArgs e)
        {
            CanDashboardServices canDashboardServices = new CanDashboardServices();
            vehicleList = canDashboardServices.GetVehicleData();
        }
    }
}