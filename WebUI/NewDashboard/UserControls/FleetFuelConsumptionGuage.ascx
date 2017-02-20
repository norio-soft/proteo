<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FleetFuelConsumptionGuage.ascx.cs" Inherits="Orchestrator.WebUI.NewDashboard.UserControls.FleetFuelConsumptionGuage" %>
<%@ Import Namespace="Orchestrator.WebUI.Services" %>
<%@ Import Namespace="System.Collections" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="Orchestrator.WebUI.NewDashboard" %>

<script type='text/javascript' src='/NewDashboard/Scripts/guage.js'></script>
    <script type='text/javascript'>
        var gauges = [];
        var viewid = "FleetFuelConsumptionGuage";
        var size = $('#' + viewid).height();
        function createGauge(name, label, sublabel) {
            
            var config =
				{
				    label: label,
                    sublabel: sublabel,
				    minorTicks: 5,
                    max: 20
				}
                config.size = size +20;
            config.redZones = [];
            config.redZones.push({ from: 0, to: 6 });

            config.yellowZones = [];
            config.yellowZones.push({ from: 6, to: 10 });

            config.greenZones = [];
            config.greenZones.push({ from: 10, to: 20 });

            gauges[name] = new Gauge(name, config);
            gauges[name].render();
        }

        function createGauges() {
            createGauge("FleetFuelConsumptionGuage", "MPG", "<%Response.Write(Math.Round(fleetFuelLitres).ToString());%> Litres");
        }

        function updateGauges() {
            for (var key in gauges) {
                gauges[key].redraw(<%Response.Write(fleetFuelConsumption.ToString());%>);
            }
        }

        function initialize() {
            createGauges();
            setInterval(updateGauges, 100000000);
        }

        initialize();
        updateGauges();
    </script>
