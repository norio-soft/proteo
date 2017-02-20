<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorstDriverBraking.ascx.cs" Inherits="Orchestrator.WebUI.NewDashboard.UserControls.WorstDriverBraking" %>
<%@ Import Namespace="Orchestrator.WebUI.Services" %>
<%@ Import Namespace="System.Collections" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="Orchestrator.WebUI.NewDashboard" %>


<script type ="text/javascript">
$(function() {

      var chart = new Highcharts.Chart({
            chart: {
                renderTo: 'WorstDriverBraking',
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
                while(i < 10 && i < infringementList.Count)
                {
                    InfringementCount infringementCount = infringementList[i];
                    Response.Write("'" + infringementCount.Name + "',");
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
                },
                series: {
                cursor: 'pointer',
                point: {
                    events: {
                        click: function() {
                           window.open(this.options.url,'width=600', 'height=400');
                        }
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
                name: 'Infringements',
                 borderColor: '#303030',
                data: [<%
                i=0;
                while(i < 10 && i < infringementList.Count)
                {
                    InfringementCount infringementCount = infringementList[i];
                    string url =""; /*= "http://localhost:21000/reports/reportviewer2.aspx?rn=CAN.DriverOverview&DriverId=" + infringementCount.Id + "&FromDate=" + timePeriod.StartDate.Date.Year + "-" + timePeriod.StartDate.Date.Month + "-" + timePeriod.StartDate.Date.Day + "&ToDate=" + timePeriod.EndDate.Date.Year + "-" + timePeriod.EndDate.Date.Month + "-" + timePeriod.EndDate.Date.Day + "&OrgUnitId=3"; */
                    if(infringementCount.Count >= 10)
                    {
                        Response.Write("{ y:"+infringementCount.Count + ", color: '#A01414', url:'" +url+ "'},");
                    }
                    else
                    {
                        Response.Write("{ y:"+infringementCount.Count + ", color: '#FEB64C', url:'" +url+ "'},");
                    }
                    
                    i++;
                }  
                 %>]
            }]
        });
    });

    </script>