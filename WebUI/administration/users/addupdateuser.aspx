<%@ Page Language="c#" MasterPageFile="~/WizardMasterPage.master" Inherits="Orchestrator.WebUI.administration.users.addupdateuser"
    CodeBehind="addupdateuser.aspx.cs" %>

<asp:Content ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">User Details</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script language="javascript">
        function moveSelectedOptions(from, to) {
            // Unselect matching options, if required
            if (arguments.length > 3) {
                var regex = arguments[3];
                if (regex != "") {
                    unSelectMatchingOptions(from, regex);
                }
            }
            // Move them over
            for (var i = 0; i < from.options.length; i++) {
                var o = from.options[i];
                if (o.selected) {
                    to.options[to.options.length] = new Option(o.text, o.value, false, false);
                }
            }
            // Delete them from original
            for (var i = (from.options.length - 1); i >= 0; i--) {
                var o = from.options[i];
                if (o.selected) {
                    from.options[i] = null;
                }
            }
            if ((arguments.length < 3) || (arguments[2] == true)) {
                //sortSelect(from);
                //sortSelect(to);
            }
            from.selectedIndex = -1;
            to.selectedIndex = -1;

            document.all('<%=txtSelectedRoles.ClientID%>').value = '';
            var lbAvailableRoles = '<%=lbAvailableRoles.ClientID %>'

            if (from.id == "<%=lbAvailableRoles.ClientID %>") {
                for (var p = 0; p < to.options.length; p++) {
                    var o = to.options[p];
                    document.all('<%=txtSelectedRoles.ClientID%>').value += ',' + o.value;

                }
            }
            else {
                for (var p = 0; p < from.options.length; p++) {
                    var o = from.options[p];
                    document.all('<%=txtSelectedRoles.ClientID%>').value += ',' + o.value;
                }
            }

        }

        function moveAllOptions(from, to) {
            selectAllOptions(from);
            if (arguments.length == 2) {
                moveSelectedOptions(from, to);
            }
            else if (arguments.length == 3) {
                moveSelectedOptions(from, to, arguments[2]);
            }
            else if (arguments.length == 4) {
                moveSelectedOptions(from, to, arguments[2], arguments[3]);
            }
            document.all('<%=txtSelectedRoles.ClientID%>').value = '';

            if (from.id == "<%=lbAvailableRoles.ClientID %>") {
                for (var p = 0; p < to.options.length; p++) {
                    var o = to.options[p];
                    document.all('<%=txtSelectedRoles.ClientID%>').value += ',' + o.value;

                }
            }
            else {
                for (var p = 0; p < from.options.length; p++) {
                    var o = from.options[p];
                    document.all('<%=txtSelectedRoles.ClientID%>').value += ',' + o.value;
                }
            }

        }
        function selectAllOptions(obj) {
            for (var i = 0; i < obj.options.length; i++) {
                obj.options[i].selected = true;
            }
        }

        function confirmUserRemoval() {
            var retVal = confirm("Are you sure you wish to remove this User, as this action cannot be undone once confirmed?");
            return retVal;
        }
    </script>

    <asp:ValidationSummary ID="valSummary" runat="server"></asp:ValidationSummary>
    
    <table>
        <asp:Panel ID="pnlClient" runat="server" Visible="False">
            <tr>
                <td class="formCellLabel">Client</td>
                <td colspan="3">
                    <telerik:RadComboBox ID="cboClient" runat="server" RadControlsDir="~/script/RadControls/"
                        EnableLoadOnDemand="true" ShowMoreResultsBox="false" MarkFirstMatch="true" Skin="WindowsXP"
                        ItemRequestTimeout="500" Width="255px" Overlay="true" ZIndex="50" Height="400px"
                        AllowCustomText="false">
                    </telerik:RadComboBox>
                    <asp:RequiredFieldValidator ID="rfvClient" runat="server" ControlToValidate="cboClient"
                        Display="Dynamic" ErrorMessage="Please select a client."><img src="../../images/error.gif" alt="Please select a client." /></asp:RequiredFieldValidator>
                    <asp:CustomValidator ID="cfvClient" runat="server" ControlToValidate="cboClient"
                        Display="Dynamic" EnableClientScript="False" ErrorMessage="Please select a client."><img src="../../images/error.gif" alt="Please select a client." /></asp:CustomValidator>
                </td>
                <td colspan="2">
                </td>
            </tr>
        </asp:Panel>
        <tr>
            <td class="formCellLabel">User Name</td>
            <td colspan="3">
                <asp:TextBox ID="txtUserName" runat="server" autocomplete="off"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="formCellLabel">Forenames</td>
            <td colspan="3">
                <asp:TextBox ID="txtForenames" runat="server" autocomplete="off"></asp:TextBox><asp:RequiredFieldValidator
                    ID="rfvForenames" runat="server" ErrorMessage="The Forename must be completed"
                    Display="None" ControlToValidate="txtForenames"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="formCellLabel">Surname</td>
            <td colspan="3">
                <asp:TextBox ID="txtSurname" runat="server" autocomplete="off"></asp:TextBox><asp:RequiredFieldValidator
                    ID="rfvSurname" runat="server" ErrorMessage="The Surname must be completed" Display="None"
                    ControlToValidate="txtSurname"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <asp:Panel ID="pnlPassword" runat="server">
            <tr>
                <td class="formCellLabel">Password</td>
                <td colspan="3">
                    <asp:TextBox ID="txtPassword" runat="server" autocomplete="off" TextMode="password"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword"
                        Display="None" ErrorMessage="The Password must be completed"></asp:RequiredFieldValidator>
                    <asp:CustomValidator ID="rfvComplex" runat="server" Display="None" ErrorMessage="The new password does not conform to complex password rules. Please try a different password that is at least 8 characters long, is not the same as the username and contains upper and lowercase letters along with numbers."></asp:CustomValidator>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">Confirm Password</td>
                <td colspan="3">
                    <asp:TextBox ID="txtConfirmPassword" runat="server" autocomplete="off" TextMode="password"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvConfirmPassword" runat="server" ControlToValidate="txtConfirmPassword"
                        Display="None" ErrorMessage="The Confirm Password must be completed"></asp:RequiredFieldValidator>
                    <asp:CompareValidator ID="cfvPassword" runat="server" ControlToValidate="txtConfirmPassword"
                        Display="None" ErrorMessage="The Password and the Confirmation do not match."
                        ControlToCompare="txtPassword"></asp:CompareValidator>
                </td>
            </tr>
        </asp:Panel>
        <tr>
            <td class="formCellLabel" height="20">E-Mail</td>
            <td colspan="3" height="20">
                <asp:TextBox ID="txtEmail" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail"
                    Display="None" ErrorMessage="The Email must be completed"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revEmail" runat="server" ErrorMessage="Please enter a valid email."
                    Display="None" ControlToValidate="txtEmail" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <asp:Panel ID="pnlTeam" runat="server">
            <tr>
                <td class="formCellLabel">Team</td>
                <td colspan="3">
                    <asp:DropDownList ID="cboTeam" runat="server" Width="136px">
                    </asp:DropDownList>
                </td>
            </tr>
        </asp:Panel>
        <tr>
            <td class="formCellLabel">User Status</td>
            <td colspan="3">
                <asp:DropDownList ID="cboUserStatus" runat="server" Width="136px" Enabled="false">
                </asp:DropDownList>
            </td>
        </tr>
        <tr id="trPartTime" runat="server">
            <td class="formCellLabel" style="width: 175px;">Part time?
            </td>
            <td colspan="3">
                <asp:CheckBox ID="chkIsPartTime" runat="server" Enabled="false"></asp:CheckBox>
            </td>
        </tr>
        <tr>
            <td class="formCellLabel" style="width: 175px;">Allow this user to log in from anywhere?
            </td>
            <td colspan="3">
                <asp:CheckBox ID="chkCanAccessFromAnywhere" runat="server"></asp:CheckBox>
            </td>
        </tr>
        <tr>
            <td class="formCellLabel">User has scanned license</td>
            <td colspan="3">
                <asp:CheckBox ID="chkScannedLicense" runat="server"></asp:CheckBox>
            </td>
        </tr>
        <tr>
            <td class="formCellLabel" rowspan="3">User Groups</td>
        </tr>
        <tr>
            <td rowspan="2">
                Roles user does NOT have:
                <asp:ListBox ID="lbAvailableRoles" runat="server" Width="200px" DataTextField="Description"
                    DataValueField="RoleId" SelectionMode="multiple"></asp:ListBox>
            </td>
            <td>
                <br />
                <input type="button" name="add" value=">" onclick="moveSelectedOptions(document.all['<%=lbAvailableRoles.ClientID%>'],document.all['<%=lbSelectedRoles.ClientID%>'])">&nbsp;<input
                    type="button" name="addall" value=">>" onclick="moveAllOptions(document.all['<%=lbAvailableRoles.ClientID%>'],document.all['<%=lbSelectedRoles.ClientID%>'])">
            </td>
            <td rowspan="2">
                Roles user currently has:
                <select id="lbSelectedRoles" name="lbSelectedRoles" runat="server" multiple style="width: 200px">
                </select>
                <asp:CustomValidator ID="cvSelectedRoles" runat="server" Display="none" ControlToValidate="lbSelectedRoles"
                    EnableClientScript="true" ErrorMessage="No user roles selected to assign to user."><img src="../../images/Error.gif" height="16" width="16" title="No user roles selected to assign to user." /></asp:CustomValidator>
                <input type="hidden" id="txtSelectedRoles" runat="server" name="txtSelectedRoles"/>
            </td>
        </tr>
            <tr>
                <td>
                    <input type="button" name="removeall" value="<" onclick="moveSelectedOptions(document.all['<%=lbSelectedRoles.ClientID%>'],document.all['<%=lbAvailableRoles.ClientID%>'])">&nbsp;<input
                        type="button" name="remove" value="<<" onclick="moveAllOptions(document.all['<%=lbSelectedRoles.ClientID%>'],document.all['<%=lbAvailableRoles.ClientID%>'])">
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td colspan="3">
                    <asp:Label ID="lblMessage" runat="server" CssClass="errorMessage" EnableViewState="false"></asp:Label>
                </td>
            </tr>
            <asp:Panel ID="pnlEmailDetails" runat="server" Visible="False">
                <tr>
                    <td>
                        <asp:CheckBox ID="chkEmailDetails" runat="server" Checked="True" Text="Email Details to Client" />
                    </td>
                </tr>
            </asp:Panel>
    </table>

    <div class="buttonbar">
        <asp:Button ID="btnRemove" runat="server" Width="75px" Text="Remove" OnClientClick="if(!confirmUserRemoval()) return false;" OnClick="btnRemove_Click" Visible="false" />
        <asp:Button ID="cmdLock" runat="server" Width="75px" Text="Lock" OnClick="cmdLock_Click"></asp:Button>
        <asp:Button ID="cmdChangePassword" runat="server" Width="127px" Text="Change Password" OnClick="cmdChangePassword_Click"></asp:Button>
        <asp:Button ID="btnAdd" runat="server" Text="Add" Width="75" ></asp:Button>
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CausesValidation="false" OnClick="btnCancel_Click" Width="75"></asp:Button>
    </div>
            
</asp:Content>
