	using System;
	using System.Collections.Specialized;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using Orchestrator.Globals;
namespace Orchestrator.WebUI.UserControls
{
	
	/// <summary>
	///		Summary description for TakePCVs.
	/// </summary>
	public partial class resourceAuditHistory : System.Web.UI.UserControl
	{
		#region Form Elements


		#endregion

		#region Page Variables

		private int					m_resourceId = 0;
		 
		#endregion

		#region Property Interfaces

		private int ResourceId
		{
			get
			{
				if (Request.QueryString["ResourceId"] != null)
					return Convert.ToInt32(Request.QueryString["ResourceId"]);
				else
				{
					if (ViewState["C_RESOURCE_ID"] != null)
						return (int) ViewState["C_RESOURCE_ID"];
					else
						return 0;
				}
			}
			set
			{
				ViewState["C_RESOURCE_ID"] = value;
			}
		}

		#endregion

		#region Page Load/Init/Error

		protected void Page_Load(object sender, System.EventArgs e)
		{
			m_resourceId = ResourceId;
			if (m_resourceId > 0)
			{
				LoadAuditHistory();
			}
		}

		private void resourceAuditHistory_Init(object sender, EventArgs e)
		{

		}

		public void Reload()
		{
			LoadAuditHistory();
		}

		#endregion

		#region Events & Methods

		private void LoadAuditHistory()
		{
			Facade.IResource facResource = new Facade.Resource();
			DataSet ds = facResource.GetAuditDetailsForResourceId(m_resourceId);
			dgAuditHistory.DataSource = ds;
			dgAuditHistory.DataBind();
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
			this.Init +=new EventHandler(resourceAuditHistory_Init);
		}
		#endregion
	}
}
