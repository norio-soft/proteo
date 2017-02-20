<%@ Control Language="c#" Inherits="Orchestrator.WebUI.UserControls.headerModal" Codebehind="headerModal.ascx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Orchestrator</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<meta http-equiv="Refresh" content="<%=Orchestrator.Globals.Configuration.SessionTimeout%>; url=<%=Page.ResolveUrl("~/security/login.aspx")%>?t=1">
		<base target="_self">
		<LINK href="<%=Page.ResolveUrl("~/style/Styles.css")%>" type="text/css" rel="stylesheet">
	</HEAD>
	<body bgColor="#cccccc">
	  		<table id="ControlPanelLayout" style="BORDER-COLLAPSE: collapse" borderColor="#336699"
				cellSpacing="0" cellPadding="0" width="400" align="center" bgColor="#ffffff" border="1">
				<tr>
					<td>
						<table cellSpacing="0" cellPadding="0" width="100%" border="0">
							<tr bgColor="#01314a">
								<td><IMG src="<%=Page.ResolveUrl("~/images/dev/LogoBannerHeaderModal.gif")%>" border="0" width="380" height="49"></td>
								<td style="BACKGROUND-COLOR:white" width="100%">&nbsp;</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td vAlign="top">
						<table cellSpacing="0" cellPadding="0" width="100%">
							<tr>
								<td id="ContentPane" vAlign="top" >
									<table cellSpacing="0" cellPadding="0" width="100%">
										<tr>
											<td vAlign="top" align="left" style="padding-left:2px; padding-right:2px;">
												<!-- Begin Content -->