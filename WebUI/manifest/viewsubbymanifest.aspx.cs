using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Orchestrator.WebUI.manifest
{
    public partial class viewsubbymanifest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int resourceManifestID = -1;
            bool excludeFirstLine = false;
            int extraRows = 0;
            bool usePlannedTimes = true;
            bool includeScript = false;
            bool showFullAddress = false;
            bool useInstructionOrder = true;

            int.TryParse(Request.QueryString["rmID"], out resourceManifestID);
            bool.TryParse(Request.QueryString["excludeFirstLine"], out excludeFirstLine);
            int.TryParse(Request.QueryString["extraRows"], out extraRows);
            bool.TryParse(Request.QueryString["usePlannedTimes"], out usePlannedTimes);
            bool.TryParse(Request.QueryString["useScript"], out includeScript);
            bool.TryParse(Request.QueryString["showFullAddress"], out showFullAddress);
            bool.TryParse(Request.QueryString["useInstructionOrder"], out useInstructionOrder);

            GenerateAndShowManifest(resourceManifestID, excludeFirstLine, extraRows, usePlannedTimes, includeScript, showFullAddress);
        }

        #region Manifest Creation

        private void GenerateAndShowManifest(int resourceManifestID, bool excludeFirstLine, int extraRows, bool usePlannedTimes, bool includeScript, bool showFullAddress)
        {
            Facade.ResourceManifest facResourceManifest = new Orchestrator.Facade.ResourceManifest();
            Entities.ResourceManifest rm = new Orchestrator.Entities.ResourceManifest();
            rm = facResourceManifest.GetResourceManifest(resourceManifestID);

            // Retrieve the resource manifest 
            NameValueCollection reportParams = new NameValueCollection();
            DataSet manifests = new DataSet();
            manifests.Tables.Add(ManifestGeneration.GetSubbyManifest(rm.ResourceManifestId, rm.SubcontractorId, usePlannedTimes, excludeFirstLine, showFullAddress, true));

            if (manifests.Tables[0].Rows.Count > 0)
            {
                // Add blank rows if applicable
                if (extraRows > 0)
                    for (int i = 0; i < extraRows; i++)
                    {
                        DataRow newRow = manifests.Tables[0].NewRow();
                        manifests.Tables[0].Rows.Add(newRow);
                    }

                //-------------------------------------------------------------------------------------	
                //									Load Report Section 
                //-------------------------------------------------------------------------------------	
                reportParams.Add("ManifestName", rm.Description);
                reportParams.Add("ManifestID", rm.ResourceManifestId.ToString());
                reportParams.Add("UsePlannedTimes", usePlannedTimes.ToString());
                Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.RunSheet;
                Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;
                Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = manifests;
                Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = "";
                Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";

                // Show the user control
                if (includeScript)
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", "<script language=\"javascript\">location.href='/reports/reportviewer.aspx?wiz=true';</script>");
            }
        }

        #endregion
    }
}
