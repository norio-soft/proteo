<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/default_tableless.Master" CodeBehind="AddOrder.aspx.cs" Inherits="Orchestrator.WebUI.Groupage.AddOrder" EnableEventValidation="false" Title="Add Orders" %>

<%@ Register TagPrefix="p1" TagName="Order" Src="~/UserControls/order.ascx" %>
<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h2>Add Order</h2></asp:Content>
<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadWindowManager ID="rwmOrder" runat="server" Modal="true" ShowContentDuringLoad="false" ReloadOnShow="true" KeepInScreenBounds="true" VisibleStatusbar="false" EnableViewState="false">
        <Windows>
            <telerik:RadWindow runat="server" ID="largeWindow" Height="600" Width="900" />
            <telerik:RadWindow runat="server" ID="extraWindow" Height="300" Width="400" />
            <telerik:RadWindow runat="server" ID="cancelOrderWindow" Height="300" Width="500" />
        </Windows>
    </telerik:RadWindowManager>
    <script type="text/javascript">
        var initiated = false;

        $(window).resize(function() {
            if (initiated) {
                var docwidth = $(window).width();
                var docheight = $(window).height();

                setCookie("OrderDetailWidth", docwidth, 365);
                setCookie("OrderDetailHeight", docheight, 365);
            }
        });

        window.setTimeout("sizeToFit()", 300);

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
            var headerHeight = $(".masterpagelite_layoutHeader").innerHeight();
            var navHeight = $(".masterpagelite_layoutNav").innerHeight();
            var footerHeight = $(".masterpagelite_layoutFooter").innerHeight();

            orderContanier.css({
                /*"height": 0 + docHeight - headerHeight - navHeight - footerHeight - 90,
                "overflow-y": "scroll",*/
                "margin-bottom": "10px"
            });


            moveTo(0, 0);

            var currentdocwidth = $(window).width();
            var currentdocheight = $(window).height();

            if ('<%= Request.QueryString["hm"]%>' == '1') {
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

    <h1 style="display:none;"><asp:Label runat="server" ID="lblHeader"></asp:Label></h1>

    <p1:Order runat="server" id="ucOrder"></p1:Order>
</asp:Content>
