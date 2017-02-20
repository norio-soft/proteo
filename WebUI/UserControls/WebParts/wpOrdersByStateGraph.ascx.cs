using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.UI.WebControls.WebParts;

namespace Orchestrator.WebUI.WebParts
{

    public partial class wpOrdersByStateGraph : System.Web.UI.UserControl, IWebPart
    {

        public enum eDisplayOption { Last_30_Days, This_Week, This_Month, This_Year };

        public wpOrdersByStateGraph()
        {
            this.Title = "Orders By State ";
            this.Display = eDisplayOption.Last_30_Days;

            this.ShowPending = true;
            this.ShowAwaitingApproval = true;
            this.ShowApproved = true;
            this.ShowDelivered = true;
            this.ShowInvoiced = true;
            this.ShowCancelled = true;
            this.ShowRejected = true;
        }

        #region Public Properties

        [Personalizable(PersonalizationScope.User), WebBrowsable(true)]
        public eDisplayOption Display { get; set; }
        [Personalizable(PersonalizationScope.User), WebBrowsable(true), WebDescription("Show Pending")]
        public bool ShowPending { get; set; }
        [Personalizable(PersonalizationScope.User), WebBrowsable(true), WebDescription("Show Awaiting Approval")]
        public bool ShowAwaitingApproval { get; set; }
        [Personalizable(PersonalizationScope.User), WebBrowsable(true), WebDescription("Show Approved")]
        public bool ShowApproved { get; set; }
        [Personalizable(PersonalizationScope.User), WebBrowsable(true), WebDescription("Show Delivered")]
        public bool ShowDelivered { get; set; }
        [Personalizable(PersonalizationScope.User), WebBrowsable(true), WebDescription("Show Invoiced")]
        public bool ShowInvoiced { get; set; }
        [Personalizable(PersonalizationScope.User), WebBrowsable(true), WebDescription("Show Cancelled")]
        public bool ShowCancelled { get; set; }
        [Personalizable(PersonalizationScope.User), WebBrowsable(true), WebDescription("Show Rejected")]
        public bool ShowRejected { get; set; }

        public string CatalogIconImageUrl { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string TitleIconImageUrl { get; set; }
        public string TitleUrl { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string DataUrl { get; set; }

        #endregion Public Properties

        protected override void OnPreRender(EventArgs e)
        {
            switch (this.Display)
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
            
            this.DataUrl = string.Format(
                "{0}/api/order/statisticsbystate?dateFrom={1:s}&dateTo={2:s}&orderStateIDs={3}",
                ConfigurationManager.AppSettings["ApiBaseUrl"],
                StartDate,
                EndDate,
                this.GetOrderStateIDs());

            this.Title = "Orders By State For " + Display.ToString().Replace("_", " ");
        }

        private void SetWeekDays()
        {
            var currentDate = DateTime.Today;

            while (currentDate.DayOfWeek != DayOfWeek.Monday)
            {
                currentDate = currentDate.AddDays(-1);
            }

            StartDate = currentDate;
            EndDate = currentDate.AddDays(6);
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

        private string GetOrderStateIDs()
        {
            var allOrderStates = Enum.GetValues(typeof(eOrderStatus)).Cast<eOrderStatus>().Where(os => os > 0).ToList();
            var orderStates = new List<eOrderStatus>(allOrderStates.Count);

            if (this.ShowPending)
                orderStates.Add(eOrderStatus.Pending);

            if (this.ShowAwaitingApproval)
                orderStates.Add(eOrderStatus.Awaiting_Approval);

            if (this.ShowApproved)
                orderStates.Add(eOrderStatus.Approved);

            if (this.ShowDelivered)
                orderStates.Add(eOrderStatus.Delivered);

            if (this.ShowInvoiced)
                orderStates.Add(eOrderStatus.Invoiced);

            if (this.ShowCancelled)
                orderStates.Add(eOrderStatus.Cancelled);

            if (this.ShowRejected)
                orderStates.Add(eOrderStatus.Rejected);

            // To prevent display problems if no states are selected then show all.
            if (!orderStates.Any())
                orderStates = allOrderStates;

            return string.Join(",", orderStates.Select(os => (int)os));
        }

    }

}