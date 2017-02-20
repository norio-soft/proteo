<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="True" CodeBehind="ZoneMapList.aspx.cs" Inherits="Orchestrator.WebUI.Tariff.ZoneMapList" %>
<%@ Import namespace="Humanizer" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Zone Maps</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <fieldset runat="server" class="invisiableFieldset" id="fsZoneMaps">
        <asp:CheckBox runat="server" ID="chkEnabledOnly" Text="Enabled Only" Checked="true"  
            AutoPostBack="true" />
        <telerik:RadGrid ID="grdZoneMaps" runat="server" AutoGenerateColumns="false">
            <MasterTableView DataKeyNames="ZoneMapID">
                <Columns>
                    <telerik:GridHyperLinkColumn HeaderText="Zone Map" DataTextField="Description"
                        DataNavigateUrlFormatString="EditZonePostcodes.aspx?ZoneMapID={0}" DataNavigateUrlFields="ZoneMapID" />
                    <telerik:GridTemplateColumn DataField="ZoneType" HeaderText="Zone Type">
                        <ItemTemplate><%# ((Orchestrator.eZoneType)Eval("ZoneType")).Humanize() %></ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridCheckBoxColumn DataField="IsEnabled" HeaderText="Enabled" />
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
        <div class="buttonbar">
            <asp:Button ID="btnAdd" runat="server" Text="Add Zone Map" />
        </div>
    </fieldset>
</asp:Content>
