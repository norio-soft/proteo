<%@ Page language="c#" Inherits="Orchestrator.WebUI.Resource.Driver.WeekendSheet" Codebehind="WeekendSheet.aspx.cs" MasterPageFile="~/default_tableless.master" Title="Weekend Sheet" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>
<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Weekend Sheet</h1></asp:Content>	

<asp:content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script language="javascript" type="text/javascript">
    
    $(document).ready(function () {
        SetFilterArea();
        FilterOptionsDisplayHide();
    });

    function SetFilterArea() {
        var width = $("#overlayedClearFilterBox").width();
        var height = $("#overlayedClearFilterBox").height();
        var position = $("#overlayedClearFilterBox").position();
        $("#overlayedIframe").css("width", width + 10);
        $("#overlayedIframe").css("height", height + 25);
        $("#overlayedIframe").css("top", position.top);
        $("#overlayedIframe").css("left", position.left);
    }

    // Function to show the filter options overlay box
    function FilterOptionsDisplayShow() {
        $("#overlayedClearFilterBox").css({ 'display': 'block' });
        $("#filterOptionsDiv").css({ 'display': 'none' });
        $("#filterOptionsDivHide").css({ 'display': 'block' });

        SetFilterArea();

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

</script>
    <h2>Driver activity over the weekend and Monday.</h2>
                    <div class="ToolbarBlue" style="height: 24px; padding: 1px 1px 1px 3px; background-position: top;">
		    <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()" style="display: none;">Show filter Options</div>
		    <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">Close filter Options</div>
	    </div>
        <!--Hidden Filter Options-->
    <iframe id="overlayedIframe" style="position: absolute; z-index: 95; background:white;"></iframe>
    <div class="overlayedFilterBox" id="overlayedClearFilterBox" style="display: block; padding-bottom:5px;">
	<fieldset>
		<legend>Weekend sheet filter</legend>
		<table>
			<tr>
				<td class="formCellLabel">Depot:</td>
				<td class="formCellField"colspan="2"><asp:CheckBoxList id="chkTrafficArea" runat="server" DataValueField="TrafficAreaId" DataTextField="Description"></asp:CheckBoxList></td>
			</tr>
			<tr>
				<td class="formCellLabel">Start Date:</td>
				<td class="formCellField"><telerik:RadDatePicker id="dteStartDate" Width="100" runat="server" >
                <DateInput runat="server"
                dateformat="dd/MM/yy">
                </DateInput>
                </telerik:RadDatePicker></td>
				<td class="formCellField"><asp:RequiredFieldValidator id="rfvStartDate" runat="server" Display="Dynamic" ControlToValidate="dteStartDate" ErrorMessage="Please supply a start date."><img src="../../images/Error.gif" height="16" width="16" title="Please supply a start date." /></asp:RequiredFieldValidator></td>
			</tr>
			<tr>
				<td class="formCellLabel">End Date:</td>
				<td class="formCellField"><telerik:RadDatePicker id="dteEndDate" Width="100" runat="server">
                <DateInput runat="server"
                dateformat="dd/MM/yy">
                </DateInput>
                </telerik:RadDatePicker></td>
				<td class="formCellField"><asp:RequiredFieldValidator id="rfvEndDate" runat="server" Display="Dynamic" ControlToValidate="dteEndDate" ErrorMessage="Please supply an end date."><img src="../../images/Error.gif" height="16" width="16" title="Please supply an end date." /></asp:RequiredFieldValidator></td>
			</tr>
			<tr>
				<td class="formCellField" colspan="3">
					<asp:Label id="lblError" runat="server" cssclass="ControlErrorMessage" Visible="False"></asp:Label>
				</td>
			</tr>
		</table>
	</fieldset>
    	<div class="buttonbar">
		<nfvc:NoFormValButton id="btnReport" runat="server" Text="Generate Report"></nfvc:NoFormValButton>
	</div>
    </div>

	<uc1:ReportViewer id="reportViewer" runat="server" EnableViewState="False" Visible="False" Text="No driver activity found during specified time period."></uc1:ReportViewer>
    <script type="text/javascript">
        FilterOptionsDisplayHide();
    </script>		
</asp:content>
