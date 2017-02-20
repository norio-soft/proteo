<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="resource.ascx.cs" Inherits="Orchestrator.WebUI.UserControls.resource" %>

<%@ Register TagPrefix="uc" TagName="DriverTime" Src="~/UserControls/DriverTime.ascx" %>

<table width="100%">
    <tr>
        <td class="formCellLabel">Driver</td>
        <td class="formCellInput">
            <telerik:RadComboBox ID="cboDriver" runat="server" EnableLoadOnDemand="true" MarkFirstMatch="false" OnClientItemsRequesting="cboDriver_OnClientItemsRequesting" OnClientSelectedIndexChanged="cboDriver_OnClientSelectedIndexChanged" >
                <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetDrivers" />
            </telerik:RadComboBox>
            <img id="imgDriverTime" class="clockIcon" src="/images/icon-clock.png" title="Show driver time" alt="Driver time" width="16" height="16" style="cursor: pointer; display: none;" />
        </td>
    </tr>
    <tr>
        <td class="formCellLabel">Vehicle</td>
        <td class="formCellInput">
            <telerik:RadComboBox ID="cboVehicle" runat="server" EnableLoadOnDemand="true" MarkFirstMatch="false" OnClientItemsRequesting="cboVehicle_OnClientItemsRequesting">
                <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetVehicles" />
            </telerik:RadComboBox>
        </td>
    </tr>
    <tr>
        <td class="formCellLabel">Trailer</td>
        <td class="formCellInput">
            <telerik:RadComboBox ID="cboTrailer" runat="server" EnableLoadOnDemand="true" OnClientItemsRequesting="cboTrailer_OnClientItemsRequesting" >
                <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetAllTrailers" /></telerik:RadComboBox>
        </td>
    </tr>
    <tr runat="server" id="trTrailerType">
        <td class="formCellLabel">Planning Category</td>
        <td class="formCellInput">
            <telerik:RadComboBox ID="cboPlanningCategory"  runat="server" DataTextField="DisplayShort" DataValueField="ID" ></telerik:RadComboBox>
        </td>
    </tr>
</table>

<uc:DriverTime runat="server" ID="driverTime" />

<script type="text/javascript">
     // resource script handling
        
        var depotID = <%=DepotID %>;
        var cboVehicle = null;
        var cboTrailer = null; 
   
        function cboDriver_OnClientItemsRequesting(sender, eventArgs)
        {
            
            var context = eventArgs.get_context();
            context["FilterString"] = depotID;
        }
        
        function cboVehicle_OnClientItemsRequesting(sender, eventArgs)
        {
            var context = eventArgs.get_context();
            context["FilterString"] = depotID;
        }
        
        function cboTrailer_OnClientItemsRequesting(sender, eventArgs)
        {
            var context = eventArgs.get_context();
            context["FilterString"] = depotID;
        }
        
        
        function cboDriver_OnClientSelectedIndexChanged(sender, eventArgs)
        {
            // if the driver has a usual vehicle set this on the vehicle dropdown;
            var item = eventArgs.get_item();

            if (item.get_attributes().getAttribute("usualVehicleID") != "0")
            {
                if (cboVehicle == null)
                    cboVehicle =  $find("<%=cboVehicle.ClientID %>");
                
                cboVehicle.set_text(item.get_attributes().getAttribute("usualVehicle"));
                cboVehicle.set_value(item.get_attributes().getAttribute("usualVehicleID"));
                
                if (cboTrailer == null)
                    cboTrailer = $find("<%=cboTrailer.ClientID %>");

                var input = cboTrailer.get_inputDomElement();    
                input.focus();
            }
            else
            {
                 if (cboVehicle == null)
                    cboVehicle =  $find("<%=cboVehicle.ClientID %>");
                    
                 var input = cboVehicle.get_inputDomElement();    
                input.focus();
            }
        }
        
        function cboTrailer_OnClientSelectedIndexChanged(sender, eventArgs)
        {
        
            var cbo = $find("<%=cboPlanningCategory.ClientID %>");
            if (sender.get_value() != "" && sender.get_value() != "0")
                cbo.disable();
            else
                cbo.enable();
                
        }
        
        function cboTrailerType_OnClientSelectedIndexChanged(sender, eventArgs)
        {
            var cbo = $find("<%=cboTrailer.ClientID %>");
            if (cbo.get_value() != "0")
                cbo.disable();
            else
                cbo.enable();
        }

    $(function() {
        if (Boolean.parse('<%= this.IsDriverTimeEnabled %>')) {
            var imgDriverTime = $('#imgDriverTime');
            imgDriverTime.show();

            imgDriverTime.on('click', function() {
                var cboDriver = $find('<%= cboDriver.ClientID %>');
                var driverID = cboDriver.get_value();
                new DriverTime(driverID).show();
            });
        }
    });
</script>

