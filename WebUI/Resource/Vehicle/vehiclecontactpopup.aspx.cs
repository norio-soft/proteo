using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Orchestrator.WebUI.Resource.Vehicle
{
    public partial class VehicleContactPopUp : Orchestrator.Base.BasePage
    {
        protected DataView vwVehicle = null;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["resourceId"] == string.Empty || Request.QueryString["resourceId"] == null) return;
            int _resourceId = int.Parse(Request.QueryString["resourceId"]);

            DataAccess.IVehicle dacVehicle = new DataAccess.Vehicle();
            DataSet dsVehicle = dacVehicle.GetDataSetForVehicleId(_resourceId);
            vwVehicle = dsVehicle.Tables[0].DefaultView;
            this.DataBind();
        }
    }
}