<%@ Page Title="" Language="C#" MasterPageFile="~/WizardMasterPage.Master" AutoEventWireup="true" CodeBehind="ChangeDepot.aspx.cs" Inherits="Orchestrator.WebUI.Traffic.ChangeDepot" %>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Change Planning Depot</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table>
        <tr><td colspan="2"><h1>Change Planning Depot for Run</h1></td></tr>
        <tr>
            <td>Current Depot</td><td><asp:Label ID="lblCurrentDepot" runat="server"></asp:Label></td>
        </tr>
        <tr>
            <td>New Depot</td><td><asp:DropDownList ID="cboControlArea" runat="server"></asp:DropDownList></td>
        </tr>
      <%--  <tr>
            <td>Change only this Leg or Whole Run</td>
            <td><asp:RadioButtonList runat="server" ID="rblType">
                <asp:ListItem Text="Only this Leg" Value="Leg"></asp:ListItem>
                <asp:ListItem Text="Whole Run" Value="Run" Selected="True"></asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>--%>
    </table>
     <div class="buttonbar">
        <asp:Button ID="btnUpdate" runat="server" Text="Update"/>
        <asp:Button ID="btnCancel" runat="server" Text="Close" />
    </div>
</asp:Content>
