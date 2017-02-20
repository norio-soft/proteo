using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Orchestrator.WebUI.Resource
{
    public partial class getResourceScheduleDatahtml : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            GetQueryStringVariables();
        }

        protected string m_ResourceScheduleId;
        protected string m_Description;
        protected DateTime m_FromDate;
        protected DateTime m_ToDate;


        private void GetQueryStringVariables()
        {
            m_Description = Request.QueryString["Description"];
            m_ResourceScheduleId = Request.QueryString["ResourceId"];

            m_FromDate = DateTime.Parse(Request.QueryString["FromDate"]);
            m_ToDate = DateTime.Parse(Request.QueryString["ToDate"]);
            
            lblFromDate.Text = RemoveTimePartIfZero(m_FromDate);
            lblToDate.Text = RemoveTimePartIfZero(m_ToDate);

            lblResourceScheduleDescription.Text = "<b><u>" + m_Description + "</u></b>";
        }

        /// <summary>
        /// Returns only the date part if the time is 00:00:00
        /// </summary>
        /// <param name="time"></param>
        /// <returns>Returns a string version of the datetime</returns>
        private string RemoveTimePartIfZero(DateTime time)
        {
            return time.TimeOfDay == new TimeSpan(0, 0, 0, 0) ? time.ToShortDateString() : time.ToString();
        }
    }
}