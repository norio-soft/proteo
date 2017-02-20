<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VehicleFuelConsumptionBarChart.ascx.cs" Inherits="Orchestrator.WebUI.NewDashboard.UserControls.VehicleFuelConsumptionBarChart" %>
<%@ Import Namespace="Orchestrator.WebUI.Services" %>
<%@ Import Namespace="System.Collections" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="Orchestrator.WebUI.NewDashboard" %>

<script type ="text/javascript">
$(function() {

      var chart = new Highcharts.Chart({
            chart: {
                renderTo: 'VehicleFuelConsumptionBarChart',
                type: 'bar'
            },
            title: {
                text: ''
            },
            subtitle: {
                text: ''
            },
            xAxis: {
                categories: [<% 
                int i =0;
                while(i < 10 && i < fuelConsumptionList.Count)
                {
                    FuelConsumption fuelConsumption = fuelConsumptionList[i];
                    Response.Write("'" + fuelConsumption.Name + "',");
                    i++;
                } 
                %> ],
                title: {
                    text: null
                }
            },
            yAxis: {
                min: 0,
                title: {
                    text: '',
                    align: 'high'
                },
                labels: {
                    overflow: 'justify'
                }
            },
            tooltip: {
                formatter: function() {
                    return ''+
                        this.series.name +': '+ this.y;
                }
            },
            plotOptions: {
                bar: {
                    dataLabels: {
                        enabled: true
                    }
                }
            },
            legend: {
                layout: 'vertical',
                align: 'right',
                verticalAlign: 'top',
                x: -100,
                y: 100,
                floating: true,
                borderWidth: 1,
                backgroundColor: '#FFFFFF',
                shadow: true
            },
            credits: {
                enabled: false
            },
            series: [{
                showInLegend: false,
                name: 'MPG',
                data: [<%
                i=0;
                while(i < 10 && i < fuelConsumptionList.Count)
                {
                    FuelConsumption fuelConsumption = fuelConsumptionList[i];
                    Response.Write(Math.Round(fuelConsumption.MPG, 2) + ",");
                    i++;
                }  
                 %>]
            }]
        });
    });

    </script>