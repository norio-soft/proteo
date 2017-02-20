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

using System.Web.Caching;

namespace Orchestrator.WebUI.WebParts
{
    public partial class wpOrdersByState : System.Web.UI.UserControl, IWebPart
    {

        #region WebPart Support
        #region Private fields
        private String _catalogImageUrl = string.Empty;
        private String _description = string.Empty;
        private String _subTitle = string.Empty;
        private String _title = "Orders By State";

        private DateTime _startDate = DateTime.MinValue;
        private DateTime _endDate = DateTime.MaxValue;
        public enum eDisplayOption { Last_30_Days, This_Week, This_Month, This_Year };
        private eDisplayOption _displayOption = eDisplayOption.Last_30_Days;

        private bool _awaitingApproval = true;
        private bool _approved = true;
        private bool _pending = true;
        private bool _delivered = true;
        private bool _invoiced = true;
        private bool _cancelled = true;

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
        // WHich States to Show
        [Personalizable(PersonalizationScope.User), WebBrowsable(true), WebDescription("Show Pending")]
        public bool ShowPending
        {
            get { return _pending; }
            set { _pending = value; }
        }

        [Personalizable(PersonalizationScope.User), WebBrowsable(true), WebDescription("Show Awaiting Approval")]
        public bool ShowAwaitingApproval
        {
            get { return _awaitingApproval; }
            set { _awaitingApproval = value; }
        }

        [Personalizable(PersonalizationScope.User), WebBrowsable(true), WebDescription("Show Delivered")]
        public bool ShowDelivered
        {
            get { return _delivered; }
            set { _delivered = value; }
        }

        [Personalizable(PersonalizationScope.User), WebBrowsable(true), WebDescription("Show Approved")]
        public bool ShowApproved
        {
            get { return _approved; }
            set { _approved = value; }
        }

        [Personalizable(PersonalizationScope.User), WebBrowsable(true), WebDescription("Show Invoiced")]
        public bool ShowInvoiced
        {
            get { return _invoiced; }
            set { _invoiced = value; }
        }

        [Personalizable(PersonalizationScope.User), WebBrowsable(true), WebDescription("Show Cancelled")]
        public bool ShowCancelled
        {
            get { return _cancelled; }
            set { _cancelled = value; }
        }

        [Personalizable(PersonalizationScope.User), WebBrowsable(true)]
        public eDisplayOption Display
        {
            get { return _displayOption; }
            set { _displayOption = value; DisplayChart(); }
        }

        #endregion
        #endregion


        protected void Page_Load(object sender, EventArgs e)
        {
            Trace.Write("wpOrdersByState Loading");
            //The following code is used for the new javascript dashboard
            if (this.Attributes["mode"] != null)
            {
                int mode = int.Parse(this.Attributes["mode"]);
                switch (mode)
                {
                    case 1:
                        _displayOption = eDisplayOption.Last_30_Days;
                        break;
                    case 2:
                        _displayOption = eDisplayOption.This_Week;
                        break;
                    case 3:
                        _displayOption = eDisplayOption.This_Month;
                        break;
                    case 4:
                        _displayOption = eDisplayOption.This_Year;
                        break;
                }
            }
            DisplayChart();
            Trace.Write("wpOrdersByState Complete");
            
        }

        private void DisplayChart()
        {
            switch (Display)
            {
                case eDisplayOption.This_Week:
                    SetWeekDays();
                    break;
                case eDisplayOption.This_Month:
                    SetMonthDays();
                    break;
                case eDisplayOption.This_Year:
                    StartDate = new DateTime(DateTime.Today.Year, 01, 01, 00, 00, 00);
                    EndDate = new DateTime(DateTime.Today.Year, 12, 31, 23, 59, 59);
                    break;
                case eDisplayOption.Last_30_Days:
                    SetLast30Days();
                    break;
            }

            DataSet dsKPI = GetData();
           
            this.Title = "Orders By State For " + Display.ToString().Replace("_", " ");

            gvOrdersByState.DataSource = dsKPI;
            gvOrdersByState.DataBind();


        }

        private void SetWeekDays()
        {
            DateTime currentDate = DateTime.Today;

            while (currentDate.DayOfWeek != DayOfWeek.Monday)
            {
                currentDate = currentDate.AddDays(-1);
            }
#if DEBUG 
            StartDate = new DateTime(2006,11,01,00,00,00);
            EndDate = new DateTime(2006,11,07,23,59,59);
#else
            StartDate = currentDate;
            EndDate = currentDate.AddDays(6);
#endif

        }

        private void SetMonthDays()
        {
            StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01);
            EndDate = new DateTime(StartDate.Year, StartDate.Month, new System.Globalization.GregorianCalendar().GetDaysInMonth(StartDate.Year, StartDate.Month));
        }

        private void SetLast30Days()
        {
            StartDate = DateTime.Today.AddDays(-30);
            EndDate = DateTime.Today;
        }

        private DataSet GetData()
        {

            string orderStates = GetOrderStates();
            string cacheKey = this.UniqueID + orderStates + StartDate.ToString() + "||" + EndDate.ToString();

            DataSet dsKPI = null;
            if (Cache[cacheKey] == null)
            {
                Orchestrator.Facade.IKPI facKPI = new Orchestrator.Facade.KPI();
                dsKPI = facKPI.GetOrdersByStateForPeriod(StartDate, EndDate, orderStates);
                Cache.Add(cacheKey, dsKPI, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 15, 00), CacheItemPriority.Normal, null);
            }
            else
            {
                dsKPI = (DataSet)Cache[cacheKey];
            }

            return dsKPI;

        }

        private string GetOrderStates()
        {
            string OrderStates = string.Empty;

            if (ShowAwaitingApproval) { OrderStates = ((int)eOrderStatus.Awaiting_Approval).ToString(); }
            if (ShowApproved) 
            {
                if (OrderStates.Length > 0) 
                    OrderStates += "," + ((int)eOrderStatus.Approved).ToString(); 
                   ((int)eOrderStatus.Approved).ToString();
            }
            if (ShowDelivered)
            {
                if (OrderStates.Length > 0)
                    OrderStates += "," + ((int)eOrderStatus.Delivered).ToString();
                ((int)eOrderStatus.Delivered).ToString();
            }

            if (ShowInvoiced)
            {
                if (OrderStates.Length > 0)
                    OrderStates += "," + ((int)eOrderStatus.Invoiced).ToString();
                ((int)eOrderStatus.Invoiced).ToString();
            }

            if (ShowCancelled)
            {
                if (OrderStates.Length > 0)
                    OrderStates += "," + ((int)eOrderStatus.Cancelled).ToString();
                ((int)eOrderStatus.Cancelled).ToString();
            }


            // to prevent display problems if no states are selected then show all.
            if (OrderStates.Length == 0)
                OrderStates = "1,2,3,4,5,6";
            return OrderStates;
        }
    }
}
