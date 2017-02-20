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

namespace Orchestrator.WebUI.scan
{
	/// <summary>
	/// Summary description for viewer.
	/// </summary>
	public partial class viewer : Orchestrator.Base.BasePage
	{
		#region Form Elements
		

		#endregion

		#region Protected Fields

		protected string userFullName;
		protected int scannedFormId;

		#endregion

		#region Public Properties

		public bool IsLocal 
		{
			get 
			{ 
				if (Request.QueryString["UseLocal"].Equals("1"))
					return true;
				else
					return false;
			}
		}

		public string FormType
		{
			get
			{
				if (Request.QueryString["TYPEID"] != null)
				{
					return Request.QueryString["TYPEID"].ToString();
				}
				else
				{
					return "Form";
				}
			}
		}
		#endregion

		#region Page Load

		protected void Page_Load(object sender, System.EventArgs e)
		{
			userFullName = ((Entities.CustomPrincipal)Page.User).UserName;

			if (!IsLocal && Request.QueryString["ScannedFormID"] != null)
			{
				txtFileName.Text = "";
				Facade.IForm facForm = new Facade.Form();
				scannedFormId = int.Parse(Request.QueryString["ScannedFormId"]);
				Entities.Scan scan = facForm.GetForScannedFormId(scannedFormId);
				txtFirstFormPageID.Text = Request.QueryString["PageNumber"];
				txtTotalPages.Text = scan.PageCount.ToString();
			}
			else
			{
				txtTotalPages.Text = Request.QueryString["TotalPages"];
				txtFileName.Text = Request.QueryString["FileName"];
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
