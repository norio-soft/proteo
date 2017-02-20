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

namespace Orchestrator.WebUI.administration.users
{

    public partial class Users : Orchestrator.Base.BasePage
    {

        protected bool m_isClient = false;

        private int? _activeFullTimeLicenseCount = null;
        private int? _activePartTimeLicenseCount = null;

        protected int ActiveFullTimeLicenseCount
        {
            get
            {
                if (!_activeFullTimeLicenseCount.HasValue)
                    this.GetActiveLicenceCounts();

                return _activeFullTimeLicenseCount.Value;
            }
        }

        protected int ActivePartTimeLicenseCount
        {
            get
            {
                if (!_activePartTimeLicenseCount.HasValue)
                    this.GetActiveLicenceCounts();

                return _activePartTimeLicenseCount.Value;
            }
        }

        private void GetActiveLicenceCounts()
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<Repositories.IUserRepository>(uow);
                _activeFullTimeLicenseCount = repo.GetActiveFullTimeLicenseCount();
                _activePartTimeLicenseCount = repo.GetActivePartTimeLicenseCount();
            }
        }

        protected int TotalFullTimeLicenseCount
        {
            get { return Orchestrator.Globals.Configuration.FullTimeLicenseCount; }
        }

        protected int TotalPartTimeLicenseCount
        {
            get { return Orchestrator.Globals.Configuration.PartTimeLicenseCount; }
        }

        protected DataTable GridData
        {
            get { return (DataTable)this.ViewState["_gridData"]; }
            set { this.ViewState["_gridData"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditUser);

            if (Request.QueryString["IsClient"] != null)
                m_isClient = true;

        }

        protected void cmdAddUser_Click(object sender, System.EventArgs e)
        {
            if (m_isClient)
                Response.Redirect("addupdateuser.aspx?isclient=true");
            else
                Response.Redirect("addupdateuser.aspx");
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.grdUsers.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdUsers_NeedDataSource);

            this.dlgUser.DialogCallBack += new EventHandler(dlgUser_DialogCallBack);
        }

        void dlgUser_DialogCallBack(object sender, EventArgs e)
        {
            if (this.dlgUser.ReturnValue == "CloseAndRefresh")
            {
                GetData();
                grdUsers.DataBind();
            }
        }

        private void GetData()
        {
            Facade.IUser facUser = new Facade.User();
            DataSet dsUsers = facUser.GetAllUsers();

            DataView dv = dsUsers.Tables[0].DefaultView;

            if (m_isClient)
            {
                dv.RowFilter = "OrganisationName <> ''";
                grdUsers.Columns[0].Visible = true;
            }
            else
            {
                grdUsers.Columns[0].Visible = false;
                dv.RowFilter = "OrganisationName IS NULL ";
                grdUsers.MasterTableView.GroupByExpressions.Clear();
            }

            grdUsers.DataSource = dv;
        }

        void grdUsers_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            GetData();
        }


    }
}