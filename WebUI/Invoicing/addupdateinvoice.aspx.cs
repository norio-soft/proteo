using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;  
using System.IO;
using System.Globalization;
using System.Text;

using System.Transactions;

using Orchestrator.WebUI.UserControls;
using Orchestrator.Entities; 
using Orchestrator.Globals;

using P1TP.Components.Web.Validation;
using Orchestrator.WebUI.Security;


namespace Orchestrator.WebUI.Invoicing
{
	/// <summary>
	/// Summary description for addupdateinvoice.
	/// </summary>
	public partial class addupdateinvoice : Orchestrator.Base.BasePage
	{	
		#region Group Point Class
		[Serializable]				  
			private class GroupPoint
		{
			#region Private Fields
			private int			m_pointId = 0;
			private string		m_invoiceId = string.Empty;
            private string      m_batchId = string.Empty;
			private string		m_jobIdList = string.Empty;
			private string		m_fullAddress = string.Empty;
			private string		m_organisationName = string.Empty;
			private string		m_pointName = string.Empty;
			private string		m_collectionPoint = string.Empty;

			#endregion

			#region Constructors
			public GroupPoint(){}
			public GroupPoint(int pointId, string invoiceId, string jobIdList, string fullAddress, string organisationName, string pointName, string collectionPoint)
			{
				m_pointId = pointId; 
				m_invoiceId = invoiceId;
				m_jobIdList = jobIdList;
				m_fullAddress = fullAddress;
				m_organisationName = organisationName;
				m_pointName = pointName;
				m_collectionPoint = collectionPoint;
			}
			#endregion
		
			#region Public Properties
		
			public int PointId
			{
				get {return m_pointId;}
				set {m_pointId = value;}
			}

			public string InvoiceId
			{
				get {return m_invoiceId;}
				set {m_invoiceId= value;}
			}

			public string JobIdList
			{
				get {return m_jobIdList;}
				set {m_jobIdList= value;}
			}

			public string FullAddress
			{
				get {return m_fullAddress;}
				set {m_fullAddress = value;}
			}

			public string OrganisationName
			{
				get {return m_organisationName;}
				set {m_organisationName = value;}
			}

			public string PointName
			{
				get {return m_pointName;}
				set {m_pointName = value;}
			}

			public string CollectionPoint
			{
				get { return m_collectionPoint; }
				set { m_collectionPoint = value; }
			}
			#endregion 
		}	   
		#endregion
	
		#region Constant & Enums
		
		
        private const string C_CLIENTID_VS = "ClientId";
        private const string C_CLIENTNAME_VS = "ClientName";
        private const string C_BATCHID_VS = "BatchId";
        private const string C_JOBIDCSV_VS = "JobIdCSV";
        private const string C_EXTRAIDCSV_VS = "ExtraIdCSV";
        private const string C_FILENAME_VS = "FileName";
        private const string C_FILELOCATION_VS = "FileLocation";

        private const string C_GROUP_POINT_ARRAY_VS = "GroupPointArray";
        private const string C_GRID_LOCATION_ID_VS = "GridLocationId";

		#endregion

		#region Page Variables
        private int ClientId
        {
            get { return Convert.ToInt32(this.ViewState[C_CLIENTID_VS]); }
            set { this.ViewState[C_CLIENTID_VS] = value; }
        }

        private string ClientName
        {
            get { return Convert.ToString(this.ViewState[C_CLIENTNAME_VS]); }
            set { this.ViewState[C_CLIENTNAME_VS] = value; }
        }

        private int BatchId
        {
            get { return Convert.ToInt32(this.ViewState[C_BATCHID_VS]); }
            set { this.ViewState[C_BATCHID_VS] = value; }
        }

        private string JobIdCSV
        {
            get { return Convert.ToString(this.ViewState[C_JOBIDCSV_VS]); }
            set { this.ViewState[C_JOBIDCSV_VS] = value; }
        }

        private string ExtraIdCSV
        {
            get { return Convert.ToString(this.ViewState[C_EXTRAIDCSV_VS]); }
            set { this.ViewState[C_EXTRAIDCSV_VS] = value; }
        }

        private string FileName
        {
            get { return Convert.ToString(this.ViewState[C_FILENAME_VS]); }
            set { this.ViewState[C_FILENAME_VS] = value; }
        }

        private string FileLocation
        {
            get { return Convert.ToString(this.ViewState[C_FILELOCATION_VS]); }
            set { this.ViewState[C_FILELOCATION_VS] = value; }
        }

        private Entities.Invoice Invoice
        {
            get { return (Entities.Invoice )this.ViewState["Invoice"]; }
            set { this.ViewState["Invoice"] = value; }
        }

        private ArrayList GroupPointArray
        {
            get { return (ArrayList)this.ViewState[C_GROUP_POINT_ARRAY_VS]; }
            set { this.ViewState[C_GROUP_POINT_ARRAY_VS] = value; }
        }

        private int GridLocationId
        {
            get { return (int)this.ViewState[C_GRID_LOCATION_ID_VS]; }
            set { this.ViewState[C_GRID_LOCATION_ID_VS] = value; }
        }

        private int InvoiceId
        {
            get { return (int)this.ViewState["InvoiceId"]; }
            set { this.ViewState["InvoiceId"] = value; }
        }

        private DateTime StartDate
        {
            get { return Convert.ToDateTime(this.ViewState["StartDate"]); }
            set { this.ViewState["StartDate"] = value; }
        }

        private DateTime EndDate
        {
            get { return Convert.ToDateTime(this.ViewState["EndDate"]); }
            set { this.ViewState["EndDate"] = value; }
        }

        private bool				m_isUpdate = false;
		//private ArrayList			m_arrJobId = null;
		//private ArrayList			m_arrExtraId;
		//private string				m_extraIdCSV = String.Empty;
		//private int					m_InvoiceNo = 0;
		//private int					m_batchId = 0;
		//private Entities.Invoice	m_Invoice;		
		//private string				m_jobIdCSV = String.Empty;
		//private string				m_fileLocation;
		//private string				m_fileName;
		//private int					m_clientId = 0;
        //private string              m_clientName = String.Empty;


		#endregion 

		#region Form Elements

		protected System.Web.UI.WebControls.Panel					pnlPCVDeleted;
		protected System.Web.UI.WebControls.CheckBox				chkDiscrepancies;
		protected System.Web.UI.WebControls.Label					lblDatePosted;
		protected System.Web.UI.WebControls.Label					lblDatePrinted;
		
		protected System.Web.UI.WebControls.TextBox					txtVAT;
		protected System.Web.UI.WebControls.TextBox					txtAmount;
		protected System.Web.UI.WebControls.TextBox					txtReason;
		protected System.Web.UI.WebControls.TextBox					txtTotalAmount;
		#endregion
	
		#region Page/Load/Init/Error
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.Invoicing);

			//Save the invoiceId if not a postback
			if (!this.IsPostBack)
                this.InvoiceId = Convert.ToInt32(Request.QueryString["InvoiceId"]);

            //Dwetermine whether the invoice or new or updated
            if (this.InvoiceId > 0 || lblInvoiceNo.Text != "To Be Issued ... (This invoice has not yet been saved, add invoice to allocate Invoice No.)")
                m_isUpdate = true;			
			
            if (!this.IsPostBack)
            {
                //Check whether to see if this is a batch invoice
                //If it is then BatchId, ClientId and JobsIds get passed on the QueryString
                //so put them into the viewstate
                int batchId = Convert.ToInt32(Request.QueryString["BatchId"]);
                if (batchId != 0)
                {
                    SaveQueryStringParameters();

                    Facade.IInvoiceBatches facInvoiceBatches = new Facade.Invoice();
                    dteInvoiceDate.SelectedDate = facInvoiceBatches.GetInvoiceDate(batchId);
                }

                //If not a Batch then the params will be passed through the Session
                //so put them into the ViewState
                else
                {
                    SaveSessionParmeters();
                }

                PopulateStaticControls();

                if (m_isUpdate)
                {
                    LoadInvoice();
                    LoadReport();
                }
            }

            // New Invoice  
            //m_arrJobId = (ArrayList) Session["JobIds"];  
			//m_arrExtraId = (ArrayList) Session["ExtraIds"];
            //m_fileLocation = (string) Session["FileLocation"];
			//m_fileName = (string) Session["FileName"];

