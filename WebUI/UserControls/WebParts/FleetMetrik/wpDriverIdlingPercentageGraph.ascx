<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="wpDriverIdlingPercentageGraph.ascx.cs" Inherits="Orchestrator.WebUI.UserControls.WebParts.FleetMetrik.wpDriverIdlingPercentageGraph" %>

<div id="chartDriverIdlingPercentage" style="overflow-y: hidden;">
    <p class="no-data-message" style="display:none;">No data to display</p>
    <svg style="height:220px; width: 490px; margin-bottom: 10px; display:none;"></svg>
    <a href="javascript:popup('<%= this.ReportDriverUrl %>', 1200, 750);">Report</a>
</div>

<script>
    var createDriverIdlingGraph = function () {
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


            var getData = apiService.get('<%= this.DataUrl %>');
            getData.done(function (data) {
                var hasData = data.length > 0;
                $('#chartDriverIdlingPercentage .no-data-message').toggle(!hasData);
                $('#chartDriverIdlingPercentage svg').toggle(hasData);

                var chartData = data.map(function (i) {
                    return {
                        label: i.DriverName,
                        value: i.Percentage,
                        id: i.IdentityId
                    }
                })

                var formattedData = [{ values: chartData.reverse() }];

                d3.select('#chartDriverIdlingPercentage svg')
                    .datum(formattedData)
                    .call(chart);

                chart.multibar.dispatch.on("elementClick", function (e) {
                    popup('<%= this.ReportUrl %>' + e.data.id, 1200, 750);
                });

                d3.selectAll("#chartDriverIdlingPercentage rect")
                .style("fill", function (d, i) {
                    return determineBarColor(d.value, CANBenchmarkTarget.IdlingPercentage, CANBenchmarkBaseline.IdlingPercentage, false);
                });
                return chart;
            });


            
        });
    }
    
</script>