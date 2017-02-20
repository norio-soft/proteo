<%@ Page language="c#" Inherits="Orchestrator.WebUI.resource.vehicle.VehicleService" Codebehind="VehicleService.aspx.cs" MasterPageFile="~/WizardMasterPage.Master" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


     <table id="Table2" width="100%" cellpadding="0" cellspacing="0" border="0">
   
    <tr>
        <td class="layoutContentMiddle" valign="top" align="left" style="overflow: scroll;">
            <div class="layoutContentMiddleInner">
                <asp:label id=lblConfirmation runat="server" cssclass="confirmation" visible="false" text="The new vehicle has been added successfully.">The new service for this vehicle has been added successfully.</asp:label>
                    <asp:datagrid id="dgVehicleService" runat="server" AutoGenerateColumns="False" AllowSorting="false"
                        AllowPaging="True" pagesize="20" width="100%" cellpadding="2" backcolor="White" border="1"
                        cssclass="DataGridStyle" PagerStyle-Mode="NumericPages" PagerStyle-HorizontalAlign="Right"
                        OnPageIndexChanged="dgVehicleService_Page">
                        <AlternatingItemStyle CssClass="rgAltRow"></AlternatingItemStyle>
                        <ItemStyle CssClass="rgRow"></ItemStyle>
                        <HeaderStyle CssClass="rgHeader"></HeaderStyle>
                        <Columns>
                            <asp:BoundColumn Visible="False" DataField="VehicleServiceId" HeaderText="VehicleServiceId"></asp:BoundColumn>
                            <asp:BoundColumn DataField="VehicleServiceDueDate" SortExpression="VehicleServiceDueDate" HeaderText="Vehicle Service Date" DataFormatString="{0:dd/MM/yy}"></asp:BoundColumn>
                        </Columns>
                        <PagerStyle HorizontalAlign="Right" CssClass="DataGridListPagerStyle" Mode="NumericPages"></PagerStyle>
                    </asp:datagrid>
                    
                    <fieldset>
                        <table id="Table1" width="300">
                            <tr>
                                <td class="formCellLabel">
	                                <asp:Label id="Label1" runat="server">New Service Date</asp:Label>
	                            </td>
                                <td class="formCellField">
                                    <telerik:RadDateInput id=dteNewServiceDate runat="server" dateformat="dd/MM/yy" ToolTip="Please enter a new service date."></telerik:RadDateInput>
                                </td>
                                <td class="formCellField">
                                    <asp:RequiredFieldValidator id=rfvMOTExpiry runat="server" ControlToValidate="dteNewServiceDate" ErrorMessage="Please supply a service date."><img src="../../images/Error.gif" height='16' width='16' title='Please supply the date the vehicle&quote;s current MOT expires on'</asp:RequiredFieldValidator>
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                    
                    <div class="buttonbar">
                        <asp:button id="btnCancel" runat="server" Text="Back" CausesValidation="False" />
                        <asp:button id="btnAddVehicleService" runat="server" Text="Add Service Date" onclick="btnAddVehicleService_Click"></asp:button>
                    </div>
                </div>            
            </td>
        </tr>
    </table>
</asp:Content>