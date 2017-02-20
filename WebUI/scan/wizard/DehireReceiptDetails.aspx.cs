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

namespace Orchestrator.WebUI.scan.wizard
{
    public partial class DehireReceiptDetails : Orchestrator.Base.BasePage
    {

        #region Properties

        private const string vs_DehireReceipt = "vs_DehireReceipt";
        public Entities.Scan DehireReceipt
        {
            get
            {
                Entities.Scan bookingForm = null;

                if (ViewState[vs_DehireReceipt] != null)
                    bookingForm = (Entities.Scan)ViewState[vs_DehireReceipt];

                return bookingForm;
            }
            set { ViewState[vs_DehireReceipt] = value; }
        }

        protected int ScannedFormId
        {
            get
            {
                int formId = -1;
                if (Request.QueryString["ScannedFormId"] != null)
                    formId = int.Parse(Request.QueryString["ScannedFormId"].ToString());

                return formId;
            }
        }

        protected string AppendOrReplace
        {
            get
            {
                string appendOrReplace = String.Empty;
                if (Request.QueryString["AppendOrReplace"] != null)
                    appendOrReplace = Request.QueryString["AppendOrReplace"].ToString();

                return appendOrReplace;
            }
        }

        protected int OrderId
        {
            get
            {
                int orderId = -1;
                if (Request.QueryString["OrderId"] != null)
                    orderId = int.Parse(Request.QueryString["OrderId"].ToString());

                return orderId;
            }
        }

        protected int DehireReceiptId
        {
            get
            {
                int dehireReceiptId = -1;
                if (Request.QueryString["DeHireReceiptId"] != null)
                    dehireReceiptId = int.Parse(Request.QueryString["DeHireReceiptId"].ToString());

                return dehireReceiptId;
            }
        }

        protected string DehireReceiptNumber
        {
            get
            {
                string dehireReceiptNumber = string.Empty;
                if (Request.QueryString["DHNo"] != null)
                    dehireReceiptNumber = Request.QueryString["DHNo"].ToString();

                return dehireReceiptNumber;
            }
        }

        protected string FileName
        {
            get
            {
                string fileName = String.Empty;
                if (Request.QueryString["FileName"] != null)
                    fileName = Request.QueryString["FileName"].ToString();

                return fileName;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
                this.Initialise();

            ((WizardMasterPage)this.Master).WizardTitle = "Document Wizard";
        }
        
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnNext.Click += new EventHandler(btnNext_Click);
        }

        void btnNext_Click(object sender, EventArgs e)
        {
            if (this.ScannedFormId > 0)
            {
                Facade.Form facForm = new Facade.Form();
                this.DehireReceipt = facForm.GetForScannedFormId(this.ScannedFormId);
            }

            int scanFormId = -1;
            // With job POD scanning
            if (this.OrderId > 0)
            {
                Facade.Form facForm = new Facade.Form();
                Facade.IDeHireReceipt facDehireReceipt = new Facade.CollectDrop();

                string userID = ((Entities.CustomPrincipal)Page.User).UserName;

                if (this.DehireReceipt == null)
                    this.DehireReceipt = new Entities.Scan();

                this.DehireReceipt.ScannedDateTime = DateTime.Today;
                this.DehireReceipt.FormTypeId = eFormTypeId.DehireReceipt;
                this.DehireReceipt.ScannedFormPDF = this.FileName;

                // N = No document available
                if (this.AppendOrReplace.Contains("N"))
                {
                    if (this.DehireReceipt.ScannedFormId == 0)
                        this.DehireReceipt.ScannedFormPDF = Orchestrator.Globals.Constants.NO_DOCUMENT_AVAILABLE;
                }
                else
                    this.DehireReceipt.ScannedFormPDF = this.FileName;

                if (this.AppendOrReplace.Contains("A"))
                    this.DehireReceipt.IsAppend = true;
                else
                    this.DehireReceipt.IsAppend = false;

                // if this is not set then we don't know where to save the file locally so it must have been uploaded immediately.
                if (this.AppendOrReplace.Contains("U") || String.IsNullOrEmpty(Globals.Configuration.ScannedDocumentPath))
                    this.DehireReceipt.IsUploaded = true;
                else
                    this.DehireReceipt.IsUploaded = false;

                // Update the receipt number,
                facDehireReceipt.UpdateReceiptNumber(DehireReceiptId, txtDehireReceiptNumber.Text, userID);
                // Create the scanned form.
                scanFormId = facForm.Create(this.DehireReceipt, userID);

                if (scanFormId > 0)
                    facDehireReceipt.UpdateScannedForm(DehireReceiptId, scanFormId, userID);
            }

            this.Close(this.ScannedFormId.ToString());
        }
        
        private void Initialise()
        {
            ((WizardMasterPage)this.Master).WizardTitle = "Document Wizard";

            txtDehireReceiptNumber.Text = DehireReceiptNumber;
        }
    }
}
