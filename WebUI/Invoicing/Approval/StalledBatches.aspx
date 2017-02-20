<%@ Page Language="C#" MasterPageFile="~/default_tableless.master" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Invoicing.Approval.StalledBatches" Title="Stalled Invoicing Batches" Codebehind="StalledBatches.aspx.cs" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Stalled Invoice Batches</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <h2>Below is some information regarding invoice batches which have not been progressed.  There is a 15 minute delay until stalled batches appear. To work on a batch, click the "Continue" button, to remove a batch, click the "Remove" button.</h2>
    <telerik:RadGrid runat="server" ID="gvBatches" AllowPaging="false" AllowSorting="true" Skin="Office2007" EnableAJAX="true" AutoGenerateColumns="false" AllowMultiRowSelection="false" AllowFilteringByColumn="false" AllowAutomaticInserts="false">
        <MasterTableView Width="100%" DataKeyNames="PreInvoiceBatchID" AllowFilteringByColumn="false">
           <RowIndicatorColumn Display="false"></RowIndicatorColumn>
            <Columns>
                <telerik:GridBoundColumn DataField="CreateUserID" HeaderText="Created By" HeaderStyle-Width="150" />
                <telerik:GridBoundColumn DataField="CreateDate" HeaderText="Created On" DataFormatString="{0:dd/MM/yy}" HeaderStyle-Width="100" />
                <telerik:GridTemplateColumn HeaderText="Invoice Type" HeaderStyle-Width="100">
                    <ItemTemplate>
                        <asp:Label ID="lblInvoiceType" runat="server"></asp:Label>
                        <asp:HiddenField ID="hidInvoiceType" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Item Count" HeaderStyle-Width="100">
                    <ItemTemplate>
                        <asp:Label ID="lblItemCount" runat="server"></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridButtonColumn ButtonType="PushButton" CommandName="ContinueGroup" CommandArgument="WorkflowInstanceID" HeaderText="" HeaderStyle-Width="80" Text="Continue" />
                <telerik:GridButtonColumn ButtonType="PushButton" CommandName="RemoveGroup" CommandArgument="WorkflowInstanceID" HeaderText="" HeaderStyle-Width="80" Text="Remove" />
            </Columns>            
        </MasterTableView>
        <FilterMenu CssClass="FilterMenuClass"></FilterMenu>
    </telerik:RadGrid>
    <div class="buttonbar">
        <asp:Button ID="btnRefresh" runat="server" Text="Refresh" CausesValidation="false" />
    </div>

</asp:Content>