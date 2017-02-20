<%@ Page Language="C#" MasterPageFile="~/default_tableless.master" AutoEventWireup="true" CodeBehind="drivermanifest.aspx.cs" Inherits="Orchestrator.WebUI.manifest.drivermanifest" Title="Driver Manifest Data" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<fieldset>
<h3><asp:Label ID="lblManifestTitle" runat="server"></asp:Label></h3>
<h2>Job ID<asp:Label ID="lblJobID" runat="server"></asp:Label></h2>
</fieldset>
<div style="height:8px;"></div>
<fieldset>
<telerik:RadGrid runat="server" id="grdInstructions" AutoGenerateColumns="false">
    <MasterTableView>
        <Columns>
            <telerik:GridTemplateColumn HeaderText="Instruction">
                <ItemTemplate>
                    <%#Eval("Action") %><br /><%#Eval("Point") %>
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridBoundColumn DataField="ActionDateTime" HeaderText="At"></telerik:GridBoundColumn>
            <telerik:GridTemplateColumn HeaderText="Orders">
                <ItemTemplate>
                    <%#Eval("Orders") %>
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridBoundColumn HeaderText="Vehicle" DataField="Vehicle"></telerik:GridBoundColumn>
            <telerik:GridBoundColumn HeaderText="Trailer" DataField="Trailer"></telerik:GridBoundColumn>
        </Columns>
    </MasterTableView>
</telerik:RadGrid>
</fieldset>
</asp:Content>
