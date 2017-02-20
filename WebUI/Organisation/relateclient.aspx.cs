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
    public partial class RelateClient : Orchestrator.Base.BasePage
    {

        #region Page Properties
        protected int IdentityID
        {
            get
            {
                int retval = -1;
                try
                { retval = int.Parse(Request.QueryString["ID"]); }
                catch { }
                return retval;
            }
        }

        protected string OrganisationName
        {
            get
            {
                string retval = string.Empty;
                try
                { retval = Request.QueryString["ON"]; }
                catch { }
                return retval;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            lblRelated.Text = string.Format(lblRelated.Text, this.OrganisationName);
        }


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnRelate.Click += new EventHandler(btnRelate_Click);
            this.btnCancel.Click += new EventHandler(btnCancel_Click);
            this.cboClient.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
        }

        void btnRelate_Click(object sender, EventArgs e)
        {
            bool isClientCustomerCalling = false;
            bool.TryParse(Request.QueryString["CC"], out isClientCustomerCalling);

            int idenityID = int.Parse(cboClient.SelectedValue);
            int relatedIdentityID = this.IdentityID;

            // if this is being called from the Client Customer  Screen (addupdateclientscustomer.aspx) 
            // reverse the entries
            if (isClientCustomerCalling)
            {
                int tempID = idenityID;
                idenityID = relatedIdentityID;
                relatedIdentityID = tempID;
            }

            Facade.IIdentityRelationship facIdentity = new Facade.IdentityRelationship();
            int retVal = facIdentity.CreateRelationship(idenityID, relatedIdentityID, eRelationshipType.IsClient, Page.User.Identity.Name);

            if (retVal > 0)
                this.Close("refresh");
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void cboClient_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            cboClient.Items.Clear();

            Facade.IReferenceData facReferenceData = new Facade.ReferenceData();
            DataSet ds = facReferenceData.GetAllClientsFiltered(e.Text);
            DataTable dt = ds.Tables[0];
            DataTable boundResults = dt.Clone();

            int itemsPerRequest = 20;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > dt.Rows.Count)
                endOffset = dt.Rows.Count;

            for (int i = itemOffset; i < endOffset; i++)
                boundResults.ImportRow(dt.Rows[i]);

            cboClient.DataSource = boundResults;
            cboClient.DataBind();

            if (dt.Rows.Count > 0)
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
        }

    }
}