using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Orchestrator.WebUI.Traffic.JobManagement.DriverCallIn
{
    /// <summary>
    /// Summary description for Shortage..
    /// </summary>
    public partial class Shortage : Orchestrator.Base.BasePage
    {
        private const string C_JOB_VS = "C_JOB_VS";
        private const string C_POINT_ID_VS = "C_POINT_ID_VS";

        #region Form Elements

        protected UserControls.BusinessRuleInfringementDisplay infringementDisplay;



        #endregion

        #region Page Variables

        #region Protected

        protected Entities.Job m_job = null;
        protected int m_jobId = 0;	// The id of the job we are currently manipulating.
        protected int m_pointId = 0;	// The id of the point visited in the job we are currently manipulating.

        #endregion

        #region Private

        private int m_instructionId = 0;		// The id of the instruction that is being called-in.	

        private bool m_canEdit = false;

        private DataSet m_shortageReasons;

        #endregion

        #endregion

        #region Page Load/Init

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage, eSystemPortion.TakeCallIn);
            m_canEdit = Orchestrator.WebUI.Security.Authorise.CanAccess(eSystemPortion.TakeCallIn);
            btnStoreShortage.Enabled = m_canEdit;

            m_jobId = Convert.ToInt32(Request.QueryString["jobId"]);
            m_instructionId = Convert.ToInt32(Request.QueryString["instructionId"]);
            if (m_instructionId == 0)
                Response.Redirect("tabProgress.aspx?wiz=true&jobId=" + m_jobId.ToString() + "&csid=" + this.CookieSessionID);
            else
            {
                Facade.IInstructionActual facInstructionActual = new Facade.Instruction();
                DataSet dsInstructionActual = facInstructionActual.GetForInstructionId(m_instructionId);

                if (dsInstructionActual.Tables[0].Rows.Count == 0)
                    Response.Redirect("tabProgress.aspx?wiz=true&jobId=" + m_jobId.ToString() + "&csid=" + this.CookieSessionID);
            }

            try
            {
                hidShortageId.Value = Convert.ToInt32(Request.QueryString["refusalId"]).ToString();
            }
            catch { }
            ConfigureReasons();

            // Set please wait button clicks!
            btnStoreShortage.Attributes.Add("onMouseUp", @"javascript:HideTop(true);");

            if (!IsPostBack)
            {
                LoadJob();
                using (Facade.IInstruction facInstruction = new Facade.Instruction())
                    cboShortageDocket.DataSource = facInstruction.GetDocketsForInstructionId(m_instructionId);

                cboShortageDocket.DataTextField = "DocketNumberDisplay";
                cboShortageDocket.DataValueField = "DocketNumber";
                cboShortageDocket.DataBind();

                Facade.IOrder facOrder = new Facade.Order();
                cboOrder.DataSource = facOrder.GetOrdersForInstructionID(m_instructionId);
                cboOrder.DataBind();

                if (hidShortageId.Value != "0")
                    BindShortage(Convert.ToInt32(hidShortageId.Value));
                else
                    ClearShortages();
            }
            else
            {
                if (m_instructionId == 0)
                    m_instructionId = Convert.ToInt32(hidInstructionId.Value);
                m_job = (Entities.Job)ViewState[C_JOB_VS];
            }

            if (((Entities.CustomPrincipal)this.Page.User).IsInRole(((int)eUserRole.SubConPortal).ToString()))
                this.buttonBar.Visible = false;

            hidInstructionId.Value = m_instructionId.ToString();
        }

        private void Shortage_Init(object sender, EventArgs e)
        {
            btnStoreShortage.Click += new EventHandler(btnStoreShortage_Click);
            btnClearShortage.Click += new EventHandler(btnClearShortage_Click);

            cfvShortageQtyDespatched.ServerValidate += new ServerValidateEventHandler(cfvMustBeWholeNumber_Validate);
            cfvShortageQty.ServerValidate += new ServerValidateEventHandler(cfvMustBeWholeNumber_Validate);
            this.cboOrder.ItemDataBound += new Telerik.Web.UI.RadComboBoxItemEventHandler(cboOrder_ItemDataBound);
        }

        void cboOrder_ItemDataBound(object sender, Telerik.Web.UI.RadComboBoxItemEventArgs e)
        {
            e.Item.Text = ((DataRowView)e.Item.DataItem)["OrderID"].ToString();
        }

        #endregion

        /// <summary>
        /// Binds the shortage the user has selected to edit to the shortage.
        /// </summary>
        /// <param name="refusalId">The unique id of the goods refusal to edit.</param>
        private void BindShortage(int refusalId)
        {
            Entities.GoodsRefusal selectedGoodsRefusal = null;

            using (Facade.IGoodsRefusal facGoodsRefusal = new Facade.GoodsRefusal())
                selectedGoodsRefusal = facGoodsRefusal.GetForRefusalId(refusalId);

            if (selectedGoodsRefusal != null)
            {
                // Bind the information to the panel.
                ClearShortages();
                hidShortageId.Value = refusalId.ToString();

                if (selectedGoodsRefusal.Docket != string.Empty)
                {
                    cboShortageDocket.ClearSelection();
                    cboShortageDocket.Items.FindByValue(selectedGoodsRefusal.Docket.ToString()).Selected = true;
                }

                txtShortageProductName.Text = selectedGoodsRefusal.ProductName;
                txtShortageProductCode.Text = selectedGoodsRefusal.ProductCode;
                txtShortagePackSize.Text = selectedGoodsRefusal.PackSize;
                txtShortageQtyDespatched.Text = selectedGoodsRefusal.QuantityOrdered.ToString();
                txtShortageQty.Text = selectedGoodsRefusal.QuantityRefused.ToString();

                cboShortageReason.ClearSelection();
                eGoodsRefusedType refusalType = (eGoodsRefusedType)selectedGoodsRefusal.RefusalType;
                cboShortageReason.Items.FindByText(Utilities.UnCamelCase(Enum.GetName(typeof(eGoodsRefusedType), refusalType))).Selected = true;
                txtShortageNotes.Text = selectedGoodsRefusal.RefusalNotes;

                if ((selectedGoodsRefusal.RefusalStatus != eGoodsRefusedStatus.Outstanding && selectedGoodsRefusal.RefusalType != (int)eGoodsRefusedType.Shorts && selectedGoodsRefusal.RefusalType != (int)eGoodsRefusedType.OverAndAccepted) && m_job.JobType != eJobType.Return)
                    btnStoreShortage.Enabled = false;
                else
                    btnStoreShortage.Enabled = true;
            }
        }

        /// <summary>
        /// Resets the shortage good controls on the page.
        /// </summary>
        private void ClearShortages()
        {
            // Bind the Drop Down Lists
            cboShortageReason.Items.Clear();
            cboShortageReason.DataSource = m_shortageReasons;
            cboShortageReason.DataBind();

            // Reset the Text Boxes
            hidShortageId.Value = "0";
            txtShortageProductName.Text = String.Empty;
            txtShortageProductCode.Text = String.Empty;
            txtShortagePackSize.Text = String.Empty;
            txtShortageQtyDespatched.Text = String.Empty;
            txtShortageQty.Text = String.Empty;
            txtShortageNotes.Text = String.Empty;

            btnStoreShortage.Text = "Record Shortage";
        }

        /// <summary>
        /// Configures the reasons so that only valid shortage reasons appear in the shortage dropdown, whilst other reasons
        /// appear in the refusal drop down.
        /// </summary>
        private void ConfigureReasons()
        {
            using (Facade.IGoodsRefusal facGoods = new Facade.GoodsRefusal())
                m_shortageReasons = facGoods.GetReasonTypes();

            for (int rowIndex = 0; rowIndex < m_shortageReasons.Tables[0].Rows.Count; rowIndex++)
            {
                eGoodsRefusedType type = (eGoodsRefusedType)(int)m_shortageReasons.Tables[0].Rows[rowIndex]["RefusalTypeId"];

                switch (type)
                {
                    case eGoodsRefusedType.Shorts:
                        break;
                    case eGoodsRefusedType.OverAndAccepted:
                        break;
                    default:
                        // Remove this option.
                        m_shortageReasons.Tables[0].Rows.RemoveAt(rowIndex);
                        rowIndex--;
                        break;
                }
            }
        }

        /// <summary>
        /// Creates the goods refusal record against the current instruction and point.
        /// </summary>
        /// <param name="refusal">The GoodsRefusal object.</param>
        /// <param name="userId">The user recording the refusal.</param>
        /// <returns>Success indicator.</returns>
        private bool CreateRefusal(Entities.GoodsRefusal refusal, string userId)
        {
            bool success = false;
            m_instructionId = Convert.ToInt32(hidInstructionId.Value);

            using (Facade.IGoodsRefusal facGoodsRefusal = new Facade.GoodsRefusal())
                success = facGoodsRefusal.Create(refusal, m_job.JobType, m_instructionId, userId);

            return success;
        }

        /// <summary>
        /// Retrieve the job the user is working on and place it in ViewState.
        /// </summary>
        private void LoadJob()
        {
            using (Facade.IJob facJob = new Facade.Job())
            {
                m_job = facJob.GetJob(m_jobId, true);

                if (m_job.JobState == eJobState.Cancelled)
                    Response.Redirect("../../../Job/job.aspx?wiz=true&jobId=" + m_job.JobId.ToString() + "&csid=" + this.CookieSessionID);
            }

            ViewState[C_JOB_VS] = m_job;
        }

        /// <summary>
        /// Populates the details about the shortage goods.
        /// </summary>
        /// <returns>The populated object.</returns>
        private Entities.GoodsRefusal PopulateShortage()
        {
            Entities.GoodsRefusal goodsRefusal = new Entities.GoodsRefusal();

            goodsRefusal.OriginalOrderId = int.Parse(cboOrder.SelectedValue);
            goodsRefusal.Docket = cboOrder.SelectedValue;

            // Populate the goods refusal object.
            goodsRefusal.RefusalId = Convert.ToInt32(hidShortageId.Value);
            goodsRefusal.RefusalReceiptNumber = string.Empty;

            goodsRefusal.ReturnPoint = null;
            goodsRefusal.StorePoint = null;

            eGoodsRefusedType refusedType = (eGoodsRefusedType)Enum.Parse(typeof(eGoodsRefusedType), cboShortageReason.SelectedValue.Replace(" ", ""));

            goodsRefusal.ProductCode = txtShortageProductCode.Text;
            goodsRefusal.PackSize = txtShortagePackSize.Text;
            goodsRefusal.ProductName = txtShortageProductName.Text;
            goodsRefusal.QuantityOrdered = (txtShortageQtyDespatched.Text == String.Empty) ? 0 : Convert.ToInt32(txtShortageQtyDespatched.Text);
            goodsRefusal.QuantityRefused = (txtShortageQty.Text == String.Empty) ? 0 : Convert.ToInt32(txtShortageQty.Text);
            goodsRefusal.RefusalNotes = txtShortageNotes.Text;
            goodsRefusal.TimeFrame = DateTime.Now.AddDays(14);
            goodsRefusal.InstructionId = Convert.ToInt32(hidInstructionId.Value);

            goodsRefusal.RefusalType = (int)refusedType;
            goodsRefusal.RefusalStatus = eGoodsRefusedStatus.NotApplicable;

            return goodsRefusal;
        }

        /// <summary>
        /// Updates the goods refusal record.
        /// </summary>
        /// <param name="refusal">The GoodsRefusal object.</param>
        /// <param name="userId">The user updating the refusal.</param>
        /// <returns>Success indicator.</returns>
        private bool UpdateRefusal(Entities.GoodsRefusal refusal, string userId)
        {
            bool success = false;

            using (Facade.IGoodsRefusal facGoodsRefusal = new Facade.GoodsRefusal())
                success = facGoodsRefusal.Update(refusal, userId);

            return success;
        }

        #region Button Events

        private void btnClearShortage_Click(object sender, EventArgs e)
        {
            Response.Redirect("tabReturns.aspx?wiz=true&jobId=" + m_jobId.ToString() + "&instructionId=" + m_instructionId.ToString() + "&csid=" + this.CookieSessionID);
        }

        private void btnStoreShortage_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                Entities.GoodsRefusal refusal = PopulateShortage();

                bool success = false;
                string userId = ((Entities.CustomPrincipal)Page.User).UserName;

                if (refusal.RefusalId == 0)
                    success = CreateRefusal(refusal, userId);
                else
                    success = UpdateRefusal(refusal, userId);

                if (success)
                    Response.Redirect("tabReturns.aspx?wiz=true&jobId=" + m_jobId.ToString() + "&instructionId=" + m_instructionId.ToString() + "&csid=" + this.CookieSessionID);
            }
        }

        #endregion

        #region Validation Events

        protected void cfvMustBeWholeNumber_Validate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(args.Value, 0, true);
        }

        protected void cfvMustBeNumber_Validate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(args.Value, 0, false);
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
            this.Init += new EventHandler(Shortage_Init);
        }
        #endregion
    }
}
