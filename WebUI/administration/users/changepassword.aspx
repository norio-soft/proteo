<%@ Page language="c#" Inherits="Orchestrator.WebUI.administration.users.changepassword" MasterPageFile="~/WizardMasterPage.master" Title="Change Password" Codebehind="changepassword.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContentFromChildPage" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Change Password</asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

	<asp:panel id="pnlChangePassword" runat="server">
		<table style="font-size:14px;" cellspacing="0">
            <tr>
                <td class="form-label">User Name:</td>
                <td><asp:textbox id="txtUsername" Width="150" Runat="server"></asp:textbox></td>
            </tr>
            <tr>
			    <td class="form-label">New Password:</td>
				<td><asp:textbox id="txtNewPassword" Width="100" Runat="server" TextMode="Password"></asp:textbox></td>
	        <tr>
			    <td class="form-label">Confirm:</td>
				<td><asp:textbox id="txtConfirmPassword" Width="100" Runat="server" TextMode="Password"></asp:textbox></td>
            </tr>
            <tr>
                <td colspan="6">
            								
				<P><asp:requiredfieldvalidator id="rfvUserName" runat="server" ControlToValidate="txtUsername" Display="None" ErrorMessage="The User Name is required"></asp:requiredfieldvalidator></P>
				<P><asp:requiredfieldvalidator id="rfvConfirmPwd" runat="server" ControlToValidate="txtConfirmPassword" Display="None"
						ErrorMessage="Please confirm your new Password"></asp:requiredfieldvalidator></P>
				<P><asp:comparevalidator id="rfvCompareNewPwds" runat="server" ControlToValidate="txtConfirmPassword" Display="None"
						ErrorMessage="The new and confirmed passwords do not match" ControlToCompare="txtNewPassword"></asp:comparevalidator></P>
				<P><asp:customvalidator id="rfvComplexPwd" runat="server" ControlToValidate="txtNewPassword" Display="None"
						ErrorMessage="The new password does not conform to complex password rules. Please try a different password."></asp:customvalidator></P>
				<P><asp:validationsummary id="rfvValidationSummary" runat="server"></asp:validationsummary></P>
				<p><asp:label id="lblMessage" runat="server" visible="false" cssclass="Error"></asp:label></p>
				<asp:Label id="lblDisclaimer" runat="server" ></asp:Label>
			    </td>
            </tr>
		</table>

        <div class="buttonBar">
            <asp:button id="btnSubmit" Runat="server" Text="Change Password" onclick="btnSubmit_Click"></asp:button>
            <input type="button" onclick="location.href='addupdateuser.aspx?identityId=<%=Request["identityId"]%>'" value="Cancel" class="button">
        </div>

	</asp:panel>

	<asp:panel id="pnlChangePasswordConfirmation" visible="false" runat="server">
		<span style="font-size:12px;">The password has been changed.</span>
		<div class="buttonbar">
			<asp:Button id="btnBack" runat="server" Text="Back to list"/>
		</div>
	</asp:panel>

</asp:Content>