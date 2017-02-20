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


using P1TP.Components.Web.UI;

namespace Orchestrator.WebUI.Point
{
	/// <summary>
	/// Summary description for viewrates.
	/// </summary>
	public partial class viewrates : Orchestrator.Base.BasePage
	{
		#region Form Elements






		#endregion

		#region Page Variables

		protected	int	m_collectionClientId = 0;
		protected	int	m_collectionTownId = 0;
		protected	int	m_collectionPointId = 0;

		protected	int	m_deliveryClientId = 0;
		protected	int	m_deliveryTownId = 0;
		protected	int	m_deliveryPointId = 0;

		#endregion

		#region Property Interfaces

		private string SortCriteria
		{
			get { return (string) ViewState["C_SORT_CRITERIA"]; }
			set { ViewState["C_SORT_CRITERIA"] = value; }
		}

		private string SortDirection
		{
			get { return (string) ViewState["C_SORT_DIRECTION"]; }
			set { ViewState["C_SORT_DIRECTION"] = value; }
		}

		#endregion

		#region Page Load/Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);
 
			if (!IsPostBack)
				BindRates();
		}

		private void viewrates_Init(object sender, EventArgs e)
		{
			cboCollectionClient.ItemsRequested +=new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboCollectionClient_ItemsRequested);
            cboCollectionPoint.SelectedIndexChanged += new Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventHandler(cboCollectionPoint_SelectedIndexChanged);
			cboCollectionPoint.ItemsRequested +=new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboCollectionPoint_ItemsRequested);
            btnClearCollectionPoint.Click += new EventHandler(btnClearCollectionPoint_Click);


			cboDeliveryClient.ItemsRequested +=new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboDeliveryClient_ItemsRequested);
            cboDeliveryPoint.ItemsRequested +=new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboDeliveryPoint_ItemsRequested);
            cboDeliveryPoint.SelectedIndexChanged += new Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventHandler(cboDeliveryPoint_SelectedIndexChanged);
			btnClearDeliveryPoint.Click += new EventHandler(btnClearDeliveryPoint_Click);

			
            cboJobClient.ItemsRequested +=new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboJobClient_ItemsRequested);
            cboJobClient.SelectedIndexChanged += new Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventHandler(cboJobClient_SelectedIndexChanged);
			btnClearJobClient.Click += new EventHandler(btnClearJobClient_Click);

			dgRates.ItemCommand += new DataGridCommandEventHandler(dgRates_ItemCommand);
			dgRates.ItemDataBound += new DataGridItemEventHandler(dgRates_ItemDataBound);
			dgRates.SortCommand += new DataGridSortCommandEventHandler(dgRates_SortCommand);

			chkShowEnded.CheckedChanged += new EventHandler(chkShowEnded_CheckedChanged);
		}

        void cboJobClient_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            BindRates();
        }

        void cboDeliveryPoint_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            BindRates();
        }

        void cboCollectionPoint_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            BindRates();
        }

		#endregion

		private void BindRates()
		{
			int collectionPoint = 0;
			if (cboCollectionPoint.SelectedValue != "")
				collectionPoint = Convert.ToInt32(cboCollectionPoint.SelectedValue);
			int deliveryPoint = 0;
            if (cboDeliveryPoint.SelectedValue != "")
                deliveryPoint = Convert.ToInt32(cboDeliveryPoint.SelectedValue);
			int client = 0;
            if (cboJobClient.SelectedValue != "")
                client = Convert.ToInt32(cboJobClient.SelectedValue);

			using (Facade.IJobRate facJobRate = new Facade.Job())
			{
				DataView dv = new DataView(facJobRate.GetRates(collectionPoint, deliveryPoint, client, chkShowEnded.Checked).Tables[0]);
				string sortExpression = String.Empty;
				sortExpression = SortCriteria + " " + SortDirection;
				dv.Sort = sortExpression.Trim();
				dgRates.DataSource = dv;
			}
			dgRates.DataBind();
		}

		#region DbCombo Events

		

		#endregion

		#region Button Events

		private void btnClearCollectionPoint_Click(object sender, EventArgs e)
		{
            cboCollectionClient.SelectedValue = String.Empty;
			cboCollectionClient.Text = String.Empty;
			m_collectionClientId = 0;

			cboCollectionPoint.SelectedValue = String.Empty;
			cboCollectionPoint.Text = String.Empty;
			m_collectionPointId = 0;

			BindRates();
		}


		private void btnClearDeliveryPoint_Click(object sender, EventArgs e)
		{
            cboDeliveryClient.SelectedValue = String.Empty;
			cboDeliveryClient.Text = String.Empty;
			m_deliveryClientId = 0;

            cboDeliveryPoint.SelectedValue = String.Empty;
			cboDeliveryPoint.Text = String.Empty;
			m_deliveryPointId = 0;

			BindRates();
		}

		private void btnClearJobClient_Click(object sender, EventArgs e)
		{
            cboJobClient.SelectedValue = String.Empty;
			cboJobClient.Text = String.Empty;

			BindRates();
		}

		#endregion

		#region CheckBox Events

		private void chkShowEnded_CheckedChanged(object sender, EventArgs e)
		{
			BindRates();
		}

		#endregion

		#region DataGrid Events

		private void dgRates_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			switch (e.CommandName.ToLower())
			{
				case "end":
					// End the rate's applicability.
					Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditRate);
					using (Facade.IJobRate facJobRate = new Facade.Job())
						facJobRate.EndRate(Convert.ToInt32(e.Item.Cells[0].Text), ((Entities.CustomPrincipal) Page.User).UserName);

					BindRates();
					break;
			}
		}

		private void dgRates_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				DataRowView item = (DataRowView) e.Item.DataItem;

				// Set the point information
				int collectionPointId = (int) item["CollectionPointId"];
				int deliveryPointId = (int) item["DeliveryPointId"];

				HtmlGenericControl collectionSpan = new HtmlGenericControl("span");
				collectionSpan.Attributes.Add("onMouseOver", "javascript:ShowPoint(\"..\\\\Point\\\\GetPointAddressHtml.aspx\", \"" + collectionPointId.ToString() + "\");");
				collectionSpan.Attributes.Add("onMouseOut", "javascript:HidePoint();");
                collectionSpan.InnerText = (string)item["CollectionPoint"];
				e.Item.Cells[2].Controls.Clear();
				e.Item.Cells[2].Controls.Add(collectionSpan);

				HtmlGenericControl deliverySpan = new HtmlGenericControl("span");
				deliverySpan.Attributes.Add("onMouseOver", "javascript:ShowPoint(\"..\\\\Point\\\\GetPointAddressHtml.aspx\", \"" + deliveryPointId.ToString() + "\");");
				deliverySpan.Attributes.Add("onMouseOut", "javascript:HidePoint();");
				deliverySpan.InnerText = (string) item["DeliveryPoint"];
				e.Item.Cells[3].Controls.Clear();
				e.Item.Cells[3].Controls.Add(deliverySpan);

				bool canEdit = false;
				if (item["EndDate"] == DBNull.Value)
					canEdit = true;
				else
				{
					DateTime endDate = (DateTime) item["EndDate"];

					if (endDate >= DateTime.UtcNow)
						canEdit = true;
				}

				e.Item.Cells[8].Enabled = canEdit;
			}		
		}

		private void dgRates_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			if (e.SortExpression == SortCriteria)
			{
				// switch the direction
				if (SortDirection == "ASC")
					SortDirection = "DESC";
				else
					SortDirection = "ASC";
			}
			else
			{
				// apply the new sort
				SortCriteria = e.SortExpression;
				SortDirection = "DESC";
			}

			BindRates();
		}

		#endregion

		#region DBCombo's Server Methods and Initialisation

	
        void cboCollectionClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboCollectionClient.Items.Clear();

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
                cboCollectionClient.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

        void cboDeliveryClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {

            cboDeliveryClient.Items.Clear();

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
                cboDeliveryClient.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

		void cboJobClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {

            cboJobClient.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllJobRelatedFiltered(e.Text);

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
                cboJobClient.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

        void cboCollectionPoint_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            cboCollectionPoint.Items.Clear();
            int identityId = 0;
            string searchText = "";
            if (e.Context["FilterString"] != null && e.Context["FilterString"].ToString() != "")
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
            System.Diagnostics.Debug.Write(e.Value + " " + e.Text);

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
                rcItem.Value = dt.Rows[i]["PointId"].ToString();
                cboCollectionPoint.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

        void cboDeliveryPoint_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            cboDeliveryPoint.Items.Clear();
            int identityId = 0;
            string searchText = "";
            if (e.Context["FilterString"] != null && e.Context["FilterString"].ToString() != "")
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
            System.Diagnostics.Debug.Write(e.Value + " " + e.Text);

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
                rcItem.Value = dt.Rows[i]["PointId"].ToString();
                cboDeliveryPoint.Items.Add(rcItem);
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
			this.Init += new EventHandler(viewrates_Init);
		}
		#endregion
	}
}
