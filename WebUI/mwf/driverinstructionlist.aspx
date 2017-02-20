<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="driverinstructionlist.aspx.cs" Inherits="Orchestrator.WebUI.mwf.driverinstructionlist" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">
    <script src="../script/jquery-ui-1.9.2.min.js" type="text/javascript"></script>
    <script src="../script/jquery.blockUI-2.64.0.min.js" type="text/javascript"></script>
    <script src="../script/loading-panel.js" type="text/javascript"></script>
    <script src="driverinstructionlist.aspx.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1 runat="server" id="headerText">Worker's current instructions</h1>
    
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table class="filter">
        <tr>
            <td class="formCellLabel">
                <label>Worker</label>
            </td>
            <td valign="top">
                <telerik:RadComboBox ID="DriverPicker" runat="server" ClientIDMode="Static" DataValueField="Value"
                    DataTextField="Text" AppendDataBoundItems="true" Width="200px" DropDownWidth="200px" AutoPostBack="true">
                    <Items>
                        <telerik:RadComboBoxItem Value="" Text="- select -" />
                    </Items>
                </telerik:RadComboBox>
            </td>
        </tr>
    </table>

    <telerik:RadGrid runat="server" ID="InstructionsGrid" AutoGenerateColumns="false" AllowSorting="true">
        <MasterTableView PageSize="15" AllowPaging="false" DataKeyNames="ID, StatusID, CommunicationStatusID" ClientDataKeyNames="ID" CommandItemDisplay="Top">
            <CommandItemTemplate>
                <input type="button" id="RefreshButton" class="buttonClassSmall" value="Refresh" />
                <input type="button" id="ReassignButton" class="buttonClassSmall" value="Reassign all" />
                <input type="button" id="CommunicateButton" class="buttonClassSmall" value="Communicate new and changed" />
                <input type="button" id="CommunicateAllButton" class="buttonClassSmall" value="Communicate all" />
            </CommandItemTemplate>
            <Columns>
                <telerik:GridHyperLinkColumn HeaderText="Run ID" DataTextField="JobID" DataNavigateUrlFields="JobID"
                    DataNavigateUrlFormatString="/job/job.aspx?jid={0}&csid=xx" />
                <telerik:GridBoundColumn DataField="CustomerOrder" HeaderText="Customer order" />
                <telerik:GridBoundColumn DataField="CustomerReference" HeaderText="Customer ref" />
                <telerik:GridBoundColumn DataField="InstructionType" HeaderText="Type" />
                <telerik:GridBoundColumn DataField="Location" HeaderText="Location" />
                <telerik:GridBoundColumn DataField="ArriveDateTime" HeaderText="Arrival time" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                <telerik:GridBoundColumn DataField="RunStatus" HeaderText="Run status" />
                <telerik:GridBoundColumn DataField="InstructionStatus" HeaderText="Instruction status" />
                <telerik:GridBoundColumn DataField="Driver" HeaderText="Worker" />
                <telerik:GridBoundColumn DataField="VehicleReg" HeaderText="Vehicle" />
                <telerik:GridBoundColumn DataField="CommunicationStatus" HeaderText="Communication status" />
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>

    <div id="ReassignDriverDialog" style="display: none; background-color: White;">
        <h3>Reassign all instructions to worker</h3>
        <telerik:RadComboBox runat="server" ID="ReassignDriverPicker" DataValueField="Value" DataTextField="Text" MarkFirstMatch="true" AppendDataBoundItems="true" OnClientSelectedIndexChanged="reassignDriverPicker_selectedIndexChanged">
            <Items>
                <telerik:RadComboBoxItem Value="" Text="- unassign all -" />
            </Items>
        </telerik:RadComboBox>
        <br /><br />
        <div id="AssignDriverWarningDiv" style="display: none; margin-bottom: 10px;">
            <h2 id="AssignDriverWarningMessage">
                Warning: This worker already has one or more potentially conflicting instructions.
                <a href="#" id="AssignDriverWarningLink">Click here to see the worker's current instructions.</a>
            </h2>
        </div>
        <div>
            <input type="button" id="ReassignDriverButton" value="OK" />
        </div>
    </div>
</asp:Content>