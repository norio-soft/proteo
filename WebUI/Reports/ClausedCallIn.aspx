<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/default_tableless.master"
    CodeBehind="ClausedCallIn.aspx.cs" Inherits="Orchestrator.WebUI.Reports.ClausedCallIn" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    
        <h1>
            Claused Call in</h1>
        <fieldset>
            <legend>Filter Options</legend>
            <table>
                <tr>
                    <td runat="server" id="tdDateOptions">
                        <table>
                            <tr>
                                <td class="formCellLabel">
                                    Client
                                </td>
                                <td class="formCellInput" colspan="2">
                                    <telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                                        MarkFirstMatch="false" AllowCustomText="False" ShowMoreResultsBox="false" Width="355px"
                                        Height="300px" OnClientItemsRequesting="cboClient_itemsRequesting">
                                        <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetClients" />
                                    </telerik:RadComboBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel" style="width: 100;">
                                    Date From
                                </td>
                                <td class="formCellField">
                                    <telerik:RadDatePicker ID="dteStartDate" runat="server" Width="100" ToolTip="The Start Date for the filter">
                                    <DateInput runat="server"
                                    DateFormat="dd/MM/yy">
                                    </DateInput>
                                    </telerik:RadDatePicker>
                                </td>
                                <td class="formCellField" style="width: 30;">
                                    <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ControlToValidate="dteStartDate"
                                        ValidationGroup="grpRefresh" ErrorMessage="Please enter a Start Date"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel" style="width: 100;">
                                    Date To
                                </td>
                                <td class="formCellField">
                                    <telerik:RadDatePicker ID="dteEndDate" runat="server" Width="100" ToolTip="The Start Date for the filter">
                                    <DateInput runat="server"
                                    DateFormat="dd/MM/yy">
                                    </DateInput>
                                    </telerik:RadDatePicker>
                                </td>
                                <td class="formCellField" style="width: 30;">
                                    <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ControlToValidate="dteEndDate"
                                        ValidationGroup="grpRefresh" ErrorMessage="Please enter an End Date"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </fieldset>
        <div class="buttonbar">
            <asp:Button ID="btnExport" runat="server" Text="Export" ValidationGroup="grpRefresh"
                CausesValidation="true" />
        </div>
    

    <script type="text/javascript">
        function cboClient_itemsRequesting(sender, eventArgs) {
            try {
                var context = eventArgs.get_context();
                if ('<%=string.IsNullOrEmpty(Request.QueryString["oid"]) %>' == 'True') {
                    context["DisplaySuspended"] = false;
                }
                else {
                    context["DisplaySuspended"] = true;
                }
            }
            catch (err) { }
        }
    </script>

</asp:Content>
