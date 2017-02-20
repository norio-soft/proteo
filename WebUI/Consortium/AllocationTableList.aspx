<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="AllocationTableList.aspx.cs" Inherits="Orchestrator.WebUI.Consortium.AllocationTableList" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>Allocation Tables</h1>
</asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="width: 400px;">
        <fieldset>
            <legend>Allocation Zone Tables</legend>

            <telerik:RadGrid ID="grdZoneTables" runat="server" AutoGenerateColumns="false">
                <MasterTableView DataKeyNames="AllocationZoneTableID">
                    <Columns>
                        <telerik:GridHyperLinkColumn DataTextField="Description" DataNavigateUrlFormatString="EditAllocationZoneTable.aspx?aztid={0}" DataNavigateUrlFields="AllocationZoneTableID" HeaderText="Description" />
                        <telerik:GridBoundColumn DataField="ZoneMapDescription" HeaderText="Zone Map" />
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>

            <div class="buttonbar">
                <input type="button" value="Add Allocation Zone Table" onclick="location.href='addallocationzonetable.aspx'" />
            </div>
        </fieldset>
        
        <fieldset>
            <legend>Allocation Point Tables</legend>

            <telerik:RadGrid ID="grdPointTables" runat="server" AutoGenerateColumns="false">
                <MasterTableView DataKeyNames="AllocationPointTableID">
                    <Columns>
                        <telerik:GridHyperLinkColumn DataTextField="Description" DataNavigateUrlFormatString="EditAllocationPointTable.aspx?aptid={0}" DataNavigateUrlFields="AllocationPointTableID" HeaderText="Description" />
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>

            <div class="buttonbar">
                <input type="button" value="Add Allocation Point Table" onclick="location.href='addallocationpointtable.aspx'" />
            </div>
        </fieldset>
    </div>
</asp:Content>