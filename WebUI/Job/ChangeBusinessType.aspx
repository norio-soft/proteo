<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Job.ChangeBusinessType" MasterPageFile="~/WizardMasterPage.master" Title="Manage Business Type" Codebehind="ChangeBusinessType.aspx.cs" %>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Manage Business Type</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
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
    
    <div>
        <asp:Label ID="lblError" runat="server" CssClass="Error" Visible="false" style="font-size:10pt; color:Red;"></asp:Label>
        <table>
            <tr>
                <td class="formCellLabel">Current Business Type</td>
                <td class="formCellField"><asp:Label ID="lblBusinessType" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="formCellLabel">New Business Type</td>
                <td class="formCellField"><telerik:RadComboBox ID="cboBusinessType" runat="server" Skin="WindowsXP"></telerik:RadComboBox></td>
            </tr>
        </table>
    </div>
    <div class="buttonbar">
        <asp:Button ID="btnOK" runat="server" Text="OK" style="width:75px;"  />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" style="width:75px;" CausesValidation="false" />
    </div>
    
    <asp:label ID="InjectScript" runat="server"></asp:label>
    
</asp:Content>
