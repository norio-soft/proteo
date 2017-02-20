using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Globalization;
using System.IO;
using System.Data;
using System.Web.UI.HtmlControls;

namespace Orchestrator.WebUI.Invoicing
{
    public partial class InvoicingImport : Orchestrator.Base.BasePage
    {
        //------------------------------------------------------------------------------------

        protected class ResultType
        {
            public int OrderID { get; set; }
            public int ImportedInvoiceItemID { get; set; }
            public string OrderStatus { get; set; }
            public string Ref1 { get; set; }
            public string Ref2 { get; set; }
            public string Ref3 { get; set; }
            public bool CanBeInvoiced { get; set; }
            public Decimal DifferenceAmount { get; set; }
            public Decimal DifferenceWeightAmount { get; set; }
            public int OrderCount { get; set; }
            public IGrouping<object, DataRow> Items { get; set; }
        }

        //------------------------------------------------------------------------------------

        #region Properties

        private DataSet _dsBusinessType;
        private DataSet BusinessTypeDataSet
        {
            get
            {
                if (_dsBusinessType == null)
                {
                    Facade.IBusinessType facBusinessType = new Facade.BusinessType();
                    _dsBusinessType = facBusinessType.GetAll();

                    //Set the Primary Key on the DataSet to allow Find to be used
                    _dsBusinessType.Tables[0].PrimaryKey = new DataColumn[] { _dsBusinessType.Tables[0].Columns[0] };
                }

                return _dsBusinessType;
            }
        }

        #endregion

        //------------------------------------------------------------------------------------

        #region Exposed Methods

        protected CultureInfo GetCulture(int? lcid)
        {
            if (!lcid.HasValue)
                return new CultureInfo(Orchestrator.Globals.Configuration.NativeCulture);
            else
                return new CultureInfo((int)lcid);
        }

        #endregion

        //------------------------------------------------------------------------------------

        #region Events

        //------------------------------------------------------------------------------------

        #region Page

        //------------------------------------------------------------------------------------

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                ConfigureDisplay();
        }

