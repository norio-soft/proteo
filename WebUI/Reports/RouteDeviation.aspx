<%@ Page Language="C#" MasterPageFile="~/default_tableless.master" AutoEventWireup="true" CodeBehind="RouteDeviation.aspx.cs" Inherits="Orchestrator.WebUI.Reports.RouteDeviation" %>
<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Route Deviation</h1></asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<script language="javascript" type="text/javascript">
    // Function to show the filter options overlay box
    function FilterOptionsDisplayShow() {
        $("#overlayedClearFilterBox").css({ 'display': 'block' });
        $("#filterOptionsDiv").css({ 'display': 'none' });
        $("#filterOptionsDivHide").css({ 'display': 'block' });
        var fr = document.getElementById("overlayedIframe");
        fr.style.display = "block";
    }

    function FilterOptionsDisplayHide() {
        $("#overlayedClearFilterBox").css({ 'display': 'none' });
        $("#filterOptionsDivHide").css({ 'display': 'none' });
        $("#filterOptionsDiv").css({ 'display': 'block' });
        var fr = document.getElementById("overlayedIframe");
        fr.style.display = "none";

    }

    $(document).ready(function () {
        var width = $("#overlayedClearFilterBox").width();
        var height = $("#overlayedClearFilterBox").height();
        var position = $("#overlayedClearFilterBox").position();
        $("#overlayedIframe").css("width", width);
        $("#overlayedIframe").css("height", height + 25);
        $("#overlayedIframe").css("top", position.top);
        $("#overlayedIframe").css("left", position.left);
        FilterOptionsDisplayHide();

        /*Fix that allows menu bars to overlay reports when using Adobe PDF plugin for Google Chrome */
        window.onload = function () {
            changeIframeToObject();
        }

        function changeIframeToObject() {
            $("iframe[src*=\"/Reports\"]").replaceWith("<object data=\"/Reports/ActiveReportPdf.ashx\" type=\"application/pdf\" width=\"100%\" height=\"1000\" style=\"z-index: 1;\"></object>")
        }

        Telerik.Web.UI.Overlay.IsSupported = function () {
            return true;
        }
    });

  
