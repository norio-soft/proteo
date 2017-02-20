<%@ Page Language="C#" MasterPageFile="~/default_tableless.master" AutoEventWireup="true" CodeBehind="ExtrasRevenue.aspx.cs" Inherits="Orchestrator.WebUI.Reports.ExtrasRevenue" Title="Untitled Page" %>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>Extras Revenue</h1>
    <fieldset>
	    <legend>Filter Options</legend>
	    <table>
	        <tr>
	            <td class="formCellLabel">Client</td>
	            <td class="formCellField">
                    <telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500" MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" AllowCustomText="False" ShowMoreResultsBox="false" Skin="WindowsXP" Width="355px" Height="300px" SelectOnTab="false" />
	            </td>
	        </tr>
	        <tr>
	            <td class="formCellLabel">Start Date</td>
	            <td class="formCellField">
	                <telerik:RadDateInput ID="rdiStartDate" runat="server" Width="70px" DateFormat="dd/MM/yy" />&nbsp;<asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ControlToValidate="rdiStartDate" ValidationGroup="grpDisplayReport" ErrorMessage="Please enter a start date" />
	            </td>
	        </tr>
	        <tr>
	            <td class="formCellLabel">End Date</td>
	            <td class="formCellField">
	                <telerik:RadDateInput ID="rdiEndDate" runat="server" Width="70px" DateFormat="dd/MM/yy" />&nbsp;<asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ControlToValidate="rdiEndDate" ValidationGroup="grpDisplayReport" ErrorMessage="Please enter an end date" />
	            </td>
	        </tr>
	    </table>
    </fieldset>
    <div class="buttonbar">
        <asp:Button id="btnDisplayReport" runat="server" Text="Display Report" ValidationGroup="grpDisplayReport" />
    </div>
    
    <div style="padding-top:25px; border-style:none;">
        <asp:Label ID="lblError" runat="server" Visible="false" />
    </div>
    	    	        
	<div>
        <uc1:ReportViewer id="reportViewer" runat="server" Visible="False"></uc1:ReportViewer>
	</div>

</asp:Content>