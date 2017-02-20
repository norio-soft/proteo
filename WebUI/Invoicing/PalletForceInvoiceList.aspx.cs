using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;

namespace Orchestrator.WebUI.Invoicing
{
    public partial class PalletForceInvoiceList : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                ConfigureDisplay();
        }

        private void ConfigureDisplay()
        {
            Facade.IPalletForceImportPreInvoice facPFPreInvoice = new Facade.PreInvoice();
            
            grdPalletForcePreInvoices.DataSource = facPFPreInvoice.GetAllImportedPreInvoices();
            grdPalletForcePreInvoices.DataBind();
        }
    }
}
