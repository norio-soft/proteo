<%@ Page Title="Export Orders" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="OrderListExport.aspx.cs" Inherits="Orchestrator.WebUI.Reports.OrderListExport" %>

<%@ Register TagPrefix="uc" TagName="BusinessTypeCheckList" Src="~/UserControls/BusinessTypeCheckList.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>Order List CSV Export</h1>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table cellspacing="6px">
        <tr>
            <td class="formCellLabel">Client</td>
            <td class="formCellField">
				<telerik:RadComboBox ID="clientCombo" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500" MarkFirstMatch="true"
                    Height="300px" Overlay="true" CausesValidation="False"  ShowMoreResultsBox="false" Width="350px" AllowCustomText="True"
					OnClientItemsRequesting="clientCombo_itemsRequesting">
					<WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetClients" />
				</telerik:RadComboBox>
            </td>
        </tr>
        <tr>
            <td class="formCellLabel">Collection Date</td>
            <td class="formCellField">
                from
                <telerik:RadDateInput ID="startDate" runat="server" DateFormat="dd/MM/yy" Width="65px" />

                <asp:RequiredFieldValidator ID="startDateRequired" runat="server" Display="Dynamic" ControlToValidate="startDate" ErrorMessage="Please enter the start date.">
    			    <img src="/images/Error.gif" height="16" width="16" title="Please enter the start date." alt="" />
                </asp:RequiredFieldValidator>

                to

                <telerik:RadDateInput ID="endDate" runat="server" DateFormat="dd/MM/yy" Width="65px" />

                <asp:RequiredFieldValidator ID="endDateRequired" runat="server" Display="Dynamic" ControlToValidate="endDate" ErrorMessage="Please enter the end date.">
                    <img src="/images/Error.gif" height="16" width="16" title="Please enter the end date." alt="" />
                </asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="formCellLabel">Business Types</td>
            <td class="formCellField"><uc:BusinessTypeCheckList ID="businessTypeCheckList" runat="server" ItemCountPerRow="5" /></td>
        </tr>
        <tr>
            <td class="formCellLabel">
                Sub-Contractor
            </td>
            <td class="formCellInput" colspan="2">
                <telerik:RadComboBox ID="cboSubcontractor" runat="server" EnableLoadOnDemand="true"
                    ItemRequestTimeout="500" MarkFirstMatch="false" RadControlsDir="~/script/RadControls/"
                    AllowCustomText="False" ShowMoreResultsBox="false" Width="355px" Height="300px">
                    <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetSubContractors" />
                </telerik:RadComboBox>
            </td>
        </tr>
        <tr>
            <td class="formCellLabel">Invoiced?</td>
            <td class="formCellField">
                <asp:RadioButtonList ID="rblInvoiced" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Text="All" Selected="True" />
                    <asp:ListItem Text="No" />
                    <asp:ListItem Text="Yes" />
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td class="formCellLabel">Sub-Contractor Invoiced?</td>
            <td class="formCellField">
                <asp:RadioButtonList ID="rblSubcontractorInvoiced" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Text="All" Selected="True" />
                    <asp:ListItem Text="No" />
                    <asp:ListItem Text="Yes" />
                </asp:RadioButtonList>
            </td>
        </tr>
    </table>

    <div class="buttonBar">
        <asp:Button ID="exportButton" runat="server" Text="Export Orders" />
    </div>

    <script type="text/javascript">
        function clientCombo_itemsRequesting(sender, eventArgs) {
            eventArgs.get_context()["DisplaySuspended"] = false;
        }
    </script>
</asp:Content>
