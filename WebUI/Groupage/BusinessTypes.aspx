<%@ Page Language="C#" MasterPageFile="~/default_tableless.master" AutoEventWireup="true" Inherits="Orchestrator.WebUI.BusinessTypes" Title="Haulier Enterprise" Codebehind="BusinessTypes.aspx.cs" %>

<asp:Content ContentPlaceHolderID="Header" runat="server"></asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Business Types for Your Organisation</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<fieldset>
    <telerik:RadGrid runat="server" ID="grdBusinessTypes" AutoGenerateColumns="false" EnableAJAX="true" Width="600" >
        <MasterTableView DataKeyNames="BusinessTypeID">
            <DetailTables>
                <telerik:GridTableView DataKeyNames="BusinessTypeID"  >
                    <ParentTableRelation>
                        <telerik:GridRelationFields DetailKeyField="BusinessTypeID" MasterKeyField="BusinessTypeID" />
                    </ParentTableRelation>
                    <Columns>
                        <telerik:GridBoundColumn HeaderText="Nominal Code" DataField="NominalCode"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderText="Description" DataField="Description"></telerik:GridBoundColumn>
                    </Columns>
                </telerik:GridTableView>
            </DetailTables>
            <Columns>
                <telerik:GridHyperLinkColumn HeaderText="Description" DataNavigateUrlFields="BusinessTypeID" HeaderStyle-Width="180" DataNavigateUrlFormatString="managebusinesstype.aspx?BTID={0}" DataTextField="Description"></telerik:GridHyperLinkColumn>
                <telerik:GridTemplateColumn HeaderText="Capture the Call-In Information">
                    <ItemTemplate>
                        <%# ((bool)DataBinder.Eval(Container.DataItem, "CaptureDeBriefInformation") == true) ? "Yes" : "No" %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Show Create Run Checkbox on Add Order" DataField="ShowCreateJob">
                    <ItemTemplate>
                        <%# ((bool)DataBinder.Eval(Container.DataItem, "ShowCreateJob") == true) ? "Yes" : "No"%>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                 <telerik:GridTemplateColumn HeaderText="Default the Create Run Checkbox to Checked" DataField="CreateJobChecked">
                    <ItemTemplate>
                        <%# ((bool)DataBinder.Eval(Container.DataItem, "CreateJobChecked") == true) ? "Yes" : "No"%>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
</fieldset>
<div class="buttonbar">
    <asp:Button ID="btnAddBusinessType" runat="server" Text="Add New Business Type" />
    <asp:button id="btnRefresh" runat="server" text="Refresh" />
</div>

</asp:Content>