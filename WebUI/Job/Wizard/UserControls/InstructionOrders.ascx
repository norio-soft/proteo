<%@ Reference Page="~/job/wizard/wizard.aspx" %>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Job.Wizard.UserControls.InstructionOrders" Codebehind="InstructionOrders.ascx.cs" %>
<%@ Register TagPrefix="uc" TagName="Point" Src="~/UserControls/point.ascx" %>
<%@ Register TagPrefix="uc" TagName="InstructionOrders" Src="~/UserControls/InstructionOrders.ascx" %>
<%@ Register TagPrefix="uc" TagName="Orders" Src="~/UserControls/orders.ascx" %>


<script language="javascript" type="text/javascript">
<!--
    width = 950;
    height = 750;
//-->
</script>

<table width="100%" height="472" cellpadding="0" cellspacing="0" border="0" id="tblMain">
	<tr height="56">
		<td bgcolor="white" style="PADDING-RIGHT:3px">
			<table width="100%" cellpadding="0" cellspacing="0" align="right">
				<tr>
					<td width="100%" valign="top" align="left">
						<div style="PADDING-LEFT:20px; FONT-WEIGHT:bold; PADDING-TOP:5px"><STRONG>
							<asp:Label id=lblCollectDrop runat="server" Font-Size="Medium"></asp:Label></STRONG>
						</div>
			            <div style="PADDING-LEFT: 20px; FONT-WEIGHT: bold; PADDING-TOP: 5px">Configure Orders for Collection</div>
						<div style="PADDING-LEFT:35px;PADDING-TOP:2px">
							Specify which orders you wish to collect.
						</div>
					</td>
					<td align="right">
						<img src="../../images/p1logo.gif" width="50" height="50">
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr height="1" bgcolor="#aca899">
		<td colspan="2" height="1" ></td>
	</tr>

	<tr height="305" bgcolor="#ece9d8">
		<td style="PADDING-RIGHT:10px;PADDING-LEFT:5px;VERTICAL-ALIGN:top;PADDING-TOP:10px" width="100%">
			<div>
                <fieldset style="padding:0px;margin-top:5px; margin-bottom:5px;">
                    <div style="height:22px; border-bottom:solid 1pt silver;padding:2px;margin-bottom:5px; color:#ffffff; background-color:#5d7b9d;">Collection Details</div>
                    <table width="100%" border="0" cellpadding="2" cellspacing="0">
				        <tr>
				            <td colspan="2">
				                <uc:Point id="ucPoint" runat="server" CanCreateNewPoint="False" />
				            </td>
				        </tr>
				        <tr>
						    <td align="left" colspan="2">
							    <table width="500" cellpadding="0" cellspacing="0" border="0">
								    <tr>
						                <td nowrap="nowrap"><span>Booked Date and Time</span>&nbsp;</td>
									    <td nowrap="nowrap" width="110"><telerik:RadDateInput id="dteBookedDate" runat="server" dateformat="dd/MM/yy" ToolTip="The Booked Date"></telerik:RadDateInput><asp:CustomValidator ID="cfvValidDateRange" Runat="server" ControlToValidate="dteBookedDate" Display="Dynamic" EnableClientScript="False" ErrorMessage="The Date must be no more than 1 day before today and no more than 3 days ahead of today."><img src="../../images/error.png" Title="The Date must be no more than 1 day before today and no more than 3 days ahead of today."></asp:CustomValidator></td>
									    <td width="110"><telerik:RadDateInput id="dteBookedTime" runat="server" dateformat="t" DataMode="EditModeText" NullText="AnyTime" Nullable="True"></telerik:RadDateInput></td>
									    <td width="280"><asp:requiredfieldvalidator id="rfvBookedDate" runat="server" ControlToValidate="dteBookedDate" Display="Dynamic" ErrorMessage="Please enter a Booked Date and Time."><img src="../../images/error.png" Title="Please enter a Booked Date and Time."></asp:requiredfieldvalidator></td>
								    </tr>
							    </table>
						    </td>
				        </tr>
					    <tr>
					        <td colspan="2">
					            <br />
                                <div style="height:22px; border-bottom:solid 1pt silver;padding:2px;color:#ffffff; background-color:#99BEDE;text-align:right;">
                                    <asp:Button ID="btnViewAvailableOrders" runat="server" Text="Refresh Orders" CausesValidation="true" />
                                </div>
                            </td>
					    </tr>
				    </table>
				</fieldset>
                <fieldset style="padding:0px;margin-top:5px; margin-bottom:5px;">
                    <div style="height:22px; border-bottom:solid 1pt silver;padding:2px;margin-bottom:5px; color:#ffffff; background-color:#5d7b9d;">This area shows the orders you have selected to collect from this location.  If you wish to remove an order from the collection pool, untick it.</div>
                    <uc:InstructionOrders ID="ucInstructionOrders" runat="server" ShowCollectionColumns="false"/>
			        <asp:CustomValidator ID="cfvOkayToRemoveAllOrders" Runat="server" ControlToValidate="dteBookedDate" Display="Dynamic" EnableClientScript="False" ErrorMessage="You can not remove all the orders from this collection as it is the only collection on this job."><img src="../../images/error.png" Title="You can not remove all the orders from this collection as it is the only collection on this job."></asp:CustomValidator>
				</fieldset>
                <fieldset style="padding:0px;margin-top:5px; margin-bottom:5px;">
                    <div style="height:22px; border-bottom:solid 1pt silver;padding:2px;margin-bottom:5px; color:#ffffff; background-color:#5d7b9d;">This area shows the additional orders you could collect from this location. If you wish to collect additional orders, tick them.</div>
	                <uc:Orders ID="ucOrders" runat="server" AllowFilteringByColumn="False" ShowFilterOptions="False" ShowClearFiltersButton="False" ShowSelectOrdersButton="False" ShowAddOrderButton="false"  />
				</fieldset>
            </div>
		</td>
	</tr>

	<tr height="1" bgcolor="#aca899">
		<td colspan="2" height="1" ></td>
	</tr>
	<tr height="46" bgcolor="#ece9d8">
		<td colspan="2" align="right" height="46" style="PADDING-RIGHT:10px">
			<asp:Button ID="btnBack" Runat="server" CausesValidation="False" Text="< Back"></asp:Button>
			<asp:button id="btnNext" runat="server" CausesValidation="True" Text="Next >"></asp:button>
			&nbsp;&nbsp;
			<asp:Button ID="btnCancel" Runat="server" CausesValidation="False" Text="Cancel"></asp:Button>
		</td>
	</tr>
</table>