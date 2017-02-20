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
    public partial class GetJobDataForJobPopUp : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            GetQueryStringVariables();
        }

        protected int m_jobId = 0;
        protected int m_legPointId = 0;
        protected int m_PointId = 0;
        protected int m_instructionId = 0;
        protected eInstructionType m_instructionType;
        protected DateTime m_bookedDateTime;
        protected bool  m_isAnyTime = false;
        protected DateTime m_plannedStartDateTime;
        protected DateTime m_plannedEndDateTime;
        protected Entities.Point m_point = null;
        protected bool m_hasInstruction = false;

        private void GetQueryStringVariables()
        {

            m_jobId = int.Parse(Request.QueryString["JobID"]);
            DataSet dsJobDetails = null;

            DataAccess.IJob dacJob = new DataAccess.Job();
            dsJobDetails = dacJob.GetJobDetailsLite(m_jobId);
            dacJob = null;
            
            lblJobId.Text = m_jobId.ToString();

            rptLegs.DataSource = dsJobDetails.Tables[0];
            rptLegs.DataBind();
        }


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.rptLegs.ItemDataBound += new RepeaterItemEventHandler(rptLegs_ItemDataBound);
        }

        void rptLegs_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                ((Label)e.Item.FindControl("lblFromOrganisation")).Text = ((DataRowView)e.Item.DataItem)["FromOrganisation"].ToString();
                ((Label)e.Item.FindControl("lblFromPoint")).Text = ((DataRowView)e.Item.DataItem)["FromPoint"].ToString();
                ((Label)e.Item.FindControl("lblToOrganisation")).Text = ((DataRowView)e.Item.DataItem)["ToOrganisation"].ToString();
                ((Label)e.Item.FindControl("lblToPoint")).Text = ((DataRowView)e.Item.DataItem)["ToPoint"].ToString();
                ((Label)e.Item.FindControl("lblFromDateTime")).Text = ((DateTime)((DataRowView)e.Item.DataItem)["LegPlannedStartDateTime"]).ToString("dd/MM HH:mm");
                ((Label)e.Item.FindControl("lblToDateTime")).Text = ((DateTime)((DataRowView)e.Item.DataItem)["LegPlannedEndDateTime"]).ToString("dd/MM HH:mm");
            }
        }
    }
}