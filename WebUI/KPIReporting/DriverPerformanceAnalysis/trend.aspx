<%@ Page language="c#" MasterPageFile="~/default_tableless.Master" Inherits="Orchestrator.WebUI.KPIReporting.DriverPerformanceAnalysis.Trend" Codebehind="Trend.aspx.cs" Title="Driver Performance Analysis (Trend)" %>


<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>
<%@ Register TagPrefix="uc1" TagName="reportViewer" Src="~/UserControls/ReportViewer.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Driver Performance Analysis (Trend)</h1></asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">

	<fieldset>
		<legend><strong>Driver Performance Analysis</strong></legend>
		<table width="100%" border="0" cellpadding="1" cellspacing="0">
			<tr>
				<td width="100">Select Driver</td>
				<td colspan="2">
                    <telerik:RadComboBox ID="cboDriver" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                        MarkFirstMatch="true" Overlay="true" ZIndex="50" Height="400px"
                        ShowMoreResultsBox="true" Skin="WindowsXP" Width="355px">
                    </telerik:RadComboBox>

					<asp:RequiredFieldValidator id="rfvDriver" runat="server" ControlToValidate="cboDriver" Display="Dynamic" ErrorMessage="Please select a driver.">
						<img src="../../images/Error.gif" height="16" width="16" title="Please select a driver." /></asp:RequiredFieldValidator>
					<nfvc:NoFormValButton id="btnSelectDriver" runat="server" Text="Select Driver" NoFormValList="rfvStartDate,cfvStartDate,rfvEndDate"></nfvc:NoFormValButton>
				</td>
			</tr>
			<tr>
				<td width="100">Selected Drivers</td>
				<td colspan="2">
					<asp:DataGrid id="dgDrivers" runat="server" AutoGenerateColumns="False" AllowPaging="False" AllowSorting="False"
						visible="False">
						<Columns>
							<asp:BoundColumn HeaderText="ResourceId" DataField="ResourceId" Visible="False"></asp:BoundColumn>
							<asp:TemplateColumn HeaderText="Driver Name">
								<ItemTemplate>
									<%# ((Orchestrator.Entities.Driver) Container.DataItem).Individual.FullName %>
								</ItemTemplate>
								<ItemStyle Width="400"></ItemStyle>
							</asp:TemplateColumn>
							<asp:ButtonColumn HeaderText="" ButtonType="PushButton" Text="Remove" CommandName="RemoveDriver"></asp:ButtonColumn>
						</Columns>
						<itemstyle cssclass="DataGridListItem"></itemstyle>
						<headerstyle cssclass="DataGridListHead"></headerstyle>
						<alternatingitemstyle cssclass="DataGridListItemAlt"></alternatingitemstyle>
					</asp:DataGrid>
					<asp:Label id="lblNote" runat="server"></asp:Label>
				</td>
			</tr>
			<tr>
				<td>Report</td>
				<td colspan="2">
					<asp:DropDownList id="cboTrendPeriod" runat="server"></asp:DropDownList>
				</td>
			</tr>
			<tr>
				<td>Start Date</td>
				<td width="80"><telerik:RadDatePicker id="dteStartDate" runat="server" Width="100">
                <DateInput runat="server"
                dateformat="dd/MM/yy">
                </DateInput>
                </telerik:RadDatePicker></td>
				<td width="100%">
					<asp:RequiredFieldValidator id="rfvStartDate" runat="server" ControlToValidate="dteStartDate" ErrorMessage="Please specify a start date."
						Display="Dynamic">
						<img src="../../images/Error.gif" height="16" width="16" title="Please specify a start date." /></asp:RequiredFieldValidator>
					<asp:CustomValidator id="cfvStartDate" runat="server" ControlToValidate="dteStartDate" ErrorMessage="The start date must be before the end date.">
						<img src="../../images/Error.gif" height="16" width="16" title="The start date must be before the end date." /></asp:CustomValidator>
				</td>
			</tr>
			<tr>
				<td>End Date</td>
				<td><telerik:RadDatePicker id="dteEndDate" runat="server" Width="100">
                <DateInput runat="server"
                dateformat="dd/MM/yy">
                </DateInput>
                </telerik:RadDatePicker></td>
				<td><asp:RequiredFieldValidator id="rfvEndDate" runat="server" ControlToValidate="dteEndDate" ErrorMessage="Please specify an end date."><img src="../../images/Error.gif" height="16" width="16" title="Please specify an end date." /></asp:RequiredFieldValidator></td>
			</tr>
		</table>
		<div class="buttonbar">
			<nfvc:NoFormValButton id="btnViewReport" runat="server" Text="View Report" NoFormValList="rfvDriver"></nfvc:NoFormValButton>
		</div>
	</fieldset>

	<uc1:reportViewer id="reportViewer1" runat="server" visible="false"></uc1:reportViewer>

</asp:Content>