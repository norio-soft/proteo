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

namespace Orchestrator.WebUI.Traffic
{
    public partial class comboStreamerDepotCode : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            this.cboDepotCode.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboDepotCode_ItemsRequested);
        }

        void cboDepotCode_ItemsRequested(object sender, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            ((Telerik.Web.UI.RadComboBox)sender).Items.Clear();
            System.Data.DataTable reader = null;
            if (Cache["__depotCodes"] == null)
            {
                Facade.IReferenceData facResource = new Facade.ReferenceData();
                reader = facResource.GetAllDepotCodes();
                Cache.Add("__depotCodes", reader, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 20, 0), System.Web.Caching.CacheItemPriority.Normal,null);
            }
            else
            {
                reader = (DataTable) Cache["__depotCodes"];
            }

            int itemsPerRequest = 20;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > reader.Rows.Count)
                endOffset = reader.Rows.Count;

            DataTable dt = reader;
            Telerik.Web.UI.RadComboBoxItem rcItem = null;
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new Telerik.Web.UI.RadComboBoxItem();
                rcItem.Text = dt.Rows[i]["DepotCode"].ToString();
                rcItem.Value = dt.Rows[i]["DepotCode"].ToString();
                ((Telerik.Web.UI.RadComboBox)sender).Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }
    }
}