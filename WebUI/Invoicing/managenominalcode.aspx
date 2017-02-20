<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.ManageNominalCode" MasterPageFile="~/WizardMasterPage.master" Title="Manage Nominal Code" Codebehind="managenominalcode.aspx.cs" %>

<asp:Content ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
    <script language="javascript" type="text/javascript">
        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow; //Will work in Moz in all cases, including clasic dialog
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow; //IE (and Moz az well) 
            return oWindow;
        }

        function CloseOnReload() {
            GetRadWindow().Close();
        }

        function RefreshParentPage() {
            GetRadWindow().BrowserWindow.location.reload();
            GetRadWindow().Close();
        }

        function confirmUserRemoval() {
            var retVal = confirm("Are you sure you wish to remove ALL mappings for this Nominal Code? This action cannot be undone once confirmed?.");
            return retVal;
        }
 
    </script>
</asp:Content>

<asp:Content ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Manage Nominal Code</asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="height:250px; ">
        <asp:Label ID="lblError" runat="server" CssClass="Error" Visible="false" style="font-size:10pt; color:Red;"></asp:Label>
        <table>
            <tr>
                <td>Default Job Type</td>
                <td><telerik:RadComboBox ID="cboJobType" runat="server" Skin="WindowsXP"></telerik:RadComboBox></td>
            </tr>
            <tr>
                <td>Nominal Code</td>
                <td><asp:TextBox id="txtNominalCode" runat="server"></asp:TextBox><asp:RequiredFieldValidator ID="rfvNominalCode" runat="server" ControlToValidate="txtNominalCode" Display="dynamic" ErrorMessage="You must enter a code."><img src="../images/error.png" title="You must enter a Nominal Code" /> </asp:RequiredFieldValidator></td>
            </tr>
            <tr>
                <td>Description</td>
                <td><asp:TextBox id="txtDescription" runat="server" Width="200"></asp:TextBox><asp:RequiredFieldValidator ID="rfvDescription" runat="server" ControlToValidate="txtDescription" Display="dynamic" ErrorMessage="You must enter a description."><img src="../images/error.png" title="You must enter a Descriptionf ro the use of this Nominal Code" /> </asp:RequiredFieldValidator></td>
            </tr>
            <tr>
                <td>Active</td>
                <td><asp:CheckBox id="chkIsActive" runat="server" Width="200"></asp:CheckBox></td>
            </tr>
            <tr id="trAssignmentNote" runat="server">
                <td>Note</td>
                <td>
                    This Nominal Code is currently being used by the below mappings.<br />
                    Please manually remove all mappings if you want to make this Nominal Code in-active. Or press Unsassign to remove ALL mappings.
                </td>
            </tr>
            <tr id="trAssignments" runat="server">
                <td style="font-weight:bold">Assignments:</td>
                <td></td>
            </tr>
            <tr id="trBusinessTypes" runat="server">
                <td>Business Types mapped</td>
                <td><asp:Label ID="lblBusinessTypesMapped" runat="server"></asp:Label></td>
            </tr>
            <tr id="trExtraTypes" runat="server">
                <td>Extra Types mapped</td>
                <td><asp:Label ID="lblExtraTypesMapped" runat="server"></asp:Label></td>
            </tr>
            <tr id="trOrganisations" runat="server">
                <td>Organisations mapped</td>
                <td><asp:Label ID="lblOrganisationsMapped" runat="server"></asp:Label></td>
            </tr>
            <tr id="trVehicles" runat="server">
                <td>Vehicles mapped</td>
                <td><asp:Label ID="lblVehiclesMapped" runat="server"></asp:Label></td>
            </tr>
        </table>
        </div>
    <div class="buttonbar">
        <asp:Button ID="btnOK" runat="server" Text="OK" style="width:75px;" />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" style="width:75px;" CausesValidation="false" />
        <asp:Button ID="btnUnassign" runat="server" Text="Unassign" style="width:75px;" OnClientClick="if(!confirmUserRemoval()) return false;" />
    </div>
    
    <asp:label ID="InjectScript" runat="server"></asp:label>
</asp:Content>