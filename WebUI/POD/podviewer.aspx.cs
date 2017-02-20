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

using ComponentArt.Web.UI;

namespace Orchestrator.WebUI.POD
{

    public partial class POD_podviewer : Orchestrator.Base.BasePage
    {
        int scannedFormId = int.Parse("148");

        protected void Page_Load(object sender, EventArgs e)
        {
           
            Facade.IForm facForm = new Facade.Form();
            Entities.Scan scan = facForm.GetForScannedFormId(scannedFormId);
            for (int i = 0; i < scan.PageCount; i++)
            {
                TabStripTab tab = new TabStripTab();
                string imgName = "_page" + ((int)(i + 1)).ToString() + ".jpg";
                tab.Look.ImageUrl = imgName;
                tab.SelectedLook.ImageUrl = imgName;
                tab.ClientSideCommand = "selectPage(" + i.ToString() + ");";
                ThumbnailsTabStrip.Tabs.Add(tab);
            }

            CurrentImage.Width = Unit.Percentage(100);
            CurrentImage.Height = Unit.Percentage(100);

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.AjaxImage.Callback += new CallBack.CallbackEventHandler(AjaxImage_Callback);
            this.AjaxImageTitle.Callback += new CallBack.CallbackEventHandler(AjaxImageTitle_Callback);
        }

        void AjaxImageTitle_Callback(object sender, CallBackEventArgs e)
        {
            ImageTitle.Text = "Page " + e.Parameter;
            ImageTitle.RenderControl(e.Output);
        }

        void AjaxImage_Callback(object sender, CallBackEventArgs e)
        {
            int page = int.Parse(e.Parameter);
            CurrentImage.ImageUrl = "getImage.aspx?ScannedFormId=" + scannedFormId + "&page=" + e.Parameter;
            CurrentImage.RenderControl(e.Output);
        }

        void AjaxThumbnails_Callback(object sender, CallBackEventArgs e)
        {

        }
    }
}