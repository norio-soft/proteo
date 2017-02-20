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
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;


namespace Orchestrator.WebUI.Traffic.JobManagement
{
	/// <summary>
	/// Summary description for tabProgress.
	/// </summary>
	public partial class tabProgress : Orchestrator.Base.BasePage
	{
		private const string C_JOB_VS = "C_JOB_VS";
		
		#region Properties

        private		Entities.Organisation	m_organisation		= null;		// The client of the job.

		protected	Entities.Job			m_job				= null;
		protected	int						m_jobId				= 0;	// The id of the job we are currently manipulating.
		
		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage, eSystemPortion.TakeCallIn);
			m_jobId = Convert.ToInt32(Request.QueryString["jobId"]);
			if (!IsPostBack)
			{
				// Bind the job information.
				BindJob();
                if (Request.QueryString["instructionId"] == string.Empty || Request.QueryString["instructionId"] == "0")
                    CallInTabStrip1.Visible = false;
			}
		}

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.dlgUpdateDockets.DialogCallBack += new EventHandler(generic_DialogCallBack);
            this.dlgAddExtra.DialogCallBack += new EventHandler(generic_DialogCallBack);
        }

        void dlgAddExtra_DialogCallBack(object sender, EventArgs e)
        {
            
        }

		#region DataLoading and Binding

		private void BindJob()
		{
			using (Facade.IJob facJob = new Facade.Job())
			{
				LoadJob();

				using (Facade.IOrganisation facOrganisation = new Facade.Organisation())
					m_organisation = facOrganisation.GetForIdentityId(m_job.IdentityId);

				XmlDocument jobDocument = m_job.ToXml();
                XslCompiledTransform transformer = new XslCompiledTransform();
				transformer.Load(Server.MapPath(@"..\..\..\xsl\instructions.xsl"));
				XmlUrlResolver resolver = new XmlUrlResolver();
				XPathNavigator navigator = jobDocument.CreateNavigator();

				// Populate the Collections.
				XsltArgumentList collectionArgs = new XsltArgumentList();
				if (m_job.JobType == eJobType.PalletReturn)
					collectionArgs.AddParam("InstructionTypeId", "", "5");
				else
					collectionArgs.AddParam("InstructionTypeId", "", "1");
				collectionArgs.AddParam("DocketText", "", m_organisation.DocketNumberText);
				collectionArgs.AddParam("webserver", "", Orchestrator.Globals.Configuration.WebServer);
				StringWriter sw = new StringWriter();
				transformer.Transform(navigator, collectionArgs, sw);
				lblCollections.Text = sw.GetStringBuilder().ToString();

				// Populate the Deliveries.
				sw = new StringWriter();
				XsltArgumentList deliveryArgs = new XsltArgumentList();
				deliveryArgs.AddParam("InstructionTypeId", "", "2,6,7");
				deliveryArgs.AddParam("DocketText", "", m_organisation.DocketNumberText);
				deliveryArgs.AddParam("webserver", "", Orchestrator.Globals.Configuration.WebServer);
				transformer.Transform(navigator, deliveryArgs, sw);
				lblDeliveries.Text = sw.GetStringBuilder().ToString();

				// Populate the Pallet Handling.
				sw = new StringWriter();
				XsltArgumentList leavePalletArgs = new XsltArgumentList();
				leavePalletArgs.AddParam("InstructionTypeId", "", "3");
				leavePalletArgs.AddParam("DocketText", "", m_organisation.DocketNumberText);
				leavePalletArgs.AddParam("webserver", "", Orchestrator.Globals.Configuration.WebServer);
				transformer.Transform(navigator, leavePalletArgs, sw);
				lblLeavePallets.Text = sw.GetStringBuilder().ToString();
				sw = new StringWriter();
				XsltArgumentList dehirePalletArgs = new XsltArgumentList();
				dehirePalletArgs.AddParam("InstructionTypeId", "", "4");
				dehirePalletArgs.AddParam("DocketText", "", m_organisation.DocketNumberText);
				dehirePalletArgs.AddParam("webserver", "", Orchestrator.Globals.Configuration.WebServer);
				transformer.Transform(navigator, dehirePalletArgs, sw);
				lblDeHirePallets.Text = sw.GetStringBuilder().ToString();

                if (lblLeavePallets.Text == lblDeHirePallets.Text)
				{
					lblLeavePallets.Text = "No Pallet Handling has been configured for this job.";
					lblDeHirePallets.Text = String.Empty;
				}
			}

            LoadEmptyPallets();
		}

		private void LoadJob()
		{
			using (Facade.IJob facJob = new Facade.Job())
			{
				m_job = facJob.GetJob(m_jobId);

				if (m_job.JobState == eJobState.Cancelled)
                    Response.Redirect("../../Job/job.aspx?wiz=true&jobId=" + m_job.JobId + "&csid=" + this.CookieSessionID);

                Facade.IInstruction facInstruction = new Facade.Instruction();
                m_job.Instructions = facInstruction.GetForJobId(m_job.JobId);
            }

			ViewState[C_JOB_VS] = m_job;
		}

        private void LoadEmptyPallets()
        {
            Facade.IJob facJob = new Facade.Job();
            DataSet ds = facJob.GetUnhandledPalletsForJob(m_jobId, null);

            lvTrailerPallets.DataSource = ds;
            lvTrailerPallets.DataBind();

            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                trEmptyPallets.Style.Remove("display");
            else
                trEmptyPallets.Style.Add("display", "none");
        }

		#endregion

		#region Events

        void generic_DialogCallBack(object sender, EventArgs e)
        {
            BindJob();
        }

		#endregion
	}
}
