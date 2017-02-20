namespace Orchestrator.WebUI.UserControls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Summary description for footer.
	/// </summary>
	public partial class footer : System.Web.UI.UserControl
	{
        Entities.CustomPrincipal user;
        protected	bool		m_isWizard			= false;
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (Request["wiz"]!=null && Request["wiz"].ToString()=="true")
			{
				m_isWizard = true;
                __pnlFooter.Visible = false;
			}
            user = (Entities.CustomPrincipal)Page.User;
            lblUser.Text = user.Name;
		}

		public string IsWizard
		{
			get 
			{
				if (m_isWizard)
					return "none";
				else
					return string.Empty;
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion
	}
}
