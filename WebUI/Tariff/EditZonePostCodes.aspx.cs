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

    public partial class EditZonePostCodes : Orchestrator.Base.BasePage
    {

        //Used to bind data to the Areas grid
        public class PostcodeAreaDisplay
        {
            public int PostcodeAreaID { get; set; }
            public string Area { get; set; }
            public string Zone { get; set; }
         }

        #region Properties

        public int ZoneMapID
        {
            get { return int.Parse(this.Request.QueryString["ZoneMapID"]); }
        }

        #endregion

        #region Event Handlers
        
        protected void Page_Init(object sender, EventArgs e)
        {
            btnZoneMapList.Click += btnZoneMapList_Click;
            btnEditZoneMap.Click += btnEditZoneMap_Click;
            btnAllocate.Click += btnAllocate_Click;
            grdRegions.SelectedIndexChanged += grdRegions_SelectedIndexChanged;
            this.Title = "Orchestrator - Edit Zone Postcodes";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                using (var uow = DIContainer.CreateUnitOfWork())
                {
                    LoadZoneMap(uow);
                    LoadZones(uow);
                    LoadRegions(uow);
                    LoadAreas(uow);
                    LoadZoneAreas(uow);
                }
            }
        }

        private void btnAllocate_Click(object sender, EventArgs e)
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                //Allocate the selected Area to the selected Zone
                AllocateAreasToZone(uow);

                //Reload the Areas grid as the newly allocated Zone needs to be shown
                //next to the Area in the Areas grid
                LoadAreas(uow);

                //Display the newly allocated Areas
                LoadZoneAreas(uow);
            }
        }

        private void btnEditZoneMap_Click(object sender, EventArgs e)
        {
            this.Response.Redirect("~/Tariff/EditZoneMap.aspx?ZoneMapID=" +this.ZoneMapID.ToString());
        }

        private void grdRegions_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                //Load the Areas for the selected Region
                LoadAreas(uow);

                //Its necessary to reload the Zone Ares grid as it is generated
                //at runtime
                LoadZoneAreas(uow);
            }
        }

        protected void btnZoneMapList_Click(object sender, EventArgs e)
        {
            this.Response.Redirect("~/Tariff/ZoneMapList.aspx");
        }

        #endregion

        #region Data Loading

        private void LoadZoneMap(IUnitOfWork uow)
        {
            var repo = DIContainer.CreateRepository<Repositories.IZoneMapRepository>(uow);
            var zoneMap = repo.Find(this.ZoneMapID);
            lblZoneMap.Text = zoneMap.Description;
        }

        private void LoadZones(IUnitOfWork uow)
        {
            var repo = DIContainer.CreateRepository<Repositories.IZoneMapRepository>(uow);

            var zones =
                from z in repo.GetZonesForZoneMap(this.ZoneMapID)
                orderby z.Description
                select new { z.ZoneID, z.Description };

            grdZones.DataSource = zones.ToList();
            grdZones.DataBind();

            if (grdZones.Items.Count > 0)
                grdZones.Items[0].Selected = true;
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

                //Get the Areas for a Region and also the Zone if it is allocated to one
                var areas =
                    from pa in repo.GetPostcodeAreasForRegion(postcodeRegionID)
                    orderby pa.Postcode
                    let zpa = pa.ZonePostcodeAreas.FirstOrDefault(zpa => zpa.Zone.ZoneMapID == this.ZoneMapID)
                    select new PostcodeAreaDisplay
                    {
                        PostcodeAreaID = pa.PostcodeAreaID,
                        Area = pa.Postcode,
                        Zone = zpa == null ? string.Empty : zpa.Zone.Description,
                    };

                grdAreas.DataSource = areas.ToList();
            }

            grdAreas.DataBind();
        }


        private void LoadZoneAreas(IUnitOfWork uow)
        {
            var repo = DIContainer.CreateRepository<Repositories.IZoneMapRepository>(uow);
            var zones = repo.GetZonesForZoneMap(this.ZoneMapID).OrderBy(z => z.Description);

            HtmlTableRow tr;
            HtmlTableCell td;

            foreach (var zone in zones)
            {
                var postcodes = repo.GetPostcodesForZone(zone.ZoneID);
                var postcodeList = postcodes.Any() ? string.Join(", ", postcodes) : "&nbsp;";

                tr = new HtmlTableRow();
                grdZoneAreas.Rows.Add(tr);
                tr.Attributes.Add("class", "rgRow");

                td = new HtmlTableCell();
                tr.Cells.Add(td);
                td.Attributes.Add("class", "rgHeader");
                td.Attributes.Add("style", "white-space:nowrap");
                td.InnerHtml = zone.Description;

                td = new HtmlTableCell();
                tr.Cells.Add(td);
                td.Attributes.Add("class", "rgRow");
                td.InnerHtml = postcodeList;
            }
        }

        //This is an aborted attempt to display the allocated Areas as ranges (e.g. NR1-20, PE,)
        //Problems: Areas are sorted alphabetically so NR1, NR10 ... NR2, NR20
        private void LoadZoneAreas2(IUnitOfWork uow)
        {
            var zoneMapRepo = DIContainer.CreateRepository<Repositories.IZoneMapRepository>(uow);
            var postcodeRegionRepo = DIContainer.CreateRepository<Repositories.IPostcodeRegionRepository>(uow);

            var zoneMapAreas =
                zoneMapRepo.GetZonesForZoneMap(this.ZoneMapID)
                .SelectMany(z => z.ZonePostcodeAreas)
                .Select(zpa => new 
                {
                    ZoneID = zpa.Zone.ZoneID,
                    PostcodeAreaID = zpa.PostcodeAreaID,
                })
                .ToList();

            var postcodeAreas =
                postcodeRegionRepo.GetAll()
                .SelectMany(pr => pr.PostcodeAreas)
                .Select(pa => new
                {
                    PostcodeRegionID = pa.PostcodeRegionID,
                    PostcodeRegion = pa.PostcodeRegion.Description,
                    PostcodeAreaID = pa.PostcodeAreaID,
                    PostcodeArea = pa.Postcode,
                    PostcodeDistrict = pa.Postcode.Replace(pa.PostcodeRegion.Description, string.Empty)
                })
                .ToList();

            var zones = zoneMapRepo.GetZonesForZoneMap(this.ZoneMapID).OrderBy(z => z.Description).ToList();

            var zoneAreaDisplays = new Dictionary<string, string>();

            foreach (var zone in zones)
            {
                string zoneAreaDisplay = string.Empty;
                string previousRegion = string.Empty;
                string previousDistrict = string.Empty;
                string district = string.Empty;
                int firstNumericDistrict = 0;
                int numericDistrict;
                bool isInZone = false;
                bool isPreviousInZone = false;
                bool isDistrictNumeric = false;

                foreach (var area in postcodeAreas.OrderBy(a => a.PostcodeRegion).ThenBy(a => a.PostcodeDistrict))
                {
                    //Get the district portion of the Postcode Area by removing the Area from it
                    district = area.PostcodeArea.Replace(area.PostcodeRegion, string.Empty);
                    numericDistrict = 0;
                    isDistrictNumeric = int.TryParse(district, out numericDistrict);
                    isInZone = zoneMapAreas.Any(za => za.ZoneID == zone.ZoneID && za.PostcodeAreaID == area.PostcodeAreaID );

                    if (area.PostcodeRegion != previousRegion )
                    {
                        if (isPreviousInZone)
                        {
                            if (firstNumericDistrict == 1)
                                zoneAreaDisplay += previousRegion + ", ";
                            else
                                zoneAreaDisplay += string.Format("{0}{1}-{2}, ", previousRegion, firstNumericDistrict, previousDistrict);
                        }
                        firstNumericDistrict = 0;
                        isPreviousInZone = false;
                    }

                    if (!isDistrictNumeric)
                    {

                    }

                    if (isInZone)
                    {
                        if (!isPreviousInZone && isDistrictNumeric)
                            firstNumericDistrict = numericDistrict;
                        isPreviousInZone = true;
                    }
                    else
                    {
                        if (isPreviousInZone)
                        {
                            if (firstNumericDistrict.ToString() == previousDistrict)
                                zoneAreaDisplay += string.Format("{0}{1}, ", previousRegion, previousDistrict);
                            else
                                zoneAreaDisplay += string.Format("{0}{1}-{2}, ", previousRegion, firstNumericDistrict, previousDistrict);
                        }
                        //firstAreaNo = 0;
                        isPreviousInZone = false;
                    }

                    previousDistrict = district;
                    previousRegion = area.PostcodeRegion;
                }

                if (zoneAreaDisplay.Length >= 2)
                    zoneAreaDisplay = zoneAreaDisplay.Remove(zoneAreaDisplay.Length - 2, 2);

                zoneAreaDisplays.Add(zone.Description, zoneAreaDisplay);
            }

            BuildZoneAreasGrid(zoneAreaDisplays);
        }

        private void BuildZoneAreasGrid(Dictionary<string, string> zoneAreaDisplays)
        {
            HtmlTableRow tr;
            HtmlTableCell td;

            foreach (var zoneAreaDisplay in zoneAreaDisplays)
            {
                tr = new HtmlTableRow();

                td = new HtmlTableCell();
                tr.Cells.Add(td);
                td.Attributes.Add("class", "GridRow_Orchestrator");
                td.InnerText = zoneAreaDisplay.Key;

                td = new HtmlTableCell();
                tr.Cells.Add(td);
                td.Attributes.Add("class", "GridRow_Orchestrator");
                td.InnerText = zoneAreaDisplay.Value;
            }

        }

        #endregion

        #region Data Saving

        private void AllocateAreasToZone(IUnitOfWork uow)
        {
            var repo = DIContainer.CreateRepository<Repositories.IZoneMapRepository>(uow);

            //Get the ZonePostcodeAreas for this ZoneMap
            var zonePostcodeAreas = repo.GetZonesForZoneMap(this.ZoneMapID).SelectMany(z => z.ZonePostcodeAreas);

            //Get the selected Zone 
            var zoneID = (int)grdZones.SelectedValue;

            //For each selected Area, find whether it has already been allocated to a zone
            //and either allocate or reallocate it as necessary
            foreach (GridDataItem gdi in grdAreas.SelectedItems)
            {
                var postcodeAreaID = (int)gdi.OwnerTableView.DataKeyValues[gdi.ItemIndex]["PostcodeAreaID"];
                var zonePostcodeArea = zonePostcodeAreas.SingleOrDefault(zpa => zpa.PostcodeAreaID == postcodeAreaID);

                if (zonePostcodeArea == null)
                {
                    //Allocate the selected Area to the selected Zone
                    var zone = repo.FindZone(zoneID);
                    zonePostcodeArea = new Models.ZonePostcodeArea { PostcodeAreaID = postcodeAreaID };
                    zone.ZonePostcodeAreas.Add(zonePostcodeArea);
                }
                else
                {
                    //Reallocate the selected area to the selected Zone
                    zonePostcodeArea.ZoneID = zoneID;
                }
            }

            uow.SaveChanges();
        }

        #endregion

    }

}
