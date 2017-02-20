<%@ Page Title="Vehicle Planning" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="VehiclePlanning.aspx.cs" Inherits="Orchestrator.WebUI.planning.VehiclePlanning" %>
<%@ Register Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">
    <script src="http://code.jquery.com/ui/1.10.2/jquery-ui.js"></script>
    <link rel="stylesheet" type="text/css" href="/style/dhtmlxscheduler.css">
    <link rel="stylesheet" type="text/css" href="/style/dhtmlxscheduler_glossy.css">
    <link rel="stylesheet" type="text/css" href="vehicleplanning.css">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1><%= Page.Title %></h1>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManagerProxy runat="server" ID="scriptManagerProxy">
        <Services>
            <asp:ServiceReference Path="~/Services/VehiclePlanning.svc" />
        </Services>
    </asp:ScriptManagerProxy>

    <div id="container">
        <div class="ui-layout-west">
            <div id="orderstoplan">
                <h3>Filter</h3>
                <div>
                    <div id="filteroptions">
                        <input type="radio" name="filteroption" id="rdoViewAll" value="0" /><label for="rdoViewAll">All</label>
                        <input type="radio" name="filteroption" id="rdoViewImport"  value="1"/><label for="rdoViewImport">Import</label>
                        <input type="radio" name="filteroption" id="rdoViewExport" value="2"/><label for="rdoViewExport">Export</label>
                        <input type="radio" name="filteroption" id="rdoViewUK" value="3"/><label for="rdoViewUK">UK</label>
                    </div>
                </div>
                <h3 id="ordersTitle">Orders</h3>
                <div>
                     <ul id="unplannedOrders" class="column"></ul>
                </div>
            </div>
        </div>

        <div class="ui-layout-center">
            <div id="vehiclePlanningScheduler" class="dhx_cal_container" style="width: 100%; height: 100%;">
                <div class="dhx_cal_navline">
                    <div class="dhx_cal_prev_button">&nbsp;</div>
                    <div class="dhx_cal_next_button">&nbsp;</div>
                    <div class="dhx_cal_today_button"></div>
                    <div class="dhx_cal_date"></div>
                </div>
                <div class="dhx_cal_header">
                </div>
                <div class="dhx_cal_data">
                </div>
            </div>
        </div>
    </div>
    <div id="dialog-modal" title="Planning">
      <button id="showOrder">View Order</button>
      <p style="cursor:pointer">Send To Driver</p>
    </div>
    <cc1:Dialog runat="server" ID="dlgOrder" ClientIDMode="Static" ReturnValueExpected="true" AutoPostBack="true" URL="/groupage/manageorder.aspx" Height="900" Width="1200" ></cc1:Dialog>
    <script src="/script/jquery.layout-latest.min.js"></script>
    <script src="/script/jquery.blockUI-2.64.0.min.js"></script>
    <script src="/script/dhtmlxscheduler.js"></script>
    <script src="/script/dhtmlxscheduler_tooltip.js"></script>
    <script src="/script/dhtmlxscheduler_limit.js"></script>
    <script src="/script/dhtmlxscheduler_timeline.js"></script>
  
    <script src="//cdnjs.cloudflare.com/ajax/libs/moment.js/2.0.0/moment.min.js"></script>
    <script src="/script/handlebars-1.0.rc.2.js"></script>
    <script src="/script/templates.js"></script>
    <script src="vehicleplanning.aspx.js"></script>
</asp:Content>
