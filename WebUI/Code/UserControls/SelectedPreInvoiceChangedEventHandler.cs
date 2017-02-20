using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Orchestrator.WebUI
{
    public delegate void SelectedPreInvoiceChangedEventHandler(object sender, SelectedPreInvoiceChangedEventArgs e);

    public class SelectedPreInvoiceChangedEventArgs : EventArgs
    {
        private int _preInvoiceId = -1;

        public SelectedPreInvoiceChangedEventArgs(int preInvoiceId)
        {
            _preInvoiceId = preInvoiceId;
        }

        public int PreInvoiceID
        {
            get { return _preInvoiceId; }
        }
    }

}