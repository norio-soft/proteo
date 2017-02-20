using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Services;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Xml.Serialization;

using Orchestrator.Entities;
using Orchestrator.Facade;
using Orchestrator.WebUI.Job.Wizard.UserControls;

using ComponentArt.Web.UI;
using Orchestrator.Repositories;

namespace Orchestrator.WebUI
{
    /// <summary>
    /// Summary description for jobDetails.
    /// </summary>
    public partial class JobDisplay : Orchestrator.Base.BasePage
    {
        protected override PageStatePersister PageStatePersister
        {
            get
            {
                return new HiddenFieldPageStatePersister(this);
            }
        }

        #region Constants
        private const string C_JOB_VS = "C_JOB_VS";
        private const string C_CONFIRM_EXTRA_ADD_VS = @"javascript:return(confirm('Please ensure you update the job\'s pricing in order to save the newly added extra.'))";
        private const string C_UDPATE_EXTRA_ADD_VS = @"javascript:return(confirm('Please ensure you update the job\'s pricing to save the update to this extra.'))";
        private readonly string C_TRAFFIC_SHEET_XML = "TrafficSheetFilterXML";

        #endregion

        private Entities.Job m_job;

        public Entities.Job Job
        {
            get
            {
                if (m_job != null)
                {
                    return m_job;
                }
                else
                {
                    return GetJobEntityFromCache();
                }
            }
            set { m_job = value; }
        }

        protected string m_subbyName = string.Empty;

        #region Page Variables

        protected int m_jobId;
        private XmlDocument m_jobXml;

        protected Entities.TrafficSheetFilter _trafficSheetFilter = null;

        #endregion

        #region Page Load/Init

        private TextBox m_txtSubContractRate;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            // Check to see if the user has requested to remove an order, or uncontract a leg
            string target = Request.Params["__EVENTTARGET"];
            string args = Request.Params["__EVENTARGUMENT"];

            if (!string.IsNullOrEmpty(target) && target.ToLower() == "uncontractleg" && !string.IsNullOrEmpty(args))
            {
                UncontractLeg(Convert.ToInt32(args));
                RefreshJobEntityCache();
            }

            // Hack: temporary means of grabbing new subcontract rate before it is overwritten by subsequent
            // postback (and prior to updating it), otherwise users new value is lost.
            // Requires fixing at some point.
            if (!string.IsNullOrEmpty(target) && target.EndsWith("lnkSaveRate"))
                m_txtSubContractRate = (TextBox)JobDisplay.FindControlRecursive(this, "txtSubContractRate");

            if (!string.IsNullOrEmpty(args) && args.ToLower() == "refresh")
                RefreshJobEntityCache();

            PopulateSubContract(m_jobId, true);
            LoadCookieDefaults();

            if (!IsPostBack)
            {
                // This is the first time the user has requested page
                RefreshJobEntityCache();

                //#region Add Refusals: Open the order shuffler for given orders so the user can add refusals.
                //string addRefusalOrderIds = Request.QueryString["ARO"]; /* ARO= Add Refusal Orders */
                //if (!string.IsNullOrEmpty(addRefusalOrderIds))
                //{
                //    string[] ids = addRefusalOrderIds.Split(",".ToCharArray());
                //    string js = string.Format("var shufflerUrl = 'manageorder.aspx?oid=|||&rid=|||'; var shufflerRowIds = '{0}'; var currentRowId= '{1}';", addRefusalOrderIds, ids[0]);
                //    this.ClientScript.RegisterClientScriptBlock(this.GetType(), "AddRefusalOrderIds", js, true);
                //    this.dlgOrderShuffler.Open();
                //    //
                //}
                //#endregion

                if (m_jobId > 0)
                    LoadJob();
            }
            else
            {
                m_jobId = Job.JobId;
                if (Job.JobType == eJobType.Groupage)
                {
                    LoadOrderHandlingForGroupage();
                }
            }

            //Hide the pricing, Show Load Order buttons if Groupage
            if (Job.JobType == eJobType.Groupage)
            {
                btnPricing.Style.Add(HtmlTextWriterStyle.Display, "none");
                trShowLoadOrder.Style.Add(HtmlTextWriterStyle.Display, "none");

                // Hide the Job Extras and Show the Manifest Production Panel
                pnlExtra.Visible = false;
                pnlManifest.Visible = true;

                //RefreshJobEntityCache();
                PopulateManifests();
            }

            //Hide the Produce PILs button if the job is not Groupage or Normal
            if (Job.JobType != eJobType.Groupage && Job.JobType != eJobType.Normal)
                trProducePILs.Style.Add(HtmlTextWriterStyle.Display, "none");

            btnPodLabels.Visible = Globals.Configuration.PodLabelsEnabled;

            this.dteManifestDate.SelectedDate = this.dteManifestDate.MinDate = DateTime.Today;
        }

        protected string plannedStartDateString = "";
        protected string plannedEndDateString = "";

        private void LoadOrderHandlingForGroupage()
        {
            bool retVal = false;

            DataAccess.Organisation dacOrg = new DataAccess.Organisation();

            ph_OrderHandling.Controls.Clear();
            DetailsOrderHandling oh = (DetailsOrderHandling)LoadControl("~/Job/Wizard/UserControls/DetailsOrderHandling.ascx");
            oh.Job = this.Job;
            oh.OnAfterMoveMergeCommand += new Orchestrator.WebUI.Job.Wizard.UserControls.AfterMoveMergeCommand(oh_OnAfterMoveMergeCommand);
            oh.Refresh += new EventHandler(oh_Refresh);
            ph_OrderHandling.Controls.Add(oh);
            trGroupageOrderHandling.Visible = true;

            if (this.Job.Instructions.Exists(i => i.CollectDrops.Exists(cd => cd.OrderAction == eOrderAction.Cross_Dock)))
                btnTippingSheet.Visible = true;

            foreach (Entities.Instruction ins in Job.Instructions)
            {
                if (ins.InstructionTypeId == (int)eInstructionType.Load || ins.InstructionTypeId == (int)eInstructionType.Drop)
                    foreach (Entities.CollectDrop cd in ins.CollectDrops)
                    {
                        bool trackingPalletType = DataAccess.PalletType.TrackingPalletTypeForClient(cd.Order.CustomerIdentityID, cd.PalletTypeID);
                        bool captureDebriefs = dacOrg.CaptureDeliveryDebriefsForClient(cd.Order.CustomerIdentityID);

                        if (trackingPalletType && captureDebriefs)
                        {
                            retVal = true;
                            break;
                        }
                    }

                if (ins.InstructionTypeId == (int)eInstructionType.PickupPallets || ins.InstructionTypeId == (int)eInstructionType.LeavePallets || ins.InstructionTypeId == (int)eInstructionType.DeHirePallets)
                    retVal = true;

                if (ins.Trailer != null)
                {
                    // Just a check to see of any pallets have been redeemed on this run (i.e. from PCVs) even if the current pallets are not being tracked.
                    Facade.ITrailer facTrailer = new Facade.Resource();
                    DataSet ds = facTrailer.GetPalletCountForTrailer(ins.Trailer.ResourceId, false);
                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        var queryResultsSet = ds.Tables[0].Rows.Cast<DataRow>().AsEnumerable();

                        var results = (from row in queryResultsSet
                                       where row.Field<int>("JobID") == ins.JobId
                                       select row);

                        if(results.Count() > 0)
                            retVal = true;
                    }
                }

                if (retVal)
                    break;
            }

            if (Job.Instructions.Count > 0)
            {
                plannedStartDateString = Job.Instructions[0].PlannedArrivalDateTime.ToString("dd/MM/yy");
                plannedEndDateString = Job.Instructions[(Job.Instructions.Count - 1)].PlannedArrivalDateTime.ToString("dd/MM/yy");
            }

            DateTime plannedStartDate;
            DateTime plannedEndDate;
            for (int i = 0; i < Job.Instructions.Count; i++)
            {
                Entities.Instruction ins = Job.Instructions[i];

                if (i == 0) plannedStartDate = ins.PlannedArrivalDateTime;

            }

            btnConfigurePalletHandling.Visible = retVal;

