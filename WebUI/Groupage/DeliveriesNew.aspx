<%@ Page Language="C#" AutoEventWireup="True" Inherits="Orchestrator.WebUI.Groupage.DeliveriesNew"
    EnableViewStateMac="false" MasterPageFile="~/default_tableless.Master" Title="Groupage - Deliveries"
    CodeBehind="DeliveriesNew.aspx.cs" EnableViewState="true" %>

<%@ Register TagPrefix="p1" TagName="BusinessTypeCheckList" Src="~/UserControls/BusinessTypeCheckList.ascx" %>
<%@ Register TagPrefix="p1" TagName="Point" Src="~/UserControls/point.ascx" %>
<%@ Register TagPrefix="p1" TagName="Resource" Src="~/UserControls/resource.ascx" %>
<%@ Register TagPrefix="cc1" Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI" %>

<asp:Content ContentPlaceHolderID="Header" runat="server">
    <style type="text/css" media="print">
        #GridDataDiv_Orchestrator
        {
            height: 10000px;
        }
    </style>

    <script type="text/javascript" src="/script/jquery-ui-1.9.2.min.js"></script>

    <script type="text/javascript" src="/script/jquery.blockUI-2.64.0.min.js"></script>

    <!-- "reload" querystring below to force browser to get latest javascript file changes.  Change date value when subsequent changes to js file are made. -->
    <script type="text/javascript" src="DeliveriesNew.aspx.js?reload=20150106"></script>

    <script type="text/javascript" language="javascript" src="/script/tooltippopups.js"></script>

    <script type="text/javascript" language="javascript" src="/script/jquery.quicksearch-1.3.1.js"></script>

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
        .bookInWindow
        {
            position: absolute;
            z-index: 10;
            width: 390px;
            height: 150px;
            text-align: center;
            font: 14px Verdana, Arial, Helvetica, sans-serif;
            background: url(../images/white.png);
            padding: 12px 10px 12px 17px;
            display: none;
        }
        .titleBar
        {
            text-align: left;
            vertical-align: middle;
            font-weight: bold;
        }
        
        
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1 class="deliveriesIcon">Deliveries</h1>
</asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <cc1:Dialog ID="dlgAddUpdatePointGeofence" runat="server" Width="1034" Height="600" Mode="Normal" URL="/Point/Geofences/GeofenceManagement.aspx"></cc1:Dialog>
    <cc1:Dialog ID="dlgManifestGeneration" runat="server" Width="1200" Height="900" Mode="Normal" URL="/Reports/Reportviewer.aspx?wiz=true" ReturnValueExpected="true"></cc1:Dialog>
    <cc1:Dialog ID="dlgLoadingSheet" runat="server" Width="1200" Height="900" Mode="Normal" URL="/Reports/Reportviewer.aspx?wiz=true" ReturnValueExpected="false"></cc1:Dialog>
    <cc1:Dialog ID="dlgPilLabel" runat="server" Width="1200" Height="900"  Mode="Normal" URL="/Reports/Reportviewer.aspx?wiz=true" ReturnValueExpected="false"></cc1:Dialog>
    
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
                                    <telerik:RadDatePicker ID="dteStartDate" runat="server"  ToolTip="The start date for the filter">
                                    <DateInput runat="server"
                                    DateFormat="dd/MM/yy">
                                    </DateInput>
                                    </telerik:RadDatePicker>
                                </td>
                                <td class="formCellLabel">
                                    Date To
                                </td>
                                <td class="formCellField">
                                    <telerik:RadDatePicker ID="dteEndDate" runat="server"  ToolTip="The end date for the filter">
                                    <DateInput Runat="server"
                                    DateFormat="dd/MM/yy">
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
                        Client
                    </td>
                    <td class="formCellField" valign="top">
                        <telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                            AutoPostBack="false" MarkFirstMatch="true" Height="300px" Overlay="true" CausesValidation="False"
                            ShowMoreResultsBox="false" Width="350px" AllowCustomText="True" OnClientItemsRequesting="cboClient_itemsRequesting">
                            <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetClients" />
                        </telerik:RadComboBox>
                    </td>
                    <td class="formCellLabel">
                        Collection Point
                    </td>
                    <td class="formCellField">
                        <p1:Point runat="server" ID="ucCollectionPointFilter" CanCreateNewPoint="false" ShowFullAddress="true"
                            CanClearPoint="true" CanUpdatePoint="false" ShowPointOwner="true" Visible="true"
                            IsDepotVisible="true" />
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                    </td>
                    <td class="formCellField">
                        <asp:CheckBox ID="chkShowNotPlanned" Checked="true" Text="Only Show orders that have NOT been planned for Delivery"
                            runat="server"></asp:CheckBox><br />
                        <asp:CheckBox ID="chkShowAll" Text="Show <b>ALL</b> approved orders" runat="server">
                        </asp:CheckBox>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Delivery Area
                    </td>
                    <td class="formCellField" colspan="3">
                        <asp:CheckBox ID="chkSelectAllTrafficAreas" runat="server" Checked="false" Text="Select all Traffic Areas"
                            Style="padding-left: 3px;" />
                        <asp:CheckBoxList ID="cblTrafficAreas" runat="server" DataValueField="TrafficAreaID"
                            DataTextField="Description" RepeatDirection="Horizontal" RepeatColumns="8">
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
                        Service Level
                    </td>
                    <td class="formCellField" colspan="3">
                        
                        <asp:CheckBoxList ID="cblServiceLevel" runat="server" DataValueField="OrderServiceLevelID" DataTextField="Description" RepeatDirection="Horizontal" RepeatColumns="5">
                        </asp:CheckBoxList>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Surcharges
                    </td>
                    <td class="formCellField" colspan="3">
                        <asp:CheckBoxList ID="cblSurcharges" runat="server" DataValueField="ExtraTypeID"
                            DataTextField="Description" RepeatDirection="Horizontal" RepeatColumns="7">
                        </asp:CheckBoxList>
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
                        Refusals
                    </td>
                    <td class="formCellField">
                        <asp:CheckBox runat="server" ID="chkOrderOnlyShowRefusals" Text="Only show Refusals" />
                        <asp:CheckBox Visible="false" runat="server" ID="chkOnTrailer" Text="On Trailer" />&nbsp;
                        <asp:CheckBox Visible="true" runat="server" ID="chkOffTrailer" Text="Off Trailer" />
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
            </table>
        </fieldset>
        <div class="buttonbar">
            <asp:Button ID="btnRefresh" runat="server" Text="Refresh" Width="75" />
            <asp:Button ID="btnConnectionError" runat="server" CausesValidation="False" OnClick="btnConnectionError_Click" Style="position: static; display: none" Text="Cancel" />
            <input type="button" id="Button2" runat="server" value="Cancel and close" onclick="FilterOptionsDisplayHide()" />
        </div>
    </div>

    <div class="overlayedClearDataBox" id="overlayedClearDataBox" style="display: none;">
        <div id="tabs">
            <ul>
                <li><a href="#tabs-1">Load builder</a></li>
                <li><a href="#tabs-2">Pickup point</a></li>
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
                            <legend>Runs</legend>To add an order to an existing run either click on a Del Run
                            ID in the Grid or click here:
                            <input type="button" value="Add To Existing Run" class="buttonClass" onclick="ShowUpdateJob();" />
                        </fieldset>
                        <table id="tblLoadBuilderOrders" class="DataGridStyle" style="width: 95%; font-weight: normal;
                            display: none;" cellpadding="2" cellspacing="0">
                            <tr class="DataGridListHeadSmall">
                                <th>
                                    DO
                                </th>
                                <th style="width: 30px;">
                                    ID
                                </th>
                                <th style="display: none;">
                                    From
                                </th>
                                <th style="width: 100px;">
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
                                        cursor: pointer; margin: 2px 2px 0 0;" /><input type="text" id="txtOrder" value="" style="width: 15px;
                                            color: Black;" />
                                </td>
                                <td style="width: 15px;" class="orderID">
                                    <span id="orderID">OrderID</span>
                                </td>
                                <td style="display: none;">
                                    <span id="collectionPoint">collectionpoint</span>
                                </td>
                                <td style="width: 135px;">
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
                </div>
            </div>
            <div id="tabs-2">
                <div class="loadBuilder" style="height: 355px; overflow-x: hidden; overflow-y: auto;">
                    <fieldset style="margin-bottom: 5px;">
                        <legend>Collect from</legend>
                        <table style="width: 315px;">
                            <tr>
                                <td>
                                    <asp:CheckBox ID="chkTrunkCollection" runat="server" Text="Pick these up from a Cross Dock point" />
                                </td>
                            </tr>
                            <tr id="trCollectionPoint">
                                <td>
                                    <table>
                                        <tr>
                                            <td colspan="2">
                                                <p1:Point runat="server" ID="ucCollectionPoint" ShowFullAddress="false" CanChangePoint="true"
                                                    CanCreateNewPoint="false" CanUpdatePoint="false" Width="300" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="formCellLabel" style="width: 33%;">
                                                Collection Date
                                            </td>
                                            <td class="formCellInput">
                                                <telerik:RadDateInput ID="dteCollectionDate" Width="65" runat="server" DateFormat="dd/MM/yy"
                                                    DisplayDateFormat="dd/MM/yy">
                                                </telerik:RadDateInput>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="formCellLabel">
                                                Collection Time
                                            </td>
                                            <td class="formCellInput">
                                                <telerik:RadDateInput ID="dteCollectionTime" runat="server" DateFormat="HH:mm" EmptyMessage="Anytime"
                                                    DisplayDateFormat="HH:mm" Width="65" SelectedDate="01/01/01 08:00">
                                                </telerik:RadDateInput>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                    <div id="collectionPointText" class="infoPanel" style="font-weight: normal;">
                        You will be picking up the orders from the cross dock location as specified. This
                        will create a run with 1 collection and 1 or more deliveries</div>
                </div>
            </div>
            <div id="tabs-3">
                <div class="loadBuilder" style="height: 355px; overflow-x: hidden; overflow-y: auto;
                    font-weight: normal;">
                    <fieldset>
                        <legend>Resource this</legend>
                        <asp:RadioButton ID="rbOwnResource" runat="server" GroupName="rbResource" Text="Use own resource" Checked="true" />
                        <asp:RadioButton ID="rbSubContractor" runat="server" GroupName="rbResource" Text="Use subcontractor" />
                        <asp:RadioButton ID="rbAllocation" runat="server" GroupName="rbResource" Text="Allocate" Visible="false" />
                        <div id="pnlOwnresource">
                            <p1:Resource runat="server" ID="ucResource" ShowPlanningCatgeory="True"></p1:Resource>
                        </div>
                        <div id="pnlSubContractor" style="display: none;">
                            <table>
                                <tr>
                                    <td colspan="2">
                                        <telerik:RadComboBox ID="cboSubContractor" runat="server" ShowMoreResultsBox="false"
                                            MarkFirstMatch="true" ItemRequestTimeout="500" Width="100%" EnableLoadOnDemand="true"
                                            OnClientSelectedIndexChanged="cboSubContractor_SelectedIndexChanged">
                                            <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetSubContractors" />
                                        </telerik:RadComboBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label for="rbWholeJob">
                                            Subcontract whole run</label><input type="radio" id="rbWholeJob" value="0" name="SubContractOption" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <label for="rbPerOrder">
                                            Subcontract per order</label><input type="radio" id="rbPerOrder" value="1" name="SubContractOption" checked="checked" />
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
                                <tr id="trTrunkDate" style="display: none;">
                                    <td class="formCellLabel">
                                        Trunk Date
                                    </td>
                                    <td class="formCellInput">
                                        <telerik:RadDateInput ID="dteTrunkDate" runat="server" DateFormat="dd/MM/yy" DisplayDateFormat="dd/MM/yy">
                                        </telerik:RadDateInput>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div id="pnlAllocate" style="display: none;">
                            <table>
                                <tr>
                                    <td colspan="2">
                                        <telerik:RadComboBox ID="cboAllocatedTo" runat="server" Skin="WindowsXP" Width="200" DropDownWidth="300" ShowMoreResultsBox="false" ItemRequestTimeout="500" EnableLoadOnDemand="true" OnClientTextChange="cboAllocatedTo_TextChange">
                                            <WebServiceSettings Path="~/ws/combostreamers.asmx" Method="GetSubContractors" />
                                        </telerik:RadComboBox>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div>
                            <input type="checkbox" id="chkApplyLoadNo" value="ApplyLoadNo" checked="checked" /><label
                                for="chkApplyLoadNo">Apply Load No to orders</label>
                            <input type="text"id="txtLoadNo" value="" />
                            <br />
                            <input type="checkbox" id="chkCreateOrderGroup" value="createOrderGroup"/><label
                                for="chkCreateOrderGroup">Create a group for the orders</label>
                            <br />
                            <input type="checkbox" id="chkMultipleRuns" value="multipleRuns"/><label
                                for="chkOrdersPerRun">Create a separate run for each</label>
                                <input type="text" id="txtOrdersPerRun" value="10" style="width:24px" maxlength="2"/>
                                orders
                            <br />
                             <div id="pnlCreateManifest" style="margin-bottom: 0px; margin-top: 0px;">
                                <input type="checkbox" id="chkCreateManifest" value="createManifest" /><label for="chkCreateManifest">Create
                                    and show the manifest</label>
                            </div>
                            <input type="checkbox" id="chkShowJobOnCreation" value="showJobOnCreation" /><label
                                for="chkShowJobOnCreation">Show the run details when created</label>
                            <br />
                            <input type="checkbox" id="chkPrintPodLabels" runat="server" value="printPods" /><asp:Label
                                for="chkPrintPodLabels" runat="server" ID="lblPrintPodLabels">Print POD labels</asp:Label>
                            <br />
                            <input type="checkbox" id="chkPrintPilLabels" value="printPils" /><asp:Label
                                for="chkPrintPilLabels" runat="server" ID="lblPrinttPilLabels">Print PIL labels</asp:Label>
                            <br />
                            <input type="checkbox" id="chkCreateLoadingSheet" value="createLoadingSheet" /><label
                                for="chkCreateLoadingSheet">Create and show the loading sheet</label>
                            <br />
                            <input type="checkbox" id="chkShowInProgress" value="2" checked="checked" /><label
                                for="chkShowInProgress">Show as communicated</label>
                        </div>
                    </fieldset>
                    <div id="manifestOptions" style="display: none;">
                        <fieldset>
                            <legend>Manifest options</legend>
                            <asp:RadioButtonList ID="rblOrdering" RepeatDirection="Horizontal" runat="server"
                                Style="display: none;">
                                <asp:ListItem Text="Order by Planned Times" Value="0" />
                                <asp:ListItem Text="Order by Run Booked Times" Value="1" Selected="True" />
                            </asp:RadioButtonList>
                            <input type="checkbox" id="chkExcludeFirstRow" value="1" checked="checked" /><label
                                for="chkExcludeFirstRow">Exclude the first row from the Manifest</label>
                            <br />
                            <input type="checkbox" id="chkShowFullAddress" value="1" /><label for="chkShowFullAddress">Show
                                the full address on the manifest</label>
                            <br />
                            <span style="width: 80px;">Manifest Date:</span><telerik:RadDateInput ID="dteManifestDate"
                                runat="server" OnClientDateChanged="ManifestDate_OnDateChanged">
                            </telerik:RadDateInput>
                            <br />
                            <span style="width: 80px;">Manifest Title :</span><input type="text" id="txtManifestTitle"
                                style="width: 250px;" />
                            <br />
                            Number of blank rows to include:<input type="text" id="txtExtraRows" value="0" style="width: 25px;" />
                        </fieldset>
                    </div>
                    <div class="buttonbar" style="margin-bottom: 1px; margin-top: 3px; display: none;" id="createDeliveryJob">
                        <input id="btnCreateDeliveryJob" type="button" value="Create Delivery Run" style="font-size: 11px;" onclick="if(!orderSubmitCheck('btnCreateDeliveryJob')) return false;" />
                        <input id="btnCancelCreateDeliveryJob" type="button" value="Cancel" style="font-size: 11px;" onclick="CancelAndResetScreen();" />
                    </div>
                </div>
            </div>
            <div class="selectedInfo">
                <span id="dvSelected" style="margin-top: 10px 0 0 0;">Number of Pallets : 0</span>
            </div>
            <div>
                <div id="divBusinessTypes">
                </div>
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
                    <div class="jobViewInner">
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
                        <input type="checkbox" id="chkLoadAndGo" checked="checked" value="0" onclick="SelectLoadAndGo(this);" /><label
                            id="lblLoadAndGo" for="chkLoadAndGo">Add selected orders as load and go.</label>
                        <br />
                        <input type="checkbox" id="chkUpdateManifest" value="updateManifest" checked="checked" /><label
                            id="lblUpdateManifest" for="chkUpdateManifest">Update and show the manifest</label>
                    </div>
                    <div class="buttonBar">
                        <input type="button" value="Add Order(s) to Run" onclick="UpdateDeliveryJob();" /><input
                            type="button" value="Cancel" onclick="CancelAndResetScreen();" />
                    </div>
                </div>
            </div>
            <div id="tabs-5" class="jobView">
            </div>
            <div id="tabs-6">
                <div class="jobView">
                    <table id="tblExistingOrders" class="DataGridStyle" style="width: 95%; font-weight: normal;
                        display: none;" cellpadding="2" cellspacing="0">
                        <tr class="DataGridListHeadSmall">
                            <th>
                                DO
                            </th>
                            <th style="width: 30px;">
                                ID
                            </th>
                            <th style="display: none;">
                                From
                            </th>
                            <th style="width: 100px;">
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
                                    margin: 2px 2px 0 0;" /><input type="text" id="Text1" value="0" style="width: 15px;
                                        color: Black;" />
                            </td>
                            <td style="width: 15px;" class="orderID">
                                <span id="orderID">OrderID</span>
                            </td>
                            <td style="display: none;">
                                <span id="collectionPoint">collectionpoint</span>
                            </td>
                            <td style="width: 135px;">
                                <span id="deliveryPoint">deliverypoint</span>
                            </td>
                            <td style="width: 15px;">
                                <span id="palletSpaces">palletspaces</span>
                            </td>
                            <td style="width: 15px;">
                                <span id="weight">weight</span>
                            </td>
                        </tr>
                    </table>
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
                                <asp:ListItem Text="Delivery Vehicle" Value="DeliveryVehicle" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Delivery Driver" Value="DeliveryDriver" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Business Type" Value="BusinessType" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Client" Value="Customer" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Collection Point" Value="CollectionPoint" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Collection Town" Value="CollectionTown" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Collect At" Value="CollectionDateTime" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Delivery Run" Value="DeliveryJob" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Drop Order" Value="DropOrder" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Delivery Order No" Value="DeliveryOrderNumber" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Customer Order No" Value="CustomerOrderNumber" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="References" Value="References" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Delivery Customer Name" Value="DeliveryCustomer" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Delivery Point" Value="DeliveryPoint" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Delivery Address Line 1" Value="DeliveryAddressLine1" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Delivery Address Line 2" Value="DeliveryAddressLine2" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Delivery Address Lines" Value="DeliveryAddressLines" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Goods Type" Value="GoodsType" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="No Pallets" Value="NoPallets" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Pallet Spaces" Value="Spcs" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Weight" Value="Weight" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Deliver By" Value="DeliveryDateTime" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Delivery Notes" Value="DeliveryNotes" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Traffic Notes" Value="TrafficNotes" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Surcharges" Value="Surcharges" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="Service Level" Value="OrderServiceLevelShortCode" Selected="True"></asp:ListItem>
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
                <input type="button" id="Button1" runat="server" value="Cancel" onclick="ColumnDisplayHide();" />
            </div>
        </div>
    </div>

    <asp:Panel ID="pnlResourceManifestLinkPopup" runat="server" Style="width: 400px;
        background-color: white; border-width: 2px; border-color: black; border-style: solid;
        padding: 20px; color: Silver; display: none;">
        <h2>
            Resource Manifest</h2>
        <p>
            The Resource Manifest could not be generated.
            <br />
            Please follow the link below to manually create / update.
        </p>
        <p>
            <a href="javascript:window.open('/manifest/resourcemanifestlist.aspx'); NextAction();">
                Create Resource Manifest</a></p>
        <div class="buttonBar">
            <asp:Button ID="btnClose" runat="server" Text="Ok" /></div>
    </asp:Panel>

    <asp:Button ID="btnTest" Text="Test Button" runat="server" Style="display: none;" />
    
    <ajaxToolkit:ModalPopupExtender runat="server" ID="extener" BehaviorID="programmaticModalPopupBehavior" TargetControlID="btnTest" BackgroundCssClass="modalBackground" PopupControlID="pnlResourceManifestLinkPopup" OkControlID="btnClose" OnOkScript="javascript:NextAction();"></ajaxToolkit:ModalPopupExtender>
    
    <input type="hidden" id="radGridClickedRowIndex" name="radGridClickedRowIndex" />
    <asp:HiddenField ID="hidCheckSubmit" runat="server" Value="" />

    <telerik:RadGrid runat="server" ID="grdDeliveries" AllowPaging="false" AllowSorting="true" AutoGenerateColumns="false" AllowMultiRowSelection="true">
        <MasterTableView CommandItemDisplay="Top" GroupLoadMode="Client" CommandItemStyle-BackColor="White" DataKeyNames="Weight,NoPallets,PalletSpaces" ClientDataKeyNames="OrderGroupID" TableLayout="Fixed">
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
                <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()">
                    Show filter Options</div>
                <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()"
                    style="display: none;">
                    Close filter Options</div>
                <div class="overlayedRefreshIcon">
                    <asp:Button ID="btnRefreshTop" runat="server" Text="Refresh" OnClick="btnRefresh_Click_NoFilterUpdates" /></div>
                <asp:Label ID="lblQF" runat="server" Text="Quick Filter:" />
                <div id="grdFilterHolder">
                </div>
            </CommandItemTemplate>
            <Columns>
                <telerik:GridTemplateColumn UniqueName="chkSelectColumn" HeaderStyle-Width="30px"
                    Resizable="true">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkOrderID" runat="server" orderID='<%# ((System.Data.DataRowView)Container.DataItem)["OrderID"].ToString() %>' />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Del Veh" DataField="DeliveryVehicleRegNo" UniqueName="DeliveryVehicle"
                    HeaderStyle-Width="30px" />
                <telerik:GridTemplateColumn HeaderText="Driver" UniqueName="DeliveryDriver" ItemStyle-Width="60px">
                    <ItemTemplate>
                        <asp:PlaceHolder ID="plcDeliveryDriver" runat="server">&nbsp;</asp:PlaceHolder>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="BT" DataField="Businesstype" SortExpression="BusinessType"
                    UniqueName="BusinessType" HeaderStyle-Width="20" />
                <telerik:GridTemplateColumn HeaderText="Group" ItemStyle-Width="20px" HeaderStyle-Width="20px" UniqueName="GroupedOrderImage">
                    <ItemTemplate>
                        <asp:PlaceHolder ID="plcGroupedOrderImage" runat="server">&nbsp;</asp:PlaceHolder>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="ID" SortExpression="OrderID" UniqueName="OrderID"
                    ItemStyle-Width="80" HeaderStyle-Width="55" Resizable="true">
                    <ItemTemplate>
                        <span><a onclick='javascript:viewOrderProfile(<%# ((System.Data.DataRowView)Container.DataItem)["OrderID"].ToString() %>); return false;'
                            target="_blank" href="#">
                            <%# ((System.Data.DataRowView)Container.DataItem)["OrderID"].ToString()%>
                        </a>
                            <img style="padding-left: 2px;" runat="server" id="imgOrderBookedIn" visible="false"
                                class="imgOrderBookedIn" src="/App_Themes/Orchestrator/Img/MasterPage/icon-tick-small.png"
                                alt="Booked In" />
                            <img style="padding-left: 2px;" runat="server" id="imgGoodsType" visible="false"
                                src="/App_Themes/Orchestrator/Img/MasterPage/icon-hazard.png" alt="Hazardous Goods Type" />
                        </span>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Client" SortExpression="Customer" DataField="Customer"
                    HeaderStyle-Width="80" UniqueName="Customer" />
                 <telerik:GridTemplateColumn HeaderText="Coll Point" SortExpression="CollectionPoint" ItemStyle-Font-Bold="true" UniqueName="CollectionPoint" >
                    <ItemTemplate>
                         <a href="#" onclick class="ShowPointTooltip" rel="/point/getpointaddresshtml.aspx" onclick="openAlterPointWindow(this);" pointid="<%# Eval("CollectionPointId") == DBNull.Value ? -1 : Eval("CollectionPointId") %>" instructionid="" >
                            <%# Eval("CollectionPoint")%>
                         </a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Coll Town" SortExpression="CollectionTown" DataField="CollectionTown"
                    ItemStyle-Font-Bold="true" UniqueName="CollectionTown" />
                <telerik:GridTemplateColumn HeaderText="Collect At" SortExpression="CollectionDateTime" UniqueName="CollectionDateTime">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblCollectAt"></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Run" HeaderStyle-Width="60" UniqueName="DeliveryJob" SortExpression="DeliveryJobID"
                    Resizable="true">
                    <ItemTemplate>
                        <a id="lnkUpdateJob" href="#" onclick='LoadJob(<%# Eval("DeliveryJobID") %>);' style='<%# Eval("DeliveryjobID") != DBNull.Value ? "" : "display:none;" %>'>
                            <%# Eval("DeliveryJobID") %></a> <a href="#" onclick='ViewJob(<%# Eval("DeliveryJobID") %>);'
                                style='<%# Eval("DeliveryjobID") != DBNull.Value ? "" : "display:none;"%>'>View</a>
                        <input id="chkJobID" runat="server" onclick='<%# Eval("DeliveryJobID") != DBNull.Value ? "LoadingSummarySheetSelection(this);" : "" %>'
                            value='<%# Eval("DeliveryJobID")%>' type="checkbox" style='<%# Eval("DeliveryJobID") != DBNull.Value ? "" : "display:none;" %>' />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="DO" SortExpression="DropOrder" DataField="DropOrder"
                    HeaderStyle-Width="20" Resizable="true" UniqueName="DropOrder" />
                <telerik:GridBoundColumn HeaderText="Del ref" SortExpression="DeliveryOrderNumber"
                    DataField="DeliveryOrderNumber" HeaderStyle-Width="80" UniqueName="DeliveryOrderNumber" />
                <telerik:GridBoundColumn HeaderText="Load ref" SortExpression="CustomerOrderNumber"
                    DataField="CustomerOrderNumber" HeaderStyle-Width="80" UniqueName="CustomerOrderNumber" />
                <telerik:GridTemplateColumn UniqueName="References" HeaderText="Refs">
                    <ItemTemplate>
                        <%# Eval("CustomerOrderNumber") %>
                        <%# Eval("DeliveryOrderNumber") %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Del Name" SortExpression="DeliveryCustomer"
                    DataField="DeliveryCustomer" UniqueName="DeliveryCustomer" />
                <telerik:GridTemplateColumn HeaderText="Del Point" SortExpression="DeliveryPoint" ItemStyle-Font-Bold="true" UniqueName="DeliveryPoint" >
                    <ItemTemplate>
                         <a href="#" class="ShowPointTooltip" rel="/point/getpointaddresshtml.aspx" onclick="openAlterPointWindow(this);" pointid="<%# Eval("DeliveryPointId") == DBNull.Value ? -1 : Eval("DeliveryPointId") %>" instructionid="" >
                            <%# Eval("DeliveryPoint")%>
                         </a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Del Addr 1" SortExpression="AddressLine1" DataField="AddressLine1"
                    UniqueName="DeliveryAddressLine1" />
                <telerik:GridBoundColumn HeaderText="Del Addr 2" SortExpression="AddressLine2" DataField="AddressLine2"
                    UniqueName="DeliveryAddressLine2" />
                <telerik:GridTemplateColumn HeaderText="Del Addr" UniqueName="DeliveryAddressLines">
                    <ItemTemplate>
                        <%# string.IsNullOrEmpty(Eval("AddressLine1").ToString()) ? "" : Eval("AddressLine1").ToString() %>
                        <%# string.IsNullOrEmpty(Eval("AddressLine2").ToString()) ? "" : "<br/>" + Eval("AddressLine2").ToString() %>
                        <%# string.IsNullOrEmpty(Eval("AddressLine3").ToString()) ? "" : "<br/>" + Eval("AddressLine3").ToString() %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Del Town" SortExpression="PostTown" DataField="PostTown"
                    ItemStyle-Font-Bold="true" UniqueName="DeliveryTown" />
                <telerik:GridTemplateColumn HeaderText="Post Code" SortExpression="PostCode" DataField="PostCode"
                    UniqueName="PostCode" HeaderStyle-Width="40" Resizable="true">
                    <ItemTemplate>
                        <asp:PlaceHolder ID="phPostCode" runat="server"></asp:PlaceHolder>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Goods Type"  DataField="GoodsTypeDescription"
                    HeaderStyle-Width="20" Resizable="true" UniqueName="GoodsType" />
                <telerik:GridBoundColumn HeaderText="Plts" SortExpression="NoPallets" DataField="NoPallets"
                    UniqueName="NoPallets" HeaderStyle-Width="25" />
                <telerik:GridBoundColumn HeaderText="Spcs" SortExpression="Spcs" DataField="PalletSpaces"
                    UniqueName="Spcs" DataFormatString="{0: 0.##}" HeaderStyle-Width="25" />
                <telerik:GridBoundColumn HeaderText="Kg" SortExpression="Weight" DataField="Weight"
                    UniqueName="Weight" DataFormatString="{0:0}" HeaderStyle-Width="25" />
                <telerik:GridTemplateColumn HeaderText="Deliver By" SortExpression="DeliveryDateTime" UniqueName="DeliveryDateTime">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblDeliverAt"></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Delivery Notes" DataField="DeliveryNotes"
                    UniqueName="DeliveryNotes">
                    <ItemTemplate>
                        <%# string.IsNullOrEmpty(Eval("DeliveryNotes").ToString()) ? "" : Eval("DeliveryNotes").ToString().Substring(0, Math.Min(100, Eval("DeliveryNotes").ToString().Length)) + (Eval("DeliveryNotes").ToString().Length > 100 ? "..." : "") %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Traffic Notes" DataField="TrafficNotes"
                    UniqueName="TrafficNotes">
                    <ItemTemplate>
                        <%# string.IsNullOrEmpty(Eval("TrafficNotes").ToString()) ? "" : Eval("TrafficNotes").ToString().Substring(0, Math.Min(100, Eval("TrafficNotes").ToString().Length)) + (Eval("TrafficNotes").ToString().Length > 100 ? "..." : "") %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="Surcharges" UniqueName="Surcharges" HeaderText="Surcharges" />
                <telerik:GridBoundColumn DataField="OrderServiceLevelShortCode" UniqueName="OrderServiceLevelShortCode"
                    HeaderText="SL" HeaderStyle-Width="55" />
            </Columns>
        </MasterTableView>
        <ClientSettings AllowColumnsReorder="true" ReorderColumnsOnClient="true" AllowGroupExpandCollapse="true" ColumnsReorderMethod="Reorder">
            <Resizing AllowColumnResize="true" AllowRowResize="false" EnableRealTimeResize="false" ResizeGridOnColumnResize="true" ClipCellContentOnResize="true" />
            <ClientEvents OnRowContextMenu="RowContextMenu" />
            <Scrolling UseStaticHeaders="true" AllowScroll="true" ScrollHeight="600px" />
        </ClientSettings>
    </telerik:RadGrid>

    <telerik:RadContextMenu ID="RadMenu1" IsContext="True" runat="server" Skin="Outlook"
        OnClientItemClicked="OnClick" ContextMenuElementID="none">
    </telerik:RadContextMenu>

    <div class="buttonbar">
        <asp:Button ID="btnExportOrder" runat="server" Text="Export" OnClick="btnExportOrder_Click" />
        <asp:Button ID="btnCreateDeliveryNote" runat="server" Text="Generate Delivery Note" />
        <asp:Button ID="btnLoadingSummarySheet" runat="server" Text="Load Sheet" /><span
            style="width: 50px;">&nbsp;</span><asp:Button ID="btnSaveGridSettings" runat="server"
                Text="Save Grid Layout" />
        <asp:Button ID="btnResetGridLayout" runat="server" Text="Reset Grid Layout" />
        <input type="button" id="btnColumnDisplayShow" runat="server" value="Configure Columns"
            onclick="ColumnDisplayShow()" />
    </div>

    <div id="bookInWindow" class="bookInWindow">
        <table style="text-align: left; margin-left: 20px;">
            <tr>
                <td colspan="2">
                    <div class="titleBar">
                        Book in details</div>
                </td>
            </tr>
            <tr>
                <td style="width: 120px;">
                    Booked In With
                </td>
                <td style="text-align: left;">
                    <input type="text" style="width: 150px;" id="txtBookedInWith" />
                </td>
            </tr>
            <tr>
                <td>
                    References
                </td>
                <td style="text-align: left;">
                    <input type="text" style="width: 200px;" id="txtBookedInReferences" />
                </td>
            </tr>
            <tr>
                <td>
                    Booked in for
                </td>
                <td>
                    <div id="bookedInTime">
                        <input type="radio" id="optTimeW" name="BookInTime" value="0" checked="checked" />Window
                        <input type="radio" id="optTimeT" name="BookInTime" value="1" />Timed
                        <br />
                        <asp:TextBox ID="txtBookInFromDate" runat="server" Width="70px" EnableViewState="false" />
                        <asp:TextBox ID="txtBookInFromTime" runat="server" Width="40px" EnableViewState="false" />
                        <asp:TextBox ID="txtBookInByFromDate" runat="server" Width="70px" EnableViewState="false" />
                        <asp:TextBox ID="txtBookInByFromTime" runat="server" Width="40px" EnableViewState="false" />
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <div class="buttonbar">
                        <input type="button" value="Save" id="btnSaveBookIn" onclick="BookIn(this);" />
                        <input type="button" value="Cancel" onclick="$('#bookInWindow').hide();" />
                    </div>
                </td>
            </tr>
        </table>
    </div>

    <telerik:RadInputManager ID="rimDeliveries" runat="server">
        <telerik:DateInputSetting BehaviorID="DateInputBehaviour" InitializeOnClient="false"
            Culture="en-GB" EmptyMessage="Enter a valid Date" DateFormat="dd/MM/yy" SelectionOnFocus="SelectAll"
            EnabledCssClass="DateControl" FocusedCssClass="DateControl_Focused" HoveredCssClass="DateControl_Hover"
            InvalidCssClass="DateControl_Error" DisplayDateFormat="dd/MM/yy">
        </telerik:DateInputSetting>
        <telerik:DateInputSetting BehaviorID="TimeInputBehaviour" InitializeOnClient="false"
            Culture="en-GB" EmptyMessage="Anytime" DateFormat="HH:mm" DisplayDateFormat="HH:mm"
            EnabledCssClass="TimeControl" FocusedCssClass="TimeControl_Focused" HoveredCssClass="TimeControl_Hover"
            InvalidCssClass="TimeControl_Error">
        </telerik:DateInputSetting>
        <telerik:NumericTextBoxSetting BehaviorID="NumericInputBehaviour" InitializeOnClient="false"
            Culture="en-GB" Type="Currency" DecimalDigits="2" MinValue="0" EnabledCssClass="TextControl"
            FocusedCssClass="TextControl_Focused" HoveredCssClass="TextControl_Hover" InvalidCssClass="TextControl_Error">
        </telerik:NumericTextBoxSetting>
    </telerik:RadInputManager>

    <script type="text/javascript">
        var orders = "";
        var jobs = "";
        var isUpdating = false;
        var groupHandlingIsActive = false;

        var mtv = null;
        var grdOrders = null;
        var menu = null;
        var dteCollectionDate = null;
        var dteCollectionTime = null;
        var dteManifestDate = null;
        var cboSubContractor = null;
        var cboAllocatedTo = null;
        var txtSubContractRate = -1;
        var cboDriver = null;
        var palletNetworkID = <%=Orchestrator.Globals.Configuration.PalletNetworkID %>;
        var createGroupByDefault = "<%=Orchestrator.Globals.Configuration.LoadBuilderDefaultCreateGroup.ToString() %>";

        var totalWeight = <%=Weight.ToString() %>;
        var totalPallets = <%=NoPallets.ToString() %>;
        var totalSpaces = <%=NoPalletSpaces.ToString() %>;              
        var userID = <%=((Page.User) as Orchestrator.Entities.CustomPrincipal).IdentityId %>;
        var userName = "<%=((Page.User) as Orchestrator.Entities.CustomPrincipal).UserName %>";
        var palletNetworkBusinessTypeID = <%=this.PalletNetworkBusinessTypeID %>;
        var btnRefresh = $get("<%=btnRefresh.ClientID %>");
        
        var dteBookInFromDate = "#<%=txtBookInFromDate.ClientID %>";
        var dteBookInFromTime = "#<%=txtBookInFromTime.ClientID %>";
        var dteBookInByFromDate = "#<%=txtBookInByFromDate.ClientID %>";
        var dteBookInByFromTime = "#<%=txtBookInByFromTime.ClientID %>";
        var dteTrunkDate = null;
       
        function DriverOnClientBlur(sender, eventArgs) {
            $('#txtManifestTitle').val(sender.get_text() + " - " + dteManifestDate.get_displayValue());
        }
        
        function DeliverPointAlterPosition(pointId) {
            var qs = "pointId=" + pointId;
            <%=dlgAddUpdatePointGeofence.ClientID %>_Open(qs);
        }
    
        function pageLoad()
        {
            menu = $find("<%=RadMenu1.ClientID %>");
            mtv = $find("<%=grdDeliveries.ClientID %>").get_masterTableView();
            grdOrders = $find("<%=grdDeliveries.ClientID%> %>");
            dteCollectionDate = $find("<%=dteCollectionDate.ClientID%>");
            dteCollectionTime = $find("<%=dteCollectionTime.ClientID%>");
            dteManifestDate = $find("<%=dteManifestDate.ClientID%>");
            cboSubContractor = $find("<%=cboSubContractor.ClientID %>");
            txtSubContractRate = $find("<%=txtSubbyRate.ClientID %>");
            cboAllocatedTo = $find("<%=cboAllocatedTo.ClientID %>");
            dteTrunkDate = $find("<%=dteTrunkDate.ClientID %>");
            cboDriver = $find('ctl00_ContentPlaceHolder1_ucResource_cboDriver');
            cboDriver.add_selectedIndexChanged(DriverOnClientBlur);
            cboDriver.add_onClientBlur(DriverOnClientBlur);
            showTotals(false);
            
        }
        
        function refreshConnection()
        {
            alert("There was an error processing your request, please try again");
            document.getElementById('<%=btnConnectionError.ClientID%>').click();
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
            <%=dlgLoadingSheet.ClientID %>_Open('rpk=' + result.d);
        }

        function LoadingSheet_Failure(error) {
            alert("Something went wrong when creating the Loading Sheet.");
        }

        function GenerateAndShowPil_Success(result) {
            <%=dlgPilLabel.ClientID %>_Open('rpk=' + result.d);
        }

        function GenerateAndShowPil_Failure(error) {
            alert("Something went wrong when creating the PILs.");
        }
        
        function DisplayManifestLinkWindow(jobID, rmID, excludeFirstRow, usePlannedTimes, extraRows, showFullAddress)
        {
            var modalPopupBehavior = $find('programmaticModalPopupBehavior');
            modalPopupBehavior.show();
        }
        
        function NextAction() {

            var url = "/groupage/deliveriesNew.aspx"+ getCSIDSingle();

            location.href = url;
        }
        
        function GridCreated(sender, args)  
        {  
            var masterTableHeader = sender.get_masterTableViewHeader().get_element();  
            var masterTable = sender.get_masterTableView().get_element();  
            if (masterTable.clientHeight < masterTable.parentNode.clientHeight)  
            {  
                masterTableHeader.parentNode.style.marginRight = "0px";  
                masterTableHeader.parentNode.style.paddingRight = "16px";  
                masterTable.style.width = "100%";  
                masterTableHeader.style.width = "100%";  
                masterTable.style.tableLayout = "auto";  
                masterTableHeader.style.tableLayout = "auto";  
                sender._scrolling.initializeAutoLayout();  
                sender.repaint();  
            }  
        } 
        
        $(document).ready(function() {
            $('#<%=grdDeliveries.ClientID %>_ctl00 tbody tr:not(.rgGroupHeader)').quicksearch({
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

            var setShowNotPlannedEnabled = function() {
                var isShowAllChecked = $(':checkbox[id$=chkShowAll]').prop('checked');
                var chkShowNotPlanned = $(':checkbox[id$=chkShowNotPlanned]');
                chkShowNotPlanned.add(chkShowNotPlanned.parent('span')).prop('disabled', isShowAllChecked);
            }
            
            setShowNotPlannedEnabled();

            $(':checkbox[id*=chkShowAll]').click(function() {
                setShowNotPlannedEnabled();
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
                CreateDeliveryJob();
            }
        }
    </script>

</asp:Content>
