<%@ Control Language="c#" Inherits="Orchestrator.WebUI.UserControls.ReportViewer" Codebehind="ReportViewer.ascx.cs" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>
<telerik:RadCodeBlock runat="server" ID="RadCodeBlock1">
<script language="javascript" type="text/javascript">

    function HandleEmailChanged(item) {
        var txtEmail = document.getElementById("<%=txtEmail.ClientID %>");

        if (item.Value != "")
            txtEmail.Text = item.Value;
        else
            txtEmail.Text = item.Text;
    }

    function HandleFaxNumberChanged(item) {
        var txtFaxNumber = document.getElementById("<%=txtFaxNumber.ClientID %>");

        if (item.Value != "")
            txtFaxNumber.Text = item.Value;
        else
            txtFaxNumber.Text = item.Text;
    }
     
</script>		
</telerik:RadCodeBlock>
<div>
	<table width="100%" border="0" cellpadding="1" cellspacing="0">
		<tr>
			<td align="left" valign="top" rowspan="2" width="50%">
				<asp:Label ID="lblConfirmation" runat="server" CssClass="confirmation" Visible="False"></asp:Label>
			</td>
			<td class="formCellLabel">
				Fax Number:
			</td>
			<td>  
				<telerik:RadComboBox ID="cboFaxNumber" runat="server" HighlightTemplatedItems="true" Width="350px" 
				OnClientSelectedIndexChanged="HandleFaxNumberChanged" AllowCustomText="true" MarkFirstMatch="true">
                    <HeaderTemplate>
                        <table style="width:345px; text-align:left;">
                            <tr><th style="width:65%;">Contact Name</th><th style="width:35%;">Contact Detail</th></tr>
                        </table>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <table style="width:345px; text-align:left;">
                            <tr>
                                <td style="width:65%;">
                                    <%#DataBinder.Eval(Container.DataItem, "ContactName")%>
                                </td>
                                <td style="width:35%;">
                                    <%#DataBinder.Eval(Container.DataItem, "ContactDetail")%>
                                </td>
                            </tr>
                        </table>
                    </ItemTemplate>
                </telerik:RadComboBox>
            
				<asp:TextBox ID="txtFaxNumber" Runat="server" style="display:none"/>
			</td>
			<td>			
				<asp:RequiredFieldValidator id="rfvFaxNumber" runat="server" Display="Dynamic" EnableClientScript="true" ControlToValidate="cboFaxNumber" ValidationGroup="Fax"></asp:RequiredFieldValidator>
				<asp:RegularExpressionValidator id="revFaxNumber" runat="server" Display="Dynamic" EnableClientScript="true" ControlToValidate="txtFaxNumber" ValidationGroup="Fax"></asp:RegularExpressionValidator>
			</td>
			<td>
				<nfvc:NoFormValButton id="btnFaxReport"  runat="server" NoFormValList="rfvEmailAddress,revEmailAddress" Text="Fax this Report" CssClass="buttonClass" ValidationGroup="Fax" Width="140px"></nfvc:NoFormValButton>
			</td>
		</tr>
		<tr>
			<td class="formCellLabel">
				Email Address:
			</td>
			<td> 				
				<telerik:RadComboBox ID="cboEmail" runat="server" Skin="Orchestrator" HighlightTemplatedItems="true" Width="350px" 
				OnClientSelectedIndexChanged="HandleEmailChanged" AllowCustomText="true" MarkFirstMatch="true">
                    <HeaderTemplate>
                        <table style="width:345px; text-align:left;">
                            <tr><th style="width:50%;">Contact Name</th><th style="width:50%;">Contact Detail</th></tr>
                        </table>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <table style="width:345px; text-align:left;">
                            <tr>
                                <td style="width:50%;">
                                    <%#DataBinder.Eval(Container.DataItem, "ContactName")%>
                                </td>
                                <td style="width:50%;">
                                    <%#DataBinder.Eval(Container.DataItem, "ContactDetail")%>
                                </td>
                            </tr>
                        </table>
                    </ItemTemplate>
                </telerik:RadComboBox>
                
                <asp:TextBox ID="txtEmail" Runat="server" style="display:none"/>
			</td>
			<td>
				<asp:RequiredFieldValidator id="rfvEmailAddress" runat="server" Display="Dynamic" EnableClientScript="true" ControlToValidate="cboEmail" ValidationGroup="Email"></asp:RequiredFieldValidator>
				<asp:RegularExpressionValidator id="revEmailAddress" runat="server" Display="Dynamic" EnableClientScript="true" ControlToValidate="txtEmail" ValidationGroup="Email"></asp:RegularExpressionValidator>
			</td>
			<td>
				<nfvc:NoFormValButton id="btnEmailReport" CssClass="buttonClass" runat="server" NoFormValList="rfvFaxNumber,revFaxNumber" Text="Email this Report" ValidationGroup="Email" Width="140px"></nfvc:NoFormValButton>
			</td>
		</tr>
		<tr align="right" style="display: none">
			<td colspan="4">
				<asp:Button ID="btnExcelReport" Runat="server" CssClass="buttonClass" CausesValidation="False" Text="Excel Version"></asp:Button>
			</td>
		</tr>
	</table>
</div>
<iframe runat="server" width="100%" height="1000" id="iReport" src=""></iframe>

