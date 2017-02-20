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
using Telerik.Web.UI;
using System.Collections.Generic;
using System.Linq;


namespace Orchestrator.WebUI.Traffic.JobManagement.DriverCallIn
{
    /// <summary>
    /// Summary description for Refusal.
    /// </summary>
    public partial class Refusal : Orchestrator.Base.BasePage
    {
        private const string C_JOB_VS = "C_JOB_VS";
        private const string C_POINT_ID_VS = "C_POINT_ID_VS";

        protected UserControls.BusinessRuleInfringementDisplay infringementDisplay;
        protected Panel pnlGoods;
        protected Label lblGoodsCount;
        protected DataGrid dgGoods;
        protected Entities.Job m_job = null;
        protected int m_jobId = 0;	// The id of the job we are currently manipulating.
        protected int m_pointId = 0;	// The id of the point visited in the job we are currently manipulating.
        protected int m_GoodsStoreIdentityId = 0;			// The id of the organisation we are going to store the goods at.
        protected int m_GoodsStorePointId = 0;			// The id of the point we are going to store the goods at.
        protected int m_GoodsReturnIdentityId = 0;			// The id of the organisation we are going to return the goods to.
        protected int m_GoodsReturnPointId = 0;			// The id of the point we are going to return the goods to.

        private int m_instructionId = 0;		// The id of the instruction that is being called-in.	
        private bool m_canEdit = false;
        private DataSet m_refusalReasons;

