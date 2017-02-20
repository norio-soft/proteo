<%@ Reference Control="~/usercontrols/reportviewer.ascx" %>
<%@ Page Language="c#" MasterPageFile="~/default_tableless.master" Inherits="Orchestrator.WebUI.Job.demurrageReport" CodeBehind="demurrageReport.aspx.cs" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components" %>
<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <h1>Demurrage Report</h1>
    <h2><asp:Label ID="lbltitle" runat="server" Text="By selecting a client and date range you can see all the demurrages imposed on a client."></asp:Label></h2>
    <fieldset>
        <legend>Filter Options</legend>
        <table>
            <tr>
                <td class="formCellLabel">Client</td>
                <td class="formCellField">
                    <telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                        MarkFirstMatch="true" ShowMoreResultsBox="false" Width="355px" height="300px"
                        AllowCustomText="false">
                    </telerik:RadComboBox>
                    <asp:RequiredFieldValidator ID="rfvClient" runat="server" ControlToValidate="cboClient"
                        ErrorMessage="Please supply a client to report on."><img src="../images/Error.gif" height="16" width="16" title="Please supply a client to report on." /></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">Start Date</td>
                <td class="formCellField">
                    <telerik:RadDateInput ID="dteStartDate" runat="server" dateformat="dd/MM/yy"
                        ToolTip="The earliest date to report on.">
                    </telerik:RadDateInput>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">End Date</td>
                <td class="formCellField">
                    <telerik:RadDateInput ID="dteEndDate" runat="server" dateformat="dd/MM/yy" ToolTip="The last date to report on.">
                    </telerik:RadDateInput>
                </td>
            </tr>
        </table>
    </fieldset> 
    <div class="buttonbar">
        <nfvc:NoFormValButton ID="btnReport" runat="server" Text="Generate Report"></nfvc:NoFormValButton>
    </div>
    <uc1:ReportViewer id="reportViewer" runat="server" Visible="False">
    </uc1:ReportViewer>  
</asp:Content>
