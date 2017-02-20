using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Telerik.Web.UI;

namespace Orchestrator.WebUI.Consortium
{
    public partial class EditAllocationZoneTable : Orchestrator.Base.BasePage
    {

		#region Private Classes

        private struct ZoneWithAllocation
        {
            public int ZoneID { get; set; }
            public string Description { get; set; }
            public int? ConsortiumMemberIdentityID { get; set; }
            public string ConsortiumMemberName { get; set; }
        }

 		#endregion Private Classes

        #region Properties

        private int TableID
        {
            get { return (int)ViewState["AllocationZoneTableID"]; }
            set { ViewState["AllocationZoneTableID"] = value; }
        }

        #endregion

        #region Event Handlers

        protected void Page_Init(object sender, EventArgs e)
        {
            lvZones.ItemDataBound += new EventHandler<ListViewItemEventArgs>(lvZones_ItemDataBound);
            btnSave.Click += new EventHandler(btnSave_Click);

            this.Title = "Orchestrator - Edit Allocation Zone Table";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                int? tableID = Utilities.ParseNullable<int>(Request.QueryString["aztid"]);
                if (!tableID.HasValue)
                    throw new InvalidOperationException("Cannot view edit allocation zone table page without passing the table id in the query string");

                this.TableID = tableID.Value;
                LoadAllocationZoneTable();
            }
        }

        private void lvZones_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            var item = e.Item as ListViewDataItem;
            if (item == null)
                return;

            var allocationZone = (ZoneWithAllocation)item.DataItem;

            if (allocationZone.ConsortiumMemberIdentityID.HasValue)
            {
                var cboConsortiumMember = (RadComboBox)item.FindControl("cboConsortiumMember");
                cboConsortiumMember.SelectedValue = allocationZone.ConsortiumMemberIdentityID.Value.ToString();
                cboConsortiumMember.Text = allocationZone.ConsortiumMemberName;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveAllocationZoneTable();
            LoadAllocationZoneTable();
        }

        #endregion

        #region Data Loading

        private void LoadAllocationZoneTable()
        {
            var table =
                this.DataContext.AllocationZoneTableSet
                .Include("ZoneMap.Zones")
                .Include("AllocationZones.ConsortiumMember")
                .First(t => t.AllocationZoneTableID == this.TableID);

            txtDescription.Text = table.Description;
            lblZoneMap.Text = table.ZoneMap.Description;

            var zones =
                from z in table.ZoneMap.Zones
                orderby z.Description
                let az = table.AllocationZones.FirstOrDefault(i => i.Zone.ZoneId == z.ZoneId)
                let isAllocated = az != null && az.ConsortiumMember != null
                select new ZoneWithAllocation
                {
                    ZoneID = z.ZoneId,
                    Description = z.Description,
                    ConsortiumMemberIdentityID = isAllocated ? (int?)az.ConsortiumMember.IdentityId : null,
                    ConsortiumMemberName = isAllocated ? az.ConsortiumMember.OrganisationName : null,
                };

            lvZones.DataSource = zones;
            lvZones.DataBind();
        }

        #endregion

        #region Data Saving

        private void SaveAllocationZoneTable()
        {
            var allocations = lvZones.Items.ToDictionary(
                i => (int)lvZones.DataKeys[i.DataItemIndex].Value,
                i => Utilities.ParseNullable<int>(((RadComboBox)i.FindControl("cboConsortiumMember")).SelectedValue));
            
            Facade.IAllocation facAllocation = new Facade.Allocation();
            facAllocation.UpdateAllocationZoneTable(
                this.TableID,
                txtDescription.Text,
                allocations);
        }

        #endregion

    }
}

