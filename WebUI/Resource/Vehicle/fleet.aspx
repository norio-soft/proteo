<%@ Page Title="Fleet view" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="Fleet.aspx.cs" Inherits="Orchestrator.WebUI.Resource.Vehicle.Fleet"  %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Fleet View</h1></asp:Content>

<asp:Content ID="Content" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript">

        $(document).ready(function () {
            resizediv();
            window.setTimeout('resizediv()', 1000);
        });

        $(window).resize(function () {
            resizediv();
        });

        function resizediv() {

                var layoutHeaderHeight = $(".masterpagelite_layoutHeader").height();
                var layoutNavHeight = $(".masterpagelite_layoutNav").height();
                var layoutFooterHeight = $(".masterpagelite_layoutFooter").height();

                $('#mapfleetContainer').height($(window).height() - (layoutHeaderHeight) - (layoutNavHeight) - (layoutFooterHeight) - 25);
                $('#mapfleetContainer').width($(window).width() - 2);             
        }

        var _fullSize = false;
        var mapLeft = 0;
        var mapTop = 0;
        var mapWidth = 0;
        var mapHeight = 0;
        var mapPosition = null;


</script>
<style type="text/css">
    .masterpagelite_contentHolder 
    {
        min-height: 0 !important;
        padding: 0 !important;	
    }

    .masterpagelite_contentTop 
    {
        height: 0 !important;
        border: 0 !important;;	
    }

    .masterpagelite_contentBottom 
    {
        height: 0 !important;	
        border: 0 !important;;	
    }
</style>

    <iframe id= "mapfleetContainer" src="../../ng/fleet" 
        style="height:100%;width:100%;border:0"
        allowfullscreen
        webkitallowfullscreen
        mozallowfullscreen>

    </iframe>
</asp:Content>
