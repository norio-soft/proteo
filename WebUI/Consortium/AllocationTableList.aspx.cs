using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Consortium
{
    public partial class AllocationTableList : Orchestrator.Base.BasePage
    {

        protected void Page_Init(object sender, EventArgs e)
        {
            grdZoneTables.NeedDataSource += new GridNeedDataSourceEventHandler(grdZoneTables_NeedDataSource);
            grdPointTables.NeedDataSource += new GridNeedDataSourceEventHandler(grdPointTables_NeedDataSource);
            this.Title = "Orchestrator - Allocation Tables";
        }

        private void grdZoneTables_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            grdZoneTables.DataSource =
                from azt in this.DataContext.AllocationZoneTableSet.Include("ZoneMap")
                orderby azt.Description
                select new
                {
                    azt.AllocationZoneTableID,
                    azt.Description,
                    ZoneMapDescription = azt.ZoneMap.Description
                };
        }

        private void grdPointTables_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            grdPointTables.DataSource =
                from apt in this.DataContext.AllocationPointTableSet
                orderby apt.Description
                select new
                {
                    apt.AllocationPointTableID,
                    apt.Description
                };
        }

    }
}
