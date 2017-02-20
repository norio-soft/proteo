<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/default_tableless.master" Inherits="Orchestrator.WebUI.GoodsType.AddUpdateGoodsType" Codebehind="AddUpdateGoodsType.aspx.cs" %>

<asp:Content ContentPlaceHolderID="Header" runat="server"></asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Configure Goods Type</h1></asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1" >
    <h2>Allows you to configure a new or existing type of goods</h2>
    <fieldset>
        <legend>Goods Type Information</legend>
        <table>
            <tr>
                <td class="formCellLabel">Description</td>
                <td class="formCellField">
                    <asp:TextBox id="txtDescription" runat="server" MaxLength="100"></asp:TextBox>
                    <asp:RequiredFieldValidator id="rfvDescription" runat="server" ControlToValidate="txtDescription" ErrorMessage="Please supply a description." Display="Dynamic"><img src="../images/Error.gif" height="16" width="16" title="Please supply a description." /></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">Short Code</td>
                <td class="formCellField"><asp:TextBox id="txtShortCode" runat="server" MaxLength="2" Size="1" ToolTip="The short code is used to highlight the type of goods being collected or delivered when you are planning, if you don't want anything to be displayed, leave this field blank."></asp:TextBox></td>
            </tr>
            <tr>
                <td class="formCellLabel">Default (for new clients only)</td>
                <td class="formCellField"><asp:CheckBox runat="server" ID="chkDefault" />  </td>
            </tr>
            <tr>
                <td class="formCellLabel">Is this goods type hazardous?</td>
                <td class="formCellField"><asp:CheckBox runat="server" ID="chkHazardous" />  </td>                
            </tr>
        </table>
    </fieldset>
    <div class="buttonbar">
        <asp:Button id="btnAdd" runat="server" Text="Add Goods Type" />
        <asp:Button id="btnReturn" runat="server" Text="Return to List" CausesValidation="false" />
    </div>
</asp:Content>