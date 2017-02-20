<%@ Page Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Job_multicallin" Title="Multi Debrief" Codebehind="multicallin.aspx.cs" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
<h1>Quick Call In</h1>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    
    <h2>You can select and call in any of the outstanding call-ins that are listed below.</h2>    
    <asp:Literal runat="server" ID="litError"></asp:Literal>

    <div class="buttonBar">
        <asp:Button ID="btnCallInTop" runat="server" Text="Call In" />
    </div>
    <telerik:RadGrid ID="grdCallIns" runat="server" Skin="Orchestrator" AutoGenerateColumns="false" AllowMultiRowSelection="true">
        <MasterTableView DataKeyNames="InstructionId, JobId" >
            <Columns>
                <telerik:GridClientSelectColumn uniquename="checkboxSelectColumn" HeaderStyle-HorizontalAlign=Left HeaderStyle-Width="40" HeaderText="" ></telerik:GridClientSelectColumn>
                <telerik:GridBoundColumn DataField="JobId" HeaderText="Run ID"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn datafield="LoadNo" headertext="Load No" itemstyle-width="80" SortExpression="LoadNo" />
                <telerik:GridBoundColumn datafield="Type" headertext="Type" itemstyle-width="40" SortExpression="Type" />
                <telerik:GridBoundColumn datafield="Client" headertext="Client" itemstyle-width="140" SortExpression="Client" />
                <telerik:GridBoundColumn datafield="Destination" headertext="Destination" itemstyle-width="140" SortExpression="Destination" />                                
                <telerik:GridBoundColumn datafield="BookedDateTime" dataformatstring="{0:dd/MM HH:mm}" headertext="Booked" itemstyle-width="100" SortExpression="BookedDateTime" />                
                <telerik:GridBoundColumn datafield="Docket" headertext="Docket" itemstyle-width="400" SortExpression="Docket" />                                
                <telerik:GridBoundColumn datafield="NoPallets" headertext="Pallets" itemstyle-width="45" SortExpression="NoPallets" />                                
                <telerik:GridBoundColumn datafield="Weight" headertext="Weight" DataFormatString="{0:F0}" itemstyle-width="80" SortExpression="Weight" />                                
                <telerik:GridBoundColumn datafield="Driver" headertext="Driver" itemstyle-width="120" SortExpression="Driver" />                                
                <telerik:GridBoundColumn datafield="TrailerRef" headertext="Trailer" itemstyle-width="40" SortExpression="TrailerRef" /> 
            </Columns>
        </MasterTableView>
        <ClientSettings>
            <Selecting AllowRowSelect="true"  EnableDragToSelectRows="true" />
        </ClientSettings>
    </telerik:RadGrid>
    <div class="buttonBar">
        <asp:Button ID="btnCallIn" runat="server" Text="Call In" />
    </div>
</asp:Content>