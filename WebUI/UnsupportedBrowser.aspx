<%@ Page Language="C#" AutoEventWireup="true"  Inherits="Orchestrator.WebUI.UnsupportedBrowser" Codebehind="UnsupportedBrowser.aspx.cs" %>

<!doctype html>
<html lang="en">
    <head runat="server">
        <meta charset="utf-8" />
		
        <title><%= Orchestrator.Globals.Configuration.WebUITitle %> - Unsupported Browser</title>
		
		<link href="/style/newLogin.css" type="text/css" rel="stylesheet" />
		<link href="/style/hms.css" type="text/css" rel="stylesheet" />
    </head>

	<body class="loginBody" style="PADDING-TOP:10px;TEXT-ALIGN:center">
		<form id="Form1" method="post" runat="server">
			<div id="loginContainer">
			    <div class="loginLogo">&nbsp;</div>
			    <div class="loginBoxOuter">
			        <div class="loginBoxTop">&nbsp;</div>
			        <div class="loginBoxMiddle">
			            <div class="loginOrganisationNameContainer">
    			            <asp:Label ID="lblOrganisationName" runat="server" style="font-size: 18px; color:#CCCFD1"><%=Orchestrator.Globals.Configuration.InstallationCompanyName %></asp:Label>
                        </div>
                        <div class="loginInformationTextContainer">
                            <p class="loginInformationText">You are using an outdated browser which is not supported for use with <%= Orchestrator.Globals.Configuration.WebUITitle %>.</p>
                        </div>
                        <div class="loginInformationTextContainer">
                            <p class="loginInformationText">Please install the latest version of <a href="http://www.google.com/intl/en_uk/chrome/browser/">Google&nbsp;Chrome</a> or <a href="http://windows.microsoft.com/en-gb/internet-explorer/download-ie">Microsoft&nbsp;Internet&nbsp;Explorer</a>.</p>
                        </div>
                        <div class="loginInformationTextContainer">
                            <p class="loginInformationText">If you are using Internet Explorer 9 or later in Compatibility Mode please switch off Compatibility Mode for this site and retry.</p>
                            <p class="loginInformationText">Please note that Internet Explorer 9 will also no longer be supported following the next version upgrade of <%= Orchestrator.Globals.Configuration.WebUITitle %>.</p>
                        </div>
			        </div>
			        <div class="loginBoxBottom">&nbsp;</div>
			        <div class="loginFooter">Application Provided and supported by <a href="http://www.proteo.co.uk/">Proteo Ltd</a></div>
			    </div>
			</div>
		</form>
	</body>
</html>
