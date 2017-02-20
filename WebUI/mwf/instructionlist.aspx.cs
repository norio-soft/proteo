using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Configuration;
using Telerik.Web.UI;
using Orchestrator.Models;
using Orchestrator.Repositories;

namespace Orchestrator.WebUI.mwf
{
    public partial class instructionlist : Orchestrator.Base.BasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetDefaultFilters();
                loadFilterOptionsFromSession();
                InstructionsGrid.Rebind();

                int interval = 0;
                try
                {
                    int.TryParse(ConfigurationManager.AppSettings["InstructionListRefreshTime"].ToString(), out interval);
                }
                catch (Exception ex)
                {
                    // do nothig this is just a hang up from the MWF code.
                }
                hfInterval.Value = interval.ToString();
            }
            else
            {
                //refresh grid data as the page reloads on a set timer
                InstructionsGrid.Rebind();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.FilterButton.Click += this.FilterButton_Click;
            this.ResetButton.Click += this.ResetButton_Click;

            this.InstructionsGrid.NeedDataSource += (o, evt) => this.InstructionsGrid.DataSource = this.GetInstructionsGridData();
            this.InstructionsGrid.ItemDataBound += this.InstructionsGrid_ItemDataBound;

            LoadDrivers();
            LoadVehicles();
            LoadStatuses();
            LoadCommunicationStatuses();
        }

        private void InstructionsGrid_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem item = (GridDataItem)e.Item;
                int communicationStatusID = (int)item.GetDataKeyValue("CommunicationStatusID");
                if ((communicationStatusID != (int)MWFCommunicationStatusEnum.ReceivedOnDevice))
                {
                    TableCell cell = item["CommunicationStatus"];
                    cell.ForeColor = System.Drawing.ColorTranslator.FromHtml("#EF4337");
                }
            }
        }

        private object GetInstructionsGridData()
        {
            Facade.IPoint facPoint = new Facade.Point();
            Facade.Resource facDriver = new Facade.Resource();
            var signatureImageBaseUri =  Utilities.GetSignatureImageBaseUri();
            
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<IMWF_InstructionRepository>(uow);
                var instructions = repo.GetAll();

                // Apply filters
                if (!string.IsNullOrEmpty(DriverPicker.SelectedValue))
                {
                    int selectedDriverID = int.Parse(DriverPicker.SelectedValue);
                    instructions = instructions.Where(i => i.Driver.ResourceID == selectedDriverID);
                }
                if (!string.IsNullOrEmpty(VehiclePicker.SelectedValue))
                {
                    string selectedVehicleReg = VehiclePicker.SelectedValue;
                    instructions = instructions.Where(i => i.VehicleReg == selectedVehicleReg);
                }

                var statusIDs = Utilities.ExtractCheckBoxListValues(StatusCheckboxList);
                if (statusIDs.Count > 0)
                {
                    instructions = instructions.Where(e => statusIDs.Contains((int)e.Status));
                }

                // Comm status filter
                var communicationStatusIDs = Utilities.ExtractCheckBoxListValues(CommunicationStatusCheckBoxList);
                if (communicationStatusIDs.Count > 0)
                {
                    instructions = instructions.Where(e => communicationStatusIDs.Contains((int)e.CommunicationStatus));
                }

                if (ArrivalFrom.SelectedDate != null && ArrivalTo.SelectedDate != null)
                {
                    instructions = instructions.Where(i => i.ArriveDateTime >= ArrivalFrom.SelectedDate.Value &&
                        i.ArriveDateTime <= ArrivalTo.SelectedDate.Value);
                }

                if (ArrivalFrom.SelectedDate != null && ArrivalTo.SelectedDate == null)
                {
                    instructions = instructions.Where(i => i.ArriveDateTime >= ArrivalFrom.SelectedDate.Value);
                }

                if (!string.IsNullOrEmpty(txtSearchField.Text))
                {
                    var searchTextAsStr = txtSearchField.Text.Trim();
                    var searchTextAsInt = Utilities.ParseNullable<int>(txtSearchField.Text);

                    if (searchTextAsInt.HasValue)
                        instructions = instructions.Where(i => i.Job.Title.Contains(txtSearchField.Text.Trim()) || i.Job.ID == searchTextAsInt.Value);
                    else
                        instructions = instructions.Where(i => i.Job.Title.Contains(searchTextAsStr) || i.OrderInstructions.FirstOrDefault().Order.ItemTitle.Contains(searchTextAsStr));
                }

                var queryData = instructions.Select(i => new
                {
                    i.ID,
                    JobID = i.Job.ID,
                    InstructionTypeID = i.InstructionType,
                    CustomerReference = i.Job.Title,
                    CustomerOrder = (i.OrderInstructions.Any()) ? i.OrderInstructions.FirstOrDefault().Order.ItemTitle : "",
                    PointDescription = i.Point.Description,
                    DriverIndividual = i.Driver == null ? null : i.Driver.Individual,
                    Vehicle = i.VehicleReg == null ? string.Empty : i.VehicleReg,
                    i.ArriveDateTime,
                    i.DepartDateTime,
                    i.CommunicateDateTime,
                    i.LastUpdateDateTime,
                    CommunicationStatusID = i.CommunicationStatus,
                    StatusID = i.Status,
                    SignedBy = i.InstructionSignatures.FirstOrDefault().SignedBy,
                    SignedComment = i.InstructionSignatures.FirstOrDefault().Comment,
                    SignatureImage = i.InstructionSignatures.FirstOrDefault().ImageName,
                    SignedDateTime = i.CompleteDateTime,
                    CQuantity = i.InstructionType == MWFInstructionTypeEnum.Collect ?
                        i.OrderInstructions.Sum(f => f.Order.ConfirmedCollectQuantity) :
                        i.OrderInstructions.Sum(f => f.Order.ConfirmedDeliverQuantity),
                    RunStatus = i.Job.Status,
                    SignatureLatitude = i.InstructionSignatures.FirstOrDefault().Latitude,
                    SignatureLongitude = i.InstructionSignatures.FirstOrDefault().Longitude,
                    OrderID = (i.OrderInstructions.Any()) ? (int?)i.OrderInstructions.FirstOrDefault().Order.ID : (int?)null,
                    OrderComplete = i.CompleteDateTime.HasValue,
                    i.DriveDateTime,
                    i.CompleteDateTime
                }).OrderByDescending(i => i.ArriveDateTime).ToList();

                var gridData = queryData.Select(qd => new
                {
                    qd.ID,
                    qd.JobID,
                    InstructionType = MWF_Instruction.GetInstructionTypeDescription(qd.InstructionTypeID),
                    qd.CustomerReference,
                    qd.CustomerOrder,
                    Location = qd.PointDescription,
                    ArrivalTime = qd.ArriveDateTime,
                    Status = MWF_Instruction.GetStatusDescription(qd.StatusID),
                    qd.RunStatus,
                    qd.StatusID,
                    qd.CommunicationStatusID,
                    Driver = qd.DriverIndividual == null ? string.Empty : Entities.Utilities.MergeStrings(" ", qd.DriverIndividual.FirstNames, qd.DriverIndividual.LastName),
                    VehicleReg = qd.Vehicle,
                    CommunicationStatus = MWF_Instruction.GetCommunicationStatusDescription((int)qd.CommunicationStatusID),
                    CommunicationDateTime = qd.CommunicateDateTime,
                    SignedBy = qd.SignedBy,
                    SignedComment = qd.SignedComment,
#if DEBUG
                    SignatureImage = String.IsNullOrEmpty(qd.SignatureImage) ? "" : "http://demo.orchestrator.co.uk/signatures/" + qd.SignatureImage,
#else
                    SignatureImage = String.IsNullOrEmpty(qd.SignatureImage) ? "" : new Uri(signatureImageBaseUri, qd.SignatureImage).AbsoluteUri,
#endif
                    SignedDateTime = qd.SignedDateTime.HasValue ? qd.SignedDateTime.Value.ToLongDateString() + " " + qd.SignedDateTime.Value.ToLongTimeString() : "",
                    ConfirmedQuantity = qd.CQuantity,
                    qd.SignatureLatitude,
                    qd.SignatureLongitude,
                    qd.OrderID,
                    qd.OrderComplete,
                    qd.DriveDateTime,
                    qd.CompleteDateTime
                });

                return gridData.ToList();
            }
        }

        private void FilterButton_Click(object sender, EventArgs e)
        {
            saveFilterOptionsToSession();
            this.InstructionsGrid.CurrentPageIndex = 0;
            this.InstructionsGrid.Rebind();
        }

        void ResetButton_Click(object sender, EventArgs e)
        {
            ClearFilters();
            saveFilterOptionsToSession();
            InstructionsGrid.DataSource = null;
            InstructionsGrid.Rebind();
        }

        private void ClearFilters()
        {
            DriverPicker.ClearSelection();
            VehiclePicker.ClearSelection();

            ArrivalFrom.Clear();
            ArrivalTo.Clear();

            this.txtSearchField.Text = string.Empty;

            StatusCheckboxList.ClearSelection();
            CommunicationStatusCheckBoxList.ClearSelection();

            SetDefaultFilters();

            InstructionsGrid.MasterTableView.SortExpressions.Clear();
        }

        private void LoadDrivers()
        {
            Facade.Resource facDriver = new Facade.Resource();
            var drivers = facDriver.GetAllDrivers(false).Tables[0].AsEnumerable();
            var dropdownData = drivers.Select(d => new
            {
                Text = d.Field<string>("Fullname"),
                Value = d.Field<int>("resourceid")
            });
            this.DriverPicker.DataSource = dropdownData;
            this.DriverPicker.DataBind();
        }

        private void LoadVehicles()
        {
            Facade.Resource facVehicle = new Facade.Resource();
            var vehicles = facVehicle.GetAllVehicles().Tables[0].AsEnumerable();
            var dropdownData = vehicles.Select(d => new
            {
                Text = d.Field<string>("RegNo"),
                Value = d.Field<string>("RegNo")
            });
            this.VehiclePicker.DataSource = dropdownData;
            this.VehiclePicker.DataBind();
        }

        private void LoadStatuses()
        {
            var statuses = Enum.GetValues(typeof(MWFStatusEnum)).Cast<MWFStatusEnum>()
                .Where(s => (((int)s == (int)MWFStatusEnum.Complete) || ((int)s == (int)MWFStatusEnum.OnSite) || ((int)s == (int)MWFStatusEnum.Drive) || ((int)s == (int)MWFStatusEnum.Unknown)))
                .Select(s => new
                {
                    Text = MWF_Instruction.GetStatusDescription(s),
                    Value = (int)s
                }).Distinct();

            this.StatusCheckboxList.DataSource = statuses;
            this.StatusCheckboxList.DataBind();
        }

        private void LoadCommunicationStatuses()
        {
            var communicationsStatuses = Enum.GetValues(typeof(MWFCommunicationStatusEnum)).Cast<MWFCommunicationStatusEnum>().Select(s => new
            {
                Text = MWF_Instruction.GetCommunicationStatusDescription(s),
                Value = (int)s
            }).Distinct();

            this.CommunicationStatusCheckBoxList.DataSource = communicationsStatuses;
            this.CommunicationStatusCheckBoxList.DataBind();
        }

        private void SetDefaultFilters()
        {
            foreach (ListItem statusCheckbox in this.StatusCheckboxList.Items)
            {
                statusCheckbox.Selected = (int.Parse(statusCheckbox.Value) != (int)MWFStatusEnum.Complete);
            }
        }

        #region Private Functions

        private void saveFilterOptionsToSession()
        {
            //Search Text
            if (!string.IsNullOrEmpty(txtSearchField.Text))
                Session["instructionlist_filters_searchtext"] = txtSearchField.Text;
            else
                Session.Remove("instructionlist_filters_searchtext");
            //Worker
            if (!string.IsNullOrEmpty(DriverPicker.SelectedValue))
                Session["instructionlist_filters_driverid"] = DriverPicker.SelectedValue;
            else
                Session.Remove("instructionlist_filters_driverid");
            //Vehicle
            if (!string.IsNullOrEmpty(VehiclePicker.SelectedValue))
                Session["instructionlist_filters_vehicleid"] = VehiclePicker.SelectedValue;
            else
                Session.Remove("instructionlist_filters_vehicleid");
            //ArrivalFrom
            if (ArrivalFrom.SelectedDate.HasValue)
                Session["instructionlist_filters_arrivalfrom"] = ArrivalFrom.SelectedDate.Value;
            else
                Session.Remove("instructionlist_filters_arrivalfrom");
            //ArrivalTo
            if (ArrivalTo.SelectedDate.HasValue)
                Session["instructionlist_filters_arrivalto"] = ArrivalTo.SelectedDate.Value;
            else
                Session.Remove("instructionlist_filters_arrivalto");
            //Status
            foreach (ListItem item in StatusCheckboxList.Items)
                Session["instructionlist_filters_status_" + item.Text.Replace(" ", "")] = item.Selected.ToString().ToLower();
            //Communication Status
            foreach (ListItem item in CommunicationStatusCheckBoxList.Items)
                Session["instructionlist_filters_commstatus_" + item.Text.Replace(" ", "")] = item.Selected.ToString().ToLower();
        }

        private void loadFilterOptionsFromSession()
        {
            //If searching using the search box do not load other filters from session state.
            if (!string.IsNullOrEmpty(Request.QueryString["ss"]))
            {
                ClearFilters();
                txtSearchField.Text = Request.QueryString["ss"];

                foreach (ListItem item in StatusCheckboxList.Items)
                {
                    item.Selected = true;
                }

                return;
            }

            DateTime datetime;

            //Search Text
            if (Session["instructionlist_filters_searchtext"] != null)
                txtSearchField.Text = Session["instructionlist_filters_searchtext"].ToString();
            //ClientId
            if (Session["instructionlist_filters_driverid"] != null)
                DriverPicker.SelectedValue = Session["instructionlist_filters_driverid"].ToString();
            //Vehicle
            if (Session["instructionlist_filters_vehicleid"] != null)
                VehiclePicker.SelectedValue = Session["instructionlist_filters_vehicleid"].ToString();
            //ArrivalFrom
            if (Session["instructionlist_filters_arrivalfrom"] != null)
            {
                if (DateTime.TryParse(Session["instructionlist_filters_arrivalfrom"].ToString(), out datetime))
                    ArrivalFrom.SelectedDate = datetime;
            }
            //ArrivalFrom
            if (Session["instructionlist_filters_arrivalto"] != null)
            {
                if (DateTime.TryParse(Session["instructionlist_filters_arrivalto"].ToString(), out datetime))
                    ArrivalTo.SelectedDate = datetime;
            }
            //Status
            foreach (ListItem item in StatusCheckboxList.Items)
            {
                if (Session["instructionlist_filters_status_" + item.Text.Replace(" ", "")] != null)
                    item.Selected = Convert.ToBoolean(Session["instructionlist_filters_status_" + item.Text.Replace(" ", "")]);
            }
            //Communication Status
            foreach (ListItem item in CommunicationStatusCheckBoxList.Items)
            {
                if (Session["instructionlist_filters_commstatus_" + item.Text.Replace(" ", "")] != null)
                    item.Selected = Convert.ToBoolean(Session["instructionlist_filters_commstatus_" + item.Text.Replace(" ", "")]);
            }
        }

        #endregion

    }
}