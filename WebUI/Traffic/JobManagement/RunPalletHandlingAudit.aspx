<%@ Page Language="C#" MasterPageFile="~/WizardMasterPage.Master" AutoEventWireup="true" CodeBehind="RunPalletHandlingAudit.aspx.cs" Inherits="Orchestrator.WebUI.Traffic.JobManagement.RunPalletHandlingAudit" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Run Pallet Handling Audit</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <div style="height:275px; overflow:scroll;">
        <asp:ListView ID="lvTrailerAudit" runat="server">
            <LayoutTemplate>
                <table class="Grid" cellpadding="0" cellspacing="0" style="width:100%;">
                    <thead class="HeadingRow">
                        <th>Action</th>
                        <th>Client</th>
                        <th>Point</th>
                        <th>Driver</th>
                        <th>Vehicle</th>
                        <th>Trailer</th>
                        <th>No Pallets</th>
                        <th>PalletType</th>
                        <th>User</th>
                        <th>Create Date</th>
                    </thead>
                    <tbody>
                        <tr id="itemPlaceHolder" runat="server" />
                    </tbody>
                </table>
            </LayoutTemplate>
            <ItemTemplate>
                <tr class="Row">
                    <td><%# Eval("Description") %></td>
                    <td><%# Eval("OrganisationName") %></td>
                    <td><%# Eval("PointDescription") %></td>
                    <td><%# Eval("Driver") %></td>
                    <td><%# Eval("Vehicle") %></td>
                    <td><%# Eval("Trailer") %></td>
                    <td><%# Eval("NoOfPallets") %></td>
                    <td><%# Eval("PalletType") %></td>
                    <td><%# Eval("CreateUserID") %></td>
                    <td><%# ((DateTime)Eval("CreateDate")).ToString("g") %></td>
                </tr>
            </ItemTemplate>
            <EmptyDataTemplate>
                There are currently no pallet handling actions recorded on this run.
            </EmptyDataTemplate>
        </asp:ListView>
    </div>
    
    <div class="buttonbar" style="margin-top:10px;">
        <asp:Button ID="btnRefresh" runat="server" Text="Refresh" />
        <asp:Button ID="btnClose" runat="server" Text="Close" />
    </div>

</asp:Content>
