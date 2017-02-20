using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Orchestrator.WebUI
{
    public partial class fmwebparts2 : System.Web.UI.Page
    {
        public string DataUrl { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.DataUrl = string.Format("{0}/api/benchmark",
            ConfigurationManager.AppSettings["ApiBaseUrl"]);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (Page.Theme == "FleetMetrik" || Orchestrator.Globals.Configuration.FleetMetrikInstance)
            {
                radTabs.Tabs[0].Visible = false;
            }
        }

        protected void wpmManager_AuthorizeWebPart(object sender, WebPartAuthorizationEventArgs e)
        {
            // Hide the Unallocated Orders web part if allocation is not enabled
            if (e.Path.EndsWith("wpUnallocatedOrders.ascx", StringComparison.CurrentCultureIgnoreCase))
                e.IsAuthorized = Orchestrator.WebUI.Utilities.IsAllocationEnabled();
        }
    }
}