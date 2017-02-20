<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="scheduleresource.aspx.cs" Inherits="Orchestrator.WebUI.Schedule.scheduleresourcenew" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>Your Resources</h1>
    <style>
        .RadScheduler .rfbGroup .rfbLabel
        {
            width: 100px;
        }
        .validationErrorIcon
        {
            width: 300px;
        }
        html body .riSingle .riTextBox
        {
            width: 95%;
        }
        .riContentWrapper
        {
            width: 95%;
        }
    </style>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:SqlDataSource ID="SchedulerDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:Orchestrator%>" 
    SelectCommand="[spResourceSchedule_GetForResourceTypeId]" SelectCommandType="StoredProcedure"
    DeleteCommand="[spResourceSchedule_Delete]" DeleteCommandType="StoredProcedure"
    InsertCommand="[spResourceSchedule_Create]" InsertCommandType="StoredProcedure"
    UpdateCommand="[spResourceSchedule_Update]" UpdateCommandType="StoredProcedure">
    <SelectParameters>
        <asp:ControlParameter Name="ResourceTypeId" ControlID="cboResourceTypes" PropertyName="SelectedValue" />
        <asp:ControlParameter Name="VisibleStartDate" ControlID="RadScheduler1" PropertyName="VisibleRangeStart" />
        <asp:ControlParameter Name="VisibleEndDate" ControlID="RadScheduler1" PropertyName="VisibleRangeEnd" />
        <asp:ControlParameter Name="DriverTypeId" ControlID="cboDriverTypes" PropertyName="SelectedValue" DefaultValue="0" />
        <asp:ControlParameter Name="OrganisationLocationId" ControlID="cboDepots" PropertyName="SelectedValue" DefaultValue="0" />
        <asp:ControlParameter Name="ShowUnavailable" ControlID="chkShowUnavailable" PropertyName="Checked" DefaultValue="false" />
    </SelectParameters>
    <DeleteParameters>
        <asp:Parameter Name="ResourceScheduleId" Type="Int32" />
    </DeleteParameters>
    <InsertParameters>
        <asp:Parameter Name="ResourceActivityTypeId" Type="Int32" />
        <asp:Parameter Name="ResourceId" Type="Int32" />
        <asp:Parameter Name="StartDateTime" Type="DateTime" />
        <asp:Parameter Name="EndDateTime" Type="DateTime" />
        <asp:Parameter Name="Comments" Type="String" />
        <asp:Parameter Name="UserId" Type="String" />
     </InsertParameters>
     <UpdateParameters>
        <asp:Parameter Name="ResourceScheduleId" Type="Int32" />
        <asp:Parameter Name="ResourceActivityTypeId" Type="Int32" />
        <asp:Parameter Name="ResourceId" Type="Int32" />
        <asp:Parameter Name="StartDateTime" Type="DateTime" />
        <asp:Parameter Name="EndDateTime" Type="DateTime" />
        <asp:Parameter Name="Comments" Type="String" />
        <asp:Parameter Name="UserId" Type="String" />
     </UpdateParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="ActivityTypeDataSource" runat="server" 
        ConnectionString="<%$ ConnectionStrings:Orchestrator%>" 
        SelectCommand="[spResourceSchedule_GetActivityTypeForType]" SelectCommandType="StoredProcedure">
        <SelectParameters>
            <asp:ControlParameter Name="ResourceTypeId" ControlID="cboResourceTypes"  PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="ResourceDataSource" runat="server" 
        ConnectionString="<%$ ConnectionStrings:Orchestrator%>" 
        SelectCommand="[spResource_GetAllByType]" SelectCommandType="StoredProcedure"> 
        <SelectParameters>
            <asp:ControlParameter Name="ResourceTypeId" ControlID="cboResourceTypes" PropertyName="SelectedValue" />
            <asp:ControlParameter Name="ShowUnavailable" ControlID="chkShowUnavailable" PropertyName="Checked" />
        </SelectParameters>
    </asp:SqlDataSource>
    <asp:PlaceHolder ID="Placeholder1" runat="server">
        <fieldset>
            <legend>Filter Options</legend>
            <table>
                <tr>
                    <td class="formCellLabel">
                        Resource Type:
                    </td>
                    <td class="formCellField">
                        <asp:DropDownList ID="cboResourceTypes" runat="server" AutoPostBack="true">
                        </asp:DropDownList>
                    </td>                    
                </tr>
                <tr style="<%= showDriverTypes == true ? "": "display:none" %>">
                    <td class="formCellLabel">
                        Driver Type:
                    </td>
                    <td class="formCellField">
                        <asp:DropDownList ID="cboDriverTypes" runat="server" AutoPostBack="true">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr style="<%= showDepots == true ? "": "display:none" %>">
                <td class="formCellLabel">
                        Depot:
                    </td>
                    <td class="formCellField">
                        <asp:DropDownList ID="cboDepots" runat="server" AutoPostBack="true">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Show Unavailable Resources
                    </td>
                    <td class="formCellField">
                        <asp:CheckBox ID="chkShowUnavailable" runat="server" AutoPostBack="true" Checked="false" />
                    </td>
                </tr>
            </table>
        </fieldset>
    </asp:PlaceHolder>
    
    <telerik:RadScheduler ID="RadScheduler1" runat="server" 
        SelectedView="MonthView"  DataSourceID="SchedulerDataSource" 
        DataKeyField="ResourceScheduleId" DataStartField="StartDateTime" 
        DataEndField="EndDateTime" DataSubjectField="Comments" Height="" 
        Skin="WebBlue" FirstDayOfWeek="Monday" LastDayOfWeek="Sunday" 
        StartInsertingInAdvancedForm="True" DayStartTime="00:00:00" 
        EnableExactTimeRendering="True" OnClientAppointmentMoveStart="OnClientAppointmentMoveStart" WeekView-HeaderDateFormat="dddd, d-MMMM-yyyy" >
        <Reminders Enabled="False" />
        <TimelineView UserSelectable="False"></TimelineView>
        <MonthView VisibleAppointmentsPerDay="50"/>
        <TimeSlotContextMenuSettings EnableDefault="true" />
        <AppointmentContextMenuSettings EnableDefault="true" />
        <AppointmentTemplate>
            <div>
                <%# Container.Appointment.Resources.GetResourceByType("Resource Name").Text%> - <%# Container.Appointment.Resources.GetResourceByType("Activity Type").Text%>
            </div>
        </AppointmentTemplate>
        <AdvancedForm Modal="True" />
        <ResourceTypes>
            <telerik:ResourceType DataSourceID="ActivityTypeDataSource" ForeignKeyField="ResourceActivityTypeId" 
                KeyField="ResourceActivityTypeId" Name="Activity Type" TextField="Description" />
            <telerik:ResourceType DataSourceID="ResourceDataSource" ForeignKeyField="ResourceId" 
                KeyField="ResourceId" Name="Resource Name" TextField="Description" />
        </ResourceTypes>
    </telerik:RadScheduler>

    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" skin="WebBlue" BackgroundPosition="Top">
    </telerik:RadAjaxLoadingPanel>

    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="RadScheduler1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="RadScheduler1" LoadingPanelID="RadAjaxLoadingPanel1"/>
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>

    <script type="text/javascript">
        function OnClientAppointmentMoveStart(sender, eventArgs) {
            //Disables the Appointment dragging feature.
            eventArgs.set_cancel(true);
        }

        // Without this, anything other than 100% Zoom on Chrome will crash the screen on Post-Back
        // This will be fixed in Q3 2014 SP1 and can be removed from this solution when upgraded
        // http://www.telerik.com/forums/system-formatexception-input-string-was-not-in-a-correct-format-thrown-on-chrome-when-browser-is-zoomed-in-out
        Telerik.Web.UI.RadScheduler.prototype.saveClientState = function () {
            // Temporary workaround required to stop Zoom / Scroll error in Chrome. Call Ref: 81269
            return '{"scrollTop":' + Math.round(this._scrollTop) + ',"scrollLeft":' + Math.round(this._scrollLeft) + ',"isDirty":' + this._isDirty + '}';
        }
    </script>  
</asp:Content>