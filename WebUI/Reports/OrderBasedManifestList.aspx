<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderBasedManifestList.aspx.cs" MasterPageFile="~/default_tableless.master" Inherits="Orchestrator.WebUI.Reports.OrderBasedManifestList" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>Order Manifests <asp:Label ID="lblTitle" runat="server" Font-Bold="false" Font-Size="Small"></asp:Label></h1>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadAjaxPanel ID="radPanel" runat="server">
        
        <h2>
            All Order manifests for the specified date range, you can change which manifests
            you see by changing the dates and clicking on Get Manifests
        </h2>
        
        <div style="font-size: 210%">
            <asp:ValidationSummary runat="server" />
            <span>
                From
                <telerik:RadDateInput ID="dteStartDate" EmptyMessage="Enter Date" runat="server" DisplayDateFormat="dd/MM/yy" DateFormat="dd/MM/yy"></telerik:RadDateInput>
                <asp:RequiredFieldValidator ID="rfvStartDate" ControlToValidate="dteStartDate" runat="server" ErrorMessage="You must give a start date" Display="None"></asp:RequiredFieldValidator>
            </span>
            <span>
                To
                <telerik:RadDateInput ID="dteEndDate" runat="server" EmptyMessage="Enter Date" DisplayDateFormat="dd/MM/yy" DateFormat="dd/MM/yy">
                </telerik:RadDateInput><asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ControlToValidate="dteEndDate" ErrorMessage="You must give an end date" Display="None"></asp:RequiredFieldValidator>
            </span>
            <asp:CustomValidator ID="cvDateRange" runat="server" ErrorMessage="The date range is too large." Display="None" ControlToValidate="dteEndDate"></asp:CustomValidator>
        </div>
        
        <div class="buttonbar" style="height: 30px; margin-top: 5px; padding: 4px; color: #ffffff;
            background-color: #99BEDE;">
            <asp:Button ID="btnGetManifests" runat="server" CssClass="buttonClass" Text="Get Manifests" />
            <input style="width: 175px;" value="Create New Order Manifest" type="button" onclick="javascript:window.location = 'orderbasedmanifest.aspx';" />
        </div>
        
        <telerik:RadGrid runat="server" ID="grdManifestList" AllowFilteringByColumn="false"
            Skin="Office2007" AllowPaging="true" AllowSorting="true" AllowMultiRowSelection="true"
            EnableAJAX="true" AutoGenerateColumns="false" ShowStatusBar="false" PageSize="250">
            <ClientSettings AllowDragToGroup="true" AllowColumnsReorder="true">
                <Selecting AllowRowSelect="True" EnableDragToSelectRows="true"></Selecting>
            </ClientSettings>
            <PagerStyle NextPageText="Next &amp;gt;" PrevPageText="&amp;lt; Prev" Mode="NextPrevAndNumeric">
            </PagerStyle>
            <MasterTableView DataKeyNames="OrderManifestId">
                <RowIndicatorColumn Display="false">
                </RowIndicatorColumn>
                <Columns>
                    <telerik:GridTemplateColumn UniqueName="OrderId" HeaderText="Manifest No." DataField="OrderManifestId"
                        SortExpression="OrderManifestId" AllowFiltering="true">
                        <ItemTemplate>
                            <asp:HyperLink runat="server" ID="hypOrderManifestId"></asp:HyperLink>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn UniqueName="Description" DataField="Description" HeaderText="Name"
                        AllowFiltering="true" />
                    <telerik:GridTemplateColumn UniqueName="ManifestDate" HeaderText="Manifest Date"
                        AllowFiltering="true">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblManifestDate"></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="Resource" HeaderText="Resource/Subcontractor"
                        AllowFiltering="true">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblResource"></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn UniqueName="CreateDate" DataField="CreateDate" HeaderText="Created"
                        AllowFiltering="true" />
                    <telerik:GridBoundColumn UniqueName="CreateUserId" DataField="CreateUserId" HeaderText="Created By"
                        AllowFiltering="true" />
                    <telerik:GridBoundColumn UniqueName="LastUpdateDate" DataField="LastUpdateDate" HeaderText="Last Updated"
                        AllowFiltering="true" />
                    <telerik:GridBoundColumn UniqueName="LastUpdateUserId" DataField="LastUpdateUserId"
                        HeaderText="Last Updated By" AllowFiltering="true" />
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
    
    </telerik:RadAjaxPanel>
</asp:Content>