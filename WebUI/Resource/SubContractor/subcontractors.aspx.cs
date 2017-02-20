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
    public partial class Resource_SubContractor_subcontractors : Orchestrator.Base.BasePage
    {
        #region Page Fields

        private const string vs_rowFilter = "vs_rowFilter";
        protected string rowFilter
        {
            get
            {
                if (this.ViewState[vs_rowFilter] == null)
                    if (Request.QueryString["filter"] != null)
                        this.ViewState[vs_rowFilter] = Request.QueryString["filter"].ToString();
                    else
                        this.ViewState[vs_rowFilter] = "a";

                return (string)this.ViewState[vs_rowFilter];
            }
            set { this.ViewState[vs_rowFilter] = value; }
        }

        #endregion

        #region Page Load / Init

        protected void Page_Load(object sender, EventArgs e)
        {
          
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.grdOrganisations.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdOrganisations_NeedDataSource);
            this.grdOrganisations.ItemCommand += new GridCommandEventHandler(grdOrganisations_ItemCommand);

            this.btnExport.Click +=new EventHandler(btnExport_Click);
            /*this.btnExportTop.Click += new EventHandler(btnExport_Click);*/
            this.chkShowDeleted.CheckedChanged += new EventHandler(chkShowDeleted_CheckedChanged);
        }

        void chkShowDeleted_CheckedChanged(object sender, EventArgs e)
        {
            grdOrganisations.Rebind();
        }

        void btnExport_Click(object sender, EventArgs e)
        {
            this.grdOrganisations.ExportSettings.OpenInNewWindow = true;
            this.grdOrganisations.MasterTableView.ExportToExcel();
        }

        #endregion

        #region Grid Events

        void grdOrganisations_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            DataSet ds = null;
            using (Facade.Organisation facOrganisation = new Facade.Organisation())
                ds = facOrganisation.GetAllForTypeFiltered((int)eOrganisationType.SubContractor, rowFilter, chkShowDeleted.Checked);

            grdOrganisations.DataSource = ds.Tables[0];
        }

        void grdOrganisations_ItemCommand(object source, GridCommandEventArgs e)
        {
            if (e.CommandName.ToLower() == "alpha")
            {
                string letter = e.CommandArgument.ToString()[0].ToString();
                rowFilter = e.CommandArgument.ToString().ToLower() == "all" ? "" : letter;
                grdOrganisations.Rebind();
            }

            if (e.CommandName.ToLower() == "search")
            {
                Button btn = (Button)e.CommandSource;
                TextBox txt = btn.NamingContainer.FindControl("txtSearch") as TextBox;

                if (source is RadGrid)
                    rowFilter = txt.Text;

                grdOrganisations.Rebind();
            }

            if (e.CommandName.ToLower() == "btnexport")
            {
                btnExport_Click(this, e);
            }
        }
        #endregion
    }
}