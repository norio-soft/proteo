<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="DriverMessageList.aspx.cs" Inherits="Orchestrator.WebUI.mwf.DriverMessageList" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1 runat="server" id="headerText">MWF Messages</h1>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="ToolbarBlue" style="height: 24px; padding: 1px 1px 1px 3px; background-position: top;">
        <div class="overlayedFilterIconOff" id="filterOptionsDiv">
            Show filter options
        </div>
        <div class="overlayedFilterIconOn" id="filterOptionsDivHide" style="display: none;">
            Close filter options
        </div>
        <asp:Button ID="ToolbarRefreshButton" runat="server" Text="Refresh" />
    </div>

    <div class="overlayedFilterBox" id="overlayedClearFilterBox" style="display: none;">
        <table>
            <tr>
                <td class="formCellLabel">
                    <label>Driver</label>
                </td>
                <td class="formCellField">
                    <telerik:RadComboBox ID="DriverPicker" runat="server" ClientIDMode="Static" DataValueField="Value"
                        DataTextField="Text" AppendDataBoundItems="true" Width="200px" DropDownWidth="200px">
                        <Items>
                            <telerik:RadComboBoxItem Value="" Text="- select -" />
                        </Items>
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">Message date</td>
                <td class="formCellField" style="width: 400px;">
					<telerik:RadDatePicker ID="StartDate" runat="server" Width="100px" DateInput-DateFormat="dd/MM/yy" />
                    <asp:RequiredFieldValidator ID="StartDateRequired" runat="server" ControlToValidate="StartDate" ToolTip="Please specify a start date" Display="Dynamic" />
                    to
                    <telerik:RadDatePicker ID="EndDate" runat="server" Width="100px" DateInput-DateFormat="dd/MM/yy" />
                    <asp:CustomValidator ID="StartDateValid" runat="server" ControlToValidate="StartDate" ErrorMessage="The start date must be before the end date" Display="Dynamic" />
                </td>
            </tr>
        </table>

        <div class="buttonbar">
            <asp:Button ID="FilterOptionsRefreshButton" runat="server" Text="Refresh" />
        </div>
    </div>

    <telerik:RadGrid runat="server" ID="InstructionsGrid" AutoGenerateColumns="false" AllowSorting="true">
        <MasterTableView AllowPaging="false">
            <Columns>
                <telerik:GridBoundColumn DataField="CreateUserID" HeaderText="Sent by" />
                <telerik:GridBoundColumn DataField="CommunicateDateTime" HeaderText="Date/time sent" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                <telerik:GridBoundColumn DataField="Location" HeaderText="Point" />
                <telerik:GridBoundColumn DataField="ArriveDateTime" HeaderText="Message date/time" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                <telerik:GridBoundColumn DataField="DriverNames" HeaderText="Driver(s)" />
                <telerik:GridBoundColumn DataField="Message" HeaderText="Message" />
                <telerik:GridBoundColumn DataField="AcknowledgedDateTime" HeaderText="Acknowledged" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                <telerik:GridHyperLinkColumn HeaderText="Run ID" DataTextField="JobID" DataNavigateUrlFields="JobID" DataNavigateUrlFormatString="javascript:openJobDetails({0})" />
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>

    <script src="DriverMessageList.aspx.js"></script>
</asp:Content>