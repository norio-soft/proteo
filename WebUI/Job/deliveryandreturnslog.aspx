<%@ Reference Control="~/usercontrols/reportviewer.ascx" %>

<%@ Page Language="c#" Inherits="Orchestrator.WebUI.Job.DeliveryAndReturnsLog" CodeBehind="DeliveryAndReturnsLog.aspx.cs" %>

<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components" %>
<%@ Register TagPrefix="cc3" Namespace="P1TP.Components.Web.UI" Assembly="P1TP.Components" %>
<asp:content id="Content1" contentplaceholderid="ContentPlaceHolder1" runat="Server">
	<fieldset>
		<legend>
			<strong>Client Delivery and Returns Log</strong></legend>
			
			<asp:Label id="lblbTitle" runat="server" Text="Automatic & Manually Produce a Daily Delivery and Returns Log"></asp:Label>
		<table>
			<tr>
				<td width="100">Client:</td>
				<td colspan="2">
                    <telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                        MarkFirstMatch="true" ShowMoreResultsBox="true" Width="355px">
                    </telerik:RadComboBox>
					<asp:RequiredFieldValidator id="rfvClient" runat="server" Display="Dynamic" ControlToValidate="cboClient" ErrorMessage="Please select/enter a client.">
						<img src="../images/Error.gif" height='16' width='16' title='Please select/enter a client.'></asp:RequiredFieldValidator>
				</TD>
			</tr>
			<tr>
				<td>Start&nbsp;Date:&nbsp;&nbsp;</td>
				<td width="80"> 
					<telerik:RadDateInput id="dteStartDate" runat="server" dateformat="dd/MM/yy"></telerik:RadDateInput>
				</td>
				<td width="100%">	
					<asp:RequiredFieldValidator id="rfvDateFrom" runat="server" Display="Dynamic" ControlToValidate="dteStartDate"
						ErrorMessage="Please enter the start date. ">
						<img src="../images/Error.gif" height='16' width='16' title='Please enter the start date.'></asp:RequiredFieldValidator>
					<asp:CustomValidator id="cfvStartDate" runat="server" OnServerValidate="cfvStartDate_ServerValidate" ControlToValidate="dteStartDate" ErrorMessage="The start date must be before the end date."><img src="../images/Error.gif" height="16" width="16" title="The start date must be before the end date." /></asp:CustomValidator>		
				</TD>
			</tr>
			<TR>
				<TD>End&nbsp;Date:&nbsp;&nbsp;</TD>
				<TD> 
					<telerik:RadDateInput id="dteEndDate" runat="server" dateformat="dd/MM/yy"></telerik:RadDateInput>
				</TD>
				<TD>
					<asp:RequiredFieldValidator id="rfvDateTo" runat="server" Display="Dynamic" ControlToValidate="dteEndDate" ErrorMessage="Please enter the end date.">
						<img src="../images/Error.gif" height='16' width='16' title='Please enter the end date.'></asp:RequiredFieldValidator></TD>
			</TR>
		</TABLE>
	</fieldset>
	<table>
		<tr>
			<td align="center">
				<asp:Label id="lblError" cssclass="ControlErrorMessage" visible="True" runat="server"/>
			</td>
		</tr>
	</table>
	<div class="buttonbar">
		<nfvc:NoFormValButton id="btnGenerate" ServerClick="btnGenerate_Click" runat="server" text="Generate Report"></nfvc:NoFormValButton>
		<input type="reset" value="Reset">
	</div>
		<uc1:ReportViewer id="reportViewer" runat="server" EnableViewState="False" Visible="False" ViewerWidth="100%" ViewerHeight="800"></uc1:ReportViewer>
</asp:content>
