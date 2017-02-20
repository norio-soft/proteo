using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI.HtmlControls;

using Orchestrator;

namespace Orchestrator.WebUI.scan.wizard
{
    public partial class scanDocument : Orchestrator.Base.BasePage
    {
        //--------------------------------------------------------------------------

        private string cookieScanner = "Orchestrator.ScannerId";

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

        protected int ScannerId
        {
            get
            {
                int scannerId = -1;
                if (Request.Cookies[cookieScanner] != null)
                {
                    // If the cookie contains a valid scanner id then use it.
                    int cookieScannerId;
                    var success = int.TryParse(Request.Cookies[cookieScanner].Value, out cookieScannerId);
                    
                    if (success)
                        scannerId = cookieScannerId;
                }
                return scannerId;
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

        //--------------------------------------------------------------------------

        protected string FileName
        {
            get
            {
                string uploadedFilePath = String.Empty;
                if (ViewState["FileName"] != null)
                    uploadedFilePath = ViewState["FileName"].ToString();

                return uploadedFilePath;
            }
            set
            {
                ViewState["FileName"] = value;
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

        protected int JobId
        {
            get
            {
                int jobId = -1;
                if (Request.QueryString["JobId"] != null)
                    jobId = int.Parse(Request.QueryString["JobId"].ToString());

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

                return collectDropId;
            }
        }

        //--------------------------------------------------------------------------

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

        protected int ScannerTransferMode
        {
            get
            {
                return Globals.Configuration.ScannerTransferMode;
            }
        }

        //--------------------------------------------------------------------------

        protected string AppendOrReplace
        {
            get
            {
                string appendOrReplace = "SR";
                if (Request.QueryString["AppendOrReplace"] != null)
                    appendOrReplace = Request.QueryString["AppendOrReplace"].ToString();

                return appendOrReplace;
            }
        }

        //--------------------------------------------------------------------------

        public string ServerName
        {
            get
            {
                string servername = Page.Request.Url.Authority;
                int indexOfColon = servername.IndexOf(':');

                if(indexOfColon > -1)
                    servername = servername.Substring(0, indexOfColon);
                
                return servername;
            }
        }

        //--------------------------------------------------------------------------

        public string UserName
        {
            get
            {
                return ((Orchestrator.Entities.CustomPrincipal)Page.User).UserName;
            }
        }

        //--------------------------------------------------------------------------

        public string ServerPort
        {
            get
            {
                string serverport = Page.Request.Url.Authority;
                int indexOfColon = serverport.IndexOf(':');

                if (indexOfColon > -1)
                {
                    serverport = serverport.Substring((indexOfColon + 1),
                    (serverport.Length - indexOfColon) - 1);
                }
                else
                    serverport = Globals.Configuration.HTTPUploadPort.ToString();

                return serverport;
            }

        }

        //--------------------------------------------------------------------------

        public string ScannerUploadPage
        {
            get
            {
                string uploadpage = string.Empty;
                uploadpage = "scanreceive.aspx";
                return uploadpage;
            }
        }

        //--------------------------------------------------------------------------

        public string ImageSaveLocation
        {
            get
            {
                string location = Globals.Configuration.ScannedDocumentPath;

                // javascript needs the additional backslahes
                if (!String.IsNullOrEmpty(location))
                    if (!location.EndsWith("\\"))
                        location += "\\";

                return location.Replace(@"\", @"\\");
            }
        }

        //--------------------------------------------------------------------------

        public string ScannedDocumentNetworkPath
        {
            get
            {
                string location = Globals.Configuration.ScannedDocumentNetworkPath;

                // javascript needs the additional backslahes
                if (!String.IsNullOrEmpty(location))
                {
                    if (!location.EndsWith("\\"))
                        location += "\\";

                    if (!location.StartsWith("\\\\"))
                        location = "\\\\" + location;
                }

                return location.Replace(@"\", @"\\");
            }
        }

        //--------------------------------------------------------------------------

        protected string RandomKey
        {
            get
            {
                DateTime date = DateTime.Now;
                return String.Format("{0}{1}{2}{3}{4}{5}", date.Year, date.Month, 
                    date.Day, date.Minute, date.Second, date.Millisecond);
            }
        }

        //--------------------------------------------------------------------------

        protected string DatePathForImmediateUpload
        {
            get
            {
                DateTime date = DateTime.Now;
                return String.Format("{0}\\{1}\\{2}", date.Year, date.Month, date.Day);
            }
        }

        //--------------------------------------------------------------------------

        protected void Page_Load(object sender, EventArgs e)
        {
            ((WizardMasterPage)this.Master).WizardTitle = "Document Wizard";

            this.Page.ClientScript.GetPostBackEventReference(this, "");

            string eventArg = Request.Params.Get("__EVENTARGUMENT");

            if (!String.IsNullOrEmpty(eventArg))
            {
                this.FileName = eventArg;
                this.btnNext_Click(null, null);
            }
            else
                if (String.IsNullOrEmpty(Orchestrator.Globals.Configuration.ScannedDocumentPath))
                    this.chkUploadNow.Visible = false;
                else
                    if (Orchestrator.Globals.Configuration.ScanFormSendNow)
                        this.chkUploadNow.Checked = true;
        }

        //--------------------------------------------------------------------------

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnBack.Click += btnBack_Click;
        }

        //--------------------------------------------------------------------------

        void btnBack_Click(object sender, EventArgs e)
        {
            // This is a temporary workaround for an issue caused by javascript history api not working in windows opened using showModalDialog.
            // The longer-term solution is to implement using a method other than showModalDialog which is deprecated in html5 anyway, and then
            // the Back button can simply call history.back() in javascript.
            if (Request.QueryString["WizardBackUrl"] != null)
                Response.Redirect(HttpUtility.UrlDecode(Request.QueryString["WizardBackUrl"]));
        }

        //--------------------------------------------------------------------------

        private void btnNext_Click(object sender, System.EventArgs e)
        {
            if (!String.IsNullOrEmpty(this.FileName))
            {
                if(this.ScannerId == -1)
                    if (!String.IsNullOrEmpty(this.hidSelectedScannerID.Value))
                    {
                        HttpCookie scannerCookie = new HttpCookie(cookieScanner, 
                            this.hidSelectedScannerID.Value);

                        scannerCookie.Expires = DateTime.Today.AddYears(50);
                        Response.Cookies.Add(scannerCookie);
                    }

                Orchestrator.Facade.ReferenceData facForm = new Orchestrator.Facade.ReferenceData();
                Entities.FormType formType = facForm.GetForFormTypeId(this.ScannedFormTypeId);

                string nextPage = formType.FormTypePage;

                nextPage += "?ScannedFormId=" + this.ScannedFormId.ToString();
                nextPage += "&ScannedFormTypeId=" + this.ScannedFormTypeId.ToString();
                nextPage += "&JobId=" + this.JobId;
                nextPage += "&OrderId=" + this.OrderId;
                nextPage += "&OrgId=" + this.OrganisationIdentityId;
                nextPage += "&OrgDocId=" + this.OrganisationDocumentId;
                nextPage += "&CollectDropId=" + this.CollectDropId;
                nextPage += "&PointId=" + this.PointId;
                nextPage += "&ManifestId=" + this.ManifestId;
                nextPage += "&DeHireReceiptId=" + this.DehireReceiptId;
                nextPage += "&DHNo=" + this.DehireReceiptNumber;
                nextPage += "&AppendOrReplace=" + this.AppendOrReplace;
                nextPage += "&FileName=" + this.FileName;

                if (this.Request.QueryString["dcb"] != null)
                    nextPage += "&dcb=" + this.Request.QueryString["dcb"].ToString();

                nextPage += "&WizardBackUrl=" + HttpUtility.UrlEncode(Request.Url.PathAndQuery);

                Response.Redirect(nextPage);
            }
        }

        //--------------------------------------------------------------------------
    }
}
