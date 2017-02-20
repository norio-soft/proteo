using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using System.Text.RegularExpressions;

using Orchestrator.Entities;
using Orchestrator.Logging;
using Orchestrator.Globals;
using Orchestrator.WebUI.Reports;

using P1TP.Components.Web.Validation;
using System.Collections.Generic;

namespace Orchestrator.WebUI.UserControls
{
	

	/// <summary>
	///	Summary description for ReportViewer.
	/// </summary>
	public partial class ReportViewer : System.Web.UI.UserControl
	{
		#region Constants

		private const char C_EMAIL_ADDRESS_DELIMITER_VS = ';';

		private const string C_EMAIL_VALIDATION_MESSAGE_VS = @"<img src='{0}' height='16' width='16' title='Please supply a valid email address to email this report to.' />";
		private const string C_FAX_VALIDATION_MESSAGE_VS = @"<img src='{0}' height='16' width='16' title='Please supply a valid fax number to fax this report to.' />";
		
		#endregion

		#region Form Elements





		#endregion

		#region Page Variables

		private string		m_viewerHeight	= "500";
		private string		m_viewerWidth	= "100%";

		private int			m_identityId	= 0;

		#endregion
	 
		#region Property Interfaces

		public string ViewerHeight
		{
			get { return m_viewerHeight; }
			set { m_viewerHeight = value.Replace("%", ""); }
		}

		public string ViewerWidth
		{
			get { return m_viewerWidth; }
			set { m_viewerWidth = value; }
		}

		public int IdentityId
		{
			get { return m_identityId; }
			set
			{
				m_identityId = value;
				ImportOrganisationValues();
			}
		}

		#endregion

		#region Page Load/Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (string.IsNullOrEmpty(Request.QueryString["rpk"]))
                iReport.Attributes.Add("src", Page.ResolveUrl("~/Reports/ActiveReportPdf.ashx"));
			else
                iReport.Attributes.Add("src", Page.ResolveUrl(string.Format("~/Reports/ActiveReportPdf.ashx?rpk={0}", Request.QueryString["rpk"])));

			if (Request.QueryString["rcbID"] == null)
			{
				#region Configure Validation Handling

				revFaxNumber.ValidationExpression = Constants.UK_TELEPHONE_NUMBER_REGULAR_EXPRESSION.ToString();
				// revEmailAddress.ValidationExpression = Constants.EMAIL_REGULAR_EXPRESSION.ToString();
				revEmailAddress.ValidationExpression = Constants.MULTIPLE_EMAIL_REGULAR_EXPRESSION.ToString();
				// Configure the validator controls
				// include all the validators on the page (except the ones on this page)
				StringBuilder sbPage = new StringBuilder();
				StringBuilder sbControl = new StringBuilder();

				string validationImageUrl = Page.ResolveUrl("~/Images/Error.gif");

				revFaxNumber.ErrorMessage = string.Format(C_FAX_VALIDATION_MESSAGE_VS, validationImageUrl);
				rfvFaxNumber.ErrorMessage = string.Format(C_FAX_VALIDATION_MESSAGE_VS, validationImageUrl);

				revEmailAddress.ErrorMessage = string.Format(C_EMAIL_VALIDATION_MESSAGE_VS, validationImageUrl);
				rfvEmailAddress.ErrorMessage = string.Format(C_EMAIL_VALIDATION_MESSAGE_VS, validationImageUrl);

				foreach (IValidator validator in Page.Validators)
				{
					if (!this.Controls.Contains((Control)validator))
					{
						if (sbPage.Length != 0)
							sbPage.Append(",");

						sbPage.Append(((Control)validator).ID);
					}
					else
					{
						if (sbControl.Length != 0)
							sbControl.Append(",");

						sbControl.Append(((Control)validator).ID);
					}
				}

				// Assign the validation controls found on the rest of the page (i.e. not in this user control)
				// to the user control contained Fax and Email report button.
				if (btnFaxReport.NoFormValList != String.Empty)
					btnFaxReport.NoFormValList += ",";
				btnFaxReport.NoFormValList += sbPage.ToString();

				if (btnEmailReport.NoFormValList != String.Empty)
					btnEmailReport.NoFormValList += ",";
				btnEmailReport.NoFormValList += sbPage.ToString();

				if (sbControl.Length != 0)
				{
					// Get the user control validators.
					string ucValidators = sbControl.ToString();

					AssignUCValidatorsToPageNoFormValButtons(Page.Controls, ucValidators);
				}

				#endregion

				lblConfirmation.Visible = false;
			}
		}

