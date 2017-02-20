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

    public partial class EditScale : Orchestrator.Base.BasePage
    {

        #region Properties

        public int ScaleID
        {
            get { return int.Parse(this.Request.QueryString["ScaleID"]); }
        }

        #endregion

        #region Event Handlers

        protected void Page_Init(object sender, EventArgs e)
        {
            this.Title = "Orchestrator - Scale";
            btnSave.Click += btnSave_Click;
            btnScaleList.Click += btnScaleList_Click;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.LoadScale();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            this.SaveScale();
        }

        protected void btnScaleList_Click(object sender, EventArgs e)
        {
            this.Response.Redirect("~/Tariff/ScaleList.aspx");
        }

        #endregion

        #region Data Loading

        private void LoadScale()
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<Repositories.IScaleRepository>(uow);
                var scale = repo.Find(this.ScaleID);

                txtScaleDescription.Text = scale.Description;
                chkIsEnabled.Checked = scale.IsEnabled;
                lblMetric.Text = scale.Metric.Humanize();

                this.LoadScaleValues(scale);
            }
        }

        private void LoadScaleValues(Models.Scale scale)
        {
            var inputSetting = (NumericTextBoxSetting)rimScaleValues.GetSettingByBehaviorID("ScaleValues");

            switch (scale.Metric)
            {
                case eMetric.PalletSpaces:
                    inputSetting.Type = NumericType.Number;
                    inputSetting.DecimalDigits = 2;
                    break;

                case eMetric.Weight:
                    inputSetting.Type = NumericType.Number;
                    inputSetting.DecimalDigits = 0;
                    break;
            }

            //Reset the hidden field that holds the selected cellIds
            hidValueChanges.Value = string.Empty;

            HtmlTableRow tr = null;
            HtmlTableCell td = null;

            var scaleValues = scale.ScaleValues.OrderBy(sv => sv.Value).Select(sv => new { sv.ScaleValueID, sv.Value }).ToList();

            //For each Scale Value
            foreach (var scaleValue in scaleValues)
            {
                tr = new HtmlTableRow();
                tr.Attributes.Add("class", "GridRow_Orchestrator");
                grdValues.Rows.Add(tr);

                //Add a cell for the Value
                td = new HtmlTableCell();
                tr.Cells.Add(td);

                //Create a new input box and add it to the cell
                var input = new TextBox
                {
                    ID = string.Format("cell:{0}", scaleValue.ScaleValueID),
                    Text = scaleValue.Value.ToString(),
                };

                td.Controls.Add(input);

                //Each input box will call a function when its value is changed so that 
                //the cell colour can be changed and the id of th input recorded in the hidden field
                input.Attributes.Add("onchange", "Value_onchange(this);");
            }

            int newValues = 10 - scaleValues.Count;

            if (newValues <= 0)
                newValues = 3;

            for (var i = 0; i < newValues; i++)
            {
                tr = new HtmlTableRow();
                tr.Attributes.Add("class", "GridRow_Orchestrator");
                grdValues.Rows.Add(tr);

                //Add a cell for the Scale Value
                td = new HtmlTableCell();
                tr.Cells.Add(td);

                //Create a new input box and add it to the cell
                //Set the cells id so that it indicates to Zone and Pallets
                var input = new TextBox { ID = string.Format("new:{0}", i) };
                td.Controls.Add(input);

                //Each input box will call a function when its value is chnaged so that 
                //the cell colour can be changed and the id of th input recorded in the hidden field
                input.Attributes.Add("onchange", "Value_onchange(this);");
            }
        }

        #endregion

        #region Data Saving

        private void SaveScale()
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<Repositories.IScaleRepository>(uow);
                var scale = repo.Find(this.ScaleID);

                //Set the Scale's Description and IsEnabled
                scale.Description = txtScaleDescription.Text;
                scale.IsEnabled = chkIsEnabled.Checked;

                //Get a list of the Zone descriptions that were changed
                string[] cellIds = hidValueChanges.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                //For each changed Zone description
                foreach (var cellId in cellIds)
                {
                    //The cellId is cell:<ZoneId> or new:<n>
                    string[] parts = cellId.Split(':');
                    var isNew = parts[0].Contains("new");
                    var scaleValueID = int.Parse(parts[1]);

                    //A bug in the Telerik RadInputManager means that if a 0 is entered it is not passed back.
                    //Therefore the value is now added to the hidValueChanges rather than relying on the form variables
                    var newValue = string.IsNullOrWhiteSpace(parts[2]) ? (decimal?)null : decimal.Parse(parts[2]);

                    Models.ScaleValue scaleValue;

                    if (isNew)
                    {
                        if (newValue.HasValue)
                        {
                            scaleValue = new Models.ScaleValue { Value = newValue.Value };
                            scale.ScaleValues.Add(scaleValue);
                        }
                    }
                    else
                    {
                        //TODO What if a TariffTable is already using it?
                        scaleValue = scale.ScaleValues.Single(sv => sv.ScaleValueID == scaleValueID);

                        if (newValue.HasValue)
                            scaleValue.Value = newValue.Value;
                        else
                            repo.RemoveScaleValue(scaleValue);
                    }
                }

                try
                {
                    uow.SaveChanges();
                }
                catch (Exception ex)
                {
                    var isDuplicate = false;

                    do
                    {
                        if (ex.Message.Contains("IX_tblScaleValue_ScaleIdValue"))
                        {
                            isDuplicate = true;
                            break;
                        }

                        ex = ex.InnerException;
                    }
                    while (ex != null);

                    if (isDuplicate)
                        this.MessageBox("You cannot have the same value twice.");
                    else
                        throw;
                }

                this.LoadScaleValues(scale);
            }
        }

        #endregion

    }

}
