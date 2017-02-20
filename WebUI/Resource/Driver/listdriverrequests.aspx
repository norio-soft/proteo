<%@ Page language="c#" Inherits="Orchestrator.WebUI.Resource.Driver.ListDriverRequests" Codebehind="ListDriverRequests.aspx.cs" MasterPageFile="~/default_tableless.master"   Title="List Driver Requests" %>
<%@ Register TagPrefix="P1" Namespace="P1TP.Components.Web.UI" Assembly="P1TP.Components" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>List Driver Requests</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <h2>A list of driver requests is shown below.</h2>
    <div class="formPageContainer">	
	    <fieldset>
		    <legend>Driver requests</legend>
		    <table>
			    <tr>
				    <td class="formCellLabel">Show requests that apply between</td>
				    <td class="formCellField">
					    <table cellpadding="0" cellspacing="0">
						    <tr>
							    <td><telerik:RadDatePicker runat="server" ID="dteFilterStartDate" Width="100">
                                <DateInput runat="server"
                                DateFormat="dd/MM/yy">
                                </DateInput>
                                </telerik:RadDatePicker> </td>
							    <td><asp:RequiredFieldValidator id="rfvFilterStartDate" runat="server" Display="Dynamic" ControlToValidate="dteFilterStartDate" ErrorMessage="Please supply a start date"><img src="../../images/Error.gif" height="16" width="16" title="Please supply a start date" /></asp:RequiredFieldValidator></td>
						        <td>&nbsp;and&nbsp;</td>							    
						        <td><telerik:RadDatePicker runat="server" ID="dteFilterEndDate" Width="100">
                                <DateInput runat="server"
                                DateFormat="dd/MM/yy">
                                </DateInput>
                                </telerik:RadDatePicker> </td>
						    </tr>
					    </table>
				    </td>
			    </tr>
			    <tr>
				    <td class="formCellLabel">For driver</td>
				    <td class="formCellField" colspan="3">
                        <telerik:RadComboBox ID="cboDrivers" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                            MarkFirstMatch="true" RadControlsDir="~/script/RadControls/"
                            ShowMoreResultsBox="true" EnableTheming="true" SkinsPath="~/RadControls/ComboBox/Skins/" Skin="Orchestrator" Width="275px">
                        </telerik:RadComboBox>
    					
				    </td>
			    </tr>
		    </table>
		    
		</fieldset>
		    
	    <div class="buttonbar">
	        <asp:Button id="btnFilter" runat="server" Text="Filter"></asp:Button>
        </div>

	    <asp:Label id="lblConfirmation" runat="server" CssClass="confirmation"></asp:Label>

	    <P1:PrettyDataGrid id="dgRequests" runat="server" RowHighlightColor="255, 255, 128" CssClass="DataGridStyle" GridLines="Both" 
		    AutoGenerateColumns="False" RowClickEventCommandName="Select" AllowSorting="True" RowSelectionEnabled="False" 
		    GroupingColumn="" AllowGrouping="False" AllowCollapsing="False" StartCollapsed="False" width="100%">
		    <SELECTEDITEMSTYLE CssClass="DataGridListItemSelected"></SELECTEDITEMSTYLE>
		    <ALTERNATINGITEMSTYLE CssClass="DataGridListItemAlt"></ALTERNATINGITEMSTYLE>
		    <ITEMSTYLE CssClass="DataGridListItem"></ITEMSTYLE>
		    <HEADERSTYLE CssClass="DataGridListHead"></HEADERSTYLE>
		    <FOOTERSTYLE BackColor="#CCCCCC" ForeColor="Black"></FOOTERSTYLE>
		    <COLUMNS>
			    <asp:TemplateColumn HeaderText="Driver" SortExpression="FullName" ItemStyle-VerticalAlign="Top" ItemStyle-Width="20%">
				    <ItemTemplate>
					    <%# DataBinder.Eval(Container.DataItem, "FullName") %>
					    <input type="hidden" id="hidRequestId" runat="server" value='<%# DataBinder.Eval(Container.DataItem, "DriverRequestId") %>'>
				    </ItemTemplate>
			    </asp:TemplateColumn>
			    <asp:BoundColumn HeaderText="Date" DataField="AppliesUntil" SortExpression="AppliesUntil" ItemStyle-VerticalAlign="Top" DataFormatString="{0:dd/MM/yy}" ItemStyle-Width="10%"></asp:BoundColumn>
			    <asp:BoundColumn HeaderText="Request" DataField="RequestText" SortExpression="RequestText" ItemStyle-VerticalAlign="Top" ItemStyle-Width="70%"></asp:BoundColumn>
			    <asp:ButtonColumn HeaderText="Edit" CommandName="Edit" ButtonType="LinkButton" Text="Edit"></asp:ButtonColumn>
			    <asp:ButtonColumn HeaderText="Delete" CommandName="Delete" ButtonType="LinkButton" Text="Delete"></asp:ButtonColumn>
		    </COLUMNS>
		    <PAGERSTYLE BackColor="#A1C0F6" ForeColor="Black" Mode="NumericPages" HorizontalAlign="Center"></PAGERSTYLE>
	    </P1:PrettyDataGrid>
    						
	    <div class="buttonbar">
		    <asp:Button id="btnAddRequest" runat="server" Text="Add Request" CausesValidation="False"></asp:Button>
	    </div>
    	
    </div>
</asp:Content>