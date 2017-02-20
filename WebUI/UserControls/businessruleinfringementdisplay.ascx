<%@ Control Language="c#" Inherits="Orchestrator.WebUI.UserControls.BusinessRuleInfringementDisplay" Codebehind="BusinessRuleInfringementDisplay.ascx.cs" %>
<table border="0" width="100%" cellpadding="1" cellspacing="0" class="ControlErrorMessage">
    <asp:Repeater id="repBusinessRuleInfringements" runat="server">
	    <HeaderTemplate>
		    <tr>
			    <td><b>You are unable to do that, please review the following messages and retry.</b></td>
		    </tr>
	    </HeaderTemplate>
	    <ItemTemplate>
		    <tr>
			    <td style="color:Red;padding-left:10px;"><li><%# DataBinder.Eval(Container.DataItem, "Description") %></li></td>
		    </tr>
	    </ItemTemplate>
</asp:Repeater>
</table>
