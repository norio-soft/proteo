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

namespace Orchestrator.WebUI.ExchangeRate
{
    public partial class AddExchangeRate : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                ConfigureDisplay();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnAdd.Click += new EventHandler(btnAdd_Click);
            btnClose.Click += new EventHandler(btnClose_Click);
            cfvCurrency.ServerValidate += new ServerValidateEventHandler(cfvCurrency_ServerValidate);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "scriptCloseWindow",
                            @"<script language='javascript' type='text/javascript'>
                                    GetRadWindow().Close();
                            </script>");
        }

        private void ConfigureDisplay()
        {
            Label PageTitle = ((System.Web.UI.UserControl)(Page.Master)).FindControl("lblWizardTitle") as Label;

            if (PageTitle != null)
                PageTitle.Text = "Add Exchange Rate";

            rdiEffectiveFrom.MinDate = DateTime.Today.AddYears(-1);
            rntExchangeRate.Culture.NumberFormat.NumberDecimalDigits = 5;

            //rntExchangeRate.MaxValue = 5.00000;
            //rntExchangeRate.MinValue = 0.00001;
            //rntExchangeRate.MaxLength = 7;
            //rntExchangeRate.Culture.NumberFormat.CurrencyDecimalDigits = 5;

            Facade.ExchangeRates facER = new Facade.ExchangeRates();
            DataSet currencies = facER.GetAllCurrencies();

            rcbCurrency.Items.Clear();
            RadComboBoxItem rcbi = new RadComboBoxItem("Please Select", "-1");
            rcbCurrency.Items.Add(rcbi);

            CultureInfo culture = new CultureInfo(Orchestrator.Globals.Configuration.NativeCulture);

            foreach (DataRow dr in currencies.Tables[0].Rows)
                if (dr["CurrencyID"].ToString() != Facade.Culture.GetCurrencySymbol(culture.LCID))
                {
                    RadComboBoxItem trcbi = new RadComboBoxItem(dr["CurrencyName"].ToString(), dr["CurrencyID"].ToString());
                    rcbCurrency.Items.Add(trcbi);
                }
        }

        private Entities.ExchangeRate GetPageValues()
        {
            Entities.ExchangeRate exRate = new Entities.ExchangeRate();

            exRate.CurrencyID = rcbCurrency.SelectedValue;
            exRate.EffectiveDate = rdiEffectiveFrom.SelectedDate.HasValue ? (DateTime)rdiEffectiveFrom.SelectedDate : DateTime.MinValue;
            exRate.ExchangeRateValue = Convert.ToDecimal(rntExchangeRate.Value);

            return exRate;
        }

        void cfvCurrency_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (rcbCurrency.SelectedValue == "-1")
                args.IsValid = false;
            else
                args.IsValid = true;

            rntExchangeRate.Culture.NumberFormat.NumberDecimalDigits = 5;
        }

        void btnAdd_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                int exchangeRateID = -1;
                Facade.ExchangeRates facER = new Facade.ExchangeRates();
                Entities.ExchangeRate exRate = GetPageValues();

                exchangeRateID = facER.Create(exRate, Page.User.Identity.Name);

                if (exchangeRateID > -1)
                {
                    btnAdd.Enabled = false;
                    pnlSuccess.Visible = true;
                }
                else
                    pnlFailed.Visible = true;
            }
        }
    }
}
