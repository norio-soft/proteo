<%@ Page language="c#" Inherits="Orchestrator.WebUI.Resource.Driver.ViewDriverStartTimes" Codebehind="ViewDriverStartTimes.aspx.cs" MasterPageFile="~/default_tableless.master"   Title="View Driver Start Times" %>

<asp:Content ContentPlaceHolderID="Header" runat="server">
    <script language="javascript" type="text/javascript">
    <!--
        function ClockIn(resourceId) {
            openDialog('<%=Page.ResolveUrl("~/Resource/Driver/EnterDriverStartTimes.aspx")%>?wiz=true&resourceId=' + resourceId + '&date=<%=StartDateString%>', '500', '280');
        }
    //-->
    </script>
</asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>View Driver Start Times</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>View driver start times for the given date below.</h2>

    <div class="formPageContainer">
	    <fieldset><legend>Filter Options</legend>
		    <table>
			    <tr>
				    <td class="formCellLabel">Date</td>
				    <td class="formCellField">
				        <telerik:RadDatePicker ID="dteDate" Width="100" runat="server">
                        <DateInput runat="server"
                        DateFormat="dd/MM/yy">
                        </DateInput>
                        </telerik:RadDatePicker>
				    </td>
				    <td>
					    <asp:RequiredFieldValidator id="rfvDate" runat="server" Display="Dynamic" ControlToValidate="dteDate" ErrorMessage="Please supply a date"><img src="../../images/newMasterPage/icon-warning.png" height="16" width="16" title="Please supply a date" /></asp:RequiredFieldValidator>
				    </td>
			    </tr>
		    </table>
	    </fieldset>
	    <asp:DataGrid id="dgDrivers" runat="server" AutoGenerateColumns="False" AllowSorting="False" AllowPaging="True" pagesize="40" width="100%" cellpadding="2" backcolor="White" border="1" cssclass="DataGridStyle" PagerStyle-Mode="NumericPages" PagerStyle-HorizontalAlign="Right" >
		    <AlternatingItemStyle CssClass="DataGridListItemAlt"></AlternatingItemStyle>
		    <ItemStyle CssClass="DataGridListItem"></ItemStyle>
		    <HeaderStyle CssClass="DataGridListHead"></HeaderStyle>
		    <Columns>
			    <asp:BoundColumn DataField="FullName" HeaderText="Driver"></asp:BoundColumn>
			    <asp:TemplateColumn HeaderText="Time Started" ItemStyle-VerticalAlign="Middle" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="15%" ItemStyle-Width="15%">
				    <ItemTemplate>
					    <img id="imgClockIn" runat="server" src="../../images/newMasterPage/icon-clockin.png" alt="Enter this driver's start time for " style="VERTICAL-ALIGN: -3px; cursor: hand">
					    <a href="javascript:ClockIn('<%# DataBinder.Eval(Container.DataItem, "ResourceId")%>');" alt="Alter this driver's start time."><%# DataBinder.Eval(Container.DataItem, "StartDateTime", "{0:HH:mm}") %></a>
				    </ItemTemplate>
			    </asp:TemplateColumn>
			    <asp:BoundColumn DataField="Notes" HeaderText="Notes"></asp:BoundColumn>
		    </Columns>
	    </asp:DataGrid>
        <div class="buttonbar"><asp:Button id="btnGet" runat="server" Text="Get Start Times"></asp:Button></div>
    </div>
    
</asp:Content>