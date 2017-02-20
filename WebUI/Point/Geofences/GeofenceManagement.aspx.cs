using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using Orchestrator.WebUI.Services;
using Telerik.Web.UI;
using System.Web.UI.HtmlControls;

namespace Orchestrator.WebUI.Point.Geofences
{
    public partial class GeofenceManagement : System.Web.UI.Page
    {
        #region Properties
        private int PointID
        {
            get
            {
                return int.Parse(this.ViewState["_pointID"].ToString());
            }

            set{
                this.ViewState["_pointID"] = value;
            }
        }
        private int NotificationID
        {
            get
            {
                return int.Parse(this.ViewState["_notificationID"].ToString());
            }

            set
            {
                this.ViewState["_notificationID"] = value;
            }
        }

        public string HereMapsCoreJS
        {
            get { return Properties.Settings.Default.HereMapsJSCoreUrl; }
        }

        public string HereMapsServiceJS
        {
            get { return Properties.Settings.Default.HereMapsJSServiceUrl; }
        }

        public string HereMapsEventsJS
        {
            get { return Properties.Settings.Default.HereMapsJSMapEventsUrl; }
        }

        public string HereMapsUIJS
        {
            get { return Properties.Settings.Default.HereMapsJSUIUrl; }
        }

        public string HereMapsUICSS
        {
            get { return Properties.Settings.Default.HereMapsJSUICSSUrl; }
        }

        public string HereMapsClusteringJS
        {
            get { return Properties.Settings.Default.HereMapsJSClusteringUrl; }
        }

        public string HereMapsApplicationCode
        {
            get { return Properties.Settings.Default.HereMapsApplicationCode; }
        }

        public string HereMapsApplicationId
        {
            get { return Properties.Settings.Default.HereMapsApplicationId; }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int pointID = -1;
                if (int.TryParse(Request.QueryString["pointid"], out pointID))
                    this.PointID = pointID;

                var point = LoadPoint(pointID);
                lblPointDescription.Text = point.Description;
                lblPointID.Text = pointID.ToString();
            }

        
            #region // set up the Week Active Period Dialog Editor
            TableRow tr = new TableRow();

            TableCell tc = new TableCell();
            tc.Text = "&nbsp;";
            tr.Cells.Add(tc);
            tc = new TableCell();
            tc.Text = "&nbsp;";
            tr.Cells.Add(tc);

            for (int i = 0; i < 24; i++)
            {
                tc = new TableCell();
                tc.Text = i.ToString("00");
                tc.ColumnSpan = 2;
                tr.Cells.Add(tc);
            }

            tc = new TableCell();
            tc.Text = "&nbsp;";
            tr.Cells.Add(tc);

            tc = new TableCell();
            tc.RowSpan = 2;
            System.Web.UI.HtmlControls.HtmlButton button = new HtmlButton();
            button.InnerText = "All";
            tc.Attributes.Add("onclick", "SetRowAll(true);return false;");
            tc.Controls.Add(button);
            tr.Cells.Add(tc);

            tc = new TableCell();
            tc.RowSpan = 2;
            button = new HtmlButton();
            button.InnerText = "None";
            tc.Attributes.Add("onclick", "SetRowAll(false);return false;");
            tc.Controls.Add(button);
            tr.Cells.Add(tc);
            #endregion

            #region New row
            tblMain.Rows.Add(tr);

            tr = new TableRow();

            tc = new TableCell();
            tc.Text = "&nbsp;";
            tr.Cells.Add(tc);
            tc = new TableCell();
            tc.Text = "&nbsp;";
            tr.Cells.Add(tc);

            for (int i = 0; i < 24; i++)
            {
                tc = new TableCell();
                tc.Text = "&nbsp;&nbsp;";
                //tc.Text = "0 <br /> 0";
                tr.Cells.Add(tc);

                tc = new TableCell();
                tc.Text = "&nbsp;&nbsp;";
                //tc.Text = "3 <br /> 0";
                tr.Cells.Add(tc);
            }

