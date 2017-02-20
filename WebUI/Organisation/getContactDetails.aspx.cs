using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Orchestrator.WebUI.Organisation
{
    public partial class getContactDetails : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int identityId = Convert.ToInt32(Request.QueryString["IdentityId"]);

            // Telephone Number
            // Fax Number
            // Primary Contact Email
            // Head Office Address

            Entities.Organisation organisation = null;

            if (identityId > 0)
            {
                string cacheName = "_organisation" + identityId.ToString();

                if (Cache[cacheName] == null)
                {
                    Facade.IOrganisation facOrganisation = new Facade.Organisation();
                    organisation = facOrganisation.GetForIdentityId(identityId);
                    Cache.Add(cacheName, organisation, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 20, 0), System.Web.Caching.CacheItemPriority.Normal, null);
                }
                else
                {
                    organisation = (Entities.Organisation) Cache[cacheName];
                }
            }

            if (organisation != null)
            {
                // An organisation has been selected for viewing so display it.
                XslCompiledTransform transformer = new XslCompiledTransform();
                transformer.Load(Server.MapPath(@"..\xsl\organisationContactDetails.xsl"));

                XmlUrlResolver resolver = new XmlUrlResolver();
                XPathNavigator navigator = organisation.ToXml().CreateNavigator();

                // Populate the Organisation Contact Details.
                StringWriter sw = new StringWriter();
                transformer.Transform(navigator, null, sw);
                Response.Write(sw.GetStringBuilder().ToString());

                if (Response.IsClientConnected)
                {
                    Response.Flush();
                    Response.End();
                }
                return;
            }
        }
    }
}