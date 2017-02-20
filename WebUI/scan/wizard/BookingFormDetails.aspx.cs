using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Orchestrator.WebUI.scan.wizard
{
    public partial class BookingFormDetails : Orchestrator.Base.BasePage
    {
        //--------------------------------------------------------------------------

        public Entities.Scan BookingForm
        {
            get
            {
                Entities.Scan bookingForm = null;

                if (ViewState["BookingForm"] != null)
                    bookingForm = (Entities.Scan)ViewState["BookingForm"];

                return bookingForm;
            }
            set { ViewState["BookingForm"] = value; }
        }

        //--------------------------------------------------------------------------

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

        //--------------------------------------------------------------------------

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

        //--------------------------------------------------------------------------

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

        //--------------------------------------------------------------------------

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

        //--------------------------------------------------------------------------

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
                this.Initialise();

            ((WizardMasterPage)this.Master).WizardTitle = "Document Wizard";
        }

        //--------------------------------------------------------------------------

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }

        //--------------------------------------------------------------------------

        private void Initialise()
        {
            if (this.ScannedFormId > 0)
            {
                Facade.Form facForm = new Facade.Form();
                this.BookingForm = facForm.GetForScannedFormId(this.ScannedFormId);
            }

            int scanFormId = -1;
            // With job POD scanning
            if (this.OrderId > 0)
            {
                Facade.Form facForm = new Facade.Form();

                if (this.BookingForm == null)
                    this.BookingForm = new Entities.Scan();

                this.BookingForm.ScannedDateTime = DateTime.Today;
                this.BookingForm.FormTypeId = eFormTypeId.BookingForm;
                this.BookingForm.ScannedFormPDF = this.FileName;

                if (this.AppendOrReplace.Contains("A"))
                    this.BookingForm.IsAppend = true;
                else
                    this.BookingForm.IsAppend = false;

                // if this is not set then we don't know where to save the file locally so it must have been uploaded immediately.
                if (this.AppendOrReplace.Contains("U") || String.IsNullOrEmpty(Globals.Configuration.ScannedDocumentPath))
                    this.BookingForm.IsUploaded = true;
                else
                    this.BookingForm.IsUploaded = false;

                scanFormId = facForm.Create(this.BookingForm, ((Entities.CustomPrincipal)Page.User).UserName);

                if (scanFormId > 0)
                {
                    Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();
                    facOrder.UpdateBookingFormScannedFormId(this.OrderId, scanFormId, ((Entities.CustomPrincipal)Page.User).UserName);
                }
            }

            this.Close(this.ScannedFormId.ToString());
        }

        //--------------------------------------------------------------------------
    }
}
