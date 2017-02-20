<%@ Page language="c#" Inherits="Orchestrator.WebUI.Traffic.JobManagement.LoadReleaseNotification" Codebehind="LoadReleaseNotification.aspx.cs" MasterPageFile="~/WizardMasterPage.Master" Title="Haulier Enterprise" %>

<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>
<%@ Register TagPrefix="uc" Namespace="Orchestrator.WebUI.Controls" Assembly="WebUI" %>
<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Load Release Notification</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

	<fieldset>
	    <legend><strong>Collections for this job</strong></legend>
	    <table>
		    <tr>
			    <td>	
				    <asp:datagrid id="dgJobCollections" runat="server" AutoGenerateColumns="False" AllowSorting="True" AllowPaging="True"
				    pagesize="20" width="100%" cellpadding="2" backcolor="White" border="1" cssclass="DataGridStyle"
				    PagerStyle-Mode="NumericPages" PagerStyle-HorizontalAlign="Right" OnItemDataBound="dgJobCollections_ItemDataBound">
					    <AlternatingItemStyle CssClass="DataGridListItemAlt"></AlternatingItemStyle>
					    <ItemStyle CssClass="DataGridListItem"></ItemStyle>
					    <HeaderStyle CssClass="DataGridListHead"></HeaderStyle>
					    <Columns>
						    <asp:BoundColumn DataField="JobId" HeaderText="Job Id"></asp:BoundColumn>
						    <asp:BoundColumn DataField="LoadNo" HeaderText="Load No"></asp:BoundColumn>
						    <asp:BoundColumn DataField="Client" HeaderText="Client"></asp:BoundColumn>
						    <asp:BoundColumn DataField="Destination" HeaderText="Destination"></asp:BoundColumn>
						    <asp:BoundColumn DataField="Delivery Time" HeaderText="Delivery Time" DataFormatString="{0:HH:mm}"></asp:BoundColumn>
						    <asp:BoundColumn DataField="Driver" HeaderText="Driver"></asp:BoundColumn>
						    <asp:BoundColumn DataField="RegNo" HeaderText="Vehicle Reg"></asp:BoundColumn>
						    <asp:BoundColumn DataField="TrailerRef" HeaderText="Trailer Ref"></asp:BoundColumn>
						    <asp:TemplateColumn HeaderText="Fax Load Release Notification">
							    <ItemTemplate>
								    <uc:RdoBtnGrouper id="rbgCollection" runat="server" GroupName="Collection" OnCheckedChanged="rbgCollection_CheckedChanged" Visible="True" AutoPostBack="True" CausesValidation="false"></uc:RdoBtnGrouper>
							    </ItemTemplate>
						    </asp:TemplateColumn>
						    <asp:BoundColumn DataField="InstructionId" Visible="False"/>
					    </Columns>
				    </asp:datagrid>
			    </td>
		    </tr>
		</table>
	</fieldset>
			
	<asp:Panel id="pnlEnterInformation" runat="server" Visible="False">
    	<fieldset>
			<legend><strong>Information to be included on Load Release Fax</strong></legend>
	        <table>
		        <tr>
			        <td>To:</td>
			        <td><asp:TextBox id="txtTo" runat="server"/></td>
		        </tr>
		        <tr>	
			        <td>From:</td>
			        <td><asp:TextBox id="txtFrom" runat="server"/></td>
		        </tr>
		        <tr>
			        <td>Ref:</td>
			        <td><asp:TextBox id="txtRef" runat="server"/>
		        </tr>
            </table>	
		</fieldset>
	</asp:Panel>
			
	<div class="buttonbar" align="left">
		<nfvc:NoFormValButton id="btnReport" runat="server" Text="Generate Load Release Notification" Visible="True"></nfvc:NoFormValButton>
	</div>
        
    <uc1:ReportViewer id="reportViewer" runat="server" ViewerWidth="100%" ViewerHeight="800" EnableViewState="False"></uc1:ReportViewer>
    
</asp:Content>