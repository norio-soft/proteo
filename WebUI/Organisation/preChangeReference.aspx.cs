using System;

namespace Orchestrator.WebUI.Organisation
{
    public partial class preChangeReference : Orchestrator.Base.BasePage
    {
        private const string C_OrderList_VS = "C_OrderList_VS";
        private const string C_JobList_VS = "C_JobList_VS";
        private const string C_ClientID_VS = "C_ClientID_VS";

        public int IdentityID
        {
            get { return ViewState[C_ClientID_VS] == null ? -1 : int.Parse(ViewState[C_ClientID_VS].ToString()); }
            set { ViewState[C_ClientID_VS] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && IdentityID < 0) IdentityID = int.Parse(Request.QueryString["clientID"]);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnRedirect.Click += new EventHandler(delegate
                                                           {
                                                               Session[C_OrderList_VS] = hidOrders.Value;
                                                               Session[C_JobList_VS] = hidJobs.Value;
                                                               Response.Redirect(string.Format("ChangeReference.aspx?clientID={0}", IdentityID));
                                                           });
        }
    }
}