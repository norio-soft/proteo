<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Traffic.TSResource" Async="true"   Codebehind="TSResource.aspx.cs"  %>

<%@ Register TagPrefix="Componentart" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register TagPrefix="radP" Namespace="Telerik.WebControls" Assembly="RadPanelbar.Net2" %>
<%@ Register TagPrefix="cs" Namespace="Codesummit" Assembly="WebModalControls" %>
<%@ Register TagPrefix="cc1" Namespace="Orchestrator.WebUI" Assembly="Orchestrator.WebUI.Dialog" %>
<%@ Register TagPrefix="uc" TagName="MwfDriverMessaging" Src="~/UserControls/mwf/DriverMessaging.ascx" %>
<%@ Register TagPrefix="uc" TagName="DriverTime" Src="~/UserControls/DriverTime.ascx" %>

<!doctype html>
<html lang="en">

<head id="hdr" runat="server">
    <meta charset="utf-8" />

    <title>Haulier Enterprise - My Resource</title>
    <base target="_self" />

    <link href="/style/newStyles.css" rel="stylesheet" type="text/css" />
    <link href="/style/helpTip.css" rel="stylesheet" type="text/css" />
    <link href="/style/trafficsheetprint.css" type="text/css" rel="stylesheet" media="print" />
    <link href="/App_Themes/Orchestrator/CommonStyles.Orchestrator.css" type="text/css" rel="Stylesheet" />
     
    <style type="text/css">
        html,body
        {
          margin:0;
          padding:0;
          border:none;
        }
        .stateBooked  td.DataCell 
        { 
          cursor: default;
          padding: 3px; 
          padding-top: 2px; 
          padding-bottom: 1px; 
          border-bottom: 1px solid #EAE9E1; 
          font-family: verdana; 
          font-size: 11px; 
          background-color: #FFFFFF; 
          
        }

        .statePlanned td.DataCell 
        { 
            height:20px;
            background-color: #CCFFCC;
            cursor: default;
            border-bottom: 1px solid #EAE9E1; 
        }

        .stateInProgress td.DataCell 
        { 
            height:20px;
            background-color: #99FF99; 
            cursor: default;
            border-bottom: 1px solid #EAE9E1; 
        }

        .stateCompleted td.DataCell 
        { 
            height:20px;
            background-color: LightBlue; 
            cursor: default;
            border-bottom: 1px solid #EAE9E1; 
        }

        .Row td.DataCell 
        { 
        cursor: default;
        padding: 3px; 
        padding-top: 2px; 
        padding-bottom: 1px; 
        border-bottom: 1px solid #EAE9E1; 
        font-family: verdana; 
        font-size: 10px; 
        width:100%;
        } 

        .AlternatingRow td.DataCell 
        { 
        cursor: default;
        padding: 3px; 
        padding-top: 2px; 
        padding-bottom: 1px; 
        border-bottom: 1px solid #EAE9E1; 
        font-family: verdana; 
        font-size: 10px; 
        width:100%;
        }
        
        .tablefix 
        {
            display: table !important;
            width: 99% !important;    
        }
    </style>
     
    <script src="//ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js"></script>
    <script>window.jQuery || document.write('<script src="/script/jquery-1.10.2.min.js">\x3C/script>')</script>
    <script src="/script/jquery-migrate-1.2.1.js"></script>
    <script src="/script/show-modal-dialog.js"></script>
    <script type="text/javascript" src="/script/jquery-ui-1.9.2.min.js"></script>
    <script type="text/javascript" src="/script/jquery.blockUI-2.64.0.min.js"></script>    
    <script type="text/javascript" src="/script/popAddress.js"></script>
    <script type="text/javascript" src="/script/helptip.js"></script>
  
    <script type="text/javascript">
        var _driverTimeMenuIndex = 2;
        var _takeGoodsOffTrailerMenuIndex = 3;
        var _sendMwfMessageMenuIndex = 4;

        var _jobsComingInLoaded = false;
        var _driversComingInLoaded = false;
        var _vehiclesComingInLoaded = false;
        var _trailersComingInLoaded = false;

        var _MyVehicles = false;
        var _MyTrailers = false;

        //Resource Menu Handling

        var _jobId;
        var _instructionId;
        var _driver;
        var _driverResourceId;
        var _regNo;
        var _vehicleResourceId;
        var _trailerRef;
        var _trailerResourceId;
        var _instructionPlannedStart;
        var _instructionPlannedEnd;
        var _depotCode;
        var _lastUpdateDate;
        var _instructionStateId;
        var _linkJobSourceJobId;
        var _linkJobSourceInstructionId;
        var _gpsUnitID;

        var myDriverSelect = 0;
        var myTrailerSelect = 0;
        var myVehicleSelect = 0;
        var jobSelect = 0;
        var driverSelect = 0;
        var vehicleSelect = 0;
        var trailerSelect = 0;

        $(function() {
            $('[id$=grdMyDrivers_table]').on('contextmenu', function(e) {
                e.preventDefault();
                e.stopPropagation();
            });
        });

        function ShowContextMenuLite(jobId, instructionId, legPointId, rowId) {
            _jobId = jobId;
            _instructionId = instructionId;
            _legPointId = legPointId;

            var yMousePos = window.event.y;
            var xMousePos = window.event.x;

            // left
            xMousePos = window.event.clientX - 2 + document.body.scrollLeft;

            // top
            yMousePos = window.event.clientY + 18 + document.body.scrollTop;

            ResourceGridContextMenu.ShowContextMenu(xMousePos, yMousePos);
            return false;
        }

        var lastHighlightedRow = "";
        var lastHighlightedRowColour = "";
        var lastHighlightedRowClass = "";

        function HighlightRow(row) {
            var rowElement;

            if (lastHighlightedRow != "") {
                rowElement = document.getElementById(lastHighlightedRow);
                rowElement.style.backgroundColor = lastHighlightedRowColour;
                rowElement.className = lastHighlightedRowClass;
            }

            rowElement = document.getElementById(row);
            lastHighlightedRow = row;
            lastHighlightedRowColour = rowElement.style.backgroundColor;
            lastHighlightedRowClass = rowElement.className;
            rowElement.style.backgroundColor = "";
            rowElement.className = 'SelectRowTrafficSheetLite';

            if (event.button != 2) { ResourceGridContextMenu.Hide(); return; }

            ResourceGridContextMenu.ShowContextMenu(window.event);
        }

        function loadJobsComingIn(pnl) {
            //Callback Jobs Coming In
            if (pnl.ID == "Panel4" && !_jobsComingInLoaded) {
                Panel4_1_cbJobsComingIn.Callback();
                _jobsComingInLoaded = true;
            }

            //Callback Drivers Coming In
            if (pnl.ID == "Panel5" && !_driversComingInLoaded) {
                Panel5_1_cbDriversComingIn.Callback();
                _driversComingInLoaded = true;
            }

            //Callback Vehicles Coming In
            if (pnl.ID == "Panel6" && !_vehiclesComingInLoaded) {
                Panel6_1_cbVehiclesComingIn.Callback();
                _vehiclesComingInLoaded = true;
            }

            //Callback Trailers Coming In
            if (pnl.ID == "Panel7" && !_trailersComingInLoaded) {
                Panel7_1_cbTrailersComingIn.Callback();
                _trailersComingInLoaded = true;
            }

            //Callback My Vehicles
            if (pnl.ID == "Panel3" && !_MyVehicles) {
                Panel3_1_cbMyVehicles.Callback();
                _MyVehicles = true;
            }

            //Callback My Trailers
            if (pnl.ID == "Panel2" && !_MyTrailers) {
                Panel2_1_cbMyTrailers.Callback();
                _MyTrailers = true;
            }
        }

        var _driverResourceId;
        var _fullName;
        var _trailerResourceId;
        var _trailerRefNo;
        var _vehicleResourceId;
        var _vehicleRegNo;
        var _controlArea;
        var _trafficArea;

        function ShowDriverMenu(item) {
            _driverResourceId = item.GetMember("DriverResourceId").Value;
            _fullName = item.GetMember("FullName").Value;
            _vehicleResourceId = item.GetMember("VehicleResourceId").Value;
            _vehicleRegNo = item.GetMember("RegNo").Value;
            _trailerResourceId = null;
            _trailerRef = null;

            var allowMwfCommunication = item.GetMember("AllowMwfCommunication").Value;

            if (item.GetMember("ControlAreaId") != null)
                _controlArea = item.GetMember("ControlAreaId").Value;
            if (item.GetMember("TrafficAreaId") != null)
                _trafficArea = item.GetMember("TrafficAreaId").Value;
            if (item.GetMember("GPSUnitID") != null)
                _gpsUnitID = item.GetMember("GPSUnitID").Value;

            var driverTimeMenu = ResourceGridContextMenu.ChildItemArray[_driverTimeMenuIndex];
            contextMenuItemEnable(driverTimeMenu, true);

            var takeGoodsOffTrailerMenu = ResourceGridContextMenu.ChildItemArray[_takeGoodsOffTrailerMenuIndex];
            contextMenuItemEnable(takeGoodsOffTrailerMenu, false);

            var sendMwfMessageMenu = ResourceGridContextMenu.ChildItemArray[_sendMwfMessageMenuIndex];
            contextMenuItemEnable(sendMwfMessageMenu, allowMwfCommunication);

            ShowContextMenuLite(0, 0, 0, 0);
        }

        function ShowVehicleMenu(item) {
            _driverResourceId = null;
            _fullName = null;
            _vehicleResourceId = item.GetMember("VehicleResourceId").Value;
            _vehicleRegNo = item.GetMember("RegNo").Value;
            _trailerResourceId = null;
            _trailerRef = null;
            if (item.GetMember("ControlAreaId") != null)
                _controlArea = item.GetMember("ControlAreaId").Value;
            if (item.GetMember("TrafficAreaId") != null)
                _trafficArea = item.GetMember("TrafficAreaId").Value;

            if (item.GetMember("GPSUnitID") != null)
                _gpsUnitID = item.GetMember("GPSUnitID").Value;

            var driverTimeMenu = ResourceGridContextMenu.ChildItemArray[_driverTimeMenuIndex];
            contextMenuItemEnable(driverTimeMenu, false);

            var takeGoodsOffTrailerMenu = ResourceGridContextMenu.ChildItemArray[_takeGoodsOffTrailerMenuIndex];
            contextMenuItemEnable(takeGoodsOffTrailerMenu, false);
                
            var sendMwfMessageMenu = ResourceGridContextMenu.ChildItemArray[_sendMwfMessageMenuIndex];
            contextMenuItemEnable(sendMwfMessageMenu, false);

            ShowContextMenuLite(0, 0, 0, 0);
        }

        function ShowTrailerMenu(item) {
            _driverResourceId = null;
            _fullName = null;
            _vehicleResourceId = null;
            _vehicleRegNo = null;
            _trailerResourceId = item.GetMember("TrailerResourceId").Value;
            _trailerRef = item.GetMember("TrailerRef").Value;
            if (item.GetMember("ControlAreaId") != null)
                _controlArea = item.GetMember("ControlAreaId").Value;
            if (item.GetMember("TrafficAreaId") != null)
                _trafficArea = item.GetMember("TrafficAreaId").Value;

            if (item.GetMember("GPSUnitID") != null)
                _gpsUnitID = item.GetMember("GPSUnitID").Value;

            var driverTimeMenu = ResourceGridContextMenu.ChildItemArray[_driverTimeMenuIndex];
            contextMenuItemEnable(driverTimeMenu, false);
            driverTimeMenu.Visible = false;

            var takeGoodsOffTrailerMenu = ResourceGridContextMenu.ChildItemArray[_takeGoodsOffTrailerMenuIndex];
            var canTakeGoodsOff = item.GetMember("RefusalCount") != null && item.GetMember("RefusalCount").Value == true;
            contextMenuItemEnable(takeGoodsOffTrailerMenu, canTakeGoodsOff);
            
            var sendMwfMessageMenu = ResourceGridContextMenu.ChildItemArray[_sendMwfMessageMenuIndex];
            contextMenuItemEnable(sendMwfMessageMenu, false);

            ShowContextMenuLite(0, 0, 0, 0);
        }

        function ContextMenuClickHandler(item) {
            ResourceGridContextMenu.Hide();

            if (item.Text == "Give Resource") {
                openGiveResourcesWindow(_driverResourceId, _fullName, _vehicleResourceId, _vehicleRegNo, _trailerResourceId, _trailerRef, _controlArea, _trafficArea);
            }
            else if (item.Text == "Set Location") {
                openSetResourceLocation(_driverResourceId, _fullName, _vehicleResourceId, _vehicleRegNo, _trailerResourceId, _trailerRef);
            }
            else if (item.Text == "Show Location") {
                ShowPosition();
            }
            else if (item.Text == "Driver Time") {
                showDriverTime(_driverResourceId);
            }
            else if (item.Text == "Take Goods off Trailer") {
                PageMethods.TakeGoodsOffTrailer(_trailerResourceId, TakeGoodsOffTrailer_Success, TakeGoodsOffTrailer_Failure);
            }
            else if (item.Text == "Send MWF Message") {
                sendMwfMessage(_driverResourceId);
            }
        }

        function TakeGoodsOffTrailer_Success(result) {
            Panel2_1_cbMyTrailers.Callback();
        }

        function TakeGoodsOffTrailer_Failure(error) {
            alert("Error removing goods from trailer.");
        }

        function ShowPosition() {
            if (_gpsUnitID == null || _gpsUnitID == "")
                alert("There is no GPS Information available for this resource.");
            else {
                var url = "/gps/getcurrentlocation.aspx?uid=" + _gpsUnitID;
                window.open(url, "", "height=600, width=630, scrollbars=0");
            }
        }

        function reload() {
            location.href = location.href;
        }

        function contextMenuItemEnable(contextMenuItem, value) {
            if (value) {
                contextMenuItem.Enabled = true;
                contextMenuItem.CssClass = "MenuItem";
                contextMenuItem.LookId = "DefaultLookItem";
            }
            else {
                contextMenuItem.Enabled = false;
                contextMenuItem.CssClass = "DisabledMenuItem";
                contextMenuItem.LookId = "DisabledLookItem";
            }
        }

        function setTravelNotesCallBack(source, returnValue) {
            var values = JSON.parse(returnValue);
            document.getElementById("lnkSetTravelNotes_" + values.resourceID).innerHTML = values.notes;
        }

    </script>
