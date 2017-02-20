using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Transactions;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Tariff
{
    public partial class AddTariffTable : Orchestrator.Base.BasePage
    {

        #region Properties

        //Used client side to default cboFromTariffTable
        //This is necessary as using SelectedValue during a callback doesn't work
        protected int? TariffTableID
        {
            get { return (int?)ViewState["TariffTableID"]; }
            private set { ViewState["TariffTableID"] = value; }
        }

        //Used client side to default cboFromTariffVersion
        //This is necessary as using SelectedValue during a callback doesn't work
        protected int TariffVersionID
        {
            get { return (int)ViewState["TariffVersionID"]; }
            private set { ViewState["TariffVersionID"] = value; }
        }

        #endregion

        #region Event Handlers

        protected void Page_Init(object sender, EventArgs e)
        {
            btnSave.Click += new EventHandler(btnSave_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
            this.Title = "Orchestrator - Add Tariff Table";

            cboCopyFromVersion.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(cboCopyFromVersion_ItemsRequested);
            cboCopyFromTable.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(cboCopyFromTable_ItemsRequested);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                using (var uow = DIContainer.CreateUnitOfWork())
                {
                    var repo = DIContainer.CreateRepository<Repositories.ITariffRepository>(uow);

                    //If a TariffTableID was passed use it to get the TariffTable and TarriffVersion
                    //otherwise use the TariffVersionID passed to just get a TariffVersion
                    Models.TariffVersion tariffVersion;

                    if (!string.IsNullOrWhiteSpace(this.Request.QueryString["TariffTableID"]))
                    {
                        this.TariffTableID = int.Parse(this.Request.QueryString["TariffTableID"]);
                        var tariffTable = repo.FindTariffTable(this.TariffTableID.Value);
                        tariffVersion = tariffTable.TariffVersion;
                    }
                    else
                    {
                        int tariffVersionID = int.Parse(this.Request.QueryString["TariffVersionID"]);
                        tariffVersion = repo.FindTariffVersion(tariffVersionID);
                    }

                    this.TariffVersionID = tariffVersion.TariffVersionID;

                    LoadZones(tariffVersion);
                    LoadGoodsTypes();
                    LoadServiceLevels();
                    LoadPalletTypes();
                    LoadTariffVersion(tariffVersion);
                    LoadCopyFromTariffs(tariffVersion);
                }

                cboCollectionZone.Focus();
            }
        }

        protected void cboCopyFromVersion_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            int tariffID = 0;
            int.TryParse(e.Text, out tariffID);

            LoadCopyFromVersions(tariffID);
        }

        protected void cboCopyFromTable_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            int tariffVersionID = 0;
            int.TryParse(e.Text, out tariffVersionID);

            LoadCopyFromTables(tariffVersionID);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            //If the Copy option is selelected get the TariffVersionID of the selected
            //TariffVersion and use it to copy the Tariff 
            Models.TariffTable table = null;

            //We can't use SelectedIndex to check if a selection has been made because
            //when a combo is AJAX populated Items, SelectedIndex and SelectedItem are not updated
            if (optCopyTable.Checked && !string.IsNullOrEmpty(cboCopyFromTable.SelectedValue))
                table = CopyTariffTable(int.Parse(cboCopyFromTable.SelectedValue));
            else if (optEmptyTable.Checked)
                table = CreateEmptyTariffTable();
            else if (optImportTable.Checked)
            {
                try
                {
                    table = ImportRateTable();
                }
                catch (ApplicationException ae)
                {
                    this.lblImportErrorMessage.Text = ae.Message;
                }
            }

            //Redirect and pass new TariffTableID so that it is selected
            if (table != null)
                this.Response.Redirect("~/Tariff/EditTariff.aspx?TariffTableID=" + table.TariffTableID.ToString());
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.Response.Redirect("~/Tariff/TariffList.aspx");
        }

        #endregion

        #region Data Loading

        private void LoadZones(Models.TariffVersion tariffVersion)
        {
            var zones =
                from z in tariffVersion.ZoneMap.Zones
                orderby z.Description
                select new { z.ZoneID, z.Description };

            cboCollectionZone.DataValueField = "ZoneID";
            cboCollectionZone.DataTextField = "Description";
            cboCollectionZone.DataSource = zones.ToList();
            cboCollectionZone.DataBind();
            cboCollectionZone.Items.Insert(0, new RadComboBoxItem("Any"));
        }

        private void LoadGoodsTypes()
        {
            cboGoodsType.DataValueField = "GoodsTypeId";
            cboGoodsType.DataTextField = "Description";
            cboGoodsType.DataSource = Facade.GoodsType.GetAllActiveGoodsTypes();
            cboGoodsType.DataBind();
            cboGoodsType.Items.Insert(0, new RadComboBoxItem("Any"));
        }

        private void LoadServiceLevels()
        {
            Facade.IOrderServiceLevel facServiceLevel = new Facade.Order();

            cboServiceLevel.DataValueField = "OrderServiceLevelId";
            cboServiceLevel.DataTextField = "Description";
            cboServiceLevel.DataSource = facServiceLevel.GetAll();
            cboServiceLevel.DataBind();
            cboServiceLevel.Items.Insert(0, new RadComboBoxItem("Any"));
        }

        private void LoadPalletTypes()
        {
            cboPalletType.DataValueField = "PalletTypeId";
            cboPalletType.DataTextField = "Description";
            cboPalletType.DataSource = Facade.PalletType.GetAllPalletTypes();
            cboPalletType.DataBind();
            cboPalletType.Items.Insert(0, new RadComboBoxItem("Any"));
        }

        private void LoadTariffVersion(Models.TariffVersion tariffVersion)
        {
            lblTariffDescription.Text = tariffVersion.Tariff.Description;
            lblVersionDescription.Text = tariffVersion.Description;
            lblStartDate.Text = tariffVersion.StartDate.ToString("dd/MM/yy");
        }

        private void LoadCopyFromTariffs(Models.TariffVersion tariffVersion)
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<Repositories.ITariffRepository>(uow);

                var tariffs =
                    from t in repo.GetAll()
                    orderby t.Description
                    select new { t.TariffID, t.Description };

                cboCopyFromTariff.Items.Clear();
                cboCopyFromTariff.DataValueField = "TariffID";
                cboCopyFromTariff.DataTextField = "Description";
                cboCopyFromTariff.DataSource = tariffs.ToList();
                cboCopyFromTariff.DataBind();
            }

            //Default to the Tariff of the TariffVersion passed 
            if (cboCopyFromTariff.Items.Count > 0)
                cboCopyFromTariff.SelectedValue = tariffVersion.TariffID.ToString();

            //LoadCopyFromVersions should NOT be called here as a bug with the RadCombo means that if
            //it is later loaded from the requestItems AJAX call, and the user does not chnage 
            //the default (first) selection, the value of SelectedValue will be incorrect.
        }

        private void LoadCopyFromVersions(int tariffID)
        {
            cboCopyFromVersion.Items.Clear();

            if (tariffID > 0)
            {
                using (var uow = DIContainer.CreateUnitOfWork())
                {
                    var repo = DIContainer.CreateRepository<Repositories.ITariffRepository>(uow);

                    var versions =
                        from tv in repo.GetTariffVersionsForTariff(tariffID)
                        orderby tv.StartDate descending
                        select new { tv.TariffVersionID, tv.Description };

                    cboCopyFromVersion.Items.Clear();
                    cboCopyFromVersion.DataValueField = "TariffVersionID";
                    cboCopyFromVersion.DataTextField = "Description";
                    cboCopyFromVersion.DataSource = versions.ToList();
                    cboCopyFromVersion.DataBind();
                }

                //Default to the TariffVersion passed
                if (cboCopyFromVersion.Items.Count > 0)
                {
                    var tariffVersionID = this.TariffVersionID.ToString();

                    if (cboCopyFromVersion.Items.FindItemByValue(tariffVersionID) != null)
                        cboCopyFromVersion.SelectedValue = tariffVersionID;
                    else
                        cboCopyFromVersion.SelectedIndex = 0;
                }
            }
        }

        private void LoadCopyFromTables(int tariffVersionID)
        {
            cboCopyFromTable.Items.Clear();

            if (tariffVersionID <= 0)
                return;

            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<Repositories.ITariffRepository>(uow);

                var tables =
                    from tt in repo.GetTariffTablesForTariffVersion(tariffVersionID)
                    where tt.TariffVersion.TariffVersionID == tariffVersionID
                    orderby tt.Description descending
                    select new { tt.TariffTableID, tt.Description };

                cboCopyFromTable.DataValueField = "TariffTableID";
                cboCopyFromTable.DataTextField = "Description";
                cboCopyFromTable.DataSource = tables.ToList();
                cboCopyFromTable.DataBind();
            }

            //Default to the TariffTable passed if one was
            if (cboCopyFromTable.Items.Count > 0)
            {
                cboCopyFromTable.SelectedIndex = 0;

                if (this.TariffTableID.HasValue && cboCopyFromTable.FindItemByValue(this.TariffTableID.ToString()) != null)
                    cboCopyFromTable.SelectedValue = TariffTableID.ToString();
            }
        }

        #endregion

        #region Data Saving

        private Models.TariffTable ImportRateTable()
        {
            //Create a new TariffVersion and assign entered values
            var table = this.PopulateEmptyTariffTable();

            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<Repositories.ITariffRepository>(uow);
                var tariffVersion = repo.FindTariffVersion(this.TariffVersionID);

                //Add the new Tariff table to the Tariff Version
                tariffVersion.TariffTables.Add(table);

                //////////////////////////////////////////////////////
                // Insert the rates here

                var scaleValues = tariffVersion.Scale.ScaleValues.OrderBy(sv => sv.Value).Select(sv => new { sv.ScaleValueID, sv.Value }).ToList();
                var fieldCount = 0;

                using (var sr = new StringReader(txtImportRateTable.Text))
                {
                    var line = string.Empty;

                    while (!string.IsNullOrEmpty(line = sr.ReadLine()))
                    {
                        // File will be tab delimited.
                        string[] fields = line.Split("\t".ToCharArray());
                        fieldCount = fields.Length;

                        if ((fieldCount - 1) != scaleValues.Count)
                            throw new ApplicationException("Column count and scale count mismatch. You must provide a column of data for each scale value.");

                        string zoneDescription = fields[0];
                        var zone = tariffVersion.ZoneMap.Zones.FirstOrDefault(z => z.Description == zoneDescription);

                        if (zone == null)
                        {
                            // if the zone description starts with a number then add a leading "0" and try and find it again
                            if (char.IsDigit(zoneDescription.Substring(0, 1), 0))
                            {
                                for (var i = 0; i < 2; i++)
                                {
                                    zoneDescription = string.Format("0{0}", zoneDescription);
                                    zone = tariffVersion.ZoneMap.Zones.FirstOrDefault(z => z.Description == zoneDescription);

                                    if (zone != null)
                                        break;
                                }
                            }

                            if (zone == null)
                                throw new ApplicationException(string.Format("Zone {0} cannot be found for the selected tariff table.", zoneDescription));
                        }

                        for (var iCol = 1; iCol < fieldCount; iCol++)
                        {
                            // Check the zone exists.
                            // Get the scale value to update if there is one, else create it.
                            var scaleValue = scaleValues[iCol - 1];
                            var rate = 0m;
                            var isNumeric = decimal.TryParse(RemoveSpacesAndCurrencySymbol(fields[iCol]), out rate);

                            if (!isNumeric)
                                throw new ApplicationException(string.Format("The rate: {0} is not a valid number.", fields[iCol]));

                            var tariffRate = new Models.TariffRate { ZoneID = zone.ZoneID, ScaleValueID = scaleValue.ScaleValueID, Rate = rate, IsEnabled = true };
                            table.TariffRates.Add(tariffRate);
                        }
                    }
                }

                //
                //////////////////////////////////////////////////////

                //Save changes to database
                uow.SaveChanges();
            }

            //and return the new tariff with its assigned TariffId
            return table;
        }

        private string RemoveSpacesAndCurrencySymbol(string field)
        {
            return field.Replace("£", "").Replace("€", "").Trim();
        }

        private Models.TariffTable CreateEmptyTariffTable()
        {
            //Create a new TariffVersion and assign entered values
            var table = this.PopulateEmptyTariffTable();

            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<Repositories.ITariffRepository>(uow);
                var tariffVersion = repo.FindTariffVersion(this.TariffVersionID);

                //Add the new TariffVersion to the Tariff
                tariffVersion.TariffTables.Add(table);

                //Save changes to database
                uow.SaveChanges();
            }

            //and return the new tariff with its assigned TariffId
            return table;
        }

        private Models.TariffTable CopyTariffTable(int tariffTableID)
        {
            //Create a new TariffVersion and assign entered values
            var table = this.PopulateEmptyTariffTable();

            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<Repositories.ITariffRepository>(uow);

                //Get the TariffTable to copy from
                var copyFromTable = repo.FindTariffTable(tariffTableID);
                table.RateDecimalPlaces = copyFromTable.RateDecimalPlaces;

                //Add the new Tariff table to the Tariff Version
                var tariffVersion = repo.FindTariffVersion(this.TariffVersionID);
                tariffVersion.TariffTables.Add(table);

                //Save changes to database
                uow.SaveChanges();

                //Call a sproc to copy the selected TariffTable's TariffRates to the new TariffVersion
                var increaseRate = txtIncreaseRate.Value.HasValue ? (decimal)txtIncreaseRate.Value.Value : 0m;

                repo.TariffTableCopyTariffRates(
                    tariffTableID,
                    table.TariffTableID,
                    increaseRate);
            }

            //and return the new TariffTable with its assigned TariffTableID
            return table;
        }

        private Models.TariffTable PopulateEmptyTariffTable()
        {
            var table = new Models.TariffTable
            {
                RateDecimalPlaces = 2,
                Description = txtTableDescription.Text,
                MultiCollectRate = (decimal)rntxtMultiCollectRate.Value,
                MultiDropRate = (decimal)rntxtMultiDropRate.Value,
                UseGreatestRateForPrimaryRate = chkUseGreatestRateForPrimaryRate.Checked,
            };

            if (cboCollectionZone.SelectedIndex > 0)
                table.CollectionZoneID = int.Parse(cboCollectionZone.SelectedValue);

            if (cboServiceLevel.SelectedIndex > 0)
                table.OrderServiceLevelID = int.Parse(cboServiceLevel.SelectedValue);

            if (cboPalletType.SelectedIndex > 0)
                table.PalletTypeID = int.Parse(cboPalletType.SelectedValue);

            if (cboGoodsType.SelectedIndex > 0)
                table.GoodsTypeID = int.Parse(cboGoodsType.SelectedValue);

            return table;
        }

        #endregion

    }

}
