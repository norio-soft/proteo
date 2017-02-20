using Orchestrator.Base;
using Orchestrator.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Orchestrator.WebUI.administration.DeliveryWindow
{
    /// <summary>
    /// Matrix Page
    /// </summary>
    public partial class Matrix : BasePage
    {
        /// <summary>
        /// ID for the selected Matrix
        /// </summary>
        public Int32 SelectedWindow { get { return Request.QueryString["id"] == null ? 0 : Int32.Parse(Request.QueryString["id"]); } }
        public Int32 SelectedMatrix
        {
            get
            {
                return ViewState["matrix"] == null ? 0 : Int32.Parse(ViewState["matrix"].ToString());
            }
            set
            {
                ViewState["matrix"] = value;
            }
        }

        /// <summary>
        /// Page Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditRate);

                string control = Request.Form["__EVENTTARGET"];

                if (SelectedWindow > 0)
                {
                    // Populate the header
                    GetHeader(SelectedWindow);
                }
            }

            if (Page.IsPostBack)
            {
                string control = Request.Form["__EVENTTARGET"];

                if (SelectedMatrix > 0)
                {
                    SaveData();
                }
            }

            if (SelectedMatrix > 0)
            {

            }

        }

        /// <summary>
        /// GetHeader
        /// </summary>
        /// <param name="matrixId"></param>
        protected void GetHeader(int matrixId)
        {


            Facade.DeliveryWindow facDeliveryWindowSetup = new Orchestrator.Facade.DeliveryWindow();
            DataSet ds = facDeliveryWindowSetup.GetMatrixDetails(this.SelectedWindow, SelectedMatrix);


            txtDescription.Text = GetColumnValue(ds, "Description");
            txtZone.Text = GetColumnValue(ds, "ZoneMap");


            ddlEffectiveDates.DataSource = ds.Tables[1];
            ddlEffectiveDates.DataTextField = "EffectiveDate";
            ddlEffectiveDates.DataValueField = "DeliveryWindowId";
            ddlEffectiveDates.DataBind();


            GenerateScreenFromDataSet(ds, true);
        }

        private static string GetColumnValue(DataSet ds, String column)
        {
            String description = ds.Tables[0].Rows[0][column].ToString();
            return description;
        }

        protected void btnAddEffectiveDate_Click(object sender, EventArgs e)
        {

            SetEditScreen(true);
            dteEffectiveFrom.SelectedDate = DateTime.Now.Date;

            Facade.DeliveryWindow facDeliveryWindowSetup = new Orchestrator.Facade.DeliveryWindow();

            int sundayAdjustmentHours = SafeParse(txtSundayAdjustmentHours.Text, 0);
            int mondayAdjustmentHours = SafeParse(txtMondayAdjustmentHours.Text, 0);
            int multiCollectTransitionHours = SafeParse(txtMultiCollectionTime.Text, 1);
            string description = txtDescription.Text;
            var deliveryWindowType = (eDeliveryWindowType)int.Parse(cboDeliveryWindowType.SelectedValue);
            DateTime effectiveDate = dteEffectiveFrom.SelectedDate.Value;

            SelectedMatrix = facDeliveryWindowSetup.AddDeliveryWindowDetails(SelectedWindow, sundayAdjustmentHours, mondayAdjustmentHours, multiCollectTransitionHours, deliveryWindowType, effectiveDate, Page.User.Identity.Name);
            GetHeader(SelectedWindow);
        }

        private void SetEditScreen(Boolean editStatus)
        {
            detailsPanel.Visible = editStatus;
            pnlEffectiveDateAddButtons.Visible = !editStatus;
            pnlEffectiveDates.Visible = !editStatus;
        }


        int SafeParse(string field, int defaultvalue = 0)
        {

            int.TryParse(field, out defaultvalue);
            return defaultvalue;
        }



        /// <summary>
        /// Generate Screen From DataSet
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="reload"></param>
        private void GenerateScreenFromDataSet(DataSet ds, Boolean reload)
        {
            int generalCellWidth = 60;
            int smallCellWidth = 20;
            string timeFormat = "H:mm";

            DataTable dt = ds.Tables[2]; // Time Ranges
            DataTable dtHeader = ds.Tables[5]; // header information
            DataTable dtWindowTypes = ds.Tables[4];

            cboDeliveryWindowType.DataSource = dtWindowTypes;
            cboDeliveryWindowType.DataTextField = "Description";
            cboDeliveryWindowType.DataValueField = "DeliveryWindowTypeID";
            cboDeliveryWindowType.DataBind();


            if (dtHeader.Rows.Count > 0)
            {
                DataRow drHeader = dtHeader.Rows[0];

                if (drHeader != null)
                {
                    txtSundayAdjustmentHours.Text = String.Format("{0}", drHeader["SundayAdjustmentHours"]);
                    txtMondayAdjustmentHours.Text = String.Format("{0}", drHeader["MondayAdjustmentHours"]);
                    dteEffectiveFrom.DbSelectedDate = (DateTime)drHeader["EffectiveDate"];
                    txtMultiCollectionTime.Text = String.Format("{0}", drHeader["MultiCollectTransitionHours"]);

                    cboDeliveryWindowType.SelectedValue = drHeader["DeliveryWindowTypeID"].ToString();

                }
            }

            int rowCount = dt.Rows.Count;

            tblDelWindow.Rows.Clear();

            DataTable dtZones = ds.Tables[6];

            // Top line of GRid
            TableRow rt = new TableRow();
            TableRow rtSecondLine = new TableRow();

            TableCell blankCell = new TableCell();
            blankCell.Text = "---";
            blankCell.Width = generalCellWidth;
            rt.Cells.Add(blankCell);


            TableCell zoneCell = new TableCell();
            zoneCell.Text = String.Format("Zone Map:");
            zoneCell.Width = generalCellWidth;
            rtSecondLine.Cells.Add(zoneCell);

            foreach (DataRow timeWindowSection in dt.Rows)
            {
                DateTime startTime = Convert.ToDateTime(timeWindowSection[1].ToString());
                DateTime endTime = Convert.ToDateTime(timeWindowSection[2].ToString());

                TableCell startDate = new TableCell();
                startDate.Width = generalCellWidth;
                startDate.Text = startTime.ToString(timeFormat);
                startDate.CssClass = "formCellLabel";
                rt.Cells.Add(startDate);

                TableCell endDate = new TableCell();
                endDate.Text = endTime.ToString(timeFormat);
                endDate.Width = generalCellWidth;
                endDate.CssClass = "formCellLabel";
                rt.Cells.Add(endDate);

                TableCell monAdjust = new TableCell();
                monAdjust.Width = generalCellWidth;
                monAdjust.Text = "";
                rt.Cells.Add(monAdjust);


                TableCell tcHours = new TableCell();
                tcHours.Text = "Hours";
                tcHours.Width = smallCellWidth;
                tcHours.CssClass = ".formCellLabel";
                tcHours.HorizontalAlign = HorizontalAlign.Center;
                rtSecondLine.Cells.Add(tcHours);

                TableCell tcMinutes = new TableCell();
                tcMinutes.Text = "Minutes";
                tcMinutes.Width = smallCellWidth;
                tcMinutes.CssClass = ".formCellLabel";
                tcMinutes.HorizontalAlign = HorizontalAlign.Center;
                rtSecondLine.Cells.Add(tcMinutes);

                TableCell tcMondayAdjustmentText = new TableCell();
                tcMondayAdjustmentText.Text = "Monday Adjustment";
                tcMondayAdjustmentText.Width = smallCellWidth;
                tcMondayAdjustmentText.HorizontalAlign = HorizontalAlign.Center;
                rtSecondLine.Cells.Add(tcMondayAdjustmentText);

            }

            tblDelWindow.Rows.Add(rt);
            tblDelWindow.Rows.Add(rtSecondLine);

            // Build Rows with Data

            List<DeliveryTimeDetailKey> keyValues = new List<DeliveryTimeDetailKey>();

            // Populate grid with Data
            foreach (DataRow times in ds.Tables[3].Rows)
            {

                Int32 ZoneId = Int32.Parse(times[0].ToString());
                Int32 TimeWindowId = Int32.Parse(times[1].ToString());

                Int32 Hours = Int32.Parse(times[2].ToString());
                Int32 Minutes = Int32.Parse(times[3].ToString());
                Boolean MondayAdj = Boolean.Parse(times[4].ToString());

                DeliveryTimeDetailKey key = new DeliveryTimeDetailKey();

                key.Zone = ZoneId;
                key.Window = TimeWindowId;

                DeliveryTimeDetail detail = new DeliveryTimeDetail();

                detail.Hours = Hours;
                detail.Minutes = Minutes;
                detail.MondayAdjustment = MondayAdj;

                key.Details = detail;

                keyValues.Add(key);
            }

            // Display Grid

            for (int rows = 0; rows < dtZones.Rows.Count; rows++)
            {
                DataRow zoneRow = dtZones.Rows[rows];
                TableRow timeDataRow = new TableRow();

                TableCell idCell = new TableCell();

                int zoneID = Convert.ToInt32(zoneRow["ZoneID"].ToString());

                idCell.Text = zoneRow["Description"].ToString();

                timeDataRow.Cells.Add(idCell);

                TableCell idEmpty = new TableCell();

                foreach (DataRow timeZones in dt.Rows)
                {
                    int windowTimeRange = Int32.Parse(timeZones["TimeRangeID"].ToString());
                    TextBox txtBoxHr = new TextBox();
                    txtBoxHr.ID = String.Format("H:{0}:{1}", zoneID, windowTimeRange);
                    txtBoxHr.MaxLength = 2;
                    txtBoxHr.Width = smallCellWidth;

                    TextBox txtBoxMin = new TextBox();
                    txtBoxMin.ID = String.Format("M:{0}:{1}", zoneID, windowTimeRange);
                    txtBoxMin.Width = smallCellWidth;
                    txtBoxMin.MaxLength = 2;

                    CheckBox chkAdj = new CheckBox();
                    chkAdj.ID = String.Format("B:{0}:{1}", zoneID, windowTimeRange);

                    Boolean found = false;

                    foreach (DeliveryTimeDetailKey keypair in keyValues)
                    {
                        if (keypair.Window == windowTimeRange && keypair.Zone == zoneID)
                        {
                            txtBoxHr.Text = keypair.Details.Hours.ToString();
                            txtBoxMin.Text = keypair.Details.Minutes.ToString();
                            chkAdj.Checked = keypair.Details.MondayAdjustment;
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        txtBoxHr.Text = "0";
                        txtBoxMin.Text = "0";
                        chkAdj.Checked = false;
                    }

                    txtBoxHr.Attributes.Add("onchange", "ValidateHours(this,99);");
                    txtBoxMin.Attributes.Add("onchange", "ValidateHours(this,59);");
                    chkAdj.Attributes.Add("onclick", "ValidateChk(this);");


                    RangeValidator rngValidator = new RangeValidator();
                    rngValidator.MinimumValue = "0";
                    rngValidator.MaximumValue = "23";
                    rngValidator.ControlToValidate = txtBoxHr.ID;
                    rngValidator.Display = ValidatorDisplay.Dynamic;
                    rngValidator.ErrorMessage = "Hours must be between 0 and 23";
                    rngValidator.EnableClientScript = true;
                    rngValidator.Type = ValidationDataType.Integer;


                    TableCell cellHours = new TableCell();
                    cellHours.Width = smallCellWidth;
                    cellHours.HorizontalAlign = HorizontalAlign.Center;
                    cellHours.Controls.Add(txtBoxHr);

                    TableCell cellMinutes = new TableCell();
                    cellMinutes.Width = smallCellWidth;
                    cellMinutes.HorizontalAlign = HorizontalAlign.Center;
                    cellMinutes.Controls.Add(txtBoxMin);

                    TableCell cellMonAdj = new TableCell();
                    cellMonAdj.Width = smallCellWidth;
                    cellMonAdj.HorizontalAlign = HorizontalAlign.Center;

                    cellMonAdj.Controls.Add(chkAdj);

                    timeDataRow.Cells.Add(cellHours);
                    timeDataRow.Cells.Add(cellMinutes);
                    timeDataRow.Cells.Add(cellMonAdj);

                }

                tblDelWindow.Rows.Add(timeDataRow);
            }
        }


        /// <summary>
        /// Update and redirect list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            SaveData();
        }

        /// <summary>
        /// Save data
        /// </summary>
        private void SaveData()
        {
            Table currentTable = tblDelWindow;

            String updates = hidDelWindowChanges.Value;

            // create otherwise an update
            Facade.DeliveryWindow facDeliveryWindowSetup = new Orchestrator.Facade.DeliveryWindow();

            try
            {
                int sundayAdjustmentHours = int.Parse(txtSundayAdjustmentHours.Text);
                int mondayAdjustmentHours = int.Parse(txtMondayAdjustmentHours.Text);
                int multiCollectTransitionHours = int.Parse(txtMultiCollectionTime.Text);
                var deliveryWindowType = (eDeliveryWindowType)int.Parse(cboDeliveryWindowType.SelectedValue);
                DateTime effectiveDate = dteEffectiveFrom.SelectedDate.Value;

                // Validate the delivery Window is correct
                if (facDeliveryWindowSetup.CheckDeliveryWindow(effectiveDate, SelectedWindow, SelectedMatrix))
                {
                    ClientMessage("You can only use an effective date once");
                }
                else
                {
                    facDeliveryWindowSetup.UpdateDeliveryWindow(SelectedMatrix,
                      sundayAdjustmentHours, mondayAdjustmentHours, multiCollectTransitionHours, txtDescription.Text, deliveryWindowType, effectiveDate, true, Page.User.Identity.Name);

                    // check to see if there are any updates
                    if (updates.Length > 0)
                    {
                        UpdateCells(updates);
                    }
                }

                GetHeader(SelectedWindow);
            }
            catch (Exception ex)
            {
                String newError = string.Format("Unexpected error {0} {1}", ex.Message, ex.InnerException);
                ClientMessage(newError);
            }



        }

        private void ClientMessage(String message)
        {
            String csName = "ButtonClickScript";
            Type csType = this.GetType();

            // Get a ClientScriptManager reference from the Page class.
            ClientScriptManager cs = Page.ClientScript;

            // Check to see if the client script is already registered.
            if (!cs.IsClientScriptBlockRegistered(csType, csName))
            {
                StringBuilder csText = new StringBuilder();
                csText.Append("<script type=\"text/javascript\"> ");
                csText.Append("alert('" + message + "'); ");
                csText.Append("</script>");
                cs.RegisterClientScriptBlock(csType, csName, csText.ToString());
            }
        }


        /// <summary>
        /// Process which cells have been changed and update the database
        /// </summary>
        /// <param name="updates"></param>
        private void UpdateCells(String updates)
        {
            Facade.DeliveryWindow facDeliveryWindowSetup = new Orchestrator.Facade.DeliveryWindow();
            // 

            string[] cellIds = updates.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string updateCommand in cellIds)
            {

                string data = this.Request.Form[updateCommand];
                int timeData = 0;
                string[] cellDetail = updateCommand.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                Int32 zoneId = Int32.Parse(cellDetail[1]);
                Int32 timeWindow = Int32.Parse(cellDetail[2]);
                String updateCell = cellDetail[0].Substring(cellDetail[0].Length - 1); // get last character


                switch (updateCell)
                {
                    case "H":
                        timeData = int.Parse(data);
                        facDeliveryWindowSetup.UpdateDeliveryTimes(SelectedMatrix, zoneId, timeWindow, timeData, null, null, Page.User.Identity.Name);
                        break;
                    case "M":
                        timeData = int.Parse(data);
                        facDeliveryWindowSetup.UpdateDeliveryTimes(SelectedMatrix, zoneId, timeWindow, null, timeData, null, Page.User.Identity.Name);
                        break;
                    case "B":
                        if (data == null) data = string.Empty;
                        Boolean adjustMonday = data.ToUpper() == "ON" ? true : false;
                        facDeliveryWindowSetup.UpdateDeliveryTimes(SelectedMatrix, zoneId, timeWindow, null, null, adjustMonday, Page.User.Identity.Name);
                        break;

                }
            }
            hidDelWindowChanges.Value = string.Empty;

        }

        /// <summary>
        /// Changed/updated the window type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cboDeliveryWindowType_SelectedIndexChanged(object sender, EventArgs e)
        {
            SaveData();

            tblDelWindow.Rows.Clear();

            GetHeader(SelectedWindow);

        }

        /// <summary>
        /// Cancel Button pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCancel_Click(object sender, EventArgs e)
        {

            // redirect back to list page
            Response.Redirect(string.Format("Matrix.aspx?id={0}", SelectedWindow));
        }

        /// <summary>
        /// Delete Button pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            // Delete the current window
            Facade.DeliveryWindow facDeliveryWindowSetup = new Orchestrator.Facade.DeliveryWindow();
            facDeliveryWindowSetup.DeleteWindow(SelectedMatrix);

            // redirect back to list page
            Response.Redirect(string.Format("Matrix.aspx?id={0}", SelectedWindow));
        }

        /// <summary>
        /// User has changed the effect date drop down
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ddlEffectiveDates_SelectedIndexChanged(object sender, EventArgs e)
        {

            SetEditScreen(true);
            SelectedMatrix = int.Parse(ddlEffectiveDates.SelectedValue);
            GetHeader(SelectedWindow);

        }

    }
}