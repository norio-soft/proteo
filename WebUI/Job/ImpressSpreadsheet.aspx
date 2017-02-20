<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Job.ImpressSpreadsheet" Codebehind="ImpressSpreadsheet.aspx.cs" MasterPageFile="~/default_tableless.Master" Title="Impress Spreadsheet" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server"> 
    <h1>Impress Spreadsheet</h1>
    <h2>Generate the impress spreadsheet</h2>
    <fieldset>
        <legend>Filter Options</legend>
        <table>
            <tr>
                <td class="formCellLabel">Start Date</td>
                <td class="formCellField">
                    <telerik:RadDateInput ID="dteStartDate" runat="server" dateformat="dd/MM/yy"
                        ToolTip="The earliest date to report on.">
                    </telerik:RadDateInput>
                </td>
                <td class="formCellField">
                    <asp:requiredfieldvalidator id="rfvStartDate" runat="server" controltovalidate="dteStartDate"
                        errormessage="Please specify a start date." display="Dynamic"><img src="../images/Error.gif" height="16" width="16" title="Please specify a start date." /></asp:requiredfieldvalidator>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">End Date</td>
                <td class="formCellField">
                    <telerik:RadDateInput ID="dteEndDate" runat="server" dateformat="dd/MM/yy" ToolTip="The last date to report on.">
                    </telerik:RadDateInput>
                </td>
                <td class="formCellField">
                    <asp:requiredfieldvalidator id="rfvEndDate" runat="server" controltovalidate="dteEndDate"
                        errormessage="Please specify an end date." display="Dynamic"><img src="../images/Error.gif" height="16" width="16" title="Please specify an end date." /></asp:requiredfieldvalidator>
                </td>
            </tr>
        </table>
    </fieldset>
    <div class="buttonbar">
	    <asp:Button id="btnExportToCSV" runat="server" text="Export To CSV"></asp:Button>
    </div>
</asp:Content>
