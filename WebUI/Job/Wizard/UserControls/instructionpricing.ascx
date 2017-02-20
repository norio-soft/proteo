<%@ Reference Page="~/job/wizard/wizard.aspx" %>
<%@ Control Language="c#" Inherits="Orchestrator.WebUI.Job.Wizard.UserControls.InstructionPricing" Codebehind="InstructionPricing.ascx.cs" %>
<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>
<table width="100%" height="472" cellpadding="0" cellspacing="0" border="0" id="tblMain">
	<tr height="56">
		<td bgcolor="white" align="right" style="PADDING-RIGHT:10px" >
			<table width="100%" cellpadding="0" cellspacing="0" align="right">
				<tr>
					<td width="100%" valign="top" align="left">
						<div style="PADDING-LEFT:20px; FONT-WEIGHT:bold; PADDING-TOP:5px">
							Rate Analysis
						</div>
						<div style="PADDING-LEFT:35px;PADDING-TOP:2px">
							Please ensure that all parts of the jobs have a valid rate against them.
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
					<tr id="trRateAnalysis" runat="server">
						<td>
			                <asp:Label ID="lblRateAnalysis" runat="server"></asp:Label><img id="imgRatesRequired" runat="server" src="../../../images/error.gif" alt="Please configure the rates, or check the checkbox below." />
			                <br />
			                <asp:Repeater ID="repRates" runat="server">
			                    <HeaderTemplate>
									<table width="100%" border="1" cellpadding="1" cellspacing="0" bordercolor="#FFFFFF">
										<thead bgcolor="#ECE9D8">
											<tr>
												<th valign="top">Collection Point</th>
												<th valign="top">Delivery Point</th>
												<th valign="top">&nbsp;</th>
											</tr>
										</thead>
										<tbody bgcolor="#FFFFFF">
			                    </HeaderTemplate>
			                    <ItemTemplate>
			                                <tr>
			                                    <td valign="top">
			                                        <span id="spnCollectionPoint" runat="server"><%# DataBinder.Eval(Container.DataItem, "CollectionPoint") %></span>
			                                        <input type="hidden" runat="server" id="hidCollectionPointId" runat="server" value='<%# DataBinder.Eval(Container.DataItem, "CollectionPointId") %>' />
			                                        <input type="hidden" runat="server" id="hidDeliveryPointId" runat="server" value='<%# DataBinder.Eval(Container.DataItem, "DeliveryPointId") %>' />
			                                    </td>
			                                    <td valign="top">
			                                        <span id="spnDeliveryPoint" runat="server"><%# DataBinder.Eval(Container.DataItem, "DeliveryPoint") %></span>
			                                    </td>
			                                    <td valign="top" align="center" width="80">
				                                    <asp:Button ID="btnSetRate" runat="server" CommandName="setRate" CausesValidation="False" Text="Set Rate" Width="75px" Height="24px" />
			                                    </td>
			                                </tr>
			                    </ItemTemplate>
			                    <FooterTemplate>
			                            </tbody>
                                    </table>
			                    </FooterTemplate>
			                </asp:Repeater>
			                <asp:CheckBox ID="chkManualRateEntry" runat="server" Text="Ignore rate requirements" TextAlign="Right" />
						</td>
					</tr>
				</table>
			</div>
		</td>
	</tr>
	<tr height="2" bgcolor="#aca899">
		<td colspan="2" height="3" style="BORDER-TOP:#aca899 1pt solid; BORDER-BOTTOM:white 1pt solid"></td>
	</tr>
	<tr height="46" bgcolor="#ece9d8">
		<td colspan="2" align="right" height="46" style="PADDING-RIGHT:10px">
			<asp:Button ID="btnBack" Runat="server" CausesValidation="False" Text="< Back"></asp:Button>
			<asp:button id="btnNext" runat="server" CausesValidation="True" Text="Next >"></asp:Button>
			&nbsp;&nbsp;
			<asp:Button ID="btnCancel" Runat="server" CausesValidation="False" Text="Cancel"></asp:Button>
		</td>
	</tr>
</table>
<script language="javascript" type="text/javascript">
<!--
	function HidePage()
	{
		document.getElementById("tblMain").style.display = "none";
		document.getElementById("tblRotate").style.display = "block";
	}
//-->
</script>
<table width="596" height="472" cellpadding="0" cellspacing="0" border="0" id="tblRotate" style="display: none">
	<tr height="56">
		<td bgcolor="white" align="right" style="PADDING-RIGHT:10px" >
			<table width="100%" cellpadding="0" cellspacing="0" align="right">
				<tr>
					<td width="100%" valign="top">
						<div style="PADDING-LEFT:20px; FONT-WEIGHT:bold; PADDING-TOP:5px">
							Processing
						</div>
						<div style="PADDING-LEFT:35px;PADDING-TOP:2px">
							Your actions are being processed, please wait.
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
		<td style="PADDING-RIGHT:10px;PADDING-LEFT:35px;VERTICAL-ALIGN:top; padding-top:10px;" width="100%"></td>
	</tr>
	<tr height="2" bgcolor="#aca899">
		<td colspan="2" height="3" style="BORDER-TOP:#aca899 1pt solid; BORDER-BOTTOM:white 1pt solid"></td>
	</tr>
	<tr height="46" bgcolor="#ece9d8">
		<td colspan="2" align="right" height="46" style="PADDING-RIGHT:10px"></td>
	</tr>
</table>