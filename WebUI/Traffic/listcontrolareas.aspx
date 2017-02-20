<%@ Page language="c#" Inherits="Orchestrator.WebUI.ConrtolArea.ListControlAreas" Codebehind="ListControlAreas.aspx.cs" %>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>
<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>
<uc1:header id="Header1" title="Control Area Managment" SubTitle="Manage your control and traffic areas below." XMLPath="controlAreaContextMenu.xml" runat="server"></uc1:header>

<form id="Form1" method="post" runat="server">
	<asp:Label id="lblConfirmation" runat="server" CssClass="Confirmation"></asp:Label>
	<div align="center">
		<table width="99%" cellspacing="2" cellpadding="0" border="0">
			<tr valign="top">
				<td>
					<fieldset>
						<legend>Control Areas</legend>
						<table width="100%">
							<tr>
								<td valign="top" width="25%">
									Control Areas<br>
									<asp:DropDownList id="cboControlArea" runat="server" AutoPostBack="True"></asp:DropDownList>
									<asp:RequiredFieldValidator id="rfvControlArea" runat="server" ControlToValidate="cboControlArea" Display="Dynamic" ErrorMessage="Please select a control area to work with." InitialValue="-1"><img src="../images/error.gif" alt="Please select a control area to work with." /></asp:RequiredFieldValidator>
									<br>
									<asp:Button id="btnAddNewControlArea" runat="server" Text="Add" CausesValidation="False"></asp:Button>
									&nbsp;
									<nfvc:NoFormValButton id="btnAlterControlArea" runat="server" NoFormValList="rfvControlAreaTrafficAreas,rfvAddTrafficArea,rfvControlAreaDescription,cfvControlAreaDescription,,rfvTrafficArea,rfvTrafficAreaUsers,rfvAddPlanner,rfvTrafficAreaDescription,cfvTrafficAreaDescription,rfvTrafficAreaCode,rfvControlAreaForNewTrafficaArea,rfvMoveToControlArea" Text="Alter"></nfvc:NoFormValButton>
								</td>
								<td valign="top" width="25%">
									<asp:Panel id="pnlConfigureControlAreasTrafficAreas" runat="server">
										Traffic Areas<br>
										<asp:DropDownList id="cboControlAreaTrafficAreas" runat="server">
										</asp:DropDownList>
										<asp:RequiredFieldValidator id="rfvControlAreaTrafficAreas" runat="server" ControlToValidate="cboControlAreaTrafficAreas" Display="Dynamic" ErrorMessage="Please select a traffic area to remove."><img src="../images/error.gif" alt="Please select a traffic area to remove." /></asp:RequiredFieldValidator>
										<br>
										<asp:Button id="btnAddTrafficArea" runat="server" CausesValidation="False" Text="Add"></asp:Button>
									</asp:Panel>
								</td>
								<td valign="top" width="50%">
									<asp:Panel id="pnlAddTrafficAreaToControlArea" runat="server">
										Add Traffic Area<br>
										<asp:DropDownList id="cboAddTrafficArea" runat="server">
										</asp:DropDownList>
										<asp:RequiredFieldValidator id="rfvAddTrafficArea" runat="server" ControlToValidate="cboAddTrafficArea" Display="Dynamic" ErrorMessage="Please select a traffic area to add."><img src="../images/error.gif" alt="Please select a traffic area to add." /></asp:RequiredFieldValidator>
										<br>
										<nfvc:NoFormValButton id="btnSelectTrafficArea" runat="server" NoFormValList="rfvControlArea,rfvControlAreaTrafficAreas,rfvControlAreaDescription,cfvControlAreaDescription,,rfvTrafficArea,rfvTrafficAreaUsers,rfvAddPlanner,rfvTrafficAreaDescription,cfvTrafficAreaDescription,rfvTrafficAreaCode,rfvControlAreaForNewTrafficaArea,rfvMoveToControlArea" Text="Select"></nfvc:NoFormValButton>
									</asp:Panel>
								</td>
							</tr>
							<tr>
								<td colspan="3">
									<asp:Panel id="pnlConfigureControlArea" runat="server">
										<table>
											<tr>
												<td>Description</td>
												<td>
													<asp:TextBox id="txtControlAreaDescription" runat="server"></asp:TextBox>
													<asp:RequiredFieldValidator id="rfvControlAreaDescription" runat="server" ControlToValidate="txtControlAreaDescription" Display="Dynamic" ErrorMessage="Please specify the name of the control area."><img src="../images/error.gif" alt="Please select the name of the control area." /></asp:RequiredFieldValidator>
													<asp:CustomValidator id="cfvControlAreaDescription" runat="server" ControlToValidate="txtControlAreaDescription" Display="Dynamic" ErrorMessage="Please specify a unique control area name." EnableClientScript="False"><img src="../images/error.gif" alt="Please specify a unique control area name." /></asp:CustomValidator>
												</td>
											</tr>
											<tr>
												<td colspan="2">
													<asp:Button id="btnCancelControlArea" runat="server" CausesValidation="False" Text="Cancel"></asp:Button>
													&nbsp;
													<nfvc:NoFormValButton id="btnActionControlArea" runat="server" NoFormValList="rfvControlArea,rfvControlAreaTrafficAreas,rfvAddTrafficArea,rfvTrafficArea,rfvTrafficAreaUsers,rfvAddPlanner,rfvTrafficAreaDescription,cfvTrafficAreaDescription,rfvTrafficAreaCode,rfvMoveToControlArea" Text="Add"></nfvc:NoFormValButton>
												</td>
											</tr>
										</table>
									</asp:Panel>
								</td>
							</tr>
						</table>
					</fieldset>
				</td>
			</tr>
			<tr>
				<td>
					<fieldset>
						<legend>Traffic Areas</legend>
						<table width="100%">
							<tr>
								<td valign="top" width="25%">
									Traffic Areas<br>
									<asp:DropDownList id="cboTrafficArea" runat="server" AutoPostBack="True"></asp:DropDownList>
									<asp:RequiredFieldValidator id="rfvTrafficArea" runat="server" ControlToValidate="cboTrafficArea" Display="Dynamic" ErrorMessage="Please select a traffic area to work with." InitialValue="-1"><img src="../images/error.gif" alt="Please select a traffic area to work with." /></asp:RequiredFieldValidator>
									<br>
									<asp:Button id="btnAddNewTrafficArea" runat="server" Text="Add" CausesValidation="False"></asp:Button>
									&nbsp;
									<nfvc:NoFormValButton id="btnAlterTrafficArea" runat="server" NoFormValList="rfvControlArea,rfvControlAreaTrafficAreas,rfvAddTrafficArea,rfvControlAreaDescription,cfvControlAreaDescription,,rfvTrafficAreaUsers,rfvAddPlanner,rfvTrafficAreaDescription,cfvTrafficAreaDescription,rfvTrafficAreaCode,rfvControlAreaForNewTrafficaArea,rfvMoveToControlArea" Text="Alter"></nfvc:NoFormValButton>
								</td>
								<td valign="top" width="25%">
									<asp:Panel id="pnlConfigureTrafficAreaUsers" runat="server">
										Users<br>
										<asp:DropDownList id="cboTrafficAreaUsers" runat="server">
										</asp:DropDownList>
										<asp:RequiredFieldValidator id="rfvTrafficAreaUsers" runat="server" ControlToValidate="cboTrafficAreaUsers" Display="Dynamic" ErrorMessage="Please select a planner to remove."><img src="../images/error.gif" alt="Please select a planner to remove." /></asp:RequiredFieldValidator>
										<br>
										<asp:Button id="btnAddUser" runat="server" CausesValidation="False" Text="Add"></asp:Button>
										&nbsp;
										<nfvc:NoFormValButton id="btnRemoveUser" runat="server" NoFormValList="rfvControlArea,rfvControlAreaTrafficAreas,rfvAddTrafficArea,rfvControlAreaDescription,cfvControlAreaDescription,,rfvTrafficArea,rfvAddPlanner,rfvTrafficAreaDescription,cfvTrafficAreaDescription,rfvTrafficAreaCode,rfvControlAreaForNewTrafficaArea,rfvMoveToControlArea" Text="Remove"></nfvc:NoFormValButton>
									</asp:Panel>
								</td>
								<td valign="top" width="50%">
									<asp:Panel id="pnlAddUserToTrafficArea" runat="server">
										Add User<br>
										<asp:DropDownList id="cboAddPlanner" runat="server">
										</asp:DropDownList>
										<asp:RequiredFieldValidator id="rfvAddPlanner" runat="server" ControlToValidate="cboAddPlanner" Display="Dynamic" ErrorMessage="Please select a planner to add."><img src="../images/error.gif" alt="Please select a planner to add." /></asp:RequiredFieldValidator>
										<br>
										<nfvc:NoFormValButton id="btnSelectUser" runat="server" NoFormValList="rfvControlArea,rfvControlAreaTrafficAreas,rfvAddTrafficArea,rfvControlAreaDescription,cfvControlAreaDescription,,rfvTrafficArea,rfvTrafficAreaUsers,rfvTrafficAreaDescription,cfvTrafficAreaDescription,rfvTrafficAreaCode,rfvControlAreaForNewTrafficaArea,rfvMoveToControlArea" Text="Select"></nfvc:NoFormValButton>
									</asp:Panel>
								</td>
							</tr>
							<tr>
								<td colspan="3">
									<asp:Panel id="pnlConfigureTrafficArea" runat="server">
										<table>
											<tr>
												<td>Description</td>
												<td>
													<asp:TextBox id="txtTrafficAreaDescription" runat="server"></asp:TextBox>
													<asp:RequiredFieldValidator id="rfvTrafficAreaDescription" runat="server" ControlToValidate="txtTrafficAreaDescription" Display="Dynamic" ErrorMessage="Please specify the name of the traffic area."><img src="../images/error.gif" alt="Please select the name of the traffic area." /></asp:RequiredFieldValidator>
													<asp:CustomValidator id="cfvTrafficAreaDescription" runat="server" ControlToValidate="txtTrafficAreaDescription" Display="Dynamic" ErrorMessage="Please specify a unique traffic area name." EnableClientScript="False"><img src="../images/error.gif" alt="Please specify a unique traffic area name." /></asp:CustomValidator>
												</td>
											</tr>
											<tr>
												<td>Control Area</td>
												<td>
													<asp:DropDownList id="cboControlAreaForNewTrafficArea" runat="server" InitialValue="-1"></asp:DropDownList>
													<asp:RequiredFieldValidator id="rfvControlAreaForNewTrafficaArea" runat="server" ControlToValidate="cboControlAreaForNewTrafficArea" Display="Dynamic" ErrorMessage="Please select a control area for this traffic area."><img src="../images/error.gif" alt="Please select a control area for this traffic area." /></asp:RequiredFieldValidator>
												</td>
											</tr>
											<tr>
												<td colspan="2">
													<asp:Button id="btnCancelTrafficArea" runat="server" CausesValidation="False" Text="Cancel"></asp:Button>
													&nbsp;
													<nfvc:NoFormValButton id="btnActionTrafficArea" runat="server" NoFormValList="rfvControlArea,rfvControlAreaTrafficAreas,rfvAddTrafficArea,rfvControlAreaDescription,cfvControlAreaDescription,,rfvTrafficArea,rfvTrafficAreaUsers,rfvAddPlanner,rfvMoveToControlArea" Text="Add"></nfvc:NoFormValButton>
												</td>
											</tr>
										</table>
									</asp:Panel>
								</td>
							</tr>
						</table>
					</fieldset>
				</td>
			</tr>
		</table>
	</div>
</form>

<uc1:footer id="Footer1" runat="server"></uc1:footer>
