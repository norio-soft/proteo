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
    public partial class setTrafficArea : Orchestrator.Base.BasePage
    {
        private const string C_Instruction_VS = "C_Instruction_VS";

        private int m_instructionID;
        private DateTime m_lastUpdateDate;
        private Entities.Instruction m_instruction;

        protected void Page_Load(object sender, EventArgs e)
        {
            m_instructionID = Convert.ToInt32(Request.QueryString["InstructionID"]);
            m_lastUpdateDate = DateTime.Parse(Request.QueryString["LastUpdateDate"]);

            if (!IsPostBack)
                InitialisePage();
            else
                m_instruction = (Entities.Instruction)ViewState[C_Instruction_VS];
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.btnConfirm.Click += new EventHandler(btnConfirm_Click);
            this.btnCancel.Click += new EventHandler(btnCancel_Click);
        }

        void btnConfirm_Click(object sender, EventArgs e)
        {
            int newControlAreaId = Convert.ToInt32(cboControlArea.SelectedValue);
            int newTrafficAreaId = Convert.ToInt32(cboTrafficArea.SelectedValue);

            bool success = true;

            if (newControlAreaId != m_instruction.ControlAreaId || newTrafficAreaId != m_instruction.TrafficAreaId)
            {
                // Update the control area and traffic area for the leg, and it's following legs if the checkbox is checked.
                // If the checkbox is checked, the resources on the leg are moved onto the receiving planner's resource list.
                Facade.IJob facJob = new Facade.Job();

                // The end instruction of the leg should always be passed when calling SetControlArea. This is different from 
                // m_instruction, which is the start instruction of the leg. The Start instruction of the leg detertmines the 
                // traffic area responsible for planning the leg.
                int instructionOrder = m_instruction.InstructionOrder + 1;

                if (m_instruction.InstructionOrder == 0)
                {
                    // The instruction is the 1st on the job
                    // Edge case: ensure it is not the only one (ie. collection only job)
                    // If it is then we shouldn't increment the instruction order when calling SetControlArea.
                    Facade.IInstruction facInstruction = new Facade.Instruction();
                    Entities.InstructionCollection instructions = facInstruction.GetForJobId(m_instruction.JobId);

                    if (instructions.Count == 1)
                        instructionOrder--;
                }

                Entities.FacadeResult result = facJob.SetControlArea(m_instruction.JobId, newControlAreaId, newTrafficAreaId, instructionOrder, chkApplyToAllFollowingInstructions.Checked, m_lastUpdateDate, ((Entities.CustomPrincipal)Page.User).UserName);
                success = result.Success;

                if (!success)
                {
                    // Display the infringements.
                    idErrors.Infringements = result.Infringements;
                    idErrors.DisplayInfringments();
                    idErrors.Visible = true;
                    trErrors.Visible = true;
                }
            }

            if (success)
            {
                this.ReturnValue = "refresh";
                this.Close();
            }
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void InitialisePage()
        {
            // Retrieve the instruction.
            Facade.IInstruction facInstruction = new Facade.Instruction();
            m_instruction = facInstruction.GetInstruction(m_instructionID);
            ViewState[C_Instruction_VS] = m_instruction;

            // Populate the control and traffic area dropdowns.
            Facade.IControlArea facControlArea = new Facade.Traffic();
            cboControlArea.DataSource = facControlArea.GetAll();
            cboControlArea.DataBind();
            cboTrafficArea.DataSource = ((Facade.ITrafficArea)facControlArea).GetAll();
            cboTrafficArea.DataBind();

            // Select the current control and traffic area assigned to the leg.
            cboControlArea.Items.FindByValue(m_instruction.ControlAreaId.ToString()).Selected = true;
            cboTrafficArea.Items.FindByValue(m_instruction.TrafficAreaId.ToString()).Selected = true;

            // Enable the checkbox if this is not the last leg in the job.
            Facade.IJob facJob = new Facade.Job();
            Entities.InstructionCollection instructions = facInstruction.GetForJobId(m_instruction.JobId);
            if (instructions.Count == m_instruction.InstructionOrder + 1)
                chkApplyToAllFollowingInstructions.Enabled = false;
            else
                chkApplyToAllFollowingInstructions.Enabled = true;
        }
    }
}