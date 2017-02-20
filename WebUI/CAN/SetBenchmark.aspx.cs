using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Orchestrator.WebUI.CAN
{
    public partial class SetBenchmark : Orchestrator.Base.BasePage
    {
        private DateTime FromDate
        {
            get { return DateTime.Parse(this.Request.QueryString["FromDate"]);  }
        }

        private DateTime ToDate
        {
            get { return DateTime.Parse(this.Request.QueryString["ToDate"]); }
        }

        private int? OrgUnitId
        {
            get 
            { 
                string s = this.Request.QueryString["OrgUnitId"];
                if (string.IsNullOrEmpty(s))
                    return null;
                else
                    return int.Parse(s);
            }
        }

        private int? DriverIdentityId
        {
            get
            {
                string s = this.Request.QueryString["Id"];
                if (string.IsNullOrEmpty(s))
                    return null;
                else
                    return int.Parse(s);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            btnSetBenchmark.Click += new EventHandler(btnSetBenchmark_Click);

            if (!this.IsPostBack)
            {
                LoadData();
            }

        }

        void btnSetBenchmark_Click(object sender, EventArgs e)
        {
            int CANBenchmarkTypeId = int.Parse(optBenchMarkType.SelectedValue);
            bool isMPGOnly = (optBenchmarkValues.SelectedValue == "MPGOnly");

            CreateBenchmark(CANBenchmarkTypeId, isMPGOnly);

            btnSetBenchmark.Enabled = false;
            lblSuccess.Visible = true;
            this.Close();
        }

        void LoadData()
        {
            lblPeriodValue.Text = String.Format("From {0:d} to {1:d}", this.FromDate, this.ToDate);

            if (this.OrgUnitId.HasValue)
                lblGroupingValue.Text = EF.DataContext.Current.OrgUnit_GetPathForOrgUnit(this.OrgUnitId.Value);
            else
                lblGroupingValue.Text = "Not grouped";

            if (this.DriverIdentityId.HasValue)
            {
                var driver = ( 
                    from d in EF.DataContext.Current.Drivers.Include("Individual")
                    where d.Individual.IdentityId == this.DriverIdentityId
                    select d).FirstOrDefault();

                if (driver != null)
                    lblDriverValue.Text = driver.Individual.FirstNames + " " + driver.Individual.LastName;
            }
            else
                lblDriverValue.Text = "Average of all drivers";
        }

        void CreateBenchmark(int CANBenchmarkTypeId, bool isMPGOnly)
        {
            EF.DataContext.Current.CANBenchmark_Create(this.FromDate, this.ToDate, CANBenchmarkTypeId, this.DriverIdentityId, this.OrgUnitId, isMPGOnly);
        }
    }
}