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

    public partial class AddScale : Orchestrator.Base.BasePage
    {

        #region Event Handlers
        
        protected void Page_Init(object sender, EventArgs e)
        {
            btnSave.Click +=new EventHandler(btnSave_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
            this.Title = "Orchestrator - Add Scale";
       }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                LoadMetrics();
                LoadCopyFromScales();

                txtScaleDescription.Focus();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            //If the Copy option is selelected get the TariffVersionId of the selected
            //TariffVersion and use it to copy the Tariff 
            Models.Scale scale;

            //We can't use SelectedIndex to check if a selection has been made because
            //when a combo is AJAX populated Items, SelectedIndex and SelectedItem are not updated
            if (optCopyScale.Checked && cboCopyFromScale.SelectedIndex > -1)
                scale = CreateScale(int.Parse(cboCopyFromScale.SelectedValue));
            else
                scale = CreateScale();

            //Redirect and pass new TariffTableId so that it is selected
            this.Response.Redirect("~/Tariff/EditScale.aspx?ScaleID=" + scale.ScaleID.ToString());
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.Response.Redirect("~/Tariff/ScaleList.aspx");
        }

        #endregion

        #region Data Loading

        private void LoadMetrics()
        {
            var metrics = Enum.GetValues(typeof(eMetric)).Cast<eMetric>().Select(m => new { MetricID = (int)m, Description = m.Humanize() });

            cboMetric.DataValueField = "MetricID";
            cboMetric.DataTextField = "Description";
            cboMetric.DataSource = metrics.ToList();
            cboMetric.DataBind();
        }

        private void LoadCopyFromScales()
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<Repositories.IScaleRepository>(uow);

                var scales =
                    from s in repo.GetAll()
                    orderby s.Description
                    select new { s.ScaleID, s.Description };

                cboCopyFromScale.Items.Clear();
                cboCopyFromScale.DataValueField = "ScaleID";
                cboCopyFromScale.DataTextField = "Description";
                cboCopyFromScale.DataSource = scales.ToList();
                cboCopyFromScale.DataBind();
            }
        }

        #endregion

        #region Data Saving

        private Models.Scale CreateScale(int? copyScaleID = null)
        {
            //Create a new Scale and assign entered values
            var scale = new Models.Scale
            {
                Metric = (eMetric)int.Parse(cboMetric.SelectedValue),
                Description = txtScaleDescription.Text,
                IsEnabled = true,
            };

            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<Repositories.IScaleRepository>(uow);
                repo.Add(scale);

                //Save changes to database
                uow.SaveChanges();

                //Copy the Scale Values
                if (copyScaleID.HasValue)
                    repo.CopyScaleValues(copyScaleID.Value, scale.ScaleID);
            }

            //and return the new Scale with its assigned ScaleID
            return scale;
        }

        #endregion

    }

}
