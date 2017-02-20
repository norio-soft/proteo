using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Orchestrator.Repositories;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.mwf
{
    public partial class SearchMwfPhotos : System.Web.UI.Page
    {
        bool HasPassedInParams;
        
        /// <summary>
        /// Perform the search either on postback or if either an OrderId or InstructionId query string has been passed in.
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            HasPassedInParams = false;

            if (!IsPostBack)
            {
                PopulateDriverDropdown();

                if (!string.IsNullOrEmpty(Request.QueryString["OrderId"]) || !string.IsNullOrEmpty(Request.QueryString["InstructionId"]))
                {
                    HasPassedInParams = true;
                    rntbOrderId.Text = Request.QueryString["OrderId"];
                    rntbInstructionId.Text = Request.QueryString["InstructionId"];
                    this.grdPhotos.Rebind();
                }

            }
        }

        /// <summary>
        /// Call overridden OnInit method
        /// </summary>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.grdPhotos.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdPhotos_NeedDataSource);
            this.grdPhotos.ItemCommand += new GridCommandEventHandler(grdPhotos_ItemCommand);
            this.btnSearch.Click += new EventHandler(btnSearch_Click);
        }

        /// <summary>
        /// Return an anchor link to navigate to the photo image
        /// </summary>
        protected string GetPhotoImageLink(string photoFileName)
        {
            string fullPathToImage = Path.Combine(Globals.Configuration.TTPhotoStorageLocation, photoFileName);
            Uri imageUrl = new Uri(fullPathToImage);
            string imgHtml = "<a href=\"javascript:openNewPopupWindow('photo','{0}', 900, 900, 'yes')\"><img src=\"{1}\" border=0 height=60 width=80 ></a>";
            return string.Format(imgHtml, imageUrl.LocalPath, fullPathToImage);
        }

        /// <summary>
        /// Query the database for photos based on the search parameters
        /// </summary>
        protected void grdPhotos_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            if (IsPostBack || HasPassedInParams)
            {
                using (var uow = DIContainer.CreateUnitOfWork())
                {
                    var repo = DIContainer.CreateRepository<Repositories.IPhotoRepository>(uow);

                    int? orderId = null;
                    if (rntbOrderId.Value.HasValue)
                        orderId = Convert.ToInt32(rntbOrderId.Value);
                    
                    int? runId = null;
                    if (rntbRunId.Value.HasValue)
                        runId = Convert.ToInt32(rntbRunId.Value);

                    int? instructionId = null;
                    if (rntbInstructionId.Value.HasValue)
                        instructionId = Convert.ToInt32(rntbInstructionId.Value);

                    DateTime? photoDate = dtePhotoDate.SelectedDate;

                    int? driverId = null;
                    if (cboDriver.SelectedIndex > 0)
                        driverId = Convert.ToInt32(cboDriver.SelectedValue);
                    
                    PhotoType? photoType = null;
                    if (cboPhotoType.SelectedIndex > 0)
                        photoType = (PhotoType)Convert.ToInt32(cboPhotoType.SelectedValue);

                    var photos = repo.GetForFindPhoto(orderId, runId, instructionId, photoDate, driverId, photoType);

                    grdPhotos.DataSource = photos.ToList();
                }
            }
        }

        /// <summary>
        /// Perform the button clcik for the search button
        /// </summary>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            this.grdPhotos.Rebind();
        }

        /// <summary>
        /// Remove a photo from the database
        /// </summary>
        protected void grdPhotos_ItemCommand(object source, GridCommandEventArgs e)
        {
            switch (e.CommandName.ToLower())
            {
                case "remove":
                    int photoID = int.Parse(((GridDataItem)e.Item).GetDataKeyValue("PhotoId").ToString());
 
                    using (var uow = DIContainer.CreateUnitOfWork())
                    {
                        var repo = DIContainer.CreateRepository<Repositories.IPhotoRepository>(uow);
                        Uri uri = new Uri(Globals.Configuration.TTPhotoStorageLocation);
                        repo.DeletePhoto(photoID, Server.MapPath(uri.AbsolutePath));
                    }

                    this.grdPhotos.Rebind();
                
                break;
            }
        }

        /// <summary>
        /// Populate the driver dropdown with all drivers (i.e. including deleted drivers)
        /// </summary>
        private void PopulateDriverDropdown()
        {
            Facade.IDriver driver = new Facade.Resource();

            cboDriver.Items.Clear();

            System.Data.DataSet dsDrivers = driver.GetAllDrivers(true);
            cboDriver.DataSource = dsDrivers;
            cboDriver.DataTextField = "FullName";
            cboDriver.DataValueField = "IdentityId";
            cboDriver.DataBind();

            cboDriver.Items.Insert(0, new RadComboBoxItem("- Select -", "-1"));
        }

    }
}