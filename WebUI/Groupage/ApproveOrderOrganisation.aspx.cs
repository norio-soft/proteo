using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Transactions;

namespace Orchestrator.WebUI.Groupage
{
    public partial class ApproveOrderOrganisation : Orchestrator.Base.BasePage
    {
        public Entities.Organisation Organisation { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                int organisationID = Convert.ToInt32(Request.QueryString["OrganisationID"].ToString());
                
                // Set the page title.
                this.Master.WizardTitle = string.Format("Approve Organisation");

                lblUnnaprovedOrganisation.Text = string.Format("Please approve the Organisation");
                lblUnnaprovedOrganisation.Font.Bold = true;

                Facade.Organisation facOrganisation = new Orchestrator.Facade.Organisation();
                this.Organisation = facOrganisation.GetForIdentityId(organisationID);
                string organisationName = string.IsNullOrWhiteSpace(this.Organisation.OrganisationName) ? "Unnamed" : this.Organisation.OrganisationName;

                lblNameAndStatus.Text = "<b>Name:</b> " + organisationName;
                lblNameAndStatus.Text = lblNameAndStatus.Text + "<br /><br /><b>Status:</b> " + this.Organisation.IdentityStatus.ToString();

                string jsInjection = @"
                try { resizeTo(630, 730); }
                catch (err) { }
                window.focus();
                moveTo(30, 20);";

                this.ClientScript.RegisterClientScriptBlock(this.GetType(), "ApproveOnly", jsInjection, true);
            }
        }

        protected void btnApproveOrganisation_Click(object sender, EventArgs e)
        {
            Facade.IOrganisation facOrganisation = new Facade.Organisation();
            int organisationID = Convert.ToInt32(Request.QueryString["OrganisationID"].ToString());
            this.Organisation = facOrganisation.GetForIdentityId(organisationID);
            this.Organisation.IdentityStatus = eIdentityStatus.Active;

            Entities.FacadeResult result = null;
            result = facOrganisation.Update(this.Organisation, this.Page.User.Identity.Name);

            if (result.Success)
            {
                this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ApprovePoint", "window.opener.__doPostBack('','Refresh');window.close();", true);
            }
            else
            {
                idErrors.Infringements = result.Infringements;
                idErrors.DisplayInfringments();
                idErrors.Visible = true;
            }
        }

        protected void btnCloseForm_Click(object sender, EventArgs e)
        {
            this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "CancelApprovePoint", "window.close();", true);
        }
    }
}
