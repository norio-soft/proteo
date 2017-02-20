<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.ManageServiceLevel" MasterPageFile="~/WizardMasterPage.master" Title="Manage Service Level" Codebehind="manageservicelevel.aspx.cs" %>
<asp:Content ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Manage service level</asp:Content>
<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script language="javascript" type="text/javascript">
        function GetRadWindow()
        {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow; //Will work in Moz in all cases, including clasic dialog
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;//IE (and Moz az well) 
            return oWindow;
        }

        function CloseOnReload()
        {
            GetRadWindow().Close();
        }

        function RefreshParentPage()
        {
            GetRadWindow().BrowserWindow.location.reload();
            GetRadWindow().Close();
        }
 
    </script>
    <fieldset>
        <asp:Label ID="lblError" runat="server" CssClass="Error" Visible="false" style="font-size:10pt; color:Red;"></asp:Label>
        <table>
            <tr>
                <td class="formCellLabel">Short Code</td>
                <td class="formCellField"><asp:TextBox ID="txtShortDescription" runat="server" MaxLength="50" Width="50"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="formCellLabel">Description</td>
                <td class="formCellField"><asp:TextBox id="txtDescription" runat="server" Width="200"></asp:TextBox><asp:RequiredFieldValidator ID="rfvDescription" runat="server" ControlToValidate="txtDescription" Display="dynamic" ErrorMessage="You must enter a description."><img src="../images/error.png" title="You must enter a Descriptionf ro the use of this Nominal Code" /> </asp:RequiredFieldValidator></td>
            </tr>
            <tr>
                <td class="formCellField">Number of Days</td>
                <td class="formCellField"><telerik:RadNumericTextBox ID="rntNumberOfDays" runat="server" MinValue="0" NumberFormat-DecimalDigits="0" Type="Number" /></td>
            </tr>
            <tr>
                <td class="formCellField"></td>
                <td class="formCellField"><asp:Label ID="lblLastUpdate" runat="server"></asp:Label></td>
            </tr>
        </table>
   </fieldset>
    <div class="buttonbar">
        <asp:Button ID="btnOK" runat="server" Text="OK" style="width:75px;" />
        <input type="button" onclick="javascript:CloseOnReload();" value="Cancel" style="width:75px" />
    </div>
    <asp:label ID="InjectScript" runat="server"></asp:label>
</asp:Content>
