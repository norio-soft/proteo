<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Gazetteer.ascx.cs" Inherits="Orchestrator.WebUI.NewDashboard.UserControls.Gazetteer" %>
<%@ Import Namespace="Orchestrator.WebUI.Services" %>


<table id="gaz">
<thead>
<tr>
<th>Reg</th><th>Where</th><th>When</th>
</tr>
</thead>
<tbody>
<% 
    foreach (Vehicle currentVehicle in vehicleList)
    {
        Response.Write("<tr><td>" + currentVehicle.RegistrationNo + "</td><td>" + currentVehicle.CurrentLocation + "</td><td>" + currentVehicle.GPSTimeStamp + "</td></tr>");
    }
    %>
</tbody>
</table>

<script type="text/javascript">

    $(document).ready(function() {
    $('#gaz').dataTable( {
        
        "bPaginate": false,
        "bScrollCollapse": true
    } );
} );
</script>