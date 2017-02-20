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

namespace Orchestrator.WebUI.Resource.Vehicle
{
	/// <summary>
	/// Summary description for addupdatevehiclekey.
	/// </summary>
	public partial class addupdatevehiclekey : Orchestrator.Base.BasePage
	{
		#region Form Elements


		
		
		
		#endregion

		#region Constants

		private const string C_KEY = "Key";
		private const string C_KEY_ID = "KeyId";

		#endregion

		#region Page Variables

		private int m_keyId = 0;
		private int m_resourceId = 0;
		private Entities.VehicleKey m_key;
		private bool m_isUpdate = false;

		#endregion

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditResource);

			m_keyId = GetVehicleKeyId();
			m_resourceId = Convert.ToInt32(Request.QueryString["resourceId"]);

			if (m_keyId > 0)
				m_isUpdate = true;

			if (!IsPostBack)
			{
				PopulateStaticControls();
				ConfigureReturnLink();

				if (m_isUpdate)
					LoadVehicleKey();
			}
			else
			{
				// Retrieve the key from viewstate
				m_key = (Entities.VehicleKey) ViewState[C_KEY];
			}
		}

		private bool AddKey()
		{
			Facade.IVehicleKey facVehicleKey = new Facade.Resource();
			string userId = ((Entities.CustomPrincipal)Page.User).UserName;
			int vehicleKeyId = facVehicleKey.Create(m_key, userId);

			if (vehicleKeyId > 0)
			{
				return true;
			}
			else
			{
				lblConfirmation.Text = "There was an error adding the vehicle key, please try again.";
				lblConfirmation.Visible = true;
				lblConfirmation.ForeColor = Color.Red;
				return false;
			}
		}

		private void ConfigureReturnLink()
		{
			string link = "<a href=\"addupdatevehicle.aspx?resourceId=" + m_resourceId + "\">Return to vehicle</a><br>";
			lblReturnLink.Text = link;
			lblReturnLink.Visible = true;
		}

		private int GetVehicleKeyId()
		{
			// Attempt from QS
			int retVal = Convert.ToInt32(Request.QueryString["vehicleKeyId"]);
			if (retVal > 0)
			{
				// Store in VS
				ViewState[C_KEY_ID] = retVal;
			}
			else
			{
				// Attempt from VS
				retVal = Convert.ToInt32(ViewState[C_KEY_ID]);
			}

			return retVal;
		}

		private void LoadVehicleKey()
		{
			Facade.IVehicleKey facVehicleKey = new Facade.Resource();
			m_key = facVehicleKey.GetForVehicleKeyId(m_keyId);
			ViewState[C_KEY] = m_key;

			ddlKeyTypes.SelectedValue = Utilities.UnCamelCase(m_key.VehicleKeyType.ToString());
			txtSerial.Text = m_key.Serial;

			btnAdd.Text = "Update Vehicle Key";
		}

		private void PopulateKey()
		{
			if (m_key == null)
			{
				m_key = new Orchestrator.Entities.VehicleKey();
				m_key.ResourceId = m_resourceId;
			}

			m_key.VehicleKeyType = (eVehicleKeyType) Enum.Parse(typeof(eVehicleKeyType), ddlKeyTypes.SelectedValue.Replace(" ", ""), true);
			m_key.Serial = txtSerial.Text;
		}

		private void PopulateStaticControls()
		{
			// Key Types
			ddlKeyTypes.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eVehicleKeyType)));
			ddlKeyTypes.DataBind();
		}

		private bool UpdateKey()
		{
			Facade.IVehicleKey facVehicleKey = new Facade.Resource();
			string userId = ((Entities.CustomPrincipal)Page.User).UserName;
			return facVehicleKey.Update(m_key, userId);
		}

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
			this.btnAdd.Click += new EventHandler(btnAdd_Click);

		}
		#endregion

		private void btnAdd_Click(object sender, EventArgs e)
		{
			bool retVal = false;
			PopulateKey();

			if (m_isUpdate)
			{
				retVal = UpdateKey();
			}
			else
			{
				retVal = AddKey();
			}

			if (m_isUpdate)
			{
				lblConfirmation.Text = "The Vehicle Key has " + (retVal ? "" : "not ") + "been updated successfully.";
			}
			else
			{
				lblConfirmation.Text = "The Vehicle Key has " + (retVal ? "" : "not ") + "been added successfully.";

				// switch to update mode
				ViewState[C_KEY_ID] = m_key.ResourceId;
				ViewState[C_KEY] = m_key;
			}

			lblConfirmation.Visible = true;
		}
	}
}
