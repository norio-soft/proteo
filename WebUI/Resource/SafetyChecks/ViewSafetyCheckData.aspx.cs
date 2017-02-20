using System.Drawing;
using System.Web.UI.HtmlControls;
using Orchestrator.Entities;
using Orchestrator.Globals;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Reporting;

namespace Orchestrator.WebUI.Resource.SafetyChecks
{
    public partial class View : System.Web.UI.Page
    {
        #region Constants
        private const string PHOTOS_VIEW_LINK_TEXT = "View";            // Hyperlink text for the "view" link when a fault has photo(s)
        private const string NON_BREAKING_SPACE_ENTITYREF = "&nbsp;";   // Whatever content to go in empty table cells; IE sucks
        private const string SIGNATURE_SAVE_AS_IMAGE_PREFIX = "scd_";
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

            #region Extract QS values
            bool paramterErrorDetected = false;
            Guid driverId = Guid.Empty;
            DateTime checkDate = DateTime.Now;
            if (Request.QueryString["dId"] != null && Request.QueryString["cDate"] != null)
            {
                try
                {
                    driverId = Guid.Parse(Request.QueryString["dId"]);
                    checkDate = DateTime.Parse(Request.QueryString["cDate"]);
                }
                catch (Exception ex) { paramterErrorDetected = true; }
            }
            else paramterErrorDetected = true;
            #endregion

            safetyCheckFaultListHeader.TableSection = TableRowSection.TableHeader;  // Required, sorry.
            if (!paramterErrorDetected)
            {
                if (!IsPostBack)
                {
                    LoadSafetyCheckResults(driverId, checkDate);
                }
            }
            // If any params are missing or in wrong format (or dont relate to each other) page simply loads with no data
            // This seems sufficient - it opens in a popup anyway, so won't throw an exception or show error here
        }

        /// <summary>
        /// Load the safety check data for the given driver at the specified date/time.
        /// </summary>
        /// <param name="driverId">Owner driver BS GUID</param>
        /// <param name="checkDate">The specific time of check to retrieve; accurate to seconds</param>
        private void LoadSafetyCheckResults(Guid driverId, DateTime checkDate)
        {
            Guid customerId = new Guid(Configuration.BlueSphereCustomerId);
            SafetyCheckCombinedResults record = MobileWorkerFlow.MWFServicesCommunication.Client.GetSafetyCheckResultComplete(customerId, driverId, checkDate);

            if (record.DriverId != Guid.Empty)
            {
                lblDriverTitle.Text = record.DriverTitle;

                if (record.VehicleSafetyCheckDataId != Guid.Empty)
                {
                    lblVehicleTitle.Text = record.VehicleTitle;
                    lblVehicleProfile.Text = record.VehicleSafetyCheckProfileTitle;
                    lblVehicleCheckDate.Text = ((DateTime) record.CheckedDate).ToString(CultureInfo.CurrentCulture); // ReSharper hates this line but I guarenteee CheckedDate will never be NULL
                    lblVehicleStatus.Text = record.VehicleSafetyCheckResultTerm;
                    lblVehicleStatus.CssClass =
                        faultStatusToCssClassName(
                            (SafetyCheckCombinedResultsFault.FaultStatusEnum) record.VehicleSafetyCheckResult);
                    appendFaultListIntoTable(record.VehicleFaultList, record.VehicleSafetyCheckProfileTitle);
                    vehicleSigImg.DataValue = record.VehicleSignature;
                }
                else
                {
                    lblVehicleTitle.Text = NON_BREAKING_SPACE_ENTITYREF;
                    lblVehicleProfile.Text = NON_BREAKING_SPACE_ENTITYREF;
                    lblVehicleCheckDate.Text = NON_BREAKING_SPACE_ENTITYREF;
                    lblVehicleStatus.Text = NON_BREAKING_SPACE_ENTITYREF;
                    lblVehicleStatus.CssClass = "scData";
                    //vehicleSigImg default imageUrl is a "n/a" image so do nothing.
                    vehicleSafetyCheckData.Style.Add("opacity", "0.2");
                }
                vehicleSigImg.SavedImageName = SIGNATURE_SAVE_AS_IMAGE_PREFIX + record.VehicleSafetyCheckDataId;

                if (record.TrailerSafetyCheckDataId != Guid.Empty)
                {
                    lblTrailerTitle.Text = record.TrailerTitle;
                    lblTrailerProfile.Text = record.TrailerSafetyCheckProfileTitle;
                    lblTrailerCheckDate.Text = ((DateTime)record.CheckedDate).ToString(CultureInfo.CurrentCulture);
                    lblTrailerStatus.Text = record.TrailerSafetyCheckResultTerm;
                    lblTrailerStatus.CssClass = faultStatusToCssClassName((SafetyCheckCombinedResultsFault.FaultStatusEnum)record.TrailerSafetyCheckResult);
                    appendFaultListIntoTable(record.TrailerFaultList, record.TrailerSafetyCheckProfileTitle);
                    trailerSigImg.DataValue = record.VehicleSignature;
                }
                else
                {
                    lblTrailerTitle.Text = NON_BREAKING_SPACE_ENTITYREF;
                    lblTrailerProfile.Text = NON_BREAKING_SPACE_ENTITYREF;
                    lblTrailerCheckDate.Text = NON_BREAKING_SPACE_ENTITYREF;
                    lblTrailerStatus.Text = NON_BREAKING_SPACE_ENTITYREF;
                    lblTrailerStatus.CssClass = "scData";
                    //trailerSigImg default imageUrl is a "n/a" image so do nothing.
                    trailerSafetyCheckData.Style.Add("opacity", "0.2"); // Opacity 0.2 looks cool but IE misalignment looks stupid. I prefer cool over IE.
                }
                trailerSigImg.SavedImageName = SIGNATURE_SAVE_AS_IMAGE_PREFIX + record.TrailerSafetyCheckDataId;
            }
        }

