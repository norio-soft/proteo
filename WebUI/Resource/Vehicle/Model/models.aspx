<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="models.aspx.cs" Inherits="Orchestrator.WebUI.administration.models" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>
        Models
    </h1>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Header" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="RadAjaxLoadingPanel1">
    <telerik:RadGrid runat="server" ID="grdModels" Skin="Orchestrator" AutoGenerateColumns="false" OnItemDataBound="grdModels_OnItemDataBound" OnNeedDataSource="grdModels_NeedDataSource" OnUpdateCommand="grdModels_UpdateCommand" OnInsertCommand="grdModels_InsertCommand" AllowAutomaticUpdates="true" >
        <MasterTableView DataKeyNames="VehicleModelId, VehicleManufacturerId, ModelDescription, ManufacturerDescription" CommandItemDisplay="Top" >        
        <CommandItemSettings ShowAddNewRecordButton="true" AddNewRecordText="Add New Model" ShowRefreshButton="false" />
            <CommandItemTemplate>
                <asp:button runat="server" class="buttonClassSmall" CommandName="InitInsert"  Text="Add New Model" />
            </CommandItemTemplate>
            <Columns>                
                <telerik:GridEditCommandColumn>
                <ItemStyle Width="10%" />
                </telerik:GridEditCommandColumn>
                
                <telerik:GridTemplateColumn ReadOnly="true" HeaderText="Model Id" DataField="VehicleModelId" UniqueName="VehicleModelId">
                </telerik:GridTemplateColumn>

                <telerik:GridTemplateColumn ReadOnly="true" HeaderText="Manufacturer Id" DataField="VehicleManufacturerId" UniqueName="VehicleManufacturerId" >
                </telerik:GridTemplateColumn>

                <telerik:GridTemplateColumn HeaderText="Manufacturer" DataField="ManufacturerDescription"  UniqueName="ManufacturerDescription">
                    <ItemTemplate>
                        <asp:Label ID="ManufacturerCurrent" runat="server" Text='<%# Bind("ManufacturerDescription") %>' />
                    </ItemTemplate>
                    <InsertItemTemplate>
                        <telerik:RadComboBox ID="ManufacturerNew" runat="server" Width="150px" AutoPostBack="true" EnableLoadOnDemand="true" DataTextField="ManufacturerDescription" DataValueField="VehicleManufacturerId" OnItemsRequested="ManufacturerDropDown_ItemsRequested"  >
                        </telerik:RadComboBox>
                    </InsertItemTemplate>
                    <EditItemTemplate>
                        <telerik:RadComboBox ID="ManufacturerEdit" runat="server" Width="150px" AutoPostBack="true" DataTextField="ManufacturerDescription" DataValueField="VehicleManufacturerId" >
                        </telerik:RadComboBox>
                    </EditItemTemplate>
                </telerik:GridTemplateColumn>

                <telerik:GridTemplateColumn HeaderText="Model" DataField="ModelDescription"  UniqueName="ModelDescription">
                    <ItemTemplate>
                        <asp:Label ID="ModelDescription" runat="server" Text='<%# Eval("ModelDescription") %>' />
                    </ItemTemplate>
                    <InsertItemTemplate>
                        <telerik:RadTextBox ID="ModelDescriptionNew" runat="server" Text='<%# Eval("ModelDescription") %>' Width="150px" />
                    </InsertItemTemplate>
                    <EditItemTemplate>
                        <telerik:RadTextBox ID="ModelDescriptionEdit" runat="server" Text='<%# Eval("ModelDescription") %>' Width="150px" />
                    </EditItemTemplate>
                </telerik:GridTemplateColumn>

            </Columns>
        </MasterTableView>
        <ClientSettings>
            <Selecting AllowRowSelect="true" />
        </ClientSettings>
    </telerik:RadGrid>    
</telerik:RadAjaxPanel>
<telerik:RadWindowManager ID="RadWindowManager1" runat="server" />
<telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" IsSticky="true" runat="server" />
</asp:Content>