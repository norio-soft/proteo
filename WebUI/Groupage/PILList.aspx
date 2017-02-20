<%@ Page Language="C#" MasterPageFile="~/default_tableless.master" AutoEventWireup="true"
    Inherits="Groupage_PILList" Title="PIL List" CodeBehind="PILList.aspx.cs" %>

<%@ Register Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI"
    TagPrefix="cc1" %>
<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>
        PIL List</h1>
</asp:Content>
<asp:Content ContentPlaceHolderID="Header" runat="server">
    <cc1:Dialog runat="server" ID="dlgOrder" ReturnValueExpected="true" AutoPostBack="true"
        URL="/groupage/manageorder.aspx" Height="900" Width="1200">
    </cc1:Dialog>

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
            var btn = document.getElementById("<%=btnPIL.ClientID %>")
            btn.disabled = false;
        }

        function RowDeSelected(row) {
            totalSelected--;
            var btn = document.getElementById("<%=btnPIL.ClientID %>")
            btn.disabled = totalSelected < 1;
        }

        function validateBusinessType(sender, eventArgs) {
            var firstItem = $("#" + '<%=cboBusinessType.ClientID %>');
            var result = false;
            var value = parseInt(firstItem.val(), 10);

            if (!isNaN(value) && value > 0)
                result = true;

            eventArgs.IsValid = result;
            return eventArgs;
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

</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
                <div class="ToolbarBlue" style="height: 24px; padding: 1px 1px 1px 3px; background-position: top;">
		    <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()" style="display: none;">Show filter Options</div>
		    <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">Close filter Options</div>
                    <asp:Button ID="btnPIL" runat="server" Text="Print PIL" Width="100" Enabled="false" />
        <asp:Button ID="btnCreateDeliveryNote" runat="server" Text="Delivery Note" />
        <asp:Button ID="btnPodLabel" runat="server" Text="Pod Label" />
	    </div>
        <!--Hidden Filter Options-->
        <div class="overlayedFilterBox" id="overlayedClearFilterBox" style="display: block;">
    <fieldset>
        <legend>Filter Options</legend>
        <table>
            <tr>
                <td class="formCellLabel">
                    From Date
                </td>
                <td class="formCellField" colspan="2">
                    <telerik:RadDatePicker ID="dteStartDeliveryDate" runat="server" Width="100px" >
                    <DateInput runat="server"
                    DateFormat="dd/MM/yy">
                    </DateInput>
                    </telerik:RadDatePicker>
                    <asp:RequiredFieldValidator runat="server" ID="rfvStartDeliveryDate" ControlToValidate="dteStartDeliveryDate"
                        ValidationGroup="grpGenerate"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">
                    To Date
                </td>
                <td class="formCellField">
                    <telerik:RadDatePicker ID="dteEndDeliveryDate" runat="server" Width="100px" >
                    <DateInput runat="server"
                    DateFormat="dd/MM/yy">
                    </DateInput>
                    </telerik:RadDatePicker>
                    <asp:RequiredFieldValidator runat="server" ID="rfvEndDeliveryDate" ControlToValidate="dteEndDeliveryDate"
                        ValidationGroup="grpGenerate"></asp:RequiredFieldValidator>
                </td>
                <td>
                    <asp:RadioButtonList ID="cboSearchAgainstDate" runat="server" RepeatDirection="horizontal">
                            <asp:ListItem Text="Collection Date" Value="COL"></asp:ListItem>
                            <asp:ListItem Text="Delivery Date" Value="DEL"></asp:ListItem>
                            <asp:ListItem Text="Collection and Delivery Dates" Selected="true" Value="BOTH"></asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">
                    Business Type
                </td>
                <td class="formCellField">
                    <asp:DropDownList ID="cboBusinessType" runat="server" DataValueField="BusinessTypeID"
                        DataTextField="Description">
                    </asp:DropDownList>
                    <asp:CustomValidator ID="cfvBusinessType" runat="server" ValidationGroup="grpGenerate"
                        ControlToValidate="cboBusinessType" ErrorMessage="Please select a business type for this order."
                        Display="Dynamic" ClientValidationFunction="validateBusinessType">
                        <img src="/App_Themes/Orchestrator/Img/MasterPage/icon-warning.png" alt="Please select a business type for this order." />
                    </asp:CustomValidator>
                </td>
            </tr>
        </table>
    </fieldset>
    <div class="buttonbar">
        <asp:Button ID="btnGenerate" runat="server" Text="Generate List" Width="100" ValidationGroup="grpGenerate" />

    </div>
    </div>
    
    <asp:HiddenField runat="server" ID="hidLastSelectedBusinessTypeId" />
    <telerik:RadGrid runat="server" ID="grdLoadOrder" AutoGenerateColumns="false" AllowSorting="true"
        AllowMultiRowSelection="true">
        <MasterTableView DataKeyNames="OrderID">
            <Columns>
                <telerik:GridClientSelectColumn UniqueName="checkboxSelectColumn" HeaderStyle-HorizontalAlign="Left"
                    HeaderStyle-Width="40" HeaderText="">
                </telerik:GridClientSelectColumn>
                <telerik:GridTemplateColumn HeaderText="Order&nbsp;Id" SortExpression="OrderID" HeaderStyle-Width="80px">
                    <ItemTemplate>
                        <a runat="server" ID="hypOrderId"></a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Order Number" DataField="CustomerOrderNumber">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Delivery Order Number" DataField="DeliveryOrderNumber">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Pallets" DataField="NoPallets">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Account Code" ItemStyle-Width="110px" DataField="AccountCode">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Customer" DataField="CustomerOrganisationName">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Destination" DataField="DeliveryPointDescription">
                </telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings>
            <Selecting AllowRowSelect="true" />
            <ClientEvents OnRowSelected="RowSelected" OnRowDeselected="RowDeSelected" />
            <Resizing AllowColumnResize="true" AllowRowResize="false" EnableRealTimeResize="false" ResizeGridOnColumnResize="true" ClipCellContentOnResize="true" />
        </ClientSettings>
    </telerik:RadGrid>
    <script type="text/javascript">
        FilterOptionsDisplayHide();
    </script>
</asp:Content>
