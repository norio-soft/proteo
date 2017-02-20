<%@ Page Language="C#" MasterPageFile="~/WizardMasterPage.master" AutoEventWireup="true" Inherits="Groupage_updateOrder" Title="Update Order"  Codebehind="updateOrder.aspx.cs" %>
<%@ MasterType TypeName="Orchestrator.WebUI.WizardMasterPage" %>
<%@ Register TagPrefix="p1" TagName="Order" Src="~/UserControls/order.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript" src="../script/toolTipPopUps.js"></script>
    <p1:Order runat="server" id="ucOrder"></p1:Order>
</asp:Content>
