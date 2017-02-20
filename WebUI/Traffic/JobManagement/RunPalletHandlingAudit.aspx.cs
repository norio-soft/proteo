using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;

namespace Orchestrator.WebUI.Traffic.JobManagement
{
    public partial class RunPalletHandlingAudit : Orchestrator.Base.BasePage
    {
        private const string vs_JobId = "vs_JobId";
        public int JobId
        {
            get { return ViewState[vs_JobId] == null ? -1 : (int)ViewState[vs_JobId]; }
            set { ViewState[vs_JobId] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                ConfigureDisplay();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnRefresh.Click += new EventHandler(btnRefresh_Click);
            btnClose.Click += new EventHandler(btnClose_Click);
        }

        private void ConfigureDisplay()
        {
            int jobId = -1;

            if (int.TryParse(Request.QueryString["jID"], out jobId))
                JobId = jobId;

            Rebind();
        }

        private void Rebind()
        {
            using (Facade.IPalletBalance facPalletBalance = new Facade.Pallet())
            {
                lvTrailerAudit.DataSource = facPalletBalance.GetRunAuditTrail(JobId);
                lvTrailerAudit.DataBind();
            }
        }

        void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void btnRefresh_Click(object sender, EventArgs e)
        {
            Rebind();
        }

    }
}
