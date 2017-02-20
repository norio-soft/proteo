<%@ Page Language="C#" AutoEventWireup="True" Inherits="Default2" Trace="false"  Codebehind="Default.aspx.cs" MasterPageFile="~/default_tableless.master"   %>

<%@ Register Assembly="System.Web.Silverlight" Namespace="System.Web.UI.SilverlightControls" TagPrefix="asp" %>

<%@ Register Src="~/usercontrols/webparts/wpPageMenu.ascx" TagName="PageMenu" TagPrefix="uc" %>
<%@ Register Src="~/usercontrols/webparts/wpOrdersReadyToInvoice.ascx" TagName="OrdersReadyToInvoice" TagPrefix="wp" %>
<%@ Register Src="~/usercontrols/webparts/wpOrdersByCreator.ascx" TagName="OrdersByCreator" TagPrefix="wp" %>
<%@ Register Src="~/usercontrols/webparts/wpOSCallIns.ascx" TagName="OSCallIns" TagPrefix="wp" %>

<%@ Register Src="~/usercontrols/webparts/wpClientPalletBalance.ascx" TagName="ClientPalletBalance" TagPrefix="wp" %>
<%@ Register Src="~/usercontrols/webparts/wpPointPalletBalance.ascx" TagName="PointPalletBalance" TagPrefix="wp" %>

<%@ Register Src="~/usercontrols/webparts/wpTrafficNews.ascx" TagName="TrafficNews" TagPrefix="wp" %>
<%@ Register Src="~/usercontrols/webparts/wpClientExtras.ascx" TagName="ClientExtras" TagPrefix="wp" %>
<%@ Register Src="~/usercontrols/webparts/wpPointsAwaiting.ascx" TagName="PointsAwaiting" TagPrefix="wp" %>
<%@ Register Src="~/usercontrols/webparts/wpMessagesAwaiting.ascx" TagName="MessagesAwaiting" TagPrefix="wp" %>
<%@ Register Src="~/usercontrols/webparts/wpEOTL.ascx" TagName="EOTL" TagPrefix="wp" %>
<%@ Register Src="~/usercontrols/webparts/wpTop10PCVLocations.ascx" TagName="Top10PCVLocations" TagPrefix="wp" %>


<%@ Register Src="~/usercontrols/webparts/wpOrdersByState.ascx" TagName="OrdersByState" TagPrefix="wp" %>
<%@ Register Src="~/usercontrols/webparts/wpOrdersByStateGraph.ascx" TagName="OrdersByStateGraph" TagPrefix="wp" %>
<%@ Register Src="~/usercontrols/webparts/wpUnapprovedOrders.ascx" TagName="UnapprovedOrders" TagPrefix="wp" %>
<%@ Register Src="~/usercontrols/webparts/wpUnallocatedOrders.ascx" TagName="UnallocatedOrders" TagPrefix="wp" %>

<%@ Register Src="~/usercontrols/webparts/wpOutstandingPODS.ascx" TagName="OutstandingPOD" TagPrefix="wp" %>

