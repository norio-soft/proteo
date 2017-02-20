using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Linq;
using System.Collections.Generic;

namespace Orchestrator.WebUI.Organisation
{
	/// <summary>
	/// Summary description for addupdateclientscustomer.
	/// </summary>
	public partial class addupdateclientscustomer : Orchestrator.Base.BasePage
	{
        protected override PageStatePersister PageStatePersister
        {
            get
            {
                return new HiddenFieldPageStatePersister(this);
            }
        }
		#region Constants & Enums

		private const string C_ORGANISATION_VS = "Organisation";
		private const string C_IDENTITY_ID_VS = "IdentityId";
        private const string C_ORIGINALSTATUS_ID_VS = "IdentityId";

        #endregion

        #region Form Elements






        #endregion

        #region Page Variables

        private Entities.Organisation						m_organisation;
		protected string									m_organisationName = String.Empty;
		private eOrganisationType							m_organisationType = eOrganisationType.ClientCustomer;
		protected int										m_identityId = 0;
		private bool										m_isUpdate = false;
        private eIdentityStatus                             m_originalStatus = 0;

		#endregion

		#region Page Load/Init
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditOrganisations, eSystemPortion.GeneralUsage);
			btnAdd.Enabled = Orchestrator.WebUI.Security.Authorise.CanAccess(eSystemPortion.AddEditOrganisations);
			btnPromote.Enabled = Orchestrator.WebUI.Security.Authorise.CanAccess(eSystemPortion.AddEditOrganisations);
            pnlDeliveryNotification.Visible = Globals.Configuration.ClientCustomerDeliveryNotificationEnabled;

			// Setup
			ResetPageDisplay();
			RetrieveQueryStringParameters();
			SetWhereIAm();

			if (!IsPostBack)
			{
				if (m_isUpdate)
					LoadOrganisation();
			}
			else
			{
				if (m_isUpdate)
				{
					// Retrieve the organisation details from the viewstate
					m_organisation = (Entities.Organisation) ViewState[C_ORGANISATION_VS];
					m_organisationName = m_organisation.OrganisationName;
                    m_originalStatus = (eIdentityStatus)ViewState[C_ORIGINALSTATUS_ID_VS];
				}
			}

            if (m_originalStatus == eIdentityStatus.Unapproved)
                approveNote.Visible = true;
            else
                approveNote.Visible = false;
			
