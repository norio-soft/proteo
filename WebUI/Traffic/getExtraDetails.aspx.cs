using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Orchestrator.WebUI.Traffic
{
    public partial class Traffic_getExtraDetails : Orchestrator.Base.BasePage
    {
        #region Templates

        private string header = @"<table width=""300"" border=""0"" cellpadding=""2"" cellspacing=""1"">
                                    <tbody>
                                        <tr>
                                            <td><b>Order</b></td>
                                            <td><b>Type</b></td>
                                            <td><b>State</b></td>
                                            <td><b>Contact</b></td>
                                            <td><b>Amount</b></td>
                                        </tr>";

        private string content = @"<tr>
                                       <td valign=""top"">{0}</td>
                                       <td valign=""top"">{1}</td>
                                       <td valign=""top"">{2}</td>
                                       <td valign=""top"">{3}</td>
                                       <td valign=""top"">{4}</td>
                                   </tr>";

        private string footer = @"  </tbody>
                                  </table>";

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            int instructionID = Convert.ToInt32(Request.QueryString["InstructionID"]);
            DataSet extra = null;

            if (instructionID > 0)
            {
                string cacheName = "_instruction" + instructionID.ToString();

                if (Cache[cacheName] == null)
                {
                    Facade.IJob facJob = new Facade.Job();
                    extra = facJob.GetExtrasForInstructionID(instructionID);
                    Cache.Add(cacheName, extra, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 20, 0), System.Web.Caching.CacheItemPriority.Normal, null);
                }
                else
                {
                    extra = (DataSet)Cache[cacheName];
                }
            }

            if (extra != null)
            {
                StringBuilder output = new StringBuilder();
                output.Append(header);

                foreach (DataRow dr in extra.Tables[0].Rows)
                    output.Append(string.Format(content, dr["OrderID"].ToString(), dr["ExtraType"].ToString(), dr["ExtraState"].ToString(), dr["ClientContact"].ToString(), ((decimal)dr["ExtraAmount"]).ToString("C")));

                output.Append(footer);

                phExtras.Controls.Add(new LiteralControl(output.ToString()));
            }
        }
    }

}
