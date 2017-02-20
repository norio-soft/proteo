<%@ Reference Control="~/usercontrols/reportviewer.ascx" %>
<%@ Page language="c#" Inherits="Orchestrator.WebUI.Job.CallInSheet" MasterPageFile="~/default_tableless.Master" Codebehind="CallInSheet.aspx.cs" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>
<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Call In Sheet</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<script language="javascript" type="text/javascript">

    $(document).ready(function () {
        SetFilterArea();
        FilterOptionsDisplayHide();
    });

    window.onload = function () {
        changeIframeToObject();
    }

    function SetFilterArea() {
        var width = $("#overlayedClearFilterBox").width();
        var height = $("#overlayedClearFilterBox").height();
        var position = $("#overlayedClearFilterBox").position();
        $("#overlayedIframe").css("width", width);
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

    function changeIframeToObject() {
        $("iframe[src*=\"/Reports\"]").replaceWith("<object data=\"/Reports/ActiveReportPdf.ashx\" type=\"application/pdf\" width=\"100%\" height=\"1000\" style=\"z-index: 1;\"></object>")
    }

    Telerik.Web.UI.Overlay.IsSupported = function () {
        return true;
    }

    function isClientSelected(events, args)
    {
        var combo = $find("<%=cboClient.ClientID%>");
        if (combo.findItemByText(combo.get_text()) == null)
            args.IsValid = false;
        else
            args.IsValid = true;
    }
    
</script>
    <h2><asp:Label id="lblTitle" runat="server" Text="Completed runs for a client for a specified time period."></asp:Label></h2>
    <p>
	    <asp:Label ID="lblReportError" Runat="server" CssClass="ControlErrorMessage" Text="No Call Ins found for this client for this period." Visible="False"/>
    </p>
	<div id="overlayedToolbar" class="ToolbarBlue" style="height: 24px; padding: 1px 1px 1px 3px; background-position: top;">
	<div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()" style="display: none;">Show filter Options</div>
	<div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">Close filter Options</div>
    <nfvc:NoFormValButton id="btnExportToCSV" runat="server" text="Export To CSV"></nfvc:NoFormValButton>
        <nfvc:NoFormValButton id="btnExportDailyLog" runat="server" text="Export Daily Log"></nfvc:NoFormValButton>
	</div>
    <!--Hidden Filter Options-->
    <iframe id="overlayedIframe" style="position: absolute; z-index: 95; background:white;"></iframe>
    <div class="overlayedFilterBox" id="overlayedClearFilterBox" style="display: block; padding-bottom:5px;">
    <fieldset>
		<legend>Filter Options</legend>
        <table>
			<tr>
				<td class="formCellLabel">Client</td>
				<td class="formCellField">
                    <telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                        MarkFirstMatch="true" ShowMoreResultsBox="false" Skin="WindowsXP" Width="355px" AllowCustomText="false" Overlay="true">
                    </telerik:RadComboBox>
					<asp:RequiredFieldValidator id="rfvClient" runat="server" EnableClientScript="False" ControlToValidate="cboClient" ErrorMessage="Please supply a client to report on."><img src="../images/Error.gif" height="16" width="16" title="Please supply a client to report on." /></asp:RequiredFieldValidator>
                    <asp:CustomValidator ID="cfvClient" runat="server" ErrorMessage="Select a client from the list." ClientValidationFunction="isClientSelected" ControlToValidate="cboClient"></asp:CustomValidator>
				</td>
			</tr>
			<tr>
				<td class="formCellLabel">Start Date</td>
				<td class="formCellField">
					<telerik:RadDatePicker id="dteStartDate" runat="server" width ="100" ToolTip="The earliest date to report on.">
                    <DateInput runat="server"
                    dateformat="dd/MM/yy">
                    </DateInput>
                    </telerik:RadDatePicker>	
				</td>
				<td class="formCellField">	
					<asp:RequiredFieldValidator id="rfvStartDate" runat="server" ControlToValidate="dteStartDate" ErrorMessage="Please specify a start date." Display="Dynamic"><img src="../images/Error.gif" height="16" width="16" title="Please specify a start date." /></asp:RequiredFieldValidator>
					<asp:CustomValidator id="cfvStartDate" runat="server" OnServerValidate="cfvStartDate_ServerValidate" ControlToValidate="dteStartDate" ErrorMessage="The start date must be before the end date."><img src="../images/Error.gif" height="16" width="16" title="The start date must be before the end date." /></asp:CustomValidator>
				</td>
			</tr>
			<tr>
				<td class="formCellLabel">End Date</td>
				<td class="formCellField">
					<telerik:RadDatePicker id="dteEndDate" runat="server" Width="100" ToolTip="The last date to report on.">
                    <DateInput runat="server"
                    dateformat="dd/MM/yy">
                    </DateInput>
                    </telerik:RadDatePicker>
				</td>
				<td class="formCellField">	
					<asp:RequiredFieldValidator id="rfvEndDate" runat="server" ControlToValidate="dteEndDate" ErrorMessage="Please specify an end date." Display="Dynamic"><img src="../images/Error.gif" height="16" width="16" title="Please specify an end date." /></asp:RequiredFieldValidator>
				</td>
			</tr>
			<tr>
				<td class="formCellLabel">Order by</td>
				<td class="formCellField" colspan="3">
					<asp:RadioButtonList id="rblOrderBy" runat="server" RepeatDirection="Horizontal">
                        <asp:ListItem Text="Delivery Date" Value="ArrivalDateTime" Selected ="True"></asp:ListItem>
						<asp:ListItem Text="Booked Date" Value="BookedDateTime"></asp:ListItem>
						<asp:ListItem Text="Load Number" Value="NumericLoadNo"></asp:ListItem>
					</asp:RadioButtonList>
				</td>
			</tr>
			<tr>
				<td class="formCellLabel">Timing</td>
				<td class="formCellField" colspan="3">
					<asp:CheckBox runat="server" ID="chkShowLatesOnly" Text="Show Lates Only" />
				</td>
			</tr>
		</table>	
	</fieldset>
    	<div class="buttonbar">
		<nfvc:NoFormValButton id="btnReport" runat="server" Text="Generate Report"></nfvc:NoFormValButton>		
	</div>
    </div>

	<uc1:ReportViewer id="reportViewer" runat="server" Visible="False"></uc1:ReportViewer>
    <script type="text/javascript">
        FilterOptionsDisplayHide();
    </script>
</asp:Content>
