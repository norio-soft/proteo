using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Orchestrator.WebUI.FuelSurcharge
{
    public partial class AddFuelSurcharge : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                ConfigureDisplay();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.btnAdd.Click += new EventHandler(btnAdd_Click);
        }

        private void ConfigureDisplay()
        {
            //Label PageTitle = ((System.Web.UI.UserControl)(Page.Master)).FindControl("lblWizardTitle") as Label;

            //if (PageTitle != null)
            //    PageTitle.Text = "Add Fuel Surcharge Rate";

            rdiEffectiveFrom.MinDate = DateTime.Today.AddYears(-1);
            rdiEffectiveFrom.SelectedDate = DateTime.Now; // Default to current day.

            rntFuelSurchargeRate.Type = Telerik.Web.UI.NumericType.Number;
            rntFuelSurchargeRate.Culture.NumberFormat.NumberDecimalDigits = 2;
        }

        void btnAdd_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                int noSaved = 0;
                var fuelSurcharge = (from fs in this.DataContext.FuelSurchargeSet
                                where fs.EffectiveDate == rdiEffectiveFrom.SelectedDate
                                select fs).FirstOrDefault();

                if (fuelSurcharge == null)
                {
                    fuelSurcharge = new EF.FuelSurcharge();
                    this.DataContext.AddToFuelSurchargeSet(fuelSurcharge);
                    fuelSurcharge.EffectiveDate = rdiEffectiveFrom.SelectedDate.Value;
                }

                decimal rate = Convert.ToDecimal(rntFuelSurchargeRate.Value.Value);

                fuelSurcharge.SurchargeRate = rate;
                noSaved = this.DataContext.SaveChanges();

                this.ReturnValue = "refresh";
                this.Close();
            }
        }
    }
}
