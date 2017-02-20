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

namespace Orchestrator.WebUI.Job
{
    public partial class bulkchangerate : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            cboOrganisation.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboOrganisation_ItemsRequested);
            cfvIncreaseBy.ServerValidate += new ServerValidateEventHandler(cfvIncreaseBy_ServerValidate);
            btnSubmit.Click += new EventHandler(btnSubmit_Click);
        }

        void btnSubmit_Click(object sender, EventArgs e)
        {
            lblConfirmation.Visible = false;
            if (Page.IsValid)
            {
                decimal percentage = decimal.Parse(txtIncreaseBy.Text);
                int clientId = 0;
                int.TryParse(cboOrganisation.SelectedValue, out clientId);

                if (clientId > 0 || cboOrganisation.Text == string.Empty)
                {
                    Facade.IJobRate facJobRate = new Facade.Job();
                    bool success = facJobRate.Update(clientId, percentage, ((Entities.CustomPrincipal)Page.User).UserName);

                    if (success)
                    {
                        if (clientId == 0)
                            lblConfirmation.Text = "All active rates have been increased by " + percentage + "%.";
                        else
                            lblConfirmation.Text = "All active rates for " + cboOrganisation.Text + " have been increased by " + percentage + "%.";
                    }
                    else
                        lblConfirmation.Text = "You rate increase was not applied.";
                    lblConfirmation.Visible = true;
                }
                else
                    lblConfirmation.Text = "You rate increase was not applied.";
            }
        }

        void cfvIncreaseBy_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;
            decimal percentage = 0;
            if (decimal.TryParse(txtIncreaseBy.Text, out percentage))
            {
                args.IsValid = true;
            }
        }

        #region Combo's Server Methods and Initialisation

        void cboOrganisation_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboOrganisation.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllOrganisationsFiltered(e.Text, eOrganisationType.Client, false);

            int itemsPerRequest = 20;
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
                cboOrganisation.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

        #endregion
    }
}
