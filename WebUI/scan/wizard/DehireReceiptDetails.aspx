<%@ Page Language="C#" MasterPageFile="~/WizardMasterPage.Master" AutoEventWireup="true" CodeBehind="DehireReceiptDetails.aspx.cs" Inherits="Orchestrator.WebUI.scan.wizard.DehireReceiptDetails" Title="Untitled Page" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div>
    <fieldset>
        <legend> Please save the Dehire Receipt </legend>
        <table>
            <tr>
			    <td class="formCellLabel">Dehire Receipt Number</td>
			    <td class="formCellField">
			        <asp:TextBox ID="txtDehireReceiptNumber" runat="server" Width="100px" Text='<%= this.DehireReceiptNumber.ToString() %>' />
			        <asp:RequiredFieldValidator ID="rfvDeHireReceipt" runat="server" ControlToValidate="txtDehireReceiptNumber" Display="Dynamic" ValidationGroup="scanDehire" ErrorMessage="Please enter Dehire Reciept Number.">
                        <img id="Img4" runat="server" alt="" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter Dehire Reciept Number." />
                    </asp:RequiredFieldValidator>
			    </td>
		    </tr>
            <tr>
                <td colspan="2">
                    <br/>
                    Click Save to attach the Dehire Receipt to Order Number: <%=this.OrderId.ToString() %>
                </td>
            </tr>
        </table>
    </fieldset>
    <div class="buttonbar">
        <input type="button" id="btnBack" value="< Back" onclick="javascript:history.go(-1);" />
        <asp:button runat="server" id="btnNext" text="Save" ValidationGroup="scanDehire"></asp:button>
        &nbsp;&nbsp;
        <input type="button" value="Cancel" onclick="window.close();" />	
    </div>
</div>
</asp:Content>