using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace Orchestrator.WebUI.WebParts
{
    public partial class wpOutstandingPODS : System.Web.UI.UserControl, IWebPart    
    {
        #region Web Part Support

        #region Private fields
        private String _catalogImageUrl = string.Empty;
        private String _description = string.Empty;
        private String _subTitle = "[0]";
        private String _title = "Outstanding PODs (Days)";

        #endregion

        #region Public Properties
        public String CatalogIconImageUrl
        {
            get { return _catalogImageUrl; }
            set { _catalogImageUrl = value; }
        }
        public String Description
        {
            get { return _description; }
            set { _description = value; }
        }
        public String Subtitle
        {
            get { return string.Empty; }
            set { ; }
        }
        public String Title
        {
            get { return _title; }
            set { _title = value; }
        }
        public String TitleIconImageUrl
        {
            get { return string.Empty; }
            set { ; }
        }
        public String TitleUrl
        {
            get { return string.Empty; }
            set { ; }
        }

       
        #endregion

        #endregion


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.grdOverview.RowDataBound += new GridViewRowEventHandler(grdOverview_RowDataBound);
            this.gvDetails.RowDataBound +=new GridViewRowEventHandler(gvDetails_RowDataBound);
            this.btnCancel.Click += new EventHandler(btnCancel_Click);
        }

        void gvDetails_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                 DataRowView drv = ((DataRowView)e.Row.DataItem);
                HtmlAnchor aScanPOD = e.Row.FindControl("aScanPOD") as HtmlAnchor;
                DateTime deliveryDateTime = (DateTime)drv["ArrivalDateTime"];
                string CustomerOrderNumber = drv["CustomerOrderNumber"].ToString();
                int details = (int)drv["CollectDropID"];
                int JobId = (int)drv["JobID"];


                aScanPOD.HRef = @"javascript:OpenNewPODWindow(" + JobId.ToString() + "," + details + ");";
                

            }
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            btnCancel.Visible = false;
            GetOverviewData();
        }

       

        public void lnkCustomer_Click(object sender, EventArgs e)
        {
            LinkButton lnk = sender as LinkButton;
            int identityID = int.Parse(lnk.CommandArgument);
            int age =999;
            switch( lnk.CommandName)
            {
                case "4Days": age = 4; break;
                case "7Days": age = 7; break;
                case "15Days": age = 15; break;
                case "Over15Days": age = 9999; break;
            }
            GetDetailData(identityID, age);
        }

        void grdOverview_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView drv = e.Row.DataItem as DataRowView;
                if ((int)drv["4Days"] == 0)
                {
                    ((LinkButton)e.Row.FindControl("lnk4Days")).Enabled = false;
                    
                }
                if ((int)drv["7Days"] == 0)
                    ((LinkButton)e.Row.FindControl("lnk7Days")).Enabled = false;
                if ((int)drv["15Days"] == 0)
                    ((LinkButton)e.Row.FindControl("lnk15Days")).Enabled = false;
            }
        }

        
        protected void Page_Load(object sender, EventArgs e)
        {
            Trace.Write("Loading PODs");
            if (!IsPostBack)
            {
                GetOverviewData();
            }
        }

        private void GetOverviewData()
        {
            DataAccess.POD dacPOD = new Orchestrator.DataAccess.POD();
            grdOverview.DataSource = dacPOD.GetAgedOutstandingPOD(null, null);
            grdOverview.DataBind();

            gvDetails.Visible = false;
            if (grdOverview.Visible == false)
                grdOverview.Visible = true;
        }


        private void GetDetailData(int identityID, int age)
        {
            DataAccess.POD dacPOD = new Orchestrator.DataAccess.POD();
            gvDetails.DataSource = dacPOD.GetAgedOutstandingPOD(identityID, age);
            gvDetails.DataBind();

            grdOverview.Visible = false;
            gvDetails.Visible = true;
            btnCancel.Visible = true;
        }
    }
}