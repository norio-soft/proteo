using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;


namespace Orchestrator.WebUI.Job.Wizard.UserControls
{
    public partial class AddRate : System.Web.UI.UserControl, IDefaultButton
    {

        #region IDefaultButton
        public System.Web.UI.Control DefaultButton
        {
            get { return this.btnNext; }
        }
        #endregion
        private Entities.Job m_job = null;

        protected int m_collectionPointId = 0;
        protected string m_collectionPoint = string.Empty;
        protected int m_deliveryPointId = 0;
        protected string m_deliveryPoint = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            RecoverControlVariables();

            if (!IsPostBack)
            {
                btnCancel.Attributes.Add("onClick", wizard.C_CONFIRM_MESSAGE);
                btnNext.Attributes.Add("onClick", "javascript:HidePage();");
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnBack.Click += new EventHandler(btnBack_Click);
            btnNext.Click += new EventHandler(btnNext_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
        }

        private void RecoverControlVariables()
        {
            m_collectionPointId = Convert.ToInt32(Session[wizard.C_COLLECTION_POINT_ID]);
            m_collectionPoint = (string)Session[wizard.C_COLLECTION_POINT];
            m_deliveryPointId = Convert.ToInt32(Session[wizard.C_DELIVERY_POINT_ID]);
            m_deliveryPoint = (string)Session[wizard.C_DELIVERY_POINT];
        }

        #region Event Handlers & Events

        private void GoToStep(string step)
		{
			string url = "wizard.aspx?step=" + step;
            if (m_job != null && m_job.JobId > 0)
                url += "&jobId=" + m_job.JobId.ToString();

			Response.Redirect(url);
		}

		private void btnBack_Click(object sender, EventArgs e)
		{
			// Clear the session variables used to help add the new rate
			Session[wizard.C_COLLECTION_POINT_ID] = null;
			Session[wizard.C_DELIVERY_POINT_ID] = null;
            Session[wizard.C_COLLECTION_POINT] = null;
            Session[wizard.C_DELIVERY_POINT] = null;

			GoToStep("JD");
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			GoToStep("CANCEL");
		}

		private void btnNext_Click(object sender, EventArgs e)
		{
			if (Page.IsValid)
			{
                m_job = (Entities.Job) Session[wizard.C_JOB];

                // Populate the job rate.
                Entities.JobRate jobRate = new Entities.JobRate();
                jobRate.IdentityId = m_job.IdentityId;
                jobRate.CollectionPointId = m_collectionPointId;
                jobRate.DeliveryPointId = m_deliveryPointId;
                jobRate.FullLoadRate = Decimal.Parse(txtFullLoadRate.Text);
                jobRate.PartLoadRate = Decimal.Parse(txtPartLoadRate.Text);
                jobRate.MultiDropRate = Decimal.Parse(txtMultiDropRate.Text);

                jobRate.StartDate = DateTime.UtcNow;
                if (! dteEndDate.SelectedDate.HasValue)
                {
                    // Not great.
                    jobRate.EndDate = new DateTime(1753, 1, 1, 0, 0, 0);
                }
                else
                    jobRate.EndDate = dteEndDate.SelectedDate.Value;

                // Create the new rate.
                Facade.IJobRate facJobRate = new Facade.Job();
                int jobRateId = facJobRate.Create(jobRate, ((Entities.CustomPrincipal)Page.User).UserName);

                if (jobRateId > 0)
                    if (m_job.JobId > 0)
                        GoToStep("PC");
                    else
					    GoToStep("JD");
			}
		}

		#endregion 
    }
}