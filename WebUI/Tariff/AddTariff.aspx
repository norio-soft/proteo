<%@ Page Title="Add Tariff" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" 
    CodeBehind="AddTariff.aspx.cs" Inherits="Orchestrator.WebUI.Tariff.AddTariff" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>
        Create New Tariff
    </h1>
    <fieldset runat="server" id="fsTariff" class="invisiableFieldset">
        <br />
        <br />
        <table>
            <tr>
                <td class="formCellLabel">
                    Tariff Description
                </td>
                <td class="formCellField">
                    <asp:TextBox ID="txtTariffDescription" CssClass="fieldInputBox" runat="server" Width="250"
                        MaxLength="39"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvTariffDescription" runat="server" ErrorMessage="Please enter a description for this tariff"
                        ControlToValidate="txtTariffDescription" Display="Dynamic">
                                <img src="/images/Error.png" height="16" width="16" title="Please enter a description for this tariff" />
                    </asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">
                    Start Date
                </td>
                <td>
                    <telerik:RadDateInput runat="server" ID="dteStartDate" DateFormat="dd/MM/yy" DisplayDateFormat="dd/MM/yy"
                        Style="width: 60px">
                        <ClientEvents OnValueChanged="updateVersionDescription" />
                    </telerik:RadDateInput>
                    <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ErrorMessage="Please enter a start date for this tariff"
                        ControlToValidate="dteStartDate" Display="Dynamic">
                                <img src="/images/Error.png" height="16" width="16" title="Please enter a start date for this tariff" />
                    </asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td style="width: 150px" class="formCellLabel">
                    Version Description
                </td>
                <td class="formCellField">
                    <asp:TextBox ID="txtVersionDescription" CssClass="fieldInputBox" runat="server" Width="250"
                        MaxLength="50"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvVersionDescription" runat="server" ErrorMessage="Please enter a description for this tariff version"
                        ControlToValidate="txtVersionDescription" Display="Dynamic">
                                <img src="/images/Error.png" height="16" width="16" title="Please enter a description for this tariff version" />
                    </asp:RequiredFieldValidator>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <asp:RadioButton ID="optCopyTariff" runat="server" Text="Copy from Tariff" GroupName="CreateTariff"
            Checked="true"></asp:RadioButton>
        <br />
        <div id="divCopyTariff">
            <table style="margin-left: 50px">
                <tr>
                    <td class="formCellLabel" style="width: 150px;">
                        Copy From Tariff</td>
                    <td class="formCellField">
                        <telerik:RadComboBox runat="server" ID="cboCopyFromTariff" Width="250" OnClientSelectedIndexChanged="cboCopyFromTariff_ClientSelectedIndexChanged" />
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Copy from Version
                    </td>
                    <td class="formCellField">
                        <telerik:RadComboBox runat="server" ID="cboCopyFromVersion" Width="250" OnClientItemsRequested="cboCopyFromVersion_ClientItemsRequested" />
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Increase Tariff Table Rates By
                    </td>
                    <td class="formCellField">
                        <telerik:RadNumericTextBox runat="server" ID="txtIncreaseRate" Width="50"  MaxValue="200" MinValue="-200" NumberFormat-DecimalDigits="2" Type="Percent" Value="0.00"/>
                    </td>
                </tr>
            </table>
        </div>
        <br />
        <br />
        <asp:RadioButton ID="optEmptyTariff" runat="server" Text="Empty Tariff" GroupName="CreateTariff">
        </asp:RadioButton>
        <br />
        <div id="divEmptyTariff">
            <table style="margin-left: 50px">
                <tr>
                    <td class="formCellLabel" style="width: 150px">
                        Is For Sub Contractors
                    </td>
                    <td class="formCellField">
                        <asp:CheckBox runat="server" ID="chkIsForSubContractor" />
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Zone Type
                    </td>
                    <td class="formCellField">
                        <asp:RadioButtonList runat="server" ID="optZoneType" RepeatDirection="Horizontal"
                            AutoPostBack="True" />
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel" style="width: 150px">
                        Zone Map
                    </td>
                    <td class="formCellField">
                        <telerik:RadComboBox runat="server" ID="cboZoneMap" Width="250" />
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Metric
                    </td>
                    <td class="formCellField">
                        <asp:RadioButtonList runat="server" ID="optMetric" RepeatDirection="Horizontal"
                         AutoPostBack="True" />
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel" style="width: 150px">
                        Scale
                    </td>
                    <td class="formCellField">
                        <telerik:RadComboBox runat="server" ID="cboScale" Width="250" />
                        <asp:RequiredFieldValidator ID="rfvScale" runat="server" ErrorMessage="Please select a Scale for this Tariff. If none are available to choose from, you will need to create one for the given Metric."
                            ControlToValidate="cboScale" Display="Dynamic">
                                <img src="/images/Error.png" height="16" width="16" title="Please select a Scale for this Tariff. If none are available to choose from, you will need to create one for the given Metric." />
                        </asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Multiply Rate By Order Metric Value
                    </td>
                    <td class="formCellField">
                        <asp:CheckBox runat="server" ID="chkMultiplyByOrderValue" />
                        <asp:Label runat="server" ID="lblAdditionalMultiplier" Text="Additional Multiplier" AssociatedControlID="txtAdditionalMultiplier" Enabled="false" />
                        <telerik:RadNumericTextBox runat="server" ID="txtAdditionalMultiplier" Type="Number" NumberFormat-DecimalDigits="3" MinValue="0" Enabled="false" />
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Ignore Additional Collections From A Delivery Point
                    </td>
                    <td class="formCellField">
                        <asp:CheckBox runat="server" ID="chkIgnoreAdditionalCollectsFromADeliveryPoint" />
                    </td>
                </tr>
            </table>
            
        </div>
        <br />
        <br />
        <div class="buttonbar">
            <asp:Button ID="btnSave" runat="server" Text="Save" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CausesValidation="false" />
        </div>
    </fieldset>

    <script type="text/javascript">
        //Trigger CopyFromTariff combo changed after initial page load
        //to cause child combos to load
        function pageLoad(sender, eventArgs) {
            if (!eventArgs.isPartialLoad) {
                cboCopyFromTariff_ClientSelectedIndexChanged();
            }
        }

        $(document).ready(function() {
            $("#<%= chkMultiplyByOrderValue.ClientID %>").click(function() {
                configureAdditionalMultiplierField(this.checked);
            });
        });

        function configureAdditionalMultiplierField(enable) {
            $("#<%= lblAdditionalMultiplier.ClientID %>").prop("disabled", !enable);

            var txtAdditionalMultiplier = $find("<%= txtAdditionalMultiplier.ClientID %>");
            if (enable) {
                txtAdditionalMultiplier.enable();
            }
            else {
                txtAdditionalMultiplier.clear();
                txtAdditionalMultiplier.disable();
            }
        }
        
        function cboCopyFromTariff_ClientSelectedIndexChanged(sender, eventArgs) {
            var cboCopyFromTariff = $find("<%= cboCopyFromTariff.ClientID %>");
            var cboCopyFromVersion = $find("<%= cboCopyFromVersion.ClientID %>");

            cboCopyFromVersion.clearSelection();
            cboCopyFromVersion.requestItems(cboCopyFromTariff.get_value(), false);
        }

        function cboCopyFromVersion_ClientItemsRequested(sender, eventArgs) {
            var items = sender.get_items();

            if (items.get_count() > 0)
                items.getItem(0).select();
        }

        function updateVersionDescription() {
            var tariffDesc = document.getElementById("<%= txtTariffDescription.ClientID %>").value;
            var startDate = $find("<%= dteStartDate.ClientID %>").get_value();

            if (tariffDesc.length > 0 && startDate.length > 0) {
                var _txtVersionDescription = document.getElementById("<%= txtVersionDescription.ClientID %>");
                _txtVersionDescription.value = tariffDesc + " - " + startDate;
            }
        }

    </script>

</asp:Content>
