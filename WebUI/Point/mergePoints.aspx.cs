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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Web.SessionState;
using Telerik.Web.UI;
using Orchestrator.Globals;
using System.Text;

namespace Orchestrator.WebUI.Point
{

    public partial class Point_mergePoints : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditOrganisations, eSystemPortion.AddEditPoints, eSystemPortion.GeneralUsage);
            btnMergePoints.Enabled = Orchestrator.WebUI.Security.Authorise.CanAccess(eSystemPortion.AddEditOrganisations, eSystemPortion.AddEditPoints);

            if (!string.IsNullOrEmpty(Request.QueryString["rcbID"]))
                return;
            //btnMergePoints.Attributes.Add("onclick", string.Format("javascript:giveWarning();"));

        }


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);


            this.repPoints.NeedDataSource += new GridNeedDataSourceEventHandler(repPoints_NeedDataSource);
            this.repPoints.ItemDataBound += new GridItemEventHandler(repPoints_ItemDataBound);
            this.btnMergePoints.Click +=new EventHandler(btnMergePoints_Click);
            this.ucPoint.SelectedPointChanged += new SelectedPointChangedEventHandler(ucPoint_SelectedPointChanged);
            this.btnRefresh.Click += new EventHandler(btnRefresh_Click);
            this.pointDescriptionRadSlider.ValueChanged += new EventHandler(pointDescriptionRadSlider_ValueChanged);
            this.addressLine1RadSlider.ValueChanged += new EventHandler(addressLine1RadSlider_ValueChanged);
            this.townRadSlider.ValueChanged += new EventHandler(townRadSlider_ValueChanged);
        }

        void repPoints_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            if (this.ucPoint.SelectedPoint != null || this.ucPoint.PointID > 0)
            {
                BusinessLogicLayer.IPoint busPoint = new BusinessLogicLayer.Point();
                Entities.Point point = (this.ucPoint.SelectedPoint != null) ? this.ucPoint.SelectedPoint : null;

                if (point == null)
                    point = busPoint.GetPointForPointId(this.ucPoint.PointID);

                if (point == null)
                {
                    this.ucPoint.SelectedPoint = null;
                    this.ucPoint.PointID = -1;
                    this.repPoints.DataSource = new DataTable();
                }
                else
                {

                    this.repPoints.DataSource = new DataTable();
                    repPoints.DataSource = busPoint.GetFuzzyPointsToMerge(this.ucPoint.SelectedPoint.Description, float.Parse(this.pointDescriptionRadSlider.SelectedValue), String.Empty, 0, this.ucPoint.SelectedPoint.Address.AddressLine1,
                        float.Parse(this.addressLine1RadSlider.SelectedValue), this.ucPoint.SelectedPoint.Address.PostTown, float.Parse(this.townRadSlider.SelectedValue),
                        this.chkMatchOnPostCode.Checked ? this.ucPoint.SelectedPoint.Address.PostCode : String.Empty, 0);
                }
            }
            else
                this.repPoints.DataSource = new DataTable();
        }

        void townRadSlider_ValueChanged(object sender, EventArgs e)
        {
            this.repPoints.Rebind();
        }

        void addressLine1RadSlider_ValueChanged(object sender, EventArgs e)
        {
            this.repPoints.Rebind();
        }

        void pointDescriptionRadSlider_ValueChanged(object sender, EventArgs e)
        {
            this.repPoints.Rebind();
        }

        void btnRefresh_Click(object sender, EventArgs e)
        {
            this.repPoints.Rebind();
        }

        void ucPoint_SelectedPointChanged(object sender, SelectedPointChangedEventArgs e)
        {
            if (e.SelectedPoint != null)
                this.repPoints.Rebind();
            else
                this.repPoints.DataSource = null;
        }
    
        #region Event Handlers

        #region ComboBox Event Handlers

        private void LoadData()
        {
            if (this.ucPoint.SelectedPoint != null)
            {
                BusinessLogicLayer.IPoint busPoint = new BusinessLogicLayer.Point();
                repPoints.DataSource = busPoint.GetFuzzyPointsToMerge(this.ucPoint.SelectedPoint.Description, float.Parse(this.pointDescriptionRadSlider.SelectedValue), String.Empty, 0, this.ucPoint.SelectedPoint.Address.AddressLine1,
                    float.Parse(this.addressLine1RadSlider.SelectedValue), this.ucPoint.SelectedPoint.Address.PostTown, float.Parse(this.townRadSlider.SelectedValue),
                    this.chkMatchOnPostCode.Checked ? this.ucPoint.SelectedPoint.Address.PostCode : String.Empty, 0);
            }
            else
                this.repPoints.DataSource = null;
        }

        void repPoints_ItemDataBound(object o, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.AlternatingItem || e.Item.ItemType == GridItemType.Item)
            {
                RadioButton rbMasterPoint = (RadioButton)e.Item.FindControl("btnRadioKeep");
                CheckBox chkMergeTo = (CheckBox)e.Item.FindControl("chkRow");

                rbMasterPoint.Attributes.Add("PointID", ((DataRowView)e.Item.DataItem)["PointId"].ToString());
                chkMergeTo.Attributes.Add("PointID", ((DataRowView)e.Item.DataItem)["PointId"].ToString());

                if (((DataRowView)e.Item.DataItem)["OrganisationLocationTypeId"] != DBNull.Value)
                {
                    e.Item.BackColor = Color.Silver;
                    chkMergeTo.Attributes.Add("Style", "Display:none");
                }

                if (rbMasterPoint != null)
                {
                    string pointID = ((DataRowView)e.Item.DataItem)["PointId"].ToString();
                    rbMasterPoint.Attributes.Add("onclick",string.Format("checkMergeTo({0}, {1}, {2});", rbMasterPoint.ClientID, chkMergeTo.ClientID, pointID));
                    rbMasterPoint.Attributes.Add("RichPointId", pointID);
                }
            }
        }

        protected void repPoints_Click(object o, EventArgs e)
        {

        }

        #endregion

        #endregion

        [System.Web.Services.WebMethod]
        public static string MergePoint(int mergeToPoint, int mergeFrom, string userName)
        {
            string pointCompleted = String.Empty;
            try
            {
                
                Facade.IPoint facPoint = new Facade.Point();
                List<Entities.Point> mergedPoints = new List<Entities.Point>();
                mergedPoints = facPoint.MergePoints(mergeToPoint, mergeFrom.ToString(), userName);

                if (mergedPoints.Count > 0)
                    pointCompleted = mergedPoints[0].PointId + "," + mergedPoints[0].Description + " has been merged successfully.";
                else
                {
                    throw new Exception("Point " + mergeFrom + " could not be merged.");
                }

            }
            catch(Exception ex)
            {
                pointCompleted = mergeFrom + "," + ex.Message;
            }

            return pointCompleted;
        }

        protected void btnMergePoints_Click(object sender, EventArgs e)
        {
            Facade.IPoint facPoint = new Facade.Point();
            List<Entities.Point> mergedPoints = new List<Entities.Point>();
            string userId = ((Entities.CustomPrincipal)Page.User).UserName;
            RadioButton rdoButton = null;
            CheckBox chkBox = null;
            string mergeToPoint = "";
            StringBuilder mergeFromPoints = new StringBuilder();

            foreach (Telerik.Web.UI.GridItem gi in repPoints.Items)
            {
                rdoButton = (RadioButton)gi.FindControl("btnRadioKeep");
                chkBox = (CheckBox)gi.FindControl("chkRow");
                if (rdoButton.Checked == true)
                    mergeToPoint = rdoButton.Attributes["PointID"];
                if (chkBox.Checked)
                {
                    if (mergeFromPoints.Length == 0)
                    {
                        mergeFromPoints = mergeFromPoints.Append(chkBox.Attributes["PointID"]);
                    }
                    else
                        mergeFromPoints = mergeFromPoints.Append("," + chkBox.Attributes["PointID"]);
                }
            }
               
            if (mergeFromPoints.Length > 0 && mergeToPoint.Length > 0)
            {
                mergedPoints = facPoint.MergePoints(Convert.ToInt32(mergeToPoint), mergeFromPoints.ToString(), userId);
                repPoints.Rebind();
            }
        }
    }
}
