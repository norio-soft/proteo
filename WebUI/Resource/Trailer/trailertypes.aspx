<%@ Page Language="C#" MasterPageFile="~/default_tableless.master" AutoEventWireup="true" CodeBehind="trailertypes.aspx.cs" Inherits="Orchestrator.WebUI.Resource.Trailer.trailertypes" Title="Trailer Types" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Trailer Types</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<asp:UpdatePanel ID="pnlTrailerTypes" runat="server" >

    <ContentTemplate>             
    
    <h2>Please note that you can add, or amend existing Trailer types, but you cannot remove Trailer types.</h2>
        <fieldset class="invisiableFieldset" runat="server" id="fsTrailerTypes">
            <telerik:RadGrid ID="grdTrailerTypes" runat="server" Skin="Orchestrator" AutoGenerateColumns="false">
                <MasterTableView DataKeyNames="TrailerTypeId, Description, CreateDate, CreateUserID, LastUpdateDate, LastUpdateUserID">
                    <Columns>
                        <telerik:GridButtonColumn HeaderText="Trailer Type" DataTextField="Description" ButtonType="LinkButton" CommandName="select" />
                        <telerik:GridBoundColumn DataField="CreateDate" HeaderText="Created" DataFormatString="{0:dd/MM/yy}"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="CreateUserID" HeaderText="Created By"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="LastUpdateDate" HeaderText="Last Updated" DataFormatString="{0:dd/MM/yy}"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="LastUpdateUserID" HeaderText="Last Updated By"></telerik:GridBoundColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
            <div class="buttonbar">
                <asp:Button ID="btnAdd" runat="server" Text="Add Trailer Type" />
                <asp:button id="btnRefresh" runat="server" text="Refresh" />    
            </div>
        </fieldset> 
        <fieldset class="invisiableFieldset" runat="server" id="fsTrailerType" visible="false">
            <fieldset>
                <legend>Update Trailer type</legend>
                <table>
                    <tr>
                        <td class="formCellLabel">Description</td>
                        <td class="formCellField"><asp:HiddenField ID="hidTrailerTypeID" runat="server" /> <asp:TextBox ID="txtDescription" CssClass="fieldInputBox" runat="server" Width="255" MaxLength="1024"></asp:TextBox><asp:RequiredFieldValidator ID="rfvDescription" runat="server" ErrorMessage="Please enter a description" ControlToValidate="txtDescription"></asp:RequiredFieldValidator></td>
                    </tr>
                    <tr>
                        <td colspan="2" style="padding: 5px 5px 5px 15px"><asp:Label ID="lblCreated" runat="server"></asp:Label>&nbsp;&nbsp;&nbsp<asp:Label ID="lblUpdated" runat="server"></asp:Label></td>
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
