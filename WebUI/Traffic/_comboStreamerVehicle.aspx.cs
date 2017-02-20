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

    public partial class comboStreamerVehicle : Orchestrator.Base.BasePage
    {
        private int dropDownRows = 5;

        protected override void OnInit(EventArgs e)
        {
            cboVehicle.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboVehicle_ItemsRequested);      
        }

        void cboVehicle_ItemsRequested(object sender, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            ((Telerik.Web.UI.RadComboBox)sender).Items.Clear();
            Telerik.Web.UI.RadComboBoxItem rcItem = new Telerik.Web.UI.RadComboBoxItem();

            DataSet ds = null;
            string[] clientArgs = e.Context["FilterString"].ToString().Split(':');
            if (e.Context["FilterString"] != null)
            {
                if (clientArgs[0] == "true")
                {
                    // Get the Drivers usual vehicle
                    Facade.IDriver facDriver = new Facade.Resource();
                    Entities.Driver driver = facDriver.GetDriverForResourceId(int.Parse(clientArgs[1]));

                    Entities.Vehicle vehicle = ((Facade.IVehicle)facDriver).GetForVehicleId(driver.AssignedVehicleId);
                    if (vehicle != null)
                    {
                        

                        rcItem.Text = vehicle.RegNo;
                        rcItem.Value = vehicle.ResourceId.ToString();
                        rcItem.Selected = true;
                        ((Telerik.Web.UI.RadComboBox)sender).Items.Add(rcItem);
                    }
                }
                else
                {
                    int controlAreaId = 0;
                    int[] trafficAreas = new int[clientArgs.Length - 1];
                    controlAreaId = int.Parse(clientArgs[0]);

                    for (int i = 1; i < clientArgs.Length; i++)
                    {
                        trafficAreas[i - 1] = int.Parse(clientArgs[i]);
                    }

                    Facade.IResource facResource = new Facade.Resource();
                    ds = facResource.GetAllResourcesFiltered(e.Text, eResourceType.Vehicle, controlAreaId, trafficAreas, true);

                }
            }
            else
            {
                Facade.IResource facResource = new Facade.Resource();
                ds = facResource.GetAllResourcesFiltered(e.Text, eResourceType.Vehicle, false);

            }

            if (ds != null)
            {
                int itemsPerRequest = 20;
                int itemOffset = e.NumberOfItems;
                int endOffset = itemOffset + itemsPerRequest;
                if (endOffset > ds.Tables[0].Rows.Count)
                    endOffset = ds.Tables[0].Rows.Count;

                DataTable dt = ds.Tables[0];
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

        }
        public int DropDownSize
        {
            set { dropDownRows = value; }
        }
    }

}