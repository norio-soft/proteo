<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DashboardTest.aspx.cs" Inherits="Orchestrator.WebUI.NewDashboard.DashboardTest" MasterPageFile="~/default_tableless.Master"%>
<%@ Import Namespace="Orchestrator.WebUI.NewDashboard" %>
<%@ Import Namespace="System.Data" %>
<%@ Register Src="~/usercontrols/webparts/wpTop10PCVLocations.ascx" TagName="Top10PCVLocations" TagPrefix="wp" %>
<%@ Register Src="~/usercontrols/webparts/wpClientPalletBalance.ascx" TagName="ClientPalletBalance" TagPrefix="wp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">

    <script src="/json2.js" type="text/javascript"></script>
    <link rel="stylesheet" type="text/css" href="/NewDashboard/Scripts/jslate/css/jquery-ui.css" /><link rel="stylesheet" type="text/css" href="/NewDashboard/Scripts/jslate/css/codemirror/codemirror.css" /><link rel="stylesheet" type="text/css" href="/NewDashboard/Scripts/jslate/css/codemirror/ambiance.css" /><link rel="stylesheet" type="text/css" href="/NewDashboard/Scripts/jslate/css/colorbox.css" /><link rel="stylesheet" type="text/css" href="/NewDashboard/Scripts/jslate/css/topnav.css" /><script type="text/javascript" src="/NewDashboard/Scripts/jslate/js/jquery.js"></script><script type="text/javascript" src="/NewDashboard/Scripts/jslate/js/jquery-ui.js"></script><script type="text/javascript" src="/NewDashboard/Scripts/jslate/js/jquery.colorbox-min.js"></script><script type="text/javascript" src="/NewDashboard/Scripts/jslate/js/codemirror/codemirror.js"></script><script type="text/javascript" src="/NewDashboard/Scripts/jslate/js/codemirror/javascript.js"></script><script type="text/javascript" src="/NewDashboard/Scripts/jslate/js/codemirror/xml.js"></script><script type="text/javascript" src="/NewDashboard/Scripts/jslate/js/codemirror/css.js"></script><script type="text/javascript" src="/NewDashboard/Scripts/jslate/js/codemirror/htmlmixed.js"></script><script type="text/javascript" src="/NewDashboard/Scripts/jslate/js/highstock.js"></script><script type="text/javascript" src="/NewDashboard/Scripts/jslate/js/gray.js"></script><script type="text/javascript" src="/NewDashboard/Scripts/jslate/js/d3/d3.js"></script><script type="text/javascript" src="/NewDashboard/Scripts/jslate/js/d3/d3.csv.js"></script><script type="text/javascript" src="/NewDashboard/Scripts/jslate/js/d3/d3.chart.js"></script><script type="text/javascript" src="/NewDashboard/Scripts/jslate/js/d3/d3.geo.js"></script><script type="text/javascript" src="/NewDashboard/Scripts/jslate/js/d3/d3.geom.js"></script><script type="text/javascript" src="/NewDashboard/Scripts/jslate/js/d3/d3.layout.js"></script><script type="text/javascript" src="/NewDashboard/Scripts/jslate/js/d3/d3.time.js"></script> 
    <script type="text/javascript" src="/NewDashboard/Scripts/datatables/media/js/jquery.dataTables.js"></script>
    <script type="text/javascript" src="/NewDashboard/Scripts/datatables/media/js/jquery.dataTables.min.js"></script>
    <link rel="Stylesheet" type="text/css" href="/NewDashboard/Scripts/datatables/media/css/jquery.dataTables.css" />
    <link rel="Stylesheet" type="text/css" href="/NewDashboard/Scripts/datatables/media/css/demo_table.css" />
    <link rel="Stylesheet" type="text/css" href="/NewDashboard/Scripts/datatables/media/css/demo_table_jui.css" />

<script language="javascript" type="text/javascript">
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

<style type="text/css">
    /** Scaffold View **/
dl {
	line-height: 2em;
	margin: 0em 0em;
	width: 60%;
}
dl .altrow {
	background: #f4f4f4;
}
dt {
	font-weight: bold;
	padding-left: 4px;
	vertical-align: top;
}
dd {
	margin-left: 10em;
	margin-top: -2em;
	vertical-align: top;
}

.indent{
    margin-left: 20px;
}

