<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="unsupportedbrowser.aspx.cs" Inherits="Orchestrator.WebUI.ng.unsupportedbrowser" EnableViewState="false" Theme="" %>


<%@ Import Namespace="System.Web.Optimization" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta name="viewport" content="initial-scale=1,user-scalable=no,maximum-scale=1" />
    <title>Proteo Enterprise</title>
    <%: Styles.Render("~/bundles/styles/lib") %>
    <%: Styles.Render("~/bundles/styles/app") %>
    <base href="/ng/" />
</head>

<body >
    <!--[if gte IE 10 | !IE ]><!--><script type="text/javascript">location.href='/ng/index.aspx';</script><![endif]-->

    <div class="container-fluid infoPage">
        <div class="row">
            <a href="/" class="infoPageLogo"></a>
            <h1 class="infoPageTitle">Unsupported browser</h1>
            <p>You are using an outdated browser which is not supported for use with the Proteo Enterprise Drag & Drop.</p>
            <p>Please install the latest version of <a href="http://www.google.com/intl/en_uk/chrome/browser/">Google&nbsp;Chrome</a> or <a href="http://windows.microsoft.com/en-gb/internet-explorer/download-ie">Microsoft&nbsp;Internet&nbsp;Explorer</a>.</p>
            <p>If you are using Internet Explorer 10 or later in Compatibility Mode please switch off Compatibility Mode for this site and retry.</p>
            <footer class="infoPageFooter">Application Provided and supported by <a href="http://www.proteo.co.uk/">Proteo Ltd</a></footer>
        </div>
    </div>

</body>
</html>
