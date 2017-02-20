<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true"
    CodeBehind="AddTariffVersion.aspx.cs" Inherits="Orchestrator.WebUI.Tariff.AddTariffVersion" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>
        Create New Tariff
        Version</h1>
    <fieldset runat="server" id="fsTariff" class="invisiableFieldset">
        <br />
        <br />
        <table>
            <tr>
                <td class="formCellLabel">
                    Tariff Description
                </td>
                <td class="formCellField">
                    <asp:Label ID="lblTariffDescription" runat="server" ></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">
                    Start Date
                </td>
                <td>
                    <asp:Label ID="lblStartDate" runat="server" ></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">
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
        <asp:RadioButton ID="optCopyVersion" runat="server" Text="Copy from Tariff Version" GroupName="CreateTariff"
            Checked="true"></asp:RadioButton>
        <br />
        <div id="divCopyVersion">
            <table style="margin-left: 50px">
                <tr>
                    <td class="formCellLabel" style="width: 150px;">
                        Copy From Tariff
                    </td>
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
        <asp:RadioButton ID="optEmptyVersion" runat="server" Text="Empty Tariff Version" GroupName="CreateTariff">
        </asp:RadioButton>
        <br />
        <div id="divEmptyVersion">
            <table style="margin-left: 50px">
                <tr>
                    <td class="formCellLabel" style="width: 150px">
                        Zone Map
                    </td>
                    <td class="formCellField">
                        <telerik:RadComboBox runat="server" ID="cboZoneMap" Width="250" />
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel" style="width: 150px">
                        Scale
                    </td>
                    <td class="formCellField">
                        <telerik:RadComboBox runat="server" ID="cboScale" Width="250" />
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

        function cboCopyFromTariff_ClientSelectedIndexChanged(sender, eventArgs) {
            var cboCopyFromVersion = $find("<%= cboCopyFromVersion.ClientID %>");

            cboCopyFromVersion.requestItems(sender.get_selectedItem().get_value());
        }

        function cboCopyFromVersion_ClientItemsRequested(sender, eventArgs) {
            var items = sender.get_items();

            if (items.get_count() > 0)
                items.getItem(0).select();
        }

    </script>

</asp:Content>
