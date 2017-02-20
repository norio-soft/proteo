using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;

namespace Orchestrator.WebUI.POD
{

    //------------------------------------------------------------------------------------

    public partial class ImportedPods : Orchestrator.Base.BasePage
    {

        //------------------------------------------------------------------------------------

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                ConfigureDisplay();
        }

        //------------------------------------------------------------------------------------

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnDelete.Click += new EventHandler(btnDelete_Click);
            this.btnDeleteBottom.Click += new EventHandler(btnDelete_Click);
            this.btnMatchRoutine.Click += new EventHandler(btnMatchRoutine_Click);
            this.btnMatchRoutineBottom.Click += new EventHandler(btnMatchRoutine_Click);
            this.btnRefresh.Click += new EventHandler(btnRefresh_Click);
            this.btnRefreshBottom.Click += new EventHandler(btnRefresh_Click);
            this.btnUpdateOrderIds.Click += new EventHandler(btnUpdateOrderIds_Click);
            this.btnUpdateOrdersIdsBottom.Click += new EventHandler(btnUpdateOrderIds_Click);

            this.lvItems.ItemDataBound += new EventHandler<ListViewItemEventArgs>(lvItems_ItemDataBound);
        }

        //------------------------------------------------------------------------------------

        protected void btnUpdateOrderIds_Click(object sender, EventArgs e)
        {
            // find the imported pods marked for deletion
            foreach (ListViewDataItem lv in this.lvItems.Items)
                if (lv.ItemType == ListViewItemType.DataItem)
                {
                    HiddenField isDirty = lv.FindControl("hidRowDirty") as HiddenField;
                    TextBox txtOrder = lv.FindControl("txtOrderId") as TextBox;
                    // we need to grab the chkDelete control as it holds the ImportedPodId
                    System.Web.UI.HtmlControls.HtmlInputCheckBox chkDelete = lv.FindControl("chkDelete") as System.Web.UI.HtmlControls.HtmlInputCheckBox;

                    if (chkDelete != null && isDirty != null && isDirty.Value.ToUpper() == "TRUE")
                    {
                        int importedPodId = 0;
                        int.TryParse(chkDelete.Attributes["ImportedPodId"], out importedPodId);

                        EF.ImportedPod importedPod = (from ip in EF.DataContext.Current.ImportedPodSet
                                                          where ip.ImportedPodId == importedPodId
                                                          select ip).FirstOrDefault();

                        if (importedPod != null)
                        {
                            importedPod.OrderId = txtOrder.Text;
                            importedPod.LastUpdatedDate = DateTime.Now;
                            importedPod.LastUpdatedUserId = Page.User.Identity.Name;
                            EF.DataContext.Current.SaveChanges();
                        }
                    }
                }

            this.RebindGrid();
        }

        //------------------------------------------------------------------------------------

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            this.RebindGrid();
        }

        //------------------------------------------------------------------------------------

        protected void btnMatchRoutine_Click(object sender, EventArgs e)
        {
            const string EVENT_LOG_SOURCE = "ImportedPods";
            const string EVENT_LOG_NAME = "Application";

            if (!System.Diagnostics.EventLog.SourceExists(EVENT_LOG_SOURCE))
                System.Diagnostics.EventLog.CreateEventSource(EVENT_LOG_SOURCE, EVENT_LOG_NAME);

            System.Diagnostics.EventLog log = new System.Diagnostics.EventLog(EVENT_LOG_NAME, ".", EVENT_LOG_SOURCE);

            Facade.ImportedPod facImportedPod = new Facade.ImportedPod();

            if (this.dteImportFromDate.SelectedDate.HasValue && this.dteImportToDate.SelectedDate.HasValue)
            {
                DateTime toDate = this.dteImportToDate.SelectedDate.Value;
                toDate = toDate.AddHours(23);
                toDate = toDate.AddMinutes(59);
                toDate = toDate.AddSeconds(59);
                facImportedPod.MatchImportedPods(log, Page.User.Identity.Name, this.dteImportFromDate.SelectedDate.Value, toDate);
            }
            else
                facImportedPod.MatchImportedPods(log, Page.User.Identity.Name);

            this.RebindGrid();
        }

        //------------------------------------------------------------------------------------

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            // find the imported pods marked for deletion
            foreach (ListViewDataItem lv in this.lvItems.Items)
                if (lv.ItemType == ListViewItemType.DataItem)
                {
                    System.Web.UI.HtmlControls.HtmlInputCheckBox chkDelete = lv.FindControl("chkDelete") as System.Web.UI.HtmlControls.HtmlInputCheckBox;

                    if (chkDelete != null && chkDelete.Checked)
                    {
                        int importedPodId = 0;
                        int.TryParse(chkDelete.Attributes["ImportedPodId"], out importedPodId);

                        EF.ImportedPod importedPod = (from ip in EF.DataContext.Current.ImportedPodSet
                                                      where ip.ImportedPodId == importedPodId
                                                      select ip).FirstOrDefault();

                        if (importedPod != null)
                        {
                            try
                            {
                                // delete the accompanying file
                                if (File.Exists(Path.Combine(importedPod.ImageFolder, importedPod.ImageName)))
                                    File.Delete(Path.Combine(importedPod.ImageFolder, importedPod.ImageName));
                            }
                            catch (System.IO.IOException ioex)
                            { 
                                // we can delete the row from the table even if we couldn't delete the file.
                            }

                            EF.DataContext.Current.DeleteObject(importedPod);
                        }
                    }
                }

            EF.DataContext.Current.SaveChanges();
            this.RebindGrid();
        }

        //------------------------------------------------------------------------------------

        private void RebindGrid()
        {
            DateTime fromDate = this.dteImportFromDate.SelectedDate.Value;
            DateTime toDate = this.dteImportToDate.SelectedDate.Value;
            toDate = toDate.AddHours(23);
            toDate = toDate.AddMinutes(59);
            toDate = toDate.AddSeconds(59);

            List<EF.ImportedPod> importedPods = (from ip in EF.DataContext.Current.ImportedPodSet
                                                 where ip.CreateDate >= fromDate && ip.CreateDate <= toDate
                                                 select ip).ToList();

            if (importedPods != null && importedPods.Count > 0)
            {
                buttonBarBottom.Visible = true;
                btnDelete.Visible = true;
                btnUpdateOrderIds.Visible = true;
                btnMatchRoutine.Visible = true;
            }
            else
            {
                buttonBarBottom.Visible = false;
                btnDelete.Visible = false;
                btnUpdateOrderIds.Visible = false;
                btnMatchRoutine.Visible = false;
            }
            lvItems.DataSource = importedPods;
            lvItems.DataBind();
        }

        //------------------------------------------------------------------------------------

        private void ConfigureDisplay()
        {
            DateTime fromDate = DateTime.Today.AddDays(-7);
            DateTime toDate = DateTime.Today;

            if (this.dteImportFromDate.SelectedDate.HasValue)
                fromDate = this.dteImportFromDate.SelectedDate.Value;
            else
                this.dteImportFromDate.SelectedDate = fromDate;

            if (this.dteImportToDate.SelectedDate.HasValue)
                toDate = this.dteImportToDate.SelectedDate.Value;
            else
                this.dteImportToDate.SelectedDate = toDate;

            this.RebindGrid();
        }

        //------------------------------------------------------------------------------------

        protected void lvItems_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            ListViewDataItem lv = e.Item as ListViewDataItem;
            EF.ImportedPod ip = lv.DataItem as EF.ImportedPod;

            System.Web.UI.HtmlControls.HtmlAnchor imageFile =
                lv.FindControl("ancImage") as System.Web.UI.HtmlControls.HtmlAnchor;

            System.Web.UI.HtmlControls.HtmlInputCheckBox chkDelete =
                lv.FindControl("chkDelete") as System.Web.UI.HtmlControls.HtmlInputCheckBox;

            if (lv.ItemType == ListViewItemType.DataItem)
            {
                // ImportedPods is the virtual directory. if this is not working make sure the virtual directory is present
                if (chkDelete != null && ip != null)
                    chkDelete.Attributes.Add("ImportedPodId", ip.ImportedPodId.ToString());

                if (imageFile != null && ip != null)
                {
                    string virtualPath = Server.MapPath("~/ImportedPods");
                    string actualPath = "/ImportedPods" + Path.Combine(ip.ImageFolder.Replace(virtualPath, ""), ip.ImageName);
                    imageFile.HRef = actualPath;
                }
            }
        }

        //------------------------------------------------------------------------------------
        
    }

    //------------------------------------------------------------------------------------

}
