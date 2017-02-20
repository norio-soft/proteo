<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="ManagePlanningCategories.aspx.cs" Inherits="Orchestrator.WebUI.Resource.ManagePlanningCategories" %>
<%@ Register Assembly="System.Web.Entity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" Namespace="System.Web.UI.WebControls" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Planning Categories</h1></asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadGrid runat="server" ID="grdPlanningCategories">
        <HeaderContextMenu EnableTheming="True">
            <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
        </HeaderContextMenu>

        <MasterTableView DataKeyNames="ID" AutoGenerateColumns="False" EditMode="InPlace" ShowFooter="true" CommandItemDisplay="Top">
            <RowIndicatorColumn>
                <HeaderStyle Width="20px"></HeaderStyle>
            </RowIndicatorColumn>

            <ExpandCollapseColumn>
                <HeaderStyle Width="20px"></HeaderStyle>
            </ExpandCollapseColumn>

            <CommandItemTemplate>
                <asp:LinkButton ID="LinkButton2" runat="server" CommandName="InitInsert">Add New</asp:LinkButton>
            </CommandItemTemplate>

            <Columns>
                <telerik:GridEditCommandColumn ButtonType="LinkButton" UniqueName="editcommand"></telerik:GridEditCommandColumn>
                <telerik:GridBoundColumn DataField="ID" HeaderText="ID" ReadOnly="true" 
                    DataType="System.Int32" SortExpression="ID"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="DisplayShort" HeaderText="Short Description" 
                    SortExpression="DisplayShort"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="DisplayLong" HeaderText="Long Description" 
                    SortExpression="DisplayLong"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="CreateDate" HeaderText="Created" ReadOnly="true"
                    DataType="System.DateTime" SortExpression="CreateDate"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="CreateUserID" HeaderText="Created By" ReadOnly="true"
                    SortExpression="CreateUserID"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="LastUpdateDate" HeaderText="Last Updated" ReadOnly="true"
                    DataType="System.DateTime" SortExpression="LastUpdateDate" 
                    UniqueName="LastUpdateDate"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="LastUpdateUserID" ReadOnly="true"
                    HeaderText="LastUpdateUserID" SortExpression="LastUpdated By"></telerik:GridBoundColumn>
            </Columns>

            <EditFormSettings>
                <EditColumn UniqueName="EditCommandColumn1"></EditColumn>
            </EditFormSettings>
        </MasterTableView>

        <FilterMenu EnableTheming="True">
            <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
        </FilterMenu>
    </telerik:RadGrid>
</asp:Content>
