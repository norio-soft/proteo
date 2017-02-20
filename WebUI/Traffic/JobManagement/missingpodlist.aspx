<%@ Reference Control="~/usercontrols/reportviewer.ascx" %>
<%@ Page language="c#" MasterPageFile="~/default_tableless.master" Inherits="Orchestrator.WebUI.Traffic.JobManagement.MissingPODList" Codebehind="MissingPODList.aspx.cs" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>
<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>
<%@ Register Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI" TagPrefix="cc1" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Missing POD List</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<h2>Missing PODs are displayed below</h2>
    <cc1:Dialog ID="dlgDocumentWizard" URL="/scan/wizard/ScanOrUpload.aspx" Width="550" Height="550" AutoPostBack="true" Mode="Modal"
    runat="server" ReturnValueExpected="true">
    </cc1:Dialog>

	<fieldset>
		<legend>Missing POD List</legend>

		<table>
			<tr>
				<td class="formCellLabel">Client</td>
				<td class="formCellField">
                    <telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                        MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" 
                        ShowMoreResultsBox="true" Skin="WindowsXP" Width="355px">
                    </telerik:RadComboBox>
					&nbsp;
					<asp:RequiredFieldValidator id="rfvClient" runat="server" ControlToValidate="cboClient" ErrorMessage="Please supply a client to report on."><img src="../../images/Error.gif" height="16" width="16" title="Please supply a client to report on." /></asp:RequiredFieldValidator>
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
			                <td>&nbsp;&nbsp;<asp:RequiredFieldValidator id="rfvDateFrom" runat="server" Display="Dynamic" ControlToValidate="dteStartDate" ErrorMessage="Please enter the start date. "><img src="../../images/Error.gif" height='16' width='16' title='Please enter the start date.'></asp:RequiredFieldValidator></td>
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
                            <td>&nbsp;&nbsp;<asp:RequiredFieldValidator id="rfvDateTo" runat="server" Display="Dynamic" ControlToValidate="dteEndDate" ErrorMessage="Please enter the end date."><img src="../../images/Error.gif" height='16' width='16' title='Please enter the end date.'></asp:RequiredFieldValidator></td>
			            </tr>
			        </table>
			    </td>
			</tr>
			<tr>
				<td class="formCellInput" colspan="2" style="padding-top:10px">
					<asp:Label ID="lblReportError" Runat="server" CssClass="ControlErrorMessage" Text="No missing PODs found for client" Visible="False"/>
				</td>
			</tr>
		</table>
	</fieldset>
	<div class="buttonbar">
	    <nfvc:NoFormValButton id="btnReport" runat="server" Text="Generate Report"></nfvc:NoFormValButton>
	    <input type="reset" value="Reset">
	</div>


    <asp:GridView ID="gvMissingPOD" runat="server" AllowSorting="true" AutoGenerateColumns="false" Width="100%" EnableViewState="true" CellSpacing="0" CellPadding="0" CssClass="Grid">
        <HeaderStyle CssClass="HeadingRowLite" Height="22" VerticalAlign="middle" />
        <RowStyle CssClass="Row" />
        <AlternatingRowStyle BackColor="WhiteSmoke" />
        <SelectedRowStyle CssClass="SelectedRow" />
        <Columns>
            <asp:TemplateField HeaderStyle-Width="30" >
                <ItemTemplate>
                    <a href="javascript:OpenPODWindow(<%#Eval("JobID") %>,<%#Eval("CollectDropID")%>,'scan',<%#(Eval("PODReturnComment") == System.DBNull.Value || Eval("PODReturnComment") == string.Empty)  ? "''" : "'" + Eval("PODReturnComment") + "'"%>)">Scan</a>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Run ID" SortExpression="JobId">
                <ItemTemplate>
                    <a href="javascript:openJobDetailsWindow(<%#Eval("JobId") %>)"><%#Eval("JobId") %></a>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Order ID" SortExpression="JobId">
                <ItemTemplate>
                    <a href="javascript:openOrderProfile(<%#Eval("OrderID") %>)"><%#Eval("OrderID") %></a>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="Client" HeaderText="Client" />
            <asp:BoundField DataField="DeliveryName" HeaderText="Delivery Name" SortExpression="DeliveryName" />
            <asp:BoundField DataField="DeliveryTown" HeaderText="Delivery Town" SortExpression="DeliveryTown" />
            <asp:BoundField DataField="DeliveryDate" HeaderText="Delivery Date" DataFormatString="{0:dd/MM/yy HH:mm}" SortExpression="DeliveryDate" />
            <asp:BoundField DataField="LoadNo" HeaderText="Load Number" SortExpression="LoadNo" />
            <asp:BoundField DataField="DocketNumber" HeaderText="Docket Number" SortExpression="DocketNumber" />
            <asp:BoundField DataField="QuantityOrdered" HeaderText="Qty Ordered" SortExpression="QuantityOrdered" />
            <asp:BoundField DataField="PalletsStated" HeaderText="Plts Stated" SortExpression="PalletsStated" />
            <asp:BoundField DataField="Driver" HeaderText="Driver" SortExpression="Driver" />
        </Columns>
    </asp:GridView>
	
	
	<uc1:ReportViewer id="reportViewer" runat="server" Visible="False"></uc1:ReportViewer>
	<script language="javascript" type="text/javascript">
	<!--
        function openJobDetailsWindow(jobId)
        {
            openDialogWithScrollbars('../../Job/Job.aspx?wiz=true&jobId=' + jobId + getCSID(), '1220', '800');
        }
        function openOrderProfile(orderID) {
            window.open("../../groupage/ManageOrder.aspx?OID=" + orderID);
        }

        function OpenPODWindow(jobId, collectDropId, mode, podReturnComment) {
            var podFormType = 2;
            var qs = "ScannedFormTypeId=" + podFormType;
            qs += "&JobId=" + jobId;
            qs += "&CollectDropId=" + collectDropId;
            qs += "&PODReturnComment=" + encodeURIComponent(podReturnComment);
            var dlgmode = (mode != undefined ? mode : "scan");
            qs += "&PODDlgMode=" + dlgmode;

            <%=dlgDocumentWizard.ClientID %>_Open(qs);
        }
    //-->
	</script>
</asp:Content>