using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Security;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Orchestrator.WebUI.administration
{
    public partial class ControlAreaManagement : Orchestrator.Base.BasePage
    {
     
        protected void Page_Load(object sender, EventArgs e)
        {
            BaseUrlScriptTag.Text = string.Format(
               "<script type='text/javascript'>angular.module('controlAreaManagementApp').run(function ($rootScope) {{$rootScope.peBaseUrl = '{0}'; $rootScope.apiBaseUrl = '{1}'; $rootScope.authCookieName = '{2}';}});</script>",
               this.Request.Url.GetLeftPart(UriPartial.Authority),
               ConfigurationManager.AppSettings["ApiBaseUrl"],
               FormsAuthentication.FormsCookieName);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
       
        }

    }
}