		private void ReportViewer_Init(object sender, EventArgs e)
		{
			this.btnFaxReport.Click +=	new EventHandler(btnFaxReport_Click);
			this.btnEmailReport.Click += new EventHandler(btnEmailReport_Click);
			btnExcelReport.Click += new EventHandler(btnExcelReport_Click);
		}

		#endregion

		/// <summary>
		/// Assign the validation controls found on this user control to any NoFormValButton found on the
		/// page, this will stop the email requirement being enforced.
		/// </summary>
		/// <param name="validators"></param>
		private void AssignUCValidatorsToPageNoFormValButtons(System.Web.UI.ControlCollection cc, string validators)
		{
			foreach (Control control in cc)
			{
				if (control.GetType().FullName == "P1TP.Components.Web.Validation.NoFormValButton")
				{
					NoFormValButton nfvb = (NoFormValButton) control;

					if (!this.Controls.Contains(nfvb))
					{
						if (nfvb.NoFormValList.Trim() == String.Empty)
							nfvb.NoFormValList = validators;
						else
							nfvb.NoFormValList += "," + validators;
					}
				}

				AssignUCValidatorsToPageNoFormValButtons(control.Controls, validators);
			}
		}

		private void ImportOrganisationValues()
		{
			cboEmail.Text = String.Empty;
			cboEmail.SelectedValue = String.Empty;
			cboEmail.DataSource = null;
			cboFaxNumber.SelectedValue = String.Empty;
			cboFaxNumber.Text = String.Empty;
			cboFaxNumber.DataSource = null;
			txtFaxNumber.Text = String.Empty;
			txtEmail.Text = String.Empty;

			if (m_identityId > 0)
			{
				Facade.IOrganisation facOrganisation = new Facade.Organisation();
				Entities.Organisation organisation = facOrganisation.GetForIdentityId(m_identityId);

				//RJD: changed to use IndividualContacts instead of PrimaryContact STILL NEEDS WORK
				if (organisation != null && (organisation.IndividualContacts != null && organisation.IndividualContacts.Count > 0))
				{
					DataSet emailDataSet = this.GetContactDataSet(organisation.IndividualContacts,eContactType.Email);

					cboEmail.DataSource = emailDataSet;
					cboEmail.DataMember = "contactTable";
					cboEmail.DataValueField = "ContactDetail";
					cboEmail.DataTextField = "ContactDisplay";
					cboEmail.DataBind();

					if (this.cboEmail.Items.Count > 0)
						this.txtEmail.Text = this.cboEmail.Items[0].Value;

					DataSet faxDataSet = this.GetContactDataSet(organisation.IndividualContacts, eContactType.Fax);

					if (organisation != null && organisation.Locations != null)
					{
						Entities.OrganisationLocation headOffice = organisation.Locations.GetHeadOffice();

						if (!String.IsNullOrEmpty(headOffice.FaxNumber))
						{
							DataRow dr = faxDataSet.Tables[0].NewRow();
							dr["ContactName"] = "Head Office";
							dr["ContactDetail"] = headOffice.FaxNumber;
							dr["ContactDisplay"] = String.Format("{0} - {1}", dr["ContactName"], headOffice.FaxNumber);
							faxDataSet.Tables[0].Rows.InsertAt(dr, 0);
						}
					}

					cboFaxNumber.DataSource = faxDataSet;
					cboFaxNumber.DataMember = "contactTable";
					cboFaxNumber.DataValueField = "ContactDetail";
					cboFaxNumber.DataTextField = "ContactDisplay";
					cboFaxNumber.DataBind();

					if (this.cboFaxNumber.Items.Count > 0)
						this.txtFaxNumber.Text = this.cboFaxNumber.Items[0].Value;
				}
			}
			else
				cboFaxNumber.Text = txtFaxNumber.Text = String.Empty;
		}

