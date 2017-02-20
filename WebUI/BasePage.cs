using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;

using Orchestrator.Logging;
using System.Text;
using Orchestrator.WebUI;

namespace Orchestrator.Base
{
    public class BasePage : System.Web.UI.Page
    {

        #region Dialog Handling

        //The AlwaysOnTop mode passes the opening window in the dialogArgumnets and does NOT
        //set the window.opener.

        private const string CALLBACK_FUNCTION =
@"function __dialogCallBack(source, returnValue){{
window.returnValue=returnValue;
if (typeof(window.dialogArguments)!='undefined' && window.dialogArguments!=null)
    window.dialogArguments.eval('{0}(null,""'+ returnValue + '"")');
else if (typeof(window.opener)!='undefined' && window.opener!=null)   
    eval('window.opener.{0}(source,""'+ returnValue + '"")');
}}
";

        private const string CALLBACK_SETVALUE =
@"function __dialogCallBack(source, returnValue){{
window.returnValue=returnValue;
}}
";

        private const string CALLBACK_CALL = "__dialogCallBack(window, '{0}');";

        private bool _close = false;
        private string _returnValue = string.Empty;

        private const string vs_userNotClient = "vs_userNotClient";
        private const string vs_userSubCon = "vs_userSubCon";

        protected bool IsClientUser
        {
            get
            {
                bool retVal = true;

                if (ViewState[vs_userNotClient] == null)
                    ViewState[vs_userNotClient] = Page.User.IsInRole(((int)Orchestrator.eUserRole.ClientUser).ToString());

                retVal = (bool)ViewState[vs_userNotClient];
                return retVal;
            }
        }

        protected bool IsSubConUser
        {
            get
            {
                bool retVal = true;

                if (ViewState[vs_userSubCon] == null)
                    ViewState[vs_userSubCon] = Page.User.IsInRole(((int)Orchestrator.eUserRole.SubConPortal).ToString());

                retVal = (bool)ViewState[vs_userSubCon];
                return retVal;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (this.IsClientUser && !Orchestrator.WebUI.Properties.Settings.Default.UseOldClientPortal)
            {
                Server.Transfer("~/security/login.aspx?CPR=1");
                return;
            }

            string dcb = this.Request.QueryString["dcb"];

            string script = string.Empty;

            if (!string.IsNullOrEmpty(dcb))
                script = string.Format(CALLBACK_FUNCTION, dcb);
            else
                script = string.Format(CALLBACK_SETVALUE, dcb);

            if (!string.IsNullOrEmpty(_returnValue))
                script += string.Format(CALLBACK_CALL, _returnValue);

            //If Close has been called then set the ReturnValue and close the form
            //The ReturnValue is set here as well in case a Callback function hasn't been
            //created which happens when dcb is not passed, e.g. when Modal
            if (_close) script += string.Format("window.returnValue='{0}';window.close();", _returnValue);

            if (script.Length > 0)
                this.ClientScript.RegisterStartupScript(this.GetType(), "DialogCallBack", script, true);

        }

        /// <summary>
        /// Set the ReturnValue. A callback will be done if it is not an empty string and a callback function is available.
        /// </summary>
        public string ReturnValue
        {
            get { return _returnValue; }
            set { _returnValue = value; }
        }

        /// <summary>
        /// Close the window after the page has been shown.
        /// </summary>
        public void Close()
        {
            _close = true;
        }

        /// <summary>
        /// Set the ReturnValue and callback to the opening window the close this window after the page has been shown.
        /// </summary>
        /// <param name="ReturnValue"></param>
        public void Close(string ReturnValue)
        {
            _close = true;
            _returnValue = ReturnValue;
        }

        #endregion

        #region Logging

        private Logger _logger;
        private bool _logging;

        protected void Page_PreInit(object sender, EventArgs e)
        {
            _logging = this.GetAppSetting("EnableWebLogStats");

            if (_logging)
            {
                _logger = new Logger(this.Context.ApplicationInstance);
                _logger.Start();
            }

            this.RegisterShortCutKeyScripts();
            this.GetSessionIDFromQueryString();

        }

        protected void Page_Unload(object sender, EventArgs e)
        {
            if (_logging)
            {
                _logger.Finish(Page.IsPostBack);
            }
        }

        #endregion

        #region Cookie Handling

        private string _cookieSessionID = string.Empty;
        public string CookieSessionID
        {
            get
            {
                if (string.IsNullOrEmpty(_cookieSessionID))
                {
                    _cookieSessionID = Utilities.GetRandomString(6);
                }

                return _cookieSessionID;
            }
            set
            {
                _cookieSessionID = value;
            }
        }

        private void GetSessionIDFromQueryString()
        {
            if (!String.IsNullOrEmpty(Request.QueryString["csid"]))
            {
                _cookieSessionID = Request.QueryString["csid"];
            }
        }

        #endregion

        //Provide every page with an easy way to instaitate the DataContext
        public EF.DataContext DataContext
        {
            get { return Orchestrator.EF.DataContext.Current; }
        }

        public void MessageBox(string Message)
        {
            this.ClientScript.RegisterStartupScript(this.GetType(), "MessageBox", string.Format("alert(\"{0}\");", Message), true);
        }

        public void MessageBox(string message, bool autoDismiss)
        {
            string js =
                @"$(document).ready(function() {
            var outer = $.create(
                            'div',

                            { 'id': 'injectedOuterDiv',
                                'style': '',
                                'class': 'GlobalAutoDismissMessage_OuterDiv'
                            },

                            [' ']
                        );

            var inner = $.create(
                            'div',
                            { 
                                'id': 'injectedMessageDiv',
                                'class': 'GlobalAutoDismissMessage_InnerDiv'
                            },
                            null
                        );

            //var logo = $.create(
                            //'img',
                            //{   
                                //'id': 'img1',
                                //'class': 'GlobalAutoDismissMessage_LogoImg',
                                //'src': '../../../images/orch-logo-circle.png'
                            //},
                            //null
                        //);

            var spanMessage = $.create(
                            'span',
                            { 
                                'id': 'spanMessage',
                                'class': 'GlobalAutoDismissMessage_MessageSpan'
                            },
                            [' " + message + @"']
                        );

            $('#messageWrapper').append($(outer));
            $(outer).append($(inner));
            //$(inner).append($(logo));
            $(inner).append($(spanMessage));

            setTimeout('hideMessage()', 3000);
        });

        function hideMessage() {

            var div = $('div[id*=injectedOuterDiv]');
            $(div).fadeOut('slow');    
        }";

            this.ClientScript.RegisterStartupScript(this.GetType(), "MessageBox", js, true);
        }

        #region Private methods

        private bool GetAppSetting(string appSettingName)
        {
            bool appSettingValue = false;
            string appSettingString = ConfigurationManager.AppSettings[appSettingName];

            if (!String.IsNullOrEmpty(appSettingString))
                if (appSettingString.ToLower() == "true")
                    appSettingValue = true;

            return appSettingValue;
        }

        private void RegisterShortCutKeyScripts()
        {
            string clientScriptFileKey = "ClientScriptsFile";
            string cookieSessionIdFileKey = "CookieSessionScriptFile";
            string clientScriptWireUpKey = "ShortCutKeyWireUp";

            Type clientScriptManagerType = this.GetType();

            ClientScriptManager clientScriptManager = this.Page.ClientScript;

            // Check to see if the client script is already registered.
            if (!clientScriptManager.IsClientScriptIncludeRegistered(clientScriptManagerType, clientScriptFileKey))
            {
                string clientScriptUrl = this.ResolveUrl("~/script/scripts.js");

                clientScriptManager.RegisterClientScriptInclude(clientScriptManagerType, clientScriptFileKey, clientScriptUrl);
            }

            if (!clientScriptManager.IsClientScriptIncludeRegistered(clientScriptManagerType, cookieSessionIdFileKey))
            {
                string clientScriptUrl = this.ResolveUrl("~/script/cookie-session-id.js");

                clientScriptManager.RegisterClientScriptInclude(clientScriptManagerType, cookieSessionIdFileKey, clientScriptUrl);
            }

            // make sure scripts have not already been registered
            if (!clientScriptManager.IsStartupScriptRegistered(clientScriptManagerType, clientScriptWireUpKey))
            {
                string clientScriptWireUpText = @"document.onkeyup = ShortCutKeyCapture;";

                clientScriptManager.RegisterStartupScript(clientScriptManagerType, clientScriptWireUpKey, clientScriptWireUpText, true);
            }
        }

        #endregion
    }
}
