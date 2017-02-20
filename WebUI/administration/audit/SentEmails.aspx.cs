using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Orchestrator.WebUI.administration.audit
{
    public partial class SentEmails : Orchestrator.Base.BasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Configure the filter date range (one week up to and including today)
                dteStartDate.SelectedDate = DateTime.Today.AddDays(-7);
                dteEndDate.SelectedDate = DateTime.Today;

                // populate the ReportTypes
                List<string> names =new List<string>(Enum.GetNames(typeof(eReportType)));
                var orderedList = names.OrderBy(a => a).ToList();
                orderedList.Insert(0, "All");
                cboReportType.DataSource = orderedList;
                cboReportType.DataBind();
                cboReportType.SelectedIndex = 0;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            btnFilter.Click += btnFilter_Click;
            grdEmails.NeedDataSource += grdEmails_NeedDataSource;
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
                grdEmails.Rebind();
        }

        private void grdEmails_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            Facade.IAudit facAudit = new Facade.Audit();
            int reportType = 0;
            if (!string.IsNullOrWhiteSpace(cboReportType.SelectedValue) && cboReportType.SelectedValue != "All")
            {
                reportType = (int)((eReportType)Enum.Parse(typeof(eReportType), cboReportType.SelectedValue));
            }
            
            var ds = facAudit.GetEmails(dteStartDate.SelectedDate.Value.Date, dteEndDate.SelectedDate.Value.Date,reportType, txtEmailAddress.Text );
            grdEmails.DataSource = ds;
        }

    }
}