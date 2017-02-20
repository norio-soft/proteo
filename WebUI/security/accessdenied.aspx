<%@ Page language="c#" Inherits="Orchestrator.WebUI.Security.accessdenied" Codebehind="accessdenied.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" > 

<html>
  <head runat="server">
    <title>Access Denied</title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">
    <LINK href="/style/hms.css" type="text/css" rel="stylesheet">
  </head>
  <body>
	
    <form id="Form1" method="post" runat="server">
		<table width="450" align="center" style="BORDER-RIGHT: #000000 1px solid; PADDING-RIGHT: 5px; BORDER-TOP: #000000 1px solid; PADDING-LEFT: 5px; PADDING-BOTTOM: 5px; BORDER-LEFT: #000000 1px solid; PADDING-TOP: 5px; BORDER-BOTTOM: #000000 1px solid; BACKGROUND-COLOR: white">
			<tr>
				<td align="right">
					<img id="Image1" src='<%=Page.ResolveUrl("~/images/logon_banner.gif")%>' border="0">
				</td>
			</tr>
			<tr>
				<td align="center">	
					<br>
					<span style="FONT-WEIGHT:bold">Access denied.</span>
					<br>
					<br>
					User <b><asp:Label id="lblUser" runat="server"/></b> cannot access the requested functionality.
					<br>
					<br>
					Only users from the <asp:Label id="lblRole" runat="server"/> user groups can access this functionality.
					<br>
					<br>
					To logon as another user, <a href="login.aspx?LO=1">click here.</a>
				</td>
			</tr>
			<tr>
				<td align="center">
						<hr>
					<span style="FONT-SIZE:10px">This is a private computer system.
						<br>
						<br>
						Unauthorised access to this system is an offence under the Computer Misuse Act.
						<br>
						<br>
						Unauthorised disclosure of information on this system is an offence under the 
						Data Protection Act. </span>
						<hr>
						If you feel that you have reached this page in error, please contact a
						System Administrator and request you be added to the correct user group.
						<br>
						Alternatively, call the helpdesk on 0845 644 3720.
				</td>
			</tr>
			<tr>
				<td style="FONT-SIZE:10px;FONT-FAMILY:tahoma;TEXT-ALIGN:center">
					<hr>
					<img id="Image2" src="../images/p1Logo.gif" height="16" width="16" alt="P1TP" border="0"><br>
					Application Provided by and supported by P1 technology Partners Ltd
				</td>
			</tr>
		</table>
     </form>
	
  </body>
</html>
