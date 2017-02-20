using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Orchestrator.WebUI.Consortium
{
    public partial class OrderAllocationHistory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int? orderID = Utilities.ParseNullable<int>(Request.QueryString["oid"]);
            if (!orderID.HasValue)
                return;

            Facade.IOrder facOrder = new Facade.Order();
            rgHistory.DataSource = facOrder.GetOrderSubcontractorHistory(orderID.Value);
            rgHistory.DataBind();
        }
    }
}
