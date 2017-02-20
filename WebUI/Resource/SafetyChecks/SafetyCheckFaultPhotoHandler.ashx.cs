using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using Orchestrator.Entities;
using Orchestrator.Globals;

namespace Orchestrator.WebUI.Resource.SafetyChecks
{
    /// <summary>
    /// Summary description for SafetyCheckFaultPhotoHandler
    /// </summary>
    public class SafetyCheckFaultPhotoHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "image/jpg";

            #region Extract QS values

            bool parameterErrorDetected = false;
            Guid safetyCheckFaultPhotoId = Guid.Empty;

            if (context.Request.QueryString["scfi"] != null)
            {
                try
                {
                    safetyCheckFaultPhotoId = Guid.Parse(context.Request.QueryString["scfi"]);
                }
                catch (Exception ex)
                {
                    parameterErrorDetected = true;
                }
            }
            else
            {
                parameterErrorDetected = true;
            }

            #endregion

            byte[] binaryData = new byte[0];
            
            if (!parameterErrorDetected && safetyCheckFaultPhotoId != Guid.Empty)
            {
                Guid customerId = new Guid(Configuration.BlueSphereCustomerId);                
                SafetyCheckCombinedResultsFaultImage record = MobileWorkerFlow.MWFServicesCommunication.Client.GetSafetyCheckResultFaultPhoto(customerId, safetyCheckFaultPhotoId);
                // Beware if the user provides a valid format guid that links back to no image then ImageBytes will be null and BianryWrite goes nuts so replace with an empty array
                binaryData = record.ImageBytes ?? new byte[0];
            }

            context.Response.AddHeader("Content-Disposition", "attachment; filename=" + safetyCheckFaultPhotoId + ".jpg");
            context.Response.BinaryWrite(binaryData);
            context.Response.Flush();
            context.Response.End();
        }

        public bool IsReusable
        {
            get { return false; }
        }

    }
}