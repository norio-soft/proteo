using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchestrator.Repositories;

namespace Orchestrator.WebUI.Api
{

    public class InstructionHistoryController : BaseApiController
    {

        //GET api/instructionhistory?driverID=1&fromDate=2013-01-01&toDate=2013-01-08
        public object GetForLeg(int startInstructionID, int endInstructionID)
        {
            return ServiceCallHandler(() =>
            {
                using (var uow = DIContainer.CreateUnitOfWork())
                {
                    BusinessLogicLayer.Traffic.ITrafficHistory bllTrafficHistory = new BusinessLogicLayer.Traffic.TrafficHistory();
                    var instructionRepo = DIContainer.CreateRepository<IInstructionRepository>(uow);
                    var mwfInstructionRepo = DIContainer.CreateRepository<IMWF_InstructionRepository>(uow);
                    Facade.ITrafficHistory facTrafficHistory = new Facade.TrafficHistory(uow, instructionRepo, mwfInstructionRepo, bllTrafficHistory);
                    return facTrafficHistory.GetForLeg(startInstructionID, endInstructionID);
                }
            });
        }

    }

}