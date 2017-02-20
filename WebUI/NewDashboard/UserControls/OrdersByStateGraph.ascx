<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrdersByStateGraph.ascx.cs"
    Inherits="Orchestrator.WebUI.NewDashboard.UserControls.OrdersByState" %>
<%@ Import Namespace="Orchestrator.DataAccess" %>
<%@ Import Namespace="System.Data" %>

<script type="text/javascript">
 $(function () {
            var chart = new Highcharts.Chart({
                chart: {
                    renderTo: 'OrdersByStateGraph',
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: false
                },
                title: {
                    text: ''
                },
                tooltip: {
                    formatter: function () {
                        return '<b>' + this.point.name + '</b>: ' + Math.round(this.percentage) + ' %';
                    }
                },
                plotOptions: {
                    pie: {
                        allowPointSelect: true,
                        cursor: 'pointer',
                        dataLabels: {
                            enabled: true,
                            formatter: function() {
						return this.point.y;
                        }
                        },
                        showInLegend: true
                    }
                },
                series: [{
                    type: 'pie',
                    name: 'Browser share',
                    data: [
                    <%
			foreach (DataRow row in ordersByStateDS.Tables[0].Rows)
			{
				if(Convert.ToInt32(row["Total"]) > 0)
				{
					Response.Write("['" + row["Description"].ToString() +"'," +row["Total"].ToString() + "],\n");
				}
			}
			%>
                  
			]
                }]
            });
        });

</script>
