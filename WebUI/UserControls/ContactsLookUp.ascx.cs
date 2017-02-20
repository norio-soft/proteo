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
using Telerik.Web.UI;
using System.Collections.Generic;
using Orchestrator.Entities;

namespace Orchestrator.WebUI.UserControls
{
    public partial class ContactsLookUp : System.Web.UI.UserControl
    {
        #region Private members
        private Entities.Organisation organisationDetails = null;
        #endregion

        #region Public properties

        public bool IsClient
        {
            get
            {
                object isClientVS = ViewState["isClient"];
                bool isClient = true;

                if (isClientVS != null)
                    isClient = Convert.ToBoolean(isClientVS);

                return isClient;
            }
            set { ViewState["isClient"] = value; }
        }

        public bool IsCalledFromHotKey
        {
            get
            {
                object isCalledFromHotKeyVS = ViewState["isCalledFromHotKey"];
                bool isCalledFromHotKey = false;

                if (isCalledFromHotKeyVS != null)
                    isCalledFromHotKey = Convert.ToBoolean(isCalledFromHotKeyVS);

                return isCalledFromHotKey;
            }
            set { ViewState["isCalledFromHotKey"] = value; }
        }

        public int OrganisationId
        {
            get
            {
                object organisationIdVS = ViewState["organisationId"];
                int organisationId = 0;

                if (organisationIdVS != null)
                    organisationId = Convert.ToInt32(organisationIdVS);

                return organisationId;
            }
            set { ViewState["organisationId"] = value; }
        }

        public int IndividualContactId
        {
            get
            {
                object individualContactIdVS = ViewState["individualContactId"];
                int individualContactId = 0;

                if (individualContactIdVS != null)
                    individualContactId = Convert.ToInt32(individualContactIdVS);

                return individualContactId;
            }
            set { ViewState["individualContactId"] = value; }
        }

        public Entities.Organisation OrganisationDetails
        {
            get
            {
                if (this.OrganisationId > 0)
                {
                    object organisation = ViewState["organisationDetails"];

                    if (organisation != null)
                        if (((Entities.Organisation)organisation).IdentityId == this.OrganisationId)
                            organisationDetails = (Entities.Organisation)organisation;
                        else
                        {
                            Facade.Organisation org = new Orchestrator.Facade.Organisation();
                            organisationDetails = org.GetForIdentityId(this.OrganisationId);
                        }
                    else
                    {
                        Facade.Organisation org = new Orchestrator.Facade.Organisation();
                        organisationDetails = org.GetForIdentityId(this.OrganisationId);
                    }
                }

                return organisationDetails;
            }
            set
            {
                organisationDetails = value;
                ViewState["organisationDetails"] = organisationDetails;
            }
        }

        #endregion

        #region Form events

        protected void Page_Load(object sender, EventArgs e)
        {
            this.clientLabel.Text = "Select " + ((this.IsCalledFromHotKey) ? "client/sub contractor" : 
                (this.IsClient) ? "client" : "sub contractor");
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.cboClient.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
            this.cboClient.SelectedIndexChanged += new RadComboBoxSelectedIndexChangedEventHandler(cboClient_SelectedIndexChanged);
            this.cboIndividualContacts.SelectedIndexChanged += new RadComboBoxSelectedIndexChangedEventHandler(cboIndividualContacts_SelectedIndexChanged);
            this.cboIndividualContacts.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(cboIndividualContacts_ItemsRequested);
            this.grdContacts.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdContacts_NeedDataSource);
        }

        #endregion

        #region Control Events

