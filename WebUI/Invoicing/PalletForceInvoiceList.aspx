<%@ Page Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="PalletForceInvoiceList.aspx.cs" Inherits="Orchestrator.WebUI.Invoicing.PalletForceInvoiceList" Title="Haulier Enterprise" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">
</asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>List of PalletForce Pre Invoices</h1></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <h2>All the PalletForce Pre Invoices currently imported into the system.</h2>
    
    <div class="buttonbar" style="margin-bottom:5px;">
        <asp:Button ID="btnGetPrePalletForceInvoices" runat="server" CssClass="buttonClass" Text="Get Pre Invoices" />
        <input style="width: 200px;" value="Create New Pre Invoice" type="button" onclick="javascript:window.location = 'AddUpdatePalletForceInvoice.aspx';" />
    </div>
    
    <div>
        <telerik:RadGrid runat="server" ID="grdPalletForcePreInvoices" Skin="Orchestrator" AllowFilteringByColumn="false" AllowPaging="true" AllowSorting="true" AllowMultiRowSelection="true" EnableAJAX="true" AutoGenerateColumns="false" ShowStatusBar="false" PageSize="250">
            <ClientSettings AllowDragToGroup="true" AllowColumnsReorder="true">
                <Selecting AllowRowSelect="True" EnableDragToSelectRows="true"></Selecting>
            </ClientSettings>
            <PagerStyle NextPageText="Next &amp;gt;" PrevPageText="&amp;lt; Prev" Mode="NextPrevAndNumeric">
            </PagerStyle>
            <MasterTableView DataKeyNames="PreInvoiceID">
                <RowIndicatorColumn Display="false">
                </RowIndicatorColumn>
                <Columns>
                    <telerik:GridHyperLinkColumn UniqueName="hlcPreInvoiceID" HeaderText="Pre Invoice ID" DataType="System.Int32" DataTextFormatString="{0}" DataTextField="PreInvoiceID" DataNavigateUrlFormatString="AddUpdatePalletForceInvoice.aspx?pfID={0}" DataNavigateUrlFields="PreInvoiceID" />
                    <telerik:GridBoundColumn UniqueName="InvoiceAccountCode" HeaderText="Account Code" DataField="AccountCode" SortExpression="InvoiceAccountCode" AllowFiltering="true" />
                    <telerik:GridBoundColumn UniqueName="InvoiceNo" HeaderText="Invoice No" DataField="InvoiceNo" SortExpression="InvoiceNo" AllowFiltering="true" />
                    <telerik:GridBoundColumn UniqueName="InvoiceDate" HeaderText="Invoice Date" DataField="InvoiceDate" SortExpression="InvoiceDate" AllowFiltering="true" DataFormatString="{0:D}" />
                    <telerik:GridBoundColumn UniqueName="InvoiceNET" HeaderText="Net" DataField="NetAmount" SortExpression="Net" AllowFiltering="true" DataFormatString="{0:C}" />
                    <telerik:GridBoundColumn UniqueName="InvoiceVAT" HeaderText="VAT" DataField="TaxAmount" SortExpression="VAT" AllowFiltering="true" DataFormatString="{0:C}" />
                    <telerik:GridBoundColumn UniqueName="InvoiceTotal" HeaderText="Invoice Total" DataField="TotalAmount" SortExpression="Total" AllowFiltering="true" DataFormatString="{0:C}" />
                    <telerik:GridBoundColumn UniqueName="CreateDate" DataField="CreateDateTime" HeaderText="Created" AllowFiltering="true" />
                    <telerik:GridBoundColumn UniqueName="CreateUserId" DataField="CreateUserID" HeaderText="Created By" AllowFiltering="true" />
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
    </div>
    
    <div class="buttonbar">
        <asp:Button ID="btnGetPrePalletForceInvoices1" runat="server" CssClass="buttonClass" Text="Get Pre Invoices" ValidationGroup="vgGetPFPreInvoices" />
        <input style="width: 200px;" value="Create New Pre Invoice" type="button" onclick="javascript:window.location = 'AddUpdatePalletForceInvoice.aspx';" />
    </div>

</asp:Content>