			infringementDisplay.Visible = true;
		}

		protected void addupdateorganisation_Init(object sender, EventArgs e)
		{
			btnPromote.Click += new EventHandler(btnPromote_Click);
		}

		#endregion 

		#region Mehtods & Functions
		private void ResetPageDisplay()
		{
			lblConfirmation.Visible = false;

            ddStatuses.Items.Clear();
            foreach (eIdentityStatus status in Enum.GetValues(typeof(eIdentityStatus)))
                ddStatuses.Items.Add(new ListItem(status.ToString(), ((int)status).ToString()));

		}

		private void SetWhereIAm()
		{
			//			if (m_relatedIdentityId != 0)
			//			{
			//				string links = "<a href=\"addupdatecientscustomer.aspx?identityId=" + Request["identityId"] + "\"> Return to " + Request["parentOrganisationName"] + "</a><br/>";
			//				lblWhereAmI.Text = links;
			//				lblWhereAmI.Visible = true;
			//			}
		}

		private void RetrieveQueryStringParameters()
		{
			// Get the identity id
			m_identityId = Convert.ToInt32(Request.QueryString["identityId"]);
			
			if (m_identityId == 0) // Look in the viewstate for the identity id - this will be set after an add (allows straight conversion into update mode)
				m_identityId = Convert.ToInt32(ViewState[C_IDENTITY_ID_VS]);

			if (m_identityId > 0)
				m_isUpdate = true;
		}
		#endregion

		#region Add/Update/Load/Populate Organisation
		private bool AddOrganisation()
		{
			int identityId = 0;
			bool success = false;
			Entities.FacadeResult retVal = new Entities.FacadeResult(0);

			Facade.IOrganisation facOrganisation = new Facade.Organisation();
			string userId = ((Entities.CustomPrincipal) Page.User).UserName;

			retVal = facOrganisation.Create(m_organisation, userId);

			if (!retVal.Success)
			{
				infringementDisplay.Infringements = retVal.Infringements;
				infringementDisplay.DisplayInfringments();
				infringementDisplay.Visible = true;

				lblConfirmation.Text = "There was an error adding the clients customer, please try again.";
				lblConfirmation.Visible = true;
				lblConfirmation.ForeColor = Color.Red;
				success = false;
			}
			else
			{
				identityId = retVal.ObjectId;
				success = true;
			}

            Repositories.DTOs.GeofenceNotificationSettings settings = new Repositories.DTOs.GeofenceNotificationSettings();
            settings.IdentityId = m_organisation.IdentityId;
            settings.Enabled = chkBoxEnableNotification.Checked;
            if (chkBoxEnableNotification.Checked)
            {
                settings.NotificationType = Convert.ToInt32(cbNotificationType.SelectedValue);
                settings.NotifyWhen = Convert.ToInt32(cbNotifyWhen.SelectedValue);
                settings.Recipient = txtContactDetail.Text;
            }
            facOrganisation.AddOrUpdate(settings, userId);

			return success;
		}

		private void LoadOrganisation()
		{
			// Retrieve the organisation and place it in viewstate
			Facade.IOrganisation facOrganisation = new Facade.Organisation();
			m_organisation = facOrganisation.GetForIdentityId(m_identityId);
			m_organisationName = m_organisation.OrganisationName;
			ViewState[C_ORGANISATION_VS] = m_organisation;
		
			txtOrganisationName.Text = m_organisation.OrganisationName;

            ddStatuses.SelectedValue = ((int)m_organisation.IdentityStatus).ToString();
            m_originalStatus = m_organisation.IdentityStatus;
            ViewState[C_ORIGINALSTATUS_ID_VS] = m_originalStatus;

			// Set the update buttons and labels
			btnAddLocation.Enabled = true;
		    btnAdd.Text = "Update";
			
			PopulateLocations();

            Repositories.DTOs.GeofenceNotificationSettings settings = facOrganisation.GetSettings(m_identityId);
            if (settings != null)
            {
                cbNotifyWhen_ItemsRequested(cbNotifyWhen, null);

                chkBoxEnableNotification.Checked = settings.Enabled;
                cbNotificationType.SelectedValue = settings.NotificationType.ToString();
                cbNotifyWhen.SelectedValue = settings.NotifyWhen.ToString();
                txtContactDetail.Text = settings.Recipient;
            }
		}

		private void PopulateOrganisation()
		{
			if (m_isUpdate)
			{
				// Recover from viewstate
				m_organisation = (Entities.Organisation) ViewState[C_ORGANISATION_VS];
			}
			else
			{
				// Create a new organisation
				m_organisation = new Entities.Organisation();
			}

			//--------------------------------------------------------------------------
			// 1. Populate the Organisation
			//--------------------------------------------------------------------------
			m_organisation.OrganisationName = txtOrganisationName.Text;
            m_organisation.OrganisationDisplayName = m_organisation.OrganisationName;
			m_organisation.OrganisationType = m_organisationType;

            m_organisation.IdentityStatus = (eIdentityStatus)Int32.Parse(ddStatuses.SelectedValue);

			//if (chkSuspended.Checked)
			//	m_organisation.IdentityStatus = eIdentityStatus.Deleted;
			//else
			//	m_organisation.IdentityStatus = eIdentityStatus.Active;
		}

		private bool UpdateOrganisation()
		{
			Facade.IOrganisation facOrganisaton = new Facade.Organisation();
			string userId = ((Entities.CustomPrincipal) Page.User).UserName;

            if(m_organisation.IdentityStatus == eIdentityStatus.Active && m_originalStatus == eIdentityStatus.Unapproved)
            {
                // Approve all the Points for this Client Customer too.
                Facade.IPoint facPoint = new Facade.Point();
                bool success = facPoint.ApproveAllPointsForOganisation(m_organisation.IdentityId);
            }


            Entities.FacadeResult retVal = facOrganisaton.Update(m_organisation, userId);

			if (!retVal.Success)
			{
				infringementDisplay.Infringements = retVal.Infringements;
				infringementDisplay.DisplayInfringments();
				infringementDisplay.Visible = true;
			}

            if (Globals.Configuration.ClientCustomerDeliveryNotificationEnabled)
            {
                Repositories.DTOs.GeofenceNotificationSettings settings = new Repositories.DTOs.GeofenceNotificationSettings();
                settings.IdentityId = m_organisation.IdentityId;
                settings.Enabled = chkBoxEnableNotification.Checked;

                if (chkBoxEnableNotification.Checked)
                {
                    settings.NotificationType = Convert.ToInt32(cbNotificationType.SelectedValue);
                    settings.NotifyWhen = Convert.ToInt32(cbNotifyWhen.SelectedValue);
                    settings.Recipient = txtContactDetail.Text;
                }

                facOrganisaton.AddOrUpdate(settings, userId);
            }

			return retVal.Success;
		}

		#endregion		

		#region Button Events
		protected void btnAdd_Click(object sender, EventArgs e)
		{
			bool success = false;

			if (Page.IsValid)
			{
				PopulateOrganisation();

				if (m_isUpdate)
					success = UpdateOrganisation();
				else
					success = AddOrganisation();

				if (m_isUpdate)
				{
					lblConfirmation.Text = "The Clients Customer has " + (success ? "" : "not ") + "been updated successfully.";

					if (success)
						LoadOrganisation();
				}
				else
				{
					lblConfirmation.Text = "The Clients Customer has " + (success ? "" : "not ") + "been Added successfully.";

					if (success)
					{
						lblConfirmation.ForeColor = Color.Blue;
						infringementDisplay.Visible = false;
						
						// Set viewstate objects
						ViewState[C_IDENTITY_ID_VS] = m_organisation.IdentityId;
						ViewState[C_ORGANISATION_VS] = m_organisation;
                        ViewState[C_ORIGINALSTATUS_ID_VS] = m_originalStatus;

						m_identityId = m_organisation.IdentityId;
						LoadOrganisation();
					}
				}

				lblConfirmation.Visible = true;
			}
		}

		protected void btnListClientCustomers_Click(object sender, EventArgs e)
		{
		    Response.Redirect("clientcustomers.aspx");
		}

		
		private void btnPromote_Click(object sender, EventArgs e)
		{
			if (m_identityId > 0)
			{
				using (Facade.IOrganisation facOrganisation = new Facade.Organisation())
				{
					if (facOrganisation.UpdateOrganisationType(m_identityId, eOrganisationType.Client, ((Entities.CustomPrincipal) Page.User).UserName))
						Response.Redirect("addupdateorganisation.aspx?identityId=" + m_identityId.ToString());
					else
						lblConfirmation.Text = "The client's customer was not promoted.";
				}
			}
			else
				lblConfirmation.Text = "You can not promote the client's customer until you have created it.";

			lblConfirmation.Visible = true;
		}

		protected void btnViewPoints_Click(object sender, EventArgs e)
		{
			Response.Redirect("../Point/listpoints.aspx?identityId=" + m_identityId.ToString());
		}

		#endregion

		#region Locations
		protected void btnAddLocation_Click(object sender, EventArgs e)
		{
			Response.Redirect("addupdateorganisationlocation.aspx?identityId=" + m_identityId + "&organisationName=" + m_organisationName);
		}

		private DataSet GetLocationData()
		{
			Facade.IOrganisation facOrganisation = new Facade.Organisation();
			return facOrganisation.GetLocationsForIdentityId(m_identityId);
		}

		private string LocationSortCriteria
		{
			get { return (string) ViewState["LocationSortCriteria"]; }
			set { ViewState["LocationSortCriteria"] = value; }
		}

		private string LocationSortDirection
		{
			get { return (string) ViewState["LocationSortDirection"]; }
			set { ViewState["LocationSortDirection"] = value; }
		}
		
		private void PopulateLocations()
		{
			PopulateLocations(LocationSortCriteria + " " + LocationSortDirection);
		}

		private void PopulateLocations(string sort)
		{
			DataView dvLocations = new DataView(GetLocationData().Tables[0]);
			dvLocations.Sort = sort.Trim();

			if (dvLocations.Table.Rows.Count == 0)
			{
			 dgLocations.Visible = false;
			}

			dgLocations.DataSource = dvLocations;
			dgLocations.DataBind();
		}

		protected void dgLocations_Page(Object sender, DataGridPageChangedEventArgs e) 
		{
			dgLocations.CurrentPageIndex = e.NewPageIndex;
			PopulateLocations();
		}

		protected void dgLocations_SortCommand(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{
			DataSet dsLocations = GetLocationData();
			DataView dvLocations = new DataView(dsLocations.Tables[0]);

			// Configure the sort
			if (e.SortExpression == LocationSortCriteria)
			{
				// switch direction
				if (LocationSortDirection == "desc")
					LocationSortDirection = "asc";
				else
					LocationSortDirection = "desc";
			}
			else
			{
				// new sort
				LocationSortCriteria = e.SortExpression;
				LocationSortDirection = "desc";
			}

			dvLocations.Sort = LocationSortCriteria + " " + LocationSortDirection;
			dgLocations.DataSource = dvLocations;
			dgLocations.DataBind();
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
			this.Init += new System.EventHandler(this.addupdateorganisation_Init);

		}
		#endregion
	
        protected void cbNotifyWhen_ItemsRequested(object sender, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cbNotifyWhen.Items.Clear();

            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<Repositories.IClientCustomerNotificationConditionTypesRepository>(uow);
                List<Models.ClientCustomerNotificationConditionTypes> conditionTypes = repo.GetAll().ToList();
		
                cbNotifyWhen.Items.AddRange((from conditionType in conditionTypes select new Telerik.Web.UI.RadComboBoxItem
                {
                    Text = conditionType.Description,
                    Value = conditionType.Id.ToString()
                }));
            }
        }
	}
}
