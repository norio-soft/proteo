namespace Orchestrator.WebUI.UserControls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

//	using EeekSoft.Web;

	/// <summary>
	///	Summary description for alerter.
	/// </summary>
	public partial class alerter : System.Web.UI.UserControl
	{
		#region Form Elements


		#endregion

		#region Page Variables

		private int m_userIdentityId = 0;

		#endregion
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			m_userIdentityId = ((Entities.CustomPrincipal) Page.User).IdentityId;
			DisplayAlerts();
		}

		#region Private Members

		private bool DismissAlert(int alertId)
		{
			// Dismiss the alert
			Facade.IAlert facAlert = new Facade.Alert();
			bool success = facAlert.Dismiss(alertId, ((Entities.CustomPrincipal) Page.User).UserName);

			// Clear all the alerts and re-populate.
			plcHolderAlerts.Controls.Clear();
			DisplayAlerts();

			return success;
		}

		private void DisplayAlert(Entities.Alert alert)
		{
            //PopupWin popup = new PopupWin();

            //int height = 75;
            //int gap = 15;
            //string title = String.Empty;
            //string message = String.Empty;
            //string link = String.Empty;

            //popup.ActionType = PopupAction.RaiseEvents;
            //popup.AutoShow = true;
            //popup.DockMode = PopupDocking.BottomRight;
            //popup.Height = new Unit(height.ToString() + "px");
            //popup.OffsetY = (height + gap) * plcHolderAlerts.Controls.Count;

            //popup.LinkClicked += new EventHandler(popup_LinkClicked);
            //popup.PopupClosed += new EventHandler(popup_PopupClosed);

            //Facade.IReferenceData facReferenceData = new Facade.ReferenceData();

            //switch (alert.AlertType)
            //{
            //    case eAlertType.CheckJobResourcing:
            //        title = "Job " + alert.ObjectId.ToString() + " requires attention.";
            //        message = "One of the resources being used on this job requires your attention.";
            //        break;
            //    case eAlertType.DriverRequestedInformation:
            //        title = "Driver requires information.";
            //        message = facReferenceData.GetDriverNameForResourceId(alert.ObjectId) + " has requested information.";
            //        break;
            //    case eAlertType.APointRequiresIntegration:
            //        title = "A Point Requires Integration";
            //        message = "A job that requires integration is waiting for one or more points to be resolved.";
            //        break;
            //    default:
            //        title = "Unknown alert type";
            //        message = "The alert type was not recognised.";
            //        break;
            //}

            //facReferenceData.Dispose();

            //popup.Attributes.Add("alertId", alert.AlertId.ToString());
            //popup.Attributes.Add("alertTypeId", ((int) alert.AlertType).ToString());
            //popup.Attributes.Add("objectId", alert.ObjectId.ToString());

            //popup.Title = title;
            //popup.Message = message;

            //plcHolderAlerts.Controls.Add(popup);
		}

		private void DisplayAlerts()
		{
            // Retrieve this user's alerts
			Facade.IAlert facAlert = new Facade.Alert();
			Entities.AlertCollection alertColl = facAlert.GetForUser(m_userIdentityId);

			if (alertColl != null && alertColl.Count > 0)
			{
				foreach (Entities.Alert alert in alertColl)
				{
					DisplayAlert(alert);
				}
			}
		}

		#endregion

		#region Event Handlers

		private void popup_LinkClicked(object sender, EventArgs e)
		{
            //int alertId = Int32.Parse(((PopupWin) sender).Attributes["alertId"]);
            //int objectId = Int32.Parse(((PopupWin) sender).Attributes["objectId"]);
            //eAlertType alertType = (eAlertType) Int32.Parse(((PopupWin) sender).Attributes["alertTypeId"]);

            //if (DismissAlert(alertId))
            //{
            //    switch (alertType)
            //    {
            //        case eAlertType.CheckJobResourcing:
            //            Response.Redirect("~/Job/job.aspx?jobId=" + objectId.ToString());
            //            break;
            //        case eAlertType.DriverRequestedInformation:
            //            Response.Redirect("~/Resource/Driver/ListUnsureDrivers.aspx");
            //            break;
            //        case eAlertType.APointRequiresIntegration:
            //            Response.Redirect("~/Integration/IntegratePoints.aspx");
            //            break;
            //        default:
            //            break;
            //    }
            //}
		}

		private void popup_PopupClosed(object sender, EventArgs e)
		{
            //int alertId = Int32.Parse(((PopupWin) sender).Attributes["alertId"]);
            //eAlertType alertType = (eAlertType) Int32.Parse(((PopupWin) sender).Attributes["alertTypeId"]);

            //switch (alertType)
            //{
            //    case eAlertType.CheckJobResourcing:
            //        DismissAlert(alertId);
            //        break;
            //    case eAlertType.DriverRequestedInformation:
            //        DismissAlert(alertId);
            //        break;
            //    case eAlertType.APointRequiresIntegration:
            //        DismissAlert(alertId);
            //        break;
            //    default:
            //        break;
            //}
		}

		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{

		}
		#endregion

	}
}
