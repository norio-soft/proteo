<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="wpOutstandingPODS.ascx.cs" Inherits="Orchestrator.WebUI.WebParts.wpOutstandingPODS" %>
    <asp:GridView ID="grdOverview" runat="server" AutoGenerateColumns="false" DataKeyNames="CustomerIdentityID" cellspacing="0" cellpadding="0" borderwidth="0" cssclass="Grid" showfooter="true" showheader="true">
    <headerstyle cssclass="webpartHeadingRow" />
    <rowStyle  cssclass="Row" />
    <AlternatingRowStyle  backcolor="WhiteSmoke" />
    <SelectedRowStyle  cssclass="SelectedRow" />
    <columns>
        <asp:boundfield datafield="OrganisationName" HeaderText="Customer" ItemStyle-Width="240" />
        <asp:TemplateField HeaderText="+4 &#160;">
            <ItemTemplate>
                <asp:LinkButton ID="lnk4Days" OnClick="lnkCustomer_Click" CommandName="4Days" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "CustomerIdentityId")%>' runat="server"><%#DataBinder.Eval(Container.DataItem, "4Days")%></asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="+7 &#160;">
            <ItemTemplate>
                <asp:LinkButton ID="lnk7Days" OnClick="lnkCustomer_Click"  CommandName="7Days" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "CustomerIdentityId")%>' runat="server"><%#DataBinder.Eval(Container.DataItem, "7Days")%></asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="+15 or later &#160;">
            <ItemTemplate>
                <asp:LinkButton ID="lnk15Days" OnClick="lnkCustomer_Click" CommandName="15Days" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "CustomerIdentityId")%>' runat="server"><%#DataBinder.Eval(Container.DataItem, "15Days")%></asp:LinkButton>
            </ItemTemplate>
        </asp:TemplateField>
    </columns>
</asp:GridView>
    

<asp:GridView ID="gvDetails" runat="server" AutoGenerateColumns="false" enableviewstate="false" cellspacing="0" cellpadding="0" borderwidth="0" cssclass="Grid" showfooter="true" showheader="true">
    <headerstyle cssclass="webpartHeadingRow" />
    <rowStyle  cssclass="Row" />
    <AlternatingRowStyle  backcolor="WhiteSmoke" />
    <SelectedRowStyle  cssclass="SelectedRow" />
    <columns>
        <asp:TemplateField HeaderText="Order" ItemStyle-Width="50">
            <ItemTemplate>
                <a target="_blank" href='/groupage/ManageOrder.aspx?oID=<%#DataBinder.Eval(Container.DataItem, "OrderID") %>'><%#DataBinder.Eval(Container.DataItem, "OrderID" )%></a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Scan" ItemStyle-Width="50">
            <ItemTemplate>
                <a id="aScanPOD" runat="server" href="">Scan</a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="ArrivalDateTime" DataFormatString="{0:dd/MM/yy}" HeaderText="Delivery" ItemStyle-Width="75px" />
         <asp:BoundField DataField="Driver" HeaderText="Delivered by" />
    </columns>
</asp:GridView>
<asp:Button ID="btnCancel" runat="server" Text="Return to list" Visible="false" />
<script type="text/javascript" language="javascript">
    function OpenNewPODWindow(jobId, collectDropId) {
        var podFormType = 2;
        var url = '<%=this.ResolveUrl("~/scan/wizard/ScanOrUpload.aspx")%>';
        url += "?ScannedFormTypeId=" + podFormType;
        url += "&JobId=" + jobId;
        url += "&CollectDropId=" + collectDropId;

        openResizableDialogWithScrollbars(url, 550, 500);
    }
</script>
