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

using Orchestrator.Globals;

using System.Text;

using P1TP.Components.Web.UI;

namespace Orchestrator.WebUI.Organisation
{
	/// <summary>
	/// Summary description for listclientcustomers.
	/// </summary>
	public partial class listclientcustomers : Orchestrator.Base.BasePage
	{
		#region Form Elements



		#endregion

		#region Page_Load

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

			PopulateClientsCustomers();
			
		}

		#endregion

		#region Populate DataGrid

		private void PopulateClientsCustomers()
		{
			using (Facade.Organisation facOrganisation = new Facade.Organisation())
				dgClients.DataSource = facOrganisation.GetAllForType((int) eOrganisationType.ClientCustomer);

			dgClients.DataBind();
		}

		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			PageNumbersPager1.DataGrid = dgClients;
			NextBackPager1.DataGrid = dgClients;
			NextBackPager2.DataGrid = dgClients;
			Firstlastpager1.DataGrid = dgClients;
			Firstlastpager2.DataGrid = dgClients;
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			dgClients.ItemCommand += new DataGridCommandEventHandler(dgClients_ItemCommand);
			dgClients.ItemDataBound += new DataGridItemEventHandler(dgClients_ItemDataBound);
			dgClients.PageIndexChanged += new DataGridPageChangedEventHandler(dgClients_PageIndexChanged);
            this.cboClient.ItemsRequested +=new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
		}
		#endregion

		#region DbCombo

		void cboClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {

            cboClient.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllOrganisationsFiltered(e.Text, eOrganisationType.ClientCustomer);

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

		#endregion

		#region Event Handler

		protected void btnUpdate_Click(object sender, System.EventArgs e)
		{
			if (Page.IsValid)
				Response.Redirect("addupdateorganisation.aspx?identityId=" + cboClient.SelectedValue);
		}

		#endregion

		private void dgClients_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			switch (e.CommandName.ToLower())
			{
				case "select":
					Response.Redirect("addupdateclientscustomer.aspx?identityId=" + ((HtmlInputHidden) e.Item.FindControl("hidIdentityId")).Value);
					break;
			}
		}

		private void dgClients_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				DataRowView rv = (DataRowView) e.Item.DataItem;

				if (rv["AddressLine1"] != DBNull.Value && ((string) rv["AddressLine1"]).Length > 0)
				{
					StringBuilder sb = new StringBuilder();

					sb.Append((string) rv["AddressLine1"]);
					sb.Append("<br>");
				
					if (rv["AddressLine2"] != DBNull.Value && ((string) rv["AddressLine2"]).Length > 0)
					{
						sb.Append((string) rv["AddressLine2"]);
						sb.Append("<br>");
					}

					if (rv["AddressLine3"] != DBNull.Value && ((string) rv["AddressLine3"]).Length > 0)
					{
						sb.Append((string) rv["AddressLine3"]);
						sb.Append("<br>");
					}

					if (rv["PostTown"] != DBNull.Value && ((string) rv["PostTown"]).Length > 0)
					{
						sb.Append((string) rv["PostTown"]);
						sb.Append("<br>");
					}

					if (rv["County"] != DBNull.Value && ((string) rv["County"]).Length > 0)
					{
						sb.Append((string) rv["County"]);
						sb.Append("<br>");
					}

					if (rv["PostCode"] != DBNull.Value && ((string) rv["PostCode"]).Length > 0)
					{
						sb.Append((string) rv["PostCode"]);
						sb.Append("<br>");
					}

					((Label) e.Item.FindControl("lblAddress")).Text = sb.ToString();
				}
			}
		}

		private void dgClients_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			dgClients.CurrentPageIndex = e.NewPageIndex;
			PopulateClientsCustomers();
		}
	}
}