        //------------------------------------------------------------------------------------

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.cboFile.SelectedIndexChanged += new Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventHandler(cboFile_SelectedIndexChanged);
            this.cboBusinessType.SelectedIndexChanged += new EventHandler(cboBusinessType_SelectedIndexChanged);
            this.btnRefreshInvoices.Click += new EventHandler(btnRefreshInvoices_Click);
            this.btnRefreshInvoiceBottom.Click += new EventHandler(btnRefreshInvoices_Click);
            this.btnRefreshFiles.Click += new EventHandler(btnRefreshFiles_Click);
            this.btnDelete.Click += new EventHandler(btnDelete_Click);
            this.btnCreatePreInvoice.Click += new EventHandler(btnCreatePreInvoice_Click);
            this.btnCreatePreInvoiceBottom.Click += new EventHandler(btnCreatePreInvoice_Click);
            this.lvPreInvoiceItems.ItemDataBound += new EventHandler<ListViewItemEventArgs>(lvPreInvoiceItems_ItemDataBound);
            this.lvAdditional.ItemDataBound += new EventHandler<ListViewItemEventArgs>(lvAdditional_ItemDataBound);
            this.dlgMatchOrder.DialogCallBack += new EventHandler(dlgMatchOrder_DialogCallBack);
            this.dlgOrder.DialogCallBack += new EventHandler(dlgOrder_DialogCallBack);
            this.btnExport.Click += new EventHandler(btnExport_Click);
            this.btnExportBottom.Click += new EventHandler(btnExport_Click); 
        }

        //------------------------------------------------------------------------------------

        #endregion

        //------------------------------------------------------------------------------------

        #region ListViews

        //------------------------------------------------------------------------------------

        protected void lvAdditional_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            ListViewDataItem lv = e.Item as ListViewDataItem;
            DataRow dr = lv.DataItem as DataRow;

            if (lv.ItemType == ListViewItemType.DataItem)
            {
                CheckBox chkExclude = lv.FindControl("chkExcludeAdditional") as CheckBox;
                if (chkExclude != null)
                {
                    chkExclude.Attributes.Add("OrderId", dr["OrderId"].ToString());
                    chkExclude.Attributes.Add("OrderStatus", dr["OrderStatus"].ToString());

                    if (dr["PreinvoiceId"] == DBNull.Value && dr["InvoiceId"] == DBNull.Value)
                        chkExclude.Attributes.Add("CanBeInvoiced", true.ToString());
                    else
                        chkExclude.Attributes.Add("CanBeInvoiced", false.ToString());
                }

                HtmlTableCell statusCell = lv.FindControl("statusCell") as HtmlTableCell;

                if (dr["InvoiceId"] != DBNull.Value || dr["PreinvoiceId"] != DBNull.Value)
                    if (statusCell != null)
                    {
                        statusCell.BgColor = "#F6FF00"; //yellow - invoiced
                        statusCell.InnerText = "I";
                    }
            }
        }

        //------------------------------------------------------------------------------------

        protected void lvItems_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            int DifferenceAmountIndex = 18, DifferenceWeightIndex = 19;
            string highlightChargeCell = "ChargesCell", highlightWeightCell = "WeightsCell";
            ListViewDataItem lv = e.Item as ListViewDataItem;
            DataRow dr = lv.DataItem as DataRow;
            HtmlTableCell statusCell = lv.FindControl("statusCell") as HtmlTableCell;
            HtmlAnchor lnkOrder = lv.FindControl("lnkOrder") as HtmlAnchor;

            if (lv.ItemType == ListViewItemType.DataItem)
            {
                this.HighlightDifference(lv, highlightChargeCell, DifferenceAmountIndex);
                this.HighlightDifference(lv, highlightWeightCell, DifferenceWeightIndex);

                if (lnkOrder != null)
                    if (dr["OrderID"] == DBNull.Value)
                    {
                        if (statusCell != null)
                        {
                            statusCell.BgColor = "#FC445D"; //red - unmatched
                            statusCell.InnerText = "U";
                        }

                        lnkOrder.InnerText = "Match";
                        lnkOrder.HRef = "javascript:matchOrder(" + dr["ImportedInvoiceItemID"] + ", " + dr["Ref2"] +")";
                    }
                    else
                    {
                        lnkOrder.InnerText = dr["OrderID"].ToString();
                        lnkOrder.HRef = "javascript:viewOrderProfile(" + dr["OrderID"] + ")";

                        if (dr["InvoiceId"] != DBNull.Value || dr["PreinvoiceId"] != DBNull.Value)
                            if (statusCell != null)
                            {
                                statusCell.BgColor = "#F6FF00"; //yellow - invoiced
                                statusCell.InnerText = "I";
                            }
                    }
            }
        }

        //------------------------------------------------------------------------------------

        protected void lvPreInvoiceItems_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            ListViewDataItem lv = e.Item as ListViewDataItem;
            ResultType dr = lv.DataItem as ResultType;

            if (lv.ItemType == ListViewItemType.DataItem)
            {
                CheckBox chkExclude = lv.FindControl("chkExclude") as CheckBox;
                if (chkExclude != null)
                {
                    chkExclude.Attributes.Add("OrderId", dr.OrderID.ToString());
                    chkExclude.Attributes.Add("OrderStatus", dr.OrderStatus);
                    chkExclude.Attributes.Add("CanBeInvoiced", dr.CanBeInvoiced.ToString());
                    chkExclude.Attributes.Add("ImportedInvoiceItemId", dr.ImportedInvoiceItemID.ToString());
                }
            }
        }

        //------------------------------------------------------------------------------------

        #endregion

        //------------------------------------------------------------------------------------

        #region Buttons

        //------------------------------------------------------------------------------------

        protected void btnExport_Click(object sender, EventArgs e)
        {
            int importedInvoiceId = 0;
            int.TryParse(this.cboFile.SelectedValue, out importedInvoiceId);

            int businessTypeId = 0;
            int.TryParse(cboBusinessType.SelectedValue, out businessTypeId);

            List<int> selectedOrderIDs = new List<int>();
            selectedOrderIDs = this.GetOrderIdsForExport();

            List<int> importedInvoiceItemIDsToExclude = new List<int>();
            importedInvoiceItemIDsToExclude = this.GetImportedInvoiceItemIDsToExclude();

            if (selectedOrderIDs.Count > 0)
            {
                Facade.ImportedInvoice facimportedInvoice = new Facade.ImportedInvoice();
                DataSet ds = facimportedInvoice.GetOrdersForMatchingExport(importedInvoiceId, businessTypeId, selectedOrderIDs, importedInvoiceItemIDsToExclude);

                Facade.ExtraType facExtraTypes = new Orchestrator.Facade.ExtraType();
                List<Entities.ExtraType> extraTypes = facExtraTypes.GetForIsEnabled(true);

                foreach (Entities.ExtraType et in extraTypes)
                    ds.Tables[0].Columns.Add("[" + et.Description + "]", typeof(decimal));

                // Extra data
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    DataRow[] extraRows = ds.Tables[1].Select("OrderID = " + dr["OrderID"]);
                    foreach (DataRow extra in extraRows)
                        if (dr.Table.Columns.Contains("[" + extra["ExtraType"] + "]"))
                            dr["[" + extra["ExtraType"] + "]"] = extra["ExtraAmount"]; 
                }

                Session["__ExportDS"] = ds.Tables[0];
                Response.Redirect("../reports/csvexport.aspx?filename=InvoiceMatchingExport.CSV");
            }
            else
                this.lblStatus.Text = "0 Orders could be exported.";
        }

        //------------------------------------------------------------------------------------

        protected void btnCreatePreInvoice_Click(object sender, EventArgs e)
        {
            Orchestrator.Facade.IPreInvoice facPreInvoice = new Orchestrator.Facade.PreInvoice();
            DateTime invoiceDate = DateTime.Today;
            string userName = Page.User.Identity.Name;
            string batchRef = this.txtBatchRef.Text;
            List<int> selectedOrderIDs = new List<int>();
            selectedOrderIDs = this.GetOrderIdsForPreInvoice();

            if (selectedOrderIDs.Count > 0)
            {
                // Update the orders that have been flagged.
                Facade.IOrder facOrder = new Facade.Order();
                if (facOrder.FlagAsReadyToInvoice(selectedOrderIDs, userName))
                {
                    int batchID = facPreInvoice.CreateBatch(invoiceDate, selectedOrderIDs, userName);

                    try
                    {
                        if (batchID > 0)
                        {
                            // Kick off the workflow.
                            GenerateInvoiceClient gic = new GenerateInvoiceClient("Orchestrator.InvoiceService");
                            gic.GenerateGroupageInvoiceAutoRun(batchID, batchRef, new Orchestrator.Contracts.DataContracts.NotificationParty[] { }, String.Empty, String.Empty, userName);

                            this.RebindGrid();
                        }
                    }
                    catch (System.ServiceModel.EndpointNotFoundException exc)
                    {
                        // Not possible to send message to workflow host - send email to support.
                        Utilities.SendSupportEmailHelper("GenerateInvoiceClient.GenerateGroupageInvoiceAutoRun(int, Orchestrator.Entities.NotificationParty[], string)", exc);
                        //this.lblError.Text = exc.Message;
                        this.lblStatus.Text = "Error creating invoice: " + exc.Message;
                    }
                }
                else
                    this.lblStatus.Text = "Not all Orders could be flagged as ready to invoice.";
            }
            else
                this.lblStatus.Text = "0 Orders could be invoiced.";
        }

        //------------------------------------------------------------------------------------

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            int importedInvoiceId = 0;
            int.TryParse(this.cboFile.SelectedValue,out importedInvoiceId);
            if (importedInvoiceId > 0)
            {
                Facade.ImportedInvoice facImportedInvoice = new Orchestrator.Facade.ImportedInvoice();
                facImportedInvoice.DeleteImportedInvoiceFile(importedInvoiceId);

                this.LoadFiles();
                this.RebindGrid();
            }
        }

        //------------------------------------------------------------------------------------

        protected void btnRefreshFiles_Click(object sender, EventArgs e)
        {
            this.LoadFiles();

            this.cboFile.Text = String.Empty;
            this.cboFile.SelectedValue = String.Empty;
            this.txtBatchRef.Text = String.Empty;
            this.lblFromSystem.Text = String.Empty;

            this.RebindGrid();
        }

        //------------------------------------------------------------------------------------

        protected void btnRefreshInvoices_Click(object sender, EventArgs e)
        {
            this.RebindGrid();
        }

        //------------------------------------------------------------------------------------

        #endregion

        //------------------------------------------------------------------------------------

        #region combos

        //------------------------------------------------------------------------------------

        protected void cboBusinessType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.RebindGrid();
        }

        //------------------------------------------------------------------------------------

        protected void cboFile_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if(!String.IsNullOrEmpty(this.cboFile.SelectedValue))
            {
                int selectedValue = int.Parse(this.cboFile.SelectedValue);
                this.RebindGrid();
                EF.ImportedInvoice importedInvoices = (from ii in EF.DataContext.Current.ImportedInvoiceSet
                                                       where ii.ImportedInvoiceID == selectedValue
                                                       select ii).FirstOrDefault();

                if (importedInvoices != null)
                {
                    this.lblFromSystem.Text = importedInvoices.FromSystem;

                    if (String.IsNullOrEmpty(this.txtBatchRef.Text))
                        this.txtBatchRef.Text = importedInvoices.BatchRef;
                }
            }
        }

        //------------------------------------------------------------------------------------

        #endregion 
        
        //------------------------------------------------------------------------------------

        protected void dlgMatchOrder_DialogCallBack(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(this.dlgMatchOrder.ReturnValue))
            {
                string[] ids = this.dlgMatchOrder.ReturnValue.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                int importedInvoiceItemId = -1;
                int orderId = -1;
                int.TryParse(ids[0], out importedInvoiceItemId);
                int.TryParse(ids[1], out orderId);
                
                EF.ImportedInvoiceItem iii = (from i in DataContext.ImportedInvoiceItemSet
                                              where i.ImportedInvoiceItemID == importedInvoiceItemId
                                              select i).FirstOrDefault();

                if (iii != null)
                {
                    EF.Order order = (from o in EF.DataContext.Current.OrderSet
                                      where o.OrderId == orderId
                                      select o).FirstOrDefault();

                    if (order != null)
                    {
                        iii.Order = order;
                        iii.OrderReference.EntityKey = Orchestrator.EF.DataContext.CreateKey("OrderSet", "OrderId", order.OrderId);

                        EF.DataContext.Current.SaveChanges();
                        this.RebindGrid();
                    }
                }
            }
        }

        //------------------------------------------------------------------------------------

        protected void dlgOrder_DialogCallBack(object sender, EventArgs e)
        {
            if(!String.IsNullOrEmpty(this.dlgOrder.ReturnValue))
                this.RebindGrid();
        }

        //------------------------------------------------------------------------------------

        #endregion

        //------------------------------------------------------------------------------------

        #region Methods

        //------------------------------------------------------------------------------------

        protected bool HighlightDifference(ListViewDataItem lv, string cellName, int index)
        {
            bool highlighted = false;
            DataRow dr = lv.DataItem as DataRow;
            if (dr != null)
                if (dr.ItemArray[index] != System.DBNull.Value && Convert.ToDecimal(dr.ItemArray[index]) != 0)
                {
                    HtmlTableCell highlightCell = lv.FindControl(cellName) as HtmlTableCell;
                    if (highlightCell != null)
                    {
                        highlightCell.BgColor = "#c0e4ff";
                        highlighted = true;
                    }
                }

            return highlighted;
        }

        //------------------------------------------------------------------------------------

        protected bool HighlightDifference(ListViewDataItem lv, string cellName, int index, string colour)
        {
            bool highlighted = false;
            DataRow dr = lv.DataItem as DataRow;
            if (dr != null)
                if (dr.ItemArray[index] != System.DBNull.Value && Convert.ToDecimal(dr.ItemArray[index]) > 0)
                {
                    HtmlTableCell highlightCell = lv.FindControl(cellName) as HtmlTableCell;
                    if (highlightCell != null)
                    {
                        highlightCell.BgColor = colour;
                        highlighted = true;
                    }
                }

            return highlighted;
        }

        //------------------------------------------------------------------------------------

        private List<int> GetOrderIdsForPreInvoice()
        {
            List<int> orderIds = new List<int>();

            // Get orders from main grid
            foreach (ListViewDataItem lv in lvPreInvoiceItems.Items)
                if (lv.ItemType == ListViewItemType.DataItem)
                {
                    CheckBox chkExclude = lv.FindControl("chkExclude") as CheckBox;

                    if (chkExclude != null && !chkExclude.Checked)
                    {
                        bool canInvoiceOrder = false;
                        if (chkExclude.Attributes["OrderStatus"].ToString() != String.Empty)
                            if ((eOrderStatus)Enum.Parse(typeof(eOrderStatus), chkExclude.Attributes["OrderStatus"].ToString().Replace(" ", "_")) == eOrderStatus.Delivered
                                && Convert.ToBoolean(chkExclude.Attributes["CanBeInvoiced"]) == true)
                                canInvoiceOrder = true;

                        if (canInvoiceOrder)
                            orderIds.Add(int.Parse(chkExclude.Attributes["OrderId"]));
                    }
                }

            // Get Orders from additional grid
            foreach (ListViewDataItem lv in lvAdditional.Items)
                if (lv.ItemType == ListViewItemType.DataItem)
                {
                    CheckBox chkExclude = lv.FindControl("chkExcludeAdditional") as CheckBox;

                    if (chkExclude != null && !chkExclude.Checked)
                    {
                        bool canInvoiceOrder = false;
                        if (chkExclude.Attributes["OrderStatus"].ToString() != String.Empty)
                            if ((eOrderStatus)Enum.Parse(typeof(eOrderStatus), chkExclude.Attributes["OrderStatus"].ToString().Replace(" ","_")) == eOrderStatus.Delivered
                                && Convert.ToBoolean(chkExclude.Attributes["CanBeInvoiced"]) == true)
                                canInvoiceOrder = true;

                        if (canInvoiceOrder)
                            orderIds.Add(int.Parse(chkExclude.Attributes["OrderId"]));
                    }
                }

            return orderIds;
        }

        //------------------------------------------------------------------------------------

        private List<int> GetOrderIdsForExport()
        {
            List<int> orderIds = new List<int>();

            // Get orders from main grid
            foreach (ListViewDataItem lv in lvPreInvoiceItems.Items)
                if (lv.ItemType == ListViewItemType.DataItem)
                {
                    CheckBox chkExclude = lv.FindControl("chkExclude") as CheckBox;

                    if (chkExclude != null && !chkExclude.Checked)
                    {
                        bool canIncludeOrder = false;
                        if (chkExclude.Attributes["OrderStatus"].ToString() != String.Empty)
                            canIncludeOrder = true;

                        if (canIncludeOrder)
                            orderIds.Add(int.Parse(chkExclude.Attributes["OrderId"]));
                    }
                }

            // Get Orders from additional grid
            foreach (ListViewDataItem lv in lvAdditional.Items)
                if (lv.ItemType == ListViewItemType.DataItem)
                {
                    CheckBox chkExclude = lv.FindControl("chkExcludeAdditional") as CheckBox;

                    if (chkExclude != null && !chkExclude.Checked)
                    {
                        bool canIncludeOrder = false;
                        if (chkExclude.Attributes["OrderStatus"].ToString() != String.Empty)
                            canIncludeOrder = true;

                        if (canIncludeOrder)
                            orderIds.Add(int.Parse(chkExclude.Attributes["OrderId"]));
                    }
                }

            return orderIds;
        }

        //------------------------------------------------------------------------------------

        private List<int> GetImportedInvoiceItemIDsToExclude()
        {
            List<int> importedInvoiceItemIDsToExclude = new List<int>();

            // Get orders from main grid
            foreach (ListViewDataItem lv in lvPreInvoiceItems.Items)
                if (lv.ItemType == ListViewItemType.DataItem)
                {
                    CheckBox chkExclude = lv.FindControl("chkExclude") as CheckBox;

                    if (chkExclude != null && chkExclude.Checked)
                        importedInvoiceItemIDsToExclude.Add(int.Parse(chkExclude.Attributes["ImportedInvoiceItemId"]));
                }

            // Get Orders from additional grid
            foreach (ListViewDataItem lv in lvAdditional.Items)
                if (lv.ItemType == ListViewItemType.DataItem)
                {
                    CheckBox chkExclude = lv.FindControl("chkExcludeAdditional") as CheckBox;

                    if (chkExclude != null && chkExclude.Checked)
                        if (!String.IsNullOrEmpty(chkExclude.Attributes["ImportedInvoiceItemId"]))
                            importedInvoiceItemIDsToExclude.Add(int.Parse(chkExclude.Attributes["ImportedInvoiceItemId"]));
                }

            return importedInvoiceItemIDsToExclude;
        }

        //------------------------------------------------------------------------------------

        private void ConfigureDisplay()
        {
            DateTime fromDate = DateTime.Today.AddDays(-7);
            DateTime toDate = DateTime.Today;

            if(this.dteImportFromDate.SelectedDate.HasValue)
                fromDate = this.dteImportFromDate.SelectedDate.Value;
            else
                this.dteImportFromDate.SelectedDate = fromDate;

            if(this.dteImportToDate.SelectedDate.HasValue)
                toDate = this.dteImportToDate.SelectedDate.Value;
            else
                this.dteImportToDate.SelectedDate = toDate;

            this.LoadBusinessTypes();
            this.LoadFiles();
            this.RebindGrid();
        }

        //------------------------------------------------------------------------------------

        private void LoadBusinessTypes()
        {
            if (this.BusinessTypeDataSet.Tables[0] == null)
                return;

            cboBusinessType.DataSource = this.BusinessTypeDataSet;
            cboBusinessType.DataBind();
            cboBusinessType.Items.Insert(0, "-- All -- ");

            // If there is only one business type then select it by default.
            if (this.BusinessTypeDataSet.Tables[0].Rows.Count == 1)
                cboBusinessType.SelectedIndex = 1;
        }

        //------------------------------------------------------------------------------------

        private void LoadFiles()
        {
            cboFile.Items.Clear();

            DateTime fromDate = this.dteImportFromDate.SelectedDate.Value;
            DateTime toDate = this.dteImportToDate.SelectedDate.Value;
            toDate = toDate.AddHours(23);
            toDate = toDate.AddMinutes(59);
            toDate = toDate.AddSeconds(59);

            List<EF.ImportedInvoice> importedInvoices = (from ii in EF.DataContext.Current.ImportedInvoiceSet
                                                         where ii.CreateDateTime >= fromDate && ii.CreateDateTime <= toDate
                                                         select ii).ToList();

            if (importedInvoices.Count > 0)
                foreach (EF.ImportedInvoice importedInvoice in importedInvoices)
                {
                    Telerik.Web.UI.RadComboBoxItem rcItem = null;
                    rcItem = new Telerik.Web.UI.RadComboBoxItem();
                    rcItem.Text = importedInvoice.Filename;
                    rcItem.Value = importedInvoice.ImportedInvoiceID.ToString();
                    cboFile.Items.Add(rcItem);
                }

            this.cboFile.Items.Insert(0,  new Telerik.Web.UI.RadComboBoxItem("-- Please Select -- ","-1"));

            this.txtBatchRef.Text = String.Empty;
            this.lblFromSystem.Text = String.Empty;
        }

        //------------------------------------------------------------------------------------

        private void RebindGrid()
        {
            int importedInvoiceId = 0;
            int.TryParse(this.cboFile.SelectedValue,out importedInvoiceId);

            int businessTypeId = 0;
            int.TryParse(cboBusinessType.SelectedValue, out businessTypeId);

            Func<IEnumerable<DataRow>, decimal> getImportedItemDifferenceAmount = drs =>
            drs.Any(dr => dr.Field<int>("ImportedInvoiceItemID") == -1) ? drs.First(dr => dr.Field<int>("ImportedInvoiceItemID") == -1).Field<decimal>("DifferenceAmount") : 0m;
            
            Func<IEnumerable<DataRow>, decimal> getImportedItemDifferenceWeightAmount = drs =>
            drs.Any(dr => dr.Field<int>("ImportedInvoiceItemID") == -1) ? drs.First(dr => dr.Field<int>("ImportedInvoiceItemID") == -1).Field<decimal>("DifferenceWeightAmount") : 0m;

            Func<IEnumerable<DataRow>, string> getOrderStatus = drs =>
            drs.Any(dr => dr.Field<int>("ImportedInvoiceItemID") == -1) ? drs.First(dr => dr.Field<int>("ImportedInvoiceItemID") == -1).Field<string>("OrderStatus") : String.Empty;

            Func<IEnumerable<DataRow>, string> getLoadNumber = drs =>
            drs.Any(dr => dr.Field<int>("ImportedInvoiceItemID") == -1) ? drs.First(dr => dr.Field<int>("ImportedInvoiceItemID") == -1).Field<string>("Ref2") : String.Empty;

            Func<IEnumerable<DataRow>, string> getDocketNumber = drs =>
            drs.Any(dr => dr.Field<int>("ImportedInvoiceItemID") == -1) ? drs.First(dr => dr.Field<int>("ImportedInvoiceItemID") == -1).Field<string>("Ref1") : String.Empty;

            Func<IEnumerable<DataRow>, string> getShipmentNumber = drs =>
            drs.Any(dr => dr.Field<int>("ImportedInvoiceItemID") == -1) ? drs.First(dr => dr.Field<int>("ImportedInvoiceItemID") == -1).Field<string>("Ref3") : String.Empty;

            Func<IEnumerable<DataRow>, bool> getCanBeInvoiced = drs =>
            drs.Any(dr => dr.Field<int>("ImportedInvoiceItemID") == -1 && dr.Field<int?>("PreinvoiceId") == null && dr.Field<int?>("InvoiceId") == null) ? true : false;

            Facade.ImportedInvoice facimportedInvoice = new Facade.ImportedInvoice();
            DataSet ds = facimportedInvoice.GetImportedInvoiceItems(importedInvoiceId, businessTypeId);

            if (ds != null)
            {
                var queryInvoiceItems = ds.Tables[0].Rows.Cast<DataRow>().AsEnumerable();
                var resultRows = from row in queryInvoiceItems
                                 where row["OrderID"] != System.DBNull.Value
                                 group row by row["OrderID"] into g
                                 select new ResultType()
                                 {
                                     ImportedInvoiceItemID = -1, // we don't need to set this as it is not used for matched orders 
                                     OrderID = Convert.ToInt32(g.Key),
                                     OrderStatus = getOrderStatus(g),
                                     Ref1 = getDocketNumber(g),
                                     Ref2 = getLoadNumber(g),
                                     Ref3 = getShipmentNumber(g),
                                     CanBeInvoiced = getCanBeInvoiced(g),
                                     DifferenceAmount = getImportedItemDifferenceAmount(g),
                                     DifferenceWeightAmount = getImportedItemDifferenceWeightAmount(g), 
                                     OrderCount = g.Count(),
                                     Items = g
                                 };

                var unMatched = from row in queryInvoiceItems
                                where row["OrderID"] == System.DBNull.Value
                                group row by row["ImportedInvoiceItemID"] into g
                                select new ResultType()
                                {
                                    ImportedInvoiceItemID = g.First().Field<int>("ImportedInvoiceItemID"),
                                    OrderID = -1,
                                    OrderStatus = String.Empty,
                                    Ref1 = g.First().Field<string>("Ref1"),
                                    Ref2 = g.First().Field<string>("Ref2"),
                                    Ref3 = g.First().Field<string>("Ref3"),
                                    CanBeInvoiced = false,
                                    DifferenceAmount = 0m,
                                    DifferenceWeightAmount = 0m,
                                    OrderCount = 1,
                                    Items = g
                                };

                resultRows = resultRows.Concat(unMatched);
                resultRows = resultRows.OrderBy(res => res.Ref2).ThenBy(res => res.Ref1);

                lvPreInvoiceItems.DataSource = resultRows;
                lvPreInvoiceItems.DataBind();

                var queryAdditionalItems = ds.Tables[1].Rows.Cast<DataRow>().AsEnumerable();

                lvAdditional.DataSource = queryAdditionalItems;
                lvAdditional.DataBind();
            }
        }

        //------------------------------------------------------------------------------------

        #endregion

        //------------------------------------------------------------------------------------

    }
}
