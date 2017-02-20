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

namespace Orchestrator.WebUI.Organisation
{
	/// <summary>
	/// Summary description for addupdateorganisationreference.
	/// </summary>
	public partial class addupdateorganisationreference : Orchestrator.Base.BasePage
	{
		#region Form Elements
		
		


		#endregion

		#region Constants

		private const string C_REFERENCE_VS = "Reference";
		private const string C_REFERENCE_ID_VS = "ReferenceId";

		#endregion

		#region Page Variables

		private int m_identityId = 0;
		private int m_referenceId = 0;
		private bool m_isUpdate = false;
		private Entities.OrganisationReference m_reference;

		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditOrganisations);

			m_identityId = Int32.Parse(Request.QueryString["identityId"]);
			m_referenceId = Convert.ToInt32(Request.QueryString["organisationReferenceId"]);
			if (m_referenceId > 0)
			{
				m_isUpdate = true;
				ViewState[C_REFERENCE_ID_VS] = m_referenceId;
			}
			else
			{
				m_referenceId = Convert.ToInt32(ViewState[C_REFERENCE_ID_VS]);
				if (m_referenceId > 0)
					m_isUpdate = true;
			}

			if (!IsPostBack)
			{
				PopulateStaticControls();
				ConfigureReturnLink();

				if (m_isUpdate)
				{
					LoadReference();
				}
			}
			else
			{
				// retrieve the reference from the viewstate
				m_reference = (Entities.OrganisationReference) ViewState[C_REFERENCE_VS];
			}
		}

		/// <summary>
		/// Configures a link that allows the user to return to the organisation they were manipulating.
		/// </summary>
		private void ConfigureReturnLink()
		{
			string link = "<a href=\"addupdateorganisation.aspx?identityId=" + m_identityId + "\">Return to " + Request.QueryString["organisationName"] + "</a><br>";
			lblReturnLink.Text = link;
			lblReturnLink.Visible = true;
		}

		/// <summary>
		/// Creates the reference
		/// </summary>
		/// <returns>true if the reference creation succeeded, false otherwise.</returns>
		private bool CreateReference()
		{
			bool retVal = false;

			Facade.IOrganisationReference facOrganisationReference = new Facade.Organisation();
			string userId = ((Entities.CustomPrincipal) Page.User).UserName;
			m_referenceId = facOrganisationReference.Create(m_reference, userId);
			
			if (m_referenceId == 0)
			{
				retVal = false;
			}
			else
			{
				m_reference.OrganisationReferenceId = m_referenceId;
				retVal = true;
			}

			return retVal;
		}

		/// <summary>
		/// Retrieves the reference identified by m_referenceId from the organisation and populates the form controls.
		/// Only occurs in update mode.
		/// </summary>
		private void LoadReference()
		{
			// retrieve the reference and store it in viewstate
			Facade.IOrganisationReference facOrganisationReference = new Facade.Organisation();
			m_reference = facOrganisationReference.GetReferenceForOrganisationReferenceId(m_referenceId);
			ViewState[C_REFERENCE_VS] = m_reference;

			// populate the form controls
			txtDescription.Text = m_reference.Description;
			cboDataType.SelectedValue = Utilities.UnCamelCase(m_reference.DataType.ToString());
			cboStatus.SelectedValue = Utilities.UnCamelCase(m_reference.Status.ToString());

			btnAdd.Text = "Update";
		}

		/// <summary>
		/// Populates the reference object with the new information.
		/// </summary>
		private void PopulateReference()
		{
			if (m_reference == null)
			{
				// adding a new reference, configure identity
				m_reference = new Orchestrator.Entities.OrganisationReference();
				m_reference.IdentityId = m_identityId;
			}

			// Update the reference based on it's settings.
			m_reference.Description = txtDescription.Text;
			m_reference.DataType = (eOrganisationReferenceDataType) Enum.Parse(typeof(eOrganisationReferenceDataType), cboDataType.SelectedValue.Replace(" ", ""), true);
			m_reference.Status = (eOrganisationReferenceStatus) Enum.Parse(typeof(eOrganisationReferenceStatus), cboStatus.SelectedValue.Replace(" ", ""), true);
            m_reference.DisplayOnInvoice = chkCanDisplayOnInvoice.Checked;
            m_reference.MandatoryOnOrder = chkIsMandatoryOnOrder.Checked;
        }
		
		private void PopulateStaticControls()
		{
			cboDataType.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eOrganisationReferenceDataType)));
			cboDataType.DataBind();

			cboStatus.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eOrganisationReferenceStatus)));
			cboStatus.DataBind();
		}

		/// <summary>
		/// Updates the reference
		/// </summary>
		/// <returns>true if the update succeeded, false otherwise</returns>
		private bool UpdateReference()
		{
			Facade.IOrganisationReference facOrganisationReference = new Facade.Organisation();
			string userId = ((Entities.CustomPrincipal) Page.User).UserName;
			return facOrganisationReference.Update(m_reference, userId);
		}

		#region Event Handlers

		protected void btnAdd_Click(object sender, System.EventArgs e)
		{
			bool retVal = false;

			if (Page.IsValid)
			{
				PopulateReference();

				if (m_reference.OrganisationReferenceId == 0)
				{
					// create a new reference
					retVal = CreateReference();
				}
				else
				{
					// update an existing reference
					retVal = UpdateReference();
				}


				if (m_isUpdate)
				{
					lblConfirmation.Text = "The Reference has " + (retVal ? "" : "not ") + "been updated successfully.";
				}
				else
				{
					lblConfirmation.Text = "The Reference has " + (retVal ? "" : "not ") + "been created successfully.";

					if (retVal)
					{
						ViewState[C_REFERENCE_ID_VS] = m_reference.OrganisationReferenceId;
						ViewState[C_REFERENCE_VS] = m_reference;
						btnAdd.Text = "Update";
					}
				}

				lblConfirmation.Visible = true;
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

		}
		#endregion
	}
}
