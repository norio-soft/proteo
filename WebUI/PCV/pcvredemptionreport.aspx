<%@ Reference Control="~/usercontrols/reportviewer.ascx" %>
<%@ Page language="c#" Inherits="Orchestrator.WebUI.PCV.PCVRedemptionReport" Codebehind="PCVRedemptionReport.aspx.cs" MasterPageFile="~/default_tableless.Master" %>
<%@ OutputCache Location="None" %>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>
<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h2><asp:Label ID="h1Text" runat="server"></asp:Label></h2></asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2><asp:Label ID="h2Text" runat="server"></asp:Label></h2>
    <fieldset>
        <legend>Report Parameters</legend>
		<table>
			<tr>
				<td class="formCellLabel">Client</td>
				<td class="formCellField" colspan="2">
                    <telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                        MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" AllowCustomText="false"
                        ShowMoreResultsBox="false" Skin="WindowsXP" Width="355px">
                    </telerik:RadComboBox>
					<asp:RequiredFieldValidator id="rfvClient" runat="server" Enabled="False" EnableClientScript="False" ControlToValidate="cboClient" ErrorMessage="Please supply a client to report on."><img src="../images/Error.gif" height="16" width="16" title="Please supply a client to report on." /></asp:RequiredFieldValidator>
				</td>
			</tr>
			<tr>
				<td class="formCellLabel">Start Date</td>
				<td class="formCellField">
					<telerik:RadDatePicker id="dteDateFrom" runat="server" Width="100">
                    <DateInput runat="server"
                    dateformat="dd/MM/yy">
                    </DateInput>
                    </telerik:RadDatePicker>
				</td>
				<td class="formCellField">
					<asp:CustomValidator id="cfvStartDate" runat="server" OnServerValidate="cfvStartDate_ServerValidate" ControlToValidate="dteDateFrom" ErrorMessage="The start date must be before the end date."><img src="../images/Error.gif" height="16" width="16" title="The start date must be before the end date." /></asp:CustomValidator>
				</td>
			</tr>
			<tr>
				<td class="formCellLabel">End Date</td>
				<td class="formCellField">
					<telerik:RadDatePicker id="dteDateTo" Width="100" runat="server">
                    <DateInput runat="server"
                    dateformat="dd/MM/yy">
                    </DateInput>
                    </telerik:RadDatePicker>
				</td>
			</tr>
			<tr>
				<td class="formCellField" colspan="3">
					<asp:Label id="lblError" EnableViewState="False" cssclass="ControlErrorMessage" visible="False" runat="server"/>
				</td>
			</tr>
		</table>
	</fieldset>
	<div class="buttonBar">
		<nfvc:NoFormValButton  id="btnGenerateReport" runat="server" Text="Generate Report"/>
		<nfvc:NoFormValButton  id="btnReset" runat="server" Text="Reset"/>
	</div>
	<uc1:ReportViewer id="reportViewer" runat="server" EnableViewState="False" Visible="False" ViewerWidth="100%" ViewerHeight="800"></uc1:ReportViewer>
    <script language="javascript" type="text/javascript">
    <!--
	   /* function ClearQueryString()
	    {
		    alert("ClearQueryString()");
		    document.all.Form1.action = "PCVRedemptionReport.aspx";
		    alert(document.all.Form1.action);
	    }*/

	    function OpenPCVWindowForEdit(pcvId)
	    {
            // OBSOLETE - PCVs are no longer supported, and the code that was called from this method is no longer functional... user should not end up here any longer anyway.
	    }
    	
	    if ('<%=m_pcvId%>' != '0')
		    OpenPCVWindowForEdit('<%=m_pcvId%>');
    //-->
    </script>  
</asp:Content>
