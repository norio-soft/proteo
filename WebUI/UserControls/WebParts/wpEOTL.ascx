<%@ Control Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.WebParts.wpEOTL" CodeBehind="wpEOTL.ascx.cs" %>

<!--[if lt IE 10]><p style="color: darkgray;">Dashboard chart widgets are not supported on Internet Explorer 9.  Please upgrade to a more recent version of Internet Explorer or use Google Chrome.</p><![endif]-->
<div style="position: relative;"> <%-- Relative-positioned wrapper is needed for correct positioning of chart's interactive tooltip.  This may be a bug that is fixed in the future and this div may then no longer be necessary. --%>
    <div id="chartEOTL">
        <svg style="height: 220px; width: 470px;"></svg>
    </div>
</div>

<script>
    nv.addGraph(function() {
        var chart = nv.models.lineChart()
            .margin({ left: 30 })
            .useInteractiveGuideline(true);

        chart.xAxis.tickFormat(function(d) { return d3.time.format('%b %d')(new Date(d)); });
        chart.yAxis.tickFormat(d3.format('.d'));

        var getData = apiService.get('<%= this.DataUrl %>');

        getData.done(function (data) {
            var chartData = data.map(function (i) {
                return {
                    key: i.seriesName,
                    values: i.values.map(function (v) {
                        return {
                            x: new Date(v.date),
                            y: Math.round(v.total * 100) / 100,
                        };
                    }),
                };
            });

            d3.select('#chartEOTL svg').datum(chartData).call(chart);
        });

        return chart;
    });

</script>
