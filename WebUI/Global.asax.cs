using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Optimization;
using System.Web.Routing;
using System.Xml.Serialization;

namespace Orchestrator.WebUI
{

    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(Object sender, EventArgs e)
        {
            Orchestrator.Globals.Configuration.InitialiseApplication();

            GlobalConfiguration.Configure(this.OnConfigured);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        private void OnConfigured(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Web API dependency injection
            DIConfig.Register(config);
        }

        protected void Application_EndRequest(Object sender, EventArgs e)
        {
            // Dispose of the Entity Framework ObjectContext
            if (Orchestrator.EF.DataContext.IsCurrentIntantiated)
                Orchestrator.EF.DataContext.Destroy();
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            if (!Request.IsAuthenticated)
                return;

            Security.Authentication.ValidateSession();
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
#if (!DEBUG)
            try
            {
                Exception exc = Server.GetLastError();
                //EmailExceptionToSupport(exc);
                Utilities.LastError = exc;
            }
            finally
            {
                Response.Redirect("~/error.aspx");
            }
#endif
        }

        protected void Session_End(Object sender, EventArgs e)
        {
        }

        protected void Application_End(Object sender, EventArgs e)
        {
        }

        // UnhandledException is to be called for any exceptions that are not handled by the Global Exception Handler.
        // E.g. Web-service calls.
        public static void UnhandledException(Exception exception)
        {
            try
            {
                if (HttpContext.Current != null)
                    Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                else
                    Elmah.ErrorLog.GetDefault(null).Log(new Elmah.Error(exception));
            }
            catch
            {
                try
                {
                    Logging.ApplicationLog.WriteError(exception.Message);
                }
                catch
                {
                    // If we are unable to log the exception then don't throw another, otherwise we just end up in a loop here.
                }
            }
        }

        private static void EmailExceptionToSupport(Exception exception)
        {
            StringBuilder sb = new StringBuilder();
            XmlSerializer xmls;
            StringWriter strWriter = null;
            HttpContext ctx = HttpContext.Current;

            sb.Append("An Orchestrator error has been encountered." + Environment.NewLine);
            sb.Append("Host Machine: " + Environment.MachineName + Environment.NewLine);
            sb.Append("Date: " + DateTime.Now.ToString("dd/MM/yy HH:mm:ss") + Environment.NewLine);
            sb.Append(Environment.NewLine);

            #region Details of Request

            sb.Append("Request Information" + Environment.NewLine);
            sb.Append("*******************" + Environment.NewLine);
            try
            {
                sb.Append("URL: " + ctx.Request.Url.AbsoluteUri + Environment.NewLine);
                sb.Append(string.Format("QueryString: {0} items, with {1} length", ctx.Request.QueryString.Keys.Count.ToString(), ctx.Request.QueryString.ToString().Length) + Environment.NewLine);
                sb.Append(Environment.NewLine);
                foreach (string key in ctx.Request.QueryString.AllKeys)
                    sb.Append(string.Format("             {0}: {1}", key, ctx.Request.QueryString[key]) + Environment.NewLine + Environment.NewLine);
                sb.Append(Environment.NewLine);
            }
            catch (Exception rexc)
            {
                sb.Append("Could not retrieve all request information: " + rexc.Message + Environment.NewLine);
            }

            #endregion

            #region Details of User

            sb.Append("User Information" + Environment.NewLine);
            sb.Append("****************" + Environment.NewLine);
            try
            {
                sb.Append("User: ");
                if (System.Threading.Thread.CurrentPrincipal != null && System.Threading.Thread.CurrentPrincipal is Entities.CustomPrincipal)
                {
                    Entities.CustomPrincipal user = System.Threading.Thread.CurrentPrincipal as Entities.CustomPrincipal;
                    sb.Append(user.UserName + Environment.NewLine);
                    sb.Append("Name: " + user.Name + Environment.NewLine);
                    sb.Append("Roles: " + user.UserRole + Environment.NewLine);
                }
                else
                    sb.Append("unknown" + Environment.NewLine);
                sb.Append("Is Authenticated: " + ctx.Request.IsAuthenticated + Environment.NewLine);
                sb.Append("User Host Name: " + ctx.Request.UserHostName + Environment.NewLine);
                sb.Append("User Host Address: " + ctx.Request.UserHostAddress + Environment.NewLine);
                sb.Append(Environment.NewLine);
            }
            catch (Exception uexc)
            {
                sb.Append("Could not retrieve all user information: " + uexc.Message + Environment.NewLine);
            }

            #endregion

            #region Details of Exception

            sb.Append("Exception Information" + Environment.NewLine);
            sb.Append("*********************" + Environment.NewLine);
            try
            {
                while (exception != null)
                {
                    sb.Append("Type: " + exception.GetType().Name + Environment.NewLine);
                    sb.Append("Message: " + exception.Message + Environment.NewLine);
                    sb.Append("Stack Trace: " + exception.StackTrace + Environment.NewLine);
                    sb.Append("Source: " + exception.Source + Environment.NewLine);
                    sb.Append("Serialised as: " + Environment.NewLine);

                    if (exception != null && exception.GetType().IsSerializable)
                    {
                        try
                        {
                            xmls = new XmlSerializer(exception.GetType());
                            strWriter = new StringWriter();
                            xmls.Serialize(strWriter, exception);
                            sb.Append(strWriter.ToString() + Environment.NewLine);
                        }
                        catch (Exception serialisationException)
                        {
                            sb.Append("Failed to serialise (" + serialisationException.Message + ")" + Environment.NewLine);
                        }
                        finally
                        {
                            if (strWriter != null)
                            {
                                strWriter.Close();
                                strWriter = null;
                            }
                        }
                    }
                    else
                        sb.Append("Does not support serialisation." + Environment.NewLine);
                    sb.Append(Environment.NewLine);

                    exception = exception.InnerException;
                }
            }
            catch (Exception eexc)
            {
                sb.Append("Could not retrieve all exception information: " + eexc.Message + Environment.NewLine);
            }

            #endregion

            #region Details of Session

            sb.Append("Session Information" + Environment.NewLine);
            sb.Append("*******************" + Environment.NewLine);
            try
            {
                sb.Append(string.Format("Session: {0} items", ctx.Session.Keys.Count) + Environment.NewLine);
                sb.Append(Environment.NewLine);
                foreach (string key in ctx.Session.Keys)
                {
                    object sessionItem = ctx.Session[key];

                    sb.Append(string.Format("         Key: {0}", key) + Environment.NewLine);
                    sb.Append(string.Format("         Type: {0}", sessionItem != null ? sessionItem.GetType().Name : "null") + Environment.NewLine);
                    sb.Append(string.Format("         Value: {0}", sessionItem != null ? sessionItem.ToString() : "null") + Environment.NewLine);
                    sb.Append("         Serialised as: " + Environment.NewLine);

                    if (sessionItem != null && sessionItem.GetType().IsSerializable)
                    {
                        try
                        {
                            xmls = new XmlSerializer(sessionItem.GetType());
                            strWriter = new StringWriter();
                            xmls.Serialize(strWriter, sessionItem);
                            sb.Append(strWriter.ToString() + Environment.NewLine);
                        }
                        catch (Exception serialisationException)
                        {
                            sb.Append("         Failed to serialise (" + serialisationException.Message + ")" + Environment.NewLine);
                        }
                        finally
                        {
                            if (strWriter != null)
                            {
                                strWriter.Close();
                                strWriter = null;
                            }
                        }
                    }
                    else
                        sb.Append("         Does not support serialisation." + Environment.NewLine);
                    sb.Append(Environment.NewLine);
                }
                sb.Append(Environment.NewLine);
            }
            catch (Exception sexc)
            {
                sb.Append("Could not retrieve all session information: " + sexc.Message + Environment.NewLine);
            }

            #endregion

            string errorText = sb.ToString();

            try
            {
                MailMessage mailMessage = new MailMessage();

                mailMessage.From = new MailAddress(Orchestrator.Globals.Configuration.MailFromAddress,
                    Orchestrator.Globals.Configuration.MailFromName);

                mailMessage.To.Add("support@orchestrator.co.uk");

                mailMessage.Subject = "Orchestrator Error - " + Environment.MachineName;
                mailMessage.Body = sb.ToString();
                mailMessage.IsBodyHtml = false;

                SmtpClient smtp = new System.Net.Mail.SmtpClient();
                smtp.Host = Globals.Configuration.MailServer;
                smtp.Credentials = new NetworkCredential(Globals.Configuration.MailUsername,
                    Globals.Configuration.MailPassword);

                smtp.Send(mailMessage);
                mailMessage.Dispose();

            }
            catch { }
        }

    }

}
