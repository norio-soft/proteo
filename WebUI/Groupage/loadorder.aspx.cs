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

public partial class Groupage_loadorder : Orchestrator.Base.BasePage
{
    #region Page Properties

    private DataSet LoadOrder
    {
        get { return (DataSet)this.ViewState["_loadOrder"]; }
        set { this.ViewState["_loadOrder"] = value; }
    }

    public DataTable DataSource
    {
        get
        {
            object res = this.Session["_ds"];
            if (res == null)
            {
                this.Session["_ds"] = LoadOrder.Tables[3];
            }

            return (DataTable)this.Session["_ds"];
        }
    }

    #endregion

    #region Page Load/Init

    protected void Page_Load(object sender, EventArgs e)
    { }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        this.grdLoadOrder.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdLoadOrder_NeedDataSource);
        this.grdLoadOrder.ItemDataBound += new GridItemEventHandler(grdLoadOrder_ItemDataBound);
        this.grdLoadOrder.ItemCreated += new GridItemEventHandler(grdLoadOrder_ItemCreated);

        this.btnPIL.Click += new EventHandler(btnPIL_Click);
        this.btnLoadSheet.Click += new EventHandler(btnLoadSheet_Click);
        this.btnCancel.Click += new EventHandler(btnCancel_Click);
    }

    #endregion

    #region Private Functions

    #region Data Loading and Display

    protected string GetColour(int orderID)
    {
        if (this.LoadOrder.Tables[2].Select("OrderID = " + orderID.ToString()).Length > 0)
            return "white";

        if (this.LoadOrder.Tables[1].Select("OrderID = " + orderID.ToString()).Length > 0)
            return "silver";

        return "green";
    }

    private int[] GetOrderCount()
    {
        int[] retVal = new int[this.LoadOrder.Tables[3].Rows.Count];
        int x = 1;
        for (int i = 0; i < retVal.Length; i++)
            retVal[i] = i + 1;

        return retVal;
    }

    #endregion

    private void LoadPILReport()
    {
        NameValueCollection reportParams = new NameValueCollection();

        string OrderIDs = string.Empty;

        foreach (GridDataItem gdi in grdLoadOrder.Items)
        {
            CheckBox chk = gdi.FindControl("chkPIL") as CheckBox;
            if (chk.Checked)
            {
                OrderIDs += chk.Attributes["OrderID"] + ",";
            }
        }


        DataSet dsPIL = null;
        Orchestrator.Facade.ICollectDropLoadOrder facLoadOrder = new Orchestrator.Facade.CollectDrop();
        dsPIL = facLoadOrder.GetPILData(OrderIDs);

        //-------------------------------------------------------------------------------------	
        //									Load Report Section 
        //-------------------------------------------------------------------------------------	
        Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.PIL;
        Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;
        Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsPIL;
        Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
        Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";

        // Show the user control
        Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", "<script language=\"javascript\">window.open('" + Page.ResolveClientUrl("../reports/reportviewer.aspx?wiz=true") + "');</script>");
        //Response.Redirect("../reports/reportviewer.aspx");
    }

    private bool IsToBeCrossDocked(int OrderID)
    {
        DataRow[] rows = this.LoadOrder.Tables[3].Select(string.Format("OrderID={0}", OrderID));
        bool retVal = false;
        if (rows.Length == 1)
        {
            retVal = (int)rows[0]["OrderActionID"] == (int)eOrderAction.Cross_Dock;
            // the following check should be there for completeness, however, I have removed this
            // top provide more flexibilty : GRD
            //retVal = retVal & (bool)rows[0]["PlannedForDelivery"];
        }
        return retVal;
    }

    private bool IsNewLoad(int orderID)
    {
        return this.LoadOrder.Tables[1].Select("OrderID = " + orderID.ToString()).Length > 0;
    }

    #endregion

    #region Events

    #region Button

    void btnLoadSheet_Click(object sender, EventArgs e)
    {
        int instructionId = int.Parse(Request.QueryString["instructionID"]);

        Response.Redirect("loadsheet.aspx?instructionID=" + instructionId.ToString());
    }

    void btnPIL_Click(object sender, EventArgs e)
    {
        LoadPILReport();
    }

    void btnCancel_Click(object sender, EventArgs e)
    {
        this.Close();
    }

    #endregion

    #region Grid

    void grdLoadOrder_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        if (this.LoadOrder == null)
        {
            int instructionId = 0;
            int.TryParse(Request.QueryString["instructionID"], out instructionId);

            Orchestrator.Facade.ICollectDropLoadOrder facLoadOrder = new Orchestrator.Facade.CollectDrop();
            this.LoadOrder = facLoadOrder.GetLoadOrderForInstruction(instructionId);
        }
        this.lblAmendedBy.Text = "The Load Order was last amended by <b>" + this.LoadOrder.Tables[0].Rows[0]["LastUpdateUserID"].ToString() + "</b> at <b>" + ((DateTime)this.LoadOrder.Tables[0].Rows[0]["LastUpdateDateTime"]).ToString("dd/MM/yy HH:mm") + "</b>";
        grdLoadOrder.DataSource = this.LoadOrder.Tables[4];
    }

    void grdLoadOrder_ItemCreated(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            CheckBox chk = e.Item.FindControl("chkPIL") as CheckBox;
            chk.Attributes.Add("onclick", "SetPrintPILButtonState(this);");
        }
    }

    void grdLoadOrder_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            TextBox txt = e.Item.FindControl("txtOrder") as TextBox;
            int indx = e.Item.ItemIndex + 1;
            txt.Text = indx.ToString();
            if (indx == this.LoadOrder.Tables[3].Rows.Count)
            {
                txt.Text = "L";
            }

            //int collectDropID = int.Parse(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["OrderID"].ToString());
            int OrderID = int.Parse(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["OrderID"].ToString());
            if (IsNewLoad(OrderID))
            {
                e.Item.BackColor = System.Drawing.Color.FromArgb(197, 205, 243);
            }
            CheckBox chk = e.Item.FindControl("chkPIL") as CheckBox;
            chk.Attributes.Add("OrderID", OrderID.ToString());

            //if (!IsToBeCrossDocked(OrderID))
            //{
            //    chk.Enabled = false;
            //}
        }
    }

    #endregion

    #region PostBack

    protected override void RaisePostBackEvent(IPostBackEventHandler sourceControl, string eventArgument)
    {
        base.RaisePostBackEvent(sourceControl, eventArgument);

        try
        {
            if (sourceControl == grdLoadOrder)
            {
                string[] eventArgumentData = eventArgument.Split(',');
                if (eventArgumentData[0] == "RowMoved")
                {
                    GridItem movedItem = ((grdLoadOrder.MasterTableView.Controls[0] as Table).Rows[int.Parse(eventArgumentData[1])] as GridItem);
                    GridItem beforeItem = ((grdLoadOrder.MasterTableView.Controls[0] as Table).Rows[int.Parse(eventArgumentData[2])] as GridItem);

                    object key1 = grdLoadOrder.MasterTableView.DataKeyValues[movedItem.ItemIndex]["OrderID"];
                    object key2 = grdLoadOrder.MasterTableView.DataKeyValues[beforeItem.ItemIndex]["OrderID"];



                    string currentLoadOrder = this.LoadOrder.Tables[0].Rows[0]["CurrentLoadOrder"].ToString();
                    currentLoadOrder = currentLoadOrder.Replace(key2.ToString(), "??");
                    currentLoadOrder = currentLoadOrder.Replace(key1.ToString(), "!!");
                    currentLoadOrder = currentLoadOrder.Replace("??", key1.ToString());
                    currentLoadOrder = currentLoadOrder.Replace("!!", key2.ToString());

                    Orchestrator.Facade.ICollectDropLoadOrder facLoadOrder = new Orchestrator.Facade.CollectDrop();
                    int instructionId = int.Parse(Request.QueryString["instructionID"]);
                    facLoadOrder.UpdateLoadOrder(instructionId, currentLoadOrder, Page.User.Identity.Name);
                    this.LoadOrder = null;

                    grdLoadOrder.Rebind();
                }
            }
        }
        catch (Exception ex)
        {

        }
    }

    #endregion

    #endregion
}