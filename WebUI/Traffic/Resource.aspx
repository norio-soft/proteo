<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Traffic.Resource" Async="true" Theme="outlook" Codebehind="Resource.aspx.cs" %>
<%@ Register TagPrefix="Componentart" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register TagPrefix="cs" Namespace="Codesummit" Assembly="WebModalControls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>Orchestrator - My Resource</title>
     <link href="../style/Styles.css" rel="stylesheet" type="text/css" />
     <LINK href="../style/trafficsheetprint.css" type="text/css" rel="stylesheet" media="print">
     <script language="javascript" src="../script/scripts.js" type="text/javascript"></script>
    <script language="javascript" src="../script/popAddress.js" type="text/javascript"></script>
     <link href="../style/helpTip.css" rel="stylesheet" type="text/css" />
     <script src="../script/helptip.js"></script>
    
     <style>
        html,body
        {
          margin:0;
          padding:0;
          height:100%;
          border:none
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
        } 
    </style>
  
  <script type="text/javascript">
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

      function OnPanelExpanded(sender, args) {
          //Callback Jobs Coming In
          if (sender.ID == "Panel4" && !_jobsComingInLoaded) {
              Panel4_1_cbJobsComingIn.Callback();
              _jobsComingInLoaded = true;
          }

          //Callback Drivers Coming In
          if (sender.ID == "Panel5" && !_driversComingInLoaded) {
              Panel5_1_cbDriversComingIn.Callback();
              _driversComingInLoaded = true;
          }

          //Callback Vehicles Coming In
          if (pnlsenderID == "Panel6" && !_vehiclesComingInLoaded) {
              Panel6_1_cbVehiclesComingIn.Callback();
              _vehiclesComingInLoaded = true;
          }

          //Callback Trailers Coming In
          if (sender.ID == "Panel7" && !_trailersComingInLoaded) {
              Panel7_1_cbTrailersComingIn.Callback();
              _trailersComingInLoaded = true;
          }

          //Callback My Vehicles
          if (sender.ID == "Panel3" && !_MyVehicles) {
              Panel3_1_cbMyVehicles.Callback();
              _MyVehicles = true;
          }

          //Callback My Trailers
          if (sender.ID == "Panel2" && !_MyTrailers) {
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
          if (item.GetMember("ControlAreaId") != null)
              _controlArea = item.GetMember("ControlAreaId").Value;
          if (item.GetMember("TrafficAreaId") != null)
              _trafficArea = item.GetMember("TrafficAreaId").Value;
          if (item.GetMember("GPSUnitID") != null)
              _gpsUnitID = item.GetMember("GPSUnitID").Value;
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
          else if (item.Text == "Show Location")
              ShowPosition();
      }

      function ShowPosition() {
          if (_gpsUnitID == null || _gpsUnitID == "")
              alert("There is no GPS Information available for this resource.");
          else {
              var url = "/gps/getcurrentlocation.aspx?uid=" + _gpsUnitID;
              window.open(url, "", "height=600, width=630, scrollbars=0");
          }
      }
  
  </script>
 
   
</head>
<body style="background-color:#ebebd6;">
    <form id="form1" runat="server">
     <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
     <asp:Panel ID="pnlConfirmation" runat="server" Visible="false">
        <div class="MessagePanel">
            <asp:Label ID="lblMessage" runat="server"><asp:Image ID="imgIcon" runat="server" ImageUrl="~/images/ico_warning.gif" />You do not have a default filter, please contact support.</asp:Label>
        </div>
   </asp:Panel>
   <cs:webmodalanchor id="MyClientSideAnchor" title="Sub-Contract Job" runat="server" clientsidesupport="true"
		windowwidth="580" windowheight="532" scrolling="false" url="addupdatedriver.aspx" handledevent="onclick"
		linkedcontrolid="RadPanelbar1"></cs:webmodalanchor>
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
        ClientSideOnItemSelect="ContextMenuClickHandler"
        >
      <ItemLooks>
        <ComponentArt:ItemLook LookID="DefaultItemLook" CssClass="MenuItem" HoverCssClass="MenuItemHover" LabelPaddingLeft="15" LabelPaddingRight="10" LabelPaddingTop="3" LabelPaddingBottom="3" />
        <ComponentArt:ItemLook LookID="BreakItem" ImageUrl="break.gif" CssClass="MenuBreak" ImageHeight="1" ImageWidth="100%" />
        <ComponentArt:ItemLook LookId="DisabledItemLook" CssClass="DisabledMenuItem" HoverCssClass="DisabledMenuItem"  LabelPaddingLeft="15" LabelPaddingRight="10" LabelPaddingTop="3" LabelPaddingBottom="3" />
      </ItemLooks>
      </ComponentArt:Menu>
    
            <table class="HeadingCell" style="width:100%;height:25px;" cellpadding="0" cellspacing="0">
                <tr>
                    <td class="HeadingCellText" nowrap="nowrap">My Resource</td>
                    <td width="100%" align="center">
                        <font color="<%=DepotId > 0 ? "red" : "green" %>"><b>Drawing resource from <%=DepotId > 0 ? "Depot" : "Areas" %></b></font>
                    </td>
                    <td align="right" nowrap="nowrap">
                        <a id="lnkClearSection" href="javascript:ClearSelection();">Clear Selection</a>
                        &nbsp;&nbsp;
                        <a id="lnkReload" href="javascript:location.href=location.href;">Reload</a>
                    </td>
                </tr>
            </table>
      <telerik:RadPanelBar runat="server" ID="RadPanelbar1" Skin="Outlook" Height="3000" Width="100%" ExpandMode="FullExpandedItem" OnClientItemExpand="OnPanelExpanded" >
            <Items>
                 <telerik:RadPanelItem id="rpiDrivers" runat="server" Text="My Drivers" Expanded="true"  ImageUrl="/App_Themes/Orchestrator/Panelbar/Img/icon-MyDrivers.png" >
                    <Items>
                        <telerik:RadPanelItem runat="server">
                            <ItemTemplate>
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
                                        TreeLineImageWidth="22" 
                                        TreeLineImageHeight="19" 
                                        runat="server"
                                        KeyboardEnabled ="true"
                                        ImagesBaseUrl = "~/images/"
                                        TreeLineImagesFolderUrl="~/images/lines/"
                                        GroupBy="DriverType" Height="100%" FillContainer="true" OnContextMenu="ShowDriverMenu" >
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
                                                 
                                                 >
                                                 <Columns>
                                                    <ComponentArt:GridColumn DataField="DriverResourceId" HeadingText=" " DataCellClientTemplateId="PreSelectDriverTemplate" FixedWidth="true" Width="18" />
                                                    <ComponentArt:GridColumn DataField="FullName" HeadingText="Full Name" DataCellClientTemplateId="DriverTemplate"  FixedWidth="true" Width="100" />
                                                    <ComponentArt:GridColumn DataField="LastLocation" HeadingText="Last Call In"  Width="80" />
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
                                                 </Columns>
                                                </ComponentArt:GridLevel>
                                        </Levels>

                                        <ClientTemplates>
                                           
                                            <ComponentArt:ClientTemplate ID="PreSelectDriverTemplate">
    			                                <input type="radio" name="myDriverSelect" value="## DataItem.GetMember("DriverResourceId").Value ##" group="driver" onclick="javascript:SelectDriverResource(this, '## DataItem.GetMember("FullName").Value ##', ## DataItem.GetMember("DriverResourceId").Value ##, '## DataItem.GetMember("RegNo").Value ##', ## DataItem.GetMember("VehicleResourceId").Value ##);" />
                                            </ComponentArt:ClientTemplate>
                                            <ComponentArt:ClientTemplate ID="ClientTemplate2">
                                               <a href="javascript:ShowFuture('## DataItem.GetMember("DriverResourceId").Value ##','3');" onmouseover="showHelpTipUrl(event,'~/resource/driver/drivercontactpopup.aspx?identityId=Individual:##DataItem.GetMember("DriverIdentityId").Value##');" onmouseout="hideHelpTip(this);" class="helpLink">## DataItem.GetMember("FullName").Value ##</a>
                                               <br />
                                               <a href="javascript:ShowFuture('## DataItem.GetMember("VehicleResourceId").Value ##','1');" onmouseover="showHelpTipUrl(event,'~/resource/vehicle/vehiclecontactpopup.aspx?resourceId=##DataItem.GetMember("VehicleResourceId").Value##');" onmouseout="hideHelpTip(this);" class="helpLink">## DataItem.GetMember("RegNo").Value ##</a>
                                               <br />
                                               ##ShowSetTravelNotes(DataItem)##
                                               <br />
                                               ##GetPalletHandling(DataItem)##
                                               ##GetRequests(DataItem)##
                                            </ComponentArt:ClientTemplate>
                                          
                                            <ComponentArt:ClientTemplate ID="DriverStartTimeTemplate">
                                                ##ShowClockIn(DataItem )##
                                            </ComponentArt:ClientTemplate>
                                            
                                        </ClientTemplates>
                                </ComponentArt:Grid>
                            </ItemTemplate>
                        </telerik:RadPanelItem>
                    </Items>
                    </telerik:RadPanelItem>
                 <telerik:RadPanelItem ID="rpiTrailer" runat="server" Text="My Trailers" ImageUrl="/App_Themes/Orchestrator/Panelbar/Img/icon-MyTrailers.png">
                    <Items>
                        <telerik:RadPanelItem runat="server">
                            <ItemTemplate>
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
                          >
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
                       <table width="100%" height="100%" cellspacing="0" cellpadding="0" border="0">
                       <tr>
                         <td align="center">
                         <table cellspacing="0" cellpadding="0" border="0">
                         <tr>
                    <td style="font-size:10px;">Loading... </td>
                    <td><img src="../images/spinner.gif" width="16" height="16" border="0"></td>
                         </tr>
                         </table>      
                         </td>
                       </tr>
                       </table>
                     </LoadingPanelClientTemplate>
                         </Componentart:CallBack>
                            </ItemTemplate>
                        </telerik:RadPanelItem>
                    </Items>
                 </telerik:RadPanelItem>
                 <telerik:RadPanelItem id="rpiMyVehicles" runat="server" Text="My Vehicles" ImageUrl="/App_Themes/Orchestrator/Panelbar/Img/icon-MyVehicles.png">
                    <Items>
                        <telerik:RadPanelItem runat="server">
                            <ItemTemplate>
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
                                          
                                          OnContextMenu="ShowVehicleMenu">
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
                                       <table width="100%" height="100%" cellspacing="0" cellpadding="0" border="0">
                                       <tr>
                                         <td align="center">
                                         <table cellspacing="0" cellpadding="0" border="0">
                                         <tr>
                                    <td style="font-size:10px;">Loading... </td>
                                    <td><img src="../images/spinner.gif" width="16" height="16" border="0"></td>
                                         </tr>
                                         </table>      
                                         </td>
                                       </tr>
                                       </table>
                                     </LoadingPanelClientTemplate>
                                </Componentart:CallBack>
                            
                            </ItemTemplate>
                        </telerik:RadPanelItem>
                    
                    </Items>
                 </telerik:RadPanelItem>
                 <telerik:RadPanelItem id="rpiJobsComingIn" runat="server" Text="Jobs Coming In"  ImageUrl="/App_Themes/Orchestrator/Panelbar/Img/icon-injobs.png">
                    <Items>
                        <telerik:RadPanelItem runat="server">
                            <ItemTemplate>
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
                                             width="550"
                                              ImagesBaseUrl = "~/images/"
                                             
                                             >
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
                                            </ClientTemplates>
                                           
                                    </ComponentArt:Grid>
                                   </Content>
                                   <LoadingPanelClientTemplate>
                                       <table width="100%" height="100%" cellspacing="0" cellpadding="0" border="0">
                                       <tr>
                                         <td align="center">
                                         <table cellspacing="0" cellpadding="0" border="0">
                                         <tr>
                                    <td style="font-size:10px;">Loading... </td>
                                    <td><img src="../images/spinner.gif" width="16" height="16" border="0"></td>
                                         </tr>
                                         </table>      
                                         </td>
                                       </tr>
                                       </table>
                                     </LoadingPanelClientTemplate>

                                   </Componentart:CallBack>
                            </ItemTemplate>
                        </telerik:RadPanelItem>
                    </Items>
                 </telerik:RadPanelItem>
                 <telerik:RadPanelItem id="rpiDriversComingIn" runat="server" Text="Driver Coming In" ImageUrl="/App_Themes/Orchestrator/Panelbar/Img/icon-inDrivers.png">
                    <Items>
                        <telerik:RadPanelItem runat="server">
                            <ItemTemplate>
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
                                             Height="100%" OnContextMenu="ShowDriverMenu">
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
                                       <table width="465" height="480" cellspacing="0" cellpadding="0" border="0">
                                       <tr>
                                         <td align="center">
                                         <table cellspacing="0" cellpadding="0" border="0">
                                         <tr>
                                            <td style="font-size:10px;">Loading... </td>
                                            <td><img src="../images/spinner.gif" width="16" height="16" border="0"></td>
                                         </tr>
                                         </table>      
                                         </td>
                                       </tr>
                                       </table>
                                     </LoadingPanelClientTemplate>

                                   </Componentart:CallBack>
                            </ItemTemplate>
                        </telerik:RadPanelItem>
                    </Items>
                 </telerik:RadPanelItem>
                 <telerik:RadPanelItem id="rptVehiclesComingIn" runat="server" Text="Vehicles Coming In" ImageUrl="/App_Themes/Orchestrator/Panelbar/Img/icon-inVehicles.png">
                    <Items>
                        <telerik:RadPanelItem runat="server">
                            <ItemTemplate>
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
                                         Height="100%" OnContextMenu="ShowVehicleMenu">
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
                                       <table width="465" height="480" cellspacing="0" cellpadding="0" border="0">
                                       <tr>
                                         <td align="center">
                                         <table cellspacing="0" cellpadding="0" border="0">
                                         <tr>
                                    <td style="font-size:10px;">Loading... </td>
                                    <td><img src="../images/spinner.gif" width="16" height="16" border="0"></td>
                                         </tr>
                                         </table>      
                                         </td>
                                       </tr>
                                       </table>
                                     </LoadingPanelClientTemplate>

                                   </Componentart:CallBack>
                            </ItemTemplate>
                        </telerik:RadPanelItem>
                    </Items>
                 </telerik:RadPanelItem>
                 <telerik:RadPanelItem id="rpiTrailersComingIn" runat="server" Text="Trailers Coming In" ImageUrl="/App_Themes/Orchestrator/Panelbar/Img/icon-inTrailers.png">
                    <Items>
                        <telerik:RadPanelItem runat="server">
                            <ItemTemplate>
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
                                         Height="100%" OnContextMenu="ShowTrailerMenu">
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
                                       <table width="465" height="480" cellspacing="0" cellpadding="0" border="0">
                                       <tr>
                                         <td align="center">
                                         <table cellspacing="0" cellpadding="0" border="0">
                                         <tr>
                                    <td style="font-size:10px;">Loading... </td>
                                    <td><img src="../images/spinner.gif" width="16" height="16" border="0"></td>
                                         </tr>
                                         </table>      
                                         </td>
                                       </tr>
                                       </table>
                                     </LoadingPanelClientTemplate>

                                   </Componentart:CallBack>
                            </ItemTemplate>
                            </telerik:RadPanelItem>
                    </Items>
                 </telerik:RadPanelItem>
            </Items>
       </telerik:RadPanelBar>
       
     <script language="javascript" type="text/javascript">
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
        
         // Filter data
        var activeControlAreaId = <%=ControlAreaId %>;
        var activeTrafficAreaIds = "<%=TrafficAreaIds %>";
            
	     //Window Code
        function _showModalDialog(url, width, height, windowTitle)
        {
            MyClientSideAnchor.WindowHeight= height + "px";
            MyClientSideAnchor.WindowWidth= width + "px";
            
            MyClientSideAnchor.URL = url;
            MyClientSideAnchor.Title = windowTitle;
	        var returnvalue = MyClientSideAnchor.Open();
            
            
	        if (returnvalue == true)
	        {
	            document.all.form1.submit();
		    }
	        return returnvalue;	        
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
                openResizableDialogWithScrollbars('../Job/Job.aspx?wiz=true&jobId=' + jobId+ getCSID(),'1220','800');
    		else
    		    alert("This resource has never been involved on a job.");
        }
        
        function openTrafficAreaWindow(url)
        {
            _showModalDialog(url, 500, 221, "Change Traffic Area");
        }
        
        function openAlterBookedTimesWindow(jobId, lastUpdateDate)
        {
            var url = "changeBookedTimes.aspx?jobId=" + jobId + "&LastUpdateDate=" + lastUpdateDate;
            _showModalDialog(url, 500, 320, "Change Booked Times");
        }
                
        function openAlterPlannedTimesWindow(jobId, lastUpdateDate)
        {
            var url = "changePlannedTimes.aspx?jobId=" + jobId + "&LastUpdateDate=" + lastUpdateDate;
            _showModalDialog(url, 700, 320, "Change Planned Times");
        }
                
        function openSubContractWindow(jobId, lastUpdateDate)
        {
            var url = "SubContract.aspx?jobId=" + jobId + "&lastUpdateDate=" + lastUpdateDate + "&CA=" + activeControlAreaId + "&TA=" + activeTrafficAreaIds;
            _showModalDialog(url, 400,320, 'Sub-Contract Job');
        }
        
        function SetTravelNotes(resourceId, lnk)
        {
             var url = "setTravelNotes.aspx?resourceId=" + resourceId;
            
            MyClientSideAnchor.WindowHeight= 320 + "px";
            MyClientSideAnchor.WindowWidth= 400 + "px";
            MyClientSideAnchor.Title = "Set Travel Notes";
            
            MyClientSideAnchor.URL = url;
            var returnvalue = MyClientSideAnchor.Open();
            var output = MyClientSideAnchor.OutputData;
            if (output && output.length > 0)
            {
                var iStart, iLength
                iStart   = 23;
                iLength =  unescape(output).length - (4 + iStart) ;
                
                var travelNotes = unescape(output).substr(iStart, iLength);
                travelNotes = travelNotes.replace("+" ," ");
                //document.getElementById("lblStartTime").innerHTML= "<a href=\"javascript:ClockIn(" + resourceId + ");\">" + startTime + "</a>";
                document.getElementById("lnkSetTravelNotes_" + resourceId).innerHTML = travelNotes;
                //lnk.innerHTML = startTime;
            }
        }
    
        function openTravelNotesWindow(driverResourceId)
        {
            var url = "setTravelNotes.aspx?resourceId=" + driverResourceId;
            _showModalDialog(url, 400, 320, 'Set Travel Notes');
        }
        
        function openResourceWindow(InstructionId, Driver, DriverResourceId, RegNo, VehicleResourceId, TrailerRef, TrailerResourceId, legStart, legEnd, DepotCode, lastUpdateDate, jobId)
        {
          if (document.getElementById('hidDriverResourceId').value != '')
          {
            DriverResourceId = document.getElementById('hidDriverResourceId').value;
            Driver = document.getElementById('hidDriverResourceName').value;
          }
          if (document.getElementById('hidVehicleResourceId').value != '')
          {
            VehicleResourceId = document.getElementById('hidVehicleResourceId').value;
            RegNo = document.getElementById('hidVehicleResourceName').value;
          }
          if (document.getElementById('hidTrailerResourceId').value != '')
          {
            TrailerResourceId = document.getElementById('hidTrailerResourceId').value;
            TrailerRef = document.getElementById('hidTrailerResourceName').value;
          }
          
          var url = "resourceThis.aspx?iID=" + InstructionId + "&Driver=" + Driver + "&DR=" + DriverResourceId + "&RegNo=" + RegNo + "&VR=" + VehicleResourceId + "&TrailerRef=" + TrailerRef + "&TR=" + TrailerResourceId + "&LS=" + legStart + "&LE=" + legEnd + "&DC=" + DepotCode + "&CA=" + activeControlAreaId + "&TA=" + activeTrafficAreaIds + "&LastUpdateDate=" + lastUpdateDate + "&jobId=" + jobId + getCSID();

            _showModalDialog(url, 750, 500, "Resource This");
        }
        
        function openCommunicateWindow(InstructionId, Driver, DriverResourceId, JobId, lastUpdateDate)
        {
            var url = "communicateThis.aspx?iID=" + InstructionId + "&Driver=" + Driver + "&DR=" + DriverResourceId + "&jobId=" + JobId + "&LastUpdateDate=" + lastUpdateDate ;
            _showModalDialog(url, 500, 600, "Communicate This");
        }
        
        function openTrunkWindow(InstructionId, Driver, RegNo, LastUpdateDate)
        {
            var url = "trunk.aspx?iID=" + InstructionId + "&Driver=" + Driver + "&RegNo=" + RegNo + "&LastUpdateDate=" + LastUpdateDate;
            _showModalDialog(url, 550, 358, "Trunk Leg");
        }
        
        function openRemoveTrunkWindow(JobId, InstructionId, LastUpdateDate)
        {
            var url = "removetrunk.aspx?jobId=" + JobId + "&iID=" + InstructionId + "&LastUpdateDate=" + LastUpdateDate;
            _showModalDialog(url, 550, 358, "Remove Trunk Leg");
        }
        
        function openLinkJobWindow(JobId, SourceJobJobId, SourceJobInstructionId, LastUpdateDate)
        {
            var url = "linkJob.aspx?jobId=" + JobId + "&SourceJobJobId=" + SourceJobJobId + "&SourceJobInstructionId=" + SourceJobInstructionId + "&LastUpdateDate=" + LastUpdateDate;
            _showModalDialog(url, 550, 358, "Link Jobs");
        }
        
        function openUpdateLocation(instructionId)
        {
            var url = "updateResourceLocations.aspx?instructionid=" + instructionId;
            _showModalDialog(url, 400, 320, 'Update Resource Locations');
        }
        
        function openPalletHandlingWindow(jobId)
        {
            var url = "JobManagement/addupdatepallethandling.aspx?jobId=" + jobId;
            _showModalDialog(url, 550, 725, 'Configure Pallet Handling');
        }
        
        function OpenJobDetails(jobId)
	    {
            openResizableDialogWithScrollbars('../job/job.aspx?wiz=true&jobId=' + jobId + getCSID(),'600','400');
	    }
        
        function GetLegAction(instructionStateId, jobId, lastUpdateDate)
        {
            if (instructionStateId == 1)
                return "<a href='javascript:openSubContractWindow(" + jobId + ", " + lastUpdateDate +"'>Sub-Contract</a>";
        }
    	function ClockIn(resourceId)
        {
            var url = "../Resource/Driver/EnterDriverStartTimes.aspx?resourceId=" + resourceId + "&date=<%=DateTime.UtcNow.ToString("dd/MM/yyyy")%>";
            var retVal = _showModalDialog(url, 500, 280, 'Set Driver Start Time');
            
            if(retVal)
                RefreshPurpleBook();
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
  var intCompensate = 20;
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
 
   function ClockIn(resourceId, lnk)
    {
        var url = "../Resource/Driver/EnterDriverStartTimes.aspx?resourceId=" + resourceId + "&date=<%=DateTime.UtcNow.ToString("dd/MM/yyyy")%>";
        MyClientSideAnchor.WindowHeight= 280 + "px";
        MyClientSideAnchor.WindowWidth= 500+ "px";
        MyClientSideAnchor.Title = "Set Driver Start Time";
        
        MyClientSideAnchor.URL = url;
        var returnvalue = MyClientSideAnchor.Open();
        var output = MyClientSideAnchor.OutputData;
        if (output && output.length > 0)
        {
            var iStart, iLength
            iStart   = 30;
            
            var startTime = unescape(output).substr(iStart, 5);
            startTime = startTime.replace("+" ," ");
            //document.getElementById("lblStartTime").innerHTML= "<a href=\"javascript:ClockIn(" + resourceId + ");\">" + startTime + "</a>";
            document.getElementById("lnkStartTime_" + resourceId).innerHTML = startTime;
            //lnk.innerHTML = startTime;
        }
    }

    function ShowClockIn(dataItem)
    {
        var ret;
        if(dataItem.GetMember("StartDateTime2").Value.length > 0)
        {
            var dateStartTime = dataItem.GetMember("StartDateTime2").Value;
            var dteString = /*dateStartTime.substr(8,2) + "/" + dateStartTime.substr(5,2) + " " + */ dateStartTime.substr(11,2) + ":" + dateStartTime.substr(14,2);
            ret = "<a id=\"lnkStartTime_" + dataItem.GetMember("DriverResourceId").Value + "\" href=\"javascript:ClockIn(" + dataItem.GetMember("DriverResourceId").Value + ",this)\">" + dteString + "</a>";
        }
        else
        {
            ret = "<a id=\"lnkStartTime_" + dataItem.GetMember("DriverResourceId").Value + "\" href=\"javascript:ClockIn(" + dataItem.GetMember("DriverResourceId").Value + ", this)\"><img src=\"../images/clockin.gif\" border=\"0\" /></a>";
        }
        
        return ret;
    }
    
    function ShowSetTravelNotes(dataItem)
    {
        var ret;
        if(dataItem.GetMember("TravelNotes").Value.length > 0)
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
	
	function openUpdateLocation(instructionId)
        {
            var url = "updateResourceLocations.aspx?instructionid=" + instructionId;
            _showModalDialog(url, 400, 320, 'Update Resource Locations');
        }
        
        function openGiveResourcesWindow(driverResourceId, driverFullName, vehicleResourceId, vehicleRegNo, trailerResourceId, trailerRef, controlArea, TrafficArea)
        {
            var url = "giveResources.aspx?dr=" + driverResourceId + "&fn=" + driverFullName + "&vr=" + vehicleResourceId + "&regNo=" + vehicleRegNo + "&tr=" + trailerResourceId + "&tn=" + trailerRef + "&ca=" + controlArea + "&ta=" + TrafficArea;
            
            _showModalDialog(url, 400, 320, 'Give Resources');
        }
        
        function openSetResourceLocation(driverResourceId, driverFullName, vehicleResourceId, vehicleRegNo, trailerResourceId, trailerRef)
        {
            
            var url = "setlocation.aspx?dr=" + driverResourceId + "&dn=" + driverFullName + "&vr=" + vehicleResourceId + "&vn=" + vehicleRegNo + "&tr=" + trailerResourceId + "&tn=" + trailerRef;
            
            _showModalDialog(url, 500, 320, 'Set Resource Location');
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
        //window.onresize = OnPanelResize;
</script>
    

        <div id="divPointAddress" style="z-index=5;display:none; background-color:Wheat;padding:2px 2px 2px 2px;">
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
