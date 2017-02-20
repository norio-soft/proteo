<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="wpVehicleMPGGraph.ascx.cs" Inherits="Orchestrator.WebUI.UserControls.WebParts.FleetMetrik.wpVehicleMPGGraph" %>

<div id="chartWorstVehicleMPG" style="overflow-y: hidden;">
    <p class="no-data-message" style="display:none;">No data to display</p>
    <svg style="height:220px; width: 490px; margin-bottom: 10px; display:none;"></svg>
    <a href="javascript:popup('<%= this.ReportVehicleUrl %>', 1200, 750);">Report</a>
</div>

<script>
    var createVehicleMPGGraph = function () {
        //TODO Scale graph http://www.jeromecukier.net/blog/2011/08/11/d3-scales-and-color/
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

            var getData = apiService.get('<%= this.DataUrl %>');
            getData.done(function (data) {
                var hasData = data.length > 0;
                $('#chartWorstVehicleMPG .no-data-message').toggle(!hasData);
                $('#chartWorstVehicleMPG svg').toggle(hasData);

                var chartData = data.map(function (i) {
                    return {
                        label: i.VehicleRegistration,
                        value: i.MPG
                    }
                })

                var formattedData = [{ values: chartData.reverse() }];
                //chart.forceY(formattedData[0].values[0].value - 1);

                d3.select('#chartWorstVehicleMPG svg')
                    .datum(formattedData)
                    .call(chart);

                d3.selectAll("#chartWorstVehicleMPG rect")
                   .style("fill", function (d, i) {
                       return determineBarColor(d.value, CANBenchmarkTarget.MPG, CANBenchmarkBaseline.MPG, true);
                   });
            });


            return chart;
        });
    }
    
</script>