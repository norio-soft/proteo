<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Invoicing.UninvoicedValueForDateRange" Codebehind="UninvoicedValueForDateRange.aspx.cs" MasterPageFile="~/default_tableless.Master" Title="Uninvoiced Total for Date Range" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>Uninvoiced Total for Date Range</h1>
    <h2>Enter the date range below</h2>
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
	    <asp:Button id="btnReport" runat="server" Text="Generate Report"></asp:Button>
	    <asp:Button id="btnExportToCSV" runat="server" text="Export To CSV"></asp:Button>
    </div>
	<asp:GridView ID="gvResults" runat="server" AllowSorting="true" gridlines="vertical" autogeneratecolumns="false" cssclass="Grid" width="50%" visible="true">
        <headerstyle cssclass="HeadingRow" height="25" verticalalign="middle"/>
        <rowStyle height="20" cssclass="Row" />
        <AlternatingRowStyle height="20" backcolor="WhiteSmoke" />
        <SelectedRowStyle height="20" cssclass="SelectedRow" />
        <columns>
            <asp:BoundField HeaderText="Client" DataField="OrganisationName" ItemStyle-Width="50%" />
            <asp:BoundField HeaderText="Number of Jobs" DataField="JobCount" ItemStyle-Width="25%" ItemStyle-HorizontalAlign="Right" />
            <asp:BoundField HeaderText="Total Amount" DataField="TotalAmount" DataFormatString="{0:C}" HtmlEncode="False" ItemStyle-Width="25%" ItemStyle-HorizontalAlign="Right" />
        </columns>
    </asp:GridView>
</asp:Content>