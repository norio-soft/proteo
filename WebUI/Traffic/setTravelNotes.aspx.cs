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

namespace Orchestrator.WebUI.Traffic
{
    public partial class setTravelNotes : Orchestrator.Base.BasePage
    {
        private int m_driverResourceId = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            m_driverResourceId = Convert.ToInt32(Request.QueryString["resourceId"]);

            if (!IsPostBack)
            {
                Facade.IDriver facDriver = new Facade.Resource();
                lblDriverName.Text = facDriver.GetDriverForResourceId(m_driverResourceId).Individual.FullName;
                txtTravelNotes.Text = facDriver.GetTravelNotes(m_driverResourceId);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnCancel.Click += new EventHandler(btnCancel_Click);
            btnConfirm.Click += new EventHandler(btnConfirm_Click);
        }

        void btnConfirm_Click(object sender, EventArgs e)
        {
            Facade.IDriver facDriver = new Facade.Resource();
            bool success = facDriver.UpdateTravelNotes(m_driverResourceId, txtTravelNotes.Text, ((Entities.CustomPrincipal) Page.User).UserName);

            if (success)
                this.ReturnValue = string.Format("{{ \\\\\"resourceID\\\\\": {0}, \\\\\"notes\\\\\": \\\\\"{1}\\\\\" }}",
                    m_driverResourceId.ToString(),
                    txtTravelNotes.Text.Replace("\"","").Replace("'",""));

            this.Close();
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}