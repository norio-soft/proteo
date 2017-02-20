using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Globalization;

using System.Threading;

namespace Orchestrator.WebUI.Invoicing.SubContractorSB
{
    /// <summary>
    /// Summary description for InvoiceExtraPreparation.
    /// </summary>
    public partial class InvoiceExtraPreparation : Orchestrator.Base.BasePage
    {
        protected override PageStatePersister PageStatePersister
        {
            get
            {
                return new HiddenFieldPageStatePersister(this);
            }
        }

        #region Constants
        private const string C_EXPORTCSV_VS = "ExportCSV";
        private const string _jobViewOwnerLink = "javascript:openDialogWithScrollbars('~/traffic/JobManagement/pricing2.aspx?wiz=true&jobId={0}'+ getCSID(), '800','600');";
        private const string _orderViewOwnerLink = "javascript:openOrderProfile({0});";
        #endregion

        #region Enum

        private enum eDataGridColumns { ExtraId, JobId, ExtraType, OrganisationName, CustomDescription, ExtraState, ClientContact, ForeignAmount, IncludeInInvoice };

        #endregion

        #region Page Variables
        private int m_IdentityId = 0;
        private string extraIdCSV = string.Empty;
        private Dictionary<int, CultureInfo> cultures = new Dictionary<int, CultureInfo>();
		#endregion

        #region Page Load

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.Invoicing);

            if (Request.QueryString["rcbID"] != null) return;

            if (Request.QueryString["IdentityId"] != null)
            {
                m_IdentityId = Convert.ToInt32(Request.QueryString["IdentityId"]);
            }

            this.cultures.Add(2057, new CultureInfo(2057));

