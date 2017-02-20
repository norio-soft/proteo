<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="TrackingProfiles.aspx.cs" Inherits="Orchestrator.WebUI.Profiles.TrackingProfiles" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
<h2>Manage Tracking Profiles</h2>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<style type="text/css">
        #targetTextBox
        {
        }
        #targetTextBox textarea
        {
            border: 1px solid #979797;
            font: 12px/1.2em "segoe ui" ,arial,sans-serif;
            cursor: default;
        }
        .example-panel
        {
            background: transparent url(bg.jpg) no-repeat 0 0;
            position: relative;
            width: 748px;
            height: 383px;
        }
        #RadListBox1
        {
            position: absolute;
            top: 143px;
            left: 68px;
        }
        #RadListBox2
        {
            position: absolute;
            top: 143px;
            left: 304px;
        }
        #targetTextBox
        {
            left: 545px;
            position: absolute;
            top: 143px;
        }
    </style>
<telerik:RadAjaxManager runat="server" >
    <AjaxSettings>
        <telerik:AjaxSetting AjaxControlID="mpProfile">
            <UpdatedControls> 
                <telerik:AjaxUpdatedControl ControlID="mpProfile" />
            </UpdatedControls>
        </telerik:AjaxSetting>
    </AjaxSettings>
</telerik:RadAjaxManager>
<telerik:RadMultiPage ID="mpProfile" runat="server" RenderSelectedPageOnly="true">
    <telerik:RadPageView ID="rpvProfiles" runat="server" Selected="true">
        <telerik:RadGrid runat="server" ID="rgProfiles" AutoGenerateColumns="false" AllowSorting="true">
            <MasterTableView DataKeyNames="ProfileID">
                <Columns>
                    <telerik:GridButtonColumn ButtonType="ImageButton" ImageUrl="/app_themes/fleetmetrik/img/masterpage/icon-pencil.png" CommandName="Edit"  />
                    <telerik:GridButtonColumn ButtonType="LinkButton" DataTextField="Title" CommandName="Edit" HeaderText="Title" />
                    <telerik:GridCheckBoxColumn DataField="IsDefault" HeaderText="Is Default" HeaderStyle-Width="80" />
                    <telerik:GridBoundColumn DataField="TimedFrequencyTitle" HeaderText="Active Frequency"  />
                    <telerik:GridBoundColumn DataField="SendFrequencyTitle" HeaderText="Send Frequency"  />
                    <telerik:GridBoundColumn DataField="IdlingFrequencyTitle" HeaderText="Idling Time" />
                    <telerik:GridBoundColumn DataField="DistanceFrequencyKmString" HeaderText="Distance (km)" />
                    <telerik:GridBoundColumn DataField="SleepingFrequencyTitle" HeaderText="Sleeping Frequency" />
                    <telerik:GridBoundColumn DataField="SendSizeTitle" HeaderText="Send Size" />
                    <telerik:GridBoundColumn DataField="BatchSizeTitle" HeaderText="Batch Size"  Visible="false" />
                    <telerik:GridBoundColumn DataField="QueuesizeTitle" HeaderText="Queue Size" Visible="false" />
                    <telerik:GridBoundColumn DataField="ReportOnEvents" HeaderText="Events" DataType="System.Boolean" />
                    <telerik:GridBoundColumn DataField="ReportOnCANEvents" HeaderText="CAN Events" DataType="System.Boolean" />
                    <telerik:GridBoundColumn DataField="DirectionAngleTitle" HeaderText="Direction Angle" />
                    <telerik:GridButtonColumn ButtonType="LinkButton" DataTextField="NumberOfVehicles" HeaderText="# of Vehicles" CommandName="Vehicles" />
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>    
        <div class="buttonbar">
            <asp:Button ID="btnAdd" runat="server" Text="Add New" CssClass="buttonclass" />
        </div>
    </telerik:RadPageView>
    <telerik:RadPageView runat="server" Width="100%">
        <asp:ValidationSummary runat="server" EnableClientScript="true" ShowSummary="true" HeaderText="Changes cannot be saved until the following are changed." />
        <table>
            <tr>
                <td class="formCellLabel-Horizontal">Title</td>
                <td class="formCellField-Horizontal"><asp:TextBox ID="txtTitle" runat="server" Width="150"></asp:TextBox><asp:RequiredFieldValidator runat="server" ControlToValidate="txtTitle" ErrorMessage="Please give the Profile a title." Display="None"></asp:RequiredFieldValidator></td>
            </tr>

             <tr>
                <td class="formCellLabel-Horizontal">Active Frequency</td>
                <td class="formCellField-Horizontal"><telerik:RadTimePicker runat="server" ID="rtpActiveFrequency" DateInput-DateFormat="HH:mm:ss" TimePopupButton-Visible="false"></telerik:RadTimePicker> </td>
            </tr>
            <tr>
                <td class="formCellLabel-Horizontal">Send Frequency</td>
                <td class="formCellField-Horizontal"><telerik:RadTimePicker runat="server" ID="rtpSendFrequency" DateInput-DateFormat="HH:mm:ss" TimePopupButton-Visible="false"></telerik:RadTimePicker> </td>
            </tr>
            <tr>
                <td class="formCellLabel-Horizontal">Idle Report Time</td>
                <td class="formCellField-Horizontal"><telerik:RadTimePicker runat="server" ID="rtpIdleReportTime" DateInput-DateFormat="HH:mm:ss" TimePopupButton-Visible="false"></telerik:RadTimePicker> </td>
            </tr>
            <tr>
                <td class="formCellLabel-Horizontal">Sleep Frequency</td>
                <td class="formCellField-Horizontal"><telerik:RadTimePicker runat="server" ID="rtpSleepFrequency" DateInput-DateFormat="HH:mm:ss" TimePopupButton-Visible="false"></telerik:RadTimePicker> </td>
            </tr>
            <tr>
                <td class="formCellLabel-Horizontal">Direction Angle</td>
                <td class="formCellField-Horizontal"><telerik:RadNumericTextBox runat="server" ID="rntDirectionAngle" NumberFormat-DecimalDigits="0" MinValue="0" MaxValue="360" /></td>
            </tr>
             <tr>
                <td class="formCellLabel-Horizontal">Distance Reporting </td>
                <td class="formCellField-Horizontal"><telerik:RadNumericTextBox runat="server" ID="rntDistance" NumberFormat-DecimalDigits="0" /></td>
            </tr>
             <tr>
                <td class="formCellLabel-Horizontal">Report On Events</td>
                <td class="formCellField-Horizontal"><asp:CheckBox runat="server" ID="chkReportOnEvents" /></td>
            </tr>
            <tr>
                <td class="formCellLabel-Horizontal">Report On CANbus Events</td>
                <td class="formCellField-Horizontal"><asp:CheckBox runat="server" ID="chkReportOnCANEvents" /></td>
            </tr>
            <tr>
                <td class="formCellLabel-Horizontal">Is Default Profile</td>
                <td class="formCellField-Horizontal"><asp:CheckBox runat="server" ID="chkIsDefault" /></td>
            </tr>
        </table>
        <asp:Panel runat="server" Visible="false" CssClass="infoPanel" ID="pnlInfo">
            <asp:Label runat="server" ID="lblMessage" CssClass="info"></asp:Label>
        </asp:Panel>
        <div class="buttonbar">
            <asp:Button ID="btnSave" Text="Save" runat="server" CssClass="buttonclass" />
             <asp:Button ID="btnCancel" Text="Cancel" runat="server" CssClass="buttonclass" CausesValidation="false" />
        </div>
    </telerik:RadPageView>
    <telerik:RadPageView runat="server" ID="rpvVehicles">
        <table style="border:none;">
            <tr>
                <th>
                    Vehicles In profile
                </th>
                <th>
                    Vehicles
                </th>
            </tr>
            <tr>
                <td><telerik:RadListBox runat="server" ID="lstVehiclesInProfile" SelectionMode="Multiple" Width="200px" Height="400px" AllowTransfer="true" TransferToID="lstVehicles" EnableDragAndDrop="true"></telerik:RadListBox></td>
                <td><telerik:RadListBox runat="server" ID="lstVehicles" Width="200px" Height="400px" SelectionMode="Multiple" ></telerik:RadListBox></td>
            </tr>

        </table>
        <div class="buttonbar">
            <asp:Button ID="btnSaveVehicles" runat="server" Text="Save" CssClass="buttonclass" />
            <asp:Button ID="btnCancelVehicles" runat="server" Text="Cancel" CssClass="buttonclass"  />
        </div>
    </telerik:RadPageView>
</telerik:RadMultiPage>

</asp:Content>
