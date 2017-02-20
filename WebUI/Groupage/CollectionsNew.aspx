<%@ Page Language="C#" AutoEventWireup="True" EnableViewStateMac="false" MasterPageFile="~/default_tableless.Master" Title="Groupage - Collections" CodeBehind="CollectionsNew.aspx.cs" Inherits="Orchestrator.WebUI.Groupage.CollectionsNew" EnableViewState="true" %>

<%@ Register TagPrefix="p1" TagName="BusinessTypeCheckList" Src="~/UserControls/BusinessTypeCheckList.ascx" %>
<%@ Register TagPrefix="p1" TagName="Point" Src="~/UserControls/point.ascx" %>
<%@ Register TagPrefix="p1" TagName="Resource" Src="~/UserControls/resource.ascx" %>
<%@ Register TagPrefix="cc1" Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">
    <script type="text/javascript" src="/script/jquery-ui-1.9.2.min.js"></script>
    <script type="text/javascript" src="/script/jquery.blockUI-2.64.0.min.js"></script>
    <script type="text/javascript" src="/script/jquery.fixedheader.js"></script>
    <script type="text/javascript" src="CollectionsNew.aspx.js"></script>
    <script type="text/javascript" language="javascript" src="/script/tooltippopups.js"></script>
    <script type="text/javascript" language="javascript" src="/script/jquery.quicksearch-1.3.1.js" ></script>

    <style type="text/css">
        h3
        {
            clear: left;
            font-size: 12px;
            font-weight: normal;
            padding: 0 0 1em;
            margin: 0;
        }
        /*.masterpage_layout {width: 1700px;}*/.RadGrid_Orchestrator *
        {
            font-family: Verdana !important;
            font-size: 10px !important;
        }
        .overlayedDataBox
        {
            width: 330px !important;
        }
    </style>
</asp:Content>

