using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Tariff
{

    public partial class AddTariffVersion : Orchestrator.Base.BasePage
    {

        #region Properties

        private int? _tariffID = null;
        public int TariffID
        {
            get { return (_tariffID = _tariffID ?? int.Parse(this.Request.QueryString["TariffID"])).Value; }
        }

        #endregion

        #region Event Handlers
        
        protected void Page_Init(object sender, EventArgs e)
        {
            btnSave.Click +=new EventHandler(btnSave_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
            this.Title = "Orchestrator - Add Tariff Version";

            cboCopyFromVersion.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(cboCopyFromVersion_ItemsRequested);
       }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                using (var uow = DIContainer.CreateUnitOfWork())
                {
                    var repo = DIContainer.CreateRepository<Repositories.ITariffRepository>(uow);
                    var tariff = this.GetTariff(uow);
                    var previousVersion = this.GetPreviousVersion(tariff);

                    this.LoadTariff(tariff, previousVersion);
                    this.LoadCopyFromTariffs(uow, tariff);
                    this.LoadZoneMaps(uow, tariff, previousVersion);
                    this.LoadScales(uow, tariff, previousVersion);
                }

                txtVersionDescription.Focus();
            }
        }

        private void cboCopyFromVersion_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            int tariffID = 0;
            int.TryParse(e.Text, out tariffID);

            using (var uow = DIContainer.CreateUnitOfWork())
            {
                this.LoadCopyFromVersions(uow, tariffID);
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            //If the Copy option is selelected get the TariffVersionId of the selected
            //TariffVersion and use it to copy the Tariff 
            Models.TariffVersion version;

            if (optCopyVersion.Checked && cboCopyFromVersion.SelectedIndex > -1)
                version = CopyTariffVersion(int.Parse(cboCopyFromVersion.SelectedValue));
            else
                version = CreateEmptyTariffVersion();

            //Redirect to EditTariff so that the new Tariff can be editted
            this.Response.Redirect("~/Tariff/EditTariff.aspx?TariffId=" + this.TariffID.ToString());
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            this.Response.Redirect("~/Tariff/TariffList.aspx");
        }

        #endregion

        #region Data Loading

        private Models.Tariff GetTariff(IUnitOfWork uow)
        {
            var repo = DIContainer.CreateRepository<Repositories.ITariffRepository>(uow);
            return repo.Find(this.TariffID);
        }

        private Models.TariffVersion GetPreviousVersion(Models.Tariff tariff)
        {
            return tariff.TariffVersions.OrderByDescending(tv => tv.StartDate).First();
        }

        private void LoadTariff(Models.Tariff tariff, Models.TariffVersion previousVersion)
        {
            if (!previousVersion.FinishDate.HasValue)
                throw new ApplicationException("The last tariff version has not been finished.");

            lblTariffDescription.Text = tariff.Description;

            //Set the new TariffVersion StartDate to the day after the previous version's FinishDate
            lblStartDate.Text = previousVersion.FinishDate.Value.AddDays(1).ToString("dd/MM/yy");

            //Default the new TariffVersion's Description
            txtVersionDescription.Text = lblTariffDescription.Text + " - " + lblStartDate.Text;
        }

        private void LoadZoneMaps(IUnitOfWork uow, Models.Tariff tariff, Models.TariffVersion previousVersion)
        {
            //Get the ZoneMaps that are relevant for the Tariff's ZoneType and load the combo with them
            var zoneMapRepo = DIContainer.CreateRepository<Repositories.IZoneMapRepository>(uow);
            var zoneMaps = zoneMapRepo.GetForZoneType(tariff.ZoneType).Select(zm => new { zm.ZoneMapID, zm.Description });

            cboZoneMap.DataValueField = "ZoneMapID";
            cboZoneMap.DataTextField = "Description";

            cboZoneMap.DataSource = zoneMaps.ToList();
            cboZoneMap.DataBind();

            //Default to the previous version's ZoneMap
            if (cboZoneMap.Items.Any())
                cboZoneMap.SelectedValue = previousVersion.ZoneMapID.ToString();
        }

        private void LoadScales(IUnitOfWork uow, Models.Tariff tariff, Models.TariffVersion previousVersion)
        {
            //Get the Scales that are relevant for the Tariff's Metric and load the combo with them
            var scaleRepo = DIContainer.CreateRepository<Repositories.IScaleRepository>(uow);
            var scales = scaleRepo.GetForMetric(tariff.Metric).Select(s => new { s.ScaleID, s.Description });

            cboScale.DataValueField = "ScaleID";
            cboScale.DataTextField = "Description";

            cboScale.DataSource = scales.ToList();
            cboScale.DataBind();

            //Default to the previous version's ZoneMap
            if (cboScale.Items.Any())
                cboScale.SelectedValue = previousVersion.ScaleID.ToString();
        }

        private void LoadCopyFromTariffs(IUnitOfWork uow, Models.Tariff tariff)
        {
            var repo = DIContainer.CreateRepository<Repositories.ITariffRepository>(uow);

            var tariffs =
                from t in repo.GetAll()
                orderby t.Description
                select new { t.TariffID, t.Description };

            cboCopyFromTariff.DataValueField = "TariffID";
            cboCopyFromTariff.DataTextField = "Description";
            cboCopyFromTariff.DataSource = tariffs.ToList();
            cboCopyFromTariff.DataBind();

            //Default to the Tariff 
            if (cboCopyFromTariff.Items.Count > 0)
            {
                cboCopyFromTariff.SelectedValue = tariff.TariffID.ToString();
                this.LoadCopyFromVersions(uow, int.Parse(cboCopyFromTariff.SelectedValue));
            }
        }

        private void LoadCopyFromVersions(IUnitOfWork uow, int tariffID)
        {
            if (tariffID > 0)
            {
                var repo = DIContainer.CreateRepository<Repositories.ITariffRepository>(uow);
                var versions = repo.GetTariffVersionsForTariff(tariffID).OrderByDescending(tv => tv.StartDate).Select(tv => new { tv.TariffVersionID, tv.Description });

                cboCopyFromVersion.DataValueField = "TariffVersionID";
                cboCopyFromVersion.DataTextField = "Description";
                cboCopyFromVersion.DataSource = versions.ToList();
                cboCopyFromVersion.DataBind();

                //Default to the latest TariffVersion for the selected Tariff
                if (cboCopyFromVersion.Items.Count > 0)
                    cboCopyFromVersion.SelectedIndex = 0;
            }
            else
                cboCopyFromVersion.Items.Clear();
        }

        #endregion

        #region Data Saving

        private Models.TariffVersion CreateEmptyTariffVersion()
        {
            //Create a new TariffVersion and assign entered values
            var version = new Models.TariffVersion
            {
                Description = txtVersionDescription.Text,
                ZoneMapID = int.Parse(cboZoneMap.SelectedValue),
                ScaleID = int.Parse(cboScale.SelectedValue),
            };

            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var tariff = this.GetTariff(uow);
                var previousVersion = this.GetPreviousVersion(tariff);

                version.StartDate = previousVersion.FinishDate.Value.AddDays(1);

                //Add the new TariffVersion to the Tariff
                tariff.TariffVersions.Add(version);

                //Save changes to database
                uow.SaveChanges();
            }

            //and return the new tariff with its assigned TariffId
            return version;
        }

        private Models.TariffVersion CopyTariffVersion(int tariffVersionID)
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var tariff = this.GetTariff(uow);
                var previousVersion = this.GetPreviousVersion(tariff);

                //Get the TariffVersion to copy from
                var repo = DIContainer.CreateRepository<Repositories.ITariffRepository>(uow);
                var copyFromVersion = repo.FindTariffVersion(tariffVersionID);

                //Create a new TariffVersion and assign entered values
                var version = new Models.TariffVersion
                {
                    StartDate = previousVersion.FinishDate.Value.AddDays(1),
                    Description = txtVersionDescription.Text,
                };

                //Copy the selected version's ZoneMap and Scale
                version.ZoneMap = copyFromVersion.ZoneMap;
                version.Scale = copyFromVersion.Scale;

                //Add the new TariffVersion to the Tariff
                tariff.TariffVersions.Add(version);

                //Save changes to database
                uow.SaveChanges();

                //Call a sproc to copy the selected TariffVersion's TariffTables to the new TariffVersion
                var increaseRate = txtIncreaseRate.Value.HasValue ? (decimal)txtIncreaseRate.Value.Value : 0m;

                repo.TariffVersionCopyTariffTables(
                    copyFromVersion.TariffVersionID,
                    version.TariffVersionID,
                    increaseRate);

                //Return the new tariff with its assigned TariffId
                return version;
            }
        }

        #endregion

    }

}