        void cboClient_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(this.cboClient.SelectedValue))
            {
                this.OrganisationId = Convert.ToInt32(this.cboClient.SelectedValue);

                BusinessLogicLayer.Individual busIndividual = new Orchestrator.BusinessLogicLayer.Individual();
                List<Entities.Individual> individualContacts = busIndividual.GetForRelatedIdentityId(this.OrganisationId);

                this.cboIndividualContacts.Items.Clear();

                individualContacts = busIndividual.GetForRelatedIdentityId(this.OrganisationId);

                DataTable individualDataSource = this.GetIndividualTable(individualContacts);
                this.cboIndividualContacts.DataSource = individualDataSource;
                this.cboIndividualContacts.DataTextField = "FullName";
                this.cboIndividualContacts.DataValueField = "IdentityId";
                this.cboIndividualContacts.DataBind();

                if (this.cboIndividualContacts.Items.Count == 0)
                    this.grdContacts.DataSource = null;
                else
                {
                    this.cboIndividualContacts.SelectedValue = this.cboIndividualContacts.Items[0].Value;
                    this.cboIndividualContacts.Text = this.cboIndividualContacts.Items[0].Text;
                    this.cboIndividualContacts.Items[0].Selected = true;

                    this.IndividualContactId = Convert.ToInt32(this.cboIndividualContacts.SelectedValue);

                    this.PopulateContactsGrid();
                }
            }
        }

        void cboIndividualContacts_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(this.cboIndividualContacts.SelectedValue))
            {
                this.IndividualContactId = int.Parse(this.cboIndividualContacts.SelectedValue);
                this.OrganisationId = int.Parse(this.cboClient.SelectedValue);

                this.PopulateContactsGrid();
            }
        }

        void grdContacts_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            this.PopulateContactsGrid();
        }

        void cboClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            Facade.IReferenceData facReferenceData = new Facade.ReferenceData();
            DataSet ds;
            DataTable dt;
            RadComboBoxItem rcItem;
            int itemsPerRequest = 20;
            int itemOffset;
            int endOffset;

            this.cboClient.Items.Clear();

            if (this.IsCalledFromHotKey)
                ds = facReferenceData.GetClientAndSubcontractorLookup(e.Text);
            else if (this.IsClient)
                ds = facReferenceData.GetAllClientsFiltered(e.Text);
            else
                ds = facReferenceData.GetAllSubContractorsFiltered(e.Text);

            itemOffset = e.NumberOfItems;
            endOffset = itemOffset + itemsPerRequest;

            dt = ds.Tables[0];
            dt.DefaultView.Sort = "OrganisationName";
            dt.DefaultView.ApplyDefaultSort = true;

            if (endOffset > dt.Rows.Count)
                endOffset = dt.Rows.Count;

            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new RadComboBoxItem();
                rcItem.Text = dt.Rows[i]["OrganisationName"].ToString();
                rcItem.Value = dt.Rows[i]["IdentityID"].ToString();
                cboClient.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset, dt.Rows.Count);
        }

        void cboIndividualContacts_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {

        }

        #endregion

        #region Private methods

        private void PopulateContactsGrid()
        {
            DataTable contactInfo = new DataTable();

            contactInfo.Columns.Add(eContactType.Email.ToString());
            contactInfo.Columns.Add(eContactType.Telephone.ToString());
            contactInfo.Columns.Add(eContactType.MobilePhone.ToString());
            contactInfo.Columns.Add(eContactType.PersonalMobile.ToString());
            contactInfo.Columns.Add(eContactType.Fax.ToString());

            if (this.IndividualContactId > 0)
            {
                foreach (Individual individual in this.OrganisationDetails.IndividualContacts)
                    if (individual.IdentityId == this.IndividualContactId)
                    {
                        DataRow newRow = contactInfo.NewRow();

                        newRow[eContactType.Email.ToString()] = this.GetContactValue(individual.Contacts, eContactType.Email);
                        newRow[eContactType.Telephone.ToString()] = this.GetContactValue(individual.Contacts, eContactType.Telephone);
                        newRow[eContactType.MobilePhone.ToString()] = this.GetContactValue(individual.Contacts, eContactType.MobilePhone);
                        newRow[eContactType.PersonalMobile.ToString()] = this.GetContactValue(individual.Contacts, eContactType.PersonalMobile);
                        newRow[eContactType.Fax.ToString()] = this.GetContactValue(individual.Contacts, eContactType.Fax);

                        contactInfo.Rows.Add(newRow);

                        this.grdContacts.DataSource = contactInfo;
                        this.grdContacts.DataBind();

                        break;
                    }
            }
            else
                this.grdContacts.DataSource = contactInfo;
        }

        private string GetContactValue(Entities.ContactCollection contacts, eContactType type)
        {
            string contactValue = String.Empty;

            if (contacts != null)
            {
                Entities.Contact tempContact = contacts.GetForContactType(type);

                if (tempContact != null)
                    contactValue = tempContact.ContactDetail;
            }

            return contactValue;
        }

        private DataTable GetIndividualTable(List<Entities.Individual> individuals)
        {
            DataTable individualsTable = new DataTable();

            individualsTable.Columns.Add("FullName");
            individualsTable.Columns.Add("ContactType");
            individualsTable.Columns.Add("IdentityId");

            foreach (Entities.Individual individual in individuals)
            {
                DataRow newRow = individualsTable.NewRow();
                newRow["FullName"] = String.Format("{0} {1}",individual.Title,individual.FullName);
                newRow["ContactType"] = Utilities.UnCamelCase(individual.IndividualContactType.ToString());
                newRow["IdentityId"] = individual.IdentityId;

                individualsTable.Rows.Add(newRow);
            }

            return individualsTable;
        }

        #endregion
    }
}