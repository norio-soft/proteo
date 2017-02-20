<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Job.SubContractedHistory" Codebehind="SubContractedHistory.aspx.cs" MasterPageFile="~/default_tableless.Master" Title="Sub-Contracted History" %>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>

<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>
<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>	
<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>Sub-Contracted History</h1>
    <h2>A list of historical runs that have been sub-contracted over a date range.</h2>
    <fieldset>
	    <legend>Sub-Contracted History</legend>
	    <table>
		    <tr>
			    <td class="formCellLabel">Sub-Contractor</td>
			    <td class="formCellField">
                    <telerik:RadComboBox ID="cboSubContractor" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                        MarkFirstMatch="true" RadControlsDir="~/script/RadControls/"
                        ShowMoreResultsBox="false" Skin="WindowsXP" Width="355px" AllowCustomText="false" Height="300px">
                    </telerik:RadComboBox>
				    <asp:Label ID="lblError" cssclass="ControlErrorMessage" EnableViewState="False" Visible="False" Runat="server"/>
			    </td>
		    </tr>
			<tr>
			    <td class="formCellLabel">Start Date</td>
			    <td class="formCellField">
			        <table border="0" cellpadding="0" cellspacing="0">
			            <tr>
			                <td><telerik:RadDatePicker id="dteStartDate" Width="100" runat="server">
                            <DateInput runat="server"
                            dateformat="dd/MM/yy">
                            </DateInput>
                            </telerik:RadDatePicker></td>
			                <td><asp:RequiredFieldValidator id="rfvDateFrom" runat="server" Display="Dynamic" ControlToValidate="dteStartDate" ErrorMessage="Please enter the start date. "></asp:RequiredFieldValidator></td>
			            </tr>
			        </table>
			    </td>
			</tr>
			<tr>
			    <td class="formCellLabel">End Date</td>
			    <td class="formCellField">
			        <table border="0" cellpadding="0" cellspacing="0">
			            <tr>
    		                <td><telerik:RadDatePicker id="dteEndDate" Width="100" runat="server">
                            <DateInput runat="server"
                            dateformat="dd/MM/yy">
                            </DateInput>
                            </telerik:RadDatePicker></td>
                            <td><asp:RequiredFieldValidator id="rfvDateTo" runat="server" Display="Dynamic" ControlToValidate="dteEndDate" ErrorMessage="Please enter the end date."><img height='16' width='16' title='Please enter the end date.'></asp:RequiredFieldValidator></td>
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
        <nfvc:NoFormValButton id="btnExportToCSV" runat="server" text="Export To CSV"></nfvc:NoFormValButton>
        <asp:Button ID="btnReset" Runat="server" Text="Reset" CausesValidation="False"></asp:Button>
    </div>
    <telerik:RadAjaxLoadingPanel Visible="true" ID="LoadingPanel1" IsSticky="false" runat="server">
        <img alt="Loading" src='<%= RadAjaxLoadingPanel.GetWebResourceUrl(Page, "Telerik.Web.UI.Skins.Default.Ajax.loading.gif") %>' />
    </telerik:RadAjaxLoadingPanel>

     <telerik:RadAjaxManager ID="ramSubContractorHistory" runat="server" EnableHistory="True">
        <AjaxSettings >
            <telerik:AjaxSetting AjaxControlID="btnExportToCSV">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlContentWrapper" LoadingPanelID="LoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            
        </AjaxSettings>
    </telerik:RadAjaxManager>


	<uc1:ReportViewer id="reportViewer" runat="server" Visible="False"></uc1:ReportViewer>

</asp:Content>
