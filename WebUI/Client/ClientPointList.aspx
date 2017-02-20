<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/default_tableless_client.master" CodeBehind="ClientPointList.aspx.cs" Inherits="Orchestrator.WebUI.Client.ClientPointList" %>

<asp:Content ContentPlaceHolderID="Header" runat="server"></asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Collection / Delivery Point List</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="buttonbar">
        <asp:Button ID="btnRefreshTop" runat="server"  Text="Refresh" Width="75"  />
        <asp:Button ID="Button1" runat="server"  Text="Back" Width="75" OnClientClick="location.href='/default.aspx'; return false;"  />
    </div>
    
    <div style="height: 10px;">
    </div>
    <telerik:RadGrid runat="server" ID="grdPoints" AllowFilteringByColumn="false" AllowPaging="true" PageSize="50" AllowSorting="true" 
    AllowMultiRowSelection="true" AutoGenerateColumns="false" ShowStatusBar="false" EnableViewState="false" aja>
        <mastertableview datakeynames="PointId">
                <RowIndicatorColumn Display="false">
                </RowIndicatorColumn>
                <Columns>
                    <telerik:GridBoundColumn Display="true" UniqueName="OrganisationName" DataField="OrganisationName" HeaderText="Organisation Name" />
                    <telerik:GridBoundColumn Display="true" UniqueName="PointDescription" DataField="PointDescription" HeaderText="Point" />
                    <telerik:GridBoundColumn Display="true" UniqueName="AddressLine1" DataField="AddressLine1" HeaderText="Address Line 1" />
                    <telerik:GridBoundColumn Display="true" UniqueName="AddressLine2" DataField="AddressLine2" HeaderText="Address Line 2" />
                    <telerik:GridBoundColumn Display="true" UniqueName="AddressLine3" DataField="AddressLine3" HeaderText="Address Line 3" />
                    <telerik:GridBoundColumn Display="true" UniqueName="PostTown" DataField="PostTown" HeaderText="Post Town" />
                    <telerik:GridBoundColumn Display="true" UniqueName="County" DataField="County" HeaderText="County" />
                    <telerik:GridBoundColumn Display="true" UniqueName="PostCode" DataField="PostCode" HeaderText="Post Code" />
                    <telerik:GridBoundColumn Display="true" UniqueName="CountryDescription" DataField="CountryDescription" HeaderText="Country" />
                </Columns>
            </mastertableview>
            <PagerStyle Mode="NumericPages"></PagerStyle>
    </telerik:RadGrid>
    <br />
    <div class="buttonbar">
        <asp:Button ID="btnRefresh" runat="server" Text="Refresh" Width="75"  /><asp:Button ID="Button2" runat="server"  Text="Back" Width="75" OnClientClick="location.href='/default.aspx'; return false;"  />
    </div>
</asp:Content>