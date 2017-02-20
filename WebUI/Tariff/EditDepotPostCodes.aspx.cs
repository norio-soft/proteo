using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Tariff
{

    public partial class EditDepotPostCodes : Orchestrator.Base.BasePage
    {

        //Used to bind data to the Areas grid
        public class PostcodeAreaDisplay
        {
            public int PostcodeAreaID { get; set; }
            public string Area { get; set; }
            public string Depot { get; set; }
         }

        #region Event Handlers
        
        protected void Page_Init(object sender, EventArgs e)
        {
            btnAllocate.Click += btnAllocate_Click;
            grdRegions.SelectedIndexChanged += grdRegions_SelectedIndexChanged;
            this.Title = "Orchestrator - Edit Depot Postcodes";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                using (var uow = DIContainer.CreateUnitOfWork())
                {
                    LoadNetworks(uow);
                    LoadDepots(uow);
                    LoadRegions(uow);
                    LoadAreas(uow);
                    LoadDepotAreas(uow);
                }
            }
        }

        private void btnAllocate_Click(object sender, EventArgs e)
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                //Allocate the selected Area to the selected Depot
                AllocateAreasToDepot(uow);

                //Reload the Areas grid as the newly allocated Depot needs to be shown
                //next to the Are in the Areas grid
                LoadAreas(uow);

                //Display the newly allocated Areas
                LoadDepotAreas(uow);
            }
        }

        private void grdRegions_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                //Load the Areas for the selected Region
                LoadAreas(uow);

                //Its necessary to reload the Depot Ares grid as it is generated at runtime
                LoadDepotAreas(uow);
            }
        }

        #endregion

        #region Data Loading

        private void LoadNetworks(IUnitOfWork uow)
        {
            var repo = DIContainer.CreateRepository<Repositories.IDepotRepository>(uow);

            var networks =
                from n in repo.GetAllNetworks()
                orderby n.Organisation.OrganisationName
                select new { n.IdentityID, n.Organisation.OrganisationName };

            cboNetworks.DataValueField = "IdentityID";
            cboNetworks.DataTextField = "OrganisationName";
            cboNetworks.DataSource = networks.ToList();
            cboNetworks.DataBind();
        }

        private void LoadDepots(IUnitOfWork uow)
        {
            var repo = DIContainer.CreateRepository<Repositories.IDepotRepository>(uow);
            var networkIdentityID = int.Parse(cboNetworks.SelectedValue);

            var depots =
                from d in repo.GetForNetworkIdentityID(networkIdentityID)
                orderby d.Code
                select new { d.DepotID, d.Code };

            grdDepots.DataSource = depots.ToList();
            grdDepots.DataBind();

            if (grdDepots.Items.Count > 0)
                grdDepots.Items[0].Selected = true;
        }

        private void LoadRegions(IUnitOfWork uow)
        {
            var repo = DIContainer.CreateRepository<Repositories.IPostcodeRegionRepository>(uow);

            var postcodeRegions =
                from pr in repo.GetAll()
                orderby pr.Description
                select new { pr.PostcodeRegionID, pr.Description };

            grdRegions.DataSource = postcodeRegions.ToList();
            grdRegions.DataBind();

            if (grdRegions.Items.Count > 0)
                grdRegions.Items[0].Selected = true;
        }

        private void LoadAreas(IUnitOfWork uow)
        {
            if (grdRegions.SelectedValue == null)
                grdAreas.DataSource = null;
            else
            {
                var repo = DIContainer.CreateRepository<Repositories.IPostcodeRegionRepository>(uow);
                var postcodeRegionID = int.Parse(grdRegions.SelectedValue.ToString());
                var networkIdentityID = int.Parse(cboNetworks.SelectedValue);

                //Get the Areas for a Region and also the Depot if it is allocated to one
                var areas =
                    from pa in repo.GetPostcodeAreasForRegion(postcodeRegionID)
                    orderby pa.Postcode
                    let dpa = pa.DepotPostcodeAreas.FirstOrDefault(dpa => dpa.Depot.NetworkIdentityID == networkIdentityID)
                    select new PostcodeAreaDisplay
                    {
                        PostcodeAreaID = pa.PostcodeAreaID,
                        Area = pa.Postcode,
                        Depot = dpa == null ? string.Empty : dpa.Depot.Code,
                    };

                grdAreas.DataSource = areas.ToList();
            }

            grdAreas.DataBind();
        }

        private void LoadDepotAreas(IUnitOfWork uow)
        {
            if (cboNetworks.SelectedIndex < 0)
                return;

            var networkIdentityID = int.Parse(cboNetworks.SelectedValue);
            var repo = DIContainer.CreateRepository<Repositories.IDepotRepository>(uow);
            var depots = repo.GetForNetworkIdentityID(networkIdentityID).OrderBy(d => d.Code);

            HtmlTableRow tr;
            HtmlTableCell td;

            foreach (var depot in depots)
            {
                var postcodes = repo.GetPostcodesForDepot(depot.DepotID);
                var postcodeList = postcodes.Any() ? string.Join(", ", postcodes) : "&nbsp;";

                tr = new HtmlTableRow();
                grdDepotAreas.Rows.Add(tr);
                tr.Attributes.Add("class", "rgRow");

                td = new HtmlTableCell();
                tr.Cells.Add(td);
                td.Attributes.Add("class", "rgHeader");
                td.Attributes.Add("style", "white-space:nowrap");
                td.InnerHtml = depot.Code;

                td = new HtmlTableCell();
                tr.Cells.Add(td);
                td.Attributes.Add("class", "rgRow");
                td.InnerHtml = postcodeList;
            }
        }

        #endregion


        #region Data Saving

        private void AllocateAreasToDepot(IUnitOfWork uow)
        {
            var repo = DIContainer.CreateRepository<Repositories.IDepotRepository>(uow);
            var networkIdentityID = int.Parse(cboNetworks.SelectedValue);

            //Get the DepotPostcodeAreas for this DepotMap
            var depotPostcodeAreas = repo.GetForNetworkIdentityID(networkIdentityID).SelectMany(d => d.DepotPostcodeAreas);

            //Get the selected Depot 
            var depotID = (int)grdDepots.SelectedValue;

            //For each selected Area, find whether it has already been allocated to a Depot
            //and either allocate or reallocate it as necessary
            foreach (GridDataItem gdi in grdAreas.SelectedItems)
            {
                var postcodeAreaID = (int)gdi.OwnerTableView.DataKeyValues[gdi.ItemIndex]["PostcodeAreaID"];
                var depotPostcodeArea = depotPostcodeAreas.SingleOrDefault(dpa => dpa.PostcodeAreaID == postcodeAreaID);

                if (depotPostcodeArea == null)
                {
                    //Allocate the selected Area to the selected Depot
                    var depot = repo.Find(depotID);
                    depotPostcodeArea = new Models.DepotPostcodeArea { PostcodeAreaID = postcodeAreaID };
                    depot.DepotPostcodeAreas.Add(depotPostcodeArea);
                }
                else
                {
                    //Reallocate the selected area to the selected Depot
                    depotPostcodeArea.DepotID = depotID;
                }
            }

            uow.SaveChanges();
        }

        #endregion

    }

}
