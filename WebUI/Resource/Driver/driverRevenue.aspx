<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/default_tableless.master" Title="Driver List" CodeBehind="driverRevenue.aspx.cs" Inherits="Orchestrator.WebUI.Resource.Driver.driverRevenue" %>
<%@ Register Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI" TagPrefix="cc1" %>
<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>
    
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript" language="javascript" src="/script/jquery.quicksearch-1.3.1.js"></script>
    <script type="text/javascript" src="/script/jquery.ajaxupload.3.6.js"></script>
    <script type="text/javascript" src="/script/jquery-ui-min.js"></script>
    <script type="text/javascript" src="/script/jquery.blockUI-2.64.0.min.js"></script>

    <h1>Driver Revenue</h1>

    <div style="width:100%">
        <table>
            <tr>
                <td class="formCellLabel">
                Driver
                </td>
                <td class="formCellField">
                    <telerik:RadComboBox ID="cboDriver" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                            OnClientDropDownClosing="ValidateClientSideClosing" OnClientItemsRequesting="cboDriver_OnClientItemsRequesting"
                            MarkFirstMatch="true" ShowMoreResultsBox="false" AllowCustomText="true" Skin="WindowsXP"
                            Width="160px" Overlay="true" Height="100px" >
                            <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetAllDrivers" />
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">
                    Start Date
                </td>
                <td class="formCellField">
                    <telerik:RadDateInput DateFormat="dd/MM/yy" DisplayDateFormat="dd/MM/yy" Width="60" runat="server" ID="dteStartDate">
                    </telerik:RadDateInput>
                </td>
                <td class="formCellLabel">
                    Week No.
                </td>
                <td class="formCellField">
                    <asp:DropDownList ID="cboWeekNo" runat="server" DataTextField="WeekNumber"
                        DataValueField="ID">
                        <asp:ListItem Text="1" Value="1"></asp:ListItem>
                        <asp:ListItem Text="2" Value="2"></asp:ListItem>
                        <asp:ListItem Text="3" Value="3"></asp:ListItem>
                        <asp:ListItem Text="4" Value="4"></asp:ListItem>
                        <asp:ListItem Text="5" Value="5"></asp:ListItem>
                        <asp:ListItem Text="6" Value="6"></asp:ListItem>
                        <asp:ListItem Text="7" Value="7"></asp:ListItem>
                        <asp:ListItem Text="8" Value="8"></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <tr>
                    <td class="formCellLabel">
                        End Date
                    </td>
                    <td class="formCellField">
                        <telerik:RadDateInput DateFormat="dd/MM/yy" DisplayDateFormat="dd/MM/yy" Width="60" runat="server" ID="dteEndDate">
                        </telerik:RadDateInput>
                    </td>
                </tr>
            </tr>
        </table>
        <br/>
        <div class="buttonbar">
            <asp:Button ID="btnRefresh" runat="server" Text="Refresh" />
        </div>
    </div>
    <div>
        <uc1:ReportViewer ID="reportViewer" runat="server" Visible="False" ViewerHeight="800" ViewerWidth="100%"></uc1:ReportViewer>
    </div>
    
    <telerik:RadAjaxManager ID="ramJobInstructions" runat="server" ClientEvents-OnResponseEnd="ramJobInstructionsAjaxRequest_OnResponseEnd">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="ramJobInstructions">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="lvPreInvoiceItems" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <script type="text/javascript">

        function ramJobInstructionsAjaxRequest_OnResponseEnd(sender, eventArgs) {
            hideLoading();
        }

        // Block UI
        //#region 
        function showLoading(messageContent) {
            $.blockUI({
                message: '<div style="margin-left:30px;"><span id="UpdatableMessage">' + messageContent + '</span></div>',
                css: {
                    border: 'none',
                    padding: '15px',
                    backgroundColor: '#000',
                    '-webkit-border-radius': '10px',
                    '-moz-border-radius': '10px',
                    opacity: '.5',
                    color: '#fff'
                }
            });
        }

        function ValidateClientSideClosing(item) {
            if (item != null)
                if (item.get_text().length > 0 && item.get_value().length < 1)
                return false;
        }

        function cboDriver_OnClientItemsRequesting(sender, eventArgs) {
            var _cboDriver = $find("<%=cboDriver.ClientID %>");
            var context = eventArgs.get_context();
            context["FilterString"] = _cboDriver.get_value();
        }

        function hideLoading() {
            $.unblockUI();
        }
        
    </script>
</asp:Content>