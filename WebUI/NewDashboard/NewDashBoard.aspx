<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NewDashBoard.aspx.cs" MasterPageFile="~/default_tableless.Master" Inherits="Orchestrator.WebUI.NewDashboard.NewDashBoard" %>

<%@ Import Namespace="Orchestrator.DataAccess" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Collections" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="Orchestrator.WebUI.WebParts" %>
<%@ Import Namespace="Orchestrator.WebUI.Services" %>

<%@ Register Src="~/usercontrols/webparts/wpUnapprovedOrders.ascx" TagName="UnapprovedOrders" TagPrefix="wp" %>
<%@ Register Src="~/usercontrols/webparts/wpTop10PCVLocations.ascx" TagName="Top10PCVLocations" TagPrefix="wp" %>
<%@ Register Src="~/usercontrols/webparts/wpClientPalletBalance.ascx" TagName="ClientPalletBalance" TagPrefix="wp" %>
<%@ Register Src="~/usercontrols/webparts/wpUnallocatedOrders.ascx" TagName="UnallocatedOrders" TagPrefix="wp" %>
<%@ Register Src="~/usercontrols/webparts/wpTrafficNews.ascx" TagName="TrafficNews" TagPrefix="wp" %>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>
        New Dashboard</h1>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">
    <script src="script/HighCharts/highcharts.js"></script>
    <link rel="stylesheet" type="text/css" href="http://app.jslate.com/css/jquery-ui.css" /><link rel="stylesheet" type="text/css" href="http://app.jslate.com/css/codemirror/codemirror.css" /><link rel="stylesheet" type="text/css" href="http://app.jslate.com/css/codemirror/ambiance.css" /><link rel="stylesheet" type="text/css" href="http://app.jslate.com/css/colorbox.css" /><link rel="stylesheet" type="text/css" href="http://app.jslate.com/css/topnav.css" /><script type="text/javascript" src="http://app.jslate.com/js/jquery.js"></script><script type="text/javascript" src="http://app.jslate.com/js/jquery-ui.js"></script><script type="text/javascript" src="http://app.jslate.com/js/jquery.colorbox-min.js"></script><script type="text/javascript" src="http://app.jslate.com/js/codemirror/codemirror.js"></script><script type="text/javascript" src="http://app.jslate.com/js/codemirror/javascript.js"></script><script type="text/javascript" src="http://app.jslate.com/js/codemirror/xml.js"></script><script type="text/javascript" src="http://app.jslate.com/js/codemirror/css.js"></script><script type="text/javascript" src="http://app.jslate.com/js/codemirror/htmlmixed.js"></script><script type="text/javascript" src="http://app.jslate.com/js/highstock.js"></script><script type="text/javascript" src="http://app.jslate.com/js/gray.js"></script><script type="text/javascript" src="http://app.jslate.com/js/d3/d3.js"></script><script type="text/javascript" src="http://app.jslate.com/js/d3/d3.csv.js"></script><script type="text/javascript" src="http://app.jslate.com/js/d3/d3.chart.js"></script><script type="text/javascript" src="http://app.jslate.com/js/d3/d3.geo.js"></script><script type="text/javascript" src="http://app.jslate.com/js/d3/d3.geom.js"></script><script type="text/javascript" src="http://app.jslate.com/js/d3/d3.layout.js"></script><script type="text/javascript" src="http://app.jslate.com/js/d3/d3.time.js"></script> 

<style type="text/css">
    /** Scaffold View **/
dl {
	line-height: 2em;
	margin: 0em 0em;
	width: 60%;
}
dl .altrow {
	background: #f4f4f4;
}
dt {
	font-weight: bold;
	padding-left: 4px;
	vertical-align: top;
}
dd {
	margin-left: 10em;
	margin-top: -2em;
	vertical-align: top;
}

.indent{
    margin-left: 20px;
}

