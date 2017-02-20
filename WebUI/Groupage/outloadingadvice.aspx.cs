using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Groupage
{
public partial class OutloadingAdvice : Orchestrator.Base.BasePage
{
    #region Public Properties
    public List<int> Points
    {
        get
        {
            List<int>  _points = new List<int>();
            foreach (GridItem row in grdOrders.SelectedItems)
            {
                int pointID = int.Parse(row.OwnerTableView.DataKeyValues[row.ItemIndex]["PointID"].ToString());

                _points.Add(pointID);

            }
            return _points;
        }
    }
    #endregion


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            dteCollectionDate.SelectedDate = DateTime.Today;
        }
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        this.btnRefresh.Click += new EventHandler(btnRefresh_Click);
        this.btnGenerate.Click += new EventHandler(btnGenerate_Click);
    }

    void btnGenerate_Click(object sender, EventArgs e)
    {
        PrintOutloadingAdvice();
    }

    void btnRefresh_Click(object sender, EventArgs e)
    {
        this.grdOrders.Rebind();
    }
    
    private void PrintOutloadingAdvice()
    {
        // Detrmine the Outloading Advice Sheets to Print
        List<int> collectionPoints = Points;
        Facade.IOrder facOrder= new Facade.Order();
        DataSet ds = facOrder.GetOrdersForOutloadingAdviceSheet(collectionPoints, dteCollectionDate.SelectedDate.Value);
        

        
        NameValueCollection reportParams = new NameValueCollection();
        //-------------------------------------------------------------------------------------	
        //									Load Report Section 
        //-------------------------------------------------------------------------------------	
        Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.OutloadingAdviceSheet;
        Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;
        Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = ds;
        Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
        Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";

        // Show the user control
        Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", "<script language=\"javascript\">window.open('" + Page.ResolveClientUrl("../reports/reportviewer.aspx?wiz=true") + "');</script>");
        //Response.Redirect("../reports/reportviewer.aspx");
    }
}
}