		private DataSet GetContactDataSet(List<Entities.Individual> individualContacts, eContactType contactType)
		{
			DataSet contactsDataSet = new DataSet();
			DataTable contactTable = new DataTable("contactTable");

			string contactNameColumn = "ContactName";
			string contactDetailColumn = "ContactDetail";
			string ContactDisplayColumn = "ContactDisplay";

			contactTable.Columns.Add(contactNameColumn);
			contactTable.Columns.Add(contactDetailColumn);
			contactTable.Columns.Add(ContactDisplayColumn);

			contactsDataSet.Tables.Add(contactTable);

			foreach (Entities.Individual individual in individualContacts)
				if (individual.Contacts != null)
				{
					Entities.Contact contact = individual.Contacts.GetForContactType(contactType);
					if (contact != null)
					{
						DataRow row = contactsDataSet.Tables[0].NewRow();

						row[contactNameColumn] = String.Format("{0} {1} {2}",
							individual.Title, individual.FirstNames, individual.LastName);

						row[contactDetailColumn] = contact.ContactDetail;

						row[ContactDisplayColumn] = String.Format("{0} - {1}", row[contactNameColumn], row[contactDetailColumn]);

						contactsDataSet.Tables[0].Rows.Add(row);
					}
				}

			return contactsDataSet;
		}

		#region Button Event Handlers

		private void btnFaxReport_Click(object sender, EventArgs e)
		{
			btnFaxReport.DisableServerSideValidation();

			string faxNumber = cboFaxNumber.Text;

			// As the value is appended to the end of the display value, if the text property does not end with value, a custom fax number has
			// been supplied - in which case we should not use the value and use the text property that has already been picked up.
			if (cboFaxNumber.SelectedValue != string.Empty)
				faxNumber = cboFaxNumber.SelectedValue;

			this.txtFaxNumber.Text = faxNumber;

			this.rfvFaxNumber.Validate();
			this.revFaxNumber.Validate();

			if (Page.IsValid)
			{
				var reportExporter = new ActiveReportExporter();
				
				#if DEBUG
					lblConfirmation.Text = "Fax Not sent, in Debug mode";
				#else
					lblConfirmation.Text = reportExporter.FaxReport(faxNumber);
				#endif

				lblConfirmation.Visible = true;
			}
		}

		private void btnEmailReport_Click(object sender, EventArgs e)
		{
			btnEmailReport.DisableServerSideValidation();

			string emailAddress = this.cboEmail.Text;

			if (this.cboEmail.SelectedValue != String.Empty)
				emailAddress = this.cboEmail.SelectedValue;

			this.txtEmail.Text = emailAddress;

			this.rfvEmailAddress.Validate();
			this.revEmailAddress.Validate();

			if (Page.IsValid)
			{
                var reportExporter = new ActiveReportExporter();

				bool sent = false;

				sent = reportExporter.EmailReport(emailAddress);

				if (sent)
					lblConfirmation.Text = "Your report has been emailed.";
				else
					lblConfirmation.Text = "Your report has not been emailed.";

				lblConfirmation.Visible = true;
			}		
		}

		private void btnExcelReport_Click(object sender, EventArgs e)
		{
            var reportExporter = new ActiveReportExporter();
			reportExporter.ExecuteReportExcel();
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Init += new EventHandler(ReportViewer_Init);
		}
		#endregion
	}
}