        /// <summary>
        /// Translate a safety check result into a CSS class
        /// </summary>
        /// <param name="status">safety check result status from enum</param>
        /// <returns>scPass, scFail or scDiscPass</returns>
        private string faultStatusToCssClassName(SafetyCheckCombinedResultsFault.FaultStatusEnum status)
        {
            string result = "scData ";
            switch (status)
            {
                case SafetyCheckCombinedResultsFault.FaultStatusEnum.Pass:
                    result += "scPass";
                    break;
                case SafetyCheckCombinedResultsFault.FaultStatusEnum.DiscretePass:
                    result += "scDiscPass";
                    break;
                case SafetyCheckCombinedResultsFault.FaultStatusEnum.Fail:
                    result += "scFail";
                    break;
                default:
                    result += "";
                    break;
            }
            return result;
        }

        private int faultListRowCount;
        /// <summary>
        /// Take a list of faults and append them into the fault table, putting hte parentProfileTitle into the appropriate column
        /// </summary>
        /// <param name="faultList">List of SafetyCheckCombinedResultsFault items</param>
        /// <param name="parentProfileTitle">Name of the profile to which these faults relate</param>
        private void appendFaultListIntoTable(List<Orchestrator.Entities.SafetyCheckCombinedResultsFault> faultList, string parentProfileTitle)
        {
            foreach (SafetyCheckCombinedResultsFault fault in faultList)
            {
                TableRow faultRow = new TableRow();
                if (faultListRowCount % 2 == 1) faultRow.CssClass = "odd";
                faultRow.TableSection = TableRowSection.TableBody;
                faultRow.Cells.Add(newCell(parentProfileTitle));
                faultRow.Cells.Add(newCell(fault.Title));
                faultRow.Cells.Add(newCell(fault.StatusTerm));
                faultRow.Cells.Add(newCell(fault.Comment));
                System.Web.UI.WebControls.TableCell photoCell = newCell(NON_BREAKING_SPACE_ENTITYREF);
                if (fault.HasPhotosYN)
                {
                    photoCell = new System.Web.UI.WebControls.TableCell();
                    System.Web.UI.WebControls.HyperLink viewLink = new System.Web.UI.WebControls.HyperLink();
                    viewLink.NavigateUrl = "javascript:openNewPopupWindow('Fault Photos','/Resource/SafetyChecks/ViewSafetyCheckFaultPhotos.aspx?scfId=" + fault.SafetyCheckFaultId + "');";
                    viewLink.Text = PHOTOS_VIEW_LINK_TEXT;
                    photoCell.Controls.Add(viewLink);
                }
                faultRow.Cells.Add(photoCell);
                safetyCheckFaultListTable.Rows.Add(faultRow);
                faultListRowCount++;
            }
        }

        /// <summary>
        /// New table cell (TD) with the given text content
        /// </summary>
        /// <param name="content">Textual content</param>
        private System.Web.UI.WebControls.TableCell newCell(string content) { return this.newCell(content, String.Empty); }
        /// <summary>
        /// New table cell (TD) with the given text content and css class
        /// </summary>
        /// <param name="content">Textual content</param>
        /// <param name="cssClass">CssClass to apply to the cell</param>
        private System.Web.UI.WebControls.TableCell newCell(string content, string cssClass)
        {
            System.Web.UI.WebControls.TableCell result = new System.Web.UI.WebControls.TableCell();
            result.Text = content;
            result.CssClass = cssClass;
            return result;
        }
    }
}