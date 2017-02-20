using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Orchestrator.WebUI.Api
{

    public abstract class BaseApiController : ApiController
    {

        /// <summary>
        /// Web service call wrapper that logs any unhandled exceptions.
        /// </summary>
        internal static TResult ServiceCallHandler<TResult>(Func<TResult> serviceAction)
        {
            TResult retVal;

            try
            {
                retVal = serviceAction();
            }
            catch (Exception ex)
            {
                WebUI.Global.UnhandledException(ex);
                throw;
            }

            return retVal;
        }

    }

}