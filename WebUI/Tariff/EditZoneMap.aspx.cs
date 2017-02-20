using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Humanizer;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Tariff
{

    public partial class EditZoneMap : Orchestrator.Base.BasePage
    {

        #region Properties

        public int ZoneMapID
        {
            get { return int.Parse(this.Request.QueryString["ZoneMapID"]); }
        }

        #endregion

        #region Event Handlers

        protected void Page_Init(object sender, EventArgs e)
        {
            btnSave.Click += btnSave_Click;
            btnZonePostcodes.Click += btnZonePostcodes_Click;
            btnZoneMapList.Click += btnZoneMapList_Click;
            this.Title = "Orchestrator - Zone Map";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                LoadZoneMap();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SaveZoneMap();
        }

        void btnZonePostcodes_Click(object sender, EventArgs e)
        {
            this.Response.Redirect("~/Tariff/EditZonePostcodes.aspx?ZoneMapID=" + this.ZoneMapID);
        }

        protected void btnZoneMapList_Click(object sender, EventArgs e)
        {
            this.Response.Redirect("~/Tariff/ZoneMapList.aspx");
        }

        #endregion

        #region Data Loading

        private void LoadZoneMap()
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<Repositories.IZoneMapRepository>(uow);
                var zoneMap = repo.Find(this.ZoneMapID);

                txtZoneMapDescription.Text = zoneMap.Description;
                chkIsEnabled.Checked = zoneMap.IsEnabled;
                lblZoneType.Text = zoneMap.ZoneType.Humanize();

                LoadZones(zoneMap);
            }
        }

        private void LoadZones(Models.ZoneMap zoneMap)
        {
            //Reset the hidden field that holds the selected cellIds
            hidZoneChanges.Value = string.Empty;

            HtmlTableRow tr = null;
            HtmlTableCell td = null;

            //For each ZoneMap Zone
            foreach (var zone in zoneMap.Zones.OrderBy(z => z.Description))
            {
                tr = new HtmlTableRow();
                tr.Attributes.Add("class", "GridRow_Orchestrator");
                grdZones.Rows.Add(tr);

                //Add a cell for the Zone
                td = new HtmlTableCell();
                tr.Cells.Add(td);

                //Create a new input box and add it to the cell
                //Set the cells id so that it indicates to Zone and Pallets
                TextBox input = new TextBox();
                td.Controls.Add(input);
                input.ID = string.Format("cell:{0}", zone.ZoneID);
                input.Text = zone.Description;

                //Each input box will call a function when its value is changed so that 
                //the cell colour can be changed and the id of th input recorded in the hidden field
                input.Attributes.Add("onchange", "Zone_onchange(this);");
            }

            int newZones = 10;

            for (int i = 0; i < newZones; i++)
            {
                tr = new HtmlTableRow();
                tr.Attributes.Add("class", "GridRow_Orchestrator");
                grdZones.Rows.Add(tr);

                //Add a cell for the Zone
                td = new HtmlTableCell();
                tr.Cells.Add(td);

                //Create a new input box and add it to the cell
                //Set the cells id so that it indicates to Zone and Pallets
                TextBox input = new TextBox();
                td.Controls.Add(input);
                input.ID = string.Format("new:{0}", i);

                //Each input box will call a function when its value is chnaged so that 
                //the cell colour can be changed and the id of th input recorded in the hidden field
                input.Attributes.Add("onchange", "Zone_onchange(this);");
            }
        }

        #endregion

        #region Data Saving

        private void SaveZoneMap()
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<Repositories.IZoneMapRepository>(uow);
                var zoneMap = repo.Find(this.ZoneMapID);

                //Set the ZoneMap's Description and IsEnabled
                zoneMap.Description = txtZoneMapDescription.Text;
                zoneMap.IsEnabled = chkIsEnabled.Checked;

                //Get a list of the Zone descriptions that were changed
                string[] cellIds = hidZoneChanges.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                //For each changed Zone description
                foreach (string cellId in cellIds)
                {
                    //The cellId is cell:<ZoneId> or new:<n>
                    string[] parts = cellId.Split(':');
                    bool isNew = parts[0].Contains("new");
                    int zoneID = int.Parse(parts[1]);
                    string description = this.Request.Form[cellId].Trim();

                    Models.Zone zone = null;

                    //If the description is blank then delete the zone
                    if (string.IsNullOrEmpty(description))
                    {
                        if (isNew)
                        {
                            //do nothing
                        }
                        else
                        {
                            if (repo.DoesZoneHaveTariffRates(zoneID))
                            {
                                this.MessageBox(string.Format("The zone {0} that you have tried to delete has Tariff Rates assigned to it. Please remove these before attempting to delete this zone again.", zone.Description));
                                return;
                            }

                            zone = zoneMap.Zones.Single(z => z.ZoneID == zoneID);

                            if (zone.ZonePostcodeAreas.Any())
                            {
                                this.MessageBox(string.Format("The zone {0} that you have tried to delete has Postcode Areas assigned to it. Please remove these before attempting to delete this zone again.", zone.Description));
                                return;
                            }

                            repo.RemoveZone(zone);
                        }
                    }
                    else
                    {
                        if (isNew)
                        {
                            zone = new Models.Zone { ZoneType = zoneMap.ZoneType };
                            zoneMap.Zones.Add(zone);
                        }
                        else
                        {
                            zone = zoneMap.Zones.Single(z => z.ZoneID == zoneID);
                        }

                        zone.Description = this.Request.Form[cellId];
                    }
                }

                uow.SaveChanges();

                LoadZones(zoneMap);
            }
        }
        
        #endregion

    }

}
