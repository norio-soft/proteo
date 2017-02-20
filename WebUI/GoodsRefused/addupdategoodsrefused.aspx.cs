using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Xml.Serialization;

using Orchestrator.WebUI.Controls;
using Orchestrator.WebUI.UserControls;
using Orchestrator.WebUI.Traffic.JobManagement;

namespace Orchestrator.WebUI.GoodsRefused
{
    /// <summary>
    /// Summary description for addupdategoodsrefused.
    /// </summary>
    public partial class addupdategoodsrefused : Orchestrator.Base.BasePage
    {
        #region Page Variables

        private string _guid = string.Empty;
        private bool _isUpdate = false;
        private bool _createPendingGoodsRefused = false;
        private int _instructionId = 0;
        private int _refusalId = 0;
        private int _orderId = 0;
        private int _jobId = 0;
        private int _redeliveryId = 0;

        private Entities.GoodsRefusal _goodsRefused;

        public List<Entities.GoodsRefusal> PendingRefusals
        {
            get
            {
                if (Session["PendingRefusals"] == null)
                    Session["PendingRefusals"] = new List<Entities.GoodsRefusal>();
                return (List<Entities.GoodsRefusal>)Session["PendingRefusals"];
            }
            set { Session["PendingRefusals"] = value; }
        }

        private const string vs_isWindowed = "vs_isWindowed";
        public bool IsWindowed
        {
            get { return ViewState[vs_isWindowed] == null ? false : (bool)ViewState[vs_isWindowed]; }
            set { ViewState[vs_isWindowed] = value; }
        }

        private const string vs_oldLocation = "vs_oldLocation";
        public eGoodsRefusedLocation OldLocation
        {
            get { return ViewState[vs_oldLocation] == null ? eGoodsRefusedLocation.OffTrailer : (eGoodsRefusedLocation)ViewState[vs_oldLocation]; }
            set { ViewState[vs_oldLocation] = value; }
        }

        #endregion

        #region Form Elements

        protected System.Web.UI.WebControls.Panel pnlPCVDeleted;
        protected System.Web.UI.WebControls.Label lblDeliveryPoint;
        protected System.Web.UI.WebControls.Label lblGoodsLocation;

        #endregion

        #region Page/Load/Init/Error

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage, eSystemPortion.AddEditJob, eSystemPortion.TakeCallIn);
            btnAdd.Enabled = Orchestrator.WebUI.Security.Authorise.CanAccess(eSystemPortion.AddEditJob, eSystemPortion.TakeCallIn);

            if (string.IsNullOrEmpty(Request.QueryString["showRP"]))
                this.trReturnPoint.Visible = false;
            else
                this.trReturnPoint.Visible = (1 == Convert.ToInt32(Request.QueryString["showRP"]));

            _createPendingGoodsRefused = Convert.ToBoolean(Request.QueryString["IsPending"]);
            _guid = Request.QueryString["Guid"];
            _instructionId = Convert.ToInt32(Request.QueryString["instructionId"]);
            _orderId = Convert.ToInt32(Request.QueryString["OrderId"]);
            _jobId = Convert.ToInt32(Request.QueryString["JobId"]);
            _refusalId = Convert.ToInt32(Request.QueryString["RefusalId"]);

            if (Request.QueryString["RedeliveryId"] != null && !String.IsNullOrEmpty(Request.QueryString["RedeliveryId"])) 
                _redeliveryId = Convert.ToInt32(Request.QueryString["RedeliveryId"]);

            // Launched as a Window
            IsWindowed = !string.IsNullOrEmpty(Request.QueryString["isWindowed"]) ? true : false;

            if (_instructionId > 0)
                _isUpdate = false;

            if (_refusalId > 0)
                _isUpdate = true;

