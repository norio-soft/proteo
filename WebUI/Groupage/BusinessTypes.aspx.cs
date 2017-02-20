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
    public partial class BusinessTypes : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.grdBusinessTypes.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdBusinessTypes_NeedDataSource);
            btnAddBusinessType.Click += new EventHandler(btnAddBusinessType_Click);
            btnRefresh.Click += new EventHandler(btnRefresh_Click);
        }

        void btnAddBusinessType_Click(object sender, EventArgs e)
        {
            Response.Redirect("managebusinesstype.aspx");
        }

        void btnRefresh_Click(object sender, EventArgs e)
        {
            grdBusinessTypes.Rebind();
        }

        void grdBusinessTypes_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            Facade.IBusinessType facBusinessType = new Facade.BusinessType();
            DataSet ds = facBusinessType.GetAll();
            if (!e.IsFromDetailTable)
            {
                grdBusinessTypes.MasterTableView.DataSource = ds.Tables[0];
            }

            grdBusinessTypes.MasterTableView.DetailTables[0].DataSource = ds.Tables[1];
            
            

        }
    }
}