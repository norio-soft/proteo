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

namespace Orchestrator.WebUI.Organisation
{
    public partial class ClientCustomers : Orchestrator.Base.BasePage
    {
        #region Page Fields
        protected DataTable GridData
        {
            get { return (DataTable)this.ViewState["_gridData"]; }
            set { this.ViewState["_gridData"] = value; }
        }

        protected DataView FilteredData
        {
            get { return (DataView)this.ViewState["_filteredData"]; }
            set { this.ViewState["_filteredData"] = value; }
        }
        #endregion

        #region Moved the viewstate to the server for optimisation
        protected override PageStatePersister PageStatePersister
        {
            get
            {
                return new SessionPageStatePersister(this.Page);
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.grdOrganisations.NeedDataSource +=new GridNeedDataSourceEventHandler(grdOrganisations_NeedDataSource);
            this.grdOrganisations.ItemCommand += new GridCommandEventHandler(grdOrganisations_ItemCommand);
            this.grdOrganisations.ItemCreated +=new GridItemEventHandler(grdOrganisations_ItemCreated);

        }

        #region Grid Events

        void grdOrganisations_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            if (this.FilteredData == null)
            {
                DataSet ds = null;
                using (Facade.Organisation facOrganisation = new Facade.Organisation())
                {
                    if (Request.QueryString["incomplete"] != null)
                        ds = facOrganisation.GetIncomplete();
                    else
                        ds = facOrganisation.GetAllForType((int)eOrganisationType.ClientCustomer);
                }
                this.GridData = ds.Tables[0];
                this.FilteredData = ds.Tables[0].DefaultView;
                grdOrganisations.DataSource = this.FilteredData;
            }
            else
            {
                grdOrganisations.DataSource = this.FilteredData;
            }

        }

        void grdOrganisations_ItemCreated(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {

            if (e.Item is GridCommandItem)
            {
                GridCommandItem gci = e.Item as GridCommandItem;

                // gci.Load += new EventHandler(gci_Load);

            }
            //if (e.Item is GridPagerItem)
            //{
            //    GridPagerItem gpi = e.Item as GridPagerItem;
            //    gpi.PagerContentCell.Controls.Clear();

            //    for (int i = 65; i <= 65 + 25; i++)
            //    {
            //        LinkButton lb;
            //        LiteralControl lc;
            //        if (i == 65)
            //        {
            //            lb = new LinkButton();
            //            lc = new LiteralControl();
            //            lc.Text = " ";
            //            lb.Text = "ALL";
            //            lb.CommandName = "alpha";
            //            lb.CommandArgument = "%";
            //            gpi.PagerContentCell.Controls.Add(lb);
            //            gpi.PagerContentCell.Controls.Add(lc);
            //        }

            //        lb = new LinkButton();
            //        lc = new LiteralControl();
            //        lc.Text = " ";
            //        lb.CommandName = "alpha";
            //        lb.Text = "" + (char)i;
            //        lb.CommandArgument = "" + (char)i;
            //        gpi.PagerContentCell.Controls.Add(lb);
            //        gpi.PagerContentCell.Controls.Add(lc);

            //        if (i == 90)
            //        {
            //            TextBox txtFilter = new TextBox();
            //            gpi.PagerContentCell.Controls.Add(txtFilter);
            //        }

            //    }
            //}

        }

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

        void grdOrganisations_ItemCommand(object source, GridCommandEventArgs e)
        {
            if (e.CommandName.ToLower() == "alpha")
            {
                string letter = e.CommandArgument.ToString()[0].ToString();
                //int pageNumber = int.Parse(e.CommandArgument.ToString()[1].ToString());
                //int bot, top;
                //bot = pageNumber * grdOrganisations.PageSize;
                //top = bot + grdOrganisations.PageSize;

                string rowFilter = "OrganisationName LIKE '{0}%'";
                DataView dv = this.GridData.DefaultView;
                dv.RowFilter = string.Format(rowFilter, letter);
                this.FilteredData = dv;
                grdOrganisations.Rebind();
            }

            if (e.CommandName.ToLower() == "search")
            {
                Button btn = (Button)e.CommandSource;
                TextBox txt = btn.NamingContainer.FindControl("txtSearch") as TextBox;

                if (source is RadGrid)
                {
                    string filterText = txt.Text;
                    string rowFilter = "OrganisationName LIKE '%{0}%'";
                    DataView dv = this.GridData.DefaultView;
                    dv.RowFilter = string.Format(rowFilter, filterText);
                    this.FilteredData = dv;

                }
                grdOrganisations.Rebind();
            }
        }
        #endregion
    }

}