<asp:Content ContentPlaceHolderID="Header" runat="server">
	<script src="script/jquery-ui-1.9.2.min.js" type="text/javascript"></script>
    <script src ="/script/cookie-session-id.js" type="text/javascript"></script>
    
    <script src="/bower_components/d3/d3.min.js"></script>
    
    <link href="/bower_components/nvd3/build/nv.d3.min.css" rel="stylesheet" />
    <script src="/bower_components/nvd3/build/nv.d3.min.js"></script>

    <script src="/script/api-service.js"></script>

	<script type="text/javascript">

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
			if (tabText != "" && tabText != null && tabText != "TMS Dashboard") {
				var tab = tabStrip.findTabByText(tabText);
				if (tab != null && tab.get_url() != "nofleetmetrik.aspx") {
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
		}

	</script>
	<style type="text/css">
		.fleetmetriktab .rtsOut
		{
			background-color: #5a7495;  
			font-size: 11px; 
			color: #FFF;
			background-image: url('/app_themes/FleetMetrik/img/Masterpage/bluetoolbar-bg.jpg');
			background-repeat: repeat-x;
			background-position: top;
			padding: 3px 3px 3px 5px;
			border-bottom: 1px solid #363636;
		}
		
		.fleetmetriktab .rtsOut .rtsIn .rtsTxt
		{
			color:Yellow;
		}
	</style>
</asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Dashboard</h1></asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Panel id="pnlWebParts" runat="server" Visible="false">
        <telerik:RadTabStrip ID="radTabs" runat="server" OnClientTabSelected="onTabSelected">
			<Tabs>
				<telerik:RadTab Text="TMS Dashboard" runat="server" NavigateUrl="default.aspx" Selected="true"  />
				<telerik:RadTab Text="FleetMetrik Dashboard"   runat="server" NavigateUrl="fmwebparts2.aspx" CssClass="fleetmetriktab" />
			</Tabs>
		</telerik:RadTabStrip>
		<div>

	    <table width="100%">
			<tr>
				<td colspan="3" align="right">
					<asp:WebPartManager runat="server" id="wpmManager" Personalization-Enabled="true" OnAuthorizeWebPart="wpmManager_AuthorizeWebPart" />
					<uc:PageMenu runat="server" id="PageMenu"></uc:PageMenu>
					<asp:CatalogZone ID="CatalogZone1" Runat="server" BackColor="#F7F6F3" BorderColor="#363636" BorderWidth="1px" Font-Names="Verdana" Padding="6" width="300">
						<ZoneTemplate>
							<asp:PageCatalogPart Runat="server" ID="PageCatalogPart1" title="Parts On Page That Are Closed" />
						</ZoneTemplate>
						<HeaderVerbStyle Font-Bold="False" Font-Size="0.8em" Font-Underline="False" ForeColor="#333333" />
						<PartTitleStyle CssClass="webpartTitleBar" />
						<PartChromeStyle cssclass="webpartContainer" /> 
						<InstructionTextStyle Font-Size="0.8em" ForeColor="#333333" />
						<PartLinkStyle Font-Size="0.8em" />
						<EmptyZoneTextStyle Font-Size="0.8em" ForeColor="#333333" />
						<LabelStyle Font-Size="0.8em" ForeColor="#333333" />
						<VerbStyle Font-Names="Verdana" Font-Size="0.8em" ForeColor="#333333" />
						<PartStyle BorderColor="#F7F6F3" BorderWidth="5px" />
						<SelectedPartLinkStyle Font-Size="0.8em" />
						<FooterStyle cssclass="webpartCatalogFooter" />
						<HeaderStyle cssclass="webpartCatalogHeader" />
						<EditUIStyle Font-Names="Verdana" Font-Size="0.8em" ForeColor="#333333" />
					</asp:CatalogZone>
					<asp:EditorZone ID="EditorZone1" Runat="server" BackColor="#F7F6F3" BorderColor="#363636" BorderWidth="1px" Font-Names="Verdana" Padding="6">
						<ZoneTemplate>
							<asp:AppearanceEditorPart Runat="server" ID="AppearanceEditorPart1" />
							<asp:PropertyGridEditorPart runat="server" Id="PropertyGridEditorPart1"></asp:PropertyGridEditorPart>
						</ZoneTemplate>
						<HeaderStyle cssclass="webpartEditorHeader" />
						<LabelStyle Font-Size="0.8em" ForeColor="#333333" />
						<HeaderVerbStyle Font-Bold="False" Font-Size="11px" Font-Underline="False" ForeColor="#333333" />
						<PartChromeStyle cssclass="webpartEditorFooter" /> 
						<PartStyle BorderColor="#F7F6F3" BorderWidth="5px" />
						<FooterStyle cssclass="webpartCatalogFooter" />
						<EditUIStyle Font-Names="Verdana" Font-Size="11px" ForeColor="#333333" />
						<InstructionTextStyle Font-Size="11px" ForeColor="#333333" />
						<ErrorStyle Font-Size="11px" />
						<VerbStyle Font-Names="Verdana" Font-Size="0.8em" ForeColor="#333333" />
						<EmptyZoneTextStyle Font-Size="11px" ForeColor="#333333" />
						<PartTitleStyle CssClass="webpartEditorTitleBar" />
					</asp:EditorZone>
				</td>
			</tr>
			<tr valign="top">
				<td width="33%">
					<asp:WebPartZone ID="Left" runat="server" BorderColor="#CCCCCC" Font-Names="Verdana" Height="100%" Padding="6" Width="100%" >
						<ZoneTemplate>
							<div id="wpCallIns"></div>
							<wp:OrdersByCreator runat="server" ID="OrdersByCreator" />
							<wp:OrdersReadyToInvoice runat="server" ID="ucOrdersReadyToInvoice" />
							<wp:OrdersByStateGraph runat="server" ID="ucOrderByStateGraph" />
							<wp:OrdersByState runat="server" ID="ucOrdersByState" />
							<wp:ClientPalletBalance runat="server" id="ucClientPalletBalance"></wp:ClientPalletBalance>
							<wp:PointPalletBalance runat="server" id="ucPointPalletBalance"></wp:PointPalletBalance>
						</ZoneTemplate>

						<PartChromeStyle cssclass="webpartContainer" />
						<PartTitleStyle CssClass="webpartTitleBar" />
						<MenuLabelStyle CssClass="webpartIconArrow" />
						<MenuLabelHoverStyle CssClass="webpartIconArrow" />
						<TitleBarVerbStyle cssclass="webpartTitleVerb" />
						<EmptyZoneTextStyle Font-Size="0.8em" />
						<MenuPopupStyle BorderWidth="1px" Font-Size="11px" BorderStyle="Solid" BorderColor="#363636" BackColor="whitesmoke" />
						<MenuVerbStyle ForeColor="#5a7495"  Font-Names="Verdana" />
						<MenuVerbHoverStyle ForeColor="#000000"  Font-Names="Verdana" />
						<HeaderStyle Font-Size="11px" ForeColor="#CCCCCC" HorizontalAlign="Center" />
						<PartStyle Font-Size="11px" ForeColor="#333333" />
					</asp:WebPartZone>
				</td>
				<td width="33%">
					<asp:WebPartZone ID="Middle" runat="server" BorderColor="#CCCCCC" Font-Names="Verdana" Height="100%" Padding="6" Width="100%">
						<ZoneTemplate>
							<wp:UnapprovedOrders runat="server" id="ucUnapprovedOrders"></wp:UnapprovedOrders>
							<wp:UnallocatedOrders runat="server" id="ucUnallocatedOrders"></wp:UnallocatedOrders>
							<wp:OSCallIns runat="server" id="ucOSCallIns"></wp:OSCallIns>
							<wp:ClientExtras runat="server" id="ucClientExtras"></wp:ClientExtras>
							<wp:PointsAwaiting runat="server" id="ucPointsAwaiting"></wp:PointsAwaiting>
							<wp:MessagesAwaiting runat="server" id="ucMessagesAwaiting"></wp:MessagesAwaiting>
							<wp:OutstandingPOD runat="server" id="ucOutstandingPOD"></wp:OutstandingPOD>
						</ZoneTemplate>
						<PartChromeStyle cssclass="webpartContainer" />
						<PartTitleStyle CssClass="webpartTitleBar" />
						<MenuLabelStyle CssClass="webpartIconArrow" />
						<MenuLabelHoverStyle CssClass="webpartIconArrow" />
						<TitleBarVerbStyle cssclass="webpartTitleVerb" />
						<EmptyZoneTextStyle Font-Size="0.8em" />
						<MenuPopupStyle BorderWidth="1px" Font-Size="11px" BorderStyle="Solid" BorderColor="#363636" BackColor="whitesmoke" />
						<MenuVerbStyle ForeColor="#5a7495"  Font-Names="Verdana" />
						<MenuVerbHoverStyle ForeColor="#000000"  Font-Names="Verdana" />
						<HeaderStyle Font-Size="11px" ForeColor="#CCCCCC" HorizontalAlign="Center" />
						<PartStyle Font-Size="11px" ForeColor="#333333" />
					</asp:WebPartZone>
					
				</td>
				<td width="33%">
					<asp:WebPartZone ID="Right" runat="server" BorderColor="#CCCCCC" Font-Names="Verdana" Height="100%" Padding="6" Width="100%">
						<ZoneTemplate>
						   <wp:TrafficNews runat="server" ID="ucTrafficNews"></wp:TrafficNews>
							<wp:EOTL runat="server" ID="ucEOTL"></wp:EOTL>
							<wp:Top10PCVLocations runat="server" ID="ucTop10PCVLocations" />
							

						</ZoneTemplate>
						<PartChromeStyle cssclass="webpartContainer" />
						<PartTitleStyle CssClass="webpartTitleBar" />
						<MenuLabelStyle CssClass="webpartIconArrow" />
						<MenuLabelHoverStyle CssClass="webpartIconArrow" />
						<TitleBarVerbStyle cssclass="webpartTitleVerb" />
						<EmptyZoneTextStyle Font-Size="0.8em" />
						<MenuPopupStyle BorderWidth="1px" Font-Size="11px" BorderStyle="Solid" BorderColor="#363636" BackColor="whitesmoke" />
						<MenuVerbStyle ForeColor="#5a7495"  Font-Names="Verdana" />
						<MenuVerbHoverStyle ForeColor="#000000"  Font-Names="Verdana" />
						<HeaderStyle Font-Size="11px" ForeColor="#CCCCCC" HorizontalAlign="Center" />
						<PartStyle Font-Size="11px" ForeColor="#333333" />
					</asp:WebPartZone>
				 </td>
			</tr>
		</table>
	</div>
	</asp:Panel>
	<asp:Panel id="pnlLogo" runat="server" visible="false">
		<table style="width:100%; height:100%">
			<tr style="height:100%">
				<td style="width:100%; text-align:center;">
					<img src="/images/orch-logo-circle.png" />
				</td>
			</tr>
		</table>
	</asp:Panel>
	<asp:Panel ID="pnlClient" runat="server" Visible="false">
	<h1>Client Suite</h1>
		<div class="layoutDefault-boxOuter" style="width:400px;">
			<div class="layoutDefault-boxInner">
				<div class="layoutDefault-boxHeader">
					<h1>Orders for this week</h1>
				</div>
				<div class="layoutDefault-boxContent">
					<asp:Silverlight ID="slOrdersForWeek" runat="server" Source="~/ClientBin/ClientDashboard/Orchestrator.SL.ClientDashboard.xap" MinimumVersion="2.0.31005.0" Width="100%" Height="300" InitParameters="ShowPart=OrdersByDayOfWeek" />
				</div>
			</div>
		</div>
		<div class="layoutDefault-boxOuter" style="width:400px;">
			<div class="layoutDefault-boxInner">
				<div class="layoutDefault-boxHeader">
					<h1>Outstanding POD's</h1>
				</div>
				<div class="layoutDefault-boxContent">
					<asp:Silverlight ID="slOutstandingPODs" runat="server" Source="~/ClientBin/ClientDashboard/Orchestrator.SL.ClientDashboard.xap" MinimumVersion="2.0.31005.0" Width="100%" Height="300" InitParameters="ShowPart=OutstandingPODSummary" />
				</div>
			</div>
		</div>
		<div class="layoutDefault-boxOuter" style="width:330px;">
			<div class="layoutDefault-boxInner">
				<div class="layoutDefault-boxHeader">
					<h1>Unbilled orders up to <asp:Label id="lblUnbilledEndDate" runat="server"></asp:Label></h1>
				</div>
				<div class="layoutDefault-boxContent">
					<asp:Silverlight ID="slUninvoicedWork" runat="server" Source="~/ClientBin/ClientDashboard/Orchestrator.SL.ClientDashboard.xap"  MinimumVersion="2.0.31005.0" Width="300" Height="300" InitParameters="ShowPart=AllUninvoicedWork"  />
				</div>
			</div>
		</div>    
		 <div class="layoutDefault-boxOuter" style="width:500px;">
			<div class="layoutDefault-boxInner">
				<div class="layoutDefault-boxHeader">
					<h1>Sales history</h1>
				</div>
				<div class="layoutDefault-boxContent">
					<asp:Silverlight ID="slInvoicedRevenue" runat="server" Source="~/ClientBin/ClientDashboard/Orchestrator.SL.ClientDashboard.xap" MinimumVersion="2.0.31005.0" Width="100%" Height="300" InitParameters="ShowPart=ClientTurnoverGraph" />
				</div>
			</div>
		</div>    
		<div class="clearDiv"></div>
	</asp:Panel>
	<asp:Panel ID="pnlSubCon" runat="server" Visible="false">
	<h1>Member Portal</h1>
		<div class="clearDiv"></div>
	</asp:Panel>
	
</asp:Content>
