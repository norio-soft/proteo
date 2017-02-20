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

using Orchestrator;
using System.Collections.Generic;

namespace Orchestrator.WebUI.Job
{
    public partial class AddExtra : Orchestrator.Base.BasePage
    {
        #region Fields

        private int m_jobId = 0;
        private int m_orderId = 0;
        private int m_extraId = 0;
        private bool m_demurrage = false;
        private int m_instructionId = 0;


        protected int InstructionID
        {
            get
            {
                if (m_instructionId < 0)
                    m_instructionId = int.Parse(Request.QueryString["instructionId"]);
                return m_instructionId;
            }
        }
        #endregion

        #region Page Loading/Setup/Init

        protected void Page_Load(object sender, EventArgs e)
        {
            // Recover parameters from the query string.
            int.TryParse(Request.QueryString["jobId"], out m_jobId);
            int.TryParse(Request.QueryString["extraId"], out m_extraId);
            int.TryParse(Request.QueryString["orderId"], out m_orderId);

            m_demurrage = int.TryParse(Request.QueryString["instructionId"], out m_instructionId);

            if (!IsPostBack)
            {
                PopulateStaticControls();
                if (m_extraId > 0)
                    LoadExtra();
            }
        }

        private void PopulateStaticControls()
        {

            // Load the Customers for this instruction
            Facade.IOrder facOrder = new Facade.Order();
            DataSet dsclients =  facOrder.GetOrdersForInstructionID(this.InstructionID);
            cboClient.DataSource = dsclients;
            cboClient.DataBind();

            List<String> Clients = new List<string>();
            
                foreach (DataRow row in dsclients.Tables[0].Rows)
                {
                    if (!Clients.Contains(row["CustomerOrganisationName"].ToString()))
                        Clients.Add(row["CustomerOrganisationName"].ToString());
                }

                if (Clients.Count == 1)
                {
                    cboClient.Enabled = false;
                    cboClient.Items[0].Selected = true;
                }
            


            Facade.ExtraType facExtraType = new Orchestrator.Facade.ExtraType();
            bool? getActiveExtraTypes = true;
            cboExtraType.DataSource = facExtraType.GetForIsEnabled(getActiveExtraTypes);
            cboExtraType.DataValueField = "ExtraTypeId";
            cboExtraType.DataTextField = "Description";
            cboExtraType.DataBind();

            cboExtraState.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eExtraState)));
            cboExtraState.DataBind();

            if (m_demurrage)
            {
                cboExtraType.Items.FindByText(eExtraType.Demurrage.ToString()).Selected = true;
                cboExtraType.Enabled = false;
                pnlDemurrageComments.Visible = true;
            }
            else
                cboExtraType.Items.RemoveAt(((int)eExtraType.Demurrage));
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnClose.Click += new EventHandler(btnClose_Click);
            this.btnDeleteExtra.Click += new EventHandler(btnDeleteExtra_Click);
            this.btnAddExtra.Click += new EventHandler(btnAddExtra_Click);
            this.cboExtraType.SelectedIndexChanged += new EventHandler(cboExtraType_SelectedIndexChanged);
        }

        void cboExtraType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((eExtraType)Enum.Parse(typeof(eExtraType), cboExtraType.SelectedItem.Text.Replace(" ", "")) == eExtraType.Custom)
                pnlCustomExtra.Visible = true;
            else
                pnlCustomExtra.Visible = false;
        }

        #endregion

        #region Control Event Handlers

        void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void btnDeleteExtra_Click(object sender, EventArgs e)
        {
            Facade.IJobExtra facExtra = new Facade.Job();
            facExtra.DeleteExtra(m_extraId);

            this.ReturnValue = bool.TrueString;
            this.Close("DeleteExtra");
        }

        void btnAddExtra_Click(object sender, EventArgs e)
        {
            if (this.IsValid)
            {
                Facade.IJobExtra facExtra = new Facade.Job();
                Entities.Extra extra = new Orchestrator.Entities.Extra();
                if ((eExtraState)Enum.Parse(typeof(eExtraState), cboExtraState.SelectedValue.Replace(" ", "")) == eExtraState.Accepted && txtClientContact.Text == "")
                {
                    pnlError.Visible = true;
                    lblError.Text = "If the Extra has been accepted please state whom by.";
                    return;
                }

                if (m_extraId > 0)
                {
                    UpdateExtra();
                }
                else
                {
                    CreateExtra();
                }
            }

        }

        #endregion

        #region Data Loading/Manipulation

        private void LoadExtra()
        {
            Facade.IJobExtra facExtra = new Facade.Job();
            Entities.Extra extra = facExtra.GetExtraForExtraId(m_extraId);

            cboExtraState.ClearSelection();
            cboExtraState.Items.FindByValue(Utilities.UnCamelCase(extra.ExtraState.ToString())).Selected = true;
            txtClientContact.Text = extra.ClientContact;
            txtAmount.Text = extra.ExtraAmount.ToString("C");

            //if (extra.ExtraType == eExtraType.Demurrage)
            //{
            //    pnlDemurrageComments.Visible = true;
            //    txtDemurrageComments.Text = extra.DemurrageComments;

            //    if (cboExtraState.Items.FindByValue(eExtraType.Demurrage.ToString()) == null)
            //    {
            //        cboExtraType.Items.Insert(0, new ListItem(eExtraType.Demurrage.ToString()));
            //        cboExtraType.Enabled = false;
            //        pnlDemurrageComments.Visible = true;
            //    }
            //}

            //if (extra.ExtraType == eExtraType.Custom)
            //{
                pnlCustomExtra.Visible = true;
                txtCustomDescription.Text = extra.CustomDescription;
            //}

            cboExtraType.ClearSelection();

            cboExtraType.Items.FindByValue(((int)extra.ExtraType).ToString()).Selected = true;

            btnAddExtra.Enabled = extra.ExtraState != eExtraState.Invoiced;
            btnDeleteExtra.Enabled = extra.ExtraState != eExtraState.Invoiced;

            Session["_extra"] = extra;

            btnDeleteExtra.Enabled = true;
        }

        private void CreateExtra()
        {
            Entities.Extra extra = new Orchestrator.Entities.Extra();
            
            
            if (m_orderId > 0)
                extra.OrderID = m_orderId;

            extra.ExtraType = (eExtraType)Enum.Parse(typeof(eExtraType), cboExtraType.SelectedValue.Replace(" ", ""));
            extra.ExtraState = (eExtraState)Enum.Parse(typeof(eExtraState), cboExtraState.SelectedValue.Replace(" ", ""));
            extra.ClientContact = txtClientContact.Text;
            extra.ExtraAmount = decimal.Parse(txtAmount.Text, System.Globalization.NumberStyles.Currency);
            extra.OrderID = int.Parse(cboClient.SelectedValue);

            //if (extra.ExtraType == eExtraType.Demurrage)
            //{
            //    extra.DemurrageComments = txtDemurrageComments.Text;
            //    extra.InstructionId = m_instructionId;
            //}

            //if (extra.ExtraType == eExtraType.Custom)
                extra.CustomDescription = txtCustomDescription.Text;

            string userId = ((Entities.CustomPrincipal)Page.User).UserName;
            Facade.IJobExtra facExtra = new Facade.Job();
            int extraId = facExtra.AddExtra(extra, userId);
            if (extraId > 0)
            {
                this.ReturnValue = bool.TrueString;
                this.Close("AddExtra");
            }
            else
            {
                lblError.Text = "There was a problem trying to Add this extra.";
                pnlError.Visible = true;
            }
        }

        private void UpdateExtra()
        {
            Entities.Extra extra = (Entities.Extra)Session["_extra"];
            extra.ExtraType = (eExtraType)Enum.Parse(typeof(eExtraType), cboExtraType.SelectedValue.Replace(" ", ""));
            extra.ExtraState = (eExtraState)Enum.Parse(typeof(eExtraState), cboExtraState.SelectedValue.Replace(" ", ""));
            extra.ClientContact = txtClientContact.Text;
            extra.ExtraAmount = decimal.Parse(txtAmount.Text, System.Globalization.NumberStyles.Currency);
            
            //This page is only ever used for adding Job based extras which are
            //NOT multi-currency. So set ForeignAmount to the UK Amount
            //(Its not necessary to do this for the create because the BL does
            //it for us - but it has to be done for the Update as the logic is also
            //used by the Extras Invoice.).
            extra.ForeignAmount = extra.ExtraAmount;

            //if (extra.ExtraType == eExtraType.Demurrage)
            //    extra.DemurrageComments = txtDemurrageComments.Text;

            //if (extra.ExtraType == eExtraType.Custom)
                extra.CustomDescription = txtCustomDescription.Text;

            bool success = false;
            if (extra.OrderID > 0)
            {
                Facade.IOrderExtra facExtra = new Facade.Order();
                success = facExtra.Update(extra, ((Entities.CustomPrincipal)Page.User).UserName);
            }
            else
            {
                Facade.IJobExtra facExtra = new Facade.Job();
                success = facExtra.UpdateExtra(extra, ((Entities.CustomPrincipal)Page.User).UserName);
            }

            if (success)
            {
                this.ReturnValue = bool.TrueString;
                this.Close("AddExtra");
            }
            else
            {
                lblError.Text = "There was a problem trying to update this extra.";
                pnlError.Visible = true;
            }
        }

        #endregion
    }
}