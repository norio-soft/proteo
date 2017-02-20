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

public partial class UserControls_comboStreamers_cboPointOwner : Orchestrator.Base.BasePage
{

    #region Private fields/constants
    private const string VS_CLIENTUSER_ORGANISATION_IDENTITYID = "__vsClientUserOrganisationIdentityID";
    #endregion

    #region Public Properties
    /// <summary>
    /// This is a helper property that is used to ensure correct naming.
    /// </summary>
    public int ClientUserOrganisationIdentityID
    {
        get { return this.Session[VS_CLIENTUSER_ORGANISATION_IDENTITYID] != null ? (int)this.Session[VS_CLIENTUSER_ORGANISATION_IDENTITYID] : -1; }
        set { this.Session[VS_CLIENTUSER_ORGANISATION_IDENTITYID] = value; }
    }
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected override void OnInit(EventArgs e)
    {
       base.OnInit(e);
       this.cboPointOwner.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboPointOwner_ItemsRequested);
    }

    void cboPointOwner_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
    {
        //cboDeliveryPointOwner.Items.Clear();


        // if this is being called by a client they can only access clients of their own for now.
        DataSet ds = null;
        if (this.ClientUserOrganisationIdentityID > 0)
        {
            Orchestrator.Facade.IOrganisation facOrganisation = new Orchestrator.Facade.Organisation();
            ds = facOrganisation.GetClientsForIdentityIdFiltered(this.ClientUserOrganisationIdentityID, e.Text);
                        
        }
        else
        {
            Orchestrator.Facade.IReferenceData facReferenceData = new Orchestrator.Facade.ReferenceData();
            ds = facReferenceData.GetAllOrganisationsFiltered("%" + e.Text, false);
        }

        DataTable dt = ds.Tables[0];

        int itemsPerRequest = 20;
        int itemOffset = e.NumberOfItems;
        int endOffset = itemOffset + itemsPerRequest;
        if (endOffset > dt.Rows.Count)
            endOffset = dt.Rows.Count;


        
        Telerik.Web.UI.RadComboBoxItem rcItem = null;
        for (int i = itemOffset; i < endOffset; i++)
        {
            rcItem = new Telerik.Web.UI.RadComboBoxItem();
            rcItem.Text = dt.Rows[i]["OrganisationName"].ToString();
            rcItem.Value = dt.Rows[i]["IdentityId"].ToString();
            cboPointOwner.Items.Add(rcItem);
        }

        if (dt.Rows.Count > 0)
        {
            e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
        }
    }
}
