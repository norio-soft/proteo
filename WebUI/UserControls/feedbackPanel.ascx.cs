using System;
using System.Diagnostics;

namespace Orchestrator.WebUI.UserControls
{
    public partial class feedbackPanel : System.Web.UI.UserControl
    {
        #region Properties

        private TraceLevel _feedbackLevel = TraceLevel.Error;
        public TraceLevel Level
        {
            private get { return _feedbackLevel; }
            set
            {
                _feedbackLevel = value;
                ConfigureControl();
            }
        }

        private string _message;
        public string Message
        {
            private get { return _message; }
            set
            {
                _message = value;
                ConfigureControl();
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                ConfigureControl();
        }

        private void ConfigureControl()
        {
            switch (Level)
            {
                case TraceLevel.Off:
                case TraceLevel.Verbose:
                    imgError.Visible = false;
                    break;
                case TraceLevel.Error:
                    imgError.ImageUrl = Page.ResolveUrl("~/images/ico_critical.gif");
                    imgError.AlternateText = "Error";
                    break;
                case TraceLevel.Warning:
                    imgError.ImageUrl = Page.ResolveUrl("~/images/ico_warning.gif");
                    imgError.AlternateText = "Warning";
                    break;
                case TraceLevel.Info:
                    imgError.ImageUrl = Page.ResolveUrl("~/images/ico_info.gif");
                    imgError.AlternateText = "Info";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            lblFeedbackMessage.Text = Message;
        }
    }
}