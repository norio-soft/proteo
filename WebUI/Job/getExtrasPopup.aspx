<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Job.GetExtrasPopup" Codebehind="getExtrasPopup.aspx.cs" %>
<%@ OutputCache Duration="1800" VaryByParam="JobId" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head id="hdr" runat="server">
        <title>Orchestrator</title>
        <base target="_self" />
    </head>
    <body>
        <asp:Literal ID="litExtraPopup" runat="server" />
    </body>
</html>