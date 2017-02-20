<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WorkForClientNew.aspx.cs" Inherits="Orchestrator.WebUI.Organisation.WorkForClientNew" MasterPageFile="~/default_tableless.master" Title="Work for Client" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Client Revenue</h1></asp:Content>

<asp:Content ContentPlaceHolderID="Header" runat="server"></asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script language="javascript" type="text/javascript">
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

        $(document).ready(function () {
            /*stops the table having a black background when viewed in a small window/screen*/
            var width = $(".rgMasterTable").width();
            $(".masterpagelite_layoutContainer").css("min-width", width + 20 + "px");
        }); 
</script>


    <h2 id="heading" runat="server"></h2>
    <div class="ToolbarBlue" style="height: 24px; padding: 1px 1px 1px 3px; background-position: top;">
		<div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()" style="display: none;">Show filter Options</div>
		<div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">Close filter Options</div>
        <asp:Button ID="btnExport" runat="server" Text="Export" Width="80px" ToolTip="Extract the data in a format you can open in Excel" CausesValidation="true" />
	</div>

    <!--Hidden Filter Options-->
    <div class="overlayedFilterBox" id="overlayedClearFilterBox" style="display: block;"> 
          
        <fieldset>
            <legend>Filter Options</legend>
            <table>
                <tr>
                    <td class="formCellLabel">Date Range</td>
                    <td class="formCellInput">
                        <telerik:RadDropDownList
                            id="lstDateRange"
                            runat="server"
                            Width="165px"
                            />
                    </td>
                </tr>
                 <tr>
                    <td class="formCellLabel">Client</td>
                    <td class="formCellInput">
                        <telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                            MarkFirstMatch="false" AllowCustomText="False" ShowMoreResultsBox="false"
                            OnClientItemsRequesting="cboClient_itemsRequesting"
                            Width="165px">
                            <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetClients" />
                        </telerik:RadComboBox>
                    </td>
            </tr>


           </table>        
        </fieldset>

        <div id="buttonBar" runat="server" class="buttonBar">
            <asp:Button ID="btnRefresh" runat="server" Text="Refresh" Width="80px" ToolTip="Requery Orchestrator with the dates above" CausesValidation="true" />         
        </div>

    </div>
  
    <%-- Grid --%>
    <telerik:RadGrid runat="server" ID="grdClientRevenue" AllowPaging="false" AutoGenerateColumns="false" EnableViewState="false">
        <MasterTableView Width="100%" NoMasterRecordsText="No revenue data for the specified date range.">
            <RowIndicatorColumn Display="true"></RowIndicatorColumn>
        </MasterTableView>
        <ClientSettings />
    </telerik:RadGrid>

    <script type="text/javascript">
        FilterOptionsDisplayHide();

        function cboClient_itemsRequesting(sender, eventArgs) {
            try {
                var context = eventArgs.get_context();
                context["DisplaySuspended"] = true;
            }
            catch (err) { }
        }

    </script>
</asp:Content>