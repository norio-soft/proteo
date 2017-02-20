using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Orchestrator;
using System.IO;

namespace Orchestrator.WebUI.scan.wizard
{
    public partial class ScanOrUpload : Orchestrator.Base.BasePage
    {
        #region PageVariables

            private const string NEW_WEBTWAIN_LIC_PATH = "//scan//wizard//Resources//lic.txt";

        #endregion

        protected PODDialogMode dialogMode = PODDialogMode.Scan;
        protected enum PODDialogMode
        {
            Scan,
            Manual
        }


        //--------------------------------------------------------------------------

        protected int ScannedFormTypeId
        {
            get
            {
                int formTypeId = -1;
                if (Request.QueryString["ScannedFormTypeId"] != null)
                    formTypeId = int.Parse(Request.QueryString["ScannedFormTypeId"].ToString());

                return formTypeId;
            }
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

        protected int OrganisationIdentityId
        {
            get
            {
                int orgId = -1;
                if (Request.QueryString["OrgId"] != null)
                    orgId = int.Parse(Request.QueryString["OrgId"].ToString());

                return orgId;
            }
        }

        //--------------------------------------------------------------------------

        protected int OrganisationDocumentId
        {
            get
            {
                int orgDocId = -1;
                if (Request.QueryString["OrgDocId"] != null)
                    orgDocId = int.Parse(Request.QueryString["OrgDocId"].ToString());

                return orgDocId;
            }
        }

        //--------------------------------------------------------------------------
        protected int JobId
        {
            get
            {
                int jobId = -1;
                
                if (Request.QueryString["JobId"] != null)
                    jobId = int.Parse(Request.QueryString["JobId"].ToString());
                else
                {
                    Facade.IOrder facOrder = new Facade.Order();
                    jobId = facOrder.GetDeliveryJobIDForOrderID(this.OrderId);
                }

                return jobId;
            }
        }

        //--------------------------------------------------------------------------

        protected int CollectDropId
        {
            get
            {
                int collectDropId = -1;
                
                if (Request.QueryString["CollectDropId"] != null)
                    collectDropId = int.Parse(Request.QueryString["CollectDropId"].ToString());
                else
                {
                    Facade.IOrder facOrder = new Facade.Order();
                    collectDropId = facOrder.GetDeliveryCollectDropIDForPODScanner(this.OrderId);
                }

                return collectDropId;
            }
        }

        //---------------------------------------------------------------------------------------

        protected int PointId
        {
            get
            {
                int pointId = 0;
                if (Request.QueryString["PointId"] != null)
                    pointId = int.Parse(Request.QueryString["PointId"].ToString());

                return pointId;
            }
        }

        //---------------------------------------------------------------------------------------

        protected int ManifestId
        {
            get
            {
                int manifestId = -1;
                if (Request.QueryString["ManifestId"] != null)
                    manifestId = int.Parse(Request.QueryString["ManifestId"].ToString());

                return manifestId;
            }
        }

        //--------------------------------------------------------------------------

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

        //--------------------------------------------------------------------------

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

        //--------------------------------------------------------------------------

        protected string DialogModeParam
        {
            get
            {
                string dlgModeQuerystring = string.Empty;
                if (Request.QueryString["PODDlgMode"] != null)
                    dlgModeQuerystring = Request.QueryString["PODDlgMode"];

                return dlgModeQuerystring;
            }
        }

        //--------------------------------------------------------------------------
        protected string PODReturnComment
        {
            get
            {
                return Request.QueryString["PODReturnComment"] ?? string.Empty;
            }
        }

        //--------------------------------------------------------------------------

        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.Initialise();
                txtPODReturnComment.Text = PODReturnComment;
            }
        }

        //--------------------------------------------------------------------------

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnNext.Click += new System.EventHandler(btnNext_Click);
            this.rblOperation.SelectedIndexChanged += new EventHandler(rblOperation_SelectedIndexChanged);

