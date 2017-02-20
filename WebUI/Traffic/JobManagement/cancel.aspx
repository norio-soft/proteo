<%@ Page language="c#" Inherits="Orchestrator.WebUI.Traffic.JobManagement.cancel" Codebehind="cancel.aspx.cs" %>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>
<uc1:header id="Header1" title="Job Cancellation" SubTitle="Use this page to cancel a job." runat="server"></uc1:header>

	<form id="Form1" method="post" runat="server">
		<asp:Label id="lblConfirmation" runat="server" cssclass="confirmation" visible="false"></asp:Label>
		<br>
		<asp:checkbox id="chkMarkforCancellation" runat="server" Width="160px" TextAlign="Left" Text="Mark for Cancellation"></asp:checkbox>
		<br>
		Reason for Cancellation
		<br>
		<asp:textbox id="txtCancellationReason" runat="server" Width="336px" TextMode="MultiLine" Height="64px"></asp:textbox>
		<asp:RequiredFieldValidator id="rfvCancellationReason" runat="server" ControlToValidate="txtCancellationReason" ErrorMessage="Please enter a reason for cancelling."><img src="../../images/error.png"  Title="Please enter a reason for cancelling."></asp:RequiredFieldValidator>
		
		<asp:Button id="btnCancelJob" runat="server" Text="Cancel Job"></asp:Button>
	</form>

<uc1:footer id="Footer1" runat="server"></uc1:footer>
