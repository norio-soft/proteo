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
    public partial class RemoveTrunk : Orchestrator.Base.BasePage
    {
        #region Protected Fields
        protected int m_instructionID = 0;
        protected int m_jobId = 0;
        protected Entities.Instruction m_instruction = null;
        protected Entities.Instruction m_previousInstruction = null;
        #endregion

        #region Page Load/Init

        protected void Page_Load(object sender, EventArgs e)
        {
            m_instructionID = int.Parse(Request.QueryString["iID"]);
            m_jobId = int.Parse(Request.QueryString["jobId"]);
            PopulateForm();               
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnRemoveTrunk.Click += new EventHandler(btnRemoveTrunk_Click);
            this.btnCancel.Click += new EventHandler(btnCancel_Click);
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void btnRemoveTrunk_Click(object sender, EventArgs e)
        {
           RemoveTrunkInstruction();
        }

        private string GetResponse()
        {
            string retVal = "<RemoveTrunkLeg />";
            return retVal;
        }

        #endregion

        #region Page Setup 
        private void PopulateForm()
        {
            Facade.IInstruction facInstruction = new Facade.Instruction();
            m_instruction = facInstruction.GetInstruction(m_instructionID);

            bool canRemoveTrunkPoint = false;
            if (m_instruction != null)
            {
                if (m_instruction.InstructionTypeId == (int)eInstructionType.Trunk && (m_instruction.CollectDrops.Count == 0 || m_instruction.CollectDrops[0].OrderID == 0))
                    canRemoveTrunkPoint = true; // This instruction is either a trunk or a trunk that has zero orders.

                // Get the previous instruction.
                Entities.InstructionCollection instructions = facInstruction.GetForJobId(m_instruction.JobId);
                m_previousInstruction = instructions.GetPreviousInstruction(m_instruction.InstructionID);
            }

            pnlConfirmation.Visible = canRemoveTrunkPoint;
            pnlCannotRemoveTrunk.Visible = !canRemoveTrunkPoint;
            btnRemoveTrunk.Visible = canRemoveTrunkPoint;
        }
        #endregion

        #region Trunking Methods


        private void RemoveTrunkInstruction()
        {
            Entities.CustomPrincipal user = (Entities.CustomPrincipal)Page.User;

            Facade.IJob facJob = new Facade.Job();
            Entities.FacadeResult result = facJob.RemoveInstruction(m_jobId, m_instructionID, user.UserName);

            if (result.Success)
            {
                this.ReturnValue = "refresh";
                this.Close();
            }
            else
            {
                infrigementDisplay.Infringements = result.Infringements;
                infrigementDisplay.DisplayInfringments();
            }


        }
        #endregion

        #region Properties

        protected string LegStart
        {
            get { return m_previousInstruction.Point.Description; }
        }

        protected string StartPointId
        {
            get { return m_previousInstruction.Point.PointId.ToString(); }
        }

        protected string LegEnd
        {
            get { return m_instruction.Point.Description; }
        }

        protected string EndPointId
        {
            get { return m_instruction.Point.PointId.ToString(); }
        }

        #endregion
    }
}