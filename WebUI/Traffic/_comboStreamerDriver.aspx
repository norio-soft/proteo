<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Traffic.drivercomboStreamer" Codebehind="~/Traffic/_comboStreamerDriver.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server"></head>
<body>
    <form id="form1" runat="server">
        <telerik:RadComboBox ID="cboDriver" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
            MarkFirstMatch="true" ShowMoreResultsBox="true" Width="125px" ExternalCallBackPage="" >
        </telerik:RadComboBox>
    </form>
</body>
</html>
