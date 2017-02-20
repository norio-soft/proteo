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

using Telerik.Web.UI;

namespace Orchestrator.WebUI.Resource.Trailer
{
	/// <summary>
	/// Summary description for TrailerList.
	/// </summary>
	public partial class TrailerList : Orchestrator.Base.BasePage
	{

		private int searchType = 0;	
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

			if (Request.QueryString["searchType"] != null)
				searchType = Convert.ToInt32(Request.QueryString["searchType"]);

            if (!IsPostBack)
            {
                if (Request.QueryString["showAvailable"] != null)
                {
                    dteStartDate.SelectedDate = DateTime.Today;
                    dteEndDate.SelectedDate = DateTime.Today;
                    fsDateFilter.Visible = true;
                    pnlTrailer.Visible = false;
                }
                else
                    PopulateTrailers();
            }
            
            //else if (!IsPostBack)
            //    PopulateTrailers();

			lblNote.Visible = false;
		}

		#region Web Form Designer generated code

		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);

            grdTrailers.NeedDataSource += new GridNeedDataSourceEventHandler(grdTrailers_NeedDataSource);
            grdTrailers.ItemDataBound += new GridItemEventHandler(grdTrailers_ItemDataBound);
            dlgAddUpdateTrailer.DialogCallBack += new EventHandler(dlgAddUpdateTrailer_DialogCallBack);
            btnRefresh.Click += new EventHandler(btnRefresh_Click);
		}

        void btnRefresh_Click(object sender, EventArgs e)
        {
            grdTrailers.Rebind();
        }

        void dlgAddUpdateTrailer_DialogCallBack(object sender, EventArgs e)
        {
            lblNote.Text = this.ReturnValue;
            lblNote.Visible = true;
            PopulateTrailers();
        }

        void grdTrailers_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridCommandItem)
            {
                HtmlInputButton btnAddNewTrailer = e.Item.FindControl("btnAddNewTrailer") as HtmlInputButton;
                btnAddNewTrailer.Attributes.Add("onclick", dlgAddUpdateTrailer.GetOpenDialogScript());
            }
            if (e.Item is GridDataItem)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;

                HtmlAnchor hypAddUpdateTrailer = (HtmlAnchor)e.Item.FindControl("hypAddUpdateTrailer");
                hypAddUpdateTrailer.InnerText = drv["TrailerRef"].ToString();
                hypAddUpdateTrailer.HRef = string.Format("javascript:{0}", dlgAddUpdateTrailer.GetOpenDialogScript("resourceId=" + drv["ResourceId"].ToString()));
            }
        }
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            btnFilter.Click += new EventHandler(btnFilter_Click);
		}

        void btnFilter_Click(object sender, EventArgs e)
        {
            PopulateTrailers();
        }

		#endregion

        void grdTrailers_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            grdTrailers.DataSource = GetTrailerData();
        }

		///	<summary> 
		///	Data Grid Trailers Sort Command
		///	</summary>
		private void PopulateTrailers()
		{
            grdTrailers.Rebind();
		}
	
		///	<summary> 
		///	Data Set Get Trailer Data
		///	</summary>
		private DataSet GetTrailerData()
		{
			DataSet dsTrailers = null;

            if (Request.QueryString["showAvailable"] != null)
            {

                DateTime startDate = DateTime.Parse(dteStartDate.SelectedDate.ToString());
                startDate = startDate.Subtract(startDate.TimeOfDay);

                DateTime endDate = DateTime.Parse(dteEndDate.SelectedDate.ToString());
                endDate = endDate.Subtract(endDate.TimeOfDay).Add(new TimeSpan(23, 59, 59));

                Facade.IResource facTrailer = new Facade.Resource();
                dsTrailers = facTrailer.GetAvailableForDateRange(eResourceType.Trailer, startDate, endDate);

                pnlTrailer.Visible = true;
            }
            else if (!String.IsNullOrEmpty(txtFilterTrailerRef.Text))
            {
                Facade.ITrailer facResource = new Facade.Resource();
                dsTrailers = facResource.GetFiltered(false, txtFilterTrailerRef.Text);
                pnlTrailer.Visible = true;
            }
            else
            {
                Facade.ITrailer facResource = new Facade.Resource();
                dsTrailers = facResource.GetAll(false, false);
                pnlTrailer.Visible = true;
            }

			return dsTrailers;
		}
	}
}
