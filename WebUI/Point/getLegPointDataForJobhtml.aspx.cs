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

namespace Orchestrator.WebUI.Point
{
    public partial class getLegPointDataForJobhtml : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            GetQueryStringVariables();
        }

        protected int m_PointId = 0;
        protected int m_instructionId = 0;
        protected eInstructionType m_instructionType;
        
        protected bool  m_isAnyTime = false;
        protected bool m_hasInstruction = false;

        protected DateTime m_bookedDateTime;
        protected DateTime m_plannedStartDateTime;
        protected DateTime m_plannedEndDateTime;

        protected Entities.Point m_point = null;

        private void GetQueryStringVariables()
        {
            m_PointId = int.Parse(Request.QueryString["PointId"]);

            if (Request.QueryString["InstructionId"] != "" && Request.QueryString["InstructionId"]!= null)
            {
                 m_instructionId = int.Parse(Request.QueryString["InstructionId"]);
                 m_instructionType = (eInstructionType)(int.Parse(Request.QueryString["InstructionTypeId"]));
                 m_bookedDateTime = DateTime.Parse(Request.QueryString["BookedTime"]);
                 m_isAnyTime = (int.Parse(Request.QueryString["IsAnyTime"])) == 1 ? true : false;
                 m_hasInstruction = true;
            }

            m_plannedStartDateTime = DateTime.Parse(Request.QueryString["LegPlannedStart"]);
            m_plannedEndDateTime = DateTime.Parse(Request.QueryString["LegPlannedEnd"]);

            Facade.IPoint facPoint = new Facade.Point();
            m_point = facPoint.GetPointForPointId(m_PointId);

            string addressString = "<b>" + m_point.Description + "</b> <br/>";
            addressString += m_point.Address.ToString().Replace(Environment.NewLine, "<br/>");

            lblAddress.Text = addressString;

            string instructionType = string.Empty;
            string bookedTimes = string.Empty;
            string plannedTimes = string.Empty;
            if (m_hasInstruction)
            {
                instructionType = "<b>Instruction Type :: " + Utilities.UnCamelCase(m_instructionType.ToString()) + "</b>";
                lblInstructionType.Text = instructionType;
                bookedTimes = "<b>Booked : </b>" + m_bookedDateTime.ToString("dd/MM HH:mm");
            }
            else
            {
                instructionType = "<span style=\"color:blue;\">This is a Trunk Leg.</span>";
                lblInstructionType.Text = instructionType;
            }

            plannedTimes = "<b>Planned Start : </b>" + m_plannedStartDateTime.ToString("dd/MM HH:mm") + "<br/>";
            plannedTimes += "<b>Planned End : </b>" + m_plannedEndDateTime.ToString("dd/MM HH:mm") + "<br/>";
            lblBookedDateTime.Text = bookedTimes;
            lblPlannedTimes.Text = plannedTimes;
        }
    }
}