<%@ Page Title="Premier KPI Export" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="True" CodeBehind="WisbechRoadwaysKPIs.aspx.cs" Inherits="Orchestrator.WebUI.KPIReporting.WisbechRoadwaysKPIs" %>

<%@ Register TagPrefix="p1" TagName="Point" Src="~/UserControls/point.ascx" %>
<%@ Register TagPrefix="p1" TagName="BusinessTypeCheckList" Src="~/UserControls/BusinessTypeCheckList.ascx" %>

<asp:Content id="cntHead" ContentPlaceHolderID="Header" runat="server">
    <style type="text/css">
        td.formCellField {
            vertical-align: top;
            padding-top: 2px;
        }

        table.kpi td {
            vertical-align: top;
        }

        td.checkListCell {
            white-space: nowrap;
        }
        
        div.clientsCaption {
            margin-top: 2px;
        }
    </style>
</asp:Content>

<asp:Content ID="cntTitle" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>Premier KPI Export</h1>
</asp:Content>

<asp:Content ID="cntMain" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManagerProxy ID="scriptManagerProxy" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/KPIReporting/WisbechRoadwaysKPIs.aspx.js" />
        </Scripts>
    </asp:ScriptManagerProxy>

    <p>Premier Foods and Tesco KPI Export</p>
    <p>All date ranges are inclusive</p>

    <asp:Panel ID="pnlProductReturns" runat="server" DefaultButton="btnProductReturns" GroupingText="Product Returns">
        <div class="clientsCaption">Premier only</div>
                
        <table class="kpi">
            <tr>
                <td>
                    <table>
                        <tr>
                            <td class="formCellLabel">Start Date</td>
                            <td class="formCellField">
                                <telerik:RadDateInput ID="dtePRStartDate" runat="server" DateFormat="dd/MM/yy" Width="65px" />

                                <asp:RequiredFieldValidator ID="rfvPRStartDate" runat="server" Display="Dynamic" ControlToValidate="dtePRStartDate" ValidationGroup="vgPR" ErrorMessage="Please enter the start date.">
    			                    <img src="/images/Error.gif" height="16" width="16" title="Please enter the start date." alt="" />
                                </asp:RequiredFieldValidator>
                            </td>

                            <td class="formCellLabel">End Date</td>
                            <td class="formCellField">
                                <telerik:RadDateInput ID="dtePREndDate" runat="server" DateFormat="dd/MM/yy" Width="65px" />

                                <asp:RequiredFieldValidator ID="rfvPREndDate" runat="server" Display="Dynamic" ControlToValidate="dtePREndDate" ValidationGroup="vgPR" ErrorMessage="Please enter the end date.">
                                    <img src="/images/Error.gif" height="16" width="16" title="Please enter the end date." alt="" />
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                    </table>
                </td>

                <td>
                    <div class="buttonBar">
                        <asp:Button ID="btnProductReturns" runat="server" Text="Export Product Returns" ValidationGroup="vgPR" />
                    </div>
                </td>
            </tr>
        </table>
    </asp:Panel>

    <asp:Panel ID="pnlTonnesToCustomer" runat="server" DefaultButton="btnTonnesToCustomer" GroupingText="Tonnes to Customer">
        <div class="clientsCaption">Premier and Tesco</div>

        <table class="kpi">
            <tr>
                <td>
                    <table>
                        <tr>
                            <td class="formCellLabel">Business Type</td>
                            <td class="formCellField">
                                <p1:BusinessTypeCheckList ID="chkTTCBusinessType" runat="server" />

                                <asp:CustomValidator ID="cvBusinessTypes" runat="server" Display="Dynamic" ValidationGroup="vgTTC" ClientValidationFunction="cvBusinessTypes_Validate" ErrorMessage="Please select at least one business type.">
    			                    <img src="/images/Error.gif" height="16" width="16" title="Please select at least one business type." alt="" />
                                </asp:CustomValidator>
                            </td>

                            <td class="formCellLabel">Start Date</td>
                            <td class="formCellField">
                                <telerik:RadDateInput ID="dteTTCStartDate" runat="server" DateFormat="dd/MM/yy" Width="65px" />
                        
                                <asp:RequiredFieldValidator ID="rfvTTCStartDate" runat="server" Display="Dynamic" ControlToValidate="dteTTCStartDate" ValidationGroup="vgTTC" ErrorMessage="Please enter the start date.">
    			                    <img src="/images/Error.gif" height="16" width="16" title="Please enter the start date." alt="" />
                                </asp:RequiredFieldValidator>
                            </td>

                            <td class="formCellLabel">End Date</td>
                            <td class="formCellField">
                                <telerik:RadDateInput ID="dteTTCEndDate" runat="server" DateFormat="dd/MM/yy" Width="65px" />
                                <asp:RequiredFieldValidator ID="rfvTTCEndDate" runat="server" Display="Dynamic" ControlToValidate="dteTTCEndDate" ValidationGroup="vgTTC" ErrorMessage="Please enter the end date.">
                                    <img src="/images/Error.gif" height="16" width="16" title="Please enter the end date." alt="" />
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                    </table>
                </td>

                <td>
                    <div class="buttonBar">
                        <asp:Button ID="btnTonnesToCustomer" runat="server" Text="Export Tonnes To Customer" ValidationGroup="vgTTC" />
                    </div>
                </td>
            </tr>
        </table>
    </asp:Panel>
        
    <asp:Panel ID="pnlDeliveryPerformance" runat="server" DefaultButton="btnDeliveryPerformance" GroupingText="Delivery Performance">
        <div class="clientsCaption">Premier and Tesco</div>

        <table class="kpi">
            <tr>
                <td>
                    <table>
                        <tr>
                            <td class="formCellLabel">Start Date</td>
                            <td class="formCellField">
                                <telerik:RadDateInput ID="dteDPStartDate" runat="server" DateFormat="dd/MM/yy" Width="65px" />

                                <asp:RequiredFieldValidator ID="rfvDPStartDate" runat="server" Display="Dynamic" ControlToValidate="dteDPStartDate" ValidationGroup="vgDP" ErrorMessage="Please enter the start date.">
    			                    <img src="/images/Error.gif" height="16" width="16" title="Please enter the start date." alt="" />
                                </asp:RequiredFieldValidator>
                            </td>

                            <td class="formCellLabel">End Date</td>
                            <td class="formCellField">
                                <telerik:RadDateInput ID="dteDPEndDate" runat="server" DateFormat="dd/MM/yy" Width="65px" />

                                <asp:RequiredFieldValidator ID="rfvDPEndDate" runat="server" Display="Dynamic" ControlToValidate="dteDPEndDate" ValidationGroup="vgDP" ErrorMessage="Please enter the end date.">
                                    <img src="/images/Error.gif" height="16" width="16" title="Please enter the end date." alt="" />
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                    </table>
                </td>

                <td>
                    <div class="buttonBar">
                        <asp:Button ID="btnDeliveryPerformance" runat="server" Text="Export Delivery Performance" ValidationGroup="vgDP" />
                    </div>
                </td>
            </tr>
        </table>
    </asp:Panel>

    <asp:Panel ID="pnlOrderRefusals" runat="server" DefaultButton="btnOrderRefusals" GroupingText="Order Refusals">
        <div class="clientsCaption">Premier only</div>
                
        <table class="kpi">
            <tr>
                <td>
                    <table>
                        <tr>
                            <td class="formCellLabel">Start Date</td>
                            <td class="formCellField">
                                <telerik:RadDateInput ID="dteORStartDate" runat="server" DateFormat="dd/MM/yy" Width="65px" />

                                <asp:RequiredFieldValidator ID="rfvORStartDate" runat="server" Display="Dynamic" ControlToValidate="dteORStartDate" ValidationGroup="vgOR" ErrorMessage="Please enter the start date.">
    			                    <img src="/images/Error.gif" height="16" width="16" title="Please enter the start date." alt="" />
                                </asp:RequiredFieldValidator>
                            </td>

                            <td class="formCellLabel">End Date</td>
                            <td class="formCellField">
                                <telerik:RadDateInput ID="dteOREndDate" runat="server" DateFormat="dd/MM/yy" Width="65px" />

                                <asp:RequiredFieldValidator ID="rfvOREndDate" runat="server" Display="Dynamic" ControlToValidate="dteOREndDate" ValidationGroup="vgOR" ErrorMessage="Please enter the end date.">
                                    <img src="/images/Error.gif" height="16" width="16" title="Please enter the end date." alt="" />
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                    </table>
                </td>

                <td>
                    <div class="buttonBar">
                        <asp:Button ID="btnOrderRefusals" runat="server" Text="Export Order Refusals" ValidationGroup="vgOR" />
                    </div>
                </td>
            </tr>
        </table>
    </asp:Panel>

    <asp:Panel ID="pnlOutstandingPODs" runat="server" DefaultButton="btnOutstandingPods" GroupingText="Outstanding PODs">
        <div class="clientsCaption">Premier and Princes (Daily, Backhaul and Consolidated only) and Tesco (Tesco Daily only)</div>

        <table class="kpi">
            <tr>
                <td>
                    <table>
                        <tr>
                            <td class="formCellLabel">Start Date</td>
                            <td>
                                <telerik:RadDateInput ID="dtePODStartDate" runat="server" DateFormat="dd/MM/yy" Width="65px" />
                                <asp:RequiredFieldValidator ID="rfvPODDateFrom" runat="server" Display="Dynamic" ControlToValidate="dtePODStartDate" ValidationGroup="vgPOD" ErrorMessage="Please enter the start date.">
    			                    <img src="/images/Error.gif" height="16" width="16" title="Please enter the start date." alt="" />
                                </asp:RequiredFieldValidator>
                            </td>
                            <td class="formCellLabel">End Date</td>
                            <td>
                                <telerik:RadDateInput ID="dtePODEndDate" runat="server" DateFormat="dd/MM/yy" Width="65px" />
                                <asp:RequiredFieldValidator ID="rfvPODDateTo" runat="server" Display="Dynamic" ControlToValidate="dtePODEndDate" ValidationGroup="vgPOD" ErrorMessage="Please enter the end date.">
                                    <img src="/images/Error.gif" height="16" width="16" title="Please enter the end date." alt="" />
                                </asp:RequiredFieldValidator>
                            </td>
                            <td>
                                <asp:RadioButtonList ID="rblPODReportType" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Text="Summary" Selected="True" />
                                    <asp:ListItem Text="Detail" />
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                    </table>
                </td>

                <td>
                    <div class="buttonBar">
                        <asp:Button ID="btnOutstandingPods" runat="server" Text="Export Outstanding Pods" ValidationGroup="vgPOD" />
                    </div>
                </td>
            </tr>
        </table>
    </asp:Panel>

    <asp:Panel ID="pnlFactoryClearance" runat="server" DefaultButton="btnFactoryClearance" GroupingText="Factory Clearance">
        <div class="clientsCaption">Premier only</div>
        
        <table class="kpi">
            <tr>
                <td>
                    <table>
                        <tr>
                            <td class="formCellLabel">Start Date</td>
                            <td>
                                <telerik:RadDateInput ID="dteFCStartDate" runat="server" DateFormat="dd/MM/yy" Width="65px" />
                                <asp:RequiredFieldValidator ID="rfvFCStartDate" runat="server" Display="Dynamic" ControlToValidate="dteFCStartDate" ValidationGroup="vgFC" ErrorMessage="Please enter the start date.">
    			                    <img src="/images/Error.gif" height="16" width="16" title="Please enter the start date." alt="" />
                                </asp:RequiredFieldValidator>
                            </td>
                            <td class="formCellLabel">End Date</td>
                            <td>
                                <telerik:RadDateInput ID="dteFCEndDate" runat="server" DateFormat="dd/MM/yy" Width="65px" />
                                <asp:RequiredFieldValidator ID="rfvFCEndDate" runat="server" Display="Dynamic" ControlToValidate="dteFCEndDate" ErrorMessage="Please enter the end date.">
                                    <img src="/images/Error.gif" height="16" width="16" title="Please enter the end date." alt="" />
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                    </table>
                </td>

                <td>
                    <div class="buttonBar">
                        <asp:Button ID="btnFactoryClearance" runat="server" Text="Export Factory Clearance" ValidationGroup="vgFC" />
                    </div>
                </td>
            </tr>
        </table>
    </asp:Panel>

    <asp:Panel ID="pnlCollectionPerformance" runat="server" DefaultButton="btnCollectionPerformance" GroupingText="Collection Performance">
        <div class="clientsCaption">Premier and Tesco</div>
                
        <table class="kpi">
            <tr>
                <td>
                    <table>
                        <tr>
                            <td class="formCellLabel">Start Date</td>
                            <td class="formCellField">
                                <telerik:RadDateInput ID="dteCPStartDate" runat="server" DateFormat="dd/MM/yy" Width="65px" />

                                <asp:RequiredFieldValidator ID="rfvCPStartDate" runat="server" Display="Dynamic" ControlToValidate="dteCPStartDate" ValidationGroup="vgCP" ErrorMessage="Please enter the start date.">
    			                    <img src="/images/Error.gif" height="16" width="16" title="Please enter the start date." alt="" />
                                </asp:RequiredFieldValidator>
                            </td>

                            <td class="formCellLabel">End Date</td>
                            <td class="formCellField">
                                <telerik:RadDateInput ID="dteCPEndDate" runat="server" DateFormat="dd/MM/yy" Width="65px" />

                                <asp:RequiredFieldValidator ID="rfvCPEndDate" runat="server" Display="Dynamic" ControlToValidate="dteCPEndDate" ValidationGroup="vgCP" ErrorMessage="Please enter the end date.">
                                    <img src="/images/Error.gif" height="16" width="16" title="Please enter the end date." alt="" />
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                    </table>
                </td>

                <td>
                    <div class="buttonBar">
                        <asp:Button ID="btnCollectionPerformance" runat="server" Text="Export Collection Performance" ValidationGroup="vgCP" />
                    </div>
                </td>
            </tr>
        </table>
    </asp:Panel>

    <asp:Panel ID="pnlCapacityUsage" runat="server" DefaultButton="btnCapacityUsage" GroupingText="Capacity Usage">
        <div class="clientsCaption">Premier and Tesco</div>
                
        <table class="kpi">
            <tr>
                <td>
                    <table>
                        <tr>
                            <td class="formCellLabel">Start Date</td>
                            <td class="formCellField">
                                <telerik:RadDateInput ID="dteCUStartDate" runat="server" DateFormat="dd/MM/yy" Width="65px" />

                                <asp:RequiredFieldValidator ID="rfvCUStartDate" runat="server" Display="Dynamic" ControlToValidate="dteCUStartDate" ValidationGroup="vgCU" ErrorMessage="Please enter the start date.">
    			                    <img src="/images/Error.gif" height="16" width="16" title="Please enter the start date." alt="" />
                                </asp:RequiredFieldValidator>
                            </td>

                            <td class="formCellLabel">End Date</td>
                            <td class="formCellField">
                                <telerik:RadDateInput ID="dteCUEndDate" runat="server" DateFormat="dd/MM/yy" Width="65px" />

                                <asp:RequiredFieldValidator ID="rfvCUEndDate" runat="server" Display="Dynamic" ControlToValidate="dteCUEndDate" ValidationGroup="vgCU" ErrorMessage="Please enter the end date.">
                                    <img src="/images/Error.gif" height="16" width="16" title="Please enter the end date." alt="" />
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                    </table>
                </td>

                <td>
                    <div class="buttonBar">
                        <asp:Button ID="btnCapacityUsage" runat="server" Text="Export Capacity Usage" ValidationGroup="vgCU" />
                    </div>
                </td>
            </tr>
        </table>
    </asp:Panel>

</asp:Content>
