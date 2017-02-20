<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="wpVehicleFleetMPGGauge.ascx.cs" Inherits="Orchestrator.WebUI.UserControls.WebParts.FleetMetrik.wpVehicleFleetMPGGauge" %>

<div id="chartVehicleFleetMPG" style="overflow-y: hidden;">
    <p class="no-data-message" style="display: none;">No data to display</p>
    <div id="chartVehicleFleetMPGgauge" style="display: none"></div>
    <a href="javascript:popup('<%= this.ReportUrl %>', 1200, 750);">Report</a>
</div>

<script>

    createFleetGauge = function () {
        var getData = apiService.get('<%= this.DataUrl %>');

        getData.done(function (data) {
            var fuelPriceData = apiService.get('<%= this.FuelUrl %>');
            fuelPriceData.done(function (fuelPrice) {
                var hasData = data;

                if (data.MPG > 20)
                    data.MPG = 20;
                createGauge("chartVehicleFleetMPGgauge", "MPG", 0, 20, data.MPG);
                $('#chartVehicleFleetMPG .no-data-message').toggle(!hasData);
                $('#chartVehicleFleetMPGgauge').toggle(hasData);

                var mpgText = $("text:contains('MPG')")[0];
                var state = 0;
                mpgText.onclick = function () {
                    var c02 = data.FuelUsed * 2.63;
                    var gallons = data.FuelUsed * 0.219969248;
                    if (state == 3)
                        state = 0;
                    else
                        state++;

                    if (mpgText.innerHTML) {
                        switch (state) {
                            case 0:
                                mpgText.innerHTML = "MPG";
                                break;
                            case 1:
                                mpgText.innerHTML = Math.round(c02) + " CO2";
                                break;
                            case 2:
                                mpgText.innerHTML = Math.round(gallons) + " galls";
                                break;
                            case 3:
                                mpgText.innerHTML = "£" + +(data.FuelUsed * (fuelPrice)).toFixed(2);
                                break;
                        }
                    } else if (mpgText.firstChild) { //IE 11 picks up an SVG Text element which does not have innerHTML.
                        switch (state) {
                            case 0:
                                mpgText.firstChild.data = "MPG";
                                break;
                            case 1:
                                mpgText.firstChild.data = Math.round(c02) + " CO2";
                                break;
                            case 2:
                                mpgText.firstChild.data = Math.round(gallons) + " galls";
                                break;
                            case 3:
                                mpgText.firstChild.data = "£" + +(data.FuelUsed * (fuelPrice)).toFixed(2);
                                break;
                        }




                    }
                }

            });

        });
    }
        

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
                config.yellowZones = [{ from: CANBenchmarkBaseline.MPG, to: CANBenchmarkTarget.MPG }];
                config.redZones = [{ from: config.min, to: CANBenchmarkBaseline.MPG }];
                config.greenZones = [{ from: CANBenchmarkTarget.MPG, to: config.max }];

                var gauge = new Gauge(name, config);
                gauge.render();

                gauge.redraw(value);
            }
        
        



</script>
