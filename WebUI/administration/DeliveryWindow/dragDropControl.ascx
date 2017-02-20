<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="dragDropControl.ascx.cs" Inherits="Orchestrator.WebUI.administration.DeliveryWindow.dragDropControl" %>

<asp:Panel ID="pnlRegion" runat="server">
    <fieldset>
        <legend>




         <asp:Label ID="lblRegion" runat="server">X</asp:Label></legend>
        <table>
            <tr>
                <td valign="centre">
                    <asp:Button ID="btnAddSingle" runat="server" Text="->" OnClick="btnAddSingle_Click" />
                    <br />
                     <asp:Button ID="btnRemove" runat="server" Text="<-"  OnClick="btnRemove_Click"/>
                <td>
                    <asp:ListBox ID="lstPostcodes" runat="server" SelectionMode="Multiple" Height ="300px" Width="60px" ></asp:ListBox>
                </td>
            </tr>
           

        </table>

        <asp:Literal ID="region" runat="server" Visible="false"></asp:Literal>
        <asp:TextBox ID="keys" runat="server"  Visible="false"></asp:TextBox>
    </fieldset>
</asp:Panel>
