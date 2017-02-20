using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using System.IO;
using Telerik.Web.UI;

using Orchestrator.Entities;

namespace Orchestrator.WebUI.document
{
    public partial class upload : System.Web.UI.Page
    {
        #region page properties

        private eFormTypeId FormType
        {
            get
            {
                eFormTypeId formType = eFormTypeId.POD;

                if (!String.IsNullOrEmpty(Request.QueryString["type"]))
                    formType = (eFormTypeId)Enum.Parse(typeof(eFormTypeId), Request.QueryString["type"]);

                return formType;
            }
        }

        private int OrderID
        {
            get
            {
                int orderID = -1;
                int.TryParse(Request.QueryString["oid"], out orderID);
                return orderID;
            }
        }

        private int CollectDropID
        {
            get
            {
                int collectDropID = -1;
                int.TryParse(Request.QueryString["cdid"], out collectDropID);
                return collectDropID;
            }
        }

        

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // This property defaults to eFormTypeId.POD if it has not been supplied
                if (this.FormType == eFormTypeId.POD)
                {
                    Facade.POD facPod = new Orchestrator.Facade.POD();
                    Entities.POD pod = null;

                    if (this.OrderID > 0)
                        pod = facPod.GetForOrderID(OrderID);
                    if (this.CollectDropID > 0)
                        pod = facPod.GetPODForCollectDropID(CollectDropID);

                    if (pod != null)
                    {
                        lblDescription.Text = "There has already been a PDF uploaded for this order, if you upload another one this will overrwite the existing file.";
                        txtTicketNo.Text = pod.TicketNo;
                        dteSignatureDate.SelectedDate = pod.SignatureDate;
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(Request.QueryString["ticket"]))
                            txtTicketNo.Text = Request.QueryString["ticket"];
                        if (!String.IsNullOrEmpty(Request.QueryString["sigDate"]))
                            dteSignatureDate.SelectedDate = DateTime.Parse(Request.QueryString["sigDate"]);
                    }
                }
                // This property defaults to eFormTypeId.POD if it has not been supplied
                else if(this.FormType == eFormTypeId.BookingForm)
                {
                    Orchestrator.Facade.Form facBF = new Orchestrator.Facade.Form();
                    Facade.IOrder facOrder = new Orchestrator.Facade.Order();

                    Entities.Order order = null;
                    Entities.Scan bookingForm = null;

                    txtTicketNo.Visible = false;
                    lblTicketRef.Visible = false;
                    this.btnUpload.CausesValidation = false;
                    dteSignatureDate.Visible = false;
                    lblSignatureDate.Visible = false;

                    if (this.OrderID > 0)
                        order = facOrder.GetForOrderID(OrderID);

                    if (order != null && order.BookingFormScannedFormId != null)
                        bookingForm = facBF.GetForScannedFormId((int)order.BookingFormScannedFormId);

                    if (bookingForm != null)
                        lblDescription.Text = "There has already been a booking form uploaded for this order, if you upload another one this will overwrite the existing file.";
                    else
                        lblDescription.Text = String.Empty;
                }
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnUpload.Click += new EventHandler(btnUpload_Click);
        }

        void btnUpload_Click(object sender, EventArgs e)
        {
            string fileDestination = Server.MapPath("~/PDFS");
            string uploadDirectory = fileDestination;

            if (!fileDestination.EndsWith("\\"))
                fileDestination += "\\";

            if (radUpload.UploadedFiles.Count == 0)
                lblDescription.Text = "Please select a file to upload.";
            else
                foreach (UploadedFile f in radUpload.UploadedFiles)
                {
                    fileDestination += DateTime.Now.Year.ToString() + "\\" + DateTime.Now.Month.ToString() + "\\" + DateTime.Now.Day.ToString();

                    // Determine the folder structure and ensure that this exists.
                    if (!Directory.Exists(fileDestination))
                        Directory.CreateDirectory(fileDestination);

                    if (!fileDestination.EndsWith("\\"))
                        fileDestination += "\\";

                    // Set final destination and file name 
                    fileDestination += f.GetName();

                    f.SaveAs(fileDestination, true);

                    // This property defaults to eFormTypeId.POD if it has not been supplied
                    if (this.FormType == eFormTypeId.POD )
                        CreatePOD(fileDestination.Replace(uploadDirectory, ""));
                    else if(this.FormType == eFormTypeId.BookingForm)
                        CreateBookingForm(fileDestination.Replace(uploadDirectory, ""));
                }
        }

        private void CreateBookingForm(string fileName)
        {
            //if (fileName.StartsWith("\\"))
            //    fileName = fileName.Substring(1);

            Orchestrator.Facade.Form facBF = new Orchestrator.Facade.Form();
            Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();

            Entities.Scan bookingForm = new Scan();
            bookingForm.FormTypeId = eFormTypeId.BookingForm;
            bookingForm.ScannedDateTime = DateTime.UtcNow;
            bookingForm.ScannedFormPDF = fileName;

            try
            {
                bookingForm.ScannedFormId = facBF.Create(bookingForm, ((Entities.CustomPrincipal)Page.User).UserName);
                facOrder.UpdateBookingFormScannedFormId(OrderID, bookingForm.ScannedFormId, ((Entities.CustomPrincipal)Page.User).UserName);

                this.ClientScript.RegisterStartupScript(this.GetType(), "CloseWindow", "<script type=\"text/javascript\">opener.location.href=opener.location.href; window.close();</script>");
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
                lblError.Visible = true;
            }
        }

        private void CreatePOD(string fileName)
        {
            //if (fileName.StartsWith("\\"))
            //    fileName = fileName.Substring(1);

            Entities.POD pod = null;
            Facade.POD facPOD = new Orchestrator.Facade.POD();
           
            //Determine the Delivery job and collect drop for this order.
            if (this.OrderID > 0)
                pod = facPOD.GetForOrderID(OrderID);
            if (this.CollectDropID > 0)
                pod = facPOD.GetPODForCollectDropID(this.CollectDropID);

            // Is there already a POD for this collect drop?

            if (pod == null)
            {
                pod = new Orchestrator.Entities.POD();

                // get the details we need for the pod record.

                Facade.Order facOrder = new Orchestrator.Facade.Order();
                Facade.ICollectDrop facCollectDrop = new Orchestrator.Facade.CollectDrop();
                Dictionary<string, int> orderDetails = null;
                if (this.OrderID > 0)
                    orderDetails = facOrder.GetDeliveryDetails(this.OrderID);
                else
                    orderDetails = facCollectDrop.GetDeliveryDetails(this.CollectDropID);

                // Do we default to now (upload date) or do we look at the create date of the s

                pod.CollectDropId = orderDetails["CollectDropID"];
                pod.JobId = orderDetails["JobID"];
                pod.OrganisationId = orderDetails["CustomerIdentityID"];
                pod.FormTypeId = eFormTypeId.POD;

            }
           
            pod.ScannedDateTime = DateTime.Now;
            pod.SignatureDate = (DateTime)dteSignatureDate.SelectedDate;
            pod.TicketNo = txtTicketNo.Text;
            pod.ScannedFormPDF = fileName;
            try
            {
                if (pod.PODId > 0)
                    facPOD.Update(pod, Page.User.Identity.Name);
                else
                    facPOD.Create(pod, pod.JobId, pod.OrganisationId, pod.CollectDropId, Page.User.Identity.Name);

                this.ClientScript.RegisterStartupScript(this.GetType(), "CloseWindow", "<script type=\"text/javascript\">opener.location.href=opener.location.href; window.close();</script>");
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
                lblError.Visible = true;
            }
        }
    }
}
