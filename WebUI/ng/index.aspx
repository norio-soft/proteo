<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="Orchestrator.WebUI.ng.index" EnableViewState="false" Theme="" %>

<%@ Import Namespace="System.Web.Optimization" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" ng-app="peApp">
<head>
    <meta name="viewport" content="initial-scale=1,user-scalable=no,maximum-scale=1" />
    <title ng-bind="title"></title>
    <script src="http://js.api.here.com/v3/3.0/mapsjs-core.js"></script>
    <script src="http://js.api.here.com/v3/3.0/mapsjs-service.js"></script>
    <script src="http://js.api.here.com/v3/3.0/mapsjs-mapevents.js"></script>
    <script src="http://js.api.here.com/v3/3.0/mapsjs-ui.js"></script>
    <link href='http://fonts.googleapis.com/css?family=Roboto:400,500' rel='stylesheet' type='text/css'>
    <link rel="stylesheet" type="text/css" href="http://js.api.here.com/v3/3.0/mapsjs-ui.css"/>
    <%: Styles.Render("~/bundles/styles/lib") %>
    <%: Styles.Render("~/bundles/styles/app") %>
    <base href="/ng/" />
</head>
<body>
    <!--[if lt IE 10]><script type="text/javascript">location.href = '/ng/unsupportedbrowser.aspx';</script><![endif]-->
    <ui-view></ui-view>

    <form runat="server">
        <%: Scripts.Render("~/bundles/jquery") %>
        <%: Scripts.Render("~/bundles/angular") %>
        <%: Scripts.Render("~/bundles/lib") %>
        <%: Scripts.Render("~/bundles/app") %>
        <asp:Literal runat="server" ID="BaseUrlScriptTag"></asp:Literal>
        <%: Scripts.Render("~/bundles/app-config-run") %>
    </form>
</body>
</html>
