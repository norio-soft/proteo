<%@ Register TagPrefix="uc1" TagName="title" Src="../UserControls/title.ascx" %>
<%@ Page language="c#" Inherits="Orchestrator.WebUI.Security.changepassword" Codebehind="changepassword.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
    <head runat="server">
        <title>Change Password</title>
        <link href="../App_Themes/Orchestrator/Login.Orchestrator.css" type="text/css" rel="stylesheet" />

        <meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1" />
        <meta name="CODE_LANGUAGE" Content="C#" />
        <meta name="vs_defaultClientScript" content="JavaScript" />
        <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5" />
        <link href='<%=Page.ResolveUrl("~/style/hms.css")%>' type="text/css" rel="stylesheet" />
    </head>
    <body class="loginBody">
        <form id="Form1" method="post" runat="server">
	        <asp:panel id="pnlChangePassword" runat="server">
	            <br />
	            <br />
	            <div id="loginContainer">
	                <div class="loginLogo" style="background-image: none;"><img src="../App_Themes/Orchestrator/Img/Login/he-logo.png" alt="Haulier-Enterprise" /></div>
	                <div class="loginBoxOuter">
	                    <div class="loginBoxTop">&nbsp;</div>
	                    <div class="loginBoxMiddle">
                        <div class="loginOrganisationNameContainer">
	                        <span id="lblChangePassword" style="font-size: 18px; color:#CCCFD1">Change Password</span>
                            </div>
	                        <p><asp:label id=lblPasswordExpired runat="server" Text="Your current password has expired please change your password." visible="false"></asp:label></p>
	                        <asp:Label ID="lblOrganisationName" runat="server"><%=Orchestrator.Globals.Configuration.InstallationCompanyName %></asp:Label>
	                        <div class="loginCredentialsContainer">
	                            <table class="loginCredentials">
	                                <tr>
	                                    <td class="fieldLabel" style="width: 200px;">Username</td>
	                                    <td class="fieldInput"><asp:textbox id=txtUsername Width="150" Runat="server"></asp:textbox></td>
	                                </tr>
	                                <tr>
	                                    <td class="fieldLabel" style="width: 200px;">Current Password</td>
	                                    <td class="fieldInput"><asp:textbox id=txtOldPassword Width="150" Runat="server" TextMode="Password"></asp:textbox></td>
	                                </tr>
	                                <tr>
	                                    <td class="fieldLabel" style="width: 200px;">New Password</td>
	                                    <td class="fieldInput"><asp:textbox id=txtNewPassword Width="150" Runat="server" TextMode="Password"></asp:textbox></td>
	                                </tr>
	                                <tr>
	                                    <td class="fieldLabel" style="width: 200px;">Confirm</td>
	                                    <td class="fieldInput"><asp:textbox id=txtConfirmPassword Width="150" Runat="server" TextMode="Password"></asp:textbox></td>
	                                </tr>
                                </table>
                            </div>
    		                
                            <div id="loginValidationBoxes">
                                <asp:requiredfieldvalidator id=rfvUserName runat="server" ControlToValidate="txtUsername" Display="None" ErrorMessage="The User Name is required"></asp:requiredfieldvalidator>
                                <asp:requiredfieldvalidator id=rfvOldPwd runat="server" ControlToValidate="txtOldPassword" Display="None" ErrorMessage="The Old Password is required"></asp:requiredfieldvalidator>
                                <asp:requiredfieldvalidator id=rfvNewPwd runat="server" ControlToValidate="txtNewPassword" Display="None" ErrorMessage="The new Password is required"></asp:requiredfieldvalidator>
                                <asp:requiredfieldvalidator id=rfvConfirmPwd runat="server" ControlToValidate="txtConfirmPassword" Display="None" ErrorMessage="Please confirm your new Password"></asp:requiredfieldvalidator>
                                <asp:comparevalidator id=rfvCompareNewPwds runat="server" ControlToValidate="txtConfirmPassword" Display="None" ErrorMessage="The new and confirmed passwords do not match" ControlToCompare="txtNewPassword"></asp:comparevalidator>
                                <asp:customvalidator id=rfvComplexPwd runat="server" ControlToValidate="txtNewPassword" Display="None" ErrorMessage="The new password does not conform to complex password rules. Please try a different password."></asp:customvalidator>
                                <asp:validationsummary id=rfvValidationSummary runat="server"></asp:validationsummary>
                                <asp:label id=lblMessage runat="server" visible="false" cssclass="Error"></asp:label>
                            </div>
    		                <div class ="loginButtonBarContainer">
                            <div class="loginButtonBar">
                            <table>
			                    <tr>
                                <td class="field" align="right"><asp:button id=btnSubmit Text="Change Password" CssClass="buttonclass" Runat="server" onclick="btnSubmit_Click" align="right"></asp:button></td>
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
	                    <div class="loginFooter">Application Provided and supported by <a href="http://www.proteo.co.uk/">Proteo</a></div>
	                </div>
	            </div>
            </asp:panel>
            <!-- -->
			<asp:panel id="pnlChangePasswordConfirmation" runat="server" visible="false">   
                <br />
	            <br />
	            <div id="loginContainer">
                    <div class="loginLogo" style="background-image: none;"><img src="../App_Themes/Orchestrator/Img/Login/he-logo.png" alt="Orchestrator" /></div>
                    <div class="loginBoxOuter">
                        <div class="loginBoxTop">&nbsp;</div>
                        <div class="loginBoxMiddle">
                             <div class="loginOrganisationNameContainer">
	                        <span id="lblPasswordChanged" style="font-size: 18px; color:#CCCFD1">Password Changed</span>
                            </div>
                            <p><asp:label id=Label1 runat="server" Text="Your current password has expired please change your password." visible="false"></asp:label></p>
                            <asp:Label ID="Label2" runat="server"><%=Orchestrator.Globals.Configuration.InstallationCompanyName %></asp:Label>
                            <div class="loginCredentialsContainer">
                                <p>Your password has been changed.</p>
                            </div>
                            <div class ="loginButtonBarContainer">
                            <div class="loginButtonBar">
                            <table>
			                    <tr>
                                <td class="field" align="right">
                                <asp:button id="btnContinue" CssClass="buttonclass" runat="server" text="Continue" onclick="btnContinue_Click"></asp:button></td>
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
                        <div class="loginFooter">Application Provided and supported by <a href="http://www.proteo.co.uk/">Proteo</a></div>
                    </div>
                </div>			
            </asp:panel>		
        </form>
    </body>
</html>