            if (DialogModeParam.ToUpper() == "MANUAL")
            {   // Put dialog in "manual entry" mode if specified. Default behaviour (if not specified) is PODDialogMode.Scan
                dialogMode = PODDialogMode.Manual;
            }
        }

        //--------------------------------------------------------------------------

        private void rblOperation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.rblOperation.SelectedValue == "2")
            {
                this.rblAppandOReplace.Items[0].Selected = true;
                this.rblAppandOReplace.Items[1].Selected = false;
                this.rblAppandOReplace.Items[1].Enabled = false;
            }
            else
                this.rblAppandOReplace.Items[1].Enabled = true;
        }

        //--------------------------------------------------------------------------
        protected void btnRecordPODReturn_Click(object sender, EventArgs e)
        {
            bool PODReceived = false;

            if (this.rblPODReceived.SelectedValue == "0")
            {
                PODReceived = true;
            }

            Facade.IPOD facPOD = new Facade.POD();
            int retVal = facPOD.RecordPODReturn(JobId, CollectDropId, 0, PODReceived, txtPODReturnComment.Text, ((Entities.CustomPrincipal)Page.User).UserName);

            this.Close("-1");
        }

        private bool IsNewDynamicWebTwainLicensed
        {
            get
            {
                var filepath = Server.MapPath(NEW_WEBTWAIN_LIC_PATH);

                return File.Exists(filepath);
            }
        }

        private void btnNext_Click(object sender, System.EventArgs e)
        {
            string nextPage = String.Empty;
            string appendOrReplace = String.Empty;
            bool PODReceived = false;

            if (this.rblOperation.SelectedValue == "0")
            {
                appendOrReplace = "S";
                nextPage = IsNewDynamicWebTwainLicensed ? "scanDocumentNew.aspx" : "scanDocument.aspx";
            }
            else if (this.rblOperation.SelectedValue == "1")
            {
                appendOrReplace = "U";
                nextPage = "uploadDocument.aspx";
            }
            else if (this.rblOperation.SelectedValue == "2")
            {
                // Skip the scanning page
                Orchestrator.Facade.ReferenceData facForm = new Orchestrator.Facade.ReferenceData();
                Entities.FormType formType = facForm.GetForFormTypeId(this.ScannedFormTypeId);

                nextPage = formType.FormTypePage;
                appendOrReplace = "N";
            }

            if (this.rblAppandOReplace.SelectedValue == "0")
                appendOrReplace += "R";
            else if (this.rblAppandOReplace.SelectedValue == "1")
                appendOrReplace += "A";

            nextPage += "?ScannedFormId=" + this.ScannedFormId.ToString();
            nextPage += "&ScannedFormTypeId=" + this.ScannedFormTypeId.ToString();
            nextPage += "&JobId=" + this.JobId.ToString(); ;
            nextPage += "&OrderId=" + this.OrderId.ToString(); ;
            nextPage += "&OrgId=" + this.OrganisationIdentityId.ToString();
            nextPage += "&OrgDocId=" + this.OrganisationDocumentId.ToString();
            nextPage += "&CollectDropId=" + this.CollectDropId.ToString();
            nextPage += "&PointId=" + this.PointId.ToString();
            nextPage += "&ManifestId=" + this.ManifestId.ToString();
            nextPage += "&DeHireReceiptId=" + this.DehireReceiptId.ToString();
            nextPage += "&DHNo=" + this.DehireReceiptNumber.ToString();
            nextPage += "&AppendOrReplace=" + appendOrReplace;
            nextPage += "&PODReceived=" + PODReceived;

            if (this.Request.QueryString["dcb"] != null)
                nextPage += "&dcb=" + this.Request.QueryString["dcb"].ToString();

            nextPage += "&WizardBackUrl=" + HttpUtility.UrlEncode(Request.Url.PathAndQuery);

            Response.Redirect(nextPage);
        }

        //--------------------------------------------------------------------------

        private void Initialise()
        {
        
            ((WizardMasterPage)this.Master).WizardTitle = "Document Wizard";

            if (this.ScannedFormId > 0)
            {
                Orchestrator.Facade.Form facForm = new Orchestrator.Facade.Form();
                Entities.Scan scannedform = facForm.GetForScannedFormId(this.ScannedFormId);

                tblPODReturn.Visible = false;

                this.AppendOrReplaceFieldSet.Style["display"] = "";
                this.appendReplaceText.Style["display"] = "";

                if (scannedform.ScannedFormPDF == Globals.Constants.NO_DOCUMENT_AVAILABLE)
                {
                    this.AppendOrReplaceFieldSet.Style["display"] = "none";
                    this.appendReplaceText.Style["display"] = "none";
                    this.rblAppandOReplace.Style["display"] = "none";
                }
                else
                    this.rblAppandOReplace.Style["display"] = "";

                this.appendReplaceText.InnerHtml = "A "
                    + scannedform.FormTypeId.ToString() + " document already exists.";
                    
                if(scannedform.IsUploaded)
                {
                    this.appendReplaceText.InnerHtml += " [ <a target='_blank' href='" + scannedform.ScannedFormPDF + "' > View </a> ]";
                }
                else
                    this.appendReplaceText.InnerHtml += " [ Not yet uploaded ]";
            }
            else
            {
                this.AppendOrReplaceFieldSet.Visible = false;
                this.appendReplaceText.Visible = false;
                this.rblAppandOReplace.Visible = false;
            }

            Facade.IUser facUser = new Facade.User();
            Entities.User loggedOnUser = facUser.GetUserByUserName(this.Page.User.Identity.Name);
            if (!loggedOnUser.HasScannerLicense)
            {
                // Disable the scanning option
                this.rblOperation.Items[0].Selected = false;
                this.rblOperation.Items[0].Text = "You do not currently have a license to scan.";
                this.rblOperation.Items[0].Enabled = false;

                // select the upload option.
                this.rblOperation.Items[1].Selected = true;
            }
        }

        

        

        //--------------------------------------------------------------------------
    }
}
