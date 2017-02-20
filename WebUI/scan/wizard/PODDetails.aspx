<%@ Page Language="C#" MasterPageFile="~/WizardMasterPage.master" EnableEventValidation="false" AutoEventWireup="true" CodeBehind="PODDetails.aspx.cs" Inherits="Orchestrator.WebUI.scan.wizard.PODDetails" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div> 
        <fieldset>
        <legend> Please enter POD information </legend>
        <table width="100%">
		    <tr valign=top>
			    <td>
				    <asp:Label runat="server" id="lblTicketNo" text="Ticket Number"></asp:Label>
			    </td>
			    <td>
				    <asp:TextBox id="txtTicketNo" runat="server" ></asp:TextBox>
			    </td>
		    </tr>
		    <tr valign=top>
			    <td>
				    <asp:Label runat="server" id="lblSignatureDate" text="Signature Date"></asp:Label>
			    </td>
			    <td>
				    <telerik:RadDateInput id="dteSignatureDate" runat="server" dateformat="dd/MM/yy" MinDate="1900-01-01" ></telerik:RadDateInput>
                    <asp:RequiredFieldValidator ID="rfvSignatureDate" runat="server" ControlToValidate="dteSignatureDate" Display="Dynamic" 
                      ErrorMessage="Please enter a valid signature date."><img src="/images/Error.gif" height="16" width="16" title="Please enter a valid signature date." /></asp:RequiredFieldValidator></td>
		    </tr>
		</table>
        <asp:Panel ID="pnlReferences" runat="server" Height="300" ScrollBars="Auto" >
            <fieldset>
                <legend>Additional References</legend>
                <asp:Button ID="btnAddReference" runat="server" Text="Add Reference" Height="100%"/>
                <br/>
	                <asp:DataGrid ID="dgAdditionalReferences" runat="server" AutoGenerateColumns="false">
	                    <Columns>
	                        <asp:TemplateColumn HeaderText="Reference" HeaderStyle-Width="150px" HeaderStyle-Font-Bold="true">
	                            <ItemTemplate>
	                                <%# (Container.DataItem as string).ToString() %>
	                            </ItemTemplate>
	                            <EditItemTemplate>
	                                <asp:TextBox ID="txtReference" runat="server" MaxLength="50" Width="120px"></asp:TextBox>
	                            </EditItemTemplate>
	                            <ItemStyle VerticalAlign="Middle" />
	                        </asp:TemplateColumn>
	                        <asp:EditCommandColumn ButtonType="LinkButton" EditText="Edit" UpdateText="Update" CancelText="Cancel" ItemStyle-VerticalAlign="Middle"></asp:EditCommandColumn>
	                        <asp:ButtonColumn ButtonType="LinkButton" CommandName="Delete" Text="Delete"></asp:ButtonColumn>
	                    </Columns>
	                </asp:DataGrid>
            </fieldset>
            </asp:Panel>
        </fieldset>
        <div class="buttonbar">
            <asp:Button runat="server" id="btnBack" text="< Back" causesvalidation="False" />
            <asp:button runat="server" id="btnNext" text="Save"></asp:button>
            &nbsp;&nbsp;
            <input type="button" value="Cancel" onclick="window.close();" />	
        </div>
    </div> 
</asp:Content>
