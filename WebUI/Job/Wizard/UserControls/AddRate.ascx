<%@ Reference Page="~/job/wizard/wizard.aspx" %>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Job.Wizard.UserControls.AddRate" Codebehind="AddRate.ascx.cs" %>

<table width="100%" height="452" cellpadding="0" cellspacing="0" border="0" id="tblMain">
	<tr height="56">
		<td bgcolor="white" align="right" style="PADDING-RIGHT:10px">
			<table width="100%" cellpadding="0" cellspacing="0" align="right">
				<tr>
					<td width="100%" valign="top" align="left">
						<div style="PADDING-LEFT:20px; FONT-WEIGHT:bold; PADDING-TOP:5px">
							Adding a New Rate
						</div>
						<div style="PADDING-LEFT:35px;PADDING-TOP:2px">
							Add rate for jobs collecting from 
							<%=m_collectionPoint%>
							<br />and delivering to 
							<%=m_deliveryPoint%>.
						</div>
					</td>
					<td align="right">
						<img src="../../images/p1logo.gif" width="50" height="50">
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr height="2" bgcolor="#aca899">
		<td colspan="2" height="3" style="BORDER-TOP:#aca899 1pt solid; BORDER-BOTTOM:white 1pt solid"></td>
	</tr>
	<tr height="305" bgcolor="#ece9d8">
		<td style="PADDING-RIGHT:10px;PADDING-LEFT:35px;VERTICAL-ALIGN:top;PADDING-TOP:10px"
			width="100%">
			<div>
				<table width="300" border="0" cellpadding="0" cellspacing="0">
				    <tr>
				        <td width="100">Full Load Rate</td>
						<td align=right width="20">£</td>
						<td>
							<asp:TextBox ID="txtFullLoadRate" Runat="server" Width="70px"></asp:TextBox>
							<asp:RequiredFieldValidator ID="rfvFullLoadRate" Runat="server" Display="Dynamic" ControlToValidate="txtFullLoadRate" ErrorMessage="Please specify the full load rate."><img src="../../images/error.png"  Title="Please specify the full load rate."></asp:RequiredFieldValidator>
							<asp:RegularExpressionValidator ID="revFullLoadRate" Runat="server" Display="Dynamic" ControlToValidate="txtFullLoadRate" ValidationExpression="^(£|-£|£-|-)?([0-9]{1}[0-9]{0,2}(\,[0-9]{3})*(\.[0-9]{0,2})?|[1-9]{1}[0-9]{0,}(\.[0-9]{0,2})?|0(\.[0-9]{0,2})?|(\.[0-9]{1,2})?)$" ErrorMessage="Please enter a valid currency value for the full load rate."><img src="../../images/error.png"  Title="Please enter a valid currency value for the full load rate."></asp:RegularExpressionValidator>
						</td>
				    </tr>
				    <tr>
				        <td>Part Load Rate</td>
						<td align=right>£</td>
						<td>
							<asp:TextBox ID="txtPartLoadRate" Runat="server" Width="70px"></asp:TextBox>
							<asp:RequiredFieldValidator ID="rfvPartLoadRate" Runat="server" Display="Dynamic" ControlToValidate="txtPartLoadRate" ErrorMessage="Please specify the part load rate."><img src="../../images/error.png"  Title="Please specify the part load rate."></asp:RequiredFieldValidator>
							<asp:RegularExpressionValidator ID="revPartLoadRate" Runat="server" Display="Dynamic" ControlToValidate="txtPartLoadRate" ValidationExpression="^(£|-£|£-|-)?([0-9]{1}[0-9]{0,2}(\,[0-9]{3})*(\.[0-9]{0,2})?|[1-9]{1}[0-9]{0,}(\.[0-9]{0,2})?|0(\.[0-9]{0,2})?|(\.[0-9]{1,2})?)$" ErrorMessage="Please enter a valid currency value for the part load rate."><img src="../../images/error.png"  Title="Please enter a valid currency value for the part load rate."></asp:RegularExpressionValidator>
						</td>
				    </tr>
				    <tr>
				        <td>Multi Drop Rate</td>
						<td align=right>£</td>
						<td >
							<asp:TextBox ID="txtMultiDropRate" Runat="server" Width="70px"></asp:TextBox>
							<asp:RequiredFieldValidator ID="rfvMultiDropRate" Runat="server" Display="Dynamic" ControlToValidate="txtMultiDropRate" ErrorMessage="Please specify the multi-drop rate."><img src="../../images/error.png"  Title="Please specify the multi-drop rate."></asp:RequiredFieldValidator>
							<asp:RegularExpressionValidator ID="revMultiDropRate" Runat="server" Display="Dynamic" ControlToValidate="txtMultiDropRate" ValidationExpression="^(£|-£|£-|-)?([0-9]{1}[0-9]{0,2}(\,[0-9]{3})*(\.[0-9]{0,2})?|[1-9]{1}[0-9]{0,}(\.[0-9]{0,2})?|0(\.[0-9]{0,2})?|(\.[0-9]{1,2})?)$" ErrorMessage="Please enter a valid currency value for the multi-drop rate."><img src="../../images/error.png"  Title="Please enter a valid currency value for the multi-drop rate."></asp:RegularExpressionValidator>
						</td>
				    </tr>
				    <tr>
				        <td colspan="2">End Date</td>
				        <td><telerik:RadDateInput id="dteEndDate" runat="server" dateformat="dd/MM/yy" Nullable="true" NullText="no end" ToolTip="The date the rate should stop being applied" Width="60px"></telerik:RadDateInput></td>
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
<script language="javascript" type="text/javascript">
<!--
	function HidePage()
	{
		if (typeof(Page_ClientValidate) == 'function')
		{
			if (Page_ClientValidate())
			{
	            document.getElementById("tblMain").style.display = "none";
		        document.getElementById("tblRotate").style.display = "";
			}
		}
		else
		{
	        document.getElementById("tblMain").style.display = "none";
		    document.getElementById("tblRotate").style.display = "";
		}
	}
//-->
</script>
<table width="100%" height="452" cellpadding="0" cellspacing="0" border="0" id="tblRotate" style="display: none">
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