        #region Page Load/Init

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage, eSystemPortion.TakeCallIn);
            m_canEdit = Orchestrator.WebUI.Security.Authorise.CanAccess(eSystemPortion.TakeCallIn);
            btnStoreGoods.Enabled = m_canEdit;
            btnStoreGoodsAndReset.Enabled = m_canEdit;

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
                hidRefusalId.Value = Convert.ToInt32(Request.QueryString["refusalId"]).ToString();
            }
            catch { }
            ConfigureReasons();

            // Set please wait button clicks!
            btnStoreGoods.Attributes.Add("onMouseUp", @"javascript:HideTop(true);");
            btnStoreGoodsAndReset.Attributes.Add("onMouseUp", @"javascript:HideTop(true);");

            if (!IsPostBack)
            {
                LoadJob();
                //using (Facade.IInstruction facInstruction = new Facade.Instruction())
                    //cboDocket.DataSource = facInstruction.GetDocketsForInstructionId(m_instructionId);
                
                
                Facade.IOrder facOrder = new Facade.Order();
                cboOrder.DataSource = facOrder.GetOrdersForInstructionID(m_instructionId);
                cboOrder.DataBind();
                    

                //cboDocket.DataValueField = "DocketNumber";
                //cboDocket.DataTextField = "DocketNumberDisplay";

                //cboDocket.DataBind();

                if (hidRefusalId.Value != "0")
                    BindGoodsRefusal(Convert.ToInt32(hidRefusalId.Value));
                else
                    ClearGoods();
            }
            else
            {
                if (m_instructionId == 0)
                    m_instructionId = Convert.ToInt32(hidInstructionId.Value);
                m_job = (Entities.Job)ViewState[C_JOB_VS];
            }

            if(((Entities.CustomPrincipal)this.Page.User).IsInRole(((int)eUserRole.SubConPortal).ToString()))
                this.buttonBar.Visible = false;

            hidInstructionId.Value = m_instructionId.ToString();
        }

        private void Refusal_Init(object sender, EventArgs e)
        {
            cfvGoodsStatus.ServerValidate += new ServerValidateEventHandler(cfvGoodsStatus_ServerValidate);

            btnStoreGoods.Click += new EventHandler(btnStoreGoods_Click);
            btnStoreGoodsAndReset.Click += new EventHandler(btnStoreGoodsAndReset_Click);
            btnCancelGoods.Click += new EventHandler(btnCancelGoods_Click);
            this.cboOrder.ItemDataBound += new RadComboBoxItemEventHandler(cboOrder_ItemDataBound);
            //this.cboOrder.DataBound += new EventHandler(cboOrder_DataBound);
            cfvQuantityReturned.ServerValidate += new ServerValidateEventHandler(cfvMustBeWholeNumber_Validate);
            cfvQuantityOrdered.ServerValidate += new ServerValidateEventHandler(cfvMustBeWholeNumber_Validate);
        }

        void cboOrder_ItemDataBound(object sender, RadComboBoxItemEventArgs e)
        {
            e.Item.Text = ((DataRowView)e.Item.DataItem)["OrderID"].ToString();
        }

        
        void cfvGoodsStatus_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;

            eGoodsRefusedStatus status = (eGoodsRefusedStatus)Enum.Parse(typeof(eGoodsRefusedStatus), cboGoodsStatus.SelectedItem.Text.Replace(" ", ""));
            if (status == eGoodsRefusedStatus.Dumped || status == eGoodsRefusedStatus.Outstanding || status == eGoodsRefusedStatus.NotApplicable)
                args.IsValid = true;
        }

        #endregion

        /// <summary>
        /// Binds the goods refusal the user has selected to edit to the goods refusal panel.
        /// </summary>
        /// <param name="refusalId">The unique id of the goods refusal to edit.</param>
        private void BindGoodsRefusal(int refusalId)
        {
            Entities.GoodsRefusal selectedGoodsRefusal = null;

            using (Facade.IGoodsRefusal facGoodsRefusal = new Facade.GoodsRefusal())
                selectedGoodsRefusal = facGoodsRefusal.GetForRefusalId(refusalId);

            if (selectedGoodsRefusal != null)
            {
                // Bind the goods information to the panel.
                ClearGoods();
                //cboGoodsType.ClearSelection();
                //cboGoodsType.Items.FindByValue(Utilities.UnCamelCase(Enum.GetName(typeof(eReturnType), (eReturnType)selectedGoodsRefusal.ReturnType))).Selected = true;
                hidRefusalId.Value = refusalId.ToString();

                //if (selectedGoodsRefusal.Docket != string.Empty)
                //{
                    //cboDocket.ClearSelection();
                    //ListItem docketItem = cboDocket.Items.FindByValue(selectedGoodsRefusal.Docket.ToString());

                    //if (docketItem != null)
                        //docketItem.Selected = true;
                //}
                
                if (selectedGoodsRefusal.NewOrderId.HasValue && selectedGoodsRefusal.NewOrderId.Value > 0)
                    this.cboOrder.Enabled = false;
                else
                    this.cboOrder.Enabled = true;

                this.cboOrder.ClearSelection();
                this.cboOrder.Items.FindItemByValue(selectedGoodsRefusal.OriginalOrderId.ToString()).Selected = true;

                txtProductName.Text = selectedGoodsRefusal.ProductName;
                txtProductCode.Text = selectedGoodsRefusal.ProductCode;
                txtReceiptNumber.Text = selectedGoodsRefusal.RefusalReceiptNumber;
                txtPackSize.Text = selectedGoodsRefusal.PackSize;
                txtQuantityOrdered.Text = selectedGoodsRefusal.QuantityOrdered.ToString();
                txtQuantityReturned.Text = selectedGoodsRefusal.QuantityRefused.ToString();

                rntNoPallets.Value = selectedGoodsRefusal.NoPallets;
                rntNoPalletSpaces.Value = Convert.ToDouble(selectedGoodsRefusal.NoPalletSpaces);

                cboGoodsReasonRefused.ClearSelection();
                eGoodsRefusedType refusalType = (eGoodsRefusedType)selectedGoodsRefusal.RefusalType;
                cboGoodsReasonRefused.Items.FindByText(Utilities.UnCamelCase(Enum.GetName(typeof(eGoodsRefusedType), refusalType))).Selected = true;
                cboGoodsStatus.ClearSelection();
                cboGoodsStatus.Items.FindByText(Utilities.UnCamelCase(Enum.GetName(typeof(eGoodsRefusedStatus), selectedGoodsRefusal.RefusalStatus))).Selected = true;
                cboLocation.ClearSelection();
                
                ListItem selectedItem = cboLocation.Items.FindByText(Utilities.UnCamelCase(Enum.GetName(typeof(eGoodsRefusedLocation), selectedGoodsRefusal.RefusalLocation)));
                if (selectedItem == null)
                {
                    string value = Utilities.UnCamelCase(Enum.GetName(typeof(eGoodsRefusedLocation), selectedGoodsRefusal.RefusalLocation));
                    ListItem newItem = new ListItem(value, value);
                    cboLocation.Items.Add(newItem);
                    newItem.Selected = true;
                }
                
                rdiReturnDate.SelectedDate = selectedGoodsRefusal.TimeFrame.Date;
                txtGoodsNotes.Text = selectedGoodsRefusal.RefusalNotes;
                if (selectedGoodsRefusal.ReturnPoint != null)
                {
                    m_GoodsReturnIdentityId = selectedGoodsRefusal.ReturnPoint.IdentityId;
                    m_GoodsReturnPointId = selectedGoodsRefusal.ReturnPoint.PointId;
                    ucReturnedToPoint.SelectedPoint = selectedGoodsRefusal.ReturnPoint;
                }
                if (selectedGoodsRefusal.StorePoint != null)
                {
                    m_GoodsStoreIdentityId = selectedGoodsRefusal.StorePoint.IdentityId;
                    m_GoodsStorePointId = selectedGoodsRefusal.StorePoint.PointId;
                    ucStoredAtPoint.SelectedPoint = selectedGoodsRefusal.StorePoint;
                }

                if (selectedGoodsRefusal.DeviationReasonId.HasValue)
                    cboDeviationReason.SelectedValue = selectedGoodsRefusal.DeviationReasonId.Value.ToString();

                if ((selectedGoodsRefusal.RefusalStatus != eGoodsRefusedStatus.Outstanding && selectedGoodsRefusal.RefusalType != (int)eGoodsRefusedType.Shorts && selectedGoodsRefusal.RefusalType != (int)eGoodsRefusedType.OverAndAccepted) && m_job.JobType != eJobType.Return)
                    btnStoreGoods.Enabled = false;
                else
                    btnStoreGoods.Enabled = true;
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

        /// <summary>
        /// Resets the goods entry controls on the page.
        /// </summary>
        private void ClearGoods()
        {
            // Bind the Drop Down Lists
            //cboGoodsType.Items.Clear();
            //cboGoodsType.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eReturnType)));
            //cboGoodsType.DataBind();
            //cboGoodsType.SelectedIndex = 1;
            cboGoodsReasonRefused.Items.Clear();
            cboGoodsReasonRefused.DataSource = m_refusalReasons;
            cboGoodsReasonRefused.DataBind();
            cboGoodsStatus.Items.Clear();
            cboGoodsStatus.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eGoodsRefusedStatus)));
            cboGoodsStatus.DataBind();
            cboLocation.Items.Clear();
            cboLocation.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eGoodsRefusedLocation)));
            cboLocation.DataBind();

            this.LoadDeviationReasonCodes();
            
            cboLocation.Items.Remove("Off Trailer");
            cboLocation.Items.Remove("At Store");

            (cboOrder.Header.FindControl("lblHeader") as Label).Text = Orchestrator.Globals.Configuration.SystemDocketNumberText;

            // Reset the Text Boxes
            hidRefusalId.Value = "0";
            txtProductName.Text = String.Empty;
            txtProductCode.Text = String.Empty;
            txtReceiptNumber.Text = String.Empty;
            txtPackSize.Text = String.Empty;
            txtQuantityOrdered.Text = String.Empty;
            txtQuantityReturned.Text = String.Empty;
            rdiReturnDate.SelectedDate = DateTime.Now.AddDays(Orchestrator.Globals.Configuration.RefusalNoOfDaysToReturnGoods);
            txtGoodsNotes.Text = String.Empty;

            rntNoPallets.Value = 0;
            rntNoPalletSpaces.Value = 0;

            // If there is another piece of refused goods on this instruction we should reuse the store and return point.
            //int goodsRefusalId = 0;

            //using (Facade.IGoodsRefusal facGoodsRefusal = new Facade.GoodsRefusal())
            //{
            //    DataSet dsGoods = facGoodsRefusal.GetRefusalsForInstructionId(m_instructionId);

            //    foreach (DataRow row in dsGoods.Tables[0].Rows)
            //    {
            //        eGoodsRefusedType refusalType = (eGoodsRefusedType)Enum.Parse(typeof(eGoodsRefusedType), ((string)row["RefusalType"]).Replace(" ", ""));
            //        if (refusalType != eGoodsRefusedType.Shorts && refusalType != eGoodsRefusedType.OverAndAccepted)
            //        {
            //            goodsRefusalId = (int)row["RefusalId"];
            //            break;
            //        }
            //    }

            //    if (goodsRefusalId > 0)
            //    {
            //        Entities.GoodsRefusal refusal = facGoodsRefusal.GetForRefusalId(goodsRefusalId);

            //        if (refusal.StorePoint != null)
            //        {
            //            m_GoodsStoreIdentityId = refusal.StorePoint.IdentityId; ;
            //            m_GoodsStorePointId = refusal.StorePoint.PointId;
            //        }

            //        if (refusal.ReturnPoint != null)
            //        {
            //            m_GoodsReturnIdentityId = refusal.ReturnPoint.IdentityId;
            //            m_GoodsReturnPointId = refusal.ReturnPoint.PointId;
            //        }
            //    }
            //}

            btnStoreGoods.Text = "Record Goods";
            btnStoreGoodsAndReset.Text = "Record Goods & Reset";
        }

        /// <summary>
        /// Configures the reasons so that only valid shortage reasons appear in the shortage dropdown, whilst other reasons
        /// appear in the refusal drop down.
        /// </summary>
        private void ConfigureReasons()
        {
            using (Facade.IGoodsRefusal facGoods = new Facade.GoodsRefusal())
                m_refusalReasons = facGoods.GetReasonTypes();

            for (int rowIndex = 0; rowIndex < m_refusalReasons.Tables[0].Rows.Count; rowIndex++)
            {
                eGoodsRefusedType type = (eGoodsRefusedType)(int)m_refusalReasons.Tables[0].Rows[rowIndex]["RefusalTypeId"];

                switch (type)
                {
                    case eGoodsRefusedType.Shorts:
                        // Remove this option.
                        m_refusalReasons.Tables[0].Rows.RemoveAt(rowIndex);
                        rowIndex--;
                        break;
                    case eGoodsRefusedType.OverAndAccepted:
                        // Remove this option.
                        m_refusalReasons.Tables[0].Rows.RemoveAt(rowIndex);
                        rowIndex--;
                        break;
                    default:
                        break;
                }
            }
        }

        #region Button Events

        /// <summary>
        /// Cancels the changes to the goods refusal object.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancelGoods_Click(object sender, EventArgs e)
        {
            Response.Redirect("tabReturns.aspx?wiz=true&jobId=" + m_jobId.ToString() + "&instructionId=" + m_instructionId.ToString() + "&csid=" + this.CookieSessionID);
        }

        /// <summary>
        /// Record the refused/returned goods details.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStoreGoods_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                Entities.GoodsRefusal refusal = PopulateGoodsRefusal();

                // Only continue if the point requirements are met for this goods refusal.
                // The only time this will evaluate to false is if the user has tried to create a new point for either
                // the StorePoint or ReturnPoint and it has failed - the infringements will be displayed in the user control.
                if (refusal.StorePoint != null && (ucReturnedToPoint.PointID > 0 || refusal.ReturnPoint != null))
                {
                    bool success = false;
                    string userId = ((Entities.CustomPrincipal)Page.User).UserName;

                    if (refusal.RefusalId == 0)
                        success = CreateRefusal(refusal, userId);
                    else
                        success = UpdateRefusal(refusal, userId);

                    if (success)
                    {
                        Response.Redirect("tabReturns.aspx?wiz=true&jobId=" + m_jobId.ToString() + "&instructionId=" + m_instructionId.ToString() + "&csid=" + this.CookieSessionID);
                    }
                }
            }
        }

        private void btnStoreGoodsAndReset_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                Entities.GoodsRefusal refusal = PopulateGoodsRefusal();

                // Only continue if the point requirements are met for this goods refusal.
                // The only time this will evaluate to false is if the user has tried to create a new point for either
                // the StorePoint or ReturnPoint and it has failed - the infringements will be displayed in the user control.
                if (refusal.StorePoint != null && (ucReturnedToPoint.PointID > 0 || refusal.ReturnPoint != null))
                {
                    bool success = false;
                    string userId = ((Entities.CustomPrincipal)Page.User).UserName;

                    if (refusal.RefusalId == 0)
                        success = CreateRefusal(refusal, userId);
                    else
                        success = UpdateRefusal(refusal, userId);

                    if (success)
                    {
                        this.MessageBox("Your goods have been recorded.", true);
                    }
                }
            }

            // Reset all the fields except for the store point, return point and order
            ClearGoods();
        }

        #endregion

        #region Entity Population Events

        /// <summary>
        /// Creates a new point based on the separate parts of the point passed as arguements.
        /// </summary>
        /// <param name="organisationId">The identity id of the organisation this point is registered to.</param>
        /// <param name="organisationName">The name of the organisation this point is registered to.</param>
        /// <param name="closestTownId">The id of the town id that is closest to this point.</param>
        /// <param name="description">The description that should be given to this point.</param>
        /// <param name="addressLine1">The first line of the address.</param>
        /// <param name="addressLine2">The second line of the address.</param>
        /// <param name="addressLine3">The third line of the address.</param>
        /// <param name="postTown">The town.</param>
        /// <param name="county">The county the point is within.</param>
        /// <param name="postCode">The post code.</param>
        /// <param name="longitude">The longitude attached to this point.</param>
        /// <param name="latitude">The latitude attached to this point.</param>
        /// <param name="trafficAreaId">The traffic area for this point.</param>
        /// <param name="userId">The id of the user creating this point.</param>
        /// <returns>The id of the new point created, or 0 if there were infringments encountered.</returns>
        private int CreateNewPoint(int organisationId, string organisationName, int closestTownId, string description, string addressLine1, string addressLine2, string addressLine3, string postTown, string county, string postCode, decimal longitude, decimal latitude, int trafficAreaId, string userId)
        {
            Entities.FacadeResult retVal = null;

            Entities.Point point = new Entities.Point();
            point.Address = new Entities.Address();
            point.Address.AddressLine1 = addressLine1;
            point.Address.AddressLine2 = addressLine2;
            point.Address.AddressLine3 = addressLine3;
            point.Address.PostTown = postTown;
            point.Address.County = county;
            point.Address.PostCode = postCode;
            point.Address.Longitude = longitude;
            point.Address.Latitude = latitude;
            point.Address.TrafficArea = new Entities.TrafficArea();
            point.Address.TrafficArea.TrafficAreaId = trafficAreaId;
            point.Address.AddressType = eAddressType.Point;

            point.Description = description;
            point.IdentityId = organisationId;
            point.Latitude = latitude;
            point.Longitude = longitude;
            point.OrganisationName = organisationName;
            Facade.IPostTown facPostTown = new Facade.Point();
            point.PostTown = facPostTown.GetPostTownForTownId(closestTownId);

            Facade.IPoint facPoint = new Facade.Point();
            retVal = facPoint.Create(point, userId);

            if (retVal.Success)
                return retVal.ObjectId;
            else
            {
                infringementDisplay.Infringements = retVal.Infringements;
                infringementDisplay.DisplayInfringments();

                return 0;
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
        /// Populates the details about the refused/returned goods.
        /// </summary>
        /// <returns>The populated object.</returns>
        private Entities.GoodsRefusal PopulateGoodsRefusal()
        {
            Entities.GoodsRefusal goodsRefusal = new Entities.GoodsRefusal();

            goodsRefusal.Docket = cboOrder.SelectedValue;
            goodsRefusal.OriginalOrderId = int.Parse(cboOrder.SelectedValue);

            // Populate the goods refusal object.
            goodsRefusal.RefusalId = Convert.ToInt32(hidRefusalId.Value);
            goodsRefusal.RefusalReceiptNumber = txtReceiptNumber.Text;

            // Set the return point.
            //if (cboGoodsReturnPoint.Text != String.Empty)
            //{
            // Configure the return point.
            int returnPoint = 0;

            //	if (cboGoodsReturnPoint.Value == String.Empty)
            //	{
            // A new return point has been specified.
            //returnPoint = CreateNewPoint(Convert.ToInt32(cboGoodsReturnOrganisation.Value), cboGoodsReturnOrganisation.Text, Convert.ToInt32(cboGoodsReturnTown.Value), cboGoodsReturnPoint.Text, txtGoodsReturnAddressLine1.Text, txtGoodsReturnAddressLine2.Text, txtGoodsReturnAddressLine3.Text, txtGoodsReturnPostTown.Text, txtGoodsReturnCounty.Text, txtGoodsReturnPostCode.Text, Convert.ToDecimal(txtGoodsReturnLongitude.Text), Convert.ToDecimal(txtGoodsReturnLatitude.Text), Convert.ToInt32(hidGoodsReturnTrafficArea.Value), ((Entities.CustomPrincipal) Page.User).UserName);
            //	}
            //	else

            returnPoint = ucReturnedToPoint.PointID;

            if (returnPoint > 0)
            {
                Facade.IPoint facPoint = new Facade.Point();
                goodsRefusal.ReturnPoint = facPoint.GetPointForPointId(returnPoint);
            }
            //}

            eGoodsRefusedType refusedType = (eGoodsRefusedType)Enum.Parse(typeof(eGoodsRefusedType), cboGoodsReasonRefused.SelectedValue.Replace(" ", ""));

            // Set the store point.
            int storePoint = 0;

            //if (cboGoodsStorePoint.Value == String.Empty)
            //{
            //if (cboGoodsStorePoint.Text != String.Empty)
            //{
            // A new store point has been specified.
            //storePoint = CreateNewPoint(Convert.ToInt32(cboGoodsStoreOrganisation.Value), cboGoodsStoreOrganisation.Text, Convert.ToInt32(cboGoodsStoreTown.Value), cboGoodsStorePoint.Text, txtGoodsStoreAddressLine1.Text, txtGoodsStoreAddressLine2.Text, txtGoodsStoreAddressLine3.Text, txtGoodsStorePostTown.Text, txtGoodsStoreCounty.Text, txtGoodsStorePostCode.Text, Convert.ToDecimal(txtGoodsStoreLongitude.Text), Convert.ToDecimal(txtGoodsStoreLatitude.Text), Convert.ToInt32(hidGoodsStoreTrafficArea.Value), ((Entities.CustomPrincipal) Page.User).UserName);
            //}
            //}
            //else

            storePoint = ucStoredAtPoint.PointID;

            if (storePoint > 0)
            {
                Facade.IPoint facPoint = new Facade.Point();
                goodsRefusal.StorePoint = facPoint.GetPointForPointId(storePoint);
            }

            goodsRefusal.RefusalLocation = (eGoodsRefusedLocation)Enum.Parse(typeof(eGoodsRefusedLocation), cboLocation.SelectedValue.Replace(" ", ""));
            goodsRefusal.ProductCode = txtProductCode.Text;
            goodsRefusal.PackSize = txtPackSize.Text;
            goodsRefusal.ProductName = txtProductName.Text;
            goodsRefusal.QuantityOrdered = (txtQuantityOrdered.Text == String.Empty) ? 0 : Convert.ToInt32(txtQuantityOrdered.Text);
            goodsRefusal.QuantityRefused = (txtQuantityReturned.Text == String.Empty) ? 0 : Convert.ToInt32(txtQuantityReturned.Text);
            goodsRefusal.RefusalNotes = txtGoodsNotes.Text;
            goodsRefusal.TimeFrame = rdiReturnDate.SelectedDate.HasValue ? rdiReturnDate.SelectedDate.Value : DateTime.Now.AddDays(Orchestrator.Globals.Configuration.RefusalNoOfDaysToReturnGoods);
            goodsRefusal.InstructionId = Convert.ToInt32(hidInstructionId.Value);

            goodsRefusal.NoPallets = Convert.ToInt32(rntNoPallets.Value.Value);
            goodsRefusal.NoPalletSpaces = Convert.ToDecimal(rntNoPalletSpaces.Value.Value);

            goodsRefusal.RefusalType = Convert.ToInt32(cboGoodsReasonRefused.SelectedItem.Value);
            goodsRefusal.RefusalStatus = (eGoodsRefusedStatus)Enum.Parse(typeof(eGoodsRefusedStatus), cboGoodsStatus.SelectedValue.Replace(" ", ""));

            Facade.IInstruction facInstruction = new Facade.Instruction();
            Entities.Instruction instruction = facInstruction.GetInstruction(m_instructionId);

            int deviationReasonId;
            int.TryParse(this.cboDeviationReason.SelectedValue, out deviationReasonId);

            if (deviationReasonId == 0)
                goodsRefusal.DeviationReasonId = -1;
            else
                goodsRefusal.DeviationReasonId = deviationReasonId;

            goodsRefusal.RefusedFromPointId = instruction.PointID;

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
            this.Init += new EventHandler(Refusal_Init);
        }
        #endregion
    }
}
