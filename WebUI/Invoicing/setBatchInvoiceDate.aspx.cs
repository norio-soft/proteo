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

namespace Orchestrator.WebUI.Invoicing
{
    public partial class setBatchInvoiceDate : Orchestrator.Base.BasePage
    {
        private int m_batchId;

        protected void Page_Load(object sender, EventArgs e)
        {
            m_batchId = int.Parse(Request.QueryString["BatchId"]);

            if (!IsPostBack)
            {
                dteInvoiceDate.SelectedDate = Convert.ToDateTime(Request.QueryString["InvoiceDate"]);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnConfirm.Click += new EventHandler(btnConfirm_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            mwhelper.CausePostBack = false;
            mwhelper.OutputData = dteInvoiceDate.SelectedDate.Value.ToString("dd/MM/yyyy");
            mwhelper.CloseForm = true;
        }

        void btnConfirm_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                Facade.IInvoiceBatches facInvoiceBatches = new Facade.Invoice();

                if (facInvoiceBatches.UpdateInvoiceDate(m_batchId, dteInvoiceDate.SelectedDate.Value, ((Entities.CustomPrincipal)Page.User).UserName))
                {
                    mwhelper.CausePostBack = false;
                    mwhelper.OutputData = dteInvoiceDate.SelectedDate.Value.ToString("dd/MM/yyyy");
                    mwhelper.CloseForm = true;
                }
            }
        }
    }
}