            if (retVal)
            {
                btnConfigurePalletHandling.OnClientClick = "openPalletHandlingWindow(" + m_jobId.ToString() + ");return false;";
                trRunPHAudit.Style.Add("display", "");
            }
            else
                trRunPHAudit.Style.Add("display", "none");
        }

        void oh_Refresh(object sender, EventArgs e)
        {
            this.Response.Redirect(this.Request.Url.ToString());
        }

        public void oh_OnAfterMoveMergeCommand()
        {
            grdTrafficSheet.DataSource = new Facade.Instruction().GetLegPlan(Job.Instructions, true).Legs();
            grdTrafficSheet.DataBind();
        }

        private void jobDetails_Init(object sender, EventArgs e)
        {
            m_jobId = Convert.ToInt32(Request.QueryString["jobId"]);
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

            repSubContractors.ItemDataBound += new RepeaterItemEventHandler(repSubContractors_ItemDataBound);
            this.chkSingleInvoice.CheckedChanged += new EventHandler(chkSingleInvoice_CheckedChanged);
            this.grdTrafficSheet.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdTrafficSheet_NeedDataSource);
            this.grdTrafficSheet.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(grdTrafficSheet_ItemDataBound);
            this.grdTrafficSheet.ItemCommand += new Telerik.Web.UI.GridCommandEventHandler(grdTrafficSheet_ItemCommand);
            btnProducePILs.Click += new EventHandler(btnProducePILs_Click);

            this.btnTippingSheet.Click += new EventHandler(btnTippingSheet_Click);
            this.btnPodLabels.Click += new EventHandler(btnPodLabels_Click);
            this.btnLoadingSummarySheet.Click += new EventHandler(btnLoadingSummarySheet_Click);

            this.dlgPalletHandling.DialogCallBack += new EventHandler(dlgPalletHandling_DialogCallBack);
            this.dlgAddDestination.DialogCallBack += (o, ev) => { this.Response.Redirect(this.Request.Url.ToString()); };
            this.dlgAddMultiDestination.DialogCallBack += (o, ev) => { this.Response.Redirect(this.Request.Url.ToString()); };
            this.dlgBookedTimes.DialogCallBack += (o, ev) => {  this.Response.Redirect(this.Request.Url.ToString()); };
            this.dlgCommunicate.DialogCallBack += (o, ev) => { this.Response.Redirect(this.Request.Url.ToString()); };
            this.dlgMultiTrunk.DialogCallBack += (o, ev) => { this.Response.Redirect(this.Request.Url.ToString()); };
            this.dlgPlannedTimes.DialogCallBack += (o, ev) => { this.Response.Redirect(this.Request.Url.ToString()); };
            this.dlgRemoveDestination.DialogCallBack += (o, ev) => { this.Response.Redirect(this.Request.Url.ToString()); };
            this.dlgRemoveTrunk.DialogCallBack += (o, ev) => {  this.Response.Redirect(this.Request.Url.ToString()); };
            this.dlgResourceThis.DialogCallBack += (o, ev) => {  this.Response.Redirect(this.Request.Url.ToString()); };
            this.dlgSubcontract.DialogCallBack += (o, ev) => {  this.Response.Redirect(this.Request.Url.ToString()); };
            this.dlgTrafficArea.DialogCallBack += (o, ev) => {  this.Response.Redirect(this.Request.Url.ToString()); };
            this.dlgTrailerType.DialogCallBack += (o, ev) => {  this.Response.Redirect(this.Request.Url.ToString()); };
            this.dlgTrunk.DialogCallBack += (o, ev) => {  this.Response.Redirect(this.Request.Url.ToString()); };
            this.dlgUpdateOrderDelivery.DialogCallBack += (o, ev) => {  this.Response.Redirect(this.Request.Url.ToString()); };
            this.dlgAddOrder.DialogCallBack += (o, ev) => {  this.Response.Redirect(this.Request.Url.ToString()); };
            this.dlgUpdateDockets.DialogCallBack += (o, ev) => {  this.Response.Redirect(this.Request.Url.ToString()); };
            this.dlgChangePlanningDepot.DialogCallBack += (o, ev) => {  this.Response.Redirect(this.Request.Url.ToString()); };
            RadMenu1.PreRender += new EventHandler(RadMenu1_PreRender);
            this.lnkCopyRun.Click += new EventHandler(lnkCopyRun_Click);
            this.lnkMergeRun.Click += LnkMergeRun_Click;
            dlgMergeRun.DialogCallBack += (o, ev) =>
            {
                this.Response.Redirect(this.Request.Url.ToString());
            };

            this.grdMwfMessages.NeedDataSource += grdMwfMessages_NeedDataSource;
            this.grdMwfMessages.ItemDataBound += grdMwfMessages_ItemDataBound;
        }

        private void LnkMergeRun_Click(object sender, EventArgs e)
        {
            if(Job.JobState > eJobState.InProgress)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "showalert", "alert('The current Run is in a state where no other runs can be merged to it');", true);
                return;
            }
            var qs = "jid=" + m_jobId;
            dlgMergeRun.Open(qs);            
        }

        void RadMenu1_PreRender(object sender, EventArgs e)
        {
            Telerik.Web.UI.RadMenuItem rmi = RadMenu1.Items.FindItemByText("Quick Communicate This");
            if(rmi != null)
                rmi.Visible = Orchestrator.Globals.Configuration.ShowQuickCommunicateThis;
        }

        void dlgPalletHandling_DialogCallBack(object sender, EventArgs e)
        {
            this.Response.Redirect(this.Request.Url.ToString());
        }

        void btnPodLabels_Click(object sender, EventArgs e)
        {
            //Use the Pub-Sub service to Print the POD Labels
            Facade.PODLabelPrintingService podLabelPrintingService = new Facade.PODLabelPrintingService();
            bool isPrinted = podLabelPrintingService.PrintPODLabels(this.Job.JobId);

            string message;
            if (isPrinted)
                message = "Your POD labels have been sent to the Printer";
            else
                message = "The POD Label Printing Service is not available. Please restart the service on your print server and try again.";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "PrintPODLabel", "alert('" + message + "');", true);
        }

        void btnTippingSheet_Click(object sender, EventArgs e)
        {
            NameValueCollection reportParams = new NameValueCollection();
            ReadOnlyCollection<Entities.Instruction> foundInstructions = null;

            if (this.Job.JobType == eJobType.Groupage && this.Job.Instructions.Exists(i => i.CollectDrops.Exists(cd => cd.OrderAction == eOrderAction.Cross_Dock)))
                foundInstructions = this.Job.Instructions.FindAll(i => i.CollectDrops.Exists(cd => cd.OrderAction == eOrderAction.Cross_Dock));

            if (foundInstructions != null)
            {
                #region Pop-up Report

                DataSet ds = null;
                StringBuilder sb = new StringBuilder();
                Facade.IJob facJob = new Facade.Job();

                foreach (Entities.Instruction i in foundInstructions)
                    if (sb.Length > 0)
                    {
                        if (!sb.ToString().Contains(i.InstructionID.ToString()))
                            sb.Append("," + i.InstructionID.ToString());
                    }
                    else
                        sb.Append(i.InstructionID.ToString());

                ds = facJob.GetTipSheet(sb.ToString(), Orchestrator.Globals.Configuration.PalletNetworkID);

                //-------------------------------------------------------------------------------------	
                //									Load Report Section 
                //-------------------------------------------------------------------------------------	
                Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.TippingSheet;
                Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;
                Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = ds;
                Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
                Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";

                // Show the user control
                Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", "<script language=\"javascript\">window.open('" + Page.ResolveClientUrl("../reports/reportviewer.aspx?wiz=true") + "');</script>");

                #endregion
            }
        }

        void btnProducePILs_Click(object sender, EventArgs e)
        {
            NameValueCollection reportParams = new NameValueCollection();
            DataSet dsPIL = null;

            List<int> orderIDs = new List<int>();
            List<int> jobIDs = new List<int>();

            //if (Job.JobType == eJobType.Normal)
            //{
            //    jobIDs.Add(Job.JobId);
            //}
            //else if (this.Job.JobType == eJobType.Groupage)
            //{
                Facade.IOrder facOrder = new Facade.Order();
                List<Entities.Order> ordersOnJob = facOrder.GetForJobID(m_jobId);

                foreach (Entities.Order order in ordersOnJob)
                {
                    if (!orderIDs.Contains(order.OrderID))
                        orderIDs.Add(order.OrderID);
                }
            //}

            Orchestrator.Facade.ICollectDropLoadOrder facLoadOrder = new Orchestrator.Facade.CollectDrop();

            if (orderIDs.Count > 0 || jobIDs.Count > 0)
            {
                #region Pop-up Report

                dsPIL = facLoadOrder.GetPILData(Entities.Utilities.GetCSV(orderIDs));

                //-------------------------------------------------------------------------------------	
                //									Load Report Section 
                //-------------------------------------------------------------------------------------	
                Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.PIL;
                Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;
                Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsPIL;
                Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
                Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";

                // Show the user control
                Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", "<script language=\"javascript\">window.open('" + Page.ResolveClientUrl("../reports/reportviewer.aspx?wiz=true") + "');</script>");

                #endregion
            }
        }

        void btnLoadingSummarySheet_Click(object sender, EventArgs e)
        {
            List<int> LocalJobIDs = new List<int>();
            LocalJobIDs.Add(m_jobId);

            if (LocalJobIDs.Count > 0)
            {
                #region Pop-up Report
                Facade.IJob facJob = new Facade.Job();
                NameValueCollection reportParams = new NameValueCollection();
                DataSet dsLSS = facJob.GetLoadingSummarySheet(LocalJobIDs);

                //-------------------------------------------------------------------------------------	
                //									Load Report Section 
                //-------------------------------------------------------------------------------------	
                Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.LoadingSummarySheet;
                Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;
                Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsLSS;
                Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
                Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";

                // Show the user control
                Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", "<script language=\"javascript\">window.open('" + Page.ResolveClientUrl("../reports/reportviewer.aspx?wiz=true") + "');</script>");
                #endregion
            }
        }

        void grdTrafficSheet_ItemCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
        {
            TextBox txtRate = e.Item.FindControl("txtSubContractRate") as TextBox;
            HtmlAnchor lnkViewInvoice = e.Item.FindControl("lnkViewInvoice") as HtmlAnchor;
            LinkButton lnkSubContractRate = e.Item.FindControl("lnkSubContractRate") as LinkButton;
            LinkButton lnkCancel = e.Item.FindControl("lnkCancel") as LinkButton;
            LinkButton lnkSave = e.Item.FindControl("lnkSaveRate") as LinkButton;

            switch (e.CommandName.ToLower())
            {
                case "edit":
                    e.Item.Edit = true;
                    break;
                case "cancel":
                    e.Item.Edit = false;
                    break;
                case "save":
                    e.Item.Edit = false;
                    decimal rate = 0;
                    int lcID = int.Parse(txtRate.Attributes["LCID"]);
                    Facade.IJobSubContractor facSub = new Facade.Job();
                    CultureInfo culture = new CultureInfo(lcID);

                    if (Decimal.TryParse(txtRate.Text, System.Globalization.NumberStyles.Currency, culture, out rate))
                    {
                        int jobSubContractID = int.Parse(txtRate.Attributes["jobSubContractID"]);

                        facSub.UpdateRate(jobSubContractID, rate, this.User.Identity.Name);
                        lnkSubContractRate.Text = rate.ToString("C", culture);
                        Job.SubContractors = facSub.GetSubContractorForJobId(Job.JobId);

                        AddJobEntityToCache(Job);
                    }
                    break;
            }

            LoadJob();
        }

        void grdTrafficSheet_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is Telerik.Web.UI.GridDataItem)
            {
                Entities.LegView leg = e.Item.DataItem as Entities.LegView;

                int cellcount = 11;
                if (leg.StartLegPoint.Instruction != null && (leg.StartLegPoint.Instruction.InstructionState != leg.EndLegPoint.Instruction.InstructionState))
                {
                    // display the differences
                    if (leg.StartLegPoint.Instruction.InstructionState == eInstructionState.Planned)
                    {
                        for (int i = 0; i < cellcount; i++)
                        {
                            //e.Item.Cells[i].BackColor = ColorTranslator.FromHtml(Orchestrator.WebUI.Utilities.GetJobStateColourForHTML(Orchestrator.eJobState.Planned)); ;
                            e.Item.Cells[i].CssClass = "LegstatePlanned";
                        }
                    }
                    if (leg.StartLegPoint.Instruction.InstructionState == eInstructionState.InProgress)
                    {
                        for (int i = 0; i < cellcount; i++)
                            e.Item.Cells[i].CssClass = "LegstateInProgress";
                        //e.Item.Cells[i].BackColor = ColorTranslator.FromHtml(Orchestrator.WebUI.Utilities.GetJobStateColourForHTML(Orchestrator.eJobState.InProgress));
                    }
                    if (leg.StartLegPoint.Instruction.InstructionState == eInstructionState.Completed)
                    {
                        for (int i = 0; i < cellcount; i++)
                            e.Item.Cells[i].CssClass = "LegstateCompleted";
                        //e.Item.Cells[i].BackColor = ColorTranslator.FromHtml(Orchestrator.WebUI.Utilities.GetJobStateColourForHTML(Orchestrator.eJobState.Completed));
                    }
                    if (leg.EndLegPoint.Instruction.InstructionState == eInstructionState.Planned)
                    {
                        for (int i = 7; i < e.Item.Cells.Count; i++)
                            e.Item.Cells[i].CssClass = "LegstatePlanned";
                        //e.Item.Cells[i].BackColor = ColorTranslator.FromHtml(Orchestrator.WebUI.Utilities.GetJobStateColourForHTML(Orchestrator.eJobState.Planned));
                    }
                    if (leg.EndLegPoint.Instruction.InstructionState == eInstructionState.InProgress)
                    {
                        for (int i = 7; i < e.Item.Cells.Count; i++)
                            e.Item.Cells[i].CssClass = "LegstateInProgress";
                        //e.Item.Cells[i].BackColor = ColorTranslator.FromHtml(Orchestrator.WebUI.Utilities.GetJobStateColourForHTML(Orchestrator.eJobState.InProgress));
                    }
                    if (leg.EndLegPoint.Instruction.InstructionState == eInstructionState.Completed)
                    {
                        for (int i = 7; i < e.Item.Cells.Count; i++)
                            e.Item.Cells[i].CssClass = "LegstateCompleted";
                        //e.Item.Cells[i].BackColor = ColorTranslator.FromHtml(Orchestrator.WebUI.Utilities.GetJobStateColourForHTML(Orchestrator.eJobState.Completed)); 
                    }
                }
                else
                {
                    #region Set the Leg Color display
                    switch (leg.State)
                    {
                        case Entities.LegView.eLegState.Planned:
                            e.Item.CssClass = "LegstatePlanned";
                            break;
                        case Entities.LegView.eLegState.InProgress:
                            e.Item.CssClass = "LegstateInProgress";
                            break;
                        case Entities.LegView.eLegState.Completed:
                            e.Item.CssClass = "LegstateCompleted";
                            break;
                    }
                    #endregion
                }

                // Plan By
                HtmlAnchor hypPlanBy = e.Item.FindControl("hypPlanBy") as HtmlAnchor;
                hypPlanBy.InnerText = leg.DepotCode;
                int startInstructionID = 0;
                if (leg.StartLegPoint.Instruction != null)
                    startInstructionID = leg.StartLegPoint.Instruction.InstructionID;
                else
                    startInstructionID = ((ReadOnlyCollection<Entities.LegView>)grdTrafficSheet.DataSource)[leg.Instruction.InstructionOrder - 2].InstructionID;
                hypPlanBy.HRef = string.Format(@"javascript:openTrafficAreaWindow({0},'{1}',{2});", startInstructionID.ToString(), Job.LastUpdateDate.ToString("dd/MM/yy HH:mm:ss"), Job.JobId.ToString());

                // Start
                Label lblStart = e.Item.FindControl("lblStart") as Label;
                lblStart.Text = leg.StartLegPoint.PlannedDateTime.ToString("dd/MM HH:mm");

                // Collection Point
                HtmlGenericControl spnCollectionPoint = e.Item.FindControl("spnCollectionPoint") as HtmlGenericControl;
                //spnCollectionPoint.Attributes.Add("onmouseover", "ShowPointToolTip(this," + leg.StartLegPoint.Point.PointId.ToString() + ");");
                spnCollectionPoint.Attributes.Add("pointid", leg.StartLegPoint.Point.PointId.ToString());
                //spnCollectionPoint.Attributes.Add("onmouseout", "closeToolTip();");
                //spnCollectionPoint.Attributes.Add("class", "orchestratorLink");
                spnCollectionPoint.InnerText = leg.StartLegPoint.Point.Description;

                // Finish
                Label lblFinish = e.Item.FindControl("lblFinish") as Label;
                lblFinish.Text = leg.EndLegPoint.PlannedDateTime.ToString("dd/MM HH:mm");

                // Destination Point
                HtmlGenericControl spnDestinationPoint = e.Item.FindControl("spnDestinationPoint") as HtmlGenericControl;
                spnDestinationPoint.Attributes.Add("pointid", leg.EndLegPoint.Point.PointId.ToString());
                //spnDestinationPoint.Attributes.Add("onmouseover", "ShowPointToolTip(this," + leg.EndLegPoint.Point.PointId.ToString() + ");");
                //spnDestinationPoint.Attributes.Add("onmouseout", "closeToolTip();");
                //spnDestinationPoint.Attributes.Add("class", "orchestratorLink");

                spnDestinationPoint.InnerText = leg.EndLegPoint.Point.Description;

                // Driver
                string driverFullName = "";

                HtmlGenericControl spnDriver = e.Item.FindControl("spnDriver") as HtmlGenericControl;

                spnDriver.Attributes.Add("onmouseout", "javascript:closeToolTip();");
                spnDriver.Attributes.Add("class", "orchestratorLink");

                if (leg.SubContractorForDisplay != null && leg.Driver == null)
                {
                    // The driver is a subby. Pass the Organisation IdentityId.
                    driverFullName = spnDriver.InnerText = leg.SubContractorForDisplay.OrganisationDisplayName;
                    spnDriver.Attributes.Add("onmouseover", "javascript:ShowContactInformationToolTip(this, 'Organisation:" + leg.SubContractorForDisplay.IdentityId.ToString() + "');");
                }
                else if (leg.SubContractorForDisplay == null && leg.Driver != null)
                {
                    // The driver is an inhouse resource. Pass Individual IdentityId.
                    // The driver is a subby. Pass the Organisation IdentityId.
                    driverFullName = spnDriver.InnerText = leg.Driver.Individual.FullName;
                    spnDriver.Attributes.Add("onmouseover", "javascript:ShowContactInformationToolTip(this, 'Individual:" + leg.Driver.Individual.IdentityId + "');");
                }

                // Vehicle
                string resourceWindowLink = string.Format(string.Format(@"javascript:openResourceWindow({0},'{1}', '{2}', '{3}', '{4}', '{5}', '{6}' , '{7}', '{8}', '{9}', '{10}', '{11}' )",
                                                leg.InstructionID.ToString(), // instructionId
                                                leg.Driver == null ? "" : leg.Driver.Individual.FullName, // Fullname
                                                leg.Driver == null ? "" : leg.Driver.ResourceId.ToString(), //DriverResourceId
                                                leg.Vehicle == null ? "" : leg.Vehicle.RegNo,
                                                leg.Vehicle == null ? "" : leg.Vehicle.ResourceId.ToString(),
                                                leg.Trailer == null ? "" : leg.Trailer.TrailerRef,
                                                leg.Trailer == null ? "" : leg.Trailer.ResourceId.ToString(),
                                                leg.StartLegPoint.PlannedDateTime.ToString("dd/MM/yy HH:mm"),
                                                leg.EndLegPoint.PlannedDateTime.ToString("dd/MM/yy HH:mm"),
                                                leg.DepotCode, // DepotCode
                                                Job.LastUpdateDate.ToString("dd/MM/yy HH:mm:ss"),
                                                Job.JobId.ToString()
                                                ));

                HtmlAnchor hypVehicle = e.Item.FindControl("hypVehicle") as HtmlAnchor;
                hypVehicle.HRef = resourceWindowLink;
                hypVehicle.InnerText = leg.Vehicle == null ? "" : leg.Vehicle.RegNo;

                // Trailer
                HtmlAnchor hypTrailer = e.Item.FindControl("hypTrailer") as HtmlAnchor;
                hypTrailer.HRef = resourceWindowLink;
                hypTrailer.InnerText = leg.Trailer == null ? "" : leg.Trailer.TrailerRef;

                // Subby Rate
                TextBox txtRate = e.Item.FindControl("txtSubContractRate") as TextBox;
                LinkButton lnkCancel = e.Item.FindControl("lnkCancel") as LinkButton;
                LinkButton lnkSave = e.Item.FindControl("lnkSaveRate") as LinkButton;
                HtmlAnchor lnkViewInvoice = e.Item.FindControl("lnkViewInvoice") as HtmlAnchor;
                LinkButton lnkSubContractRate = e.Item.FindControl("lnkSubContractRate") as LinkButton;

                txtRate.Visible = e.Item.Edit;
                lnkCancel.Visible = e.Item.Edit;
                lnkSave.Visible = e.Item.Edit;
                lnkSubContractRate.Visible = !(e.Item.Edit);

                // Set the leg subby rate
                if (leg.Instruction.JobSubContractID > 0)
                {
                    foreach (Entities.JobSubContractor jobSubContractor in Job.SubContractors)
                    {
                        if (jobSubContractor.JobSubContractID == leg.Instruction.JobSubContractID)
                        {
                            CultureInfo culture = new CultureInfo(jobSubContractor.LCID);

                            // We have found the job subcontract object - now get the rate
                            lnkSubContractRate.Text = jobSubContractor.ForeignRate.ToString("C", culture);
                            txtRate.Text = jobSubContractor.ForeignRate.ToString("C", culture);

                            Facade.IJobSubContractor facJobSubContractor = new Facade.Job();
                            lnkSubContractRate.Enabled = txtRate.Enabled = facJobSubContractor.IsRateEditable(jobSubContractor.JobSubContractID);

                            txtRate.Attributes.Add("jobSubContractID", jobSubContractor.JobSubContractID.ToString());
                            txtRate.Attributes.Add("LCID", jobSubContractor.LCID.ToString());

                            // We have found the jobsubcontractor so we can break from the loop.
                            break;
                        }
                    }
                }

                // Set the hidden key values
                const string SEPARATOR = "|";

                StringBuilder sb = new StringBuilder();
                sb.Append(Job.JobId.ToString()); // 0: JobId
                sb.Append(SEPARATOR);
                sb.Append(leg.InstructionID.ToString());        // 1: InstructionId
                sb.Append(SEPARATOR);

                if (leg.SubContractorForDisplay != null && leg.Driver == null)
                {
                    // Don't add resourced driver references to the keys collection as it should not get passed in the 
                    // query string to pages such as resourcethis.aspx
                    sb.Append(driverFullName);                  // 2: FullName
                    sb.Append(SEPARATOR);                       // 3: DriverResourceId
                    sb.Append("-1");
                    sb.Append(SEPARATOR);                       // 4: SubContractorId
                    sb.Append(leg.SubContractorForDisplay.IdentityId);
                    sb.Append(SEPARATOR);

                }
                else if (leg.SubContractorForDisplay == null && leg.Driver != null)
                {
                    // Check for possible resourced driver
                    sb.Append(driverFullName);                  // 2: FullName
                    sb.Append(SEPARATOR);
                    sb.Append(leg.Driver.ResourceId.ToString());// 3: DriverResourceId
                    sb.Append(SEPARATOR);                       // 4: SubContractorId
                    sb.Append("-1");
                    sb.Append(SEPARATOR);
                }
                else
                {
                    // Don't add resourced driver references to the keys collection as it should not get passed in the 
                    // query string to pages such as resourcethis.aspx
                    sb.Append(SEPARATOR);                       // 2: FullName
                    sb.Append(SEPARATOR);                       // 3: DriverResourceId
                    sb.Append(SEPARATOR);                       // 4: SubContractorId
                }

                if (leg.Vehicle == null)
                {
                    // Don't add resourced vehicle references to the keys collection as it should not get passed in the 
                    // query string to pages such as resourcethis.aspx
                    sb.Append(SEPARATOR); // 5: RegNo
                    sb.Append(SEPARATOR); // 6: VehicleResourceId
                }
                else
                {
                    // Check for possible resourced vehicle
                    sb.Append(leg.Vehicle.RegNo); // 5: RegNo
                    sb.Append(SEPARATOR);
                    sb.Append(leg.Vehicle.ResourceId.ToString()); // 6: VechicleResourceId
                    sb.Append(SEPARATOR);
                }

                sb.Append(leg.Trailer == null ? "" : leg.Trailer.TrailerRef); // 7: TrailerRef
                sb.Append(SEPARATOR);
                sb.Append(leg.Trailer == null ? "" : leg.Trailer.ResourceId.ToString()); // 8: TrailerResourceId
                sb.Append(SEPARATOR);
                sb.Append(leg.StartLegPoint.PlannedDateTime.ToString("dd/MM/yy HH:mm:ss")); // 9: PlannedDateTime (Start)
                sb.Append(SEPARATOR);
                sb.Append(leg.EndLegPoint.PlannedDateTime.ToString("dd/MM/yy HH:mm:ss")); // 10: PlannedDateTime (End)
                sb.Append(SEPARATOR);
                sb.Append(leg.DepotCode); // 11: DepotCode
                sb.Append(leg.TrafficArea == null ? "" : leg.TrafficArea.ShortName.Trim());
                sb.Append(SEPARATOR);
                sb.Append(Job.LastUpdateDate.ToString("dd/MM/yy HH:mm:ss")); // 12: LastUpdateDate
                sb.Append(SEPARATOR);
                sb.Append(((int)leg.State).ToString()); // 13: State
                sb.Append(SEPARATOR);
                sb.Append(((int)Job.JobType).ToString()); // 14: Job TypeID
                sb.Append(SEPARATOR);
                sb.Append(leg.StartLegPoint.Instruction != null ? leg.StartLegPoint.Instruction.InstructionID.ToString() : "-1"); //15: Start InstructionID;
                sb.Append(SEPARATOR);
                sb.Append(leg.StartLegPoint.Instruction != null ? ((int)leg.StartLegPoint.Instruction.InstructionState).ToString() : "-1"); //16: Start Instruction State;

                var allowMwfMessaging =
                    leg.Driver != null &&
                    leg.Instruction.DriverCommunication != null &&
                    !string.IsNullOrWhiteSpace(leg.Driver.Passcode) &&
                    leg.Instruction.DriverCommunication.DriverCommunicationType == eDriverCommunicationType.ToughTouch;

                sb.Append(SEPARATOR);
                sb.Append(allowMwfMessaging.ToString().ToLower()); //17: AllowMwfMessaging

                ((ITextControl)e.Item.FindControl("lblHiddenKeys")).Text = sb.ToString();
            }
        }

        void lnkCopyRun_Click(object sender, EventArgs e)
        {
            Facade.IJob facJob = new Facade.Job();
            int jobID = facJob.Copy(Job, Page.User.Identity.Name);
            Response.Redirect("job.aspx?jobId=" + jobID + "&csid=" + this.CookieSessionID);
        }

        void grdTrafficSheet_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            grdTrafficSheet.DataSource = new Facade.Instruction().GetLegPlan(Job.Instructions, true).Legs();
        }

        void chkSingleInvoice_CheckedChanged(object sender, EventArgs e)
        {
            Entities.Job j = GetJobEntityFromCache();
            j.IsSingleInvoice = chkSingleInvoice.Checked;
            Facade.IJob facJob = new Facade.Job();
            facJob.UpdateSingleInvoiceState(j, this.Page.User.Identity.Name);
        }

        void repSubContractors_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Facade.IOrganisation facOrg = new Facade.Organisation();
            Entities.JobSubContractor js = null;
            Entities.Organisation org = null;
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                js = (Entities.JobSubContractor)e.Item.DataItem;
                // We need the SubContractors Name
                decimal rate = ((Entities.JobSubContractor)e.Item.DataItem).ForeignRate;
                org = facOrg.GetForIdentityId(js.ContractorIdentityId);

                CultureInfo culture = new CultureInfo(((Entities.JobSubContractor)e.Item.DataItem).LCID);

                // The invoice details
                Label lblOrganisationName = e.Item.FindControl("lblSubContractor") as Label;
                lblOrganisationName.Text = org.OrganisationDisplayName;

                HtmlAnchor lnkViewInvoice = e.Item.FindControl("lnkViewInvoice") as HtmlAnchor;
                lnkViewInvoice.InnerText = js.InvoiceID.ToString();
                lnkViewInvoice.HRef = "~/" + js.PDFLocation.ToString();

                Label lblSubContractRate = e.Item.FindControl("lblSubContractRate") as Label;
                lblSubContractRate.Text = rate.ToString("C", culture);

                Label lblSubcontractReference = e.Item.FindControl("lblSubcontractReference") as Label;
                lblSubcontractReference.Text = js.Reference;

                Label lblIsAttended = e.Item.FindControl("lblIsAttended") as Label;
                lblIsAttended.Text = js.IsAttended ? "Yes" : "No";
            }
        }

        #endregion

        private void LoadJob()
        {
            // Dont get the job entity xml if its a groupage job.
            if (Job.JobType != eJobType.Groupage)
                m_jobXml = Job.ToXml();

            PopulateJobInformation(Job);

            // Load the Extras 
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

            this.pnlMwfMessages.Visible = Globals.Configuration.IsMwfCustomer;

            LoadCookieDefaults();
        }

        #region Extra Stuff

        protected Entities.ExtraCollection GetExtras()
        {
            Entities.ExtraCollection extras = new Entities.ExtraCollection();

            foreach (Entities.Extra extra in Job.Extras)
            {
                if (extra.ExtraType != eExtraType.Demurrage)
                    extras.Add(extra);
            }
            return extras;
        }

        protected void cboExtraState_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList cboExtraState = (DropDownList)sender;

            eExtraState state = (eExtraState)Enum.Parse(typeof(eExtraState), cboExtraState.SelectedValue.Replace(" ", ""));

            DataGridItem containingItem = (DataGridItem)cboExtraState.Parent.Parent;
            ((RequiredFieldValidator)containingItem.FindControl("rfvClientContact")).Enabled = state == eExtraState.Accepted;
        }
        #endregion

        #region Properties

        protected string ControlAreaId
        {
            get
            {
                return _trafficSheetFilter.ControlAreaId.ToString();
            }
        }

        protected string DepotID
        {

            get
            {
                return _trafficSheetFilter.DepotId.ToString();
            }
        }

        protected string TrafficAreaIds
        {
            get { return Entities.Utilities.CommaSeparatedIDs(_trafficSheetFilter.TrafficAreaIDs); }
        }

        public string StartDate
        {
            get { return _trafficSheetFilter.FilterStartDate.ToString("dd/MM/yy"); }
        }

        public string EndDate
        {
            get { return _trafficSheetFilter.FilterEnddate.ToString("dd/MM/yy"); }
        }

        public string DepotId
        {
            get { return _trafficSheetFilter.DepotId.ToString(); }
        }

        #endregion

        private void PopulateJobInformation(Entities.Job job)
        {
            // Populate the Job fieldset.
            lblJobId.Text = m_jobId.ToString();
            ucJobStateIndicator.JobState = job.JobState;
            this.Master.WizardTitle = "Run Details [" + job.JobId + "]";

            this.hypRunHistory.HRef = string.Format("javascript:{0}", this.dlgRunHistory.GetOpenDialogScript(string.Format("JobId={0}", this.Job.JobId)));

            spanEditJob.Visible = job.JobType != eJobType.Groupage;

            // Groupage jobs should not display this invoice link. This code should not be 
            // reached for groupage jobs as they should never have a job state of invoiced.
            if ((eJobState)job.JobState == eJobState.Invoiced)
            {
                int invoiceId = 0;
                Facade.IInvoice facInvoice = new Facade.Invoice();
                DataSet ds = facInvoice.GetInvoiceIdForJobId(job.JobId);
                invoiceId = Convert.ToInt32(ds.Tables[0].Rows[0]["InvoiceId"].ToString());

                if (invoiceId != 0)
                {
                    hlInvoice.NavigateUrl = "~/Invoicing/addupdateInvoice.aspx?InvoiceId=" + invoiceId; //"javascript:openDialogWithScrollbars('../Invoicing/addupdateInvoice.aspx?InvoiceId=" + invoiceId.ToString() + ",'600','400')"; 
                    hlInvoice.Text = "Invoice Id: " + invoiceId.ToString();
                    hlInvoice.Visible = true;
                }
                else
                    hlInvoice.Visible = false;

                lnkClient.Visible = false;
                lblClient.Visible = true;
                hlBusinessType.Enabled = false;
                hlNominalCode.Enabled = false;

            }
            else
            {
                hlInvoice.Visible = false;
                lnkClient.Visible = true;
                lblClient.Visible = false;
            }

            lblClient.Text = job.Client;
            lnkClient.Text = job.Client;

            lblJobType.Text = Utilities.UnCamelCase(job.JobType.ToString());
            if (job.BusinessTypeID > 0)
            {
                Facade.IBusinessType facBusinessType = new Facade.BusinessType();
                Entities.BusinessType businessType = facBusinessType.GetForBusinessTypeID(job.BusinessTypeID);

                if (businessType != null)
                    hlBusinessType.Text = businessType.Description;
                else
                    hlBusinessType.Text = "No Business Type Set";
            }
            else
                hlBusinessType.Text = "No Business Type Set";

            if (job.NominalCode != null)
                hlNominalCode.Text = job.NominalCode.ShortCode;
            else
                hlNominalCode.Text = "No Nominal Code Set";

            chkSingleInvoice.Checked = job.IsSingleInvoice;

            if (job.CurrentTrafficArea == null)
                lblCurrentTrafficArea.Text = "Unknown";
            else
                lblCurrentTrafficArea.Text = job.CurrentTrafficArea.TrafficAreaName;

            lblStockMovement.Text = (job.IsStockMovement ? "Yes" : "No");

            using (Facade.IPlannerRequest facPlannerRequest = new Facade.PlannerRequest())
            {
                if ((facPlannerRequest.GetPlannerRequestsForJobId(job.JobId)).Tables[0].Rows.Count > 0)
                {
                    imgHasRequests.Visible = true;
                    imgHasRequests.Attributes.Add("onClick", "javascript:ShowPlannerRequests('" + job.JobId.ToString() + "');");
                }
                else
                    imgHasRequests.Visible = false;
            }

            // Populate the Job Details fieldset.
            trLoadNumber.Visible = job.JobType != eJobType.Groupage;
            Entities.Organisation client;
            using (Facade.IOrganisation facOrganisation = new Facade.Organisation())
            {
                client = facOrganisation.GetForIdentityId(job.IdentityId);
            }
            lblLoadNumber.Text = job.LoadNumber;
            lblLoadNumberText.Text = client.LoadNumberText;
            repJobReferences.DataSource = job.References;
            repJobReferences.DataBind();
            if (job.JobType == eJobType.Return)
                lblReturnReferenceNumber.Text = job.ReturnReceiptNumber;
            else
                lblReturnReferenceNumber.Text = "N/A";
            
            tdTransShippingSheet.Visible = false;

            btnAddExtra.Visible = job.JobType != eJobType.Groupage;

            int numberOfPallets = 0;
            bool hasAtLeastOneCalledInInstruction = false;

            foreach (Entities.Instruction instruction in job.Instructions)
            {
                if (instruction.InstructionTypeId == (int)eInstructionType.Load)
                    numberOfPallets = numberOfPallets + instruction.TotalPallets;

                if (instruction.InstructionActuals != null && instruction.InstructionActuals.Count > 0)
                    hasAtLeastOneCalledInInstruction = true;
            }

            lblNumberOfPallets.Text = numberOfPallets.ToString();

            if (job.ForCancellation)
            {
                lblMarkedForCancellation.Text = "Yes - " + job.ForCancellationReason;
                tdCancelJobAndOrders.Visible = tdCancelJob.Visible = false;
            }
            else
            {
                lblMarkedForCancellation.Text = "No";
                if (!hasAtLeastOneCalledInInstruction && (job.JobState == eJobState.Booked || job.JobState == eJobState.Planned || job.JobState == eJobState.InProgress))
                    tdCancelJobAndOrders.Visible = tdCancelJob.Visible = true;
                else
                    tdCancelJobAndOrders.Visible = tdCancelJob.Visible = false;
            }


            Orchestrator.Facade.IJobSubContractor facJS = new Orchestrator.Facade.Job();

            if (facJS.IsOnSubcontractorInvoice(m_jobId))
            {
                tdCancelJobAndOrders.Visible = tdCancelJob.Visible = false;
            }

            trCollectDrop.Visible = false;
            trGroupageOrderHandling.Visible = false;

            if (job.JobType == eJobType.Groupage)
            {
                LoadOrderHandlingForGroupage();
            }
            else
            {
                XslCompiledTransform transformer = new XslCompiledTransform();
                transformer.Load(Server.MapPath(@"..\xsl\instructions.xsl"));
                XmlUrlResolver resolver = new XmlUrlResolver();
                XPathNavigator navigator = m_jobXml.CreateNavigator();

                // Populate the Collections.
                XsltArgumentList collectionArgs = new XsltArgumentList();
                collectionArgs.AddParam("InstructionTypeId", "", "1, 5");
                collectionArgs.AddParam("DocketText", "", client.DocketNumberText);
                collectionArgs.AddParam("webserver", "", Orchestrator.Globals.Configuration.WebServer);
                collectionArgs.AddParam("showShortAddress", "", "true");
                collectionArgs.AddParam("allowRemoveOrderButton", "", "false");

                string userId = ((Entities.CustomPrincipal)Page.User).UserName;
                int plannerId = ((Entities.CustomPrincipal)Page.User).IdentityId;

                collectionArgs.AddParam("userId", "", userId);
                collectionArgs.AddParam("plannerId", "", plannerId);

                StringWriter sw = new StringWriter();
                transformer.Transform(navigator, collectionArgs, sw);
                lblCollections.Text = sw.GetStringBuilder().ToString();

                // Populate the Deliveries.
                sw = new StringWriter();
                XsltArgumentList deliveryArgs = new XsltArgumentList();
                switch (job.JobType)
                {
                    case eJobType.Groupage:
                        deliveryArgs.AddParam("InstructionTypeId", "", "2, 7, 3, 4"); // Drops, Trunks, Leave Pallets and Dehire Pallets
                        break;
                    case eJobType.Normal:
                        deliveryArgs.AddParam("InstructionTypeId", "", "2, 3, 4"); // Drops, Leave Pallets and Dehire Pallets
                        break;
                    case eJobType.PalletReturn:
                        deliveryArgs.AddParam("InstructionTypeId", "", "3, 4"); // Leave Pallets and Dehire Pallets
                        break;
                    case eJobType.Return:
                        deliveryArgs.AddParam("InstructionTypeId", "", "2, 6"); // Drops and Leave Goods
                        break;
                }
                deliveryArgs.AddParam("DocketText", "", client.DocketNumberText);
                deliveryArgs.AddParam("webserver", "", Orchestrator.Globals.Configuration.WebServer);
                deliveryArgs.AddParam("showShortAddress", "", "true");
                transformer.Transform(navigator, deliveryArgs, sw);
                lblDeliveries.Text = sw.GetStringBuilder().ToString();

                trCollectDrop.Visible = true;
            }

            // Populate the links section.
            switch (job.JobType)
            {
                case eJobType.Normal:
                    ancEditJob.HRef = "javascript:openResizableDialogWithScrollbars('wizard/wizard.aspx?jobId=" + m_jobId.ToString() + "', '623', '508');";
                    break;
                case eJobType.Groupage:
                    ancEditJob.HRef = "javascript:openResizableDialogWithScrollbars('wizard/wizard.aspx?jobId=" + m_jobId.ToString() + "', '623', '508');";
                    break;
                case eJobType.PalletReturn:
                    ancEditJob.HRef = "addupdatepalletreturnjob.aspx?wiz=true&jobId=" + m_jobId.ToString();
                    break;
                case eJobType.Return:
                    ancEditJob.HRef = "addupdategoodsreturnjob.aspx?wiz=true&jobId=" + m_jobId.ToString();
                    break;
            }

            // Sub Contrater Section 
            PopulateSubContract(m_jobId);

            if (job.HasBeenSubContracted && job.SubContractors[0].SubContractWholeJob)
            {
                // Changed as job.Instructions is only valid before the job is created. Stephen Newman 09/06/07
                foreach (Entities.Instruction instruction in job.Instructions)
                    if (instruction.InstructionActuals != null && instruction.InstructionActuals.Count > 0)
                        btnUnSubContract.Enabled = false;
            }

            // Populate the job revenue information
            PopulateJobRevenueInformation(job);
        }

        private void PopulateJobRevenueInformation(Entities.Job thisJob)
        {
            // Get the total revenue.
            decimal totalRevenue = 0;

            Facade.IJobExtra facExtras = new Facade.Job();

            foreach (Entities.Extra currentExtra in Job.Extras)
                totalRevenue += currentExtra.ExtraAmount;

            DataSet extras = new DataSet();

            switch (thisJob.JobType)
            {
                case eJobType.Normal:
                case eJobType.PalletReturn:
                case eJobType.Return:
                    totalRevenue += thisJob.Charge.JobChargeAmount;
                    break;
                case eJobType.Groupage:
                    Facade.IOrderExtra facOrderExtra = new Facade.Order();
                    List<int> processedGroups = new List<int>();
                    foreach (Entities.Instruction instruction in thisJob.Instructions)
                        if (instruction.InstructionTypeId == (int)eInstructionType.Drop || instruction.InstructionTypeId == (int)eInstructionType.DeHirePallets)
                            foreach (Entities.CollectDrop collectDrop in instruction.CollectDrops)
                            {
                                totalRevenue += collectDrop.Order.Rate;
                                extras = facOrderExtra.GetExtrasForOrderID(collectDrop.Order.OrderID);

                                foreach (DataRow row in extras.Tables[0].Rows)
                                    totalRevenue += (decimal)row["ExtraAmount"];
                            }
                    break;
            }

            // Get the total sub-contractor cost.
            decimal totalSubContractorCost = 0;
            if (thisJob.SubContractors.Count > 0)
            {
                foreach (Entities.JobSubContractor jobSubContractor in thisJob.SubContractors)
                {
                    if (jobSubContractor.SubContractWholeJob)
                    {
                        totalSubContractorCost += jobSubContractor.Rate;
                    }
                    else
                    {
                        foreach (Entities.Instruction instruction in thisJob.Instructions)
                        {
                            if (instruction.JobSubContractID == jobSubContractor.JobSubContractID)
                                totalSubContractorCost += jobSubContractor.Rate;
                            else if (thisJob.JobType == eJobType.Groupage)
                            {
                                if (instruction.InstructionTypeId == (int)eInstructionType.Drop)
                                    foreach (Entities.CollectDrop collectDrop in instruction.CollectDrops)
                                        if (collectDrop.Order.JobSubContractID == jobSubContractor.JobSubContractID)
                                            totalSubContractorCost += jobSubContractor.Rate;

                            }
                        }
                    }
                }
            }

            // Output the results.
            lblTotalRevenue.Text = totalRevenue.ToString("C");
            lblTotalSubcontractorCost.Text = totalSubContractorCost.ToString("C");

            decimal totalMargin = totalRevenue - totalSubContractorCost;
            decimal percentageMargin = 0;

            if (totalRevenue > 0)
                percentageMargin = (totalMargin / totalRevenue);

            lblTotalMargin.Text = totalMargin.ToString("C");
            lblPercentageMargin.Text = percentageMargin.ToString("P", System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat).Replace(" %", "%");
        }

        private void PopulateSubContract(int jobId, bool reloadSubbies)
        {
            if (reloadSubbies)
            {
                Facade.IJobSubContractor facJobSubContractor = new Facade.Job();
                Job.SubContractors = facJobSubContractor.GetSubContractorForJobId(jobId);
                AddJobEntityToCache(Job);
            }

            PopulateSubContract(jobId);
        }

        private void PopulateSubContract(int jobId)
        {
            Facade.IOrganisation facOrganisation = new Facade.Organisation();

            // Bind legs as these now have the subby information on them.
            grdTrafficSheet_NeedDataSource(this, new Telerik.Web.UI.GridNeedDataSourceEventArgs(Telerik.Web.UI.GridRebindReason.ExplicitRebind));
            //removed as the telerik grid will call need datasource when it needs it.
            //grdTrafficSheet.DataBind();

            // 1 Sub Contractor for Whole job
            if (Job.SubContractors != null && Job.SubContractors.Count == 1 && Job.SubContractors[0].SubContractWholeJob)
            {
                // Only 1 Sub Contractor so We can show the bring back in house button.
                repSubContractors.DataSource = Job.SubContractors;
                repSubContractors.DataBind();
                repSubContractors.Visible = true;
                btnUnSubContract.Visible = true;
                lblSubContracted.Visible = false;

                Entities.Organisation subby = facOrganisation.GetForIdentityId(Job.SubContractors[0].ContractorIdentityId);

                m_subbyName = subby.OrganisationDisplayName;
                lnkUpdateSubcontractInformation.Visible = true;
            }
            else if (Job.SubContractors != null && Job.SubContractors.Count > 0 && !Job.SubContractors[0].SubContractWholeJob)
            {
                repJobReferences.DataBind();
                btnUnSubContract.Visible = false;
                repSubContractors.Visible = true;
                repSubContractors.DataSource = Job.SubContractors;
                repSubContractors.DataBind();
                repSubContractors.Visible = false;
                lblSubContracted.Visible = true;
                lblSubContracted.Text = "You can view which sub contractors are satisfying this run below.";
                lnkUpdateSubcontractInformation.Visible = true;
            }
            else
            {
                repSubContractors.Visible = false;
                btnUnSubContract.Visible = false;
                lblSubContracted.Visible = true;
                lblSubContracted.Text = "This run has not been sub contracted.";
                lnkUpdateSubcontractInformation.Visible = false;
            }
        }

        void btnUnSubContract_Click(object sender, EventArgs e)
        {
            Facade.IJobSubContractor facJobSubContractor = new Facade.Job();

            List<Entities.JobSubContractor> subContractors = facJobSubContractor.GetSubContractorForJobId(m_jobId);
            if (subContractors.Count == 1 && subContractors[0].SubContractWholeJob)
            {
                if (facJobSubContractor.Delete(m_jobId, ((Entities.CustomPrincipal)Page.User).UserName))
                {
                    Job.SubContractors = facJobSubContractor.GetSubContractorForJobId(Job.JobId);

                    // Refresh the Job Entity 
                    RefreshJobEntityCache();

                    // Load the Job
                    LoadJob();

                    //Re-bind the Trafficsheet
                    grdTrafficSheet.Rebind();
                }
            }
            else
            { } // we are removing a subcontractor from a leg and this cannot be done from here.
        }

        private void UncontractLeg(int instructionID)
        {
            Facade.IJobSubContractor facJobSubContractor = new Facade.Job();

            List<Entities.JobSubContractor> subContractors = facJobSubContractor.GetSubContractorForJobId(m_jobId);
            if (subContractors.Count == 1 && subContractors[0].SubContractWholeJob)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", "<script language=\"javascript\">alert(\"You cannot un-subcontract individual legs on Jobs which have been subcontacted as \\\"Whole Job\\\".\\n\\nPlease press the \\\"Bring Back in House\\\" button to remove ALL subcontractors from this Job.\");</script>");
            }
            else
            {
                Orchestrator.Facade.IJobSubContractor facJS = new Orchestrator.Facade.Job();
                facJS.UncontractInstruction(m_jobId, instructionID, this.Page.User.Identity.Name);
            }
        }

        private void chkPaid_CheckedChanged(object sender, EventArgs e)
        {
            PopulateSubContract(m_jobId);
        }

        private void chkReceived_CheckedChanged(object sender, EventArgs e)
        {
            PopulateSubContract(m_jobId);
        }

        #region Event Handlers

        protected void repLegs_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            ListItemType itemType = e.Item.ItemType;

            if (itemType == ListItemType.Item || itemType == ListItemType.AlternatingItem)
            {
                Entities.LegView leg = (Entities.LegView)e.Item.DataItem;

                XslTransform transformer = new XslTransform();
                transformer.Load(Server.MapPath(@"..\xsl\Point.xsl"));
                XmlUrlResolver resolver = new XmlUrlResolver();
                XPathNavigator navigator = m_jobXml.CreateNavigator();

                // Display the start address
                Label lblStartAddress = (Label)e.Item.FindControl("lblStartAddress");
                XsltArgumentList startArgs = new XsltArgumentList();
                startArgs.AddParam("LegPointId", "", leg.StartLegPoint.Instruction.PointID);
                StringWriter sw = new StringWriter();
                transformer.Transform(navigator, startArgs, sw, resolver);
                lblStartAddress.Text = sw.GetStringBuilder().ToString();

                // Display the end address
                Label lblEndAddress = (Label)e.Item.FindControl("lblEndAddress");
                XsltArgumentList endArgs = new XsltArgumentList();
                endArgs.AddParam("LegPointId", "", leg.EndLegPoint.Instruction.PointID);
                sw = new StringWriter();
                transformer.Transform(navigator, endArgs, sw, resolver);
                lblEndAddress.Text = sw.GetStringBuilder().ToString();
            }
        }

        #endregion

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);

            this.btnUnSubContract.Click += new EventHandler(btnUnSubContract_Click);
            this.lnkCancelJob.Click += new EventHandler(lnkCancelJob_Click);
            this.lnkCancelJobAndOrders.Click += new EventHandler(lnkCancelJobAndOrders_Click);
        }

        private void InitializeComponent()
        {
            this.Init += new EventHandler(jobDetails_Init);
        }

        #endregion

        #region Cookie Stuff

        private void LoadCookieDefaults()
        {

            try
            {
                _trafficSheetFilter = Utilities.GetFilterFromCookie(this.CookieSessionID, Request);
                if (_trafficSheetFilter == null)
                    GenerateCookie();
            }
            catch
            {
                // invalid or old values stored in the cookie simply regenerate.
                GenerateCookie();
            }

        }


        private void GenerateCookie()
        {
            _trafficSheetFilter = Utilities.GenerateCookie(this.CookieSessionID, this.Response, ((Entities.CustomPrincipal)Page.User).IdentityId);
        }

        #endregion

        #region Job Entity Cache Stuff

        private void AddJobEntityToCache(Entities.Job job)
        {
            if (Cache.Get("JobEntityForJobId" + m_jobId) != null)
            {
                Cache.Remove("JobEntityForJobId" + m_jobId);
            }

            Cache.Add("JobEntityForJobId" + m_jobId.ToString(),
                        job,
                        null,
                        Cache.NoAbsoluteExpiration,
                        TimeSpan.FromMinutes(5),
                        CacheItemPriority.Normal,
                        null);
        }

        private Entities.Job GetJobEntityFromCache()
        {
            Entities.Job job = (Entities.Job)Cache.Get("JobEntityForJobId" + m_jobId);

            if (job == null)
            {
                Facade.IJob facJob = new Facade.Job();
                Facade.IPCV facPCV = new Facade.PCV();
                Facade.IJobSubContractor facJobSubContractor = new Facade.Job();

                job = facJob.GetJob(m_jobId, true, true);
                job.Charge = ((Facade.IJobCharge)facJob).GetForJobId(m_jobId);
                job.Extras = facJob.GetExtras(m_jobId, true);
                job.PCVs = facPCV.GetForJobId(m_jobId);
                job.References = ((Facade.IJobReference)facJob).GetJobReferences(m_jobId);
                job.SubContractors = facJobSubContractor.GetSubContractorForJobId(m_jobId);

                AddJobEntityToCache(job);

                m_job = job;
            }

            return job;
        }

        private Entities.Job RefreshJobEntityCache()
        {
            Entities.Job job = null;

            Facade.IJob facJob = new Facade.Job();
            Facade.IPCV facPCV = new Facade.PCV();
            Facade.IJobSubContractor facJobSubContractor = new Facade.Job();

            job = facJob.GetJob(m_jobId, true, true);
            job.Charge = ((Facade.IJobCharge)facJob).GetForJobId(m_jobId);
            job.Extras = facJob.GetExtras(m_jobId, true);
            job.PCVs = facPCV.GetForJobId(m_jobId);
            job.References = ((Facade.IJobReference)facJob).GetJobReferences(m_jobId);
            job.SubContractors = facJobSubContractor.GetSubContractorForJobId(m_jobId);

            if (job != null)
            {
                AddJobEntityToCache(job);
                this.Job = job;
            }
            else
            {
                // System in inconsistent state, throw invalid operation ex.
                throw new InvalidOperationException();
            }

            return job;
        }

        #endregion

        public static Control FindControlRecursive(Control Root, string Id)
        {
            // Check first to see if the control has been found.
            // If we have found the control then return it back up the recursive call stack
            if (Root.ID == Id)
                return Root;

            // otherwise, keep looking.
            foreach (Control Ctl in Root.Controls)
            {
                Control FoundCtl = FindControlRecursive(Ctl, Id);
                if (FoundCtl != null)
                    return FoundCtl;
            }

            // If we get here then the control has not been found.
            return null;
        }

        void lnkCancelJobAndOrders_Click(object sender, EventArgs e)
        {
            bool cancellationResult = false;
            Facade.IJob facJob = new Facade.Job();

            try
            {
                cancellationResult = facJob.CancelJob(m_jobId, true, "Cancelled from Run screen by  " + User.Identity.Name, User.Identity.Name);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);

                // Record the operation as failed.
                cancellationResult = false;
            }

            if (cancellationResult)
            {
                // Cause the page to close automatically.
                Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", "<script language=\"javascript\">window.close();</script>");
            }
            else
            {
                // Tell the user that the job could not be cancelled.
                Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", "<script language=\"javascript\">alert(\"The job cancellation failed\");</script>");
            }
        }

        void lnkCancelJob_Click(object sender, EventArgs e)
        {
            bool cancellationResult = false;
            Facade.IJob facJob = new Facade.Job();

            try
            {
                cancellationResult = facJob.UpdateState(m_jobId, eJobState.Cancelled, User.Identity.Name);
            }
            catch (Exception ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);

                // Record the operation as failed.
                cancellationResult = false;
            }

            if (cancellationResult)
            {
                // Cause the page to close automatically.
                Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", "<script language=\"javascript\">window.close();</script>");
            }
            else
            {
                // Tell the user that the job could not be cancelled.
                Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", "<script language=\"javascript\">alert(\"The job cancellation failed\");</script>");
            }
        }

        private void grdMwfMessages_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<IMWF_InstructionRepository>(uow);
                var gridData = repo.GetForHENonOrderInstructionsGridForRun(this.Job.JobId);
                grdMwfMessages.DataSource = gridData;
            }
        }

        private void grdMwfMessages_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is Telerik.Web.UI.GridDataItem)
            {
                var item = (Telerik.Web.UI.GridDataItem)e.Item;
                DateTime? driveDateTime = (DateTime?)item.GetDataKeyValue("DriveDateTime");
                DateTime? completeDateTime = (DateTime?)item.GetDataKeyValue("CompleteDateTime");
                item.CssClass = completeDateTime.HasValue ? "LegstateCompleted" : "LegstateInProgress";
            }
        }

        #region Page methods
        [WebMethod()]
        public static bool RemoveCallIn(int jobID, int instructionID)
        {
            int instructionActualID = -1, pointID = -1;
            bool retVal = true;
            string userID = HttpContext.Current.User.Identity.Name;

            Facade.IInstructionActual facActual = new Facade.Instruction();
            Facade.IJob facJob = new Facade.Job();

            try
            {
                Entities.Job job = facJob.GetJob(jobID, true);
                if (job.Instructions.GetForInstructionId(instructionID).InstructionActuals.Count > 0)
                {
                    Entities.Instruction currentInstruction = job.Instructions.GetForInstructionId(instructionID);
                    instructionActualID = currentInstruction.InstructionActuals[0].InstructionActualId;
                    pointID = currentInstruction.PointID;
                    facActual.Delete(job, instructionID, pointID, instructionActualID, userID);
                }
            }
            catch (Exception ex)
            {
                WebUI.Global.UnhandledException(ex);
                throw;
            }

            return retVal;
        }

        [WebMethod()]
        public static bool RemoveCallInWithGoodsRefusals(int jobID, int instructionID)
        {
            int instructionActualID = -1, pointID = -1;
            bool retVal = true;
            string userID = HttpContext.Current.User.Identity.Name;

            Facade.IInstructionActual facActual = new Facade.Instruction();
            Facade.IJob facJob = new Facade.Job();
            Facade.IGoodsRefusal facGoodsRefusal = new Facade.GoodsRefusal();

            try
            {
                Entities.Job job = facJob.GetJob(jobID, true);
                if (job.Instructions.GetForInstructionId(instructionID).InstructionActuals.Count > 0)
                {
                    Entities.Instruction currentInstruction = job.Instructions.GetForInstructionId(instructionID);
                    instructionActualID = currentInstruction.InstructionActuals[0].InstructionActualId;
                    pointID = currentInstruction.PointID;
                    facActual.Delete(job, instructionID, pointID, instructionActualID, userID);
                    DataSet goodsRefusal = facGoodsRefusal.GetRefusalsForInstructionId(instructionID);

                    foreach (DataRow item in goodsRefusal.Tables[0].Rows)
                    {
                        facGoodsRefusal.Delete((int)item["RefusalId"], userID);
                    }
                }
            }
            catch (Exception ex)
            {
                WebUI.Global.UnhandledException(ex);
                throw;
            }

            return retVal;
        }


        [System.Web.Services.WebMethod]
        public static bool UnCommunicate(int jobID, int instructionID)
        {
            try
            {
                Facade.IDriverCommunication facDriverCommunication = new Facade.Resource();
                bool retVal = facDriverCommunication.RemoveDriverCommunication(jobID, instructionID);
                return retVal;
            }
            catch (Exception ex)
            {
                WebUI.Global.UnhandledException(ex);
                throw;
            }
        }

        [System.Web.Services.WebMethod]
        public static bool Communicate(int instructionID, string driver, int driverResourceId, int vehicleResourceId, int subContractorId, int jobID, string userId)
        {
            try
            {
                bool retVal = false;

                Entities.DriverCommunication communication = new Entities.DriverCommunication();
                communication.Comments = "Communicated via Run Details";
                communication.DriverCommunicationStatus = eDriverCommunicationStatus.Accepted;

                communication.DriverCommunicationType = eDriverCommunicationType.Manifest;
                communication.NumberUsed = "unknown";

                if (subContractorId > 0)
                {
                    Facade.IJobSubContractor facSubContractor = new Facade.Job();
                    communication.DriverCommunicationId = facSubContractor.CreateCommunication(jobID, instructionID, subContractorId, communication, userId);
                }
                else
                {
                    Facade.IDriverCommunication facDriverCommunication = new Facade.Resource();
                    communication.DriverCommunicationType = facDriverCommunication.GetDefaultCommunicationType(driverResourceId, vehicleResourceId);
                    communication.DriverCommunicationId = facDriverCommunication.Create(jobID, driverResourceId, communication, userId, instructionID);
                }

                retVal = communication.DriverCommunicationId > 0;
                return retVal;
            }
            catch (Exception ex)
            {
                WebUI.Global.UnhandledException(ex);
                throw;
            }
        }
        #endregion

        #region Manifest Stuff
        private void PopulateManifests()
        {
            // Load the Order Manifests for the resoure and this job.
            Facade.IManifest facManifest = new Facade.Manifest();
            DataSet dsManifests = facManifest.GetResourceManifestsForJob(m_jobId);
            if (dsManifests.Tables[0].Rows.Count > 0)
            {
                lvManifests.ItemDataBound += new EventHandler<ListViewItemEventArgs>(lvManifests_ItemDataBound);
                lvManifests.DataSource = dsManifests;
                lvManifests.DataBind();
            }
        }

        void lvManifests_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                DataRowView drv = ((ListViewDataItem)e.Item).DataItem as DataRowView;
                HyperLink view = e.Item.FindControl("lnkViewManifest") as HyperLink;
                LinkButton create = e.Item.FindControl("hidCreateManifestButton") as LinkButton;

                if (drv["ResourceManifestID"] == DBNull.Value)
                {
                    view.Visible = false;
                    e.Item.FindControl("lnkCreateManifest").Visible = true;
                    create.Attributes.Add("resourceid", drv["ResourceID"].ToString());
                    create.Attributes.Add("resourcename", drv["ResourceName"].ToString());
                    create.Attributes.Add("isSubContractor", Convert.ToBoolean(drv["IsSubContractor"]).ToString());
                    create.Click += new EventHandler(createManifest_Click);
                }
                else
                {
                    bool isSubContractor = Convert.ToBoolean(drv["IsSubContractor"]);

                    switch (isSubContractor)
                    {
                        case true:
                            view.NavigateUrl = "/manifest/viewsubbymanifest.aspx?rmId=" + drv["ResourceManifestID"].ToString() + "&excludeFirstLine=false&extraRows=0&usePlannedTimes=false&showFullAddress=true&useInstructionOrder=true&useScript=true";
                            break;
                        case false:
                            view.NavigateUrl = "/manifest/viewmanifest.aspx?rmId=" + drv["ResourceManifestID"].ToString() + "&excludeFirstLine=false&extraRows=0&usePlannedTimes=true&showFullAddress=true&useInstructionOrder=true&useScript=true";
                            break;
                    }

                    view.Target = "_blank";
                    view.Visible = true;
                    create.Visible = false;
                }
            }
        }

        void createManifest_Click(object sender, EventArgs e)
        {
            LinkButton button = sender as LinkButton;

            int runningJobOrder = 0;
            int resourceID = int.Parse(button.Attributes["resourceid"]);
            string resourceName = button.Attributes["resourcename"];
            bool isSubContractor = bool.Parse(button.Attributes["isSubContractor"]);
            int ResourceManifestId = -1;

            Facade.ResourceManifest facResourceManifest = new Orchestrator.Facade.ResourceManifest();
            Facade.Instruction facInstruction = new Orchestrator.Facade.Instruction();
            Entities.ResourceManifest resourceManifest = new Orchestrator.Entities.ResourceManifest();
            List<int> instructionIDs = null;

            if (isSubContractor)
                instructionIDs = facInstruction.GetInstructionIDsForSubContractor(m_jobId, resourceID, true);
            else
                instructionIDs = facInstruction.GetInstructionIDsForDriverResource(m_jobId, resourceID, true);

            // Create a new resource Manifest
            resourceManifest.ManifestDate = Entities.Utilities.ParseNullable<DateTime>(hidManifestDate.Value) ?? DateTime.Today;
            resourceManifest.Description = string.Format("{0} - {1:dd/MM/yy}", resourceName, resourceManifest.ManifestDate);
            resourceManifest.ResourceManifestJobs = new List<Entities.ResourceManifestJob>();

            if (isSubContractor)
                resourceManifest.SubcontractorId = resourceID;
            else
                resourceManifest.ResourceId = resourceID;

            // Add the resource manifest job to the collection.
            foreach (int instructionID in instructionIDs)
            {
                Entities.ResourceManifestJob rmj = new Orchestrator.Entities.ResourceManifestJob();
                rmj.JobId = m_jobId;
                rmj.JobOrder = runningJobOrder;
                rmj.InstructionId = instructionID;
                resourceManifest.ResourceManifestJobs.Add(rmj);
                runningJobOrder++;
            }

            // Create the new resource manifest.
            ResourceManifestId = facResourceManifest.CreateResourceManifest(resourceManifest, this.Page.User.Identity.Name);

            // Redirect to the drivers ResourceManifestJob edit page.
            if (isSubContractor)
                Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowSubbyManifest", string.Format("window.open('/manifest/viewsubbymanifest.aspx?rmID={0}&excludeFirstLine=0&extraRows=0&usePlannedTimes=true&useScript=true');", ResourceManifestId), true);
            else
                Page.ClientScript.RegisterStartupScript(this.GetType(), "ShowManifest", string.Format("window.open('/manifest/viewmanifest.aspx?rmID={0}&excludeFirstLine=0&extraRows=0&usePlannedTimes=true&showFullAddress=true&useScript=true');", ResourceManifestId), true);
        }
        #endregion

    }
}
