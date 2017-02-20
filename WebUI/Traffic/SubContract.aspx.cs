using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.ObjectModel;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using System.Text;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Globalization;
using Orchestrator;

using Telerik.Web.UI;
using System.Linq;

namespace Orchestrator.WebUI.Traffic
{
    public partial class SubContract : Orchestrator.Base.BasePage
    {
        private enum SubContractingMethod { WholeJob, SpecificLegs, PerOrder };

        #region Form Fields

        protected int m_jobId = 0;
        private Entities.Job m_job = null;

        protected Entities.Job Job
        {
            get
            {
                if (m_job == null)
                {
                    m_job = (Entities.Job)Cache.Get("JobEntityForJobId" + m_jobId);
                    if (m_job == null)
                    {
                        Facade.IJob facJob = new Facade.Job();
                        m_job = facJob.GetJob(m_jobId, true, true);
                    }
                }

                return m_job;
            }
            set
            {
                m_job = value;
            }
        }

        protected bool m_isUpdate = false;

        #endregion

        #region Page Load/Init

        private bool FindAllCompletedInstructions(Entities.Instruction instruction)
        {
            return (instruction.InstructionState == eInstructionState.Completed);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
          
            Label PageTitle = ((System.Web.UI.UserControl)(Page.Master)).FindControl("lblWizardTitle") as Label;
            
            if (PageTitle != null)
                PageTitle.Text = "Sub Contract";

            m_jobId = int.Parse(Request.QueryString["jobId"]);
            m_isUpdate = (Request.QueryString["isUpdate"] != null && Request.QueryString["isUpdate"].ToString() == "true");

            if (!IsPostBack && Request.QueryString["rcbId"] == null)
            {
                // You can only sub-contract per order if the job is a groupage job.
                if (Job.JobType != eJobType.Groupage)
                    rdoSubContractMethod.Items.RemoveAt(2);

                // If any of the legs have been completed then the sub contract whole job should be disabled (how can you sub the whole
                // job to someone when part of the job has already been completed.
                ReadOnlyCollection<Entities.Instruction> completedInstructions = Job.Instructions.FindAll(new Predicate<Orchestrator.Entities.Instruction>(FindAllCompletedInstructions));
                if (completedInstructions != null && completedInstructions.Count > 0)
                {
                    // disable the whole job radio button.
                    rdoSubContractMethod.Items[0].Selected = false;
                    rdoSubContractMethod.Items[0].Enabled = false;
                }

                if (!m_isUpdate)
                    grdLegs.Visible = false;
                else
                    if (Request.QueryString["rcbId"] == null)
                    {
                        // Populate with the correct settings.
                        Facade.IJobSubContractor facJobSubContractor = new Facade.Job();
                        Facade.IOrganisation facOrganisation = new Facade.Organisation();
                        Entities.JobSubContractor jobSubContractor = facJobSubContractor.GetSubContractorForJobId(m_jobId)[0];
                        rntSubContractRate.Culture = new CultureInfo(jobSubContractor.LCID);

                        cboSubContractor.SelectedValue = jobSubContractor.ContractorIdentityId.ToString();
                        cboSubContractor.Text = facOrganisation.GetNameForIdentityId(jobSubContractor.ContractorIdentityId);
                        chkUseSubContractorTrailer.Checked = jobSubContractor.UseSubContractorTrailer;
                        rntSubContractRate.Text = jobSubContractor.ForeignRate.ToString();

                        cboSubContractor.Enabled = false;
                        chkUseSubContractorTrailer.Enabled = false;
                        pnlTrailer.Visible = true;
                    }

                if (Globals.Configuration.SubContractorCommunicationsRequired)
                {
                    lblShowAsCommunicated.Visible = true;
                    chkShowAsCommunicated.Visible = true;
                }
            }

            InjectScript.Text = string.Empty;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
           // ((WizardMasterPage)this.Master).HideScriptManager = true;
            cboSubContractor.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboSubContractor_ItemsRequested);
            //cboSubContractor.SelectedIndexChanged += new RadComboBoxSelectedIndexChangedEventHandler(cboSubContractor_SelectedIndexChanged);
//            cfvSubContrator.ServerValidate += new ServerValidateEventHandler(cfvSubContrator_ServerValidate);
            btnConfirmSubContract.Click += new EventHandler(btnConfirmSubContract_Click);

