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

namespace Orchestrator.WebUI.PCV
{
	/// <summary>
	/// Summary description for ListPCVsRequiringScan.
	/// </summary>
	public partial class ListPCVsRequiringScan : Orchestrator.Base.BasePage
	{
		#region Form Elements


		#endregion

		#region Page Load/Init/Error

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

			if(!IsPostBack)
				PopulateRequiringScan();
		}

		#endregion

		#region Populate and Display Controls/Elements

		private void PopulateRequiringScan()
		{
			Facade.IPCV facPCV = new Facade.PCV();
			DataSet dsRequiringScan = facPCV.GetRequiringScan();
			dgRequiringScan.DataSource = dsRequiringScan;
			dgRequiringScan.DataBind();
		}

		private void dgRequiringScan_SortCommand(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{
			Facade.IPCV facPCV = new Facade.PCV();
			DataSet dsRequiringScan = facPCV.GetRequiringScan();

			//' Retrieve the data source from session state.
			DataTable dt = dsRequiringScan.Tables[0];

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
			dgRequiringScan.DataSource = dv;
			dgRequiringScan.DataBind();
		}


		private void dgRequiringScan_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			dgRequiringScan.CurrentPageIndex = e.NewPageIndex;
			PopulateRequiringScan();
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
		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			PageNumbersPager1.DataGrid = dgRequiringScan;
			NextBackPager1.DataGrid = dgRequiringScan;
			NextBackPager2.DataGrid = dgRequiringScan;
			Firstlastpager1.DataGrid = dgRequiringScan;
			Firstlastpager2.DataGrid = dgRequiringScan;
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.dgRequiringScan.PageIndexChanged += new System.Web.UI.WebControls.DataGridPageChangedEventHandler(this.dgRequiringScan_PageIndexChanged);
			this.dgRequiringScan.SortCommand += new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.dgRequiringScan_SortCommand);
		}
		#endregion
	}
}
