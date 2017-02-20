<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="True" CodeBehind="ScaleList.aspx.cs" Inherits="Orchestrator.WebUI.Tariff.ScaleList" %>
<%@ Import namespace="Humanizer" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Scales</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <fieldset runat="server" class="invisiableFieldset" id="fsScales">
        <asp:CheckBox runat="server" ID="chkEnabledOnly" Text="Enabled Only" Checked="true" AutoPostBack="true" />

        <telerik:RadGrid ID="grdScales" runat="server" AutoGenerateColumns="false">
            <MasterTableView DataKeyNames="ScaleID">
                <Columns>
                    <telerik:GridHyperLinkColumn HeaderText="Scale" DataTextField="Description"
                        DataNavigateUrlFormatString="EditScale.aspx?scaleID={0}" DataNavigateUrlFields="ScaleID" />
                    <telerik:GridTemplateColumn DataField="Metric" HeaderText="Metric">
                        <ItemTemplate><%# ((Orchestrator.eMetric)Eval("Metric")).Humanize() %></ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridCheckBoxColumn DataField="IsEnabled" HeaderText="Enabled" />
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>

        <div class="buttonbar">
            <asp:Button ID="btnAdd" runat="server" Text="Add Scale" />
        </div>
    </fieldset>
</asp:Content>
