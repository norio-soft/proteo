<%@ Page Language="C#" MasterPageFile="~/default_tableless.master" AutoEventWireup="true" CodeBehind="FuelSurchargeList.aspx.cs" Inherits="Orchestrator.WebUI.FuelSurcharge.FuelSurchargeList" %>

<%@ Register Assembly="System.Web.Entity, Version=3.5.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" Namespace="System.Web.UI.WebControls" TagPrefix="asp" %>
<%@ Register Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI" TagPrefix="cc1" %>

<asp:Content ContentPlaceHolderID="Header" runat="server"></asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Standard Fuel Surcharge Rates</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <cc1:Dialog ID="dlgAddFuelSurcharge" runat="server" Width="400" Height="320" Mode="Modal" URL="AddFuelSurcharge.aspx" AutoPostBack="true" ReturnValueExpected="true"></cc1:Dialog>
    
    <h2>
        Standard Fuel Surcharges are applied to orders whose client are setup to use the standard fuel surcharge
        percentage rates. Note that clients can override this standard fuel surcharge or have
        a percentage adjustment relative to it.
    </h2>
    <br />

    <asp:Panel runat="server" ID="pnlUserAuthorised">
        <telerik:RadGrid EnableViewState="false" ID="grdFuelSurcharge" runat="server"
            AllowPaging="True" PagerStyle-AlwaysVisible="true" AllowSorting="True" Width="700">
            <PagerStyle CssClass="DataGridListPagerStyle" Mode="NumericPages" />
            <ClientSettings>
                <Selecting AllowRowSelect="True" />
            </ClientSettings>
            <MasterTableView ClientDataKeyNames="FuelSurchargeID" AutoGenerateColumns="False">
                <RowIndicatorColumn>
                    <HeaderStyle Width="20px"></HeaderStyle>
                </RowIndicatorColumn>
                <ExpandCollapseColumn>
                    <HeaderStyle Width="20px"></HeaderStyle>
                </ExpandCollapseColumn>
                <Columns>
                    <telerik:GridBoundColumn HeaderText="Surcharge %" DataFormatString="{0:N2} %"
                        DataField="SurchargeRate" UniqueName="SurchargeRate" DataType="System.Decimal"
                        ReadOnly="True" SortExpression="SurchargeRate">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn Visible="false" HeaderText="Fuel Surcharge ID" DataField="FuelSurchargeID"
                        UniqueName="FuelSurchargeID" DataType="System.Int32" ReadOnly="True" SortExpression="FuelSurchargeID">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn HeaderText="Effective Date" DataFormatString="{0:dd/MM/yy}"
                        DataField="EffectiveDate" UniqueName="EffectiveDate" DataType="System.DateTime"
                        ReadOnly="True" SortExpression="EffectiveDate">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="CreateDate" DataType="System.DateTime" HeaderText="Create Date"
                        ReadOnly="True" SortExpression="CreateDate" UniqueName="CreateDate">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="CreateUserID" HeaderText="Created By" ReadOnly="True"
                        SortExpression="CreateUserID" UniqueName="CreateUserID">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="LastUpdateDate" DataType="System.DateTime" HeaderText="Last Updated"
                        ReadOnly="True" SortExpression="LastUpdateDate" UniqueName="LastUpdateDate">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="LastUpdateUserID" HeaderText="Updated By" ReadOnly="True"
                        SortExpression="LastUpdateUserID" UniqueName="LastUpdateUserID">
                    </telerik:GridBoundColumn>
                </Columns>
            </MasterTableView>
            <FilterMenu EnableTheming="True">
                <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
            </FilterMenu>
        </telerik:RadGrid>

        <div class="buttonbar">
            <asp:Button runat="server" ID="btnAddFuelSurcharge" Text="Add Fuel Surcharge" />
            <asp:Button ID="btnRefresh" runat="server" Text="Refresh" ValidationGroup="grpRefresh"
                CausesValidation="true" />
        </div>
    </asp:Panel>

    <asp:Panel runat="server" ID="pnlUserNotAuthorised" Visible="false">
        You need to be a member of the Pricing user group to access this page. Please see
        your system administrator.
    </asp:Panel>
</asp:Content>