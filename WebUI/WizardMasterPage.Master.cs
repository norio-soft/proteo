using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Text.RegularExpressions;

namespace Orchestrator.WebUI
{
    public partial class WizardMasterPage : System.Web.UI.MasterPage
    {
        #region White Space Remover
        private static readonly Regex REGEX_BETWEEN_TAGS = new Regex(@">\s+<", RegexOptions.Compiled);
        private static readonly Regex REGEX_LINE_BREAKS = new Regex(@"\n\s+", RegexOptions.Compiled);

        protected override void Render(HtmlTextWriter writer)
        {
            string removeWhiteSpace = ConfigurationManager.AppSettings["RemoveWhiteSpace"].ToString();
            if (removeWhiteSpace.ToLower() == "true")
            {
                using (HtmlTextWriter htmlwriter = new HtmlTextWriter(new System.IO.StringWriter()))
                {
                    base.Render(htmlwriter);
                    string html = htmlwriter.InnerWriter.ToString();

                    html = REGEX_BETWEEN_TAGS.Replace(html, "> <");
                    html = REGEX_LINE_BREAKS.Replace(html, string.Empty);

                    writer.Write(html.Trim());
                }
            }
            else
            {
                base.Render(writer);
            }
        }
        #endregion

        public string WizardTitle
        {
            get { return this.lblWizardTitle.Text; }
            set { this.lblWizardTitle.Text = value; }
        }

        public bool HideScriptManager
        {
            set
            {
                if (value)
                {
                    smManagerWrapper.Visible = false;
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            /////////////////////////////////////////////
            // Register the scripts.js javascript file //
            /////////////////////////////////////////////
            String csname = "scriptsJs";
            String csurl = Orchestrator.Globals.Configuration.WebServer + "/script/scripts.js";
            Type cstype = this.GetType();

            // Get a ClientScriptManager reference from the Page class.
            ClientScriptManager csm = Page.ClientScript;

            // Check to see if the include script exists already.
            if (!csm.IsClientScriptIncludeRegistered(cstype, csname))
            {
                csm.RegisterClientScriptInclude(cstype, csname, csurl);
            }

            //this.coverContentDiv.Visible = false;
            //this.coverContentDiv.Style.Add(HtmlTextWriterStyle.Display, "none");
            //this.loadingDiv.Visible = false;
            //this.loadingDiv.Style.Add(HtmlTextWriterStyle.Display, "none");
            //this.loadingImageDiv.Visible = false;
            //this.loadingImageDiv.Style.Add(HtmlTextWriterStyle.Display, "none");
        }
    }
}
