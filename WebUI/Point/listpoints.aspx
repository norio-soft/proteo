<%@ Page Language="c#" masterpagefile="~/default_tableless.master" Inherits="Orchestrator.WebUI.Point.ListPoints" Title="Points for Organisation" CodeBehind="ListPoints.aspx.cs" %>
<%@ Register Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI" TagPrefix="cc1" %>
<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Points for Organisation</h1></asp:Content>

<asp:content id="Content1" contentplaceholderid="ContentPlaceHolder1" runat="server">
    <cc1:Dialog ID="dlgManagePoint" URL="addupdatepoint2.aspx" Width="675" Height="670" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true" ></cc1:Dialog>
    <script type="text/javascript">
        
        function openManagePoint(pointID)
        {
            var qs = "pointId=" + pointID + "&identityId=" + getClientIdentityID() + "&clientName="+getClientName() + "&allowclose=true";
            // add randmo string to stop caching issues
            qs += "&__rs=" + randomString();
            <%=dlgManagePoint.ClientID %>_Open(qs);
        }


        function getClientIdentityID() {
            var cboClient = $find("<%= cboClient.ClientID %>");
            return cboClient == null ? null : (cboClient.get_value() || null);
        }

        function getClientName() {
            var cboClient = $find("<%= cboClient.ClientID %>");
            return cboClient == null ? null : (cboClient.get_text() || null);
        }

        function randomString() 
        {
            var chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXTZabcdefghiklmnopqrstuvwxyz";
            var string_length = 8;
            var randomstring = '';
            for (var i=0; i<string_length; i++) {
	            var rnum = Math.floor(Math.random() * chars.length);
	            randomstring += chars.substring(rnum,rnum+1);
            }
            return randomstring;
        }
    </script>
    <h2>Please Choose an Organisation</h2>
	<fieldset>
		<legend>Client Choice</legend>
		<table>
			<tr>
				<td>
                    <telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                        MarkFirstMatch="true" RadControlsDir="~/script/RadControls/"
                        ShowMoreResultsBox="true" Skin="WindowsXP" Width="355px">
                    </telerik:RadComboBox>
					<asp:RequiredFieldValidator id="rfvClient" runat="server" ControlToValidate="cboClient" ErrorMessage="Please select a client." Display="Dynamic"><img src="../images/error.gif" alt="Please select a client." /></asp:RequiredFieldValidator>
				</td>
				<td><asp:button CssClass="buttonClass" id="btnRefresh" runat="server" text="Select" onclick="btnRefresh_Click"></asp:button></td>
			</tr>
		</table>
	</fieldset>
	<br/>
	<asp:panel id="pnlPoints" runat="server" visible="false">
		<h3>Points for Organisation</h3>
		<asp:datagrid id="dgPoints" runat="server" OnPageIndexChanged="dgPoints_Page" PagerStyle-HorizontalAlign="Right"
			PagerStyle-Mode="NumericPages" cssclass="DataGridStyle" border="1" backcolor="white" cellpadding="2"
			pagesize="20" AllowPaging="False" AllowSorting="True" AutoGenerateColumns="False" width="100%">
			<Columns>
				<asp:TemplateColumn HeaderText="Description" SortExpression="Description">
					<ItemTemplate>
						<a href='#' onclick="openManagePoint(<%# DataBinder.Eval(Container.DataItem,"PointId")%>);">
							<%#DataBinder.Eval(Container.DataItem, "Description")%>
						</a>
					</ItemTemplate>
				</asp:TemplateColumn>
				<asp:BoundColumn DataField="Town" HeaderText="Town" SortExpression="Town"></asp:BoundColumn>
				<asp:BoundColumn DataField="Collect" HeaderText="Is Collection Point" SortExpression="Collect" Visible="false"></asp:BoundColumn>
				<asp:BoundColumn DataField="Deliver" HeaderText="Is Delivery Point" SortExpression="Deliver" Visible="false"></asp:BoundColumn>
			</Columns>
			<pagerstyle cssclass="DataGridListPagerStyle"></pagerstyle>
			<itemstyle cssclass="DataGridListItem"></itemstyle>
			<headerstyle cssclass="DataGridListHead"></headerstyle>
			<alternatingitemstyle cssclass="DataGridListItemAlt"></alternatingitemstyle>
		</asp:datagrid>
		<div class="buttonbar">
            <button onclick="openManagePoint(0);" class="buttonClass">Add New Point</button>
		</div>
	</asp:panel>
</asp:content>
