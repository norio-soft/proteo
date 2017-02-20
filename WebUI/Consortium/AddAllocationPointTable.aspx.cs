using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Telerik.Web.UI;

namespace Orchestrator.WebUI.Consortium
{
    public partial class AddAllocationPointTable : Orchestrator.Base.BasePage
    {

        protected void Page_Init(object sender, EventArgs e)
        {
            btnSave.Click += new EventHandler(btnSave_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);

            this.Title = "Orchestrator - Add Allocation Point Table";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                PopulateStaticFields();
                txtDescription.Focus();
            }
        }

        private void PopulateStaticFields()
        {
            var tables = this.DataContext.AllocationPointTableSet.OrderBy(t => t.Description);

            bool canCopy = tables.Any();
            optCopyTable.Checked = canCopy;
            optCopyTable.Enabled = canCopy;
            optEmptyTable.Checked = !canCopy;

            cboCopyFromTable.DataSource = tables;
            cboCopyFromTable.DataBind();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //If the Copy option is selected get the selected AllocationPointTableID and use it to copy the table
            EF.AllocationPointTable table;
            Facade.IAllocation facAllocation = new Facade.Allocation();

            string description = txtDescription.Text;

            if (optCopyTable.Checked)
            {
                int? tableID = Utilities.ParseNullable<int>(cboCopyFromTable.SelectedValue);
                if (!tableID.HasValue)
                    throw new InvalidOperationException("Can not copy the allocation point table because no valid existing table has been selected.");
                table = facAllocation.CopyAllocationPointTable(tableID.Value, description);
            }
            else
                table = facAllocation.CreateEmptyAllocationPointTable(description);

            //Redirect to EditAllocationPointTable page so that the new table can be edited
            this.Response.Redirect(string.Format(
                "editallocationpointtable.aspx?aptid={0}",
                table.AllocationPointTableID));
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Response.Redirect("allocationpointtablelist.aspx");
        }

    }
}
