using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Newtonsoft.Json;

namespace Orchestrator.WebUI.Security.OAuth2
{

    /// <summary>
    /// Returns an authorization code for the logged-in user which is used to retrieve a token when Proteo Enterprise
    /// is used as an external OAuth2 provider for other apps (e.g. for Proteo Analytics).
    /// </summary>
    public class Auth : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            var queryString = context.Request.QueryString;

            var clientId = queryString["client_id"];
            var redirectUri = queryString["redirect_uri"];
            var returnUri = redirectUri;

            try
            {
                var user = context.User;
                var isClientUser = user.IsInRole(((int)Orchestrator.eUserRole.ClientUser).ToString());

                Facade.ISecurity facSecurity = new Facade.Security();
                var code = facSecurity.GenerateOAuth2Code(clientId, redirectUri, user.Identity.Name, isClientUser);

                returnUri += $"?state={queryString["state"]}&code={code}";
            }
            catch (ApplicationException ex)
            {
                returnUri += $"?error={ex.Message}";
            }
            catch
            {
                returnUri += "?error=authentication_failed";
            }

            context.Response.Redirect(returnUri, false);
        }

        public bool IsReusable
        {
            get { return false; }
        }

    }

}