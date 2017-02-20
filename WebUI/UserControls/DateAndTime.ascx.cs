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

namespace Orchestrator.WebUI.UserControls
{
    public partial class DateAndTime : System.Web.UI.UserControl
    {


        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string DateToolTip
        {
            set { dteDate.ToolTip = value; }
        }

        public string TimeToolTip
        {
            set { dteTime.ToolTip = value; }
        }

        public DateTime Date
        {
            set
            {
                dteDate.SelectedDate = value;
                dteTime.SelectedDate = value;
            }

            get
            {
                DateTime retVal = DateTime.Now;
                if (dteTime.SelectedDate.HasValue)
                {
                    retVal = dteDate.SelectedDate.Value;
                    retVal = retVal.Add(dteTime.SelectedDate.Value.TimeOfDay);
                }
                return dteDate.SelectedDate.Value;
            }
        }

        public bool AnyTime
        {
            get { return !dteTime.SelectedDate.HasValue ? true : false; }
            set
            {
                dteTime.SelectedDate = (DateTime?)null;
            }
        }

        public string DateFormat
        {
            set { dteDate.DateFormat = value; }
        }

        public string TimeFormat
        {
            set { dteTime.DateFormat = value; }
        }



    }
}