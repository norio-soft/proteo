using Orchestrator.BusinessLogicLayer.Traffic;
using Orchestrator.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Orchestrator.WebUI.Traffic
{
    public partial class CommunicationHistory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {

                lblComHistory.Text = "No History to show";

                var startInstructionID = Request.QueryString["startinstructionid"];
                var endInstructionID = Request.QueryString["endinstructionid"];

                if (startInstructionID == null || endInstructionID == null)
                    return;
                else
                {
                    // goto data access
                    TrafficHistory entity = new TrafficHistory();
                    List<Entities.TrafficHistory.TrafficHistory> trafficHistory = entity.GetHistoryForMwfInstruction(int.Parse(startInstructionID));

                    IMWF_InstructionRepository mwfInstructionRepository = DIContainer.CreateRepository<IMWF_InstructionRepository>(uow);
                    var mwfInstruction = mwfInstructionRepository.GetForHaulierEnterpriseInstruction(int.Parse(endInstructionID));

                    lblComHistory.Text = string.Format("{0}{1}", lblComHistory.Text, startInstructionID);

                    if (trafficHistory.Count == 0 && mwfInstruction == null)
                    {
                        return;
                    }

                    lblComHistory.Text = "<table>";
                    foreach (Entities.TrafficHistory.TrafficHistory history in trafficHistory)
                    {
                        lblComHistory.Text =
                            string.Format("{0}<tr><td>{1}</td><td>{2}</td></tr>", lblComHistory.Text, history.TimeofEvent.ToString("HH:mm:ss"), history.EventDetails);
                    }

                    if (mwfInstruction != null && !string.IsNullOrWhiteSpace(mwfInstruction.DeviceIdentifier))
                        lblComHistory.Text = string.Format("{0}<tr><td>{1}</td><td>{2}</td></tr>", lblComHistory.Text, "Device Identifer", mwfInstruction.DeviceIdentifier);

                    lblComHistory.Text = string.Format("{0}{1}", lblComHistory.Text, "</table>");

                    //  lblComHistory.Text = string.Format("{0}{1}", lblComHistory.Text, id);
                }
            }
        }
    }
}