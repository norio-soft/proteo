using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Text;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Job
{
    public partial class driveractivity : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                PopulateStaticControls();
        }
        protected override PageStatePersister PageStatePersister
        {
            get
            {
                return new HiddenFieldPageStatePersister(this);
            }
        }
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.cboDriver.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboDriver_ItemsRequested);
            this.grdTrafficSheet.NeedDataSource +=new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdTrafficSheet_NeedDataSource);
            this.grdTrafficSheet.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(grdTrafficSheet_ItemDataBound);
            this.btnFilter.Click += new EventHandler(btnFilter_Click);
            this.btnExport.Click += new EventHandler(btnExport_Click);
            this.btnEmail.Click += new EventHandler(btnEmail_Click);
        }

        void btnEmail_Click(object sender, EventArgs e)
        {
            byte[] attachment;

            
        }

        void btnExport_Click(object sender, EventArgs e)
        {
            grdTrafficSheet.ExportSettings.FileName="DriverActivity for " + cboDriver.Text.Replace(",", " ") ;
            grdTrafficSheet.ExportSettings.ExportOnlyData = true;
            grdTrafficSheet.ExportSettings.IgnorePaging = true;

            grdTrafficSheet.MasterTableView.ExportToExcel();//("DriverActivity for " + cboDriver.Text.Replace(",", " ") , true, true);
            
        }

        void btnFilter_Click(object sender, EventArgs e)
        {
            grdTrafficSheet.Rebind();
        }

        void grdTrafficSheet_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                if (btnExport.Visible == false)
                    btnExport.Visible = true;
                

                DataRowView drv = e.Item.DataItem as DataRowView;
                string htmlColour = string.Empty;
                switch((int)drv["InstructionStateID"])
                {
                    case (int)eInstructionState.Booked:
                        htmlColour =  Utilities.GetJobStateColourForHTML(eJobState.Booked);
                        break;
                    case (int)eInstructionState.Planned:
                        htmlColour = Utilities.GetJobStateColourForHTML(eJobState.Planned);
                        break;
                    case (int)eInstructionState.InProgress:
                        htmlColour = Utilities.GetJobStateColourForHTML(eJobState.InProgress);
                        break;
                    case (int)eInstructionState.Completed:
                        htmlColour = Utilities.GetJobStateColourForHTML(eJobState.Completed);
                        break;
                }
                e.Item.BackColor = System.Drawing.ColorTranslator.FromHtml(htmlColour);
            }
        }

        protected string GetImage(string legPosition)
        {
            switch (legPosition)
            {
                case "First":
                    return Page.ResolveClientUrl("~/images/legTop.gif");
                case "Middle":
                    return Page.ResolveClientUrl("~/images/legMiddle.gif");
                case "Last":
                    return Page.ResolveClientUrl("~/images/legBottom.gif");
            }
            return Page.ResolveClientUrl("~/images/spacer.gif");
        }

        protected string GetLoadLinks(string loadLinks)
        {
            string linkTemplate = "<span id=\"spnLoadNumber\"  onMouseOver=\"ShowOrderNotesToolTip(this, {0});\" onMouseOut=\"hideAd();\" class=\"orchestratorLink\">{1}</span>";
            string[] references = loadLinks.Split('/');
            string[] parts;
            string lnk = string.Empty;
            string retVal = string.Empty;
            foreach (string link in references)
            {
                parts = link.Split('|');
                if (parts.Length > 1)
                {
                    lnk = String.Format(linkTemplate, parts[1].Trim(), parts[0].Trim());
                    if (retVal.Length > 0)
                        retVal += " / ";
                    retVal += lnk;
                }
            }
            if (retVal.Length == 0)
                retVal = loadLinks;

            return retVal;
        }

        void grdTrafficSheet_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            if (!dteStartDate.SelectedDate.HasValue)
                return;

            List<eInstructionState> instructionStates = new List<eInstructionState>();
            foreach (ListItem li in cblStates.Items)
                if (li.Selected)
                    instructionStates.Add((eInstructionState)Enum.Parse(typeof(eInstructionState), li.Value));
            
            grdTrafficSheet.DataSource = new Facade.Job().GetLegsForDriver((DateTime)dteStartDate.SelectedDate, ((DateTime) dteEndDate.SelectedDate).Subtract(dteEndDate.SelectedDate.Value.TimeOfDay).Add(new TimeSpan(23,59,59)), int.Parse(cboDriver.SelectedValue), instructionStates);
            

        }

        protected string GetCustomers(int StartInstructionTypeID, int EndInstructionTypeID, string StartOrders, string EndOrders)
        {
            string retVal = string.Empty;
            string[] orders;
            string[] orderDetails;

            // Based on the type of instruction, this determines which customers are involved
            if (StartInstructionTypeID > 0)
            {
                if (StartInstructionTypeID == (int)eInstructionType.Load)
                {
                    orders = StartOrders.Split(',');
                    foreach (string order in orders)
                    {
                        orderDetails = order.Split('|');
                        retVal += orderDetails[2] + "<br/>";
                    }
                }
            }

            if (StartInstructionTypeID == -1)
            {
                if (EndInstructionTypeID == (int)eInstructionType.Load)
                {
                    orders = EndOrders.Split(',');
                    foreach (string order in orders)
                    {
                        orderDetails = order.Split('|');
                        retVal += orderDetails[2] + "<br/>";
                    }
                }
            }


            return retVal;
        }

        protected string GetAction(object d)
        {
            if (d is DataRowView)
                return GetAction((DataRowView)d);

            return d.GetType().ToString();
        }

        protected string GetAction(DataRowView drv)
        {
            // We should be able to determine what was actually being done here.
            int startInstructionTypeID = (int)drv["StartInstructionTypeID"];
            int endInstructionTypeID = (int)drv["EndInstructionTypeID"];

            string[] Orders;
            string[] OrderDetails;
            string actions = string.Empty;

            if (startInstructionTypeID == 1)
            {
                // Then this is a Loading Leg
                Orders = drv["StartOrders"].ToString().Split(',');
                foreach (string order in Orders)
                {
                    OrderDetails = order.Split('|');
                    if (actions.Length > 0)
                        actions += "<br/>";

                    actions += string.Format("Loading {0} Pallets for {1}", OrderDetails[5], OrderDetails[2]);
                }

                switch(endInstructionTypeID)
                {
                    case 1:
                        #region Load Instruction Type
                        
                        // This is a Load and then drive to next Load 
                        Orders = drv["StartOrders"].ToString().Split(',');
                        foreach (string order in Orders)
                        {
                            OrderDetails = order.Split('|');
                            if (actions.Length > 0)
                                actions += "<br/>";

                            actions += string.Format("Loading {0} Pallets for {1}", OrderDetails[5], OrderDetails[2]);
                        } 

                        #endregion
                        break;

                    case 2:
                        #region Drop Instruction Type
                        
                        Orders = drv["EndOrders"].ToString().Split(',');
                        foreach (string order in Orders)
                        {
                            OrderDetails = order.Split('|');
                            if (actions.Length > 0)
                                actions += "<br/>";

                            actions += string.Format("Dropping {0} pallets for {1}", OrderDetails[5], OrderDetails[2]);
                        }

                        #endregion
                        break;

                    case 7:
                        #region Trunk / Cross Dock Instruction Type
                        
                        // This is a Load and drive to Trunk Point Leg
                        actions += "<br/>No action as this is a trunk";

                        #endregion
                        break;
                }
            }

            if (startInstructionTypeID == -1)
            {
                switch(endInstructionTypeID)
                {
                    case 2:
                        #region Drop Instruction Type
                        
                        // this is not a Loading Instruction, this is a simple drop 
                        Orders = drv["EndOrders"].ToString().Split(',');
                        foreach (string order in Orders)
                        {
                            OrderDetails = order.Split('|');
                            if (actions.Length > 0)
                                actions += "<br/>";

                            actions += string.Format("Dropping {0} pallets for {1}", OrderDetails[5], OrderDetails[2]);
                        }

                        #endregion
                        break;

                    case 7:
                        #region Trunk / Cross Dock Instruction Type
                        
                        // we are moving to a trunk point
                        actions += "<br/>No action as this is a trunk";

                        #endregion
                        break;
                }
            }

            return actions;
        }

        protected string GetStartAction(object row)
        {
            DataRowView drv = null;
            if (row is DataRowView)
                drv = (DataRowView)row;
            else
                return string.Empty ;

            // We should be able to determine what was actually being done here.
            int startInstructionTypeID = (int)drv["StartInstructionTypeID"];
            
            string[] Orders;
            string[] OrderDetails;
            string actions = string.Empty;

            if (startInstructionTypeID == 1)
            {
                // Then this is a Loading Leg
                Orders = drv["StartOrders"].ToString().Split(',');
                foreach (string order in Orders)
                {
                    OrderDetails = order.Split('|');
                    if (actions.Length > 0)
                        actions += "<br/>";

                    actions += string.Format("<a href=\"javascript:viewOrderProfile({2});\">Loading {0} Pallets for {1}</a>", OrderDetails[5], OrderDetails[2], OrderDetails[0]);
                }
            }

            return actions;
        }

        protected string GetEndAction(object row)
        {
            DataRowView drv = null;
            if (row is DataRowView)
                drv = (DataRowView)row;
            else
                return string.Empty;

            // We should be able to determine what was actually being done here.
            int endInstructionTypeID = (int)drv["EndInstructionTypeID"];
            int startInstructionTypeID = (int)drv["StartInstructionTypeID"];
            
            string[] Orders;
            string[] OrderDetails;

            string actions = string.Empty;

            switch(endInstructionTypeID)
            {
                case 1:
                    #region Load Instruction Type

                    string seOrders = startInstructionTypeID == -1 ? "EndOrders" : "StartOrders";

                    // This is a Load and then drive to next Load 
                    Orders = drv[seOrders].ToString().Split(',');
                    foreach (string order in Orders)
                    {
                        OrderDetails = order.Split('|');
                        if (actions.Length > 0)
                            actions += "<br/>";

                        actions += string.Format("<a href=\"javascript:viewOrderProfile({2});\">Loading {0} pallets for {1}</a>", OrderDetails[5], OrderDetails[2], OrderDetails[0]);
                    }

                    #endregion
                    break;

                case 2:
                    #region Drop Instruction Type

                    Orders = drv["EndOrders"].ToString().Split(',');
                    foreach (string order in Orders)
                    {
                        OrderDetails = order.Split('|');
                        if (actions.Length > 0)
                            actions += "<br/>";

                        actions += string.Format("<a href=\"javascript:viewOrderProfile({2});\">Dropping {0} pallets for {1}</a>", OrderDetails[5], OrderDetails[2], OrderDetails[0]);
                    }

                    #endregion
                    break;
                
                case 7:
                    #region Trunk / Cross Dock Instruction Type
                    
                    // This is a Load and drive to Trunk Point Leg
                    if (actions.Length > 0)
                        actions += "<br/>";

                    actions += "No action as this is a trunk";
                    
                    #endregion
                    break;
            }

            return actions;
        }

        #region Private Methods
        private void PopulateStaticControls()
        {
            cblStates.DataSource = Enum.GetValues(typeof(eInstructionState));
            cblStates.DataBind();

        }
        #endregion

        #region Combo box Items Requested Handlers
        protected void cboDriver_ItemsRequested(object sender, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            ((Telerik.Web.UI.RadComboBox)sender).Items.Clear();

            Facade.IResource facResource = new Facade.Resource();
            DataSet ds = null;

            #region Get ControlArea and Traffic Area
            // As thgis is only set when there is no call back or post back we need to get the correct values

            #endregion

            ds = facResource.GetAllResourcesFiltered("%" + e.Text, eResourceType.Driver, false);
         
            int itemsPerRequest = 30;
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
        #endregion
    }
}
