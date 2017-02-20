<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/default_tableless.Master" CodeBehind="palletNetworkExportv2.aspx.cs" Inherits="Orchestrator.WebUI.Groupage.palletNetworkExportv2" %>

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
            <tr>
                <td colspan="2"><asp:RadioButtonList ID="rboFilterOption" runat="server" RepeatDirection="Horizontal" AutoPostBack="true">
                    <asp:ListItem Text="Show orders to be exported." Value="0" ></asp:ListItem>
                    <asp:ListItem Text="Show orders to export and those exported." Value="1"></asp:ListItem>
                    <asp:ListItem Text="Only show orders already exported." Value="2"></asp:ListItem>
                </asp:RadioButtonList>
                </td>
            </tr>
     </table>
     </fieldset>
     
     
     <telerik:RadGrid runat="server" ID="grdOrders" AutoGenerateColumns="false" Width="100%" sort>
        <MasterTableView CommandItemDisplay=Top ShowFooter="true" DataKeyNames="OrderId" EditMode="EditForms" >
        <SortExpressions>
            <telerik:GridSortExpression FieldName="JobId" SortOrder="Ascending" />
        </SortExpressions>  
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
                        <td>
                            <div style="background-color: LightBlue;">
                                Failed Validation</div>
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
                <telerik:GridTemplateColumn HeaderText="Run&nbsp;Id" SortExpression="JobId" HeaderStyle-Width="30px" >
                    <ItemTemplate>
                         <a href="javascript:ViewRun(<%#Eval("JobID") %>);"><%#Eval("JobID")%></a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Order&nbsp;Id" SortExpression="OrderID" HeaderStyle-Width="30px" >
                    <ItemTemplate>
                        <a href="javascript:ViewOrderProfile(<%#Eval("OrderID") %>);"><%#Eval("OrderID") %></a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Client" ReadOnly="true" SortExpression="CustomerOrganisationName"  DataField="CustomerOrganisationName" UniqueName="Customer">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Deliver Where" SortExpression="DeliveryPointDescription"
                    DataField="DeliveryPointDescription" ItemStyle-Wrap="false" UniqueName="DeliveryPointDescription" HeaderStyle-Width="120">
                </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn HeaderText="Deliver When" SortExpression="DeliveryDateTime" UniqueName="DeliveryDateTime">
                    <ItemTemplate>
                    <asp:Label id="lblDeliverAt" runat="server"></asp:Label>
                    </ItemTemplate> 
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Refs" SortExpression="DeliveryOrderNumber"
                    DataField="DeliveryOrderNumber" UniqueName="Refs">
                 <ItemTemplate>
                       <%#Eval("DeliveryOrderNumber") %> </br> <%#Eval("CustomerOrderNumber") %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn DataField="NoPallets" FooterText="Plts<br/>"
                    FooterStyle-Wrap="false" HeaderText="No Pallets" FooterStyle-Font-Bold="true" ItemStyle-Width="25"
                    HeaderStyle-Width="25" FooterStyle-Width="25" UniqueName="NoPallets" EditFormColumnIndex="0">
                    <ItemTemplate>
                        <telerik:RadNumericTextBox ID="txtNoPallets" Width="25" runat="server" Type="Number" NumberFormat-DecimalDigits="0" Value='<%#Convert.ToInt32(DataBinder.Eval(Container.DataItem, "NoPallets"))%>'>                       </telerik:RadNumericTextBox>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:Label ID="lblpalletstotal" runat="server"></asp:Label>
                    </FooterTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn DataField="PalletSpaces" FooterStyle-Wrap="false" FooterStyle-Font-Bold="true"
                    HeaderText="Spaces" ItemStyle-Width="25" HeaderStyle-Width="25" FooterStyle-Width="25" UniqueName="PalletSpaces" EditFormColumnIndex="0">
                    <ItemTemplate>
                         <telerik:RadNumericTextBox ID="txtPalletSpaces" Width="25" runat="server" NumberFormat-DecimalDigits="0" Value='<%#Convert.ToInt32(DataBinder.Eval(Container.DataItem, "PalletSpaces"))%>'>
                        </telerik:RadNumericTextBox>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:Label ID="lblspacestotal" runat="server"></asp:Label>
                    </FooterTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn DataField="Weight" HeaderText="Kgs" FooterStyle-Wrap="false"
                    FooterStyle-Font-Bold="true" 
                    ItemStyle-Width="25" HeaderStyle-Width="25" FooterStyle-Width="25" UniqueName="Weight" EditFormColumnIndex="0">
                    <ItemTemplate>
                        <telerik:RadNumericTextBox ID="txtWeight" Width="60" NumberFormat-DecimalDigits="0" runat="server" Value='<%#Convert.ToInt32(DataBinder.Eval(Container.DataItem, "Weight"))%>'>
                        </telerik:RadNumericTextBox>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:Label ID="lblweighttotal" runat="server"></asp:Label>
                    </FooterTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn DataField="QtrPallets" HeaderText="1/4" 
                    FooterStyle-Wrap="false" FooterStyle-Font-Bold="true"
                    ItemStyle-Width="25" HeaderStyle-Width="25"
                    FooterStyle-Width="25" UniqueName="QtrPallets" EditFormColumnIndex="1">
                    <ItemTemplate>
                         <telerik:RadNumericTextBox ID="txtQtrPallets" Width="25" NumberFormat-DecimalDigits="0" runat="server" Value='<%#Convert.ToInt32(DataBinder.Eval(Container.DataItem, "QtrPallets"))%>'>
                        </telerik:RadNumericTextBox>
                    </ItemTemplate>
                    
                    <FooterTemplate>
                        <asp:Label ID="lblQtrPalletsTotal" runat="server"></asp:Label>
                    </FooterTemplate>                
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn DataField="HalfPallets" HeaderText="1/2" 
                    FooterStyle-Wrap="false" FooterStyle-Font-Bold="true"
                    ItemStyle-Width="25" HeaderStyle-Width="25"
                    FooterStyle-Width="25" UniqueName="HalfPallets" EditFormColumnIndex="1">
                    <ItemTemplate>
                         <telerik:RadNumericTextBox ID="txtHalfPallets" Width="25" NumberFormat-DecimalDigits="0" runat="server" Value='<%#Convert.ToInt32(DataBinder.Eval(Container.DataItem, "HalfPallets"))%>'>
                        </telerik:RadNumericTextBox>
                    </ItemTemplate>
                  
                    <FooterTemplate>
                        <asp:Label ID="lblHalfPalletsTotal" runat="server"></asp:Label>
                    </FooterTemplate>                  
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn DataField="FullPallets" HeaderText="Full" FooterStyle-Wrap="false" 
                    FooterStyle-Font-Bold="true" ItemStyle-Width="25" HeaderStyle-Width="25"
                    FooterStyle-Width="25" UniqueName="FullPallets" EditFormColumnIndex="1">
                    <ItemTemplate>
                        <telerik:RadNumericTextBox ID="txtFullPallets" Width="25" NumberFormat-DecimalDigits="0" runat="server" Value='<%#Convert.ToInt32(DataBinder.Eval(Container.DataItem, "FullPallets"))%>'>
                        </telerik:RadNumericTextBox>
                    </ItemTemplate>
                   
                    <FooterTemplate>
                        <asp:Label ID="lblFullPalletsTotal" runat="server"></asp:Label>
                    </FooterTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn DataField="OverPallets" HeaderText="O/S" FooterStyle-Wrap="false" FooterStyle-Font-Bold="true"
                    ItemStyle-Width="25" HeaderStyle-Width="25" FooterStyle-Width="25" UniqueName="OverPallets" EditFormColumnIndex="1">
                    <ItemTemplate>
                        <telerik:RadNumericTextBox ID="txtOverPallets" Width="25" NumberFormat-DecimalDigits="0" runat="server" Value='<%#Convert.ToInt32(DataBinder.Eval(Container.DataItem, "OverPallets"))%>'>
                        </telerik:RadNumericTextBox>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:Label ID="lblOverPalletsTotal" runat="server"></asp:Label>
                    </FooterTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="surcharges" HeaderText="Surcharges">
                    <ItemTemplate>
                        <asp:CheckBoxList ID="cblSurcharges" runat="server" RepeatDirection="Horizontal" RepeatColumns="3"></asp:CheckBoxList>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="actions">
                    <ItemTemplate>
                        <asp:LinkButton is="lnkUpdate" runat="server" CommandName="update" Text="update"></asp:LinkButton>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
    
    <telerik:RadAjaxManager ID="raxManager" runat="server">
    <AjaxSettings>
        <telerik:AjaxSetting AjaxControlID="rboFilterOption">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="grdOrders" LoadingPanelID="loadingPanel" />
            </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="btnSearch">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="grdOrders" LoadingPanelID="loadingPanel" />
            </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="btnExport">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="grdOrders" LoadingPanelID="loadingPanel" />
            </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="grdOrders">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="grdOrders" LoadingPanelID="loadingPanel" />
            </UpdatedControls>
        </telerik:AjaxSetting>
    </AjaxSettings>
 
</telerik:RadAjaxManager>
  <telerik:RadAjaxLoadingPanel ID="loadingPanel" runat="server" Transparency="10" BackColor="#d3e5eb">
        <table width='100%' cellpadding='100px;' height='70%'>
            <tr>
                <td align="center" valign="top">
                    <img src='/images/postbackLoading.gif' />
                </td>
            </tr>
        </table>
    </telerik:RadAjaxLoadingPanel>
 
 <telerik:RadCodeBlock runat="server">
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
    </telerik:RadCodeBlock>
</asp:Content>
