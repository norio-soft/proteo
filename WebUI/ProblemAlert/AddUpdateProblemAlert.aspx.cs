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


namespace Orchestrator.WebUI.ProblemAlert
{
	/// <summary>
	/// Summary description for AddUpdateProblemAlert.
	/// </summary>
	public partial class AddUpdateProblemAlert : Orchestrator.Base.BasePage
	{
		#region Constants

		private const string C_PROBLEM_ALERT = "VS_PROBLEM_ALERT";

		#endregion

		#region Page Variables

		private int						m_jobId = 0;
		private Entities.ProblemAlert	m_problemAlert = null;

		#endregion

		#region Form Elements


		#endregion

		#region Page Load/Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			m_jobId = Convert.ToInt32(Request.QueryString["jobId"]);

			if (!IsPostBack)
			{
				// Load the problem alert.
				Facade.IProblemAlert facProblemAlert = new Facade.ProblemAlert();
				m_problemAlert = facProblemAlert.GetProblemAlertForJobId(m_jobId);
				ViewState[C_PROBLEM_ALERT] = m_problemAlert;

				if (m_problemAlert != null)
				{
					// Bind the problem alert.
					txtProblemDescription.Text = m_problemAlert.Problem;
					dteETA.SelectedDate = m_problemAlert.ETA;
					chkIsResolved.Checked = m_problemAlert.State == eProblemAlertState.Complete;
				}
			}
			else
			{
				m_problemAlert = (Entities.ProblemAlert) ViewState[C_PROBLEM_ALERT];
			}

			lblMessage.Visible = false;
		}

		
		private void AddUpdateProblemAlert_Init(object sender, EventArgs e)
		{
			btnSubmit.Click += new EventHandler(btnSubmit_Click);
		}

		#endregion

		private void PopulateProblemAlert()
		{
			Facade.IProblemAlert facProblemAlert = new Facade.ProblemAlert();
			m_problemAlert = facProblemAlert.GetProblemAlertForJobId(m_jobId);

			if (m_problemAlert == null)
			{
				m_problemAlert = new Orchestrator.Entities.ProblemAlert();
				m_problemAlert.State = eProblemAlertState.New;
				m_problemAlert.JobId = m_jobId;
			}
			else
				m_problemAlert.State = eProblemAlertState.Updated;

			m_problemAlert.Problem = txtProblemDescription.Text;
			m_problemAlert.ETA = dteETA.SelectedDate.Value;
			if (chkIsResolved.Checked)
				m_problemAlert.State = eProblemAlertState.Complete;
		}

		private void btnSubmit_Click(object sender, EventArgs e)
		{
			if (Page.IsValid)
			{
				PopulateProblemAlert();

				bool success = false;
				Facade.IProblemAlert facProblemAlert = new Facade.ProblemAlert();
				if (m_problemAlert.ProblemAlertId == 0)
				{
					m_problemAlert.ProblemAlertId = facProblemAlert.Create(m_problemAlert, ((Entities.CustomPrincipal) Page.User).UserName);
					if (m_problemAlert.ProblemAlertId > 0)
						success = true;
				}
				else
					success = facProblemAlert.Update(m_problemAlert, ((Entities.CustomPrincipal) Page.User).UserName);

				if (success)
				{
					ViewState[C_PROBLEM_ALERT] = m_problemAlert;
					lblMessage.Text = "The problem alert has been stored.";
				}
				else
					lblMessage.Text = "The problem alert has not been stored.";
				lblMessage.Visible = true;
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
			this.Init += new EventHandler(AddUpdateProblemAlert_Init);
		}
		#endregion
	}
}
