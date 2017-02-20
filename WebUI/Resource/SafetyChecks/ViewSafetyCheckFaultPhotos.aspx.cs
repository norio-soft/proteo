using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Orchestrator.Entities;
using Orchestrator.Globals;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Resource.SafetyChecks
{
    public partial class Photos : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

            #region Extract QS values
            bool paramterErrorDetected = false;
            Guid safetyCheckFaultId = Guid.Empty;
            if (Request.QueryString["scfId"] != null)
            {
                try
                {
                    safetyCheckFaultId = Guid.Parse(Request.QueryString["scfId"]);
                }
                catch (Exception ex) { paramterErrorDetected = true; }
            }
            else paramterErrorDetected = true;
            #endregion

            if (!paramterErrorDetected && safetyCheckFaultId != Guid.Empty)
            {
                if (!IsPostBack)
                {
                    LoadSafetyCheckFaultPhotos(safetyCheckFaultId);
                }
            }
            // If any param error is detected, or even a valid format GUID that doesnt link back to a SafetyCheckFaultID
            // then the page will load with nothing on it; this seems adequate so no need for an else/throw error here
        }

        // Actually, images are 800x600 but pic quality is so poor we'll let the browser scale them down
        // Users are still served the 800x600 version this page just makes it look nice
        private const int TT_CAMERA_PHOTO_WIDTH = 400;
        private const int TT_CAMERA_PHOTO_HEIGHT = 300;

        private const string IMAGE_HANDLER_RELATIVE_PATH = "SafetyCheckFaultPhotoHandler.ashx";
        private const string QUERYSTRING_PREFIX = "?scfi=";

        private void LoadSafetyCheckFaultPhotos(Guid safetyCheckFaultId)
        {
            Guid customerId = new Guid(Configuration.BlueSphereCustomerId);

            // Get the list of photos for this fault:
            List<SafetyCheckCombinedResultsFaultImage> data = MobileWorkerFlow.MWFServicesCommunication.Client.GetSafetyCheckResultFaultPhotos(customerId, safetyCheckFaultId);
            int imageCounter = 1;
            foreach (SafetyCheckCombinedResultsFaultImage image in data)
            {
                Image imageControl = new Image();
                string altText = "Photo #" + imageCounter; // Something friendly for users to distinguish when there are multiple photos
                imageControl.AlternateText = altText;
                imageControl.ToolTip = altText;
                imageControl.Width = TT_CAMERA_PHOTO_WIDTH;
                imageControl.Height = TT_CAMERA_PHOTO_HEIGHT;
                // Note that the image handler will take care of setting the content-type and attachment name
                imageControl.ImageUrl = IMAGE_HANDLER_RELATIVE_PATH + QUERYSTRING_PREFIX + image.SafetyCheckFaultImageId;
                imageWrapper.Controls.Add(imageControl);

                imageCounter++;
            }
        }
    }
}