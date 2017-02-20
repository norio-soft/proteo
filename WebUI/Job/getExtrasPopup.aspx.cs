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


namespace Orchestrator.WebUI.Job
{
    public partial class GetExtrasPopup : Orchestrator.Base.BasePage
    {
        private const string extraPopupTemplate = @"<html xmlns=""http://www.w3.org/1999/xhtml"" >
                                                        <head id=""hdr"" runat=""server"">
                                                            <title>Orchestrator</title>
                                                            <base target=""_self"" />
                                                        </head>
                                                        <body>
                                                            {0}
                                                        </body>
                                                    </html>";

        protected void Page_Load(object sender, EventArgs e)
        {
            int jobId = Convert.ToInt32(Request.QueryString["jobId"]);
            string retVal = string.Empty;

			if (jobId > 0)
			{
                Entities.Extra extra = null;
                string cacheName = "_jobExtras_" + jobId.ToString();
                System.Text.StringBuilder sbOuput = new System.Text.StringBuilder();
                if (Cache[cacheName] == null)
                {
                    DataAccess.IJobExtra dacExtra = new DataAccess.Job();

                    System.Data.SqlClient.SqlDataReader reader = dacExtra.GetExtrasForJobId(jobId);

                    //build up the return string information
                    sbOuput.Append("<table cellpadding=\"3\" class=\"DataGridStyle\" ");
                    sbOuput.Append("<tr style=\"background-image: url('../images/header_rowBg.gif');background-repeat: repeat-x;height:24px;\"><td>Type</td><td>Amount</td><td>Accepted By</td></tr>");
                    while (reader.Read())
                    {
                        sbOuput.Append("<tr><td>" + reader["ExtraType"].ToString() + "</td>");
                        sbOuput.Append("<td>" + ((decimal)reader["ExtraAmount"]).ToString("C") + "</td>");
                        sbOuput.Append("<td>" + reader["ClientContact"].ToString() + "</td>");
                    }

                    sbOuput.Append("</tr></table>");
                    reader.Close();

                    Cache.Add(cacheName, sbOuput.ToString(), null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 20, 0), System.Web.Caching.CacheItemPriority.Normal, null);
                    retVal = sbOuput.ToString();
                }
                else
                {
                    retVal = Cache[cacheName].ToString();
                }
    
				if (retVal != null)
				{
                    litExtraPopup.Text = string.Empty;
                    litExtraPopup.Text = retVal.ToString();
					return;
				}
			}
        }
    }
    
}