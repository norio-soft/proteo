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
using System.Collections.Generic;
using Telerik.Web.UI;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;

namespace Orchestrator.WebUI.Invoicing.SubContractorSB
{
    public partial class autorunstart : Orchestrator.Base.BasePage
    {
        #region private variables

        private const string v_jobSubContractID = "v_jobSubContractID";
        private const string v_batchID = "v_batchID";
        private const string v_subContractorListReadyToInvoice = "v_subContractorListReadyToInvoice";
        private const string v_subContractorListLoadBatch = "v_subContractorListLoadBatch";

        #endregion

        #region Properties

        protected override PageStatePersister PageStatePersister
        {
            get
            {
                return new HiddenFieldPageStatePersister(this);
            }
        }

        private List<int> JobSubContractIDs
        {
            get { return (List<int>)ViewState[v_jobSubContractID]; }
            set { ViewState[v_jobSubContractID] = value; }
        }

        private int BatchID
        {
            get {
                int batch;
                if (ViewState[v_batchID] == null || (int)ViewState[v_batchID] <= 0)
                {
                    int.TryParse(Request.QueryString["bID"], out batch);
                    ViewState[v_batchID] = batch;
                }

                return (int)ViewState[v_batchID];
            }
            set { ViewState[v_batchID] = value; }
        }

        private bool IsUpdate()
        {
            return !string.IsNullOrEmpty(Request.QueryString["bID"]);
        }

        #endregion