.dragbox{
    margin:5px 2px  20px;
    
    position:relative;
    border:1px solid #222;
    -moz-border-radius:5px;
    -webkit-border-radius:5px;
}
.dragbox .header{
    margin:0;
    padding-right:5px;
    background:#222;
    border-bottom:1px solid #222;
    font-size: 10px;
    cursor:move;
}
.dragbox h2{
    font-size:12px;
    color:#000;
}
.dragbox-content{
    
    min-height:100px; margin:5px;
    font-family:'Lucida Grande', Verdana; font-size:0.8em; line-height:1.5em;
}
#flashMessage, #authMessage{
    position: fixed;
    float: left;
    top: 0px;
    left: 50%;
    padding: 5px;
    background-color: white;
    text-align: center;
    z-index:10;
    color: black;

    -webkit-border-radius: 0px 0px 10px 10px;
    -moz-border-radius: 0px 0px 10px 10px;
    border-radius: 0px 0px 10px 10px;
}

.headerTitle
{
   color:White;
}

#dashboardMenu
{
   height: 35px;
   border-bottom: 1px solid black; 
}

.masterpagelite_contentHolder
{
    min-height: 795px !important;
}
</style>   



</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<div id="dashboardMenu">
<div style="float:left; font-size:large">
		    <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()" style="display: none;">Show Settings</div>
		    <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">Hide Settings</div>
Current Dashboard:
<asp:DropDownList ID="dashboardList" OnSelectedIndexChanged="dbList_Index_Changed" AutoPostBack="true" EnableViewState="true" runat="server"></asp:DropDownList>
New Dashboard:
<asp:RequiredFieldValidator id="RequiredFieldValidator1" runat="server" ErrorMessage="___" ControlToValidate="addDashboardName" ValidationGroup="addDashboard"></asp:RequiredFieldValidator>
<asp:TextBox ID="addDashboardName" runat="server"></asp:TextBox>
<asp:Button ID="addNewDashboard" runat="server" ValidationGroup="addDashboard" 
        Text="Create" onclick="addNewDashboard_Click"/>
<asp:RequiredFieldValidator id="addDBNameRFV" runat="server" ControlToValidate="addDashboardName" ValidationGroup="addDashboard"></asp:RequiredFieldValidator>

Select Widget:
<asp:DropDownList ID="widgetsList" runat="server" ></asp:DropDownList>
<asp:Button ID="addWidgetButton" runat="server" Text="Add" onclick="addWidgetButton_Click"/>
</div>

</div>
<div class="overlayedFilterBox" id="overlayedClearFilterBox" style="display: block;">
<fieldset>
Change Dashboard Name:
<asp:TextBox ID="updateName" runat="server"></asp:TextBox>
<br />
<asp:Label ID="lblHomeDashboard" runat="server" Text="Make home dashboard"></asp:Label>
<asp:CheckBox ID="setHome" runat="server" />
<br />
<asp:Button ID="btnUpdateSettings" runat="server" Text="Update" 
        ValidationGroup="updateDBName" onclick="btnUpdateSettings_Click" />
        
<asp:RequiredFieldValidator ID="updateNameFV" runat="server" ErrorMessage="Enter a name!" ControlToValidate="updateName" ValidationGroup="updateDBName"></asp:RequiredFieldValidator>
</fieldset>
<br />
<asp:Button ID="deleteDB" runat="server" Text="Delete Dashboard" 
        onclick="deleteDB_Click" />
</div>
<div id="content">
<div id="wrap">

<asp:PlaceHolder ID="widgitPlaceHolder" runat="server" />

</div>
</div>
<asp:PlaceHolder ID="javascriptPlaceHolder" runat="server" />

        <script type='text/javascript'>

            $(function () {

                $(".dragbox").draggable({
                    handle: ".header",
                    grid: [10, 10],
                    stop: function (event, ui) {
                        console.log(ui);
                        var id = ui.helper.context.id.split('_')[1];
                        $.ajax({
                            type: "POST",
                            url: "UpdateWidgetOnDashboard.svc/updateWidgetPosition",
                            contentType: "application/json; charset=utf-8",
                            data: JSON.stringify({ left: ui.position.left, top: ui.position.top, widgetOnDashboardID: id }),
                            dataType: "json",
                            success: function (msg) {
                                console.log(msg);
                            }
                        });
                    }
                });
                $(".dragbox").resizable({
                    grid: [10, 10],
                    stop: function (event, ui) {
                        console.log(ui);
                        var id = ui.helper.context.id.split('_')[1];
                        $.ajax({
                            type: "POST",
                            url: "UpdateWidgetOnDashboard.svc/updateWidgetSize",
                            contentType: "application/json; charset=utf-8",
                            data: JSON.stringify({ width: ui.size.width, height: ui.size.height, widgetOnDashboardID: id }),
                            dataType: "json",
                            success: function (msg) {
                                $('#view' + id).height((ui.size.height - 30) + 'px')
                                $('#view' + id).width((ui.size.width - 10) + 'px')
                                $('#view' + id).html($('#code' + id).val())
                                console.log(msg);
                            }
                        });
 
                    }
                });
            });

            FilterOptionsDisplayHide()
    </script>
</asp:Content>
