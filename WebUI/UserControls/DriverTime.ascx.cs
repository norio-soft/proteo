using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Orchestrator.WebUI.UserControls
{

    public partial class DriverTime : System.Web.UI.UserControl
    {

        private int WeekStartDay
        {
            get
            {
                var retVal = Globals.Configuration.ReportingFirstDayOfWeek;

                if (!retVal.HasValue)
                {
                    var culture = new CultureInfo(Globals.Configuration.NativeCulture);
                    retVal = culture.DateTimeFormat.FirstDayOfWeek;
                }

                return (int)retVal.Value;
            }
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!IsPostBack)
            {
                this.hidDriverTimeWeekStartDay.Value = this.WeekStartDay.ToString();
            }
        }

    }

}