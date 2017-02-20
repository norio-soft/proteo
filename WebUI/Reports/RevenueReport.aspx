<%@ Page Language="C#" MasterPageFile="~/default_tableless.master" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Report.Reports_RevenueReport" Title="Revenue Summary" Codebehind="RevenueReport.aspx.cs" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:Panel id="pnlDefaults" runat="server" DefaultButton="btnRefresh">
        <h1>All work for Organisation</h1>
        <h2></h2>
        <fieldset>
            <legend>Filter Options</legend>
            <table>
                <tr>
                    <td runat="server" id="tdDateOptions" >
                        <table>
                            <tr>
                                <td class="formCellLabel">Date From</td>
                                <td class="formCellField"><telerik:RadDateInput id="dteStartDate" runat="server" dateformat="dd/MM/yy" ToolTip="The Start Date for the filter"></telerik:RadDateInput></td>
                                <td class="formCellField"><asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ControlToValidate="dteStartDate" ValidationGroup="grpRefresh" ErrorMessage="Please enter a Start Date"></asp:RequiredFieldValidator></td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">Date To</td>
                                <td class="formCellField"><telerik:RadDateInput id="dteEndDate" runat="server" dateformat="dd/MM/yy" ToolTip="The Start Date for the filter"></telerik:RadDateInput></td>
                                <td class="formCellField"><asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ControlToValidate="dteEndDate" ValidationGroup="grpRefresh" ErrorMessage="Please enter an End Date"></asp:RequiredFieldValidator></td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">Client</td>
                                <td class="formCellField"><telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                                    MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" AllowCustomText="False"
                                    ShowMoreResultsBox="false" Skin="WindowsXP" Width="355px" Height="300px"></telerik:RadComboBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">State</td>
                                <td class="formCellField">
                                    <asp:RadioButtonList ID="rblState" runat="server" RepeatDirection="Horizontal" RepeatColumns="2">
                                        <asp:ListItem Text="Invoiced Only" Value="INVOICED"></asp:ListItem>
                                        <asp:ListItem Text="All" Value="ALL" Selected="true"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">Nominal Codes</td>
                                <td class="formCellField">
                                    <asp:CheckBoxList ID="chkNominalCodes" runat="server" RepeatDirection="Horizontal" RepeatColumns="4" DataValueField="NominalCodeID" DataTextField="Description" />
                                </td>
                            </tr>
                       </table>
                    </td>
                </tr>
            </table>
        </fieldset>
        <div class="buttonbar">
            <asp:Button ID="btnRefresh" runat="server" Text="Refresh" ValidationGroup="grpRefresh" CausesValidation="true" />
        </div>
    </asp:Panel>

    <table id="tblSummary" runat="server"> 
        <tbody>
            <tr>
                <td>
                    <telerik:RadGrid ID="grdSummary" runat="server" AllowPaging="false" ShowGroupPanel="true" allowSorting="true" Skin="Office2007" EnableAJAX="true" AutoGenerateColumns="false" AllowMultiRowSelection="true" ShowFooter="true">
                        <MasterTableView DataKeyNames="Client" Width="100%">
                            <Columns>
                                <telerik:GridBoundColumn HeaderText="Client" DataField="Client" HeaderStyle-Width="200"></telerik:GridBoundColumn>
                                <telerik:GridBoundColumn HeaderText="Nominal Code" DataField="NominalCode" HeaderStyle-Width="200"></telerik:GridBoundColumn>
                                <telerik:GridBoundColumn HeaderText="Description" DataField="NominalCodeDescription" HeaderStyle-Width="200"></telerik:GridBoundColumn>
                                <telerik:GridTemplateColumn HeaderText="Total" HeaderStyle-Width="135">
                                    <ItemTemplate>
                                        <span id="spnCharge" onclick=""><%#((System.Data.DataRowView)Container.DataItem)["Total"].ToString().Length < 2 ? "&nbsp;" : ((decimal)((System.Data.DataRowView)Container.DataItem)["Total"]).ToString("C")%></span>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="lblTotalUninvoiced" runat="server" />
                                    </FooterTemplate>
                                    <ItemStyle HorizontalAlign="Right" />
                                    <FooterStyle HorizontalAlign="Right" />
                                </telerik:GridTemplateColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </td>
            </tr>
        </tbody>
    </table>
    
</asp:Content>

