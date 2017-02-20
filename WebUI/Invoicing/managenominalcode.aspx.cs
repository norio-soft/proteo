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

using Telerik.Web.UI;

namespace Orchestrator.WebUI
{
    public partial class ManageNominalCode : Orchestrator.Base.BasePage
    {
        #region Page Properties
        private Entities.NominalCode Code
        {
            get { return (Orchestrator.Entities.NominalCode)this.ViewState["_nominalCode"]; }
            set { this.ViewState["_nominalCode"] = value; }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                PopulateStaticControls();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnOK.Click += new EventHandler(btnOK_Click);
            this.btnCancel.Click += new EventHandler(btnCancel_Click);
            this.btnUnassign.Click += new EventHandler(btnUnassign_Click);
        }

        void btnUnassign_Click(object sender, EventArgs e)
        {
            Orchestrator.Facade.INominalCode facNominalCode = new Orchestrator.Facade.NominalCode();
            bool facResult = false;
            facResult = facNominalCode.Unassign(this.Code.NominalCodeID);

            if (facResult)
                InjectScript.Text = @"<script>RefreshParentPage()</script>";
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            this.InjectScript.Text = @"<script>CloseOnReload()</script>";
        }

        void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                Orchestrator.Facade.INominalCode facNominalCode = new Orchestrator.Facade.NominalCode();
                Orchestrator.Entities.FacadeResult facResult = null;

                int jobTypeID = 0;
                if (cboJobType.SelectedValue != "0")
                    jobTypeID = (int)Enum.Parse(typeof(eJobType), cboJobType.SelectedValue);


                if (this.Code != null) // Update
                    facResult = facNominalCode.Update(this.Code.NominalCodeID,jobTypeID, txtNominalCode.Text, txtDescription.Text, this.Page.User.Identity.Name, chkIsActive.Checked);
                else
                   facResult = facNominalCode.Create(jobTypeID, txtNominalCode.Text, txtDescription.Text, this.Page.User.Identity.Name, chkIsActive.Checked);

                if (!facResult.Success)
                {
                    string error = string.Empty;
                    foreach (Orchestrator.Entities.BusinessRuleInfringement bri in facResult.Infringements)
                        error += "&bull; " + bri.Description + "<br/>";

                    lblError.Text = error;
                    lblError.Visible = true;
                    return;
                }

                if (facResult.Success)
                    InjectScript.Text = @"<script>RefreshParentPage()</script>";
            }
            catch
            {
                lblError.Text = "There was an error adding this Nominal Code.";
                lblError.Visible = true;
            }
        }

        private void PopulateStaticControls()
        {
            cboJobType.Items.Add(new Telerik.Web.UI.RadComboBoxItem(Orchestrator.eJobType.PalletReturn.ToString(), ((int)Orchestrator.eJobType.PalletReturn).ToString()));
            cboJobType.Items.Add(new Telerik.Web.UI.RadComboBoxItem(Orchestrator.eJobType.Return.ToString(), ((int)Orchestrator.eJobType.Return).ToString()));

            cboJobType.Items.Insert(0, new Telerik.Web.UI.RadComboBoxItem("Not a Default", "0"));
            cboJobType.Items[0].Selected = true;

            if (!string.IsNullOrEmpty(Request.QueryString["nc"]))
                LoadNominalCode(int.Parse(Request.QueryString["nc"]));
        }

        private void LoadNominalCode(int nominalCodeID)
        {
            Orchestrator.Facade.INominalCode facNominalCode = new Orchestrator.Facade.NominalCode();
            Orchestrator.Entities.NominalCode nc = facNominalCode.GetForNominalCodeID(nominalCodeID);
            this.Code = nc;

            string jobType = "0";
            if (nc.JobType > 0)
                jobType = nc.JobType.ToString();
            cboJobType.ClearSelection();
            cboJobType.FindItemByValue(jobType).Selected = true;
            txtNominalCode.Text = nc.ShortCode;
            txtDescription.Text = nc.Description;
            chkIsActive.Checked = nc.IsActive;

            lblBusinessTypesMapped.Text = nc.BusinessTypeMaps.ToString();
            lblExtraTypesMapped.Text = nc.ExtraTypeMaps.ToString();
            lblOrganisationsMapped.Text = nc.OrganisationMaps.ToString();
            lblVehiclesMapped.Text = nc.VehicleMaps.ToString();

            trAssignments.Visible = trAssignmentNote.Visible = btnUnassign.Visible = nc.BusinessTypeMaps + nc.ExtraTypeMaps + nc.OrganisationMaps + nc.VehicleMaps > 0;
            chkIsActive.Enabled = !trAssignmentNote.Visible;
            trBusinessTypes.Visible = nc.BusinessTypeMaps > 0;
            trExtraTypes.Visible = nc.ExtraTypeMaps > 0;
            trOrganisations.Visible = nc.OrganisationMaps > 0;
            trVehicles.Visible = nc.VehicleMaps > 0;
        }
    }
}