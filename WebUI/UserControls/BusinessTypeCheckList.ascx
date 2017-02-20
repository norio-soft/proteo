<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BusinessTypeCheckList.ascx.cs" Inherits="Orchestrator.WebUI.UserControls.BusinessTypeCheckList" %>

<asp:ScriptManagerProxy ID="scriptManagerProxy" runat="server">
    <Scripts>
        <asp:ScriptReference Path="~/UserControls/BusinessTypeCheckList.ascx.js" />
    </Scripts>
</asp:ScriptManagerProxy>

<asp:ListView ID="lvBusinessType" runat="server" GroupItemCount="4">
    <LayoutTemplate>
        <table cellpadding="0">
            <tr>
                <td class="checkListCell">
                    <input id="chkBusinessTypeAll" type="checkbox" />
                    <label for="chkBusinessTypeAll">- all -</label>
                </td>
            </tr>
            <asp:PlaceHolder ID="groupPlaceHolder" runat="server" />
        </table>
    </LayoutTemplate>

    <GroupTemplate>
        <tr>
            <asp:PlaceHolder ID="itemPlaceHolder" runat="server" />
        </tr>
    </GroupTemplate>

    <ItemTemplate>
        <td class="checkListCell">
            <input id="chkBusinessType_<%# Eval("BusinessTypeID") %>" type="checkbox" class="chkBusinessType" />
            <label for="chkBusinessType_<%# Eval("BusinessTypeID") %>"><%# Eval("Description") %></label>
        </td>
    </ItemTemplate>
</asp:ListView>

<asp:HiddenField ID="hidBusinessTypeIDs" runat="server" />
