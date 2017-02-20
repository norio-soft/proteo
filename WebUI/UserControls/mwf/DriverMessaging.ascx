<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DriverMessaging.ascx.cs" Inherits="Orchestrator.WebUI.UserControls.mwf.DriverMessaging" %>

<asp:ScriptManagerProxy runat="server" ID="scriptManagerProxy">
    <Scripts>
        <asp:ScriptReference Path="~/UserControls/mwf/DriverMessaging.ascx.js" />
    </Scripts>
    <Services>
        <asp:ServiceReference Path="~/Services/MwfDriverMessaging.svc" />
    </Services>
</asp:ScriptManagerProxy>

<div id="SendMessageDialog" style="display: none; background-color: White;" title="MWF Driver Messaging">
    <table>
        <tr>
            <td class="formCellLabel">
                Run ID
            </td>
            <td class="formCellField">
                <span id="spnDriverMessagingRunID">- none -</span>
            </td>
        </tr>
        <tr>
            <td class="formCellLabel">
                Date/Time
            </td>
            <td class="formCellField">
                <telerik:RadDateTimePicker runat="server" ID="dteDriverMessagingDateTime" ValidationGroup="driverMessaging" />
                <span class="required">*</span>
                <asp:RequiredFieldValidator ID="rfvDriverMessagingDateTime" runat="server" ControlToValidate="dteDriverMessagingDateTime" ErrorMessage="required"
                    Display="Dynamic" ValidationGroup="driverMessaging" />
            </td>
        </tr>
        <tr>
            <td class="formCellLabel">
                Related Point
            </td>
            <td class="formCellField">
                <telerik:RadComboBox ID="cboDriverMessagingPoint" runat="server" Width="300px" DropDownWidth="400px" Height="300px" AutoPostBack="false" ItemRequestTimeout="500"
                    EnableLoadOnDemand="true" MarkFirstMatch="false" ShowMoreResultsBox="false" AllowCustomText="True" HighlightTemplatedItems="true">
                    <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetPoints" />
                </telerik:RadComboBox>
            </td>
        </tr>
    </table>

    <div class="formCellLabel">
        Message
        <span class="required">*</span>
        <asp:RequiredFieldValidator ID="rfvDriverMessagingNotes" runat="server" ControlToValidate="txtDriverMessagingNotes" ErrorMessage="required"
            Display="Dynamic" ValidationGroup="driverMessaging" />
    </div>
    <telerik:RadTextBox runat="server" ID="txtDriverMessagingNotes" Rows="5" TextMode="MultiLine" Width="400" ClientIDMode="Static" />

    <div class="buttonBar" style="margin-top: 12px;">
        <input type="button" id="btnSendMessage" value="Send" class="buttonClass" />
    </div>
</div>
