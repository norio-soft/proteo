<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="vehicleClasses.aspx.cs" Inherits="Orchestrator.WebUI.Resource.VehicleClass.vehicleClasses" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>
        Vehicle Classes
    </h1>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Header" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


<telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" LoadingPanelID="RadAjaxLoadingPanel1">
    <telerik:RadGrid Skin="Orchestrator" runat="server" ID="grdVehicleClasses" AutoGenerateColumns="false" OnNeedDataSource="grdVehicleClasses_NeedDataSource" OnUpdateCommand="grdVehicleClasses_UpdateCommand" OnInsertCommand="grdVehicleClasses_InsertCommand" AllowAutomaticUpdates="true" >
        <MasterTableView DataKeyNames="VehicleClassId, ClassDescription" CommandItemDisplay="Top" >
        <CommandItemSettings ShowAddNewRecordButton="true" AddNewRecordText="Add New Vehicle Class" ShowRefreshButton="false" />
            <CommandItemTemplate>
                <asp:Button runat="server" class="buttonClassSmall" CommandName="InitInsert"  Text="Add New Vehicle Class" />
            </CommandItemTemplate>
            <Columns>                
                <telerik:GridEditCommandColumn>
                <ItemStyle Width="10%" />
                </telerik:GridEditCommandColumn>
                
                <telerik:GridTemplateColumn ReadOnly="true" HeaderText="Vehicle Class Id" DataField="VehicleClassId" UniqueName="VehicleClassId">
                </telerik:GridTemplateColumn>

                <telerik:GridTemplateColumn HeaderText="Vehicle Class" DataField="ClassDescription"  UniqueName="ClassDescription">
                    <ItemTemplate>
                        <asp:Label ID="ClassDescription" runat="server" Text='<%# Bind("ClassDescription") %>' />
                    </ItemTemplate>
                    <InsertItemTemplate>
                        <telerik:RadTextBox ID="VehicleClassDescNew" runat="server" Text='<%# Eval("ClassDescription") %>' Width="150px" />
                    </InsertItemTemplate>
                    <EditItemTemplate>
                        <telerik:RadTextBox ID="VehicleClassDescEdit" runat="server" Text='<%# Eval("ClassDescription") %>' Width="150px" />
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