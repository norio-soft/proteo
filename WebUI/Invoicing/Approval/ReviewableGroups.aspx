<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Invoicing.Approval.ReviewableGroups" MasterPageFile="~/default_tableless.master" Title="Invoice Groups Requiring Approval" Codebehind="ReviewableGroups.aspx.cs" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Invoice Groups Requiring Approval</h1></asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    
    <h2>Below is some information regarding invoices which require approval. To work on a batch, click the "Start Approval" button.</h2>
    <telerik:RadGrid runat="server" ID="gvGroups" AllowPaging="false" AllowSorting="true" Skin="Office2007" EnableAJAX="true" AutoGenerateColumns="false" AllowMultiRowSelection="false" AllowFilteringByColumn="false" AllowAutomaticInserts="false">
        <MasterTableView Width="100%" DataKeyNames="WorkflowInstanceID" AllowFilteringByColumn="false">
           <RowIndicatorColumn Display="false"></RowIndicatorColumn>
            <Columns>
                <telerik:GridBoundColumn DataField="CreateUserID" HeaderText="Created By" HeaderStyle-Width="150" />
                <telerik:GridBoundColumn DataField="CreateDate" HeaderText="Created On" DataFormatString="{0:dd/MM/yy}" HeaderStyle-Width="100" />
                <telerik:GridBoundColumn DataField="PreInvoiceIDCount" HeaderText="Invoices Requiring Approval" />
                <telerik:GridTemplateColumn HeaderText="Total Net">
                    <ItemTemplate>
                        <asp:Label ID="netAmountCurrencyLabel" runat="server" Text=""></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Total on Extras" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <asp:Label ID="extraAmountCurrencyLabel" runat="server" Text=""></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Total Fuel Surcharge" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <asp:Label ID="fuelSurchargeAmountCurrencyLabel" runat="server" Text=""></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Total Tax" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <asp:Label ID="taxAmountCurrencyLabel" runat="server" Text=""></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Grand Total" ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <asp:Label ID="grandTotalAmountCurrencyLabel" runat="server" Text=""></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridButtonColumn ButtonType="PushButton" CommandName="ApproveGroup" CommandArgument="WorkflowInstanceID" HeaderText="" HeaderStyle-Width="80" Text="Start Approval" />
            </Columns>            
        </MasterTableView>
        <FilterMenu CssClass="FilterMenuClass"></FilterMenu>
    </telerik:RadGrid>
    <div class="buttonbar">
        <asp:Button ID="btnRefresh" runat="server" Text="Refresh" CausesValidation="false" />
    </div>
</asp:Content>