using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Orchestrator.WebUI.WebParts
{
    public partial class wpPageMenu : System.Web.UI.UserControl
    {
        #region Private Fields
        // Use a field to reference the current WebPartManager.
        WebPartManager _manager;
        #endregion

        #region Page Load/Init
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.Page.InitComplete += new EventHandler(Page_InitComplete);
            this.cboPageMenu.SelectedIndexChanged += new Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventHandler(cboPageMenu_SelectedIndexChanged);
            this.PreRender += new EventHandler(UserControls_WebParts_wpPageMenu_PreRender);
        }
        #endregion

        #region Overrides
        void UserControls_WebParts_wpPageMenu_PreRender(object sender, EventArgs e)
        {

            cboPageMenu.SelectedValue = _manager.DisplayMode.Name;
        }

        void Page_InitComplete(object sender, EventArgs e)
        {
            if (Page != null && WebPartManager.GetCurrentWebPartManager(Page) != null)
            {
                _manager = WebPartManager.GetCurrentWebPartManager(Page);

                String browseModeName = WebPartManager.BrowseDisplayMode.Name;
                Telerik.Web.UI.RadComboBoxItem item = new Telerik.Web.UI.RadComboBoxItem("View Page", "Browse");
                cboPageMenu.Items.Add(item);
                item = new Telerik.Web.UI.RadComboBoxItem("Change Settings", "Edit");
                cboPageMenu.Items.Add(item);
                item = new Telerik.Web.UI.RadComboBoxItem("Design Page", "Catalog");
                cboPageMenu.Items.Add(item);
            }
        }
        #endregion

        #region Event Handlers
        void cboPageMenu_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            String selectedMode = cboPageMenu.SelectedValue;

            WebPartDisplayMode mode = _manager.SupportedDisplayModes[selectedMode];
            if (mode != null)
                _manager.DisplayMode = mode;

        }
        #endregion


    }
}