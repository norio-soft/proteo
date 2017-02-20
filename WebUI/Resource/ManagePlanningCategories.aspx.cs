using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Resource
{

    public partial class ManagePlanningCategories : Orchestrator.Base.BasePage
    {

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.grdPlanningCategories.ItemCreated += grdPlanningCategories_ItemCreated;
            this.grdPlanningCategories.NeedDataSource += grdPlanningCategories_NeedDataSource;
            this.grdPlanningCategories.InsertCommand += grdPlanningCategories_InsertCommand;
            this.grdPlanningCategories.UpdateCommand += grdPlanningCategories_UpdateCommand;
        }

        private void grdPlanningCategories_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            using (var ctx = new Orchestrator.EF.DataContext())
            {
                this.grdPlanningCategories.DataSource = ctx.PlanningCategorySet.ToList();
            }
        }

        private void grdPlanningCategories_ItemCreated(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridEditableItem && e.Item.IsInEditMode)
            {
                if (!e.Item.OwnerTableView.IsItemInserted)
                {
                    GridEditableItem item = e.Item as GridEditableItem;
                    GridEditManager manager = item.EditManager;
                    GridTextBoxColumnEditor editor = manager.GetColumnEditor("ID") as GridTextBoxColumnEditor;
                    editor.TextBoxControl.Enabled = false;
                }
            }
        }

        private void grdPlanningCategories_InsertCommand(object sender, GridCommandEventArgs e)
        {
            var item = e.Item as GridEditableItem;

            var newValues = new Hashtable();
            e.Item.OwnerTableView.ExtractValuesFromItem(newValues, item);

            using (var ctx = new Orchestrator.EF.DataContext())
            {
                var planningCategory = new Orchestrator.EF.PlanningCategory();

                planningCategory.DisplayShort = (string)newValues["DisplayShort"];
                planningCategory.DisplayLong = (string)newValues["DisplayLong"];
                planningCategory.LastUpdateDate = planningCategory.CreateDate = DateTime.Now;
                planningCategory.LastUpdateUserId = planningCategory.CreateUserId = User.Identity.Name;

                ctx.PlanningCategorySet.AddObject(planningCategory);
                
                ctx.SaveChanges();
            }
        }

        private void grdPlanningCategories_UpdateCommand(object sender, GridCommandEventArgs e)
        {
            var item = e.Item as GridEditableItem;
            var id = (int)item.GetDataKeyValue("ID");

            var newValues = new Hashtable();
            e.Item.OwnerTableView.ExtractValuesFromItem(newValues, item);

            using (var ctx = new Orchestrator.EF.DataContext())
            {
                var planningCategory = ctx.PlanningCategorySet.FirstOrDefault(pc => pc.ID == id);
                
                planningCategory.DisplayShort = (string)newValues["DisplayShort"];
                planningCategory.DisplayLong = (string)newValues["DisplayLong"];
                planningCategory.LastUpdateDate = DateTime.Now;
                planningCategory.LastUpdateUserId = User.Identity.Name;
                
                ctx.SaveChanges();
            }

            item.Edit = false;
        }

    }

}
