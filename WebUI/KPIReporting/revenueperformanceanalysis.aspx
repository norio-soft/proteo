<%@ Page language="c#" Inherits="Orchestrator.WebUI.KPIReporting.RevenuePerformanceAnalysis" MasterPageFile="~/default_tableless.Master"   Title="Revenue Performance Analysis" Codebehind="RevenuePerformanceAnalysis.aspx.cs" %>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>
<%@ Register TagPrefix="uc1" TagName="reportViewer" Src="~/UserControls/ReportViewer.ascx" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>
<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>KPI Reporting</h1>
    <h2>Revenue Performance Analysis</h2>
    <fieldset>
	    <legend>Revenue Performance Analysis</legend>
		<table>
			<tr>
				<td class="formCellLabel">Start Date</td>
				<td class="formCellInput"><telerik:RadDatePicker id="dteStartDate" Width="100" runat="server">
                <DateInput runat="server"
                dateformat="dd/MM/yy"></DateInput>
                </telerik:RadDatePicker></td>
				<td class="formCellInput">
					<asp:RequiredFieldValidator id="rfvStartDate" runat="server" ControlToValidate="dteStartDate" ErrorMessage="Please specify a start date." Display="Dynamic"><img src="../images/Error.gif" height="16" width="16" title="Please specify a start date." /></asp:RequiredFieldValidator>
					<asp:CustomValidator id="cfvStartDate" runat="server" ControlToValidate="dteStartDate" ErrorMessage="The start date must be before the end date."><img src="../images/Error.gif" height="16" width="16" title="The start date must be before the end date." /></asp:CustomValidator>
				</td>
			</tr>
			<tr>
				<td class="formCellLabel">End Date</td>
				<td class="formCellInput"><telerik:RadDatePicker id="dteEndDate" runat="server" Width="100">
                <DateInput runat="server"
                dateformat="dd/MM/yy">
                </DateInput>
                </telerik:RadDatePicker></td>
				<td class="formCellInput"><asp:RequiredFieldValidator id="rfvEndDate" runat="server" ControlToValidate="dteEndDate" ErrorMessage="Please specify an end date."><img src="../images/Error.gif" height="16" width="16" title="Please specify an end date." /></asp:RequiredFieldValidator></td>
			</tr>
			<tr>
				<td colspan="3"><asp:CheckBox id="chkShowNumberOfJobs" runat="server" Text="Show Number of Jobs" TextAlign="Right"></asp:CheckBox></td>
			</tr>
		</table>
	</fieldset>
	<div class="buttonbar">
		<nfvc:NoFormValButton id="btnViewReport" runat="server" Text="View Report" NoFormValList="rfvDriver"></nfvc:NoFormValButton>
	</div>
    <uc1:reportViewer id="reportViewer1" runat="server" visible="false"></uc1:reportViewer>
</asp:Content>
