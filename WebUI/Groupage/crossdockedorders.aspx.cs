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

public partial class Groupage_crossdockedorders : Orchestrator.Base.BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            dteStartDate.SelectedDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01);
            dteEndDate.SelectedDate = DateTime.Today.AddDays(14);
        }
    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        this.btnRefresh.Click += new EventHandler(btnRefresh_Click);
        this.btnExport.Click += new EventHandler(btnExport_Click);
    }

    void btnExport_Click(object sender, EventArgs e)
    {
        //Response.Headers.Clear();
        this.gvOrders2.ExportSettings.OpenInNewWindow = true;

        this.gvOrders2.MasterTableView.ExportToExcel();//"Cross Docked Orders", false, true);
    }

    void btnRefresh_Click(object sender, EventArgs e)
    {
        gvOrders2.Rebind();
    }

    #region Grid Events
    protected string GetDate(DateTime date, bool anytime)
    {
        string retVal = string.Empty;

        if (anytime)
            retVal = date.ToString("dd/MM/yy") + " AnyTime";
        else
            retVal = date.ToString("dd/MM/yy HH:mm");

        return retVal;
    }

    #endregion

}
