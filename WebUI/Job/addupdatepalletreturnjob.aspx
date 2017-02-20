<%@ Page language="c#" MasterPageFile="~/default_tableless.master" Inherits="Orchestrator.WebUI.Job.addupdatepalletreturnjob" Codebehind="addupdatepalletreturnjob.aspx.cs" %>

<%@ Register TagPrefix="uc1" TagName="reportViewer" Src="~/UserControls/ReportViewer.ascx" %>
<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <div style="height:10px;"></div>
    <asp:Label id="lblTitle" runat="server" Text="Please enter the Run Details below."></asp:Label>

	<table width="100%" >
		<tr>
			<td colspan="2">
			    <fieldset>	
				<table width="100%">
					<tr>
						<td><b>Run Id</b></td>
						<td width="100%" align="left"><span style="FONT-WEIGHT:bold; font-size:12px"><asp:Label id="lblJobId" runat="server"></asp:Label></span></td>
					</tr>
					<tr>
					    <td><b>Client</b></td>
					    <td width="100%" align="left">
						    <telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ShowMoreResultsBox="false" MarkFirstMatch="true" ItemRequestTimeout="500" Width="355px"></telerik:RadComboBox>
							<asp:RequiredFieldValidator id="rfvClient" runat="server" ControlToValidate="cboClient" ErrorMessage="Please select the client this job is for."><img src="../images/error.png"  Title="Please select the client this job is for."></asp:RequiredFieldValidator>
							<asp:CustomValidator id="cfvClient" runat="server" ControlToValidate="cboClient" EnableClientScript="False" ErrorMessage="Please select the client this job is for."><img src="../images/error.gif" alt="Please select the client this job is for." /></asp:CustomValidator>
					    </td>
					</tr>
				    <tr>
					    <td>
						    <span><b>Business Type</b></span>
					    </td>
					    <td>
					        <asp:DropDownList ID="cboBusinessType" runat="server" DataValueField="BusinessTypeID" DataTextField="Description"></asp:DropDownList>
					    </td>
			        </tr>
					<tr>
						<td nowrap=nowrap><b>Load Number</b></td>
						<td>
							<asp:TextBox id="txtLoadNumber" runat="server"></asp:TextBox>
							<asp:RequiredFieldValidator id="rfvLoadNumber" runat="server" ControlToValidate="txtLoadNumber" ErrorMessage="Please specify the load number for this pallet return job." Display="Dynamic"><img src="../images/Error.gif" height="16" width="16" title="Please specify the load number for this pallet return job." /></asp:RequiredFieldValidator>
						</td>
					</tr>
					<tr>
						<td><b>Charge Amount</b></td>
						<td>
							<asp:TextBox ID="txtChargeAmount" Runat="server"></asp:TextBox>
							<asp:RequiredFieldValidator ID="rfvChargeAmount" Runat="server" Display="Dynamic" ControlToValidate="txtChargeAmount" ErrorMessage="Please specify the charge amount."><img src="../images/error.gif"  Title="Please specify the charge amount."></asp:RequiredFieldValidator>
							<asp:RegularExpressionValidator ID="revChargeAmount" Runat="server" Display="Dynamic" ControlToValidate="txtChargeAmount" ValidationExpression="^(£|-£|£-|-)?([0-9]{1}[0-9]{0,2}(\,[0-9]{3})*(\.[0-9]{0,2})?|[1-9]{1}[0-9]{0,}(\.[0-9]{0,2})?|0(\.[0-9]{0,2})?|(\.[0-9]{1,2})?)$" ErrorMessage="Please enter a valid currency value for the charge amount."><img src="../images/error.gif"  Title="Please enter a valid currency value for the charge amount."></asp:RegularExpressionValidator>
						</td>
					</tr>
			    </table>
				<uc1:infringementDisplay id="infringementDisplay" runat="server" Visible="false"></uc1:infringementDisplay>
				</fieldset>
			</td>
		</tr>
		<tr valign="top">
			<td width="50%">
				<fieldset>
					<legend>Collections</legend>
					<table width="100%">
						<asp:Panel id="pnlAddCollection" runat="server">
							<tr>
								<td valign="top" rowspan="2"><b>Collect Pallets From</b></td>
								<td>
								    <telerik:RadComboBox ID="cboCollectClient" runat="server" EnableLoadOnDemand="true" ShowMoreResultsBox="true" MarkFirstMatch="true" ItemRequestTimeout="500" Width="355px" OnClientSelectedIndexChanged="CollectClientSelectedIndexChanged" DataTextField="OrganisationName" DataValueField="IdentityId" ></telerik:RadComboBox>
									<asp:RequiredFieldValidator id="rfvCollectClient" runat="server" ControlToValidate="cboCollectClient" ErrorMessage="Please select the client that owns the point to collect pallets from."><img src="../images/error.png"  Title="Please select the client that owns the point to collect pallets from."></asp:RequiredFieldValidator>
									<asp:CustomValidator id="cfvCollectClient" runat="server" ControlToValidate="cboCollectClient" EnableClientScript="False" ErrorMessage="Please select the client that owns the point to collect pallets from."><img src="../images/error.gif" alt="Please select the client that owns the point to collect pallets from." /></asp:CustomValidator>
									<asp:Label id="lblCollectionInstructionId" runat="server" Visible="False"></asp:Label>
								</td>
							</tr>
							<tr>
								<td>
								    <telerik:RadComboBox ID="cboCollectPoint" runat="server" EnableLoadOnDemand="true" ShowMoreResultsBox="true" MarkFirstMatch="true" ItemRequestTimeout="500" Width="355px" AutoPostBack="true" OnClientItemsRequesting="CollectPointRequesting" DataTextField="Description" DataValueField="PointId" CausesValidation="false" ></telerik:RadComboBox>
									<asp:RequiredFieldValidator ID="rfvCollectPoint" Runat="server" ControlToValidate="cboCollectPoint" ErrorMessage="Please select the point to collect the pallets from."><img src="../images/error.png"  Title="Please select the point to collect the pallets from."></asp:RequiredFieldValidator>
									<asp:CustomValidator id="cfvCollectPoint" runat="server" ControlToValidate="cboCollectPoint" EnableClientScript="False" ErrorMessage="Please select the point to collect the pallets from."><img src="../images/error.gif" alt="Please select the point to collect the pallets from." /></asp:CustomValidator>
								</td>
							</tr>
							<tr>
							    <td valign="top"><b>Pallet Type</b></td>
							    <td>
							        <telerik:RadComboBox ID="cboPalletType" runat="server" Width="355px" AutoPostBack="true" DataTextField="PalletTypeDescription" DataValueField="PalletTypeID" CausesValidation="false" ></telerik:RadComboBox>
									<asp:RequiredFieldValidator ID="rfvPalletType" Runat="server" ControlToValidate="cboPalletType" ErrorMessage="Please select the pallet type to collect."><img src="/images/error.png"  Title="Please select the pallet type to collect." alt="" /></asp:RequiredFieldValidator>
							    </td>
							</tr>
							<tr>
								<td><b>Collect Pallets on</b><asp:CustomValidator ID="cfvValidDateRange" Runat="server" ControlToValidate="dteCollectDate" Display="Dynamic" EnableClientScript="False" ErrorMessage="The Date must be no more than 1 day before today and no more than 3 days ahead of today."><img src="../images/error.png" Title="The Date must be no more than 1 day before today and no more than 3 days ahead of today."></asp:CustomValidator></td>
								<td>
									<table border="0" cellpadding="0" cellspacing="0">
										<tr>
											<td><telerik:RadDateInput id="dteCollectDate" runat="server" dateformat="dd/MM/yy" ToolTip="The date to collect the pallets." CausesValidation="false" ></telerik:RadDateInput></td>
											<td>
												<asp:RequiredFieldValidator id="rfvCollectDate" runat="server" ControlToValidate="dteCollectDate" ErrorMessage="Please specify the date to collect the pallets." Display="Dynamic"><img src="../images/Error.gif" height="16" width="16" title="Please specify the date to collect the pallets." /></asp:RequiredFieldValidator>
												<asp:CustomValidator id="cfvCollectDate" runat="server" EnableClientScript="False" ControlToValidate="dteCollectDate" ErrorMessage="The collection date must be before the first dehire date, and after the last completed collection." Display="Dynamic"><img src="../images/Error.gif" height="16" width="16" title="The collection date must be before the first dehire date, and after the last completed collection." /></asp:CustomValidator>
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td><b>Collect Pallets at</b></td>
								<td><telerik:RadDateInput id="dteCollectTime" runat="server" dateformat="t" ToolTip="The time to collect the pallets."  EmptyMessage="AnyTime" CausesValidation="false"></telerik:RadDateInput></td>
							</tr>
							<tr>
								<td><b>Number of Pallets (<asp:Label id="lblPalletsAvailable" runat="server"></asp:Label>)</b></td>
								<td>
									<asp:TextBox id="txtCollectPallets" runat="server" CausesValidation="false"></asp:TextBox>
									<asp:RequiredFieldValidator id="rfvCollectPallets" runat="server" ControlToValidate="txtCollectPallets" ErrorMessage="Please specify the number of pallets to collect." Display="Dynamic"><img src="../images/Error.gif" height="16" width="16" title="Please specify the number of pallets to collect." /></asp:RequiredFieldValidator>
									<asp:CustomValidator id="cfvCollectPallets" runat="server" EnableClientScript="False" ControlToValidate="txtCollectPallets" ErrorMessage="Please specify a number for the number of pallets to collect, this number of pallets must be available at the point." Display="Dynamic"><img src="../images/Error.gif" height="16" width="16" title="Please specify a number for the number of pallets to collect, this number of pallets must be available at the point." /></asp:CustomValidator>
								</td>
							</tr>
							<tr>
								<td colspan="2">
								    <div class="buttonbar">
								        <nfvc:NoFormValButton id="btnAddCollection" runat="server" NoFormValList="rfvLoadNumber,rfvChargeAmount,revChargeAmount,rfvDehireClient,cfvDehireClient,rfvDehireClientLocationOwner,cfvDehireClientLocationOwner,rfvDehirePoint,cfvDehirePoint,rfvDehireDate,rfvDehireTime,rfvDehirePallets,cfvDehirePallets" Text="Add Collection"></nfvc:NoFormValButton>
									    <asp:Button id="btnCancelCollectionChanges" runat="server" CausesValidation="False" Text="Cancel" width="75"></asp:Button>
								    </div>
								</td>
							</tr>
						</asp:Panel>
						<tr>
							<td colspan="2">
								<asp:Repeater ID="repCollections" Runat="server">
									<HeaderTemplate>
										<table width="100%"  cellpadding="1" cellspacing="0" bordercolor="#FFFFFF" class="PurpleBookHeader">
												<tr>
													<th valign="top">Client</th>
													<th valign="top">Town</th>
													<th valign="top">Point</th>
													<th valign="top">Booked</th>
													<th valign="top">Pallets</th>
													<th colspan="4">&nbsp;</th>
												</tr>
											<tbody bgcolor="#FFFFFF">
									</HeaderTemplate>
									<ItemTemplate>
										<tr style="font-size:11px;">
											<td valign="top"><%# ((Orchestrator.Entities.Instruction) Container.DataItem).Point.OrganisationName %><input type="hidden" id="hidInstructionId" runat="server" value="<%# ((Orchestrator.Entities.Instruction) Container.DataItem).InstructionID %>" NAME="hidInstructionId"></td>
											<td valign="top"><%# ((Orchestrator.Entities.Instruction) Container.DataItem).Point.PostTown.TownName %></td>
											<td valign="top"><%# ((Orchestrator.Entities.Instruction) Container.DataItem).Point.Description %></td>
											<td valign="top" ><asp:Label ID="lblBookedDateTime" Runat="server"></asp:Label></td>
											<td valign="top" ><%# ((Orchestrator.Entities.Instruction) Container.DataItem).TotalPallets.ToString() %></td>
											<td valign="top"><asp:Button ID="btnDown" Runat="server" CausesValidation="False" CommandName="Down" Text=" &#9660; " Height="24px"></asp:Button></td>
											<td valign="top"><asp:Button ID="btnUp" Runat="server" CausesValidation="False" CommandName="Up" Text=" &#9650; " Height="24px"></asp:Button></td>
											<td valign="top"><asp:Button ID="btnAlter" Runat="server" CausesValidation="False" CommandName="Alter" Text="&#8230;" Height="24px"></asp:Button></td>
											<td valign="top"><asp:Button ID="btnDelete" Runat="server" CausesValidation="False" CommandName="Delete" Text=" x " Height="24px"></asp:Button></td>
										</tr>
									</ItemTemplate>
									<SeparatorTemplate>
										<tr>
											<td colspan="9"><hr noshade></td>
										</tr>
									</SeparatorTemplate>
									<FooterTemplate>
										</table>
									</FooterTemplate>
								</asp:Repeater>
							</td>
						</tr>
						<tr>
							<td colspan="2">
							    <div class="buttonbar">
							        <asp:Button id="btnAddNewCollection" runat="server" Text="Add New Collection" CausesValidation="False"></asp:Button>
							    </div>
							</td>
						</tr>
					</table>
				</fieldset>
			</td>
			<td width="50%">
				<fieldset>
					<legend>De-hires</legend>
					<table width="100%" cellpadding="0" cellspacing="0">
						<asp:Panel id="pnlAddDehire" runat="server">
							<tr>
								<td><b>De-hire Pallets for</b></td>
								<td>
								    <telerik:RadComboBox ID="cboDehireClient" runat="server" EnableLoadOnDemand="true" ShowMoreResultsBox="true" MarkFirstMatch="true" ItemRequestTimeout="500" Width="355px" DataTextField="OrganisationName" DataValueField="IdentityId" ></telerik:RadComboBox>
									<asp:RequiredFieldValidator id="rfvDehireClient" runat="server" ControlToValidate="cboDehireClient" ErrorMessage="Please select the client to dehire the pallets for."><img src="../images/error.png"  Title="Please select the client to dehire the pallets for."></asp:RequiredFieldValidator>
									<asp:CustomValidator id="cfvDehireClient" runat="server" ControlToValidate="cboDehireClient" EnableClientScript="False" ErrorMessage="Please select the client to degire the pallets for."><img src="../images/error.gif" alt="Please select the client to dehire the pallets for." /></asp:CustomValidator>
									<asp:Label id="lblDehireInstructionId" runat="server" Visible="False"></asp:Label>
								</td>
							</tr>
							<tr>
								<td valign="top" rowspan="2"><b>De-hire Pallets At</b></td>
								<td>
								    <telerik:RadComboBox ID="cboDehireClientLocationOwner" runat="server" EnableLoadOnDemand="true" ShowMoreResultsBox="true" MarkFirstMatch="true" ItemRequestTimeout="500" Width="355px" OnClientSelectedIndexChanged="DehireLocationOwnerClientSelectedIndexChanged" DataTextField="OrganisationName" DataValueField="IdentityId"></telerik:RadComboBox>
									<asp:RequiredFieldValidator id="rfvDehireClientLocationOwner" runat="server" ControlToValidate="cboDehireClientLocationOwner" ErrorMessage="Please select the client that owns the point to dehire pallets at."><img src="../images/error.png"  Title="Please select the client that owns the point to dehire pallets at."></asp:RequiredFieldValidator>
									<asp:CustomValidator id="cfvDehireClientLocationOwner" runat="server" ControlToValidate="cboDehireClientLocationOwner" EnableClientScript="False" ErrorMessage="Please select the client that owns the point to dehire pallets at."><img src="../images/error.gif" alt="Please select the client that owns the point to dehire pallets at." /></asp:CustomValidator>
								</td>
							</tr>
							<tr>
								<td>
								    <telerik:RadComboBox ID="cboDehirePoint" runat="server" EnableLoadOnDemand="true" ShowMoreResultsBox="true" MarkFirstMatch="true" ItemRequestTimeout="500" Width="355px" OnClientItemsRequesting="DehirePointRequesting" DataTextField="Description" DataValueField="PointId"></telerik:RadComboBox>
									<asp:RequiredFieldValidator ID="rfvDehirePoint" Runat="server" ControlToValidate="cboDehirePoint" ErrorMessage="Please specify the point to dehire the pallets at."><img src="../images/error.png"  Title="Please specify the point to dehire the pallets at."></asp:RequiredFieldValidator>
									<asp:CustomValidator id="cfvDehirePoint" runat="server" ControlToValidate="cboDehirePoint" EnableClientScript="False" ErrorMessage="Please specify the point to dehire the pallets at."><img src="../images/error.gif" alt="Please specify the point to dehire the pallets at." /></asp:CustomValidator>
								</td>
							</tr>
							<tr>
							    <td valign="top"></td>
							    <td>
							        <telerik:RadComboBox ID="rcbDehirePalletType" runat="server" Width="355px" DataTextField="Value" DataValueField="Key" ></telerik:RadComboBox>
									<asp:RequiredFieldValidator ID="rfvDehirePalletType" Runat="server" ControlToValidate="rcbDehirePalletType" ErrorMessage="Please specify the pallet type to dehire."><img src="../images/error.png"  Title="Please specify the pallet type to dehire."></asp:RequiredFieldValidator>
							    </td>
							</tr>
							<tr>
								<td><b>De-hire Pallets on</b></td>
								<td>
									<table border="0" cellpadding="0" cellspacing="0">
										<tr>
											<td><telerik:RadDateInput id="dteDehireDate" runat="server" dateformat="dd/MM/yy" ToolTip="The date to collect the pallets."></telerik:RadDateInput></td>
											<td>
												<asp:RequiredFieldValidator id="rfvDehireDate" runat="server" ControlToValidate="dteDehireDate" ErrorMessage="Please specify the date to dehire the pallets." Display="Dynamic"><img src="../images/Error.gif" height="16" width="16" title="Please specify the date to dehire the pallets." /></asp:RequiredFieldValidator>
												<asp:CustomValidator id="cfvDehireDate" runat="server" EnableClientScript="False" ControlToValidate="dteDehireDate" ErrorMessage="The dehire date must be after the collection date, and the last completed dehire." Display="Dynamic"><img src="../images/Error.gif" height="16" width="16" title="The dehire date must be after the collection date, and the last completed dehire." /></asp:CustomValidator>
											</td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td><b>De-hire Pallets at</b></td>
								<td><telerik:RadDateInput id="dteDehireTime" runat="server" dateformat="t" ToolTip="The time to collect the pallets." DataMode="EditModeText" NullText="AnyTime" Nullable="True"></telerik:RadDateInput></td>
							</tr>
							<tr>
								<td><b>Number of Pallets</b></td>
								<td>
									<asp:TextBox id="txtDehirePallets" runat="server"></asp:TextBox>
									<asp:RequiredFieldValidator id="rfvDehirePallets" runat="server" ControlToValidate="txtDehirePallets" ErrorMessage="Please specify the number of pallets to dehire." Display="Dynamic"><img src="../images/Error.gif" height="16" width="16" title="Please specify the number of pallets to dehire." /></asp:RequiredFieldValidator>
									<asp:CustomValidator id="cfvDehirePallets" runat="server" EnableClientScript="False" ControlToValidate="txtDehirePallets" ErrorMessage="Please specify a number for the number of pallets to dehire." Display="Dynamic"><img src="../images/Error.gif" height="16" width="16" title="Please specify a number for the number of pallets to dehire." /></asp:CustomValidator>
								</td>
							</tr>
							<tr>
								<td colspan="2">
								    <div class="buttonbar">
									    <nfvc:NoFormValButton id="btnAddDehire" runat="server" NoFormValList="rfvLoadNumber,rfvChargeAmount,revChargeAmount,rfvCollectClient,cfvCollectClient,rfvCollectPoint,cfvCollectPoint,rfvCollectDate,rfvCollectTime,rfvCollectPallets,cfvCollectPallets" Text="Add Dehire"></nfvc:NoFormValButton>
									    <asp:Button id="btnCancelDehireChanges" runat="server" CausesValidation="False" Text="Cancel"></asp:Button>
									</div>
								</td>
							</tr>
						</asp:Panel>
						<tr>
							<td colspan="2">
								<asp:Repeater ID="repDehires" Runat="server">
									<HeaderTemplate>
										<table width="100%" border="1" cellpadding="1" cellspacing="0" bordercolor="#FFFFFF" class="PurpleBookHeader">
												<tr>
													<th valign="top" style="border-left:solid 1pt black;">De-Hire For</th>
													<th valign="top">Client</th>
													<th valign="top">Town</th>
													<th valign="top">Point</th>
													<th valign="top">Booked</th>
													<th valign="top">Pallets</th>
													<th colspan="4">&nbsp;</th>
												</tr>
											<tbody bgcolor="#FFFFFF">
									</HeaderTemplate>
									<ItemTemplate>
										<tr>
											<td valign="top"><asp:Label id="lblDeHireOrganisationName" runat="server"></asp:Label><input type="hidden" id="hidInstructionId" runat="server" value="<%# ((Orchestrator.Entities.Instruction) Container.DataItem).InstructionID %>"></td>
											<td valign="top"><%# ((Orchestrator.Entities.Instruction) Container.DataItem).Point.OrganisationName %><input type="hidden" id="Hidden1" runat="server" value="<%# ((Orchestrator.Entities.Instruction) Container.DataItem).InstructionID %>" NAME="hidInstructionId"></td>
											<td valign="top"><%# ((Orchestrator.Entities.Instruction) Container.DataItem).Point.PostTown.TownName %></td>
											<td valign="top"><%# ((Orchestrator.Entities.Instruction) Container.DataItem).Point.Description %></td>
											<td valign="top"><asp:Label ID="lblBookedDateTime" Runat="server"></asp:Label></td>
											<td valign="top" align="right"><%# ((Orchestrator.Entities.Instruction) Container.DataItem).TotalPallets.ToString() %></td>
											<td valign="top"><asp:Button ID="btnDown" Runat="server" CausesValidation="False" CommandName="Down" Text=" &#9660; " Height="24px"></asp:Button></td>
											<td valign="top"><asp:Button ID="btnUp" Runat="server" CausesValidation="False" CommandName="Up" Text=" &#9650; " Height="24px"></asp:Button></td>
											<td valign="top"><asp:Button ID="btnAlter" Runat="server" CausesValidation="False" CommandName="Alter" Text="&#8230;" Height="24px"></asp:Button></td>
											<td valign="top"><asp:Button ID="btnDelete" Runat="server" CausesValidation="False" CommandName="Delete" Text=" x " Height="24px"></asp:Button></td>
										</tr>
									</ItemTemplate>
									<SeparatorTemplate>
										<tr>
											<td colspan="10"><hr noshade></td>
										</tr>
									</SeparatorTemplate>
									<FooterTemplate>
										</table>
									</FooterTemplate>
								</asp:Repeater>
							</td>
						</tr>
						<tr>
							<td colspan="2" align="right">
							    <div class="buttonbar">
								    <asp:Button id="btnAddNewDehire" runat="server" Text="Add New Dehire" CausesValidation="False"></asp:Button>
								</div>
							</td>
						</tr>
					</table>
				</fieldset>
			</td>
		</tr>
		<tr>
			<td colspan="2">
				<div class="buttonbar">
					<asp:Button id="btnViewDeHiringForm" runat="server" Text="Pallet De-Hiring Form"></asp:Button><asp:Button id="btnAddJob" runat="server" Text="Add Job"></asp:Button>
				</div>
			</td>
		</tr>
	</table>
	
	<div style="height:10px;"></div>
	
	<uc1:reportViewer id="reportViewer1" runat="server" visible="false"></uc1:reportViewer>
	
    <telerik:RadAjaxManager ID="ramPalletHandlingRun" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="cboCollectPoint">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="cboPalletType" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="cboPalletType">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="lblPalletsAvailable" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    
    <script language="javascript">
    
        function CollectClientSelectedIndexChanged(item)
        {
            var collectPointCombo = $find("<%=cboCollectPoint.ClientID %>");
            collectPointCombo.set_text("");
            collectPointCombo.requestItems(item.get_value(),false);
        }

        function CollectPointRequesting(sender, eventArgs)
        {
            var collectClientCombo = $find("<%=cboCollectClient.ClientID %>");

            var context = eventArgs.get_context();
            context["FilterString"] = collectClientCombo.get_value() + ";" + sender.get_text();
        }   
        
        function DehireLocationOwnerClientSelectedIndexChanged(item)
        {
            var dehirePointCombo = $find("<%=cboDehirePoint.ClientID %>");
            dehirePointCombo.set_text("");
            dehirePointCombo.requestItems(item.get_value(),false);
        }

        function DehirePointRequesting(sender, eventArgs)
        {
            var DehireClientCombo = $find("<%=cboDehireClientLocationOwner.ClientID %>");

            var context = eventArgs.get_context();
            context["FilterString"] = DehireClientCombo.get_value() + ";" + sender.get_text();
        }   
    </script>
</asp:Content>