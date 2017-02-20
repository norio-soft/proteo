using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Telerik.Web.UI;

namespace Orchestrator.WebUI.WebParts
{
    public partial class wpUnapprovedOrders : System.Web.UI.UserControl, IWebPart
    {

        #region Web Part Support
        #region Private fields
        private String _catalogImageUrl = string.Empty;
        private String _description = string.Empty;
        private String _subTitle = "[0]";
        private String _title = "Unapproved Orders";

        private DateTime _startDate = DateTime.MinValue;
        private DateTime _endDate = DateTime.MaxValue;

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

        public DateTime StartDate
        {
            get { return _startDate; }
            set { _startDate = value; }
        }

        public DateTime EndDate
        {
            set { _endDate = value; }
            get { return _endDate; }
        }

        #endregion

        #endregion

        public string Total { get { return _runningTotal.ToString(); } }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.gvUnapprovedOrders.RowDataBound +=new GridViewRowEventHandler(gvUnapprovedOrders_RowDataBound); 
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Trace.Write("wpUnapprovedOrders Loading");
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            Trace.Write("wpUnapprovedOrders Complete");
        }

        int _runningTotal = 0;
        void gvUnapprovedOrders_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                _runningTotal += Convert.ToInt32(((System.Data.DataRowView)(e.Row.DataItem)).Row[2]);
            }
        }
    }
}