<%@ Reference Page="~/job/wizard/wizard.aspx" %>
<%@ Control Language="c#" Inherits="Orchestrator.WebUI.Job.Wizard.UserControls.ClientsCustomer" Codebehind="ClientsCustomer.ascx.cs" %>
<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>
<table width="100%" height="472" cellpadding="0" cellspacing="0" border="0" id="tblMain">
	<tr height="56">
		<td bgcolor="white" align="right" style="PADDING-RIGHT:10px" >
			<table width="100%" cellpadding="0" cellspacing="0" align="right">
				<tr>
					<td width="100%" valign="top" align="left">
						<div style="PADDING-LEFT:20px; FONT-WEIGHT:bold; PADDING-TOP:5px"><STRONG>
							<asp:Label id=lblCollectDrop runat="server" Font-Size="Medium" Text="Delivery Details"></asp:Label></STRONG>
						</div>
						<div style="PADDING-LEFT:20px; FONT-WEIGHT:bold; PADDING-TOP:5px">
							Specify Delivery Customer
						</div>
						<div style="PADDING-LEFT:35px;PADDING-TOP:2px">
							Specify the Customer that will receive this Delivery.
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
						<td width="150">
							<span>Client's Customer Name</span>
						</td>
						<td>
                            <telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                                MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" Height="300px" Overlay="true"
                                ShowMoreResultsBox="false" Skin="WindowsXP" Width="355px" AllowCustomText="True">
                            </telerik:RadComboBox>
							
							<asp:RequiredFieldValidator id="rfvClient" runat="server" ControlToValidate="cboClient" ErrorMessage="Please select the client's customer this delivery is for."><img src="../../images/error.png"  Title="Please select the client's customer this delivery is for."></asp:RequiredFieldValidator>
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
									<legend>Create a new Client's Customer</legend>
									<asp:CheckBox ID="chkCreateClient" Runat="server" Checked="False" AutoPostBack="True" Text="The client you've entered does not exist, would you like to create it?" TextAlign="Left"></asp:CheckBox>
									<uc1:infringementDisplay id="infringementDisplay" runat="server" Visible="false"></uc1:infringementDisplay>
								</fieldset>
							</asp:Panel>
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