</script>
    <telerik:RadAjaxLoadingPanel runat="server" ID="RadAjaxLoadingPanel1" />
    <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="DriversRadioButton">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="VehiclesRadioButton" />
                    <telerik:AjaxUpdatedControl ControlID="DriversRadioButton" />
                    <telerik:AjaxUpdatedControl ControlID="cboDriver" />
                    <telerik:AjaxUpdatedControl ControlID="cboVehicle" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="VehiclesRadioButton">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="VehiclesRadioButton" />
                    <telerik:AjaxUpdatedControl ControlID="DriversRadioButton" />
                    <telerik:AjaxUpdatedControl ControlID="cboVehicle"  />
                    <telerik:AjaxUpdatedControl ControlID="cboDriver" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>

        <h1>Route Deviation</h1>

                        <div class="ToolbarBlue" style="height: 24px; padding: 1px 1px 1px 3px; background-position: top;">
		    <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()" style="display: none;">Show filter Options</div>
		    <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">Close filter Options</div>
            <asp:Button ID="btnExport" runat="server" Text="Export" ValidationGroup="grpRefresh" CausesValidation="true" />
	    </div>
        <!--Hidden Filter Options-->
        <iframe id="overlayedIframe" style="position: absolute; z-index: 95; background:white;"></iframe>
        <div class="overlayedFilterBox" id="overlayedClearFilterBox" style="display: block;">
        <fieldset>
            <legend>Filter Options</legend>
            <table>
                <tr>
                    <td runat="server" id="tdDateOptions" >
                        <table>
                        <tr>
                             <td style="width:75px;">          
                                <asp:RadioButton ID="DriversRadioButton" runat="server" Checked="true" Text="Drivers" GroupName="ResourceTypeGroup" AutoPostBack="true" />  
                            </td>
                            <td>
                                <asp:RadioButton ID="VehiclesRadioButton" runat="server" Text="Vehicles" GroupName="ResourceTypeGroup" AutoPostBack="true" />
                            </td>
                            <td>
                                <telerik:RadComboBox ID="cboDriver" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                                   OnClientDropDownClosing="ValidateClientSideClosing" DataTextField="Description" DataValueField="ResourceId"
                                    MarkFirstMatch="true" ShowMoreResultsBox="false" AllowCustomText="true" Width="155px" Overlay="true" Height="250px"/>

                                <telerik:RadComboBox ID="cboVehicle" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                                    OnClientDropDownClosing="ValidateClientSideClosing" Visible="false" DataTextField="Description" DataValueField="ResourceId"
                                    MarkFirstMatch="true" ShowMoreResultsBox="false" AllowCustomText="true" Width="155px" Overlay="true" Height="250px" />
                            </td>
                        </tr>
                            <tr>                               
                                <td class="formCellLabel" style="width:150;" colspan="2" >Date From</td>
                                <td class="formCellField"><telerik:RadDatePicker id="dteStartDate" runat="server" Width="100" ToolTip="The Start Date for the filter">
                                <DateInput runat="server"
                                dateformat="dd/MM/yy">
                                </DateInput>
                                </telerik:RadDatePicker></td>
                                <td class="formCellField" style="width:30;"><asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ControlToValidate="dteStartDate" ValidationGroup="grpRefresh" ErrorMessage="Please enter a Start Date">
                                <img src="../../images/Error.gif" height="16" width="16" alt="Error" title="Please enter a start date." />
                                </asp:RequiredFieldValidator></td>
                            </tr><tr>
                                <td class="formCellLabel" style="width:150;" colspan="2">Date To</td>
                                <td class="formCellField"><telerik:RadDatePicker id="dteEndDate" Width="100" runat="server" ToolTip="The Start Date for the filter">
                                <DateInput runat="server"
                                dateformat="dd/MM/yy">
                                </DateInput>
                                </telerik:RadDatePicker></td>
                                <td class="formCellField" style="width:30;"><asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ControlToValidate="dteEndDate" ValidationGroup="grpRefresh" ErrorMessage="Please enter an End Date">
                                <img src="../../images/Error.gif" height="16" width="16" title="Please enter an end date." alt="error" />
                                </asp:RequiredFieldValidator></td>
                            </tr>
                            <tr>
                                <td class="formCellLabel" style="width:150;" colspan="2">Deviation %</td>
                                <td class="formCellField"><telerik:RadNumericTextBox id="txtDeviationPerc" runat="server" NumberFormat-DecimalDigits="0" Value="10" Width="30"></telerik:RadNumericTextBox></td>
                            </tr>
                            <tr>
                                <td class="formCellLabel" style="width:150;" colspan="2">Estimated Distance > </td>
                                <td class="formCellField"><telerik:RadNumericTextBox id="txtEstimatedDistance" runat="server" NumberFormat-DecimalDigits="0" Value="20" Width="50"></telerik:RadNumericTextBox> Miles</td>
                            </tr>
                       </table>
                    </td>
                </tr>
            </table>
        </fieldset>
                <div class="buttonbar">
            <asp:Button ID="btnRefresh" runat="server" Text="Refresh" ValidationGroup="grpRefresh" CausesValidation="true" />
            
        </div>   
        </div>
  
        <div>
            <uc1:ReportViewer id="reportViewer" runat="server" Visible="False" ViewerWidth="100%" ViewerHeight="800"></uc1:ReportViewer>
        </div>

    <telerik:RadCodeBlock runat="server" ID="RadCodeBlock1">
        <script type="text/javascript">
            function ValidateClientSideClosing(item) {
                if (item != null)
                    if (item.get_text().length > 0 && item.get_value().length < 1)
                        return false;
            }

            function showRoute(jobId) {
                var url = '/ng/run/' + jobId + '/route';
                window.open(url, "Legs");

            }
        </script>
    </telerik:RadCodeBlock>
        
</asp:Content>