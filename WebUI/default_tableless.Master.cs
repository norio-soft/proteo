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
using Telerik.Web.UI;

namespace Orchestrator.WebUI
{
    public partial class Masterpage_Tableless : System.Web.UI.MasterPage
    {

        Orchestrator.Entities.CustomPrincipal user;
        public bool HideMenu
        {
            set
            {
                this.divLayoutHeader.Visible = false;
                this.divLayoutNav.Visible = false;
                this.divMenuBar.Visible = false;
            }
        }

        public RadMenu MainMenu
        {
            get
            {
                return this.RadMenu1;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            user = (Orchestrator.Entities.CustomPrincipal)Page.User;
            LoadMenu();
        }



        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Globals.Configuration.DefaultSearch.ToLower() == "orders")
                this.cblSearchType.SelectedValue = "orders";
            else if (Globals.Configuration.DefaultSearch.ToLower() == "runs")
                this.cblSearchType.SelectedValue = "runs";

            btnSearch.ServerClick += new EventHandler(btnSearch_ServerClick);
        }

        void btnSearch_ServerClick(object sender, EventArgs e)
        {
            if (cblSearchType.SelectedValue == "orders")
                Response.Redirect("/groupage/findorder.aspx?ss=" + txtSearchString.Text);
            else
                Response.Redirect("/job/jobsearch.aspx?searchString=" + txtSearchString.Text);
        }

        private string TransformXML(string sXML, string XSLPath)
        {
            XmlDataDocument oXML = new XmlDataDocument();
            //XPathDocument oXSL = new XPathDocument(XSLPath);

            XslCompiledTransform oTransform = new XslCompiledTransform();
            XsltArgumentList xslArg = new XsltArgumentList();
            string sMessage;
            //oXML.Load(new XmlTextReader(sXML, XmlNodeType.Document, null));
            oXML.Load(sXML);
            XPathNavigator nav = oXML.CreateNavigator();
            oTransform.Load(XSLPath);

            try
            {
                System.IO.MemoryStream oText = new System.IO.MemoryStream();
                XmlTextWriter xmlWrite = new XmlTextWriter(oText, System.Text.Encoding.UTF8);
                oTransform.Transform(nav, xmlWrite);
                xmlWrite.Flush();
                oText.Flush();
                oText.Position = 1;
                System.IO.StreamReader sr = new System.IO.StreamReader(oText);
                sMessage = sr.ReadToEnd();
                oText.Close();
                return sMessage;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        private void LoadMenu()
        {
            //Get the theme name if it isn't Orchestrator so that it can be used to select the menu
            string themeName = Page.Theme;
            if (themeName.Equals("Orchestrator", StringComparison.CurrentCultureIgnoreCase))
                themeName = string.Empty;

            if (user.IsInRole(((int)eUserRole.MapViewer).ToString()))
            {
                // limit to map screen and no Menu;
                Response.Redirect("ng/fleet");
            }
            else if (user.IsInRole(((int)eUserRole.ClientUser).ToString()))
            {
                if (!Page.IsPostBack)
                    RadMenu1.LoadContentFile(string.Format("~/UserControls/menu{0}Client.xml", themeName));

                WhiteLabelHeader();
            }
            else if(Globals.Configuration.FleetMetrikInstance)
            {
                if (!Page.IsPostBack)
                    RadMenu1.LoadContentFile("~/UserControls/menuFleetMetrikDefault.xml");
            }
            else if (!Page.IsPostBack)
            {

               RadMenu1.LoadContentFile(string.Format("~/UserControls/menu{0}Default.xml", themeName));
            }

            
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