<asp:Content ID="content3" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1 class="collectionsIcon">
        Collections</h1>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <cc1:Dialog ID="dlgAddUpdatePointGeofence" runat="server" Width="1034" Height="600" Mode="Normal" URL="/Point/Geofences/GeofenceManagement.aspx"></cc1:Dialog>
    <cc1:Dialog ID="dlgManifestGeneration" runat="server" Width="1200" Height="900" Mode="Normal" URL="/Reports/Reportviewer.aspx?wiz=true" OnClientDialogCallBack="manifestGeneration_CallBack" ReturnValueExpected="true" ></cc1:Dialog>
    <cc1:Dialog ID="dlgLoadingSheet" runat="server" Width="1200" Height="900" Mode="Normal" URL="/Reports/Reportviewer.aspx?wiz=true" ReturnValueExpected="false" ></cc1:Dialog>
    <cc1:Dialog ID="dlgOrder" URL="/Groupage/ManageOrder.aspx" Height="900" Width="1200" AutoPostBack="true" runat="server" Mode="Modal" ReturnValueExpected="true"></cc1:Dialog>

    <div class="overlayedFilterBox" id="overlayedClearFilterBox" style="display: none;">
        <fieldset>
            <table>
                <tr>
                    <td class="formCellLabel">
                        Date From
                    </td>
                    <td colspan="3" align="left">
                        <table>
                            <tr>
                                <td class="formCellField">
                                    <telerik:RadDatePicker ID="dteStartDate" runat="server" Width="100" ToolTip="The start date for the filter">
                                    <DateInput runat ="server"
                                     dateformat="dd/MM/yy">
                                     </DateInput>
                                    </telerik:RadDatePicker>
                                </td>
                                <td class="formCellLabel">
                                    Date To
                                </td>
                                <td class="formCellField">
                                    <telerik:RadDatePicker ID="dteEndDate" runat="server" Width="100" ToolTip="The end date for the filter">
                                    <DateInput runat="server"
                                    dateformat="dd/MM/yy">
                                    </DateInput>
                                    </telerik:RadDatePicker>
                                </td>
                                <td class="formCellLabel">
                                    Date Filters on
                                </td>
                                <td class="formCellField" colspan="3">
                                    <asp:RadioButtonList ID="rblDateFiltering" runat="server" RepeatDirection="Horizontal">
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Orders to display
                    </td>
                    <td colspan="3" align="left">
                        <table>
                            <tr>
                                <td class="formCellField" colspan="3">
                                    <asp:CheckBox runat="server" ID="chkIncludeOrdersNotPlannedForCollection" Text="Include orders <b>NOT</b> planned for collection" Checked="true" /><br />
                                    <asp:CheckBox runat="server" ID="chkIncludeOrdersXDockedAtOwnCompany" Text="Include orders cross docked at my company" /><br />
                                    <asp:CheckBox runat="server" ID="chkIncludeOrdersXDockedAtOtherCompany" Text="Include orders cross docked at another company" /><br />
                                    <asp:CheckBox runat="server" ID="chkShowAll" Text="Show <b>ALL</b> approved orders" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Collection Area
                    </td>
                    <td class="formCellField" colspan="3">
                        <asp:CheckBox ID="chkSelectAllTrafficAreas" runat="server" Text="Select all Traffic Areas" />
                        <asp:CheckBoxList ID="cblTrafficAreas" runat="server" DataValueField="TrafficAreaID"
                            DataTextField="Description" RepeatDirection="Horizontal" RepeatColumns="9">
                        </asp:CheckBoxList>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Business Type
                    </td>
                    <td class="formCellField" colspan="3">
                        <p1:BusinessTypeCheckList ID="businessTypeCheckList" runat="server" ItemCountPerRow="6" />
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                    </td>
                    <td class="formCellField" colspan="3">
                        <asp:CheckBox ID="chkRequiresBookingIn" Text="Book in required" runat="server"></asp:CheckBox>
                        <asp:CheckBox ID="chkBookedIn" Text="Booked In" runat="server"></asp:CheckBox>
                        <asp:CheckBox ID="chkDoesNotRequireBookingIn" Text="Does not require Booking In"
                            runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Trunk Date From
                    </td>
                    <td class="formCellField">
                        <asp:CheckBox ID="chkNoTrunkDate" Text="Show rows with no trunk date" Checked="true"
                            runat="server"></asp:CheckBox>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Show the Grid Collapsed
                    </td>
                    <td class="formCellField">
                        <asp:CheckBox ID="chkStartCollapsed" runat="server" Text="Show the grid delivery area totals only" />
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Refusals
                    </td>
                    <td class="formCellField">
                        <asp:CheckBox runat="server" ID="chkOrderOnlyShowRefusals" Text="Only show Refusals" />
                        <asp:CheckBox Visible="false" runat="server" ID="chkOnTrailer" Text="On Trailer" />&nbsp;
                        <asp:CheckBox Visible="true" runat="server" ID="chkOffTrailer" Text="Off Trailer" />
                    </td>
                </tr>
            </table>
        </fieldset>
        <div class="buttonbar">
            <asp:Button ID="btnRefresh" runat="server" Text="Refresh" Width="75" />
            <input type="button" id="Button2" runat="server" value="Cancel and close" onclick="FilterOptionsDisplayHide()" />
        </div>
    </div>
    
    <div class="overlayedClearDataBox" id="overlayedClearDataBox" style="display: none;">
        <div id="tabs">
            <ul>
                <li><a href="#tabs-1">Load builder</a></li>
                <li><a href="#tabs-2">Delivery point</a></li>
                <li><a href="#tabs-3">Resource</a></li>
                <li id="dragLI" class="moveTab"></li>
                <li id="resetBox" class="resetTab" onclick="resetBox();"></li>
                <li id="expandBox" class="expandTab" onclick="expandBox();"></li>
                <li id="detractBox" class="detractTab" onclick="detractBox();" style="display: none;">
                </li>
            </ul>
            <div id="tabs-1">
                <div id="loadBuilder" class="loadBuilder" style="height: 355px; overflow-x: hidden;
                    overflow-y: auto;">
                    <div class="loadBuilderInner">
                        <fieldset style="margin: 0 0 5px 0;">
                            <legend>Runs</legend>To add an order to an existing run either click on a Coll Run
                            ID in the Grid or click here:
                            <input type="button" value="Add To Existing Run" class="buttonClass" onclick="ShowUpdateJob();" />
                        </fieldset>
                        <table id="tblLoadBuilderOrders" class="DataGridStyle" style="width: 315px; font-weight: normal;
                            display: none;" cellpadding="2" cellspacing="0">
                            <tr class="DataGridListHeadSmall">
                                <th>
                                    LO
                                </th>
                                <th style="width: 30px;">
                                    ID
                                </th>
                                <th style="width: 100px;">
                                    From
                                </th>
                                <th style="display: none;">
                                    To
                                </th>
                                <th>
                                    Spcs
                                </th>
                                <th>
                                    Kg
                                </th>
                                <th>
                                    &#160;
                                </th>
                            </tr>
                            <tr class="DataGridListItem">
                                <td style="width: 20px;">
                                    <img src="/App_Themes/Orchestrator/img/masterpage/icon-cross.png" id="img2" style="display: none;
                                        cursor: pointer; margin: 2px 2px 0 0;" /><input type="text" id="txtOrder" style="width: 15px;
                                            color: Black;" />
                                </td>
                                <td style="width: 15px;" class="orderID">
                                    <span id="orderID">OrderID</span>
                                </td>
                                <td style="width: 135px;">
                                    <span id="collectionPoint">collectionpoint</span>
                                </td>
                                <td style="display: none;">
                                    <span id="deliveryPoint">deliverypoint</span>
                                </td>
                                <td style="width: 15px;">
                                    <span id="palletSpaces">palletspaces</span>
                                </td>
                                <td style="width: 15px;">
                                    <span id="weight">weight</span>
                                </td>
                                <td style="width: 18px;">
                                    <img alt="Remove order from Load" src="/App_Themes/Orchestrator/img/masterpage/icon-cross.png"
                                        id="imgRemove" style="cursor: pointer; margin: 2px 2px 0 0;" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div class="buttonBar">
                        <input type="button" class="buttonClassSmall" value="Re-Order" onclick="ReOrder();" />
                    </div>
                    <div style="min-height: 20px">
                        <div id="divBusinessTypes">
                        </div>
                    </div>
                </div>
            </div>
            <div id="tabs-2">
                <div class="loadBuilder" style="height: 355px; overflow-x: hidden; overflow-y: auto;">
                    <fieldset style="margin-bottom: 5px;">
                        <legend>Deliver To</legend>
                        <table style="width: 315px;">
                            <tr>
                                <td>
                                    <input type="checkbox" id="chkTrunkDelivery" value="0" checked="checked" /><label for="chkTrunkDelivery">Deliver these to a Cross Dock point</label>
                                </td>
                            </tr>
                            <tr id="trDeliveryPoint">
                                <td>
                                    <table>
                                        <tr>
                                            <td colspan="2">
                                                <p1:Point runat="server" ID="ucDeliveryPoint" ShowFullAddress="false" CanChangePoint="true"
                                                    CanCreateNewPoint="false" CanUpdatePoint="false" Width="300" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="formCellLabel" style="width: 33%;">
                                                Delivery Date
                                            </td>
                                            <td class="formCellInput">
                                                <telerik:RadDateInput ID="dteDeliveryDate" Width="65" runat="server" DateFormat="dd/MM/yy"
                                                    DisplayDateFormat="dd/MM/yy">
                                                </telerik:RadDateInput>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="formCellLabel">
                                                Delivery Time
                                            </td>
                                            <td class="formCellInput">
                                                <telerik:RadDateInput ID="dteDeliveryTime" runat="server" DateFormat="HH:mm" EmptyMessage="Anytime"
                                                    DisplayDateFormat="HH:mm" Width="65" SelectedDate="01/01/01 08:00">
                                                </telerik:RadDateInput>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                    <div id="deliveryPointText" class="infoPanel" style="font-weight: normal;">
                        The orders selected will be to their Delivery points. This will create a run with
                        1 or more collections and 1 or more deliveries</div>
                </div>
            </div>
            <div id="tabs-3">
                <div class="loadBuilder" style="height: 355px; overflow-x: hidden; overflow-y: auto;">
                    <fieldset>
                        <legend>Resource this</legend>
                        <input type="radio" value="0" name="rdoresource" id="rbOwnResource" checked="checked" /><label
                            for="rbOwnResource">use own resource</label><input type="radio" value="1" name="rdoresource"
                                id="rbSubContractor" /><label for="rbSubContractor">Use Subcontractor</label>
                        <div id="pnlOwnresource">
                            <p1:Resource runat="server" ID="ucResource"></p1:Resource>
                        </div>
                        <div id="pnlSubContractor" style="display: none;">
                            <table>
                                <tr>
                                    <td colspan="2">
                                        <telerik:RadComboBox ID="cboSubContractor" runat="server"
                                            ShowMoreResultsBox="false" MarkFirstMatch="true" ItemRequestTimeout="500" Width="100%"
                                            EnableLoadOnDemand="true">
                                            <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetSubContractors" />
                                        </telerik:RadComboBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label for="rbWholeJob">
                                            Subcontract whole run</label><input type="radio" id="rbWholeJob" value="0" name="SubContractOption"
                                                checked="checked" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label for="rbPerOrder">
                                            Subcontract per order</label><input type="radio" id="rbPerOrder" value="1" name="SubContractOption" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formCellLabel">
                                        Subcontract for
                                    </td>
                                    <td class="formCellInput">
                                        <telerik:RadNumericTextBox ID="txtSubbyRate" runat="server" Type="Currency" MinValue="0"
                                            Value="0" Width="80">
                                        </telerik:RadNumericTextBox>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div>
                            <input type="checkbox" id="chkCreateOrderGroup" value="createOrderGroup" /><label for="chkCreateOrderGroup">Create a group for the orders</label>
                            <br />
                            <input type="checkbox" id="chkCreateManifest" value="createManifest" /><label for="chkCreateManifest">Create and show the manifest</label>
                            <br />
                            <input type="checkbox" id="chkShowJobOnCreation" value="showJobOnCreation" /><label for="chkShowJobOnCreation">Show the run details when created</label>
                            <br />
                            <input type="checkbox" id="chkCreateLoadingSheet" value="createLoadingSheet" /><label for="chkCreateLoadingSheet">Create and show the loading sheet</label>
                            <br />
                            <input type="checkbox" id="chkShowInProgress" value="2" checked="checked" /><label for="chkShowInProgress">Show as communicated</label>
                        </div>
                    </fieldset>
                    <div id="manifestOptions" style="display: none;">
                        <fieldset>
                            <legend>Manifest options</legend>
                            <asp:RadioButtonList ID="rblOrdering" runat="server" RepeatDirection="Horizontal" Style="display:none;">
                                <asp:ListItem Text="Order by Planned Times" Value="0" />
                                <asp:ListItem Text="Order by Run Booked Times" Value="1" Selected="True" />
                            </asp:RadioButtonList>
                            <input type="checkbox" id="chkExcludeFirstRow" value="1" checked="checked" /><label for="chkExcludeFirstRow">Exclude the first row from the Manifest</label>
                            <br />
                            <input type="checkbox" id="chkShowFullAddress" value="1" /><label for="chkShowFullAddress">Show the full address on the manifest</label>
                            <br />
                            <span style="width: 80px;">Manifest Date:</span><telerik:RadDateInput ID="dteManifestDate" runat="server" OnClientDateChanged="ManifestDate_OnDateChanged"></telerik:RadDateInput>
                            <br />
                            <span style="width: 80px;">Manifest Title :</span><input type="text" id="txtManifestTitle" style="width: 250px;" />
                            <br />
                            Number of blank rows to include:<input type="text" id="txtExtraRows" value="0" style="width: 25px;" />
                        </fieldset>
                    </div>
                    <div class="buttonbar" style="margin-bottom: 1px; margin-top: 3px; display: none;"
                        id="createCollectionJob">
                        <input id="btnCreateCollectionJob" type="button" value="Create Collection Run" style="font-size: 11px;" onclick="if(!orderSubmitCheck('btnCreateCollectionJob')) return false;" />
                        <input id="btnCancelCreateCollectionJob" type="button" value="Cancel" style="font-size: 11px;" onclick="CancelAndResetScreen();" />
                    </div>
                </div>
            </div>
            <div class="selectedInfo">
                <span id="dvSelected" style="margin-top: 10px 0 0 0;">Number of Pallets : 0</span>
            </div>
        </div>
        <div id="tabs2" style="display: none;">
            <ul>
                <li><a href="#tabs-4">Instructions</a></li>
                <li><a href="#tabs-5">Pick up point</a></li>
                <li><a href="#tabs-6">Existing Orders</a></li>
                <li id="dragLI" class="moveTab"></li>
                <li id="resetBox" class="resetTab" onclick="resetBox();"></li>
                <li id="expandBox" class="expandTab" onclick="expandBox();"></li>
                <li id="detractBox" class="detractTab" onclick="detractBox();" style="display: none;">
                </li>
            </ul>
            <div id="tabs-4">
                <div class="jobView">
                    <div>
                        Instructions for the run :
                        <input type="text" id="txtJobID" style="width: 65px;" />
                        &nbsp;
                        <input type="button" id="btnFindJob" value="Find" onclick="FindJob();" />
                    </div>
                    <div style="height: 255px; overflow-x: hidden; overflow-y: auto;">
                        <table id="tblInstructions" class="DataGridStyle" style="width: 95%; font-weight: normal;"
                            cellpadding="2" cellspacing="0">
                            <tr class="DataGridListHeadSmall">
                                <th>
                                    &nbsp;
                                </th>
                                <th>
                                    &nbsp;
                                </th>
                                <th>
                                    Where
                                </th>
                                <th>
                                    When
                                </th>
                            </tr>
                        </table>
                        <input type="checkbox" id="chkLoadAndGo" checked="checked" value="0" onclick="SelectLoadAndGo(this);" /><label id="lblLoadAndGo" for="chkLoadAndGo">Add selected orders as load and go.</label>
                        <br />
                        <input type="checkbox" id="chkUpdateManifest" value="updateManifest" checked="true" /><label id="lblUpdateManifest" for="chkUpdateManifest">Update and show the manifest</label>
                    </div>
                    <div class="buttonbar" style="margin-bottom: 1px; margin-top: 3px; text-align: center;">
                        <input type="button" value="Add Order(s) to Run" onclick="UpdateCollectionJob();" />
                        <input type="button" value="Cancel" onclick="CancelAndResetScreen();" />
                    </div>
                </div>
            </div>
            <div id="tabs-5" class="jobView">
            </div>
            <div id="tabs-6">
                <div class="jobView">
                    <div style="height: 315px; overflow-x: hidden; overflow-y: auto;">
                        <table id="tblExistingOrders" class="DataGridStyle" style="width: 95%; font-weight: normal;
                            display: none;" cellpadding="2" cellspacing="0">
                            <tr class="DataGridListHeadSmall">
                                <th>
                                    LO
                                </th>
                                <th style="width: 30px;">
                                    ID
                                </th>
                                <th style="width: 100px;">
                                    From
                                </th>
                                <th style="display: none;">
                                    To
                                </th>
                                <th>
                                    Spcs
                                </th>
                                <th>
                                    Kg
                                </th>
                            </tr>
                            <tr class="DataGridListItem">
                                <td style="width: 40px;">
                                    <img src="/App_Themes/Orchestrator/img/masterpage/icon-cross.png" id="img1" style="cursor: pointer;
                                        margin: 2px 2px 0 0;" /><input type="text" id="Text2" value="0" style="width: 15px;
                                            color: Black;" />
                                </td>
                                <td style="width: 15px;" class="orderID">
                                    <span id="Span2">OrderID</span>
                                </td>
                                <td style="width: 135px;">
                                    <span id="Span3">collectionpoint</span>
                                </td>
                                <td style="display: none;">
                                    <span id="Span4">deliverypoint</span>
                                </td>
                                <td style="width: 15px;">
                                    <span id="Span5">palletspaces</span>
                                </td>
                                <td style="width: 15px;">
                                    <span id="Span6">weight</span>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
        </div>
        <div id="dvColumnDisplay" class="ui-tabs ui-corner-all">
            <fieldset>
                <legend>Display Columns</legend>
                <div>
                    <div id="columnDisplayAccordion">
                        <h3>Standard Columns</h3>
                        <div>
                            <asp:CheckBoxList ID="cblGridColumns" runat="server">
                                <asp:ListItem Text="Collection Vehicle" Value="CollectionVehicle" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Collection Driver" Value="CollectionDriver" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Client" Value="Customer" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Delivery Point" Value="DeliveryPoint" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Delivery Town" Value="DeliveryTown" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Final Destination Point" Value="DestinationPoint" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Final Destination Town" Value="DestinationTown" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Collection Run" Value="CollectionJob" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Collect Order" Value="CollectionOrder" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Collection Customer" Value="CollectionCustomer" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Collection Point" Value="CollectionPoint" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Collection Address Line 1" Value="CollectionAddress1" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Collection Address Line 2" Value="CollectionAddress2" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Collection Address Lines" Value="CollectionAddressLines" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Goods Type" Value="GoodsType" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="No Pallets" Value="NoPallets" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Pallet Spaces" Value="Spcs" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Weight" Value="Weight" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Collection Notes" Value="CollectionNotes" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Traffic Notes" Value="TrafficNotes" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Surcharges" Value="Surcharges" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Service Level" Value="OrderServiceLevelShortCode" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Requesting Depot" Value="RequestingDepot" Selected="True"></asp:ListItem>
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
                <input type="button" id="Button1" runat="server" value="Cancel and close" onclick="ColumnDisplayHide()" />
            </div>
        </div>
    </div>
    
    <asp:Panel ID="pnlResourceManifestLinkPopup" runat="server" style="width: 400px; background-color: white; border-width: 2px; border-color: black; border-style: solid; padding: 20px; color: Silver; display: none;">
        <h2>Resource Manifest</h2>
        <p>
            The Resource Manifest could not be generated.
            <br />
            Please follow the link below to manually create / update.
         </p>
        <p><a href="javascript:window.open('/manifest/resourcemanifestlist.aspx'); NextAction();">Create Resource Manifest</a></p>
        <div class="buttonBar"><asp:Button ID="btnClose" runat="server" Text="Ok" /></div>
    </asp:Panel>
    <asp:Button ID="btnTest" Text="Test Button" runat="server" Style="display: none;" />
    <ajaxToolkit:ModalPopupExtender runat="server" ID="extener" BehaviorID="programmaticModalPopupBehavior"
        TargetControlID="btnTest" BackgroundCssClass="modalBackground" PopupControlID="pnlResourceManifestLinkPopup"
        OkControlID="btnClose" OnOkScript="javascript:NextAction();"  >
    </ajaxToolkit:ModalPopupExtender>    
    
    <input type="hidden" id="radGridClickedRowIndex" name="radGridClickedRowIndex" />
    <asp:HiddenField ID="hidCheckSubmit" runat="server" Value="" />
    
    <telerik:RadGrid runat="server" ID="grdOrders" AllowPaging="false" AllowSorting="true"
        AutoGenerateColumns="false" AllowMultiRowSelection="true" >
       <mastertableview commanditemdisplay="Top" grouploadmode="Client" commanditemstyle-backcolor="White" DataKeyNames="Weight,NoPallets,PalletSpaces" ClientDataKeyNames="OrderGroupID" TableLayout="Fixed">
            <GroupByExpressions>
                <telerik:GridGroupByExpression>
                    <GroupByFields>
                        <telerik:GridGroupByField FieldName="TrafficAreaShortName" />
                    </GroupByFields>
                    <SelectFields>
                        <telerik:GridGroupByField FieldName="TrafficAreaShortName" FieldAlias="TrafficAreaShortName" />
                        <telerik:GridGroupByField FieldName="OrderID" Aggregate="Count" FieldAlias="Count" />
                    </SelectFields>
                </telerik:GridGroupByExpression>
            </GroupByExpressions>
            <RowIndicatorColumn Display="false">
            </RowIndicatorColumn>
            <CommandItemTemplate>
                <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()">Show filter Options</div>
                <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()" style="display: none;">Close filter Options</div>
                <div class="overlayedRefreshIcon"><asp:Button ID="btnRefreshTop" runat="server" Text="Refresh" OnClick="btnRefresh_Click_NoFilterUpdate" /></div>
                <asp:Label ID="lblQF" runat="server" Text="Quick Filter:" />
                <div id="grdFilterHolder" ></div>
            </CommandItemTemplate>
            <Columns>
                <telerik:GridTemplateColumn UniqueName="chkSelectColumn" HeaderStyle-Width="25" >
                    <ItemTemplate>
                        <asp:CheckBox ID="chkOrderID" runat="server" orderID='<%# ((System.Data.DataRowView)Container.DataItem)["OrderID"].ToString() %>' />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Coll Veh" DataField="CollectionVehicleRegNo" UniqueName="CollectionVehicle" />
                <telerik:GridTemplateColumn HeaderText="Driver" UniqueName="CollectionDriver">
                    <ItemTemplate>
                        <a href="#" onclick='showManifest(<%# Eval("ResourceManifestID") %>);'>
                            <%# Eval("CollectionDriver") %></a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn ItemStyle-Width="12px" HeaderStyle-Width="12px" UniqueName="GroupedOrderImage">
                    <ItemTemplate>
                        <asp:PlaceHolder ID="plcGroupedOrderImage" runat="server">&nbsp;</asp:PlaceHolder>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="ID" SortExpression="OrderID" UniqueName="OrderID" HeaderStyle-Width="55">
                    <ItemTemplate>
                        <span><a runat="server" id="hypOrder"></a>
                            <!--<img style="padding-left: 2px;" runat="server" id="imgOrderBookedIn" visible="false"
                                src="/App_Themes/Orchestrator/Img/MasterPage/icon-tick-small.png" alt="Booked In" />-->
                            <img style="padding-left: 2px;" runat="server" id="imgGoodsType" visible="false"
                                src="/App_Themes/Orchestrator/Img/MasterPage/icon-hazard.png" alt="Hazardous Goods Type" />
                        </span>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Client" SortExpression="Customer" DataField="Customer" HeaderStyle-Width="80" UniqueName="Customer" />
                <telerik:GridTemplateColumn HeaderText="Del Point" SortExpression="DeliveryPoint" ItemStyle-Font-Bold="true" UniqueName="DeliveryPoint" >
                    <ItemTemplate>
                         <a href="#" class="ShowPointTooltip" rel="/point/getpointaddresshtml.aspx" onclick="openAlterPointWindow(this);" pointid="<%# Eval("DeliveryPointId") == DBNull.Value ? -1 : Eval("DeliveryPointId") %>" instructionid="" >
                            <%# Eval("DestinationPoint") %>
                         </a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Del Town" SortExpression="DeliveryTown" DataField="DeliveryTown" ItemStyle-Font-Bold="true" UniqueName="DeliveryTown" />
                <telerik:GridBoundColumn HeaderText="Destination Point" SortExpression="DestinationPoint" DataField="DestinationPoint" ItemStyle-Font-Bold="true" UniqueName="DestinationPoint" />
                <telerik:GridBoundColumn HeaderText="Destination Town" SortExpression="DestinationTown" DataField="DestinationTown" ItemStyle-Font-Bold="true" UniqueName="DestinationTown" />
                <telerik:GridTemplateColumn HeaderText="Coll" HeaderStyle-Width="60" UniqueName="CollectionJob" SortExpression="CollectionJobID">
                    <ItemTemplate>
                        <a id="lnkUpdateJob" href="#" onclick='LoadJob(<%# Eval("CollectionJobID") %>);' style='<%# Eval("CollectionjobID") != DBNull.Value ? "": "display:none;" %>'>
                            <%# Eval("CollectionJobID") %></a> <a href="#" onclick='ViewJob(<%# Eval("CollectionJobID") %>);'
                                style='<%# Eval("CollectionjobID") != DBNull.Value ? "": "display:none;" %>'>View</a>
                        <input id="chkJobID" runat="server" onclick='<%# Eval("CollectionJobID") != DBNull.Value ? "LoadingSummarySheetSelection(this);" : "" %>'
                            value='<%# Eval("CollectionJobID") %>' type="checkbox" style='<%# Eval("CollectionJobID") != DBNull.Value ? "": "display:none;" %>' />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="CO" SortExpression="CollectOrder" DataField="CollectOrder" HeaderStyle-Width="20"  UniqueName="CollectionOrder" />
                <telerik:GridBoundColumn HeaderText="Collection Name" SortExpression="CollectionCustomer" DataField="CollectionCustomer" UniqueName="CollectionCustomer" />
                <telerik:GridTemplateColumn HeaderText="Coll Point" SortExpression="CollectionPoint" ItemStyle-Font-Bold="true" UniqueName="CollectionPoint" >
                    <ItemTemplate>
                         <a href="#" class="ShowPointTooltip" rel="/point/getpointaddresshtml.aspx" onclick="openAlterPointWindow(this);" pointid="<%# Eval("CollectionPointId") == DBNull.Value ? -1 : Eval("CollectionPointId") %>" instructionid="" >
                            <%# Eval("CollectionPoint") %>
                         </a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Coll Addr 1" SortExpression="AddressLine1" DataField="AddressLine1" UniqueName="CollectionAddress1" />
                <telerik:GridBoundColumn HeaderText="Coll Addr 2" SortExpression="AddressLine2" DataField="AddressLine2" UniqueName="CollectionAddress2" />
                <telerik:GridTemplateColumn HeaderText="Coll Addr" UniqueName="CollectionAddressLines">
                    <ItemTemplate>
                        <%# string.IsNullOrEmpty(Eval("AddressLine1").ToString()) ? "" : Eval("AddressLine1").ToString() %>
                        <%# string.IsNullOrEmpty(Eval("AddressLine2").ToString()) ? "" : "<br/>" + Eval("AddressLine2").ToString() %>
                        <%# string.IsNullOrEmpty(Eval("AddressLine3").ToString()) ? "" : "<br/>" + Eval("AddressLine3").ToString() %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Coll Town" SortExpression="PostTown" DataField="PostTown" ItemStyle-Font-Bold="true" UniqueName="CollectionTown" />
                <telerik:GridTemplateColumn HeaderText="Post Code" SortExpression="PostCode" DataField="PostCode" UniqueName="PostCode" HeaderStyle-Width="40" >
                    <ItemTemplate>
                        <asp:PlaceHolder ID="phPostCode" runat="server"></asp:PlaceHolder>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Goods Type"  DataField="GoodsTypeDescription"
                    HeaderStyle-Width="20" Resizable="true" UniqueName="GoodsType" />
                <telerik:GridBoundColumn HeaderText="Plts" SortExpression="NoPallets" DataField="NoPallets" UniqueName="NoPallets" HeaderStyle-Width="25"  />
                <telerik:GridBoundColumn HeaderText="Spcs" SortExpression="Spcs" DataField="PalletSpaces" UniqueName="Spcs" DataFormatString="{0: 0.##}" HeaderStyle-Width="25"  />
                <telerik:GridBoundColumn HeaderText="Kg" SortExpression="Weight" DataField="Weight" UniqueName="Weight" DataFormatString="{0:0}" HeaderStyle-Width="25"  />
                <telerik:GridTemplateColumn HeaderText="Collect At" SortExpression="CollectionDateTime">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblCollectAt"></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Collection Notes" DataField="CollectionNotes" UniqueName="CollectionNotes">
                    <ItemTemplate>
                        <%# string.IsNullOrEmpty(Eval("CollectionNotes").ToString()) ? "" : Eval("CollectionNotes").ToString().Substring(0, Math.Min(100, Eval("CollectionNotes").ToString().Length)) + (Eval("CollectionNotes").ToString().Length > 100 ? "..." : "") %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Traffic Notes" DataField="TrafficNotes" UniqueName="TrafficNotes">
                    <ItemTemplate>
                        <%# string.IsNullOrEmpty(Eval("TrafficNotes").ToString()) ? "" : Eval("TrafficNotes").ToString().Substring(0, Math.Min(100, Eval("TrafficNotes").ToString().Length)) + (Eval("TrafficNotes").ToString().Length > 100 ? "..." : "") %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="Surcharges" UniqueName="Surcharges" HeaderText="Surcharges" />
                <telerik:GridBoundColumn DataField="OrderServiceLevelShortCode" UniqueName="OrderServiceLevelShortCode" HeaderText="SL" HeaderStyle-Width="15" />
                <telerik:GridBoundColumn DataField="RequestingDepot" UniqueName="RequestingDepot" HeaderText="Requesting Depot" />
                

            </Columns>
        </MasterTableView>
        <ClientSettings AllowColumnsReorder="true" ReorderColumnsOnClient="true" AllowGroupExpandCollapse="true" ColumnsReorderMethod="Reorder">
            <Resizing AllowColumnResize="true" AllowRowResize="false" EnableRealTimeResize="false" ResizeGridOnColumnResize="false" ClipCellContentOnResize="true" />
            <ClientEvents OnRowContextMenu="RowContextMenu" />
            <Scrolling UseStaticHeaders="true" AllowScroll="true" ScrollHeight="500px" />
        </ClientSettings>
    </telerik:RadGrid>
    
    <telerik:RadContextMenu ID="RadMenu1" IsContext="True" runat="server" OnClientItemClicked="OnClick"
        ContextMenuElementID="none">
    </telerik:RadContextMenu>
    
    <div class="buttonbar" id="divCreateCollectionButtonBar" style="margin-top: 10px;"
        runat="server">
        <asp:Button ID="btnExportOrder" runat="server" Text="Export" OnClick="btnExportOrder_Click" />
        <asp:Button ID="btnCreateDeliveryNote" runat="server" Text="Generate Delivery Note" />
        <asp:Button ID="btnLoadingSummarySheet" runat="server" Text="Load Sheet" />
        <asp:Button ID="btnSaveGridSettings" runat="server" Text="Save Grid Layout" />
        <input type="button" id="btnColumnDisplayShow" runat="server" value="Configure Columns"
            onclick="ColumnDisplayShow()" />
    </div>

    <script type="text/javascript">
        var orders = "";
        var ordersSelectedOnDeliveryJob = "";
        var ordersOnDeliveryJobCrossDockPointIds = "";
        
        var defaultGroupageCollectionRunDeliveryPointId = "<%=(GroupageCollectionRunDeliveryPoint == null) ? "" : GroupageCollectionRunDeliveryPoint.PointId.ToString() %>";
        var defaultGroupageCollectionRunDeliveryPointDescription = "<%=GroupageCollectionRunDeliveryPoint.Description %>";
        
        var jobs = "";
        var isUpdating = false;
        var groupHandlingIsActive = false;
        
        var mtv = null;
        var menu = null;
        var dteDeliveryDate = null;
        var dteDeliveryTime = null;
        var dteManifestDate = null;
        var cboSubContractor = null;
        var txtSubContractRate = -1;
        var cboDriver = null;
        
        var totalWeight = <%=Weight.ToString() %>;
        var totalPallets = <%=NoPallets.ToString() %>;
        var totalSpaces = <%=NoPalletSpaces.ToString() %>;              
        var userID = <%=((Page.User) as Orchestrator.Entities.CustomPrincipal).IdentityId %>;
        var userName = "<%=((Page.User) as Orchestrator.Entities.CustomPrincipal).UserName %>";
        var btnRefresh = $get("<%=btnRefresh.ClientID %>");
        
        function DriverOnClientBlur(sender, eventArgs) {
            $('#txtManifestTitle').val(sender.get_text() + " - " + dteManifestDate.get_displayValue());
        }
        
        function CollectionPointAlterPosition(pointId) {
            var qs = "pointId=" + pointId;
            <%=dlgAddUpdatePointGeofence.ClientID %>_Open(qs);
        }
   
        function pageLoad()
        {
            menu = $find("<%=RadMenu1.ClientID %>");
            mtv = $find("<%=grdOrders.ClientID %>").get_masterTableView();
            dteDeliveryDate = $find("<%=dteDeliveryDate.ClientID %>");
            dteDeliveryTime = $find("<%=dteDeliveryTime.ClientID %>");
            dteManifestDate = $find("<%=dteManifestDate.ClientID %>");
            cboSubContractor = $find("<%=cboSubContractor.ClientID %>");
            txtSubContractRate = $find("<%=txtSubbyRate.ClientID %>");
            cboDriver = $find('ctl00_ContentPlaceHolder1_ucResource_cboDriver');
            cboDriver.add_selectedIndexChanged(DriverOnClientBlur);
            showTotals(false);
        }
        
        function manifestGeneration_CallBack(object, value)
        {
            if (value.length > 0 && $('#chkCreateLoadingSheet')[0].checked) {
                PageMethods.GenerateAndShowLoadingSheet(value, LoadingSheet_Success, LoadingSheet_Failure);
            }
            else {
                var url = "/groupage/collectionsNew.aspx"+ getCSIDSingle();

                location.href = url;

            }
        }
        
        function ManifestGeneration_Success(result) {
            var parts = result.toString().split(",");
            var qs = "jID=" + parts[0];
            <%=dlgManifestGeneration.ClientID %>_Open(qs);
        }

        function ManifestGeneration_Failure(error) {
            alert("Something went wrong when creating the Manifest.");
        }
        
        function LoadingSheet_Success(result) {
            <%=dlgLoadingSheet.ClientID %>_Open();
            location.href = location.href;
        }

        function LoadingSheet_Failure(error) {
            alert("Something went wrong when creating the Loading Sheet.");
        }
        
        function DisplayManifestLinkWindow(jobID, rmID, excludeFirstRow, usePlannedTimes, extraRows, showFullAddress)
        {
            var modalPopupBehavior = $find('programmaticModalPopupBehavior');
            modalPopupBehavior.show();
        }
        
        function NextAction() {
            var url = "/groupage/collectionsNew.aspx"+ getCSIDSingle();

            location.href = url;

        }
        
        $(document).ready(function() {
            $('#<%=grdOrders.ClientID %>_ctl00 tbody tr:not(.rgGroupHeader)').quicksearch({
                position: 'after',
                labelText: '',
                attached: '#grdFilterHolder',
                delay: 100
            });

            $('.ShowPointTooltip').each(function(i, item){
                $(item).qtip({
                            style: {name: 'dark',
                                    width:{min:176}
                            },
                            position: { adjust: { screen: true } },
                            content: {
                                            url:$(item).attr('rel') ,
                                            data: {pointId: $(item).attr('pointid'),instructionId: $(item).attr('instructionid')}, 
                                            method: 'get'
                                         }
                            }
                            
                            
                );
            });

            var checked = $(':checkbox[id*=chkShowAll]').prop("checked");
            $(':checkbox[id*=chkIncludeOrdersNotPlannedForCollection]').prop("disabled", checked);
            $(':checkbox[id*=chkIncludeOrdersXDockedAtOwnCompany]').prop("disabled", checked);
            $(':checkbox[id*=chkIncludeOrdersXDockedAtOtherCompany]').prop("disabled", checked);
            
            $(':checkbox[id*=chkShowAll]').click(function (index, ele) {
                var checked = $(this).prop("checked");
                $(':checkbox[id*=chkIncludeOrdersNotPlannedForCollection]').prop("disabled", checked);
                $(':checkbox[id*=chkIncludeOrdersXDockedAtOwnCompany]').prop("disabled", checked);
                $(':checkbox[id*=chkIncludeOrdersXDockedAtOtherCompany]').prop("disabled", checked);
            });

            // Resets the Add Order Button submission check on the page load.
            $('input:hidden[id*=hidCheckSubmit]').val("false");
        });   
        
        function openAlterPointWindow(sender)
        {
            var pointId = $(sender).attr('pointid');
            var url = "/Point/Geofences/GeofenceManagement.aspx?pointId=" + pointId;
            window.open(url, "Point",  "Width=1034, Height=600, Scrollbars=0, Resizable=1")
        }

        // Added to prevent the add order button being clicked more than once.
        function orderSubmitCheck(btnId)
        {
            if($('input:hidden[id*=hidCheckSubmit]').val() == "true")
                return false;
            else
            {                
                $('input:hidden[id*=hidCheckSubmit]').val("true");
                $('input[id*=' + btnId + ']').val("Please Wait...")
                CreateCollectionJob();
            }
        }
    </script>

</asp:Content>