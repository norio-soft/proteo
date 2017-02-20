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
	/// Summary description for takePCVs.
	/// </summary>
	public partial class takePCVs : Orchestrator.Base.BasePage
	{
		#region Form Elements


		#endregion

		#region Page Variables

		protected int		m_jobId = 0;
        protected int       m_pointID = 0;

		#endregion

		#region Page Load/Init/Error

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage, eSystemPortion.AttachPCVsToJob);

			if (!IsPostBack)
			{
				m_jobId = Convert.ToInt32(Request.QueryString["JobId"]);

            
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
		}
		#endregion
	}
}
