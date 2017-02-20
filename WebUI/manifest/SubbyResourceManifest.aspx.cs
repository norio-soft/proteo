using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Telerik.WebControls;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.manifest
{
    public partial class SubbyResourceManifest : Orchestrator.Base.BasePage
    {
        #region Properties

        private const string REMOVED_JOB_INSTRUCTION_TEXT = "Job removed";

        private Entities.ResourceManifest SavedResourceManifest
        {
            get
            {
                if (this.ViewState["ResourceManifest"] == null)
                    return null;
                else
                    return (Entities.ResourceManifest)this.ViewState["ResourceManifest"];
            }
            set
            {
                this.ViewState["ResourceManifest"] = value;
            }
        }

        private int ResourceManifestId
        {
            get
            {
                if (this.ViewState["ResourceManifestId"] == null)
                    return 0;
                else
                    return (int)this.ViewState["ResourceManifestId"];
            }
            set
            {
                this.ViewState["ResourceManifestId"] = value;
            }
        }

        #endregion

        #region Page Load

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.grdResourceManifestJobs.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdResourceManifestJobs_NeedDataSource);
            this.grdResourceManifestJobs.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(grdResourceManifestJobs_ItemDataBound);

            this.grdResourceManifestAddJobs.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdResourceManifestAddJobs_NeedDataSource);
            this.grdResourceManifestAddJobs.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(grdResourceManifestAddJobs_ItemDataBound);

            this.btnDisplayManifest.Click += new EventHandler(btnDisplayManifest_Click);
            this.btnCreatePIL.Click += new EventHandler(btnCreatePIL_Click);
            this.btnAddJobsToManifest.Click += new EventHandler(btnAddJobsToManifest_Click);
            this.btnAddSelectedJob.Click += new EventHandler(btnAddSelectedJob_Click);
            this.btnCancelAddJobs.Click += new EventHandler(btnCancelAddJobs_Click);
            this.btnGetInstructions.Click += new EventHandler(btnGetInstructions_Click);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                ConfigureDisplay();
        }

        #endregion

        #region Private Functiosn

        private void ConfigureDisplay()
        {
            this.ResourceManifestId = Convert.ToInt32(this.Page.Request.QueryString["rmId"].ToString());
            this.lblManifestNumber.Text = this.ResourceManifestId.ToString();

            // If rmID (resource manifest id) is null then throw exception
            Facade.ResourceManifest facResourceManifest = new Orchestrator.Facade.ResourceManifest();
            this.SavedResourceManifest = facResourceManifest.GetResourceManifest(this.ResourceManifestId);

            txtManifestName.Text = this.SavedResourceManifest.Description;
            dteManifestDate.SelectedDate = this.SavedResourceManifest.ManifestDate;

            dteStartdate.SelectedDate = DateTime.Today;
            dteEndDate.SelectedDate = DateTime.Today.AddDays(1);

            txtExtraRowCount.Value = Orchestrator.Globals.Configuration.DefaultResourceManifestNoOfBlankLines;
            chkUsePlannedTimes.Checked = Orchestrator.Globals.Configuration.UsePlannedTimes;
            chkShowFullAddress.Checked = Orchestrator.Globals.Configuration.ResourceManifestShowFullAddress;

            if (this.SavedResourceManifest.ResourceManifestJobs.Count == 1)
                chkExcludeFirstRow.Enabled = chkExcludeFirstRow.Checked = false;
            else
                chkExcludeFirstRow.Checked = Orchestrator.Globals.Configuration.ExcludeResourceManifestFirstLine;    
        }

        private string GetOrderIDs(DataRow row)
        {
            string[] Orders;
            string[] OrderDetails;
            string orderIDs = string.Empty;
            if (row["InstructionTypeID"] != DBNull.Value)
            {
                int InstructionTypeID = (int)row["InstructionTypeID"];
                if (InstructionTypeID == 1 || InstructionTypeID == 2)
                {
                    Orders = row["Orders"].ToString().Split(new string[] { "^^" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string order in Orders)
                    {
                        OrderDetails = order.Split('|');
                        if (orderIDs.Length > 0)
                            orderIDs += ",";
                        orderIDs += OrderDetails[0];
                    }
                }
            }

            return orderIDs;
        }

        private string GetInstructionText(DataRow row)
        {
            // We should be able to determine what was actually being done here.
            int InstructionTypeID = (int)row["InstructionTypeID"];
            string[] Orders;
            string[] OrderDetails;

            DateTime plannedArrivalDateTime = chkUsePlannedTimes.Checked ? Convert.ToDateTime(row["PlannedArrivalDateTime"]) : Convert.ToDateTime(row["OrderDateTime"]);

            string actions = string.Empty;
            string OrderIDs = string.Empty;
            if (InstructionTypeID == 1)
            {
                // Then this is a Loading Leg

                Orders = row["Orders"].ToString().Split(new string[] { "^^" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string order in Orders)
                {
                    OrderDetails = order.Split('|');

                    if (OrderDetails.Length > 1)
                    {
                        if (actions.Length > 0)
                            actions += "<br/>";

                        actions += string.Format("Loading {0} Pallets for {1} at <b>{2} ({3})</b> using Trailer:{4}", OrderDetails[5], OrderDetails[2], row["PostTown"], plannedArrivalDateTime.ToString("dd/MM HH:mm"), row["TrailerRef"] == DBNull.Value ? string.Empty : row["TrailerRef"].ToString());
                    }

                    OrderIDs = GetOrderIDs(row);
                }
            }
            if (InstructionTypeID == 2)
            {
                Orders = row["Orders"].ToString().Split(new string[] { "^^" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string order in Orders)
                {
                    OrderDetails = order.Split('|');
                    if (actions.Length > 0)
                        actions += "<br/>";
                    if (OrderDetails.Length > 1)
                        actions += string.Format("Dropping {0} pallets for {1} at <b>{2} ({3})</b> using Trailer:{4}", OrderDetails[5], OrderDetails[2], row["PostTown"], plannedArrivalDateTime.ToString("dd/MM HH:mm"), row["TrailerRef"].ToString());
                    OrderIDs = GetOrderIDs(row);
                }

            }

            if (InstructionTypeID == 7)
            {

                actions += string.Format("Trunk to <b>{0} ({1})</b>  using Trailer:{2}", row["PostTown"], plannedArrivalDateTime.ToString("dd/MM HH:mm"), row["TrailerRef"]);
                OrderIDs = GetOrderIDs(row);
                // This is a Load and drive to Trunk Point Leg

            }

            return actions;
        }

        #endregion

        #region Events

        #region Grid

        void grdResourceManifestJobs_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            DataSet dsDrivers = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("JobID", typeof(int)));
            dt.Columns.Add(new DataColumn("SubContractorIdentityID", typeof(string)));
            dt.Columns.Add(new DataColumn("OrganisationName", typeof(string)));
            dt.Columns.Add(new DataColumn("Instructions", typeof(string)));
            dt.Columns.Add(new DataColumn("EarliestDateTime", typeof(DateTime)));
            dt.Columns.Add(new DataColumn("OrderIDs", typeof(string)));
            dt.Columns.Add(new DataColumn("ResourceManifestId", typeof(int)));
            dt.Columns.Add(new DataColumn("Removed", typeof(bool)));
            dt.Columns.Add(new DataColumn("ActiveOnAnotherManifestId", typeof(string)));
            dt.Columns.Add(new DataColumn("ResourceManifestJobId", typeof(int)));
            dsDrivers.Tables.Add(dt);

            Facade.ResourceManifest facResourceManifest = new Orchestrator.Facade.ResourceManifest();
            DataSet ds = facResourceManifest.GetResourceManifestJobInstructions(this.ResourceManifestId);

            int currentJobID = -1;
            int currentDriver = -1;
            bool rowAdded = true;

            string InstructionText = string.Empty;
            string myOrderIds = String.Empty;
            bool hasDriverChanged = false;

            DataRow row = null;
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string orderIDs = String.Empty;
                DateTime plannedArrivalDateTime = chkUsePlannedTimes.Checked ? Convert.ToDateTime(dr["PlannedArrivalDateTime"]) : Convert.ToDateTime(dr["OrderDateTime"]);

                // Get the OrderIDs for the PIL Production.
                orderIDs = GetOrderIDs(dr);

                if (((int)dr["SubContractorIdentityID"] != currentDriver))
                {
                    if (row != null)
                    {
                        if (InstructionText.Length > 0)
                        {
                            row["Instructions"] = InstructionText;
                            row["OrderIDs"] = myOrderIds;
                        }
                        // Add the row to the table
                        dt.Rows.Add(row);
                        rowAdded = true;

                    }
                    currentDriver = (int)dr["SubContractorIdentityID"];
                    InstructionText = String.Empty;
                    myOrderIds = String.Empty;
                    hasDriverChanged = true;
                }
                if (dr["JobID"] != DBNull.Value && (((int)dr["JobID"] != currentJobID) || hasDriverChanged))
                {
                    if (row != null && rowAdded == false)
                    {
                        if (InstructionText.Length > 0)
                        {
                            row["Instructions"] = InstructionText;
                            row["OrderIDs"] = myOrderIds;
                            // Add the row to the table
                            dt.Rows.Add(row);
                        }
                    }
                    hasDriverChanged = false;
                    currentJobID = (int)dr["JobID"];
                    InstructionText = String.Empty;
                    myOrderIds = String.Empty;

                    // Add a New Row
                    row = dt.NewRow();
                    row["OrganisationName"] = dr["OrganisationName"];
                    row["JobID"] = dr["JobID"];
                    row["EarliestDateTime"] = plannedArrivalDateTime;
                    row["SubContractorIdentityID"] = dr["SubContractorIdentityID"];
                    rowAdded = false;
                    // Add the First Instruction to the Text
                    InstructionText = GetInstructionText(dr);
                    myOrderIds = GetOrderIDs(dr);
                    row["Instructions"] = InstructionText;
                    row["OrderIDs"] = myOrderIds;
                    row["ResourceManifestId"] = dr["ResourceManifestId"];
                    row["Removed"] = dr["Removed"];
                    row["ActiveOnAnotherManifestId"] = dr["ActiveOnAnotherManifestId"];
                    row["ResourceManifestJobId"] = dr["ResourceManifestJobId"];
                }
                else if (dr["JobID"] != DBNull.Value && (int)dr["JobID"] == currentJobID)
                {
                    if (plannedArrivalDateTime < ((DateTime)row["EarliestDateTime"]))
                        row["EarliestDateTime"] = plannedArrivalDateTime;
                    //Add the instruction
                    InstructionText += "<br/>" + GetInstructionText(dr);
                    myOrderIds += ("," + GetOrderIDs(dr));
                    row["Instructions"] = InstructionText;
                }
                else
                {
                    // Not assigned
                    if (row != null && rowAdded == false)
                    {
                        if (InstructionText.Length > 0)
                        {
                            row["Instructions"] = InstructionText;
                            row["OrderIDs"] = myOrderIds;
                            // Add the row to the table
                            dt.Rows.Add(row);
                        }
                    }

                    // Add a New Row
                    row = dt.NewRow();
                    row["OrganisationName"] = dr["OrganisationName"];
                    row["SubContractorIdentityID"] = dr["SubContractorIdentityID"];
                    row["JobID"] = dr["JobID"];
                    rowAdded = false;
                    InstructionText = String.Empty;
                    myOrderIds = String.Empty;
                    row["ResourceManifestId"] = dr["ResourceManifestId"];
                    row["Removed"] = dr["Removed"];
                    row["ActiveOnAnotherManifestId"] = dr["ActiveOnAnotherManifestId"];
                    row["ResourceManifestJobId"] = dr["ResourceManifestJobId"];
                }
            }
            if (rowAdded == false && row != null && row["OrganisationName"] != DBNull.Value)
            {
                dt.Rows.Add(row);
            }

            foreach (DataRow r in dt.Rows)
                if (r["Removed"].ToString().ToLower() == "true")
                    r["Instructions"] = REMOVED_JOB_INSTRUCTION_TEXT;

            this.grdResourceManifestJobs.DataSource = dsDrivers;

        }

        private int _runningJobOrder = 0;
        void grdResourceManifestJobs_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is Telerik.Web.UI.GridDataItem)
            {
                int jobId = -1;
                if (e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["JobID"] != DBNull.Value)
                {
                    jobId = (int)e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["JobID"];
                }

                CheckBox chkDriverManifest = e.Item.FindControl("chkDriverManifest") as CheckBox;
                chkDriverManifest.Attributes.Add("onclick", "$('input[id*=hidManifestChanged]').val('true');");

                HtmlInputHidden hidJobOrder = e.Item.FindControl("hidJobOrder") as HtmlInputHidden;
                hidJobOrder.Value = (++_runningJobOrder).ToString();

                HtmlAnchor jobIdLink = e.Item.FindControl("jobIdLink") as HtmlAnchor;
                Label availableLabel = e.Item.FindControl("availableLabel") as Label;

                if (jobId <= 0)
                {
                    CheckBox chkPIL = e.Item.FindControl("chkPIL") as CheckBox;
                    chkPIL.Enabled = false;
                    chkDriverManifest.Enabled = false;
                    availableLabel.Visible = true;
                    jobIdLink.Visible = false;
                }
                else
                {
                    string activeOnAnotherManifestId = ((DataRowView)e.Item.DataItem)["ActiveOnAnotherManifestId"].ToString();
                    string instructions = ((DataRowView)e.Item.DataItem)["Instructions"].ToString();
                    if (instructions == REMOVED_JOB_INSTRUCTION_TEXT && !String.IsNullOrEmpty(activeOnAnotherManifestId))
                    {
                        // Job for this driver is already on another manifest, do not allow user to re-add it.
                        chkDriverManifest.Enabled = false;
                    }

                    availableLabel.Visible = false;
                    jobIdLink.Visible = true;
                    jobIdLink.HRef = "javascript:openJobDetailsWindow(" + jobId.ToString() + ")";
                    jobIdLink.InnerText = jobId.ToString();
                }
            }
        }

        void grdResourceManifestAddJobs_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is Telerik.Web.UI.GridDataItem)
            {
                CheckBox chkDriverManifest = e.Item.FindControl("chkDriverManifest") as CheckBox;
                HyperLink jobIdLink = e.Item.FindControl("jobIdLink") as HyperLink;
                Label availableLabel = e.Item.FindControl("availableLabel") as Label;

                int jobID = -1;
                if (e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["JobID"] != DBNull.Value)
                {
                    jobID = (int)e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["JobID"];
                }

                string driverResourceId = ((DataRowView)e.Item.DataItem)["SubContractorIdentityID"].ToString();
                //chkDriverManifest.Attributes.Add("onclick", "javascript:driverCheckChange(" + driverResourceId.ToString() + ", this);");

                if (jobID <= 0)
                {
                    chkDriverManifest.Enabled = false;

                    CheckBox chkOrderManifest = e.Item.FindControl("chkOrderManifest") as CheckBox;
                    chkOrderManifest.Enabled = false;
                    availableLabel.Visible = true;
                    jobIdLink.Visible = false;
                }
                else
                {
                    availableLabel.Visible = false;
                    jobIdLink.Visible = true;
                    jobIdLink.NavigateUrl = "javascript:openJobDetailsWindow(" + jobID.ToString() + ")";
                    jobIdLink.Text = jobID.ToString();
                    btnAddSelectedJob.Visible = true;
                }
            }
        }

        void grdResourceManifestAddJobs_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            Facade.ResourceManifest facade = new Facade.ResourceManifest();

            DateTime startDate = (DateTime)dteStartdate.SelectedDate;
            DateTime endDate = (DateTime)dteEndDate.SelectedDate;


            DataSet dsDrivers = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("JobID", typeof(int)));
            dt.Columns.Add(new DataColumn("SubContractorIdentityID", typeof(int)));
            dt.Columns.Add(new DataColumn("OrganisationName", typeof(string)));
            dt.Columns.Add(new DataColumn("Instructions", typeof(string)));
            dt.Columns.Add(new DataColumn("EarliestDateTime", typeof(DateTime)));
            dt.Columns.Add(new DataColumn("OrderIDs", typeof(string)));
            //dt.Columns.Add(new DataColumn("ResourceManifestId", typeof(int)));
            dsDrivers.Tables.Add(dt);

            DataSet ds = facade.GetResourceManifestJobsToAddForSubbyManifest(Convert.ToInt32(this.SavedResourceManifest.SubcontractorId), this.ResourceManifestId, startDate, endDate);

            int currentJobID = -1;
            int currentDriver = -1;
            bool rowAdded = true;

            string InstructionText = string.Empty;
            string myOrderIds = String.Empty;
            bool hasDriverChanged = false;

            DataRow row = null;
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string orderIDs = String.Empty;
                DateTime plannedArrivalDateTime = chkUsePlannedTimes.Checked ? Convert.ToDateTime(dr["PlannedArrivalDateTime"]) : Convert.ToDateTime(dr["OrderDateTime"]);

                // Get the OrderIDs for the PIL Production.
                orderIDs = GetOrderIDs(dr);

                if (((int)dr["SubContractorIdentityID"] != currentDriver))
                {
                    if (row != null)
                    {
                        if (InstructionText.Length > 0)
                        {
                            row["Instructions"] = InstructionText;
                            row["OrderIDs"] = myOrderIds;
                        }
                        // Add the row to the table
                        dt.Rows.Add(row);
                        rowAdded = true;

                    }
                    currentDriver = (int)dr["SubContractorIdentityID"];
                    InstructionText = String.Empty;
                    myOrderIds = String.Empty;
                    hasDriverChanged = true;
                }
                if (dr["JobID"] != DBNull.Value && (((int)dr["JobID"] != currentJobID) || hasDriverChanged))
                {
                    if (row != null && rowAdded == false)
                    {
                        if (InstructionText.Length > 0)
                        {
                            row["Instructions"] = InstructionText;
                            row["OrderIDs"] = myOrderIds;
                            // Add the row to the table
                            dt.Rows.Add(row);
                        }
                    }
                    hasDriverChanged = false;
                    currentJobID = (int)dr["JobID"];
                    InstructionText = String.Empty;
                    myOrderIds = String.Empty;

                    // Add a New Row
                    row = dt.NewRow();
                    row["OrganisationName"] = dr["OrganisationName"];
                    row["JobID"] = dr["JobID"];
                    row["EarliestDateTime"] = plannedArrivalDateTime;
                    row["SubContractorIdentityID"] = dr["SubContractorIdentityID"];
                    rowAdded = false;
                    // Add the First Instruction to the Text
                    InstructionText = GetInstructionText(dr);
                    myOrderIds = GetOrderIDs(dr);
                    row["Instructions"] = InstructionText;
                    row["OrderIDs"] = myOrderIds;
                    //row["ResourceManifestId"] = dr["ResourceManifestId"];
                }
                else if (dr["JobID"] != DBNull.Value && (int)dr["JobID"] == currentJobID)
                {
                    if (plannedArrivalDateTime < ((DateTime)row["EarliestDateTime"]))
                        row["EarliestDateTime"] = plannedArrivalDateTime;
                    //Add the instruction
                    InstructionText += "<br/>" + GetInstructionText(dr);
                    myOrderIds += ("," + GetOrderIDs(dr));
                    row["Instructions"] = InstructionText;
                }
                else
                {
                    // Not assigned
                    if (row != null && rowAdded == false)
                    {
                        if (InstructionText.Length > 0)
                        {
                            row["Instructions"] = InstructionText;
                            row["OrderIDs"] = myOrderIds;
                            // Add the row to the table
                            dt.Rows.Add(row);
                        }
                    }

                    // Add a New Row
                    row = dt.NewRow();
                    row["OrganisationName"] = dr["OrganisationName"];
                    row["SubContractorIdentityID"] = dr["SubContractorIdentityID"];
                    row["JobID"] = dr["JobID"];
                    rowAdded = false;
                    InstructionText = String.Empty;
                    myOrderIds = String.Empty;
                    //row["ResourceManifestId"] = dr["ResourceManifestId"];
                }
            }
            if (rowAdded == false && row != null && row["OrganisationName"] != DBNull.Value)
            {
                dt.Rows.Add(row);
            }

            this.grdResourceManifestAddJobs.DataSource = dsDrivers;
            //this.grdResourceManifestAddJobs.DataBind();
        }

        #endregion

        #region Button

        protected void btnDisplayManifest_Click(object sender, EventArgs e)
        {
            // Save the new job order (if its been changed)
            if (hidManifestChanged.Value.ToLower() == "true")
            {
                Facade.ResourceManifest facResourceManifest = new Orchestrator.Facade.ResourceManifest();

                // Save the job order
                foreach (GridItem row in grdResourceManifestJobs.Items)
                {
                    if (row.ItemType == GridItemType.AlternatingItem || row.ItemType == GridItemType.Item)
                    {
                        HtmlInputHidden hidJobId = row.FindControl("hidJobId") as HtmlInputHidden;
                        HtmlInputHidden hidJobOrder = row.FindControl("hidJobOrder") as HtmlInputHidden;
                        HtmlInputHidden hidResourceManifestJobId = row.FindControl("hidResourceManifestJobId") as HtmlInputHidden;
                        CheckBox chkDriverManifest = row.FindControl("chkDriverManifest") as CheckBox;

                        //Entities.ResourceManifestJob rmj = this.SavedResourceManifest.ResourceManifestJobs.Find(o => o.JobId == Convert.ToInt32(hidJobId.Value));
                        Entities.ResourceManifestJob rmj = this.SavedResourceManifest.ResourceManifestJobs.Find(o => o.ResourceManifestJobId == Convert.ToInt32(hidResourceManifestJobId.Value));
                        rmj.JobOrder = Convert.ToInt32(hidJobOrder.Value);

                        // Also identify whether the user has removed the job from the manifest.
                        rmj.Removed = !chkDriverManifest.Checked;
                    }
                }

                this.SavedResourceManifest.Description = txtManifestName.Text;
                this.SavedResourceManifest.ManifestDate = dteManifestDate.SelectedDate.Value;

                facResourceManifest.UpdateResourceManifest(this.SavedResourceManifest, this.Page.User.Identity.Name);

                this.SavedResourceManifest = facResourceManifest.GetResourceManifest(this.ResourceManifestId);
            }

            grdResourceManifestJobs.Rebind();

            // Retrieve the resource manifest 
            NameValueCollection reportParams = new NameValueCollection();
            DataSet manifests = new DataSet();

            manifests.Tables.Add(ManifestGeneration.GetSubbyManifest(this.SavedResourceManifest.ResourceManifestId, this.SavedResourceManifest.SubcontractorId, chkUsePlannedTimes.Checked, chkExcludeFirstRow.Checked, chkShowFullAddress.Checked, true));

            if (manifests.Tables[0].Rows.Count > 0)
            {
                // Add blank rows if applicable
                int extraRows = int.Parse(txtExtraRowCount.Text);
                if (extraRows > 0)
                    for (int i = 0; i < extraRows; i++)
                    {
                        DataRow newRow = manifests.Tables[0].NewRow();
                        manifests.Tables[0].Rows.Add(newRow);
                    }

                //-------------------------------------------------------------------------------------	
                //									Load Report Section 
                //-------------------------------------------------------------------------------------	
                reportParams.Add("ManifestName", this.txtManifestName.Text);
                reportParams.Add("ManifestID", this.lblManifestNumber.Text);
                reportParams.Add("UsePlannedTimes", chkUsePlannedTimes.Checked.ToString());
                Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.RunSheet;
                Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;
                Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = manifests;
                Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = "";
                Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";

                // Show the user control
                Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", "<script language=\"javascript\">window.open('" + Page.ResolveClientUrl("../reports/reportviewer.aspx?wiz=true") + "');</script>");
            }

        }

        void btnCreatePIL_Click(object sender, EventArgs e)
        {
            NameValueCollection reportParams = new NameValueCollection();
            DataSet dsPIL = null;

            Orchestrator.Facade.ICollectDropLoadOrder facLoadOrder = new Orchestrator.Facade.CollectDrop();
            CheckBox chkPIL = null;
            HiddenField hidOrderIDs = null;
            // We have to get the OrderIDs for each of the Jobs Selected on this report.
            string orders = string.Empty;
            foreach (Telerik.Web.UI.GridDataItem gdi in grdResourceManifestJobs.Items)
            {
                chkPIL = gdi.FindControl("chkPIL") as CheckBox;
                if (chkPIL != null && chkPIL.Checked)
                {
                    hidOrderIDs = gdi.FindControl("hidOrderIDs") as HiddenField;

                    if (orders.Length > 0)
                        orders += ",";
                    orders += hidOrderIDs.Value;
                }
            }

            #region Pop-up Report
            dsPIL = facLoadOrder.GetPILData(orders);

            if (dsPIL.Tables[0].Rows.Count > 0)
            {
                //-------------------------------------------------------------------------------------	
                //									Load Report Section 
                //-------------------------------------------------------------------------------------	
                Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.PIL;
                Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;
                Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsPIL;
                Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
                Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";

                // Show the user control
                Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", "<script language=\"javascript\">window.open('" + Page.ResolveClientUrl("../reports/reportviewer.aspx?wiz=true") + "');</script>");
            }
            #endregion
        }

        void btnGetInstructions_Click(object sender, EventArgs e)
        {
            this.grdResourceManifestAddJobs.Rebind();
            btnAddSelectedJob.Visible = true;
        }

        void btnCancelAddJobs_Click(object sender, EventArgs e)
        {
            pnlAddJobs.Visible = false;
            pnlExistingManifest.Visible = true;
        }

        void btnAddSelectedJob_Click(object sender, EventArgs e)
        {
            Facade.ResourceManifest facResourceManifest = new Orchestrator.Facade.ResourceManifest();

            int highestJobOrder = 0;
            // Find the highest job order and simply add it to the end
            foreach (Entities.ResourceManifestJob rmj in this.SavedResourceManifest.ResourceManifestJobs)
            {
                if (rmj.JobOrder > highestJobOrder)
                {
                    highestJobOrder = rmj.JobOrder;
                }
            }

            foreach (Telerik.Web.UI.GridItem row in grdResourceManifestAddJobs.Items)
            {
                if (row.ItemType == Telerik.Web.UI.GridItemType.AlternatingItem || row.ItemType == Telerik.Web.UI.GridItemType.Item)
                {
                    HtmlInputHidden hidJobId = row.FindControl("hidJobId") as HtmlInputHidden;
                    HtmlInputHidden hidJobOrder = row.FindControl("hidJobOrder") as HtmlInputHidden;
                    CheckBox chkDriverManifest = row.FindControl("chkDriverManifest") as CheckBox;

                    int jobId = int.Parse(hidJobId.Value);

                    if (chkDriverManifest.Checked)
                    {
                        // Double check to make sure the rmj isn't already on the manifest.
                        Entities.ResourceManifestJob rmj = this.SavedResourceManifest.ResourceManifestJobs.Find(o => o.JobId == Convert.ToInt32(hidJobId.Value));
                        if (rmj != null)
                        {
                            // The rmj is already on the manifest, simply change its removed flag
                            if (rmj.Removed == true)
                            {
                                rmj.Removed = false;
                            }
                        }
                        else
                        {
                            //Get Instructions Resourced
                            Facade.Instruction facInstruction = new Facade.Instruction();
                            List<int> instructionIDs = facInstruction.GetInstructionIDsForSubContractor(jobId, this.SavedResourceManifest.SubcontractorId.Value, true);

                            foreach (int instructionID in instructionIDs)
                            {
                                // create a new rmj to add to the manifest
                                Entities.ResourceManifestJob newRmj = new Orchestrator.Entities.ResourceManifestJob();
                                newRmj.JobOrder = ++highestJobOrder;
                                newRmj.ResourceManifestId = this.SavedResourceManifest.ResourceManifestId;
                                newRmj.JobId = Convert.ToInt32(hidJobId.Value);
                                newRmj.InstructionId = instructionID;
                                this.SavedResourceManifest.ResourceManifestJobs.Add(newRmj);
                            }
                        }
                    }
                }
            }

            facResourceManifest.UpdateResourceManifest(this.SavedResourceManifest, this.Page.User.Identity.Name);
            this.SavedResourceManifest = facResourceManifest.GetResourceManifest(this.ResourceManifestId);

            grdResourceManifestJobs.Rebind();

            // Clear the add jobs grid.
            grdResourceManifestAddJobs.DataSource = null;
            grdResourceManifestAddJobs.DataBind();
            pnlAddJobs.Visible = false;
            pnlExistingManifest.Visible = true;
        }

        void btnAddJobsToManifest_Click(object sender, EventArgs e)
        {
            pnlAddJobs.Visible = true;
            grdResourceManifestAddJobs.Rebind();
        }

        #endregion

        #endregion
    }
}