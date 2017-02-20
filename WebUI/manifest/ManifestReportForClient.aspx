<%@ Page Title="Manifest Report For Client" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="ManifestReportForClient.aspx.cs" Inherits="Orchestrator.WebUI.manifest.ManifestReportForClient" %>

<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components" %>
<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1><%= Page.Title %></h1>
    <h2>A report of the instructions for a specified client and date range</h2>
    <fieldset>
	    <legend>Report On</legend>
	    <table>
		    <tr>
			    <td class="formCellLabel">Client</td>
			    <td class="formCellField">
                    <telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                        MarkFirstMatch="true" RadControlsDir="~/script/RadControls/"
                        ShowMoreResultsBox="false" Skin="WindowsXP" Width="355px" AllowCustomText="false" Height="300px">
                    </telerik:RadComboBox>
                    <asp:RequiredFieldValidator id="rfvClient" runat="server" Display="Dynamic" ControlToValidate="cboClient" ErrorMessage="Please select the client."></asp:RequiredFieldValidator>
				    <asp:Label ID="lblError" runat="server" cssclass="ControlErrorMessage" EnableViewState="False" Visible="false" />
			    </td>
		    </tr>
			<tr>
			    <td class="formCellLabel">Start Date</td>
			    <td class="formCellField">
			        <table border="0" cellpadding="0" cellspacing="0">
			            <tr>
			                <td><telerik:RadDateInput id="dteStartDate" runat="server" dateformat="dd/MM/yy"></telerik:RadDateInput></td>
			                <td><asp:RequiredFieldValidator id="rfvDateFrom" runat="server" Display="Dynamic" ControlToValidate="dteStartDate" ErrorMessage="Please enter the start date."></asp:RequiredFieldValidator></td>
			            </tr>
			        </table>
			    </td>
			</tr>
			<tr>
			    <td class="formCellLabel">End Date</td>
			    <td class="formCellField">
			        <table border="0" cellpadding="0" cellspacing="0">
			            <tr>
    		                <td><telerik:RadDateInput id="dteEndDate" runat="server" dateformat="dd/MM/yy"></telerik:RadDateInput></td>
                            <td><asp:RequiredFieldValidator id="rfvDateTo" runat="server" Display="Dynamic" ControlToValidate="dteEndDate" ErrorMessage="Please enter the end date."></asp:RequiredFieldValidator></td>
			            </tr>
			        </table>
			    </td>
			</tr>
		    <tr>
			    <td colspan="2">
				    
			    </td>
		    </tr>
	    </table>
    </fieldset>	    

    <div class="buttonbar">
        <nfvc:NoFormValButton id="btnReport" runat="server" Text="Generate Report"></nfvc:NoFormValButton>
        <asp:Button ID="btnReset" Runat="server" Text="Reset" CausesValidation="False"></asp:Button>
    </div>

	<uc1:ReportViewer id="reportViewer" runat="server" Visible="False"></uc1:ReportViewer>
</asp:Content>
