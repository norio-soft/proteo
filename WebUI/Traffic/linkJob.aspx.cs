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

using System.Collections.Generic;

namespace Orchestrator.WebUI.Traffic
{
    public partial class linkJob : Orchestrator.Base.BasePage
    {
        private int m_jobId = 0;
        private int m_sourceJobId = 0;
        private int m_sourceInstructionId = 0;
        private DataSet m_dsLinkedLegs = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            m_jobId = Convert.ToInt32(Request.QueryString["jobId"]);

            if (Request.QueryString["SourceJobJobId"] == "undefined")
            {
                // Check for possiblity of updating existing links.
                Facade.IJob facJob = new Facade.Job();
                m_dsLinkedLegs = facJob.GetJobsLinks(m_jobId);

                if (m_dsLinkedLegs.Tables[0].Rows.Count > 0)
                {
                    m_sourceJobId = (int)m_dsLinkedLegs.Tables[0].Rows[0]["JobId"];
                    m_sourceInstructionId = (int)m_dsLinkedLegs.Tables[0].Rows[0]["SourceInstructionId"];

                    if (!IsPostBack)
                    {
                        chkLinkDriver.Checked = (bool)m_dsLinkedLegs.Tables[0].Rows[0]["LinkDriver"];
                        chkLinkVehicle.Checked = (bool)m_dsLinkedLegs.Tables[0].Rows[0]["LinkVehicle"];
                        chkLinkTrailer.Checked = (bool)m_dsLinkedLegs.Tables[0].Rows[0]["LinkTrailer"];
                    }
                }
            }
            else
            {
                m_sourceJobId = Convert.ToInt32(Request.QueryString["SourceJobJobId"]);
                m_sourceInstructionId = Convert.ToInt32(Request.QueryString["SourceJobInstructionId"]);
            }

            if (m_sourceJobId == 0 || m_sourceInstructionId == 0)
            {
                pnlLinkJobs.Visible = false;
                pnlError.Visible = true;
                btnConfirm.Visible = false;
            }
            else
                if (!IsPostBack)
                    BindLegs();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            chkCheckAll.CheckedChanged += new EventHandler(chkCheckAll_CheckedChanged);
            repLegs.ItemDataBound += new RepeaterItemEventHandler(repLegs_ItemDataBound);

            btnCancel.Click += new EventHandler(btnCancel_Click);
            btnConfirm.Click += new EventHandler(btnConfirm_Click);
        }

        void chkCheckAll_CheckedChanged(object sender, EventArgs e)
        {
            foreach (RepeaterItem repItem in repLegs.Items)
                if (repItem.ItemType == ListItemType.Item || repItem.ItemType == ListItemType.AlternatingItem)
                    ((CheckBox)repItem.FindControl("chkThisLeg")).Checked = chkCheckAll.Checked;
        }

        void repLegs_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (m_dsLinkedLegs != null)
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    foreach (DataRow row in m_dsLinkedLegs.Tables[0].Rows)
                        if (row["TargetInstructionId"].ToString() == ((HtmlInputHidden)e.Item.FindControl("hidInstructionId")).Value)
                            ((CheckBox)e.Item.FindControl("chkThisLeg")).Checked = true;
                }
            }
        }

        void btnConfirm_Click(object sender, EventArgs e)
        {
            LinkJobs();
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            mwhelper.CausePostBack = false;
            mwhelper.CloseForm = true;
        }

        private void BindLegs()
        {
            Facade.IJob facJob = new Facade.Job();
            Entities.Job job = facJob.GetJob(m_jobId, true, true);

            Entities.LegPlan lp = new Facade.Instruction().GetLegPlan(job.Instructions, false);
            repLegs.DataSource = lp.Legs();
            repLegs.DataBind();

            if (job.Instructions.Count > 0)
            {
                if (job.Instructions[0].Driver != null)
                    chkLinkDriver.Text = "Link " + job.Instructions[0].Driver.Individual.FullName;
                if (job.Instructions[0].Vehicle != null)
                    chkLinkVehicle.Text = "Link " + job.Instructions[0].Vehicle.RegNo;
                if (job.Instructions[0].Trailer != null)
                    chkLinkTrailer.Text = "Link " + job.Instructions[0].Trailer.TrailerRef;
            }
        }

        private void LinkJobs()
        {
            bool linkDriver = chkLinkDriver.Checked;
            bool linkVehicle = chkLinkVehicle.Checked;
            bool linkTrailer = chkLinkTrailer.Checked;

            List<int> legIds = new List<int>();

            foreach (RepeaterItem repItem in repLegs.Items)
                if (repItem.ItemType == ListItemType.Item || repItem.ItemType == ListItemType.AlternatingItem)
                {
                    CheckBox chkThisLeg = (CheckBox)repItem.FindControl("chkThisLeg");

                    if (chkThisLeg.Checked)
                    {
                        HtmlInputHidden hidInstructionId = (HtmlInputHidden)repItem.FindControl("hidInstructionId");
                        legIds.Add(int.Parse(hidInstructionId.Value));
                    }
                }

            DateTime lastUpdateDate = DateTime.Parse(Request.QueryString["LastUpdateDate"]);

            Facade.IJob facJob = new Facade.Job();
            Entities.FacadeResult result = facJob.LinkJobs(m_sourceInstructionId, m_jobId, legIds, linkDriver, linkVehicle, linkTrailer, lastUpdateDate, ((Entities.CustomPrincipal)Page.User).UserName);

            if (result.Success)
            {
                mwhelper.CloseForm = true;
                mwhelper.CausePostBack = true;
                mwhelper.OutputData = "<linkJobs />";
            }
            else
            {
                infrigementDisplay.Infringements = result.Infringements;
                infrigementDisplay.DisplayInfringments();
                infrigementDisplay.Visible = true;
            }
        }
    }
}