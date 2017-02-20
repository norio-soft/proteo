<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="FMReports.aspx.cs" Inherits="Orchestrator.WebUI.Reports.FMReports" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">


    <!-- Latest compiled and minified CSS -->
    <link rel="stylesheet" href="//netdna.bootstrapcdn.com/bootstrap/3.1.1/css/bootstrap.min.css">

    <!-- Optional theme -->
    <link rel="stylesheet" href="//netdna.bootstrapcdn.com/bootstrap/3.1.1/css/bootstrap-theme.min.css">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>Driver Behaviour Reporting
    </h1>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <h1 style="font-size: 33px">Driver Behaviour</h1>
        <div class="row">
            <div class="col-md-3">
                <div class="well">
                    <img src="../images/driveroverview_s1.png" />
                    <div class="btn-toolbar" role="toolbar" style="padding-top: 10px;">

                        <button type="button" class="btn btn-primary" onclick="runReport('FleetMetrik.DriverOverview');">Run </button>

                    </div>
                </div>
            </div>
            <div class="col-md-3">
                <div class="well">
                    <img src="../images/drivergrading_s1.png" />
                    <div class="btn-toolbar" role="toolbar" style="padding-top: 10px;">

                        <div class="btn-group">
                            <button type="button" class="btn btn-primary dropdown-toggle " data-toggle="dropdown">Run <span class="caret"></span></button>
                            <ul class="dropdown-menu" role="menu">
                                <li><a href="javascript:runReport('FleetMetrik.DriverGradingByWeek');">By Week</a></li>
                                <li><a href="javascript:runReport('FleetMetrik.DriverGradingByMonth');">By Month</a></li>
                                <li><a href="javascript:runReport('FleetMetrik.DriverLeagueTable');">League Table</a></li>
                                <li><a href="javascript:runReport('CAN.DriverGradingDetail');">Driver Grading Detail</a></li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="row">
                    <div class="col-md-5">
                        <div class="well">
                            <img src="../images/trends_r1_c1_s1.png" />
                            <div class="btn-toolbar" role="toolbar" style="padding-top: 10px;">

                                <div class="btn-group">
                                    <button type="button" class="btn btn-primary dropdown-toggle " data-toggle="dropdown">Run <span class="caret"></span></button>
                                    <ul class="dropdown-menu" role="menu">
                                        <li><a href="javascript:runReport('FleetMetrik.HarshBrakingTrendByWeek');">By Week</a></li>
                                        <li><a href="javascript:runReport('FleetMetrik.HarshBrakingTrendByMonth');">By Month</a></li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-5">
                        <div class="well">
                            <img src="../images/trends_r1_c5_s1.png" />
                            <div class="btn-toolbar" role="toolbar" style="padding-top: 10px;">

                                <div class="btn-group">
                                    <button type="button" class="btn btn-primary dropdown-toggle " data-toggle="dropdown">Run <span class="caret"></span></button>
                                    <ul class="dropdown-menu" role="menu">
                                        <li><a href="javascript:runReport('FleetMetrik.SpeedingTrendByWeek');">By Week</a></li>
                                        <li><a href="javascript:runReport('FleetMetrik.SpeedingTrendByMonth');">By Month</a></li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-5">
                        <div class="well">
                            <img src="../images/trends_r3_c2_s1.png" />
                            <div class="btn-toolbar" role="toolbar" style="padding-top: 10px;">

                                <div class="btn-group">
                                    <button type="button" class="btn btn-primary dropdown-toggle " data-toggle="dropdown">Run <span class="caret"></span></button>
                                    <ul class="dropdown-menu" role="menu">
                                        <li><a href="javascript:runReport('FleetMetrik.IdlingTrendByWeek');">By Week</a></li>
                                        <li><a href="javascript:runReport('FleetMetrik.IdlingTrendByMonth');">By Month</a></li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-5">
                        <div class="well">
                            <img src="../images/trends_r3_c5_s1.png" />
                            <div class="btn-toolbar" role="toolbar" style="padding-top: 10px;">

                                <div class="btn-group">
                                    <button type="button" class="btn btn-primary dropdown-toggle " data-toggle="dropdown">Run <span class="caret"></span></button>
                                    <ul class="dropdown-menu" role="menu">
                                        <li><a href="javascript:runReport('FleetMetrik.OverRevvingTrendByWeek');">By Week</a></li>
                                        <li><a href="javascript:runReport('FleetMetrik.OverRevvingTrendByMonth');">By Month</a></li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- Latest compiled and minified JavaScript -->
    <script src="//netdna.bootstrapcdn.com/bootstrap/3.1.1/js/bootstrap.min.js"></script>
    <script src="http://cdnjs.cloudflare.com/ajax/libs/moment.js/2.6.0/moment.min.js"></script>
    <script type="text/javascript">
        var startDate = moment().subtract('years', 1).startOf('day').format('DD/MM/YY HH:mm:ss');
        var endDate = moment().format('DD/MM/YY HH:mm:ss');
        var overviewStartDate = moment().subtract('months', 1).startOf('month').format('DD/MM/YY HH:mm:ss');
        var overviewEndDate = moment().subtract('months', 1).endOf('month').format('DD/MM/YY HH:mm:ss');

        function runReport(reportName) {
            if (reportName == "FleetMetrik.DriverOverview" || reportName == "FleetMetrik.DriverLeagueTable" || reportName == "CAN.DriverGradingDetail") {
                popup('/Reports/ReportViewer2.aspx?rn=' + reportName + '&StartDate=' + overviewStartDate + '&EndDate=' + overviewEndDate, 1200, 750)
                //} else if (reportName == "FleetMetrik.DriverLeagueTable") {
                //    popup('/Reports/ReportViewer2.aspx?rn=' + reportName + '&FromDate=' + overviewStartDate + '&ToDate=' + overviewEndDate, 1200, 750)
            }
            else {
                popup('/Reports/ReportViewer2.aspx?rn=' + reportName + '&StartDate=' + startDate + '&EndDate=' + endDate, 1200, 750)
            }
        }
    </script>
</asp:Content>
