using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.UI.HtmlControls;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.GoodsRefused
{
    public partial class listShortsAndOvers : Orchestrator.Base.BasePage
    {

        //----------------------------------------------------------------------------------

        private DataSet m_ds = null;

        //----------------------------------------------------------------------------------

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        //----------------------------------------------------------------------------------

        protected void listShortsAndOvers_Init(object sender, EventArgs e)
        {
           //this.cboClient.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);

            this.btnSearch.Click += new EventHandler(btnSearch_Click);
            this.btnSearchBottom.Click += new EventHandler(btnSearchBottom_Click);

            this.btnSave.Click += new EventHandler(btnSave_Click);
            this.btnSaveBottom.Click += new EventHandler(btnSaveBottom_Click);

            this.grdRefusals.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdRefusals_NeedDataSource);
            this.grdRefusals.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(grdRefusals_ItemDataBound);

            this.dlgRefusal.DialogCallBack += new EventHandler(dlgRefusal_DialogCallBack);
        }

        void btnSaveBottom_Click(object sender, EventArgs e)
        {
            this.Save();
        }

        void btnSave_Click(object sender, EventArgs e)
        {
            this.Save();
        }

        private void Save()
        {
            List<int> refusalIdsChecked = new List<int>();
            List<int> refusalIdsNotChecked = new List<int>();

            foreach (GridItem item in grdRefusals.Items)
            {
                if (item is GridDataItem)
                {
                    using (CheckBox chk = (CheckBox)item.FindControl("chkSelectShort"))
                    {
                        int refusalId = int.Parse(chk.Attributes["RefusalId"]);
                        if (chk.Checked)
                            refusalIdsChecked.Add(refusalId);
                        else
                            refusalIdsNotChecked.Add(refusalId);
                    }
                }
            }

            Facade.IGoodsRefusal facGoodsRefusal = new Facade.GoodsRefusal();
            if (refusalIdsChecked.Count > 0)
                facGoodsRefusal.UpdateShortAndOverChecked(refusalIdsChecked, true, ((Entities.CustomPrincipal)Page.User).UserName);

            if (refusalIdsNotChecked.Count > 0)
                facGoodsRefusal.UpdateShortAndOverChecked(refusalIdsNotChecked, false, ((Entities.CustomPrincipal)Page.User).UserName);

            grdRefusals.Rebind();
        }

        //----------------------------------------------------------------------------------

        void dlgRefusal_DialogCallBack(object sender, EventArgs e)
        {
            if (this.dlgRefusal.ReturnValue == "Refresh_Redeliveries_And_Refusals")
            {
                GetData();
            }
        }

        //----------------------------------------------------------------------------------

        void btnSearchBottom_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
                GetData();
        }

        //----------------------------------------------------------------------------------

        void btnSearch_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
                GetData();
        }

        //----------------------------------------------------------------------------------

        private void GetData()
        {
            grdRefusals.Rebind();
        }

        //----------------------------------------------------------------------------------

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Init += new System.EventHandler(this.listShortsAndOvers_Init);
        }
        #endregion

        //----------------------------------------------------------------------------------

        void grdRefusals_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;

                // Selection checkbox.
                using (CheckBox chk = (CheckBox)e.Item.FindControl("chkSelectShort"))
                {
                    chk.Attributes.Add("RefusalId", drv["RefusalId"].ToString());
                    chk.Checked = Convert.ToBoolean(drv["Checked"].ToString());
                }

                DateTime startOfRefusalDate = (DateTime)drv["CollectDropDateTime"];
                startOfRefusalDate = startOfRefusalDate.Subtract(startOfRefusalDate.TimeOfDay);

                HtmlAnchor hypOriginalOrder = (HtmlAnchor)e.Item.FindControl("hypOriginalOrder");
                if (drv["OriginalOrderId"] != DBNull.Value)
                {
                    int orderId = (int)drv["OriginalOrderId"];
                    string queryString = string.Format("oid={0}", orderId.ToString());
                    hypOriginalOrder.HRef = string.Format("javascript:{0}", this.dlgOrder.GetOpenDialogScript(queryString));
                    hypOriginalOrder.InnerText = orderId.ToString();
                }
            }
        }

        //----------------------------------------------------------------------------------

        private string ConvertApostrophesToASCII(string originalString)
        {
            return originalString.Replace("'", "&#39"); // This is the ASCII Numerical Code for the ' symbol.
        }

        //----------------------------------------------------------------------------------

        void grdRefusals_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {

            Facade.IGoodsRefusal facGoods = new Facade.GoodsRefusal();

            int clientId = cboClient.SelectedValue == "" ? 0 : Convert.ToInt32(cboClient.SelectedValue);
            int collectionPointId = ucCollectionPoint.SelectedPoint == null ? 0 : ucCollectionPoint.SelectedPoint.PointId;

            DateTime startDate = rdiStartDate.SelectedDate.HasValue ? rdiStartDate.SelectedDate.Value : new DateTime(1900,01,01);
            DateTime endDate = rdiEndDate.SelectedDate.HasValue ? rdiEndDate.SelectedDate.Value : new DateTime(1900, 01, 01);
            //Set the date range to be the entire day
            endDate = endDate.Subtract(endDate.TimeOfDay).Add(new TimeSpan(23, 59, 59));
            m_ds = facGoods.GetShortsAndOversWithDates(clientId, startDate, endDate,collectionPointId,chkAlreadyChecked.Checked);

            grdRefusals.DataSource = m_ds;
        }

        //----------------------------------------------------------------------------------

    }

    //----------------------------------------------------------------------------------

}