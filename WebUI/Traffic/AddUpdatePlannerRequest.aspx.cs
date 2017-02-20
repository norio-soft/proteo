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

namespace Orchestrator.WebUI.Traffic
{
	/// <summary>
	/// Summary description for AddUpdatePlannerRequest.
	/// </summary>
	public partial class AddUpdatePlannerRequest : Orchestrator.Base.BasePage
	{
		private const string C_REQUEST_VS = "C_REQUEST_VS";

		#region Form Elements




		#endregion

		#region Page Variables

		private	int	m_requestId	= 0;
		private int	m_sourceJobId = 0;
		private int m_targetJobId = 0;

		private Entities.PlannerRequest m_request = null;

		private bool m_isUpdate = false;

		#endregion

		#region Page Load/Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage, eSystemPortion.Plan);

			m_requestId = Convert.ToInt32(Request.QueryString["RequestId"]);
			m_sourceJobId = Convert.ToInt32(Request.QueryString["SourceJobId"]);
			m_targetJobId = Convert.ToInt32(Request.QueryString["TargetJobId"]);

			m_request = (Entities.PlannerRequest) ViewState[C_REQUEST_VS];

			if (!IsPostBack)
			{
				if (m_requestId > 0)
				{
					m_isUpdate = true;

					using (Facade.IPlannerRequest facPlannerRequest = new Facade.PlannerRequest())
					{
						m_request = facPlannerRequest.GetPlannerRequestForRequestId(m_requestId);
						ViewState[C_REQUEST_VS] = m_request;

						txtRequestText.Text = m_request.RequestText;
						txtSourceJobId.Text = m_request.SourceJobId.ToString();
						txtTargetJobId.Text = m_request.TargetJobId.ToString();
						chkUseDriver.Checked = m_request.UseDriver;
						chkUseVehicle.Checked = m_request.UseVehicle;
						chkUseTrailer.Checked = m_request.UseTrailer;
					}
				}
				else
				{
					if (m_sourceJobId > 0)
						txtSourceJobId.Text = m_sourceJobId.ToString();
					if (m_targetJobId > 0)
						txtTargetJobId.Text = m_targetJobId.ToString();
				}
			}

			if (m_request != null)
			{
				m_isUpdate = true;
				btnAdd.Text = "Update Request";
			}
			else
				btnAdd.Text = "Add Request";

			lblConfirmation.Visible = false;
		}

		private void AddUpdatePlannerRequest_Init(object sender, EventArgs e)
		{
			cfvSourceJobId.ServerValidate += new ServerValidateEventHandler(JobId_ServerValidate);
			cfvTargetJobId.ServerValidate += new ServerValidateEventHandler(JobId_ServerValidate);
			btnAdd.Click += new EventHandler(btnAdd_Click);
			btnListRequest.Click += new EventHandler(btnListRequest_Click);
		}

		#endregion

		private void PopulateRequest()
		{
			if (m_request == null)
				m_request = new Entities.PlannerRequest();

			m_request.SourceJobId = Convert.ToInt32(txtSourceJobId.Text);
			m_request.TargetJobId = Convert.ToInt32(txtTargetJobId.Text);
			m_request.RequestText = txtRequestText.Text;
			m_request.UseDriver = chkUseDriver.Checked;
			m_request.UseVehicle = chkUseVehicle.Checked;
			m_request.UseTrailer = chkUseTrailer.Checked;
		}

		#region Button Events

		private void btnAdd_Click(object sender, EventArgs e)
		{
			if (Page.IsValid)
			{
				PopulateRequest();

				bool success = false;
				string userId = ((Entities.CustomPrincipal) Page.User).UserName;

				using (Facade.IPlannerRequest facPlannerRequest = new Facade.PlannerRequest())
				{
					if (m_isUpdate)
						success = facPlannerRequest.Update(m_request, userId);
					else
					{
						m_request.RequestId = facPlannerRequest.Create(m_request, userId);
						success = m_request.RequestId > 0;
					}

					lblConfirmation.Text = "The request has " + (success ? "" : "not") + "been " + (m_isUpdate ? "updated." : "added.");
					lblConfirmation.Visible = true;

					if (success)
					{
						ViewState[C_REQUEST_VS] = m_request;
						btnAdd.Text = "Update Request";
					}
				}
			}
		}

		private void btnListRequest_Click(object sender, EventArgs e)
		{
			Response.Redirect("ListPlannerRequests.aspx");
		}

		#endregion

		#region Validation Events

		private void JobId_ServerValidate(object source, ServerValidateEventArgs args)
		{
			args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(args.Value, 1, true);

			if (args.IsValid)
			{
				try
				{
					int jobId = Convert.ToInt32(args.Value);

					using (Facade.IJob facJob = new Facade.Job())
					{
						if (facJob.GetJob(jobId) == null)
							args.IsValid = false;
					}
				}
				catch
				{
					args.IsValid = false;
				}
			}
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Init += new EventHandler(AddUpdatePlannerRequest_Init);
		}
		#endregion
	}
}
