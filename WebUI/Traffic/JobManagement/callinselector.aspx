<%@ Page language="c#" Inherits="Orchestrator.WebUI.Traffic.JobManagement.CallInSelector" Codebehind="CallInSelector.aspx.cs" %>
<%@ Register TagPrefix="uc1" TagName="callInSelector" Src="~/UserControls/CallInSelector.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" > 

<html>
	<head runat="server">
		<title></title>
		<script language="javascript" type="text/javascript" src="/script/Silverlight.js"></script>
	</head>
	<body style="background-color: whitesmoke;">
		<form id="Form1" method="post" runat="server">
			<uc1:callInSelector id="callInSelector1" runat="server"></uc1:callInSelector>
		</form>
	</body>
</html>
