<%@ Page language="c#" Inherits="Orchestrator.WebUI.Reports.ReportViewer" MasterPageFile="~/WizardMasterPage.master" title="Report Viewer" Codebehind="ReportViewer.aspx.cs" %>
<%@ MasterType TypeName="Orchestrator.WebUI.WizardMasterPage" %>
<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>
<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<uc1:ReportViewer id="reportViewer" runat="server"></uc1:ReportViewer>
</asp:Content>