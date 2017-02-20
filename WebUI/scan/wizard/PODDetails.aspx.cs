using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Orchestrator;
using System.Data;
using System.IO;

namespace Orchestrator.WebUI.scan.wizard
{
    public partial class PODDetails : Orchestrator.Base.BasePage
    {

        //--------------------------------------------------------------------------

        public Entities.POD Pod
        {
            get
            {
                Entities.POD pod = null;

                if (ViewState["Pod"] != null)
                    pod = (Entities.POD)ViewState["Pod"];

                return pod;
            }
            set { ViewState["Pod"] = value; }
        }

        //--------------------------------------------------------------------------

        private const string c_AdditionalReferences_VS = "c_AdditionalReferences_VS";

        //--------------------------------------------------------------------------

        protected List<string> AdditionalReferences
        {
            get
            {
                if (ViewState[c_AdditionalReferences_VS] == null)
                {
                    if (this.Pod == null)
                    {
                        ViewState[c_AdditionalReferences_VS] = new List<string>();
                        return (List<string>)ViewState[c_AdditionalReferences_VS];
                    }
                    else if (ViewState[c_AdditionalReferences_VS] == null)
                    {
                        ViewState[c_AdditionalReferences_VS] = this.Pod.AdditionalReferences;
                        return (List<string>)ViewState[c_AdditionalReferences_VS];
                    }
                    else
                        return (List<string>)ViewState[c_AdditionalReferences_VS];
                }
                else
                    return (List<string>)ViewState[c_AdditionalReferences_VS];
            }
            set
            {
                ViewState[c_AdditionalReferences_VS] = value;
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
                int jobId = 0;
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
                int collectDropId = 0;
                if (Request.QueryString["CollectDropId"] != null)
                    collectDropId = int.Parse(Request.QueryString["CollectDropId"].ToString());

                return collectDropId;
            }
        }

        //--------------------------------------------------------------------------

        protected int OrderId
        {
            get
            {
                int orderId = 0;
                if (Request.QueryString["OrderId"] != null)
                    orderId = int.Parse(Request.QueryString["OrderId"].ToString());

                return orderId;
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

        protected bool PODReceived
        {
            get
            {
                bool podReceived = false;
                if (Request.QueryString["PODReceived"] != null)
                    podReceived = bool.Parse(Request.QueryString["PODReceived"].ToString());

                return podReceived;
            }
        }


        //--------------------------------------------------------------------------

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
                this.Initialise();
        }

        //--------------------------------------------------------------------------

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.dgAdditionalReferences.EditCommand += dgAdditionalReferences_EditCommand;
            this.dgAdditionalReferences.UpdateCommand += dgAdditionalReferences_UpdateCommand;
            this.dgAdditionalReferences.DeleteCommand += dgAdditionalReferences_DeleteCommand;
            this.dgAdditionalReferences.CancelCommand += dgAdditionalReferences_CancelCommand;
            this.dgAdditionalReferences.ItemDataBound += dgAdditionalReferences_ItemDataBound;

            this.btnAddReference.Click += btnAddReference_Click;
            this.btnBack.Click += btnBack_Click;
            this.btnNext.Click += btnNext_Click;

        }

        //--------------------------------------------------------------------------

        private void btnBack_Click(object sender, EventArgs e)
        {
            // This is a temporary workaround for an issue caused by javascript history api not working in windows opened using showModalDialog.
            // The longer-term solution is to implement using a method other than showModalDialog which is deprecated in html5 anyway, and then
            // the Back button can simply call history.back() in javascript.
            if (Request.QueryString["WizardBackUrl"] != null)
                Response.Redirect(HttpUtility.UrlDecode(Request.QueryString["WizardBackUrl"]));
        }

        //--------------------------------------------------------------------------

