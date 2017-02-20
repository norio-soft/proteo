<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrdersCreatedByUser.ascx.cs"
    Inherits="Orchestrator.WebUI.NewDashboard.UserControls.OrdersCreatedByUser" %>
<%@ Import Namespace="Orchestrator.DataAccess" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="Orchestrator.WebUI.WebParts" %>
<script type="text/javascript">

        $(function () {
           var chart = new Highcharts.Chart({
                chart: {
                    renderTo: 'OrdersCreatedByUser',
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: false
                },
                title: {
                    text: null
                },
                tooltip: {
                    formatter: function () {
                        return '<b>' + this.point.name + '</b>: ' + this.percentage + ' %';
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
                foreach(DataRow row in  ordersByUserDS.Tables[0].Rows)
                    {
                       if(Convert.ToInt32(row["Total"]) > 0)
                       {
                            Response.Write("['" + row["CreateUserID"].ToString() + "'," + row["Total"].ToString() + "],\n");
                        
                        }
                    }
                 %>
			]
                }]
            });
        });

</script>
