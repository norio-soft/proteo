<%@ Reference Page="~/job/wizard/wizard.aspx" %>
<%@ Control Language="c#" Inherits="Orchestrator.WebUI.Job.Wizard.UserControls.SelectClient" Codebehind="Client.ascx.cs" %>
<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>
<table width="100%" height="472" cellpadding="0" cellspacing="0" border="0">
	<tr height="56">
		<td bgcolor="white" align="right" style="PADDING-RIGHT:10px" >
			<table width="100%" cellpadding="0" cellspacing="0" align="right">
				<tr>
					<td width="100%" valign="top" align="left">
						<div style="PADDING-LEFT:20px; FONT-WEIGHT:bold; PADDING-TOP:5px">
							Client Selection
						</div>
						<div style="PADDING-LEFT:35px;PADDING-TOP:2px">
							Please select the client this job is for.
						</div>
					</td>
					<td align="right">
						<img src="../../images/p1logo.gif" width="50" height="50">			
					</td>
				</tr>
			</table>
		</td>
	</tr>	
	<tr height="2" bgcolor="#aca899" >
		<td colspan="2" height="3" style="BORDER-TOP:#aca899 1pt solid; BORDER-BOTTOM:white 1pt solid"></td>
	</tr>
	<tr height="305" bgcolor="#ece9d8">
		<td style="PADDING-RIGHT:10px;PADDING-LEFT:35px;VERTICAL-ALIGN:top; padding-top:10px;" width="100%">
			<div>
				<table width="100%" border="0" cellpadding="0" cellspacing="0">
					<tr>
						<td width="100">
							<span>Client Name</span>
						</td>
						<td>
                            <telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                                MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" Height="300px" Overlay="true"
                                ShowMoreResultsBox="false" Skin="WindowsXP" Width="355px" AllowCustomText="True">
                            </telerik:RadComboBox>
							<asp:RequiredFieldValidator id="rfvClient" runat="server" ControlToValidate="cboClient" ErrorMessage="Please select the client this job is for."><img src="../../images/error.png"  Title="Please select the client this job is for."></asp:RequiredFieldValidator>
						</td>
					</tr>
					<tr>
						<td colspan="2">
							<div class="spacer" style="height:10px;"></div>
						</td>
					</tr>
					<tr>
						<td colspan="2">
							<asp:Panel id="pnlCreateNewOrganisation" Runat="server" Visible="False">
								<fieldset>
									<legend>Select a Client</legend>
									<asp:CheckBox ID="chkCreateClient" Runat="server" Checked="False" Text="The client you've entered does not exist, would you like to create it?" TextAlign="Left" Visible="False"></asp:CheckBox>
									<uc1:infringementDisplay id="newClientInfringementDisplay" runat="server" Visible="false"></uc1:infringementDisplay>
									<div style="padding-top:10px">The client name you've entered does not match a client in the system.<br>You can create a <a href="../../Organisation/addupdateorganisation.aspx?wiz=true" target="_blank">new client</a> or <a href="../../Organisation/listclientcustomers.aspx?wiz=true" target="_blank">promote a client customer</a>.</div>
									<div style="padding-top:10px; display:none">If you choose to create this client, please ensure you complete the Client's information, via the Client area, as soon as possible.</div>
								</fieldset>
							</asp:Panel>
							<asp:Panel ID="pnlPromoteOrganisation" Runat="server" Visible="False">
								<fieldset>
									<legend>Promote Client Customer</legend>
									<asp:CheckBox ID="chkPromoteClientCustomer" Runat="server" Checked="False" Text="The client you've selected is currently configured as a client's customer, would you like to promote it to a client?" TextAlign="Left"></asp:CheckBox>
									<uc1:infringementDisplay id="promoteClientInfringementDisplay" runat="server" Visible="false"></uc1:infringementDisplay>
									<div style="padding-top:10px">If you choose to promote this client, please ensure you complete the Client's information, via the Client area, as soon as possible.</div>
								</fieldset>
							</asp:Panel>
						</td>
					</tr>
					<tr>
						<td width="100">
							<span>Business Type</span>
						</td>
						<td>
						    <asp:DropDownList ID="cboBusinessType" runat="server" DataValueField="BusinessTypeID" DataTextField="Description"></asp:DropDownList>
						</td>
				    </tr>
				</table>
			</div>
		</td>
	</tr>
	<tr height="2" bgcolor="#aca899" >
		<td colspan="2" height="3" style="BORDER-TOP:#aca899 1pt solid; BORDER-BOTTOM:white 1pt solid"></td>
	</tr>
	<tr height="46" bgcolor="#ece9d8" >
		<td colspan="2" align="right" height="46" style="PADDING-RIGHT:10px">
			<asp:button id="btnNext" runat="server" CausesValidation="True" Text="Next >"></asp:Button>
			&nbsp;&nbsp;
			<asp:Button ID="btnCancel" Runat="server" CausesValidation="False" Text="Cancel"></asp:Button>
		</td>
	</tr>
</table>