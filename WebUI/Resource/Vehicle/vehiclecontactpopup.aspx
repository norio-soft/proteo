<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Resource.Vehicle.VehicleContactPopUp"
    CodeBehind="vehiclecontactpopup.aspx.cs" %>

<%@ OutputCache Duration="3600" VaryByParam="resourceId" %>
<html>
<head runat="server">
</head>
<body>
    <table>
        <tr>
            <td colspan="2" style="border-bottom: solid 1pt black;">
                <b>
                    <%# vwVehicle[0]["RegNo"].ToString()%></b>
            </td>
        </tr>
        <tr>
            <td>
                <b>Cab Number:</b>
            </td>
            <td>
                <b>
                    <%# vwVehicle[0]["TelephoneNumber"].ToString()%></b>
            </td>
        </tr>
    </table>
</body>
</html>
