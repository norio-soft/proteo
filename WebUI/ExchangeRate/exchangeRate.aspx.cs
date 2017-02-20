using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Globalization;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.currency
{
    public partial class exchangeRate : System.Web.UI.Page
    {
        private DateTime EffectiveDate
        {
            get
            {
                if (rdiExchange.SelectedDate.HasValue)
                    return (DateTime)rdiExchange.SelectedDate;
                
                return new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                ConfigureDisplay();
        
        }

        private void ConfigureDisplay()
        {
            CultureInfo culture = new CultureInfo(Orchestrator.Globals.Configuration.NativeCulture);

            Facade.IExchangeRates facER = new Facade.ExchangeRates();
            DataSet currencies = facER.GetAllCurrencies();

            rcbCurrency.Items.Clear();
            RadComboBoxItem rcbi = new RadComboBoxItem("Please Select", "-1");
            rcbCurrency.Items.Add(rcbi);

            if (currencies.Tables.Count > 0 && currencies.Tables[0].Rows.Count > 0)
                foreach (DataRow dr in currencies.Tables[0].Rows)
                    if (dr["CurrencyID"].ToString() != Facade.Culture.GetCurrencySymbol(culture.LCID))
                    {
                        RadComboBoxItem trcbi = new RadComboBoxItem(dr["CurrencyName"].ToString(), dr["CurrencyID"].ToString());
                        rcbCurrency.Items.Add(trcbi);
                    }

            //If only 1 currency available, select it by default.
            if (rcbCurrency.Items.Count == 2)
                rcbCurrency.SelectedIndex = 1;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnRefresh.Click += new EventHandler(btnRefresh_Click);
            rgExchangeRates.SortCommand += new GridSortCommandEventHandler(rgExchangeRates_SortCommand);
            rgExchangeRates.PageIndexChanged += new GridPageChangedEventHandler(rgExchangeRates_PageIndexChanged);
        }

        #region Events

        void btnRefresh_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                GetDataForDataGrid();
            }
        }

        void rgExchangeRates_SortCommand(object source, GridSortCommandEventArgs e)
        {
            GetDataForDataGrid();
        }

        void rgExchangeRates_PageIndexChanged(object source, GridPageChangedEventArgs e)
        {
            GetDataForDataGrid();
        }

        void GetDataForDataGrid()
        {
            Facade.IExchangeRates facER = new Facade.ExchangeRates();

            if (rdiExchange.SelectedDate.HasValue)
                rgExchangeRates.DataSource = facER.GetForSelectedCurrencyAndDate(rcbCurrency.SelectedValue, (DateTime)rdiExchange.SelectedDate.Value);
            else
                rgExchangeRates.DataSource = facER.GetForSelectedCurrency(rcbCurrency.SelectedValue);
 
            rgExchangeRates.DataBind();
        }

        #endregion
    }
}