			if (!IsPostBack)
			{
                if (Request.QueryString["edit"] == "true")
                {
                    extraIdCSV = Session["_selectedExtras"].ToString();
                    hidSelectedExtras.Value = Session["_selectedExtras"].ToString();
                    m_IdentityId = int.Parse(Session["_selectedExtraIdentityId"].ToString());
                }

                PopulateStaticControls();
                if (Request.QueryString["edit"] == "true" || Request.QueryString["IdentityId"] != null)
                    BindData();
                else
                    Utilities.ClearInvoiceSession();

                this.sameCurrencyValidationLabel.Text = String.Empty;
			}
		}

        #endregion

        #region Private Methods

        private void PopulateStaticControls()
        {
            Facade.ExtraType facExtraType = new Orchestrator.Facade.ExtraType();

            //cboExtraType.DataSource = facExtraType.GetForIsEnabled(getActiveExtraTypes); // Enum.GetNames(typeof(eExtraType));

            //Get all Extras that apply to Subbies AND Pallet Networks
            //At present this will only be the Hub Charge and at some point the Network and Subby Extras
            //may need to be dealt with seperately
            cboExtraType.DataSource = facExtraType.GetForAppliesTo(new List<eExtraAppliesTo> 
                {eExtraAppliesTo.SubContractor, eExtraAppliesTo.Network} );

            cboExtraType.DataValueField = "ExtraTypeId";
            cboExtraType.DataTextField = "Description";
            cboExtraType.DataBind();
            cboExtraType.Items.Insert(0, "");

            cboSelectExtraState.DataSource = Enum.GetNames(typeof(eExtraState));
            cboSelectExtraState.DataBind();
            cboSelectExtraState.Items.RemoveAt((int)eExtraState.Invoiced - 1);
            cboSelectExtraState.Items.Insert(0, "");

            if (m_IdentityId != 0)
            {
                //Get the Client name for id.
                Facade.IOrganisation facOrg = new Facade.Organisation();
                string name = facOrg.GetNameForIdentityId(m_IdentityId);

                cboSubContractor.SelectedValue = m_IdentityId.ToString();
                cboSubContractor.Text = name;
            }
        }

        #region Perform Extra Search

        private DataSet PerformExtraSearch()
        {
            int identityID = 0;
            int jobID = 0;
            int extraTypeID = 0;
            string extraStateCSV = string.Empty;
            DateTime startDate = (DateTime)SqlDateTime.MinValue;
            DateTime endDate = (DateTime)SqlDateTime.MinValue;

            int.TryParse(cboSubContractor.SelectedValue, out identityID);
            int.TryParse(txtJobId.Text, out jobID);
            if (cboExtraType.SelectedValue != string.Empty)
                extraTypeID = Convert.ToInt32(cboExtraType.SelectedValue);
            if (cboSelectExtraState.SelectedValue != string.Empty)
                extraStateCSV = ((int)(eExtraState)Enum.Parse(typeof(eExtraState), cboSelectExtraState.SelectedValue)).ToString();
            else
            {
                // All extra states, excluding Invoiced:
                extraStateCSV =
                    ((int)eExtraState.AwaitingResponse).ToString() + "," +
                    ((int)eExtraState.Accepted).ToString() + "," +
                    ((int)eExtraState.Refused).ToString();
            }
            if (dteDateFrom.Text != "" && dteDateTo.Text != "")
            {
                startDate = dteDateFrom.SelectedDate.Value;
                startDate = startDate.Subtract(startDate.TimeOfDay);
                endDate = dteDateTo.SelectedDate.Value;
                endDate = endDate.Subtract(endDate.TimeOfDay);
                endDate = endDate.Add(new TimeSpan(0, 23, 59, 59, 999));
            }

            Facade.IInvoiceExtra facInvoiceExtra = new Facade.Invoice();
            DataSet dsExtras = facInvoiceExtra.GetExtrasWithParamsForSubbies(
                identityID,
                jobID,
                extraTypeID,
                extraStateCSV,
                startDate,
                endDate);

            dsExtras.Tables[0].Columns.Add("Include", typeof(Boolean));

            if (dsExtras.Tables[0].Rows.Count == 0 || cboSubContractor.SelectedValue == "")
                btnCreateInvoice.Visible = false;
            else
                btnCreateInvoice.Visible = true;

            // Check whether account is on hold
            if (dsExtras.Tables[0].Rows.Count > 0)
            {
                if (Convert.ToInt32(dsExtras.Tables[0].Rows[0]["OnHold"]) == 1)
                {
                    gvExtras.Enabled = false;
                    lblExtrasOnHold.Visible = true;
                    lblExtrasOnHold.Text = cboSubContractor.Text + "'s account has been put on hold, please go to <A HREF=../Organisation/addupdateorganisation.aspx?IdentityId=" + Convert.ToInt32(cboSubContractor.SelectedValue) + ">" + cboSubContractor.Text + "'s details to change.</A>";
                }
                else
                    lblExtrasOnHold.Visible = false;

                gvExtras.Visible = true;
            }
            else
            {
                lblExtrasOnHold.Visible = true;
                lblExtrasOnHold.Text = "With the given parameters no extras have been found to invoice.";
                gvExtras.Visible = false;
            }

            // Add COMBO Extra State Functionality
            DataTable tblExtras = new DataTable("Extras");
            DataTable tblExtraStates = new DataTable("ExtraStates");

            // Get a table containing the extra states.
            tblExtraStates = new DataTable("ExtraStates");
            tblExtraStates.Columns.Add(new DataColumn("ExtraStateId", typeof(int)));
            tblExtraStates.Columns.Add(new DataColumn("ExtraState", typeof(string)));

            string[] extraStates = Utilities.UnCamelCase(Enum.GetNames(typeof(eExtraState)));
            foreach (string item in extraStates)
            {
                if (item.ToString() != "Invoiced")
                {
                    DataRow state = tblExtraStates.NewRow();
                    state[0] = (int)Enum.Parse(typeof(eExtraState), item.Replace(" ", ""));
                    state[1] = item.ToString();
                    tblExtraStates.Rows.Add(state);
                }
            }

            // Add the extra states table to the dataset and set the name of the first table to make the relationship easier to understand.
            dsExtras.Tables[0].TableName = "Extras";
            dsExtras.Tables.Add(tblExtraStates);

            // Configure the relationship.
            dsExtras.Relations.Add(dsExtras.Tables["ExtraStates"].Columns["ExtraStateId"], dsExtras.Tables["Extras"].Columns["ExtraStateId"]);

            return dsExtras;
        }

        #endregion

        private void BindData()
        {
            hidExtraCount.Value = "0";
            hidExtraTotal.Value = "0";

            DataView dv = new DataView(PerformExtraSearch().Tables[0]);

            string sortBy = (SortCriteria + " " + SortDir).Trim();

            if (sortBy != "")
                dv.Sort = sortBy;

            gvExtras.DataSource = dv;

            Session[C_EXPORTCSV_VS] = dv.Table;

            gvExtras.DataBind();

            if (gvExtras.Rows.Count >= 1)
                btnExport.Visible = true;
            else
                btnExport.Visible = false;

        }

        #endregion

        #region Combo's Server Methods and Initialisation


        void cboSubContractor_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {

            cboSubContractor.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllSubContractorsFiltered(e.Text);

            int itemsPerRequest = 20;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            DataTable dt = ds.Tables[0];
            Telerik.Web.UI.RadComboBoxItem rcItem = null;
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new Telerik.Web.UI.RadComboBoxItem();
                rcItem.Text = dt.Rows[i]["OrganisationName"].ToString();
                rcItem.Value = dt.Rows[i]["IdentityId"].ToString();
                cboSubContractor.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }

        }


        #endregion

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Init += new System.EventHandler(this.InvoiceExtraPreparation_Init);
            this.cboSubContractor.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboSubContractor_ItemsRequested);

        }

        #endregion

        #region Event Handlers

        #region Button Event Handlers

		#endregion

		#region Event Handlers

		#region Button Event Handlers

		private void btnCreateInvoice_Click(object sender, EventArgs e)
		{
            if (Page.IsValid)
                if (this.SameCurrencyValidation())
                {
                    this.sameCurrencyValidationLabel.Text = String.Empty;
                    ArrayList arrExtraId = null;

                    arrExtraId = GetExtrasSelected();

                    if (arrExtraId.Count != 0)
                    {
                        Session["ExtraIds"] = arrExtraId;
                    }

                    Session["ExtraIdCSV"] = hidSelectedExtras.Value;
                    Session["ExtraIds"] = arrExtraId;
                    Session["IdentityId"] = cboSubContractor.SelectedValue;

                    Server.Transfer("AddUpdateExtraInvoice.aspx");
                }
                else
                    this.sameCurrencyValidationLabel.Text = " * All selected extras must be of the same culture.";
		}

        private void btnFilter_Click(object sender, EventArgs e)
        {
            hidSelectedExtras.Value = String.Empty;
            hidExtraTotal.Value = string.Empty;
            hidExtraCount.Value = string.Empty;
            BindData();
        }

        #endregion

        #region DataGrid Event Handlers

        #endregion

        protected void cboSelectExtraState_SelectedIndexChanged(object sender, EventArgs e)
        {
            eExtraState state = (eExtraState)Enum.Parse(typeof(eExtraState), ((DropDownList)sender).SelectedValue);

            DataGridItem containingItem = ((DataGridItem)((DropDownList)sender).Parent.Parent);

            RequiredFieldValidator rfvClientContact = (RequiredFieldValidator)containingItem.FindControl("rfvClientContact");

            rfvClientContact.Enabled = (state == eExtraState.Accepted || state == eExtraState.Refused);
        }

        protected void InvoiceExtraPreparation_Init(object sender, EventArgs e)
        {
            btnExport.Click += new EventHandler(btnExport_Click);
            btnCreateInvoice.Click += new EventHandler(btnCreateInvoice_Click);
            btnFilter.Click += new EventHandler(btnFilter_Click);

            cfvDateTo.ServerValidate += new ServerValidateEventHandler(cfvDateTo_ServerValidate);

            gvExtras.RowDataBound += new GridViewRowEventHandler(gvExtras_RowDataBound);
            dlgExtra.DialogCallBack += new EventHandler(dlgExtra_DialogCallBack);

        }

        decimal runingTotal = 0;
        int runningCount = 0;

        void gvExtras_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView drv = (DataRowView)e.Row.DataItem;

                int LCID = 2057;
                if (drv["LCID"] != DBNull.Value && Convert.ToInt32(drv["LCID"]) != -1)
                    LCID = Convert.ToInt32(drv["LCID"]);

                if (!this.cultures.ContainsKey(LCID))
                    this.cultures.Add(LCID, new CultureInfo(LCID));

                decimal extraAmount = Convert.ToDecimal(drv["ForeignAmount"]);
                Label amountCurrency = (Label)e.Row.FindControl("extraAmountCurrencyLabel");

                amountCurrency.Text = extraAmount.ToString("C", this.cultures[LCID]);

                int extraId = (int)drv["ExtraId"];
                
                eExtraState extraState = (eExtraState)Enum.Parse(typeof(eExtraState), (string)drv["ExtraState"]);

                CheckBox chkIncludeExtra = (CheckBox)e.Row.FindControl("chkIncludeExtra");

                if (chkIncludeExtra != null)
                {
                    if (extraState != eExtraState.Accepted || Convert.ToBoolean(drv["BeingInvoiced"]))
                        chkIncludeExtra.Enabled = false;
                    else
                    {
                        // Checkbox & Charge Amount
                        chkIncludeExtra.Attributes.Add("onClick", "countSelectedExtras (this, " + extraId + ", " + extraAmount + ")");

                        if (extraIdCSV.IndexOf(extraId.ToString()) > -1)
                        {
                            chkIncludeExtra.Checked = true;
                            runingTotal += extraAmount;
                            runningCount++;
                            hidExtraTotal.Value = runingTotal.ToString();
                            hidExtraCount.Value = runningCount.ToString();
                            //hidSelectedExtras.Value = hidSelectedExtras.Value + extraId + ",";
                        }
                    }
                }

                HtmlAnchor lnkViewOwner = e.Row.FindControl("lnkViewOwner") as HtmlAnchor;
                if (drv["OrderID"] == DBNull.Value || (int)drv["ExtraTypeId"] == 2)
                {
                    lnkViewOwner.HRef = string.Format(_jobViewOwnerLink, (int)drv["JobID"]);
                    lnkViewOwner.InnerText = drv["JobID"].ToString();
                }
                else
                {
                    lnkViewOwner.HRef = string.Format(_orderViewOwnerLink, (int)drv["OrderID"]);
                    lnkViewOwner.InnerText = drv["OrderID"].ToString();
                }
            }
        }

        void dlgExtra_DialogCallBack(object sender, EventArgs e)
        {
            extraIdCSV = hidSelectedExtras.Value;

            if (dlgExtra.ReturnValue.Contains("AddExtra"))
                BindData();
        }

        //void MyClientSideAnchor_OnWindowClose(Codesummit.WebModalAnchor sender)
        //{
        //    extraIdCSV = hidSelectedExtras.Value;

        //    if (sender.OutputData.IndexOf("AddExtra") > 0)
        //    {
        //        BindData();
        //    }
        //}



        void btnExport_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                //  Get data set and then put in session
                DataView dvExport = new DataView((DataTable)Session[C_EXPORTCSV_VS]);

                Session["__ExportDS"] = (DataTable)Session[C_EXPORTCSV_VS];

                Server.Transfer("../Reports/csvexport.aspx?filename=InvoiceExtraPreparation.csv");
            }
        }

        #endregion

        private ArrayList GetExtrasSelected()
        {
            string working = hidSelectedExtras.Value;
            ArrayList retval = new ArrayList();

            if (working.Length > 0)
                working = working.Substring(0, working.Length - 1);

            string[] extraIds = working.Split(',');

            foreach (string extraId in extraIds)
            {
                if (extraId.ToString() != string.Empty)
                    retval.Add(extraId);
            }

            return retval;
        }

        #region DataGrid Event Handlers (EditCommand/CancelCommand/UpdateCommand)



        private void UpdateExtras(ComponentArt.Web.UI.GridItem item, string command)
        {
            switch (command)
            {
                case "UPDATE":
                    int extraId = Convert.ToInt32(item["ExtraId"].ToString());

                    Facade.IJobExtra facJobExtra = new Facade.Job();

                    Entities.Extra updatingExtra = facJobExtra.GetExtraForExtraId(extraId);

                    updatingExtra.ExtraState = (eExtraState)Enum.Parse(typeof(eExtraState), item["ExtraState"].ToString());

                    Facade.IExchangeRates facER = new Facade.ExchangeRates();
                    if (updatingExtra.ExchangeRateID != null)
                    {
                        updatingExtra.ExtraAmount = facER.GetConvertedRate((int)updatingExtra.ExchangeRateID,
                            Decimal.Parse(item["ForeignAmount"].ToString(), NumberStyles.Currency));
                    }
                    else
                    {
                        updatingExtra.ExtraAmount = Decimal.Parse(item["ForeignAmount"].ToString(), NumberStyles.Currency);
                    }

                    updatingExtra.ForeignAmount = Decimal.Parse(item["ForeignAmount"].ToString(), NumberStyles.Currency);

                    updatingExtra.ClientContact = item["ClientContact"].ToString();

                    facJobExtra.UpdateExtra(updatingExtra, ((Entities.CustomPrincipal)Page.User).UserName);
                    break;
            }
        }

        private void dgExtras_ItemContentCreated(object sender, ComponentArt.Web.UI.GridItemContentCreatedEventArgs e)
        {
            // This is the OLD chkIncludeExtra stuff.
            int extraId = 0;

            CheckBox chkIncludeExtra = (CheckBox)e.Content.FindControl("chkIncludeExtra");

            extraId = Convert.ToInt32(e.Item["ExtraId"]); // DataRowView

            if (chkIncludeExtra != null)
            {
                // Checkbox & Charge Amount
                chkIncludeExtra.Attributes.Add("onClick", "countSelectedExtras (this, " + extraId + ")"); // DataRowView
                if (cboSubContractor.SelectedValue != String.Empty)
                {
                    if (e.Item["ExtraState"].ToString() == "Accepted")
                        chkIncludeExtra.Visible = true;
                    else
                        chkIncludeExtra.Visible = false;
                }
                chkIncludeExtra.Visible = false;
            }
        }

        #endregion

        #region DataGrid Event Handlers (OnItemDataBound/OnPageIndexChanged/OnSortCommand)


        #endregion

        #region Validation

        protected void cfvDateTo_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (dteDateFrom.Text != string.Empty && dteDateTo.Text != string.Empty)
            {
                if (dteDateFrom.SelectedDate.Value.Subtract(dteDateFrom.SelectedDate.Value.TimeOfDay) <= dteDateTo.SelectedDate.Value.Subtract(dteDateTo.SelectedDate.Value.TimeOfDay))
                    args.IsValid = true;
                else
                    args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }

        private bool SameCurrencyValidation()
        {
            bool sameCurrency = false;
            DataTable extras = this.PerformExtraSearch().Tables[0];
            ArrayList extraIds = this.GetExtrasSelected();

            int LCID = -1;
            bool differentCultureFound = false;

            foreach (string extraIdString in extraIds)
            {
                int extraId = Convert.ToInt32(extraIdString);
                foreach (DataRow dr in extras.Rows)
                {
                    if (extraId == Convert.ToInt32(dr["ExtraId"]))
                    {
                        if (LCID == -1)
                        {
                            LCID = (dr["LCID"] == DBNull.Value) ? 2057 : (Convert.ToInt32(dr["LCID"]) == -1) ? 2057 : Convert.ToInt32(dr["LCID"]);
                            break;
                        }
                        else
                        {
                            if (LCID != ((dr["LCID"] == DBNull.Value) ? 2057 : (Convert.ToInt32(dr["LCID"]) == -1) ? 2057 : Convert.ToInt32(dr["LCID"])))
                            {
                                differentCultureFound = true;
                                break;
                            }
                        }
                    }
                    else
                        continue;
                }

                if (differentCultureFound)
                    break;
            }

            sameCurrency = !differentCultureFound;

            return sameCurrency;
        }

		#endregion

        #region Properties

        /// <summary>
        ///	Sort Dir
        ///	</summary>
        protected string SortDir
        {
            get { return (string)ViewState["sortDir"]; }
            set { ViewState["sortDir"] = value; }
        }

        ///	<summary> 
        ///	Sort Criteria
        ///	</summary
        protected string SortCriteria
        {
            get { return (string)ViewState["sortCriteria"]; }
            set { ViewState["sortCriteria"] = value; }
        }

        #endregion

        #endregion
    }
}
