using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using Orchestrator;
using Orchestrator.Entities;

namespace Orchestrator.WebUI
{
    public partial class default_tableless_client : System.Web.UI.MasterPage
    {
        Orchestrator.Entities.CustomPrincipal user;

        protected void Page_Load(object sender, EventArgs e)
        {
            user = (Orchestrator.Entities.CustomPrincipal)Page.User;
            LoadMenu();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnSearch.ServerClick += new EventHandler(btnSearch_ServerClick);
        }

        void btnSearch_ServerClick(object sender, EventArgs e)
        {
            if (cblSearchType.SelectedValue == "orders")
                Response.Redirect("/groupage/findorder.aspx?ss=" + txtSearchString.Text);
            else
                Response.Redirect("/job/jobsearch.aspx?searchString=" + txtSearchString.Text);
        }

        private void LoadMenu()
    {
        //Get the theme name if it isn't Orchestrator so that it can be used to select the menu
        string themeName = Page.Theme;
        if (themeName.Equals("Orchestrator", StringComparison.CurrentCultureIgnoreCase))
            themeName = string.Empty;

            if (user.IsInRole(((int)eUserRole.ClientUser).ToString()))
            {
                RadMenu1.LoadContentFile(string.Format("~/UserControls/menu{0}Client.xml", themeName));

                Telerik.Web.UI.RadMenuItem knaufMenuItem = null;

                // The knauf menu item must be on the First Menu
                knaufMenuItem = RadMenu1.Items.FindItemByText("Knauf Shunt Loading Sheet");

                if (knaufMenuItem != null)
                {
                    if (user.IsInRole(((int)eUserRole.KnaufLoadingSheetInClientPortal).ToString()))
                    {
                        knaufMenuItem.Visible = true;
                    }
                    else 
                    {
                        knaufMenuItem.Visible = false;
                    }
                }
            }
            else if (!Page.IsPostBack)
                RadMenu1.LoadContentFile(string.Format("~/UserControls/menu{0}Default.xml", themeName));

            WhiteLabelHeader();
    
        }

        private void WhiteLabelHeader()
        {
            HtmlLink css = new HtmlLink();
            css.Href = "/WhiteLabel/" + Globals.Configuration.ClientPortalCssFileName;
            css.Attributes["rel"] = "stylesheet";
            css.Attributes["type"] = "text/css";
            css.Attributes["media"] = "all";
            hdr.Controls.Add(css); ;
        }
    }
}
