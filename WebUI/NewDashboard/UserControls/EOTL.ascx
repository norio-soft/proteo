<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EOTL.ascx.cs" Inherits="Orchestrator.WebUI.NewDashboard.UserControls.EOTL" %>
<%@ Import Namespace="Orchestrator.DataAccess" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="Orchestrator.WebUI.WebParts" %>

<script type="text/javascript">
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
         foreach (DataRow row in eotlDS.Tables[0].Rows)
            {
                DateTime startdate = (DateTime)row["Period"];
                Response.Write("'"+startdate.Day.ToString()+"',");
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
					this.x +': '+ this.y;
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
            foreach (DataRow row in eotlDS.Tables[0].Rows)
            {
                double earlyValue = row["Early"] == DBNull.Value ? 0 : double.Parse(row["Early"].ToString()) ;
                Response.Write(earlyValue + ",");
            } 
            %>]
		}, {
			name: 'On Time',
			data: [<%foreach (DataRow row in eotlDS.Tables[0].Rows)
            {
                double onTimeValue = row["OnTime"] == DBNull.Value ? 0 : double.Parse(row["OnTime"].ToString()) ;
                Response.Write(onTimeValue + ",");
            } 
            %>]
		}, {
			name: 'Early And On Time',
			data: [<%foreach (DataRow row in eotlDS.Tables[0].Rows)
            {
                double earlyOnTimeValue = row["EarlyOnTime"] == DBNull.Value ? 0 : double.Parse(row["EarlyOnTime"].ToString()) ;
                Response.Write(earlyOnTimeValue + ",");
            } 
            %>]
		}, {
			name: 'Late',
			data: [<%foreach (DataRow row in eotlDS.Tables[0].Rows)
            {
                double lateValue = row["Late"] == DBNull.Value ? 0 : double.Parse(row["Late"].ToString()) ;
                Response.Write(lateValue + ",");
            } 
            %>]
		}]
	});
});
</script>