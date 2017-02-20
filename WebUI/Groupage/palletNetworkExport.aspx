<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/default_tableless.Master" CodeBehind="palletNetworkExport.aspx.cs" Inherits="Orchestrator.WebUI.Groupage.palletNetworkExport" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>Pallet Network Export</h1>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script type="text/javascript" src="/script/jquery-ui-min.js"></script>
    <script type="text/javascript" src="/script/jquery.blockUI-2.64.0.min.js"></script>
    <h1>Pallet Network Export</h1>
    <h2></h2>
    <fieldset>
     <table>
            <tr>
                <td class="formCellLabel">
                    Trunk From
                </td>
                <td class="formCellInput">
                    <telerik:RadDateInput DateFormat="dd/MM/yy" DisplayDateFormat="dd/MM/yy" Width="55"
                        runat="server" ID="dteTrunkDate">
                    </telerik:RadDateInput>
                        <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="buttonClassSmall" />
                        <asp:Button ID="btnExport" runat="server" Text="Export" CssClass="buttonClassSmall" />
                </td>
            </tr>
     </table>
     </fieldset>
     <telerik:RadGrid runat="server" ID="grdOrders" AutoGenerateColumns="false" Width="100%">
        <MasterTableView CommandItemDisplay=Top ShowFooter="true" DataKeyNames="OrderId" EditMode="EditForms">
            <CommandItemTemplate>
                <table cellpadding="0" cellspacing="0" class="keyTable">
                    <tr>
                        <td>
                            Row colour key:
                        </td>
                        <td>
                            <div style="background-color: Pink;">
                                Export Unprocessed</div>
                        </td>
                        <td>
                            <div style="background-color: Violet;">
                                Export Processed</div>
                        </td>
                        <td>
                            <div style="background-color: Red;">
                                Export Errored</div>
                        </td>
                    </tr>
                </table>
            </CommandItemTemplate>
            <EditFormSettings ColumnNumber="2">
                <FormTableItemStyle Wrap="False" Width="100%"></FormTableItemStyle>
                <FormMainTableStyle GridLines="Horizontal" CellSpacing="0" CellPadding="3" Width="100%" />
                <FormTableStyle GridLines="None" CellSpacing="0" CellPadding="2" BackColor="white"
                    Width="100%" />
                <FormTableAlternatingItemStyle Wrap="False"></FormTableAlternatingItemStyle>
                <EditColumn ButtonType="LinkButton" InsertText="Insert" UpdateText="Update" UniqueName="EditCommandColumn1"
                    CancelText="Cancel">
                </EditColumn>
                <FormTableButtonRowStyle BackColor="#FFFFE7" HorizontalAlign="Left"></FormTableButtonRowStyle>
            </EditFormSettings>
            <Columns>
                <telerik:GridTemplateColumn HeaderStyle-Width="20" HeaderText="">
                    <HeaderTemplate>
                        <input type="checkbox" onClick="javascript:HandleSelectAll(this);" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:CheckBox ID="chkSelectOrder" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridEditCommandColumn UniqueName="EditLink" Visible="true">
                    <ItemStyle Width="20px" />
                </telerik:GridEditCommandColumn>
                <telerik:GridTemplateColumn HeaderText="Run&nbsp;Id" SortExpression="JobId" HeaderStyle-Width="30px" >
                    <ItemTemplate>
                        <asp:HyperLink runat="server" ID="hypRun" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Order&nbsp;Id" SortExpression="OrderID" HeaderStyle-Width="30px" >
                    <ItemTemplate>
                        <asp:HyperLink runat="server" ID="hypUpdateOrder" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Client" ReadOnly="true" SortExpression="CustomerOrganisationName" ItemStyle-Width="120px" DataField="CustomerOrganisationName" UniqueName="Customer">
                </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn FooterStyle-Font-Bold="true" ReadOnly="true" HeaderText="Rate" SortExpression="ForeignRate" ItemStyle-Width="30px" UniqueName="Rate" EditFormColumnIndex="0">
                    <ItemTemplate>
                        <asp:Label ID="lblRate" runat="server" ></asp:Label>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:Label ID="lblRateTotal" runat="server"></asp:Label>
                    </FooterTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn FooterStyle-Font-Bold="true" ReadOnly="true" HeaderText="Subcontract Cost" SortExpression="SubContractRate" ItemStyle-Width="30px" UniqueName="SubContractRate">
                    <ItemTemplate>
                        <asp:Label ID="lblSubContractCost" runat="server"></asp:Label>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:Label ID="lblSubContractCostTotal" runat="server"></asp:Label>
                    </FooterTemplate>                    
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn FooterStyle-Font-Bold="true" ReadOnly="true" HeaderText="Hub Charge" SortExpression="HubCharge" ItemStyle-Width="30px" UniqueName="HubCharge">
                    <ItemTemplate>
                        <asp:Label ID="lblHubCharge" runat="server"></asp:Label>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:Label ID="lblHubChargeTotal" runat="server"></asp:Label>
                    </FooterTemplate>  
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn DataField="NoPallets" FooterText="Plts<br/>"
                    FooterStyle-Wrap="false" HeaderText="No Pallets" FooterStyle-Font-Bold="true" ItemStyle-Width="25"
                    HeaderStyle-Width="25" FooterStyle-Width="25" UniqueName="NoPallets" EditFormColumnIndex="0">
                    <ItemTemplate>
                        <asp:Label ID="lblNoPallets" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "NoPallets")%>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <telerik:RadNumericTextBox ID="txtNoPallets" runat="server" Type="Number" NumberFormat-DecimalDigits="0" Value='<%#Convert.ToInt32(DataBinder.Eval(Container.DataItem, "NoPallets"))%>'>
                        </telerik:RadNumericTextBox>
                    </EditItemTemplate> 
                    <FooterTemplate>
                        <asp:Label ID="lblpalletstotal" runat="server"></asp:Label>
                    </FooterTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn DataField="PalletSpaces" FooterStyle-Wrap="false" FooterStyle-Font-Bold="true"
                    HeaderText="Spaces" ItemStyle-Width="25" HeaderStyle-Width="25" FooterStyle-Width="25" UniqueName="PalletSpaces" EditFormColumnIndex="0">
                    <ItemTemplate>
                        <asp:Label ID="lblPalletSpaces" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "PalletSpaces")%>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <telerik:RadNumericTextBox ID="txtPalletSpaces" runat="server" NumberFormat-DecimalDigits="0" Value='<%#Convert.ToInt32(DataBinder.Eval(Container.DataItem, "PalletSpaces"))%>'>
                        </telerik:RadNumericTextBox>
                    </EditItemTemplate> 
                    <FooterTemplate>
                        <asp:Label ID="lblspacestotal" runat="server"></asp:Label>
                    </FooterTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn DataField="Weight" HeaderText="Kgs" FooterStyle-Wrap="false"
                    FooterStyle-Font-Bold="true" 
                    ItemStyle-Width="25" HeaderStyle-Width="25" FooterStyle-Width="25" UniqueName="Weight" EditFormColumnIndex="0">
                    <ItemTemplate>
                        <asp:Label ID="lblWeight" runat="server" Text='<%#Convert.ToInt32(DataBinder.Eval(Container.DataItem, "Weight"))%>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <telerik:RadNumericTextBox ID="txtWeight" NumberFormat-DecimalDigits="0" runat="server" Value='<%#Convert.ToInt32(DataBinder.Eval(Container.DataItem, "Weight"))%>'>
                        </telerik:RadNumericTextBox>
                    </EditItemTemplate> 
                    <FooterTemplate>
                        <asp:Label ID="lblweighttotal" runat="server"></asp:Label>
                    </FooterTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn DataField="QtrPallets" HeaderText="1/4" 
                    FooterStyle-Wrap="false" FooterStyle-Font-Bold="true"
                    ItemStyle-Width="25" HeaderStyle-Width="25"
                    FooterStyle-Width="25" UniqueName="QtrPallets" EditFormColumnIndex="1">
                    <ItemTemplate>
                        <asp:Label ID="lblQtrPallets" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "QtrPallets")%>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <telerik:RadNumericTextBox ID="txtQtrPallets" NumberFormat-DecimalDigits="0" runat="server" Value='<%#Convert.ToInt32(DataBinder.Eval(Container.DataItem, "QtrPallets"))%>'>
                        </telerik:RadNumericTextBox>
                    </EditItemTemplate>    
                    <FooterTemplate>
                        <asp:Label ID="lblQtrPalletsTotal" runat="server"></asp:Label>
                    </FooterTemplate>                
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn DataField="HalfPallets" HeaderText="1/2" 
                    FooterStyle-Wrap="false" FooterStyle-Font-Bold="true"
                    ItemStyle-Width="25" HeaderStyle-Width="25"
                    FooterStyle-Width="25" UniqueName="HalfPallets" EditFormColumnIndex="1">
                    <ItemTemplate>
                        <asp:Label ID="lblHalfPallets" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "HalfPallets")%>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <telerik:RadNumericTextBox ID="txtHalfPallets" NumberFormat-DecimalDigits="0" runat="server" Value='<%#Convert.ToInt32(DataBinder.Eval(Container.DataItem, "HalfPallets"))%>'>
                        </telerik:RadNumericTextBox>
                    </EditItemTemplate>  
                    <FooterTemplate>
                        <asp:Label ID="lblHalfPalletsTotal" runat="server"></asp:Label>
                    </FooterTemplate>                  
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn DataField="FullPallets" HeaderText="Full" FooterStyle-Wrap="false" 
                    FooterStyle-Font-Bold="true" ItemStyle-Width="25" HeaderStyle-Width="25"
                    FooterStyle-Width="25" UniqueName="FullPallets" EditFormColumnIndex="1">
                    <ItemTemplate>
                        <asp:Label ID="lblFullPallets" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "FullPallets")%>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <telerik:RadNumericTextBox ID="txtFullPallets" NumberFormat-DecimalDigits="0" runat="server" Value='<%#Convert.ToInt32(DataBinder.Eval(Container.DataItem, "FullPallets"))%>'>
                        </telerik:RadNumericTextBox>
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:Label ID="lblFullPalletsTotal" runat="server"></asp:Label>
                    </FooterTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn DataField="OverPallets" HeaderText="O/S" FooterStyle-Wrap="false" FooterStyle-Font-Bold="true"
                    ItemStyle-Width="25" HeaderStyle-Width="25" FooterStyle-Width="25" UniqueName="OverPallets" EditFormColumnIndex="1">
                    <ItemTemplate>
                        <asp:Label ID="lblOverPallets" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "OverPallets")%>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <telerik:RadNumericTextBox ID="txtOverPallets" NumberFormat-DecimalDigits="0" runat="server" Value='<%#Convert.ToInt32(DataBinder.Eval(Container.DataItem, "OverPallets"))%>'>
                        </telerik:RadNumericTextBox>
                    </EditItemTemplate>
                    <FooterTemplate>
                        <asp:Label ID="lblOverPalletsTotal" runat="server"></asp:Label>
                    </FooterTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
    <script language="javascript" type="text/javascript">
        function ViewOrderProfile(orderID) {
            var url = "/Groupage/ManageOrder.aspx?wiz=true&oID=" + orderID;

            var wnd = window.open(url,"Order", "width=1180, height=900, resizable=1, scrollbars=1");
        }

        function ViewRun(runId) {
            var url = "/job/job.aspx?jobId=" + runId + getCSID();

            window.open(url);
        }

        function HandleSelectAll(chk) {
            $(":checkbox[id$=chkSelectOrder]").each(function(chkIndex) { if (this.checked != chk.checked) this.click(); });
        }

        function HandleSelection(chk, rowIndex) {
            var mtv = $find("<%=grdOrders.ClientID %>").get_masterTableView();
            var dataItem = mtv.get_dataItems()[rowIndex].get_element();

            if (chk.checked) {
                mtv.selectItem(dataItem);
                dataItem.className = "SelectedRow_Orchestrator";
            }
            else {
                mtv.deselectItem(dataItem);
                dataItem.className = "GridRow_Orchestrator";
            }
        }
    </script>
</asp:Content>