            this.chkUseSubContractorTrailer.CheckedChanged += new EventHandler(chkUseSubContractorTrailer_CheckedChanged);
            rdoSubContractMethod.SelectedIndexChanged += new EventHandler(rdoSubContractMethod_SelectedIndexChanged);
            this.grdLegs.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdLegs_NeedDataSource);
            this.grdLegs.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(grdLegs_ItemDataBound);
        }

        #endregion

        #region Control Events

        void cfvSubContrator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = Utilities.ValidateNumericValue(cboSubContractor.SelectedValue);
        }

        void chkUseSubContractorTrailer_CheckedChanged(object sender, EventArgs e)
        {
            this.pnlTrailer.Visible = !chkUseSubContractorTrailer.Checked;
        }

        void rdoSubContractMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            SubContractingMethod method = (SubContractingMethod)int.Parse(rdoSubContractMethod.SelectedValue);

            if (method == SubContractingMethod.WholeJob)
                grdLegs.Visible = false;
            else
            {
                grdLegs.Visible = true;
                grdLegs.Rebind();
            }
        }

        void btnConfirmSubContract_Click(object sender, EventArgs e)
        {
            if (Page.IsValid && ValidateSubContractLeg())
            {
                // Check whether the user is trying to subcontract instructions that have resources assigned.
                // Only applies to driver and vehicle resources.
                Entities.InstructionCollection instructions = Job.Instructions;

                // Extract the collections.
                Entities.Instruction selected = instructions.Find(
                    delegate(Entities.Instruction instruction)
                    {
                        return instruction.InstructionOrder == 0;
                    });

                // No resources exist for selected instructions, so continue with save oepration.
                SubContractingMethod method = (SubContractingMethod)int.Parse(rdoSubContractMethod.SelectedValue);
                string userID = ((Entities.CustomPrincipal)Page.User).UserName;
                
                Entities.JobSubContractor jobSubContractor = GetJobSubContract(selected);
                DateTime lastUpdateDate = DateTime.Parse(Request.QueryString["LastUpdateDate"]);

                // Set the job to be sub-contracted
                Entities.FacadeResult result = null;

                switch (method)
                {
                    case SubContractingMethod.WholeJob:
                        //add something that finds if any of the other legs have been subbed out already.
                        result = SubContractWholeJob(jobSubContractor, lastUpdateDate, userID);
                        break;
                    case SubContractingMethod.SpecificLegs:
                        result = SubContractSpecificLegs(jobSubContractor, lastUpdateDate, userID);
                        break;
                    case SubContractingMethod.PerOrder:
                        result = SubContractPerOrder(jobSubContractor, lastUpdateDate, userID);
                        break;
                }

                if (Globals.Configuration.SubContractorCommunicationsRequired
                    && chkShowAsCommunicated.Checked)
                    CommunicateInstructionsForSubContractor(Job.JobId, jobSubContractor.ContractorIdentityId, userID);

                if (result.Success)
                {
                    Cache.Remove("JobEntityForJobId" + m_jobId.ToString());
                    //InjectScript.Text = "<script>RefreshParentPage();</script>";
                    this.ReturnValue = "refresh";
                    this.Close();
                }
                else
                {
                    infrigementDisplay.Infringements = result.Infringements;
                    infrigementDisplay.DisplayInfringments();
                    infrigementDisplay.Visible = true;

                    pnlConfirmation.Visible = false;
                }
            }
        }

        private static bool CommunicateInstructionsForSubContractor(int jobID, int subContractorId, string userId)
        {
            if (!Globals.Configuration.SubContractorCommunicationsRequired)
                return false;

            Entities.DriverCommunication communication = new Entities.DriverCommunication();
            communication.Comments = "Communicated via Deliveries ScreSubcontractorCommunicationsRequireden";
            communication.DriverCommunicationStatus = eDriverCommunicationStatus.Accepted;
            string mobileNumber = "unknown";

            communication.DriverCommunicationType = eDriverCommunicationType.Manifest;
            communication.NumberUsed = mobileNumber;

            Facade.IInstruction facInstruction = new Facade.Instruction();
            List<int> instructionIds = facInstruction.GetInstructionIDsForSubContractor(jobID, subContractorId, false);

            Facade.IJobSubContractor facJob = new Facade.Job();
            foreach(int instructionId in instructionIds)
                communication.DriverCommunicationId = facJob.CreateCommunication(jobID, subContractorId, communication, userId);

            return true;
        }

        private string GetResponse()
        {
            string retVal = "<SubContractLeg />";
            return retVal;
        }

        private Entities.FacadeResult SubContractWholeJob(Entities.JobSubContractor jobSubContractor, DateTime lastUpdateDateTime, string userID)
        {
            Facade.IJobSubContractor facJobSubContractor = new Facade.Job();

            if (m_isUpdate)
                return facJobSubContractor.Update(m_jobId, jobSubContractor, lastUpdateDateTime, userID);
            else
            {
                if (chkUseSubContractorTrailer.Checked)
                    return facJobSubContractor.Create(m_jobId, new List<int>(), new List<int>(), jobSubContractor, lastUpdateDateTime, userID, chkForceRemoveResources.Checked);
                else
                    return facJobSubContractor.Create(m_jobId, new List<int>(), new List<int>(), jobSubContractor, int.Parse(cboTrailer.SelectedValue), lastUpdateDateTime, userID, chkForceRemoveResources.Checked);
            }
        }

        private Entities.FacadeResult SubContractSpecificLegs(Entities.JobSubContractor jobSubContractor, DateTime lastUpdateDateTime, string userID)
        {
            Facade.IJobSubContractor facJobSubContractor = new Facade.Job();

            if (!m_isUpdate)
            {
                List<int> instructionIDs = new List<int>();
                foreach (GridItem gdi in grdLegs.SelectedItems)
                {
                    int instructionID = int.Parse(gdi.OwnerTableView.DataKeyValues[gdi.ItemIndex]["InstructionID"].ToString());
                    instructionIDs.Add(instructionID);
                }

                if (chkUseSubContractorTrailer.Checked)
                    return facJobSubContractor.Create(m_jobId, instructionIDs, new List<int>(), jobSubContractor, lastUpdateDateTime, userID, chkForceRemoveResources.Checked);
                else
                    return facJobSubContractor.Create(m_jobId, instructionIDs, new List<int>(), jobSubContractor, int.Parse(cboTrailer.SelectedValue), lastUpdateDateTime, userID, chkForceRemoveResources.Checked);
            }
            else
                return GenerateValidationError("UnableToUpdate", "Unable to update sub-contracted information in this way");
        }

        private Entities.FacadeResult SubContractPerOrder(Entities.JobSubContractor jobSubContractor, DateTime lastUpdateDateTime, string userID)
        {
            Facade.IJobSubContractor facJobSubContractor = new Facade.Job();

            if (!m_isUpdate)
            {
                List<int> instructionIDs = new List<int>();
                List<int> orderIDs = new List<int>();
                foreach (GridItem gdi in grdLegs.SelectedItems)
                {
                    Label lblOrderIDs = (Label)gdi.FindControl("lblOrderIDs");
                    if (lblOrderIDs.Text == string.Empty)
                    {
                        int instructionID = int.Parse(gdi.OwnerTableView.DataKeyValues[gdi.ItemIndex]["InstructionID"].ToString());
                        instructionIDs.Add(instructionID);
                    }
                    else
                    {
                        string[] split = new string[] { ", " };
                        string[] arrOrderIDs = lblOrderIDs.Text.Split(split, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string orderID in arrOrderIDs)
                            orderIDs.Add(int.Parse(orderID));
                    }
                }

                if (chkUseSubContractorTrailer.Checked)
                    return facJobSubContractor.Create(m_jobId, instructionIDs, orderIDs, jobSubContractor, lastUpdateDateTime, userID, chkForceRemoveResources.Checked);
                else
                    return facJobSubContractor.Create(m_jobId, instructionIDs, orderIDs, jobSubContractor, int.Parse(cboTrailer.SelectedValue), lastUpdateDateTime, userID, chkForceRemoveResources.Checked);
            }
            else
                return GenerateValidationError("UnableToUpdate", "Unable to update sub-contracted information in this way");
        }

        bool ValidateSubContractLeg()
        {
            SubContractingMethod method = (SubContractingMethod)int.Parse(rdoSubContractMethod.SelectedValue);

            if (method == SubContractingMethod.SpecificLegs || method == SubContractingMethod.PerOrder)
            {
                if (grdLegs.SelectedItems.Count == 0)
                {
                    Entities.BusinessRuleInfringement bri = new Orchestrator.Entities.BusinessRuleInfringement("InvalidLegs", "You must select at least 1 Leg to sub-contract if you are not sub-contracting the whole job.");
                    infrigementDisplay.Infringements = new Orchestrator.Entities.BusinessRuleInfringementCollection();
                    infrigementDisplay.Infringements.Add(bri);
                    infrigementDisplay.DisplayInfringments();
                    return false;
                }
            }

            return true;
        }

        Entities.JobSubContractor GetJobSubContract(Entities.Instruction selected)
        {
            Facade.IExchangeRates facER = new Facade.ExchangeRates();
            decimal rate = Decimal.Parse(rntSubContractRate.Text, System.Globalization.NumberStyles.Currency);

            Entities.JobSubContractor jobSubContract = new Entities.JobSubContractor(int.Parse(cboSubContractor.SelectedValue), rate, chkUseSubContractorTrailer.Checked);
            jobSubContract.LCID = rntSubContractRate.Culture.LCID;

            CultureInfo culture = new CultureInfo(Orchestrator.Globals.Configuration.NativeCulture);

            if (jobSubContract.LCID != culture.LCID) // Default
            {
                jobSubContract.ExchangeRateID = facER.GetCurrentExchangeRateID(Facade.Culture.GetCurrencySymbol(jobSubContract.LCID), selected.PlannedArrivalDateTime);
                jobSubContract.Rate = facER.GetConvertedRate((int)jobSubContract.ExchangeRateID, jobSubContract.ForeignRate);
            }
            else
                jobSubContract.Rate = decimal.Round(jobSubContract.ForeignRate, 4, MidpointRounding.AwayFromZero);

            return jobSubContract;
        }

        Entities.FacadeResult GenerateValidationError(string key, string value)
        {
            Dictionary<string, string> infringements = new Dictionary<string, string>();
            infringements.Add(key, value);
            return GenerateValidationError(infringements);
        }

        Entities.FacadeResult GenerateValidationError(Dictionary<string, string> infringements)
        {
            Entities.FacadeResult result = new Orchestrator.Entities.FacadeResult(false);

            result.Infringements = new Entities.BusinessRuleInfringementCollection();
            foreach (string key in infringements.Keys)
            {
                Entities.BusinessRuleInfringement infringement = new Orchestrator.Entities.BusinessRuleInfringement(key, infringements[key]);
                result.Infringements.Add(infringement);
            }

            return result;
        }

        #endregion

        #region Leg Grid

        void grdLegs_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            ReadOnlyCollection<Entities.LegView> legs = new Facade.Instruction().GetLegPlan(Job.Instructions, true).FindAllLegs(this.FilterLegs);
            grdLegs.DataSource = legs;
        }

        public bool FilterLegs(Entities.LegView leg)
        {
            return (leg.State != Entities.LegView.eLegState.InProgress && leg.State != Entities.LegView.eLegState.Completed);
        }

        void grdLegs_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is Telerik.Web.UI.GridDataItem)
            {
                if (!string.IsNullOrEmpty(rdoSubContractMethod.SelectedValue))
                {
                    SubContractingMethod method = (SubContractingMethod)int.Parse(rdoSubContractMethod.SelectedValue);
                    Label lblOrderIDs = (Label)e.Item.FindControl("lblOrderIDs");

                    // Default the order ids to none (this will cause the rate to be added to the leg).
                    lblOrderIDs.Text= string.Empty;

                    if (method == SubContractingMethod.PerOrder)
                    {
                        Entities.Instruction instruction = null;
                        using (Facade.IInstruction facInstruction = new Facade.Instruction())
                        {
                            instruction = (e.Item.DataItem as Entities.LegView).Instruction;
                        }

                        if (instruction.InstructionTypeId == (int)eInstructionType.Drop && instruction.CollectDrops.Count > 0 && instruction.CollectDrops[0].OrderID > 0)
                        {
                            // This is a delivery instruction with an order - add all the orders to the hidden field.
                            StringBuilder orderIDs = new StringBuilder();
                            foreach (Entities.CollectDrop collectDrop in instruction.CollectDrops)
                                if (collectDrop.OrderID > 0)
                                {
                                    if (orderIDs.Length > 0)
                                        orderIDs.Append(", ");
                                    orderIDs.Append(collectDrop.OrderID.ToString());
                                }

                            lblOrderIDs.Text = orderIDs.ToString();
                        }
                    }

                    ///////////////////////////////////////////////////////////
                    Entities.LegView leg = e.Item.DataItem as Entities.LegView;

                    // InstructionId
                    HiddenField hidInstructionId = e.Item.FindControl("hidInstructionId") as HiddenField;
                    hidInstructionId.Value = leg.InstructionID.ToString();

                    // Collection Point
                    HtmlGenericControl spnCollectionPoint = e.Item.FindControl("spnCollectionPoint") as HtmlGenericControl;
                    spnCollectionPoint.Attributes.Add("class", "orchestratorLink");
                    spnCollectionPoint.InnerText = leg.StartLegPoint.Point.Description;

                    // Destination Point
                    HtmlGenericControl spnDestinationPoint = e.Item.FindControl("spnDestinationPoint") as HtmlGenericControl;
                    spnDestinationPoint.Attributes.Add("class", "orchestratorLink");
                    spnDestinationPoint.InnerText = leg.EndLegPoint.Point.Description;

                    // Driver
                    HtmlGenericControl spnDriver = e.Item.FindControl("spnDriver") as HtmlGenericControl;
                    spnDriver.Attributes.Add("class", "orchestratorLink");

                    if (leg.SubContractorForDisplay != null && leg.Driver == null)
                    {
                        // The driver is a subby. Pass the Organisation IdentityId.
                        spnDriver.InnerText = leg.SubContractorForDisplay.OrganisationDisplayName;
                    }
                    else if (leg.SubContractorForDisplay == null && leg.Driver != null)
                    {
                        // The driver is an inhouse resource. Pass Individual IdentityId.
                        // The driver is a subby. Pass the Organisation IdentityId.
                        spnDriver.InnerText = leg.Driver.Individual.FullName;
                    }

                    // Trailer
                    HtmlGenericControl spnTrailer = e.Item.FindControl("spnTrailer") as HtmlGenericControl;
                    spnTrailer.InnerText = leg.Trailer == null ? "" : leg.Trailer.TrailerRef;

                }
            }
        }

        #endregion

        #region Combo Box Loading

        void cboSubContractor_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboSubContractor.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllSubContractorsFiltered(e.Text, false);

            int itemsPerRequest = 5;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            DataTable dt = ds.Tables[0];
            Telerik.Web.UI.RadComboBoxItem rcItem = null;
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new Telerik.Web.UI.RadComboBoxItem();
                rcItem.Text = dt.Rows[i]["OrganisationName"].ToString();
                rcItem.Value = dt.Rows[i]["IdentityId"].ToString();
                cboSubContractor.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

        protected void cboSubContractor_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            int subbyIdentityId = int.Parse(cboSubContractor.SelectedValue);

            Facade.IOrganisation facOrg = new Facade.Organisation();
            rntSubContractRate.Culture = new CultureInfo(facOrg.GetCultureForOrganisation(subbyIdentityId));
        
            //Check for missing documents
            if (Globals.Configuration.AlertForMissingClientDocuments)
            {
                AlertForMissingSubbyDocuments(subbyIdentityId);
            }

        }

        private void AlertForMissingSubbyDocuments(int subbyIdentityId)
        {
            //Get the OrganisationDocuments for this Org
            List<EF.OrganisationDocument> orgDocs = (
                from od in EF.DataContext.Current.OrganisationDocuments.Include("ScannedForm.FormType")
                where od.Organisation.IdentityId == subbyIdentityId
                select od).ToList();

            string missingDocAlert = string.Empty;

            if (!orgDocs.Any(od => od.ScannedForm.FormType.FormTypeId == (int)eFormTypeId.SubbyTnCs))
                missingDocAlert += "The Subcontractor T&Cs document has not been scanned.<br />";

            if (!orgDocs.Any(od => od.ScannedForm.FormType.FormTypeId == (int)eFormTypeId.CreditApplicationForm))
                missingDocAlert += "The Credit Application Form has not been scanned.<br />";

            lblMissingDocumentsAlert.Text = missingDocAlert;
        }

        #endregion
    }
}
