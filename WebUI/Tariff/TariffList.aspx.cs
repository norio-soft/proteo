using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Tariff
{

    public partial class TariffList : Orchestrator.Base.BasePage
    {

        protected void Page_Init(object sender, EventArgs e)
        {
            btnAdd.Click += btnAdd_Click;
        }

        void grdTariffs_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            LoadTariffs();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                LoadTariffs();
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            this.Response.Redirect("~/Tariff/AddTariff.aspx");
        }

        private void LoadTariffs()
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<Repositories.ITariffRepository>(uow);

                //Get the Tariffs with their latest version
                var tariffs =
                    from t in repo.GetAll()
                    from tv in t.TariffVersions
                    where tv.StartDate == t.TariffVersions.Max(o => o.StartDate)
                    orderby t.Description
                    select new
                    {
                        TariffID = t.TariffID,
                        TariffDescription = t.Description,
                        IsForSubContractor = t.IsForSubContractor,
                        VersionDescription = tv.Description,
                        StartDate = tv.StartDate,
                        FinishDate = tv.FinishDate,
                        ZoneMap = tv.ZoneMap.Description,
                        Scale = tv.Scale.Description
                    };

                //and bind the result to the grid
                grdTariffs.DataSource = tariffs.ToList();
                grdTariffs.DataBind();
            }
        }
        
    }

}
