<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VehicleIdlingBarChart.ascx.cs" Inherits="Orchestrator.WebUI.NewDashboard.UserControls.VehicleIdlingBarChart" %>
<%@ Import Namespace="Orchestrator.WebUI.Services" %>
<%@ Import Namespace="System.Collections" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="Orchestrator.WebUI.NewDashboard" %>

<script type ="text/javascript">
$(function() {

      var chart = new Highcharts.Chart({
            chart: {
                renderTo: 'VehicleIdlingBarChart',
                type: 'bar',
                marginRight: 30
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
                while(i < 10 && i < idlingTimeList.Count)
                {
                    IdlingTime idlingTime = idlingTimeList[i];
                    Response.Write("'" + idlingTime.Name + "',");
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
                    return ''+ this.y + '%';
                }
            },
            plotOptions: {
                bar: {
                    dataLabels: {
                        enabled: true,
                        formatter: function() { 
                        return this.y +'%';
                        }
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
                name: "Idling",
                data: [<%
                i=0;
                while(i < 10 && i < idlingTimeList.Count)
                {
                    IdlingTime idlingTime = idlingTimeList[i];
                    Response.Write(Math.Round(idlingTime.Percentage, 2) + ",");
                    i++;
                }  
                 %>]
            }]
        });
    });

    </script>