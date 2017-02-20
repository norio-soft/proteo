using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Orchestrator.WebUI.PCV
{
    public partial class pcvcreatedconf : Orchestrator.Base.BasePage
    {
        private int _scannedFormID = -1;
        public int ScannedFormID
        {
            get { return _scannedFormID; }
        }

        private int _pcvID = -1;
        public int PCVID
        {
            get { return _pcvID; }
        }

        private const string vs_returnValue = "vs_returnValue";
        public string ChildReturnValue
        {
            get { return ViewState[vs_returnValue] == null ? string.Empty : (string)ViewState[vs_returnValue]; }
            set { ViewState[vs_returnValue] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if(!string.IsNullOrEmpty(Request.QueryString["sfID"]))
                    int.TryParse(Request.QueryString["sfID"], out _scannedFormID);

                if(!string.IsNullOrEmpty(Request.QueryString["pcvID"]))
                    int.TryParse(Request.QueryString["pcvID"], out _pcvID);

                lblPCVID.Text = PCVID.ToString();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnClose.Click += new EventHandler(btnClose_Click);

            dlgScanDocument.DialogCallBack += new EventHandler(dlgScanDocument_DialogCallBack);
        }

        void dlgScanDocument_DialogCallBack(object sender, EventArgs e)
        {
            Dialog lsd = sender as Dialog;
            if (lsd != null)
                ChildReturnValue = lsd.ReturnValue;
        }

        void btnClose_Click(object sender, EventArgs e)
        {
            this.ReturnValue = ChildReturnValue;
            this.Close();
        }
    }
}
