namespace Orchestrator.WebUI.UserControls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	/// Displays the business rules that were infringed during a Facade call.
	/// </summary>
	public partial class BusinessRuleInfringementDisplay : System.Web.UI.UserControl
	{
		#region Private Fields

		private Entities.BusinessRuleInfringementCollection		m_infringements = null;

		#endregion

		#region Page Variables


		#endregion

		#region Property Interfaces

		public Entities.BusinessRuleInfringementCollection Infringements
		{
			get { return m_infringements; }
			set { m_infringements = value; }
		}

		#endregion

		protected void Page_Load(object sender, System.EventArgs e) {}

		public void DisplayInfringments()
		{
			if (m_infringements != null)
			{
				repBusinessRuleInfringements.DataSource = m_infringements;
				repBusinessRuleInfringements.DataBind();
				this.Visible = true;
			}
			else
				this.Visible = false;
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
