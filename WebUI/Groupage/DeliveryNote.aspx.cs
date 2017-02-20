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
using Orchestrator;
using Telerik.Web.UI;

public partial class Groupage_DeliveryNote : Orchestrator.Base.BasePage
{
    #region Properties

    public int OrderID
    {
        get { return ViewState["vs_orderID"] != null ? (int)ViewState["vs_orderID"] : 0; }
        set { ViewState["vs_orderID"] = value; }
    }

    public int JobID
    {
        get { return ViewState["vs_jobID"] != null ? (int)ViewState["vs_jobID"] : 0; }
        set { ViewState["vs_jobID"] = value; }
    }

    #endregion

    #region PageLoad / OnInit

    protected void Page_Load(object sender, EventArgs e)
    {
        Label PageTitle = ((System.Web.UI.UserControl)(Page.Master)).FindControl("lblWizardTitle") as Label;

        if (PageTitle != null)
            PageTitle.Text = "Delivery Note";
    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        this.grdDeliveries.NeedDataSource += new GridNeedDataSourceEventHandler(grdDeliveries_NeedDataSource);
        this.btnCreate.Click += new EventHandler(btnCreate_Click);
    }

    #endregion

    #region Datagrid

    void grdDeliveries_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    {
        if (!IsPostBack && (!string.IsNullOrEmpty(Request.QueryString["oID"]) && (!string.IsNullOrEmpty(Request.QueryString["jobId"]))))
        {
            int orderID = 0;
            int jobID = 0;
            int.TryParse(Request.QueryString["oID"], out orderID);
            int.TryParse(Request.QueryString["jobId"], out jobID);
            OrderID = orderID;
            JobID = jobID;

            Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();

            if (OrderID > 0)
            {
                grdDeliveries.Columns.FindByUniqueName("OrderID").Visible = true;
                grdDeliveries.DataSource = facOrder.GetDisplayList(OrderID);
            }
            else
            {
                grdDeliveries.Columns.FindByUniqueName("OrderID").Visible = false;
                grdDeliveries.DataSource = facOrder.GetDisplayListForJobID(JobID);
            }
        }
    }

    #endregion

    #region Button

    void btnCreate_Click(object sender, EventArgs e)
    {
        NameValueCollection reportParams = new NameValueCollection();
        DataSet dsDelivery = null;

        string collectDropIDs = string.Empty;

        foreach (GridDataItem item in grdDeliveries.SelectedItems)
        {
            collectDropIDs += item.OwnerTableView.DataKeyValues[item.ItemIndex]["CollectDropID"].ToString();
            collectDropIDs += ",";
        }

        Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();
        dsDelivery = facOrder.GetDeliveryNoteData(collectDropIDs);

        //-------------------------------------------------------------------------------------	
        //									Load Report Section 
        //-------------------------------------------------------------------------------------	
        Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.DeliveryNote;
        Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;
        Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsDelivery;
        Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
        Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";

        // Show the user control
        Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", "<script language=\"javascript\">window.open('" + Page.ResolveClientUrl("../reports/reportviewer.aspx?wiz=true") + "');</script>");
        //Response.Redirect("../reports/reportviewer.aspx?wiz=true");
    }

    #endregion
}
