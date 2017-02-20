<%@ Page Title="Collection / Delivery Window" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="Matrix.aspx.cs" Inherits="Orchestrator.WebUI.administration.DeliveryWindow.Matrix" %>

<asp:Content ID="contentTitle" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1><%= Page.Title %></h1>
</asp:Content>

<asp:Content ID="contentHead" ContentPlaceHolderID="Header" runat="server">
    <style type="text/css">
        .formCellLabel {
            width: 200px;
        }
    </style>
</asp:Content>

<asp:Content ID="contentBody" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <fieldset>
        <legend>Collection / Delivery Window</legend>

        <asp:Table ID="tblHeaderInfo" runat="server">
            <asp:TableRow ID="tr1" runat="server">
                <asp:TableCell ID="tr1tc1" runat="server" CssClass="formCellLabel">Description:</asp:TableCell>
                <asp:TableCell ID="tr1tc2" runat="server" CssClass="formCellField">
                    <asp:TextBox ID="txtDescription" runat="server"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell ID="tr1tc3" runat="server" CssClass="formCellLabel">Zone</asp:TableCell>
                <asp:TableCell ID="tr1tc4" runat="server" CssClass="formCellField">
                    <asp:TextBox ID="txtZone" runat="server" ReadOnly="true" Width="250"></asp:TextBox>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow ID="tr2" runat="server">
            </asp:TableRow>
        </asp:Table>


        <asp:Panel ID="pnlEffectiveDates" runat="server" Visible="true">
            <table>
                <tr>

                    <td class="formCellLabel">Effective Date</td>
                    <td class="formCellField">

                        <asp:DropDownList ID="ddlEffectiveDates" runat="server" AppendDataBoundItems="true"
                            OnSelectedIndexChanged="ddlEffectiveDates_SelectedIndexChanged"
                            AutoPostBack="True" Width="140px">
                            <asp:ListItem Text="--Select One--" Value="-1" /> 
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
                        <table>
                <tr>
                    <td colspan="2">
                        <div class="buttonBar">
                            <asp:Button ID="btnAddEffectiveDate" runat="server" Text="Add New" OnClick="btnAddEffectiveDate_Click" />
                        </div>

                    </td>
                </tr>
            </table>



        </asp:Panel>

        <asp:Panel ID="pnlEffectiveDateAddButtons" runat="server" Visible="true">
        </asp:Panel>
        <asp:Panel ID="detailsPanel" runat="server" Visible="false">
            <fieldset>
                <legend></legend>

                <table width="800px">
                    <tr>

                        <td class="formCellLabel">Effective From</td>
                        <td class="formCellField">
                            <telerik:RadDatePicker runat="server" ID="dteEffectiveFrom" Width="100" DateInput-ReadOnly="true">
                                <DateInput ID="dteInput" runat="server"
                                    DateFormat="dd/MM/yy">
                                </DateInput>
                            </telerik:RadDatePicker>

                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1"
                                ControlToValidate="dteEffectiveFrom"
                                Text="Effective date is required."
                                runat="server" EnableClientScript="true" Display="Dynamic" />
                        </td>

                           <td class="formCellLabel">Type</td>
                        <td class="formCellField">
                            <asp:DropDownList ID="cboDeliveryWindowType" runat="server"
                                OnSelectedIndexChanged="cboDeliveryWindowType_SelectedIndexChanged"
                                AutoPostBack="True" Width="140px">
                            </asp:DropDownList>

                        </td>

                    </tr>
                    <tr>

                     
                    </tr>
                    <tr>
                        <td class="formCellLabel">Sunday Adjustment Hours</td>
                        <td class="formCellField">
                            <asp:TextBox ID="txtSundayAdjustmentHours" runat="server" Width="30px">0</asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2"
                                ControlToValidate="txtSundayAdjustmentHours"
                                Text="Sunday Adjustment Hours can not be blank."
                                runat="server" EnableClientScript="true" Display="Dynamic" />
                        </td>
                        <td class="formCellLabel">Multi Collect Transition Time</td>
                        <td class="formCellField">
                            <asp:TextBox ID="txtMultiCollectionTime" runat="server" Width="30px">1</asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator4"
                                ControlToValidate="txtMultiCollectionTime"
                                Text="Multi Collect Transaction Time."
                                runat="server" EnableClientScript="true" Display="Dynamic" />
                        </td>
                    </tr>
                     <tr>
                        <td class="formCellLabel">Monday Adjustment Hours</td>
                        <td class="formCellField">
                            <asp:TextBox ID="txtMondayAdjustmentHours" runat="server" Width="30px">0</asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3"
                                ControlToValidate="txtMondayAdjustmentHours"
                                Text="Monday Adjustment Hours can not be blank."
                                runat="server" EnableClientScript="true" Display="Dynamic" />
                        </td>
                    </tr>
                    <tr>
                    </tr>
                </table>

                <asp:Panel ID="pnlDelWindowDetails" runat="server" Style="margin: 12px 0;">
                    <asp:Table ID="tblDelWindow" runat="server" EnableViewState="true"></asp:Table>
                    <div class="infoPanel">Notes: Hours must be between 0 and 99, Minutes must be between 0 and 59.</div>
                </asp:Panel>

                <div class="buttonBar">
                    <asp:Button ID="btnUpdate" runat="server" Text="Save Update" OnClick="btnUpdate_Click" OnClientClick="return ValidatePage();" />
                    <asp:Button ID="btnDelete" runat="server" Text="Delete" OnClick="btnDelete_Click" />
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" />
                </div>

                <input type="hidden" runat="server" id="hidDelWindowChanges1" />

                <asp:HiddenField ID="HiddenField1" runat="server" />
                <asp:HiddenField ID="HiddenField2" runat="server" />


            </fieldset>
        </asp:Panel>


        <asp:HiddenField ID="hidDelWindowChanges" runat="server" />
        <asp:HiddenField ID="hidErrors" runat="server" />
    </fieldset>

    <script type="text/javascript">

        function ValidatePage() {
            var hidErrors = $get("<%= hidErrors.ClientID %>");

            if (hidErrors.value.length > 0) {
                alert('Please recheck the form before you save again');
                return false;
            }
            else {
                return true;
            }
        }



        function tblDelWindow_onchange(input) {
            var hidDelWindowChanges = $get("<%= hidDelWindowChanges.ClientID %>");
            if (hidDelWindowChanges.value.length > 0)
                hidDelWindowChanges.value += ";";
            hidDelWindowChanges.value += input.name;
            input.parentElement.style.backgroundColor = "#ffa";
        }

        function ValidateChk(input) {
            tblDelWindow_onchange(input);
        }

        function ValidateHours(input, val) {

            tblDelWindow_onchange(input);

            if (input.value > val) {
                input.parentElement.style.backgroundColor = "red";
                MarkAsError(true, input);
            }
            else {
                MarkAsError(false, input);
            }
        }

   

        function MarkAsError(errored, input) {
            var hidErrors = $get("<%= hidErrors.ClientID %>");

            if (errored) {
                if (hidErrors.value.length > 0)
                    hidErrors.value += ";";

                hidErrors.value += input.name;
            }
            else {
                hidErrors.value = hidErrors.value.replace(hidErrors.value, "");
            }
        }

    </script>

</asp:Content>
