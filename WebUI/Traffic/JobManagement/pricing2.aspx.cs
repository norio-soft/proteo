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
using System.Web.UI.HtmlControls;

namespace Orchestrator.WebUI.Traffic.JobManagement
{
    public partial class Pricing2 : Orchestrator.Base.BasePage
    {

        #region CONSTANTS
        private const string C_JOB_VS = "C_JOB_VS";
        #endregion

        #region Fields
        protected int m_jobId = 0;

        private int m_chargeableLegStart;
        private int m_chargeableLegEnd;
        private int m_chargeableLegs;
        private decimal m_runningCostTotal = 0;
        private decimal m_runningChargeTotal = 0;
        private decimal m_runningRefusalCharge = 0;

        private decimal m_costAllocatedSoFar = 0;
        private decimal m_chargeAllocatedSoFar = 0;

        protected Entities.Job m_job = null;
        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage, eSystemPortion.Price);

            // Retrieve the job id from the QueryString
            m_jobId = Convert.ToInt32(Request.QueryString["jobId"]);
            if (m_jobId == 0)
                Response.Redirect("../../job/jobsearch.aspx");

            if (!IsPostBack)
            {
                btnUpdate.Enabled = Orchestrator.WebUI.Security.Authorise.CanAccess(eSystemPortion.Price);
                LoadJob();
                //txtRate.Attributes.Add("onblur", "updateRate(this);");
            }
            else
            {
                // Retrieve the m_job from ViewState
                m_job = (Entities.Job)ViewState[C_JOB_VS];
                pnlConfirmation.Visible = false;
                infringementDisplay.Visible = false;
            }
        }

        #endregion

        #region Overrides
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            MyClientSideAnchor.OnWindowClose += new Codesummit.WebModalAnchor.OnWindowCloseEventHandler(MyClientSideAnchor_OnWindowClose);
            gvLegs.RowDataBound += new GridViewRowEventHandler(gvLegs_RowDataBound);
            btnUpdate.Click += new EventHandler(btnUpdate_Click);
            cvRate.ServerValidate += new ServerValidateEventHandler(cvRate_ServerValidate);
            gvRefusals.RowDataBound += new GridViewRowEventHandler(gvRefusals_RowDataBound);
            odsRefusals.Selecting += new ObjectDataSourceSelectingEventHandler(odsRefusals_Selecting);
        }

        void odsRefusals_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            if (m_job.JobType == eJobType.Return)
            {
                e.InputParameters.Add("jobId", m_jobId);
            }
            else
            {
                e.Cancel = true;
            }
        }

        void gvRefusals_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ((TextBox)e.Row.FindControl("txtRefusalCharge")).Attributes.Add("onblur", "updateRefusalCharge(this);");

                m_runningRefusalCharge += decimal.Parse(((TextBox)e.Row.FindControl("txtRefusalCharge")).Text, System.Globalization.NumberStyles.Currency);
                e.Row.Attributes.Add("__key", ((DataRowView)e.Row.DataItem)["RefusalId"].ToString());
            }

            if (e.Row.RowType == DataControlRowType.Footer)
            {
                ((Label)e.Row.FindControl("lblTotalRefusalCharge")).Text = m_runningRefusalCharge.ToString("C");
            }
        }

        void cvRate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            decimal legCharge = 0;

            foreach (GridViewRow row in gvLegs.Rows)
            {
                TextBox txtCharge = (TextBox)row.FindControl("txtCharge");

                if (txtCharge.Visible == true)
                    legCharge += Decimal.Parse(txtCharge.Text, System.Globalization.NumberStyles.Currency);
            }

            if (legCharge != Decimal.Parse(txtRate.Text, System.Globalization.NumberStyles.Currency))
                args.IsValid = false;
            else
                args.IsValid = true;

        }

        void gvLegs_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                ((TextBox)e.Row.FindControl("txtCost")).Attributes.Add("onblur", "updateCost(this);");
                ((TextBox)e.Row.FindControl("txtCharge")).Attributes.Add("onblur", "updateCharge(this);");
                e.Row.Attributes.Add("__key", ((Entities.LegView)(e.Row.DataItem)).InstructionID.ToString());
            }
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                Entities.LegView leg = (Entities.LegView)e.Row.DataItem;
                Entities.LegPoint start = leg.StartLegPoint;
                Entities.LegPoint end = leg.EndLegPoint;

                Facade.IOrganisation facOrganisation = new Facade.Organisation();
                bool legsHaveCharges = false;

                Facade.IInstruction facInstruc = new Facade.Instruction();
                Entities.LegPlan legPlan = facInstruc.GetLegPlan(m_job.Instructions, false);

                foreach (Entities.LegView legVw in legPlan.Legs())
                {
                    if (legVw.EndLegPoint.Instruction.Charge > 0)
                    {
                        legsHaveCharges = true;
                        break;
                    }
                }
                TextBox txtCost = (TextBox)e.Row.FindControl("txtCost");
                TextBox txtCharge = (TextBox)e.Row.FindControl("txtCharge");

                bool aLegPointHasInstruction = false;
                if (!legsHaveCharges)
                {
                    #region Populate the configured rates


                    if ((m_job.JobType != eJobType.Groupage) && m_job.Charge.JobChargeType == eJobChargeType.FreeOfCharge || m_job.Charge.JobChargeType == eJobChargeType.FreeOfChargeInvoicing)
                    {
                        decimal zero = 0;

                        txtCost.Text = zero.ToString("C");
                        txtCharge.Text = zero.ToString("C");

                        txtCost.Enabled = false;
                        txtCharge.Enabled = false;
                    }
                    else
                    {
                        if (m_job.Charge.JobChargeType == eJobChargeType.PerPallet)
                        {
                            // No action
                        }

                    }

                    #endregion

                    // Hide the inner repeater unless it can have actuals/demurrages
                    aLegPointHasInstruction = ((start.Instruction != null) || (end.Instruction != null));

                    // Set leg cost
                    if (m_job.HasBeenSubContracted && m_job.SubContractors[0].Rate > 0)
                    {
                        try
                        {
                            decimal subContractorRate = 0, legCount = 0;
                            subContractorRate = m_job.SubContractors[0].Rate;
                            legCount = Convert.ToDecimal(new Facade.Instruction().GetLegPlan(m_job.Instructions, false).Legs().Count);

                            txtCost.Text = (subContractorRate / legCount).ToString("C");
                        }
                        catch
                        {
                            txtCost.Text = "0";
                        }
                    }

                    try
                    {
                        if (end.Instruction.InstructionTypeId == (int)eInstructionType.Drop || end.Instruction.InstructionTypeId == (int)eInstructionType.LeaveGoods || (m_job.JobType == eJobType.PalletReturn && end.Instruction.InstructionTypeId == (int)eInstructionType.DeHirePallets) || (m_job.JobType == eJobType.Groupage && end.Instruction.InstructionTypeId == (int)eInstructionType.Trunk))
                        {
                            decimal instructionTotal = 0;
                            //this leg is chargeable

                            if (m_job.Rate != null)
                            {
                                if (end.Point.PointId == m_job.Rate.DeliveryPointId)
                                {
                                    if (end.Instruction.TotalPallets >= Orchestrator.Globals.Configuration.PartLoadThreshold)
                                        instructionTotal += m_job.Rate.FullLoadRate;
                                    else
                                        instructionTotal += m_job.Rate.PartLoadRate;
                                }
                                else
                                {
                                    instructionTotal += m_job.Rate.MultiDropRate;
                                }
                            }
                            else
                            {
                                instructionTotal += m_job.Charge.JobChargeAmount / m_chargeableLegs;
                            }

                            txtCharge.Text = instructionTotal.ToString("C");
                        }
                        else
                        {
                            txtCharge.Visible = false;
                            txtCharge.Text = "0";
                        }
                    }
                    catch
                    {
                        txtCharge.Text = "0";
                    }
                }
                else
                {
                    if (!(end.Instruction.InstructionTypeId == (int)eInstructionType.Drop) && !(end.Instruction.InstructionTypeId == (int)eInstructionType.LeaveGoods) && !(m_job.JobType == eJobType.PalletReturn && end.Instruction.InstructionTypeId == (int)eInstructionType.DeHirePallets))
                    {
                        if (leg.EndLegPoint.Instruction.Charge == 0)
                            txtCharge.Visible = false;
                    }
                }

                if (m_job.JobState == eJobState.Invoiced)
                {
                    ((TextBox)e.Row.FindControl("txtCost")).Enabled = false;
                    ((TextBox)e.Row.FindControl("txtCharge")).Enabled = false;
                }

                if (m_job.JobType == eJobType.Return && aLegPointHasInstruction)
                {
                    decimal lc = m_job.Charge.JobChargeAmount / m_chargeableLegs;
                    ((TextBox)e.Row.FindControl("txtCharge")).Text = lc.ToString("C");
                }

                // Running cost & charge total calculation
                m_runningCostTotal += Decimal.Parse(((TextBox)e.Row.FindControl("txtCost")).Text, System.Globalization.NumberStyles.Currency);
                if (((TextBox)e.Row.FindControl("txtCharge")).Visible)
                    m_runningChargeTotal += Decimal.Parse(((TextBox)e.Row.FindControl("txtCharge")).Text, System.Globalization.NumberStyles.Currency);
            }
            else if (e.Row.RowType == DataControlRowType.Footer)
            {
                ((Label)e.Row.FindControl("lblTotalCost")).Text = m_runningCostTotal.ToString("C");
                ((Label)e.Row.FindControl("lblTotalCharge")).Text = m_runningChargeTotal.ToString("C");
            }
        }

        private void AddGlyph(GridView grid, GridViewRow item, int col, eInstructionType instructionType)
        {
            Label glyph = new Label();
            glyph.EnableTheming = false;
            glyph.Font.Name = "webdings";
            glyph.Font.Size = FontUnit.XSmall;
            glyph.Text = instructionType == eInstructionType.Load ? "C" : "D";

            item.Cells[col].Controls.Add(glyph);
        }


        void btnUpdate_Click(object sender, EventArgs e)
        {
            PopulateJob();
        }

        void MyClientSideAnchor_OnWindowClose(Codesummit.WebModalAnchor sender)
        {
            LoadJob();
        }
        #endregion

        #region Data Loading and Manipulation

        private void PopulateJob()
        {
            if (Page.IsValid)
            {
                #region Populate Job Charge

                m_job.Charge.JobChargeAmount = Decimal.Parse(txtRate.Text, System.Globalization.NumberStyles.Currency);

                #endregion

                #region Populate Leg Costs and Charges

                int instructionId = 0;

                // Setup the leg costs and charges
                foreach (GridViewRow item in gvLegs.Rows)
                {
                    if (item.RowType == DataControlRowType.DataRow)
                    {
                        decimal cost = Decimal.Parse(((TextBox)item.FindControl("txtCost")).Text, System.Globalization.NumberStyles.Currency);
                        decimal charge = Decimal.Parse(((TextBox)item.FindControl("txtCharge")).Text, System.Globalization.NumberStyles.Currency);

                        instructionId = int.Parse(item.Attributes["__key"].ToString());

                        Entities.LegPlan legPlan = new Facade.Instruction().GetLegPlan(m_job.Instructions, false);
                        foreach (Entities.LegView leg in legPlan.Legs())
                        {
                            if (instructionId == leg.InstructionID)
                            {
                                leg.EndLegPoint.Instruction.Cost = cost;
                                leg.EndLegPoint.Instruction.Charge = charge;
                            }
                        }
                    }
                }

                #endregion

                #region Update the Job State

                bool canBePriced = true;

                if (chkIsPriced.Enabled == false) // Don't change state of the job 
                    canBePriced = false;
                else
                    canBePriced = true;

                m_job.IsPriced = chkIsPriced.Checked && canBePriced;

                if (m_job.IsPriced)
                {
                    chkIsPriced.Checked = true;

                    switch (m_job.JobState)
                    {
                        case eJobState.Completed:
                            m_job.JobState = eJobState.BookingInIncomplete;
                            break;
                        case eJobState.BookingInIncomplete:
                            m_job.JobState = eJobState.BookingInIncomplete;
                            break;
                        case eJobState.BookingInComplete:
                            m_job.JobState = eJobState.ReadyToInvoice;
                            break;
                        case eJobState.ReadyToInvoice:
                            m_job.JobState = eJobState.ReadyToInvoice;
                            break;
                    }
                }
                else
                {
                    chkIsPriced.Checked = false;

                    switch (m_job.JobState)
                    {
                        case eJobState.Completed:
                            m_job.JobState = eJobState.Completed;
                            break;
                        case eJobState.BookingInIncomplete:
                            m_job.JobState = eJobState.BookingInIncomplete;
                            break;
                        case eJobState.BookingInComplete:
                            m_job.JobState = eJobState.BookingInComplete;
                            break;
                        case eJobState.ReadyToInvoice:
                            m_job.JobState = eJobState.BookingInComplete;
                            break;
                    }
                }

                #endregion

                // We don't update extras this way anymore so remove them from the job entity.
                m_job.Extras = null;

                if (canBePriced)
                {
                    if (gvRefusals.Visible)
                    {
                        PriceRefusals();
                    }
                }

                m_runningRefusalCharge = 0;

                foreach (GridViewRow row in gvRefusals.Rows)
                {
                    m_runningRefusalCharge += decimal.Parse(((TextBox)row.FindControl("txtRefusalCharge")).Text, System.Globalization.NumberStyles.Currency);
                }


                //Update and check that the Job Rate and the Goods Refused add up
                if (m_runningRefusalCharge == m_job.Charge.JobChargeAmount || m_runningRefusalCharge == 0)
                {
                    // Update the job
                    string userId = ((Entities.CustomPrincipal)Page.User).UserName;
                    Facade.IJob facJob = new Facade.Job();
                    Entities.FacadeResult result = facJob.Update(m_job, userId);

                    if (result.Success)
                    {
                        LoadJob();
                        pnlConfirmation.Visible = true;
                        lblConfirmation.Text = "The job pricing has been updated.";
                    }
                    else
                    {
                        pnlConfirmation.Visible = true;
                        lblConfirmation.Text = "The job pricing was not updated, please try again.";

                        infringementDisplay.Infringements = result.Infringements;
                        infringementDisplay.DisplayInfringments();
                    }
                }
                else
                {
                    // Dont allow them to continue until the values are added correctly
                    pnlConfirmation.Visible = true;
                    lblConfirmation.Text = "The job pricing was not updated, please ensure that the goods refused charges tally with the job rate.";
                }
            }
        }

        private void LoadJob()
        {
            chkIsPriced.Enabled = true;

            Facade.IJob facJob = new Facade.Job();
            Facade.IInstruction facInstruction = new Facade.Instruction();

            m_job = facJob.GetJob(m_jobId);

            // The m_job pricing can only be viewed if it is complete.
            if (m_job.JobState == eJobState.Booked || m_job.JobState == eJobState.Planned || m_job.JobState == eJobState.InProgress)
                Response.Redirect("../jobManagement.aspx?jobId=" + m_jobId.ToString() + "&csid=" + this.CookieSessionID);

            if (m_job.JobState == eJobState.Cancelled)
                Response.Redirect("../../Job/job.aspx?jobId=" + m_job.JobId.ToString() + "&csid=" + this.CookieSessionID);

            m_job.Charge = ((Facade.IJobCharge)facJob).GetForJobId(m_job.JobId);
            m_job.Instructions = facInstruction.GetForJobId(m_job.JobId);
            m_job.Extras = facJob.GetExtras(m_job.JobId, true);
            Facade.IJobRate facJobRate = new Facade.Job();
            m_job.Rate = facJobRate.GetRateForJobId(m_job.JobId);

            GetChargeableLegs();
            CountChargeableLegs();

            // Store the m_job in the ViewState
            ViewState[C_JOB_VS] = m_job;

            // The job can not be altered if it has been invoiced.
            if (m_job.JobState == eJobState.Invoiced)
            {
                txtRate.Enabled = false;
                btnUpdate.Enabled = false;
                chkIsPriced.Checked = true;
                chkIsPriced.Enabled = false;
            }

            #region Populate the Job Information

            lblJobId.Text = m_jobId.ToString();

            // Display the client this m_job is for
            Facade.IOrganisation facOrganisation = new Facade.Organisation();
            lblCustomer.Text = facOrganisation.GetNameForIdentityId(m_job.IdentityId);

            // Select the appropriate m_job type
            lblJobType.Text = Utilities.UnCamelCase(Enum.GetName(typeof(eJobChargeType), m_job.Charge.JobChargeType));

            // Display the m_job rate
            txtRate.Text = m_job.Charge.JobChargeAmount.ToString("C");

            // Set if the m_job is priced or not
            chkIsPriced.Checked = m_job.IsPriced;

            #endregion

            //Get the Extras for the Job
            pnlExtra.Visible = true;
            dgExtras.DataSource = GetExtras();
            dgExtras.DataBind();
            if (((Entities.ExtraCollection)dgExtras.DataSource).Count > 0)
            {
                dgExtras.Visible = true;
                lblNoExtras.Visible = false;
            }
            else
            {
                dgExtras.Visible = false;
                lblNoExtras.Visible = true;
            }
            btnAddExtra.Visible = m_job.JobType != eJobType.Groupage;

            // Display the legs with their associated costs and charges
            gvLegs.DataSource = new Facade.Instruction().GetLegPlan(m_job.Instructions, false).Legs();
            gvLegs.DataBind();

            if (m_job.JobType == eJobType.Return)
                gvRefusals.DataBind();
            else
                fsRefusals.Visible = false;

            checkRoundingIssue();
        }

        private void checkRoundingIssue()
        {
            decimal difference = 0;
            TextBox txt = null;
            if (m_job.Charge.JobChargeAmount != m_runningChargeTotal)
            {

                difference = m_job.Charge.JobChargeAmount - m_runningChargeTotal;

                txt = (TextBox)gvLegs.Rows[gvLegs.Rows.Count - 1].FindControl("txtCharge");
                decimal v = decimal.Parse(txt.Text, System.Globalization.NumberStyles.Currency) + difference;
                txt.Text = v.ToString("C");

                // Change the Footer Label
                Label lbl = (Label)gvLegs.FooterRow.FindControl("lblTotalCharge");
                v = decimal.Parse(lbl.Text, System.Globalization.NumberStyles.Currency) + difference;
                lbl.Text = v.ToString("C");
            }

            if (m_job.HasBeenSubContracted)
            {
                if (m_runningCostTotal != m_job.SubContractors[0].Rate)
                {
                    difference = m_job.SubContractors[0].Rate - m_runningCostTotal;

                    txt = (TextBox)gvLegs.Rows[gvLegs.Rows.Count - 1].FindControl("txtCost");
                    decimal v = decimal.Parse(txt.Text, System.Globalization.NumberStyles.Currency) + difference;
                    txt.Text = v.ToString("C");

                    // Change the Footer Label
                    Label lbl = (Label)gvLegs.FooterRow.FindControl("lblTotalCost");
                    v = decimal.Parse(lbl.Text, System.Globalization.NumberStyles.Currency) + difference;
                    lbl.Text = v.ToString("C");
                }
            }
        }

        private void GetChargeableLegs()
        {
            ReadOnlyCollection<Entities.LegView> legs = new Facade.Instruction().GetLegPlan(m_job.Instructions, false).Legs();

            // Get first leg point which is chargeable.
            for (int i = 0; i < legs.Count; i++)
            {
                Entities.LegView leg = legs[i];

                Entities.LegPoint startLegPoint = leg.StartLegPoint;
                Entities.LegPoint endLegPoint = leg.EndLegPoint;

                // If start leg point of leg has an instruction of type load,
                // current leg is chargeable.
                if (startLegPoint.Instruction != null && startLegPoint.Instruction.InstructionTypeId == (int)eInstructionType.Load)
                {
                    m_chargeableLegStart = i;
                    m_chargeableLegs++;
                }

                // Otherwise, if end leg point of leg has an instruction of type load,
                // next leg is chargeable.
                else if (endLegPoint.Instruction != null && (endLegPoint.Instruction.InstructionTypeId == (int)eInstructionType.Load || (m_job.JobType == eJobType.PalletReturn && endLegPoint.Instruction.InstructionTypeId == (int)eInstructionType.PickupPallets)) )
                {
                    m_chargeableLegStart = i + 1;
                    m_chargeableLegs++;
                }

                if (m_chargeableLegStart != 0)
                    break;
            }

            // Get last leg point which is chargeable.
            for (int i = legs.Count - 1; i >= 0; i--)
            {
                Entities.LegView leg = legs[i];

                Entities.LegPoint startLegPoint = leg.StartLegPoint;
                Entities.LegPoint endLegPoint = leg.EndLegPoint;

                // If end leg point of instruction has a drop, this leg is chargeable.
                if (endLegPoint.Instruction != null && (endLegPoint.Instruction.InstructionTypeId == (int)eInstructionType.Drop || (m_job.JobType == eJobType.PalletReturn && endLegPoint.Instruction.InstructionTypeId == (int)eInstructionType.DeHirePallets)) )
                {
                    m_chargeableLegEnd = i;
                    m_chargeableLegs++;
                }

                // Otherwise if start leg point of leg has an instruction type of drop, previous
                // leg is chargeable.
                else if (startLegPoint.Instruction != null && (startLegPoint.Instruction.InstructionTypeId == (int)eInstructionType.Drop || (m_job.JobType == eJobType.PalletReturn && startLegPoint.Instruction.InstructionTypeId == (int)eInstructionType.DeHirePallets)) )
                {
                    m_chargeableLegEnd = i - 1;
                }

                if (m_chargeableLegEnd != 0)
                    break;
            }
        }

        private void CountChargeableLegs()
        {
            ReadOnlyCollection<Entities.LegView> legs = new Facade.Instruction().GetLegPlan(m_job.Instructions, false).Legs();
            m_chargeableLegs = 0;

            Entities.LegView leg = null;
            bool legChargeable = false;

            for (int i = 0; i < legs.Count; i++)
            {
                legChargeable = false;
                leg = legs[i];

                // Does the Leg Have a trailer
                if (leg.Trailer != null || m_job.HasBeenSubContracted || leg.Vehicle.IsFixedUnit)
                {
                    //if there is a delivery on this leg then it is chargeable
                    if ((leg.EndLegPoint.Instruction.InstructionTypeId == (int)eInstructionType.Drop)
                        || (leg.EndLegPoint.Instruction.InstructionTypeId == (int)eInstructionType.LeaveGoods)
                        || ((m_job.JobType == eJobType.PalletReturn) && (leg.EndLegPoint.Instruction.InstructionTypeId == (int)eInstructionType.DeHirePallets))
                        )
                    {
                        legChargeable = true;
                        //J.Steele 05/06/08
                        //Shouldn't break here - this was causing a Divide by zero error
                        //break;
                    }

                    // Is there a delivery on any future leg
                    if (!legChargeable)
                    {
                        for (int x = i + 1; x < legs.Count; x++)
                        {
                            leg = legs[i];

                            if (
                                (leg.EndLegPoint.Instruction.InstructionTypeId == (int)eInstructionType.Drop)
                                || (leg.EndLegPoint.Instruction.InstructionTypeId == (int)eInstructionType.LeaveGoods)
                                || ((m_job.JobType == eJobType.PalletReturn) && (leg.EndLegPoint.Instruction.InstructionTypeId == (int)eInstructionType.DeHirePallets))
                                )
                            {
                                legChargeable = true;
                                break;
                            }
                        }
                    }

                    if (legChargeable)
                        m_chargeableLegs++;
                }
            }
        }

        protected Entities.ExtraCollection GetExtras()
        {
            m_job = (Entities.Job)ViewState[C_JOB_VS];

            Entities.ExtraCollection extras = new Entities.ExtraCollection();

            foreach (Entities.Extra extra in m_job.Extras)
            {
                extras.Add(extra);
            }

            return extras;
        }


        private void PriceRefusals()
        {
            TextBox txtCharge = null;
            string userId = ((Entities.CustomPrincipal)Page.User).UserName;
            decimal amount = 0;
            int refusalId = 0;
            foreach (GridViewRow row in gvRefusals.Rows)
            {
                txtCharge = (TextBox)row.FindControl("txtRefusalCharge");
                if (decimal.Parse(txtCharge.Text, System.Globalization.NumberStyles.Currency) > 0)
                {
                    amount = decimal.Parse(txtCharge.Text, System.Globalization.NumberStyles.Currency);
                    refusalId = int.Parse(row.Attributes["__key"].ToString());

                    using (Facade.IGoodsRefusal facRefusal = new Facade.GoodsRefusal())
                        facRefusal.SetRefusalCharge(refusalId, amount, userId);
                }
            }
        }
        #endregion

        protected override PageStatePersister PageStatePersister
        {
            get
            {
                return new HiddenFieldPageStatePersister(this.Page);
            }
        }

    }
}