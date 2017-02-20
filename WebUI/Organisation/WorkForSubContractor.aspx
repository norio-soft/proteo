<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="WorkForSubContractor.aspx.cs" Inherits="Orchestrator.WebUI.Organisation.WorkForSubContractor" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>All Work For Sub-Contractor</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">
    <script type="text/javascript" language="javascript" src="WorkForSubContractor.aspx.js"></script>
    
    <script type="text/javascript">
        var table = null;

        $(document).ready(function() {
            table = $get('orders');
        });

        // Function to show the filter options overlay box
        function FilterOptionsDisplayShow() {
            $("#overlayedClearFilterBox").css({ 'display': 'block' });
            $("#filterOptionsDiv").css({ 'display': 'none' });
            $("#filterOptionsDivHide").css({ 'display': 'block' });
        }

        function FilterOptionsDisplayHide() {
            $("#overlayedClearFilterBox").css({ 'display': 'none' });
            $("#filterOptionsDivHide").css({ 'display': 'none' });
            $("#filterOptionsDiv").css({ 'display': 'block' });
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
                    <div class="ToolbarBlue" style="height: 24px; padding: 1px 1px 1px 3px; background-position: top;">
		    <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()" style="display: none;">Show filter Options</div>
		    <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">Close filter Options</div>
	    </div>
        <!--Hidden Filter Options-->
        <div class="overlayedFilterBox" id="overlayedClearFilterBox" style="display: block;">    
        <fieldset>
            <legend>Sub-Contractor Work Items</legend> 
            <table>
                <tr>
                     <td>
                        <table>
                            <tr>
                                <td class="formCellLabel">Date From</td>
                                <td class="formCellField">
                                    <telerik:RadDatePicker Width="100" ID="dteDateFrom" runat="server">
                                    <DateInput runat="server"
                                    DateFormat="dd/MM/yy" DisplayDateFormat="dd/MM/yy">
                                    </DateInput>
                                    </telerik:RadDatePicker>
                                </td>
                                <td></td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">Date To</td>
                                <td class="formCellField">
                                    <telerik:RadDatePicker Width="100" ID="dteDateTo" runat="server" >
                                    <DateInput runat="server"
                                    DateFormat="dd/MM/yy" DisplayDateFormat="dd/MM/yy">
                                    </DateInput>
                                    </telerik:RadDatePicker>
                                </td>
                                <td></td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">Sub-Contractor</td>
                                <td class="formCellField">
                                    <telerik:RadComboBox ID="cboSubContractor" runat="server"  EnableLoadOnDemand="true" ItemRequestTimeout="500"
                                        MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" AllowCustomText="False"
                                        ShowMoreResultsBox="false" Skin="WindowsXP" Width="355px" Height="300px"></telerik:RadComboBox>
                                    <asp:HiddenField runat="server" ID="hidRefreshClicked" Value="true" />
                                </td>
                                <td style="text-align:right; padding-left:10px; display:none;">
                                    <asp:LinkButton ID="lnkClear" runat="server" Text="Clear" ToolTip="Clears the drop down selection" />
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">Uninvoiced Only</td>
                                <td class="formCellField">
                                    <asp:CheckBox runat="server" ID="chkUninvoiceOnly" Checked="false" />
                                </td>
                                <td></td>
                            </tr>
                       </table>
                    </td>
                </tr>
            </table>
        </fieldset>

            <div class="buttonbar">
        <asp:Button ID="btnRefresh" OnClientClick="$('input:hidden[id*=hidRefreshClicked]')[0].value = 'true';" runat="server" Text="Get Data" />
        
    </div>
    </div>
    

    
    <div>
        <asp:Label ID="lblError" runat="server" Font-Bold="true" ForeColor="Red"></asp:Label>
    </div>
    
    <div>
        <asp:Panel ID="pnlOrganisationTotals" runat="server">
            <h3>Summary</h3>
            <table class="Grid" cellpadding="0" cellspacing="0" style="border-top: 0px;">
                <thead>
                    <tr class="HeadingRow">
                        <td>Sub Contractor</td>
                        <td>Amount</td>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="repOrganisationTotal" runat="server">
                        <ItemTemplate>
                            <tr class="Row">
                                <td>
                                    <%# ((OrganisationTotals)Container.DataItem).OrganisationName %>
                                </td>
                                <td>
                                    <%# ((OrganisationTotals)Container.DataItem).OrganisationTotal.ToString("C", ((OrganisationTotals)Container.DataItem).organisationCulture) %>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <AlternatingItemTemplate>
                            <tr class="AlternatingRow">
                                <td>
                                    <%# ((OrganisationTotals)Container.DataItem).OrganisationName %>
                                </td>
                                <td>
                                    <%# ((OrganisationTotals)Container.DataItem).OrganisationTotal.ToString("C", ((OrganisationTotals)Container.DataItem).organisationCulture) %>
                                </td>
                            </tr>
                        </AlternatingItemTemplate>
                    </asp:Repeater>
                    <asp:Repeater ID="repCurrencyTotals" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td colspan="2" style="text-align:right; font-weight:bold;">
                                    <%# ((CurrencyTotals)Container.DataItem).CurrencyTotal.ToString("C", ((CurrencyTotals)Container.DataItem).currencyCulture) %>
                                </td>
                            </tr>
                        </ItemTemplate>                                    
                    </asp:Repeater>
                </tbody>
            </table>
        </asp:Panel>
    </div>
        
    <div style="margin-top:20px; margin-bottom:20px;">
        <h3>Sub Contracted Work</h3>
        <telerik:RadGrid runat="server" ID="grdSubbies" AllowMultiRowEdit="true" AllowSorting="True" Skin="Office2007" AutoGenerateColumns="False" AllowMultiRowSelection="True" GridLines="None" >
            <MasterTableView ClientDataKeyNames="JobSubContractID" Width="100%" DataKeyNames="JobSubContractID" EditMode="InPlace" >
                <RowIndicatorColumn Display="false">
                    <HeaderStyle Width="20px"></HeaderStyle>
                </RowIndicatorColumn>
                <Columns>
                    <telerik:GridBoundColumn HeaderText="Date" HeaderStyle-Width="50px" SortExpression="DeliveryDateTime" DataField="DeliveryDateTime" DataFormatString="{0:dd/MM/yy}" ReadOnly="true" ItemStyle-VerticalAlign="Top" />
                    <telerik:GridBoundColumn HeaderText="Subcontractor" DataField="SubContractorName" SortExpression="SubContractorName" ReadOnly="true" ItemStyle-Width="225"   />
                    <telerik:GridBoundColumn HeaderText="Reference" HeaderStyle-Width="210px" SortExpression="LeadReferenceValue" DataField="LeadReferenceValue" ItemStyle-VerticalAlign="Top" ReadOnly="true" />
                    <telerik:GridTemplateColumn HeaderText="From" HeaderStyle-Width="210px" ItemStyle-VerticalAlign="Top">
                        <ItemTemplate>
                            <span id="spnFrom" runat="server" onclick=""></span>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="To" HeaderStyle-Width="210px" ItemStyle-VerticalAlign="Top">
                        <ItemTemplate>
                            <span id="spnTo" runat="server" onclick=""></span>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn HeaderText="Pallets" HeaderStyle-Width="35px" SortExpression="PalletCount" DataField="PalletCount" ReadOnly="true" ItemStyle-VerticalAlign="Top" />
                    <telerik:GridBoundColumn HeaderText="Weight" HeaderStyle-Width="35px" SortExpression="Weight" DataField="Weight" DataFormatString="{0:F0}" ReadOnly="true" ItemStyle-VerticalAlign="Top" />
                    <telerik:GridTemplateColumn Display="false" UniqueName="InvoiceNo" HeaderText="Invoice No" HeaderStyle-Width="90px" ItemStyle-Width="50px" ItemStyle-VerticalAlign="Top">
                        <ItemTemplate>
                            <asp:TextBox ID="txtInvoiceNo" runat="server" Width="80px"></asp:TextBox>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Run Id" HeaderStyle-Width="80px" ItemStyle-Width="50px" ItemStyle-VerticalAlign="Top" >
                        <ItemTemplate>
                            <a id="lnkRunId" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Run / Order Id" HeaderStyle-Width="80px" ItemStyle-Width="50px" ItemStyle-VerticalAlign="Top" >
                        <ItemTemplate>
                            <a id="lnkJobRef" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="HasPODs" HeaderText="HasPODs" ItemStyle-HorizontalAlign="Right" HeaderStyle-Width="80px" ItemStyle-Width="50px" ItemStyle-VerticalAlign="Top">
                        <ItemTemplate>
                            <%# ((Orchestrator.Entities.SubContractorDataItem)Container.DataItem).HasPODs == true ? "Yes" : "No" %>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="OrderRate" HeaderText="Charge">
                        <ItemTemplate>
                            <asp:Label ID="lblOrderRate" runat="server"></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="Cost" HeaderText="Rate" ItemStyle-HorizontalAlign="Right" HeaderStyle-Width="80px" ItemStyle-Width="50px" ItemStyle-VerticalAlign="Top">
                        <ItemTemplate>
                            <asp:Label id="lblSubContractRate" runat="server"></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <input ID="txtSubContractRate" style="width:75px" type="text" runat="server" />
                        </EditItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Profit" UniqueName="Profit" Visible="false">
                        <ItemTemplate>
                            <%# ((decimal)((Orchestrator.Entities.SubContractorDataItem)Container.DataItem).OrderRate - ((Orchestrator.Entities.SubContractorDataItem)Container.DataItem).Rate).ToString("C") %>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
                <NoRecordsTemplate>
                    <div>There are no records to display</div>
                </NoRecordsTemplate>
            </MasterTableView>
        </telerik:RadGrid>
    </div>
        
    <div style="margin-bottom:10px;">
        <h3>Hub Charge</h3>
        <asp:ListView ID="lvHubCharges" runat="server">
            <LayoutTemplate>
                <div class="listViewGrid">
                    <table id="orders" cellpadding="0" cellspacing="0">
                        <thead>
                            <tr class="HeadingRow">
                                <th>OrderID</th>
                                <th>JobID</th>
                                <th>Depot</th>
                                <th>Date</th>
                                <th>Full</th>
                                <th>Half</th>
                                <th>Qtr</th>
                                <th>O/S</th>
                                <th>Weight</th>
                                <th>Service</th>
                                <th>Surcharge</th>
                                <th>PostCode</th>
                                <th>PostTown</th>
                                <th>Cost</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr id="itemPlaceHolder" runat="server" />
                        </tbody>
                    </table>
                </div>
            </LayoutTemplate>
            <ItemTemplate>
                <tr id="row" runat="server" class="group">
                    <th colspan="14"><%# Eval("OrganisationName")%> (<%# Eval("Orders")%> Orders)</th>
                </tr>
                <asp:ListView ID="lvItems" runat="server" DataSource='<%# Eval("Items") %>' >
                    <LayoutTemplate>
                        <tr runat="server" id="itemPlaceHolder" />
                    </LayoutTemplate>
                    <ItemTemplate>
                        <tr id="row" runat="server" class="Row">
                            <td> <a id="orderRef" href='javascript:viewOrderProfile(<%# ((System.Data.DataRow)Container.DataItem)["OrderID"].ToString() %>);'> <%# ((System.Data.DataRow)Container.DataItem)["OrderID"].ToString() %> </a></td>
                            <td> <a id="jobRef" href='javascript:viewJobDetails(<%# ((System.Data.DataRow)Container.DataItem)["JobID"].ToString()%>);' > <%# ((System.Data.DataRow)Container.DataItem)["JobID"].ToString()%> </a></td>
                            <td><%# ((System.Data.DataRow)Container.DataItem)["RequestingDepot"].ToString()%></td>
                            <td><%# ((DateTime)((System.Data.DataRow)Container.DataItem)["Date"]).ToString("dd/MM/yyyy")%></td>
                            <td><%# ((System.Data.DataRow)Container.DataItem)["FullPallets"].ToString()%></td>
                            <td><%# ((System.Data.DataRow)Container.DataItem)["HalfPallets"].ToString()%></td>
                            <td><%# ((System.Data.DataRow)Container.DataItem)["QtrPallets"].ToString()%></td>
                            <td><%# ((System.Data.DataRow)Container.DataItem)["OverPallets"].ToString()%></td>
                            <td><%# ((decimal)((System.Data.DataRow)Container.DataItem)["Weight"]).ToString("F2")%></td>
                            <td><%# ((System.Data.DataRow)Container.DataItem)["ServiceLevel"].ToString()%></td>
                            <td><%# ((System.Data.DataRow)Container.DataItem)["Surcharge"].ToString()%></td>
                            <td><%# ((System.Data.DataRow)Container.DataItem)["PostCode"].ToString()%></td>
                            <td><%# ((System.Data.DataRow)Container.DataItem)["PostTown"].ToString()%></td>
                            <td><%# ((decimal)((System.Data.DataRow)Container.DataItem)["HubCharge"]).ToString("C", GetCulture(((int)((System.Data.DataRow)Container.DataItem)["LCID"])))%></td>
                        </tr>
                    </ItemTemplate>
                </asp:ListView>
            </ItemTemplate>
            <EmptyDataTemplate>
                <div>
                    There are no records to display
                </div>
            </EmptyDataTemplate>
        </asp:ListView>
    </div>
        
    <div class="buttonbar" style="display: none;">
        <asp:Button ID="btnRefreshBottom" runat="server" Text="Refresh" />
        <div class="clearDiv"></div>
    </div>

    <script language="javascript" type="text/javascript">
        FilterOptionsDisplayHide();
    </script>
</asp:Content>