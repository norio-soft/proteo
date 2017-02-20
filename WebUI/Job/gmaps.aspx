<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Job.Routes.GMaps" Codebehind="gmaps.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
    <script src="http://maps.google.com/maps?file=api&v=1&key=ABQIAAAAo3vMItHcdXa81R4Q-1QxtRT2yXp_ZAY8_ufC3CFXhHIE1NvwkxRmtCxMB_7OZYNcBAuNkg7gAmUBag" type="text/javascript" ></script> 
    <script language="javascript" src="../script/scripts.js" type="text/javascript"></script>
     <link href="../style/helpTip.css" rel="stylesheet" type="text/css" />
     <script src="../script/helptip.js"></script>
    
</head>
<body>
    <form id="form1" runat="server">
    <div id="map" style="width: 500px; height: 400px"></div>
     <script type="text/javascript" defer="defer">
    //<![CDATA[
    function drawMap()
    {
        var map = new GMap(document.getElementById("map"));
        map.addControl(new GSmallMapControl());
        map.centerAndZoom(new GPoint(0.86531608497639, 52.83557051246490), 4);
    }
    
    //]]>
    </script>
    <table width="100%">
        <tr>
            <td align="right">
                <a href="this is a link" class="helpLink" onmouseover="showHelpTipUrl(event, '~/organisation/getContactDetails.aspx?identityId=1543');" onmouseout="hideHelpTip(this);">link information</a>        
                <span class="helpLink" onmouseover="showHelpTipUrl(event, '~/organisation/getContactDetails.aspx?identityId=1543');" onmouseout="hideHelpTip(this);"><img src="../images/joblink.gif" /></span>        
            </td>
        </tr>
    </table>
    </form>
    
   
</body>

</html>