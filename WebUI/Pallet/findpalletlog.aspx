<%@ Reference Control="~/usercontrols/reportviewer.ascx" %>
<%@ Page language="c#" Inherits="Orchestrator.WebUI.Pallet.FindPalletLog" Codebehind="FindPalletLog.aspx.cs" MasterPageFile="~/default_tableless.master" Title="Generate Pallet Log" %>
<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>
<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>


<asp:content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<asp:validationsummary id="vsFindPalletLog" runat="server" ShowMessageBox="True" ShowSummary="False"></asp:validationsummary>
	<h1>Generate Pallet Log</h1>
	<h2>Generate a pallet log report by completing all the information below.</h2>
	<fieldset>
		<legend>Client</legend>
		<table>
			<tr>
				<td class="formCellLabel" style="width: 70px;">Client:</td>
				<td class="formCellField">
                    <telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                        MarkFirstMatch="true" RadControlsDir="~/script/RadControls/"
                        ShowMoreResultsBox="false" Skin="Orchestrator" SkinsPath="~/RadControls/ComboBox/Skins/" Width="355px" AllowCustomText="false" Height="300px">
                    </telerik:RadComboBox>
				</td>
				<td class="formCellField">
					<asp:RequiredFieldValidator id="rfvClient" runat="server" Display="Dynamic" ControlToValidate="cboClient" ErrorMessage="Please select/enter a client.">
						<img src="../images/newMasterPage/icon-warning.png" height="16" width="16" title="Please select/enter a client."/>
					</asp:RequiredFieldValidator>
				</td>
			</tr>
		</table>
	</fieldset>
	<fieldset>
		<legend>Date From and To</legend>
		<table>
			<tr>
				<td class="formCellLabel" style="width: 70px;">
				    Date From:
				</td>
				<td class="formCellField"> 
					<telerik:RadDateInput id="dteStartDate" runat="server" dateformat="dd/MM/yy"></telerik:RadDateInput></td>
				<td class="formCellField" style="width: 500px;">
					<asp:RequiredFieldValidator id="rfvDateFrom" runat="server" Display="Dynamic" ControlToValidate="dteStartDate" ErrorMessage="Please enter the start date. ">
					    <img src="../images/newMasterPage/icon-warning.png" height='16' width='16' title='Please enter the start date.'>
					</asp:RequiredFieldValidator>
				</td>
			</tr>
			<tr>
				<td class="formCellLabel">
					Date To: 
				</td>
				<td class="formCellField"> 
					<telerik:RadDateInput id="dteEndDate" runat="server" dateformat="dd/MM/yy"></telerik:RadDateInput>
				</td>
				<td class="formCellField">
					<asp:RequiredFieldValidator id="rfvDateTo" runat="server" Display="Dynamic" ControlToValidate="dteEndDate" ErrorMessage="Please enter the end date.">
					    <img src="../images/newMasterPage/icon-warning.png" height='16' width='16' title='Please enter the end date.'>
					</asp:RequiredFieldValidator>
				</td>
			</tr>
			<tr>
				<td class="formCellField" colspan="3">
					<asp:Checkbox id="chkPCVRequired" runat="server" Text="ONLY display deliveries requiring a PCV where one was not added (i.e. less pallets returned than delivered)" Text-Align="Left"></asp:Checkbox>
				</td>
			</tr>
		</table>
	</fieldset>
	<div class="buttonbar">
		<nfvc:NoFormValButton id="btnSearch" runat="server" text="Get Pallet Log" onclick="btnSearch_Click"></nfvc:NoFormValButton>
		<nfvc:NoFormValButton id="btnClear" runat="server" text="Clear" Width="75"></nfvc:NoFormValButton>
	</div>
	<uc1:ReportViewer id="reportViewer" runat="server" Visible="False" ViewerWidth="100%" ViewerHeight="800"></uc1:ReportViewer>
</asp:content>
