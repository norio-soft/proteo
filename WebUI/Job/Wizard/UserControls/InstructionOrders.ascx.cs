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

    using Orchestrator.WebUI;
namespace Orchestrator.WebUI.Job.Wizard.UserControls
{


    public partial class InstructionOrders : System.Web.UI.UserControl, IDefaultButton
    {
        #region Page Variables

        private int                                 m_jobId;
        private Entities.Job           m_job;
        private bool                                m_isAmendment = false;
        private bool                                m_isUpdate = false;

        private Entities.Instruction   m_instruction = null;
        private int                                 m_instructionIndex;

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            m_jobId = Convert.ToInt32(Request.QueryString["jobId"]);

            if (m_jobId > 0)
                m_isUpdate = true;

            // Retrieve the job from the session variable
            m_job = (Entities.Job)Session[wizard.C_JOB];

            if (Session[wizard.C_INSTRUCTION_INDEX] != null)
            {
                m_instructionIndex = (int)Session[wizard.C_INSTRUCTION_INDEX];

                if (!m_isUpdate && m_instructionIndex != m_job.Instructions.Count)
                    m_isAmendment = true;
            }
            if ((Entities.Instruction)Session[wizard.C_INSTRUCTION] != null)
                m_instruction = (Entities.Instruction)Session[wizard.C_INSTRUCTION];

            if (!IsPostBack)
            {
                // Configure cancel button confirmation alert.
                btnCancel.Attributes.Add("onClick", wizard.C_CONFIRM_MESSAGE);

                // Can only change the point if a new instruction is being created.
                ucPoint.CanChangePoint = m_instruction == null;
                ucPoint.Visible = m_instruction == null;
                if (m_instruction != null)
                {
                    // Configure the point user control to focus on the supplied point.
                    ucPoint.SelectedPoint = m_instruction.Point;

                    dteBookedDate.SelectedDate = m_instruction.BookedDateTime;
                    if (!m_instruction.IsAnyTime)
                        dteBookedTime.SelectedDate = m_instruction.BookedDateTime;

                    // Configure the instructions orders user controls.
                    ucInstructionOrders.Instruction = m_instruction;

                    // Configure the orders user control.
                    ucOrders.CollectionPoint = m_instruction.Point;
                    ucOrders.ExcludedOrderIDs = m_instruction.GetOrderIDList();
                    ucOrders.CollectionDate = m_instruction.BookedDateTime;
                    ucOrders.CollectionIsAnytime = m_instruction.IsAnyTime;
                    if (m_job.BusinessTypeID > 0)
                        ucOrders.BusinessTypeIDs = new List<int>(new int[] { m_job.BusinessTypeID });

                    // Pass selected orders to the controls
                    if (Session[wizard.C_ADDED_ORDERS] != null)
                        ucOrders.PreSelectedOrderIDs = (List<int>)Session[wizard.C_ADDED_ORDERS];
                }
                else
                {
                    // The user is adding a new instruction to the current job.
                    m_instruction = new Entities.Instruction();
                    m_instruction.JobId = m_jobId;
                    m_instruction.InstructionTypeId = (int)eInstructionType.Load;
                    Session[wizard.C_INSTRUCTION] = m_instruction;
                    if (m_job.BusinessTypeID > 0)
                        ucOrders.BusinessTypeIDs = new List<int>(new int[] { m_job.BusinessTypeID });
                }

                // Clear out used session variables
                Session[wizard.C_ADDED_ORDERS] = null;
                Session[wizard.C_REMOVED_ORDERS] = null;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnViewAvailableOrders.Click += new EventHandler(btnViewAvailableOrders_Click);
            cfvOkayToRemoveAllOrders.ServerValidate += new ServerValidateEventHandler(cfvOkayToRemoveAllOrders_ServerValidate);

            btnBack.Click += new EventHandler(btnBack_Click);
            btnNext.Click += new EventHandler(btnNext_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
        }

        #region Private Methods

        /// <summary>
        /// Sends the user to the specified step in the job manipulation process.  Passing this job's job id on the querystring if the
        /// job already exists in the database.
        /// </summary>
        /// <param name="step">The string representation of the step to jump to, this are defined in the wizard.aspx page.</param>
        private void GoToStep(string step)
        {
            string url = "wizard.aspx?step=" + step;

            if (m_isUpdate)
                url += "&jobId=" + m_jobId.ToString();

            Response.Redirect(url);
        }

        #endregion

        #region Event Handlers

        #region Button Events

        void btnViewAvailableOrders_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                bool setPoint = false;
                bool setDate = false;

                if (ucPoint.SelectedPoint != null)
                {
                    ucOrders.CollectionPoint = ucPoint.SelectedPoint;
                    setPoint = true;
                }

                try
                {
                    DateTime date = dteBookedDate.SelectedDate.Value;
                    date = date.Subtract(date.TimeOfDay);

                    if (dteBookedTime.SelectedDate.HasValue)
                    {
                        date = date.Add(dteBookedTime.SelectedDate.Value.TimeOfDay);
                        ucOrders.CollectionIsAnytime = false;
                    }
                    else
                    {
                        date = date.Add(new TimeSpan(23, 59, 59));
                        ucOrders.CollectionIsAnytime = true;
                    }

                    ucOrders.CollectionDate = date;
                    setDate = true;
                }
                catch { }

                if (setPoint && setDate)
                    ucOrders.Rebind();
            }
        }

