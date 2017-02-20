using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Orchestrator.WebUI
{
	/// <summary>
	/// Summary description for GetPointAddressHtml.
	/// </summary>
	public partial class GetOrderNotesHTML : Orchestrator.Base.BasePage
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			int orderID = Convert.ToInt32(Request.QueryString["orderID"]);

            Entities.Order order = null;

            if (orderID > 0)
            {
                string cacheName = "_orderNotes" + orderID.ToString();

                if (Cache[cacheName] == null)
                {
                    Facade.IOrder facOrder = new Facade.Order();
                    order = facOrder.GetForOrderID(orderID);
                    if (order != null)
                        Cache.Add(cacheName, order, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 20, 0), System.Web.Caching.CacheItemPriority.Normal, null);
                }
                else
                {
                    order = (Entities.Order)Cache[cacheName];
                }

            }
            lblNotes.Text = order.Notes;
            
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
		}
		#endregion
	}
}
