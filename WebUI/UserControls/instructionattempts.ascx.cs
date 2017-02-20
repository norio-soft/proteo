namespace Orchestrator.WebUI.UserControls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///	Displays the number of instruction attempts for the given job.
	/// </summary>
	public partial class InstructionAttempts : System.Web.UI.UserControl
	{
		#region Form Elements


		#endregion

		#region Form Variables

		private int m_jobId = Convert.ToInt32(HttpContext.Current.Request.QueryString["JobId"]);

		#endregion

		#region Property Interfaces

		public int JobId
		{
			get { return m_jobId; }
			set { m_jobId = value; }
		}

		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
        {
            DataAccess.IJob dacJob = new DataAccess.Job();

			if (m_jobId > 0 && dacJob.GetJobTypeIdForJobId(m_jobId) != (int)eJobType.Groupage)
				BindReAttemptHistory();
		}

		private void BindReAttemptHistory()
		{
			DataSet dsRedeliveries = null;

			using (Facade.IRedelivery facRedelivery = new Facade.Redelivery())
				dsRedeliveries = facRedelivery.GetForJobId(m_jobId);

			if (dsRedeliveries != null && dsRedeliveries.Tables[0] != null && dsRedeliveries.Tables[0].Rows.Count > 0)
			{
				dgRedeliveries.DataSource = dsRedeliveries;
				dgRedeliveries.DataBind();
				dgRedeliveries.Visible = true;
			}
			else
				dgRedeliveries.Visible = false;
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion
	}
}
