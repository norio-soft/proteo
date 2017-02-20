using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Collections.Generic;

namespace Orchestrator.WebUI.Job
{
    public partial class ChangeJobClient : Orchestrator.Base.BasePage
    {
        private int m_jobId = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsCallback) return;

            int.TryParse(Request.QueryString["jobId"], out m_jobId);

            if (!IsPostBack)
            {
                if (m_jobId > 0)
                {
                    LoadJob();
                }
                else
                    btnCancel_Click(this, new EventArgs());
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            cboClient.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
            cboClient.SelectedIndexChanged += new Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventHandler(cboClient_SelectedIndexChanged);

            repReferences.ItemDataBound += new RepeaterItemEventHandler(repReferences_ItemDataBound);

            btnConfirm.Click += new EventHandler(btnConfirm_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
        }

        private void GetReferences(int identityId)
        {
            Facade.IOrganisationReference facOrganisationReference = new Facade.Organisation();
            repReferences.DataSource = facOrganisationReference.GetReferencesForOrganisationIdentityId(identityId, true);
            repReferences.DataBind();
        }

        private void LoadJob()
        {
            Facade.IJob facJob = new Facade.Job();
            Entities.Job job = facJob.GetJob(m_jobId);
            job.References = ((Facade.IJobReference)facJob).GetJobReferences(job.JobId);

            cboClient.Text = job.Client;
            cboClient.SelectedValue = job.IdentityId.ToString();
            cboClient.SelectedValue = job.IdentityId.ToString();

            repExistingReferences.DataSource = job.References;
            repExistingReferences.DataBind();

            GetReferences(job.IdentityId);
        }

        private void UpdateReferences()
        {
            int identityId = int.Parse(cboClient.SelectedValue);
            List<Entities.JobReference> references = new List<Entities.JobReference>();

            Facade.IOrganisationReference facOrganisationReference = new Facade.Organisation();

            foreach (RepeaterItem item in repReferences.Items)
            {
                int referenceId = Convert.ToInt32(((HtmlInputHidden)item.FindControl("hidOrganisationReferenceId")).Value);
                string referenceValue = ((TextBox)item.FindControl("txtReferenceValue")).Text;

                // Create a new reference and add it to the reference collection
                Entities.JobReference jobReference = new Entities.JobReference();
                jobReference.JobId = m_jobId;
                jobReference.Value = referenceValue;
                jobReference.OrganisationReference = facOrganisationReference.GetReferenceForOrganisationReferenceId(referenceId);
                references.Add(jobReference);
            }

            Facade.IJob facJob = new Facade.Job();
            facJob.ChangeClient(m_jobId, identityId, references, ((Entities.CustomPrincipal)Page.User).UserName);

            mwhelper.CloseForm = true;
            mwhelper.CausePostBack = true;
            mwhelper.OutputData = "<changeClient />";
        }

        #region Event Handlers

        #region Button Events

        void btnConfirm_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                UpdateReferences();
            }
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            mwhelper.CloseForm = true;
            mwhelper.CausePostBack = false;
        }

        #endregion

        #region Combobox Events

        void cboClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboClient.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllClientsFiltered(e.Text);

            int itemsPerRequest = 15;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            DataTable dt = ds.Tables[0];
            Telerik.Web.UI.RadComboBoxItem rcItem = null;
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new Telerik.Web.UI.RadComboBoxItem();
                rcItem.Text = dt.Rows[i]["OrganisationName"].ToString();
                rcItem.Value = dt.Rows[i]["IdentityId"].ToString();
                cboClient.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

        void cboClient_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            int identityId = 0;
            if (int.TryParse(((Telerik.Web.UI.RadComboBox)o).SelectedValue, out identityId))
                GetReferences(identityId);
        }

        #endregion

        #region Repeater Events

        private void repReferences_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Entities.OrganisationReferenceCollection organisationReferences = (Entities.OrganisationReferenceCollection)repReferences.DataSource;

                HtmlInputHidden hidOrganisationReferenceId = (HtmlInputHidden)e.Item.FindControl("hidOrganisationReferenceId");
                PlaceHolder plcHolder = (PlaceHolder)e.Item.FindControl("plcHolder");

                int organisationReferenceId = Convert.ToInt32(hidOrganisationReferenceId.Value);

                // Configure the validator controls
                CustomValidator validatorControl = null;

                Entities.OrganisationReference reference = organisationReferences.FindByReferenceId(organisationReferenceId);
                switch (reference.DataType)
                {
                    case eOrganisationReferenceDataType.Decimal:
                        validatorControl = new CustomValidator();
                        validatorControl.ServerValidate += new ServerValidateEventHandler(validatorControl_ServerValidateDecimal);
                        break;
                    case eOrganisationReferenceDataType.FreeText:
                        // No additional validation required.
                        break;
                    case eOrganisationReferenceDataType.Integer:
                        validatorControl = new CustomValidator();
                        validatorControl.ServerValidate += new ServerValidateEventHandler(validatorControl_ServerValidateInteger);
                        break;
                }

                if (validatorControl != null)
                {
                    // Configure the validator properties
                    validatorControl.ControlToValidate = "txtReferenceValue";
                    validatorControl.Display = ValidatorDisplay.Dynamic;
                    validatorControl.ErrorMessage = "Please supply a " + reference.Description + ".";
                    validatorControl.Text = "<img src=\"../images/error.png\"  Title=\"Please supply a " + reference.Description + ".\" />";
                    validatorControl.EnableClientScript = false;

                    plcHolder.Controls.Add(validatorControl);
                }
            }
        }

        #endregion

        #region Validation Events

        private void validatorControl_ServerValidateDecimal(object source, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(args.Value, 0, false);
        }

        private void validatorControl_ServerValidateInteger(object source, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(args.Value, 0, true);
        }

        #endregion

        #endregion
    }
}