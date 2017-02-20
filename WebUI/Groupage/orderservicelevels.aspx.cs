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

namespace Orchestrator.WebUI
{
    public partial class OrderServiceLevels : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.grdServiceLevels.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdServiceLevels_NeedDataSource);           
        }

        void grdServiceLevels_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            Orchestrator.Facade.IOrderServiceLevel facOrderServiceLevel = new Orchestrator.Facade.Order();
            DataSet ds = facOrderServiceLevel.GetAll();
            this.grdServiceLevels.DataSource = ds;
        }
    }

}