        private void btnNext_Click(object sender, EventArgs e)
        {
            // With job POD scanning
            if (this.JobId > 0)
            {
                bool sendPodToclient = false;

                Facade.POD facPod = new Orchestrator.Facade.POD();
                if (this.Pod == null)
                    this.Pod = new Entities.POD();

                this.Pod.SignatureDate = this.dteSignatureDate.SelectedDate.Value;
                this.Pod.ScannedDateTime = DateTime.Today;
                this.Pod.TicketNo = this.txtTicketNo.Text.Trim();
                this.Pod.JobId = this.JobId;
                this.Pod.CollectDropId = this.CollectDropId;
                this.Pod.OrganisationId = 0;

                // N = No document available
                if (this.AppendOrReplace.Contains("N"))
                {
                    if (this.Pod.ScannedFormId == 0)
                        this.Pod.ScannedFormPDF = Orchestrator.Globals.Constants.NO_DOCUMENT_AVAILABLE;
                }
                else
                    this.Pod.ScannedFormPDF = this.FileName;

                // A = Append to existing
                if (this.AppendOrReplace.Contains("A"))
                    this.Pod.IsAppend = true;
                else
                    this.Pod.IsAppend = false;


                this.Pod.IsUploaded = false;

                this.Pod.AdditionalReferences = this.AdditionalReferences;

                if (this.ScannedFormId > 0)
                {
                    facPod.Update(this.Pod, ((Entities.CustomPrincipal)this.User).UserName);
                    // re get the pod entry as the scannedFormId will have changed.
                    this.Pod = facPod.GetForPODId(this.Pod.PODId);
                }
                else
                {
                    int podId = facPod.Create(this.Pod, this.JobId, 0, this.CollectDropId, ((Entities.CustomPrincipal)Page.User).UserName);

                    // get the newly created pod.
                    this.Pod = facPod.GetForPODId(podId);
                }



            }



            this.Close(this.ScannedFormId.ToString());
        }

        //--------------------------------------------------------------------------

        private void Initialise()
        {
            if (this.ScannedFormId > 0)
            {
                Facade.IPOD facPOD = new Facade.POD();
                this.Pod = facPOD.GetForScannedFormId(this.ScannedFormId);

                if (Orchestrator.Globals.Configuration.AutoPopulateScanTicketNumber)
                    this.txtTicketNo.Text = this.Pod.TicketNo;
                this.dteSignatureDate.SelectedDate = this.Pod.SignatureDate;

                this.dgAdditionalReferences.DataSource = this.Pod.AdditionalReferences;
                this.dgAdditionalReferences.DataBind();
            }
            else
            {
                Facade.IPOD facPod = new Orchestrator.Facade.POD();
                DataSet dsJobDetails = facPod.GetJobDetails(this.JobId);

                foreach (DataRow row in dsJobDetails.Tables["CollectionDrop"].Rows)
                {
                    if ((int)row["CollectDropId"] == this.CollectDropId)
                    {
                        if (Orchestrator.Globals.Configuration.AutoPopulateScanTicketNumber) 
                            this.txtTicketNo.Text = row["ClientsCustomerReference"].ToString();
                        this.dteSignatureDate.SelectedDate = (DateTime)row["CollectDropDateTime"];
                        break;
                    }
                }
            }

            ((WizardMasterPage)this.Master).WizardTitle = "Document Wizard";
        }

        //--------------------------------------------------------------------------

        private void btnAddReference_Click(object sender, EventArgs e)
        {
            AdditionalReferences.Add("new reference");
            dgAdditionalReferences.EditItemIndex = AdditionalReferences.Count - 1;
            dgAdditionalReferences.DataSource = AdditionalReferences;
            dgAdditionalReferences.DataBind();
        }

        //--------------------------------------------------------------------------

        private void dgAdditionalReferences_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.EditItem)
            {
                TextBox txtReference = e.Item.FindControl("txtReference") as TextBox;
                txtReference.Text = AdditionalReferences[dgAdditionalReferences.EditItemIndex];
            }
        }

        //--------------------------------------------------------------------------

        private void dgAdditionalReferences_CancelCommand(object source, DataGridCommandEventArgs e)
        {
            dgAdditionalReferences.EditItemIndex = -1;
            dgAdditionalReferences.DataSource = AdditionalReferences;
            dgAdditionalReferences.DataBind();
        }

        //--------------------------------------------------------------------------

        private void dgAdditionalReferences_DeleteCommand(object source, DataGridCommandEventArgs e)
        {
            AdditionalReferences.RemoveAt(e.Item.ItemIndex);
            dgAdditionalReferences.EditItemIndex = -1;
            dgAdditionalReferences.DataSource = AdditionalReferences;
            dgAdditionalReferences.DataBind();
        }

        //--------------------------------------------------------------------------

        private void dgAdditionalReferences_UpdateCommand(object source, DataGridCommandEventArgs e)
        {
            TextBox txtReference = e.Item.FindControl("txtReference") as TextBox;
            AdditionalReferences[e.Item.ItemIndex] = txtReference.Text;
            dgAdditionalReferences.EditItemIndex = -1;
            dgAdditionalReferences.DataSource = AdditionalReferences;
            dgAdditionalReferences.DataBind();
        }

        //--------------------------------------------------------------------------

        private void dgAdditionalReferences_EditCommand(object source, DataGridCommandEventArgs e)
        {
            dgAdditionalReferences.EditItemIndex = e.Item.ItemIndex;
            dgAdditionalReferences.DataSource = AdditionalReferences;
            dgAdditionalReferences.DataBind();
        }

        //--------------------------------------------------------------------------
    }
}
