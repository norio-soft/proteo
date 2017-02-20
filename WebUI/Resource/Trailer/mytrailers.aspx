<%@ Page language="c#" Inherits="Orchestrator.WebUI.Resource.Trailer.MyTrailers" Codebehind="MyTrailers.aspx.cs" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<uc1:header id="Header1" runat="server" Title="My Trailer List" SubTitle="The status of your trailers is shown below." XMLPath="TrailerContextMenu.xml"></uc1:header>

<form id="Form1" method="post" runat="server">
	<div align="center">
		<table width="99%">
			<tr>
				<td></td>
			</tr>
		</table>
	</div>
</form>

<uc1:footer id="Footer1" runat="server"></uc1:footer>