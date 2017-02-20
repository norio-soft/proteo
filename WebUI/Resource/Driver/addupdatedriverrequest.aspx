<%@ Page language="c#" Inherits="Orchestrator.WebUI.Resource.Driver.AddUpdateDriverRequest" Codebehind="AddUpdateDriverRequest.aspx.cs" MasterPageFile="~/default_tableless.master"   Title="Driver Request" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Driver Request</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

	<h2>Configure driver requests below.</h2>
	
	<div class="formPageContainer">
	    <fieldset>
		    <asp:Label id="lblConfirmation" runat="server" CssClass="Confirmation"></asp:Label>
		    <table>
			    <tr>
				    <td class="formCellLabel">For driver:</td>
				    <td class="formCellField" colspan="3">
                        <telerik:RadComboBox ID="cboDrivers" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                            MarkFirstMatch="true" ShowMoreResultsBox="true" Skin="Orchestrator" Width="355px">
                        </telerik:RadComboBox>
					    <asp:RequiredFieldValidator id="rfvDriver" runat="server" Display="Dynamic" ControlToValidate="cboDrivers" ErrorMessage="Please specify which driver is making the request."><img src="../../images/newMasterPage/icon-warning.png" height="16" width="16" title="Please specify which driver is making the request." /></asp:RequiredFieldValidator>
					    <asp:CustomValidator id="cfvDriver" runat="server" Display="Dynamic" ControlToValidate="cboDrivers" EnableClientScript="False" ErrorMessage="Please specify which driver is making the request."><img src="../../images/newMasterPage/icon-warning.png" height="16" width="16" title="Please specify which driver is making the request." /></asp:CustomValidator>
				    </td>
			    </tr>
			    <tr>
				    <td class="formCellLabel">The date the request applies to:</td>
				    <td class="formCellField">
					    <table cellpadding="0px" cellspacing="0">
						    <tr>
							    <td><telerik:RadDatePicker id="dteRequestDate" runat="server" ToolTip="The date to attach this request to." Width="100px">
                                <DateInput runat="server"
                                dateformat="dd/MM/yy">
                                </DateInput>
                                </telerik:RadDatePicker></td>
							    <td><asp:RequiredFieldValidator id="rfvRequestDate" runat="server" Display="Dynamic" ControlToValidate="dteRequestDate" ErrorMessage="The date to attach this request to."><img src="../../images/newMasterPage/icon-warning.png" height="16" width="16" title="The date to attach this request to." /></asp:RequiredFieldValidator></td>
						    </tr>
					    </table>
				    </td>
				    <td colspan="2">&nbsp;</td>
			    </tr>
			    <tr>
				    <td class="formCellLabel">Request information:</td>
				    <td class="formCellField" colspan="3">
					    <asp:TextBox id="txtRequestText" runat="server" TextMode="MultiLine" Columns="40" Rows="6"></asp:TextBox>
					    <asp:RequiredFieldValidator id="rfvRequestText" runat="server" Display="Dynamic" ControlToValidate="txtRequestText" ErrorMessage="Please supply the request text."><img src="../../images/newMasterPage/icon-warning.png" height="16" width="16" title="Please supply the request text." /></asp:RequiredFieldValidator>
				    </td>
			    </tr>
		    </table>
	    </fieldset>
	    <div class="buttonbar">
			<asp:Button id="btnListRequests" runat="server" Text="List Requests"></asp:Button>&nbsp;<asp:Button id="btnAdd" runat="server" Text="Add Request"></asp:Button>
		</div>
    </div>
</asp:Content>