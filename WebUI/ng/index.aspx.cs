using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Orchestrator.WebUI.ng
{
    public partial class index : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            BaseUrlScriptTag.Text = string.Format(
                "<script type='text/javascript'>angular.module('peApp').run(function ($rootScope) {{$rootScope.peBaseUrl = '{0}'; $rootScope.apiBaseUrl = '{1}'; $rootScope.authCookieName = '{2}';}});</script>",
                this.Request.Url.GetLeftPart(UriPartial.Authority),
                ConfigurationManager.AppSettings["ApiBaseUrl"],
                FormsAuthentication.FormsCookieName);
        }
    }
}