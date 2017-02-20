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
	/// Summary description for addjob.
	/// </summary>
	public partial class addjob : Orchestrator.Base.BasePage
	{
		#region Form Elements


		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditJob);

			if (!IsPostBack)
			{
				cboJobType.Items.Clear();

				cboJobType.Items.Add(new ListItem(Enum.GetName(typeof(eJobType), eJobType.Normal), ((int) eJobType.Normal).ToString()));
				cboJobType.Items.Add(new ListItem(Enum.GetName(typeof(eJobType), eJobType.Return), ((int) eJobType.Return).ToString()));
				cboJobType.Items.Add(new ListItem(Enum.GetName(typeof(eJobType), eJobType.PalletReturn), ((int) eJobType.PalletReturn).ToString()));
			}
		}

		private void addjob_Init(object sender, EventArgs e)
		{
			btnAddJob.Click += new EventHandler(btnAddJob_Click);
		}

		private void btnAddJob_Click(object sender, EventArgs e)
		{
			eJobType jobType = (eJobType) Convert.ToInt32(cboJobType.SelectedValue);

			switch (jobType)
			{
				case eJobType.Normal:
					cboJobType.Enabled = false;
					pnlOpenWizard.Visible = true;
					break;
				case eJobType.PalletReturn:
					Response.Redirect("addupdatepalletreturnjob.aspx");
					break;
				case eJobType.Return:
					Response.Redirect("addupdategoodsreturnjob.aspx");
					break;
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
			this.Init += new EventHandler(addjob_Init);
		}
		#endregion
	}
}
