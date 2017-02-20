using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace Orchestrator.WebUI.Api
{
    public class ResourceIsAvailableController : BaseApiController
    {
        public bool GetForResource(int resourceID, int jobID, DateTime fromDate, DateTime toDate)
        {
            return ServiceCallHandler(() =>
            {

                DateTime startOfDay = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day, 0, 0, 0);
                DateTime endOfDay = new DateTime(toDate.Year, toDate.Month, toDate.Day, 23, 59, 59);

                Orchestrator.Facade.Job fac = new Orchestrator.Facade.Job();
                return fac.IsAvailableInstuction(resourceID, startOfDay, endOfDay);
            });
        }
    }
}