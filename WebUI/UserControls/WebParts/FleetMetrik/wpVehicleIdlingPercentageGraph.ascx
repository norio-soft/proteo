<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="wpVehicleIdlingPercentageGraph.ascx.cs" Inherits="Orchestrator.WebUI.UserControls.WebParts.FleetMetrik.wpVehicleIdlingPercentageGraph" %>

<div id="chartVehicleIdlingPercentage" style="overflow-y: hidden;">
    <p class="no-data-message" style="display:none;">No data to display</p>
    <svg style="height:234px; width: 490px; margin-bottom: 10px; display:none;"></svg>
    <a href="javascript:popup('<%= this.ReportVehicleUrl %>', 1200, 750);">Report</a>
</div>

<script>
    var createVehicleIdlingGraph = function () {
        nv.addGraph(function () {
            var chart = nv.models.multiBarHorizontalChart()
                .x(function (d) { return d.label })
                .y(function (d) { return d.value })
                .margin({ left: 175 })
                .showValues(true)
                .tooltips(false)
                .showControls(false)
                .showLegend(false);

            chart.yAxis
                .tickFormat(d3.format(',.2f'));

            chart.valueFormat((function (d) {
                return d.toFixed(2) + "%";
            }));

            var lowBenchmark = (CANBenchmarkBaseline.IdleTime * 100) / CANBenchmarkBaseline.Duration;
            var highBenchmark = (CANBenchmarkTarget.IdleTime * 100) / CANBenchmarkTarget.Duration;

            var getData = apiService.get('<%= this.DataUrl %>');
            getData.done(function (data) {
                var hasData = data.length > 0;
                $('#chartVehicleIdlingPercentage .no-data-message').toggle(!hasData);
                $('#chartVehicleIdlingPercentage svg').toggle(hasData);

                var chartData = data.map(function (i) {
                    return {
                        label: i.VehicleRegistration,
                        value: i.Percentage
                    }
                })

                var formattedData = [{ values: chartData.reverse() }];

                d3.select('#chartVehicleIdlingPercentage svg')
                    .datum(formattedData)
                    .call(chart);

                d3.selectAll("#chartVehicleIdlingPercentage rect")
                    .style("fill", function (d, i) {
                        return determineBarColor(d.value, highBenchmark, lowBenchmark, false);
                    });
            });


            return chart;
        });
    }
   
</script>