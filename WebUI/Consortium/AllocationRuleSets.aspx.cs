using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Consortium
{
    public partial class AllocationRuleSets : Orchestrator.Base.BasePage
    {

        private int AllocationRuleSetID
        {
            get { return (int)ViewState["AllocationRuleSetID"]; }
            set { ViewState["AllocationRuleSetID"] = value; }
        }

        private IEnumerable<EF.AllocationPointTable> _pointTables = null;
        private IEnumerable<EF.AllocationPointTable> PointTables
        {
            get { return _pointTables ?? (_pointTables = this.DataContext.AllocationPointTableSet.OrderBy(apt => apt.Description)); }
        }

        private IEnumerable<EF.AllocationZoneTable> _zoneTables = null;
        private IEnumerable<EF.AllocationZoneTable> ZoneTables
        {
            get { return _zoneTables ?? (_zoneTables = this.DataContext.AllocationZoneTableSet.OrderBy(azt => azt.Description)); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int? id = Utilities.ParseNullable<int>(Request.QueryString["rsid"]);
                if (id.HasValue)
                {
                    this.AllocationRuleSetID = id.Value;
                    LoadAllocationRuleSet();
                }
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.Title = "Orchestrator - Allocation Rule Sets";

            grdRuleSets.NeedDataSource += new GridNeedDataSourceEventHandler(grdRuleSets_NeedDataSource);
            lvPointTables.ItemDataBound += new EventHandler<ListViewItemEventArgs>(lvPointTables_ItemDataBound);
            lvZoneTables.ItemDataBound += new EventHandler<ListViewItemEventArgs>(lvZoneTables_ItemDataBound);
            btnSave.Click += new EventHandler(btnSave_Click);
        }

        private void grdRuleSets_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            grdRuleSets.DataSource =
                from ars in this.DataContext.AllocationRuleSetSet
                orderby ars.Description
                select new
                {
                    ars.AllocationRuleSetID,
                    ars.Description
                };
        }

        private void lvPointTables_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            var data = ((ListViewDataItem)e.Item).DataItem.CastByExample(new { AllocationRulePointTableID = 0, Description = string.Empty, AllocationPointTableID = (int?)null });

            var cboPointTable = (RadComboBox)e.Item.FindControl("cboPointTable");
            cboPointTable.DataSource = this.PointTables;
            cboPointTable.DataBind();

            cboPointTable.SelectedValue = data.AllocationPointTableID.ToString();
        }

        private void lvZoneTables_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            var data = ((ListViewDataItem)e.Item).DataItem.CastByExample(new { AllocationRuleZoneTableID = 0, Description = string.Empty, AllocationZoneTableID = (int?)null });

            var cboZoneTable = (RadComboBox)e.Item.FindControl("cboZoneTable");
            cboZoneTable.DataSource = this.ZoneTables;
            cboZoneTable.DataBind();

            cboZoneTable.SelectedValue = data.AllocationZoneTableID.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveAllocationRuleSet();
            grdRuleSets.Rebind();
            LoadAllocationRuleSet();
        }

        private void LoadAllocationRuleSet()
        {
            var allocationRuleSet =
                this.DataContext.AllocationRuleSetSet
                .Include("PointTables.AllocationPointTable")
                .Include("ZoneTables.AllocationZoneTable")
                .FirstOrDefault(ars => ars.AllocationRuleSetID == this.AllocationRuleSetID);

            if (allocationRuleSet == null)
                throw new InvalidOperationException(string.Format("AllocationRuleSet with ID {0} cannot be found", this.AllocationRuleSetID));

            pnlRuleSet.Visible = true;
            pnlRuleSet.GroupingText = string.Format("Rule Set \"{0}\"", allocationRuleSet.Description);
            txtDescription.Text = allocationRuleSet.Description;

            lvPointTables.DataSource =
                from pt in allocationRuleSet.PointTables
                orderby pt.Index
                select new
                {
                    pt.AllocationRulePointTableID,
                    pt.Description,
                    AllocationPointTableID = pt.AllocationPointTable == null ? (int?)null : pt.AllocationPointTable.AllocationPointTableID,
                };
            
            lvPointTables.DataBind();

            lvZoneTables.DataSource =
                from zt in allocationRuleSet.ZoneTables
                orderby zt.Index
                select new
                {
                    zt.AllocationRuleZoneTableID,
                    zt.Description,
                    AllocationZoneTableID = zt.AllocationZoneTable == null ? (int?)null : zt.AllocationZoneTable.AllocationZoneTableID,
                };

            lvZoneTables.DataBind();
        }

        private void SaveAllocationRuleSet()
        {
            var pointTables = lvPointTables.Items
                .ToDictionary(
                    i => (int)lvPointTables.DataKeys[i.DataItemIndex].Value,
                    i => int.Parse(((RadComboBox)i.FindControl("cboPointTable")).SelectedValue));

            var zoneTables = lvZoneTables.Items
                .ToDictionary(
                    i => (int)lvZoneTables.DataKeys[i.DataItemIndex].Value,
                    i => int.Parse(((RadComboBox)i.FindControl("cboZoneTable")).SelectedValue));

            Facade.IAllocation facAllocation = new Facade.Allocation();
            facAllocation.UpdateAllocationRuleSet(this.AllocationRuleSetID, txtDescription.Text, pointTables, zoneTables);
        }

    }
}
