using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Data;

namespace Orchestrator.WebUI.Integration
{
    public partial class ManageMessages : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.cboSystem.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(cboSystem_ItemsRequested);
            this.btnRefresh.Click += new EventHandler(btnRefresh_Click);
            this.grdMessageList.ItemCommand += new GridCommandEventHandler(grdMessageList_ItemCommand);
            this.grdMessageList.NeedDataSource += new GridNeedDataSourceEventHandler(grdMessageList_NeedDataSource);
            this.grdMessageList.ItemDataBound += new GridItemEventHandler(grdMessageList_ItemDatabound);
            
        }

        private void grdMessageList_ItemDatabound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                if (item["Type"].Text.ToUpper() == "EXPORT")
                {
                    Button btn = (Button)item["Select"].Controls[0];
                    btn.Enabled = false;
                }
            }
        }

        private void grdMessageList_ItemCommand(object sender, GridCommandEventArgs e)
        {
            Facade.ImportMessage facImportMessage = new Facade.ImportMessage();
            bool success = false;
            
            if (e.CommandName == "Select" )
            {
                if (e.Item is GridDataItem)
                {
                    GridDataItem dataItem = e.Item as GridDataItem;
                    dataItem.Selected = true;
                    GridDataItem dataItemRow = this.grdMessageList.SelectedItems[0] as GridDataItem;

                    string messageState = GetDataItemValue(dataItemRow, "State");

                    //if(messageState.ToUpper() == "UNPROCESSED")
                    //    Button btn = (LinkButton)dataItemRow["Select"].Controls[0];

                    string textID = GetDataItemValue(dataItemRow, "MessageID");
                    string messageType = GetDataItemValue(dataItemRow, "Type");
                    string userID = ((Entities.CustomPrincipal)Page.User).UserName;
                    int messageID;

                    if (int.TryParse(textID, out messageID))
                    {
                        success = facImportMessage.RetryMessage(messageID, messageType, userID);

                        if(success)
                            ClientScript.RegisterStartupScript(GetType(), "Error", "alert('Message successfully reset');", true);
                        else
                            ClientScript.RegisterStartupScript(GetType(), "Error", "alert('Unable to reset message, please try again');", true);

                    }
                }
            }

            grdMessageList.Rebind();

        }

        private string GetDataItemValue(GridDataItem dataItem, string columnName)
        {
            string retVal = dataItem[columnName].Text.Trim();
            if (retVal == "&nbsp;")
                retVal = string.Empty;
            return retVal;
        }

        private void grdMessageList_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            Facade.ImportMessage facImportMessage = new Facade.ImportMessage();
            string system = cboSystem.Text;
            string messageType = cboMessageType.SelectedValue;

            if (system != null && rdiStartDate.SelectedDate != null)
            {
                DataSet dsImportMessage = facImportMessage.GetAllFailedMessagesForDateRange((DateTime)rdiStartDate.SelectedDate, (DateTime)rdiEndDate.SelectedDate, system, messageType);
                this.grdMessageList.DataSource = dsImportMessage;
            }
            else
            {
                DataSet dsImportMessage = facImportMessage.GetAllFailedMessagesForDateRange(DateTime.Now.Date.AddDays(-8), DateTime.Now.Date, system, messageType);
                this.grdMessageList.DataSource = dsImportMessage;
            }
           
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (((DateTime)rdiEndDate.SelectedDate).Subtract((DateTime)rdiStartDate.SelectedDate).Days > 30)
                {
                    ClientScript.RegisterStartupScript(GetType(), "Error", "alert('The Date range is too large');", true);
                }
                else
                {
                    grdMessageList.Rebind();
                }
            }
        }

        private void cboSystem_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            cboSystem.Items.Clear();

            Facade.ImportMessage facImportMessages = new Facade.ImportMessage();
            DataSet ds = facImportMessages.GetAllSystems();

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
                rcItem.Text = dt.Rows[i]["System"].ToString();
                
                cboSystem.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }

            //if (dt.Rows.Count > 0)
            //    e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffSet.ToString(), dt.Rows.Count.ToString());
        }
    }
}