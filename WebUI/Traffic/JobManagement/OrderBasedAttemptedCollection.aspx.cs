using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Orchestrator.WebUI.Traffic.JobManagement
{
    public partial class OrderBasedAttemptedCollection : System.Web.UI.Page
    {
        private static string vs_JobId = "vs_JobId";
        public int JobId
        {
            get { return ViewState[vs_JobId] == null ? -1 : (int)ViewState[vs_JobId]; }
            set { ViewState[vs_JobId] = value; }
        }

        private static string vs_InstructionId = "vs_InstructionId";
        public int InstructionId
        {
            get { return ViewState[vs_InstructionId] == null ? -1 : (int)ViewState[vs_InstructionId]; }
            set { ViewState[vs_InstructionId] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            GetSessionIDFromQueryString();

            if (!IsPostBack)
                ConfigureDisplay();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.btnCancel.Click += new EventHandler(btnCancel_Click);
            this.btnSave.Click += new EventHandler(btnSave_Click);

        }

        #region Cookie Handling

        private string _cookieSessionID = string.Empty;
        public string CookieSessionID
        {
            get
            {
                if (string.IsNullOrEmpty(_cookieSessionID))
                {
                    _cookieSessionID = Utilities.GetRandomString(6);
                }

                return _cookieSessionID;
            }
            set
            {
                _cookieSessionID = value;
            }
        }

        private void GetSessionIDFromQueryString()
        {
            if (!String.IsNullOrEmpty(Request.QueryString["csid"]))
            {
                _cookieSessionID = Request.QueryString["csid"];
            }
        }

        #endregion

        #region Private Methods

        private void ConfigureDisplay()
        {
            int jobId, instructionId = 0;

            if(JobId == -1 && !string.IsNullOrEmpty(Request.QueryString["jId"]))
            {
                if(int.TryParse(Request.QueryString["jId"], out jobId))
                    JobId = jobId;
            }

            if(InstructionId == -1 && !string.IsNullOrEmpty(Request.QueryString["iId"]))
            {
                if (int.TryParse(Request.QueryString["iId"], out instructionId))
                    InstructionId = instructionId;
            }

            // Bind the options for redelivery reasons, collection options only.
            cboRedeliveryReason.DataSource = EF.DataContext.Current.RedeliveryReasonSet.Where(rr => rr.IsEnabled == true && rr.IsCollection == true).OrderBy(rr => rr.Description);
            cboRedeliveryReason.DataBind();

            // Bind the extra types (remove any invalid options).
            Facade.ExtraType facExtraType = new Facade.ExtraType();
            bool? getActiveExtraTypes = true;

            List<Entities.ExtraType> extraTypes = facExtraType.GetForIsEnabled(getActiveExtraTypes);
            extraTypes.RemoveAll(o => (eExtraType)(o.ExtraTypeId) == eExtraType.Custom
                || (eExtraType)(o.ExtraTypeId) == eExtraType.Demurrage
                || (eExtraType)(o.ExtraTypeId) == eExtraType.DemurrageExtra);

            cboExtraType.DataSource = extraTypes;
            cboExtraType.DataValueField = "ExtraTypeId";
            cboExtraType.DataTextField = "Description";
            cboExtraType.DataBind();

            // Bind the extra states (remove any invalid options).
            List<string> extraStates = new List<string>(Utilities.UnCamelCase(Enum.GetNames(typeof(eExtraState))));
            extraStates.Remove(Utilities.UnCamelCase(Enum.GetName(typeof(eExtraState), eExtraState.Invoiced)));
            cboExtraState.DataSource = extraStates;
            cboExtraState.DataBind();

            // Get Instruction Details
            Facade.IInstruction facIns = new Facade.Instruction();
            Facade.IPoint facPoint = new Facade.Point();
            
            Entities.Instruction currentIns = facIns.GetInstruction(InstructionId);
            Entities.Point loadPoint = facPoint.GetPointForPointId(currentIns.PointID);

            txtExtraCustomReason.Text = string.Format("Attempted Collection from {0}", loadPoint.Description);
        }

        #endregion

        #region Events

        #region Button

        void btnCancel_Click(object sender, EventArgs e)
        {
            //Response.Redirect(string.Format("/Job/Job.aspx?jobId={0}", JobId));
            if (InstructionId == -1)
                Response.Redirect(string.Format("/Job/Job.aspx?jobId={0}&csid={1]", JobId, this.CookieSessionID));
            else
                Response.Redirect(string.Format("/Traffic/JobManagement/DriverCallIn/CallIn.aspx?wiz=true&jobId={0}&instructionId={1}&csid={2}", JobId, InstructionId, this.CookieSessionID));
        }

        void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                Entities.FacadeResult success = new Orchestrator.Entities.FacadeResult(false);

                if (rdOrderAction_NotCollected.Checked)
                {
                    #region Get Page details
                    int rReasonId = 0;
                    int.TryParse(cboRedeliveryReason.SelectedValue, out rReasonId);

                    int nCPointId = 0, nDPointId = 0, dayDifference = 0;
                    bool nCIsAnytime = false, nDIsAnyTime = false;
                    DateTime nCDateTime = new DateTime(), nCByDateTime = new DateTime(), nDDateTime = new DateTime(), nDFromDateTime = new DateTime(), dateDiff = new DateTime();

                    nCDateTime = dteCollectionFromDate.SelectedDate.Value;
                    nCDateTime = nCDateTime.Add(new TimeSpan(dteCollectionFromTime.SelectedDate.Value.Hour, dteCollectionFromTime.SelectedDate.Value.Minute, 0));

                    nCByDateTime = nCDateTime;

                    if (rdCollectionBookingWindow.Checked)
                    {
                        nCByDateTime = dteCollectionByDate.SelectedDate.Value;
                        nCByDateTime = nCByDateTime.Add(new TimeSpan(dteCollectionByTime.SelectedDate.Value.Hour, dteCollectionByTime.SelectedDate.Value.Minute, 0));
                    }

                    nCIsAnytime = rdCollectionIsAnytime.Checked;

                    if (chkChangeDeliveryDate.Checked)
                    {
                        nDFromDateTime = dteDeliveryFromDate.SelectedDate.Value;
                        nDFromDateTime = nDFromDateTime.Add(new TimeSpan(dteDeliveryFromTime.SelectedDate.Value.Hour, dteDeliveryFromTime.SelectedDate.Value.Minute, 0));

                        nDDateTime = nDFromDateTime;

                        if (rdDeliveryBookingWindow.Checked)
                        {
                            nDDateTime = dteDeliveryByDate.SelectedDate.Value;
                            nDDateTime = nDDateTime.Add(new TimeSpan(dteDeliveryByTime.SelectedDate.Value.Hour, dteDeliveryByTime.SelectedDate.Value.Minute, 0));
                        }

                        nDIsAnyTime = rdDeliveryIsAnytime.Checked;
                    }

                    if (chkCollectGoodsElsewhere.Checked && ucNewCollectionPoint.SelectedPoint != null)
                        nCPointId = ucNewCollectionPoint.SelectedPoint.PointId;

                    if (chkDeliverGoodsElsewhere.Checked && ucNewDeliveryPoint.SelectedPoint != null)
                        nDPointId = ucNewDeliveryPoint.SelectedPoint.PointId;

                    Entities.Extra extra = null;

                    // If Updates processed sucessfully, log the extra against the highest value order.
                    if (chkCharging.Checked)
                    {
                        // Build an extra to cover the redelivery.
                        extra = new Entities.Extra();
                        extra.ExtraType = (eExtraType)int.Parse(cboExtraType.SelectedValue);
                        extra.ForeignAmount = (decimal)txtExtraAmount.Value;

                        extra.ExtraState = Utilities.SelectedEnumValue<eExtraState>(cboExtraState.SelectedValue); ;
                        extra.CustomDescription = txtExtraCustomReason.Text;

                        if (!string.IsNullOrEmpty(txtClientContact.Text))
                            extra.ClientContact = txtClientContact.Text;
                    }
                    #endregion

                    Facade.IAttemptedCollection facAC = new Facade.AttemptedCollection();
                    success = facAC.Create(JobId, InstructionId, rdResolutionMethod_AttemptLater.Checked, rReasonId, txtAttemptedCollectionReference.Text, txtAttemptedClientContact.Text, nCDateTime, nCByDateTime, nCIsAnytime, nCPointId, chkChangeDeliveryDate.Checked, nDFromDateTime, nDDateTime, nDIsAnyTime, nDPointId, chkCreateOnwardRun.Checked, extra, Page.User.Identity.Name);
                }

                #region Display Results
                if (success != null && success.Success)
                {
                    if (success.ObjectId > 0)
                    {
                        Facade.IJob facJob = new Facade.Job();
                        Entities.Job eJob = facJob.GetJob(JobId);

                        // If the existing run has been cancelled, then re-direct to the new run.
                        if (eJob == null)
                        {
                            JobId = success.ObjectId;
                            InstructionId = -1;
                        }
                        else
                            dlgOrder.Open(string.Format("jobId={0}", success.ObjectId));
                    }

                    btnCancel_Click(null, null);
                }
                else
                {
                    if (success != null)
                        idErrors.Infringements = success.Infringements;

                    idErrors.DisplayInfringments();
                    idErrors.Visible = true;
                }
                #endregion
            }
        }

        #endregion

        #endregion
    }
}
