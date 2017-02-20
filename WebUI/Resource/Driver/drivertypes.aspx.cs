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

public partial class Resource_Driver_drivertypes : Orchestrator.Base.BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        
       
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        this.grdNominalCodes.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdNominalCodes_NeedDataSource);
    }

    void grdNominalCodes_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        Orchestrator.Facade.IDriver facDriver = new Orchestrator.Facade.Resource();
        this.grdNominalCodes.DataSource = facDriver.GetAllDriverTypes();
    }
}
