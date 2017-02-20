<%@ Page Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="True" Inherits="Orchestrator.WebUI.Groupage_findorder" Title="Find Order" CodeBehind="findorder.aspx.cs" %>

<%@ Register Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI" TagPrefix="cc1" %>
<%@ Register TagPrefix="p1" TagName="BusinessTypeCheckList" Src="~/UserControls/BusinessTypeCheckList.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>Find Order</h1>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <cc1:Dialog ID="dlgOrderShuffler" URL="Shuffler.aspx" Width="1230" Height="840" AutoPostBack="false"
        Mode="Normal" runat="server" ReturnValueExpected="false" Scrollbars="true">
    </cc1:Dialog>
        <cc1:Dialog ID="dlgDocumentWizard" URL="/scan/wizard/ScanOrUpload.aspx" Width="550"
    Height="550" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true">
    </cc1:Dialog>

    <style type="text/css">
        .checkboxList
        {
            margin-left: -3px;
        }
        .RadPicker
        {
            width: 100px !important;
            display: inline-block !important;
        }      
    </style>

    <script type="text/javascript" src="/script/jquery-ui-1.9.2.min.js"></script>
    <script type="text/javascript" src="/script/jquery.blockUI-2.64.0.min.js"></script>
    <script type="text/javascript" src="/script/jquery.quicksearch-1.3.1.js"></script>

    <script lang="javascript" type="text/javascript">
        var shufflerUrl = 'manageorder.aspx?oid=|||&rid=|||';
        var userName = "<%=((Page.User) as Orchestrator.Entities.CustomPrincipal).UserName %>";

        $(function () {
            $('#columnDisplayAccordion').accordion({ collapsible: false, heightStyle: 'content' });

            $('#<%= grdOrders.ClientID %>_ctl00 tbody tr:not(.rgGroupHeader)').quicksearch({
                position: 'after',
                labelText: '',
                attached: '#grdFilterHolder',
                delay: 100
            });

            var $cblSearchFor = $('[id$=cblSearchFor]');

            $cblSearchFor.on('click', ':checkbox:first', function () {
                $cblSearchFor.find(':checkbox:not(:first)').prop('checked', this.checked);
            });

            $cblSearchFor.on('click', ':checkbox:not(:first)', function (a, b) {
                var allSelected = $cblSearchFor.find(':checkbox:not(:first):not(:checked)').length == 0;
                $cblSearchFor.find(':checkbox:first').prop('checked', allSelected);
            });
        });

        function updateOrder(orderID) {
            var url = "/Groupage/updateOrder.aspx?wiz=true&oID=" + orderID;
            var randomnumber = Math.floor(Math.random() * 11)
            var wnd = window.open(url, randomnumber, "width=1180, height=900, resizable=1, scrollbars=1");
        }

        function RowContextMenu(sender, eventArgs) {
            var menu = $find("<%=RadMenu1.ClientID %>");
            var evt = eventArgs.get_domEvent();

            var index = eventArgs.get_itemIndexHierarchical();
            document.getElementById("radGridClickedRowIndex").value = index;

            sender.get_masterTableView().selectItem(sender.get_masterTableView().get_dataItems()[index].get_element(), true);

            menu.show(evt);

            evt.cancelBubble = true;
            evt.returnValue = false;
            if (evt.stopPropagation) {
                evt.stopPropagation();
                evt.preventDefault();
            }

            orderID = sender.get_masterTableView().get_dataItems()[index].getDataKeyValue("OrderID");
        }

        function showLoading() {
            $.blockUI({ css: {
                border: 'none',
                padding: '15px',
                backgroundColor: '#000',
                '-webkit-border-radius': '10px',
                '-moz-border-radius': '10px',
                opacity: '.5',
                color: '#fff'
            }
            });
        }

        function hideLoading() {
            $.unblockUI();
        }

        function OnClick(sender, eventArgs) {
            var mnuCall = eventArgs.get_item().get_value();
            eventArgs.get_item().get_menu().hide();

            if (mnuCall == null || mnuCall == '')
                return;

            if (mnuCall > 0) {
                showLoading();
                PageMethods.UpdateBusinessType(orderID, mnuCall, userName, UpdateBusinessType_Success, UpdateBusinessType_Failure);
            }
            else {
                var url = "DeliveryNote.aspx?oID=" + orderID + "&JobID=0";

                var wnd = window.radopen("about:blank", "mediumWindow");
                wnd.SetUrl(url);
                wnd.SetTitle("Delivery Note:");
            }
        }

        function UpdateBusinessType_Success(result) {
            $get("<%=btnSearch.ClientID%>").click();
            hideLoading();
        }

        function UpdateBusinessType_Failure(error) {
            hideLoading();
            alert("There was an error changing the business type of the order, please try again.");
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

        // Function to display the column configure box 
        function ColumnDisplayShow() {
            $("#tabs").css({ 'display': 'none' });
            $("#dvColumnDisplay").css({ 'display': 'block' });
        }

        // Function to hide the column configure box 
        function ColumnDisplayHide() {
            $("#tabs").css({ 'display': 'block' });
            $("#dvColumnDisplay").css({ 'display': 'none' });
        }

        function OpenPODWindow(orderID) {
            var podFormType = 2;
            var qs = "ScannedFormTypeId=" + podFormType;
            qs += "&OrderID=" + orderID;

            <%=dlgDocumentWizard.ClientID %>_Open(qs);
        }
    </script>

    <input type="hidden" id="hidRecordIds" runat="server" />
    <input type="hidden" id="hidStartDate" runat="server" />
    <input type="hidden" id="hidEndDate" runat="server" />
    <input type="hidden" id="hidCollectionPointID" runat="server" />
    <input type="hidden" id="hidDeliveryPointID" runat="server" />
    <div class="overlayedFilterBox" id="overlayedClearFilterBox" style="display: block;">
        <fieldset>
            <table width="100%">
                <tr>
                    <td class="formCellLabel">
                        Search for
                    </td>
                    <td class="formCellInput">
                        <table>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtSearch" runat="server"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:CheckBoxList ID="cblSearchFor" runat="server" RepeatDirection="horizontal">
                                        <asp:ListItem Text="- all -" Value="ALL"></asp:ListItem>
                                        <asp:ListItem Text="Order ID" Value="ORDER" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Customer Order Number (Load No)" Value="LOADNO" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Delivery Order Number (Docket No)" Value="DELNO" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Custom Refs" Value="CUSTOM"></asp:ListItem>
                                        <asp:ListItem Text="Run ID" Value="RUNID"></asp:ListItem>
                                    </asp:CheckBoxList>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Date From
                    </td>
                    <td class="formCellInput">
                        <telerik:RadDatePicker  Width="100"
                            runat="server" ID="dteStartDate">
                            <DateInput runat="server"
                            DateFormat="dd/MM/yy" 
                            DisplayDateFormat="dd/MM/yy">
                            </DateInput>
                        </telerik:RadDatePicker>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Date To
                    </td>
                    <td>
                        <table style="margin-left: -3px;">
                            <tr>
                                <td class="formCellInput">
                                    <telerik:RadDatePicker  Width="100" runat="server" ID="dteEndDate">
                                        <DateInput runat="server" DateFormat="dd/MM/yy" DisplayDateFormat="dd/MM/yy"></DateInput>
                                    </telerik:RadDatePicker>
                                </td>
                                <td class="formCellInput">
                                    <asp:RadioButtonList ID="cboSearchAgainstDate" runat="server" RepeatDirection="horizontal">
                                        <asp:ListItem Text="Collection Date" Value="COL"></asp:ListItem>
                                        <asp:ListItem Text="Delivery Date" Value="DEL"></asp:ListItem>
                                        <asp:ListItem Text="Collection and Delivery Dates" Selected="true" Value="BOTH"></asp:ListItem>
                                        <asp:ListItem Text="Trunk Date" Value="TRUNK"></asp:ListItem>
                                        <asp:ListItem Text="Exported Date" Value="EXPORT"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Order Status
                    </td>
                    <td class="formCellInput" colspan="2">
                        <asp:CheckBoxList runat="server" ID="cblOrderStatus" RepeatDirection="horizontal" CssClass="checkboxList">
                        </asp:CheckBoxList>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Business Type
                    </td>
                    <td class="formCellField" colspan="2">
                        <p1:BusinessTypeCheckList ID="businessTypeCheckList" runat="server" ItemCountPerRow="6" />
                    </td>
                </tr>
                <tr id="trFilterAllocation" runat="server">
                    <td class="formCellLabel">
                        Allocation
                    </td>
                    <td class="formCellField" colspan="2">
                        <asp:RadioButtonList ID="rblAllocation" runat="server" RepeatDirection="Horizontal">
                            <asp:ListItem Value="0" Text="All" Selected="True" />
                            <asp:ListItem Value="1" Text="Unallocated only" />
                            <asp:ListItem Value="2" Text="Unallocated but previously allocated only" />
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <table>
                            <tr>
                                <td class="formCellLabel">
                                    Service Level
                                </td>
                                <td class="formCellInput" colspan="2">
                                    <telerik:RadComboBox ID="cboService" runat="server">
                                    </telerik:RadComboBox>
                                </td>
                                <td class="formCellLabel">
                                    Tracking Number
                                </td>
                                <td class="formCellInput" colspan="2">
                                    <asp:TextBox ID="txtTrackingNumber" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">
                                    Client
                                </td>
                                <td class="formCellInput" colspan="2">
                                    <telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                                        MarkFirstMatch="false" AllowCustomText="False" ShowMoreResultsBox="false" Width="355px"
                                        Height="300px" OnClientItemsRequesting="cboClient_itemsRequesting">
                                        <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetClients" />
                                    </telerik:RadComboBox>
                                </td>
                                <td class="formCellLabel">
                                    Collection Point
                                </td>
                                <td class="formCellInput" colspan="2">
                                    <telerik:RadComboBox ID="cboCollectionPointFilter" runat="server" EnableLoadOnDemand="true"
                                        ItemRequestTimeout="500" MarkFirstMatch="false" AutoPostBack="false" ShowMoreResultsBox="false"
                                        Width="390px" Height="300px" Overlay="true" AllowCustomText="True" HighlightTemplatedItems="true"
                                        CausesValidation="false" OnClientDropDownClosed="Point_CombBoxClosing">
                                        <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetPointsIncludeDeleted" />
                                    </telerik:RadComboBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">
                                    Resource
                                </td>
                                <td class="formCellInput" colspan="2">
                                    <telerik:RadComboBox ID="cboResource" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                                        MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" AllowCustomText="False"
                                        ShowMoreResultsBox="false" Skin="Orchestrator" SkinsPath="~/RadControls/ComboBox/Skins"
                                        Width="355px" Height="300px">
                                    </telerik:RadComboBox>
                                </td>
                                <td class="formCellLabel">
                                    Delivery Point
                                </td>
                                <td class="formCellInput" colspan="2">
                                    <telerik:RadComboBox ID="cboDeliveryPointFilter" runat="server" EnableLoadOnDemand="true"
                                        ItemRequestTimeout="500" MarkFirstMatch="false" AutoPostBack="false" ShowMoreResultsBox="false"
                                        Width="390px" Height="300px" Overlay="true" AllowCustomText="True" HighlightTemplatedItems="true"
                                        CausesValidation="false" OnClientDropDownClosed="Point_CombBoxClosing">
                                        <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetPointsIncludeDeleted" />
                                    </telerik:RadComboBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">
                                    Sub-Contractor
                                </td>
                                <td class="formCellInput" colspan="2">
                                    <telerik:RadComboBox ID="cboSubContractor" runat="server" EnableLoadOnDemand="true"
                                        ItemRequestTimeout="500" MarkFirstMatch="false" RadControlsDir="~/script/RadControls/"
                                        AllowCustomText="False" ShowMoreResultsBox="false" Width="355px" Height="300px">
                                        <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetSubContractors" />
                                    </telerik:RadComboBox>
                                </td>
                                <td class="formCellLabel">
                                    Goods Type
                                </td>
                                <td>
                                    <telerik:RadComboBox ID="cboGoodsType" runat="server" Skin="WindowsXP">
                                    </telerik:RadComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </fieldset>
        <div class="buttonbar">
            <asp:Button ID="btnSearch" runat="server" Text="Search" />
            <input type="button" id="Button1" runat="server" value="Cancel and close" onclick="FilterOptionsDisplayHide();" />
        </div>
    </div>
    <div class="overlayedClearDataBox" id="overlayedClearDataBox">
        <div id="dvColumnDisplay" class="ui-tabs ui-corner-all" style="display: none;">
            <fieldset>
                <legend>Display Columns</legend>
                <div>
                    <div id="columnDisplayAccordion">
                        <h3>Standard Columns</h3>
                        <div>
                            <asp:CheckBoxList ID="cblGridColumns" runat="server">
                                <asp:ListItem Text="Customer" Value="Customer" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Business Type" Value="BusinessType" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Trunk Date" Value="TrunkDate" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Rate" Value="Rate" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Extras" Value="Extras" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Order Group" Value="OrderGroupID" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Run ID" Value="RunId" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Subcontract Rate" Value="SubcontractRate" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="No Pallets" Value="NoPallets" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Pallet Spaces" Value="PalletSpaces" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Pallet Type" Value="PalletType" Selected="False"></asp:ListItem>
                                <asp:ListItem Text="Weight" Value="Weight" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Order Service Level" Value="OrderServiceLevel" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Customer Order Number" Value="CustomerOrderNumber" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Collection Point" Value="CollectionPointDescription" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Collection Date and Time" Value="CollectionDateTime" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Delivery Point" Value="DeliveryPointDescription" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Delivery Post Code" Value="DeliveryPostCode" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Delivery Date and Time" Value="DeliveryDateTime" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Delivery Order Number" Value="DeliveryOrderNumber" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Delivering Resource" Value="DeliveringResource" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Goods Type" Value="GoodsTypeDescription" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Surcharges" Value="Surcharges" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Quarter Pallets" Value="QtrPallets" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Half Pallets" Value="HalfPallets" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Full Pallets" Value="FullPallets" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Over Pallets" Value="OverPallets" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Trailer" Value="Trailer" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Vehicle" Value="Vehicle" Selected="True"></asp:ListItem>                    
                            </asp:CheckBoxList>
                        </div>
                        <h3>Organisation References</h3>
                        <div>
                            <asp:CheckBoxList runat="server" ID="cblOrganisationReferenceColumns" />
                            <div class="infoPanel">Hover over a column name to see the applicable clients.</div>
                        </div>
                    </div>
                </div>
            </fieldset>
            <div class="buttonBar">
                <asp:Button ID="btnChangeColumns" Text="Save Columns" runat="server" />
                <input type="button" id="Button2" runat="server" value="Cancel" onclick="ColumnDisplayHide();" />
            </div>
        </div>
    </div>
    <asp:Panel ID="pnlConfirmation" runat="server" Visible="false" EnableViewState="false">
        <div class="MessagePanel" style="vertical-align: middle;">
            <table>
                <tr>
                    <td>
                        <asp:Image ID="imgIcon" runat="server" ImageUrl="~/images/ico_info.gif" />
                    </td>
                    <td>
                        <asp:Label CssClass="ControlErrorMessage" ID="lblNote" runat="server" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <input type="hidden" id="radGridClickedRowIndex" name="radGridClickedRowIndex" />
    <telerik:RadGrid runat="server" ID="grdOrders" AllowPaging="false" ShowGroupPanel="false"
        ShowFooter="false" AllowSorting="true" AutoGenerateColumns="false" AllowMultiRowSelection="true"
        Width="100%" UseStaticHeaders="false">
        <MasterTableView ClientDataKeyNames="OrderID" DataKeyNames="OrderID" NoMasterRecordsText="Please click search to find orders"
            CommandItemDisplay="Top" GroupLoadMode="Client" ItemType="Orchestrator.Repositories.DTOs.FindOrderRow">
            <CommandItemTemplate>
                <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()"
                    style="display: none;">
                    Show filter Options</div>
                <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">
                    Close filter Options</div>
                <asp:Button ID="btnRefreshTop" runat="server" Text="Search" CssClass="buttonClassSmall"
                    OnClick="btnSearch_Click" />
                <asp:Button ID="btnExport" runat="server" Text="Export" CssClass="buttonClassSmall"
                    OnClick="btnExport_Click" />
                <asp:Button ID="btnGridSettings" runat="server" Text="Save Grid Layout" CssClass="buttonClassSmall"
                    OnClick="btnSaveGridSettings_Click" />
                <input type="button" id="btnColumnDisplayShow" runat="server" value="Configure Columns"
                    onclick="ColumnDisplayShow()" class="buttonClassSmall" />
                <div class="commandDataRow">
                    <span class="commandDataLabel">Pallets:</span><asp:Label ID="lblPalletsTotal" runat="server"
                        class="commandDataField"></asp:Label>
                    <span class="commandDataLabel">Spaces:</span><asp:Label ID="lblSpacesTotal" runat="server"
                        class="commandDataField"></asp:Label>
                    <span class="commandDataLabel">kgs:</span><asp:Label ID="lblWeightTotal" runat="server"
                        class="commandDataField"></asp:Label>
                    <span class="commandDataLabel">No of orders:</span><asp:Label ID="lblOrderCount"
                        runat="server" class="commandDataField"></asp:Label>
                    <span class="commandDataLabel">Quick filter:</span>
                    <div id="grdFilterHolder"></div>
                </div>
            </CommandItemTemplate>
            <RowIndicatorColumn Display="false">
            </RowIndicatorColumn>
            <Columns>
                <telerik:GridTemplateColumn HeaderText="Order&nbsp;ID" SortExpression="OrderID" HeaderStyle-Width="80px">
                    <ItemTemplate>
                        <asp:HyperLink runat="server" ID="hypUpdateOrder" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Client" SortExpression="CustomerOrganisationName"
                    DataField="CustomerOrganisationName" UniqueName="Customer">
                </telerik:GridBoundColumn>
                <telerik:GridHyperLinkColumn HeaderText="Group" SortExpression="OrderGroupID" DataTextField="OrderGroupID"
                    DataNavigateUrlFields="OrderGroupID" DataNavigateUrlFormatString="javascript:viewOrderGroup({0})" />
                <telerik:GridTemplateColumn HeaderText="Run&nbsp;ID" SortExpression="JobID" HeaderStyle-Width="80px"
                    UniqueName="RunId">
                    <ItemTemplate>
                        <asp:HyperLink ID="hypJobId" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Business Type" SortExpression="BusinessTypeDescription"
                    DataField="BusinessTypeDescription" UniqueName="BusinessType">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="TrunkDate" HeaderText="Trunk Date" DataFormatString="{0:dd/MM/yy}"
                    UniqueName="TrunkDate">
                </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn HeaderText="Rate" SortExpression="ForeignRate" ItemStyle-Width="50px" UniqueName="Rate">
                    <ItemTemplate>
                        <asp:Label ID="lblRate" runat="server"></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Extras" SortExpression="ExtrasForeignTotal" ItemStyle-Width="50px" UniqueName="Extras">
                    <ItemTemplate>
                        <asp:Label ID="lblExtras" runat="server"></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Subcontract Rate" SortExpression="SubContractRate" ItemStyle-Width="50px" UniqueName="SubcontractRate">
                    <ItemTemplate>
                        <asp:Label ID="lblSubcontractRate" runat="server"></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="NoPallets" Aggregate="Sum" FooterText="Plts<br/>"
                    FooterStyle-Wrap="false" HeaderText="Plts" FooterStyle-Font-Bold="true" ItemStyle-Width="25"
                    HeaderStyle-Width="25" FooterStyle-Width="25" UniqueName="NoPallets">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="PalletSpaces" Aggregate="Sum" FooterAggregateFormatString="Spaces<br/>{0:0}"
                    DataFormatString="{0:0}" FooterStyle-Wrap="false" FooterStyle-Font-Bold="true"
                    HeaderText="Spaces" ItemStyle-Width="25" HeaderStyle-Width="25" FooterStyle-Width="25"
                    UniqueName="PalletSpaces">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Weight" HeaderText="Kgs" Aggregate="Sum" FooterStyle-Wrap="false"
                    DataFormatString="{0:0}" FooterStyle-Font-Bold="true" FooterAggregateFormatString="Kgs<br/>{0:0}"
                    ItemStyle-Width="25" HeaderStyle-Width="25" FooterStyle-Width="25" UniqueName="Weight">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Service" SortExpression="OrderServiceLevel"
                    DataField="OrderServiceLevel" UniqueName="OrderServiceLevel" />
                <telerik:GridBoundColumn HeaderText="Order No" Aggregate="Count" FooterText="# Orders<br/>"
                    FooterStyle-Font-Bold="true" SortExpression="CustomerOrderNumber" DataField="CustomerOrderNumber"  DataFormatString="&nbsp;{0}"
                    UniqueName="CustomerOrderNumber">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Collect From" SortExpression="CollectionPointDescription"
                    DataField="CollectionPointDescription" ItemStyle-Wrap="false" UniqueName="CollectionPointDescription">
                </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn HeaderText="Collect At" SortExpression="CollectionDateTime"
                    UniqueName="CollectionDateTime">
                    <ItemTemplate>
                        <%# GetDate(Item.CollectionDateTime, Item.CollectionIsAnytime) %></ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Deliver To" SortExpression="DeliveryPointDescription"
                    DataField="DeliveryPointDescription" ItemStyle-Wrap="false" UniqueName="DeliveryPointDescription">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Deliver To Postcode" SortExpression="DeliveryPostCode" DataField="DeliveryPostCode" UniqueName="DeliveryPostCode" />
                <telerik:GridTemplateColumn HeaderText="Deliver At" SortExpression="DeliveryDateTime"
                    UniqueName="DeliveryDateTime">
                    <ItemTemplate>
                        <%# GetDate(Item.DeliveryDateTime, Item.DeliveryIsAnytime) %></ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Delivery Order Number" SortExpression="DeliveryOrderNumber"
                    DataField="DeliveryOrderNumber" DataFormatString="&nbsp;{0}" UniqueName="DeliveryOrderNumber" />
                <telerik:GridBoundColumn HeaderText="Delivering Resource" SortExpression="DeliveringResource"
                    DataField="DeliveringResource" UniqueName="DeliveringResource" />
                <telerik:GridBoundColumn HeaderText="Goods Type" SortExpression="GoodsTypeDescription"
                    DataField="GoodsTypeDescription" UniqueName="GoodsTypeDescription" />
                <telerik:GridBoundColumn HeaderText="Surcharges" SortExpression="Surcharges" DataField="Surcharges" UniqueName="Surcharges" Visible="false" />
                <telerik:GridBoundColumn HeaderText="Pallet Type" SortExpression="PalletType" DataField="PalletType" UniqueName="PalletType" Visible="false" />
                <telerik:GridBoundColumn DataField="QtrPallets" HeaderText="1/4" Aggregate="Sum"
                    FooterStyle-Wrap="false" DataFormatString="{0:0}" FooterStyle-Font-Bold="true"
                    FooterAggregateFormatString="1/4<br/>{0:0}" ItemStyle-Width="25" HeaderStyle-Width="25"
                    FooterStyle-Width="25" UniqueName="QtrPallets">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="HalfPallets" HeaderText="1/2" Aggregate="Sum"
                    FooterStyle-Wrap="false" DataFormatString="{0:0}" FooterStyle-Font-Bold="true"
                    FooterAggregateFormatString="1/2<br/>{0:0}" ItemStyle-Width="25" HeaderStyle-Width="25"
                    FooterStyle-Width="25" UniqueName="HalfPallets">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="FullPallets" HeaderText="Full" Aggregate="Sum"
                    FooterStyle-Wrap="false" DataFormatString="{0:0}" FooterStyle-Font-Bold="true"
                    FooterAggregateFormatString="Full<br/>{0}" ItemStyle-Width="25" HeaderStyle-Width="25"
                    FooterStyle-Width="25" UniqueName="FullPallets">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="OverPallets" HeaderText="O/S" Aggregate="Sum"
                    FooterStyle-Wrap="false" DataFormatString="{0:0}" FooterStyle-Font-Bold="true"
                    FooterAggregateFormatString="O/S<br/>{0:0}" ItemStyle-Width="25" HeaderStyle-Width="25"
                    FooterStyle-Width="25" UniqueName="OverPallets">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Trailer" SortExpression="Trailer" DataField="Trailer" UniqueName="Trailer" />
                <telerik:GridBoundColumn HeaderText="Vehicle" SortExpression="Vehicle" DataField="Vehicle" UniqueName="Vehicle" />            
                <telerik:GridTemplateColumn HeaderText="Has POD" HeaderStyle-Width="50">
                    <ItemTemplate>
                        <asp:HyperLink ID="lnkPOD" runat="server" NavigateUrl="" ForeColor="black" Target="_blank"></asp:HyperLink>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>                    
            </Columns>
        </MasterTableView>
        <ClientSettings AllowGroupExpandCollapse="true" AllowDragToGroup="false" AllowColumnsReorder="true"
            ReorderColumnsOnClient="true">
            <Scrolling UseStaticHeaders="true" ScrollHeight="600" AllowScroll="true" />
            <Resizing AllowColumnResize="true" AllowRowResize="false" />
            <Selecting AllowRowSelect="true" />
            <ClientEvents OnRowContextMenu="RowContextMenu" />
        </ClientSettings>
    </telerik:RadGrid>
    <telerik:RadContextMenu ID="RadMenu1" IsContext="True" runat="server" Skin="Outlook"
        OnClientItemClicked="OnClick" ContextMenuElementID="none" />
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

        <script type="text/javascript">
            function cboClient_itemsRequesting(sender, eventArgs) {
                try {
                    var context = eventArgs.get_context();
                    context["DisplaySuspended"] = true;
                }
                catch (err) { }
            }

            var shufflerRowIds = '';
            var hidRecordIdsCsv = '';
            function setHiddenRecordIds() {

                var hidRecordIds = $get('<%=this.hidRecordIds.ClientID %>');
                hidRecordIdsCsv = '';

                $('a[id*=hypUpdateOrder]').each(function(index, ele) {

                    if (hidRecordIdsCsv == "") {
                        hidRecordIdsCsv = ele.innerText;
                    }
                    else {
                        hidRecordIdsCsv = hidRecordIdsCsv + ',' + ele.innerText;
                    }
                }
            );

                hidRecordIds.value = hidRecordIdsCsv;
                shufflerRowIds = hidRecordIdsCsv;
            }

            function viewOrderGroup(orderGroupID) {
                window.open("ordergroupprofile.aspx?ogid=" + orderGroupID, "", "width=1100, height=600, resizable=1, scrollbars=1");
            }

            // The Combostreamers returns HTML which formats the Points in the drop-down. But when selected, the HTML itself will show.
            // Strip the HTML and other address information out leaving only the Point Name/Description.
            function Point_CombBoxClosing(sender, eventArgs) {

                try {

                    var collectionPointFieldID = '<%= this.cboCollectionPointFilter.ClientID %>';
                    var deliveryPointFieldID = '<%= this.cboDeliveryPointFilter.ClientID %>';
                    var hidCollectionPointID = $get('<%=this.hidCollectionPointID.ClientID %>');
                    var hidDeliveryPointID = $get('<%=this.hidDeliveryPointID.ClientID %>');

                    if (sender.get_selectedIndex() == null) {

                        if (sender.get_id() == collectionPointFieldID)
                            hidCollectionPointID.value = "";

                        if (sender.get_id() == deliveryPointFieldID)
                            hidDeliveryPointID.value = "";
                    }

                    var itemText = sender.get_selectedItem().get_text();

                    if (itemText.indexOf('</td><td>') > 0) {
                        // remove any html characters from this. and Show the Point Name only
                        var pointName = itemText.split('</td><td>')[0];

                        pointName = pointName.replace(/&(lt|gt);/g, function (strMatch, p1) {
                            return (p1 == "lt") ? "<" : ">";
                        });
                        pointName = pointName.replace(/<\/?[^>]+(>|$)/g, "");

                        var storedValue = sender.get_value();

                        if (sender.get_id() == collectionPointFieldID)
                            hidCollectionPointID.value = storedValue;
                        else
                            hidDeliveryPointID.value = storedValue;

                        sender.set_text(pointName);
                    }

                }
                catch (e) { }

            }

        </script>

    </telerik:RadCodeBlock>
</asp:Content>
