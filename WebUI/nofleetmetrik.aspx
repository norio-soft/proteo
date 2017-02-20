<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="nofleetmetrik.aspx.cs" Inherits="Orchestrator.WebUI.nofleetmetrik" Theme="Fleetmetrik" MasterPageFile="~/default_tableless.master" %>
<%@ Register Src="~/usercontrols/webparts/wpTrafficNews.ascx" TagName="TrafficNews" TagPrefix="wp" %>
<%@ Register Src="~/usercontrols/webparts/wpPageMenu.ascx" TagName="PageMenu" TagPrefix="uc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">
	<script src="script/jquery-ui-1.9.2.min.js" type="text/javascript"></script>
	
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
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>FleetMetrik Dashboard</h1></asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
 <telerik:RadTabStrip ID="radTabs" runat="server">
			<Tabs>
				<telerik:RadTab Text="TMS Dashboard" runat="server" NavigateUrl="default.aspx"/>
				<telerik:RadTab Text="FleetMetrik Dashboard" runat="server" NavigateUrl="fmwebparts2.aspx"  Selected="true" CssClass="fleetmetrikTab">
                   
                </telerik:RadTab>
			</Tabs>
		</telerik:RadTabStrip>
    <div style="text-align:center; margin:30px;font-size:12pt;">
       You currently do not have FleetMetrik, for information on how to get this please contact us on 0845 644 3750. 
    </div>
</asp:Content>
