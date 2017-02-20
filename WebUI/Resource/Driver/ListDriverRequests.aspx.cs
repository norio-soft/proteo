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

namespace Orchestrator.WebUI.Resource.Driver
{
	/// <summary>
	/// Summary description for ListDriverRequests.
	/// </summary>
	public partial class ListDriverRequests : Orchestrator.Base.BasePage
	{
		#region Form Elements

		
		




		#endregion

		#region Page Load/Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

			if (!IsPostBack)
			{
				DateTime now = DateTime.UtcNow;
				dteFilterStartDate.SelectedDate = now.Subtract(now.TimeOfDay);

				FilterRequests();
			}

			lblConfirmation.Visible = false;
		}

		private void ListDriverRequests_Init(object sender, EventArgs e)
		{
			btnFilter.Click += new EventHandler(btnFilter_Click);

			dgRequests.SortCommand += new DataGridSortCommandEventHandler(dgRequests_SortCommand);
			dgRequests.ItemCommand += new DataGridCommandEventHandler(dgRequests_ItemCommand);

			btnAddRequest.Click += new EventHandler(btnAddRequest_Click);

            cboDrivers.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboDrivers_ItemsRequested);
		}

        

		#endregion

		#region Property Interfaces

		public string SortCriteria
		{
			get { return (string) ViewState["SortCriteria"]; }
			set { ViewState["SortCriteria"] = value; }
		}

		public string SortDirection
		{
			get { return (string) ViewState["SortDirection"]; }
			set { ViewState["SortDirection"] = value; }
		}

		#endregion

		private void FilterRequests()
		{
			DateTime startDate = dteFilterStartDate.SelectedDate.Value;
			DateTime endDate = DateTime.MaxValue;

			if (dteFilterEndDate.SelectedDate.HasValue)
			{
				endDate = dteFilterEndDate.SelectedDate.Value;
				endDate = endDate.Subtract(endDate.TimeOfDay);
				endDate = endDate.Add(new TimeSpan(0, 23, 59, 59));
			}

			int resourceId = 0;
			if (cboDrivers.SelectedValue != "")
				resourceId = Convert.ToInt32(cboDrivers.SelectedValue);

			Facade.IDriver facDriver = new Facade.Resource();
			DataSet dsRequests = facDriver.GetDriverRequests(startDate, endDate, resourceId, ((Entities.CustomPrincipal) Page.User).IdentityId);

			DataView dvRequests = new DataView(dsRequests.Tables[0]);
			dvRequests.Sort = (SortCriteria + " " + SortDirection).Trim();

			dgRequests.DataSource = dvRequests;
			dgRequests.DataBind();
		}

		#region Button Events
		
		private void btnFilter_Click(object sender, EventArgs e)
		{
			if (Page.IsValid)
			{
				FilterRequests();
			}
		}

		private void btnAddRequest_Click(object sender, EventArgs e)
		{
			if (cboDrivers.SelectedValue == "")
				Response.Redirect("AddUpdateDriverRequest.aspx");
			else
				Response.Redirect("AddUpdateDriverRequest.aspx?resourceId=" + cboDrivers.SelectedValue);
		}

		#endregion

		#region Pretty Data Grid Events

		private void dgRequests_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			HtmlInputHidden hidRequestId = (HtmlInputHidden) e.Item.FindControl("hidRequestId");

			if (hidRequestId != null)
			{
				int requestId = Convert.ToInt32(hidRequestId.Value);

				switch (e.CommandName.ToLower())
				{
					case "edit":
						Response.Redirect("AddUpdateDriverRequest.aspx?requestId=" + requestId.ToString());
						break;
					case "delete":
						Facade.IDriverRequest facDriverRequest = new Facade.Resource();
						if (facDriverRequest.Delete(requestId, ((Entities.CustomPrincipal) Page.User).UserName))
							lblConfirmation.Text = "The request has been deleted.";
						else
							lblConfirmation.Text = "The request has not been deleted.";
						lblConfirmation.Visible = true;

						FilterRequests();
						break;
				}
			}
		}

		private void dgRequests_SortCommand(object source, DataGridSortCommandEventArgs e)
		{
			if (e.SortExpression == SortCriteria)
			{
				if (SortDirection == "DESC")
					SortDirection = "ASC";
				else
					SortDirection = "DESC";
			}
			else
			{
				SortCriteria = e.SortExpression;
				SortDirection = "DESC";
			}

			FilterRequests();
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
			this.Init += new EventHandler(ListDriverRequests_Init);
		}
		#endregion
	}
}
