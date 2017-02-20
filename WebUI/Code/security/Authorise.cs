using System;
using System.Collections;
using System.Web;
using System.Web.SessionState;

namespace Orchestrator.WebUI.Security
{
	/// <summary>
	/// Summary description for Authorise.
	/// </summary>
	public class Authorise
	{
		private Authorise() {}

        public static bool CanAccess(eSystemPortion systemPortion)
        {
            eSystemPortion[] arrSp = new eSystemPortion[1];
            arrSp[0] = systemPortion;
            return CanAccess(arrSp);
        }

		public static bool CanAccess(params eSystemPortion[] systemPortions)
		{
			bool canAccess = false;
			HttpContext httpContext = HttpContext.Current;

			Facade.ISecurity facSecurity = new Facade.Security();
			Facade.IUser facUser = new Facade.User();
			
			string[] userRoleString = (((Entities.CustomPrincipal) httpContext.User).UserRole.Split(new char[]{','}));
			eUserRole[] userRole = new eUserRole[userRoleString.Length];
			
			for(int i=0; i<userRoleString.Length; i++) 
				userRole[i] = (eUserRole) int.Parse(userRoleString[i]);

			// Store this user's roles and the portions testing against in the session.
			httpContext.Session["UserRole"] = userRole;
			httpContext.Session["SystemPortions"] = systemPortions;

			foreach (eSystemPortion sp in systemPortions)
				if (facSecurity.CanAccessPortion(userRole, sp))
				{
					canAccess = true;
					break;
				}

			return canAccess;
		}

		public static void EnforceAuthorisation(eSystemPortion systemPortion)
		{
			eSystemPortion[] arrSp = new eSystemPortion[1];
			arrSp[0] = systemPortion;
            EnforceAuthorisation(arrSp);
		}

		public static void EnforceAuthorisation(params eSystemPortion[] systemPortions)
		{
			if (!CanAccess(systemPortions))
				HttpContext.Current.Response.Redirect("~/security/accessdenied.aspx");
		}
	}
}
