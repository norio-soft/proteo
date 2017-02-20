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

namespace Orchestrator.WebUI.Traffic
{
    public partial class LoadOrder : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int jobId = int.Parse(Request.QueryString["JobId"]);
                DisplayLoadInformation(jobId);

            }
        }

        private void DisplayLoadInformation(int jobid)
        {
            Facade.IJob facJob = new Facade.Job();
            gvLoadOrder.DataSource = facJob.GetLoadOrderForJobId(jobid);
            gvLoadOrder.DataBind();
        }
    }
}