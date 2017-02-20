<%@ Page Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="vehicletypes.aspx.cs" Inherits="Orchestrator.WebUI.Resource.Vehicle.vehicletypes" Title="Vehicle Types" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:UpdatePanel ID="pnlVehicleTypes" runat="server" >
        <ContentTemplate>                
            <h1>Vehicle Types</h1>
            <h2>Please note that you can add, or amend existing vehicle types, but you cannot remove vehicle types.</h2>
            <fieldset runat="server" class="invisiableFieldset" id="fsVehicleTypes">
                <telerik:RadGrid ID="grdVehicleTypes" runat="server" Skin="Orchestrator" AutoGenerateColumns="false">
                    <MasterTableView DataKeyNames="VehicleTypeID, Description, CreateDate, CreateUserID, LastUpdateDate, LastUpdateUserID">
                        <Columns>
                            <telerik:GridButtonColumn HeaderText="Vehicle Type" DataTextField="Description" ButtonType="LinkButton" CommandName="select" />
                            <telerik:GridBoundColumn DataField="CreateDate" HeaderText="Created" DataFormatString="{0:dd/MM/yy}"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="CreateUserID" HeaderText="Created By"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="LastUpdateDate" HeaderText="Last Updated" DataFormatString="{0:dd/MM/yy}"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="LastUpdateUserID" HeaderText="Last Updated By"></telerik:GridBoundColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
                <div class="buttonbar">
                    <asp:Button ID="btnAdd" runat="server" Text="Add Vehicle Type" />
                    <asp:button id="btnRefresh" runat="server" text="Refresh" />    
                </div>
            </fieldset>
            <fieldset runat="server" id="fsVehicleType" class="invisiableFieldset" visible="false">
                <fieldset>
                    <legend>Update Vehicle type</legend>
                    <table>
                        <tr>
                            <td class="formCellLabel">Description</td>
                            <td class="formCellField"><asp:HiddenField ID="hidVehicleTypeID" runat="server" /><asp:TextBox ID="txtDescription" CssClass="fieldInputBox" runat="server" Width="255" MaxLength="1024"></asp:TextBox><asp:RequiredFieldValidator ID="rfvDescription" runat="server" ErrorMessage="Please enter a description" ControlToValidate="txtDescription"></asp:RequiredFieldValidator></td>
                        </tr>
                        <tr>
                            <td colspan="2" style="padding: 5px 5px 5px 15px"><asp:Label ID="lblCreated" runat="server"></asp:Label>&nbsp;&nbsp;&nbsp;<asp:Label ID="lblUpdated" runat="server"></asp:Label></td>
                        </tr>
                    </table>
                </fieldset>
                <div class="buttonbar">
                    <asp:Button ID="btnUpdate" runat="server" Text="Update" />
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" CausesValidation="false" />
                </div>
            </fieldset>
        </ContentTemplate>   
    </asp:UpdatePanel>
</asp:Content>
