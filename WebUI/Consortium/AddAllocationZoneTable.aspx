﻿<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="AddAllocationZoneTable.aspx.cs" Inherits="Orchestrator.WebUI.Consortium.AddAllocationZoneTable" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>Create New Allocation Zone Table</h1>

    <table>
        <tr>
            <td class="formCellLabel">
                Table Description
            </td>
            <td class="formCellField">
                <asp:TextBox ID="txtDescription" CssClass="fieldInputBox" runat="server" Width="250" MaxLength="50" />
                <asp:RequiredFieldValidator ID="rfvDescription" runat="server" ErrorMessage="Please enter a description for this allocation zone table" ControlToValidate="txtDescription" Display="Dynamic">
                    <img src="/images/Error.png" height="16" width="16" title="Please enter a description for this allocation zone table" alt="*" />
                </asp:RequiredFieldValidator>
            </td>
        </tr>
    </table>
    <br />

    <asp:RadioButton ID="optCopyTable" runat="server" Text="Copy From Table" GroupName="CreateTable" />
    <br />
    <table style="margin-left: 50px;">
        <tr>
            <td class="formCellLabel" style="width: 150px;">
                Existing Table
            </td>
            <td class="formCellField">
                <telerik:RadComboBox runat="server" ID="cboCopyFromTable" Width="250" DataValueField="AllocationZoneTableID" DataTextField="Description" />
            </td>
        </tr>
    </table>
    <br />

    <asp:RadioButton ID="optEmptyTable" runat="server" Text="Empty Table" GroupName="CreateTable" />
    <br />
    <table style="margin-left: 50px;">
        <tr>
            <td class="formCellLabel" style="width: 150px;">
                Zone Map
            </td>
            <td class="formCellField">
                <telerik:RadComboBox runat="server" ID="cboZoneMap" Width="250" DataValueField="ZoneMapID" DataTextField="Description" />
            </td>
        </tr>
    </table>
    <br />

    <div class="buttonbar">
        <asp:Button ID="btnSave" runat="server" Text="Create" />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" CausesValidation="false" OnClientClick="location.href='allocationtablelist.aspx'; return false;" />
    </div>
</asp:Content>