        #region Page Load/Init

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["rcbID"]))
                return;
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.Invoicing);

            if (!Page.IsPostBack)
            {
                grdSubbies.Columns.FindByUniqueName("CustomerOrderNumbers").HeaderText = Orchestrator.Globals.Configuration.SystemLoadNumberText;
                grdSubbies.Columns.FindByUniqueName("DeliveryOrderNumbers").HeaderText = Orchestrator.Globals.Configuration.SystemDocketNumberText;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnRefresh.Click += new EventHandler(btnRefresh_Click);
            btnCreateBatch.Click += new EventHandler(btnCreateBatch_Click);

            Button1.Click += new EventHandler(btnCreateBatch_Click);
            cboSubContractor.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboSubContractor_ItemsRequested);

            grdSubbies.ItemDataBound += new GridItemEventHandler(grdSubbies_ItemDataBound);
            grdSubbies.ItemCreated += new GridItemEventHandler(grdSubbies_ItemCreated);

            this.btnEditRates.Click += new EventHandler(btnEditRates_Click);
            this.btnEditRatesBottom.Click += new EventHandler(btnEditRates_Click);

            this.btnAddInvoiceReferences.Click += new EventHandler(btnAddInvoiceReferences_Click);
            this.btnAddInvoiceReferencesBottom.Click += new EventHandler(btnAddInvoiceReferences_Click);            
        }

        private void btnAddInvoiceReferences_Click(object sender, EventArgs e)
        {
            if (this.grdSubbies.Items.Count > 0)
            {
                Button AddInvoiceReferences = (Button)sender;
                GridColumn invoiceColumn = this.grdSubbies.Columns.FindByUniqueName("InvoiceNo");
                GridColumn chkColumn = this.grdSubbies.Columns.FindByUniqueName("ClientSelect");

                if (AddInvoiceReferences.Text == "Add Invoice References")
                {
                    if (invoiceColumn != null && chkColumn != null)
                    {
                        invoiceColumn.Display = true;
                        chkColumn.Display = false;
                        this.btnAddInvoiceReferences.Text = "Remove Invoice References";
                        this.btnAddInvoiceReferencesBottom.Text = "Remove Invoice References";
                    }
                }
                else if (AddInvoiceReferences.Text == "Remove Invoice References")
                {
                    foreach (GridItem item in this.grdSubbies.Items)
                    {
                        TextBox txt = (TextBox)item.FindControl("txtInvoiceNo");
                        txt.Text = String.Empty;
                    }

                    if (invoiceColumn != null && chkColumn != null)
                    {
                        invoiceColumn.Display = false;
                        chkColumn.Display = true;
                        this.btnAddInvoiceReferences.Text = "Add Invoice References";
                        this.btnAddInvoiceReferencesBottom.Text = "Add Invoice References";
                    }
                }

                //Refresh the table data or the Job/Order ID column will unpopulate
                RefreshData();

            }
        }

        private void btnEditRates_Click(object sender, EventArgs e)
        {
            if (this.grdSubbies.Items.Count > 0)
            {
                Button editRates = (Button)sender;
                if (editRates.Text == "Edit Rates")
                {
                    foreach (GridItem item in grdSubbies.MasterTableView.Items)
                    {
                        if (item is GridEditableItem)
                        {
                            GridEditableItem editableItem = item as GridDataItem;
                            editableItem.Edit = true;
                        }
                    }
                    this.btnEditRates.Text = "Update Rates";
                    this.btnEditRatesBottom.Text = "Update Rates";
                }
                else
                {
                    List<Entities.SubContractorDataItem> subbyList = this.GetDataSource();

                    foreach (GridEditableItem editedItem in grdSubbies.EditItems)
                    {
                        // UpdateRate if it has changed

                        HtmlInputText txtRate = editedItem.FindControl("txtSubContractRate") as HtmlInputText;
                        int jobSubContractID = int.Parse(editedItem.GetDataKeyValue("JobSubContractID").ToString());

                        // get the data item so we can compare the rate before it was changed
                        Entities.SubContractorDataItem subbyDataItem =
                            subbyList.Find(delegate(Entities.SubContractorDataItem item)
                            {
                                return item.JobSubContractID == jobSubContractID;
                            });

                        CultureInfo culture = new CultureInfo(subbyDataItem.LCID);

                        decimal rate = 0;
                        if (Decimal.TryParse(txtRate.Value, System.Globalization.NumberStyles.Currency, culture, out rate))
                        {
                            if (rate != subbyDataItem.Rate)
                            {
                                Facade.IJobSubContractor facSub = new Facade.Job();

                                // update the jobSubcontract rate in the database and in viewstate
                                facSub.UpdateRate(jobSubContractID, rate, this.User.Identity.Name);
                            }
                        }

                        editedItem.Edit = false;
                    }
                    this.btnEditRates.Text = "Edit Rates";
                    this.btnEditRatesBottom.Text = "Edit Rates";

                }

                this.grdSubbies.DataSource = this.GetDataSource();
                this.grdSubbies.DataBind();
            }
        }

        private void grdSubbies_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item.IsInEditMode)
                if (e.Item.DataItem != null)
                {
                    Entities.SubContractorDataItem subbyDataItem = (Entities.SubContractorDataItem)e.Item.DataItem;
                    GridEditableItem item = e.Item as GridEditableItem;
                    GridTemplateColumnEditor editor = (GridTemplateColumnEditor)item.EditManager.GetColumnEditor("Rate");
                    HtmlInputText txtRate = (HtmlInputText)editor.ContainerControl.FindControl("txtSubContractRate");

                    CultureInfo culture = new CultureInfo(subbyDataItem.LCID);
                    txtRate.Value = subbyDataItem.ForeignRate.ToString("C", culture);
                }
        }

        #endregion

        #region Private Functions

        private List<Entities.SubContractorDataItem> GetReadyToInvoiceForDates()
        {
            int identityID = 0;
            int.TryParse(cboSubContractor.SelectedValue, out identityID);

            // If all business types are selected then pass down an empty list - the stored procedure will then not filter on business type which will result in better performance
            var businessTypeIDs = chkBusinessType.AllBusinessTypesSelected ? Enumerable.Empty<int>() : chkBusinessType.SelectedBusinessTypeIDs;

            using (Facade.IJobSubContractor facJobSubContractor = new Facade.Job())
            {
                return facJobSubContractor.GetJobSubContractorsReadyToInvoice(rdiStartDate.SelectedDate, rdiEndDate.SelectedDate, identityID, businessTypeIDs);
            }
        }

        private List<Entities.SubContractorDataItem> GetDataSource()
        {
            List<Entities.SubContractorDataItem> list = null;

            // Commented out !IsPostBack as would never reach the code for loading a batch.
            // T.Lunken : 21/05/09
            if (!string.IsNullOrEmpty(Request.QueryString["rcbID"])) //|| !IsPostBack)
                list = null;
            else if (!string.IsNullOrEmpty(Request.QueryString["bID"]) || BatchID > 0)
                list = LoadBatch();
            else if(Page.IsCallback == false)
                list = GetReadyToInvoiceForDates();

            return list;
        }

        private List<Entities.SubContractorDataItem> LoadBatch()
        {
            using (Facade.IPreInvoice facPreInvoice = new Facade.PreInvoice())
            {
                rdiInvoiceDate.SelectedDate = facPreInvoice.GetBatchInvoiceDate(BatchID);
                return facPreInvoice.GetJobSubContractorsForBatch(BatchID);
            }
        }

        #endregion

        #region Event Handlers

        #region Button Events

        void btnCreateBatch_Click(object sender, EventArgs e)
        {
            if (grdSubbies.Items.Count > 0 && Page.IsValid)
            {
                string jobSubConIdAndInvoiceNo = String.Empty;
                GridColumn invoiceColumn = this.grdSubbies.Columns.FindByUniqueName("InvoiceNo");
                bool byPassCheckbox = false;

                if (invoiceColumn != null)
                    byPassCheckbox = invoiceColumn.Display;

                // Get the JobSubContractIDs that we are to Invoice
                List<int> jobSubContractIDs = new List<int>();
                foreach (GridItem row in grdSubbies.Items)
                {
                    TextBox txtInvoiceNo = (TextBox)row.FindControl("txtInvoiceNo");

                    if (row.Selected || byPassCheckbox)
                    {
                        int jobSubContractID = int.Parse(row.OwnerTableView.DataKeyValues[row.ItemIndex]["JobSubContractID"].ToString());

                        if (!String.IsNullOrEmpty(txtInvoiceNo.Text))
                        {
                            jobSubConIdAndInvoiceNo += jobSubContractID.ToString() + ":" + txtInvoiceNo.Text + ",";
                            jobSubContractIDs.Add(jobSubContractID);
                        }
                        else if (byPassCheckbox == false)
                            jobSubContractIDs.Add(jobSubContractID);
                    }
                }

                Facade.IPreInvoice facPreInvoice = new Facade.PreInvoice();
                if (IsUpdate())
                {
                    facPreInvoice.UpdateBatchForSubbySelfBill(BatchID, rdiInvoiceDate.SelectedDate.Value, jobSubContractIDs, this.Page.User.Identity.Name);
                }
                else
                    BatchID = facPreInvoice.CreateBatchForSubbySelfBill(rdiInvoiceDate.SelectedDate.Value, jobSubContractIDs, this.Page.User.Identity.Name);

                int batchID = BatchID;
                BatchID = -1;
                string url = "autorunconfirmation.aspx?bID=" + batchID.ToString();

                if(!String.IsNullOrEmpty(jobSubConIdAndInvoiceNo))
                    url += "&InvoiceNumbers=" + jobSubConIdAndInvoiceNo;

                Response.Redirect(url);
            }
        }

        void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void RefreshData()
        {
            // refresh the data.
            foreach (GridEditableItem editedItem in grdSubbies.EditItems)
                editedItem.Edit = false;

            this.btnEditRates.Text = "Edit Rates";
            this.btnEditRatesBottom.Text = "Edit Rates";

            this.grdSubbies.DataSource = this.GetDataSource();
            this.grdSubbies.DataBind();
        }

        #endregion

        #region Combobox Events

        void cboSubContractor_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboSubContractor.Items.Clear();
            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllSubContractorsFiltered("%" + e.Text);
            DataTable dt = ds.Tables[0];
            DataTable boundResults = dt.Clone();

            int itemsPerRequest = 20;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > dt.Rows.Count)
                endOffset = dt.Rows.Count;

            for (int i = itemOffset; i < endOffset; i++)
                boundResults.ImportRow(dt.Rows[i]);

            cboSubContractor.DataSource = boundResults;
            cboSubContractor.DataBind();

            if (dt.Rows.Count > 0)
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
        }

        #endregion

        #region Grid Events
        
        protected void grdSubbies_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                Entities.SubContractorDataItem dataItem = e.Item.DataItem as Entities.SubContractorDataItem;
                e.Item.Style["background-color"] = Orchestrator.WebUI.Utilities.GetJobStateColourForHTML((eJobState)dataItem.JobStateID);

                #region From/To

                HtmlGenericControl spnFrom = (HtmlGenericControl)e.Item.FindControl("spnFrom");
                HtmlGenericControl spnTo = (HtmlGenericControl)e.Item.FindControl("spnTo");

                spnFrom.InnerHtml = dataItem.Source.Replace("\r\n", "</br>");
                spnTo.InnerHtml = dataItem.Destination.Replace("\r\n", "</br>");

                #endregion

                #region Rate Link
                Label lblSubContractRate = e.Item.FindControl("lblSubContractRate") as Label;

                if (lblSubContractRate != null)
                {
                    CultureInfo culture = new CultureInfo(dataItem.LCID);
                    lblSubContractRate.Text = dataItem.ForeignRate.ToString("C", culture);
                }
                #endregion

                #region Run/Order ID

                var plcRunOrOrderID = (PlaceHolder)e.Item.FindControl("plcRunOrOrderID");
                if (plcRunOrOrderID != null && dataItem.References.Any())
                {
                    char[] characters = { 'j', 'o', 'J', 'O' };

                    // Due to the way the data items are constructed, the order or job id will be the final item in the References list
                    var id = dataItem.References.Last();

                    string href = string.Format(
                        "javascript:{0}({1});",
                        dataItem.ReportDataItemType == Entities.eSubContractorDataItem.Job ? "viewJobDetails" : "viewOrderProfile",
                        id.TrimStart(characters));

                    plcRunOrOrderID.Controls.Add(new HtmlAnchor { HRef = href, InnerText = id });
                }

                #endregion

                #region Run ID

                var plcRunID = (PlaceHolder)e.Item.FindControl("plcRunID");
                if (plcRunID != null && dataItem.JobId > 0)
                {
                    string href = string.Format(
                        "javascript:viewJobDetails({0});",
                        dataItem.JobId);

                    plcRunID.Controls.Add(new HtmlAnchor { HRef = href, InnerText = dataItem.JobId.ToString() });
                }

                #endregion
            }
        }

        #endregion

        #endregion
    }
}