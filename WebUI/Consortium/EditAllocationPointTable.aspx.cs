using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Telerik.Web.UI;
using System.Web.Script.Serialization;

namespace Orchestrator.WebUI.Consortium
{
    public partial class EditAllocationPointTable : Orchestrator.Base.BasePage
    {

        private struct Allocation { public int PointID { get; set; } public int ConsortiumMemberIdentityID { get; set; } }

        #region Properties

        private int TableID
        {
            get { return (int)ViewState["AllocationPointTableID"]; }
            set { ViewState["AllocationPointTableID"] = value; }
        }

        #endregion

        #region Event Handlers

        protected void Page_Init(object sender, EventArgs e)
        {
            btnSave.Click += new EventHandler(btnSave_Click);

            this.Title = "Orchestrator - Edit Allocation Point Table";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                int? tableID = Utilities.ParseNullable<int>(Request.QueryString["aptid"]);
                if (!tableID.HasValue)
                    throw new InvalidOperationException("Cannot view edit allocation point table page without passing the table id in the query string");

                this.TableID = tableID.Value;

                LoadAllocationPointTable();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveAllocationPointTable();
            LoadAllocationPointTable();
        }

        #endregion

        #region Data Loading

        private void LoadAllocationPointTable()
        {
            var table =
                this.DataContext.AllocationPointTableSet
                .Include("AllocationPoints.ConsortiumMember")
                .Include("AllocationPoints.Point")
                .First(t => t.AllocationPointTableID == this.TableID);

            txtDescription.Text = table.Description;

            var allocationPoints = table.AllocationPoints.Select(ap => new
                {
                    ConsortiumMemberIdentityID = ap.ConsortiumMember.IdentityId,
                    ConsortiumMemberName = ap.ConsortiumMember.OrganisationName,
                    PointID = ap.Point.PointId,
                    PointDescription = ap.Point.Description
                });

            lvAllocations.DataSource = allocationPoints;
            lvAllocations.DataBind();

            foreach (var ap in allocationPoints)
            {
                string jsonAllocation = new JavaScriptSerializer().Serialize(new Allocation { PointID = ap.PointID, ConsortiumMemberIdentityID = ap.ConsortiumMemberIdentityID });
                ScriptManager.RegisterArrayDeclaration(this, "allocations", jsonAllocation);
            }
        }

        #endregion

        #region Data Saving

        private void SaveAllocationPointTable()
        {
            var allocations = new JavaScriptSerializer().Deserialize<IEnumerable<Allocation>>(hidAllocations.Value);
            
            Facade.IAllocation facAllocation = new Facade.Allocation();
            facAllocation.UpdateAllocationPointTable(
                this.TableID,
                txtDescription.Text,
                allocations.ToDictionary(a => a.PointID, a => a.ConsortiumMemberIdentityID));
        }

        #endregion

    }
}

