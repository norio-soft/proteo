<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FleetIdlingGauge.ascx.cs" Inherits="Orchestrator.WebUI.NewDashboard.UserControls.FleetIdlingGauge" %>
<%@ Import Namespace="Orchestrator.WebUI.Services" %>
<%@ Import Namespace="System.Collections" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="Orchestrator.WebUI.NewDashboard" %>

<script type='text/javascript' src='/NewDashboard/Scripts/guage.js'></script>
    <script type='text/javascript'>
        var gauges = [];
        var viewid = "FleetIdlingGauge";
        var size = $('#' + viewid).height();
        function createGauge(name, label) {
            
            var config =
				{
				    label: label,
				    minorTicks: 5,
                    max: 20
				}
                config.size = size +20;
            config.redZones = [];
            config.redZones.push({ from: 15, to: 20 });

            config.yellowZones = [];
            config.yellowZones.push({ from: 10, to: 15 });

            config.greenZones = [];
            config.greenZones.push({ from: 0, to: 10 });

            gauges[name] = new Gauge(name, config);
            gauges[name].render();
        }

        function createGauges() {
            createGauge("FleetIdlingGauge", "% of time");
        }

        function updateGauges() {
            for (var key in gauges) {
                gauges[key].redraw(<%Response.Write(fleetIdlingTime.ToString());%>);
            }
        }

        function initialize() {
            createGauges();
            setInterval(updateGauges, 100000000);
        }

        initialize();
        updateGauges();
    </script>
