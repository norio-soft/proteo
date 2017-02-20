<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="releasenotes.aspx.cs" Inherits="Orchestrator.WebUI.help.releasenotes" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
<h1>Release Notes</h1>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<asp:Panel runat="server" ID="pnldmin">
    <asp:Button ID="btnEdit" runat="server" Text="Edit Release Notes" OnClick="Button1_Click1" /> 
    <asp:Button ID="btnSaveNotes" runat="server" Text="Save Release Notes" OnClick="Button1_Click1" /> 
</asp:Panel>
<telerik:RadEditor runat="server" ID="NotesEditor" SkinID="DefaultSetOfTools" Height="600" BackColor="White">

</telerik:RadEditor>
</asp:Content>
