using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using Orchestrator;
using Orchestrator.Entities;
using Orchestrator.WebUI;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.UserControls
{
    public partial class MultiTrunk : System.Web.UI.UserControl
    {
        private Entities.MultiTrunk _selectedMultiTrunk = null;

        public event SelectedMultiTrunkChangedEventHandler SelectedMultiTrunkChanged;
        public int MultiTrunkID { get; set; }
        public Entities.MultiTrunk SelectedMultiTrunk
        {
            get
            {
                if (_selectedMultiTrunk != null)
                {
                    return _selectedMultiTrunk;
                }
                else if (_selectedMultiTrunk == null && (MultiTrunkID > 0 || !String.IsNullOrEmpty(hidMultiTrunkId.Value)))
                {
                    int multiTrunkId;
                    if (this.MultiTrunkID > 0)
                    {
                        multiTrunkId = this.MultiTrunkID;
                    }
                    else
                    {
                        multiTrunkId = int.Parse(hidMultiTrunkId.Value);
                    }

                    Facade.MultiTrunk facMultiTrunk = new Facade.MultiTrunk();
                    _selectedMultiTrunk = facMultiTrunk.GetForMultiTrunkID(multiTrunkId);
                    return _selectedMultiTrunk;
                }

                return null;
            }
            set
            {
                _selectedMultiTrunk = value;
                MultiTrunkID = _selectedMultiTrunk.MultiTrunkId;
            }
        }

        public DateTime ArrivalDateTime
        {
            get { return dteArrival.SelectedDate.Value.Date.Add(tmeArrival.SelectedTime.Value); }
            set 
            {
                dteArrival.SelectedDate = value.Date;
                tmeArrival.SelectedTime = value.TimeOfDay;
            }
        }

        public bool UseDriver
        {
            get { return chkDriver.Checked; }
        }

        public bool UseVehicle
        {
            get { return chkVehicle.Checked; }
        }

        protected override void OnInit(EventArgs e)
        {
            this.rcbMultiTrunk.SelectedIndexChanged += new RadComboBoxSelectedIndexChangedEventHandler(rcbMultiTrunk_SelectedIndexChanged);
            this.rcbMultiTrunk.OnClientItemsRequesting = this.ClientID + "_MultiTrunkRequesting";

            base.OnInit(e);
        }

        void rcbMultiTrunk_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (rcbMultiTrunk.SelectedValue != String.Empty)
            {
                int multiTrunkId = int.Parse(rcbMultiTrunk.SelectedValue);
                hidMultiTrunkId.Value = multiTrunkId.ToString();

                Orchestrator.Facade.MultiTrunk facMultiTrunk = new Orchestrator.Facade.MultiTrunk();
                Orchestrator.Entities.MultiTrunk selectedMultiTrunk = facMultiTrunk.GetForMultiTrunkID(multiTrunkId);
                SelectedMultiTrunk = selectedMultiTrunk;

                rcbMultiTrunk.SelectedValue = selectedMultiTrunk.MultiTrunkId.ToString();
                rcbMultiTrunk.Text = selectedMultiTrunk.Description;

                pnlMultiTrunk.Visible = true;
                lblMultiTrunk.Text = selectedMultiTrunk.HtmlTableFormattedTrunkPointDescriptionsWithTravelTimes;

                // Signal to any subscribers that the multi trunk has changed.
                if (SelectedMultiTrunkChanged != null)
                    SelectedMultiTrunkChanged(this, new SelectedMultiTrunkChangedEventArgs(selectedMultiTrunk));

                tblArriveDepart.Visible = true;
                lblArrivalPoint.Text = selectedMultiTrunk.TrunkPoints[0].Point.Description;
                //lblDeparturePoint.Text = selectedMultiTrunk.TrunkPoints[selectedMultiTrunk.TrunkPoints.Count - 1].Point.Description;
            }
            else
            {
                lblMultiTrunk.Text = "";
                pnlMultiTrunk.Visible = false;
                tblArriveDepart.Visible = false;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            int instructionId = Convert.ToInt32(Request.QueryString["iID"]);

            if (!Page.IsPostBack)
            {
                using (Facade.Instruction facInstruction = new Facade.Instruction())
                {
                    Entities.Instruction affectedInstruction = null;
                    Entities.Job job = null;
                    int jobId = 0;
                    Entities.InstructionCollection instructions = null;

                    if (int.TryParse(Request.QueryString["jobId"], out jobId))
                    {

                        if (instructionId != 0)
                        {
                            affectedInstruction = facInstruction.GetInstruction(instructionId);

                            // Get the job.
                            job = (Entities.Job)Cache.Get("JobEntityForJobId" + jobId.ToString());

                            // Try and get the job from the cache
                            if (job == null)
                            {
                                Facade.IJob facJob = new Facade.Job();
                                job = facJob.GetJob(jobId);
                            }
                        }
                        else
                        {
                            // We have no instruction id - this means that ascx is being used for adding
                            // multiple destinations. We should use the last instruction on the job.

                            // Get the job.
                            job = (Entities.Job)Cache.Get("JobEntityForJobId" + jobId.ToString());

                            // Try and get the job from the cache
                            if (job == null)
                            {
                                Facade.IJob facJob = new Facade.Job();
                                job = facJob.GetJob(jobId);

                                // Job was not in the cache thus get the instruction collection from the db
                                instructions = new Facade.Instruction().GetForJobId(jobId);

                                // Get the end instruction
                                Entities.Instruction endInstruction = null;
                                endInstruction = instructions.Find(instruc => instruc.InstructionOrder == instructions.Count - 1);

                                if (endInstruction == null)
                                    throw new ApplicationException("Cannot find last instruction.");

                                affectedInstruction = endInstruction;
                            }
                            else
                            {
                                // Job is in the cache, check for instructions

                                if (job.Instructions != null)
                                {
                                    // We have instructions
                                    instructions = job.Instructions;
                                }
                                else
                                {
                                    // otherwise get a fresh instruction collection
                                    instructions = new Facade.Instruction().GetForJobId(jobId);
                                }

                                // Get the end instruction
                                Entities.Instruction endInstruction = null;
                                endInstruction = instructions.Find(instruc => instruc.InstructionOrder == instructions.Count - 1);

                                if (endInstruction == null)
                                    throw new ApplicationException("Cannot find last instruction.");

                                affectedInstruction = endInstruction;
                            }

                        }
                    }

                    if (job.JobState != eJobState.Invoiced && job.JobState != eJobState.Cancelled)
                    {
                        if (affectedInstruction.Driver != null)
                            chkDriver.Text = affectedInstruction.Driver.Individual.FullName;
                        else
                            chkDriver.Enabled = false;

                        if (affectedInstruction.Vehicle != null)
                            chkVehicle.Text = affectedInstruction.Vehicle.RegNo;
                        else
                            chkVehicle.Enabled = false;
                    }
                }
            }
        }
    }
}