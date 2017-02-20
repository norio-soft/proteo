using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Telerik.Web.UI.Grid;
using System.Collections.Specialized;
using System.Text;

namespace Orchestrator.WebUI.manifest
{
    public partial class SubbyResourceManifestBuilder : Orchestrator.Base.BasePage
    {

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

        #region Page Load/Init
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // The default manifest date is the start of tomorrow.
                if (Session[Orchestrator.Globals.Constants.SubbyManifestDate] == null)
                {
                    DateTime defaultManifestDateTime = DateTime.Now;
                    Session[Orchestrator.Globals.Constants.SubbyManifestDate] = defaultManifestDateTime.Subtract(defaultManifestDateTime.TimeOfDay);
                }

                dteManifestDate.SelectedDate = (DateTime)Session[Orchestrator.Globals.Constants.SubbyManifestDate];

                this.ResourceManifestId = Convert.ToInt32(Request.QueryString["rmID"]);
                PopulateStaticData();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.grdManifests.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdManifests_NeedDataSource);
            this.grdManifests.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(grdManifests_ItemDataBound);
            this.btnGetInstructions.Click += new EventHandler(btnGetInstructions_Click);

            this.btnCreatePIL.Click += new EventHandler(btnCreatePIL_Click);
            this.btnDriverManifests.Click += new EventHandler(btnDriverManifests_Click);
        }

        void grdManifests_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
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
                
                HtmlAnchor hypManifest = e.Item.FindControl("hypManifest") as HtmlAnchor;
                string resourceManifestId = ((DataRowView)e.Item.DataItem)["ResourceManifestId"].ToString();
                if (!string.IsNullOrEmpty(resourceManifestId))
                {
                    // Manifest numbers will be a minimum of 3 digits long.
                    string formattedResourceManifestId = resourceManifestId;
                    if (formattedResourceManifestId.Length == 1)
                        formattedResourceManifestId = string.Concat("00", formattedResourceManifestId);
                    else if (formattedResourceManifestId.Length == 2)
                        formattedResourceManifestId = string.Concat("0", formattedResourceManifestId);

                    // Provide a link to the saved manifest
                    hypManifest.InnerText = formattedResourceManifestId.ToString();
                    hypManifest.HRef = "SubbyResourceManifest.aspx?rmID=" + resourceManifestId;
                    e.Item.Enabled = false;
                }

                string driverResourceId = ((DataRowView)e.Item.DataItem)["IdentityID"].ToString();
                chkDriverManifest.Attributes.Add("onclick", "javascript:driverCheckChange(" + driverResourceId.ToString() + ", this);");


                if (jobID <= 0)
                {
                    CheckBox chkPIL = e.Item.FindControl("chkPIL") as CheckBox;
                    chkPIL.Enabled = false;
                    
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
                }
            }
        }

        void btnGetInstructions_Click(object sender, EventArgs e)
        {
            Session[Orchestrator.Globals.Constants.SubbyManifestListFromDate] = dteStartdate.SelectedDate;
            Session[Orchestrator.Globals.Constants.SubbyManifestListToDate] = dteEndDate.SelectedDate;
            Session[Orchestrator.Globals.Constants.SubbyManifestDate] = dteManifestDate.SelectedDate;
            
            this.grdManifests.Rebind();
        }

        #endregion

        #region Grid Event Handlers

        void grdManifests_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            Facade.Manifest facManifest = new Orchestrator.Facade.Manifest();

            DateTime startDate = (DateTime)dteStartdate.SelectedDate;
            DateTime endDate = (DateTime)dteEndDate.SelectedDate;

            DataSet dsDrivers = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("JobID", typeof(int)));
            dt.Columns.Add(new DataColumn("IdentityID", typeof(int)));
            dt.Columns.Add(new DataColumn("OrganisationName", typeof(string)));
            dt.Columns.Add(new DataColumn("Instructions", typeof(string)));
            dt.Columns.Add(new DataColumn("EarliestDateTime", typeof(DateTime)));
            dt.Columns.Add(new DataColumn("OrderIDs", typeof(string)));
            dt.Columns.Add(new DataColumn("ResourceManifestId", typeof(int)));
            dsDrivers.Tables.Add(dt);

            DataSet ds = facManifest.GetInstructionsForSubContractors(new List<int>(), startDate, endDate);
            int currentJobID = -1;
            int currentSubcontractor = -1;
            bool rowAdded = true;

            string InstructionText = string.Empty;
            string myOrderIds = String.Empty;
            bool hasDriverChanged = false;

            DataRow row = null;
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string orderIDs = String.Empty;

                // Get the OrderIDs for the PIL Production.
                orderIDs = GetOrderIDs(dr);

                if (((int)dr["IdentityID"] != currentSubcontractor))
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
                    currentSubcontractor = (int)dr["IdentityID"];
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
                    row["EarliestDateTime"] = dr["PlannedArrivalDateTime"];
                    row["IdentityID"] = dr["IdentityID"];
                    rowAdded = false;
                    // Add the First Instruction to the Text
                    InstructionText = GetInstructionText(dr);
                    myOrderIds = GetOrderIDs(dr);
                    row["Instructions"] = InstructionText;
                    row["OrderIDs"] = myOrderIds;
                    row["ResourceManifestId"] = dr["ResourceManifestId"];
                }
                else if (dr["JobID"] != DBNull.Value && (int)dr["JobID"] == currentJobID)
                {
                    if (((DateTime)dr["PlannedArrivalDateTime"]) < ((DateTime)row["EarliestDateTime"]))
                        row["EarliestDateTime"] = dr["PlannedArrivalDateTime"];
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
                    row["IdentityID"] = dr["IdentityID"];
                    row["JobID"] = dr["JobID"];
                    rowAdded = false;
                    InstructionText = String.Empty;
                    myOrderIds = String.Empty;
                    row["ResourceManifestId"] = dr["ResourceManifestId"];
                }
            }
            if (rowAdded == false && row != null && row["OrganisationName"] != DBNull.Value)
            {
                dt.Rows.Add(row);
            }

            this.grdManifests.DataSource = dsDrivers;

        }
        #endregion

        #region Radio Button List Handlers

        
        #endregion

        void btnDriverManifests_Click(object sender, EventArgs e)
        {
            Session[Orchestrator.Globals.Constants.SubbyManifestListFromDate] = dteStartdate.SelectedDate;
            Session[Orchestrator.Globals.Constants.SubbyManifestListToDate] = dteEndDate.SelectedDate;
            Session[Orchestrator.Globals.Constants.SubbyManifestDate] = dteManifestDate.SelectedDate;

            CheckBox chkManifest = null;
            HiddenField hidResourceIds = null;

            int runningJobOrder = 0;
            Entities.ResourceManifest resourceManifest = new Orchestrator.Entities.ResourceManifest();
            resourceManifest.ManifestDate = dteManifestDate.SelectedDate.Value;
            resourceManifest.Description = txtManifestName.Text; 
            resourceManifest.ResourceManifestJobs = new List<Entities.ResourceManifestJob>();

            Facade.ResourceManifest facResourceManifest = new Orchestrator.Facade.ResourceManifest();
            Facade.Instruction facInstruction = new Orchestrator.Facade.Instruction();

            foreach (Telerik.Web.UI.GridDataItem gdi in grdManifests.Items)
            {
                chkManifest = gdi.FindControl("chkDriverManifest") as CheckBox;
                if (chkManifest != null && chkManifest.Checked)
                {
                    if (gdi.OwnerTableView.DataKeyValues[gdi.ItemIndex]["JobID"] != DBNull.Value)
                    {
                        hidResourceIds = gdi.FindControl("hidResourceId") as HiddenField;

                        // Save ResourceManifestJob rows here
                        int subContractorID = Convert.ToInt32(hidResourceIds.Value);
                        int jobId = Convert.ToInt32(gdi.OwnerTableView.DataKeyValues[gdi.ItemIndex]["JobID"]);

                        if (resourceManifest.SubcontractorId == null || resourceManifest.SubcontractorId == 0)
                            resourceManifest.SubcontractorId = subContractorID;

                        //Get Instructions Resourced
                        List<int> instructionIDs = facInstruction.GetInstructionIDsForSubContractor(jobId, subContractorID, true);

                        foreach (int instructionID in instructionIDs)
                        {
                            // Create new resource manifest jobs rows
                            Entities.ResourceManifestJob rmj = new Orchestrator.Entities.ResourceManifestJob();
                            rmj.JobId = jobId;
                            rmj.InstructionId = instructionID;
                            rmj.JobOrder = ++runningJobOrder;

                            // Add the resource manifest job to the collection.
                            resourceManifest.ResourceManifestJobs.Add(rmj);
                        }
                    }
                }
            }

            // Create the new resource manifest.
            this.ResourceManifestId = facResourceManifest.CreateResourceManifest(resourceManifest, this.Page.User.Identity.Name);

            // Redirect to the drivers ResourceManifestJob edit page.
            Response.Redirect("SubbyResourceManifest.aspx?rmID=" + this.ResourceManifestId.ToString());
        }

        void btnCreatePIL_Click(object sender, EventArgs e)
        {
            Session[Orchestrator.Globals.Constants.SubbyManifestListFromDate] = dteStartdate.SelectedDate;
            Session[Orchestrator.Globals.Constants.SubbyManifestListToDate] = dteEndDate.SelectedDate;
            Session[Orchestrator.Globals.Constants.SubbyManifestDate] = dteManifestDate.SelectedDate;
            
            NameValueCollection reportParams = new NameValueCollection();
            DataSet dsPIL = null;

            Orchestrator.Facade.ICollectDropLoadOrder facLoadOrder = new Orchestrator.Facade.CollectDrop();
            CheckBox chkPIL = null;
            HiddenField hidOrderIDs = null;
            // We have to get the OrderIDs for each of the Jobs Selected on this report.
            string orders = string.Empty;
            foreach (Telerik.Web.UI.GridDataItem gdi in grdManifests.Items)
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

        #region functions used by the Datagrid
        protected string GetJobID(DataRowView drv)
        {
            if (drv["JobID"] == DBNull.Value)
                return "Available";
            else
                return drv["JobID"].ToString();
        }

        #endregion

        #region Private Methods

        private void PopulateStaticData()
        {
            if (Session[Orchestrator.Globals.Constants.SubbyManifestListFromDate] == null)
                Session[Orchestrator.Globals.Constants.SubbyManifestListFromDate] = DateTime.Today;

            if (Session[Orchestrator.Globals.Constants.SubbyManifestListToDate] == null)
                Session[Orchestrator.Globals.Constants.SubbyManifestListToDate] = DateTime.Today.AddDays(1);

            dteStartdate.SelectedDate = (DateTime)Session[Orchestrator.Globals.Constants.SubbyManifestListFromDate];
            dteEndDate.SelectedDate = (DateTime)Session[Orchestrator.Globals.Constants.SubbyManifestListToDate];

        }

        private string ConvertOrdersToCSV(List<miniOrder> orders)
        {
            StringBuilder retVal = new StringBuilder();
            foreach (miniOrder o in orders)
            {
                if (retVal.Length > 0) retVal.Append("^^");
                retVal.Append(o.OrderID.ToString());
                retVal.Append("|");
                retVal.Append(o.CustomerIdentityID.ToString());
                retVal.Append("|");
                retVal.Append(o.Customer);
                retVal.Append("|");
                retVal.Append(o.CustomerOrderNumber);
                retVal.Append("|");
                retVal.Append(o.DeliveryOrderNumber);
                retVal.Append("|");
                retVal.Append(o.NoPallets.ToString());
                retVal.Append("|");
                retVal.Append(o.Weight.ToString());
            }

            return retVal.ToString();
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

                        actions += string.Format("Loading {0} Pallets for {1} at <b>{2} ({3})</b> using Vehicle:{4} Trailer:{5}", OrderDetails[5], OrderDetails[2], row["PostTown"], ((DateTime)row["PlannedArrivalDateTime"]).ToString("dd/MM HH:mm"), row["RegNo"].ToString(), row["TrailerRef"].ToString());
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
                        actions += string.Format("Dropping {0} pallets for {1} at <b>{2} ({3})</b> using Vehicle:{4} Trailer:{5}", OrderDetails[5], OrderDetails[2], row["PostTown"], ((DateTime)row["PlannedArrivalDateTime"]).ToString("dd/MM HH:mm"), row["RegNo"].ToString(), row["TrailerRef"].ToString());
                    OrderIDs = GetOrderIDs(row);
                }

            }

            if (InstructionTypeID == 7)
            {

                actions += string.Format("Trunk to <b>{0} ({1})</b>  using Vehcile:{2} Trailer:{3}", row["PostTown"], ((DateTime)row["PlannedArrivalDateTime"]).ToString("dd/MM HH:mm"), row["RegNo"], row["TrailerRef"]);
                OrderIDs = GetOrderIDs(row);
                // This is a Load and drive to Trunk Point Leg

            }



            return actions;
        }

        #endregion
    }
}
