using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Orchestrator.WebUI.UserControls;
using Orchestrator.Globals;
using P1TP.Components.Web.Validation;

namespace Orchestrator.WebUI.POD
{
	/// <summary>
	/// Summary description for ViewPODs.
	/// </summary>
	public partial class ViewPODs : Orchestrator.Base.BasePage
	{
		#region Enums

		public enum eDataGridColumns { PODId, ScannedDate, SignatureDate, OrganisationName, ClientsCustomer, Destination, Pickup, DeliveryDate, TrailerRef, TicketNo, JobId, LoadNo, View, Download, EmailFax, ScannedFormId };
		#endregion

		#region Constants

		private const string C_PODDATA_VS = "PODData";

		#endregion

        private int? UserClientID
        {
            get { return (int?)ViewState["UserClientID"]; }
            set { ViewState["UserClientID"] = value; }
        }

		#region Private Fields

		private DataSet				dsPODList;

		#endregion

		#region Form Elements

		protected Panel				pnlClient;

		#endregion

		#region Page Load/Init/Error

		protected void Page_Load(object sender, System.EventArgs e)
		{
            if (!this.IsPostBack)
            {
                Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage, eSystemPortion.SearchForPODs);

                //Get the user's Identity row
                Facade.IUser facUser = new Facade.User();
                SqlDataReader reader = facUser.GetRelatedIdentity(((Entities.CustomPrincipal)Page.User).UserName);
                reader.Read();
                //Is the User a Client User
                if ((eRelationshipType)reader["RelationshipTypeId"] == eRelationshipType.IsClient)
                {
                    //Set the Client combo yo the Client of the User and diable it
                    int clientID = (int)reader["RelatedIdentityId"];
                    UserClientID = clientID;
                    cboClient.SelectedValue = Convert.ToString(clientID);
                    Facade.IOrganisation facOrganisation = new Facade.Organisation();
                    Entities.Organisation organisation = facOrganisation.GetForIdentityId(clientID);
                    cboClient.Text = organisation.OrganisationName;
                    cboClient.Enabled = false;

                }
                reader.Close();
            }

            if (UserClientID.HasValue)
                ConfigureClientUI(); //Tweak the grid headings etc. so that the are relevant for a Client user

            if (!dteDateOfDeliveryFrom.SelectedDate.HasValue)
                dteDateOfDeliveryFrom.SelectedDate = DateTime.Today.AddMonths(-5);

            if (!dteDateOfDeliveryTo.SelectedDate.HasValue)
                dteDateOfDeliveryTo.SelectedDate = DateTime.Today.AddMonths(1);

            // retrive the dataset from viewstate
			dsPODList = (DataSet) ViewState[C_PODDATA_VS];
		}

		#endregion

		private void ConfigureClientUI()
		{
            // Column names
            dgPODList.Levels[0].Columns[4].HeadingText = "Customer";
            dgPODList.Levels[0].Columns[9].HeadingText = "Our Order ID";
            dgPODList.Levels[0].Columns[10].HeadingText = "Our Run ID";
            dgPODList.Levels[0].Columns[11].HeadingText = "Your Load No";
            dgPODList.Levels[0].Columns[12].HeadingText = "Your Docket No";

            // Visibility of columns
            dgPODList.Levels[0].Columns[3].Visible = false;
            dgPODList.Levels[0].Columns[6].Visible = false;

			// Labels		
			lblClientsCustomer.Text = "Customer";
		}

		#region DataGrid Event Handlers

		private void GetPODs()
		{
			bool noCriteria = false;

            if (dteDateOfDelivery.DateInput.Text == String.Empty && dteDateOfDeliveryFrom.DateInput.Text == String.Empty && dteDateOfDeliveryTo.DateInput.Text == String.Empty
                && cboClient.Text == String.Empty && cboClientsCustomer.Text == String.Empty && txtTicketNo.Text == String.Empty
                && this.orderIdTextbox.Text == String.Empty && this.loadNoTextbox.Text == String.Empty 
                && this.docketNoTextbox.Text == String.Empty && this.customerReferenceTextbox.Text == String.Empty && 
                this.jobIdTextbox.Text == String.Empty && this.trailerRefTextbox.Text == String.Empty)
			{
				noCriteria = true;
			}

			Facade.IPOD facPOD = new Facade.POD();

			if (noCriteria)
			{
				dsPODList = facPOD.GetAll();	
			}
			else
			{
				
				// By POD Filtering
                int clientId = cboClient.SelectedValue == "" ? 0 : int.Parse(cboClient.SelectedValue);
                int clientsCustomerId = cboClientsCustomer.SelectedValue == "" ? 0 : int.Parse(cboClientsCustomer.SelectedValue);
				string ticketNo = txtTicketNo.Text == "" ? "" : txtTicketNo.Text;
                DateTime deliveryDate, deliveryDateFrom, deliveryDateTo;

				if (dteDateOfDelivery.SelectedDate == null || dteDateOfDelivery.SelectedDate.Value == DateTime.MinValue)
                    deliveryDate = Convert.ToDateTime(Convert.ToString(SqlDateTime.MinValue));
				else
                    deliveryDate = dteDateOfDelivery.SelectedDate.Value;

				if (dteDateOfDeliveryFrom.SelectedDate == null || dteDateOfDeliveryFrom.SelectedDate.Value == DateTime.MinValue)
                    deliveryDateFrom = Convert.ToDateTime(Convert.ToString(SqlDateTime.MinValue));
				else
                    deliveryDateFrom = dteDateOfDeliveryFrom.SelectedDate.Value;

				
				if (dteDateOfDeliveryTo.SelectedDate == null || dteDateOfDeliveryTo.SelectedDate.Value == DateTime.MinValue)
                    deliveryDateTo = Convert.ToDateTime(Convert.ToString(SqlDateTime.MinValue));
				else
				{
                    deliveryDateTo = dteDateOfDeliveryTo.SelectedDate.Value;
                    deliveryDateTo = deliveryDateTo.Add(new TimeSpan(23, 59, 00));
				}

				// By Job Filtering
                string loadNo = this.loadNoTextbox.Text.Trim();
                string docketNo = this.docketNoTextbox.Text.Trim();
                string customerReference = this.customerReferenceTextbox.Text.Trim(); ;
                string trailerRef = this.trailerRefTextbox.Text.Trim();
                int jobId = 0;
                int orderId = 0;

                if(!String.IsNullOrEmpty(this.jobIdTextbox.Text))
                    int.TryParse(this.jobIdTextbox.Text, out jobId);

                if (!String.IsNullOrEmpty(this.orderIdTextbox.Text))
                    int.TryParse(this.orderIdTextbox.Text, out orderId);

                dsPODList = facPOD.GetWithParams(clientId, clientsCustomerId, ticketNo, deliveryDate, deliveryDateFrom,
                    deliveryDateTo, loadNo, docketNo, customerReference, trailerRef, jobId, orderId);
			}

            ViewState["C_PODDATA_VS"] = dsPODList;

			if (dsPODList.Tables[0].Rows.Count > 0)
			{
				dgPODList.DataSource = dsPODList;
            
                dgPODList.DataBind();
				dgPODList.Visible = true;
                lblError.Visible = false;

				
			}
			else
			{
				lblError.Visible = true;
				dgPODList.Visible = false;
			}
		}
		
