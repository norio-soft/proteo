<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="manufacturers.aspx.cs" Inherits="Orchestrator.WebUI.administration.manufacturers" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>
        Manufacturers
    </h1>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Header" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


<telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="RadAjaxLoadingPanel1">
    <telerik:RadGrid runat="server" Skin="Orchestrator" ID="grdManufacturers" AutoGenerateColumns="false" OnNeedDataSource="grdManufacturers_NeedDataSource" OnUpdateCommand="grdManufacturers_UpdateCommand" OnInsertCommand="grdManufacturers_InsertCommand" AllowAutomaticUpdates="true" >
        <MasterTableView DataKeyNames="VehicleManufacturerId, Description" CommandItemDisplay="Top" CommandItemStyle-BackColor="White" TableLayout="Auto" > 
            <RowIndicatorColumn>
                <HeaderStyle Width="20px"></HeaderStyle>
            </RowIndicatorColumn>       
        <CommandItemSettings ShowAddNewRecordButton="true" AddNewRecordText="Add New Manufacturer" ShowRefreshButton="false"/>
            <CommandItemTemplate>
                <asp:Button runat="server" class="buttonClassSmall" CommandName="InitInsert"  Text="Add New Manufacturer" />
            </CommandItemTemplate>
            <Columns>                
                <telerik:GridEditCommandColumn>
                <ItemStyle Width="10%" />
                </telerik:GridEditCommandColumn>
                
                <telerik:GridTemplateColumn ReadOnly="true" HeaderText="Manufacturer Id" DataField="VehicleManufacturerId" UniqueName="VehicleManufacturerId">
                </telerik:GridTemplateColumn>

                <telerik:GridTemplateColumn HeaderText="Manufacturer" DataField="Description"  UniqueName="Description">
                    <ItemTemplate>
                        <asp:Label ID="ManufacturerDescription" runat="server" Text='<%# Bind("Description") %>' />
                    </ItemTemplate>
                    <InsertItemTemplate>
                        <telerik:RadTextBox ID="ManufacturerDescNew" runat="server" Text='<%# Eval("Description") %>' Width="150px" />
                    </InsertItemTemplate>
                    <EditItemTemplate>
                        <telerik:RadTextBox ID="ManufacturerDescEdit" runat="server" Text='<%# Eval("Description") %>' Width="150px" />
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