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

using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Orchestrator.WebUI.Traffic
{
    public partial class updateResourceLocations : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                ConfigurePage();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnConfirm.Click += new EventHandler(btnConfirm_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
        }

        private void ConfigurePage()
        {
            int instructionId = Convert.ToInt32(Request.QueryString["instructionId"]);
            Entities.Instruction instruction = null;
            using (Facade.IInstruction facInstruction = new Facade.Instruction())
            {
                instruction = facInstruction.GetInstruction(instructionId);
            }

            // Display the resource information
            if (instruction.Driver != null)
                tdDriver.InnerText = instruction.Driver.Individual.FullName;
            if (instruction.Vehicle != null)
                tdVehicle.InnerText = instruction.Vehicle.RegNo;
            if (instruction.Trailer != null)
                tdTrailer.InnerText = instruction.Trailer.TrailerRef;

            // Display the point information
            XslCompiledTransform transformer = new XslCompiledTransform();
            transformer.Load(Server.MapPath(@"..\xsl\instructions.xsl"));

            XsltArgumentList args = new XsltArgumentList();
            args.AddParam("webserver", "", Orchestrator.Globals.Configuration.WebServer);
            args.AddParam("showShortAddress", "", "false");

            XmlUrlResolver resolver = new XmlUrlResolver();
            XPathNavigator navigator = instruction.Point.ToXml().CreateNavigator();

            // Populate the Point.
            StringWriter sw = new StringWriter();
            transformer.Transform(navigator, args, sw);
            tdPoint.InnerHtml = sw.GetStringBuilder().ToString();
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            mwhelper.CausePostBack = false;
            mwhelper.CloseForm = true;
        }

        void btnConfirm_Click(object sender, EventArgs e)
        {

        }
    }
}
