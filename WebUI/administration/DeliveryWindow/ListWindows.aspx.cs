using Orchestrator.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.administration.DeliveryWindow
{
    public partial class ListWindows :  BasePage
    {
        /// <summary>
        /// SelectedWindow
        /// </summary>
        public Int32 SelectedWindow { get { return Request.QueryString["id"] == null ? 0 : Int32.Parse(Request.QueryString["id"]); } }

        /// <summary>
        /// Mode
        /// </summary>
        public String Mode { get { return Request.QueryString["mode"] == null ? "" : Request.QueryString["mode"]; } }

        public Int32 Point { get { return Request.QueryString["Point"] == null ? 0 : Convert.ToInt32( Request.QueryString["Point"] ) ; } }
        
        /// <summary>
        /// Page_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditRate);

            switch (Mode)
            {
                case "Delete":
                     Facade.DeliveryWindow facDeliveryWindowSetup = new Orchestrator.Facade.DeliveryWindow();
                     facDeliveryWindowSetup.DeleteMatrix(SelectedWindow, HttpContext.Current.User.Identity.Name);
                    break;
            }

            LoadStaticData();
            LoadData();
        }

        /// <summary>
        /// Delete Window
        /// </summary>
        protected void DeleteWindow()
        {
            // delete SelectedWindow


        }

        /// <summary>
        /// RadGrid1_PageSizeChanged
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RadGrid1_PageSizeChanged(object source, GridPageSizeChangedEventArgs e)
        {
            LoadData();
        }

        private void LoadStaticData()
        {
            this.cboZoneMap.DataSource =
                from zm in EF.DataContext.Current.ZoneMapSet
                orderby zm.Description
                select new
                {
                    zm.ZoneMapId,
                    zm.Description,
                };

            this.cboZoneMap.DataBind();
        }

        /// <summary>
        /// LoadData
        /// </summary>
        private void LoadData()
        {
            Facade.DeliveryWindow facDeliveryWindowSetup = new Orchestrator.Facade.DeliveryWindow();
            DataSet ds = facDeliveryWindowSetup.GetMatrix();
            RadGrid1.DataSource = ds;
        }

        /// <summary>
        /// RadGrid1_PageIndexChanged
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RadGrid1_PageIndexChanged(object source, Telerik.Web.UI.GridPageChangedEventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// RadGrid1_SortCommand
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RadGrid1_SortCommand(object source, Telerik.Web.UI.GridSortCommandEventArgs e)
        {
            LoadData();
        }

        [WebMethod]
        public static int CreateDeliveryWindow(string description, int zoneMapID)
        {
            var facDeliveryWindow = new Orchestrator.Facade.DeliveryWindow();

            if (facDeliveryWindow.CheckMatrix( description, zoneMapID) > 0)
            {
                throw new Exception("Zone and Description already exist");
            }

            var deliveryWindowID = facDeliveryWindow.AddMatrix( description, zoneMapID, HttpContext.Current.User.Identity.Name);
            return deliveryWindowID;
        }

    }
}