using System;
using System.Collections.Generic;
using System.Linq;

namespace Orchestrator.WebUI.Job
{
    public partial class ConvertInstruction : Orchestrator.Base.BasePage
    {
        private const string C_InstructionID_QSKey = "iid"; // The query string parameter that should identify which instruction to convert.
        private Dictionary<int, string> _businessTypeDescriptions = new Dictionary<int, string>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ValidateMandatoryParameters();
                BindInstruction();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.grdOrdersOnDrop.ItemDataBound += grdOrdersOnDrop_ItemDataBound;
            this.cfvPoint.ServerValidate += cfvPoint_ServerValidate;
            this.btnConfirmDropConversion.Click += btnConfirmDropConversion_Click;
            this.btnCancelDropConversion.Click += btnCancelConversion_Click;

            this.grdOrdersOnTrunk.ItemDataBound += grdOrdersOnTrunk_ItemDataBound;
            this.btnConfirmTrunkConversion.Click += btnConfirmTrunkConversion_Click;
            this.btnCancelTrunkConversion.Click += btnCancelConversion_Click;
        }

        private void BindInstruction()
        {
            var instruction = GetInstruction();

            switch ((eInstructionType)instruction.InstructionTypeId)
            {
                case eInstructionType.Drop:
                    BindDropInstruction(instruction);
                    break;
                case eInstructionType.Trunk:
                    BindTrunkInstruction(instruction);
                    break;
            }
        }

        private string GetBusinessTypeDescription(int businessTypeID)
        {
            string description = String.Empty;

            if (!_businessTypeDescriptions.TryGetValue(businessTypeID, out description))
            {
                // Load the description from the database and add it to the dictionary.
                Facade.BusinessType facBusinessType = new Facade.BusinessType();
                var bt = facBusinessType.GetForBusinessTypeID(businessTypeID);
                description = bt.Description;
                _businessTypeDescriptions.Add(bt.BusinessTypeID, bt.Description);
            }

            return description;
        }

        private string GetAssignedResource(Entities.Job job, Entities.Instruction instruction, Entities.Order order)
        {
            if (instruction.Driver != null)
                return instruction.Driver.Individual.FullName;
            else
            {
                // May be sub-contracted.
                var jobSubContracts = job.SubContractors.Cast<Entities.JobSubContractor>();
                var facOrganisation = new Facade.Organisation();

                if (jobSubContracts.Any(jsc => jsc.SubContractWholeJob))
                {
                    // Whole job sub-contracted out.
                    return facOrganisation.GetNameForIdentityId(jobSubContracts.First(jsc => jsc.SubContractWholeJob).ContractorIdentityId);
                }
                else if (jobSubContracts.Any(jsc => jsc.JobSubContractID == instruction.JobSubContractID))
                {
                    // Per-instruction sub-contracted out.
                    return facOrganisation.GetNameForIdentityId(jobSubContracts.First(jsc => jsc.JobSubContractID == instruction.JobSubContractID).ContractorIdentityId);
                }
                else if (instruction.InstructionTypeId == (int)eInstructionType.Drop && jobSubContracts.Any(jsc => jsc.JobSubContractID == order.JobSubContractID))
                {
                    // Sub-contracted for delivery.
                    return facOrganisation.GetNameForIdentityId(jobSubContracts.First(jsc => jsc.JobSubContractID == order.JobSubContractID).ContractorIdentityId);
                }
            }

            // No resource assigned.
            return String.Empty;
        }

        private void btnCancelConversion_Click(object sender, EventArgs e)
        {
            // Close this window.
            this.ReturnValue = "refresh";
            this.Close();
        }

        private Entities.Instruction GetInstruction()
        {
            int instructionID = int.Parse(Request.QueryString[C_InstructionID_QSKey]);
            return GetInstruction(instructionID);
        }

        private Entities.Instruction GetInstruction(int instructionID)
        {
            Facade.Instruction facInstruction = new Facade.Instruction();
            return facInstruction.GetInstruction(instructionID);
        }

        /// <summary>
        /// Validates the mandatory parameters expected by this page.
        /// </summary>
        private void ValidateMandatoryParameters()
        {
            int result;

            if (String.IsNullOrEmpty(Request.QueryString[C_InstructionID_QSKey]))
                throw new ArgumentNullException(C_InstructionID_QSKey);
            if (!int.TryParse(Request.QueryString[C_InstructionID_QSKey], out result))
                throw new ArgumentException("The instruction id must be a valid integer.", C_InstructionID_QSKey);
            var instruction = GetInstruction();
            if (instruction == null)
                throw new ArgumentException(String.Format("No instruction for supplied instruction id '{0}' exists.", result), C_InstructionID_QSKey);
            if (instruction.InstructionTypeId != (int)eInstructionType.Drop && instruction.InstructionTypeId != (int)eInstructionType.Trunk)
                throw new NotSupportedException(String.Format("The instruction supplied should be either a drop or trunk instruction.  Instruction Id '{0}' supplied, with type of '{1}'.", instruction.InstructionID, Enum.GetName(typeof(eInstructionType), instruction.InstructionTypeId)));
            if (instruction.InstructionActuals != null && instruction.InstructionActuals.Count != 0)
                throw new NotSupportedException(String.Format("You can not convert a called in instruction.  Instruction Id '{0}' supplied.", instruction.InstructionID));
        }

        protected string GetDate(DateTime date, bool anytime)
        {
            string retVal = string.Empty;

            if (anytime)
                retVal = date.ToShortDateString() + " AnyTime";
            else
                retVal = date.ToShortDateString() + " " + date.ToShortTimeString();

            return retVal;
        }

        private class ConvertInstructionOrderData
        {
            public int OrderID { get; set; }
            public string CustomerOrganisationName { get; set; }
            public string BusinessTypeDescription { get; set; }
            public string OrderServiceLevel { get; set; }
            public string CustomerOrderNumber { get; set; }
            public string DeliveryOrderNumber { get; set; }
            public string Message { get; set; }
            public int CollectionPointID { get; set; }
            public string CollectionPointDescription { get; set; }
            public DateTime CollectionDateTime { get; set; }
            public bool CollectionIsAnyTime { get; set; }
            public int DeliveryPointID { get; set; }
            public string DeliveryPointDescription { get; set; }
            public DateTime DeliveryDateTime { get; set; }
            public bool DeliveryIsAnyTime { get; set; }
            public string DeliveringResource { get; set; }
            public int LCID { get; set; }
            public decimal ForeignRate { get; set; }

            public ConvertInstructionOrderData() { }
        }
    }
}