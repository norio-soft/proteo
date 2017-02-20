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

namespace Orchestrator.WebUI.Job
{
    public partial class jobHistory : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int jobId = int.Parse(Request.QueryString["JobId"]);

            Facade.IAudit facAudit = new Facade.Audit();
            gvJobHistory.DataSource = facAudit.GetJobHistory(jobId);
            gvJobHistory.DataBind();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnClose.Click += new EventHandler(btnClose_Click);

        }

        void btnClose_Click(object sender, EventArgs e)
        {
            this.mwhelper.CloseForm = true;
            this.mwhelper.CausePostBack = false;
        }

     
    }
}