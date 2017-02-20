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
    public partial class comboStreamerTrailer : Orchestrator.Base.BasePage
    {
        private int dropDownRows = 5;

        protected override void OnInit(EventArgs e)
        {
            cboTrailer.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboTrailer_ItemsRequested);
        }

        void cboTrailer_ItemsRequested(object sender, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            ((Telerik.Web.UI.RadComboBox)sender).Items.Clear();

            #region Get ControlArea and Traffic Area
            string[] clientArgs = e.Context["FilterString"].ToString().Split(':');
            int controlAreaId = 0;
            controlAreaId = int.Parse(clientArgs[0]);

            string[] taids = clientArgs[1].Split(',');
            int[] trafficAreas = new int[taids.Length];
            for (int i = 0; i < taids.Length; i++)
            {
                trafficAreas[i] = int.Parse(taids[i]);
            }
            #endregion

            Facade.IResource facResource = new Facade.Resource();
            DataSet ds = facResource.GetAllResourcesFiltered(e.Text, eResourceType.Trailer, controlAreaId, trafficAreas, true);

            int itemsPerRequest = 20;
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