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
namespace Orchestrator.WebUI.Point
{
	/// <summary>
	/// Summary description for ListPoints.
	/// </summary>
	public partial class ListPoints : Orchestrator.Base.BasePage
	{
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

			if (!IsPostBack)
			{
				int identityId = Convert.ToInt32(Request.QueryString["identityId"]);

				if (identityId > 0)
				{
					cboClient.SelectedValue = identityId.ToString();

					Facade.IOrganisation facOrganisation = new Facade.Organisation();
                    cboClient.Text = facOrganisation.GetForIdentityId(identityId).OrganisationName;

					PopulatePoints();
				}
			}
		}
        
        
        void cboClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {

            cboClient.Items.Clear();

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
                cboClient.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
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
			this.dgPoints.PreRender +=new EventHandler(dgPoints_PreRender);
			this.dgPoints.SortCommand += new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.dgPoints_SortCommand);
            this.cboClient.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);

		}
		#endregion

		private DataSet GetPointData()
		{
			Facade.IPoint facPoint= new Facade.Point();
			DataSet dsPoints= null;;
			string searchCriteria = string.Empty;
			searchCriteria = Request.QueryString["searchCriteria1"];

			dsPoints = facPoint.GetAllForClient(Convert.ToInt32(cboClient.SelectedValue));
					
			//if (dsPoints.Tables[0].Rows.Count ==1)
			//{
			//	Response.Redirect("addupdatepoint.aspx?pointId=" + dsPoints.Tables[0].Rows[0][0].ToString());
			//}
			return dsPoints;
			
		}

		private void PopulatePoints()
		{
			DataSet dsPoints= GetPointData();
			dgPoints.DataSource = dsPoints;
			dgPoints.DataBind();
			pnlPoints.Visible = true;
		}

		private void dgPoints_SortCommand(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{
			DataSet dsPoints = GetPointData();

			//' Retrieve the data source from session state.
			DataTable dt = dsPoints.Tables[0];

			//' Create a DataView from the DataTable.
			DataView  dv = new DataView(dt);

			//' The DataView provides an easy way to sort. Simply set the
			//' Sort property with the name of the field to sort by.
			if(this.SortCriteria == e.SortExpression)
				if (this.SortDir == "desc") 
					this.SortDir = "asc";
				else
					this.SortDir = "desc";
				
			this.SortCriteria = e.SortExpression;

			dv.Sort = e.SortExpression + ' ' + this.SortDir;

			//' Re-bind the data source and specify that it should be sorted
			//' by the field specified in the SortExpression property.
			dgPoints.DataSource = dv;
			dgPoints.DataBind();
		}

		protected string SortDir
		{
			get {return (string)ViewState["sortDir"];}
			set { ViewState["sortDir"] = value;}
		}

		protected string SortCriteria
		{
			get { return (string)ViewState["sortCriteria"];}
			set {ViewState["sortCriteria"] = value;}
		}

		protected string IdentityId
		{
			get { return cboClient.SelectedValue; }
		}

		protected void dgPoints_Page(Object sender, DataGridPageChangedEventArgs e) 
		{
			dgPoints.CurrentPageIndex = e.NewPageIndex;
			PopulatePoints();
		}

		protected void btnAddNewPoint_Click(object sender, System.EventArgs e)
		{
				Response.Redirect("addupdatepoint.aspx?identityId=" + cboClient.SelectedValue + "&organisationName=" + cboClient.Text);
		}

		private void cboClient_ServerChange(object sender, System.EventArgs e)
		{
			PopulatePoints();
		}

		

		protected void btnRefresh_Click(object sender, System.EventArgs e)
		{
			if (Page.IsValid)
				PopulatePoints();
		}


		private void dgPoints_PreRender(object sender, EventArgs e)
		{
			foreach (DataGridItem item in (sender as DataGrid).Items)
			{
				if (item.Cells[2].Text.ToUpper() == "TRUE")
					item.Cells[2].Text = "Yes";
				else if (item.Cells[2].Text.ToUpper() == "FALSE")
					item.Cells[2].Text = "No";

				if (item.Cells[3].Text.ToUpper() == "TRUE")
					item.Cells[3].Text = "Yes";
				else if (item.Cells[3].Text.ToUpper() == "FALSE")
					item.Cells[3].Text = "No";
			}

		}
	}
}
