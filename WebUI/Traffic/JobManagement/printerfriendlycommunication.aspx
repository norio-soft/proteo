<%@ Page language="c#" MasterPageFile="~/WizardMasterPage.Master" Inherits="Orchestrator.WebUI.Traffic.JobManagement.printerFriendlyCommunication" Codebehind="printerFriendlyCommunication.aspx.cs" %>

<asp:Content ContentPlaceHolderID="HeaderContentFromChildPage" runat="server"></asp:Content>
<asp:Content ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Printer Friendly</asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<div align="center">
		<table width="99%">
			<tr>
				<td width="40%" align="left" valign="top" style="font-size: 14px; font-weight: bold">Communication To:</td>
				<td width="60%" align="left" valign="top" style="font-size: 14px; font-weight: bold"><asp:Label id="lblDriverFullName" runat="server"></asp:Label></td>
			</tr>
			<tr>
				<td align="left" valign="top" style="font-size: 14px; font-weight: bold">Status:</td>
				<td align="left" valign="top" style="font-size: 14px; font-weight: bold"><asp:Label id="lblCommunicationStatus" runat="server"></asp:Label></td>
			</tr>
			<tr>
				<td align="left" valign="top" style="font-size: 14px; font-weight: bold">Type:</td>
				<td align="left" valign="top" style="font-size: 14px; font-weight: bold"><asp:Label id="lblCommunicationType" runat="server"></asp:Label></td>
			</tr>
			<tr>
				<td align="left" valign="top" style="font-size: 14px; font-weight: bold">Comments:</td>
				<td align="left" valign="top" style="font-size: 14px; font-weight: bold"><asp:Label id="lblCommunicationComments" runat="server"></asp:Label></td>
			</tr>
			<tr>
				<td align="left" valign="top" style="font-size: 14px; font-weight: bold">Text:</td>
				<td align="left" valign="top" style="font-size: 14px; font-weight: bold"><asp:Label id="lblCommunicationText" runat="server"></asp:Label></td>
			</tr>
			<tr>
				<td align="left" valign="top" style="font-size: 14px; font-weight: bold">Number Used:</td>
				<td align="left" valign="top" style="font-size: 14px; font-weight: bold"><asp:Label id="lblCommunicationNumberUsed" runat="server"></asp:Label></td>
			</tr>
		</table>
		<div class="buttonBar">
		    <asp:Button ID="btnPrinterCommunication" runat="server" OnClientClick="javascript:if(!print()) return false;" Text="Print Communication" />
		    <asp:Button ID="btnClose" runat="server" OnClientClick="window.close();" Text="Close" Width="75" />
		</div>
	</div>
</asp:Content>