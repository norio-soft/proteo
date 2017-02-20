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

namespace Orchestrator.WebUI
{
	/// <summary>
	/// Summary description for UpdateResourceAvailability.
	/// </summary>
	public partial class UpdateResourceAvailability : Orchestrator.Base.BasePage
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			bool success = false;
			try
			{
				int resourceId = Convert.ToInt32(Request.QueryString["ResourceId"]);
				int resourceTypeId = Convert.ToInt32(Request.QueryString["ResourceTypeId"]);
				int resourceStatusId = Convert.ToInt32(Request.QueryString["ResourceStatusId"]);

				using (Facade.IResource facResource = new Facade.Resource())
                	success = facResource.UpdateResourceStatus(resourceId, (eResourceType) resourceTypeId, (eResourceStatus) resourceStatusId, ((Entities.CustomPrincipal) Page.User).UserName);
			}
			catch {}

			Response.Clear();
			if (success)
				Response.Write("Success");
			else
				Response.Write("Failed");
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
