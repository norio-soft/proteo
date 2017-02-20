<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/default_tableless_client.master" CodeBehind="ClientOrderList.aspx.cs" Inherits="Orchestrator.WebUI.Client.ClientOrderList" MaintainScrollPositionOnPostback="true" %>
    
<asp:Content ID="Content2" ContentPlaceHolderID="Header" runat="server"></asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Your Orders</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<telerik:RadCodeBlock runat="server" ID="rdc1">
    <script type="text/javascript" language="javascript">
        var ichkCount = 0;
        function SetPrintPILButtonState(el) {
            var btn = document.getElementById("<%=btnPIL.ClientID %>")
            if (el.checked) ichkCount++;
            else ichkCount--;

            if (ichkCount < 0) ichkCount = 0;

            if (ichkCount > 0) {
                btn.disabled = false;
            }
            else {
                btn.disabled = true;
            }
        }

        var totalSelected = 0;

        function RowSelected(row) {
            totalSelected++;
            var btn = document.getElementById("<%=btnPIL.ClientID %>");
            btn.disabled = !EnablePrinting();
        }

        function RowDeSelected(row) {
            totalSelected--;
            var btn = document.getElementById("<%=btnPIL.ClientID %>");
            btn.disabled = !EnablePrinting();
        }

        function EnablePrinting() {
            var grdOrders = $find("<%=grdOrders.ClientID %>");
            var gridSelectedItems = grdOrders.get_masterTableView().get_selectedItems();

            if (gridSelectedItems.length == 0)
                return false;

            var index = 0;
            var result = false;
            var reportType = 0;

            for (index = 0; index < gridSelectedItems.length; index++) {
                var isPalletNetwork = gridSelectedItems[index].getDataKeyValue("IsPalletNetwork");

                if (index == 0) {
                    result = true;
                    reportType = isPalletNetwork;
                }
                else {
                    result = (reportType == isPalletNetwork);
                }
            }

            return result;
        }

        // Function to show the filter options overlay box
        function FilterOptionsDisplayShow() {
            $("#overlayedClearFilterBox").css({ 'display': 'block' });
            $("#filterOptionsDiv").css({ 'display': 'none' });
            $("#filterOptionsDivHide").css({ 'display': 'block' });
        }

        function FilterOptionsDisplayHide() {
            $("#overlayedClearFilterBox").css({ 'display': 'none' });
            $("#filterOptionsDivHide").css({ 'display': 'none' });
            $("#filterOptionsDiv").css({ 'display': 'block' });
        }
        
    </script>
    </telerik:RadCodeBlock>
    <telerik:RadAjaxManager ID="ramClientOrderList" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="cboOrderStatus">
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
    <h2>
        This is a list of your orders with us. You can filter your orders by specifying information in the filter options section below and then clicking the <b>Filter</b> button.
    </h2>

    <div class="ToolbarBlue" style="height: 24px; padding: 1px 1px 1px 3px; background-position: top;">
		    <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()" style="display: none;">Show filter Options</div>
		    <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">Close filter Options</div>
            <asp:Button ID="btnPIL" runat="server" Text="Print PIL" Width="100" Enabled="false" />
	    </div>
        <!--Hidden Filter Options-->
        <div class="overlayedFilterBoxBelowText" id="overlayedClearFilterBox" style="display: block;">
    <fieldset>
        <legend>Filter Options</legend>
        I want to see my orders that have the status of
        &nbsp;
        <telerik:RadComboBox ID="cboOrderStatus" runat="server" AutoPostBack="false"/>
        &nbsp;
        with a
        <asp:RadioButton runat="server" GroupName="rbDate" ID="rbCollection" Checked="true" Text="collection date" />
        <asp:RadioButton runat="server" GroupName="rbDate" ID="rbDelivery" Text="delivery date" />
        between:
        &nbsp;
        <telerik:RadDatePicker runat="server" ID="dteOrderFilterDateFrom"  Width="100">
        <DateInput runat ="server" DateFormat="dd/MM/yy" DisplayDateFormat="dd/MM/yy">
        </DateInput>
        </telerik:RadDatePicker>
        <asp:RequiredFieldValidator ID="rfvOrderFilterDateFrom" runat="server" ControlToValidate="dteOrderFilterDateFrom" Display="Dynamic" ErrorMessage="Please enter a start date.">
            <img id="Img1" runat="server" src="~/images/error.png" title="Please enter a start date." alt="" />
        </asp:RequiredFieldValidator>
        &nbsp;and&nbsp;
        <telerik:RadDatePicker runat="server" ID="dteOrderFilterDateTo" Width="100">
        <DateInput runat="server" DateFormat="dd/MM/yy" DisplayDateFormat="dd/MM/yy"></DateInput>
        </telerik:RadDatePicker>
        <asp:RequiredFieldValidator ID="rfvOrderFilterDateTo" runat="server" ControlToValidate="dteOrderFilterDateTo" Display="Dynamic" ErrorMessage="Please enter an end date.">
            <img id="Img2" runat="server" src="~/images/error.png" title="Please enter an end date." alt="" />
        </asp:RequiredFieldValidator>
    </fieldset>
      <div class="buttonbar">
        <input type="button" style="vertical-align:bottom;" runat="server" id="btnFilter" value="Filter" />
        
    </div>
    </div>
  
    
    <telerik:RadGrid OnInit="grdOrders_Init" runat="server" ID="grdOrders" AllowFilteringByColumn="false"
        AllowPaging="false" AllowSorting="true" AllowMultiRowSelection="true"
        AutoGenerateColumns="false" ShowStatusBar="false">
        <ClientSettings AllowDragToGroup="true" AllowColumnsReorder="true">
            <Selecting AllowRowSelect="True" EnableDragToSelectRows="true"></Selecting>
        </ClientSettings>
        <MasterTableView DataKeyNames="OrderID,IsPalletNetwork" ClientDataKeyNames="IsPalletNetwork">
            <RowIndicatorColumn Display="false">
            </RowIndicatorColumn>
            <Columns>
                <telerik:GridClientSelectColumn UniqueName="checkboxSelectColumn" HeaderStyle-HorizontalAlign="Left"
                    HeaderStyle-Width="40" HeaderText=""></telerik:GridClientSelectColumn>
                <telerik:GridTemplateColumn UniqueName="OrderId" HeaderText="Order Id" DataField="OrderId"
                    SortExpression="OrderId" AllowFiltering="true">
                    <ItemTemplate>
                        <asp:HyperLink runat="server" ID="hypOrderId"></asp:HyperLink>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="OrderStatusId" HeaderText="Status" DataField="OrderStatusId"
                    SortExpression="OrderStatusId" AllowFiltering="false">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblOrderStatus"></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn UniqueName="LoadNo" DataField="CustomerOrderNumber" HeaderText="Load No"
                    AllowFiltering="true" />
                <telerik:GridTemplateColumn UniqueName="CollectFrom" HeaderText="Collect From">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblCollectFromPoint"></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn UniqueName="CollectAt" DataField="CollectionDateTime" HeaderText="Collect At"
                    AllowFiltering="true" />
                <telerik:GridTemplateColumn UniqueName="DeliverTo" HeaderText="Deliver To">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblDeliverToPoint"></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn UniqueName="DeliverAt" DataField="DeliveryDateTime" HeaderText="Deliver At"
                    AllowFiltering="true" />
                <telerik:GridBoundColumn UniqueName="DeliveryOrderNumber" DataField="DeliveryOrderNumber"
                    HeaderText="Delivery Order Number" AllowFiltering="true" />
                <telerik:GridBoundColumn HeaderText="Delivering Resource" SortExpression="DeliveringResource"
                    DataField="DeliveringResource" UniqueName="DeliveringResource" />
                <telerik:GridBoundColumn UniqueName="NoOfPallets" DataField="NoPallets" HeaderText="No of Pallets"
                    ItemStyle-HorizontalAlign="Right" AllowFiltering="true" />
                <telerik:GridTemplateColumn UniqueName="Links" HeaderText="Links" AllowFiltering="true">
                    <ItemTemplate>
                        <asp:HyperLink runat="server" ID="hypInvoice"></asp:HyperLink><br />
                        <asp:HyperLink runat="server" ID="hypPod"></asp:HyperLink>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="Rate" HeaderText="Rate" SortExpression="Rate"
                    AllowFiltering="true">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblRate"></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn UniqueName="IsPalletNetwork" DataField="IsPalletNetwork" Display="false" >
                </telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings>
            <Selecting AllowRowSelect="true" />
            <ClientEvents OnRowSelected="RowSelected" OnRowDeselected="RowDeSelected" />
        </ClientSettings>
    </telerik:RadGrid>
    <br />

    <script type="text/javascript">
        FilterOptionsDisplayHide()
    </script>
</asp:Content>
