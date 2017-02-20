<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Job.UpdateDockets" Codebehind="updateDockets.aspx.cs" MasterPageFile="~/WizardMasterPage.Master" Title="Haulier Enterprise" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Add/Update Dockets</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <div>
    
        <asp:panel ID="Panel1" runat="server" Visible="false">
            <div style="margin-top: 10px;  font-family: Trebuchet MS, Arial, Helvetica;    font-size: 1em;    font-weight: bold;	background-color:#FDE8E9;	border:2px solid #9E0B0E;	padding:2px; 	color: #9E0B0E;">
                <img src="../images/status-red.gif"  align="middle"/><asp:Label ID="Label1" runat="server"></asp:Label>
            </div>
        </asp:panel>
        
        <table>
            <tr>
                <td>Update the Load No for all of the Orders Below to </td><td><asp:TextBox ID="txtLoadNo" runat="server"></asp:TextBox></td><td><asp:Button ID="btnUpdateLoadNo" runat="server" Text="Update Load No" /></td>
            </tr>
        </table>
        
        <asp:GridView ID="gvDockets" runat="server" CssClass="Grid" AutoGenerateColumns="false" ShowFooter="true" Width="100%" >
             <headerstyle cssclass="HeadingRow" height="25" verticalalign="middle"/>
                        <rowStyle height="20" cssclass="Row" />
                        <AlternatingRowStyle height="20" backcolor="WhiteSmoke" />
                        <SelectedRowStyle height="20" cssclass="SelectedRow" />
            <Columns>
                <asp:TemplateField HeaderText="Load No">
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "CustomerOrderNumber") %></ItemTemplate>
                    <EditItemTemplate><asp:TextBox ID="txtLoadNo" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CustomerOrderNumber") %>' Width="75"></asp:TextBox><asp:RequiredFieldValidator ID="rfvLoadNo" ControlToValidate="txtLoadNo" runat="server" ErrorMessage="<img src='../images/error.png' title='Please enter a Load Number'/>" Display="dynamic"></asp:RequiredFieldValidator>
                    <asp:HiddenField ID="hidOrderID" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "OrderID") %>' />
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Docket No">
                    <ItemTemplate><%# DataBinder.Eval(Container.DataItem, "DeliveryOrderNumber") %></ItemTemplate>
                    <EditItemTemplate><asp:TextBox ID="txtDocketNo" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "DeliveryOrderNumber") %>' Width="75"></asp:TextBox><asp:RequiredFieldValidator ID="rfvDocket" ControlToValidate="txtDocketNo" runat="server" ErrorMessage="<img src='../images/error.png' title='Please enter a Docket Number'/>" Display="dynamic"></asp:RequiredFieldValidator>
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:CommandField ButtonType="Link" UpdateText="Update" CancelText="Cancel" ShowEditButton="true" ShowDeleteButton="false" ShowInsertButton="false" />
            </Columns>
        </asp:GridView>
        
    </div>
    
    <div class="buttonbar">
        <asp:Button ID="btnClose" runat="server" Text="Close" visible="true" CausesValidation="false"/>
    </div>
    
</asp:Content>