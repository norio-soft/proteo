<%@ Reference Page="~/job/wizard/wizard.aspx" %>
<%@ Control Language="c#" Inherits="Orchestrator.WebUI.Job.Wizard.UserControls.JobType" Codebehind="JobType.ascx.cs" %>
<script language="javascript" type="text/javascript">
<!--
	function checkKeyPress()
	{
		if (window.event && window.event.keyCode == 13)
			document.getElementById('<%= btnNext.ClientID %>').click();
		else
			return true;
	}
//-->
</script>
<table width="100%" height="472" cellpadding="0" cellspacing="0" border="0">
	<tr height="56">
		<td bgcolor="white" align="right" style="PADDING-RIGHT:10px" >
			<table width="100%" cellpadding="0" cellspacing="0" align="right">
				<tr>
					<td width="100%" valign="top" align="left">
						<div style="PADDING-LEFT:20px; FONT-WEIGHT:bold; PADDING-TOP:5px">
							Job Charging
						</div>
						<div style="PADDING-LEFT:35px;PADDING-TOP:2px">
							Please provide the job charge type and job charge amount.
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
				<table width="75%" border="0" cellpadding="0" cellspacing="0">
					<tr width="50">
						<td >
							<span>Job Type</span>
						</td>
						<td></td>
						<td>
							<span>Normal</span>
						</td>
					</tr>
					<tr>
						<td colspan="2">
							<div class="spacer" style="height:10px;"></div>
						</td>
					</tr>
					<tr width="50">
						<td>
							<span>Job Charge Type</span>
						</td>
						<td></td>
						<td>
							<asp:DropDownList ID="cboChargeType" Runat="server" AutoPostBack="True"></asp:DropDownList>
						</td>
					</tr>
					<tr width="50">
						<td >
							<span>Charge Amount</span>
						</td>
						<td align=right>£</td>
						<td  >
							<asp:TextBox ID="txtChargeAmount" Runat="server" Enabled="False"></asp:TextBox>
							<asp:RequiredFieldValidator ID="rfvChargeAmount" Runat="server" Display="Dynamic" ControlToValidate="txtChargeAmount" ErrorMessage="Please specify the charge amount."><img src="../../images/error.png"  Title="Please specify the charge amount."></asp:RequiredFieldValidator>
							<asp:RegularExpressionValidator ID="revChargeAmount" Runat="server" Display="Dynamic" ControlToValidate="txtChargeAmount" ValidationExpression="^(£|-£|£-|-)?([0-9]{1}[0-9]{0,2}(\,[0-9]{3})*(\.[0-9]{0,2})?|[1-9]{1}[0-9]{0,}(\.[0-9]{0,2})?|0(\.[0-9]{0,2})?|(\.[0-9]{1,2})?)$" ErrorMessage="Please enter a valid currency value for the charge amount."><img src="../../images/error.png"  Title="Please enter a valid currency value for the charge amount."></asp:RegularExpressionValidator>
						</td>
					</tr>
					<tr>
						<td colspan="3">
							<asp:CheckBox ID="chkIsStockMovement" Runat="server" Text="Is this job a stock movement job?" TextAlign="Left"></asp:CheckBox>
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
			<asp:Button id="btnNext" runat="server" CausesValidation="True" Text="Next >"></asp:Button>
			&nbsp;&nbsp;
			<asp:Button ID="btnCancel" Runat="server" CausesValidation="False" Text="Cancel"></asp:Button>
		</td>
	</tr>
</table>

<script language="javascript" type="text/javascript" defer>
<!--
	document.getElementById('<%=txtChargeAmount.ClientID%>').focus();
//-->
</script>