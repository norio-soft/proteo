<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.administration.system.ClearTrafficSheetCookie" MasterPageFile="~/default_tableless.master" Title="Clear Traffic Sheet Cookie" Codebehind="ClearTrafficSheetCookie.aspx.cs" %>

<asp:Content ContentPlaceHolderID="Header" runat="server"></asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Remove Your Traffic Sheet Filter</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <div class="infoPanel">This page should only be used following instruction from your support team.  If you wish to continue, click the continue button below.</div> 
    <div class="whitespacepusher"></div>
    <div class="buttonBar">
        <asp:Button ID="btnContinue" runat="server" Text="Continue" />
        <asp:Label ID="lblDone" runat="server" ForeColor="Black" Text="Your traffic sheet filter has been removed." Visible="False"></asp:Label>
    </div>
</asp:Content>