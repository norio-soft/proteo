<%@ Reference Control="~/usercontrols/reportviewer.ascx" %>
<%@ Page language="c#" MasterPageFile="~/default_tableless.master" Inherits="Orchestrator.WebUI.POD.outstandingPods" Codebehind="outstandingPods.aspx.cs" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>
<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>
<%@ Register Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI" TagPrefix="cc1" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Outstanding POD List</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<h2>Outstanding PODs are displayed below</h2>
<cc1:Dialog ID="dlgDocumentWizard" URL="/scan/wizard/ScanOrUpload.aspx" Width="550" Height="550" AutoPostBack="true" Mode="Modal"
    runat="server" ReturnValueExpected="true">
</cc1:Dialog>
	<fieldset>
		<legend>Outstanding POD List</legend>

		<table>
            <tr>
                <td class="formCellLabel">
                        Search for
                    </td>
                <td>
                    
                <asp:TextBox ID="txtSearchFor" runat="server" Width="350"></asp:TextBox>
                </td>
                <td>
                <asp:CheckBoxList ID="cblSearchFor" runat="server" RepeatDirection="horizontal">
                    <asp:ListItem Text="Order ID" Value="ORDERID" Selected="false"></asp:ListItem>
                    <asp:ListItem Text="Customer Order Number (Load No)" Value="LOADNO" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="Delivery Order Number (Docket No)" Value="DOCKETNO" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="Run ID" Value="JOBID"></asp:ListItem>
                </asp:CheckBoxList>
                </td>
            </tr>
			<tr>
				<td class="formCellLabel">Driver/Subby</td>
				<td class="formCellField">
                    <telerik:RadComboBox ID="cboDriver" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                        MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" 
                        ShowMoreResultsBox="true" Width="355px" Height="200px">
                        <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetAllDriversAndSubbys" />
                    </telerik:RadComboBox>
					<%--<asp:RequiredFieldValidator id="rfvClient" runat="server" ControlToValidate="cboClient" ErrorMessage="Please supply a client to report on."><img src="../../images/Error.gif" height="16" width="16" title="Please supply a client to report on." /></asp:RequiredFieldValidator>--%>
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
                    <asp:CheckBox id="chkIncludeCheckedInPODs" runat="server" selected="false" Text="Include Checked-In PODs" ToolTip="Include Checked-In PODs"/>
			    </td>

			</tr>
            <tr>
                
				<td class="formCellInput" colspan="2" style="padding-top:10px">
                    <asp:CheckBox id="chkIncludeResourced" runat="server" selected="false" Text="Include Resourced Orders" ToolTip="Order has been added to a run and instructions resourced but not called-in"/>
			    </td>
                
			</tr>
			<tr>
                
				<td class="formCellInput" colspan="2" style="padding-top:10px">
					<asp:Label ID="lblReportError" Runat="server" CssClass="ControlErrorMessage" Text="No outstanding PODs found" Visible="False"/>
			    </td>
                
			</tr>
            </table>
	</fieldset>
	<div class="buttonbar">
	    <nfvc:NoFormValButton id="btnReport" runat="server" Text="Generate Report"></nfvc:NoFormValButton>
	    <asp:Button ID="btnReset" runat="Server" Text="Reset" />
	</div>
	
	<telerik:RadGrid runat="server" ID="gvOutstandingPOD" AllowPaging="false" AllowSorting="true" AutoGenerateColumns="false" AllowMultiRowSelection="true" Width="100%" >
        <MasterTableView GroupLoadMode="Client" CommandItemStyle-BackColor="White">
            <Columns>
                <telerik:GridTemplateColumn UniqueName="lnkScan" HeaderStyle-Width="30" Resizable="false">
                    <ItemTemplate>
                        <a href="javascript:OpenPODWindow(<%#Eval("JobID") %>,<%#Eval("CollectDropID")%>,'scan',<%#(Eval("PODReturnComment") == System.DBNull.Value || Eval("PODReturnComment") == string.Empty)  ? "''" : "'" + Eval("PODReturnComment") + "'"%>)">Scan</a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="lnkJob" HeaderText="Run ID" HeaderStyle-Width="50" Resizable="false">
                    <ItemTemplate>
                        <a href="javascript:ViewJob(<%#Eval("JobID") %>)"><%#Eval("JobID") %></a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="lnkOrder" HeaderText="Order ID" HeaderStyle-Width="60" Resizable="false">
                    <ItemTemplate>
                        <a href="javascript:viewOrderProfile(<%#Eval("OrderID") %>)"><%#Eval("OrderID") %></a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="Client" HeaderText="Client" SortExpression="Client" HeaderStyle-Width="200"/>
                <telerik:GridBoundColumn DataField="DeliverTo" HeaderText="Deliver To" SortExpression="DeliverTo" HeaderStyle-Width="250"/>
                <telerik:GridBoundColumn DataField="DeliveryDate" HeaderText="Delivery Date" DataFormatString="{0:dd/MM/yy HH:mm}" SortExpression="DeliveryDate" />
                <telerik:GridBoundColumn DataField="LoadNo" HeaderText="Load Number" SortExpression="LoadNo" />
                <telerik:GridBoundColumn DataField="DocketNumber" HeaderText="Docket Number" SortExpression="DocketNumber" />
                <telerik:GridBoundColumn DataField="Weight" HeaderText="Weight" SortExpression="Weight"  />
                <telerik:GridBoundColumn DataField="PalletsStated" HeaderText="No Pallets" SortExpression="PalletsStated" />
                <telerik:GridBoundColumn DataField="Driver" HeaderText="Driver" SortExpression="Driver" />
                <telerik:GridBoundColumn DataField="PODReceivedDate" HeaderText="POD Received Date" SortExpression="PODReceivedDate" DefaultInsertValue="Not Yet Received" />
                <telerik:GridBoundColumn DataField="PODReceiverName" HeaderText="POD Receiver Name" SortExpression="PODReceiverName" DefaultInsertValue="Not Yet Received" />
                <telerik:GridBoundColumn DataField="PODReturnComment" HeaderText="Comment"/>
                <telerik:GridTemplateColumn UniqueName="PODReceived"  HeaderText="POD Received" SortExpression="PODReceived"  HeaderStyle-Width="30" Resizable="false">
                    <ItemTemplate>
                        <a href="javascript:OpenPODWindow(<%#Eval("JobID") %>,<%#Eval("CollectDropID")%>,'manual',<%#(Eval("PODReturnComment") == System.DBNull.Value || Eval("PODReturnComment") == string.Empty)  ? "''" : "'" + Eval("PODReturnComment") + "'"%>)"><%#Eval("PODReceived") %></a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings AllowColumnsReorder="true" ReorderColumnsOnClient="true" AllowGroupExpandCollapse="true" ColumnsReorderMethod="Reorder" >
            <Resizing AllowColumnResize="false" AllowRowResize="false" EnableRealTimeResize="false" ResizeGridOnColumnResize="false" ClipCellContentOnResize="true" />
        </ClientSettings>
    </telerik:RadGrid>
	</br>
	<uc1:ReportViewer id="reportViewer" runat="server" Visible="False"></uc1:ReportViewer>
	<script language="javascript" type="text/javascript">
	<!--
	    function viewOrderProfile(orderID) {
	        var url = "/Groupage/ManageOrder.aspx?wiz=true&oID=" + orderID;

	        var wnd = window.open(url, "Order", "width=1180, height=900, resizable=1, scrollbars=1");
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
        
	    function ViewJob(jobID) {

	        var url = "/job/job.aspx?jobId=" + jobID + getCSID();

            window.open(url);
        }       
    //-->
	</script>
</asp:Content>