            tc = new TableCell();
            tc.Text = "&nbsp;";
            tr.Cells.Add(tc);

            //tc = new TableCell();
            //tr.Cells.Add(tc);

            //tc = new TableCell();
            //tr.Cells.Add(tc);
            tblMain.Rows.Add(tr);
            #endregion

            #region Init Days
            for (int day = 0; day < 7; day++)
            {
                tr = new TableRow();
                tr.ID = "tr" + day.ToString();

                tc = new TableCell();
                tc.Text = ((DayOfWeek)((day + 1) % 7)).ToString().Substring(0, 3);

                tr.Cells.Add(tc);

                tc = new TableCell();
                tc.Text = "&nbsp;";
                tr.Cells.Add(tc);

                for (int i = 0; i < 24; i++)
                {
                    for (int k = 0; k < 60; k += 30)
                    {
                        tc = new TableCell();
                        tc.ID = "cell_" + day.ToString() + "_" + i.ToString() + "." + (k == 30 ? "5" : "0");
                        tr.Cells.Add(tc);
                    }
                }

                tc = new TableCell();
                tc.Text = "&nbsp;";
                tr.Cells.Add(tc);

                tc = new TableCell();
                button = new HtmlButton();
                button.InnerText = "All";
                button.Attributes.Add("onclick", "SetRow(true," + day.ToString() + ");return false;");
                tc.Controls.Add(button);
                tr.Cells.Add(tc);

                tc = new TableCell();
                button = new HtmlButton();
                button.InnerText = "None";
                button.Attributes.Add("onclick", "SetRow(false," + day.ToString() + ");return false;");
                tc.Controls.Add(button);
                tr.Cells.Add(tc);

                tblMain.Rows.Add(tr);
            }
            #endregion
        }

        #region Moved the viewstate to the server for optimisation
        protected override PageStatePersister PageStatePersister
        {
            get
            {
                return new SessionPageStatePersister(this.Page);
            }
        }
        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnGeofence.Click += (s, args) => Response.Redirect(Request.RawUrl);
            this.btnVehicles.Click += new EventHandler(btnVehicles_Click);
            this.btnNotifications.Click += new EventHandler(btnNotifications_Click);
            this.btnVehicleSave.Click += new EventHandler(btnVehicleSave_Click);

            this.btnAddContact.Click += new EventHandler(btnAddContact_Click);
            this.btnAddNotification.Click += new EventHandler(btnAddNotification_Click);

            this.rgContacts.NeedDataSource += new GridNeedDataSourceEventHandler(rgContacts_NeedDataSource);
            this.rgNotifications.NeedDataSource += new GridNeedDataSourceEventHandler(rgNotifications_NeedDataSource);
            this.rgNotifications.ItemDataBound += new GridItemEventHandler(rgNotifications_ItemDataBound);
            this.rgNotifications.ItemCommand += new GridCommandEventHandler(rgNotifications_ItemCommand);
            this.rgContacts.ItemCommand += new GridCommandEventHandler(rgContacts_ItemCommand);

