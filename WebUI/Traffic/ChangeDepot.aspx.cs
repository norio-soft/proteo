using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Orchestrator.WebUI.Traffic
{
    public partial class ChangeDepot : Orchestrator.Base.BasePage
    {
        private int InstructionID
        {
            get
            {
                int jobID = -1;
                int.TryParse(Request.QueryString["iid"], out jobID);
                return jobID;
            }
        }

        private int JobID
        {
            get
            {
                int jobID = -1;
                int.TryParse(Request.QueryString["jid"], out jobID);
                return jobID;
            }
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnUpdate.Click += btnUpdate_Click;
            btnCancel.Click += btnCancel_Click;
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            this.ReturnValue = "";
            this.Close();
        }

        void btnUpdate_Click(object sender, EventArgs e)
        {
            int selectedControlArea = int.Parse(cboControlArea.SelectedValue);
            string auditDetails = "";
            //if (rblType.SelectedValue == "Leg")
            //{
            //    var instruction = EF.DataContext.Current.InstructionSet.Include("Point").First(i => i.InstructionId == this.InstructionID);
            //    instruction.ControlAreaId = selectedControlArea;
            //    auditDetails = string.Format("Leg from {0} changed from {1} to {2}", instruction.Point.Description, lblCurrentDepot.Text, cboControlArea.SelectedItem.Text);
            //}
            //else
            //{
                var job = EF.DataContext.Current.JobSet.Include("Instructions").First(j => j.JobId == this.JobID);
                job.ControlAreaId = selectedControlArea;
                foreach (var instruction in job.Instructions)
                    instruction.ControlAreaId = job.ControlAreaId;
                auditDetails = string.Format("Run changed from {0} to {1}", lblCurrentDepot.Text, cboControlArea.SelectedItem.Text);
            //}
            EF.DataContext.Current.SaveChanges();
            DataAccess.Audit dacAudit = new DataAccess.Audit();
            dacAudit.AuditEvent(51, this.JobID, auditDetails, Page.User.Identity.Name);

            this.ReturnValue = "changedepot";
            this.Close();
            
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateStaticControls();
                var job = EF.DataContext.Current.JobSet.First(j=>j.JobId == this.JobID);
                lblCurrentDepot.Text = cboControlArea.Items.FindByValue(job.ControlAreaId.ToString()).Text;


            }
        }

        private void PopulateStaticControls()
        {
            Facade.IControlArea facControlArea = new Facade.Traffic();
            cboControlArea.DataSource = facControlArea.GetAll();
            cboControlArea.DataTextField = "Description";
            cboControlArea.DataValueField = "ControlAreaId";
            cboControlArea.DataBind();
        }
    }
}