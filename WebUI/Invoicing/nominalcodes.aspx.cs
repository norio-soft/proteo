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

public partial class Invoicing_nominalcodes : Orchestrator.Base.BasePage
{
    #region Page Load/Init
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        this.grdNominalCodes.NeedDataSource +=new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdNominalCodes_NeedDataSource);
        this.grdDefaultCodes.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdDefaultCodes_NeedDataSource);
    }

    void grdDefaultCodes_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        Orchestrator.Facade.INominalCode facNominalCode = new Orchestrator.Facade.NominalCode();
        DataSet ds = facNominalCode.GetForAllJobType();
        this.grdDefaultCodes.DataSource = ds;

    }

    
    #endregion

    #region Grid Methods
    void grdNominalCodes_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        Orchestrator.Facade.INominalCode facNominalCode = new Orchestrator.Facade.NominalCode();
        DataSet ds = facNominalCode.GetAll();
        this.grdNominalCodes.DataSource = ds;
    }
    #endregion
}
