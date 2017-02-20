<%@ Page Language="C#" AutoEventWireup="true" EnableEventValidation="false" Inherits="Orchestrator.WebUI.Organisation.preChangeReference" MasterPageFile="~/WizardMasterPage.master" Title="Haulier Enterprise" Codebehind="preChangeReference.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Change References</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="server">
    <table id="tblHidden">
        <tr>
            <td><asp:HiddenField ID="hidJobs" runat="server" Value="0" /></td>
            <td><asp:HiddenField ID="hidOrders" runat="server" Value="0" /></td>
        </tr>
        <tr>
            <td colspan="2" visible="false">
                <asp:Button ID="btnRedirect" runat="server" style="display:none;"/>
            </td>
        </tr>
    </table>
    <script language="javascript" type="text/javascript">var hidJobs = document.getElementById("<%=hidJobs.ClientID%>");var hidOrders = document.getElementById("<%=hidOrders.ClientID%>");var btn = document.getElementById("<%=btnRedirect.ClientID%>");hidJobs.value = window.opener.window.jobs;hidOrders.value = window.opener.window.orders;if (btn != null) {btn.click(); } </script>
</asp:Content>