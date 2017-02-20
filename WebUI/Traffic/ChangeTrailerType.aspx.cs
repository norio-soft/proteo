using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Orchestrator.WebUI.Traffic
{
    public partial class ChangeTrailerType : Orchestrator.Base.BasePage
    {
        private int JobID
        {
            get
            {
                int jobID = -1;
                int.TryParse(Request.QueryString["jid"], out jobID);
                return jobID;
            }
        }

        private Entities.InstructionCollection Instrutions
        {
            get
            {
                if (ViewState["_instructions"] == null)
                {
                    Facade.IInstruction facInstruction = new Facade.Instruction();
                    Entities.InstructionCollection instructions = facInstruction.GetForJobId(this.JobID);
                    ViewState["_instructions"] = instructions;
                }
                return ViewState["_instructions"] as Entities.InstructionCollection;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                cboType.DataSource = this.DataContext.PlanningCategorySet.OrderBy(pc => pc.DisplayShort);
                cboType.DataTextField = "DisplayShort";
                cboType.DataValueField = "ID";
                cboType.DataBind();

                ListItem li = new ListItem("--Please select--", "");
                cboType.Items.Insert(0, li);
                cboType.Items.Add(new ListItem("Remove Trailer Type", "-1"));

                if (Instrutions[0].PlanningCategoryID == null)
                    lblType.Text = "None";
                else
                {
                    ListItem currentTrailerType = cboType.Items.FindByValue(Instrutions[0].PlanningCategoryID.ToString());
                    lblType.Text = currentTrailerType.Text;
                }

                
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnChangeType.Click += new EventHandler(btnChangeTrailerType_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            this.ReturnValue = "";
            this.Close();
        }

        void btnChangeTrailerType_Click(object sender, EventArgs e)
        {
           
            List<int> instructionIDs = new List<int>();
            foreach(Entities.Instruction instruction in this.Instrutions)
            {
                instructionIDs.Add(instruction.InstructionID);
            }
            Facade.IInstruction facInstruction = new Facade.Instruction();
            facInstruction.AssignPlanningCategory(instructionIDs, this.JobID, int.Parse(cboType.SelectedValue), DateTime.Now, Page.User.Identity.Name);
            this.ReturnValue = "changetrailertype";
            this.Close();
        }
    }
}
