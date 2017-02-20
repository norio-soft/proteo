using System;
using System.Data;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Text;

namespace Orchestrator.WebUI.Report
{
    public partial class Reports_RevenueReport : Orchestrator.Base.BasePage
    {
        private const string c_TotalRevenue_VS = "vs_TotalRevenue";
        private decimal totalSummaryRevenue = 0m;

        #region Properties

        public DataSet TotalRevenue
        {
            get
            {
                if (ViewState[c_TotalRevenue_VS] == null)
                {
                    int identityID = 0;
                    Facade.IReferenceData facRefData = new Facade.ReferenceData();
                    int.TryParse(cboClient.SelectedValue, out identityID);

                    if (identityID > 0)
                        ViewState[c_TotalRevenue_VS] = facRefData.GetTotalRevenueReport(NominalCodeIDs, rblState.Items[0].Selected, identityID, this.dteStartDate.SelectedDate.Value, this.dteEndDate.SelectedDate.Value);
                    else
                        ViewState[c_TotalRevenue_VS] = null;
                }

                return (DataSet)ViewState[c_TotalRevenue_VS];
            }
            set { ViewState[c_TotalRevenue_VS] = value; }
        }

        public string NominalCodeIDs
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                foreach (ListItem li in chkNominalCodes.Items)
                {
                    if (li.Selected)
                    {
                        if (sb.Length > 0)
                            sb.Append(",");
                        sb.Append(li.Value);
                    }
                }

                return sb.ToString();
            }
        }

        private Decimal TotalSummaryRevenue
        {
            get { return totalSummaryRevenue; }
            set { totalSummaryRevenue = value; }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["rcbID"]))
                return;

            if (!IsPostBack)
                ConfigureDisplay();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnRefresh.Click += new EventHandler(btnRefresh_Click);
            this.cboClient.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
            this.grdSummary.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(grdSummary_ItemDataBound);
        }

        void grdSummary_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                decimal total = 0m;
                decimal.TryParse(((System.Data.DataRowView)(e.Item.DataItem)).Row["Total"].ToString(), out total);
                TotalSummaryRevenue += total;
            }
            else if (e.Item is GridFooterItem)
            {
                Label lblPalletTotal = (Label)e.Item.FindControl("lblTotalUninvoiced");
                lblPalletTotal.Text = TotalSummaryRevenue.ToString("C");
            }
        }

        private void ConfigureDisplay()
        {
            DateTime startDate = DateTime.Today.AddMonths(-1);
            System.Globalization.GregorianCalendar cal = new System.Globalization.GregorianCalendar();
            this.dteStartDate.SelectedDate = new DateTime(startDate.Year, startDate.Month, 01);
            this.dteEndDate.SelectedDate = new DateTime(startDate.Year, startDate.Month, cal.GetDaysInMonth(startDate.Year, startDate.Month));

            Facade.INominalCode facNom = new Facade.NominalCode();
            DataSet ds = facNom.GetAll();

            chkNominalCodes.DataSource = ds;
            chkNominalCodes.DataBind();
        }

        #region DropDowns

        void cboClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboClient.Items.Clear();

            Facade.IReferenceData facReferenceData = new Facade.ReferenceData();
            DataSet ds = facReferenceData.GetAllClientsFiltered(e.Text);

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
                cboClient.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

        #endregion

        #region Buttons

        void btnRefresh_Click(object sender, EventArgs e)
        {
            Facade.IReferenceData facRefData = new Facade.ReferenceData();
            int identityID = 0;
            int.TryParse(cboClient.SelectedValue, out identityID);

            TotalRevenue = facRefData.GetTotalRevenueReport(NominalCodeIDs, rblState.Items[0].Selected, identityID, dteStartDate.SelectedDate.Value, dteEndDate.SelectedDate.Value);
            grdSummary.DataSource = TotalRevenue;
            grdSummary.Rebind();
        }

        #endregion

    }
}
