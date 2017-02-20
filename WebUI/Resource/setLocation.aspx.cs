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

namespace Orchestrator.WebUI.Resource
{
    public partial class SetLocation : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnSetLocation.Click += new EventHandler(btnSetLocation_Click);
        }

        void btnSetLocation_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                // Set the Location for this Resource
                SetCurrentLocation();
            }
        }

        private void SetCurrentLocation()
        {

        }
    }
}