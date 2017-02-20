using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Orchestrator.WebUI.Traffic
{
    public partial class giveResources : Orchestrator.Base.BasePage
    {
        private int m_instructionId = 0;
        private int m_driverResourceId = 0;
        private int m_vehicleResourceId = 0;
        private int m_trailerResourceId = 0;
        private int m_controlAreaId = 0;
        private int m_trafficAreaId = 0;

        private string m_fullName = string.Empty;
        private string m_regNo = string.Empty;
        private string m_trailerRef = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["instructionId"] != null)
                m_instructionId = int.Parse(Request.QueryString["instructionId"]);

            //Updated to Support Passing the DVT as part of the QueryString
            if (Request.QueryString["dr"] != "null")
            {
                m_driverResourceId = int.Parse(Request.QueryString["dr"]);
                m_fullName = Request.QueryString["fn"];
            }

            if (Request.QueryString["vr"] != "null")
            {
                m_vehicleResourceId = int.Parse(Request.QueryString["vr"]);
                m_regNo = Request.QueryString["regNo"];
            }

            if (Request.QueryString["tr"] != "null")
            {
                m_trailerResourceId = int.Parse(Request.QueryString["tr"]);
                m_trailerRef = Request.QueryString["tn"];
            }

            if (Request.QueryString["ca"] != null)
            {
                int.TryParse(Request.QueryString["ca"], out m_controlAreaId);
                int.TryParse(Request.QueryString["ta"], out m_trafficAreaId);
            }

            if (!IsPostBack)
                InitialisePage();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnConfirm.Click += new EventHandler(btnConfirm_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            mwhelper.CausePostBack = false;
            mwhelper.CloseForm = true;
        }

        void btnConfirm_Click(object sender, EventArgs e)
        {
            Facade.IResource facResource = new Facade.Resource();
            int controlAreaId = int.Parse(cboControlArea.SelectedValue);
            int trafficAreaId = int.Parse(cboTrafficArea.SelectedValue);

            if (m_instructionId != 0)
            {
                Facade.IInstruction facInstruction = new Facade.Instruction();
                Entities.Instruction instruction = facInstruction.GetInstruction(m_instructionId);

                if (instruction.Driver != null)
                    facResource.AssignToArea(controlAreaId, trafficAreaId, instruction.Driver.ResourceId, "Driver was updated via give resources", ((Entities.CustomPrincipal)Page.User).UserName);
                if (instruction.Vehicle != null)
                    facResource.AssignToArea(controlAreaId, trafficAreaId, instruction.Vehicle.ResourceId, "Vehicle was updated via give resources", ((Entities.CustomPrincipal)Page.User).UserName);
                if (instruction.Trailer != null)
                    facResource.AssignToArea(controlAreaId, trafficAreaId, instruction.Trailer.ResourceId, "Trailer was updated via give resources", ((Entities.CustomPrincipal)Page.User).UserName);
            }
            else
            {
                if (m_driverResourceId != 0)
                    facResource.AssignToArea(controlAreaId, trafficAreaId, m_driverResourceId, "Driver was updated via give resources", ((Entities.CustomPrincipal)Page.User).UserName);
                if (m_vehicleResourceId != 0)
                    facResource.AssignToArea(controlAreaId, trafficAreaId, m_vehicleResourceId, "Vehicle was updated via give resources", ((Entities.CustomPrincipal)Page.User).UserName);
                if (m_trailerResourceId != 0)
                    facResource.AssignToArea(controlAreaId, trafficAreaId, m_trailerResourceId, "Trailer was updated via give resources", ((Entities.CustomPrincipal)Page.User).UserName);
            }

            mwhelper.CausePostBack = false;
            mwhelper.CloseForm = true;
        }

        private void InitialisePage()
        {
            // Populate the control and traffic area dropdowns.
            Facade.IControlArea facControlArea = new Facade.Traffic();
            cboControlArea.DataSource = facControlArea.GetAll();
            cboControlArea.DataBind();
            cboTrafficArea.DataSource = ((Facade.ITrafficArea)facControlArea).GetAll();
            cboTrafficArea.DataBind();

            if (m_instructionId != 0)
            {
                Facade.IInstruction facInstruction = new Facade.Instruction();
                Entities.Instruction instruction = facInstruction.GetInstruction(m_instructionId);

                if (instruction.Driver != null)
                    tdDriver.InnerText = instruction.Driver.Individual.FullName;
                if (instruction.Vehicle != null)
                    tdVehicle.InnerText = instruction.Vehicle.RegNo;
                if (instruction.Trailer != null)
                    tdTrailer.InnerText = instruction.Trailer.TrailerRef;

                // Select the current control and traffic area assigned to the leg.
                cboControlArea.Items.FindByValue(instruction.ControlAreaId.ToString()).Selected = true;
                cboTrafficArea.Items.FindByValue(instruction.TrafficAreaId.ToString()).Selected = true;
            }

            if (m_driverResourceId != 0)
            {
                tdDriver.InnerText = m_fullName;
                // Select the current control and traffic area assigned to the leg.
                if (m_controlAreaId > 0)
                    cboControlArea.Items.FindByValue(m_controlAreaId.ToString()).Selected = true;
                if (m_trafficAreaId > 0)
                    cboTrafficArea.Items.FindByValue(m_trafficAreaId.ToString()).Selected = true;
            }
            if (m_vehicleResourceId != 0)
            {
                tdVehicle.InnerText = m_regNo;
            }
            if (m_trailerResourceId != 0)
            {
                tdTrailer.InnerText = m_trailerRef;
            }
        }
    }
}