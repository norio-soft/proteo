using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.administration.ProfitabilityReporting
{

    public partial class JRProfitabilityManageVehicleExclusions : System.Web.UI.Page
    {

        protected int VehicleId;
        protected string VehicleRegNo;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            VehicleId = Convert.ToInt32(this.Request["VehicleId"]);

            this.grdProfitabilityVehicleExclusions.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdProfitabilityVehicleExclusions_NeedDataSource);
            this.btnCancel.Click += new EventHandler(btnCancel_Click);
            this.btnAdd.Click += new EventHandler(btnAdd_Click);
            this.grdProfitabilityVehicleExclusions.ItemCommand += new GridCommandEventHandler(grdProfitabilityVehicleExclusions_ItemCommand);

            if (!IsPostBack)
            {
                using (var uow = DIContainer.CreateUnitOfWork())
                {
                    var repoVehicle = DIContainer.CreateRepository<Repositories.IVehicleRepository>(uow);
                    var vehicle = repoVehicle.FindByVehicleId(VehicleId);
                    VehicleRegNo = vehicle.RegNo;
                    DataBind();
                }
            }
        }

        protected void grdProfitabilityVehicleExclusions_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repoProfit = DIContainer.CreateRepository<Repositories.IProfitReportRunRepository>(uow);
                var profitabilityVehicleExclusions = repoProfit.GetProfitReportVehicleExclusionsByVehicle(VehicleId);
                grdProfitabilityVehicleExclusions.DataSource = profitabilityVehicleExclusions.ToList();
            }
        }

        protected void grdProfitabilityVehicleExclusions_ItemCommand(object source, GridCommandEventArgs e)
        {
            switch (e.CommandName.ToLower())
            {
                case "remove":
                    int profitReportVehicleExclusionID = int.Parse(((GridDataItem)e.Item).GetDataKeyValue("ProfitReportVehicleExclusionId").ToString());

                    using (var uow = DIContainer.CreateUnitOfWork())
                    {
                        var repo = DIContainer.CreateRepository<Repositories.IProfitReportRunRepository>(uow);
                        repo.DeleteProfitReportVehicleExclusion(profitReportVehicleExclusionID);
                    }

                    this.grdProfitabilityVehicleExclusions.Rebind();

                    break;
            }
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("/resource/vehicle/addupdatevehicle.aspx?resourceid={0}", VehicleId));
        }

        void btnAdd_Click(object sender, EventArgs e)
        {
            Response.Redirect(String.Format("/administration/ProfitabilityReporting/JRProfitabilityEditVehicleExclusionPopup.aspx?=0&VehicleId={0}", VehicleId));
        }

    }
}