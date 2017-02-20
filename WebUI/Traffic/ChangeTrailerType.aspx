<%@ Page Title="" Language="C#" MasterPageFile="~/WizardMasterPage.Master" AutoEventWireup="true" CodeBehind="ChangeTrailerType.aspx.cs" Inherits="Orchestrator.WebUI.Traffic.ChangeTrailerType" %>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Change Planning Category</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table>
        <tr><td colspan="2"><h1>Change Planning Category </h1></td></tr>
        <tr>
            <td>Current Trailer Type</td><td><asp:Label ID="lblType" runat="server"></asp:Label></td>
        </tr>
        <tr>
            <td>New Trailer Type</td><td><asp:DropDownList ID="cboType" runat="server"></asp:DropDownList></td>
        </tr>
    </table>
     <div class="buttonbar">
        <asp:Button ID="btnChangeType" runat="server" Text="Change Type"/>
        <asp:Button ID="btnCancel" runat="server" Text="Close" />
    </div>
</asp:Content>