using System;
using System.Collections;
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
	/// Summary description for PODSearch.
	/// </summary>
	public partial class PODSearch : Orchestrator.Base.BasePage
	{
		#region Form Elements


		#endregion

		#region Page Load/Init/Error

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

			Facade.IUser facUser = new Facade.User();
			SqlDataReader reader = facUser.GetRelatedIdentity(((Entities.CustomPrincipal) Page.User).UserName);
			reader.Read();

			if ((eRelationshipType) reader["RelationshipTypeId"] == eRelationshipType.IsClient)
			{

				cboClient.SelectedValue = Convert.ToString((int)reader["RelatedIdentityId"]);
				Facade.IOrganisation facOrganisation = new Facade.Organisation();
				Entities.Organisation organisation = facOrganisation.GetForIdentityId((int)reader["RelatedIdentityId"]);
				cboClient.Text = organisation.OrganisationName;
				cboClient.Enabled = false;
			}

			// Put user code to initialize the page here
		}

		#endregion

		#region Populate Controls

		private void PerformSearch()
		{
			bool noCriteria = false;
			
			if (cboClient.Text == "" && txtTicketNo.Text == "" && dteSignatureDate.Text == "" && txtCustomerRef.Text == "" && txtDestination.Text == "")
				noCriteria = true;

			Facade.IPOD facPOD = new Facade.POD();

			DataSet dsPODSearchResults = null;

			if (noCriteria)
			{
				dsPODSearchResults = facPOD.GetAll();	
			}
			else
			{
				int clientId = cboClient.SelectedValue == "" ? 0 : int.Parse(cboClient.SelectedValue);
				int loadNo = txtLoadNo.Text == "" ? 0 : int.Parse(txtLoadNo.Text);
				string ticketNo = txtTicketNo.Text == "" ? "" : txtTicketNo.Text;
				string customerReference = txtCustomerRef.Text == "" ? "" : txtCustomerRef.Text;
				string destination = txtDestination.Text == "" ? "" : txtDestination.Text;
				DateTime signatureDate;
				DateTime scannedDate;

				if (dteSignatureDate.SelectedDate.Value == DateTime.MinValue)
				{
					signatureDate = Convert.ToDateTime(Convert.ToString(SqlDateTime.MinValue));
				}
				else
				{
					signatureDate = dteSignatureDate.SelectedDate.Value;
				}
				scannedDate = Convert.ToDateTime(Convert.ToString(SqlDateTime.MinValue));
				//dsPODSearchResults = facPOD.GetWithParams(clientId, ticketNo, signatureDate, scannedDate, scannedDateFrom, scannedDateTo, loadNo, customerReference, destination);
			}
			ViewState["PODSearchResults"] = dsPODSearchResults;
			dgPODSearchResults.DataSource = dsPODSearchResults;

			dgPODSearchResults.DataBind();
		}

		#endregion

		#region Event Handlers

		private void btnEmailFax_Click(object sender, System.EventArgs e)
		{
			ArrayList podsToEmail = (ArrayList)ViewState["PODsToEmail"];

			foreach(DataGridItem dgItem in dgPODSearchResults.Items)
			{
				if (((CheckBox) dgItem.FindControl("chkEmailPOD")).Checked)
					podsToEmail.Add(int.Parse(dgItem.Cells[0].Text));
			}

			string podIdsCSV = String.Empty;
			foreach (Object o in podsToEmail)
			{
				if (!(podIdsCSV == String.Empty))
					podIdsCSV += ",";
				podIdsCSV += (int) o;
			}

			if (podsToEmail.Count > 0) 
			{
				Facade.IPOD facPOD = new Facade.POD();
				LoadReport(podIdsCSV);
			}
		}

		private void btnSearch_Click(object sender, System.EventArgs e)
		{
			PerformSearch();	
		}

		protected void PODSearch_Init(object sender, System.EventArgs e)
		{
			this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
			this.btnEmailFax.Click += new System.EventHandler(this.btnEmailFax_Click);
            this.cboClient.ItemsRequested +=new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
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
			this.Init += new System.EventHandler(this.PODSearch_Init);
		}

		#endregion

		#region DataGrid Event Handlers

		protected void dgPODSearchResults_SortCommand(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{
			Facade.IPOD facPOD = new Facade.POD();
			DataSet dsPODList = (DataSet)ViewState["PODSearchResults"];

			//' Retrieve the data source from session state.
			DataTable dt = dsPODList.Tables[0];

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
			dgPODSearchResults.DataSource = dv;
			dgPODSearchResults.DataBind();
		}
		///	Sort Dir
		///	</summary
		protected string SortDir
		{
			get {return (string)ViewState["sortDir"];}
			set { ViewState["sortDir"] = value;}
		}

		///	<summary> 
		///	Sort Criteria
		///	</summary
		protected string SortCriteria
		{
			get { return (string)ViewState["sortCriteria"];}
			set {ViewState["sortCriteria"] = value;}
		}

		protected void dgPODSearchResults_Page(object sender, DataGridPageChangedEventArgs e)
		{
			dgPODSearchResults.CurrentPageIndex = e.NewPageIndex;
			dgPODSearchResults.DataSource = ViewState["PODSearchResults"];
			dgPODSearchResults.DataBind();
		}

		protected void dgPODSearchResults_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem) 
			{	
				if (e.Item.Cells[9].Text != String.Empty)
				{
					((HyperLink) e.Item.FindControl("lnkViewPOD")).NavigateUrl = @"javascript:OpenFormViewer(" + e.Item.Cells[9].Text + ");";
				}
			}
		}

		protected void dgPODSearchResults_ItemCommand(object sender, DataGridCommandEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem) 
			{	
				Facade.IPOD facPOD = new Facade.POD();
				Facade.IForm facForm = new Facade.Form();
				Entities.POD podToView = facPOD.GetForPODId(int.Parse(e.Item.Cells[0].Text));
				Entities.Scan scan = facForm.GetForScannedFormId(podToView.ScannedFormId);
				byte[] scannedFormImage;
				scannedFormImage = Convert.FromBase64String(scan.ScannedFormImage);
				Response.Clear();
				Response.ContentType = "image/tiff";
				Response.AddHeader("Content-Disposition", "attachment; filename=" + "PODTicketNo" + e.Item.Cells[1].Text + ".tif");
				Response.OutputStream.Write(scannedFormImage, 0, scannedFormImage.Length);
				Response.End();
			}
		}

		#endregion

		#region ActiveReport 

		private void LoadReport(string podIdsCSV)
		{

			// Configure the Session variables used to pass data to the report
			NameValueCollection reportParams = new NameValueCollection();

			reportParams.Add("PODIdsCSV", podIdsCSV);
			//-------------------------------------------------------------------------------------	
			//									Load Report Section 
			//-------------------------------------------------------------------------------------	
			Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.EmailFaxForm;
			Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;
							
			// Show the user control
			reportViewer.Visible = true;
		}

		#endregion

	}
}
