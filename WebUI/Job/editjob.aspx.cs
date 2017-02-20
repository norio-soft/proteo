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

namespace Orchestrator.WebUI.Job
{
	/// <summary>
	/// Summary description for editjob.
	/// </summary>
	public partial class editjob : Orchestrator.Base.BasePage
	{
		#region Page Variables

		protected	int		m_jobId = 0;
		
		#endregion

		#region Form Elements


		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditJob, eSystemPortion.GeneralUsage);

			m_jobId = Convert.ToInt32(Request.QueryString["jobId"]);

			if (m_jobId > 0)
   			{
				Facade.IJob facJob = new Facade.Job();
				Entities.Job job = facJob.GetJob(m_jobId);

				if (job != null)
				{
					switch (job.JobType)
					{
						case eJobType.Normal:
							pnlOpenWizard.Visible = true;
							break;
						case eJobType.PalletReturn:
							Response.Redirect("addupdatepalletreturnjob.aspx?jobId=" + m_jobId.ToString());
							break;
						case eJobType.Return:
							Response.Redirect("addupdategoodsreturnjob.aspx?jobId=" + m_jobId.ToString());
							break;
					}
				}
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
