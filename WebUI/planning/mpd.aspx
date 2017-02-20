<%@ Page Language="C#" AutoEventWireup="True" Inherits="Orchestrator.WebUI.planning.mpd"
    EnableViewStateMac="false" MasterPageFile="~/default_tableless.Master" Title="Deliveries"
    CodeBehind="mpd.aspx.cs" EnableViewState="true" %>

<%@ Register TagPrefix="p1" TagName="BusinessTypeCheckList" Src="~/UserControls/BusinessTypeCheckList.ascx" %>
<%@ Register TagPrefix="p1" TagName="Point" Src="~/UserControls/point.ascx" %>
<%@ Register TagPrefix="p1" TagName="Resource" Src="~/UserControls/resource.ascx" %>
<%@ Register Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI"
    TagPrefix="cc1" %>

<asp:Content ContentPlaceHolderID="Header" runat="server">
    <style type="text/css" media="print">
        #GridDataDiv_Orchestrator
        {
            height: 10000px;
        }
        
        .tablescroll {
    font: 12px normal Tahoma, Geneva, "Helvetica Neue", Helvetica, Arial, sans-serif;
}
 
 .tblClusterOrders
 {
     height:300px;
 }
 
.tablescroll td, 
.tablescroll_wrapper,
.tablescroll_head,
.tablescroll_foot { 
    border:1px solid #ccc;
}
 
.tablescroll td {
    padding:3px 5px;
    border-bottom:0;
    border-right:0; 
    font-size:10px;
}
 
.tablescroll_wrapper {
    background-color:#fff;
    border-left:0;
}
 
.tablescroll_head,
.tablescroll_foot { 
    background-color:#eee;
    border-left:0;
    border-top:0;
    font-size:11px;
    font-weight:bold;
}
 
.tablescroll_head { 
    margin-bottom:3px;
}
 
.tablescroll_foot { 
    margin-top:3px;
}
 
.tablescroll tbody tr.first td { 
    border-top:0; 
}