		#endregion

		#region Event Handlers

		private void ViewPODs_Init(object sender, System.EventArgs e)
		{
			cfvDateOfDeliveryFrom.ServerValidate +=new ServerValidateEventHandler(cfvDateOfDeliveryFrom_ServerValidate);
			this.btnFilter.Click += new System.EventHandler(btnFilter_Click);
		    this.cboClient.ItemsRequested +=new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
            this.cboClientsCustomer.ItemsRequested +=new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClientsCustomer_ItemsRequested);
		}

		private void btnFilter_Click(object sender, System.EventArgs e)
		{
			
			reportViewer.Visible = false;
			
			bool noCriteria = false;
			
			if (Page.IsValid)
			{
                if (dteDateOfDelivery.DateInput.Text == String.Empty && dteDateOfDeliveryFrom.DateInput.Text == String.Empty && dteDateOfDeliveryTo.DateInput.Text == String.Empty
                    && cboClient.Text == String.Empty && cboClientsCustomer.Text == String.Empty && txtTicketNo.Text == String.Empty
                    && this.orderIdTextbox.Text == String.Empty && this.loadNoTextbox.Text == String.Empty
                    && this.docketNoTextbox.Text == String.Empty && this.customerReferenceTextbox.Text == String.Empty &&
                    this.jobIdTextbox.Text == String.Empty && this.trailerRefTextbox.Text == String.Empty)
				{
					noCriteria = true;
				}

				if (!noCriteria)
					GetPODs();
				else
					dgPODList.Visible = false;
			}
		}
		
		#endregion

		#region DBCombo's Server Methods and Initialisation



        void cboClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {

            cboClient.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllClientsFiltered(e.Text);

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

		void cboClientsCustomer_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboClientsCustomer.Items.Clear();

            int identityID = int.Parse(e.Context["IdentityID"].ToString());

            //The Client IdentityId is passed in the combo's ClientDataString
            //If it is empty it means that a the combo selection hasn't changed
            //so check whether it was set to a client already (i.e. the Client User scenario)
            int clientId = 0;
            
            if (identityID ==0 )
            {
                if (!string.IsNullOrEmpty(cboClient.SelectedValue))
                    int.TryParse(cboClient.SelectedValue, out clientId);
            }
            else
            {
                clientId = identityID;
            }

            //A client must be specified before their Customer's can be listed
            if (clientId == 0)
                return;

            //Restrict the Client Customers returned to the Client selected
            Orchestrator.Facade.IOrganisation facOrganisation = new Orchestrator.Facade.Organisation();
            DataSet ds = facOrganisation.GetClientCustomersForClientFiltered(
                clientId, e.Text);

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
                cboClientsCustomer.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
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

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            if (IsClientUser)
                Page.MasterPageFile = "~/default_tableless_client.Master";
        }
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Init += new EventHandler(this.ViewPODs_Init);
		}
		#endregion

		#region ActiveReport 

		private void LoadReport(string podIdsCSV)
		{

			Facade.IPOD facPOD = new Facade.POD();

			DataSet dsPODs = facPOD.GetScannedPODsForPODId(podIdsCSV);
			//-------------------------------------------------------------------------------------	
			//									Load Report Section 
			//-------------------------------------------------------------------------------------	
			Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.EmailFaxForm;
			Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsPODs;
			Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
			Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";
			reportViewer.IdentityId = int.Parse(cboClient.SelectedValue);
			// Show the user control
			reportViewer.Visible = true;
		}

		#endregion

        private void cfvDateOfDeliveryFrom_ServerValidate(object source, ServerValidateEventArgs args)
		{
			if (dteDateOfDeliveryFrom.SelectedDate.Value.ToString() != "")
			{
                if (dteDateOfDeliveryFrom.SelectedDate <= dteDateOfDeliveryTo.SelectedDate)
					args.IsValid = true;
				else
					args.IsValid = false;
			}
			else
				args.IsValid = true;
		}
	}
}
