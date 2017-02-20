<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="NichollsSubContractorExport.aspx.cs" Inherits="Orchestrator.WebUI.Reports.NichollsSubContractorExport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Nicholls Sub-Contractor Export</h1></asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<div>

    <fieldset>
        <legend>Nicholls SubContractor Report</legend>
        <div style="width:250px;">
            <div class="formCellLabel" style="width:70px; float:left;">From Date</div>
            <div class="formCellField">
                <telerik:RadDateInput ID="rdiStartDate" runat="server" DateFormat="dd/MM/yy" ToolTip="The start date for the filter." Width="60px" />
                <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ControlToValidate="rdiStartDate" ValidationGroup="grpRefresh">
                    <img src="/images/Error.gif" height="16" width="16" title="Please enter a Start Date." alt="" />
                </asp:RequiredFieldValidator>
            </div>
        </div>

        <div class="clearDiv"></div>

        <div style="width:250px;">
            <div class="formCellLabel" style="width:70px; float:left;">To Date</div>
            <div class="formCellField">
                <telerik:RadDateInput ID="rdiEndDate" runat="server" DateFormat="dd/MM/yy" ToolTip="The end date for the filter." Width="60px" />
                <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ControlToValidate="rdiEndDate" ValidationGroup="grpRefresh">
                    <img src="/images/Error.gif" height="16" width="16" title="Please enter an End Date." alt="" />
                </asp:RequiredFieldValidator>
            </div>
        </div>

        <div class="clearDiv"></div>

        <br />

        <div class="buttonBar">
            <asp:Button ID="btnExport" runat="server" Text="Export" ValidationGroup="grpRefresh" />
        </div>
    </fieldset>
</div>

</asp:Content>