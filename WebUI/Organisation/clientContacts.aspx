<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="clientContacts.aspx.cs" MasterPageFile="~/WizardMasterPage.master" Inherits="Orchestrator.WebUI.Organisation.clientContacts" %>
<%@ Register TagPrefix="con" TagName="contactLookUp" Src="~/UserControls/ContactsLookUp.ascx" %>
<asp:Content ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Contacts</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <con:contactLookUp ID="contactLookUp1" runat="server" />
</asp:Content>