using System;
using System.ComponentModel;
using System.Text;
using System.Xml;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Orchestrator.WebUI
{
    public partial class getOrderCollectionDeliveryNotes : Orchestrator.Base.BasePage
    {
        private static string note = @"<div>
                                           <div style=""font-weight:bold;"">{0}</div>
                                           <div>{1}</div>
                                       </div>";

        protected void Page_Load(object sender, EventArgs e)
        {
            int orderID = -1;
            bool isCollection = false;
            int.TryParse(Request.QueryString["orderID"], out orderID);
            bool.TryParse(Request.QueryString["isCollection"], out isCollection);

            StringBuilder sb = new StringBuilder();

            Entities.Order order = null;

            if (orderID > 0)
            {
                string cacheName = "_orderCollectionDeliveryNotes" + orderID.ToString();

                if (Cache[cacheName] == null)
                {
                    Facade.IOrder facOrder = new Facade.Order();
                    order = facOrder.GetForOrderID(orderID);
                    if (order != null)
                        Cache.Add(cacheName, order, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 20, 0), System.Web.Caching.CacheItemPriority.Normal, null);
                }
                else
                    order = (Entities.Order)Cache[cacheName];

                if (isCollection)
                    sb.Append(string.Format(note, "Collection Notes", order.CollectionNotes));
                else
                    sb.Append(string.Format(note, "Delivery Notes", order.DeliveryNotes));

                Response.Write(sb.ToString());
                Response.Flush();
                Response.End();
            }
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
