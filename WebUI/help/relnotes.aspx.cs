using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Orchestrator.WebUI.help
{
    public partial class relnotes : System.Web.UI.Page
    {
        protected string path = "releaseNotes.html"; //specify the path to your file

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                divContent.InnerHtml = ReadFile(Server.MapPath(path));
            }
        }

        protected string ReadFile(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                return string.Empty;
            }

            using (System.IO.StreamReader sr = new System.IO.StreamReader(path))
            {
                return sr.ReadToEnd();
            }
        }
    }
}