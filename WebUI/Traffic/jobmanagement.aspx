<%@ Page language="c#" Inherits="Orchestrator.WebUI.Traffic.jobManagement" Codebehind="jobManagement.aspx.cs" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<uc1:header id="Header1" title="Job Management" ShowLeftMenu="False" SubTitle="Manage your Job." runat="server"></uc1:header>

<form id="frmJobManagement" method="post" runat="server">
	<br>
	<asp:Label id="lblMessage" runat="server" visible="False" cssclass="confirmation" Text="There was a problem loading the requested job " EnableViewState="False"></asp:Label>
	<br>
	<asp:Button id="btnReturnToTrafficSheet" runat="server" Text="Return to Traffic Sheet"></asp:Button>
</form>

<uc1:footer id="Footer1" runat="server"></uc1:footer>
