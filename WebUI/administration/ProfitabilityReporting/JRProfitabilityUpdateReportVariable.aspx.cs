using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Orchestrator.Repositories.DTOs.ProfitabilityReport;

namespace Orchestrator.WebUI.administration.ProfitabilityReporting
{
    public partial class JRProfitabilityUpdateReportVariable : System.Web.UI.Page
    {
        private int ReportVariableId;
        protected JRProfitabilityReportVariableWithTypeRow jRProfitabilityReportVariableWithTypeRow;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            ReportVariableId = Convert.ToInt32(this.Request["ReportVariableId"]);
            
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<Repositories.IProfitReportRunRepository>(uow);
                jRProfitabilityReportVariableWithTypeRow = repo.GetProfitReportVariablesForId(ReportVariableId);
            }
        }

        /// <summary>
        /// Call overridden OnInit method
        /// </summary>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.grdProfitabilityReportVariable.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdProfitabilityReportVariable_NeedDataSource);
            this.btnBack.Click += new EventHandler(btnBack_Click);
            this.btnSave.Click += new EventHandler(btnSave_Click);
        }

        protected void grdProfitabilityReportVariable_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<Repositories.IProfitReportRunRepository>(uow);

                var profitabilityReportVariable = repo.GetProfitReportVariableHistory(jRProfitabilityReportVariableWithTypeRow.VariableTypeId);

                grdProfitabilityReportVariable.DataSource = profitabilityReportVariable.ToList();
            }
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("JRProfitabilitySelectReportVariable.aspx");
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            ErrorMessage.Text = String.Empty;
            
            if (VariableFromDate.IsEmpty)
            {
                ErrorMessage.Text = "The new variable Valid From date must be set";
                return;
            }

            if (VariableValue.DbValue == null)
            {
                ErrorMessage.Text = "The New Value for the variable must be set";
                return;
            }

            if (Convert.ToDateTime(VariableFromDate.DbSelectedDate) < DateTime.Today)
            {
                ErrorMessage.Text = "The new variable date cannot be set to a date in the past";
                return;
            }

            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<Repositories.IProfitReportRunRepository>(uow);

                repo.CreateProfitReportVariableValue(ReportVariableId, Convert.ToDateTime(VariableFromDate.DbSelectedDate), Convert.ToDecimal(VariableValue.DbValue));
                Response.Redirect("JRProfitabilitySelectReportVariable.aspx");
            }
        }

    }
}