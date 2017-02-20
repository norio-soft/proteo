using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Orchestrator.WebUI.scan.wizard
{
    public partial class ManifestDetails : Orchestrator.Base.BasePage
    {
        //--------------------------------------------------------------------------

        public Entities.Scan Manifest
        {
            get
            {
                Entities.Scan manifest = null;

                if (ViewState["Manifest"] != null)
                    manifest = (Entities.Scan)ViewState["Manifest"];

                return manifest;
            }
            set { ViewState["Manifest"] = value; }
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

        protected string FormattedManifestId
        {
            get
            {
                string resourceManifestId = this.ManifestId.ToString();

                if (resourceManifestId.Length == 1)
                    resourceManifestId = string.Concat("00", resourceManifestId);
                else if (resourceManifestId.Length == 2)
                    resourceManifestId = string.Concat("0", resourceManifestId);

                return resourceManifestId;
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
                this.Manifest = facForm.GetForScannedFormId(this.ScannedFormId);
            }

            int scanFormId = -1;
            // With job POD scanning
            if (this.ManifestId > 0)
            {
                Facade.Form facForm = new Facade.Form();

                if (this.Manifest == null)
                    this.Manifest = new Entities.Scan();

                this.Manifest.ScannedDateTime = DateTime.Today;
                this.Manifest.FormTypeId = eFormTypeId.Manifest;

                if (String.IsNullOrEmpty(this.FileName))
                    this.Manifest.ScannedFormPDF = Orchestrator.Globals.Constants.NO_DOCUMENT_AVAILABLE;
                else
                    this.Manifest.ScannedFormPDF = this.FileName;

                if (this.AppendOrReplace.Contains("A"))
                    this.Manifest.IsAppend = true;
                else
                    this.Manifest.IsAppend = false;

                // if this is not set then we don't know where to save the file locally so it must have been uploaded immediately.
                if (this.AppendOrReplace.Contains("U") || String.IsNullOrEmpty(Globals.Configuration.ScannedDocumentPath))
                    this.Manifest.IsUploaded = true;
                else
                    this.Manifest.IsUploaded = false;

                scanFormId = facForm.Create(this.Manifest, ((Entities.CustomPrincipal)Page.User).UserName);

                if (scanFormId > 0)
                {
                    this.Manifest.ScannedFormId = scanFormId;

                    // Update manifest record here
                    Orchestrator.Facade.ResourceManifest facResourceManifest =
                        new Orchestrator.Facade.ResourceManifest();

                    facResourceManifest.UpdateResourceManifestScan(this.ManifestId, scanFormId, ((Entities.CustomPrincipal)Page.User).UserName);
                }
            }

            this.Close(this.ScannedFormId.ToString());
        }

        //--------------------------------------------------------------------------
    }
}
