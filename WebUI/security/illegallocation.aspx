<%@ Page language="c#" Inherits="Orchestrator.WebUI.security.illegallocation" Codebehind="illegallocation.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" > 

<html>
  <head runat="server">
    <title>Illegal Location</title>
    <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" Content="C#">
    <meta name=vs_defaultClientScript content="JavaScript">
    <meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">
    <LINK href='<%=Page.ResolveUrl("~/style/hms.css")%>' type="text/css" rel="stylesheet">
  </head>
  <body class="loginBody" style="PADDING-TOP:10px;TEXT-ALIGN:center">
    <form id="Form1" method="post" runat="server">
	    <div id="loginContainer">
		    <div class="loginLogo">&nbsp;</div>
		    <div class="loginBoxOuter">
		        <div class="loginBoxTop">&nbsp;</div>
		        <div class="loginBoxMiddle">
		            <div class="loginOrganisationNameContainer">
	                        <span id="lblIllegalLocation" style="font-size: 18px; color:#CCCFD1">Illegal Location</span>
                            </div>
		            <asp:Label ID="lblOrganisationName" runat="server"><%=Orchestrator.Globals.Configuration.InstallationCompanyName %></asp:Label>
                    <div class="loginCredentialsContainer">
				        <p>You cannot log in from your present location.</p>
			            <p>Please access the system from one of the pre-approved locations.</p>
		            </div>
		            <div class="loginCredentialsContainer">
		                <p>This is a private computer system. Unauthorised access to this system is an offence under the Computer Misuse Act.</p>
						<p>Unauthorised disclosure of information on this system is an offence under the Data Protection Act.</p>
					</div>
					<div class="loginCredentialsContainer">
					    <p>If you feel that you have reached this page in error, please contact a System Administrator and request you be added to the correct user group.</p>
					    <p>Alternatively, call the helpdesk on 0845 644 3720.</p>
					</div>
			    </div>
		        <div class="loginBoxBottom">&nbsp;</div>
		        <div class="loginFooter">Application Provided and supported by <a href="http://www.proteo.co.uk/">Proteo</a></div>
		    </div>
		</div>
     </form>
  </body>
</html>