            this.cvNotificationContacts.ServerValidate += new ServerValidateEventHandler(cvNotificationContacts_ServerValidate);
            this.rtvGrouping.NodeClick += new RadTreeViewEventHandler(rtvGrouping_NodeClick);
        }

        void rtvGrouping_NodeClick(object sender, RadTreeNodeEventArgs e)
        {
            int id = int.Parse(e.Node.Attributes["Id"]);
            if (id > 0)
                this.LoadVehicleScreen(id,false);
        }

        #region Geofence handling

        [WebMethod()]
        public static Orchestrator.WebUI.Services.PointGeofence LoadPoint(int pointID)
        {
            if (pointID < 0)
                return null;

            Services.GeoManagement geomanagement = new Services.GeoManagement();
            Orchestrator.WebUI.Services.PointGeofence point = geomanagement.GetPointGeofence(pointID);

            Orchestrator.WebUI.Services.PointGeofence retVal = null;
            if (point != null)
            {
                retVal = point;
            }
            
            return retVal;
        }

        [WebMethod]
        public static bool UpdatePoint(Orchestrator.WebUI.Services.PointGeofence point)
        {
             bool retval = true;
            try
            {
                Services.GeoManagement geomanagement = new Services.GeoManagement();
                
                var p = geomanagement.UpdatePointGeofence(point, System.Threading.Thread.CurrentPrincipal.Identity.Name);
                var pin = geomanagement.GetPoint(point.PointID);

                // Make sure we update the points central location
                pin.Latitide = point.Latitude;
                pin.Longitude = point.Longitude;
                geomanagement.UpdatePoint(pin, System.Threading.Thread.CurrentPrincipal.Identity.Name);
            }
            catch
            {
                retval = false;
            }
            return retval ;
        }

        #endregion

        #region Vehicle Handling
        void btnVehicles_Click(object sender, EventArgs e)
        {
            this.lbVehiclesInView.Items.Clear();
            LoadVehicleScreen(1,true);
        }

        List<FleetViewTreeViewNode> allVehicles = new List<FleetViewTreeViewNode>();

        private void AddChildren(FleetViewTreeViewNode node, RadTreeNode treeNode)
        {
            if (node.NodeType == FleetViewTreeViewNode.DataNodeType.Vehicle)
                allVehicles.Add(node);

            if (node.NodeType == FleetViewTreeViewNode.DataNodeType.OrgUnit)
            {
                if (treeNode == null)
                {
                    treeNode = new Telerik.Web.UI.RadTreeNode(node.Description, node.Id.ToString());
                    treeNode.Attributes.Add("GPSUnitID", node.GpsUnitId);
                    treeNode.Attributes.Add("ParentId", node.ParentId.ToString());
                    treeNode.Attributes.Add("Id", node.Id.ToString());
                    rtvGrouping.Nodes.Add(treeNode);
                    if (node.Children != null && node.Children.Count > 0)
                        node.Children.ForEach(x => AddChildren(x, treeNode));
                }
                else
                {
                    var tn = new RadTreeNode(node.Description, node.Id.ToString());
                    tn.Attributes.Add("GPSUnitID", node.GpsUnitId);
                    tn.Attributes.Add("ParentId", node.ParentId.ToString());
                    tn.Attributes.Add("Id", node.Id.ToString());
                    treeNode.Nodes.Add(tn);
                    if (node.Children != null && node.Children.Count > 0)
                        node.Children.ForEach(x => AddChildren(x, tn));
                }
            }
        }

        protected void LoadVehicleScreen(int Id, bool initialLoad)
        {
            this.lblScreen.Text = "Vehicles";
            this.pvVehicles.Selected = true;
            this.lbVehicles.Items.Clear();
            this.rtvGrouping.Nodes.Clear();
            Services.GeoManagement geomanagement = new Services.GeoManagement();
            var vehiclesInView = geomanagement.GetVehiclesForPointGeofence(this.PointID);
            var groups = geomanagement.GetResourceViewsForVehicle();
            groups.ForEach(n => AddChildren(n, null));
           
            if (initialLoad)
            foreach (var v in vehiclesInView.Where(a => !string.IsNullOrEmpty(a.GpsUnitId)).ToList().OrderBy(g => g.Description))
                if (!this.lbVehiclesInView.Items.Any(item => item.Value == v.Id.ToString()))
                {
                    var item = new RadListBoxItem(v.Description, v.Id.ToString());
                    item.Attributes.Add("GPSUnitID", v.GpsUnitId);
                    item.Attributes.Add("ParentId", v.ParentId.ToString());
                    lbVehiclesInView.Items.Add(item);
                }

            foreach (var v in allVehicles.Where(a => !string.IsNullOrEmpty(a.GpsUnitId) && a.ParentId == Id).ToList().OrderBy(g => g.Description))
                if (!this.lbVehiclesInView.Items.Any(item => item.Value == v.Id.ToString()))
                {
                    var item = new RadListBoxItem(v.Description, v.Id.ToString());
                    item.Attributes.Add("GPSUnitID", v.GpsUnitId);
                    item.Attributes.Add("ParentId", v.ParentId.ToString());
                    lbVehicles.Items.Add(item);
                }
        }

        void btnVehicleSave_Click(object sender, EventArgs e)
        {
            if (lbVehiclesInView.Items.Count > 0)
            {
                // only save/create if there are any vehicles in the view
                Services.GeoManagement geomanagement = new Services.GeoManagement();

                List<FleetViewTreeViewNode> selectedVehicles = new List<FleetViewTreeViewNode>();
                foreach (RadListBoxItem item in lbVehiclesInView.Items)
                    selectedVehicles.Add(new FleetViewTreeViewNode() { Id = int.Parse(item.Value), GpsUnitId = item.Attributes["GPSUnitID"] });

                geomanagement.UpdatePointGeofenceVehicles(this.PointID, selectedVehicles);
            }
        }
        #endregion

        #region Notification Handling

        public List<UserLight> CurrentRecipients
        {
            get
            {
                if (this.ViewState["_recipients"] == null)
                    this.ViewState["_recipients"] = new List<UserLight>();

                return (List<UserLight>)this.ViewState["_recipients"];
            }
            set { this.ViewState["_recipients"] = value; }
        }

        void rgContacts_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            this.rgContacts.DataSource = CurrentRecipients;
        }

        void btnAddContact_Click(object sender, EventArgs e)
        {
            this.CurrentRecipients.Add(new UserLight() { Recipient = txtContactDetail.Text, TypeId = int.Parse(cboContactType.SelectedValue), UserName = txtNotificationName.Text });
            this.txtContactDetail.Text = String.Empty;
            this.txtNotificationName.Text = String.Empty;
            rgContacts.Rebind();
        }

        protected WeekActivePeriod WeekActivePeriod
        {
            get { return (WeekActivePeriod)this.ViewState["_weekActivePeriod"]; }
            set { this.ViewState["_weekActivePeriod"] = value; }
        
        }
        void btnNotifications_Click(object sender, EventArgs e)
        {
            this.lblScreen.Text = "Notification";
            this.pvNotifications.Selected = true;
            this.txtContactDetail.Text = String.Empty;
            this.txtNotificationName.Text = String.Empty;
            this.txtNotificationTitle.Text = String.Empty;
            this.chkNotificationEnabled.Checked = false;
            this.chkOutgoing.Checked = false;
            this.chkIncoming.Checked = false;
            CurrentRecipients.Clear();
            rgContacts.Rebind();
            this.WeekActivePeriod = null;
            UpdateUIToWAP();
            CLearWAP();
            rgNotifications.Rebind();
        }

        private void UpdateUIToWAP()
        {
            TableRow tr = null;
            TableCell tc = null;
            for (int day = 0; day < 7; day++)
            {
                tr = (TableRow)tblMain.FindControl("tr" + day.ToString());

                for (int i = 0; i < 24; i++)
                {
                    tc = (TableCell)tr.Cells[(i * 2 + 2)];
                    if (WeekActivePeriod != null)
                    {
                        if (WeekActivePeriod.IsActiveForTime(new DateTime(1900, 1, 1 + day, i, 0, 0)))
                            tc.CssClass = "active1";
                        else
                            tc.CssClass = "inactive";
                    }

                    tc = (TableCell)tr.Cells[(i * 2 + 2 + 1)];
                    if (WeekActivePeriod != null)
                    {
                        if (WeekActivePeriod.IsActiveForTime(new DateTime(1900, 1, 1 + day, i, 30, 0)))
                            tc.CssClass = "active1";
                        else
                            tc.CssClass = "inactive";
                    }
                }
            }
        }

        private void CLearWAP()
        {
            TableRow tr = null;
            TableCell tc = null;
            for (int day = 0; day < 7; day++)
            {
                tr = (TableRow)tblMain.FindControl("tr" + day.ToString());

                for (int i = 0; i < 24; i++)
                {
                    tc = (TableCell)tr.Cells[(i * 2 + 2)];
                        tc.CssClass = "active1";

                    tc = (TableCell)tr.Cells[(i * 2 + 2 + 1)];
                        tc.CssClass = "active1";
                }
            }
        }

        /// <summary>
        /// Updates the passed WeekActivePeriod to reflect the state of the checkboxes. 
        /// </summary>
        /// <param name="period"></param>
        public void SetWeekActivePeriod()
        {
            if (this.WeekActivePeriod == null)
                this.WeekActivePeriod = new Services.WeekActivePeriod();

            //period.ClearAllHours();
            var days = hiddenActives.Value.Split('|');
            this.WeekActivePeriod.Monday       = int.Parse(days[0].Split('.')[0]);
            this.WeekActivePeriod.MondaySecond = int.Parse(days[0].Split('.')[1]);
            this.WeekActivePeriod.Tuesday = int.Parse(days[1].Split('.')[0]);
            this.WeekActivePeriod.TuesdaySecond = int.Parse(days[1].Split('.')[1]);
            this.WeekActivePeriod.Wednesday = int.Parse(days[2].Split('.')[0]);
            this.WeekActivePeriod.WednesdaySecond = int.Parse(days[2].Split('.')[1]);
            this.WeekActivePeriod.Thursday = int.Parse(days[3].Split('.')[0]);
            this.WeekActivePeriod.ThursdaySecond = int.Parse(days[3].Split('.')[1]);
            this.WeekActivePeriod.Friday = int.Parse(days[4].Split('.')[0]);
            this.WeekActivePeriod.FridaySecond = int.Parse(days[4].Split('.')[1]);
            this.WeekActivePeriod.Saturday = int.Parse(days[5].Split('.')[0]);
            this.WeekActivePeriod.SaturdaySecond = int.Parse(days[5].Split('.')[1]);
            this.WeekActivePeriod.Sunday = int.Parse(days[6].Split('.')[0]);
            this.WeekActivePeriod.SundaySecond = int.Parse(days[6].Split('.')[1]);
        }

        [WebMethod]
        public static List<EF.PointGeofenceNotification> LoadNotifications(int pointID)
        {
            GeoManagement client = new GeoManagement();
            List<EF.PointGeofenceNotification> retVal = new List<EF.PointGeofenceNotification>();
            retVal =  client.GetPointGeofenceNotifications(pointID);

            return retVal;
        }

        [WebMethod]
        public string AddWeekActivePeriod()
        {
            return string.Empty;
        }

        public List<UserLight> NotifictionUsers
        {
            get
            {
                return (List<UserLight>)this.ViewState["_notificationUsers"];
            }
            set
            {
                this.ViewState["_notificationUsers"] = value;
            }
        }

        void rgNotifications_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            GeoManagement client = new GeoManagement();
            this.rgNotifications.DataSource = client.GetPointGeofenceNotifications(this.PointID);
        }

        GeoManagement _ds = new GeoManagement();
        void rgNotifications_ItemDataBound(object sender, GridItemEventArgs e)
        {
            // ned this to display the recipients as something sensible
            if (e.Item is GridDataItem)
            {
                EF.PointGeofenceNotification d = (EF.PointGeofenceNotification)e.Item.DataItem;
                List<UserLight> users = _ds.GetPointGeofenceNotificationUsers(d.NotificationId);
                Label lbl = e.Item.FindControl("lblRecipients") as Label;

                string userString= string.Empty;

                foreach (var u in users)
                    userString += u.UserName + ", ";

                lbl.Text = userString;
            }
        }

        void btnAddNotification_Click(object sender, EventArgs e)
        {
            SetWeekActivePeriod();

            if (Page.IsValid)
            {
                if (this.btnAddNotification.Text == "Update Notification")
                    UpdateNotification();
                else
                    AddNotification();
            }

            this.txtContactDetail.Text = String.Empty;
            this.txtNotificationName.Text = String.Empty;
            this.txtNotificationTitle.Text = String.Empty;
            this.chkNotificationEnabled.Checked = false;
            this.chkOutgoing.Checked = false;
            this.chkIncoming.Checked = false;
            CurrentRecipients.Clear();
            rgContacts.Rebind();
            this.btnAddNotification.Text = "Add Notification";
            this.WeekActivePeriod = null;
            CLearWAP();
        }

        private void UpdateNotification()
        {
            GeoManagement client = new GeoManagement();
            EF.PointGeofenceNotification notification = client.GetPointGeofenceNotification(this.NotificationID);
            
            notification.WeekActivePeriod.Monday = this.WeekActivePeriod.Monday;
            notification.WeekActivePeriod.Monday2 = this.WeekActivePeriod.MondaySecond;
            notification.WeekActivePeriod.Tuesday = this.WeekActivePeriod.Tuesday;
            notification.WeekActivePeriod.Tuesday2 = this.WeekActivePeriod.TuesdaySecond;
            notification.WeekActivePeriod.Wednesday = this.WeekActivePeriod.Wednesday;
            notification.WeekActivePeriod.Wednesday2 = this.WeekActivePeriod.WednesdaySecond;
            notification.WeekActivePeriod.Thursday = this.WeekActivePeriod.Thursday;
            notification.WeekActivePeriod.Thursday2 = this.WeekActivePeriod.ThursdaySecond;
            notification.WeekActivePeriod.Friday = this.WeekActivePeriod.Friday;
            notification.WeekActivePeriod.Friday2 = this.WeekActivePeriod.FridaySecond;
            notification.WeekActivePeriod.Saturday = this.WeekActivePeriod.Saturday;
            notification.WeekActivePeriod.Saturday2 = this.WeekActivePeriod.SaturdaySecond;
            notification.WeekActivePeriod.Sunday = this.WeekActivePeriod.Sunday;
            notification.WeekActivePeriod.Sunday2 = this.WeekActivePeriod.SundaySecond;
            client.UpdateWeekActivePeriod(notification.WeekActivePeriod);

            client.UpdatePointGeofenceNotification(this.NotificationID, txtNotificationTitle.Text, chkNotificationEnabled.Checked, chkIncoming.Checked, chkOutgoing.Checked, CurrentRecipients);

            this.txtContactDetail.Text = String.Empty;
            this.txtNotificationName.Text = String.Empty;
            this.txtNotificationTitle.Text = String.Empty;
            this.chkNotificationEnabled.Checked = false;
            this.chkOutgoing.Checked = false;
            this.chkIncoming.Checked = false;
            CurrentRecipients.Clear();
            rgContacts.Rebind();

            rgNotifications.Rebind();

            this.WeekActivePeriod = null;
            CLearWAP();
        }

        private void AddNotification()
        {
            GeoManagement client = new GeoManagement();
            EF.PointGeofenceNotification notification = new EF.PointGeofenceNotification();
            notification.Description = txtNotificationTitle.Text;
            notification.Incoming = chkIncoming.Checked;
            notification.Outgoing = chkOutgoing.Checked;
            notification.IsEnabled = chkNotificationEnabled.Checked;

            #region Convert to an EF Week Active Period
            if (this.WeekActivePeriod.WeekActivePeriodId == Guid.Empty)
            {
                EF.WeekActivePeriod period = new EF.WeekActivePeriod();
                period.WeekActivePeriodID = Guid.NewGuid();
                period.Monday = this.WeekActivePeriod.Monday;
                period.Monday2 = this.WeekActivePeriod.MondaySecond;
                period.Tuesday = this.WeekActivePeriod.Tuesday;
                period.Tuesday2 = this.WeekActivePeriod.TuesdaySecond;
                period.Wednesday = this.WeekActivePeriod.Wednesday;
                period.Wednesday2 = this.WeekActivePeriod.WednesdaySecond;
                period.Thursday = this.WeekActivePeriod.Thursday;
                period.Thursday2 = this.WeekActivePeriod.ThursdaySecond;
                period.Friday = this.WeekActivePeriod.Friday;
                period.Friday2 = this.WeekActivePeriod.FridaySecond;
                period.Saturday = this.WeekActivePeriod.Saturday;
                period.Saturday2 = this.WeekActivePeriod.SaturdaySecond;
                period.Sunday = this.WeekActivePeriod.Sunday;
                period.Sunday2 = this.WeekActivePeriod.SundaySecond;
                notification.WeekActivePeriod = new EF.WeekActivePeriod();
                notification.WeekActivePeriod.WeekActivePeriodID = client.AddWeekActivePeriod(period).WeekActivePeriodID;
            }

            #endregion

            client.AddPointGeofenceNotification(notification, PointID, CurrentRecipients);

            rgNotifications.Rebind();
        }

        void cvNotificationContacts_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = this.CurrentRecipients.Count > 0;
        }
        
        void rgNotifications_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {

                this.CurrentRecipients.Clear();

                // Load the selected Notification 
                EF.PointGeofenceNotification notification = _ds.GetPointGeofenceNotification((int)e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["NotificationId"]);

                this.NotificationID = notification.NotificationId;

                var notifictionUsers = _ds.GetPointGeofenceNotificationUsers(this.NotificationID);
                // Load the Notification Users
                foreach (var u in notifictionUsers)
                {
                    this.CurrentRecipients.Add(new UserLight()
                    {
                        UserName = u.UserName,
                        Recipient = u.Recipient,
                        TypeId = u.TypeId
                    });
                }

                // Set the WAP
                this.WeekActivePeriod = new WeekActivePeriod();
                this.WeekActivePeriod.Monday = notification.WeekActivePeriod.Monday;
                this.WeekActivePeriod.MondaySecond = notification.WeekActivePeriod.Monday2;
                this.WeekActivePeriod.Tuesday = notification.WeekActivePeriod.Tuesday;
                this.WeekActivePeriod.TuesdaySecond = notification.WeekActivePeriod.Tuesday2;
                this.WeekActivePeriod.Wednesday= notification.WeekActivePeriod.Wednesday;
                this.WeekActivePeriod.WednesdaySecond = notification.WeekActivePeriod.Wednesday2;
                this.WeekActivePeriod.Thursday= notification.WeekActivePeriod.Thursday;
                this.WeekActivePeriod.ThursdaySecond= notification.WeekActivePeriod.Thursday2;
                this.WeekActivePeriod.Friday = notification.WeekActivePeriod.Friday;
                this.WeekActivePeriod.FridaySecond= notification.WeekActivePeriod.Friday2;
                this.WeekActivePeriod.Saturday= notification.WeekActivePeriod.Saturday;
                this.WeekActivePeriod.SaturdaySecond= notification.WeekActivePeriod.Saturday2;
                this.WeekActivePeriod.Sunday= notification.WeekActivePeriod.Sunday;
                this.WeekActivePeriod.SundaySecond= notification.WeekActivePeriod.Sunday2;

                UpdateUIToWAP();
                this.rgContacts.Rebind();

                this.txtNotificationTitle.Text = notification.Description;
                this.chkNotificationEnabled.Checked = notification.IsEnabled;
                this.chkIncoming.Checked = notification.Incoming;
                this.chkOutgoing.Checked = notification.Outgoing;
              
                this.btnAddNotification.Text = "Update Notification";
            }

            if (e.CommandName == "Delete")
            {
                this.NotificationID = (int)e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["NotificationId"];
                // need to prompt to ensure.
                
                _ds.DeletePointGeofenceNotification(this.NotificationID);
            }
        }

        void rgContacts_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                // remove the contact
                //this.CurrentRecipients.Remove(this.CurrentRecipients.First(r=> r.Recipient == e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["Recipient"].ToString()));
                this.CurrentRecipients.RemoveAt(e.Item.ItemIndex);
                this.rgContacts.Rebind();
            }
        }

        #endregion
    }

    

}