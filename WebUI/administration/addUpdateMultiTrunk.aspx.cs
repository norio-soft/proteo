using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;
using System.Globalization;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.administration
{

    //--------------------------------------------------------------------------------------------------------

    public partial class addUpdateMultiTrunk : Orchestrator.Base.BasePage
    {

        //--------------------------------------------------------------------------------------------------------

        private Entities.MultiTrunk multiTrunk = null;
        private string vsMultiTrunkName = "VS_MULTITRUNK";

        //--------------------------------------------------------------------------------------------------------

        public Entities.MultiTrunk VS_MultiTrunk
        {
            get 
            {
                // try and get the objcet from view state
                object vsMultiTrunk = ViewState[vsMultiTrunkName];

                if (vsMultiTrunk != null)
                    return (Entities.MultiTrunk)vsMultiTrunk;
                else
                    return null;
            }

            set 
            {
                ViewState[vsMultiTrunkName] = value;
            }
        }
       
        //--------------------------------------------------------------------------------------------------------

        protected void Page_Load(object sender, EventArgs e)
        {
            Label PageTitle = ((System.Web.UI.UserControl)(Page.Master)).FindControl("lblWizardTitle") as Label;

            if (PageTitle != null)
                PageTitle.Text = "Add-Update Multi-Trunk";

            this.PopulateMultiTrunk();
        }

        //--------------------------------------------------------------------------------------------------------

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.btnAddTrunkPoint.Click += new EventHandler(btnAddTrunkPoint_Click);
            this.btnSaveMultiTrunk.Click += new EventHandler(btnSaveMultiTrunk_Click);
            this.repeatCurrentTrunkPoints.ItemCommand += new RepeaterCommandEventHandler(repeatCurrentTrunkPoints_ItemCommand);
            this.repeatCurrentTrunkPoints.ItemDataBound += new RepeaterItemEventHandler(repeatCurrentTrunkPoints_ItemDataBound);
        }

        //--------------------------------------------------------------------------------------------------------

        public void repeatCurrentTrunkPoints_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType !=
                ListItemType.AlternatingItem)
                return;

            if (((Entities.MultiTrunkPoint)e.Item.DataItem).Order == 0)
            {
                ImageButton btnUp = (ImageButton)e.Item.FindControl("imgBtnUp");
                btnUp.Visible = false;

                RadNumericTextBox txtMinutesFromPreviousPoint = (RadNumericTextBox)e.Item.FindControl("txtMinutesFromPreviousPoint");
                txtMinutesFromPreviousPoint.Visible = false;
            }

            if (((Entities.MultiTrunkPoint)e.Item.DataItem).Order == this.multiTrunk.TrunkPoints.Count - 1)
            {
                ImageButton btnDown = (ImageButton)e.Item.FindControl("imgBtnDown");
                btnDown.Visible = false;
            }

        }

        //--------------------------------------------------------------------------------------------------------

        public void repeatCurrentTrunkPoints_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if(e.CommandName == "Up")
            {
                int currentIndex = Convert.ToInt32(e.CommandArgument);
                int insertAt = currentIndex - 1;

                this.multiTrunk.TrunkPoints.Insert(insertAt, this.multiTrunk.TrunkPoints[currentIndex]);

                this.multiTrunk.TrunkPoints.RemoveAt(currentIndex + 1);

                if (currentIndex == 1)
                    this.multiTrunk.TrunkPoints[currentIndex].MinutesFromPreviousPoint = 60;

                this.PopulateTrunkPointsRepeater(this.multiTrunk.TrunkPoints);
            }
            else if(e.CommandName == "Down")
            {
                int currentIndex = Convert.ToInt32(e.CommandArgument);
                int insertAt = currentIndex + 1;

                if (insertAt == this.multiTrunk.TrunkPoints.Count - 1)
                    this.multiTrunk.TrunkPoints.Add(this.multiTrunk.TrunkPoints[currentIndex]);
                else
                    this.multiTrunk.TrunkPoints.Insert(++insertAt, this.multiTrunk.TrunkPoints[currentIndex]);

                this.multiTrunk.TrunkPoints.RemoveAt(currentIndex);

                if (currentIndex == 0)
                    this.multiTrunk.TrunkPoints[currentIndex + 1].MinutesFromPreviousPoint = 60;

                this.PopulateTrunkPointsRepeater(this.multiTrunk.TrunkPoints);
            }
            else if (e.CommandName == "Delete")
            {
                int currentIndex = Convert.ToInt32(e.CommandArgument);

                this.multiTrunk.TrunkPoints.RemoveAt(currentIndex);

                this.PopulateTrunkPointsRepeater(this.multiTrunk.TrunkPoints);
            }
        }

        //--------------------------------------------------------------------------------------------------------

        public void btnSaveMultiTrunk_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(this.txtMultiTrunkName.Text))
            {
                if (this.multiTrunk != null && this.multiTrunk.TrunkPoints != null
                    && this.multiTrunk.TrunkPoints.Count > 0)
                {
                    this.multiTrunk.Description = this.txtMultiTrunkName.Text.Trim();
                    this.multiTrunk.IsEnabled = this.chkIsEnabled.Checked;

                    if (this.multiTrunk.MultiTrunkId == -1)
                        this.multiTrunk.CreateUserId = ((Entities.CustomPrincipal)this.Page.User).UserName;

                    this.multiTrunk.LastUpdateUserId = ((Entities.CustomPrincipal)this.Page.User).UserName;

                    this.UpdatePointMinutes();

                    Facade.MultiTrunk facMultiTrunk = new Orchestrator.Facade.MultiTrunk();

                    if (this.multiTrunk.MultiTrunkId > 0)
                        facMultiTrunk.Update(this.multiTrunk);
                    else
                        facMultiTrunk.Create(this.multiTrunk);

                    this.VS_MultiTrunk = this.multiTrunk;

                    this.ClientScript.RegisterClientScriptBlock(this.GetType(), "scriptCloseWindow",
                          @"<script language='javascript' type='text/javascript'>
                                    GetRadWindow().Close();
                            </script>");
                }
            }
        }

        //--------------------------------------------------------------------------------------------------------

        private void UpdatePointMinutes()
        {
            // update the minutesFromPreviousPoint property of points using the textbox values
            foreach (Entities.MultiTrunkPoint trunkPoint in this.multiTrunk.TrunkPoints)
               foreach (RepeaterItem item in this.repeatCurrentTrunkPoints.Items)
                    if (item.ItemIndex == trunkPoint.Order)
                        if (trunkPoint.Order == 0)
                            trunkPoint.MinutesFromPreviousPoint = -1;
                        else
                        {
                            RadNumericTextBox txtMinutesFromPreviousPoint = (RadNumericTextBox)item.FindControl("txtMinutesFromPreviousPoint");
                            trunkPoint.MinutesFromPreviousPoint = Convert.ToInt32(txtMinutesFromPreviousPoint.Value);
                        }     
        }

        //--------------------------------------------------------------------------------------------------------

        public void PopulateMultiTrunk()
        {
            // check to see if we have an object in viewstate
            this.multiTrunk = this.VS_MultiTrunk;

            if (this.multiTrunk == null)
            {
                // the object didn't exist in view state so get it from thw database using the id 
                object multiTrunkId = Request.QueryString["multiTrunkId"];
                if (multiTrunkId != null && Convert.ToInt32(multiTrunkId) > 0)
                {
                    Facade.MultiTrunk facMultiTrunk = new Orchestrator.Facade.MultiTrunk();
                    this.multiTrunk = facMultiTrunk.GetForMultiTrunkID(Convert.ToInt32(multiTrunkId));

                    //updates the viewstate
                    this.VS_MultiTrunk = this.multiTrunk;

                    this.txtMultiTrunkName.Text = this.multiTrunk.Description;
                    this.chkIsEnabled.Checked = this.multiTrunk.IsEnabled;

                    this.PopulateTrunkPointsRepeater(this.multiTrunk.TrunkPoints);
                }
            }

            this.ShowHideDurationFromLastPoint();
        }

        //--------------------------------------------------------------------------------------------------------

        public void PopulateTrunkPointsRepeater(List<Entities.MultiTrunkPoint> trunkPoints)
        {
            // reset the order property of the objects to reflect the current  
            // order of the colection.
            for (int i = 0; i < this.multiTrunk.TrunkPoints.Count; i++)
                this.multiTrunk.TrunkPoints[i].Order = i;

            this.VS_MultiTrunk = this.multiTrunk;

            this.repeatCurrentTrunkPoints.DataSource = trunkPoints;
 
            this.repeatCurrentTrunkPoints.DataBind();

            this.ShowHideDurationFromLastPoint();
        }

        //--------------------------------------------------------------------------------------------------------

        public void btnAddTrunkPoint_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(this.txtMultiTrunkName.Text.Trim()))
            {
                // if this is the first point being added the multi-trunk object will be null
                if (this.multiTrunk == null)
                {
                    this.multiTrunk = new Orchestrator.Entities.MultiTrunk();
                    this.multiTrunk.MultiTrunkId = -1;
                }

                this.multiTrunk.IsEnabled = this.chkIsEnabled.Checked;
                this.multiTrunk.Description = this.txtMultiTrunkName.Text.Trim();

                if(this.multiTrunk.TrunkPoints == null)
                    this.multiTrunk.TrunkPoints = new List<Orchestrator.Entities.MultiTrunkPoint>();

                Entities.MultiTrunkPoint trunkPoint =
                    new Orchestrator.Entities.MultiTrunkPoint(-1, this.multiTrunk.MultiTrunkId, this.ucCollectionPoint.PointID,
                        Convert.ToInt32(this.txtDefaultDuration.Value), this.multiTrunk.TrunkPoints.Count);

                Facade.Point facPoint = new Orchestrator.Facade.Point();
                trunkPoint.Point = facPoint.GetPointForPointId(trunkPoint.PointId);

                this.multiTrunk.TrunkPoints.Add(trunkPoint);

                this.PopulateTrunkPointsRepeater(this.multiTrunk.TrunkPoints);

                this.ucCollectionPoint.Reset();
                this.txtDefaultDuration.Text = String.Empty;

                this.VS_MultiTrunk = this.multiTrunk;
            }
        }

        //--------------------------------------------------------------------------------------------------------

        public void ShowHideDurationFromLastPoint()
        {
            if (this.multiTrunk == null || this.multiTrunk.TrunkPoints == null ||
                this.multiTrunk.TrunkPoints.Count == 0)
                this.tblTimeFromLastPointRow.Visible = false;
            else
                this.tblTimeFromLastPointRow.Visible = true;
        }

        //--------------------------------------------------------------------------------------------------------
    }

    //--------------------------------------------------------------------------------------------------------

}
