<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DriverTime.ascx.cs" Inherits="Orchestrator.WebUI.UserControls.DriverTime" %>

<link rel="stylesheet" href="//da7xgjtj801h2.cloudfront.net/2013.2.626/styles/kendo.common.min.css" />
<link rel="stylesheet" href="//da7xgjtj801h2.cloudfront.net/2013.2.626/styles/kendo.default.min.css" />

<div id="DriverTimeDialog" style="display: none; background-color: White;" title="Driver Time">
    <table>
        <tr>
            <td class="formCellLabel">Date</td>
            <td class="formCellField">
				<telerik:RadDatePicker ID="dteDriverTimeStartDate" runat="server" width ="100" ToolTip="The earliest date to report on." DateInput-DateFormat="dd/MM/yy" />
                <asp:RequiredFieldValidator ID="rfvDriverTimeStartDate" runat="server" ValidationGroup="vgDriverTime" ControlToValidate="dteDriverTimeStartDate" ErrorMessage="Please specify a start date." Display="Dynamic">
                    <img src="../images/Error.gif" height="16" width="16" title="Please specify a start date." />
                </asp:RequiredFieldValidator>
                to
                <telerik:RadDatePicker ID="dteDriverTimeEndDate" runat="server" Width="100" ToolTip="The last date to report on." DateInput-DateFormat="dd/MM/yy" />
                <asp:RequiredFieldValidator ID="rfvDriverTimeEndDate" runat="server" ValidationGroup="vgDriverTime" ControlToValidate="dteDriverTimeEndDate" ErrorMessage="Please specify an end date." Display="Dynamic">
                    <img src="../images/Error.gif" height="16" width="16" title="Please specify an end date." />
                </asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="formCellLabel">
                Driver
            </td>
            <td class="formCellField">
                <telerik:RadComboBox ID="cboDriverTimeDriver" runat="server" EnableLoadOnDemand="false" MarkFirstMatch="true"
                    OnClientItemsRequested="cboDriverTimeDriver_ItemsRequested" OnClientItemsRequesting="cboDriverTimeDriver_ItemsRequesting">
                    <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetAllDrivers" />
                </telerik:RadComboBox>
            </td>
        </tr>
    </table>

    <div class="buttonBar" style="margin: 12px 0 8px;">
        <input type="button" id="btnDriverTimeRefresh" value="Refresh" class="buttonClass" />
        <span id="LatestNonVerifiedDateStamp" style="float: right;"></span>
        <span id="LatestVerifiedDateStamp" style="float: right;"></span>
        <div style="clear: both;"></div>
    </div>

    <div id="driverTimeGrid"></div>

    <asp:HiddenField ID="hidDriverTimeWeekStartDay" runat="server" ClientIDMode="Static" />

    <script src="//da7xgjtj801h2.cloudfront.net/2013.2.626/js/kendo.all.min.js"></script>
    <script src="//da7xgjtj801h2.cloudfront.net/2013.2.626/js/cultures/kendo.culture.en-GB.min.js"></script>
    <script src="//cdnjs.cloudflare.com/ajax/libs/moment.js/2.0.0/moment.min.js"></script>
    <script src="/UserControls/DriverTime.ascx.js"></script>
</div>
