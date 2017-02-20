using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Telerik.Web.UI;

namespace Orchestrator.WebUI.WebParts
{
    public partial class wpUnallocatedOrders : System.Web.UI.UserControl, IWebPart
    {

        #region Web Part Support

        public string CatalogIconImageUrl { get; set; }
        public string Description { get; set; }
        public string Subtitle { get; set; }
        public string Title { get; set; }
        public string TitleIconImageUrl { get; set; }
        public string TitleUrl { get; set; }
        public string SubTitle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public wpUnallocatedOrders()
            : base()
        {
            this.SubTitle = "[0]";
            this.Title = "Unallocated Orders";
            this.StartDate = DateTime.MinValue;
            this.EndDate = DateTime.MaxValue;
        }

        #endregion Web Part Support

        public string Total { get { return _runningTotal.ToString(); } }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.gvUnallocatedOrders.RowDataBound += new GridViewRowEventHandler(gvUnallocatedOrders_RowDataBound); 
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Trace.Write("wpUnallocatedOrders Loading");
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            Trace.Write("wpUnallocatedOrders Complete");
        }

        private int _runningTotal = 0;
        private void gvUnallocatedOrders_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                _runningTotal += Convert.ToInt32(((System.Data.DataRowView)(e.Row.DataItem)).Row[2]);
            }
        }
    }
}