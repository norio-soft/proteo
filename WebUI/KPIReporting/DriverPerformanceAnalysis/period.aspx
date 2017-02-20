<%@ Page language="c#" MasterPageFile="~/default_tableless.Master"  Inherits="Orchestrator.WebUI.KPIReporting.DriverPerformanceAnalysis.Period" Title="Driver Performance Analysis (Period)" Codebehind="Period.aspx.cs" %>

<%@ Register TagPrefix="uc1" TagName="reportViewer" Src="/UserControls/ReportViewer.ascx" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>


<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Driver Performance Analysis (Period)</h1></asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">

<fieldset>
	<legend><strong>Driver Performance Analysis</strong></legend>
	<table width="100%" border="0" cellpadding="1" cellspacing="0">
		<tr>
			<td width="150">Driver</td>
			<td colspan="2">
                <telerik:RadComboBox ID="cboDriver" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                    MarkFirstMatch="true" ShowMoreResultsBox="true" Width="355px">
                </telerik:RadComboBox>
				<asp:RequiredFieldValidator id="rfvDriver" runat="server" ControlToValidate="cboDriver" ErrorMessage="Please select a driver."><img src="../../images/Error.gif" height="16" width="16" title="Please select a driver." /></asp:RequiredFieldValidator>
			</td>
		</tr>
		<tr>
			<td nowrap="TRUE">Start Date</td>
			<td width="80"><telerik:RadDatePicker id="dteStartDate" Width="100" runat="server">
            <DateInput runat="server"
            dateformat="dd/MM/yy">
            </DateInput>
            </telerik:RadDatePicker></td>
			<td >
				<asp:RequiredFieldValidator id="rfvStartDate" runat="server" ControlToValidate="dteStartDate" ErrorMessage="Please specify a start date." Display="Dynamic"><img src="../../images/Error.gif" height="16" width="16" title="Please specify a start date." /></asp:RequiredFieldValidator>
				<asp:CustomValidator id="cfvStartDate" runat="server" ControlToValidate="dteStartDate" ErrorMessage="The start date must be before the end date."><img src="../../images/Error.gif" height="16" width="16" title="The start date must be before the end date." /></asp:CustomValidator>
			</td>
		</tr>
		<tr>
			<td>End Date</td>
			<td><telerik:RadDatePicker id="dteEndDate" runat="server" Width="100">
            <DateInput runat="server"
            dateformat="dd/MM/yy">
            </DateInput>
            </telerik:RadDatePicker></td>
			<td><asp:RequiredFieldValidator id="rfvEndDate" runat="server" ControlToValidate="dteEndDate" ErrorMessage="Please specify an end date."><img src="../../images/Error.gif" height="16" width="16" title="Please specify an end date." /></asp:RequiredFieldValidator>
		</tr>
	</table>
	<div class="buttonbar">
		<nfvc:NoFormValButton id="btnViewReport" runat="server" Text="View Report"></nfvc:NoFormValButton>
	</div>
</fieldset>

<uc1:reportViewer id="reportViewer1" runat="server" visible="false"></uc1:reportViewer>

</asp:Content>