        void btnBack_Click(object sender, EventArgs e)
        {
            // If the session is set such that upon a cancel the user should be taken to the details page, go to that page,
            // otherwise return to the job rate information page.
            bool jumpToDetails = Session[wizard.C_JUMP_TO_DETAILS] != null && ((bool)Session[wizard.C_JUMP_TO_DETAILS]);
            Session[wizard.C_JUMP_TO_DETAILS] = false;

            if (jumpToDetails)
                GoToStep("JD");
            else if (m_job.Instructions.Count == 0)
                GoToStep("JR");
            else
                GoToStep("JD");
        }

        void btnNext_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                // Set the instruction's booked date time.
                DateTime bookedDateTime = dteBookedDate.SelectedDate.Value;
                bookedDateTime = bookedDateTime.Subtract(bookedDateTime.TimeOfDay);
                if (dteBookedDate.SelectedDate.HasValue)
                {
                    bookedDateTime = bookedDateTime.Add(dteBookedTime.SelectedDate.Value.TimeOfDay);
                    m_instruction.IsAnyTime = false;
                }
                else
                {
                    bookedDateTime = bookedDateTime.Add(new TimeSpan(23, 59, 59));
                    m_instruction.IsAnyTime = true;
                }
                m_instruction.BookedDateTime = bookedDateTime;

                // Get a list of added and removed orders.
                List<int> additions = ucOrders.OrderIDs;
                List<int> removals = ucInstructionOrders.GetForSelectionStatus(false);

                if (m_isUpdate)
                {
                    if (m_instruction.InstructionID == 0)
                    {
                        // Existing job, new instruction (configure the CDS).
                        m_instruction.Point = ucPoint.SelectedPoint;
                        m_instruction.PointID = ucPoint.SelectedPoint.PointId;
                    }
                }
                else
                {
                    if (m_isAmendment)
                    {
                        // New job, existing instruction.
                    }
                    else
                    {
                        // New job, new instruction.
                    }
                }

                // Stored added and removed instructions in the session.
                Session[wizard.C_ADDED_ORDERS] = additions;
                Session[wizard.C_REMOVED_ORDERS] = removals;

                // Store the instruction in the session.
                Session[wizard.C_INSTRUCTION] = m_instruction;

                // Advance to the next step.
                GoToStep("SGOH");
            }
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            GoToStep("CANCEL");
        }

        #endregion

        #region Validation Event Handlers

        /// <summary>
        /// Ensure that the user is not removing all the orders if this is the only collection on the job.
        /// </summary>
        void cfvOkayToRemoveAllOrders_ServerValidate(object source, ServerValidateEventArgs args)
        {
            int collectionInstructionCount = 0;

            // If we're adding a new instruction increase the collection count
            if (m_instruction.InstructionID == 0 && !m_isAmendment)
                collectionInstructionCount++;

            if (m_isUpdate)
            {
                foreach (Entities.Instruction instruction in m_job.Instructions)
                    if (instruction.InstructionTypeId == (int)eInstructionType.Load)
                        collectionInstructionCount++;
            }
            else
            {
                foreach (Entities.Instruction instruction in m_job.Instructions)
                    if (instruction.InstructionTypeId == (int)eInstructionType.Load)
                        collectionInstructionCount++;
            }

            if (collectionInstructionCount > 1)
                args.IsValid = true; // More than one collection instruction so no problem.
            else
            {
                List<int> additions = ucOrders.OrderIDs;
                List<int> removals = ucInstructionOrders.GetForSelectionStatus(false);

                if (additions.Count == 0 && removals.Count == m_instruction.CollectDrops.Count)
                    args.IsValid = false; // No orders have been added, and all orders have been removed.
                else
                    args.IsValid = true; // Either orders have been added, or not all orders have been removed.
            }
        }

        #endregion

        #endregion

        #region IDefaultButton

        public System.Web.UI.Control DefaultButton
        {
            get { return btnNext; }
        }

        #endregion
    }
}
