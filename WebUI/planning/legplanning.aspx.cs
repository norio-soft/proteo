using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Orchestrator.WebUI.planning
{
    public partial class legplanning : System.Web.UI.Page
    {
        Orchestrator.Entities.CustomPrincipal user;

        protected void Page_Load(object sender, EventArgs e)
        {
            user = (Orchestrator.Entities.CustomPrincipal)Page.User;
            LoadMenu();
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


            if (!Page.IsPostBack)
            {
                RadMenu1.LoadContentFile(string.Format("~/UserControls/menu{0}Default.xml", themeName));
            }

        }

        
    }
}