using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Orchestrator.WebUI.Traffic.JobManagement
{
	/// <summary>
	/// Summary description for printerFriendlyCommunication.
	/// </summary>
	public partial class printerFriendlyCommunication : Orchestrator.Base.BasePage
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage, eSystemPortion.Communicate);

			if (!IsPostBack)
			{
				int driverCommunicationId = Convert.ToInt32(Request.QueryString["communicationId"]);

				DisplayCommunication(driverCommunicationId);
			}
		}

		private void DisplayCommunication(int driverCommunicationId)
		{
			// Load the communication
			Entities.DriverCommunication communication = null;
			Entities.Driver driver = null;

			using (Facade.IDriver facDriver = new Facade.Resource())
			{
				communication = ((Facade.IDriverCommunication) facDriver).GetForDriverCommunicationId(driverCommunicationId);
				int driverId = facDriver.GetDriverForDriverCommunicationId(driverCommunicationId);
				driver = facDriver.GetDriverForResourceId(driverId);
			}

			if (communication != null && driver != null)
			{
				lblDriverFullName.Text = driver.Individual.FullName;
				lblCommunicationStatus.Text = Utilities.UnCamelCase(communication.DriverCommunicationStatus.ToString());
				lblCommunicationType.Text = Utilities.UnCamelCase(communication.DriverCommunicationType.ToString());
				lblCommunicationComments.Text = communication.Comments.Replace("START PLACE", "<br>START PLACE").Replace("STOP", "<br>STOP");
				lblCommunicationText.Text = communication.Text;
				lblCommunicationNumberUsed.Text = communication.NumberUsed;
			}
		}

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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
		}
		#endregion
	}
}
