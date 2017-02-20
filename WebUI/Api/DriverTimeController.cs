using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchestrator.Repositories;

namespace Orchestrator.WebUI.Api
{

    public class DriverTimeController : BaseApiController
    {

        // GET api/drivertime?driverID=1&fromDate=2013-01-01&toDate=2013-01-08
        public object GetForDriver(int? driverID, DateTime fromDate, DateTime toDate, int pageSize, int skip)
        {
            return ServiceCallHandler(() =>
            {
                if (!driverID.HasValue)
                {
                    return new Repositories.DTOs.DriverWorkingTimeSummary
                    {
                        Days = Enumerable.Empty<Repositories.DTOs.DriverWorkingTimeSummaryDay>(),
                        MostRecentDriverDateStamp = null
                    };
                }

                using (var uow = DIContainer.CreateUnitOfWork())
                {
                    var repo = DIContainer.CreateRepository<IDriverRepository>(uow);
                    return repo.GetWorkingTimeSummaryForDriver(driverID.Value, fromDate, toDate, pageSize, skip);

                }
            });
        }

    }

}