.tablescroll_head thead tr td
{
    text-align:left;
}

    </style>

    <!--Here Maps Scripts-->
    <script src="<%=HereMapsCoreJS%>"></script>
    <script src="<%=HereMapsServiceJS%>"></script>
    <script src="<%=HereMapsEventsJS%>"></script>
    <script src="<%=HereMapsUIJS%>"></script>
    <script src="<%=HereMapsClusteringJS%>"></script>
    <link rel="stylesheet" type="text/css" href="<%=HereMapsUICSS%>" />
    <!-- End Here Maps Scripts -->

    <script type="text/javascript" src="/script/jquery-ui-1.9.2.min.js"></script>
    <script type="text/javascript" src="jquery.tablescroll.js"></script>

    <script type="text/javascript" src="/script/jquery.blockUI-2.64.0.min.js"></script>

    <!-- "reload" querystring below to force browser to get latest changes, Print POD use cached version.  Change date value when subsequent changes to js file are made. -->
    <script type="text/javascript" src="DeliveriesNew.aspx.js?reload=20150106"></script>

    <script type="text/javascript" language="javascript" src="/script/tooltippopups.js"></script>

    <script type="text/javascript" language="javascript" src="/script/jquery.quicksearch-1.3.1.js"></script>
    <script src="../bower_components/underscore/underscore.js"></script>

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
     <div style="position:absolute; z-index:1200; background:rgba(255, 255, 255, 0.43); padding-right: 5px;">
     <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()">
                    Show filter Options</div>
                <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()"
                    style="display: none;">
                    Close filter Options</div>
                <div class="overlayedFilterIconOff" id="Div1" onclick="window.location.reload()">
                    Refresh</div>
                   <asp:Button ID="btnRefreshTop" runat="server" Text="Refresh" OnClick="btnRefresh_Click" Visible="false" />
                    <div class="overlayedSearchIconOff" id="btnSearch" onclick="Find();">Search</div>
                    <div id="orderSearch" style="display:inline"><input type="text" id="txtMapSearch" /></div>

                    <div id="orderCount" style="margin-top:4px; display:inline;"></div>
                    <span id="invalidOrders" style="margin-top:4px; color:Red" ></span>
                    
   </div>
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
                                    <telerik:RadDateInput ID="dteStartDate" runat="server" DateFormat="dd/MM/yy" ToolTip="The start date for the filter">
                                    </telerik:RadDateInput>
                                </td>
                                <td class="formCellLabel">
                                    Date To
                                </td>
                                <td class="formCellField">
                                    <telerik:RadDateInput ID="dteEndDate" runat="server" DateFormat="dd/MM/yy" ToolTip="The end date for the filter">
                                    </telerik:RadDateInput>
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
                    <td></td>
                    <td class="formCellField" colspan="3">
                     <asp:CheckBox ID="chkShowNotPlanned" Checked="true" Text="Only Show orders that have NOT been planned for Delivery"
                            runat="server" ></asp:CheckBox><br />
                             <asp:CheckBox ID="chkShowAll" Text="Show <b>ALL</b> approved orders" runat="server">
                        </asp:CheckBox>
                    </td>
                </tr>
               
            </table>
        </fieldset>
         <div class="buttonbar">
            <asp:Button ID="btnRefresh" runat="server" Text="Refresh" Width="75" Visible="false" />
            <input type="button" id="Button2" runat="server" value="Cancel and close" onclick="FilterOptionsDisplayHide()" />
            <input type="button" id="btnLoadMap" value="Get Deliveries" onclick="LoadDeliveries()" />
            
        </div>
    </div>   
    <div class="container">
    <cc1:Dialog ID="dlgAddUpdatePointGeofence" runat="server" Width="1034" Height="600" Mode="Normal" URL="/Point/Geofences/GeofenceManagement.aspx"></cc1:Dialog>
    <cc1:Dialog ID="dlgManifestGeneration" runat="server" Width="1200" Height="900" Mode="Normal" URL="/Reports/Reportviewer.aspx?wiz=true" ReturnValueExpected="true"></cc1:Dialog>
    <cc1:Dialog ID="dlgLoadingSheet" runat="server" Width="1200" Height="900" Mode="Normal" URL="/Reports/Reportviewer.aspx?wiz=true" ReturnValueExpected="false"></cc1:Dialog>
    <cc1:Dialog ID="dlgPilLabel" runat="server" Width="1200" Height="900"  Mode="Normal" URL="/Reports/Reportviewer.aspx?wiz=true" ReturnValueExpected="false"></cc1:Dialog>
   

   
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
    
   <div id='myMap' style="position:absolute;width:100%; height:100%; "></div>
    


    <telerik:RadContextMenu ID="RadMenu1" IsContext="True" runat="server" Skin="Outlook"
        OnClientItemClicked="OnClick" ContextMenuElementID="none">
    </telerik:RadContextMenu>

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

        var menu = null;
        var dteCollectionDate = null;
        var dteCollectionTime = null;
        var dteManifestDate = null;

        var dteStartDate = null;
        var dteEnddate  = null;

        var cblTrafficAreas = null;
        var cboSubContractor = null;
        var cboAllocatedTo = null;
        var txtSubContractRate = -1;
        var cboDriver = null;
        var palletNetworkID = <%=Orchestrator.Globals.Configuration.PalletNetworkID %>;
        var createGroupByDefault = "<%=Orchestrator.Globals.Configuration.LoadBuilderDefaultCreateGroup.ToString() %>";

        var totalWeight =0;
        var totalPallets =0;
        var totalSpaces = 0;              
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

            dteStartDate = $find("<%=dteStartDate.ClientID %>");
            dteEnddate = $find("<%=dteEndDate.ClientID %>");
            cblTrafficAreas ="<%=cblTrafficAreas.ClientID %>";
            showTotals(false);
             LoadDeliveries();
        }
        
        function ManifestGeneration_Success(result) {
            var parts = result.toString().split(",");
            var qs = "jID=" + parts[0];
            <%=dlgManifestGeneration.ClientID %>_Open(qs);
            window.location.reload();
        }

        function ManifestGeneration_Failure(error) {
            alert("Something went wrong when creating the Manifest.");
            window.location.reload();
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

            var url = "mpd.aspx"+ getCSIDSingle();

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

        //HERE Maps
        var platform = new H.service.Platform({
                app_id: '<%=HereMapsApplicationId%>',
                app_code: '<%=HereMapsApplicationCode%>'
            });
    </script>
        </div>
</asp:Content>