.dragbox{
    margin:5px 2px  20px;
    
    position:relative;
    border:1px solid #222;
    -moz-border-radius:5px;
    -webkit-border-radius:5px;
}
.dragbox .header{
    margin:0;
    padding-right:5px;
    background:#222;
    border-bottom:1px solid #222;
    font-size: 10px;
    cursor:move;
}
.dragbox h2{
    font-size:12px;
    color:#000;
}
.dragbox-content{
    
    min-height:100px; margin:5px;
    font-family:'Lucida Grande', Verdana; font-size:0.8em; line-height:1.5em;
}
#flashMessage, #authMessage{
    position: fixed;
    float: left;
    top: 0px;
    left: 50%;
    padding: 5px;
    background-color: white;
    text-align: center;
    z-index:10;
    color: black;

    -webkit-border-radius: 0px 0px 10px 10px;
    -moz-border-radius: 0px 0px 10px 10px;
    border-radius: 0px 0px 10px 10px;
}

.headerTitle
{
   color:White;
}
</style>   



</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<asp:Button runat="server" ID="databaseTest" Text="runTest" 
        onclick="databaseTest_Click" />
<div id="content">
    <div id="wrap">
    
    <div class='dragbox' id='dragbox1'

        style=' overflow: hidden; position: absolute; z-index: 2; left: 100px; top: 200px; width: 400px; height: 300px;'>

            <div class='header' style=' text-align:center'>

              <div class='headerTitle'>Orders Created For This Week  <span>&nbsp;<a href="/dbviews/delete/2311" style="float: right; margin-left: 10px;">x</a><a href="/dbviews/edit/2311" style="float: right;">edit</a></span></div></div>

            <div class='dragbox-content' id='view' style='clear: both; width: 390px; height: 270px;'><div id="ordersForWeek" style="width: 100%; height: 100%;"></div></div></div> 



            <div class='dragbox' id='dragbox_2'

        style=' overflow: hidden; position: absolute; z-index: 2; left: 300px; top: 200px; width: 400px; height: 300px;'>

            <div class='header' style='text-align:center'>

                <div class='headerTitle'>Orders By State For Last 30 Days<span>&nbsp;<a href="/dbviews/delete/2311" style="float: right; margin-left: 10px;">x</a><a href="/dbviews/edit/2311" style="float: right;">edit</a></span></div></div>

            <div class='dragbox-content' id='view2' style='clear: both; width: 390px; height: 270px;'><div id="ordersByState" style="width: 100%; height: 100%;"></div></div></div> 



            <div class='dragbox' id='dragbox3'

        style=' overflow: hidden; position: absolute; z-index: 2; left: 300px; top: 200px; width: 400px; height: 300px;'>

            <div class='header' style='text-align:center'>

                <div class='headerTitle'>Early Ontime and Late For Last 30 Days<span>&nbsp;<a href="/dbviews/delete/2311" style="float: right; margin-left: 10px;">x</a><a href="/dbviews/edit/2311" style="float: right;">edit</a></span></div></div>

            <div class='dragbox-content' id='view3' style='clear: both; width: 390px; height: 270px;'><div id="EOTL" style="width: 100%; height: 100%;"></div></div></div> 




            <div class='dragbox' id='dragbox4'

        style=' overflow: hidden; position: absolute; z-index: 2; left: 300px; top: 200px; '>

            <div class='header' style=' text-align:center'>
            <div class='headerTitle'>Unapproved Orders  <span>&nbsp;<a href="/dbviews/delete/2311" style="float: right; margin-left: 10px;">x</a><a href="/dbviews/edit/2311" style="float: right;">edit</a></span></div>
               </div>

            <div class='dragbox-content' id='view4' style='clear: both; '><div id="UnapprovedOrders" style="width: 100%; height: 100%;"><wp:UnapprovedOrders runat="server" id="ucUnapprovedOrders"></wp:UnapprovedOrders></div></div></div>
            
            
            
            
            <div class='dragbox' id='dragbox5'

        style=' overflow: hidden; position: absolute; z-index: 2; left: 300px; top: 200px; '>

            <div class='header' style=' text-align:center'>
            <div class='headerTitle'>Top 10 PCV Locations <span>&nbsp;<a href="/dbviews/delete/2311" style="float: right; margin-left: 10px;">x</a><a href="/dbviews/edit/2311" style="float: right;">edit</a></span></div>
               </div>

            <div class='dragbox-content' id='view5' style='clear: both; '><div id="Div3" style="width: 100%; height: 100%;"><wp:Top10PCVLocations runat="server" ID="ucTop10PCVLocations" /></div></div></div> 
            
            
            
            
            
            <div class='dragbox' id='dragbox6'

        style=' overflow: hidden; position: absolute; z-index: 2; left: 300px; top: 200px; '>

            <div class='header' style=' text-align:center'>
            <div class='headerTitle'>Client Pallet Balance <span>&nbsp;<a href="/dbviews/delete/2311" style="float: right; margin-left: 10px;">x</a><a href="/dbviews/edit/2311" style="float: right;">edit</a></span></div>
               </div>

            <div class='dragbox-content' id='view6' style='clear: both; '><div id="Div4" style="width: 100%; height: 100%;"><wp:ClientPalletBalance runat="server" id="ucClientPalletBalance"></wp:ClientPalletBalance></div></div></div> 






            <div class='dragbox' id='dragbox7'

        style=' overflow: hidden; position: absolute; z-index: 2; left: 300px; top: 200px; '>

            <div class='header' style=' text-align:center'>
            <div class='headerTitle'>Unallocated Orders <span>&nbsp;<a href="/dbviews/delete/2311" style="float: right; margin-left: 10px;">x</a><a href="/dbviews/edit/2311" style="float: right;">edit</a></span></div>
               </div>

            <div class='dragbox-content' id='view7' style='clear: both; '><div id="Div5" style="width: 100%; height: 100%;"><wp:UnallocatedOrders runat="server" id="ucUnallocatedOrders"></wp:UnallocatedOrders></div></div></div> 




            <div class='dragbox' id='dragbox8'

        style=' overflow: hidden; position: absolute; z-index: 2; left: 300px; top: 200px; '>

            <div class='header' style=' text-align:center'>
            <div class='headerTitle'>Traffic News <span>&nbsp;<a href="/dbviews/delete/2311" style="float: right; margin-left: 10px;">x</a><a href="/dbviews/edit/2311" style="float: right;">edit</a></span></div>
               </div>

            <div class='dragbox-content' id='view8' style='clear: both; '><div id="Div6" style="width: 100%; height: 100%;"><wp:TrafficNews runat="server" ID="ucTrafficNews"></wp:TrafficNews></div></div></div> 




            <div class='dragbox' id='dragbox9'

        style=' overflow: hidden; position: absolute; z-index: 2; left: 100px; top: 200px; width: 400px; height: 300px;'>

            <div class='header' style=' text-align:center'>

              <div class='headerTitle'>Worst Driver Braking <span>&nbsp;<a href="/dbviews/delete/2311" style="float: right; margin-left: 10px;">x</a><a href="/dbviews/edit/2311" style="float: right;">edit</a></span></div></div>

            <div class='dragbox-content' id='view9' style='clear: both; width: 390px; height: 270px;'><div id="driverBraking" style="width: 100%; height: 100%;"></div></div></div> 
            
    </div>
