<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DriverIdlingTimeBarChart.ascx.cs" Inherits="Orchestrator.WebUI.NewDashboard.UserControls.DriverIdlingTimeBarChart" %>
<%@ Import Namespace="Orchestrator.WebUI.Services" %>
<%@ Import Namespace="System.Collections" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="Orchestrator.WebUI.NewDashboard" %>

<script type ="text/javascript">
$(function() {

      var chart = new Highcharts.Chart({
            chart: {
                renderTo: 'DriverIdlingTimeBarChart',
                type: 'bar',
                marginRight: 30,
                marginBottom: 70
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
                    return this.point.name;
                }
            },
            plotOptions: {
                bar: {
                    dataLabels: {
                        enabled: true,
                        formatter: function() { 
                        return this.point.name;
                        }
                    }
                }
            },
            legend: {
            width: 250,
            reversed: true,
            floating: true,
            align: 'center',
            x: 35, // = marginLeft - default spacingLeft
            itemWidth: 120
            },
            credits: {
                enabled: false
            },
            series: [{
                showInLegend: true,
                name: "Idling Time",
                zIndex: 2,
                data: [<%
                i=0;
                while(i < 10 && i < idlingTimeList.Count)
                {
                    IdlingTime idlingTime = idlingTimeList[i];
                    Response.Write("{name: '"+WidgetMethods.secondsToTimePeriodString(idlingTime.IdlingSeconds)+"', y:"+WidgetMethods.secondsToHours(idlingTime.IdlingSeconds) + "},");
                    i++;
                }  
                 %>]
            },
            {
            showInLegend: true,
            name: "Running Time",
            zIndex: 1,
            data: [<%
                i=0;
                while(i < 10 && i < idlingTimeList.Count)
                {
                    IdlingTime idlingTime = idlingTimeList[i];
                    Response.Write("{name: '"+WidgetMethods.secondsToTimePeriodString(idlingTime.DurationSeconds)+"', y:"+WidgetMethods.secondsToHours(idlingTime.DurationSeconds) + "},");
                    i++;
                }  
                 %>]
                 
                 }]
        });
    });

    </script>