            if (!IsPostBack)
            {
                PopulateStaticControls();

                if (_isUpdate || (_guid != string.Empty && _createPendingGoodsRefused))
                    LoadGoodsRefused();
                else
                    SetDefaultValues();
            }
        }

        private void SetDefaultValues()
        {
            // Note: we are adding a new good refusal - order id should not be null

            this.rdiTimeFrame.SelectedDate = DateTime.Now.AddDays(1);

            //DateTime date = (from redel in EF.DataContext.Current.RedeliverySet
            //                 where redel.RedeliveryId == _redeliveryId
            //                 select redel.OriginalBookedDateTime).FirstOrDefault();

            //this.lblDateDelivered.Text = date.ToString("dd/MM/yy hh:mm");

            //TODO: refactor these calls into a single call.
            this.lblLoadNo.Text = (from o in EF.DataContext.Current.OrderSet
                                   where o.OrderId == _orderId
                                   select o.CustomerOrderNumber).FirstOrDefault();

            this.lblDocket.Text = (from o in EF.DataContext.Current.OrderSet
                                   where o.OrderId == _orderId
                                   select o.DeliveryOrderNumber).FirstOrDefault();

            this.lblClient.Text = (from o in EF.DataContext.Current.OrderSet
                                   where o.OrderId == _orderId
                                   select o.Organisation.OrganisationName).FirstOrDefault();

            //this.lblJobId.Text = _jobId.ToString();
        }

        protected void addupdategoodsrefused_Init(object sender, EventArgs e)
        {
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            this.btnReturnToRefusedGoods.Click += new EventHandler(btnReturnToRefusedGoods_Click);
            this.cboRefusalType.SelectedIndexChanged += new EventHandler(cboRefusalType_SelectedIndexChanged);
            this.rptReturnRunLinks.ItemDataBound += new RepeaterItemEventHandler(rptReturnRunLinks_ItemDataBound);
        }

        void rptReturnRunLinks_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                HtmlAnchor hypReturnRun = (HtmlAnchor)e.Item.FindControl("hypReturnRun");
                hypReturnRun.HRef = string.Format("javascript:OpenRunDetails({0});", e.Item.DataItem);
                hypReturnRun.InnerText = e.Item.DataItem.ToString();
            }
        }

        #endregion

        #region Add/Update/Load/Populate Goods Refused

        // Ensure this method is called only once, from Page_Load
        private void LoadGoodsRefused()
        {
            //if (_createPendingGoodsRefused)
            //{
            //    _goodsRefused = null;
            //    _goodsRefused = PendingRefusals.Find(rr => rr.Guid == _guid);

            //    if (_goodsRefused == null)
            //    {
            //        _goodsRefused = new Orchestrator.Entities.GoodsRefusal();
            //        _goodsRefused.Guid = _guid = Guid.NewGuid().ToString();
            //        SetDefaultValues();
            //        return;
            //    }
            //    SetDefaultValues();
            //}
            //else
            //{
            if (ViewState["goodsrefused"] == null)
            {
                Facade.IGoodsRefusal facGoods = new Facade.GoodsRefusal();

                if (!_createPendingGoodsRefused)
                    _goodsRefused = facGoods.GetForRefusalId(_refusalId);

                ViewState["goodsrefused"] = _goodsRefused;
            }
            else
                _goodsRefused = (Entities.GoodsRefusal)ViewState["goodsrefused"];
            //}

            if (_goodsRefused != null)
            {
                _orderId = _goodsRefused.OriginalOrderId;

                hypRefusalRunId.HRef = string.Format("javascript:OpenRunDetails({0});", _goodsRefused.JobId.ToString());
                hypRefusalRunId.InnerText = _goodsRefused.JobId.ToString();

                var returnRunIds =
                    (from i in EF.DataContext.Current.InstructionSet
                     from cd in i.CollectDrops
                     where cd.Order.OrderId == _goodsRefused.NewOrderId
                     select i.Job.JobId).Distinct().ToList();

                rptReturnRunLinks.DataSource = returnRunIds;
                rptReturnRunLinks.DataBind();

                this.lblDocket.Text = _goodsRefused.Docket;

                lblDateDelivered.Text = _goodsRefused.DateDelivered.ToShortDateString();
                lblClient.Text = _goodsRefused.OrganisationName;

                txtPackSize.Text = _goodsRefused.PackSize.ToString();
                txtProductCode.Text = _goodsRefused.ProductCode;

                eGoodsRefusedType refusalType = (eGoodsRefusedType)Convert.ToInt32(_goodsRefused.RefusalType);

                ListItem selectedItem = cboRefusalType.Items.FindByValue(_goodsRefused.RefusalType.ToString());
                if (selectedItem != null)
                {
                    cboRefusalType.ClearSelection();
                    selectedItem.Selected = true;
                }

                rdiTimeFrame.SelectedDate = _goodsRefused.TimeFrame;

                txtProductName.Text = _goodsRefused.ProductName;
                rntQuantityRefused.Value = Convert.ToDouble(_goodsRefused.QuantityRefused);
                rntQuantityOrdered.Value = Convert.ToDouble(_goodsRefused.QuantityOrdered);
                txtRefusalNotes.Text = _goodsRefused.RefusalNotes;
                txtReceiptNumber.Text = _goodsRefused.RefusalReceiptNumber;
                cboGoodsStatus.ClearSelection();
                cboGoodsStatus.Items.FindByText(Utilities.UnCamelCase(Enum.GetName(typeof(eGoodsRefusedStatus), _goodsRefused.RefusalStatus))).Selected = true;
                cboLocation.ClearSelection();
                //cboLocation.Items.FindByValue(((int)_goodsRefused.RefusalLocation).ToString()).Selected = true;
                ListItem li = cboLocation.Items.FindByValue(((int)_goodsRefused.RefusalLocation).ToString());
                if (_goodsRefused.RefusalStatus == eGoodsRefusedStatus.Returned || _goodsRefused.RefusalStatus == eGoodsRefusedStatus.Dumped)
                {
                    cboLocation.Enabled = false;
                }

                if (_goodsRefused.StorePointId > 0)
                    ucStoreAtPoint.SelectedPoint = _goodsRefused.StorePoint;

                if (_goodsRefused.ReturnPointId > 0)
                    ucReturnToPoint.SelectedPoint = _goodsRefused.ReturnPoint;

                if (_goodsRefused.DeviationReasonId.HasValue)
                    cboDeviationReason.SelectedValue = _goodsRefused.DeviationReasonId.Value.ToString();

                lblAtStore.Visible = false;
                chkAtStore.Visible = false;

                if (li != null) // The currently location is either On or Off Trailer. Show the radio option to switch between these two options only.
                {
                    cboLocation.Items.FindByValue(li.Value).Selected = true;
                    lblLocation.Visible = false;
                }
                else // Show the current location.
                {
                    cboLocation.Visible = false;
                    rfvCurrentLocation.Enabled = false;

                    lblLocation.Visible = true;
                    lblLocation.Text = Utilities.UnCamelCase(_goodsRefused.RefusalLocation.ToString());

                    if(_goodsRefused.RefusalLocation != eGoodsRefusedLocation.AtStore) // We are not At Store (and not On or Off Trailer) so show the checkbox to set to At Store.
                    {
                        lblAtStore.Visible = true;
                        chkAtStore.Visible = true;
                    }
                }
            }

            btnAdd.Text = "Update";
        }

        private void PopulateGoodsRefused()
        {
            if (ViewState["goodsrefused"] == null)
                _goodsRefused = new Entities.GoodsRefusal();
            else
                _goodsRefused = (Entities.GoodsRefusal)ViewState["goodsrefused"];

            if (_goodsRefused != null)
            {
                _goodsRefused.JobId = Convert.ToInt32(hypRefusalRunId.InnerText);
                _goodsRefused.OrganisationName = lblClient.Text;
                _goodsRefused.RefusalId = _refusalId;
                _goodsRefused.OriginalOrderId = _goodsRefused.RefusalId > 0 ? _goodsRefused.OriginalOrderId : _orderId;
                _goodsRefused.Docket = lblDocket.Text;
                _goodsRefused.ProductName = txtProductName.Text;
                _goodsRefused.QuantityOrdered = Convert.ToInt32(rntQuantityOrdered.Value.Value.ToString());
                _goodsRefused.QuantityRefused = Convert.ToInt32(rntQuantityRefused.Value.Value.ToString());
                _goodsRefused.PackSize = txtPackSize.Text;
                _goodsRefused.ProductCode = txtProductCode.Text;
                _goodsRefused.RefusalType = (int)(eGoodsRefusedType)Enum.Parse(typeof(eGoodsRefusedType), (cboRefusalType.SelectedValue));
                _goodsRefused.TimeFrame = rdiTimeFrame.SelectedDate.HasValue ? rdiTimeFrame.SelectedDate.Value : DateTime.MinValue;
                _goodsRefused.RefusalNotes = txtRefusalNotes.Text;
                _goodsRefused.RefusalReceiptNumber = txtReceiptNumber.Text;
                _goodsRefused.RefusalStatus = (eGoodsRefusedStatus)Enum.Parse(typeof(eGoodsRefusedStatus), cboGoodsStatus.SelectedValue.Replace(" ", ""));

                if (cboLocation.Visible)
                    _goodsRefused.RefusalLocation = (eGoodsRefusedLocation)Convert.ToInt32(cboLocation.SelectedValue);

                if (chkAtStore.Visible && chkAtStore.Checked)
                    _goodsRefused.RefusalLocation = eGoodsRefusedLocation.AtStore;

                if (ucStoreAtPoint.SelectedPoint != null)
                    _goodsRefused.StorePoint = ucStoreAtPoint.SelectedPoint;

                if (ucReturnToPoint.SelectedPoint != null)
                    _goodsRefused.ReturnPoint = ucReturnToPoint.SelectedPoint;

                int deviationReasonId;
                int.TryParse(this.cboDeviationReason.SelectedValue, out deviationReasonId);

                if (deviationReasonId == 0)
                    _goodsRefused.DeviationReasonId = -1;
                else
                    _goodsRefused.DeviationReasonId = deviationReasonId;
            }
        }

        protected void LoadDeviationReasonCodes()
        {
            List<EF.DeviationReasonCode> deviationReasonCodes = EF.DataContext.Current.DeviationReasonCodes.ToList();

            cboDeviationReason.Items.Add(new ListItem(String.Empty, "-1"));

            foreach (EF.DeviationReasonCode code in deviationReasonCodes)
                cboDeviationReason.Items.Add(new ListItem(code.DeviationReason, code.DeviationReasonId.ToString(), true));

            cboDeviationReason.SelectedValue = "-1";
        }

        private bool AddGoodsRefused()
        {
            Facade.IGoodsRefusal facGoods = new Facade.GoodsRefusal();
            bool retVal = false;
            string userName = ((Entities.CustomPrincipal)Page.User).UserName;

            eJobType jobType = eJobType.Normal;
            using (Facade.IInstruction facInstruction = new Facade.Instruction())
            {
                Entities.Instruction instruction = facInstruction.GetInstruction(_instructionId);

                using (Facade.IJob facJob = new Facade.Job())
                {
                    Entities.Job job = facJob.GetJob(instruction.JobId);
                    jobType = job.JobType;
                }
            }

            retVal = facGoods.Create(_goodsRefused, jobType, _instructionId, userName);

            if (!retVal)
            {
                lblConfirmation.Text = "There was an error adding the Goods Refused, please try again.";
                lblConfirmation.Visible = true;
                lblConfirmation.ForeColor = Color.Red;
                retVal = false;
            }
            else
            {
                btnAdd.Text = "Update Goods Refused";
                this.ReturnValue = "Refresh_Redeliveries_And_Refusals";
                this.Close();
            }

            return retVal;
        }

        private bool UpdateGoodsRefused()
        {
            Facade.IGoodsRefusal facGoods = new Facade.GoodsRefusal();
            bool retVal = false;
            string userName = ((Entities.CustomPrincipal)Page.User).UserName;
            retVal = facGoods.Update(_goodsRefused, userName);

            //if (IsWindowed && _goodsRefused.RefusalLocation != OldLocation)
            //{
            //    DataAccess.Audit dacAudit = new Orchestrator.DataAccess.Audit();
            //    string details = string.Format("Goods refusalID : {0} on originating runID : {1} location updated from : {2} to : {3}", _goodsRefused.RefusalId.ToString(), _goodsRefused.JobId.ToString(), OldLocation.ToString(), _goodsRefused.RefusalLocation.ToString());
            //    dacAudit.AuditEvent((int)eAuditEvent.RefusedGoodsLocationUpdatedNoJobUsed, _goodsRefused.RefusalId, details, userName);
            //}

            return retVal;
        }

        #endregion

        #region Methods	& Functions

        private void btnAdd_Click(object sender, System.EventArgs e)
        {
            if (Page.IsValid)
            {
                bool retVal = false;

                PopulateGoodsRefused();

                if (_createPendingGoodsRefused)
                {
                    this.ReturnValue = "Refresh_Redeliveries_And_Refusals";
                    this.Close();
                }
                else
                {
                    if (_isUpdate)
                        retVal = UpdateGoodsRefused();
                    else
                        retVal = AddGoodsRefused();

                    lblConfirmation.Visible = true;

                    if (retVal == true)
                    {
                        // For what it's worth, show some text before the window closes.
                        if (_isUpdate)
                            lblConfirmation.Text = "The Goods Refused has been updated successfully.";
                        else
                            lblConfirmation.Text = "The Goods Refused has been added successfully.";

                        this.ReturnValue = "Refresh_Redeliveries_And_Refusals";
                        this.Close();
                    }
                }
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

        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Init += new System.EventHandler(this.addupdategoodsrefused_Init);

        }

        #endregion

        #region Populate Static Controls

        ///	<summary> 
        /// Populate Static Controls
        ///	</summary>
        private void PopulateStaticControls()
        {
            if (IsWindowed)
                btnReturnToRefusedGoods.Text = "Cancel";

            if (_isUpdate)
                this.btnAdd.Text = "Update Goods Refused";
            else
                this.btnAdd.Text = "Add Goods Refused";

            Facade.IGoodsRefusal facGoods = new Facade.GoodsRefusal();
            DataSet dsReasonType = facGoods.GetReasonTypes();
            cboRefusalType.DataSource = dsReasonType;
            cboRefusalType.DataTextField = "Description";
            cboRefusalType.DataValueField = "RefusalTypeId";
            cboRefusalType.DataBind();

            cboGoodsStatus.Items.Clear();
            cboGoodsStatus.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eGoodsRefusedStatus)));
            cboGoodsStatus.DataBind();

            cboLocation.DataTextField = "Description";
            cboLocation.DataValueField = "RefusalLocationId";

            cboLocation.DataSource =
                from locations in EF.DataContext.Current.GoodsRefusalLocationSet
                where locations.RefusalLocationId != 2
                        && locations.RefusalLocationId != 3
                        && locations.RefusalLocationId != 4
                select new { locations.RefusalLocationId, locations.Description };

            cboLocation.DataBind();

            this.LoadDeviationReasonCodes();
        }

        #endregion

        private void btnReturnToRefusedGoods_Click(object sender, EventArgs e)
        {
            if (IsWindowed)
            {
                this.ReturnValue = bool.TrueString;
                this.Close();
            }
            else
                Response.Redirect("ListGoodsRefused.aspx?mode=1&preSelectStates=1");
        }

        private void cboRefusalType_SelectedIndexChanged(object sender, EventArgs e)
        {
            eGoodsRefusedType goodsRefusalType = (eGoodsRefusedType)Enum.Parse(typeof(eGoodsRefusedType), cboRefusalType.SelectedValue);
            if (goodsRefusalType == eGoodsRefusedType.Shorts)
            {
                lblReturnTypeHeading.Text = "Shorts";
                lblQuantityRefused.Text = "Quantity Short";
            }
            if (goodsRefusalType == eGoodsRefusedType.OverAndAccepted
                ||
                goodsRefusalType == eGoodsRefusedType.OverAndReturned
                )
            {
                lblReturnTypeHeading.Text = "Overs";
                lblQuantityRefused.Text = "Quantity Over";
            }
        }
    }
}
