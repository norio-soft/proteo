using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;

namespace Orchestrator.WebUI.scan.wizard
{
    public partial class OrganisationDocumentDetails: Orchestrator.Base.BasePage
    {
        //--------------------------------------------------------------------------

        public Entities.Scan ScannedForm
        {
            get
            {
                Entities.Scan scannedForm = null;

                if (ViewState["ScannedForm"] != null)
                    scannedForm = (Entities.Scan)ViewState["ScannedForm"];

                return scannedForm;
            }
            set { ViewState["ScannedForm"] = value; }
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

        protected int ScannedFormTypeId
        {
            get
            {

                var formTypeId = int.Parse(Request.QueryString["ScannedFormTypeId"].ToString());

                return formTypeId;
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
            Facade.Form facForm = new Facade.Form();

            if (this.ScannedFormId > 0)
            {
                this.ScannedForm = facForm.GetForScannedFormId(this.ScannedFormId);
            }
            else
            {
                this.ScannedForm = new Entities.Scan();
            }

            this.ScannedForm.ScannedDateTime = DateTime.Today;
            this.ScannedForm.FormTypeId = (eFormTypeId)this.ScannedFormTypeId ;
            this.ScannedForm.ScannedFormPDF = this.FileName;

            if (this.AppendOrReplace.Contains("A"))
                this.ScannedForm.IsAppend = true;
            else
                this.ScannedForm.IsAppend = false;

                // if this is not set then we don't know where to save the file locally so it must have been uploaded immediately.
                if (this.AppendOrReplace.Contains("U") || String.IsNullOrEmpty(Globals.Configuration.ScannedDocumentPath))
                    this.ScannedForm.IsUploaded = true;
                else
                    this.ScannedForm.IsUploaded = false;

            //Creates a new scanned form and references the previous scanned form if necessary
            int scanFormId = facForm.Create(this.ScannedForm, ((Entities.CustomPrincipal)Page.User).UserName);

            if (scanFormId > 0)
            {
                EF.OrganisationDocument orgDoc = null;

                if (this.OrganisationDocumentId > 0)
                {
                    orgDoc = DataContext.OrganisationDocuments.First(od => od.OrganisationDocumentId == this.OrganisationDocumentId);
                }
                else
                {
                    orgDoc = new EF.OrganisationDocument();
                    orgDoc.Description = DataContext.FormTypes.First(ft => ft.FormTypeId == this.ScannedFormTypeId).Description;
                    orgDoc.OrganisationReference.EntityKey = EF.DataContext.CreateKey("OrganisationSet", "IdentityId", this.OrganisationIdentityId);
                    this.DataContext.AddToOrganisationDocuments(orgDoc);
                }

                orgDoc.ScannedFormReference.EntityKey = EF.DataContext.CreateKey("ScannedForms", "ScannedFormID", scanFormId);
                this.DataContext.SaveChanges();
            }
            

            this.Close(this.ScannedFormId.ToString());
        }

        //--------------------------------------------------------------------------
    }
}