            //if (Session["BatchId"] != null)
            //{
            //    m_batchId = (int) Session["BatchId"];
            //    ViewState[C_BATCHID_VS] = m_batchId;
            //}

		
            //if (!IsPostBack)
            //{
            //    //#15857 J.Steele Prevent session problems by saving session variables immediately
            //    this.ClientId =Convert.ToInt32(Request.QueryString["ClientId"]);
                
            //    if (m_arrJobId != null)
            //    {
            //        foreach (object item in m_arrJobId)
            //        {
            //            if (m_jobIdCSV  != String.Empty)
            //                m_jobIdCSV += ",";

            //            m_jobIdCSV  += Convert.ToInt32(item);
            //        }
            //        ViewState[C_JOBIDCSV_VS] = m_jobIdCSV;
            //    }

            //    if (m_arrExtraId != null)
            //    {
            //        foreach (object item in m_arrExtraId)
            //        {
            //            if (m_extraIdCSV != string.Empty)
            //                m_extraIdCSV += ",";

            //            m_extraIdCSV += Convert.ToInt32(item);
            //        }
            //        ViewState[C_EXTRAIDCSV_VS] = m_extraIdCSV;

            //    }
     
				
		}
		
        private void SaveQueryStringParameters()
        {
            this.BatchId = Convert.ToInt32(Request.QueryString["BatchId"]);
            this.ClientId = Convert.ToInt32(Request.QueryString["ClientId"]);
            this.JobIdCSV  = Request.QueryString["JobIds"].ToString();
        }

        private void SaveSessionParmeters()
        {
            this.ClientId = Convert.ToInt32(Session["ClientId"]);
            this.ClientName = (string)Session["ClientName"];
            this.FileLocation  = (string)Session["FileLocation"];
            this.FileName = (string)Session["FileName"];
            
            //JobIds
            ArrayList jobIds = (ArrayList) Session["JobIds"];
            if (jobIds != null)
            {
                string jobIdCSV = string.Empty;
                foreach (object item in jobIds)
                {
                    if (jobIdCSV != String.Empty)
                        jobIdCSV += ",";

                    jobIdCSV += Convert.ToInt32(item);
                }
                this.JobIdCSV  = jobIdCSV;
            }

            //ExtraIds
            ArrayList extraIds = (ArrayList) Session["ExtraIds"];
            if (extraIds != null)
            {
                string extraIdCSV = String.Empty;
                foreach (object item in extraIds)
                {
                    if (extraIdCSV != String.Empty)
                        extraIdCSV += ",";

                    extraIdCSV += Convert.ToInt32(item);
                }
                this.ExtraIdCSV  = extraIdCSV;
            }
        }

        //private void SetFieldsFromViewState()
        //{
        //    m_batchId = ViewState[C_BATCHID_VS];
        //    m_clientId = ViewState[C_CLIENTID_VS] = Convert.ToInt32(Request.QueryString["ClientId"]);
        //    m_clientName = string.Empty;
        //    m_fileLocation = ViewState[C_FILELOCATION_VS] = string.Empty;
        //    m_fileName = ViewState[C_FILENAME_VS] = string.Empty;

        //    m_arrJobId = ViewState[C_JOBIDCSV_VS] = Request.QueryString["JobIds"].ToString();
        //    m_extraIdCSV = ViewState[C_EXTRAIDCSV_VS] = string.Empty;
        //}

		protected void addupdateinvoice_Init(object sender, EventArgs e)
		{
			// Checkbox Events
			this.chkIncludeFuelSurcharge.CheckedChanged += new System.EventHandler(this.chkIncludeFuelSurcharge_CheckedChanged);
			this.chkIncludeDemurrage.CheckedChanged +=new EventHandler(chkIncludeDemurrage_CheckedChanged);  
			this.chkIncludePODs.CheckedChanged +=new EventHandler(chkIncludePODs_CheckedChanged); 
			this.chkIncludeReferences.CheckedChanged +=new EventHandler(chkIncludeReferences_CheckedChanged); 
			
			this.chkSelfBillRemainder.CheckedChanged += new System.EventHandler(this.chkSelfBillRemainder_CheckedChanged);
			this.chkOverride.CheckedChanged += new System.EventHandler(this.chkOverride_CheckedChanged);
			this.chkJobDetails.CheckedChanged += new System.EventHandler(this.chkJobDetails_CheckedChanged);
			
			// Button Events
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			this.btnViewInvoice.Click += new System.EventHandler(this.btnViewInvoice_Click);
			this.btnSendToAccounts.Click += new System.EventHandler(this.btnSendToAccounts_Click);
			this.btnAddSelfBill.Click +=new EventHandler(btnAddSelfBill_Click); 

			// Data Grid Events
			this.dgGroup.ItemCommand += new DataGridCommandEventHandler(dgGroup_ItemCommand);
			this.dgGroup.ItemDataBound +=new DataGridItemEventHandler(dgGroup_ItemDataBound);
			
			// Radio Button Events
			this.rdoSortType.SelectedIndexChanged += new System.EventHandler(this.rdoSortType_SelectedIndexChanged);
			this.rdoGroupBy.SelectedIndexChanged += new System.EventHandler(this.rdoGroupBy_SelectedIndexChanged);

			// Text Box Events
			this.txtFuelSurchargeRate.TextChanged +=new EventHandler(txtFuelSurchargeRate_TextChanged);
		}
	#endregion
		
		#region Populate Static Controls
		///	<summary> 
		/// Populate Static Controls
		///	</summary>
		private void PopulateStaticControls()
		{
			//-----------------------------------------------------------------------------
			//									New Section
			//-----------------------------------------------------------------------------
			// Populate Includes from Client/Organisation Settings
			int clientId = 0;  
			
			Facade.IInvoice facInvoice = new Facade.Invoice();
			DataSet ds = facInvoice.GetInvoiceForInvoiceId(this.InvoiceId);

            if (ds.Tables[0].Rows.Count != 0)
            {
                // Populate ClientId Session Variable
                clientId = Convert.ToInt32(ds.Tables[0].Rows[0]["IdentityId"]);
            }
            else
            {
                //#15857 J.Steele Prevent session problems by saving session variables immediately
                //if (m_clientId == 0)
                //    clientId = (int)Session["ClientId"];
                //else
                    clientId = this.ClientId;
            }

            Facade.INominalCode facNominalCode = new Facade.NominalCode();
            DataSet dsNominalCode = facNominalCode.GetAllActive();

            DataTable dt = dsNominalCode.Tables[0];
            DataRow dr = dt.NewRow();
            dr["NominalCode"] = "";
            dr["Description"] = "Please Select a Nominal Code";
            dt.Rows.InsertAt(dr, 0);

            cboNominalCode.DataSource = dt;
            cboNominalCode.DataTextField = "Description";
            cboNominalCode.DataValueField = "NominalCode";
            cboNominalCode.DataBind();
            cboNominalCode.Items[0].Selected = true;

			Facade.IOrganisation facOrg = new Facade.Organisation();
			Entities.OrganisationDefault odc = facOrg.GetInvoiceSettingForIdentityId(clientId);
			
			chkIncludeReferences.Checked = odc.IncludeReferences;

			chkIncludeFuelSurcharge.Checked = odc.IncludeFuelSurcharge;
			
			if (odc.IncludeFuelSurcharge)
			{
				divFuelSurcharge.Visible = true;
				rdoFuelSurchargeType.Visible = true;
				txtFuelSurchargeRate.Visible = true;
                txtFuelSurchargeRate.Text = odc.FuelSurchargePercentage.Value.ToString("F2");  
			}
			else				
			{
				divFuelSurcharge.Visible = false;
				rdoFuelSurchargeType.Visible = false;
				txtFuelSurchargeRate.Visible = false; 
			}
			chkIncludeDemurrage.Checked = odc.IncludeDemurrage; 

			if (odc.IncludeDemurrage)
				rdoDemurrageType.Visible = true;
			else
				rdoDemurrageType.Visible = false;
								
			chkIncludePODs.Checked = odc.IncludePODs;
            chkIncludePODs.Visible = false;

			chkJobDetails.Checked = odc.IncludeJobDetails;
            chkExtrasPerJob.Checked = odc.IncludeJobExtras;
			
			chkExtraDetails.Checked = odc.IncludeExtraDetails;

            chkShowInstructionNotes.Checked = odc.IncludeInstructionNotes;
			
			// Self Bill Status
			cboSelfBillStatus.DataSource = Enum.GetNames(typeof(eSelfBillStatus));
			cboSelfBillStatus.DataBind();
			cboSelfBillStatus.Items[0].Selected = true;

			// Invoice Sort Type
			rdoSortType.DataSource =  Utilities.UnCamelCase(Enum.GetNames(typeof(eInvoiceSortType)));
			rdoSortType.DataBind();
			rdoSortType.Items[0].Selected = true;
			
			// Invoice Grouping 
			rdoGroupBy.DataSource = Enum.GetNames(typeof(eInvoiceGrouping));
			rdoGroupBy.DataBind();
			rdoGroupBy.Items[0].Selected = true;

			// Fuel Type
            rdoFuelSurchargeType.DataSource = Enum.GetNames(typeof(eInvoiceDisplayMethod));
			rdoFuelSurchargeType.DataBind();
            rdoFuelSurchargeType.Items[0].Selected = true;

			// Demurrage Type
            rdoDemurrageType.DataSource = Enum.GetNames(typeof(eInvoiceDisplayMethod));
			rdoDemurrageType.DataBind();
			
			// Invoice Type
			Facade.IOrganisation facType = new Facade.Organisation();
			DataSet dsInvoiceType = facType.GetDefaultsForIdentityId(clientId);

			eInvoiceType invoiceType = (eInvoiceType) Enum.Parse (typeof(eInvoiceType), dsInvoiceType.Tables[0].Rows[0]["InvoiceTypeId"].ToString());
			lblInvoiceType.Text = invoiceType.ToString(); 
 
			
			if (invoiceType == eInvoiceType.SelfBill)
			{
				lblClientInvoiceSelfBillAmount.Visible = true;
				lblClientSelfBillInvoiceNumber.Visible = true;
				txtClientSelfBillAmount.Visible = true; 
				txtClientSelfBillInvoiceNumber.Visible = true;
				divClientSelfBillAmount.Visible = true;

				//chkIncludePODs.Checked = chkIncludeReferences.Checked = chkIncludeDemurrage.Checked =
				//	chkIncludeFuelSurcharge.Checked = chkJobDetails.Checked = false;

				// Populate Invoice Amount From The Client (Up to the user to check that these amounts tally up
				txtClientSelfBillAmount.Text =  facInvoice.GetTotalAmountForJobIds(this.JobIdCSV).ToString("C");
                btnSendToAccounts.Visible = false;
            }

            if (this.ExtraIdCSV != null)
            {
                pnlExtras.Visible = true;
                Facade.IInvoiceExtra facInvoiceExtra = new Facade.Invoice();
                dgExtras.DataSource = facInvoiceExtra.GetExtraDataSetForExtraIds(this.ExtraIdCSV);
                dgExtras.DataBind();
                chkExtraDetails.Visible = true;
            }

            pnlExtras.Visible = chkExtraDetails.Visible = (!string.IsNullOrEmpty(this.ExtraIdCSV) || dgExtras.Items.Count > 0);

			// ONLY new invoices
			if (!m_isUpdate)
			{
				// Check that this invoice has demurrage
				Facade.IInvoice checkDem = new Facade.Invoice();
				bool hasDemurrage = checkDem.CheckDemurrageForInvoice(this.JobIdCSV );
				if (!hasDemurrage)
				{
					chkIncludeDemurrage.Visible = false;
					lblNoDemurrage.Visible = true;
				}
				
				// Default Fuel Type and Rate & Demurrage Type 
                eInvoiceDisplayMethod fuelType = (eInvoiceDisplayMethod)Enum.Parse(typeof(eInvoiceDisplayMethod), dsInvoiceType.Tables[0].Rows[0]["FuelSurchargeBreakdownTypeId"].ToString());//.ToString();

                rdoFuelSurchargeType.ClearSelection();
				rdoFuelSurchargeType.Items[(int)fuelType - 1].Selected = true;

                eInvoiceDisplayMethod demurrageType = (eInvoiceDisplayMethod)Enum.Parse(typeof(eInvoiceDisplayMethod), dsInvoiceType.Tables[0].Rows[0]["DemurrageTypeId"].ToString());
  				rdoDemurrageType.Items[(int)demurrageType - 1].Selected = true;  
			}

            if (chkJobDetails.Checked)
                pnlSort.Visible = true;
            else
                pnlSort.Visible = false;

			
			//-----------------------------------------------------------------------------
			//									Update Section
			//-----------------------------------------------------------------------------
			if (m_isUpdate) 
			{
				lblInvoiceNo.Visible = true;
				lblInvoiceNo.Text = this.InvoiceId.ToString();
				btnAdd.Text = "Update Invoice";
				// No Grouping allowed
				pnlGroup.Visible = false;
                pnlExtras.Visible = false;
			}
		}
		
		#endregion 

		#region Add/Update/Load/Populate Invoice
		///	<summary> 
		/// Load Invoice
		///	</summary>
		private void LoadInvoice()
		{
            Entities.Invoice invoice;

            if (this.Invoice == null)
			{
				Facade.IInvoice facInvoice = new Facade.Invoice();
				Facade.IInvoiceExtra facInvoiceExtra = new Facade.Invoice();
				invoice = facInvoice.GetForInvoiceId(this.InvoiceId);
				
				if (invoice.InvoiceType == eInvoiceType.SelfBill)
				{
					invoice.Extras = facInvoiceExtra.GetExtraCollectionForInvoiceId(invoice.InvoiceId);
				}

                this.Invoice = invoice;
			}
			else
				invoice = this.Invoice;

			// Load the report with the relevant details
			if (invoice != null)
			{

				lblInvoiceNo.Text = invoice.InvoiceId.ToString();  
				lblInvoiceNo.ForeColor = Color.Black;
				
				lblInvoiceType.Text = invoice.InvoiceType.ToString();
				
				if (invoice.InvoiceType == eInvoiceType.SelfBill)
				{
					lblClientInvoiceSelfBillAmount.Visible = true;
					lblClientSelfBillInvoiceNumber.Visible = true; 
					txtClientSelfBillAmount.Visible = true;
					txtClientSelfBillAmount.Text = invoice.ClientInvoiceAmount.ToString("C");
					txtClientSelfBillInvoiceNumber.Visible = true;
					txtClientSelfBillInvoiceNumber.Text = invoice.SelfBillInvoiceNumber; 
					divClientSelfBillAmount.Visible = true;
					chkSelfBillRemainder.Visible = true;
					lblRemainder.Visible = true;
				}

				if (invoice.OverrideReason != string.Empty)
				{
					pnlOverride.Visible = true;
					chkOverride.Checked = true;
					txtOverrideReason.Text = invoice.OverrideReason.ToString();
					txtOverrideGrossAmount.Text = invoice.OverrideTotalAmountGross.ToString("C");
					txtOverrideNetAmount.Text = invoice.OverrideTotalAmountNet.ToString("C");
					txtOverrideVAT.Text = invoice.OverrideTotalAmountVAT.ToString("C"); 
				}

                if (cboNominalCode.FindItemByValue(invoice.NominalCode) != null)
                    cboNominalCode.FindItemByValue(invoice.NominalCode).Selected = true;
                cboNominalCode.Enabled = !invoice.Posted;

				// Display the invoice date, but only allow the date to be altered if the invoice has not been posted.
                dteInvoiceDate.SelectedDate = invoice.InvoiceDate;
				dteInvoiceDate.Enabled = !invoice.Posted;

				lblDateCreated.Text = invoice.CreatedDate.ToShortDateString(); 
				lblDateCreated.ForeColor = Color.Black; 
				
				txtInvoiceNotes.Text = invoice.InvoiceDetails; 
				
				chkIncludePODs.Checked = invoice.IncludePODs;

				chkIncludeReferences.Checked = invoice.IncludeReferences;
				
				if (invoice.IncludeDemurrage)
				{
					chkIncludeDemurrage.Checked = true;
					rdoDemurrageType.SelectedIndex = Convert.ToInt32(invoice.DemurrageType); 
					rdoDemurrageType.Visible = true;
				}
				else
				{
					chkIncludeDemurrage.Visible = false;
					lblNoDemurrage.Visible = true;
					rdoDemurrageType.Visible = false;
				}

				if (invoice.IncludeFuelSurcharge)
				{
					chkIncludeFuelSurcharge.Checked = true;
					txtFuelSurchargeRate.Text = invoice.FuelSurchargeRate.ToString();
                    rdoFuelSurchargeType.ClearSelection();
					rdoFuelSurchargeType.SelectedIndex = Convert.ToInt32(invoice.FuelSurchargeType); 
					rdoFuelSurchargeType.Visible = true;
					divFuelSurcharge.Visible = true;
				}
				else
				{
					divFuelSurcharge.Visible = false;
					chkIncludeFuelSurcharge.Checked = false;
					rdoFuelSurchargeType.Visible = false; 
				}
					
				chkJobDetails.Checked = invoice.IncludeJobDetails;
  
				chkExtraDetails.Checked = invoice.IncludeExtraDetails;

                chkExtrasPerJob.Checked = invoice.ShowJobExtras;

                chkShowInstructionNotes.Checked = invoice.IncludeInstructionNotes;

				rdoSortType.SelectedIndex = Convert.ToInt32 (invoice.InvoiceSortingType) - 1;

				this.JobIdCSV = invoice.JobIdCSV;
	
				if (invoice.InvoiceType == eInvoiceType.SelfBill && invoice.Extras != null)
				{
					
					if (invoice.Extras.Count != 0) 
					{
						pnlExtras.Visible = true;
						dgExtras.DataSource = invoice.Extras;
						dgExtras.DataBind();	

						string extraIdCSV = "";
						foreach (Entities.Extra extra in invoice.Extras)
						{
							if (extraIdCSV.Length > 0)
								extraIdCSV += ",";
							extraIdCSV += extra.ExtraId;
						}

						this.ExtraIdCSV = extraIdCSV;
						chkExtraDetails.Visible = true;
					}
					else
					{
						pnlExtras.Visible = false; 
					}
				}
				   
				if (m_isUpdate)
				{
					if (invoice.ForCancellation)
					{
						btnAdd.Visible = false;
						btnSendToAccounts.Visible = false;
						chkPostToExchequer.Visible = false;
						chkDelete.Checked = true;
					}
					else
					{
						if (!chkPostToExchequer.Checked)
						{
							btnAdd.Visible = true;
							btnSendToAccounts.Visible = true;
							chkPostToExchequer.Visible = true;
							chkDelete.Checked = false;						
						}
					}
				}
				else
					chkPostToExchequer.Visible = true;

				if (invoice.Posted)
				{
					btnAdd.Visible = false;
					btnSendToAccounts.Visible = false; 
					chkPostToExchequer.Checked = true;
					chkPostToExchequer.Visible = true;
					pnlInvoiceDeleted.Visible = false;
					chkDelete.Visible = false;
					pnlExtras.Enabled = false;
					pnlGroup .Enabled = false;
					pnlIncludes.Enabled = false;
					pnlOverride.Enabled = false;    
					chkOverride.Enabled = false;
					pnlSelfRemainder.Enabled = false;
					txtInvoiceNotes.Enabled = false;
					rdoSortType.Enabled = false;
					txtClientSelfBillAmount.Enabled = false;
					chkSelfBillRemainder.Enabled = false;
					txtClientSelfBillInvoiceNumber.Enabled = false; 
					btnSendToAccounts.Visible = false;
					btnViewInvoice.Visible = false;
					dteInvoiceDate.Enabled = false;
					//txtReason.Enabled = false;
				}
				else
				{
					btnAdd.Visible = true;
					btnSendToAccounts.Visible = true; 
					chkPostToExchequer.Checked = false;
					pnlInvoiceDeleted.Visible = true; 
					chkDelete.Visible = true;
				}
			}	 	

			Header1.Title = "Update Invoice";
			Header1.subTitle = "Please make any changes neccessary.";
			btnAdd.Text = "Update";

            if (invoice.InvoiceType == eInvoiceType.SelfBill)
                btnSendToAccounts.Visible = false;
		}
		
		///	<summary> 
		/// Populate Invoice into m_Invoice
		///	</summary>
		private void populateInvoice()
		{
            Entities.Invoice invoice;
    
            if (this.Invoice == null)
				invoice = new Entities.Invoice();
			else
				invoice = this.Invoice;

			// Stephen Newman - 06-SEP-2005
			// Added the invoice date to the entity.
			invoice.InvoiceDate = dteInvoiceDate.SelectedDate.Value;
			
            // Set the account code.
            Entities.Organisation orgClient = null;
            Facade.Organisation facOrg = new Orchestrator.Facade.Organisation();
            orgClient = facOrg.GetForIdentityId(ClientId);
            invoice.AccountCode = orgClient.AccountCode;
            //

			if (lblInvoiceNo.Text != "To Be Issued ... (This invoice has not yet been saved, add invoice to allocate Invoice No.)")
				invoice.InvoiceId = Convert.ToInt32(lblInvoiceNo.Text);
			
			invoice.InvoiceType = (eInvoiceType)Enum.Parse(typeof(eInvoiceType), lblInvoiceType.Text);
            invoice.NominalCode = cboNominalCode.SelectedValue;

			if ((eInvoiceType) Enum.Parse(typeof(eInvoiceType), lblInvoiceType.Text) == eInvoiceType.SelfBill)
			{
				invoice.ClientInvoiceAmount = Decimal.Parse(txtClientSelfBillAmount.Text, System.Globalization.NumberStyles.Currency);
				invoice.SelfBillInvoiceNumber = txtClientSelfBillInvoiceNumber.Text;
			}
			else
			{
				invoice.ClientInvoiceAmount = 0;
				invoice.SelfBillInvoiceNumber = string.Empty;
			}
	
			invoice.InvoiceDetails = txtInvoiceNotes.Text;
 
			if (chkOverride.Checked)
			{
				invoice.OverrideTotalAmountVAT = Decimal.Parse(txtOverrideVAT.Text, System.Globalization.NumberStyles.Currency);
				invoice.OverrideTotalAmountNet = Decimal.Parse(txtOverrideNetAmount.Text, System.Globalization.NumberStyles.Currency);
				invoice.OverrideTotalAmountGross = Decimal.Parse(txtOverrideGrossAmount.Text, System.Globalization.NumberStyles.Currency);
				invoice.OverrideReason = txtOverrideReason.Text; 
			}
			else
			{
				invoice.OverrideTotalAmountVAT = 0;
				invoice.OverrideTotalAmountNet = 0;
				invoice.OverrideTotalAmountGross = 0;
				invoice.OverrideReason = string.Empty; 
			}

			if (chkIncludeDemurrage.Checked)
			{
				invoice.IncludeDemurrage = true;
                invoice.DemurrageType = (eInvoiceDisplayMethod)rdoDemurrageType.SelectedIndex;
			}
			else
				invoice.IncludeDemurrage = false;

			if (chkIncludeFuelSurcharge.Checked)
			{
				invoice.IncludeFuelSurcharge = true;
				invoice.FuelSurchargeRate = Convert.ToDecimal(txtFuelSurchargeRate.Text);
                invoice.FuelSurchargeType = (eInvoiceDisplayMethod)rdoFuelSurchargeType.SelectedIndex;
			}
			else
				invoice.IncludeFuelSurcharge = false;

			invoice.IncludePODs = chkIncludePODs.Checked; 

			invoice.IncludeReferences = chkIncludeReferences.Checked;

			invoice.Posted = chkPostToExchequer.Checked;  

			invoice.IncludeJobDetails = chkJobDetails.Checked;

			invoice.IncludeExtraDetails = chkExtraDetails.Checked;

            invoice.ShowJobExtras = chkExtrasPerJob.Checked;

            invoice.IncludeInstructionNotes = chkShowInstructionNotes.Checked;

			invoice.FileLocation = this.FileLocation;
			invoice.FileName = this.FileName;

			invoice.InvoiceSortingType = (eInvoiceSortType) rdoSortType.SelectedIndex + 1;

            //m_jobIdCSV = this.JobIdCSV;

			//m_extraIdCSV = this.ExtraIdCSV;

            invoice.JobIdCSV = this.JobIdCSV;

            int vatNo = 0;
            decimal vatRate = 0.00M;

			// Vat Rate and Vat Type
			Facade.IInvoice facInv = new Facade.Invoice();
            facInv.GetVatRateForVatType(eVATType.Standard, invoice.InvoiceDate, out vatNo, out vatRate);
			invoice.VatRate = vatRate;
            invoice.VatNo = vatNo;
			// Deleted checked not required until the Invoice's are allowed to be deleted
			if (chkDelete.Checked)
				invoice.ForCancellation = true;
			else
				invoice.ForCancellation = false;

            if (!string.IsNullOrEmpty(this.ExtraIdCSV))
			{
				Facade.IInvoiceExtra facInvoiceExtra = new Facade.Invoice();

				invoice.Extras = facInvoiceExtra.GetExtraCollectionForExtraIds(this.ExtraIdCSV);
	
//				foreach (Entities.Extra extra in m_Invoice.Extras)
//				{
//					foreach (DataGridItem dgItem in dgExtras.Items)
//					{
//						//if (extra.ExtraId.ToString() == dgItem.Cells[0].Text)
//							//extra.InclusionDescription = ((TextBox) dgItem.FindControl("txtInclusionDescription")).Text;
//					}
//				}
			}

            //Save the Invoice to ViewState
            this.Invoice = invoice;
		}
   		
	
		///	<summary> 
		/// Update Invoice
		///	</summary>
		private bool UpdateInvoice()
		{
			Facade.IInvoice facInvoice = new Facade.Invoice();
			bool retVal = false;
			string userName = ((Entities.CustomPrincipal)Page.User).UserName;
            using (TransactionScope ts = new TransactionScope())
			{
				retVal = facInvoice.Update(this.Invoice, this.ClientId, userName);
			
				ts.Complete();
			}
			
			return retVal;
		}
     		
	
		///	<summary> 
		/// Add Invoice
		///	</summary>
		private int AddInvoice()
		{
			// int invoiceId = 0;
			Facade.IInvoice facInvoice = new Facade.Invoice();

			string userName = ((Entities.CustomPrincipal)Page.User).UserName;

            //#15857 J.Steele Prevent session problems by saving session variables immediately
            //if (m_batchId != 0)
             //m_clientId = (int)ViewState[C_CLIENTID_VS];
            //else
            //    m_clientId = int.Parse(Session["ClientId"].ToString());

			//m_jobIdCSV = (string) ViewState[C_JOBIDCSV_VS]; 

			Entities.FacadeResult result = facInvoice.Create(this.Invoice, this.ClientId, userName);
            
			if (!result.Success)
			{
				lblConfirmation.Text = "There was an error adding the Invoice, please try again.";
				lblConfirmation.Visible = true;
				lblConfirmation.ForeColor = Color.Red;

				infringementDisplay.Infringements = result.Infringements;
				infringementDisplay.DisplayInfringments();

				return 0;
			}
			else
			{
				Facade.IInvoiceBatches facBatch =  new Facade.Invoice();
				
				bool success = false;
				  
				//if (ViewState[C_BATCHID_VS] != null)
				//	m_batchId = (int) ViewState[C_BATCHID_VS];  

				if (this.BatchId != 0)
					success = facBatch.Delete(this.BatchId, userName);
			}

			return result.ObjectId;
		}

		#endregion

		#region Methods and Functions
		///	<summary> 
		/// Button Add_Click
		///	</summary>
		private void btnAdd_Click(object sender, System.EventArgs e)
		{
			btnAdd.DisableServerSideValidation();

			if (Page.IsValid)
			{ 
				bool retVal = false;
				int invoiceId = 0;

                //Set m_Invoice and m_isUpdate
				populateInvoice();

				if (m_isUpdate) 
					retVal = UpdateInvoice();
				else
				{
					invoiceId = AddInvoice();
                    this.Invoice.InvoiceId = invoiceId;
				}

				if (m_isUpdate) 
				{
					if (retVal)
					{
						lblConfirmation.Text = "The Invoice has been updated successfully.";
						LoadReport();

						// If the invoice has now been marked as posted - make the invoice date readonly
						dteInvoiceDate.Enabled = !this.Invoice.Posted;
					}
					else
						lblConfirmation.Text = "The Invoice has not been updated successfully.";
				}
				else
				{
					if (invoiceId != 0)
					{
						lblConfirmation.Text = "The Invoice has been added successfully.";
						lblInvoiceNo.Text = invoiceId.ToString();
						lblInvoiceNo.ForeColor = Color.Black;
						lblDateCreated.Text = DateTime.Now.ToShortDateString();
						lblDateCreated.ForeColor = Color.Black;
						btnAdd.Text = "Update Invoice";
						m_isUpdate = true; 
						LoadReport();

                         if (this.BatchId != 0)
                        {
                            mwhelper.OutputData = "<createSingleInvoice invoiceId=\"" + invoiceId.ToString() + "\" />";
                            mwhelper.CloseForm = true;
                            mwhelper.CausePostBack = true;
                        }
					}
				}
			
				lblConfirmation.Visible=true;
				chkPostToExchequer.Visible = true;
			
				if ((eInvoiceType) Enum.Parse(typeof(eInvoiceType), lblInvoiceType.Text) == eInvoiceType.SelfBill)
				{
					chkSelfBillRemainder.Visible = true;
					lblRemainder.Visible = true; 
				}

				// Update the Group Grid if an Invoice has been added
				if (invoiceId != 0  && pnlGroup.Visible == true)
					UpdateGroupGrid(invoiceId);
			}
		}
		
		/// <summary>
		/// Load Report Depending On Invoice Type
		/// </summary>
		private void LoadReport()
		{
			// Configure the Session variables used to pass data to the report
			NameValueCollection reportParams = new NameValueCollection();

			//-------------------------------------------------------------------------------------	
			//						Job/Collect-Drops/References/Demurrages Section
			//-------------------------------------------------------------------------------------	
			Facade.IInvoice facInv = new Facade.Invoice();
	
			DataSet dsInv = null;
			if (m_isUpdate)
				dsInv = facInv.GetJobsForInvoiceId(Convert.ToInt32(lblInvoiceNo.Text));	                
			else
				dsInv = facInv.GetJobsToInvoice(this.JobIdCSV);
  
			reportParams.Add("JobIds", this.JobIdCSV);
            
            //string[] jobs   = this.JobsIdCSV.Split(',');

			reportParams.Add("ExtraIds", this.ExtraIdCSV);



			//-------------------------------------------------------------------------------------	
			//									Param Section
			//-------------------------------------------------------------------------------------	
			// Fuel Type & Rate 
            eInvoiceDisplayMethod fuelType;
			decimal newRate = 0;

			if (chkIncludeFuelSurcharge.Checked)
			{
				reportParams.Add("Fuel", "Include");

				// Pass The New Rate To Report if so...
				newRate = Convert.ToDecimal(txtFuelSurchargeRate.Text); 
				reportParams.Add("FuelRate", newRate.ToString());
				
				// Pass FuelSurchargeType
                fuelType = (eInvoiceDisplayMethod)Enum.Parse(typeof(eInvoiceDisplayMethod), rdoFuelSurchargeType.SelectedValue); 
				reportParams.Add("FuelType", fuelType.ToString());
			}

			// Override
			if (chkOverride.Checked)
			{
				reportParams.Add("Override", "Include");
				reportParams.Add("OverrideVAT", txtOverrideVAT.Text);
				reportParams.Add("OverrideNET", txtOverrideNetAmount.Text);
				reportParams.Add("OverrideGross", txtOverrideGrossAmount.Text);
				reportParams.Add("OverrideReason", txtOverrideReason.Text); 
			}

			// PODs
			if (chkIncludePODs.Checked)
				reportParams.Add("PODs", "Include");

			// References
			if (chkIncludeReferences.Checked)
				reportParams.Add("References", "Include");
 
			// Job 
			if (chkJobDetails.Checked)
			{
				reportParams.Add("JobDetails", "Include");
			
				// Demuragge 
				if (chkIncludeDemurrage.Checked)
				{
					reportParams.Add("Demurrage", "Include");	
				
					// Demurrage Type
					try
					{
                        eInvoiceDisplayMethod demurrageType = (eInvoiceDisplayMethod)Enum.Parse(typeof(eInvoiceDisplayMethod), rdoDemurrageType.SelectedValue); 
						reportParams.Add("DemurrageType", demurrageType.ToString()); 
					}
					catch (Exception) {}
				}
			}

			if (chkExtraDetails.Checked)
			{	
				reportParams.Add("ExtraIds", this.ExtraIdCSV);
				reportParams.Add("Extras", "Include");
                reportParams.Add("ExtraDetail", "include");
            }

            if (chkExtrasPerJob.Checked)
                reportParams.Add("ExtrasPerJob", "Include");

            if (chkShowInstructionNotes.Checked)
                reportParams.Add("InstructionNotes", "Include");

			// Self Bill Invoice Number
			if ((eInvoiceType)Enum.Parse(typeof(eInvoiceType), lblInvoiceType.Text) == eInvoiceType.SelfBill)
			{
				reportParams.Add("InvoiceType", "SelfBill");
				reportParams.Add("SelfBillInvoiceNumber", txtClientSelfBillInvoiceNumber.Text);
			}
			else
				reportParams.Add("InvoiceType", "Normal");

			// Client Name & Id			
			if (m_isUpdate)
			{
				Facade.IInvoice facClient = new Facade.Invoice();

				DataSet ds = facClient.GetClientForInvoiceId(Convert.ToInt32(lblInvoiceNo.Text));

				try
				{
					reportParams.Add("Client", Convert.ToString(ds.Tables[0].Rows[0]["Client"]));
					reportParams.Add("ClientId", Convert.ToString(ds.Tables[0].Rows[0]["ClientId"]));
				
					this.ClientId = int.Parse(ds.Tables [0].Rows [0]["ClientId"].ToString ());
                    this.ClientName = Convert.ToString(ds.Tables[0].Rows[0]["Client"]);

					if (!chkPostToExchequer.Checked )
					{
                        if ((eInvoiceType)Enum.Parse(typeof(eInvoiceType), lblInvoiceType.Text) != eInvoiceType.SelfBill)
						    btnSendToAccounts.Visible = true;

						pnlInvoiceDeleted.Visible = true; 
					}
				}
				catch
				{
				} 
			}
			else
			{
                //if (Convert.ToString (Session["ClientName"]) != "")
                //    reportParams.Add("Client", Convert.ToString (Session["ClientName"]));
                
			
                //if (Convert.ToString (Session["ClientId"]) !="")
                //    reportParams.Add("ClientId", Convert.ToString(Session["ClientId"]));

                //if (m_clientId == 0)
                //    m_clientId = int.Parse(Session["ClientId"].ToString());
                //else
                //{
                //    Facade.IOrganisation facOrg = new Facade.Organisation();
                //    Entities.Organisation enOrg = new Organisation();
                //    enOrg = facOrg.GetForIdentityId(m_clientId);
                //    reportParams.Add("Client", enOrg.OrganisationName.ToString());
                //    reportParams.Add("ClientId", m_clientId.ToString());
                //}

                //Get the Client Name if necessary
                if (string.IsNullOrEmpty(this.ClientName))
                {
                    Facade.IOrganisation facOrg = new Facade.Organisation();
                    Entities.Organisation enOrg = new Entities.Organisation();
                    enOrg = facOrg.GetForIdentityId(this.ClientId);
                    this.ClientName = enOrg.OrganisationName.ToString();
                }

                //Add the Client Id and Name to the report params
                reportParams.Add("ClientId", this.ClientId.ToString());
                reportParams.Add("Client", this.ClientName);
       		}
		
			// Date Range
			if (this.StartDate != DateTime.MinValue)
				reportParams.Add("startDate", this.StartDate.ToString("dd/MM/yy"));
			
			if (this.EndDate != DateTime.MinValue)
				reportParams.Add("endDate", this.EndDate.ToString("dd/MM/yy"));
			
			// Invoice Id
			if (lblInvoiceNo.Text != "To Be Issued ... (This invoice has not yet been saved, add invoice to allocate Invoice No.)")
				reportParams.Add("invoiceId", lblInvoiceNo.Text);
			else
				reportParams.Add("invoiceId", "0");
				
			// Posted To Accounts
			if (chkPostToExchequer.Checked)
				reportParams.Add("Accounts", "true");

            // Invoice Date
            reportParams.Add("InvoiceDate", dteInvoiceDate.SelectedDate.Value.ToShortDateString());

			// VAT Rate 
            int vatNo = 0;
            decimal vatRate = 0.00M;

            facInv.GetVatRateForVatType(eVATType.Standard, dteInvoiceDate.SelectedDate.Value, out vatNo, out vatRate);
			reportParams.Add("VATrate", vatRate.ToString());

			//-------------------------------------------------------------------------------------	
			//									Load Report Section 
			//-------------------------------------------------------------------------------------	
            Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.Invoice;
            Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsInv;
            Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = rdoSortType.SelectedItem.Text.Replace(" ","") ;
            Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";
            Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;
						
			// Set E-mail and Fax Fields to the defaults of the client
			reportViewer.IdentityId = this.ClientId;

			// Show the user control
			reportViewer.Visible = true;


			// Show the add invoice button. 
			if (!chkPostToExchequer.Checked)
				btnAdd.Visible = true;

			dgExtras.Columns[5].Visible = lblInvoiceType.Text == eInvoiceType.SelfBill.ToString() && btnAdd.Visible;

			// Deleted
			if (chkDelete.Checked)
				Response.Redirect("../default.aspx");
		}

		private void chkOverride_CheckedChanged(object sender, System.EventArgs e)
		{
			if (chkOverride.Checked == true)
			{
				decimal vatAmount = 0;
				decimal totalAmountGross = 0;

				//m_jobIdCSV = (string) ViewState[C_JOBIDCSV_VS];

				Facade.IInvoice facInvoice = new Facade.Invoice();

                txtOverrideNetAmount.Text= facInvoice.GetTotalAmountForJobIds(this.JobIdCSV).ToString("C"); //; txtAmountNet.Text;
				
				vatAmount = Decimal.Parse(txtOverrideNetAmount.Text, NumberStyles.Currency)	* (17.50M / 100M);

				txtOverrideVAT.Text = vatAmount.ToString("C");
				
				totalAmountGross = Decimal.Parse(txtOverrideNetAmount.Text, NumberStyles.Currency)+ vatAmount;

				txtOverrideGrossAmount.Text = totalAmountGross.ToString("C");

				pnlOverride.Visible = true;
			}
			else
			{
				pnlOverride.Visible = false;
			}
		}

		private void btnViewInvoice_Click(object sender, System.EventArgs e)
		{
			btnViewInvoice.DisableServerSideValidation();
			
			if (Page.IsValid)
			{
				//m_extraIdCSV = (string) ViewState[C_EXTRAIDCSV_VS];

				//m_jobIdCSV = (string) ViewState[C_JOBIDCSV_VS];

				// Group By if none just load the invoice 
                //if ((eInvoiceGrouping)Enum.Parse(typeof(eInvoiceGrouping), rdoGroupBy.SelectedValue) == eInvoiceGrouping.None)                
                //{
					btnViewInvoice.Visible = true;
					LoadReport();	
                //}
                //else
                //{
                //    btnViewInvoice.Visible = false;
				
                //    LoadGroupGrid(m_jobIdCSV);
                //}
			}
		}
		
		private void chkSelfBillRemainder_CheckedChanged(object sender, System.EventArgs e)
		{
			if (chkSelfBillRemainder.Checked)
				pnlSelfRemainder.Visible = true;
			else
				pnlSelfRemainder.Visible = false;
		}
		
		private void rdoSortType_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (reportViewer.Visible)
			{
                Page.Validate();

                btnViewInvoice.DisableServerSideValidation();

				if (Page.IsValid)
				{			
					//m_jobIdCSV = (string) ViewState[C_JOBIDCSV_VS];

					// Group By if none just load the invoice 
					if ((eInvoiceGrouping)Enum.Parse(typeof(eInvoiceGrouping), rdoGroupBy.SelectedValue) == eInvoiceGrouping.None)                
					{
						btnViewInvoice.Visible = true;
						LoadReport();	
					}
					else
					{
						btnViewInvoice.Visible = false;
				
						LoadGroupGrid(this.JobIdCSV);
					}
				}
			}
		}	
	
		private void chkJobDetails_CheckedChanged(object sender, EventArgs e)
		{
			if (btnAdd.Visible)
			{
				btnAdd.Visible = false;
				reportViewer.Visible = false;
			}

			if (chkJobDetails.Checked)
			{
                pnlSort.Visible = true;
				chkIncludeDemurrage.Visible = true;
				lblAcceptedDemurrage.Visible = true;

				if (lblNoDemurrage.Visible != true) 
				{
					if (chkIncludeDemurrage.Checked)
						rdoDemurrageType.Visible = true;
					else
						rdoDemurrageType.Visible = false;
				}
				else
				{
					lblAcceptedDemurrage.Visible = false;
					chkIncludeDemurrage.Visible = false;
					rdoDemurrageType.Visible = false; 
				}
			}
			else
			{
                pnlSort.Visible = false;
				lblAcceptedDemurrage.Visible = false;
				chkIncludeDemurrage.Visible = false;
				rdoDemurrageType.Visible = false; 
			}
		}

		private void btnSendToAccounts_Click(object sender, System.EventArgs e)
		{
			btnSendToAccounts.DisableServerSideValidation();

			if (Page.IsValid)
			{ 


                //The report itself updates the invoice totals including the nominal code sub totals
                //These will be required if the ionvoice is immediately posted
                Facade.IInvoice facInvoice = new Facade.Invoice();
                this.Invoice = facInvoice.GetForInvoiceId(Convert.ToInt32(lblInvoiceNo.Text));

                bool retVal = false;
                chkPostToExchequer.Checked = true;
                this.Invoice.Posted = true;

                string applicationExceptionErrorMessage = string.Empty;

                try
                {
                    //Updating the invoice with Posted=true causes it to get Posted to Accounts
                    retVal = UpdateInvoice();
                }
                catch (ApplicationException aex)
                {
                    if (aex.InnerException.Message.ToLower().Contains("client account") && aex.InnerException.Message.ToLower().Contains("does not exist"))
                        applicationExceptionErrorMessage = aex.InnerException.Message;
                    else
                        throw;
                }

				if (retVal) 
				{
					lblConfirmation.Text = "The Invoice has been posted to accounts.";
					chkPostToExchequer.Visible = true;
					chkPostToExchequer.Checked = true;
			
					// Disable all buttons now that the invoice has been posted
					btnAdd.Enabled = false;
					btnSendToAccounts.Enabled = false;
				}
				else
				{
                    if (applicationExceptionErrorMessage != string.Empty)
                        lblConfirmation.Text = applicationExceptionErrorMessage;
                    else
					    lblConfirmation.Text = "The Invoice has not been posted to accounts.";
					
                    chkPostToExchequer.Checked = false;
				}
			
				LoadReport();
			
				lblConfirmation.Visible=true;
			}
		}


		#region Self Bill Invoice
		//		private void SelfBillJobCreate()
		//		{
		//			Facade.IJob facJob = new Facade.Job();
		//
		//			string userName = ((Entities.CustomPrincipal)Page.User).UserName;
		//			
		//			Entities.Job job = new Orchestrator.Entities.Job();
		//			job.Charge = new Orchestrator.Entities.JobCharge();
		//			
		//			// Job Type
		//			job.JobType = eJobType.SelfBillRemainder;
		//			
		//			// Job State
		//			if ((eSelfBillStatus) Enum.Parse(typeof(eSelfBillStatus), cboSelfBillStatus.SelectedValue) == eSelfBillStatus.Accepted)  
		//				job.JobState = eJobState.ReadyToInvoice;  
		//			else
		//				job.JobState = eJobState.Completed; // Once Accepted change job state to ReadyToInvoice
		//
		//			eSelfBillStatus selfBillJobState = (eSelfBillStatus) Enum.Parse(typeof(eSelfBillStatus), cboSelfBillStatus.SelectedValue); 
		//
		//			// Job Is Priced
		//			job.IsPriced = true;
		// 
		//			// Charge Amount & Type
		//			job.Charge.JobChargeAmount = Convert.ToDecimal(txtSelfBillAmount.Text);
		//			job.Charge.JobChargeType = eJobChargeType.Job;
		//
		//			string whom = txtBillName.Text;
		// 
		//			int invoiceId = Convert.ToInt32(lblInvoiceNo.Text);
		//			
		//			// Get the client details
		//			string clientName = string.Empty;
		//			int clientId = 0;
		//			
		//			if (m_isUpdate)
		//			{
		//				Facade.IInvoice facClient = new Facade.Invoice();
		//
		//				DataSet ds = facClient.GetClientForInvoiceId(Convert.ToInt32(lblInvoiceNo.Text));
		//
		//				clientName = Convert.ToString(ds.Tables[0].Rows[0]["Client"]);
		//				clientId = Convert.ToInt32(ds.Tables[0].Rows[0]["ClientId"]);
		//			}
		//			else
		//			{
		//				if (Convert.ToString (Session["ClientName"]) != "")
		//					clientName = Convert.ToString (Session["ClientName"]);
		//			
		//				if (Convert.ToString (Session["ClientId"]) !="")
		//					clientId = Convert.ToInt32(Session["ClientId"]);
		//			}
		//			
		//			job.IdentityId = clientId;
		//
		//			int jobId = facJob.Create(job, whom, invoiceId, selfBillJobState, userName);
		//            
		//			if (jobId != 0)
		//			{
		//				// Return back to this page with the job id link
		//				lblJobId.Text = "The self bill job that has been created successfully and has an Job Id of :" + jobId.ToString(); 
		//
		//				// Disable the fields but offer link if they want to change
		//				txtBillName.Enabled = false;
		//				cboSelfBillStatus.Enabled = false;
		//				txtSelfBillAmount.Enabled = false;
		//				btnAddSelfBill.Enabled = false; 
		//			
		//				// TODO: Maybe even offer them a link to this job 
		//			}
		//			else
		//			{
		//				lblJobId.Text = "There was a problem adding the Self Bill Job, please try again."; 
		//				lblJobId.Visible = true;
		//				lblJobId.ForeColor = Color.Red;
		//			}
		//		}
		
		private void btnAddSelfBill_Click(object sender, EventArgs e)
		{
			//			SelfBillJobCreate();
		}
		#endregion

		#region Checkboxes
		private void chkIncludeDemurrage_CheckedChanged(object sender, EventArgs e)
		{
			if (btnAdd.Visible)
			{
				btnAdd.Visible = false;
				reportViewer.Visible = false;
			}

			if (chkIncludeDemurrage.Checked)
				rdoDemurrageType.Visible = true;
			else
				rdoDemurrageType.Visible = false;
		}

		private void chkIncludePODs_CheckedChanged(object sender, EventArgs e)
		{
			if (btnAdd.Visible)
			{
				btnAdd.Visible = false;
				reportViewer.Visible = false;
			}
		}

		private void chkIncludeReferences_CheckedChanged(object sender, EventArgs e)
		{
			if (btnAdd.Visible)
			{
				btnAdd.Visible = false;
				reportViewer.Visible = false;
			}
		}

		private void chkIncludeFuelSurcharge_CheckedChanged(object sender, System.EventArgs e)
		{
			if (btnAdd.Visible)
			{
				btnAdd.Visible = false;
				reportViewer.Visible = false;
			}

			if (chkIncludeFuelSurcharge.Checked)
			{
				divFuelSurcharge.Visible = true;
				rdoFuelSurchargeType.Visible = true;
				txtFuelSurchargeRate.Visible = true;
			}
			else
			{
				divFuelSurcharge.Visible = false;
				rdoFuelSurchargeType.Visible = false;
				txtFuelSurchargeRate.Visible = false;
			}
			
			txtFuelSurchargeRate.Text = string.Empty;
		}

		private void txtFuelSurchargeRate_TextChanged(object sender, EventArgs e)
		{
			if (btnAdd.Visible)
			{
				btnAdd.Visible = false;
				reportViewer.Visible = false;
			}
		}
		#endregion
		
		#region Grouping 
		private void LoadGroupGrid(string jobIds)
		{
			Facade.IInvoice facInv = new Facade.Invoice();
			
			DataSet ds = null;
            			
			if ((eInvoiceGrouping)Enum.Parse(typeof(eInvoiceGrouping), rdoGroupBy.SelectedValue) == eInvoiceGrouping.Delivery)
				ds = facInv.GetPointIdForJobId(jobIds, (int)eInstructionType.Drop);
			else if((eInvoiceGrouping)Enum.Parse(typeof(eInvoiceGrouping), rdoGroupBy.SelectedValue) == eInvoiceGrouping.Collection)                
				ds = facInv.GetPointIdForJobId(jobIds, (int)eInstructionType.Load); 

			DataView dv  = new DataView();
			GroupPoint gSpot = new GroupPoint();
			gSpot = null;

			ArrayList arrgSpot = new ArrayList(); 
		
			for (int i = 0; i < ds.Tables[0].Rows.Count; i++)  
			{
				DataRow previous = null;

				if (i > 0)
					previous = ds.Tables[0].Rows[i-1];
				
				DataRow current = ds.Tables[0].Rows[i];
				
				if (i > 0)
				{
					// Check to see whether the old Point is not the same the new Point
					// If not just add new Point
					if (previous.Table.Rows[0]["PointId"].ToString() !=	current.Table.Rows[i]["PointId"].ToString())
					{
						// Save old Point To Array Collection
						arrgSpot.Add(gSpot);
 
						// New group point as PointId's are different						
						gSpot = new GroupPoint();

						gSpot.PointId = Convert.ToInt32(current.Table.Rows[i]["PointId"]);
						gSpot.PointName = current.Table.Rows[i]["PointName"].ToString();
						gSpot.FullAddress = current.Table.Rows[i]["FullAddress"].ToString();
						gSpot.OrganisationName = current.Table.Rows[i]["OrganisationName"].ToString();
						gSpot.InvoiceId = "To Be Issued";
						gSpot.CollectionPoint = current.Table.Rows[i]["CollectionPoint"].ToString();
						if (gSpot.JobIdList != String.Empty)
							gSpot.JobIdList += ",";

						gSpot.JobIdList += current.Table.Rows[i]["JobId"].ToString();

						// Need to add last row into Array Collection
						if (i == ds.Tables[0].Rows.Count - 1)
							arrgSpot.Add(gSpot);
					}
					else
					{
						// Just add JobId to old Point Class
						if (gSpot.JobIdList != String.Empty)
							gSpot.JobIdList += ",";

						gSpot.JobIdList += current.Table.Rows[i]["JobId"].ToString();

						// Need to add last row into Array Collection
						if (i == ds.Tables[0].Rows.Count - 1)
							arrgSpot.Add(gSpot);
					}
				}
				else
				{
					// The first Point to be dealt with in Dataset
					gSpot = new GroupPoint();

					gSpot.PointId = Convert.ToInt32(current.Table.Rows[i]["PointId"]);
					gSpot.PointName = current.Table.Rows[i]["PointName"].ToString();
					gSpot.FullAddress = current.Table.Rows[i]["FullAddress"].ToString();
					gSpot.OrganisationName = current.Table.Rows[i]["OrganisationName"].ToString();
					gSpot.InvoiceId = "To Be Issued";
					gSpot.CollectionPoint = current.Table.Rows[i]["CollectionPoint"].ToString();
						
					if (gSpot.JobIdList  != String.Empty)
						gSpot.JobIdList += ",";

					gSpot.JobIdList += current.Table.Rows[i]["JobId"].ToString();
				}
			}
			
			if (arrgSpot.Count == 0)
				btnViewInvoice.Visible = true; 

			// Apply the data to the grid
			// Grid Column - Point Name, List of Job Ids for that Point, link button to view that particular invoice
			dgGroup.DataSource = arrgSpot;	
			dgGroup.DataBind();
			dgGroup.Visible = true;

            this.GroupPointArray = arrgSpot;
		}

		protected void dgGroup_Page(Object sender, DataGridPageChangedEventArgs e) 
		{
			dgGroup.CurrentPageIndex = e.NewPageIndex;   

			dgGroup.DataBind();
		}

		private void rdoGroupBy_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			dgGroup.Visible = false;
			reportViewer.Visible = false;
			btnAdd.Visible = false;
			btnViewInvoice.Visible = true;
 
            //This will have already been done in SaveSessionParameters
            //m_arrJobId = (ArrayList) Session["JobIds"]; 
            //if (m_arrJobId != null)
            //{
            //    m_isUpdate = false;
				
            //    foreach (object item in m_arrJobId)
            //    {
            //        if (m_jobIdCSV  != String.Empty)
            //            m_jobIdCSV += ",";

            //        m_jobIdCSV  += Convert.ToInt32(item);
            //    }
            //    ViewState[C_JOBIDCSV_VS] = m_jobIdCSV;
            //}
		}
		
		private void dgGroup_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			switch (e.CommandName.ToLower())
			{
				case "view":
					// Get location of selected Invoice
					int index = e.Item.ItemIndex;

                    this.GridLocationId = index;

					// If not Updating an invoice then reset fields
					if (e.Item.Cells[0].Text == "To Be Issued")
					{	
						m_isUpdate = false; 
						
						lblConfirmation.Visible = false;
						
						lblInvoiceNo.Text = "To Be Issued ... (This invoice has not yet been saved, add invoice to allocate Invoice No.)";
						lblInvoiceNo.ForeColor = Color.Red;
						
						lblDateCreated.Text = "N/A";
						lblDateCreated.ForeColor = Color.Red;

						btnAdd.Text = "Add Invoice";

						chkPostToExchequer.Visible = false;
					}
					
					// Use the Hidden Field of JobIds
					string JobIdList = e.Item.Cells[3].Text; 
					
					// Load the current list of job id into viewstate;
                    if (JobIdList != string.Empty && JobIdList != "0")
                    {
                        this.JobIdCSV = JobIdList;
                        LoadReport();
                    }
                    else
                        this.JobIdCSV = string.Empty;

 
                    //string[] jobsToInvoice = null; 

                    //if (JobIdList != null && JobIdList.Length > 0)
                    //    jobsToInvoice = JobIdList.Split(',');

					// If no Job Id don't load the Report Viewer
                    //if (JobIdList != "0")
                    //{
                    //    // Load Invoice in Report Viewer
                    //    m_jobIdCSV = JobIdList;
					
                    //    LoadReport();
                    //}
					
					//ArrayList arrGroup = new ArrayList();

					//arrGroup = (ArrayList) ViewState[C_GROUP_POINT_ARRAY_VS];
					
					// Apply the new ArrayList to the DataGrid
					dgGroup.DataSource = this.GroupPointArray;
					dgGroup.DataBind();

					break;
			}
		}		
		
		private void UpdateGroupGrid(int InvoiceId)
		{
			if (rdoGroupBy.SelectedValue != "None")
			{
				// Using the Viewstate objects to populate the row with the updated details 
				int location = 0;
				ArrayList arrGroup = new ArrayList();
            
				location = this.GridLocationId;
				arrGroup = this.GroupPointArray;
 
				// Edit the row with the details
				// Update the row with the InvoiceId and turn the colour to black
				// Load the arraylist with updated details arrGroup.[location];
				GroupPoint gp = (GroupPoint) arrGroup[location];
				gp.InvoiceId = InvoiceId.ToString();
			
				// Apply the new ArrayList to the DataGrid
				dgGroup.DataSource = arrGroup;
				dgGroup.DataBind();
			
				//ViewState[C_GROUP_POINT_ARRAY_VS] = arrGroup;  
                this.GroupPointArray = arrGroup;

				rdoGroupBy.Enabled = false;
			}
			rdoGroupBy.Enabled = false;
		}	

		private void dgGroup_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				if (e.Item.Cells[0].Text != "To Be Issued")
				{
					e.Item.Cells[0].ForeColor = Color.Black;
					e.Item.Cells[6].Text = "Invoice Added";
				}
			}
		}
		#endregion
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
			this.Init += new System.EventHandler(this.addupdateinvoice_Init);

		}
		#endregion

        protected override PageStatePersister PageStatePersister
        {
            get
            {
                return new HiddenFieldPageStatePersister(this);
            }
        }
	}
}
