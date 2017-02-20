<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Groupage.ManageOrder" MasterPageFile="~/WizardMasterPage.Master" Codebehind="ManageOrder.aspx.cs"     %>

<%@ Register TagPrefix="p1" TagName="Order" Src="~/UserControls/order.ascx" %>
<%@ MasterType TypeName="Orchestrator.WebUI.WizardMasterPage" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadWindowManager ID="rwmOrder" runat="server" Modal="true" ShowContentDuringLoad="false" ReloadOnShow="true" KeepInScreenBounds="true" VisibleStatusbar="false">
        <Windows>
            <telerik:RadWindow runat="server" ID="largeWindow" Height="600" Width="900" />
            <telerik:RadWindow runat="server" ID="extraWindow" Height="300" Width="400" />
            <telerik:RadWindow runat="server" ID="cancelOrderWindow" Height="300" Width="500" />
        </Windows>
    </telerik:RadWindowManager>
        <script type="text/javascript">
            var initiated = false;
            moveTo(0, 0);

            // TF: Do not resize if in an iframe  (ie. the shuffler) as it doesn't have scroll
            // bars and will prevent the user from seeing the buttons at the buttom of the screen.
            var withinIFrame = !(window.top == window);
            
            if (!withinIFrame) {

                $(window).resize(function() {
                    if (initiated) {
                        var docwidth = $(window).width();
                        var docheight = $(window).height();

                        setCookie("OrderDetailWidth", docwidth, 365);
                        setCookie("OrderDetailHeight", docheight, 365);
                    }
                });

                window.setTimeout("sizeToFit()", 300);

            }
            
            $(document).ready(function() {
                if ($(window).width() > 1180) {
                    sizeToFit();
                }
            });

            function pageLoad() {
                sizeToFit();

            }

            function sizeToFit() {
                if (initiated)
                    return;

                var orderContanier = $("#divOrderContanier");
                var docHeight = $(window).height();
                var containerHeight = $(".masterpagelite_layoutContainer").innerHeight();
                var headerHeight = $(".masterpagepopup_layoutHeaderOuter").innerHeight();

                orderContanier.css({
                    /*"height": 0 + docHeight - headerHeight - 87,
                    "overflow-y": "scroll",*/
                    "margin-bottom": "10px"
                });

                if (!withinIFrame) {

                    var currentdocwidth = $(window).width();
                    var currentdocheight = $(window).height();

                    if (getCookie("OrderDetailWidth") != null) {
                        docwidth = getCookie("OrderDetailWidth");
                        docheight = getCookie("OrderDetailHeight");
                        resizeBy((docwidth - currentdocwidth), (docheight - currentdocheight));

                        setCookie("OrderDetailWidth", docwidth, 365);
                        setCookie("OrderDetailHeight", docheight, 365);
                    }
                }
                
                initiated = true;
            }

            //Sys.Application.add_load(sizeToFit);

    </script>
    
    <p1:Order runat="server" id="ucOrder"></p1:Order>

</asp:Content>
