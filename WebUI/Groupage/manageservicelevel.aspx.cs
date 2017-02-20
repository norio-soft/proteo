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

namespace Orchestrator.WebUI
{
    public partial class ManageServiceLevel : Orchestrator.Base.BasePage
    {
        private int _serviceLevelID = -1;

        protected void Page_Load(object sender, EventArgs e)
        {
            int.TryParse(Request.QueryString["slID"], out _serviceLevelID);

            if (!IsPostBack)
            {
                LoadOrderServiceLevel();
            }
        }

        protected override void OnInit(EventArgs e)
        {
                base.OnInit(e);
                this.btnOK.Click += new EventHandler(btnOK_Click);
        }

        void btnOK_Click(object sender, EventArgs e)
        {
            Orchestrator.Facade.IOrderServiceLevel facOrder = new Orchestrator.Facade.Order();
            int retVal = -1;
            retVal = facOrder.Update(_serviceLevelID, txtDescription.Text, txtShortDescription.Text, (int?)rntNumberOfDays.Value, this.Page.User.Identity.Name);
            if (retVal > 0)
                InjectScript.Text = "<script>RefreshParentPage();</script>";
            else
            {
                lblError.Visible = true;
                if (_serviceLevelID > 0)
                    lblError.Text = "There was a problem updating the Order Service Level, please try again.";
                else
                {
                    lblError.Text = "There was a problem adding the new Order Service Level, please try again.";
                }    
            }

        }


        private void LoadOrderServiceLevel()
        {
            Label PageTitle = ((System.Web.UI.UserControl)(Page.Master)).FindControl("lblWizardTitle") as Label;

            if (PageTitle != null)
                PageTitle.Text = "Add Service Level";

            if (_serviceLevelID > 0)
            {
                Orchestrator.Facade.IOrderServiceLevel facOrderService = new Orchestrator.Facade.Order();
                DataSet ds = facOrderService.GetForOrderServiceLevel(_serviceLevelID);
                txtDescription.Text = ds.Tables[0].Rows[0]["Description"].ToString();
                txtShortDescription.Text = ds.Tables[0].Rows[0]["ShortDescription"].ToString();
                rntNumberOfDays.Value = ds.Tables[0].Rows[0]["NoOfDays"] != DBNull.Value ? (int)ds.Tables[0].Rows[0]["NoOfDays"] : (int?)null;
                string lastUpdated = "Last Updated : <b>{0}</b> by <b>{1}</b>";
                lblLastUpdate.Text = string.Format(lastUpdated, ((DateTime)ds.Tables[0].Rows[0]["LastUpdateDate"]).ToString("dd/MM/yy"), ds.Tables[0].Rows[0]["LastUpdateUserID"].ToString());
            }
        }
    }
}