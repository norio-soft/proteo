using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;


namespace Orchestrator.WebUI.Resource.Trailer
{
    public partial class GPSMappingList : Orchestrator.Base.BasePage
    {

        #region Page Properties
        public DataTable TrailerControllers 
        {
            get
            {
                return (DataTable)this.ViewState["_controllers"];
            }
            set
            {
                this.ViewState["_controllers"] = value;
            }
        }

        private bool LoadTrailers = true;
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.rgTrailers.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(rgTrailers_NeedDataSource);
            this.btnFilter.Click += new EventHandler(btnFilter_Click);
            this.btnReset.Click += new EventHandler(btnReset_Click);
            this.cboFilter.SelectedIndexChanged += new Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventHandler(cboFilter_SelectedIndexChanged);
        }
        
        void cboFilter_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (int.Parse(cboFilter.SelectedValue) == -1)
            {
                ResetFilter();
                return;
            }

            // change the filter option
            mvFilterOptions.SetActiveView(mvFilterOptions.Views[int.Parse(cboFilter.SelectedValue)]);
            mvFilterOptions.Visible = true;
            btnFilter.Visible = true;
            btnReset.Visible = true;

            // if Trailer Controllers Bind the Combo
            if (cboFilter.SelectedValue == "2" )
            {
                cboTrailerController.DataSource = this.TrailerControllers;
                cboTrailerController.DataTextField = "CurrentTrailerController";
                cboTrailerController.DataValueField = "CurrentTrailerController";
                cboTrailerController.DataBind();

            }
        }

        void raxFilterOptions_Load(object sender, EventArgs e)
        {
           
            
        }

        void btnReset_Click(object sender, EventArgs e)
        {
            ResetFilter();
        } 

        void btnFilter_Click(object sender, EventArgs e)
        {
            // filter the Grid based on the selcter filter and value
            LoadTrailers = false;
            rgTrailers.Rebind();
        }

        void upFilterOptions_Load(object sender, EventArgs e)
        {
            
        }

        private void ResetFilter()
        {
            cboFilter.FindItemByValue("-1").Selected = true;
            mvFilterOptions.Visible = false;
            btnFilter.Visible = false;
            btnReset.Visible = false;
            rgTrailers.Rebind();
        }

        void rgTrailers_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            this.rgTrailers.DataSource = GetTrailerData();
        }

        #region Private Methods
        private DataView GetTrailerData()
        {
            Facade.ITrailer facTrailer = new Facade.Resource();
            DataSet dataSet = facTrailer.GetAllWithGPS();
            DataView retVal = dataSet.Tables[0].DefaultView;

            // Get a list of the Trailer Controllers from the dataset
            if (this.TrailerControllers == null)
                TrailerControllers = retVal.ToTable(true, "CurrentTrailerController");
            
            // Do we need to apply any filter?
            if (int.Parse(cboFilter.SelectedValue) > -1)
            {
                if (int.Parse(cboFilter.SelectedValue) == 0)
                {
                    // Filter by GPS Status (Red, Amber, Green)
                    retVal.RowFilter = string.Format("GPSStatus = '{0}'", rblGPSStatus.SelectedValue);
                }
                if (int.Parse(cboFilter.SelectedValue) == 1)
                {
                    // Fulter by GPS Mapping Status (has a device been mapped)
                    retVal.RowFilter = string.Format("GPSUnitID {0}", rblMappingStatus.SelectedValue == "0" ? "Is NULL OR GPSUnitID = ''" : "IS NOT NULL AND GPSUnitID <> ''");
                }

                if (int.Parse(cboFilter.SelectedValue) == 2)
                {
                    // Filter by Trailer Controller
                    retVal.RowFilter = string.Format("CurrentTrailerController= '{0}'", cboTrailerController.SelectedValue);
                }

                if (int.Parse(cboFilter.SelectedValue) == 3)
                {
                    // Filter by Search
                    string search = txtSearch.Text;
                    if (search.StartsWith("T"))
                        search = search.Substring(1);

                    retVal.RowFilter = string.Format("TrailerRef= '{0}' OR GPSUnitID='{0}'", search);
                }

                // Set the record count
                this.litTrailerCount.Text = string.Format("{0} of {1} Trailers",  retVal.Count, dataSet.Tables[0].Rows.Count);
            }
            else
            {
                // Set the record count
                this.litTrailerCount.Text = retVal.Table.Rows.Count  + " Trailers";
            }

            return retVal;
        }
        #endregion

    }
}
