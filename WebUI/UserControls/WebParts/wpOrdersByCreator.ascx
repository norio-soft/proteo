<%@ Control Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.WebParts.wpOrdersByCreator"  Codebehind="wpOrdersByCreator.ascx.cs" %>

<!--[if lt IE 10]><p style="color: darkgray;">Dashboard chart widgets are not supported on Internet Explorer 9.  Please upgrade to a more recent version of Internet Explorer or use Google Chrome.</p><![endif]-->
<div id="chartOrdersByCreator" style="overflow-y: hidden;">
    <p class="no-data-message" style="display: none;">No data to display</p>
    <svg style="height: 220px; width: 490px; margin-bottom: 10px; display: none;"></svg>
</div>

<script>
    nv.addGraph(function () {
        var chart = nv.models.pieChart()
            .x(function (d) { return d.label })
            .y(function (d) { return d.value })
            .margin({ left: 0 })
            .showLabels(false)
            .valueFormat(d3.format(',d'))
            .legendPosition('right');

        chart.legend
            .align(false)
            .rightAlign(true)
            .margin({top: 0, right: 0, bottom: 0, left: 0});

        var getData = apiService.get('<%= this.DataUrl %>');

        getData.done(function (data) {
            var grandTotal = data.reduce(function (current, item) { return current + item.total; }, 0);

            chart.tooltip.valueFormatter(function (d) {
                var percent = d / grandTotal;
                return d + ' (' + d3.format(',.1%')(percent) + ')';
            });

            var hasData = data.length > 0;
            $('#chartOrdersByCreator .no-data-message').toggle(!hasData);
            $('#chartOrdersByCreator svg').toggle(hasData);

            var chartData = data.map(function (i) {
                return {
                    label: i.creatorName,
                    value: i.total,
                };
            });

            d3.select('#chartOrdersByCreator svg').datum(chartData).call(chart);
        });

        return chart;
    });

</script>