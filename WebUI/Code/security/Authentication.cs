using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Orchestrator.Repositories;

namespace Orchestrator.WebUI.Security
{

    public static class Authentication
    {

        private const string _rememberMeCookieName = "RememberMe";

        private class AuthenticationCookieData
        {
            public int IdentityID { get; set; }
            public string Roles { get; set; }
            public string FullName { get; set; }
            public string Username { get; set; }
            public Guid SessionID { get; set; }
        }

        internal enum eProcessUserOutcome { Success, InvalidUser, AccessDenied, RedirectToCustomerUserLandingPage, RedirectToClientPortal };

        internal static eProcessUserOutcome ProcessAuthenticatedUser(string username, bool rememberMe, bool redirectOnceLoggedIn, string customerUserSecurityKey)
        {
            var context = HttpContext.Current;

            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<IUserRepository>(uow);
                var user = repo.FindByName(username);

                if (user == null)
                    return eProcessUserOutcome.InvalidUser;

                // If this is a client user and the querystring does not contain the security key (and this type of security has been turned on) redirect to the customer user landing page
                if (user.IsInRole(eUserRole.ClientUser))
                {
                    if (!string.IsNullOrWhiteSpace(Globals.Configuration.CustomerUserSecurityKey) && !string.IsNullOrWhiteSpace(Globals.Configuration.CustomeUserLandingPage))
                    {
                        // If no security key (or incorrect key) in query string redirect to customer user landing page
                        if (customerUserSecurityKey != Globals.Configuration.CustomerUserSecurityKey)
                            return eProcessUserOutcome.RedirectToCustomerUserLandingPage;
                    }
                }

                // If the user is a client user redirect them to the new client portal, rather than logging them in
                if (!Properties.Settings.Default.UseOldClientPortal && user.IsInRole(eUserRole.ClientUser))
                    return eProcessUserOutcome.RedirectToClientPortal;

                // A user must be a member of at least one role
                if (!user.UserRoles.Any())
                    return eProcessUserOutcome.AccessDenied;

                // Determine if the user already has an active authentication ticket, for example if they logged in and then ended up here after being redirected to the change password page.
                var authTicket = GetCurrentAuthenticationTicket();

                var sessionID = Guid.NewGuid();

                if (authTicket != null)
                {
                    // Reuse the existing Session ID
                    var existingAuthenticationCookieData = ParseAuthenticationCookieData(authTicket.UserData);

                    if (existingAuthenticationCookieData != null)
                        sessionID = existingAuthenticationCookieData.SessionID;
                }

                var authenticationCookieData = new AuthenticationCookieData
                {
                    IdentityID = user.IdentityID,
                    Roles = user.RoleIDsCommaSeparated,
                    FullName = user.FullName,
                    Username = user.UserName,
                    SessionID = sessionID,
                };

                SetActiveSession(user, sessionID);
                uow.SaveChanges();

                SetRememberMeCookie(rememberMe, user.UserName);

                SetAuthenticationTicket(authenticationCookieData, sessionID);
            }

            return eProcessUserOutcome.Success;
        }

        /// <summary>
        /// Validate that the current user session has a valid forms authentication cookie containing an
        /// active Session ID and, if so, attach a CustomPrincipal for the user to the current HttpContext.
        /// </summary>
        internal static void ValidateSession()
        {
            var context = HttpContext.Current;
            var authTicket = GetCurrentAuthenticationTicket();

            if (authTicket == null)
                return;

            var authenticationCookieData = ParseAuthenticationCookieData(authTicket.UserData);

            // If the cookie data does not exist in the required format then the session is not valid
            if (authenticationCookieData == null)
            {
                context.User = null;
                return;
            }

            if (context.Request.Url.AbsolutePath.Contains(".aspx"))
            {
                using (var uow = DIContainer.CreateUnitOfWork())
                {
                    var userRepo = DIContainer.CreateRepository<IUserRepository>(uow);
                    var user = userRepo.FindByName(authenticationCookieData.Username, includeIdentity: true, includeUserRoles: true);
                    var isExcludedFromSessionValidation = user.IsProteoUser || user.IsInRole(eUserRole.ClientUser);

                    if(user.Individual.Identity.IdentityStatus == eIdentityStatus.Deleted)
                    {
                        LogOff();
                        context.Response.Redirect(context.Request.Url.AbsolutePath + "?HasLoggedOff=1&lockordeleted=2");
                        return;
                    }
                    if(user.LockedOutUntilDateTime > System.DateTime.Now)
                    {
                        LogOff();
                        context.Response.Redirect(context.Request.Url.AbsolutePath + "?HasLoggedOff=1&lockordeleted=1");
                        return;
                    }
                    // If the sessionID does not match any active session then it is not valid.
                    if (!isExcludedFromSessionValidation && !user.GetUserSessionIDs().Contains(authenticationCookieData.SessionID.ToString()))
                    {
                        LogOff();
                        context.Response.Redirect(context.Request.Url.AbsolutePath + "?HasLoggedOff=1&multiples=1");
                        return;
                    }
                }
            }

            var identity = new FormsIdentity(authTicket);

            var principal = new Entities.CustomPrincipal(
                identity,
                authenticationCookieData.IdentityID,
                authenticationCookieData.Roles,
                authenticationCookieData.FullName,
                authenticationCookieData.Username);

            context.User = principal;
        }

