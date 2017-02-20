<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Schedule_schedule" Codebehind="schedule.aspx.cs" %>
<%@ Register TagName="Schedule" TagPrefix="P1" Src="~/usercontrols/schedule.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Orchestrator - Schedule Resource</title>
    <link rel="stylesheet" type="text/css" href="../style/styles.css" />
</head>
<body>
    <form id="form1" runat="server">
        
        <fieldset>
            <legend>View Resource </legend>
            <table>
                <tr>
                    <td>By Type</td>
                    <td><asp:DropDownList ID="cboResourceType" runat="server">
                        <asp:ListItem Text="Driver" Selected="True" Value="3"></asp:ListItem>
                        <asp:ListItem Text="Vehicle" Value="1"></asp:ListItem>
                        <asp:ListItem Text="Trailer" Value="2"></asp:ListItem>
                    </asp:DropDownList></td>
                </tr>
                <tr>
                    <td>Start Date</td>
                    <td><telerik:RadDateInput id="dteStartDate" runat="server" ToolTip="The start Date to display on the Schedule" dateformat="dd/MM/yy" Width="60px"></telerik:RadDateInput></td>
					<td><asp:RequiredFieldValidator id="rfvFilterStartDate" runat="server" Display="Dynamic" ControlToValidate="dteStartDate" ErrorMessage="Please supply a start date"><img src="../images/Error.gif" height="16" width="16" title="Please supply a start date" /></asp:RequiredFieldValidator></td>
                </tr>
                <tr>
                    <td>End Date</td>
                    <td><telerik:RadDateInput id="dteEndDate" runat="server" ToolTip="The last Date to display on the Schedule" dateformat="dd/MM/yy" Width="60px"></telerik:RadDateInput></td>
					<td><asp:RequiredFieldValidator id="RequiredFieldValidator1" runat="server" Display="Dynamic" ControlToValidate="dteStartDate" ErrorMessage="Please supply a start date"><img src="../images/Error.gif" height="16" width="16" title="Please supply a start date" /></asp:RequiredFieldValidator></td>
                </tr>
            </table>
        </fieldset>
        <div class="buttonbar">
            <asp:Button ID="btnRefresh" runat="server" Text="Get Schedules" />
        </div>
        <P1:Schedule runat="server" ID="ucSchedule" GroupingField="ResourceId" TitleField="ResourceName" StartDateField="StartDateTime" EndDateField="EndDateTime" ToolTipField="Display" />
    </form>
</body>
</html>
