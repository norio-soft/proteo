<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="EditAllocationZoneTable.aspx.cs" Inherits="Orchestrator.WebUI.Consortium.EditAllocationZoneTable" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>Edit Allocation Zone Table</h1>
    
    <fieldset>
        <legend>Table</legend>
        <table width="520px">
            <tr>
                <td class="formCellLabel">
                    Table Description
                </td>
                <td class="formCellField">
                    <asp:TextBox ID="txtDescription" CssClass="fieldInputBox" runat="server" Width="250" MaxLength="50" />
                    <asp:RequiredFieldValidator ID="rfvDescription" runat="server" ErrorMessage="Please enter a description for this allocation zone table" ControlToValidate="txtDescription" Display="Dynamic">
                        <img src="/images/Error.png" height="16" width="16" title="Please enter a description for this allocation zone table" />
                    </asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">
                    Zone Map
                </td>
                <td class="formCellField">
                    <asp:Label runat="server" ID="lblZoneMap"></asp:Label>
                </td>
            </tr>
        </table>
    </fieldset>

    <fieldset>
        <legend>Allocated Consortium Members</legend>

        <asp:ListView ID="lvZones" runat="server" DataKeyNames="ZoneID">
            <LayoutTemplate>
                <table>
                    <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
                </table>
            </LayoutTemplate>
            <ItemTemplate>
                <tr>
                    <td><%# Eval("Description") %></td>
                    <td>
                        <telerik:RadComboBox ID="cboConsortiumMember" runat="server" DataValueField="key" DataTextField="value" Width="250" DropDownWidth="350" ShowMoreResultsBox="false" ItemRequestTimeout="500" EnableLoadOnDemand="true">
                            <WebServiceSettings Path="~/ws/combostreamers.asmx" Method="GetSubContractors" />
                        </telerik:RadComboBox>
                    </td>
                </tr>
            </ItemTemplate>
        </asp:ListView>
   </fieldset>
       
    <div class="buttonbar">
        <asp:Button ID="btnSave" runat="server" Text="Save Changes" />
        <asp:Button ID="btnList" runat="server" Text="Table List" CausesValidation="false" OnClientClick="location.href='allocationtablelist.aspx'; return false;" />
    </div>
</asp:Content>