</div>


    <script type="text/javascript">

        $(function () {
           var chart = new Highcharts.Chart({
                chart: {
                    renderTo: 'ordersForWeek',
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: false
                },
                title: {
                    text: ''
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
                             KPI myKPI = new KPI();
    
                DateTime startDate;
                DateTime endDate;

                DateTime currentDate = DateTime.Today;

            while (currentDate.DayOfWeek != DayOfWeek.Monday)
            {
                currentDate = currentDate.AddDays(-1);
            }
            startDate = currentDate;
            endDate = currentDate.AddDays(6);

                DataSet orderCreatorsDS = myKPI.GetOrdersByCreator(startDate, endDate);

                foreach(DataRow row in orderCreatorsDS.Tables[0].Rows)
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

        /*---------------------------------------*/

                
        $(function () {
            var chart = new Highcharts.Chart({
                chart: {
                    renderTo: 'ordersByState',
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: false
                },
                title: {
                    text: ''
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
                        wpOrdersByStateGraph myWpOrdersByStateGraph = new wpOrdersByStateGraph();
                        myWpOrdersByStateGraph.SetLast30Days();
                        DataSet ordersByStateDS = myWpOrdersByStateGraph.GetData();
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

        /*************************************/

       
$(function() {
var	chart = new Highcharts.Chart({
		chart: {
			renderTo: 'EOTL',
			type: 'line',
			marginRight: 130,
			marginBottom: 25
		},
		title: {
			text: '',
			x: -20 //center
		},
		xAxis: {
        categories: [
        <%  
         wpEOTL tempwpEOTL = new wpEOTL();
         tempwpEOTL.SetLast30Days();
         DataSet eotlDataSet = tempwpEOTL.GetData();
         
         foreach (DataRow row in eotlDataSet.Tables[0].Rows)
            {
                DateTime startdate = (DateTime)row["Period"];
                Response.Write("'"+startdate+"',");
            }
         %>
			]
		},
		yAxis: {
			title: {
				text: ''
			},
			plotLines: [{
				value: 0,
				width: 1,
				color: '#808080'
			}]
		},
		tooltip: {
			formatter: function() {
					return '<b>'+ this.series.name +'</b><br/>'+
					this.x +': '+ this.y +'Â°C';
			}
		},
		legend: {
			layout: 'vertical',
			align: 'right',
			verticalAlign: 'top',
			x: -10,
			y: 100,
			borderWidth: 0
		},
		series: [{
			name: 'Early',
			data: [<%
            foreach (DataRow row in eotlDataSet.Tables[0].Rows)
            {
                double earlyValue = row["Early"] == DBNull.Value ? 0 : double.Parse(row["Early"].ToString()) ;
                Response.Write(earlyValue + ",");
            } 
            %>]
		}, {
			name: 'On Time',
			data: [<%foreach (DataRow row in eotlDataSet.Tables[0].Rows)
            {
                double onTimeValue = row["OnTime"] == DBNull.Value ? 0 : double.Parse(row["OnTime"].ToString()) ;
                Response.Write(onTimeValue + ",");
            } 
            %>]
		}, {
			name: 'Early And On Time',
			data: [<%foreach (DataRow row in eotlDataSet.Tables[0].Rows)
            {
                double earlyOnTimeValue = row["EarlyOnTime"] == DBNull.Value ? 0 : double.Parse(row["EarlyOnTime"].ToString()) ;
                Response.Write(earlyOnTimeValue + ",");
            } 
            %>]
		}, {
			name: 'Late',
			data: [<%foreach (DataRow row in eotlDataSet.Tables[0].Rows)
            {
                double lateValue = row["Late"] == DBNull.Value ? 0 : double.Parse(row["Late"].ToString()) ;
                Response.Write(lateValue + ",");
            } 
            %>]
		}]
	});
});




/**********************************/




$(function() {

      var chart = new Highcharts.Chart({
            chart: {
                renderTo: 'driverBraking',
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
                 startDate = new DateTime(2012,05,29);
                CanDashboardServices canDasboardServices = new CanDashboardServices();
                List<InfringementCount> infringementList = canDasboardServices.GetDriverBraking(startDate,endDate, null);
                foreach(InfringementCount infringementCount in infringementList)
                {
                    Response.Write("'" + infringementCount.Name + "',");
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
                        this.series.name +': '+ this.y +' millions';
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
                name: 'Offenses',
                data: [<%
                foreach(InfringementCount infringementCount in infringementList)
                {
                    Response.Write(infringementCount.Count + ",");
                }  
                 %>]
            }]
        });
    });
    


    </script>

        <script type='text/javascript'>

            $(function () {

                $(".dragbox").draggable({
                    handle: ".header",
                    grid: [10, 10],
                    stop: function (event, ui) {
                        console.log(ui);
                        var id = ui.helper.context.id.split('_')[1];
                        $.ajax({
                            type: "POST",
                            url: "/dbviews/update/" + id,
                            data: { data: { left: ui.position.left, top: ui.position.top} },
                            success: function (msg) {
                                console.log(msg);
                            }
                        });
                    }
                });
                $(".dragbox").resizable({
                    grid: [10, 10],
                    stop: function (event, ui) {

                        var id = ui.helper.context.id.split('_')[1];
                        $.ajax({
                            type: "POST",
                            url: "/dbviews/update/" + id,
                            data: { data: { width: ui.size.width, height: ui.size.height} },
                            success: function (msg) {
                                $('#view' + id).height((ui.size.height - 30) + 'px')
                                $('#view' + id).width((ui.size.width - 10) + 'px')
                                $('#view' + id).html($('#code' + id).val())
                                console.log(msg);
                            }
                        });
                    }
                });
            });


    </script>
</asp:Content>
