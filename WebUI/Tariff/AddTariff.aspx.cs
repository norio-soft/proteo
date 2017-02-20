using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Humanizer;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Tariff
{

    public partial class AddTariff : Orchestrator.Base.BasePage
    {

        #region Event Handlers
        
        protected void Page_Init(object sender, EventArgs e)
        {
            btnSave.Click +=new EventHandler(btnSave_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
            optZoneType.SelectedIndexChanged += new EventHandler(optZoneType_SelectedIndexChanged);
            optMetric.SelectedIndexChanged += new EventHandler(optMetric_SelectedIndexChanged);

            cboCopyFromVersion.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(cboCopyFromVersion_ItemsRequested);

            //Hook up the client side events to update the TariffVersion Description
            //when the Tariff Description or Start Date are changed
            txtTariffDescription.Attributes.Add("onblur", "updateVersionDescription();");
            dteStartDate.ClientEvents.OnValueChanged = "updateVersionDescription";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                using (var uow = DIContainer.CreateUnitOfWork())
                {
                    LoadCopyFromTariffs(uow);
                    LoadZoneTypes(uow);
                    LoadMetrics(uow);
                }

                dteStartDate.SelectedDate = DateTime.Now.Date;

                txtTariffDescription.Focus();
            }

        }

        private void cboCopyFromVersion_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            int tariffID = 0;
            int.TryParse(e.Text, out tariffID);

            using (var uow = DIContainer.CreateUnitOfWork())
            {
                LoadCopyFromVersions(uow, tariffID);
            }
        }

        private void optZoneType_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Filter the Zone Maps on the Type selected
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                LoadZoneMaps(uow, (eZoneType)int.Parse(optZoneType.SelectedValue));
            }
        }

        void optMetric_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Filter the Scales on the Metric selected
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                LoadScales(uow, (eMetric)int.Parse(optMetric.SelectedValue));
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            //If the Copy option is selelected get the TariffVersionID of the selected
            //TariffVersion and use it to copy the Tariff 
            Models.Tariff tariff;

            //SelectedIndex and SelectedItem cannot be relied upon for combos loaded by AJAX
            //so always use SelectedValue
            if (optCopyTariff.Checked && cboCopyFromVersion.SelectedValue.Length > 0)
                tariff = CopyTariffVersion(int.Parse(cboCopyFromVersion.SelectedValue));
            else
                tariff = CreateEmptyTariff();

            //Redirect to EditTariff so that the new Tariff can be edited
            this.Response.Redirect("~/Tariff/EditTariff.aspx?TariffID=" + tariff.TariffID.ToString());
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            this.Response.Redirect("~/Tariff/TariffList.aspx");
        }

        #endregion

        #region Data Loading

        private void LoadZoneTypes(IUnitOfWork uow)
        {
            var zoneTypes = Enum.GetValues(typeof(eZoneType)).Cast<eZoneType>().Select(zt => new { ZoneTypeID = (int)zt, Description = zt.Humanize() });

            optZoneType.DataValueField = "ZoneTypeID";
            optZoneType.DataTextField = "Description";

            optZoneType.DataSource = zoneTypes.ToList();
            optZoneType.DataBind();

            //Default to first Zone Type (Postcode) and load
            // the Zone Maps for that Type
            if (optZoneType.Items.Count > 0)
            {
                optZoneType.SelectedIndex = 0;
                LoadZoneMaps(uow, (eZoneType)int.Parse(optZoneType.SelectedValue));
            }
        }

        private void LoadZoneMaps(IUnitOfWork uow, eZoneType zoneType)
        {
            var repo = DIContainer.CreateRepository<Repositories.IZoneMapRepository>(uow);
            var zoneMaps = repo.GetForZoneType(zoneType).Select(zm => new { zm.ZoneMapID, zm.Description });

            cboZoneMap.DataValueField = "ZoneMapID";
            cboZoneMap.DataTextField = "Description";

            cboZoneMap.DataSource = zoneMaps.ToList();
            cboZoneMap.DataBind();
        }

        private void LoadMetrics(IUnitOfWork uow)
        {
            var metrics = Enum.GetValues(typeof(eMetric)).Cast<eMetric>().Select(m => new { MetricID = (int)m, Description = m.Humanize() });

            optMetric.DataValueField = "MetricID";
            optMetric.DataTextField = "Description";

            optMetric.DataSource = metrics.ToList();
            optMetric.DataBind();

            //Default to first Metric (Pallets)
            if (optMetric.Items.Count > 0)
            {
                optMetric.SelectedIndex = 0;
                LoadScales(uow, (eMetric)int.Parse(optMetric.SelectedValue));
            }
        }

        private void LoadScales(IUnitOfWork uow, eMetric metric)
        {
            var repo = DIContainer.CreateRepository<Repositories.IScaleRepository>(uow);
            var scales = repo.GetForMetric(metric).Select(s => new { s.ScaleID, s.Description });

            cboScale.DataValueField = "ScaleID";
            cboScale.DataTextField = "Description";

            cboScale.DataSource = scales.ToList();
            cboScale.DataBind();
        }

        private void LoadCopyFromTariffs(IUnitOfWork uow)
        {
            var repo = DIContainer.CreateRepository<Repositories.ITariffRepository>(uow);
            
            var tariffs =
                from t in repo.GetAll()
                orderby t.Description
                select new { TariffID = t.TariffID, Description = t.Description };

            cboCopyFromTariff.DataValueField = "TariffID";
            cboCopyFromTariff.DataTextField = "Description";
            cboCopyFromTariff.DataSource = tariffs.ToList();
            cboCopyFromTariff.DataBind();
        }

        private void LoadCopyFromVersions(IUnitOfWork uow, int tariffID)
        {
            cboCopyFromVersion.Items.Clear();

            if (tariffID > 0)
            {
                var repo = DIContainer.CreateRepository<Repositories.ITariffRepository>(uow);

                var versions =
                    from tv in repo.GetTariffVersionsForTariff(tariffID)
                    orderby tv.StartDate descending
                    select new { tv.TariffVersionID, tv.Description };

                cboCopyFromVersion.DataValueField = "TariffVersionID";
                cboCopyFromVersion.DataTextField = "Description";
                cboCopyFromVersion.DataSource = versions.ToList();
                cboCopyFromVersion.DataBind();
            }
        }

        #endregion

        #region Data Saving

        private Models.Tariff CreateEmptyTariff()
        {
            //Create a new Tariff and assign entered values
            var tariff = new Models.Tariff
            {
                Description = txtTariffDescription.Text,
                IsForSubContractor = chkIsForSubContractor.Checked,
                ZoneType = (eZoneType)int.Parse(optZoneType.SelectedValue),
                Metric = (eMetric)int.Parse(optMetric.SelectedValue),
                MultiplyByOrderValue = chkMultiplyByOrderValue.Checked,
                AdditionalMultiplier = (chkMultiplyByOrderValue.Checked && txtAdditionalMultiplier.Value > 0) ? (decimal?)txtAdditionalMultiplier.Value : null,
                IgnoreAdditionalCollectsFromADeliveryPoint = chkIgnoreAdditionalCollectsFromADeliveryPoint.Checked,
            };

            //Create a new TariffVersion and assign entered values
            var version = new Models.TariffVersion
            {
                StartDate = dteStartDate.SelectedDate.Value.Date,
                Description = txtVersionDescription.Text,
                ZoneMapID = int.Parse(cboZoneMap.SelectedValue),
                ScaleID = int.Parse(cboScale.SelectedValue),
            };

            //Add the new TariffVersion to the new Tariff
            tariff.TariffVersions.Add(version);

            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<Repositories.ITariffRepository>(uow);
                repo.Add(tariff);

                //Save changes to database
                uow.SaveChanges();
            }

            //and return the new tariff with its assigned TariffID
            return tariff;
        }

        private Models.Tariff CopyTariffVersion(int tariffVersionID)
        {
            //Create a new Tariff and assign entered values
            var tariff = new Models.Tariff
            {
                Description = txtTariffDescription.Text,
                IsForSubContractor = chkIsForSubContractor.Checked,
            };

            //Create a new TariffVersion and assign entered values
            var version = new Models.TariffVersion
            {
                StartDate = dteStartDate.SelectedDate.Value.Date,
                Description = txtVersionDescription.Text,
            };

            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<Repositories.ITariffRepository>(uow);

                //Get the TariffVersion to copy from
                var copyFromVersion = repo.FindTariffVersion(tariffVersionID);

                //Copy the ZoneType, Metric and Multiplier values from the selected Tariff
                var copyFromTariff = copyFromVersion.Tariff;
                tariff.ZoneType = copyFromTariff.ZoneType;
                tariff.Metric = copyFromTariff.Metric;
                tariff.MultiplyByOrderValue = copyFromTariff.MultiplyByOrderValue;
                tariff.AdditionalMultiplier = copyFromTariff.AdditionalMultiplier;
                tariff.IgnoreAdditionalCollectsFromADeliveryPoint = copyFromTariff.IgnoreAdditionalCollectsFromADeliveryPoint;

                //Add the new Tariff
                repo.Add(tariff);

                //Use the selected version's ZoneMap and Scale
                version.ZoneMap = copyFromVersion.ZoneMap;
                version.Scale = copyFromVersion.Scale;

                //Add the new TariffVersion to the new Tariff
                tariff.TariffVersions.Add(version);

                //Save changes to database
                uow.SaveChanges();

                //Call a sproc to copy the selected TariffVersion's TariffTables to the new TariffVersion (including ExtraTypeRates)
                var increaseRate = txtIncreaseRate.Value.HasValue ? (decimal)txtIncreaseRate.Value.Value : 0m;

                repo.TariffVersionCopyTariffTables(
                    copyFromVersion.TariffVersionID,
                    version.TariffVersionID,
                    increaseRate);
            }

            //Return the new tariff with its assigned TariffID
            return tariff;
        }

        #endregion

    }

}
