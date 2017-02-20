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

using Orchestrator.Globals;
using Orchestrator.WebUI.Security;
using P1TP.Components.Web.Validation;

namespace Orchestrator.WebUI.Job
{
	/// <summary>
	/// Summary description for addupdatepalletreturnjob.
	/// </summary>
	public partial class addupdatepalletreturnjob : Orchestrator.Base.BasePage
	{
		private const string C_JOB_VS = "C_JOB_VS";
		private const string C_COLLECTIONS_VS = "C_COLLECTIONS_VS";
		private const string C_COLLECTION_INDEX_VS = "C_COLLECTION_INDEX_VS";
		private const string C_DEHIRES_VS = "C_DEHIRES_VS";
		private const string C_DEHIRE_INDEX_VS = "C_DEHIRES_INDEX_VS";

		#region Form Elements

		// Collections

		// Dehires

		#endregion

		#region Page Variables

		private		int						m_jobId = 0;
		private		Entities.Job			m_job = null;

		private		Entities.InstructionCollection		m_collections = null;
		private		int									m_collectionIndex = -1;
		private		Entities.InstructionCollection		m_dehires = null;
		private		int									m_dehireIndex = -1;

		protected	int						m_collectIdentityId = 0;
		protected	int						m_collectTownId = 0;
		protected	int						m_collectPointId = 0;
		protected	int						m_dehireIdentityId = 0;
		protected	int						m_dehireLocationOwnerIdentityId = 0;
		protected	int						m_dehireTownId = 0;
		protected	int						m_dehirePointId = 0;

		private		bool					m_canEdit = false;

		#endregion

		#region Page Load/Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditJob, eSystemPortion.GeneralUsage);
			m_canEdit = Orchestrator.WebUI.Security.Authorise.CanAccess(eSystemPortion.AddEditJob);

			m_jobId = Convert.ToInt32(Request.QueryString["jobId"]);

