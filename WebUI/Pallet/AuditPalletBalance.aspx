<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="AuditPalletBalance.aspx.cs" Inherits="Orchestrator.WebUI.Pallet.AuditPalletBalance" %>
<%@ Register TagPrefix="cc1" Namespace="Orchestrator.WebUI" Assembly="Orchestrator.WebUI.Dialog" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">
    <script type="text/javascript" language="javascript">
    
        function OpenJobWindow(jobID) {
            var qs = "jobId=" + jobID;
            <%=dlgJob.ClientID %>_Open(qs);
        }
    
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1><asp:Label ID="lblPalletBalanceType" runat="server" Font-Bold="false" Font-Size="Small" /></h1></asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <cc1:Dialog ID="dlgJob" runat="server" URL="/job/Job.aspx" Height="600" Width="800" Mode="Normal" AutoPostBack="false" ReturnValueExpected="false" UseCookieSessionID="true"/>
    <h2>Shows the audit trail of the selected item.</h2>
    
    <fieldset>
        <legend>Audit Trail Filter</legend>
        <table>
            <tr>
                <td class="formCellLabel">PalletType</td>
                <td class="formCellField"><telerik:RadComboBox ID="rcbPalletType" runat="server" AutoPostBack="false" DataTextField="Description" DataValueField="PalletTypeId" /></td>
            </tr>
            <tr>
                <td class="formCellLabel">Start date</td>
                <td class="formCellField">
                    <telerik:RadDateInput ID="rdiStartDate" runat="server" DateFormat="dd/MM/yy" />
                    <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" Display="Dynamic" ControlToValidate="rdiStartDate" ToolTip="Please enter a Start date.">
                        <img src="/images/error.png" alt="error" />
                    </asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">End Date</td>
                <td class="formCellField">
                    <telerik:RadDateInput ID="rdiEndDate" runat="server" DateFormat="dd/MM/yy" />
                    <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" Display="Dynamic" ControlToValidate="rdiEndDate" ToolTip="Please enter an End date.">
                        <img src="/images/error.png" alt="error" />
                    </asp:RequiredFieldValidator>
                </td>
            </tr>
        </table>
    </fieldset>
    
    <div class="buttonBar">
        <asp:Button ID="btnBack" runat="server" Text="Back" />
        <asp:Button ID="btnRefresh" runat="server" Text="Refresh" />
    </div>
    
    <div style="margin: 10px 0 10px 0;">
        
        <asp:Panel ID="pnlClient" runat="server" Visible="false">
            <asp:ListView ID="lvClientAudit" runat="server">
                <LayoutTemplate>
                    <table class="Grid" cellpadding="0" cellspacing="0" style="width:500px;">
                        <thead class="HeadingRow">
                            <th>Date</th>
                            <th>Run ID</th>
                            <th>Resource</th>
                            <th>Action</th>
                            <th>No Pallets</th>
                            <th>User</th>
                        </thead>
                        <tbody>
                            <tr id="itemPlaceHolder" runat="server" />
                        </tbody>
                    </table>
                </LayoutTemplate>
                <ItemTemplate>
                    <tr class="Row">
                        <td><%# ((DateTime)Eval("CreateDate")).ToString("g") %></td>
                        <td><a href='javascript:OpenJobWindow(<%# Eval("JobID") %>);'><%# Eval("JobID") %></a></td>
                        <td><%# Eval("Resource") %></td>
                        <td><%# Eval("Description") %></td>
                        <td><%# Eval("NoPallets") %></td>
                        <td><%# Eval("UserID") %></td>
                    </tr>
                </ItemTemplate>
                <EmptyDataTemplate>
                    There are currently no actions within the specified date range.
                </EmptyDataTemplate>
            </asp:ListView>
        </asp:Panel>
        
        <asp:Panel ID="pnlPoint" runat="server" Visible="false">
            <asp:ListView ID="lvPointAudit" runat="server">
                <LayoutTemplate>
                    <table class="Grid" cellpadding="0" cellspacing="0" style="width:500px;">
                        <thead class="HeadingRow">
                            <th>Date</th>
                            <th>Run ID</th>
                            <th>Resource</th>
                            <th>Action</th>
                            <th>No Pallets</th>
                            <th>User</th>
                        </thead>
                        <tbody>
                            <tr id="itemPlaceHolder" runat="server" />
                        </tbody>
                    </table>
                </LayoutTemplate>
                <ItemTemplate>
                    <tr class="Row">
                        <td><%# ((DateTime)Eval("CreateDate")).ToString("g") %></td>
                        <td><a href='javascript:OpenJobWindow(<%# Eval("JobID") %>);'><%# Eval("JobID") %></a></td>
                        <td><%# Eval("Resource") %></td>
                        <td><%# Eval("Description") %></td>
                        <td><%# Eval("NoPallets") %></td>
                        <td><%# Eval("UserID") %></td>
                    </tr>
                </ItemTemplate>
                <EmptyDataTemplate>
                    There are currently no actions within the specified date range.
                </EmptyDataTemplate>
            </asp:ListView>
        </asp:Panel>
        
        <asp:Panel ID="pnlTrailer" runat="server" Visible="false">
            <asp:ListView ID="lvTrailerAudit" runat="server">
                <LayoutTemplate>
                    <table class="Grid" cellpadding="0" cellspacing="0" style="width:500px;">
                        <thead class="HeadingRow">
                            <th>Date</th>
                            <th>Run ID</th>
                            <th>Resource</th>
                            <th>Action</th>
                            <th>No Pallets</th>
                            <th>User</th>
                        </thead>
                        <tbody>
                            <tr id="itemPlaceHolder" runat="server" />
                        </tbody>
                    </table>
                </LayoutTemplate>
                <ItemTemplate>
                    <tr class="Row">
                        <td><%# ((DateTime)Eval("CreateDate")).ToString("g") %></td>
                        <td><a href='javascript:OpenJobWindow(<%# Eval("JobID") %>);'><%# Eval("JobID") %></a></td>
                        <td><%# Eval("Resource") %></td>
                        <td><%# Eval("Description") %></td>
                        <td><%# Eval("NoPallets") %></td>
                        <td><%# Eval("UserID") %></td>
                    </tr>
                </ItemTemplate>
                <EmptyDataTemplate>
                    There are currently no actions within the specified date range.
                </EmptyDataTemplate>
            </asp:ListView>
        </asp:Panel>
        
    </div>

</asp:Content>