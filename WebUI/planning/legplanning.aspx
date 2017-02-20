<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="legplanning.aspx.cs" Inherits="Orchestrator.WebUI.planning.legplanning"  %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Resource Schedules</title>
    <script src="//cdnjs.cloudflare.com/ajax/libs/jquery/2.0.0/jquery.min.js"></script>
    <script src="//cdnjs.cloudflare.com/ajax/libs/jqueryui/1.10.3/jquery-ui.min.js"></script>
    <link rel="stylesheet" type="text/css" href="legplanning.css">
    <link rel="stylesheet" type="text/css" href="/style/dhtmlxscheduler.css">
    <link rel="stylesheet" type="text/css" href="/style/dhtmlxscheduler_glossy.css">
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="scriptManager" runat="server" EnablePageMethods="True" EnablePartialRendering="true">
        </asp:ScriptManager>
        <asp:ScriptManagerProxy runat="server" ID="scriptManagerProxy">
            <Services>
                <asp:ServiceReference Path="~/Services/VehiclePlanning.svc" />
            </Services>
        </asp:ScriptManagerProxy>
            <div class="ui-layout-north">
                <span style="background-image:url('/app_themes/orchestrator/img/MasterPage/he_small.png'); height:25px; background-position: left center; background-repeat: no-repeat; width:139px" ></span>
                <telerik:RadMenu runat="server" EnableOverlay="false" GroupSettings-OffsetX="0" GroupSettings-OffsetY="0"
                    AppendDataBoundItems="True" ID="RadMenu1" CausesValidation="false" >
                </telerik:RadMenu>
            </div>
            <div class="ui-layout-center">
                <div id="vehiclePlanningScheduler" class="dhx_cal_container" style="width:100%; height:100%;" >
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
            <div class="ui-layout-south">
                <p style="color:white; float:right; font-family:Verdana; font-size:0.6em;margin-right:12px;" >Copyright © Proteo 2013 (
                    <%=Orchestrator.WebUI.Utilities.GetVersionNumber(this.Page.Cache) %>)
                </p>
            </div>
        
        <script src="//cdnjs.cloudflare.com/ajax/libs/jquery-layout/1.3.0-rc-30.79/jquery.layout.min.js"></script>
        <script src="/script/jquery.blockUI-2.64.0.min.js"></script>
        <script src="/script/dhtmlxscheduler.js"></script>
        <script src="/script/dhtmlxscheduler_tooltip.js"></script>
        <script src="/script/dhtmlxscheduler_limit.js"></script>        
        <script src="/script/dhtmlxscheduler_timeline.js"></script>
        <script src="/script/dhtmlxscheduler_expand.js"></script>
        <script src="//cdnjs.cloudflare.com/ajax/libs/moment.js/2.0.0/moment.min.js"></script>
        <script src="//cdnjs.cloudflare.com/ajax/libs/handlebars.js/1.0.0-rc.3/handlebars.min.js"></script>
        <script src="/script/templates.js"></script>
        <script src="legplanning.aspx.js"></script>
    </form>
</body>
</html>
