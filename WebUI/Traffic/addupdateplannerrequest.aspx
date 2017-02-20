<%@ Page language="c#" Inherits="Orchestrator.WebUI.Traffic.AddUpdatePlannerRequest" Codebehind="AddUpdatePlannerRequest.aspx.cs" %>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>
<uc1:header id="Header1" runat="server" Title="List Planner Requests" SubTitle="A list of planner requests is shown below."></uc1:header>

<form id="Form1" method="post" runat="server">
	<div align="center">
		<table width="99%">
			<tr>
				<td width="60%">
					<fieldset>
						<legend>Planner Requests</legend>
						<asp:Label id="lblConfirmation" runat="server" CssClass="confirmation"></asp:Label>
						<table width="100%">
							<tr>
								<td valign="top">Source Job:</td>
								<td valign="top">
									<asp:TextBox id="txtSourceJobId" runat="server" Width="60px"></asp:TextBox>
									<asp:RequiredFieldValidator id="rfvSourceJobId" runat="server" Display="Dynamic" ControlToValidate="txtSourceJobId" ErrorMessage="Please supply the source job id."><img src="../images/Error.gif" height="16" width="16" title="Please supply the source job id." /></asp:RequiredFieldValidator>
									<asp:CustomValidator id="cfvSourceJobId" runat="server" Display="Dynamic" ControlToValidate="txtSourceJobId" ErrorMessage="Please supply a job id that exists in the system."><img src="../images/Error.gif" height="16" width="16" title="Please supply a job id that exists in the system." /></asp:CustomValidator>
								</td>
							</tr>
							<tr>
								<td valign="top">Target Job:</td>
								<td valign="top">
									<asp:TextBox id="txtTargetJobId" runat="server" Width="60px"></asp:TextBox>
									<asp:RequiredFieldValidator id="rfvTargetJobId" runat="server" Display="Dynamic" ControlToValidate="txtTargetJobId" ErrorMessage="Please supply the target job id."><img src="../images/Error.gif" height="16" width="16" title="Please supply the target job id." /></asp:RequiredFieldValidator>
									<asp:CustomValidator id="cfvTargetJobId" runat="server" Display="Dynamic" ControlToValidate="txtTargetJobId" ErrorMessage="Please supply a job id that exists in the system."><img src="../images/Error.gif" height="16" width="16" title="Please supply a job id that exists in the system." /></asp:CustomValidator>
								</td>
							</tr>
							<tr>
								<td valign="top">Resource Requirements:</td>
								<td valign="top">
									<asp:CheckBox id="chkUseDriver" runat="server" Text="Use Driver" TextAlign="Right"></asp:CheckBox>
									<br>
									<asp:CheckBox id="chkUseVehicle" runat="server" Text="Use Vehicle" TextAlign="Right"></asp:CheckBox>
									<br>
									<asp:CheckBox id="chkUseTrailer" runat="server" Text="Use Trailer" TextAlign="Right"></asp:CheckBox>
								</td>
							</tr>
							<tr>
								<td valign="top">Request Information:</td>
								<td valign="top">
									<asp:TextBox id="txtRequestText" runat="server" TextMode="MultiLine" Columns="40" Rows="6"></asp:TextBox>
									<asp:RequiredFieldValidator id="rfvRequestText" runat="server" Display="Dynamic" ControlToValidate="txtRequestText" ErrorMessage="Please supply the request text."><img src="../../images/Error.gif" height="16" width="16" title="Please supply the request text." /></asp:RequiredFieldValidator>
								</td>
							</tr>
							<tr>
								<td colspan="2">
									<div class="buttonbar">
										<asp:Button id="btnListRequest" runat="server" Text="List Requests" CausesValidation="False"></asp:Button>&nbsp;<asp:Button id="btnAdd" runat="server" Text="Add"></asp:Button>
									</div>
								</td>
							</tr>
						</table>
					</fieldset>
				</td>
				<td width="40%">&nbsp;</td>
			</tr>
		</table>
	</div>
</form>

<uc1:footer id="Footer1" runat="server"></uc1:footer>

