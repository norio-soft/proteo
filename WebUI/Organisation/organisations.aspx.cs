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
    public partial class Organisation_organisations : Orchestrator.Base.BasePage
    {
        #region Page Load / Init

        private const string vs_rowFilter = "vs_rowFilter";
        
        protected string RowFilter
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["incomplete"]))
                this.lblPageDescription.Text = "Below is a list of all your clients which have <b>incomplete</b> information stored in the system, you can filter this list by either clicking on the letters to the left, or entering some of the organisation's name in the filter box and clicking filter.";
            else
                this.lblPageDescription.Text = "Below is a list of all your clients stored in the system, you can filter this list by either clicking on the letters to the left, or entering some of the organisation's name in the filter box and clicking filter.";

            if (!IsPostBack)
            {
                grdOrganisations.Rebind();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            grdOrganisations.NeedDataSource += new GridNeedDataSourceEventHandler(grdOrganisations_NeedDataSource);
            grdOrganisations.ItemCommand += new GridCommandEventHandler(grdOrganisations_ItemCommand);

            /*btnExport.Click += new EventHandler(btnExport_Click);*/
            btnExportTop.Click += new EventHandler(btnExport_Click);

            optShowActive.CheckedChanged += new EventHandler(FilterOption_CheckedChanged);
            optShowSuspended.CheckedChanged += new EventHandler(FilterOption_CheckedChanged);
            optShowMissingCreditApplicationForm.CheckedChanged += new EventHandler(FilterOption_CheckedChanged);
            optShowMissingClientTnCs.CheckedChanged += new EventHandler(FilterOption_CheckedChanged);
            optShowMissingSubbyTnCs.CheckedChanged += new EventHandler(FilterOption_CheckedChanged);
        }

        void FilterOption_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton)sender).Checked)
                grdOrganisations.Rebind();
        }

        void grdOrganisations_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            DataSet ds = null;
            using (Facade.Organisation facOrganisation = new Facade.Organisation())
            {
                if (Request.QueryString["incomplete"] != null)
                    ds = facOrganisation.GetIncomplete();
                else
                {
                    if (optShowActive.Checked)
                        ds = facOrganisation.GetAllForTypeFiltered((int)eOrganisationType.Client, RowFilter, false);
                    else if (optShowSuspended.Checked)
                        ds = facOrganisation.GetAllForTypeFiltered((int)eOrganisationType.Client, RowFilter, true);

                    //The missing document all include suspended clients. This is because new clients remain suspended until
                    //documents have been scanned.
                    else if (optShowMissingCreditApplicationForm.Checked)
                        ds = facOrganisation.GetAllForTypeFiltered((int)eOrganisationType.Client, RowFilter, true, false, eFormTypeId.CreditApplicationForm);
                    else if (optShowMissingClientTnCs.Checked)
                        ds = facOrganisation.GetAllForTypeFiltered((int)eOrganisationType.Client, RowFilter, true, false, eFormTypeId.ClientTnCs);
                    else if (optShowMissingSubbyTnCs.Checked)
                        ds = facOrganisation.GetAllForTypeFiltered((int)eOrganisationType.Client, RowFilter, true, true, eFormTypeId.SubbyTnCs);
                }
            }

            grdOrganisations.DataSource = ds.Tables[0];
        }

        void btnExport_Click(object sender, EventArgs e)
        {
            this.grdOrganisations.ExportSettings.OpenInNewWindow = true;
            this.grdOrganisations.MasterTableView.ExportToExcel();//("Client List", false, true);
        }

        #endregion

        #region Grid Events

        //private void RebindGrid()
        //{
        //    DataSet ds = null;
        //    using (Facade.Organisation facOrganisation = new Facade.Organisation())
        //    {
        //        if (Request.QueryString["incomplete"] != null)
        //            ds = facOrganisation.GetIncomplete();
        //        else
        //            ds = facOrganisation.GetAllForTypeFiltered((int)eOrganisationType.Client, rowFilter, chkShowDeleted.Checked);
        //    }

        //    grdOrganisations.DataSource = ds.Tables[0];
        //    grdOrganisations.DataBind();
        //}

        void gci_Load(object sender, EventArgs e)
        {
            GridCommandItem gci = sender as GridCommandItem;

            PlaceHolder ph = gci.FindControl("phAlphaPicker") as PlaceHolder;

            for (int i = 65; i <= 65 + 25; i++)
            {
                LinkButton lb;
                LiteralControl lc;
                lb = new LinkButton();
                lc = new LiteralControl();
                lc.Text = " ";
                lb.CommandName = "alpha";
                lb.Text = "" + (char)i;
                lb.CommandArgument = "" + (char)i;
                ph.Controls.Add(lb);
                ph.Controls.Add(lc);
            }
        }

        public void grdOrganisations_ItemCommand(object source, GridCommandEventArgs e)
        {
            if (e.CommandName.ToLower() == "alpha")
            {
                string letter = e.CommandArgument.ToString()[0].ToString();
                if (e.CommandArgument.ToString().ToLower() == "nums")
                {
                    RowFilter = "";
                    grdOrganisations.Rebind();
                }
                else
                {
                    RowFilter = letter;
                    grdOrganisations.Rebind();
                }
            }

            if (e.CommandName.ToLower() == "search")
            {
                Button btn = (Button)e.CommandSource;
                TextBox txt = btn.NamingContainer.FindControl("txtSearch") as TextBox;

                if (source is RadGrid)
                {
                    RowFilter = txt.Text;
                    grdOrganisations.Rebind();
                }

            }

            if (e.CommandName.ToLower() == "btnexport")
            {
                btnExport_Click(this, e);
            }
        }
        #endregion
    }

}

