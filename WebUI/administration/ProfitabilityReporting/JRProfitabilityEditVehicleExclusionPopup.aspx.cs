using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Orchestrator.Models;

namespace Orchestrator.WebUI.administration.ProfitabilityReporting
{
    public partial class JRProfitabilityEditVehicleExclusionPopup : System.Web.UI.Page
    {
        private int ProfitReportVehicleExclusionId;
        private int VehicleId;
        protected string VehicleRegNo;

        protected ProfitReportVehicleExclusion VehicleExclusion;
        protected IDictionary<eExclusionType, string> VehicleExclusionTypes;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            this.btnCancel.Click += new EventHandler(btnCancel_Click);
            this.btnSave.Click += new EventHandler(btnSave_Click);

            ProfitReportVehicleExclusionId = Convert.ToInt32(this.Request["ProfitReportVehicleExclusionId"]);
            VehicleId = Convert.ToInt32(this.Request["VehicleId"]);

            ErrorMessage.Text = "";

            if (!IsPostBack)
            {
                using (var uow = DIContainer.CreateUnitOfWork())
                {
                    var repo = DIContainer.CreateRepository<Repositories.IProfitReportRunRepository>(uow);

                    var repoVehicle = DIContainer.CreateRepository<Repositories.IVehicleRepository>(uow);
                    var vehicle = repoVehicle.FindByVehicleId(VehicleId);
                    VehicleRegNo = vehicle.RegNo;
                    DataBind();

                    VehicleExclusionTypes = repo.AllVehicleExclusionTypeDescriptions();

                    cboVehicleExclusionType.DataSource = VehicleExclusionTypes;
                    cboVehicleExclusionType.DataBind();

                    if (ProfitReportVehicleExclusionId > 0)
                    {
                        VehicleExclusion = repo.GetProfitReportVehicleExclusionById(ProfitReportVehicleExclusionId);
                        dtVariableExclusionFromDate.SelectedDate = VehicleExclusion.VariableExclusionFromDate;
                        dtVariableExclusionToDate.SelectedDate = VehicleExclusion.VariableExclusionToDate;
                        cboVehicleExclusionType.SelectedValue = VehicleExclusion.VehicleExclusionType.ToString();
                    }

                    cboVehicleExclusionType.DataBind();
                }
            }
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(BackToPage);
        }

        void btnSave_Click(object sender, EventArgs e)
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<Repositories.IProfitReportRunRepository>(uow);

                string errMsg;
                if (!repo.ValidateProfitabilityExclusion(ProfitReportVehicleExclusionId, VehicleId, (eExclusionType)Enum.Parse(typeof(eExclusionType), cboVehicleExclusionType.SelectedValue), dtVariableExclusionFromDate.SelectedDate, dtVariableExclusionToDate.SelectedDate, out errMsg))
                {
                    ErrorMessage.Text = errMsg;
                    return;
                }

                repo.SaveProfitabilityExclusion(ProfitReportVehicleExclusionId, VehicleId, (eExclusionType)Enum.Parse(typeof(eExclusionType), cboVehicleExclusionType.SelectedValue), dtVariableExclusionFromDate.SelectedDate.Value, dtVariableExclusionToDate.SelectedDate);

                Response.Redirect(BackToPage);
            }
        }

        private string BackToPage
        {
            get
            {
                return String.Format("/administration/ProfitabilityReporting/JRProfitabilityManageVehicleExclusions.aspx?=0&VehicleId={0}", VehicleId);
            }
        }

    }
}