using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Orchestrator.WebUI
{
    public partial class Job_multicallin : Orchestrator.Base.BasePage
    {
        private int m_instructionId = 0;
        private Entities.Job m_job = null;
        private int m_jobID = 0;
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnCallIn.Click += new EventHandler(btnCallIn_Click);
            this.grdCallIns.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdCallIns_NeedDataSource);
            this.btnCallInTop.Click += new EventHandler(btnCallIn_Click);
            
        }

        void grdCallIns_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            Facade.IJob facJob = new Facade.Job();
            DataSet dsCallInLog = facJob.GetQuickCallInLog(1, 0, DateTime.Today.AddMonths(-1), DateTime.Today.AddMonths(1));
            this.grdCallIns.DataSource = dsCallInLog;


        }

        void btnCallIn_Click(object sender, EventArgs e)
        {
            litError.Visible = false;
            StoreActual();
            grdCallIns.Rebind();
        }

        private void StoreActual()
        {
            Entities.FacadeResult result = null;
            string userId = ((Entities.CustomPrincipal)Page.User).UserName;
            Entities.InstructionActual ia = null;
            Facade.IJob facJob = new Facade.Job();
            string errMessage = "<div style=\"background-color:white; color:red; border:solid 1pt black; padding:5px;\"> One or nore of the Call Ins that you wanted to create could not be done as there are either earlier debriefs to be processed first or they are on the same job as others that are for Customers that have been marked as to require the capture of Debrief information : <br/>{0}</div>";
            string jobLink = "<a href=\"job.aspx?jobId={0}&csid=xx\" target=\"_blank\">{0}</a>";
            string jobLinks = string.Empty;
            string jobIds = string.Empty;

            int i = 0;
            foreach (Telerik.Web.UI.GridDataItem gdi in grdCallIns.SelectedItems)
            {
                if ( i > 100)
                    break;
                m_instructionId = (int)gdi.OwnerTableView.DataKeyValues[gdi.ItemIndex]["InstructionId"];
                m_jobID = (int)gdi.OwnerTableView.DataKeyValues[gdi.ItemIndex]["JobId"];
                m_job = facJob.GetJob(m_jobID, true);
                ia = PopulateInstructionActual();
                result = CreateInstructionActual(ia, userId);
                if (result.Success == false)
                {
                    jobIds += string.Format(jobLink, m_jobID.ToString()) + "<br/>" ;
                }
                i++;
            }

            if (jobIds.Length > 0)
            {
                litError.Text = string.Format(errMessage, jobIds);
                litError.Visible = true;
            }
                
            
        }

        private Entities.FacadeResult CreateInstructionActual(Entities.InstructionActual instructionActual, string userId)
        {
            Entities.FacadeResult result = new Orchestrator.Entities.FacadeResult(false) ;

            Entities.PCV m_PCV = null;

            try
            {

                using (Facade.IInstructionActual facInstructionActual = new Facade.Instruction())
                {
                    DateTime startedAt = DateTime.UtcNow;
                    result = facInstructionActual.Create(m_job, instructionActual, m_PCV, userId);
                    DateTime endedAt = DateTime.UtcNow;
                    instructionActual.InstructionActualId = result.ObjectId;

                }
            }
            catch { }

            return result;
        }

        private Entities.InstructionActual PopulateInstructionActual()
        {
            Facade.IInstruction facInstruction = new Facade.Instruction();
            Entities.InstructionActual instructionActual = ((Facade.IInstructionActual)facInstruction).GetEntityForInstructionId(m_instructionId);
            if (instructionActual == null)
            {
                instructionActual = new Entities.InstructionActual();
                instructionActual.InstructionId = m_instructionId;
                instructionActual.CollectDropActuals = new Entities.CollectDropActualCollection();
            }
            Entities.Instruction i = facInstruction.GetInstruction(m_instructionId);

            // Populate the main data about a call-in.
            instructionActual.Discrepancies = "";
            instructionActual.DriversNotes = "";

            instructionActual.ArrivalDateTime = i.BookedDateTime;
            instructionActual.CollectDropDateTime = i.BookedDateTime;
            instructionActual.LeaveDateTime = i.BookedDateTime;

            // Populate the collect drop information.
            eInstructionType instructionType = (eInstructionType) i.InstructionTypeId;

            #region Instruction Type Specific Loadings

            switch (instructionType)
            {
                case eInstructionType.Load:
                    // The collect drop dockets are specified by the drop recording process - we just need to create dummy actuals (we only do this for the initial create).
                    if (instructionActual.InstructionActualId == 0)
                    {
                        Entities.Instruction instruction = facInstruction.GetInstruction(m_instructionId);

                        foreach (Entities.CollectDrop loadDocket in instruction.CollectDrops)
                        {
                            Entities.CollectDropActual loadDocketDummyActual = new Entities.CollectDropActual();

                            loadDocketDummyActual.CollectDropId = loadDocket.CollectDropId;
                            loadDocketDummyActual.NumberOfCases = 0;
                            loadDocketDummyActual.NumberOfPallets = 0;
                            loadDocketDummyActual.Weight = 0;

                            instructionActual.CollectDropActuals.Add(loadDocketDummyActual);
                        }
                    }
                    break;
                case eInstructionType.Trunk:
                    // The collect drop dockets are specified by the drop recording process - we just need to create dummy actuals (we only do this for the initial create).
                    if (instructionActual.InstructionActualId == 0)
                    {
                        Entities.Instruction instruction = i;

                        foreach (Entities.CollectDrop loadDocket in instruction.CollectDrops)
                        {
                            Entities.CollectDropActual loadDocketDummyActual = new Entities.CollectDropActual();

                            loadDocketDummyActual.CollectDropId = loadDocket.CollectDropId;
                            loadDocketDummyActual.NumberOfCases = 0;
                            loadDocketDummyActual.NumberOfPallets = 0;
                            loadDocketDummyActual.Weight = 0;

                            instructionActual.CollectDropActuals.Add(loadDocketDummyActual);
                        }
                    }
                    break;
                case eInstructionType.Drop:
                    if (m_job.JobType == eJobType.Normal || m_job.JobType == eJobType.Groupage)
                    {
                        
                        
                        //create a collectdropactual for each collectdrop
                        Entities.CollectDropActual cda = new Orchestrator.Entities.CollectDropActual();
                        foreach (Entities.CollectDrop cd in i.CollectDrops)
                        {
                            cda = new Orchestrator.Entities.CollectDropActual();
                            cda.NumberOfCases = cd.NoCases;
                            cda.NumberOfPallets = cd.NoPallets;
                            cda.NumberOfPalletsReturned = cd.NoPallets;
                            cda.CollectDropId = cd.CollectDropId;
                            cda.Weight = cd.Weight;
                        }
                        instructionActual.CollectDropActuals.Add(cda);

                    }
                    break;
            }

            #endregion

            return instructionActual;
        }

    }
}