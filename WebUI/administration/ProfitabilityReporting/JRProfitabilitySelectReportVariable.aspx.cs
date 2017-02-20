using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Orchestrator.Repositories.DTOs.ProfitabilityReport;

namespace Orchestrator.WebUI.administration.ProfitabilityReporting
{
    public partial class JRProfitabilitySelectReportVariable : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Call overridden OnInit method
        /// </summary>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.grdProfitabilityReportVariables.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdProfitabilityReportVariables_NeedDataSource);
        }

        protected void grdProfitabilityReportVariables_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<Repositories.IProfitReportRunRepository>(uow);

                var profitabilityReportVariables = repo.GetProfitReportVariablesForDate(DateTime.Today);

                grdProfitabilityReportVariables.DataSource = profitabilityReportVariables.ToList();
            }
        }

    }
}