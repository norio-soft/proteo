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


namespace Orchestrator.WebUI.Resource.Driver
{
	/// <summary>
	/// Summary description for AddUpdateDriverRequest.
	/// </summary>
	public partial class AddUpdateDriverRequest : Orchestrator.Base.BasePage
	{
		private const string C_REQUEST_VS = "C_REQUEST_VS";

		#region Form Elements





		#endregion

		#region Page Variables

		private		int				m_requestId = 0;
		private		int				m_resourceId = 0;

		private		Entities.DriverRequest	m_request = null;

		private		bool			m_isUpdate = false;

		#endregion

		#region Page Load/Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage, eSystemPortion.AddEditResource);

			m_resourceId = Convert.ToInt32(Request.QueryString["resourceId"]);
			m_requestId = Convert.ToInt32(Request.QueryString["requestId"]);

			m_request = (Entities.DriverRequest) ViewState[C_REQUEST_VS];

			if (!IsPostBack)
			{
				if (m_requestId > 0)
				{
					m_isUpdate = true;

					Facade.IDriverRequest facDriverRequest = new Facade.Resource();
					m_request = facDriverRequest.GetForRequestId(m_requestId);
					ViewState[C_REQUEST_VS] = m_request;

					txtRequestText.Text = m_request.Text;
                    dteRequestDate.SelectedDate = m_request.AppliesTo;
					m_resourceId = m_request.Driver.ResourceId;
					cboDrivers.Text = m_request.Driver.Individual.FullName;
					cboDrivers.SelectedValue = m_resourceId.ToString();
				}
				else if (m_resourceId > 0)
				{
					Facade.IDriver facDriver = new Facade.Resource();
					cboDrivers.Text = facDriver.GetDriverForResourceId(m_resourceId).Individual.FullName;
					cboDrivers.SelectedValue = m_resourceId.ToString();
				}
			}

			if (m_request != null)
			{
				m_isUpdate = true;
				cboDrivers.Enabled = false;
				btnAdd.Text = "Update Request";
			}
			else
				btnAdd.Text = "Add Request";

			lblConfirmation.Visible = false;
		}

		private void AddUpdateDriverRequest_Init(object sender, EventArgs e)
		{
			btnAdd.Click += new EventHandler(btnAdd_Click);
			btnListRequests.Click += new EventHandler(btnListRequests_Click);
            this.cboDrivers.ItemsRequested +=new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboDrivers_ItemsRequested);
            cfvDriver.ServerValidate += new ServerValidateEventHandler(cfvDriver_ServerValidate);

		}

        void cfvDriver_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(cboDrivers.SelectedValue, 1, true);
        }

		#endregion

		#region Button Events
		private void PopulateRequest()
		{
			if (m_request == null)
			{
				Facade.IDriver facDriver = new Facade.Resource();
				Entities.Driver driver = facDriver.GetDriverForResourceId(Convert.ToInt32(cboDrivers.SelectedValue));

                m_request = new Entities.DriverRequest(driver, txtRequestText.Text, dteRequestDate.SelectedDate.Value);
			}
			else
			{
				m_request.Text = txtRequestText.Text;
				m_request.AppliesTo = dteRequestDate.SelectedDate.Value;
			}
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			if (Page.IsValid)
			{
				PopulateRequest();

				Facade.IDriverRequest facDriverRequest = new Facade.Resource();
				bool success = false;
				string userId = ((Entities.CustomPrincipal) Page.User).UserName;

				if (m_isUpdate)
					success = facDriverRequest.Update(m_request, userId);
				else
				{
					m_request.RequestId = facDriverRequest.Create(m_request, userId);
					success = m_request.RequestId > 0;
				}

				lblConfirmation.Text = "The request has " + (success ? "" : "not ") + "been " + (m_isUpdate ? "updated." : "added.");
				lblConfirmation.Visible = true;

				if (success)
				{
					ViewState[C_REQUEST_VS] = m_request;
					cboDrivers.Enabled = false;
					btnAdd.Text = "Update Request";
				}
			}
		}

		private void btnListRequests_Click(object sender, EventArgs e)
		{
			Response.Redirect("ListDriverRequests.aspx");
		}

		#endregion

		#region DBCombo's Server Methods and Initialisation

		void cboDrivers_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {

            cboDrivers.Items.Clear();

            Facade.IResource facResource = new Facade.Resource();
            DataSet ds = facResource.GetAllResourcesFiltered(e.Text, eResourceType.Driver, false);

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
                rcItem.Text = dt.Rows[i]["Description"].ToString();
                rcItem.Value = dt.Rows[i]["ResourceId"].ToString();
                cboDrivers.Items.Add(rcItem);
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
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Init += new EventHandler(AddUpdateDriverRequest_Init);
		}
		#endregion
	}
}
