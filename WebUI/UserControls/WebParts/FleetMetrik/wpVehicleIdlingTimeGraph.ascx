<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="wpVehicleIdlingTimeGraph.ascx.cs" Inherits="Orchestrator.WebUI.UserControls.WebParts.FleetMetrik.wpVehicleIdlingTimeGraph" %>

<div id="chartVehicleIdlingTime" style="overflow-y: hidden;">
    <p class="no-data-message" style="display:none;">No data to display</p>
    <svg style="height:220px; width: 490px; margin-bottom: 10px; display:none;"></svg>
    <a href="javascript:popup('<%= this.ReportVehicleUrl %>', 1200, 750);">Report</a>
</div>

<script>
    nv.addGraph(function () {
        var chart = nv.models.multiBarHorizontalChart()
            .x(function (d) { return d.label })
            .y(function (d) { return d.value })
            .margin({ left: 150 })
            .showValues(true)
            .tooltips(false)
            .showControls(false)
            .showLegend(true);

        chart.yAxis.tickFormat(function (d) {
            return Math.round(d/3600);
        })


        var getData = apiService.get('<%= this.DataUrl %>');
        getData.done(function (data) {
            var hasData = data.length > 0;
            $('#chartVehicleIdlingTime .no-data-message').toggle(!hasData);
            $('#chartVehicleIdlingTime svg').toggle(hasData);

            var chartIdlingTimeData = data.reverse().map(function (i) {
                return {
                    label: i.VehicleRegistration,
                    value: i.IdleTime
                }
            })

            var chartRunningTimeData = data.map(function (i) {
                return {
                    label: i.VehicleRegistration,
                    value: i.Duration
                }
            })

            chart.valueFormat(function (d) {
                return secondsToString(d);
            });

            var chartIdlingFormattedData = { color:"#d62728", key : "Idling Time", values: chartIdlingTimeData };
            var chartRunningFormattedData = { color:"#1f77b4", key: "Running Time", values: chartRunningTimeData };

            var formattedData = [chartRunningFormattedData,chartIdlingFormattedData];

            d3.select('#chartVehicleIdlingTime svg')
                .datum(formattedData)
                .call(chart);
        });


        return chart;
    });

    function secondsToString(seconds) {

        var numdays = Math.floor(seconds / 86400);
        var numhours = Math.floor((seconds % 86400) / 3600);
        var numminutes = Math.floor(((seconds % 86400) % 3600) / 60);
        var numseconds = ((seconds % 86400) % 3600) % 60;

        return numdays + " d " + numhours + " h " + numminutes + " m ";

    }
</script>