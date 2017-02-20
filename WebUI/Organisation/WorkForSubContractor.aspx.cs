using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Telerik.Web.UI;
using System.Web.UI.HtmlControls;
using System.Globalization;

namespace Orchestrator.WebUI.Organisation
{
    public partial class WorkForSubContractor : Orchestrator.Base.BasePage
    {
        #region Properties

        protected struct CurrencyTotals
        {
            public decimal CurrencyTotal;
            public int LCID;

            public CurrencyTotals(decimal currencyTotal, int lcID)
            {
                CurrencyTotal = currencyTotal;
                LCID = lcID;
            }

            public CultureInfo currencyCulture
            {
                get
                {
                    return new CultureInfo(LCID);
                }
            }
        }

        protected struct OrganisationTotals
        {
            public int OrganisationID;
            public int LCID;
            public string OrganisationName;
            public decimal OrganisationTotal;

            public OrganisationTotals(int organisationID, int lcID, string organisationName, decimal organisationTotal)
            {
                OrganisationID = organisationID;
                LCID = lcID;
                OrganisationName = organisationName;
                OrganisationTotal = organisationTotal;
            }

            public CultureInfo organisationCulture
            {
                get
                {
                    return new CultureInfo(LCID);
                }
            }
        }

        protected List<OrganisationTotals> AllOrganisationTotals = null;
        protected List<CurrencyTotals> AllCurrencyTotals = null;

        protected CultureInfo GetCulture(int lcid)
        {
            return new CultureInfo(lcid);
        }

        private int _identityID = 0;
        public int IdentityID
        {
            get { return _identityID; }
        }

        private DateTime _dateFrom = new DateTime();
        public DateTime DateFrom
        {
            get { return _dateFrom; }
        }

        private DateTime _dateTo = new DateTime();
        public DateTime DateTo
        {
            get { return _dateTo; }
        }

        #endregion

        #region Page Load

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                ConfigureDisplay();

            

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            cboSubContractor.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboSubContractor_ItemsRequested);

            grdSubbies.ItemDataBound += new GridItemEventHandler(grdSubbies_ItemDataBound);
            grdSubbies.NeedDataSource += new GridNeedDataSourceEventHandler(grdSubbies_NeedDataSource);

            lnkClear.Click += new EventHandler(lnkClear_Click);

            Page.PreRender += new EventHandler(Page_PreRender);

            btnRefresh.Click += btnRefresh_Click;
            btnRefreshBottom.Click += btnRefresh_Click;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            grdSubbies.Rebind();

            #region Organisation Totals