        internal static void LogOff()
        {
            var context = HttpContext.Current;
            System.Web.Security.FormsAuthentication.SignOut();

            if (context.Session != null)
            {
                context.Session.Abandon();
                context.Session.Clear();
            }

            context.Response.Cookies[Orchestrator.Globals.Constants.UserRoles].Value = null;
            context.Response.Cookies[Orchestrator.Globals.Constants.UserRoles].Path = "/";
            context.Response.Cookies[Orchestrator.Globals.Constants.UserRoles].Expires = new System.DateTime(1999, 10, 12);
            context.Response.Cookies.Remove(Orchestrator.Globals.Constants.UserRoles);
            context.Request.Cookies.Remove(Orchestrator.Globals.Constants.UserRoles);
            context.Request.Cookies.Clear();
            context.User = null;
        }

        internal static string GetRememberMeUsername()
        {
            var cookie = HttpContext.Current.Request.Cookies[_rememberMeCookieName];
            
            if (cookie == null)
                return null;
            
            try
            {
                return SystemFramework.EncryptionProvider.DecryptText(cookie.Value);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Set the session ID for the user's current session.  If this leaves the user with too many active sessions then terminate the oldest session(s).
        /// </summary>
        private static void SetActiveSession(Models.User user, Guid sessionID)
        {
            var sessionIDs = user.GetUserSessionIDs().ToList();

            if (sessionIDs.Contains(sessionID.ToString()))
                return;

            if (sessionIDs.Count() >= user.AllowedSessionCount)
                // The user has too many active sessions.  Throw away the oldest sessions - if they subsequently attempt to use those sessions they will be automatically logged out.
                sessionIDs.RemoveRange(0, sessionIDs.Count() - user.AllowedSessionCount + 1);

            sessionIDs.Add(sessionID.ToString());
            user.SetUserSessionIDs(sessionIDs);
        }

        private static void SetAuthenticationTicket(AuthenticationCookieData cookieData, Guid sessionID)
        {
            var userInformation = string.Format(
                "{0}|{1}|{2}|{3}|{4}",
                cookieData.IdentityID,
                cookieData.Roles,
                cookieData.FullName,
                cookieData.Username,
                cookieData.SessionID);

            var authTicket = new FormsAuthenticationTicket(
                version: 1,
                name: cookieData.Username,
                issueDate: DateTime.UtcNow,
                expiration: DateTime.UtcNow.AddYears(10),
                isPersistent: true,
                userData: userInformation);

            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);

            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            HttpContext.Current.Response.Cookies.Add(authCookie);
        }

        private static FormsAuthenticationTicket GetCurrentAuthenticationTicket()
        {
            var authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie == null)
                return null;

            try
            {
                return FormsAuthentication.Decrypt(authCookie.Value);
            }
            catch
            {
                return null;
            }
        }

        private static AuthenticationCookieData ParseAuthenticationCookieData(string cookieData)
        {
            var userInformation = cookieData.Split('|');

            if (userInformation.Length != 5)
                return null;

            int identityID;
            Guid sessionID;

            if (!int.TryParse(userInformation[0], out identityID) || !Guid.TryParse(userInformation[4], out sessionID))
                return null;

            var authenticationCookieData = new AuthenticationCookieData
            {
                IdentityID = identityID,
                Roles = userInformation[1],
                FullName = userInformation[2],
                Username = userInformation[3],
                SessionID = Guid.Parse(userInformation[4]),
            };

            return authenticationCookieData;
        }

        private static void SetRememberMeCookie(bool rememberMe, string username)
        {
            var context = HttpContext.Current;

            if (rememberMe)
            {
                // Create new cookie that will basically do a quick log-in.
                var encryptedUsername = SystemFramework.EncryptionProvider.EncryptText(username);
                var rememberMeCookie = new HttpCookie(_rememberMeCookieName, encryptedUsername);
                rememberMeCookie.Expires = DateTime.UtcNow.AddYears(10);
                context.Response.Cookies.Add(rememberMeCookie);
            }
            else if (context.Request.Cookies[_rememberMeCookieName] != null)
            {
                // Clear the cookie
                var rememberMeCookie = new HttpCookie(_rememberMeCookieName, string.Empty);
                rememberMeCookie.Expires = DateTime.UtcNow;
                context.Response.Cookies.Add(rememberMeCookie);
            }
        }

    }

}