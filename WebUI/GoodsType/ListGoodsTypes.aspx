<%@ Page Language="C#" MasterPageFile="~/default_tableless.master" AutoEventWireup="true" Inherits="Orchestrator.WebUI.GoodsType.ListGoodsTypes" Codebehind="ListGoodsTypes.aspx.cs" %>

<asp:Content ContentPlaceHolderID="Header" runat="server"></asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>List Goods Types</h1></asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1" >
    <h2>Allows you to view a list of the goods types configured in the system</h2>
    <asp:GridView ID="gvGoodsTypes" runat="server" AllowPaging="true" AllowSorting="false" AutoGenerateColumns="false" PageSize="25" cellspacing="0" cellpadding="3" borderwidth="1" cssclass="Grid" width="850">
        <HeaderStyle cssclass="HeadingRowLite" height="22" verticalalign="middle" HorizontalAlign="left" />
        <RowStyle cssclass="Row" />
        <AlternatingRowStyle backcolor="WhiteSmoke" />
        <SelectedRowStyle cssclass="SelectedRow" />
        <Columns>
            <asp:HyperLinkField HeaderText="Description" DataTextField="Description" DataNavigateUrlFields="GoodsTypeId" DataNavigateUrlFormatString="AddUpdateGoodsType.aspx?goodsTypeId={0}" />
            <asp:BoundField HeaderText="Short Code" DataField="ShortCode" ItemStyle-Width="80" />
            <asp:TemplateField HeaderText="Default (for new clients only)" ItemStyle-Width="300">
                <ItemTemplate>
                    <asp:Label runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Default").ToString().ToLower() == "false" ? "No" : "Yes" %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>  
            <asp:TemplateField HeaderText="Hazardous" ItemStyle-Width="210">
                <ItemTemplate>
                    <asp:Label ID="Label1" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "IsHazardous").ToString().ToLower() == "false" ? "No" : "Yes" %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField> 
        </Columns>
    </asp:GridView>
    <div class="whitespacepusher"></div>
    <div class="buttonbar">
        <asp:Button id="btnAdd" runat="server" Text="Add Goods Type" />
    </div>
</asp:Content>