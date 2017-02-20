<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.administration.system.RefreshSettings" MasterPageFile="~/default_tableless.master" Title="Refresh Settings" Codebehind="RefreshSettings.aspx.cs" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Header" runat="server"></asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Refresh Settings</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <fieldset style="padding:0px;margin-top:5px; margin-bottom:5px;">
        <div style="height:22px; border-bottom:solid 1pt silver;padding:2px;margin-bottom:5px; color:#ffffff; background-color:#5d7b9d;">Refresh Your System Settings</div>
        <p>This page should only be used following instruction from your support team.  If you wish to continue, click the continue button below.</p>
    </fieldset>
    
    <br />
    
    <div class="buttonbar">
        <asp:Button ID="btnContinue" runat="server" Text="Continue" />
        <asp:Label ID="lblDone" runat="server" ForeColor="Black" Text="Your settings have been refreshed" Visible="False"></asp:Label>
    </div>
</asp:Content>