<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GetCurrentLocation.aspx.cs"
    Inherits="Orchestrator.WebUI.GPS.GetCurrentLocation" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <link rel="stylesheet" type="text/css" href="../style/styles.css" />
    <title>Show the resource location</title>
    <style type="text/css">
            html
            {
                height:100%;
            }
           body
           {
           	margin:0;
           	padding:0;
           	overflow:hidden;
           }
        #routeButtons
{
	filter: alpha(opacity=90);
	background-color: #235087;
	z-index: 80;
	color: #FFF;
	padding: 10px;
	color: #FFF;
	text-decoration: none;
	font-family: Verdana;
	font-size: 10px;		
}

    </style>


</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager runat="server" ID="scriptmanager" EnablePageMethods="true" />
    <div id="myMap" style="width: 630px; height: 550px; ">   </div>
    <div id="routeButtons" style="position:absolute;  top:562px; width:100%;   left:0px; height:22px;  text-align:center;">
        <input type="button" id="btnClose" onclick="window.close();" value="Close" style="width: 75px;" class="buttonClass" />
    </div>

    <!--Here Maps Scripts-->
    <script type="text/javascript" src="<%=HereMapsCoreJS%>"></script>
    <script type="text/javascript" src="<%=HereMapsServiceJS%>"></script>
    <script type="text/javascript" src="<%=HereMapsEventsJS%>"></script>
    <script type="text/javascript" src="<%=HereMapsUIJS%>"></script>
    <link rel="stylesheet" type="text/css" href="<%=HereMapsUICSS%>" />
    <!-- End Here Maps Scripts -->

    <script type="text/javascript">
        /// <reference path="MicrosoftAjax.js" />

        var platform = new H.service.Platform({
            app_id: '<%=HereMapsApplicationId%>',
            app_code: '<%=HereMapsApplicationCode%>'
        });

        var map = null;
        var unitID = '<%=Request.QueryString["uid"] %>';
        var pointId = '<%=Request.QueryString["pointId"] %>';
        function LoadMap() {
            
            // Fire off th loading the InfoBox Data
            var lat = <%= (Request.QueryString["lat"] == null) ? "0" : Request.QueryString["lat"] %>;
            var lng = <%= (Request.QueryString["lng"] == null) ? "0" : Request.QueryString["lng"] %>;

            var maptypes = platform.createDefaultLayers();
            var mapContainer = document.getElementById('myMap');
            var startLatLong = new H.geo.Point(lat, lng);
            map = new H.Map(mapContainer, maptypes.normal.map, {
                center: startLatLong,
                zoom: 4
            });

            var behaviour = new H.mapevents.Behavior(new H.mapevents.MapEvents(map));
            ui = mapsjs.ui.UI.createDefault(map, maptypes);
            
            if(pointId.length > 0)
            {
                PageMethods.GetPointPosition(pointId, InfoBoxDataLoaded, InfoBoxDataLoadError);
            }
            else if (unitID.length > 0)
            {
                PageMethods.GetGPSPosition(unitID, InfoBoxDataLoaded, InfoBoxDataLoadError);
            }
            else
            {
                ShowPointOnMap({ lat, lng });        
            }


        }
        var circle = null;
        
        
        function ShowPointOnMap(point)
        {
            var marker = new H.map.Marker(point);
            map.addObject(marker);
            map.setCenter(point);
            map.setZoom(14);
        }
        
        function InfoBoxDataLoaded(result, context, methodName) {

            if (result == null)
                return;
             var _title = '<%= (Request.QueryString["desc"] == null) ? "" : Request.QueryString["desc"] %>';
             if (_title.length == 0 )
                _title = result.Title;
            
            

            var marker = new H.map.Marker({lat: result.Latitude, lng: result.Longitude});

            var _description = result.Gazetteer;
            var dateTime = result.dateStamp;
            var infoboxOptions = {title:_title ,  description:_description + '\n' + result.Reason + '\n\n' + dateTime}; 

            var infoBubble = new H.ui.InfoBubble({lat: result.Latitude, lng: result.Longitude},{
                content: '<b>'+_title+'</b><br/><br/>' + _description + '\n' + result.Reason + '\n\n' + dateTime
            });

            infoBubble.addClass('hereMapsMiniInfoBubble');

            ui.addBubble(infoBubble);
            map.addObject(marker);


            map.setCenter({lat: result.Latitude, lng: result.Longitude});
            map.setZoom(14);
            
        }
        
        function InfoBoxDataLoadError(error) {
            alert("Error occurred loading Infobox data\n" + error.get_message());
        }


        Sys.Application.add_load(LoadMap);

    </script>

    </form>
</body>
</html>
