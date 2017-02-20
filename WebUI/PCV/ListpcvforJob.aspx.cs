using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Orchestrator.WebUI;

namespace Orchestrator.WebUI.PCV
{
    public partial class ListpcvforJob : Orchestrator.Base.BasePage
    {
        private int l_jobID = -1;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                ConfigureDisplay();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.btnClose.Click += new EventHandler(btnClose_Click);
        }

        private void ConfigureDisplay()
        {
            if(!string.IsNullOrEmpty(Request.QueryString["JobID"]))
                int.TryParse(Request.QueryString["JobID"], out l_jobID);

            Facade.PCV facPCV = new Facade.PCV();
            this.lvPCVs.DataSource = facPCV.GetForJobIdDataSet(l_jobID);
            this.lvPCVs.DataBind();
        }

        void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
