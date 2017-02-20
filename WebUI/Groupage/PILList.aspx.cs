using System;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Orchestrator;
using Telerik.Web.UI;
using System.Collections.Generic;

public partial class Groupage_PILList : Orchestrator.Base.BasePage
{
    private DataSet _dsBusinessType = null;

    private DataSet OrderList
    {
        get { return (DataSet)this.ViewState["_orderList"]; }
        set { this.ViewState["_orderList"] = value; }
    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        btnPIL.Click += new EventHandler(btnPIL_Click);
        btnGenerate.Click += new EventHandler(btnGenerate_Click);
        grdLoadOrder.NeedDataSource += new GridNeedDataSourceEventHandler(grdLoadOrder_NeedDataSource);
        grdLoadOrder.ItemDataBound += new GridItemEventHandler(grdLoadOrder_ItemDataBound);
        grdLoadOrder.SortCommand += new GridSortCommandEventHandler(grdLoadOrder_SortCommand);
        btnCreateDeliveryNote.Click += new EventHandler(btnCreateDeliveryNote_Click);
        btnPodLabel.Click += new EventHandler(btnPodLabel_Click);
    }

    void grdLoadOrder_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
    {
        if (OrderList == null) //Use existing dataset if column sorting.
        {
            DateTime startDate = dteStartDeliveryDate.SelectedDate.Value;
            startDate = startDate.Subtract(startDate.TimeOfDay);
            DateTime endDate = dteEndDeliveryDate.SelectedDate.Value;
            endDate = endDate.Subtract(endDate.TimeOfDay);
            endDate = endDate.Add(new TimeSpan(23, 59, 0));

            int businessTypeID = 0;
            bool parsedOK = int.TryParse(cboBusinessType.SelectedValue, out businessTypeID);
            if (parsedOK)
            {
                Orchestrator.Facade.ICollectDropLoadOrder facLoadOrder = new Orchestrator.Facade.CollectDrop();
                OrderList = facLoadOrder.GetLoadOrdersForDateAndBusinessType(startDate, endDate, businessTypeID,
                    cboSearchAgainstDate.Items[0].Selected || cboSearchAgainstDate.Items[2].Selected,
                    cboSearchAgainstDate.Items[1].Selected || cboSearchAgainstDate.Items[2].Selected);

                grdLoadOrder.DataSource = OrderList;
            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            dteStartDeliveryDate.SelectedDate = DateTime.Today.Add(new TimeSpan(24, 0, 0));
            dteEndDeliveryDate.SelectedDate = DateTime.Today.Add(new TimeSpan(71, 59, 0));
            ConfigureDisplay();
        }
    }

    void ConfigureDisplay()
    {
        LoadBusinessTypes();
    }

    #region Events

    #region Buttons

    protected void btnPodLabel_Click(object sender, EventArgs e)
    {
        List<int> orderIds = new List<int>();
        foreach (GridDataItem item in grdLoadOrder.SelectedItems)
            orderIds.Add(int.Parse(item.OwnerTableView.DataKeyValues[item.ItemIndex]["OrderID"].ToString()));

        //Use the Pub-Sub service to Print the POD Labels
        Orchestrator.Facade.PODLabelPrintingService podLabelPrintingService = new Orchestrator.Facade.PODLabelPrintingService();
        bool isPrinted = podLabelPrintingService.PrintPODLabels(orderIds);

        string message;
        if (isPrinted)
            message = "Your POD label has been sent to the Printer";
        else
            message = "The POD Label Printing Service is not available. Please restart the service on your print server and try again.";

        Page.ClientScript.RegisterStartupScript(this.GetType(), "PrintPODLabel", "alert('" + message + "');", true);
    }

    void btnPIL_Click(object sender, EventArgs e)
    {
        LoadPILReport();
    }

    void btnGenerate_Click(object sender, EventArgs e)
    {
        if (this.Page.IsValid)
        {
            int businessTypeID = 0;
            bool parsedOK = int.TryParse(cboBusinessType.SelectedValue, out businessTypeID);
            if (parsedOK)
            {
                this.hidLastSelectedBusinessTypeId.Value = businessTypeID.ToString();
                OrderList = null; //Clear dataset to re-generate.
                this.grdLoadOrder.Rebind();
            }
        }
    }

    void btnCreateDeliveryNote_Click(object sender, EventArgs e)
    {
        NameValueCollection reportParams = new NameValueCollection();
        DataSet dsDelivery = null;

        string OrderIDs = string.Empty;

        foreach (GridDataItem item in grdLoadOrder.SelectedItems)
        {
            OrderIDs += item.OwnerTableView.DataKeyValues[item.ItemIndex]["OrderID"].ToString();
            OrderIDs += ",";
        }

        Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();

        if (!String.IsNullOrEmpty(OrderIDs))
        {
            dsDelivery = facOrder.GetDeliveryNoteDataForOrderIDs(OrderIDs);

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
        }
    }

    #endregion

    #region Grid

    void grdLoadOrder_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            int OrderID = int.Parse(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["OrderID"].ToString());
            if (IsNewLoad(OrderID))
                e.Item.BackColor = System.Drawing.Color.FromArgb(197, 205, 243);

            DataRowView row = (DataRowView)e.Item.DataItem;
            string orderId = row["OrderID"].ToString();

            HtmlAnchor hypOrderId = (HtmlAnchor)e.Item.FindControl("hypOrderId");
            hypOrderId.HRef = string.Format("javascript:{0}", this.dlgOrder.GetOpenDialogScript(string.Format("oid={0}", orderId)));
            hypOrderId.InnerText = orderId.ToString();
        }
    }

    void grdLoadOrder_SortCommand(object source, GridSortCommandEventArgs e)
    {
        ConfigureDisplay();
    }

    #endregion

    #endregion

    #region Private Functions

    private DataSet BusinessTypeDataSet
    {
        get
        {
            if (_dsBusinessType == null)
            {
                Orchestrator.Facade.IBusinessType facBusinessType = new Orchestrator.Facade.BusinessType();
                _dsBusinessType = facBusinessType.GetAll();

                //Set the Primary Key on the DataSet to allow Find to be used
                _dsBusinessType.Tables[0].PrimaryKey = new DataColumn[] { _dsBusinessType.Tables[0].Columns[0] };
            }

            return _dsBusinessType;
        }
    }

    private void LoadBusinessTypes()
    {
        if (this.BusinessTypeDataSet.Tables[0] == null)
            return;

        cboBusinessType.DataSource = this.BusinessTypeDataSet;
        cboBusinessType.DataBind();
        cboBusinessType.Items.Insert(0, "-- Please Select -- ");

        // If there is only one business type then select it by default.
        if (this.BusinessTypeDataSet.Tables[0].Rows.Count == 1)
        {
            cboBusinessType.SelectedIndex = 1;
        }
    }

    private void LoadPILReport()
    {

        NameValueCollection reportParams = new NameValueCollection();
        DataSet dsPIL = null;

        string OrderIDs = string.Empty;

        foreach (GridDataItem item in grdLoadOrder.SelectedItems)
        {
            OrderIDs += item.OwnerTableView.DataKeyValues[item.ItemIndex]["OrderID"].ToString();
            OrderIDs += ",";
        }

        Orchestrator.Facade.ICollectDropLoadOrder facLoadOrder = new Orchestrator.Facade.CollectDrop();

        if (OrderIDs.Length > 0)
        {
            // Set the palletforce fields tab visible if the business type is IsPalletNetwork.
            Orchestrator.Facade.BusinessType facBusType = new Orchestrator.Facade.BusinessType();
            DataSet dsBusinessTypes = this.BusinessTypeDataSet;
            bool isNetwork = false;
            int businessTypeID = int.Parse(this.hidLastSelectedBusinessTypeId.Value);

            foreach (DataRow row in dsBusinessTypes.Tables[0].Rows)
            {
                if (Convert.ToInt32(row["BusinessTypeID"]) == businessTypeID)
                    if (Convert.ToBoolean(row["IsPalletNetwork"]) == true)
                    {
                        isNetwork = true;
                        break;
                    }
            }

            #region Pop-up Report

            eReportType reportType = eReportType.PIL;
            dsPIL = facLoadOrder.GetPILData(OrderIDs.ToString());
            DataView dvPIL;

            if (isNetwork)
            {
                reportType = Orchestrator.Globals.Configuration.PalletNetworkLabelID;

                //Need to duplicate the rows for the Pallteforce labels
                dsPIL.Tables[0].Merge(dsPIL.Tables[0].Copy(), true);
                dvPIL = new DataView(dsPIL.Tables[0], string.Empty, "OrderId, PalletCount", DataViewRowState.CurrentRows);
            }
            else
            {
                dvPIL = new DataView(dsPIL.Tables[0]);
            }

            //-------------------------------------------------------------------------------------	
            //									Load Report Section 
            //-------------------------------------------------------------------------------------	
            Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = reportType;
            Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;
            Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dvPIL;
            Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
            Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";

            // Show the user control
            Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", "<script language=\"javascript\">window.open('" + Page.ResolveClientUrl("../reports/reportviewer.aspx?wiz=true") + "');</script>");
            #endregion

        }
    }

    private bool IsNewLoad(int orderID)
    {
        bool result = false;
        if (this.OrderList == null)
        {
            result = true;
        }
        else
        {
            var x = (from rows in this.OrderList.Tables[0].AsEnumerable()
                     where rows.Field<int>("OrderID") == orderID
                     select rows).ToList();

            result = x.Count > 0;
        }

        return result;
    }

    #endregion
}
