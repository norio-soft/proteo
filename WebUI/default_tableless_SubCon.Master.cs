using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Orchestrator.WebUI
{
    public partial class default_tableless_SubCon : System.Web.UI.MasterPage
    {
        Orchestrator.Entities.CustomPrincipal user;

        protected void Page_Load(object sender, EventArgs e)
        {
            user = (Orchestrator.Entities.CustomPrincipal)Page.User;
            LoadMenu();
        }

        private void LoadMenu()
        {
            if (user.IsInRole(((int)eUserRole.SubConPortal).ToString()))
            {
                if (!Page.IsPostBack)
                    RadMenu1.LoadContentFile("~/UserControls/menuSubCon.xml");

                WhiteLabelHeader();
            }
            else
                RadMenu1.LoadContentFile("~/UserControls/menuDefault.xml");
        }

        private void WhiteLabelHeader()
        {
            HtmlLink css = new HtmlLink();
            css.Href = "/WhiteLabel/" + Globals.Configuration.SubConPortalCssFileName;
            css.Attributes["rel"] = "stylesheet";
            css.Attributes["type"] = "text/css";
            css.Attributes["media"] = "all";
            hdr.Controls.Add(css); ;
        }
    }
}
