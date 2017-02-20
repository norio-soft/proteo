<%@ Page language="c#" Inherits="Orchestrator.WebUI.security.keepsessionalive" Codebehind="keepsessionalive.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" > 

<html>
	<head>
		<title>Keep Session Alive</title>
		<meta http-equiv="refresh" content="300;url=<%=Page.ResolveUrl("~/security/keepsessionalive.aspx")%>">
	</head>
	
	<body>
		KEEPING ALIVE <%=DateTime.UtcNow.ToString("dd/MM/yy HH:mm:ss")%>
	</body>
</html>
