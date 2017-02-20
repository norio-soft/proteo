<%@ Page language="c#" MasterPageFile="~/default_tableless.master" Inherits="Orchestrator.WebUI.Traffic.Filters.ListFilters" Title="My traffic sheet filters" Codebehind="ListFilters.aspx.cs" %>
<%@ Register TagPrefix="P1" Namespace="P1TP.Components.Web.UI" Assembly="P1TP.Components" %>

<asp:Content ContentPlaceHolderID="Header" runat="server"></asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>My Traffic Sheet Filters</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <h2>Your filters are displayed below</h2>
	<div id="divPointAddress" style="z-index: 5;">
		<table>
			<tr>
				<td><span id="spnPointAddress"></span></td>
			</tr>
		</table>
	</div>
    <telerik:RadGrid runat="server" ID="grdFilters" AllowPaging="false" AllowSorting="false" Skin="Office2007" AutoGenerateColumns="false" AllowMultiRowSelection="false">
        <MasterTableView Width="100%" ClientDataKeyNames="FilterId" DataKeyNames="FilterId" >
            <RowIndicatorColumn Display="false"></RowIndicatorColumn>
            <Columns>
            <telerik:GridBoundColumn HeaderText="Filter Name" DataField="FilterName"></telerik:GridBoundColumn>                                             	
                <telerik:GridBoundColumn HeaderText="Active" DataField="IsActive" UniqueName="IsActive"></telerik:GridBoundColumn>                                             	
                <telerik:GridBoundColumn HeaderText="Control Area" DataField="ControlArea"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Default" DataField="IsDefault"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="My Runs" DataField="IsOnlyShowMyJobs"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Runs With PCVs" DataField="IsOnlyShowJobsWithPCVs"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Runs With Demurrage" DataField="IsOnlyShowJobsWithDemurrage"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Runs With Demurrage Awaiting Acceptance" DataField="IsOnlyShowJobsWithDemurrageAwaitingAcceptance"></telerik:GridBoundColumn>
                <telerik:GridTemplateColumn HeaderText="Traffic Areas" DataField="TrafficAreas">
                    <ItemTemplate>
                        <%# GetTrafficAreas(((System.Data.DataRowView)Container.DataItem)["TrafficAreas"].ToString())%>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Run States" DataField="JobStates">
                    <ItemTemplate>
                        <%# GetJobStates(((System.Data.DataRowView)Container.DataItem)["JobStates"].ToString())%>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Run Types" >
                    <ItemTemplate>
                        <%# GetJobTypes(((System.Data.DataRowView)Container.DataItem)["JobTypeIDs"].ToString())%>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridButtonColumn CommandName="SetDefault" ButtonType="PushButton" Text="Make Default" UniqueName="btnDefaults" headerStyle-Width="100" ></telerik:GridButtonColumn>
                <telerik:GridButtonColumn CommandName="SwitchActivity" ButtonType="PushButton" Text="Deactivate" UniqueName="btnActivate"></telerik:GridButtonColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings >
            <Resizing AllowColumnResize="true" AllowRowResize="false" />
        </ClientSettings>
    </telerik:RadGrid>
    <div align="right">
        <asp:CheckBox id="chkShowDeactivated" runat="server" Text="Show Deactivated Filters" Text-Align="Right" AutoPostBack="True"></asp:CheckBox>
    </div>
</asp:Content>