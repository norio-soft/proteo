﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="wpDriverOverRevvingGraph.ascx.cs" Inherits="Orchestrator.WebUI.UserControls.WebParts.FleetMetrik.wpDriverOverRevvingGraph" %>

<div id="chartDriverOverRevving" style="overflow-y: hidden;">
    <p class="no-data-message" style="display:none;">No data to display</p>
    <svg style="height:234px; width: 490px; margin-bottom: 10px; display:none;"></svg>
</div>

<script>
    var createDriverRevvingGraph = function () {
        nv.addGraph(function () {
            var chart = nv.models.multiBarHorizontalChart()
                .x(function (d) { return d.label })
                .y(function (d) { return d.value })
                .margin({ left: 150 })
                .showValues(true)
                .tooltips(false)
                .showControls(false)
                .showLegend(false);

            chart.yAxis
                .tickFormat(d3.format(',.2f'));

            var getData = apiService.get('<%= this.DataUrl %>');
            getData.done(function (data) {
                var hasData = data.length > 0;
                $('#chartDriverOverRevving .no-data-message').toggle(!hasData);
                $('#chartDriverOverRevving svg').toggle(hasData);

                var chartData = data.map(function (i) {
                    return {
                        label: i.DriverName,
                        value: i.OverRevingCount,
                        id: i.IdentityId
                    }
                })

                var formattedData = [{ values: chartData }];

                d3.select('#chartDriverOverRevving svg')
                    .datum(formattedData)
                    .call(chart);

                chart.multibar.dispatch.on("elementClick", function (e) {
                    popup('<%= this.ReportUrl %>' + e.data.id, 1200, 750);
                });

                d3.selectAll("#chartDriverOverRevving rect")
                    .style("fill", function (d, i) {
                        return determineBarColor(d.value, CANBenchmarkTarget.OverRevCount, CANBenchmarkBaseline.OverRevCount, false);
                    });
            });


            return chart;
        });
    }
    
</script>