<%@ Page Language="C#" AutoEventWireup="true"  Inherits="Orchestrator.WebUI.Security.login" Codebehind="login.aspx.cs" %>

<!doctype html>
<html lang="en">
    <head runat="server">
        <meta charset="utf-8" />
		
        <title><%= Orchestrator.Globals.Configuration.WebUITitle %></title>
		
		<link href="/style/newLogin.css" type="text/css" rel="stylesheet" />
		<link href="/style/hms.css" type="text/css" rel="stylesheet" />

        <script src="//ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js"></script>
        <script>window.jQuery || document.write('<script src="/script/jquery-1.10.2.min.js">\x3C/script>')</script>
        <script src="/script/jquery-migrate-1.2.1.js"></script>
        <script src="/script/show-modal-dialog.js"></script>
      
    </head>

	<body class="loginBody" style="PADDING-TOP:10px;TEXT-ALIGN:center">
        <!--[if lt IE 9]><script type="text/javascript">location.href='/unsupportedbrowser.aspx';</script><![endif]-->

		<form id="Form1" method="post" runat="server">
			<asp:HiddenField ID="hidComputerName" runat="server" />
			<br />
			<br />
			<div id="loginContainer">
                <div class="loginCompanyLogo">&nbsp;</div>
			    <div class="loginLogo">&nbsp;</div>
			    <div class="loginBoxOuter">
			        <div class="loginBoxTop">&nbsp;</div>
			        <div class="loginBoxMiddle">
			            <div class="loginOrganisationNameContainer">
			            <asp:Label ID="lblOrganisationName" runat="server" style="font-size: 18px; color:#CCCFD1"><%=Orchestrator.Globals.Configuration.InstallationCompanyName %></asp:Label>
                        </div>
			            <div class="loginCredentialsContainer">
			            <!--<p>To complete the login, you must enter your username and password.</p> 
			            
			            <p>Please remember your password is case-sensitive.</p>-->
			            
			                <table class="loginCredentials">
			                    <tr>
			                        <td class="fieldLabel">Username</td>
			                        <td class="fieldInput"><asp:textbox id="txtUserName" Runat="server" autocomplete="false"></asp:textbox></td>
			                    </tr>
			                    <tr>
			                        <td class="fieldLabel">Password</td>
			                        <td class="fieldInput"><asp:textbox id="txtPIN" Runat="server" TextMode="Password" autocomplete="false"></asp:textbox></td>
			                    </tr>
		                    </table>
		                    <p>(your password is case-sensitive)</p>
                            <br />
                             <p><td class="fieldInput"><asp:CheckBox ID="chkRememberMe" Runat="server" Text="Remember Me"></asp:CheckBox></p>
		                </div>
		                
		                <div id="loginValidationBoxes">
		                
		                    <asp:requiredfieldvalidator id="rfvUserName" runat="server" ControlToValidate="txtUsername" Display="Dynamic" ErrorMessage="." ><div>The Username is required</div></asp:requiredfieldvalidator>
		                    <asp:requiredfieldvalidator id="rfvPIN" runat="server" ControlToValidate="txtPIN" Display="Dynamic" ErrorMessage="."><div>The Password is required</div></asp:requiredfieldvalidator>
						    <asp:label id="lblMessage" runat="server" visible="false" cssclass="Error"></asp:label>
    		                
		                </div>
		                
		                <div class="loginButtonBarContainer">
                        <div class="loginButtonBar">
		                    <table>
			                    <tr>
			                        <td class="field" align="right"><asp:button id="btnLogon"  CssClass="buttonclass" Runat="server" Text="Logon" onclick="btnLogon_Click"></asp:button></td>
			                       
			                    </tr>
			                </table>
                            </div>
			            		</div>			            
			            <div class="clearDiv"></div>
						<p style="display: none;">This is a private computer system. Unauthorised access to this system is an offence under the Computer Misuse Act.
					    Unauthorised disclosure of information on this system is an offence under the Data Protection Act.
						If you feel that you have reached this page in error, please call the helpdesk on 0845 644 3750.</p>
			           
			        </div>
			        <div class="loginBoxBottom">&nbsp;</div>
			        <div class="loginFooter">Application Provided and supported by <a href="http://www.proteo.co.uk/">Proteo Ltd</a></div>
			    </div>
			</div>
		</form>
		<script type="text/javascript">
	        document.all['<%=txtUserName.ClientID%>'].focus();
		</script>
	</body>
</html>
