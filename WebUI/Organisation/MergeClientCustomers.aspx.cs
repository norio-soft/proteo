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
using System.ComponentModel;
using System.Drawing;
using System.Web.SessionState;
using Telerik.Web.UI;
using Orchestrator.Globals;
using System.Text;

namespace Orchestrator.WebUI.Organisation
{
    public partial class MergeClientCustomers : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditOrganisations, eSystemPortion.AddEditPoints, eSystemPortion.GeneralUsage);
            btnMergeClientCustomers.Enabled = Orchestrator.WebUI.Security.Authorise.CanAccess(eSystemPortion.AddEditOrganisations, eSystemPortion.AddEditPoints);

            if (!string.IsNullOrEmpty(Request.QueryString["rcbID"]))
                return;
            btnMergeClientCustomers.Attributes.Add("onclick", string.Format("javascript:giveWarning();"));
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.cboOrganisation.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(cboOrganisation_ItemsRequested);
            this.repClientCustomers.NeedDataSource += new GridNeedDataSourceEventHandler(repClientCustomers_NeedDataSource);
            this.repClientCustomers.ItemDataBound += new GridItemEventHandler(repClientCustomers_ItemDataBound);
            this.btnMergeClientCustomers.Click += new EventHandler(btnMergeClientCustomers_Click);
            this.btnFilter.Click += new EventHandler(btnFilter_Click);
        }

        void repClientCustomers_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            int identityID;
            int.TryParse(cboOrganisation.SelectedValue, out identityID);

            if (identityID != 0)
            {
                Facade.IReferenceData facReferenceData = new Facade.ReferenceData();
                repClientCustomers.DataSource = facReferenceData.GetAllOrganisationsFiltered(txtClientCustomerFiler.Text, eOrganisationType.ClientCustomer);
            }
        }

        void repClientCustomers_ItemDataBound(object o, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.AlternatingItem || e.Item.ItemType == GridItemType.Item)
            {
                string identityId = ((DataRowView)e.Item.DataItem)["IdentityId"].ToString();
                CheckBox chkMergeTo = (CheckBox)e.Item.FindControl("chkRow");
                chkMergeTo.Attributes.Add("IdentityId", identityId);
            }
        }

        void cboOrganisation_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            RadComboBox cboOrganisation = (RadComboBox)o;
            cboOrganisation.Items.Clear();

            Facade.IReferenceData facReferenceData = new Facade.ReferenceData();
            DataSet dsOrganisationNames = facReferenceData.GetAllOrganisationsFiltered(e.Text, eOrganisationType.ClientCustomer);

            if (dsOrganisationNames.Tables[0].Rows.Count > 0)
            {
                int itemsToRequest = 20;
                if (dsOrganisationNames.Tables[0].Rows.Count < itemsToRequest)
                    itemsToRequest = dsOrganisationNames.Tables[0].Rows.Count;

                Telerik.Web.UI.RadComboBoxItem rcItem = null;
                for (int rowIndex = 0; rowIndex < itemsToRequest; rowIndex++)
                {
                    DataRow dr = dsOrganisationNames.Tables[0].Rows[rowIndex];
                    rcItem = new Telerik.Web.UI.RadComboBoxItem();
                    rcItem.Text = dr["OrganisationName"].ToString();
                    rcItem.Value = dr["IdentityID"].ToString();
                    cboOrganisation.Items.Add(rcItem);
                }
            }
        }

        void btnFilter_Click(object sender, EventArgs e)
        {
            this.lblMessage.Text = "";
            repClientCustomers.Rebind();
        }

        protected void btnMergeClientCustomers_Click(object sender, EventArgs e)
        {
            Facade.Organisation facOrg = new Orchestrator.Facade.Organisation();
            List<Entities.Organisation> mergedClientCustomers = new List<Entities.Organisation>();
            string userId = ((Entities.CustomPrincipal)Page.User).UserName;
            CheckBox chkBox = null;
            string mergeToClientCustomer = "";
            StringBuilder mergeFromClientCustomers = new StringBuilder();

            mergeToClientCustomer = this.cboOrganisation.SelectedValue;

            foreach (Telerik.Web.UI.GridItem gi in repClientCustomers.Items)
            {
                chkBox = (CheckBox)gi.FindControl("chkRow");
                if (chkBox.Checked)
                {
                    if (mergeFromClientCustomers.Length == 0)
                    {
                        mergeFromClientCustomers = mergeFromClientCustomers.Append(chkBox.Attributes["IdentityId"]);
                    }
                    else
                        mergeFromClientCustomers = mergeFromClientCustomers.Append("," + chkBox.Attributes["IdentityId"]);
                }
            }

            if (mergeFromClientCustomers.Length > 0 && mergeToClientCustomer.Length > 0)
            {
                facOrg.MergeClientCustomers(Convert.ToInt32(mergeToClientCustomer), mergeFromClientCustomers.ToString(), userId);
                repClientCustomers.Rebind();
            }

            this.lblMessage.Text = "Merge has succeeded.";
            this.repClientCustomers.Rebind();
        }
    }
}
