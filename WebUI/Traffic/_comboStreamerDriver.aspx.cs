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
    public partial class drivercomboStreamer : Orchestrator.Base.BasePage
    {

        private int dropDownRows = 5;

        protected override void OnInit(EventArgs e)
        {
            cboDriver.ItemsRequested +=new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboDriver_ItemsRequested);
        }
        
        protected void cboDriver_ItemsRequested(object sender, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            ((Telerik.Web.UI.RadComboBox)sender).Items.Clear();

            #region Get ControlArea and Traffic Area
            string[] clientArgs = e.Context["FilterString"].ToString().Split(':');
            int controlAreaId = 0;
            int[] trafficAreas = new int[clientArgs.Length-1];
            controlAreaId = int.Parse(clientArgs[0]);

            for (int i = 1; i < clientArgs.Length; i++)
            {
                trafficAreas[i - 1] = int.Parse(clientArgs[i]);
            }
            #endregion

            Facade.IResource facResource = new Facade.Resource();
            DataSet ds = facResource.GetAllResourcesFiltered(e.Text, eResourceType.Driver,controlAreaId, trafficAreas, true) ;

            int itemsPerRequest = dropDownRows;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            DataTable dt = ds.Tables[0];
            Telerik.Web.UI.RadComboBoxItem rcItem = null;
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new Telerik.Web.UI.RadComboBoxItem();
                rcItem.Text = dt.Rows[i]["Description"].ToString();
                rcItem.Value = dt.Rows[i]["ResourceId"].ToString();
                ((Telerik.Web.UI.RadComboBox)sender).Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }

        }

        public int DropDownSize
        {
            set { dropDownRows = value; }
        }

    }
}