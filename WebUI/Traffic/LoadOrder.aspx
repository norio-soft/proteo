<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Traffic.LoadOrder" Codebehind="LoadOrder.aspx.cs" MasterPageFile="~/WizardMasterPage.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Load Order</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script language="javascript" src="../script/scripts.js" type="text/javascript"></script>
    <script language="javascript" src="../script/popAddress.js" type="text/javascript"></script>

    <div style="height: 191px; width: 100%; overflow: auto;">
        <asp:gridview id="gvLoadOrder" runat="server" AllowSorting="false" autogeneratecolumns="false" width="100%" enableviewstate="false" cellspacing="0" cellpadding="0" borderwidth="0" cssclass="Grid">
            <headerstyle cssclass="HeadingRowLite" height="22" verticalalign="middle" />
            <rowStyle  cssclass="Row" />
            <AlternatingRowStyle  backcolor="WhiteSmoke" />
            <SelectedRowStyle  cssclass="SelectedRow" />
            <columns>
                <asp:TemplateField HeaderText="From">
                    <ItemTemplate>
                        <span onmouseover='ShowPoint("~/point/getPointAddresshtml.aspx", <%# Eval("CollectPointId") %>);' onmouseout="HidePoint();"><%# Eval("CollectPoint") %></span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="At">
                    <ItemTemplate>
                        <%# ((bool)(DataBinder.Eval(Container.DataItem, "CollectIsAnyTime"))) ? ((DateTime)(DataBinder.Eval(Container.DataItem, "CollectBookedDateTime"))).ToString("dd/MM/yy") + " AnyTime" : ((DateTime)(DataBinder.Eval(Container.DataItem, "CollectBookedDateTime"))).ToString("dd/MM/yy HH:mm")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="To">
                    <ItemTemplate>
                        <span onmouseover='ShowPoint("~/point/getPointAddresshtml.aspx", <%# Eval("DeliveryPointId") %>);' onmouseout="HidePoint();"><%# Eval("DeliveryPoint") %></span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="At">
                    <ItemTemplate>
                        <%# ((bool)(DataBinder.Eval(Container.DataItem, "DeliveryIsAnyTime"))) ? ((DateTime)(DataBinder.Eval(Container.DataItem, "DeliveryBookedDateTime"))).ToString("dd/MM/yy") + " AnyTime" : ((DateTime)(DataBinder.Eval(Container.DataItem, "DeliveryBookedDateTime"))).ToString("dd/MM/yy HH:mm")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField HeaderText="Docket Number" DataField="DocketNumber" />
                <asp:BoundField HeaderText="Weight" DataField="Weight" />
            </columns>
        </asp:gridview>
    </div>
    
    <div id="divPointAddress" style="z-index=5;display:none; background-color:Wheat;padding:2px 2px 2px 2px;">
		<table style="background-color: white; border:solid 1pt black; " cellpadding="2">
			<tr>
				<td><span id="spnPointAddress"></span></td>
			</tr>
		</table>
	</div>
	
</asp:Content>