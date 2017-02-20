<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="fmwebparts2.aspx.cs" Inherits="Orchestrator.WebUI.fmwebparts2" MasterPageFile="~/default_tableless.master" %>

<%@ Register Src="~/usercontrols/webparts/wpGenericSilverlight.ascx" TagName="GenericSilverlight" TagPrefix="wp" %>
<%@ Register Src="~/usercontrols/webparts/wpGenericSilverlightWithOrgUnit.ascx" TagName="GenericSilverlightWithOrgUnit" TagPrefix="wp" %>


<%@ Register Src="~/usercontrols/webparts/FleetMetrik/wpDriverMPGGraph.ascx" TagName="DriverMPGGraph" TagPrefix="wp" %>
<%@ Register Src="~/usercontrols/webparts/FleetMetrik/wpDriverIdlingPercentageGraph.ascx" TagName="DriverIdlingPercentageGraph" TagPrefix="wp" %>
<%@ Register Src="~/usercontrols/webparts/FleetMetrik/wpDriverBrakingGraph.ascx" TagName="DriverBrakingGraph" TagPrefix="wp" %>
<%@ Register Src="~/usercontrols/webparts/FleetMetrik/wpDriverOverRevvingGraph.ascx" TagName="DriverRevvingGraph" TagPrefix="wp" %>
<%@ Register Src="~/usercontrols/webparts/FleetMetrik/wpDriverSpeedingGraph.ascx" TagName="DriverSpeedingGraph" TagPrefix="wp" %>
<%@ Register Src="~/usercontrols/webparts/FleetMetrik/wpDriverIdlingTimeGraph.ascx" TagName="DriverIdlingTimeGraph" TagPrefix="wp" %>
<%@ Register Src="~/usercontrols/webparts/FleetMetrik/wpVehicleMPGGraph.ascx" TagName="VehicleMPGGraph" TagPrefix="wp" %>
<%@ Register Src="~/usercontrols/webparts/FleetMetrik/wpVehicleIdlingPercentageGraph.ascx" TagName="VehicleIdlingPercentageGraph" TagPrefix="wp" %>
<%@ Register Src="~/usercontrols/webparts/FleetMetrik/wpVehicleIdlingTimeGraph.ascx" TagName="VehicleIdlingTimeGraph" TagPrefix="wp" %>
<%@ Register Src="~/usercontrols/webparts/FleetMetrik/wpVehicleFleetMPGGauge.ascx" TagName="VehicleFleetMPGGauge" TagPrefix="wp" %>
<%@ Register Src="~/usercontrols/webparts/FleetMetrik/wpVehicleFleetIdlingTimeGauge.ascx" TagName="VehicleFleetIdlingTimeGauge" TagPrefix="wp" %>
<%@ Register Src="~/usercontrols/webparts/FleetMetrik/wpGazetteer.ascx" TagName="Gazetteer" TagPrefix="wp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">

    <script src="/bower_components/d3/d3.min.js"></script>

    <link href="/bower_components/nvd3/build/nv.d3.min.css" rel="stylesheet" />
    <script src="/bower_components/nvd3/build/nv.d3.min.js"></script>

    <script src="/script/api-service.js"></script>

    <script src="script/jquery-ui-1.9.2.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="/script/Silverlight.js"></script>
    <script type="text/javascript" src="/script/d3-gauge.js"></script>
    <script type="text/javascript" src="/script/jquery.dataTables.min.js"></script>
    <link href="/style/jquery.dataTables.min.css" rel="stylesheet"/>
    <script type="text/javascript" src="/script/moment.min.js"></script>
    
    <script language="text/javascript" type="text/javascript">
        


        function ShowOrder(orderID) {
            openResizableDialogWithScrollbars('/client/ClientOrderProfile.aspx?wiz=true&Oid=' + orderID, 560, 700);
        }


        function onTabSelected(sender, args) {
            var tabText = args.get_tab().get_text();
            setCookie("HomePageTab", tabText);
        }

        function selectTab() {
            var tabStrip = $find('<%= radTabs.ClientID %>');
            var tabText = getCookie("HomePageTab");
            if (tabText != "" && tabText != null && tabText != "FleetMetrik Dashboard") {
                var tab = tabStrip.findTabByText(tabText);
                if (tab != null) {
                    //tab.select();
                    window.location = tab.get_navigateUrl();
                }
            }
        }

        function setCookie(name, value, exdays) {
            var exdate = new Date();
            exdate.setDate(exdate.getDate() + exdays);
            var crumb = escape(value) + ((exdays == null) ? "" : "; expires=" + exdate.toUTCString());
            document.cookie = name + "=" + crumb;
        }

        function getCookie(name) {
            var i, x, y, cookies = document.cookie.split(";");
            for (i = 0; i < cookies.length; i++) {
                x = cookies[i].substr(0, cookies[i].indexOf("="));
                y = cookies[i].substr(cookies[i].indexOf("=") + 1);
                x = x.replace(/^\s+|\s+$/g, "");
                if (x == name) {
                    return unescape(y);
                }
            }
        }

        //pageLoad automatically called by ASPNET AJAX when page ready 
        function pageLoad() {
            selectTab();
            resizediv();
            getBenchMarks();
        }

        function resizediv() {
            // Sets and width and height to the available space
            var layoutHeaderHeight = $(".masterpagelite_layoutHeader").height();
            var layoutNavHeight = $(".masterpagelite_layoutNav").height();
            var layoutFooterHeight = $(".masterpagelite_layoutFooter").height();

            //$('#slContainer').height($('.masterpagelite_contentHolder').height());
            $('#slContainer').height($(window).height() - ((layoutHeaderHeight + 20) + (layoutNavHeight + 20) + (layoutFooterHeight + 20) + 30));
            //$('#slContainer').width($(window).width() - 20);

        }



        function onSilverlightError(sender, args) {
            var appSource = "";
            if (sender != null && sender != 0) {
                appSource = sender.getHost().Source;
            }

            var errorType = args.ErrorType;
            var iErrorCode = args.ErrorCode;

            if (errorType == "ImageError" || errorType == "MediaError") {
                return;
            }

            var errMsg = "Unhandled Error in Silverlight Application " + appSource + "\n";

            errMsg += "Code: " + iErrorCode + "    \n";
            errMsg += "Category: " + errorType + "       \n";
            errMsg += "Message: " + args.ErrorMessage + "     \n";

            if (errorType == "ParserError") {
                errMsg += "File: " + args.xamlFile + "     \n";
                errMsg += "Line: " + args.lineNumber + "     \n";
                errMsg += "Position: " + args.charPosition + "     \n";
            }
            else if (errorType == "RuntimeError") {
                if (args.lineNumber != 0) {
                    errMsg += "Line: " + args.lineNumber + "     \n";
                    errMsg += "Position: " + args.charPosition + "     \n";
                }
                errMsg += "MethodName: " + args.methodName + "     \n";
            }

            throw new Error(errMsg);
        }

        var CANBenchmarkBaseline;
        var CANBenchmarkTarget;

        function getBenchMarks()
        {
            var bm = apiService.get('<%= this.DataUrl %>');
            bm.done(function (data) {
                CANBenchmarkBaseline = data[0];
                CANBenchmarkTarget = data[1];

                createFleetGauge();
                createFleetIdlingTimeGauge();
                createDriverBreaking();
                createVehicleIdlingGraph();
                createDriverMPGGraph();
                createDriverRevvingGraph();
                createDriverIdlingGraph();
                createDriverSpeedingGraph();
                createVehicleMPGGraph();
            });
        }

        function determineBarColor(value, lowBenchmark, highBenchmark, isHighBetter){
            var amber = "#FF9900";
            var red = "#DC3912";
            var green = "#109618";
            var color = amber;

            if(isHighBetter){
                if (highBenchmark != null) {
                    if (value > highBenchmark)
                        color = green;
                    else if ((lowBenchmark != null) && value > lowBenchmark)
                        color = amber;
                    else
                        color = red;
                }

            }
            else{
                if (highBenchmark != null) {
                    if (value > highBenchmark)
                        color = red;
                    else if ((lowBenchmark != null) && value > lowBenchmark)
                        color = amber;
                    else
                        color = green;
                }
            }
            return color;
        }
    </script>
    <style type="text/css">
        BODY {
            /*background-color: rgb(12, 70, 38) !important;*/
            background-color: #1c1c1c !important;
        }

        .fleetmetriktab .rtsOut {
            background-color: #5a7495;
            font-size: 11px;
            color: #FFF;
            background-image: url('/app_themes/FleetMetrik/img/Masterpage/bluetoolbar-bg.jpg');
            background-repeat: repeat-x;
            background-position: top;
            padding: 3px 3px 3px 5px;
            border-bottom: 1px solid #363636;
        }

            .fleetmetriktab .rtsOut .rtsIn .rtsTxt {
                color: Yellow;
            }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>FleetMetrik Dashboard</h1>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadTabStrip ID="radTabs" runat="server" OnClientTabSelected="onTabSelected">
        <Tabs>
            <telerik:RadTab Text="TMS Dashboard" runat="server" NavigateUrl="default.aspx" />
            <telerik:RadTab Text="FleetMetrik Dashboard" runat="server" NavigateUrl="fmwebparts2.aspx" Selected="true" CssClass="fleetmetrikTab">
            </telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>

    <asp:WebPartManager runat="server" ID="wpmManager" Personalization-Enabled="false" OnAuthorizeWebPart="wpmManager_AuthorizeWebPart" />
    <table width="100%">
        <tr valign="top">
            <td width="33%">

                <asp:WebPartZone ID="Left" runat="server" BorderColor="#CCCCCC" Font-Names="Verdana" Height="100%" Padding="6" Width="100%">
                    <ZoneTemplate>
                        <wp:DriverBrakingGraph runat="server" ID="DriverBrakingGraph" />
                        <wp:DriverIdlingTimeGraph runat="server" ID="DriverIdlingTimeGraph" />
                        <wp:VehicleFleetMPGGauge runat="server" ID ="VehicleFleetMPGGauge" />
                        <wp:VehicleIdlingPercentageGraph runat="server" ID="VehicleIdlingPercentageGraph" />
                    </ZoneTemplate>

                    <PartChromeStyle CssClass="webpartContainer" />
                    <PartTitleStyle CssClass="webpartTitleBar" />
                    <MenuLabelStyle CssClass="webpartIconArrow" />
                    <MenuLabelHoverStyle CssClass="webpartIconArrow" />
                    <TitleBarVerbStyle CssClass="webpartTitleVerb" />
                    <EmptyZoneTextStyle Font-Size="0.8em" />
                    <MenuPopupStyle BorderWidth="1px" Font-Size="11px" BorderStyle="Solid" BorderColor="#363636" BackColor="whitesmoke" />
                    <MenuVerbStyle ForeColor="#5a7495" Font-Names="Verdana" />
                    <MenuVerbHoverStyle ForeColor="#000000" Font-Names="Verdana" />
                    <HeaderStyle Font-Size="11px" ForeColor="#CCCCCC" HorizontalAlign="Center" />
                    <PartStyle Font-Size="11px" ForeColor="#333333" />
                </asp:WebPartZone>
            </td>
            <td width="33%">
                <asp:WebPartZone ID="Middle" runat="server" BorderColor="#CCCCCC" Font-Names="Verdana" Height="100%" Padding="6" Width="100%">
                    <ZoneTemplate>
                        <wp:DriverMPGGraph runat="server" ID="DriverMPGGraph" />
                        <wp:DriverRevvingGraph runat="server" ID="DriverRevvingGraph" />
                        <wp:VehicleFleetIdlingTimeGauge runat="server" ID ="VehicleFleetIdlingTimeGauge" />
                        <wp:VehicleIdlingTimeGraph runat="server" ID="VehicleIdlingTimeGraph" />
                    </ZoneTemplate>

                    <PartChromeStyle CssClass="webpartContainer" />
                    <PartTitleStyle CssClass="webpartTitleBar" />
                    <MenuLabelStyle CssClass="webpartIconArrow" />
                    <MenuLabelHoverStyle CssClass="webpartIconArrow" />
                    <TitleBarVerbStyle CssClass="webpartTitleVerb" />
                    <EmptyZoneTextStyle Font-Size="0.8em" />
                    <MenuPopupStyle BorderWidth="1px" Font-Size="11px" BorderStyle="Solid" BorderColor="#363636" BackColor="whitesmoke" />
                    <MenuVerbStyle ForeColor="#5a7495" Font-Names="Verdana" />
                    <MenuVerbHoverStyle ForeColor="#000000" Font-Names="Verdana" />
                    <HeaderStyle Font-Size="11px" ForeColor="#CCCCCC" HorizontalAlign="Center" />
                    <PartStyle Font-Size="11px" ForeColor="#333333" />
                </asp:WebPartZone>
            </td>
            <td width="33%">

                <asp:WebPartZone ID="Right" runat="server" BorderColor="#CCCCCC" Font-Names="Verdana" Height="100%" Padding="6" Width="100%">
                    <ZoneTemplate>
                       <wp:DriverIdlingPercentageGraph runat="server" ID="DriverIdlingPercentageGraph" />
                       <wp:DriverSpeedingGraph runat="server" ID="DriverSpeedingGraph" />
                       <wp:VehicleMPGGraph runat="server" ID="VehicleMPGGraph" />
                       <wp:Gazetteer runat="server" ID="Gazetteer" />
                    </ZoneTemplate>

                    <PartChromeStyle CssClass="webpartContainer" />
                    <PartTitleStyle CssClass="webpartTitleBar" />
                    <MenuLabelStyle CssClass="webpartIconArrow" />
                    <MenuLabelHoverStyle CssClass="webpartIconArrow" />
                    <TitleBarVerbStyle CssClass="webpartTitleVerb" />
                    <EmptyZoneTextStyle Font-Size="0.8em" />
                    <MenuPopupStyle BorderWidth="1px" Font-Size="11px" BorderStyle="Solid" BorderColor="#363636" BackColor="whitesmoke" />
                    <MenuVerbStyle ForeColor="#5a7495" Font-Names="Verdana" />
                    <MenuVerbHoverStyle ForeColor="#000000" Font-Names="Verdana" />
                    <HeaderStyle Font-Size="11px" ForeColor="#CCCCCC" HorizontalAlign="Center" />
                    <PartStyle Font-Size="11px" ForeColor="#333333" />
                </asp:WebPartZone>
            </td>
        </tr>
    </table>



    <%--        <div id="slContainer" >
        <object data="data:application/x-silverlight-2," type="application/x-silverlight-2" width="100%" height="100%">
		  <param name="source" value="ClientBin/FMDashboard/FMDashboard.xap"/>
		  <param name="onError" value="onSilverlightError" />
          <param name="initParams" value="CustomerID=<%=Orchestrator.Globals.Configuration.BlueSphereCustomerId %>" />
		  <param name="background" value="white" />
		  <param name="autoUpgrade" value="true" />
		  <a href="http://go.microsoft.com/fwlink/?LinkID=149156&v=4.0.50826.0" style="text-decoration:none">
 			  <img src="http://go.microsoft.com/fwlink/?LinkId=108181" alt="Get Microsoft Silverlight" style="border-style:none"/>
		  </a>
	    </object><iframe id="_sl_historyFrame" style="visibility:hidden;height:0px;width:0px;border:0px"></iframe></div>--%>
</asp:Content>