            if (!IsPostBack)
			{
                if (Page.Request.QueryString["rcbID"] != null)
                {
                    // In order for the rad control box events to fire they must be visible.
                    pnlAddCollection.Visible = true;
                    //pnlAddCollection.Style.Add("display", "");
                    pnlAddDehire.Visible = true;
                }
                else
                {
                    if (m_jobId > 0)
                        LoadJob();
                    else
                    {
                        Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditJob);
                        m_collections = new Entities.InstructionCollection();
                        m_dehires = new Entities.InstructionCollection();
                    }

                    PopulatePage();
                    ViewState[C_COLLECTIONS_VS] = m_collections;
                    ViewState[C_DEHIRES_VS] = m_dehires;
                }
			}
			else
			{
				m_job = (Entities.Job) ViewState[C_JOB_VS];
				m_collections = (Entities.InstructionCollection) ViewState[C_COLLECTIONS_VS];
				m_collectionIndex = (int) ViewState[C_COLLECTION_INDEX_VS];
				m_dehires = (Entities.InstructionCollection) ViewState[C_DEHIRES_VS];
				m_dehireIndex = (int) ViewState[C_DEHIRE_INDEX_VS];
			}
		}

		protected void addupdatepalletreturnjob_Init(object sender, EventArgs e)
		{
            this.cboClient.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
            cfvClient.ServerValidate += new ServerValidateEventHandler(cfvClient_ServerValidate);

            this.cboCollectClient.ItemsRequested +=new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboCollectClient_ItemsRequested);
            this.cboCollectPoint.ItemsRequested +=new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboCollectPoint_ItemsRequested);
            this.cboCollectPoint.SelectedIndexChanged += new Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventHandler(cboCollectPoint_SelectedIndexChanged);

            this.cboDehireClient.ItemsRequested +=new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboDehireClient_ItemsRequested);
            this.cboDehireClientLocationOwner.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboDehireClientLocationOwner_ItemsRequested);
            this.cboDehirePoint.ItemsRequested +=new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboDehirePoint_ItemsRequested);

            this.cboPalletType.SelectedIndexChanged += new Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventHandler(cboPalletType_SelectedIndexChanged);

            cfvCollectDate.ServerValidate += new ServerValidateEventHandler(cfvCollectDate_ServerValidate);
			cfvCollectPallets.ServerValidate += new ServerValidateEventHandler(cfvCollectPallets_ServerValidate);
			btnCancelCollectionChanges.Click += new EventHandler(btnCancelCollectionChanges_Click);
			btnAddCollection.Click += new EventHandler(btnAddCollection_Click);
			repCollections.ItemDataBound += new RepeaterItemEventHandler(repCollections_ItemDataBound);
			repCollections.ItemCommand += new RepeaterCommandEventHandler(repCollections_ItemCommand);
			repCollections.PreRender += new EventHandler(repCollections_PreRender);
			btnAddNewCollection.Click += new EventHandler(btnAddNewCollection_Click);

			cfvDehireDate.ServerValidate += new ServerValidateEventHandler(cfvDehireDate_ServerValidate);
			cfvDehirePallets.ServerValidate += new ServerValidateEventHandler(cfvDehirePallets_ServerValidate);
			btnCancelDehireChanges.Click += new EventHandler(btnCancelDehireChanges_Click);
			btnAddDehire.Click += new EventHandler(btnAddDehire_Click);
			repDehires.ItemDataBound += new RepeaterItemEventHandler(repDehires_ItemDataBound);
			repDehires.ItemCommand += new RepeaterCommandEventHandler(repDehires_ItemCommand);
			repDehires.PreRender += new EventHandler(repDehires_PreRender);
			btnAddNewDehire.Click += new EventHandler(btnAddNewDehire_Click);

			btnViewDeHiringForm.Click += new EventHandler(btnViewDeHiringForm_Click);
			btnAddJob.Click += new EventHandler(btnAddJob_Click);

            this.cfvValidDateRange.ServerValidate +=new ServerValidateEventHandler(cfvValidDateRange_ServerValidate);

            cfvCollectClient.ServerValidate += new ServerValidateEventHandler(cfvCollectClient_ServerValidate);
            cfvCollectPoint.ServerValidate += new ServerValidateEventHandler(cfvCollectPoint_ServerValidate);
            cfvDehireClient.ServerValidate += new ServerValidateEventHandler(cfvDehireClient_ServerValidate);
            cfvDehirePallets.ServerValidate += new ServerValidateEventHandler(cfvDehirePallets_ServerValidate);
            cfvDehireClientLocationOwner.ServerValidate += new ServerValidateEventHandler(cfvDehireClientLocationOwner_ServerValidate);
		}

        void cfvClient_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(cboClient.SelectedValue, 1, true);
        }

        void cboClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            cboClient.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllOrganisationsFiltered(e.Text);

            int itemsPerRequest = 20;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            DataTable dt = ds.Tables[0];
            Telerik.Web.UI.RadComboBoxItem rcItem = null;
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new Telerik.Web.UI.RadComboBoxItem();
                rcItem.Text = dt.Rows[i]["OrganisationName"].ToString();
                rcItem.Value = dt.Rows[i]["IdentityId"].ToString();
                cboClient.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

        void cfvDehireClientLocationOwner_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(cboDehireClientLocationOwner.SelectedValue, 1, true);
        }

        void cfvDehireClient_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(cboDehireClient.SelectedValue, 1, true);
        }

        void cfvCollectPoint_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(cboCollectPoint.SelectedValue, 1, true);
        }

        void cfvCollectClient_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(cboCollectClient.SelectedValue, 1, true);
        }

		#endregion

        #region temp 
        /// <summary>
		/// Binds the instruction supplied to the relevant controls to enable the altering of the instruction.
		/// </summary>
		/// <param name="instruction">The instruction to alter.</param>
		private void AlterInstruction(Entities.Instruction instruction)
		{
			switch ((eInstructionType) instruction.InstructionTypeId)
			{
				case eInstructionType.PickupPallets:
					btnAddNewCollection.Enabled = false;
					btnCancelCollectionChanges.Visible = true;
					pnlAddCollection.Visible = true;
                    //pnlAddCollection.Style.Add("display", "");

					if (instruction.InstructionID == 0 && m_collectionIndex == -1)
						btnAddCollection.Text = "Add Collection";
					else
						btnAddCollection.Text = "Update Collection";

					lblCollectionInstructionId.Text = instruction.InstructionID.ToString();

					cboCollectClient.SelectedValue = instruction.Point.IdentityId.ToString();
					cboCollectClient.Text = instruction.Point.OrganisationName;
					m_collectIdentityId = instruction.Point.IdentityId;
					cboCollectClient.Enabled = instruction.InstructionID == 0 && m_collectionIndex == -1;

					m_collectTownId = instruction.Point.PostTown.TownId;

                    cboCollectPoint.SelectedValue = instruction.Point.PointId.ToString();
					cboCollectPoint.Text = instruction.Point.Description;
					m_collectPointId = instruction.Point.PointId;
					cboCollectPoint.Enabled = instruction.InstructionID == 0 && m_collectionIndex == -1;

                    dteCollectDate.SelectedDate = instruction.BookedDateTime;
					if (instruction.IsAnyTime)
                        dteCollectTime.Text = String.Empty;
					else
                        dteCollectTime.SelectedDate = instruction.BookedDateTime;

					// Get the pallet balance at this point
					using (Facade.IPalletBalance facPalletBalance = new Facade.Pallet())
						lblPalletsAvailable.Text = facPalletBalance.GetPalletBalanceAtPointId(instruction.Point.PointId, -1).ToString();

					txtCollectPallets.Text = instruction.TotalPallets.ToString();

					break;
				case eInstructionType.DeHirePallets:
					btnAddNewDehire.Enabled = false;
					btnCancelCollectionChanges.Visible = true;
					pnlAddDehire.Visible = true;
					if (instruction.InstructionID == 0 && m_dehireIdentityId == -1)
						btnAddDehire.Text = "Add Dehire";
					else
						btnAddDehire.Text = "Update Dehire";

					lblDehireInstructionId.Text = instruction.InstructionID.ToString();

					cboDehireClient.SelectedValue = instruction.ClientsCustomerIdentityID.ToString();
					using (Facade.IOrganisation facOrganisation = new Facade.Organisation())
						cboDehireClient.Text = facOrganisation.GetForIdentityId(instruction.ClientsCustomerIdentityID).OrganisationName;
					m_dehireIdentityId = instruction.ClientsCustomerIdentityID;
					cboDehireClient.Enabled = instruction.InstructionID == 0 && m_dehireIdentityId == -1;

                    cboDehireClientLocationOwner.SelectedValue = instruction.Point.IdentityId.ToString();
					cboDehireClientLocationOwner.Text = instruction.Point.OrganisationName;
					m_dehireLocationOwnerIdentityId = instruction.Point.IdentityId;
					cboDehireClientLocationOwner.Enabled = instruction.InstructionID == 0 && m_dehireIdentityId == -1;

					m_dehireTownId = instruction.Point.PostTown.TownId;

                    cboDehirePoint.SelectedValue = instruction.Point.PointId.ToString();
					cboDehirePoint.Text = instruction.Point.Description;
					m_dehirePointId = instruction.Point.PointId;
					cboDehirePoint.Enabled = instruction.InstructionID == 0 && m_dehireIdentityId == -1;

                    dteDehireDate.SelectedDate = instruction.BookedDateTime;
					if (instruction.IsAnyTime)
						dteDehireTime.Text = "String.Empty";
					else
                        dteDehireTime.SelectedDate = instruction.BookedDateTime;

					txtDehirePallets.Text = instruction.TotalPallets.ToString();

					break;
			}
		}

		private void LoadJob()
		{
			using (Facade.IJob facJob = new Facade.Job())
			{
                m_job = facJob.GetJob(m_jobId, true);
                m_job.Charge = (facJob as Facade.IJobCharge).GetForJobId(m_jobId);
			}

			m_collections = new Entities.InstructionCollection();
			m_dehires = new Entities.InstructionCollection();

		    foreach (Entities.Instruction instruction in m_job.Instructions)
			    if (instruction.InstructionTypeId == (int) eInstructionType.PickupPallets)
				    m_collections.Add(instruction);
			    else if (instruction.InstructionTypeId == (int) eInstructionType.DeHirePallets)
				    m_dehires.Add(instruction);


			ViewState[C_JOB_VS] = m_job;
		}

		/// <summary>
		/// Populate a pick up pallet instruction from the controls on the page.
		/// </summary>
		/// <returns>The instruction as specified by the user.</returns>
		private Entities.Instruction PopulateCollection()
		{
			int instructionId = Convert.ToInt32(lblCollectionInstructionId.Text);

			Entities.Instruction instruction = null;
			Entities.CollectDrop pickup = null;

			if (instructionId == 0)
			{
				instruction = new Entities.Instruction(eInstructionType.PickupPallets, m_jobId, String.Empty);
                instruction.PointID = Convert.ToInt32(cboCollectPoint.SelectedValue);
                Facade.IPoint facPoint = new Facade.Point();
				instruction.Point = facPoint.GetPointForPointId(instruction.PointID);
				instruction.CollectDrops = new Entities.CollectDropCollection();
				pickup = new Entities.CollectDrop();
				instruction.CollectDrops.Add(pickup);
			}
			else
				foreach (Entities.Instruction collector in m_collections)
					if (collector.InstructionID == instructionId)
					{
						instruction = collector;
						pickup = instruction.CollectDrops[0];
					}

			DateTime bookedDateTime = dteCollectDate.SelectedDate.Value;
			bookedDateTime = bookedDateTime.Subtract(bookedDateTime.TimeOfDay);
			bool isAnyTime = dteCollectTime.Text == "AnyTime";
			if (isAnyTime)
				bookedDateTime = bookedDateTime.Add(new TimeSpan(0, 23, 59, 59));
			else
				bookedDateTime = bookedDateTime.Add(dteCollectTime.SelectedDate.Value.TimeOfDay);

			instruction.BookedDateTime = bookedDateTime;
			instruction.IsAnyTime = isAnyTime;
			
			pickup.NoPallets = Convert.ToInt32(txtCollectPallets.Text);
			pickup.Docket = String.Empty;

            pickup.PalletType = cboPalletType.SelectedItem.Text;
            pickup.PalletTypeID = int.Parse(cboPalletType.SelectedItem.Value);

			return instruction;
		}

		/// <summary>
		/// Populate a dehire pallet instruction from the controls on the page.
		/// </summary>
		/// <returns>The instruction as specified by the user.</returns>
		private Entities.Instruction PopulateDehire()
		{
			int instructionId = Convert.ToInt32(lblDehireInstructionId.Text);

			Entities.Instruction instruction = null;
			Entities.CollectDrop dehire = null;

			if (instructionId == 0)
			{
				instruction = new Entities.Instruction(eInstructionType.DeHirePallets, m_jobId, String.Empty);
                instruction.ClientsCustomerIdentityID = Convert.ToInt32(cboDehireClient.SelectedValue);
                instruction.PointID = Convert.ToInt32(cboDehirePoint.SelectedValue);
                Facade.IPoint facPoint = new Facade.Point();
				instruction.Point = facPoint.GetPointForPointId(instruction.PointID);
			
				dehire = new Entities.CollectDrop();
				instruction.CollectDrops = new Entities.CollectDropCollection();
				instruction.CollectDrops.Add(dehire);
			}
			else
				foreach (Entities.Instruction dehirer in m_dehires)
					if (dehirer.InstructionID == instructionId)
					{
						instruction = dehirer;
						dehire = instruction.CollectDrops[0];
					}

			DateTime bookedDateTime = dteDehireDate.SelectedDate.Value;
			bookedDateTime = bookedDateTime.Subtract(bookedDateTime.TimeOfDay);
			bool isAnyTime = dteDehireTime.Text == "AnyTime";
			if (isAnyTime)
				bookedDateTime = bookedDateTime.Add(new TimeSpan(0, 23, 59, 59));
			else
				bookedDateTime = bookedDateTime.Add(dteDehireTime.SelectedDate.Value.TimeOfDay);

			instruction.BookedDateTime = bookedDateTime;
			instruction.IsAnyTime = isAnyTime;

			dehire.NoPallets = Convert.ToInt32(txtDehirePallets.Text);
			dehire.Docket = String.Empty;

            dehire.PalletTypeID = int.Parse(rcbDehirePalletType.SelectedValue);

			return instruction;
		}

		private Entities.Job PopulateJob()
		{
			Entities.Job job = new Entities.Job();

			job.Charge = new Entities.JobCharge();
			job.Charge.JobChargeType = eJobChargeType.Job;
            decimal chargeAmount = 0;
            if (decimal.TryParse(txtChargeAmount.Text, System.Globalization.NumberStyles.Currency, System.Threading.Thread.CurrentThread.CurrentCulture, out chargeAmount))
                job.Charge.JobChargeAmount = chargeAmount;
            else
                job.Charge.JobChargeAmount = 0;

            job.IdentityId = Convert.ToInt32(cboClient.SelectedValue);
			job.JobState = eJobState.Booked;
			job.JobType = eJobType.PalletReturn;
			job.LoadNumber = txtLoadNumber.Text;

            int businessTypeID = 0;
            int.TryParse(cboBusinessType.SelectedValue, out businessTypeID);
            job.BusinessTypeID = businessTypeID;

			job.Instructions = new Entities.InstructionCollection();
			foreach (Entities.Instruction collection in m_collections)
				job.Instructions.Add(collection);
			foreach (Entities.Instruction dehire in m_dehires)
				job.Instructions.Add(dehire);

			return job;
		}

		private void PopulatePage()
		{
			ResetCollections();
			ResetDehires();

            Facade.IBusinessType facBusinessType = new Facade.BusinessType();
            cboBusinessType.DataSource = facBusinessType.GetAll();
            cboBusinessType.DataBind();

			if (m_job == null)
			{
				repCollections.DataSource = m_collections;
				repCollections.DataBind();

				repDehires.DataSource = m_dehires;
				repDehires.DataBind();

				btnAddNewDehire.Enabled = m_canEdit &&  m_collections.Count > 0;
				btnAddJob.Enabled = m_canEdit &&  m_collections.Count > 0;
				btnViewDeHiringForm.Visible = false;
			}
			else
			{
				lblJobId.Text = m_job.JobId.ToString();
                Facade.IOrganisation facOrganisation = new Facade.Organisation();
                cboClient.Text = facOrganisation.GetForIdentityId(m_job.IdentityId).OrganisationName;
                cboClient.SelectedValue = m_job.IdentityId.ToString();
                cboClient.Enabled = false;
				txtLoadNumber.Text = m_job.LoadNumber;
                txtChargeAmount.Text = m_job.Charge.JobChargeAmount.ToString("C");

				m_collections = new Entities.InstructionCollection();
				m_dehires = new Entities.InstructionCollection();

                cboBusinessType.ClearSelection();
                if (cboBusinessType.Items.FindByValue(m_job.BusinessTypeID.ToString()) != null)
                    cboBusinessType.Items.FindByValue(m_job.BusinessTypeID.ToString()).Selected = true;
                cboBusinessType.Enabled = false;

                foreach (Entities.Instruction instruction in m_job.Instructions)
                {
					if (instruction.InstructionTypeId == (int) eInstructionType.PickupPallets)
						m_collections.Add(instruction);
					else if (instruction.InstructionTypeId == (int) eInstructionType.DeHirePallets)
						m_dehires.Add(instruction);
				}

				repCollections.DataSource = m_collections;
				repCollections.DataBind();

				repDehires.DataSource = m_dehires;
				repDehires.DataBind();

                btnAddJob.Text = "Update";
                if (m_job.JobState == eJobState.Invoiced || m_job.JobState == eJobState.Cancelled)
                    btnAddJob.Visible = false;
				btnViewDeHiringForm.Visible = true;
			}
		}

		private void ResetCollections()
		{
            pnlAddCollection.Visible = false;
            //pnlAddCollection.Style.Add("display", "none");
			btnAddCollection.Text = "Add Collection";

			m_collectIdentityId = 0;
			cboCollectClient.Text = "";
            cboCollectClient.SelectedValue = "";
			cboCollectClient.Enabled = true;

			lblCollectionInstructionId.Text = "0";

			m_collectTownId = 0;
			
			m_collectPointId = 0;
			cboCollectPoint.Text = "";
            cboCollectPoint.SelectedValue = "";
			
			cboCollectPoint.Enabled = true;

			dteCollectDate.Text = "";
			dteCollectTime.Text = "";

			lblPalletsAvailable.Text = "";
			txtCollectPallets.Text = "";

			btnCancelCollectionChanges.Visible = false;
			m_collectionIndex = -1;
			ViewState[C_COLLECTION_INDEX_VS] = m_collectionIndex;

			bool canAddNewCollection = false;
			if (m_job == null)
				canAddNewCollection = true;
			else
			{
				if (m_job.JobState == eJobState.Booked || m_job.JobState == eJobState.Planned || m_job.JobState == eJobState.InProgress)
					canAddNewCollection = true;
				if (canAddNewCollection)
					foreach (Entities.Instruction dehire in m_dehires)
						if (dehire.InstructionActuals != null && dehire.InstructionActuals.Count > 0)
							canAddNewCollection = false;
			}

			btnAddNewCollection.Enabled = canAddNewCollection;
		}

		private void ResetDehires()
		{
			pnlAddDehire.Visible = false;
			btnAddDehire.Text = "Add Dehire";

			m_dehireIdentityId = 0;
			cboDehireClient.Text = "";
            cboDehireClient.SelectedValue = "";
			cboDehireClient.Enabled = true;

			lblDehireInstructionId.Text = "0";

			m_dehireLocationOwnerIdentityId = 0;
			cboDehireClientLocationOwner.Text = "";
            cboDehireClientLocationOwner.SelectedValue = "";
			cboDehireClientLocationOwner.Enabled = true;

			m_dehireTownId = 0;

			m_dehirePointId = 0;
			cboDehirePoint.Text = "";
            cboDehirePoint.SelectedValue = "";
			
			cboDehirePoint.Enabled = true;

			dteDehireDate.Text = "";
			dteDehireTime.Text = "";

			txtDehirePallets.Text = "";

			btnCancelDehireChanges.Visible = false;
			m_dehireIndex = -1;
			ViewState[C_DEHIRE_INDEX_VS] = m_dehireIndex;

			btnAddNewDehire.Enabled = true;
        }
        #endregion

        #region DBCombo's Server Methods and Initialisation

         void cboCollectClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
         {
             Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
             Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();

             DataSet ds = facRefData.GetAllOrganisationsFiltered(e.Text);
             DataTable dt = ds.Tables[0];
             DataTable boundResults = dt.Clone();

             int itemsPerRequest = 20;
             int itemOffset = e.NumberOfItems;
             int endOffset = itemOffset + itemsPerRequest;
             if (endOffset > dt.Rows.Count)
                 endOffset = dt.Rows.Count;

             for (int i = itemOffset; i < endOffset; i++)
                 boundResults.ImportRow(dt.Rows[i]);

             cboCollectClient.Items.Clear();
             cboCollectClient.DataSource = boundResults;
             cboCollectClient.DataBind();

             if (dt.Rows.Count > 0)
                 e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
         }

        void cboDehireClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();

            DataSet ds = facRefData.GetAllClientsFiltered(e.Text);
            DataTable dt = ds.Tables[0];
            DataTable boundResults = dt.Clone();

            int itemsPerRequest = 20;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > dt.Rows.Count)
                endOffset = dt.Rows.Count;

            for (int i = itemOffset; i < endOffset; i++)
                boundResults.ImportRow(dt.Rows[i]);

            cboDehireClient.Items.Clear();
            cboDehireClient.DataSource = boundResults;
            cboDehireClient.DataBind();

            if (dt.Rows.Count > 0)
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
        }

        void cboCollectPoint_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            
            int identityId = 0;
            string searchText = "";
            if (e.Context["FilterString"] != null && e.Context["FilterString"].ToString()  != "")
            {
                string[] values = e.Context["FilterString"].ToString().Split(';');
                try { identityId = int.Parse(values[0]); }
                catch { }
                if (values.Length > 1 && values[1] != "false" && !string.IsNullOrEmpty(values[1]))
                {
                    searchText = values[1];
                }
                else if (!string.IsNullOrEmpty(e.Text))
                    searchText = e.Text;
            }
            else
                searchText = e.Context["FilterString"].ToString();

            Orchestrator.Facade.IPoint facPoint = new Orchestrator.Facade.Point();

            DataSet ds = facPoint.GetAllForOrganisation(identityId, ePointType.Any, 0, searchText);
            DataTable dt = ds.Tables[0];
            DataTable boundResults = dt.Clone();
            
            System.Diagnostics.Debug.Write(e.Value + " " + e.Text);

            int itemsPerRequest = 20;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > dt.Rows.Count)
                endOffset = dt.Rows.Count;

            for (int i = itemOffset; i < endOffset; i++)
                boundResults.ImportRow(dt.Rows[i]);

            cboCollectPoint.Items.Clear();
            cboCollectPoint.DataSource = boundResults;
            cboCollectPoint.DataBind();

            if (dt.Rows.Count > 0)
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
        }

        void cboDehireClientLocationOwner_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();

            DataSet ds = facRefData.GetAllOrganisationsFiltered(e.Text);
            DataTable dt = ds.Tables[0];
            DataTable boundResults = dt.Clone();

            int itemsPerRequest = 20;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > dt.Rows.Count)
                endOffset = dt.Rows.Count;

            for (int i = itemOffset; i < endOffset; i++)
                boundResults.ImportRow(dt.Rows[i]);

            cboDehireClientLocationOwner.Items.Clear();
            cboDehireClientLocationOwner.DataSource = boundResults;
            cboDehireClientLocationOwner.DataBind();

            if (dt.Rows.Count > 0)
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
        }

        void cboDehirePoint_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            int identityId = 0;
            string searchText = "";
            if (e.Context["FilterString"] != null && e.Context["FilterString"].ToString()  != "")
            {
                string[] values = e.Context["FilterString"].ToString().Split(';');
                try { identityId = int.Parse(values[0]); }
                catch { }
                if (values.Length > 1 && values[1] != "false" && !string.IsNullOrEmpty(values[1]))
                {
                    searchText = values[1];
                }
                else if (!string.IsNullOrEmpty(e.Text))
                    searchText = e.Text;
            }
            else
                searchText = e.Context["FilterString"].ToString();

            Orchestrator.Facade.IPoint facPoint = new Orchestrator.Facade.Point();
            
            DataSet ds = facPoint.GetAllForOrganisation(identityId, ePointType.Any, 0, searchText);
            DataTable dt = ds.Tables[0];
            DataTable boundResults = dt.Clone();

            System.Diagnostics.Debug.Write(e.Value + " " + e.Text);

            int itemsPerRequest = 20;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > dt.Rows.Count)
                endOffset = dt.Rows.Count;

            for (int i = itemOffset; i < endOffset; i++)
                boundResults.ImportRow(dt.Rows[i]);

            cboDehirePoint.Items.Clear();
            cboDehirePoint.DataSource = boundResults;
            cboDehirePoint.DataBind();

            if (dt.Rows.Count > 0)
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
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
			this.Init += new System.EventHandler(this.addupdatepalletreturnjob_Init);

		}
		#endregion

		#region Drop Down Events

        void cboCollectPoint_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
		{
            if (cboCollectPoint.SelectedValue != String.Empty)
			{
                using (Facade.IPalletBalance facPalletBalance = new Facade.Pallet())
                {
                    DataSet ds = facPalletBalance.GetPointBalances(int.Parse(cboCollectPoint.SelectedValue));

                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        cboPalletType.DataSource = ds.Tables[0];
                        cboPalletType.DataBind();
                    }

                    if(cboPalletType.Items.Count > 0)
                        cboPalletType.Items.Insert(0, new Telerik.Web.UI.RadComboBoxItem("--Please select a Pallet Type--", "0"));
                    else
                        cboPalletType.Items.Insert(0, new Telerik.Web.UI.RadComboBoxItem("--No Pallet Types Available--", "0"));
                }
			}
		}

        void cboPalletType_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            // Get the pallet balance at this point
            using (Facade.IPalletBalance facPalletBalance = new Facade.Pallet())
                lblPalletsAvailable.Text = facPalletBalance.GetPalletBalanceAtPointId(int.Parse(cboCollectPoint.SelectedValue), int.Parse(cboPalletType.SelectedValue)).ToString();
        }

		#endregion

		#region Validation
        void cfvValidDateRange_ServerValidate(object source, ServerValidateEventArgs args)
        {
            DateTime val = DateTime.Parse(args.Value);
            DateTime minDate = DateTime.Today.AddDays(-1);
            DateTime maxDate = DateTime.Today.AddDays(3);
            if (this.ViewState["_ignorecfvValidDateRange"] != null && (((DateTime)this.ViewState["_ignorecfvValidDateRange_Value"] == val) && (bool)this.ViewState["_ignorecfvValidDateRange"]))
                args.IsValid = true;
            else
            {
                args.IsValid = (val >= minDate && val <= maxDate);
                this.ViewState["_ignorecfvValidDateRange_Value"] = val;
                this.ViewState["_ignorecfvValidDateRange"] = true;
            }
        }

		/// <summary>
		/// Validates that the date for the collection is before the earliest dehire, but after the last completed collection.
		/// </summary>
		private void cfvCollectDate_ServerValidate(object source, ServerValidateEventArgs args)
		{
			// 1. Get the collection date and time entered
			DateTime collectionDateTime = dteCollectDate.SelectedDate.Value;
			collectionDateTime = collectionDateTime.Subtract(collectionDateTime.TimeOfDay);
			bool isAnyTime = dteCollectTime.Text == "AnyTime";
			if (isAnyTime)
				collectionDateTime = collectionDateTime.Add(new TimeSpan(0, 23, 59, 59));
			else
				collectionDateTime = collectionDateTime.Add(dteCollectTime.SelectedDate.Value.TimeOfDay);

			// 2. Get the earliest dehire date time
			DateTime earliestDehire = DateTime.MaxValue;
			foreach (Entities.Instruction dehire in m_dehires)
				if (dehire.BookedDateTime < earliestDehire)
					earliestDehire = dehire.BookedDateTime;

			if (m_job == null)
			{
				// We are creating a new job.

				// 3. The date is valid if the collection date time is less than the earliest de hire datetime, or if the collection is an anytime
				//    and the 00:00:00 version of the date time is before the earliest de hire datetime.
				if (collectionDateTime <= earliestDehire)
					args.IsValid = true;
				else
				{
					if (isAnyTime)
						collectionDateTime = collectionDateTime.Subtract(collectionDateTime.TimeOfDay);
					args.IsValid = collectionDateTime <= earliestDehire;
				}
			}
			else
			{
				// We are editing an existing job.

				// 3. Get the latest completed collection time.
				DateTime lastCompleteCollectionDateTime = DateTime.MinValue;
				foreach (Entities.Instruction collection in m_collections)
					if (collection.InstructionActuals != null && collection.InstructionActuals.Count > 0)
						if (collection.InstructionActuals[0].LeaveDateTime > lastCompleteCollectionDateTime)
							lastCompleteCollectionDateTime = collection.InstructionActuals[0].LeaveDateTime;

				// 4. The date is valid if the collection date time is greater than the last completed collection, and if it is earlier than the
				//    first dehire date time - the 00:00:00 applies to the dehire as before, but the collection is measure as 23:59:59 against the
				//    completed collection.
				bool valid = false;

				if (lastCompleteCollectionDateTime <= collectionDateTime)
					valid = true;

				if (valid)
				{
					if (collectionDateTime <= earliestDehire)
						valid = true;
					else
					{
						if (isAnyTime)
							collectionDateTime = collectionDateTime.Subtract(collectionDateTime.TimeOfDay);
						valid = collectionDateTime <= earliestDehire;
					}
				}

				args.IsValid = valid;
			}
		}

		/// <summary>
		/// Validates that the number of pallets being collected is a positive integer and does not exceed the number of pallets at the selected
		/// point.
		/// </summary>
		private void cfvCollectPallets_ServerValidate(object source, ServerValidateEventArgs args)
		{
			bool success = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(args.Value, 0, true);

            if (success)
                success = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(lblPalletsAvailable.Text, 1, true);

			if (success)
				success = Convert.ToInt32(args.Value) <= Convert.ToInt32(lblPalletsAvailable.Text);

			args.IsValid = success;
		}

		/// <summary>
		/// Validates that that the date for the dehire is after the last collection, and after the last completed collection.
		/// </summary>
		private void cfvDehireDate_ServerValidate(object source, ServerValidateEventArgs args)
		{
			// 1. Get the dehire date and time entered
			DateTime dehireDateTime = dteDehireDate.SelectedDate.Value;
			dehireDateTime = dehireDateTime.Subtract(dehireDateTime.TimeOfDay);
			bool isAnyTime = dteDehireTime.Text == "AnyTime";
			if (isAnyTime)
				dehireDateTime = dehireDateTime.Add(new TimeSpan(0, 23, 59, 59));
			else
				dehireDateTime = dehireDateTime.Add(dteDehireTime.SelectedDate.Value.TimeOfDay);

			// 2. Get the latest collection date time
			DateTime latestCollection = DateTime.MinValue;
			foreach (Entities.Instruction collection in m_collections)
			{
				if (collection.IsAnyTime)
				{
					DateTime bookedDateTime = collection.BookedDateTime.Subtract(collection.BookedDateTime.TimeOfDay);
					if (bookedDateTime > latestCollection)
						latestCollection = bookedDateTime;
				}
				else if (collection.BookedDateTime > latestCollection)
					latestCollection = collection.BookedDateTime;
			}

			if (m_job == null)
			{
				// We are creating a new job.

				// 3. The date is valid if the dehire date time is greater than the latest collection datetime.
				args.IsValid = dehireDateTime >= latestCollection;
			}
			else
			{
				// We are editing an existing job.

				// 3. Get the latest completed dehire time.
				DateTime lastCompleteDehireDateTime = DateTime.MinValue;
				foreach (Entities.Instruction dehire in m_dehires)
					if (dehire.InstructionActuals != null && dehire.InstructionActuals.Count > 0)
						if (dehire.InstructionActuals[0].LeaveDateTime > lastCompleteDehireDateTime)
							lastCompleteDehireDateTime = dehire.InstructionActuals[0].LeaveDateTime;

				// 4. The date is valid if the dehire date time is greater than the last completed dehire, and if it is later than the
				//    last collection.
				args.IsValid = dehireDateTime >= lastCompleteDehireDateTime && dehireDateTime >= latestCollection;
			}		
		}

		/// <summary>
		/// Validates that the number of pallets being dehires is a positive integer.
		/// </summary>
		private void cfvDehirePallets_ServerValidate(object source, ServerValidateEventArgs args)
		{
			args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(args.Value, 0, true);
		}

		#endregion

		#region Button Events

		private void btnAddCollection_Click(object sender, EventArgs e)
		{
			btnAddCollection.DisableServerSideValidation();

			if (Page.IsValid)
			{
				Entities.Instruction instruction = PopulateCollection();

				if (m_job == null)
				{
					if (m_collectionIndex == -1)
						m_collections.Add(instruction);
					else
					{
						m_collections.RemoveAt(m_collectionIndex);
						m_collections.Insert(m_collectionIndex, instruction);
					}
					PopulatePage();
				}
				else
				{
					Entities.FacadeResult result = null;
					Entities.CustomPrincipal user = (Entities.CustomPrincipal) Page.User;

					using (Facade.IJob facJob = new Facade.Job())
					{
						if (instruction.InstructionID == 0)
							result = facJob.AddInstruction(m_job, instruction, user.IdentityId, user.UserName);
						else
							result = facJob.UpdateInstruction(m_job, instruction, user.UserName);
					}

					if (result.Success)
					{
						LoadJob();
						PopulatePage();
					}
					else
					{
						infringementDisplay.Infringements = result.Infringements;
						infringementDisplay.DisplayInfringments();
					}
				}
			}
		}

		private void btnAddDehire_Click(object sender, EventArgs e)
		{
			btnAddDehire.DisableServerSideValidation();

			if (Page.IsValid)
			{
                infringementDisplay.Visible = false;

				Entities.Instruction instruction = PopulateDehire();

				if (m_job == null)
				{
					if (m_dehireIndex == -1)
						m_dehires.Add(instruction);
					else
					{
						m_dehires.RemoveAt(m_dehireIndex);
						m_dehires.Insert(m_dehireIdentityId, instruction);
					}
					PopulatePage();
				}
				else
				{
					Entities.FacadeResult result = null;
					Entities.CustomPrincipal user = (Entities.CustomPrincipal) Page.User;

                    if (instruction.TotalPallets != m_job.Instructions[0].TotalPallets)
                    {
                        Entities.FacadeResult res = new Orchestrator.Entities.FacadeResult(false);
                        res.Infringements.Add(new Orchestrator.Entities.BusinessRuleInfringement("Pallet Counts Do Not Match", "The total number of pallets being de-hired is greater than the number being collected."));
                        infringementDisplay.Infringements = res.Infringements;
                        infringementDisplay.DisplayInfringments();
                        return;
                    }
                    

					using (Facade.IJob facJob = new Facade.Job())
					{
						if (instruction.InstructionID == 0)
							result = facJob.AddInstruction(m_job, instruction, user.IdentityId, user.UserName);
						else
							result = facJob.UpdateInstruction(m_job, instruction, user.UserName);
					}

					if (result.Success)
					{
						LoadJob();
						PopulatePage();
					}
					else
					{
						infringementDisplay.Infringements = result.Infringements;
						infringementDisplay.DisplayInfringments();
					}
				}
			}	
		}

		private void btnAddJob_Click(object sender, EventArgs e)
		{
			if (Page.IsValid)
			{
                var userName = ((Entities.CustomPrincipal)Page.User).UserName;

                if (m_job == null)
                {
                    Entities.Job job = PopulateJob();

                    using (Facade.IJob facJob = new Facade.Job())
                        m_jobId = facJob.Create(job, userName);

                    if (m_jobId > 0)
                        Response.Redirect("addupdatepalletreturnjob.aspx?jobId=" + m_jobId.ToString());
                }
                else
                {
                    decimal chargeAmount = 0;
                    if (decimal.TryParse(txtChargeAmount.Text, System.Globalization.NumberStyles.Currency, System.Threading.Thread.CurrentThread.CurrentCulture, out chargeAmount))
                        m_job.Charge.JobChargeAmount = chargeAmount;
                    else
                        m_job.Charge.JobChargeAmount = 0;

                    // Update the load numbers and price.
                    Facade.IJob facJob = new Facade.Job();
                    facJob.UpdateLoadReferenceAndPrice(m_job.JobId, txtLoadNumber.Text, m_job.Charge, userName);
                }
			}
		}

		private void btnAddNewCollection_Click(object sender, EventArgs e)
		{
			btnAddNewCollection.Enabled = false;
			btnCancelCollectionChanges.Visible = true;
			pnlAddCollection.Visible = true;
            //pnlAddCollection.Style.Add("display", "");
		}

		private void btnAddNewDehire_Click(object sender, EventArgs e)
		{
			btnAddNewDehire.Enabled = false;
			btnCancelDehireChanges.Visible = true;
			pnlAddDehire.Visible = true;

            List<KeyValuePair<int, string>> existingPalletTypes = new List<KeyValuePair<int, string>>();

            foreach(Entities.Instruction ins in m_collections)
                foreach (Entities.CollectDrop cd in ins.CollectDrops)
                {
                    KeyValuePair<int, string> newPalletType = new KeyValuePair<int,string>(cd.PalletTypeID, cd.PalletType);

                    if (!existingPalletTypes.Exists(ept => ept.Key == newPalletType.Key))
                        existingPalletTypes.Add(newPalletType);
                }

            rcbDehirePalletType.DataSource = existingPalletTypes;
            rcbDehirePalletType.DataBind();

            rcbDehirePalletType.Items.Insert(0, new Telerik.Web.UI.RadComboBoxItem("--Please select a pallet type--", "0"));
		}

		private void btnCancelCollectionChanges_Click(object sender, EventArgs e)
		{
			ResetCollections();
			btnAddNewCollection.Enabled = true;
		}

		private void btnCancelDehireChanges_Click(object sender, EventArgs e)
		{
			ResetDehires();
			btnAddNewDehire.Enabled = true;
		}
		
		private void btnViewDeHiringForm_Click(object sender, EventArgs e)
		{
			// Can only display a pallet de-hiring form for a created job
			if (m_job != null)
			{
				// Get the data needed to run the report
				Facade.IJob facJob = new Facade.Job();
				DataSet ds = facJob.GetPalletDehiringData(m_jobId);

				Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.PalletDehiringForm;
				Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = ds;
				Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
				Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";

				NameValueCollection nvc = new NameValueCollection();
				nvc.Add("JobId", m_jobId.ToString());

				Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = nvc;

				reportViewer1.Visible = true;
			}
		}

		#endregion

		#region Repeater Events

		private void repCollections_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			Entities.CustomPrincipal user = (Entities.CustomPrincipal) Page.User;
			Entities.FacadeResult result = null;

			switch (e.CommandName.ToLower())
			{
				case "alter":
					if (m_job == null)
					{
						m_collectionIndex = e.Item.ItemIndex;
						ViewState[C_COLLECTION_INDEX_VS] = m_collectionIndex;
						AlterInstruction(m_collections[e.Item.ItemIndex]);
					}
					else
					{
						int thisInstructionId = Convert.ToInt32(((HtmlInputHidden) e.Item.FindControl("hidInstructionId")).Value);
						
						foreach (Entities.Instruction instruction in m_collections)
							if (instruction.InstructionID == thisInstructionId)
								AlterInstruction(instruction);
					}
					break;
				case "delete":
					if (m_job == null)
					{
						m_collections.RemoveAt(e.Item.ItemIndex);
						ViewState[C_COLLECTIONS_VS] = m_collections;
						PopulatePage();
					}
					else
						using (Facade.IJob facJob = new Facade.Job())
							result = facJob.RemoveInstruction(m_job, Convert.ToInt32(((HtmlInputHidden) e.Item.FindControl("hidInstructionId")).Value), user.UserName);
					break;
				case "down":
					if (m_job == null)
					{
						Entities.Instruction instruction = m_collections[e.Item.ItemIndex];
						m_collections.RemoveAt(e.Item.ItemIndex);
						m_collections.Insert(e.Item.ItemIndex + 1, instruction);
						ViewState[C_COLLECTIONS_VS] = m_collections;
						PopulatePage();
					}
					else
					{
						int thisInstructionId = Convert.ToInt32(((HtmlInputHidden) e.Item.FindControl("hidInstructionId")).Value);
						int nextInstructionId = Convert.ToInt32(((HtmlInputHidden) repCollections.Items[e.Item.ItemIndex + 1].FindControl("hidInstructionId")).Value);

						using (Facade.IJob facJob = new Facade.Job())
							result = facJob.UpdateSwitchActions(m_job, nextInstructionId, thisInstructionId, user.IdentityId, user.UserName);
					}
					break;
				case "up":
					if (m_job == null)
					{
						Entities.Instruction instruction = m_collections[e.Item.ItemIndex];
						m_collections.RemoveAt(e.Item.ItemIndex);
						m_collections.Insert(e.Item.ItemIndex - 1, instruction);
						ViewState[C_COLLECTIONS_VS] = m_collections;
						PopulatePage();
					}
					else
					{
						int thisInstructionId = Convert.ToInt32(((HtmlInputHidden) e.Item.FindControl("hidInstructionId")).Value);
						int previousInstructionId = Convert.ToInt32(((HtmlInputHidden) repCollections.Items[e.Item.ItemIndex - 1].FindControl("hidInstructionId")).Value);

						using (Facade.IJob facJob = new Facade.Job())
							result = facJob.UpdateSwitchActions(m_job, thisInstructionId, previousInstructionId, user.IdentityId, user.UserName);
					}
					break;
			}

			if (result != null)
			{
				if (result.Success)
				{
					LoadJob();
					PopulatePage();
				}
				else
				{
					infringementDisplay.Infringements = result.Infringements;
					infringementDisplay.DisplayInfringments();
				}
			}
		}

		private void repCollections_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				Entities.Instruction instruction = (Entities.Instruction) e.Item.DataItem;

				Label lblBookedDateTime = (Label) e.Item.FindControl("lblBookedDateTime");
				if (instruction.IsAnyTime)
					lblBookedDateTime.Text = instruction.BookedDateTime.ToString("dd/MM/yy") + " AnyTime";
				else
					lblBookedDateTime.Text = instruction.BookedDateTime.ToString("dd/MM/yy HH:mm");
			}
		}

		private void repCollections_PreRender(object sender, EventArgs e)
		{
			int itemIndex = 0;

			using (Facade.IInstructionActual facInstructionActual = new Facade.Instruction())
				foreach (RepeaterItem item in repCollections.Items)
					if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
					{
						itemIndex = item.ItemIndex;
						bool allowUp = false;
						bool allowDown = false;
						bool allowAlter = false;
						bool allowDelete = false;
						bool hasActual = false;

						if (m_canEdit)
						{
							int instructionId = 0;

							// See if there is an actual for this instruction (only possible if the job has previously been created).
							if (m_job != null)
							{
								HtmlInputHidden hidInstructionId = (HtmlInputHidden) item.FindControl("hidInstructionId");
								instructionId = Convert.ToInt32(hidInstructionId.Value);

								Entities.InstructionActual actual = facInstructionActual.GetEntityForInstructionId(instructionId);
								hasActual = actual != null;
							}
							else
								hasActual = false;

							// Instructions can be moved if the instruction and the move target have no actuals
							if (itemIndex > 0 && !hasActual)
							{
								int previousInstructionId = Convert.ToInt32(((HtmlInputHidden) repCollections.Items[itemIndex - 1].FindControl("hidInstructionId")).Value);
								bool previousHasActual = facInstructionActual.GetEntityForInstructionId(previousInstructionId) != null;

								if (!previousHasActual)
									allowUp = true;
							}

							if (itemIndex < repCollections.Items.Count - 1 && !hasActual)
								allowDown = true;

							// Instructions can be edited unless that instruction has an actual recorded.
							allowAlter = !hasActual;

							// Instructions can be deleted unless it is the only collection or has an actual recorded.
							if (itemIndex > 0 && !hasActual)
								allowDelete = true;
						}

						((Button) item.FindControl("btnUp")).Enabled = allowUp;
						((Button) item.FindControl("btnDown")).Enabled = allowDown;
						((Button) item.FindControl("btnAlter")).Enabled = allowAlter;
						((Button) item.FindControl("btnDelete")).Enabled = allowDelete;
					}
		}

		private void repDehires_ItemCommand(object source, RepeaterCommandEventArgs e)
		{
			Entities.CustomPrincipal user = (Entities.CustomPrincipal) Page.User;
			Entities.FacadeResult result = null;

			switch (e.CommandName.ToLower())
			{
				case "alter":
					if (m_job == null)
					{
						m_dehireIndex = e.Item.ItemIndex;
						ViewState[C_DEHIRE_INDEX_VS] = m_dehireIndex;
						AlterInstruction(m_dehires[e.Item.ItemIndex]);
					}
					else
					{
						int thisInstructionId = Convert.ToInt32(((HtmlInputHidden) e.Item.FindControl("hidInstructionId")).Value);
						
						foreach (Entities.Instruction instruction in m_dehires)
							if (instruction.InstructionID == thisInstructionId)
								AlterInstruction(instruction);
					}
					break;
				case "delete":
					if (m_job == null)
					{
						m_dehires.RemoveAt(e.Item.ItemIndex);
						ViewState[C_DEHIRES_VS] = m_dehires;
						PopulatePage();
					}
					else
						using (Facade.IJob facJob = new Facade.Job())
							result = facJob.RemoveInstruction(m_job, Convert.ToInt32(((HtmlInputHidden) e.Item.FindControl("hidInstructionId")).Value), user.UserName);
					break;
				case "down":
					if (m_job == null)
					{
						Entities.Instruction instruction = m_dehires[e.Item.ItemIndex];
						m_dehires.RemoveAt(e.Item.ItemIndex);
						m_dehires.Insert(e.Item.ItemIndex + 1, instruction);
						ViewState[C_DEHIRES_VS] = m_dehires;
						PopulatePage();
					}
					else
					{
						int thisInstructionId = Convert.ToInt32(((HtmlInputHidden) e.Item.FindControl("hidInstructionId")).Value);
						int nextInstructionId = Convert.ToInt32(((HtmlInputHidden) repDehires.Items[e.Item.ItemIndex + 1].FindControl("hidInstructionId")).Value);

						using (Facade.IJob facJob = new Facade.Job())
							result = facJob.UpdateSwitchActions(m_job, nextInstructionId, thisInstructionId, user.IdentityId, user.UserName);
					}
					break;
				case "up":
					if (m_job == null)
					{
						Entities.Instruction instruction = m_dehires[e.Item.ItemIndex];
						m_dehires.RemoveAt(e.Item.ItemIndex);
						m_dehires.Insert(e.Item.ItemIndex - 1, instruction);
						ViewState[C_DEHIRES_VS] = m_dehires;
						PopulatePage();
					}
					else
					{
						int thisInstructionId = Convert.ToInt32(((HtmlInputHidden) e.Item.FindControl("hidInstructionId")).Value);
						int previousInstructionId = Convert.ToInt32(((HtmlInputHidden) repDehires.Items[e.Item.ItemIndex - 1].FindControl("hidInstructionId")).Value);

						using (Facade.IJob facJob = new Facade.Job())
							result = facJob.UpdateSwitchActions(m_job, thisInstructionId, previousInstructionId, user.IdentityId, user.UserName);
					}
					break;			
			}

			if (result != null)
			{
				if (result.Success)
				{
					LoadJob();
					PopulatePage();
				}
				else
				{
					infringementDisplay.Infringements = result.Infringements;
					infringementDisplay.DisplayInfringments();
				}
			}
		}

		private void repDehires_ItemDataBound(object sender, RepeaterItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				Entities.Instruction instruction = (Entities.Instruction) e.Item.DataItem;

				using (Facade.IOrganisation facOrganisation = new Facade.Organisation())
				{
					Label lblDeHireOrganisationName = (Label) e.Item.FindControl("lblDeHireOrganisationName");
					lblDeHireOrganisationName.Text = facOrganisation.GetForIdentityId(instruction.ClientsCustomerIdentityID).OrganisationName;
				}

				Label lblBookedDateTime = (Label) e.Item.FindControl("lblBookedDateTime");
				if (instruction.IsAnyTime)
					lblBookedDateTime.Text = instruction.BookedDateTime.ToString("dd/MM/yy") + " AnyTime";
				else
					lblBookedDateTime.Text = instruction.BookedDateTime.ToString("dd/MM/yy HH:mm");
			}
		}

		private void repDehires_PreRender(object sender, EventArgs e)
		{
			int itemIndex = 0;

			using (Facade.IInstructionActual facInstructionActual = new Facade.Instruction())
				foreach (RepeaterItem item in repDehires.Items)
					if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
					{
						itemIndex = item.ItemIndex;
						bool allowUp = false;
						bool allowDown = false;
						bool allowAlter = false;
						bool allowDelete = false;
						bool hasActual = false;

						if (m_canEdit)
						{
							int instructionId = 0;

							// See if there is an actual for this instruction (only possible if the job has previously been created).
							if (m_job != null)
							{
								HtmlInputHidden hidInstructionId = (HtmlInputHidden) item.FindControl("hidInstructionId");
								instructionId = Convert.ToInt32(hidInstructionId.Value);

								Entities.InstructionActual actual = facInstructionActual.GetEntityForInstructionId(instructionId);
								hasActual = actual != null;
							}
							else
								hasActual = false;

							// Instructions can be moved if the instruction and the move target have no actuals
							if (itemIndex > 0 && !hasActual)
							{
								int previousInstructionId = Convert.ToInt32(((HtmlInputHidden) repDehires.Items[itemIndex - 1].FindControl("hidInstructionId")).Value);
								bool previousHasActual = facInstructionActual.GetEntityForInstructionId(previousInstructionId) != null;

								if (!previousHasActual)
									allowUp = true;
							}

							if (itemIndex < repDehires.Items.Count - 1 && !hasActual)
								allowDown = true;

							// Instructions can be edited unless that instruction has an actual recorded.
							allowAlter = !hasActual;

							// Instructions can be deleted if there is no actual recorded.
							allowDelete = !hasActual;
						}

						((Button) item.FindControl("btnUp")).Enabled = allowUp;
						((Button) item.FindControl("btnDown")).Enabled = allowDown;
						((Button) item.FindControl("btnAlter")).Enabled = allowAlter;
						((Button) item.FindControl("btnDelete")).Enabled = allowDelete;
					}		
		}

		#endregion
	}
}