            foreach (GridDataItem item in grdSubbies.MasterTableView.Items)
            {
                Entities.SubContractorDataItem dataItem = item.DataItem as Entities.SubContractorDataItem;
                if (AllOrganisationTotals.Exists(aot => aot.OrganisationID == dataItem.SubContractorID && aot.LCID == dataItem.LCID))
                {
                    OrganisationTotals existingOrganisation = AllOrganisationTotals.Find(aot => aot.OrganisationID == dataItem.SubContractorID && aot.LCID == dataItem.LCID);
                    AllOrganisationTotals.Remove(existingOrganisation);
                    existingOrganisation.OrganisationTotal += dataItem.ForeignRate;
                    AllOrganisationTotals.Add(existingOrganisation);
                }
                else
                {
                    OrganisationTotals newOrganisation = new OrganisationTotals(dataItem.SubContractorID, dataItem.LCID, dataItem.SubContractorName, dataItem.ForeignRate);
                    AllOrganisationTotals.Add(newOrganisation);
                }
            }
            #endregion

        }

        void grdSubbies_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            if (Page.IsPostBack && !Page.IsCallback)
            {
                GetData();
            }
        }

        void Page_PreRender(object sender, EventArgs e)
        {
            ConfigureGrids();
        }

        #endregion

        #region Private Functions

        private void ConfigureDisplay()
        {
            //dteDateTo.SelectedDate = DateTime.Today;
            //dteDateFrom.SelectedDate = DateTime.Today.AddMonths(-1);
        }

        private void ConfigureGrids()
        {
            if (AllOrganisationTotals != null)
            {
                Facade.IJobSubContractor facJobSubContractor = new Facade.Job();
                DataSet ds = facJobSubContractor.GetAllHubChargesForJobSubContractIDsAndPalletNetwork(IdentityID, DateFrom, DateTo);

                if (ds.Tables.Count > 0)
                {
                    var hubCharges = from row in ds.Tables[0].Rows.Cast<DataRow>()
                                     group row by row["SubContractorName"] into g
                                     select new
                                     {
                                         OrganisationName = g.Key,
                                         Orders = g.Count(),
                                         Items = g
                                     };

                    lvHubCharges.DataSource = hubCharges;

                }
                else
                    lvHubCharges.DataSource = null;

                lvHubCharges.DataBind();

                if (AllOrganisationTotals.Count > 0)
                {
                    AllOrganisationTotals.Sort(delegate(OrganisationTotals x, OrganisationTotals y) { return x.OrganisationName.CompareTo(y.OrganisationName); });
                    SetCurrencyTotals();

                    repOrganisationTotal.DataSource = AllOrganisationTotals;
                    repOrganisationTotal.DataBind();

                    repCurrencyTotals.DataSource = AllCurrencyTotals;
                    repCurrencyTotals.DataBind();

                    pnlOrganisationTotals.Visible = true;
                }
                else
                    pnlOrganisationTotals.Visible = false;
            }
        }

        private void SetCurrencyTotals()
        {
            AllCurrencyTotals = new List<CurrencyTotals>();

            foreach (OrganisationTotals ot in AllOrganisationTotals)
            {
                if (AllCurrencyTotals.Exists(act => act.LCID == ot.LCID))
                {
                    CurrencyTotals existingCT = AllCurrencyTotals.Find(act => act.LCID == ot.LCID);
                    AllCurrencyTotals.Remove(existingCT);
                    existingCT.CurrencyTotal += ot.OrganisationTotal;
                    AllCurrencyTotals.Add(existingCT);
                }
                else
                {
                    CurrencyTotals newCT = new CurrencyTotals(ot.OrganisationTotal, ot.LCID);
                    AllCurrencyTotals.Add(newCT);
                }
            }
        }

        private void GetData()
        {
            int.TryParse(cboSubContractor.SelectedValue, out _identityID);

            if (dteDateFrom.SelectedDate.HasValue)
                _dateFrom = dteDateFrom.SelectedDate.Value;

            if (dteDateTo.SelectedDate.HasValue)
                _dateTo = dteDateTo.SelectedDate.Value;

            AllOrganisationTotals = new List<OrganisationTotals>();

            Facade.IJobSubContractor facJobSubContractor = new Facade.Job();
            grdSubbies.DataSource = facJobSubContractor.GetAllSubContractedJobsForDateRange(IdentityID, DateFrom, DateTo, chkUninvoiceOnly.Checked);
            AllOrganisationTotals.Clear();

            //grdSubbies.DataBind();
        }

        #endregion

        #region Events

        #region Button

        void lnkClear_Click(object sender, EventArgs e)
        {
            cboSubContractor.ClearSelection();
            cboSubContractor.SelectedIndex = 0;
            cboSubContractor.Text = "";
        }

        #endregion

        #region Drop Down

        void cboSubContractor_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboSubContractor.Items.Clear();

            Facade.IReferenceData facReferenceData = new Facade.ReferenceData();
            DataSet ds = facReferenceData.GetAllSubContractorsFiltered(e.Text);

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
                rcItem.Value = dt.Rows[i]["IdentityID"].ToString();
                cboSubContractor.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

        #endregion

        #region Grid

        void grdSubbies_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                char[] characters = { 'j', 'o', 'J', 'O' };
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
                Label lblOrderRate = e.Item.FindControl("lblOrderRate") as Label;

                if (lblSubContractRate != null)
                    lblSubContractRate.Text = dataItem.ForeignRate.ToString("C", new CultureInfo(dataItem.LCID));
                if (lblOrderRate != null)
                    lblOrderRate.Text = dataItem.OrderRate.ToString("C", new CultureInfo(dataItem.LCID)); ;

                #endregion

                #region Addtional References

                HtmlAnchor lnkJobRef = e.Item.FindControl("lnkJobRef") as HtmlAnchor;
                if (lnkJobRef != null)
                {
                    Entities.eSubContractorDataItem reportDataItemType = dataItem.ReportDataItemType;

                    switch (reportDataItemType)
                    {
                        case Entities.eSubContractorDataItem.Job:
                            foreach (string s in dataItem.References)
                            {
                                lnkJobRef.HRef = string.Format("javascript:viewJobDetails({0});", s.TrimStart(characters));
                                lnkJobRef.InnerText = s;
                            }
                            break;
                        case Entities.eSubContractorDataItem.Order:
                            foreach (string s in dataItem.References)
                            {
                                lnkJobRef.HRef = string.Format("javascript:viewOrderProfile({0});", s.TrimStart(characters));
                                lnkJobRef.InnerText = s;
                            }
                            break;
                        default:
                            break;
                    }
                }

                #endregion

                #region Run Id field

                HtmlAnchor lnkRunId = e.Item.FindControl("lnkRunId") as HtmlAnchor;
                if (lnkRunId != null)
                {
                    if (dataItem.JobId > 0)
                    {
                        lnkRunId.HRef = string.Format("javascript:viewJobDetails({0});", dataItem.JobId);
                        lnkRunId.InnerText = dataItem.JobId.ToString();
                    }
                }

                #endregion
            }
        }

        #endregion

        #endregion
    }
}