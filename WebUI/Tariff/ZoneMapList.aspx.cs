using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Tariff
{

    public partial class ZoneMapList : Orchestrator.Base.BasePage
    {

        public class ZoneMapListItem
        {
            public int TariffID { get; set; }
            public string TariffDescription { get; set; }
            public bool IsForSubContractor { get; set; }
            public string VersionDescription { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime? FinishDate { get; set; }
            public string ZoneMap { get; set; }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            btnAdd.Click += btnAdd_Click;
            chkEnabledOnly.CheckedChanged += chkEnabledOnly_CheckedChanged;
            this.Title = "Orchestrator - Zone Map";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                LoadZoneMaps();
            }
        }

        void chkEnabledOnly_CheckedChanged(object sender, EventArgs e)
        {
            LoadZoneMaps();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            this.Response.Redirect("~/Tariff/AddZoneMap.aspx");
        }

        private void LoadZoneMaps()
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<Repositories.IZoneMapRepository>(uow);

                //Get the ZoneMaps with their latest version
                var zoneMaps =
                    from zm in repo.GetAll()
                    where !chkEnabledOnly.Checked || zm.IsEnabled
                    orderby zm.Description
                    select new
                    {
                        zm.ZoneMapID,
                        zm.Description,
                        zm.ZoneType,
                        zm.IsEnabled,
                    };

                //and bind the result to the grid
                grdZoneMaps.DataSource = zoneMaps.ToList();
                grdZoneMaps.DataBind();
            }
        }

    }

}