</head>

<body style="background-color:#ffffff;" >
    <form id="form1" runat="server">
    
    <asp:ScriptManager ID="scriptManager" runat="server" EnablePageMethods="True" EnablePartialRendering="true"></asp:ScriptManager>
    
    <cc1:Dialog ID="dlgPCV" runat="server" URL="/PCV/ListpcvforJob.aspx" Width="500" Height="320" AutoPostBack="true" ReturnValueExpected="true" Mode="Normal" />
    <cc1:Dialog ID="dlgTravelNotes" runat="server" URL="/Traffic/setTravelNotes.aspx" Width="400" Height="320" AutoPostBack="False" ReturnValueExpected="true" OnClientDialogCallBack="setTravelNotesCallBack" Mode="Normal" />
    <cc1:Dialog ID="dlgLocation" runat="server" URL="/Traffic/updateResourceLocations.aspx" Width="400" Height="320" AutoPostBack="False" ReturnValueExpected="False" Mode="Normal" />
    <cc1:Dialog ID="dlgStartTime" runat="server" URL="/Resource/Driver/EnterDriverStartTimes.aspx" Width="470" Height="320" AutoPostBack="True" ReturnValueExpected="false" Mode="Normal" />
    <cc1:Dialog ID="dlgPalletHandling" runat="server" URL="/Traffic/JobManagement/AddUpdatePalletHandling.aspx" Width="1115" Height="750" AutoPostBack="true" ReturnValueExpected="true" Mode="Normal" />

    <cc1:Dialog ID="dlgGiveResources" runat="server" URL="giveResources.aspx" Width="400" Height="320" AutoPostBack="true" ReturnValueExpected="true" Mode="Normal" />
    <cc1:Dialog ID="dlgSetLocation" runat="server" URL="/Resource/setlocation.aspx" Width="500" Height="320" AutoPostBack="true" ReturnValueExpected="true" Mode="Normal" />
    
    <asp:Panel ID="pnlConfirmation" runat="server" Visible="false">
        <div class="MessagePanel">
            <asp:Label ID="lblMessage" runat="server"><asp:Image ID="imgIcon" runat="server" ImageUrl="~/images/ico_warning.gif" />You do not have a default filter, please contact support.</asp:Label>
        </div>
    </asp:Panel>
    
	<ComponentArt:Menu id="ResourceGridContextMenu" 
        SiteMapXmlFile="resourceMenu.xml"
        ExpandSlide="none"
        ExpandTransition="fade"
        ExpandDelay="250"
        CollapseSlide="none"
        CollapseTransition="fade"
        Orientation="Vertical"
        CssClass="MenuGroup"
        DefaultGroupCssClass="MenuGroup"
        DefaultItemLookID="DefaultItemLook"
        DefaultGroupItemSpacing="1"
        ImagesBaseUrl="~/images/"
        EnableViewState="false"
        ContextMenu ="Custom"
        runat="server"
        ClientSideOnItemSelect="ContextMenuClickHandler">
        
        <ItemLooks>
            <ComponentArt:ItemLook LookID="DefaultItemLook" CssClass="MenuItem" HoverCssClass="MenuItemHover" LabelPaddingLeft="15" LabelPaddingRight="10" LabelPaddingTop="3" LabelPaddingBottom="3" />
            <ComponentArt:ItemLook LookID="BreakItem" ImageUrl="break.gif" CssClass="MenuBreak" ImageHeight="1" ImageWidth="100%" />
            <ComponentArt:ItemLook LookId="DisabledItemLook" CssClass="DisabledMenuItem" HoverCssClass="DisabledMenuItem"  LabelPaddingLeft="15" LabelPaddingRight="10" LabelPaddingTop="3" LabelPaddingBottom="3" />
        </ItemLooks>
      
    </ComponentArt:Menu>
    
    <table class="HeadingCell" style="width:100%;height:25px;border-top: 0px; border-right: 0px;" cellpadding="0" cellspacing="0">
        <tr>
            <td class="HeadingCellText" nowrap="nowrap">My Resource</td>
            <td width="100%" align="center">
                <span style="background-color:White;padding:2px;">
                <font color="<%=DepotId > 0 ? "red" : "green" %>"><b>Drawing resource from <%=DepotId > 0 ? "Depot" : "Areas" %></b></font>
                </span>
            </td>
            <td align="right" nowrap="nowrap">
                <a id="lnkClearSection" href="javascript:ClearSelection();" style="color:White;">Clear Selection</a>
                &nbsp;&nbsp;
                <a id="lnkReload" href="javascript:location.href=location.href;" style="color:White;">Reload</a>
            </td>
        </tr>
    </table>
    
    <radP:RadPanelbar 
            ID="RadPanelbar1" 
            Runat="server"
            ContentFile="navdataxml.xml"
            Width="100%" height="4000px"
            SingleExpandedPanel="True"
            FullExpandedPanels="True"
            ExpandEffect="Fade"
            ExpandEffectSettings="duration=0.4"
            RadControlsDir="~/script/radcontrols/"
            ImagesBaseDir="/App_themes/orchestrator/RadPanelbar/img/" 
            AfterClientPanelItemExpanded="loadJobsComingIn">
        <PanelItemTemplates>
        
            <radP:PanelItemTemplate ID="templateMyDrivers" runat="server">
                <ContentTemplate>
                    <div style="background-image:url('/images/panelbar/collapsedbg.gif'); background-repeat:repeat-y;">
                        <asp:CheckBox ID="chkShowDriversWithoutReloads" runat="server" Text="Only Show Drivers Without Reload" AutoPostBack="true" OnCheckedChanged="ShowDriverReload_CheckedChanged" />
                    </div>
                    
                    <ComponentArt:Grid id="grdMyDrivers" 
                            RunningMode="Client" 
                            GroupingNotificationTextCssClass="GridHeaderText"
                            GroupBySortAscendingImageUrl="group_asc.gif"
                            GroupBySortDescendingImageUrl="group_desc.gif"
                            GroupBySortImageWidth="10"
                            GroupBySortImageHeight="10"
                            GroupByTextCssClass="GroupByText"
                            GroupingPageSize = "200"
                            ShowHeader="false"
                            ShowFooter="false"
                            ShowSearchBox="false"
                            SearchOnKeyPress="false"
                            PageSize="200" 
                            PagerStyle="Slider" 
                            PagerTextCssClass="GridFooterText"
                            PagerButtonWidth="41"
                            PagerButtonHeight="22"
                            SliderHeight="20"
                            PreExpandOnGroup="false"
                            SliderWidth="150" 
                            SliderGripWidth="9" 
                            SliderPopupOffsetX="20"
                            SliderPopupClientTemplateId="SliderTemplate" 
                            TreeLineImageWidth="22" 
                            TreeLineImageHeight="19" 
                            runat="server"
                            KeyboardEnabled ="true"
                            ImagesBaseUrl = "~/images/"
                            TreeLineImagesFolderUrl="~/images/lines/"
                            GroupBy="DriverType" 
                            Height="100%" 
                            FillContainer="true" 
                            OnContextMenu="ShowDriverMenu"
                            Width="100%" 
                            CssClass="tablefix">
                            <Levels>
                                <ComponentArt:GridLevel 
                                     DataKeyField="DriverResourceId"
                                     HeadingCellCssClass="HeadingCell" 
                                     HeadingRowCssClass="HeadingRow" 
                                     HeadingTextCssClass="HeadingCellText"
                                     DataCellCssClass="DataCell" 
                                     RowCssClass="Row" 
                                     SelectedRowCssClass="SelectedRow"
                                     SortAscendingImageUrl="asc.gif" 
                                     SortDescendingImageUrl="desc.gif" 
                                     SortImageWidth="10"
                                     SortImageHeight="10"
                                     GroupHeadingCssClass="GroupHeading"
                                     AlternatingRowCssClass="AlternatingRow"
                                     GroupHeadingClientTemplateId="groupByTemplate">
                                     <Columns>
                                        <ComponentArt:GridColumn DataField="DriverResourceId" HeadingText=" " DataCellClientTemplateId="PreSelectDriverTemplate" FixedWidth="true" Width="18" />
                                        <ComponentArt:GridColumn DataField="FullName" HeadingText="Full Name" DataCellClientTemplateId="DriverTemplate"  FixedWidth="true" Width="100" />
                                        <ComponentArt:GridColumn DataField="LastLocation" HeadingText="Last Call In" DataCellClientTemplateId="LastCallInTemplate" Width="80" />
                                        <ComponentArt:GridColumn DataField="Now" HeadingText="Now/Next" />
                                        <ComponentArt:GridColumn DataField="Next" HeadingText="Tomorrow"/>
                                        <ComponentArt:GridColumn DataField="StartDateTime2" HeadingText="Start" DataCellClientTemplateId="DriverStartTimeTemplate" FormatString="dd/MM HH:mm"/>                                    
                                        <ComponentArt:GridColumn DataField="DriverType" visible="false" HeadingText=" "/>
                                        <ComponentArt:GridColumn DataField="FullName" visible="false"/>
                                        <ComponentArt:GridColumn DataField="RegNo" visible="false"/>
                                        <ComponentArt:GridColumn DataField="VehicleResourceId" visible="false"/>
                                        <ComponentArt:GridColumn DataField="HasFuture" visible="false"/>
                                        <ComponentArt:GridColumn DataField="OrganisationLocationName" visible="false"/>
                                        <ComponentArt:GridColumn DataField="TravelNotes" visible="false"/>
                                        <ComponentArt:GridColumn DataField="LastLocationId" visible="false"/>
                                        <ComponentArt:GridColumn DataField="LastJobId" visible="false"/>
                                        <ComponentArt:GridColumn DataField="LeaveDateTime" visible="false"/>
                                        <ComponentArt:GridColumn DataField="PalletCount" visible="false"/>
                                        <ComponentArt:GridColumn DataField="LastCalledNoPalletHandlingInJobId" visible="false"/>
                                        <ComponentArt:GridColumn DataField="Requests" visible="false"/>
                                        <ComponentArt:GridColumn DataField="LastUsedTrailer" visible="false"/>
                                        <ComponentArt:GridColumn DataField="DriverIdentityId" visible="false"/>
                                        <ComponentArt:GridColumn DataField="ControlAreaId" visible="false"/>
                                        <ComponentArt:GridColumn DataField="TrafficAreaId" visible="false"/>
                                        <ComponentArt:GridColumn DataField="GPSUnitID" visible="false"/>
                                        <ComponentArt:GridColumn DataField="AllowMwfCommunication" visible="false"/>
                                     </Columns>
                                    </ComponentArt:GridLevel>
                            </Levels>

                            <ClientTemplates>
                                <ComponentArt:ClientTemplate ID="valignTopTemplate">
    			                    <div style="height:100%; vertical-align:top;">## DataItem.GetCurrentMember().Value ##</div>
                                </ComponentArt:ClientTemplate>
                                <ComponentArt:ClientTemplate ID="PreSelectDriverTemplate">
    			                    <input type="radio" name="myDriverSelect" value="## DataItem.GetMember("DriverResourceId").Value ##" group="driver" onclick="javascript:SelectDriverResource(this, '## DataItem.GetMember("FullName").Value ##', ## DataItem.GetMember("DriverResourceId").Value ##, '## DataItem.GetMember("RegNo").Value ##', ## DataItem.GetMember("VehicleResourceId").Value ##);" />
                                </ComponentArt:ClientTemplate>
                                <ComponentArt:ClientTemplate ID="DriverTemplate">
                                   <a href="javascript:ShowFuture('## DataItem.GetMember("DriverResourceId").Value ##','3');" onmouseover="showHelpTipUrl(event,'~/resource/driver/drivercontactpopup.aspx?identityId=Individual:##DataItem.GetMember("DriverIdentityId").Value##');" onmouseout="hideHelpTip(this);" class="helpLink">## DataItem.GetMember("FullName").Value ##</a>
                                   <br />
                                   <a href="javascript:ShowFuture('## DataItem.GetMember("VehicleResourceId").Value ##','1');" onmouseover="showHelpTipUrl(event,'~/resource/vehicle/vehiclecontactpopup.aspx?resourceId=##DataItem.GetMember("VehicleResourceId").Value##');" onmouseout="hideHelpTip(this);" class="helpLink">## DataItem.GetMember("RegNo").Value ##</a>
                                   <br />
                                   ##ShowSetTravelNotes(DataItem)##
                                   <br />
                                   ##GetPalletHandling(DataItem)##
                                   ##GetRequests(DataItem)##
                                </ComponentArt:ClientTemplate>
                                <ComponentArt:ClientTemplate ID="LastCallInTemplate">
                                    <span class="helpLink" onclick="javascript:openJobDetailsWindow('## DataItem.GetMember("LastJobId").Value ##');" onmouseover="showHelpTipUrl(event,'../point/getPointAddresshtml.aspx?pointId=## DataItem.GetMember("LastLocationId").Value ##');" onmouseout="hideHelpTip(this);">## DataItem.GetMember("LastLocation").Value ##<br />## DataItem.GetMember("LeaveDateTime").Value ## <br />## DataItem.GetMember("LastUsedTrailer").Value ##</span>
                                </ComponentArt:ClientTemplate>
                                <ComponentArt:ClientTemplate ID="groupByTemplate">
                                    ## DataItem.ColumnValue ## (## DataItem.Rows.length ##)
                                </ComponentArt:ClientTemplate>
                                <ComponentArt:ClientTemplate ID="DriverStartTimeTemplate">
                                    ##ShowClockIn(DataItem )##
                                </ComponentArt:ClientTemplate>
                            </ClientTemplates>
                    </ComponentArt:Grid>
                </ContentTemplate>
            </radP:PanelItemTemplate>
            
            <radP:PanelItemTemplate ID="templateMyTrailers">
                <ContentTemplate>
                    <div style="background-image:url('/images/panelbar/collapsedbg.gif'); background-repeat:repeat-y;">
                        <asp:CheckBox ID="chkOnlyShowEmptyTrailers" runat="server" Text="Only Show Empty Trailers" AutoPostBack="true" OnCheckedChanged="ShowEmptyTrailers_CheckedChanged" />
                        <asp:CheckBox ID="chkSortTrailersByLastLocation" runat="server" Text="Sort by last location" AutoPostBack="true" OnCheckedChanged="SortTrailers_CheckedChanged" />
                    </div>
                
                    <Componentart:CallBack ID="cbMyTrailers" runat="server">
                        <Content>
                            <ComponentArt:Grid id="grdMyTrailers" 
                                    RunningMode="Client" 
                                    GroupingNotificationTextCssClass="GridHeaderText"
                                    GroupBySortAscendingImageUrl="group_asc.gif"
                                    GroupBySortDescendingImageUrl="group_desc.gif"
                                    GroupBySortImageWidth="10"
                                    GroupBySortImageHeight="10"
                                    GroupingPageSize = "250"
                                    PageSize="250"
                                    ShowHeader="false"
                                    ShowFooter="false"
                                    ShowSearchBox="false"
                                    SearchOnKeyPress="false"
                                    PagerStyle="Slider" 
                                    PagerTextCssClass="GridFooterText"
                                    PagerButtonWidth="41"
                                    PagerButtonHeight="22"
                                    SliderHeight="20"
                                    PreExpandOnGroup="false"
                                    SliderWidth="150" 
                                    SliderGripWidth="9" 
                                    SliderPopupOffsetX="20"
                                    SliderPopupClientTemplateId="SliderTemplate" 
                                    TreeLineImageWidth="22" 
                                    TreeLineImageHeight="19" 
                                    Width="100%" runat="server"
                                    KeyboardEnabled ="true"
                                    Height="100%" 
                                    ImagesBaseUrl = "~/images/"
                                    GroupBy="TrailerType"
                                    ScrollImagesFolderUrl="~/Images/scroller/"
                                    TreeLineImagesFolderUrl="~/images/lines/"
                                    OnContextMenu="ShowTrailerMenu"
                                    CssClass="tablefix">
                                    <Levels>
                                        <ComponentArt:GridLevel 
                                             DataKeyField="TrailerResourceId"
                                             HeadingCellCssClass="HeadingCell" 
                                             HeadingRowCssClass="HeadingRow" 
                                             HeadingTextCssClass="HeadingCellText"
                                             DataCellCssClass="DataCell" 
                                             RowCssClass="Row" 
                                             SelectedRowCssClass="SelectedRow"
                                             SortAscendingImageUrl="asc.gif" 
                                             SortDescendingImageUrl="desc.gif" 
                                             SortImageWidth="10"
                                             SortImageHeight="10"
                                             GroupHeadingCssClass="GroupHeading"
                                             AlternatingRowCssClass="AlternatingRow"
                                             GroupHeadingClientTemplateId="groupByTemplate">
                                             <Columns>
                                                <ComponentArt:GridColumn DataField="TrailerResourceId" width="18" DataCellClientTemplateId="PreSelectTrailerTemplate" HeadingText=" "/>
                                                <ComponentArt:GridColumn DataField="TrailerRef" HeadingText="Ref" DataCellClientTemplateId="TrailerTemplate"  Width="60" />
                                                <ComponentArt:GridColumn DataField="LastLocation" HeadingText="Last Call In" AllowSorting="false" DataCellClientTemplateId="LastCallInTemplate" Width="120" />
                                                <ComponentArt:GridColumn DataField="Now" HeadingText="Now/Next"   AllowSorting="false" Width="120" />
                                                <ComponentArt:GridColumn DataField="Next" HeadingText="Tomorrow"   AllowSorting="false" Width="120" />
                                                <ComponentArt:GridColumn DataField="HasFuture" visible="false"/>
                                                <ComponentArt:GridColumn DataField="TrailerResourceId" visible="false"/>
                                                <ComponentArt:GridColumn DataField="LastLocation" visible="false"/>
                                                <ComponentArt:GridColumn DataField="LastLocationId" visible="false"/>
                                                <ComponentArt:GridColumn DataField="LastJobId" visible="false"/>
                                                <ComponentArt:GridColumn DataField="PalletCount" visible="false"/>
                                                <ComponentArt:GridColumn DataField="RefusalCount" visible="false"/>
                                                <ComponentArt:GridColumn DataField="LastCalledNoPalletHandlingInJobId" visible="false"/>
                                                <ComponentArt:GridColumn DataField="TrailerType" visible="false" HeadingText=" "/>
                                                <ComponentArt:GridColumn DataField="GPSUnitID" visible="false" HeadingText=" "/>
                                             </Columns>
                                            </ComponentArt:GridLevel>
                                    </Levels>
                                    
                                    <ClientTemplates>
                                         <ComponentArt:ClientTemplate ID="PreSelectTrailerTemplate">
			                                <input type="radio" name="myTrailerSelect" value="## DataItem.GetMember("TrailerResourceId").Value ##" group="trailer" onclick="javascript:SelectResource(this, '## DataItem.GetMember("TrailerRef").Value ##', ## DataItem.GetMember("TrailerResourceId").Value ##, 2);" />
                                        </ComponentArt:ClientTemplate>
                                        <ComponentArt:ClientTemplate ID="valignTopTemplate">
			                                <div style="height:100%; vertical-align:top;">## DataItem.GetCurrentMember().Value ##</div>
                                        </ComponentArt:ClientTemplate>
                                        <ComponentArt:ClientTemplate ID="TrailerTemplate">
                                           <a  href="javascript:ShowFuture('## DataItem.GetMember("TrailerResourceId").Value ##', '2');">## DataItem.GetMember("TrailerRef").Value ##</a>
                                           ##GetPalletHandling(DataItem)##
                                           ##GetRefusals(DataItem)##
                                        </ComponentArt:ClientTemplate>
                                        <ComponentArt:ClientTemplate ID="LastCallInTemplate">
                                            <span onclick="javascript:openJobDetailsWindow('## DataItem.GetMember("LastJobId").Value ##');" onmouseover="showHelpTipUrl(event,'../point/getPointAddresshtml.aspx?pointId=## DataItem.GetMember("LastLocationId").Value ##');" onmouseout="hideHelpTip(this);" class="helpLink">## DataItem.GetMember("LastLocation").Value ##</span>
                                        </ComponentArt:ClientTemplate>
                                         <ComponentArt:ClientTemplate ID="groupByTemplate">
                                            ## DataItem.ColumnValue ## (## DataItem.Rows.length ##)
                                        </ComponentArt:ClientTemplate>
                                    </ClientTemplates>
                            </ComponentArt:Grid>
                        </Content>
                        <LoadingPanelClientTemplate>
                            <div style="width:100%; text-align:center; padding-top:20px;">
                                <table cellspacing="0" cellpadding="0" border="0">
                                    <tr>
                                        <td style="font-size:10px;">Loading... </td>
                                        <td><img src="/images/spinner.gif" width="16" height="16" border="0" alt="Loading"></td>
                                    </tr>
                                </table>
                            </div>
                        </LoadingPanelClientTemplate>
                    </Componentart:CallBack>
                </ContentTemplate>
            </radP:PanelItemTemplate>
            
            <radP:PanelItemTemplate ID="templateMyVehicles">
                <ContentTemplate>
                    <div style="background-image:url('/images/panelbar/collapsedbg.gif'); background-repeat:repeat-y;">
                        <asp:CheckBox ID="chkShowEmptyVehiclesOnly" runat="server" Text="Only vehicles without a reload" AutoPostBack="true" OnCheckedChanged="ShowVehiclesReload_CheckedChanged" />
                    </div>
                    
                    <Componentart:CallBack ID="cbMyVehicles" runat="server">
                        <Content>
                            <ComponentArt:Grid id="grdMyVehicles" 
                                    RunningMode="Client" 
                                    GroupingNotificationTextCssClass="GridHeaderText"
                                    GroupBySortAscendingImageUrl="group_asc.gif"
                                    GroupBySortDescendingImageUrl="group_desc.gif"
                                    GroupBySortImageWidth="10"
                                    GroupBySortImageHeight="10"
                                    GroupingPageSize = "25"
                                    ShowHeader="false"
                                    ShowFooter="false"
                                    ShowSearchBox="false"
                                    SearchOnKeyPress="false"
                                    PageSize="250" 
                                    PagerStyle="Slider" 
                                    PagerTextCssClass="GridFooterText"
                                    PagerButtonWidth="41"
                                    PagerButtonHeight="22"
                                    SliderHeight="20"
                                    PreExpandOnGroup="false"
                                    SliderWidth="150" 
                                    SliderGripWidth="9" 
                                    SliderPopupOffsetX="20"
                                    SliderPopupClientTemplateId="SliderTemplate" 
                                    TreeLineImageWidth="22" 
                                    TreeLineImageHeight="19" 
                                    Width="100%" runat="server"
                                    KeyboardEnabled ="true"
                                    ImagesBaseUrl = "~/images/"
                                    GroupBy="VehicleType"
                                    Height="100%" 
                                    TreeLineImagesFolderUrl="~/images/lines/"
                                    OnContextMenu="ShowVehicleMenu"
                                    CssClass="tablefix">
                                    
                                    <Levels>
                                        <ComponentArt:GridLevel 
                                             DataKeyField="VehicleResourceId"
                                             HeadingCellCssClass="HeadingCell" 
                                             HeadingRowCssClass="HeadingRow" 
                                             HeadingTextCssClass="HeadingCellText"
                                             DataCellCssClass="DataCell" 
                                             RowCssClass="Row" 
                                             SelectedRowCssClass="SelectedRow"
                                             SortAscendingImageUrl="asc.gif" 
                                             SortDescendingImageUrl="desc.gif" 
                                             SortImageWidth="10"
                                             SortImageHeight="10"
                                             GroupHeadingCssClass="GroupHeading"
                                             AlternatingRowCssClass="AlternatingRow"
                                             GroupHeadingClientTemplateId="groupByTemplate"
                                             >
                                             <Columns>
                                                <ComponentArt:GridColumn DataField="VehicleResourceId" width="18" DataCellClientTemplateId="PreSelectVehicleTemplate" HeadingText=" "/>
                                                <ComponentArt:GridColumn DataField="RegNo" HeadingText="Vehicle" DataCellClientTemplateId="VehicleTemplate"  FixedWidth="true" Width="60" />
                                                <ComponentArt:GridColumn DataField="LastLocation" HeadingText="Last Call In" AllowSorting="false" DataCellClientTemplateId="LastCallInTemplate" Width="120" />
                                                <ComponentArt:GridColumn DataField="LastJobId" visible="false"/>
                                                <ComponentArt:GridColumn DataField="Now" HeadingText="Now/Next"   AllowSorting="false" width="120"/>
                                                <ComponentArt:GridColumn DataField="Next" HeadingText="Tomorrow"   AllowSorting="false" Width="120" />
                                                <ComponentArt:GridColumn DataField="HasFuture" visible="false"/>
                                                <ComponentArt:GridColumn DataField="VehicleResourceId" visible="false"/>
                                                <ComponentArt:GridColumn DataField="LastLocation" visible="false"/>
                                                <ComponentArt:GridColumn DataField="LastLocationId" visible="false"/>
                                                <ComponentArt:GridColumn DataField="VehicleType" visible="false" HeadingText=" "/>
                                                <ComponentArt:GridColumn DataField="GPSUnitID" visible="false" HeadingText=" "/>
                                             </Columns>
                                            </ComponentArt:GridLevel>
                                    </Levels>

                                    <ClientTemplates>
                                         <ComponentArt:ClientTemplate ID="PreSelectVehicleTemplate">
    			                            <input type="radio" name="myVehicleSelect" value="## DataItem.GetMember("VehicleResourceId").Value ##" group="vehicle" onclick="javascript:SelectResource(this, '## DataItem.GetMember("RegNo").Value ##', ## DataItem.GetMember("VehicleResourceId").Value ##, 1);" />
                                        </ComponentArt:ClientTemplate>
                                        <ComponentArt:ClientTemplate ID="valignTopTemplate">
    			                            <div style="height:100%; vertical-align:top;">## DataItem.GetCurrentMember().Value ##</div>
                                        </ComponentArt:ClientTemplate>
                                        <ComponentArt:ClientTemplate ID="VehicleTemplate">
                                           <a href="javascript:ShowFuture('## DataItem.GetMember("VehicleResourceId").Value ##', '1');" onmouseover="showHelpTipUrl(event,'~/resource/vehicle/vehiclecontactpopup.aspx?resourceId=##DataItem.GetMember("VehicleResourceId").Value##');" onmouseout="hideHelpTip(this);" class="helpLink">## DataItem.GetMember("RegNo").Value ##</a>
                                        </ComponentArt:ClientTemplate>
                                         <ComponentArt:ClientTemplate ID="LastCallInTemplate">
                                            <span onclick="javascript:openJobDetailsWindow('## DataItem.GetMember("LastJobId").Value ##');" onmouseover="showHelpTipUrl(event,'../point/getPointAddresshtml.aspx?pointId=## DataItem.GetMember("LastLocationId").Value ##');" onmouseout="hideHelpTip(this);" class="helpLink">## DataItem.GetMember("LastLocation").Value ##</span>
                                        </ComponentArt:ClientTemplate>
                                         <ComponentArt:ClientTemplate ID="groupByTemplate">
                                            ## DataItem.ColumnValue ## (## DataItem.Rows.length ##)
                                        </ComponentArt:ClientTemplate>
                                    </ClientTemplates>
                            </ComponentArt:Grid>
                        </Content>
                        <LoadingPanelClientTemplate>
                            <div style="width:100%; text-align:center; padding-top:20px;">
                                <table cellspacing="0" cellpadding="0" border="0">
                                    <tr>
                                        <td style="font-size:10px;">Loading... </td>
                                        <td><img src="/images/spinner.gif" width="16" height="16" border="0" alt="Loading"></td>
                                    </tr>
                                </table>
                            </div>
                        </LoadingPanelClientTemplate>
                    </Componentart:CallBack>
                </ContentTemplate>
            </radP:PanelItemTemplate>
            
            <radP:PanelItemTemplate ID="templateJobsComingIn">
                <ContentTemplate>
                    <Componentart:CallBack ID="cbJobsComingIn" runat="server">
                        <Content>
                            <ComponentArt:Grid id="grdJobsComingIn" 
                                    RunningMode="Client" 
                                    
                                    GroupingNotificationTextCssClass="GridHeaderText"
                                    GroupBySortAscendingImageUrl="group_asc.gif"
                                    GroupBySortDescendingImageUrl="group_desc.gif"
                                    GroupBySortImageWidth="10"
                                    GroupBySortImageHeight="10"
                                    GroupingPageSize = "25"
                                    ShowHeader="false"
                                    ShowFooter="false"
                                    ShowSearchBox="false"
                                    SearchOnKeyPress="false"
                                    PageSize="250" 
                                    PagerStyle="Slider" 
                                    PagerTextCssClass="GridFooterText"
                                    PagerButtonWidth="41"
                                    PagerButtonHeight="22"
                                    SliderHeight="20"
                                    PreExpandOnGroup="false"
                                    SliderWidth="150" 
                                    SliderGripWidth="9" 
                                    SliderPopupOffsetX="20"
                                    SliderPopupClientTemplateId="SliderTemplate" 
                                    TreeLineImageWidth="22" 
                                    TreeLineImageHeight="19" 
                                    runat="server"
                                    KeyboardEnabled ="true"
                                    Height="100%"
                                    width="570"
                                    ImagesBaseUrl = "~/images/"
                                    CssClass="tablefix">
                                    
                                    <Levels>
                                        <ComponentArt:GridLevel 
                                             DataKeyField="JobId"
                                             HeadingCellCssClass="HeadingCell" 
                                             HeadingRowCssClass="HeadingRow" 
                                             HeadingTextCssClass="HeadingCellText"
                                             DataCellCssClass="DataCell" 
                                             RowCssClass="Row" 
                                             SelectedRowCssClass="SelectedRow"
                                             SortAscendingImageUrl="asc.gif" 
                                             SortDescendingImageUrl="desc.gif" 
                                             SortImageWidth="10"
                                             SortImageHeight="10"
                                             GroupHeadingCssClass="GroupHeading"
                                             AlternatingRowCssClass="AlternatingRow">
                                             <Columns>
                                                <ComponentArt:GridColumn DataField="TargetInstructionId" HeadingText=" " DataCellClientTemplateId="showJobLink" />
                                                <ComponentArt:GridColumn DataField="JobId" HeadingText=" " DataCellClientTemplateId="PreSelectJobTemplate"  FixedWidth="true" Width="18" />
                                                <ComponentArt:GridColumn DataField="JobId" HeadingText="ID" DataCellClientTemplateId="JobIdTemplate"  FixedWidth="true" Width="40" />
                                                <ComponentArt:GridColumn DataField="FirstPointInMyArea" HeadingText="First Point In" DataCellClientTemplateId="firstPointInMyArea" />
                                                <ComponentArt:GridColumn DataField="FirstTimeInMyArea" HeadingText="First Time In" FormatString="dd/MM HH:mm" width="120" FixedWidth="true" />
                                                <ComponentArt:GridColumn DataField="EndPointOfJob" HeadingText="End Point"   DataCellClientTemplateId="EndPointTemplate" />
                                                <ComponentArt:GridColumn DataField="PlannedEndTimeForJob" HeadingText="Planned End" FormatString="dd/MM HH:mm" width="120" FixedWidth="true" />
                                                <ComponentArt:GridColumn DataField="PCVsOnJob" HeadingText="PCV" DataCellClientTemplateId="attachedPCVS" width="70" />
                                                <ComponentArt:GridColumn DataField="FirstPointIn" visible="false"/>
                                                <ComponentArt:GridColumn DataField="EndPoint" visible="false"/>
                                                <ComponentArt:GridColumn DataField="InstructionId" visible="false"/>
                                                <ComponentArt:GridColumn DataField="TargetJobId" visible="false"/>
                                                <ComponentArt:GridColumn DataField="InstructionStateId" visible="false" />
                                                <ComponentArt:GridColumn DataField="FullName" visible="false" />
                                                <ComponentArt:GridColumn DataField="RegNo" visible="false" />
                                                <ComponentArt:GridColumn DataField="TrailerRef" visible="false" />
                                             </Columns>
                                            <ConditionalFormats>
                                                <ComponentArt:GridConditionalFormat ClientFilter="DataItem.GetMember('InstructionStateId').Value == 1" RowCssClass="stateBooked" />
                                                <ComponentArt:GridConditionalFormat ClientFilter="DataItem.GetMember('InstructionStateId').Value == 2" RowCssClass="statePlanned" />
                                                <ComponentArt:GridConditionalFormat ClientFilter="DataItem.GetMember('InstructionStateId').Value == 3" RowCssClass="stateInProgress" />
                                                <ComponentArt:GridConditionalFormat ClientFilter="DataItem.GetMember('InstructionStateId').Value == 4" RowCssClass="stateCompleted" />
                                            </ConditionalFormats>
                                        </ComponentArt:GridLevel>
                                    </Levels>

                                    <ClientTemplates>
                                        <ComponentArt:ClientTemplate ID="showJobLink">
                                            ## GetJobLinks(DataItem)##
                                        </ComponentArt:ClientTemplate>
                                        <ComponentArt:ClientTemplate ID="PreSelectJobTemplate">
    			                            <input type="radio" name="jobSelect" value="## DataItem.GetMember("JobId").Value ##" group="preselectJobComingIn" onclick="javascript:LinkJob(this, '## DataItem.GetMember("JobId").Value ##', ## DataItem.GetMember("InstructionId").Value ##, 1);" />
                                        </ComponentArt:ClientTemplate>
                                        <ComponentArt:ClientTemplate ID="JobIdTemplate">
                                            <a href="javascript:OpenJobDetails(## DataItem.GetMember("JobId").Value ##)">## DataItem.GetMember("JobId").Value ##</a>
                                        </ComponentArt:ClientTemplate>
                                         <ComponentArt:ClientTemplate ID="firstPointInMyArea">
                                            <span onmouseover="showHelpTipUrl(event,'../point/getPointAddresshtml.aspx?pointId=## DataItem.GetMember("FirstPointIn").Value ##')" onmouseout="hideHelpTip(this);" class="helpLink">## DataItem.GetMember("FirstPointInMyArea").Value ##</span>
                                            <br />
                                            ## if (DataItem.GetMember("FullName").Value != '') DataItem.GetMember("FullName").Value + '<br />' ##
                                            ## if (DataItem.GetMember("RegNo").Value != '') DataItem.GetMember("RegNo").Value + '<br />' ##
                                            ## if (DataItem.GetMember("TrailerRef").Value != '') DataItem.GetMember("TrailerRef").Value ##
                                        </ComponentArt:ClientTemplate>
                                        <ComponentArt:ClientTemplate ID="EndPointTemplate">
                                            <span onmouseover="showHelpTipUrl(event,'../point/getPointAddresshtml.aspx?pointId=## DataItem.GetMember("EndPoint").Value ##')" onmouseout="hideHelpTip(this);" class="helpLink">## DataItem.GetMember("EndPointOfJob").Value ##</span>
                                        </ComponentArt:ClientTemplate>
                                        <ComponentArt:ClientTemplate ID="attachedPCVS">
                                            ## GetPCVLink(DataItem)##
                                        </ComponentArt:ClientTemplate>
                                    </ClientTemplates>
                                   
                            </ComponentArt:Grid>
                        </Content>
                        <LoadingPanelClientTemplate>
                            <div style="width:100%; text-align:center; padding-top:20px;">
                                <table cellspacing="0" cellpadding="0" border="0">
                                    <tr>
                                        <td style="font-size:10px;">Loading... </td>
                                        <td><img src="/images/spinner.gif" width="16" height="16" border="0" alt="Loading"></td>
                                    </tr>
                                </table>
                            </div>
                        </LoadingPanelClientTemplate>
                    </Componentart:CallBack>
                </ContentTemplate>
            </radP:PanelItemTemplate>
            
            <radP:PanelItemTemplate ID="templateDriversComingIn">
                <ContentTemplate>
                    <Componentart:CallBack ID="cbDriversComingIn" runat="server">
                        <Content>
                            <ComponentArt:Grid id="grdDriversComingIn" 
                                    GroupingNotificationTextCssClass="GridHeaderText"
                                    GroupBySortAscendingImageUrl="group_asc.gif"
                                    GroupBySortDescendingImageUrl="group_desc.gif"
                                    GroupBySortImageWidth="10"
                                    GroupBySortImageHeight="10"
                                    GroupingPageSize = "25"
                                    ShowHeader="false"
                                    ShowFooter="false"
                                    ShowSearchBox="false"
                                    SearchOnKeyPress="false"
                                    PageSize="250" 
                                    PagerStyle="Slider" 
                                    PagerTextCssClass="GridFooterText"
                                    PagerButtonWidth="41"
                                    PagerButtonHeight="22"
                                    SliderHeight="20"
                                    PreExpandOnGroup="false"
                                    SliderWidth="150" 
                                    SliderGripWidth="9" 
                                    SliderPopupOffsetX="20"
                                    SliderPopupClientTemplateId="SliderTemplate" 
                                    TreeLineImageWidth="22" 
                                    TreeLineImageHeight="19" 
                                    Width="100%" runat="server"
                                    KeyboardEnabled ="true"
                                    ImagesBaseUrl = "~/images/"
                                    Height="100%" OnContextMenu="ShowDriverMenu"
                                    CssClass="tablefix">
                                    
                                    <Levels>
                                        <ComponentArt:GridLevel 
                                             DataKeyField="DriverResourceId"
                                             HeadingCellCssClass="HeadingCell" 
                                             HeadingRowCssClass="HeadingRow" 
                                             HeadingTextCssClass="HeadingCellText"
                                             DataCellCssClass="DataCell" 
                                             RowCssClass="Row" 
                                             SelectedRowCssClass="SelectedRow"
                                             SortAscendingImageUrl="asc.gif" 
                                             SortDescendingImageUrl="desc.gif" 
                                             SortImageWidth="10"
                                             SortImageHeight="10"
                                             GroupHeadingCssClass="GroupHeading"
                                             AlternatingRowCssClass="AlternatingRow">
                                             <Columns>
                                                <ComponentArt:GridColumn DataField="DriverResourceId" HeadingText=" "  FixedWidth="true" Width="18" DataCellClientTemplateId="DriverResourceTemplate" />
                                                <ComponentArt:GridColumn DataField="FullName" HeadingText="Full Name" DataCellClientTemplateId="DriverTemplate"  FixedWidth="true" Width="100" />
                                                <ComponentArt:GridColumn DataField="LastKnownPointId" HeadingText="Last Location" Width="120"  DataCellClientTemplateId="templateLastLocation" />
                                                <ComponentArt:GridColumn DataField="PointId" HeadingText="Planned" AllowSorting="false" Width="120"  DataCellClientTemplateId="templatePlanned" />
                                                <ComponentArt:GridColumn DataField="LastKnownLocation" visible="false"/>
                                                <ComponentArt:GridColumn DataField="LastUsedVehicle" visible="false"/>
                                               
                                                <ComponentArt:GridColumn DataField="LastKnownDateTime" visible="false"/>                                    
                                                <ComponentArt:GridColumn DataField="InstructionID" visible="false"/>
                                                <ComponentArt:GridColumn DataField="DriverResourceId"  Visible="false"/>
                                                 <ComponentArt:GridColumn DataField="LastUsedTrailer" visible="false"/>
                                                 
                                                <ComponentArt:GridColumn DataField="PlannedArrivalDateTime" visible="false"/>
                                                <ComponentArt:GridColumn DataField="Description" visible="false"/>
                                                <ComponentArt:GridColumn DataField="RegNo" visible="false"/>
                                                <ComponentArt:GridColumn DataField="VehicleResourceId" visible="false"/>
                                                <ComponentArt:GridColumn DataField="InstructionID" visible="false"/>
                                                <ComponentArt:GridColumn DataField="TrailerRef" visible="false"/>
                                                <ComponentArt:GridColumn DataField="LinkJobId" visible="false"/>
                                                <ComponentArt:GridColumn DataField="LinkInstructionID" visible="false"/>
                                                <ComponentArt:GridColumn DataField="AllowMwfCommunication" visible="false"/>
                                             </Columns>
                                        </ComponentArt:GridLevel>
                                    </Levels>

                                    <ClientTemplates>
                                        <ComponentArt:ClientTemplate ID="DriverResourceTemplate">
                                            <input type="radio" name="driverSelect" value="## DataItem.GetMember("LinkJobId").Value ##" group="preselectDriverComingIn" onclick="javascript:LinkJob(this, '## DataItem.GetMember("LinkJobId").Value ##', ## DataItem.GetMember("LinkInstructionID").Value ##, 2);" />
                                        </ComponentArt:ClientTemplate>
                                        <ComponentArt:ClientTemplate ID="DriverTemplate">
                                           <a href="javascript:ShowFuture('## DataItem.GetMember("DriverResourceId").Value ##', '3');">## DataItem.GetMember("FullName").Value ##</a>
                                        </ComponentArt:ClientTemplate>
                                        <ComponentArt:ClientTemplate ID="templateLastLocation">
                                            <table >
                                                <tr><td class="DataCell"><span onmouseover="showHelpTipUrl(event,'../point/getPointAddresshtml.aspx?pointId=## DataItem.GetMember("LastKnownPointId").Value ##')" onmouseout="hideHelpTip(this);" class="helpLink" onclick="OpenJobDetails(## DataItem.GetMember("LinkJobId").Value ##);">## DataItem.GetMember("LastKnownLocation").Value ##</span></td></tr>
                                                <tr><td class="DataCell">## ShowDate(DataItem.GetMember("LastKnownDateTime").Value) ##</td></tr>
                                                <tr><td class="DataCell">## DataItem.GetMember("LastUsedVehicle").Value ##</td></tr>
                                                <tr><td class="DataCell">## DataItem.GetMember("LastUsedTrailer").Value ##</td></tr>
                                            </table>
                                        </ComponentArt:ClientTemplate>
                                         <ComponentArt:ClientTemplate ID="templatePlanned">
                                            <table>
                                                <tr><td class="DataCell">## GetShowPoint(DataItem) ##</td></tr>
                                                <tr><td class="DataCell">## ShowDate(DataItem.GetMember("PlannedArrivalDateTime").Value )##</td></tr>
                                                <tr><td class="DataCell">## DataItem.GetMember("RegNo").Value ##</td></tr>
                                                <tr><td class="DataCell">## DataItem.GetMember("TrailerRef").Value ##</td></tr>
                                            </table>
                                        </ComponentArt:ClientTemplate>
                                    </ClientTemplates>
                                   
                            </ComponentArt:Grid>
                        </Content>
                        <LoadingPanelClientTemplate>
                            <div style="width:100%; text-align:center; padding-top:20px;">
                                <table cellspacing="0" cellpadding="0" border="0">
                                    <tr>
                                        <td style="font-size:10px;">Loading... </td>
                                        <td><img src="/images/spinner.gif" width="16" height="16" border="0" alt="Loading"></td>
                                    </tr>
                                </table>
                            </div>
                        </LoadingPanelClientTemplate>
                    </Componentart:CallBack>
                </ContentTemplate>
            </radP:PanelItemTemplate>
            
            <radP:PanelItemTemplate ID="templateVehiclesComingIn">
                <ContentTemplate>
                     <Componentart:CallBack ID="cbVehiclesComingIn" runat="server">
                        <Content>
                            <ComponentArt:Grid id="grdVehiclesComingIn" 
                                    RunningMode="Client" 
                                    GroupingNotificationTextCssClass="GridHeaderText"
                                    GroupBySortAscendingImageUrl="group_asc.gif"
                                    GroupBySortDescendingImageUrl="group_desc.gif"
                                    GroupBySortImageWidth="10"
                                    GroupBySortImageHeight="10"
                                    GroupingPageSize = "25"
                                    ShowHeader="false"
                                    ShowFooter="false"
                                    ShowSearchBox="false"
                                    SearchOnKeyPress="false"
                                    PageSize="250" 
                                    PagerStyle="Slider" 
                                    PagerTextCssClass="GridFooterText"
                                    PagerButtonWidth="41"
                                    PagerButtonHeight="22"
                                    SliderHeight="20"
                                    PreExpandOnGroup="false"
                                    SliderWidth="150" 
                                    SliderGripWidth="9" 
                                    SliderPopupOffsetX="20"
                                    SliderPopupClientTemplateId="SliderTemplate" 
                                    TreeLineImageWidth="22" 
                                    TreeLineImageHeight="19" 
                                    Width="100%" runat="server"
                                    KeyboardEnabled ="true"
                                    ImagesBaseUrl = "~/images/"
                                    Height="100%" OnContextMenu="ShowVehicleMenu"
                                    CssClass="tablefix">

                                    <Levels>
                                        <ComponentArt:GridLevel 
                                             DataKeyField="VehicleResourceId"
                                             HeadingCellCssClass="HeadingCell" 
                                             HeadingRowCssClass="HeadingRow" 
                                             HeadingTextCssClass="HeadingCellText"
                                             DataCellCssClass="DataCell" 
                                             RowCssClass="Row" 
                                             SelectedRowCssClass="SelectedRow"
                                             SortAscendingImageUrl="asc.gif" 
                                             SortDescendingImageUrl="desc.gif" 
                                             SortImageWidth="10"
                                             SortImageHeight="10"
                                             GroupHeadingCssClass="GroupHeading"
                                             AlternatingRowCssClass="AlternatingRow">
                                             <Columns>
                                                <ComponentArt:GridColumn DataField="VehicleResourceId" HeadingText=" "  FixedWidth="true" Width="18" DataCellClientTemplateId="VehicleResourceTemplate" />
                                                <ComponentArt:GridColumn DataField="RegNo" HeadingText="Reg"  FixedWidth="true" Width="60" />
                                                <ComponentArt:GridColumn DataField="LastKnownPointId" HeadingText="Last Location" AllowSorting="false" Width="120"  DataCellClientTemplateId="templateLastLocation" />
                                                <ComponentArt:GridColumn DataField="PointId" HeadingText="Planned" AllowSorting="false" Width="120"  DataCellClientTemplateId="templatePlanned" />
                                                <ComponentArt:GridColumn DataField="LastKnownLocation" visible="false"/>
                                                
                                                <ComponentArt:GridColumn DataField="LastKnownDateTime" visible="false"/>                                    
                                                <ComponentArt:GridColumn DataField="InstructionID" visible="false"/>
                                                 
                                                <ComponentArt:GridColumn DataField="PlannedArrivalDateTime" visible="false"/>
                                                <ComponentArt:GridColumn DataField="Description" visible="false"/>
                                                <ComponentArt:GridColumn DataField="RegNo" visible="false"/>
                                                <ComponentArt:GridColumn DataField="LinkJobId" visible="false"/>
                                                <ComponentArt:GridColumn DataField="LinkInstructionID" visible="false"/>
                                             </Columns>
                                        </ComponentArt:GridLevel>
                                    </Levels>

                                    <ClientTemplates>
                                        <ComponentArt:ClientTemplate ID="VehicleResourceTemplate">
                                           <input type="radio" name="vehicleSelect" value="## DataItem.GetMember("LinkJobId").Value ##" group="preselectVehicleComingIn" onclick="javascript:LinkJob(this, '## DataItem.GetMember("LinkJobId").Value ##', ## DataItem.GetMember("LinkInstructionID").Value ##, 3);" />
                                        </ComponentArt:ClientTemplate>
                                        <ComponentArt:ClientTemplate ID="templateLastLocation">
                                            <table>
                                                <tr><td class="DataCell"><span onmouseover="showHelpTipUrl(event,'../point/getPointAddresshtml.aspx?pointId=## DataItem.GetMember("LastKnownPointId").Value ##')" onmouseout="hideHelpTip(this);" class="helpLink" onclick="OpenJobDetails(## DataItem.GetMember("LinkJobId").Value ##);">## DataItem.GetMember("LastKnownLocation").Value ##</span></td></tr>
                                                <tr><td class="DataCell">## ShowDate(DataItem.GetMember("LastKnownDateTime").Value) ##</td></tr>
                                            </table>
                                        </ComponentArt:ClientTemplate>
                                         <ComponentArt:ClientTemplate ID="templatePlanned">
                                            <table>
                                                <tr><td class="DataCell">## GetShowPoint(DataItem) ##</td></tr>
                                                <tr><td class="DataCell">## ShowDate(DataItem.GetMember("PlannedArrivalDateTime").Value) ##</td></tr>
                                            </table>
                                        </ComponentArt:ClientTemplate>
                                    </ClientTemplates>
                            </ComponentArt:Grid>
                        </Content>
                        <LoadingPanelClientTemplate>
                            <div style="width:100%; text-align:center; padding-top:20px;">
                                <table cellspacing="0" cellpadding="0" border="0">
                                    <tr>
                                        <td style="font-size:10px;">Loading... </td>
                                        <td><img src="/images/spinner.gif" width="16" height="16" border="0" alt="Loading"></td>
                                    </tr>
                                </table>
                            </div>
                        </LoadingPanelClientTemplate>
                    </Componentart:CallBack>
                </ContentTemplate>
            </radP:PanelItemTemplate>
            
            <radP:PanelItemTemplate ID="templateTrailersComingIn">
                <ContentTemplate>
                     <Componentart:CallBack ID="cbTrailersComingIn" runat="server">
                        <Content>
                            <ComponentArt:Grid id="grdTrailersComingIn" 
                                    RunningMode="Client" 
                                    GroupingNotificationTextCssClass="GridHeaderText"
                                    GroupBySortAscendingImageUrl="group_asc.gif"
                                    GroupBySortDescendingImageUrl="group_desc.gif"
                                    GroupBySortImageWidth="10"
                                    GroupBySortImageHeight="10"
                                    GroupingPageSize = "25"
                                    ShowHeader="false"
                                    ShowFooter="false"
                                    ShowSearchBox="false"
                                    SearchOnKeyPress="false"
                                    PageSize="250" 
                                    PagerStyle="Slider" 
                                    PagerTextCssClass="GridFooterText"
                                    PagerButtonWidth="41"
                                    PagerButtonHeight="22"
                                    SliderHeight="20"
                                    PreExpandOnGroup="false"
                                    SliderWidth="150" 
                                    SliderGripWidth="9" 
                                    SliderPopupOffsetX="20"
                                    SliderPopupClientTemplateId="SliderTemplate" 
                                    TreeLineImageWidth="22" 
                                    TreeLineImageHeight="19" 
                                    Width="100%" runat="server"
                                    KeyboardEnabled ="true"
                                    ImagesBaseUrl = "~/images/"
                                    Height="100%" OnContextMenu="ShowTrailerMenu"
                                    CssClass="tablefix">
                                    
                                    <Levels>
                                        <ComponentArt:GridLevel 
                                             DataKeyField="TrailerResourceId"
                                             HeadingCellCssClass="HeadingCell" 
                                             HeadingRowCssClass="HeadingRow" 
                                             HeadingTextCssClass="HeadingCellText"
                                             DataCellCssClass="DataCell" 
                                             RowCssClass="Row" 
                                             SelectedRowCssClass="SelectedRow"
                                             SortAscendingImageUrl="asc.gif" 
                                             SortDescendingImageUrl="desc.gif" 
                                             SortImageWidth="10"
                                             SortImageHeight="10"
                                             GroupHeadingCssClass="GroupHeading"
                                             AlternatingRowCssClass="AlternatingRow">
                                             <Columns>
                                                <ComponentArt:GridColumn DataField="TrailerResourceId" HeadingText=" "  FixedWidth="true" Width="18" DataCellClientTemplateId="TrailerResourceTemplate" />
                                                <ComponentArt:GridColumn DataField="TrailerRef" HeadingText="Ref"  FixedWidth="true" Width="30" />
                                                <ComponentArt:GridColumn DataField="LastKnownPointId" HeadingText="Last Location" AllowSorting="false" Width="120"  DataCellClientTemplateId="templateLastLocation" />
                                                <ComponentArt:GridColumn DataField="PointId" HeadingText="Planned" AllowSorting="false" Width="120"  DataCellClientTemplateId="templatePlanned" />
                                                <ComponentArt:GridColumn DataField="LastKnownLocation" visible="false"/>
                                                
                                                <ComponentArt:GridColumn DataField="LastKnownDateTime" visible="false"/>                                    
                                                <ComponentArt:GridColumn DataField="InstructionID" visible="false"/>
                                                 
                                                <ComponentArt:GridColumn DataField="PlannedArrivalDateTime" visible="false"/>
                                                <ComponentArt:GridColumn DataField="Description" visible="false"/>
                                                <ComponentArt:GridColumn DataField="LinkJobId" visible="false"/>
                                                <ComponentArt:GridColumn DataField="LinkInstructionID" visible="false"/>
                                             </Columns>
                                            </ComponentArt:GridLevel>
                                    </Levels>

                                    <ClientTemplates>
                                        <ComponentArt:ClientTemplate ID="TrailerResourceTemplate">
                                           <input type="radio" name="trailerSelect" value="## DataItem.GetMember("LinkJobId").Value ##" group="preselectTrailerComingIn" onclick="javascript:LinkJob(this, '## DataItem.GetMember("LinkJobId").Value ##', ## DataItem.GetMember("LinkInstructionID").Value ##, 4);" />
                                        </ComponentArt:ClientTemplate>
                                        <ComponentArt:ClientTemplate ID="DriverTemplate">
                                           <a href="javascript:ShowFuture('## DataItem.GetMember("DriverResourceId").Value ##', '3');">## DataItem.GetMember("FullName").Value ##</a>
                                        </ComponentArt:ClientTemplate>
                                        <ComponentArt:ClientTemplate ID="templateLastLocation">
                                            <table>
                                                <tr><td class="DataCell"><span onmouseover="showHelpTipUrl(event,'../point/getPointAddresshtml.aspx?pointId=## DataItem.GetMember("LastKnownPointId").Value ##')" onmouseout="hideHelpTip(this);" class="helpLink" onclick="OpenJobDetails(## DataItem.GetMember("LinkJobId").Value ##);">## DataItem.GetMember("LastKnownLocation").Value ##</span></td></tr>
                                                <tr><td class="DataCell">## ShowDate(DataItem.GetMember("LastKnownDateTime").Value) ##</td></tr>
                                            </table>
                                        </ComponentArt:ClientTemplate>
                                         <ComponentArt:ClientTemplate ID="templatePlanned">
                                            <table>
                                                <tr><td class="DataCell">## GetShowPoint(DataItem) ##</td></tr>
                                                <tr><td class="DataCell">## ShowDate(DataItem.GetMember("PlannedArrivalDateTime").Value) ##</td></tr>
                                            </table>
                                        </ComponentArt:ClientTemplate>
                                    </ClientTemplates>
                            </ComponentArt:Grid>
                        </Content>
                        <LoadingPanelClientTemplate>
                            <div style="width:100%; text-align:center; padding-top:20px;">
                                <table cellspacing="0" cellpadding="0" border="0">
                                    <tr>
                                        <td style="font-size:10px;">Loading... </td>
                                        <td><img src="/images/spinner.gif" width="16" height="16" border="0" alt="Loading"></td>
                                    </tr>
                                </table>
                            </div>
                        </LoadingPanelClientTemplate>
                    </Componentart:CallBack>
                </ContentTemplate>
            </radP:PanelItemTemplate>
            
        </PanelItemTemplates>
    </radP:RadPanelbar>
        
    <uc:MwfDriverMessaging runat="server" ID="mwfDriverMessaging" />
    <uc:DriverTime runat="server" ID="driverTime" />

    <script type="text/javascript">

        // Filter data
        var activeControlAreaId = <%=ControlAreaId %>;
        var activeTrafficAreaIds = "<%=TrafficAreaIds %>";

        function GetSetNotesString(note)
        {
            var notes = note.GetMember("TravelNotes").Value;
            if (notes.length > 0 )
                return notes;
            else
                return "Set  Notes";
        }

        function GetPalletHandling(dataItem)
        {
            if (dataItem.GetMember("PalletCount").Value != "0")
                return "<br /><a href=\"javascript:openPalletHandlingWindow(" + dataItem.GetMember("LastCalledNoPalletHandlingInJobId").Value + ")\"><b>&laquo; " + dataItem.GetMember("PalletCount").Value + " &raquo;</b></a>";
            else
                return "";             
        }
        
        function GetRequests(dataItem)
        {
            
            if(dataItem.GetMember("Requests").Value == "1")
            {
                return "<br /><img src=\"../images/request.gif\" title=\"There is at least one upcoming request, please review\" onclick=\"ShowDriverRequests(" + dataItem.GetMember("DriverResourceId").Value + ")\" />";
            }
            else
                return "";
        }
        
        function ShowDriverRequests(resourceId)
	    {
		    window.open('../Resource/Driver/DriverRequests.aspx?wiz=true&resourceId=' + resourceId + '&fromDateTime=<%=StartDateString%>', 'resourceRequests', 'width=550,height=400,resizable=no,scrollbars=yes');
	    }
	    
        function GetRefusals(dataItem)
        {
            if(dataItem.GetMember("RefusalCount").Value  != 0)
            {
                return "<br /><img src=\"../images/refusals.gif\"  onmouseover=\"ShowOnTrailerReturns(" + dataItem.GetMember("TrailerResourceId").Value + ")\" onmouseout=\"HidePoint();\" />";
            }
            else
            {
                return "";
            }
        }
        
        function GetImage(legPosition)
        {
            switch (legPosition)
            {
                case "First":
                    return "<img src='../images/legTop.gif' height=20 width=5 />";
                case "Middle":
                    return "<img src='../images/legMiddle.gif' height=20 width=5 />";
                case "Last":
                    return "<img src='../images/legBottom.gif' height=20 width=5 />";
            }
            return "<img src='../images/spacer.gif' height=20 width=5 />";;
        }
       
        function openJobDetailsWindow(jobId)
        {
            if (jobId > 0)
                openResizableDialogWithScrollbars('../Job/Job.aspx?wiz=true&jobId=' + jobId + getCSID(),'1220','800');
    		else
    		    alert("This resource has never been involved on a job.");
        }
        
        function ViewPCV(jobID, lnk)
        {
            var qs = "JobID=" + jobID;
            <%=dlgPCV.ClientID %>_Open(qs);
        }
        
        function SetTravelNotes(resourceId, lnk)
        {
            var qs = "resourceId=" + resourceId;
            <%=dlgTravelNotes.ClientID %>_Open(qs);
        }
       
        function openUpdateLocation(instructionId)
        {
            var qs = "instructionid=" + instructionId;
            <%=dlgLocation.ClientID %>_Open(qs);
        }
        
        function openPalletHandlingWindow(jobId)
        {
            var qs = "jobId=" + jobId;
            <%=dlgPalletHandling.ClientID %>_Open(qs);
        }
        
        function OpenJobDetails(jobId)
	    {
            openResizableDialogWithScrollbars('../job/job.aspx?wiz=true&jobId=' + jobId + getCSID(),'1150','900');
	    }
        
    	function ClockIn(resourceId)
        {
            var qs = "resourceId=" + resourceId + "&date=<%=DateTime.UtcNow.ToString("dd/MM/yyyy")%>";
            <%=dlgStartTime.ClientID %>_Open(qs);
        }
        
        function SelectDriverResource(radioButton, resourceName, resourceId, regno, vehicleResourceId)
        {
            top.document.getElementById('hidDriverResourceId').value = resourceId;
            top.document.getElementById('hidDriverResourceName').value = resourceName;
            
            if(regno.length > 0 )
            {
                top.document.getElementById('hidVehicleResourceId').value = vehicleResourceId;
                top.document.getElementById('hidVehicleResourceName').value = regno;
            }
            else
            {
                top.document.getElementById('hidVehicleResourceId').value = "";
                top.document.getElementById('hidVehicleResourceName').value = "";
            }
            myDriverSelect = radioButton;
        }
        
        function SelectResource(radioButton, resourceName, resourceId, resourceTypeId)
        {
            if (resourceTypeId == '1')
            {
                // Vehicle
                top.document.getElementById('hidVehicleResourceId').value = resourceId;
                top.document.getElementById('hidVehicleResourceName').value = resourceName;
                myVehicleSelect = radioButton;
            }
            else if (resourceTypeId == '2')
            {
                // Trailer
                top.document.getElementById('hidTrailerResourceId').value = resourceId;
                top.document.getElementById('hidTrailerResourceName').value = resourceName;
                myTrailerSelect = radioButton;
            }
            else if (resourceTypeId == '3')
            {
                // Driver
                top.document.getElementById('hidDriverResourceId').value = resourceId;
                top.document.getElementById('hidDriverResourceName').value = resourceName;
                
                //Use the Vehicle if applicable
                
            }
        }
        
        function LinkJob(radioButton, sourceJobId, sourceInstructionId, resourceTypeId)
        {
            top.document.getElementById('hidLinkJobSourceJobId').value = sourceJobId;
            top.document.getElementById('hidLinkJobSourceInstructionId').value = sourceInstructionId;
            
            if (resourceTypeId == '1')
            {
               jobSelect = radioButton;
            }
            else if (resourceTypeId == '2')
            {
                driverSelect = radioButton;
            }
            else if (resourceTypeId == '3')
            {
                vehicleSelect = radioButton;
            }
            else if (resourceTypeId == '4')
            {
                trailerSelect = radioButton;
            }            
        }
        
        function GetShowPoint(dataItem)
        {
            if(dataItem.GetMember("PointId").Value != "")
                return "<span onmouseover=\"showHelpTipUrl(event,'../point/getPointAddresshtml.aspx?pointId=" + dataItem.GetMember("PointId").Value + "');\" onmouseout=\"hideHelpTip(this);\" class=\"helpLink\"> " + dataItem.GetMember("Description").Value  + "</span>";
            else
                return "";
        }
    </script>
    
    <script type="text/javascript">
    
        function OnPanelResize()
        {
            try{
                var thePanelbar = document.getElementById("RadPanelbar1");
                var intCompensate = 125;
                var documentObj = document.documentElement;
                
                if (window.opera || (document.all && !(document.compatMode && document.compatMode == "CSS1Compat")))
                {
                    documentObj = document.body;
                }
                
                thePanelbar.style.height = (parseInt(documentObj.clientHeight) - intCompensate) + "px";
                
                if (!document.readyState)
                {
                    document.readyState = "complete";
                }
                
                RadPanelbar1.Height = parseInt(documentObj.clientHeight) - intCompensate;
                RadPanelbar1.InitFullExpandOpera();
            }
            catch(e){}
        }
     
        function ShowClockIn(dataItem)
        {
            var ret;
            if(dataItem.GetMember("StartDateTime2").Value.length > 0)
            {
                var dateStartTime = dataItem.GetMember("StartDateTime2").Value;
                var dteString = dateStartTime.substr(11,2) + ":" + dateStartTime.substr(14,2);
                ret = "<a id=\"lnkStartTime_" + dataItem.GetMember("DriverResourceId").Value + "\" href=\"javascript:ClockIn(" + dataItem.GetMember("DriverResourceId").Value + ",this)\">" + dteString + "</a>";
            }
            else
            {
                ret = "<a id=\"lnkStartTime_" + dataItem.GetMember("DriverResourceId").Value + "\" href=\"javascript:ClockIn(" + dataItem.GetMember("DriverResourceId").Value + ", this)\"><img src=\"../images/clockin.gif\" border=\"0\" /></a>";
            }
            
            return ret;
        }
        
        function GetPCVLink(dataItem)
        {
            var ret;
            
            if(dataItem.GetMember("PCVsOnJob").Value != null && dataItem.GetMember("PCVsOnJob").Value != undefined && parseInt(dataItem.GetMember("PCVsOnJob").Value) > 0 )
            {
                var jobID = dataItem.GetMember("JobId").Value;
                ret = "<a id=\"lnkPCV_" + jobID + "\" href=\"javascript:ViewPCV(" + jobID + ", this);\"><img src=\"/App_Themes/Orchestrator/Img/MasterPage/icon-pcv.png\" alt=\"PCV's attached to Job\" border=\"0\" /></a>";
            }
            
            return ret;
        }
        
        function ShowSetTravelNotes(dataItem)
        {
            var ret;
            if(dataItem.GetMember("TravelNotes").Value.trim().length > 0)
            {
                var travelNotes = dataItem.GetMember("TravelNotes").Value;
                ret = "<a id=\"lnkSetTravelNotes_" + dataItem.GetMember("DriverResourceId").Value + "\" href=\"javascript:SetTravelNotes(" + dataItem.GetMember("DriverResourceId").Value + ",this)\">" + travelNotes+ "</a>";
            }
            else
            {
                ret = "<a id=\"lnkSetTravelNotes_" + dataItem.GetMember("DriverResourceId").Value + "\" href=\"javascript:SetTravelNotes(" + dataItem.GetMember("DriverResourceId").Value + ", this)\">Set Travel Notes</a>";
            }
            
            return ret;
        }
        
        
        function ShowDate(strDate)
        {        
            var ret ;
            if (strDate.length > 0)
                ret =  strDate.substr(8, 2) + "/" + strDate.substr(5, 2) + " " + strDate.substr(11, 2) + ":" + strDate.substr(14,2);
            else
                ret = "";
            return ret;
        }
        
        function ShowFuture(resourceId, resourceTypeId)
	    {
		    window.open('<%=Page.ResolveUrl("~/Resource/Future.aspx")%>?wiz=true&resourceId=' + resourceId + '&resourceTypeId=' + resourceTypeId + '&fromDateTime=<%=StartDateString%>', 'resourceFuture', 'width=1050,height=500,resizable=yes,scrollbars=yes');
	    }
    	
	    function GetJobLinks(dataItem)
	    {
	        if(dataItem.GetMember("TargetInstructionId").Value != "")
	           return  "<span onmouseover=\"showHelpTipUrl(event,'~/job/getJobDataForJobPopup.aspx?JobId=" + dataItem.GetMember("TargetJobId").Value + "&InstructionId=" + dataItem.GetMember("TargetInstructionId").Value + "');\" onmouseout=\"hideHelpTip(this);\" class=\"helpLink\"><img src=\"../images/jobLink.gif\" height=\"18\" width=\"15\"  onclick=\"openJobDetailsWindow(" + dataItem.GetMember("TargetJobId").Value + ")\"/></span>";
	        else
	            return null;
	    }
	
        function openGiveResourcesWindow(driverResourceId, driverFullName, vehicleResourceId, vehicleRegNo, trailerResourceId, trailerRef, controlArea, TrafficArea)
        {
            var qs = "dr=" + driverResourceId + "&fn=" + driverFullName + "&vr=" + vehicleResourceId + "&regNo=" + vehicleRegNo + "&tr=" + trailerResourceId + "&tn=" + trailerRef + "&ca=" + controlArea + "&ta=" + TrafficArea;
            <%=dlgGiveResources.ClientID %>_Open(qs);
        }
        
        function openSetResourceLocation(driverResourceId, driverFullName, vehicleResourceId, vehicleRegNo, trailerResourceId, trailerRef)
        {
            var qs = "dr=" + driverResourceId + "&dn=" + driverFullName + "&vr=" + vehicleResourceId + "&vn=" + vehicleRegNo + "&tr=" + trailerResourceId + "&tn=" + trailerRef;
            <%=dlgSetLocation.ClientID %>_Open(qs);
        }

        function ExpandDrivers()
        {
            var v = eval(RadPanelbar1);
            var panel = v.GetPanelItemById("Panel1");
            panel.Expand();
        }
        
        function ExpandTrailers()
        {
            var v = eval(RadPanelbar1);
            var panel = v.GetPanelItemById("Panel2");
            panel.Expand();
        }

        function ExpandVehicles()
        {
            var v = eval(RadPanelbar1);
            var panel = v.GetPanelItemById("Panel3");
            panel.Expand();
        }
   
        function ClearSelection()
        {
            top.document.getElementById('hidVehicleResourceId').value = '';
            top.document.getElementById('hidVehicleResourceName').value = '';
            if ( myDriverSelect != 0 )
            {
                myDriverSelect.checked = false;
                myDriverSelect = 0;
            }
            
            top.document.getElementById('hidTrailerResourceId').value = '';
            top.document.getElementById('hidTrailerResourceName').value = '';
            if ( myTrailerSelect != 0 )
            {
                myTrailerSelect.checked = false;
                myTrailerSelect = 0;
            }
            
            top.document.getElementById('hidDriverResourceId').value = '';
            top.document.getElementById('hidDriverResourceName').value = '';
            if (myVehicleSelect != 0 )
            {
                myVehicleSelect.checked = false;
                myVehicleSelect = 0;
            }
            
            top.document.getElementById('hidLinkJobSourceJobId').value = '';
            top.document.getElementById('hidLinkJobSourceInstructionId').value = '';
            
            if (jobSelect !=0 )
            {
                jobSelect.checked = false;
                //jobSelect = 0;
            }
            if (driverSelect !=0 )
            {
                driverSelect.checked = false;
                //driverSelect = 0;
            }
            if (vehicleSelect !=0 )
            {
                vehicleSelect.checked = false;
                //vehicleSelect = 0;
            }
            if (trailerSelect !=0 )
            {
                trailerSelect.checked = false;
                //trailerSelect = 0;
            }
            alert('Your selections have been cleared.');
        }
    
        window.onload = OnPanelResize;

        function sendMwfMessage(driverID) {
            new MwfDriverMessaging([driverID]).sendMessage()
                .done(function() {
                    alert('The message has been sent.');
                })
                .fail(function (error) {
                    alert(error);
                });
        }

        function showDriverTime(driverID) {
            new DriverTime(driverID).show();
        }

        function pageLoad() {
            if (!Boolean.parse('<%= this.IsDriverTimeEnabled %>')) {
                var driverTimeMenu = ResourceGridContextMenu.ChildItemArray[_driverTimeMenuIndex];
                ResourceGridContextMenu.RemoveItem(driverTimeMenu);
            }
        }

</script>
    
    <div id="divPointAddress" style="z-index:5; display:none; background-color:Wheat; padding:2px 2px 2px 2px;">
	    <table style="background-color: white; border:solid 1pt black; " cellpadding="2">
		    <tr>
			    <td><span id="spnPointAddress"></span></td>
		    </tr>
	    </table>
    </div>
    
	<asp:Label ID="lblInjectScript" runat="server"></asp:Label>
	    
    </form>
</body>
</html>
