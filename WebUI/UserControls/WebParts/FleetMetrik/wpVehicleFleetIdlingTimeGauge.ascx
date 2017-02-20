<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="wpVehicleFleetIdlingTimeGauge.ascx.cs" Inherits="Orchestrator.WebUI.UserControls.WebParts.FleetMetrik.wpVehicleFleetIdlingTimeGauge" %>

<div id="chartVehicleFleetIdling" style="overflow-y: hidden;">
    <p class="no-data-message" style="display:none;">No data to display</p>
    <div id="chartVehicleFleetIdlinggauge" style="display:none"></div>
    <a href="javascript:popup('<%= this.ReportUrl %>', 1200, 750);">Report</a>
</div>

<script>

    createFleetIdlingTimeGauge = function () {
        var getData = apiService.get('<%= this.DataUrl %>');

        getData.done(function (data) {
            var hasData = data;
            if (data > 20)
                data = 20;
            createGauge("chartVehicleFleetIdlinggauge", "% of time", 0, 20, data);
            $('#chartVehicleFleetIdling .no-data-message').toggle(!hasData);
            $('#chartVehicleFleetIdlinggauge').toggle(hasData);
        });

        function createGauge(name, label, min, max, value) {
            var config =
            {
                size: 255,
                label: label,
                min: undefined != min ? min : 0,
                max: undefined != max ? max : 100,
                minorTicks: 5
            }
            var range = config.max - config.min;
            config.yellowZones = [{ from: CANBenchmarkTarget.IdlingPercentage, to: CANBenchmarkBaseline.IdlingPercentage }];
            config.redZones = [{ from: CANBenchmarkBaseline.IdlingPercentage, to: config.max }];
            config.greenZones = [{ from: config.min, to: CANBenchmarkTarget.IdlingPercentage }];

            var gauge = new Gauge(name, config);
            gauge.render();

            gauge.redraw(value);
        }
    }

    

</script>