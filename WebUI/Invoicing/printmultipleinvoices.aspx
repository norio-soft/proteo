<%@ Page Title="" Language="C#" MasterPageFile="~/WizardMasterPage.Master" AutoEventWireup="true" CodeBehind="printmultipleinvoices.aspx.cs" Inherits="Orchestrator.WebUI.Invoicing.printmultipleinvoices" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContentFromChildPage" runat="server"></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Invoices</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="ButtonBar">
        <asp:Button id="Print" OnClientClick="PrintAll(); return false;" runat="server" Text="Print All" />
    </div>
    
    <iframe id="frmAllInvoices" width="100%" height="1000px" ></iframe>
    
    <script type="text/javascript">
        function pageLoad() {
            var url = "<%=GetURLForPrint %>";
            $("#frmAllInvoices").attr("src", url);
            $("#frmAllInvoices").height($(".masterpagepopup_contentHolder").height() - 35);
        }
    </script>
</asp:Content>
