<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GenericMessageImport.aspx.cs" Inherits="Orchestrator.WebUI.administration.GenericMessageImport" MasterPageFile="~/default_tableless.master" Title="Haulier Enterprise" %>

<asp:Content ContentPlaceHolderID="Header" runat="server">
    <script type="text/javascript">

        function OnMessageTextChanged() {

            // Find the copy and paste radio button and make sure it is checked.
            $('input:radio[id*=rbCopyPaste]')[0].checked = true;
            $('input:radio[id*=rbUpload]')[0].checked = false;

        }

        function OnFileClick() {

            $('input:radio[id*=rbCopyPaste]')[0].checked = false;
            $('input:radio[id*=rbUpload]')[0].checked = true;
        }
        
    </script>
</asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Import Message</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    
    <div>
        <table>
            <tr>
                <td>
                    From System:
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtFromSystem"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvFromSystem" runat="server" Display="Dynamic" ControlToValidate="txtFromSystem" ErrorMessage="Please specify the system from which the message came from.">
                        <img id="imgFromSystem" src="/App_Themes/Orchestrator/img/MasterPage/icon-warning.png" runat="server" title="Please specify the system from which the message came from." />
                   </asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <div style="height: 15px;">
                    </div>
                </td>
            </tr>
            <tr style="margin-left: 20px;">
                <td colspan="2">
                    <h2>
                        <asp:RadioButton runat="server" ID="rbUpload" GroupName="ImportType" />&nbsp; Upload
                        the import message file</h2>
                </td>
            </tr>
            <tr id="trUpload" style="margin-left: 20px;">
                <td style="vertical-align: top;">
                    Select file:
                </td>
                <td>
                    <asp:FileUpload onclick="javascript:OnFileClick();" runat="server" ID="upload1" Width="430px" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <h2>
                        <asp:RadioButton runat="server" ID="rbCopyPaste" GroupName="ImportType" />&nbsp;
                        Copy and paste the message:</h2>
                </td>
            </tr>
            <tr id="trCopyPaste">
                <td style="vertical-align: top;">
                    Message:
                </td>
                <td>
                    <asp:TextBox onchange="javascript:OnMessageTextChanged();" runat="server" ID="txtMessage"
                        TextMode="MultiLine" Height="230px" Width="430px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <div class="buttonbar">
        <asp:Button runat="server" ID="btnSubmit" Text="Submit" />&nbsp;<asp:Label runat="server" ID="lblResult" ForeColor="Red"></asp:Label></div>
    </div>

</asp:Content>