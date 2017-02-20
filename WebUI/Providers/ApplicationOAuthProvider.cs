using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Dependencies;
using System.Web.Security;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Infrastructure;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;

namespace Orchestrator.WebUI.Providers
{

    /// <summary>
    /// Custom token provider that will generate tokens for grant type "forms_authentication_cookie" based on the current authenticated user's Forms Authentication cookie.
    /// </summary>
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {

        private const string _formsAuthenticationCookieGrantType = "forms_authentication_cookie";

        // Short-lived tokens since a new token can be requested by the browser based on the current Forms Authentication cookie without the user being aware.
        private static readonly TimeSpan _defaultAccessTokenExpireTimeSpan = TimeSpan.FromMinutes(60);

        private readonly string _publicClientId = null;
        private readonly IDependencyResolver _dependencyResolver = null;
        private readonly IRepositoryFactory _repositoryFactory = null;

        public ApplicationOAuthProvider(string publicClientId, IDependencyResolver dependencyResolver)
        {
            if (publicClientId == null)
                throw new ArgumentNullException("publicClientId");

            _publicClientId = publicClientId;
            _dependencyResolver = dependencyResolver;

            _repositoryFactory = this.ResolveDependency<IRepositoryFactory>();
        }

        public override async Task GrantCustomExtension(OAuthGrantCustomExtensionContext context)
        {
            // Only requests of grant_type "forms_authentication_cookie" are supported
            if (context.GrantType != _formsAuthenticationCookieGrantType)
            {
                var error = "unsupported_grant_type";
                context.SetError(error);
                return;
            }

            using (var uow = this.ResolveDependency<IUnitOfWork>())
            {
                var expireTimeSpan = _defaultAccessTokenExpireTimeSpan;

                // Retrieve details about the current logged-in HE Forms Authentication user
                FormsIdentity identity = null;
                Models.User user = null;

                var principal = context.Request.User as System.Security.Principal.GenericPrincipal;

                if (principal != null)
                    identity = principal.Identity as FormsIdentity;

                if (identity != null)
                {
                    var userRepo = _repositoryFactory.CreateRepository<Repositories.IUserRepository>(uow);
                    user = userRepo.FindByName(identity.Name);
                }

                if (user == null)
                {
                    var error = "not_authenticated";
                    var errorDescription = "The WebUI token endpoint is only available for users that are already authenticated via Forms Authentication with a cookie";
                    context.SetError(error, errorDescription);
                    return;
                }

                // Set the expiry of the token to one hour or the expiry time of the cookie, whichever is earlier
                var cookieExpireTimeSpan = identity.Ticket.Expiration.Subtract(DateTime.UtcNow);

                if (cookieExpireTimeSpan < expireTimeSpan && cookieExpireTimeSpan > TimeSpan.Zero)
                    expireTimeSpan = cookieExpireTimeSpan;

                // Generate the token
                context.Options.AccessTokenExpireTimeSpan = expireTimeSpan;
                var userManager = context.OwinContext.GetUserManager<Facade.Authentication.IdentityUserManager>();
                var oAuthIdentity = await userManager.CreateIdentityAsync(user, context.Options.AuthenticationType);
                var properties = CreateProperties(user.IdentityID, user.UserName, user.Individual.FirstNames, user.Individual.LastName);
                var ticket = new AuthenticationTicket(oAuthIdentity, properties);
                context.Validated(ticket);
            }
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
                context.Validated();

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                var expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                    context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        private static AuthenticationProperties CreateProperties(int userID, string userName, string firstName, string lastName)
        {
            var data = new Dictionary<string, string>
            {
                { "user_id", userID.ToString() },
                { "user_name", userName },
                { "first_name", firstName },
                { "last_name", lastName },
            };

            return new AuthenticationProperties(data);
        }

        private TDependency ResolveDependency<TDependency>()
            where TDependency : class
        {
            return _dependencyResolver.GetService(typeof(TDependency)) as TDependency;
        }

    }

}