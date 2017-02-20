using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Orchestrator.WebUI.UserControls.views
{
    public partial class resourcedescription : System.Web.UI.UserControl
    {
        public string Data;
        protected void Page_Load(object sender, EventArgs e)
        {

            // Load the Jobs/Legs for this resource
            Facade.IResource facResource = new Facade.Resource();
            int resourceID = int.Parse(Data.Split('|')[0]);
            int resourceTypeID = int.Parse(Data.Split('|')[1]);
            DataSet ds = facResource.GetTrafficSheetForResource((eResourceType)resourceTypeID, resourceID);
            if (ds.Tables[0].Rows.Count > 0)
            {
                this.grdLegs.DataSource = ds;
                grdLegs.DataBind();
            }
            else
            {
                pnlJobs.Visible = false;
                pnlNoJobs.Visible = true;
            }

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            
        }

    }
}