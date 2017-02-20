using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Dependencies;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.OAuth;
using Owin;

namespace Orchestrator.WebUI
{

    /// <summary>
    /// Web UI currently uses Forms Authentication using a cookie.  However, logged in users need to be able to access Orchestrator.WebAPI web service calls which require an ASP.NET Identity bearer token, without having to separately log in.
    /// Therefore we allow an authenticated WebUI user to retrieve a token that can be used with Orchestrator.WebAPI based on their existing credentials.
    /// </summary>
    public partial class Startup
    {

        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }
        public static string PublicClientId { get; private set; }

        public void ConfigureAuth(IAppBuilder app, IDependencyResolver dependencyResolver)
        {
            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext<Facade.Authentication.IdentityUserManager>((options, context) =>
                Facade.Authentication.UserManagerFactory.Create(options, dependencyResolver));

            // Configure the application for OAuth based flow
            PublicClientId = "self";

            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/token"),
                Provider = new Providers.ApplicationOAuthProvider(PublicClientId, dependencyResolver),
                AllowInsecureHttp = true,
            };

            // Enable the WebUI application to issue bearer tokens that can be used by Orchestrator.WebAPI to authenticate users.
            // However, WebUI itself doesn't use these tokens for authentication.
            app.UseOAuthAuthorizationServer(OAuthOptions);
            app.UseOAuthBearerAuthentication(new OAuthBearerAuthenticationOptions());
        }

    }

}