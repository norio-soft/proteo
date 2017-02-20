using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Humanizer;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Tariff
{
    public partial class EditTariff : Orchestrator.Base.BasePage
    {

        #region Properties

        private int? _tariffID = null;
        public int TariffID
        {
            get
            {
                if (!_tariffID.HasValue)
                {
                    if (!string.IsNullOrWhiteSpace(this.Request.QueryString["TariffID"]))
                        _tariffID = int.Parse(this.Request.QueryString["TariffID"]);
                    else
                    {
                        using (var uow = DIContainer.CreateUnitOfWork())
                        {
                            var repo = DIContainer.CreateRepository<Repositories.ITariffRepository>(uow);
                            _tariffID = repo.FindTariffTable(this.TariffTableID.Value).TariffVersion.TariffID;
                        }
                    }
                }

                return _tariffID.Value;
            }
        }

        public int? TariffTableID
        {
            get { return string.IsNullOrWhiteSpace(this.Request.QueryString["TariffTableID"]) ? (int?)null : int.Parse(this.Request.QueryString["TariffTableID"]); }
        }

        #endregion

        #region Event Handlers

        protected void Page_Init(object sender, EventArgs e)
        {
            cboTariffVersion.SelectedIndexChanged += new RadComboBoxSelectedIndexChangedEventHandler(cboTariffVersion_SelectedIndexChanged);
            cboTariffTable.SelectedIndexChanged += new RadComboBoxSelectedIndexChangedEventHandler(cboTariffTable_SelectedIndexChanged);
            btnNewVersion.Click += new EventHandler(btnNewVersion_Click);
            btnNewTable.Click += new EventHandler(btnNewTable_Click);
            btnSave.Click += new EventHandler(btnSave_Click);
            btnTariffList.Click += new EventHandler(btnTariffList_Click);
            btnDeleteTariffVersion.Click += new EventHandler(btnDeleteTariffVersion_Click);
            btnDeleteTariffTable.Click += new EventHandler(btnDeleteTariffTable_Click);

            this.Title = "Orchestrator - Edit Tariff";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                using (var uow = DIContainer.CreateUnitOfWork())
                {
                    LoadGoodsTypes();
                    LoadServiceLevels();
                    LoadPalletTypes();

                    LoadTariff(uow);
                }
            }
        }

        protected void cboTariffVersion_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                LoadTariffVersion(uow);
            }
        }

        protected void cboTariffTable_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<Repositories.ITariffRepository>(uow);
                
                var tariffVersionID = int.Parse(cboTariffVersion.SelectedValue);
                var tariffVersion = repo.FindTariffVersion(tariffVersionID);

                LoadExtraTypeRates(uow, tariffVersion);
                LoadTariffTable(uow);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                SaveTariffAndTariffVersion(uow);
                SaveTariffTable(uow);
                SaveTariffRates(uow);
                SaveExtraTypeRates(uow);
                
                uow.SaveChanges();

                LoadTariff(uow);
            }
        }

        protected void btnTariffList_Click(object sender, EventArgs e)
        {
            this.Response.Redirect("~/Tariff/TariffList.aspx");
        }

        protected void btnNewVersion_Click(object sender, EventArgs e)
        {
            this.Response.Redirect("~/Tariff/AddTariffVersion.aspx?TariffID=" + this.TariffID);
        }

        protected void btnNewTable_Click(object sender, EventArgs e)
        {
            //If a TariffTable is selected pass it to AddTariffTable as the default to copy from
            //otherwise pass the selected version as the default to copy from
            if (cboTariffTable.SelectedValue.Length > 0)
                this.Response.Redirect("~/Tariff/AddTariffTable.aspx?TariffTableID=" + cboTariffTable.SelectedValue);
            else
                this.Response.Redirect("~/Tariff/AddTariffTable.aspx?TariffVersionID=" + cboTariffVersion.SelectedValue);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.Response.Redirect("~/Tariff/TariffList.aspx");
        }

        protected void btnDeleteTariffVersion_Click(object sender, EventArgs e)
        {
            if (cboTariffVersion.SelectedIndex < 0)
                return;

            var tariffVersionID = int.Parse(cboTariffVersion.SelectedValue);

            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<Repositories.ITariffRepository>(uow);

                var tariff = repo.Find(this.TariffID);

                //If this is the last version for the Tariff delete the Tariff and redirect to Tariff List
                if (tariff.TariffVersions.Count == 1)
                {
                    repo.Remove(tariff);
                    uow.SaveChanges();
                    this.Response.Redirect("~/Tariff/TariffList.aspx");
                }
                else
                {
                    var tariffVersion = tariff.TariffVersions.FirstOrDefault(tv => tv.TariffVersionID == tariffVersionID);
                    repo.RemoveTariffVersion(tariffVersion);
                    uow.SaveChanges();
                    LoadTariff(uow);
                }
            }
        }

        protected void btnDeleteTariffTable_Click(object sender, EventArgs e)
        {
            if (cboTariffTable.SelectedIndex < 0)
                return;

            var tariffTableID = int.Parse(cboTariffTable.SelectedValue);

            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<Repositories.ITariffRepository>(uow);

                var tariffTable = repo.FindTariffTable(tariffTableID);
                repo.RemoveTariffTable(tariffTable);

                uow.SaveChanges();
                
                LoadTariffVersion(uow);
            }
        }

        #endregion

        #region Data Loading

        private void LoadGoodsTypes()
        {
            cboGoodsType.DataValueField = "GoodsTypeID";
            cboGoodsType.DataTextField = "Description";
            cboGoodsType.DataSource = Facade.GoodsType.GetAllActiveGoodsTypes();
            cboGoodsType.DataBind();
            cboGoodsType.Items.Insert(0, new RadComboBoxItem("Any"));
        }
        
        private void LoadServiceLevels()
        {
            Facade.IOrderServiceLevel facServiceLevel = new Facade.Order();

            cboServiceLevel.DataValueField = "OrderServiceLevelID";
            cboServiceLevel.DataTextField = "Description";
            cboServiceLevel.DataSource = facServiceLevel.GetAll();
            cboServiceLevel.DataBind();
            cboServiceLevel.Items.Insert(0, new RadComboBoxItem("Any"));
        }

        private void LoadPalletTypes()
        {
            cboPalletType.DataValueField = "PalletTypeID";
            cboPalletType.DataTextField = "Description";
            cboPalletType.DataSource = Facade.PalletType.GetAllPalletTypes();
            cboPalletType.DataBind();
            cboPalletType.Items.Insert(0, new RadComboBoxItem("Any"));
        }

        private void LoadTariff(IUnitOfWork uow)
        {
            var repo = DIContainer.CreateRepository<Repositories.ITariffRepository>(uow);
            var tariff = repo.Find(this.TariffID);

            txtTariffDescription.Text = tariff.Description;
            chkIsForSubContractor.Checked = tariff.IsForSubContractor;
            lblZoneType.Text = tariff.ZoneType.Humanize();
            lblMetric.Text = tariff.Metric.Humanize();

            //Load the multiplier values
            chkMultiplyByOrderValue.Checked = tariff.MultiplyByOrderValue;
            lblAdditionalMultiplier.Enabled = tariff.MultiplyByOrderValue;
            txtAdditionalMultiplier.Enabled = tariff.MultiplyByOrderValue;
            txtAdditionalMultiplier.Value = tariff.MultiplyByOrderValue ? (double?)tariff.AdditionalMultiplier : null;

            rbGroupedOrderUseHighestRated.Checked = !(this.rbGroupedOrderRateIndividually.Checked = tariff.GroupedOrdersRateIndividually);
            chkIgnoreAdditionalCollectsFromADeliveryPoint.Checked = !tariff.GroupedOrdersRateIndividually && tariff.IgnoreAdditionalCollectsFromADeliveryPoint;
            chkRateCombinedWherePointsMatch.Checked = tariff.GroupedOrdersRateIndividually && tariff.RateCombinedWherePointsMatch;

            LoadTariffVersions(uow, tariff);
        }

        private void LoadTariffVersions(IUnitOfWork uow, Models.Tariff tariff)
        {
            var tariffVersions = tariff.TariffVersions.OrderByDescending(tv => tv.StartDate);
            
            //Only enable the New Version button if the latest version has been finished
            btnNewVersion.Enabled = tariffVersions.First().FinishDate.HasValue;

            var currentTariffVersionID = (cboTariffVersion.SelectedIndex > -1) ? int.Parse(cboTariffVersion.SelectedValue) : (int?)null;

            cboTariffVersion.DataValueField = "TariffVersionID";
            cboTariffVersion.DataTextField = "Description";
            cboTariffVersion.DataSource = tariffVersions.Select(tv => new { tv.TariffVersionID, tv.Description }).ToList();
            cboTariffVersion.DataBind();

            //Choose the selected TariffVersion if one was passed otherwise choose the first which is the latest
            if (cboTariffVersion.Items.Count > 0)
            {
                int? tariffVersionID = null;

                if (currentTariffVersionID.HasValue)
                    tariffVersionID = currentTariffVersionID;
                else
                {
                    if (this.TariffTableID.HasValue)
                    {
                        var repo = DIContainer.CreateRepository<Repositories.ITariffRepository>(uow);
                        var tariffTable = repo.FindTariffTable(this.TariffTableID.Value);
                        tariffVersionID = tariffTable.TariffVersionID;
                    }
                }

                if (tariffVersionID.HasValue)
                {
                    var selectedTariffVersion = cboTariffTable.FindItemByValue(tariffVersionID.Value.ToString());

                    if (selectedTariffVersion != null)
                        selectedTariffVersion.Selected = true;
                }

                LoadTariffVersion(uow);
            }
        }

        private void LoadTariffVersion(IUnitOfWork uow)
        {
            if (cboTariffVersion.SelectedIndex < 0)
            {
                if (!cboTariffVersion.Items.Any())
                    return;

                cboTariffVersion.SelectedIndex = 0;
            }

            var repo = DIContainer.CreateRepository<Repositories.ITariffRepository>(uow);
            var tariffVersionID = int.Parse(cboTariffVersion.SelectedValue);

            //Get the TariffVersion and its TariffRates
            var tariffVersion = repo.FindTariffVersion(tariffVersionID);

            //Set the TariffVersion fields
            txtVersionDescription.Text = tariffVersion.Description;
            dteStartDate.SelectedDate = tariffVersion.StartDate;
            dteFinishDate.SelectedDate = tariffVersion.FinishDate;

            var zoneMapRepo = DIContainer.CreateRepository<Repositories.IZoneMapRepository>(uow);
            lblZoneMap.Text = zoneMapRepo.Find(tariffVersion.ZoneMapID).Description;

            var scaleRepo = DIContainer.CreateRepository<Repositories.IScaleRepository>(uow);
            lblScale.Text = scaleRepo.Find(tariffVersion.ScaleID).Description;

            LoadZones(tariffVersion);
            LoadTariffTables(uow, tariffVersion);
            LoadExtraTypeRates(uow, tariffVersion);
        }

        private void LoadZones(Models.TariffVersion tariffVersion)
        {
            var zones = tariffVersion.ZoneMap.Zones.OrderBy(z => z.Description).Select(z => new { z.ZoneID, z.Description });

            cboCollectionZone.DataValueField = "ZoneID";
            cboCollectionZone.DataTextField = "Description";
            cboCollectionZone.DataSource = zones.ToList();
            cboCollectionZone.DataBind();
            cboCollectionZone.Items.Insert(0, new RadComboBoxItem("Any"));
        }

        private void LoadTariffTables(IUnitOfWork uow, Models.TariffVersion tariffVersion)
        {
            cboTariffTable.Items.Clear();

            var tariffTables = tariffVersion.TariffTables.OrderBy(tt => tt.Description).Select(tt => new { tt.TariffTableID, tt.Description });
            var currentTariffTableID = (cboTariffTable.SelectedIndex > -1) ? int.Parse(cboTariffTable.SelectedValue) : (int?)null;

            //Populate the TariffTable combo
            cboTariffTable.DataValueField = "TariffTableID";
            cboTariffTable.DataTextField = "Description";
            cboTariffTable.DataSource = tariffTables; 
            cboTariffTable.DataBind();

            if (cboTariffTable.Items.Any())
            {
                //If a TariffTable was passed try to find it in the combo and select it
                int? tariffTableID = currentTariffTableID ?? this.TariffTableID;

                if (tariffTableID.HasValue)
                {
                    var selectedItem = cboTariffTable.FindItemByValue(tariffTableID.Value.ToString());

                    if (selectedItem != null)
                        selectedItem.Selected = true;
                }
            }

            LoadTariffTable(uow);
        }

        private void LoadTariffTable(IUnitOfWork uow)
        {
            if (cboTariffTable.SelectedIndex < 0)
            {
                if (!cboTariffTable.Items.Any())
                {
                    //Blank the fields if no TariffTable is available
                    txtTableDescription.Text = string.Empty;
                    cboCollectionZone.SelectedIndex = 0;
                    cboServiceLevel.SelectedIndex = 0;
                    cboGoodsType.SelectedIndex = 0;
                    cboPalletType.SelectedIndex = 0;
                    rntxtMultiCollectRate.Value = 0f;
                    rntxtMultiDropRate.Value = 0f;
                    LoadTariffRates(null);

                    return;
                }

                cboTariffTable.SelectedIndex = 0;
            }

            var repo = DIContainer.CreateRepository<Repositories.ITariffRepository>(uow);
            var tariffTableID = int.Parse(cboTariffTable.SelectedValue);
            var tariffTable = repo.FindTariffTable(tariffTableID);

            txtTableDescription.Text = tariffTable.Description;

            //ASSUMPTION: The CollectionZone, cboServiceLevel, cboGoodsType and cboPalletType combos have been populated
            
            if (!tariffTable.CollectionZoneID.HasValue)
                cboCollectionZone.SelectedIndex = 0;
            else
                cboCollectionZone.SelectedValue = tariffTable.CollectionZoneID.ToString();

            if (!tariffTable.OrderServiceLevelID.HasValue)
                cboServiceLevel.SelectedIndex = 0;
            else
                cboServiceLevel.SelectedValue = tariffTable.OrderServiceLevelID.ToString();
                
            if (!tariffTable.GoodsTypeID.HasValue)
                cboGoodsType.SelectedIndex = 0;
            else
                cboGoodsType.SelectedValue = tariffTable.GoodsTypeID.ToString();

            if (!tariffTable.PalletTypeID.HasValue)
                cboPalletType.SelectedIndex = 0;
            else
                cboPalletType.SelectedValue = tariffTable.PalletTypeID.ToString();

            rntxtMultiCollectRate.Value = (double?)tariffTable.MultiCollectRate;
            rntxtMultiDropRate.Value = (double?)tariffTable.MultiDropRate;
            chkUseGreatestRateForPrimaryRate.Checked = tariffTable.UseGreatestRateForPrimaryRate;

            LoadTariffRates(tariffTable);
        }

        private void LoadTariffRates(Models.TariffTable tariffTable)
        {
            //Reset the hidden field that holds the selected cellIDs
            hidTariffRateChanges.Value = string.Empty;

            //Return now if a TariffTable has not been selected
            if (tariffTable == null)
                return;

            //Get the Metric
            var tariffVersion = tariffTable.TariffVersion;
            var metric = tariffVersion.Tariff.Metric;

            //Get a list of zones for the selected TariffTable's TariffVersion's ZoneMap
            var zones =
                (from z in tariffVersion.ZoneMap.Zones
                 orderby z.Description
                 select new { z.ZoneID, z.Description }).ToList();

            //Get a list of scale values for the selected TariffTable's TariffVersion's ZoneMap
            var scaleValues =
                (from sv in tariffVersion.Scale.ScaleValues
                 orderby sv.Value
                 select new { sv.ScaleValueID, sv.Value }).ToList();

            //Get the TariffRates for the selected TariffTable
            var rates = tariffTable.TariffRates.Where(tr => tr.IsEnabled);

            //If the TariffTable decimal places is not the default of 2 then set it
            var format = "0.00";

            if (tariffTable.RateDecimalPlaces != 2)
            {
                (rimTariffRates.InputSettings[0] as NumericTextBoxSetting).DecimalDigits = tariffTable.RateDecimalPlaces;
                format = "0." + new string('0', tariffTable.RateDecimalPlaces);
            }

            HtmlTableRow cornerTR;
            HtmlTableRow headerTR;
            HtmlTableRow metricTR;
            HtmlTableRow mainTR;
            HtmlTableCell td;

            //Load the Corner and Header Row with Metric and Zones
            cornerTR = new HtmlTableRow();
            headerTR = new HtmlTableRow();

            grdTariffRatesCorner.Rows.Add(cornerTR);
            grdTariffRatesHeader.Rows.Add(headerTR);

            //Add the top left corner cell
            td = new HtmlTableCell();
            cornerTR.Cells.Add(td);
            td.Attributes.Add("class", "rgHeader");

            switch (metric)
            {
                case eMetric.PalletSpaces:
                case eMetric.PalletforceSpaces:
                    td.InnerText = "Pals";
                    break;

                case eMetric.Weight:
                    td.InnerText = "Kgs";
                    break;

                case eMetric.Volume:
                    td.InnerText = "m3";
                    break;
            }

            foreach (var zone in zones)
            {
                td = new HtmlTableCell();
                headerTR.Cells.Add(td);
                td.Attributes.Add("class", "rgHeader");
                td.InnerText = zone.Description;
            }

            var altRow = false;

            foreach (var scaleValue in scaleValues)
            {
                //Add a row to the Metric and Main parts.
                metricTR = new HtmlTableRow();
                mainTR = new HtmlTableRow();

                if (!altRow)
                {
                    mainTR.Attributes.Add("class", "rgRow");
                    altRow = true;
                }
                else
                {
                    mainTR.Attributes.Add("class", "rgAltRow");
                    altRow = false;
                }

                grdTariffRatesMetric.Rows.Add(metricTR);
                grdTariffRates.Rows.Add(mainTR);                

                //Add a row heading with the number of pallets
                td = new HtmlTableCell();
                metricTR.Cells.Add(td);
                td.Attributes.Add("class", "rgHeader");
                td.InnerText = scaleValue.Value.ToString();
                
                //Load the rate for the zone and pallet
                foreach (var zone in zones)
                {
                    //Create a new cell
                    td = new HtmlTableCell();
                    mainTR.Cells.Add(td);
                    
                    //Create a new input box and add it to the cell
                    //Set the cells id so that it indicates to Zone and Pallets
                    var input = new TextBox();
                    td.Controls.Add(input);
                    input.ID = string.Format("cell:{0}:{1}", zone.ZoneID, scaleValue.ScaleValueID);

                    //Find the TariffRate for this cells Zone and Pallets
                    var rate = rates.FirstOrDefault(r => r.ZoneID == zone.ZoneID && r.ScaleValueID == scaleValue.ScaleValueID);

                    if (rate != null)
                        input.Text = rate.Rate.ToString(format);

                    //Each input box will call a function when its value is chnaged so that 
                    //the cell colour can be changed and the id of th input recorded in the hidden field
                    input.Attributes.Add("onchange", "TariffRate_onchange(this);");
                }
            }
        }

        private void LoadExtraTypeRates(IUnitOfWork uow, Models.TariffVersion tariffVersion)
        {
            var extraTypeRepo = DIContainer.CreateRepository<Repositories.IExtraTypeRepository>(uow);

            var tariffRepo = DIContainer.CreateRepository<Repositories.ITariffRepository>(uow);
            var extraTypesWithRates = tariffRepo.GetExtraTypeRateSummaryForTariffVersion(tariffVersion.TariffVersionID);

            var extraTypesForSerialization =
                extraTypesWithRates.Select(etwr => new ExtraTypeWithRates
                {
                    extraTypeID = etwr.ExtraTypeID,
                    description = etwr.Description,
                    scaleValueRates = etwr.ScaleValueRates.Select(svr => new ExtraTypeWithRates.ScaleValueRate
                    {
                        scaleValue = svr.ScaleValue,
                        rate = svr.Rate,
                        isDirty = false,
                    }),
                });

            hidExtraTypeRates.Value = Newtonsoft.Json.JsonConvert.SerializeObject(extraTypesForSerialization);

            foreach (var extraType in extraTypesWithRates)
            {
                //Add a cell for the Extra
                var table = new HtmlTable();
                pnlExtraTypeRates.Controls.Add(table);
                table.Attributes.Add("class", "rgMasterTable");
                
                var tr = new HtmlTableRow();
                table.Rows.Add(tr);
                tr.Attributes.Add("class", "rgRow");
                
                var th = new HtmlTableCell("th");
                tr.Cells.Add(th);
                th.Attributes.Add("class", "rgHeader");
                th.InnerText = extraType.Description;

                //Create a new cell
                var td = new HtmlTableCell();
                td.Attributes.Add("class", "extraTypeRateSummary");
                td.Attributes.Add("data-extra-type-id", extraType.ExtraTypeID.ToString());
                tr.Cells.Add(td);
            }
        }

        private static string GenerateExtraTypeScaleRangeText(decimal fromScaleValue, decimal? toScaleValue)
        {
            if (toScaleValue.HasValue && toScaleValue.Value != fromScaleValue)
                return string.Format("{0:0.##} to {1:0.##}", fromScaleValue, toScaleValue);
            
            return fromScaleValue.ToString("0.##");
        }

        #endregion

        #region Data Saving

        private void SaveTariffAndTariffVersion(IUnitOfWork uow)
        {
            var repo = DIContainer.CreateRepository<Repositories.ITariffRepository>(uow);

            var versionID = int.Parse(cboTariffVersion.SelectedValue);
            var version = repo.FindTariffVersion(versionID);

            //Save Tariff changes
            var tariff = version.Tariff;
            tariff.Description = txtTariffDescription.Text;
            tariff.MultiplyByOrderValue = chkMultiplyByOrderValue.Checked;
            tariff.AdditionalMultiplier = chkMultiplyByOrderValue.Checked ? (decimal?)txtAdditionalMultiplier.Value : null;
            tariff.GroupedOrdersRateIndividually = rbGroupedOrderRateIndividually.Checked;
            tariff.RateCombinedWherePointsMatch = tariff.GroupedOrdersRateIndividually && chkRateCombinedWherePointsMatch.Checked;
            tariff.IgnoreAdditionalCollectsFromADeliveryPoint = !tariff.GroupedOrdersRateIndividually && chkIgnoreAdditionalCollectsFromADeliveryPoint.Checked;

            //Save TariffVersion changes
            version.Description = txtVersionDescription.Text;

            //TODO should the user be allowed to change the Zone Map and StartDate?
            version.StartDate = dteStartDate.SelectedDate.Value.Date;
            version.FinishDate = dteFinishDate.SelectedDate.HasValue ? dteFinishDate.SelectedDate.Value.Date : (DateTime?)null;
        }

        private void SaveTariffTable(IUnitOfWork uow)
        {
            if (cboTariffTable.SelectedIndex == -1)
                return;

            var repo = DIContainer.CreateRepository<Repositories.ITariffRepository>(uow);

            var tariffTableID = int.Parse(cboTariffTable.SelectedValue);
            var tariffTable = repo.FindTariffTable(tariffTableID);
                
            //Set the TariffTable columns
            tariffTable.Description = txtTableDescription.Text;
            tariffTable.CollectionZoneID = (cboCollectionZone.SelectedValue.Length > 0) ? int.Parse(cboCollectionZone.SelectedValue) : (int?)null;
            tariffTable.OrderServiceLevelID = (cboServiceLevel.SelectedValue.Length > 0) ? int.Parse(cboServiceLevel.SelectedValue) : (int?)null;
            tariffTable.GoodsTypeID = (cboGoodsType.SelectedValue.Length > 0) ? int.Parse(cboGoodsType.SelectedValue) : (int?)null;
            tariffTable.PalletTypeID = (cboPalletType.SelectedValue.Length > 0) ? int.Parse(cboPalletType.SelectedValue) : (int?)null;
            tariffTable.MultiCollectRate = (decimal)rntxtMultiCollectRate.Value.Value;
            tariffTable.MultiDropRate = (decimal)rntxtMultiDropRate.Value.Value;
            tariffTable.UseGreatestRateForPrimaryRate = chkUseGreatestRateForPrimaryRate.Checked;
        }

        private void SaveTariffRates(IUnitOfWork uow)
        {
            string[] cellIDs = hidTariffRateChanges.Value.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);

            if (!cellIDs.Any() || cboTariffTable.SelectedIndex < 0)
                return;

            var repo = DIContainer.CreateRepository<Repositories.ITariffRepository>(uow);
            
            var tariffTableID = int.Parse(cboTariffTable.SelectedValue);
            var tariffTable = repo.FindTariffTable(tariffTableID);

            foreach (var cellID in cellIDs)
            {
                //The cellID is made up of <GridName>:<ZoneId>:<Pallet>
                string[] parts = cellID.Split(':');

                var zoneID = int.Parse(parts[1]);
                var scaleValueID = int.Parse(parts[2]);

                //Find the TariffRate for the Zone and ScaleValue
                var tariffRate = tariffTable.TariffRates.SingleOrDefault(tr => tr.ScaleValueID == scaleValueID && tr.ZoneID == zoneID);

                //Create a new one if one could not be found
                if (tariffRate == null)
                {
                    tariffRate = new Models.TariffRate { ScaleValueID = scaleValueID, ZoneID = zoneID };
                    tariffTable.TariffRates.Add(tariffRate);
                }

                decimal value = 0;

                if (decimal.TryParse(this.Request.Form[cellID], out value))
                {
                    tariffRate.IsEnabled = true;
                    tariffRate.Rate = value;
                }
                else
                {
                    tariffRate.IsEnabled = false;
                    tariffRate.Rate = 0m;
                }
            }
        }

        private void SaveExtraTypeRates(IUnitOfWork uow)
        {
            var updatedExtraTypes = Newtonsoft.Json.JsonConvert.DeserializeObject<IEnumerable<ExtraTypeWithRates>>(hidExtraTypeRateChanges.Value);

            if (!updatedExtraTypes.Any())
                return;

            var repo = DIContainer.CreateRepository<Repositories.ITariffRepository>(uow);

            var tariffVersionID = int.Parse(cboTariffVersion.SelectedValue);
            var tariffVersion = repo.FindTariffVersion(tariffVersionID);

            foreach (var extraType in updatedExtraTypes)
            {
                var extraTypeID = extraType.extraTypeID;

                foreach (var scaleValue in tariffVersion.Scale.ScaleValues)
                {
                    var updatedRate = extraType.scaleValueRates.FirstOrDefault(svr => svr.scaleValue == scaleValue.Value);

                    if (updatedRate == null || !updatedRate.isDirty)
                        continue;

                    var scaleValueID = scaleValue.ScaleValueID;
                    var rate = tariffVersion.ExtraTypeRates.SingleOrDefault(etr => etr.ExtraTypeID == extraTypeID && etr.ScaleValueID == scaleValueID);

                    if (rate == null)
                    {
                        rate = new Models.ExtraTypeRate { ExtraTypeID = extraTypeID, ScaleValueID = scaleValueID };
                        tariffVersion.ExtraTypeRates.Add(rate);
                    }

                    rate.Rate = updatedRate.rate ?? 0m;
                }
            }
        }

        #endregion

		#region Helper Classes

        private class ExtraTypeWithRates
        {

            public class ScaleValueRate
            {
                public decimal scaleValue { get; set; }
                public decimal? rate { get; set; }
                public bool isDirty { get; set; }
            }

            public int extraTypeID { get; set; }
            public string description { get; set; }
            public IEnumerable<ScaleValueRate> scaleValueRates { get; set; }

        }

 		#endregion Helper Classes

    }

}

