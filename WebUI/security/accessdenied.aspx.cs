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

namespace Orchestrator.WebUI.Security
{
	/// <summary>
	/// Summary description for accessdenied.
	/// </summary>
    public partial class accessdenied : System.Web.UI.Page
	{
		#region Form Elements

		protected Label lblSystemPortion;

		#endregion

		#region Page Load

		protected void Page_Load(object sender, System.EventArgs e)
		{	
			// Get roles that can access requested SystemPortion
			Facade.IRole facRole = new Facade.Security();
			Entities.RoleCollection rolesForSystemPortions = facRole.GetRolesForSystemPortions((eSystemPortion[]) Session["SystemPortions"]);
		
			// Display Roles that can access the requested SystemPortion
			for (int i=0; i < rolesForSystemPortions.Count; i++)
			{
				Entities.Role currentRole = (Entities.Role) rolesForSystemPortions[i];

				if (i == 0)
					lblRole.Text = currentRole.Name;
				else if (i == 1 && rolesForSystemPortions.Count == 2)
					lblRole.Text += " and " + currentRole.Name;
				else if (i == 1 && rolesForSystemPortions.Count > 2)
					lblRole.Text += ", " + currentRole.Name;
				else if (i == (rolesForSystemPortions.Count - 1))
					lblRole.Text += " and " + currentRole.Name;
				else
					lblRole.Text += ", " + currentRole.Name;
			}

			// Display the username of the requesting user
			lblUser.Text += ((Entities.CustomPrincipal) Page.User).UserName;
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
