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

using System.Collections.Specialized;

using Telerik.Web.UI;
using Orchestrator;

public partial class Groupage_LOADsHEET : Orchestrator.Base.BasePage
{

    #region Page Properties
    private DataSet LoadSheetData
    {
        get { return (DataSet)this.ViewState["_loadSheetData"]; }
        set { this.ViewState["_loadSheetData"] = value; }
    }
    #endregion

    #region Page Init/Load
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        this.grdLoadSheet.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdLoadSheet_NeedDataSource);
        this.grdLoadSheet.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(grdLoadSheet_ItemDataBound);
        
        this.btnPrintLoadSheet.Click += new EventHandler(btnPrintLoadSheet_Click);
        this.btnUpdateDriverInstructions.Click +=new EventHandler(btnUpdateDriverInstructions_Click);
        this.btncancel.Click += new EventHandler(btncancel_Click);
    }

    void btncancel_Click(object sender, EventArgs e)
    {
        Response.Redirect("loadorder.aspx?instructionID=" + Request.QueryString["instructionID"]);
    }

    void btnUpdateDriverInstructions_Click(object sender, EventArgs e)
    {
        int instructionId = int.Parse(Request.QueryString["instructionID"]);

        Orchestrator.Facade.ICollectDropLoadOrder facLoadOrder = new Orchestrator.Facade.CollectDrop();
        facLoadOrder.UpdateDriverInstructions(instructionId, txtDriverNotes.Text, this.Page.User.Identity.Name);

        lblUpdateConfirmation.Visible = true;
        grdLoadSheet.Rebind();
    }

    void btnPrintLoadSheet_Click(object sender, EventArgs e)
    {
        PrintLoadSheet();
    }

    void grdLoadSheet_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            GridDataItem gdi = e.Item as GridDataItem;
            DataRowView drv = (DataRowView)gdi.DataItem;

            if (drv["ToLoad"].ToString().IndexOf(drv["CollectDropID"].ToString()) == -1)
            {
                gdi["CustomerOrderNumber"].Text = "&#160;";
            }

            if (drv["Cross Docked Ex"].ToString().Length > 0)
            {
                gdi["CustomerOrderNumber"].Text = "&#160;";
            }
        }
        
    }

    private void PrintLoadSheet()
    {

        NameValueCollection reportParams = new NameValueCollection();
        reportParams["LoadOn"] = ((DateTime)this.LoadSheetData.Tables[0].Rows[0]["LegPlannedStartDateTime"]).ToString("dd/MM/yy HH:mm");
        reportParams["LoadingVehicle"] = this.LoadSheetData.Tables[0].Rows[0]["LoadingVehicle"].ToString();
        //-------------------------------------------------------------------------------------	
        //									Load Report Section 
        //-------------------------------------------------------------------------------------	
        Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.LoadSheet;
        Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;
        Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = this.LoadSheetData;
        Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
        Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";

        // Show the user control
        Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", "<script language=\"javascript\">window.open('" + Page.ResolveClientUrl("../reports/reportviewer.aspx?wiz=true") + "');</script>");
        //Response.Redirect("../reports/reportviewer.aspx");
    }
    #endregion

    #region grid Event handlers

    void grdLoadSheet_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        int instructionId = int.Parse(Request.QueryString["instructionID"]);
        Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();
        //this.LoadSheetData = facOrder.GetLoadSheet(instructionId);
        this.grdLoadSheet.DataSource = this.LoadSheetData;

        //txtDriverNotes.Text = this.LoadSheetData.Tables[0].Rows[0]["DriverInstructions"].ToString();
    }

    #endregion
}
