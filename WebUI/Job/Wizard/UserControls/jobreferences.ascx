<%@ Reference Page="~/job/wizard/wizard.aspx" %>
<%@ Control Language="c#" Inherits="Orchestrator.WebUI.Job.Wizard.UserControls.JobReferences" Codebehind="JobReferences.ascx.cs" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>
<table width="100%" height="472" cellpadding="0" cellspacing="0" border="0">
	<tr height="56">
		<td bgcolor="white" align="right" style="PADDING-RIGHT:10px" >
			<table width="100%" cellpadding="0" cellspacing="0" align="right">
				<tr>
					<td width="100%" valign="top" align="left">
						<div style="PADDING-LEFT:20px; FONT-WEIGHT:bold; PADDING-TOP:5px">
							Job References
						</div>
						<div style="PADDING-LEFT:35px;PADDING-TOP:2px">
							Enter the job references you want to be associated with this job
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
						<td width="150" valign="top">
							<asp:Label ID="lblLoadNumber" Runat="server" Text="Load Number"></asp:Label>
						</td>
						<td valign="top">
							<asp:TextBox id="txtLoadNumber" Runat="server"></asp:TextBox>
							<asp:RequiredFieldValidator ID="rfvLoadNumber" Runat="server" ControlToValidate="txtLoadNumber" EnableClientScript="False" Display="Dynamic" ErrorMessage="Please enter the Load Number, if this job has no load number press Next > again."><img src="../../images/error.png" Title="Please enter the Load Number, if this job has no load number press Next > again."></asp:RequiredFieldValidator>
							<asp:CustomValidator ID="cfvLoadNumber" Runat="server" ControlToValidate="txtLoadNumber" EnableClientScript="False" Display="Dynamic" ErrorMessage="This value must be unique for this client."><img src="../../images/error.png" Title="This value must be unique for this client."></asp:CustomValidator>
						</td>
					</tr>
					<asp:Repeater ID="repReferences" Runat="server">
						<ItemTemplate>
							<tr>
								<td width="150" valign="top">
									<span><%# DataBinder.Eval(Container.DataItem, "Description") %></span>
									<input type="hidden" id="hidOrganisationReferenceId" runat="server" value='<%# DataBinder.Eval(Container.DataItem, "OrganisationReferenceId") %>'>
								</td>
								<td valign="top">
									<asp:PlaceHolder ID="plcHolder" Runat="server">
										<asp:TextBox ID="txtReferenceValue" Runat="server"></asp:TextBox>
										<asp:RequiredFieldValidator id="rfvReferenceValue" runat="server" ControlToValidate="txtReferenceValue" EnableClientScript="False" Display="Dynamic" ErrorMessage='Please supply a <%# DataBinder.Eval(Container.DataItem, "Description") %>.'><img src="../../images/error.png"  Title='Please supply a <%# DataBinder.Eval(Container.DataItem, "Description") %>.'></asp:RequiredFieldValidator>
									</asp:PlaceHolder>
								</td>
							</tr>
						</ItemTemplate>
					</asp:Repeater>
					<tr>
						<td colspan="2">
							<div class="spacer" style="height:10px;"></div>
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
			<nfvc:NoFormValButton id="btnNext" runat="server" CausesValidation="True" OnClick="btnNext_Click" Text="Next >"></nfvc:NoFormValButton>
			&nbsp;&nbsp;
			<asp:Button ID="btnCancel" Runat="server" CausesValidation="False" Text="Cancel"></asp:Button>
		</td>
	</tr>
</table>

<script language="javascript" type="text/javascript" defer>
<!--
	document.getElementById('<%=txtLoadNumber.ClientID%>').focus();
//-->
</script>