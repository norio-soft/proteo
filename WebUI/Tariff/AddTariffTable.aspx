<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true"
    CodeBehind="AddTariffTable.aspx.cs" Inherits="Orchestrator.WebUI.Tariff.AddTariffTable" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>
        Create New Tariff Table</h1>
    <fieldset runat="server" id="fsTariffTable" class="invisiableFieldset">
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
                    Version Description
                </td>
                <td class="formCellField">
                    <asp:Label ID="lblVersionDescription" runat="server" ></asp:Label>
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
                <td class="formCellLabel" >
                    Collection Zone
                </td>
                <td class="formCellField">
                    <telerik:RadComboBox runat="server" ID="cboCollectionZone" Width="150" OnClientSelectedIndexChanged="UpdateTableDescription"/>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel" >
                    Service Level
                </td>
                <td class="formCellField">
                    <telerik:RadComboBox runat="server" ID="cboServiceLevel" Width="150"  OnClientSelectedIndexChanged="UpdateTableDescription"/>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel" >
                    Pallet Type
                </td>
                <td class="formCellField">
                    <telerik:RadComboBox runat="server" ID="cboPalletType" Width="150" OnClientSelectedIndexChanged="UpdateTableDescription"/>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel" >
                    Goods Type
                </td>
                <td class="formCellField">
                    <telerik:RadComboBox runat="server" ID="cboGoodsType" Width="150" OnClientSelectedIndexChanged="UpdateTableDescription"/>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">
                    Table Description
                </td>
                <td class="formCellField">
                    <asp:TextBox ID="txtTableDescription" CssClass="fieldInputBox" runat="server" Width="250"
                        MaxLength="50"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvTableDescription" runat="server" ErrorMessage="Please enter a description for this tariff table"
                        ControlToValidate="txtTableDescription" Display="Dynamic">
                                    <img src="/images/Error.png" height="16" width="16" title="Please enter a description for this tariff table" />
                    </asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel" style="width: 150px;">
                    Multi Collect Rate
                </td>
                <td class="formCellField">
                    <telerik:RadNumericTextBox runat="server" ID="rntxtMultiCollectRate" MinValue="0" MaxValue="999" Width="60px" Type="Currency" Value="0.00"  /><asp:RequiredFieldValidator ID="rfvMultiCollectRate" ControlToValidate="rntxtMultiCollectRate" ErrorMessage="Please enter a Multi Collect rate or 0." Display="Dynamic"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel" style="width: 150px;">
                    Multi Drop Rate
                </td>
                <td class="formCellField">
                    <telerik:RadNumericTextBox runat="server" ID="rntxtMultiDropRate" MinValue="0" MaxValue="999" Width="60px" Type="Currency" Value="0.00"  /><asp:RequiredFieldValidator ID="rfvMultiDropRate" ControlToValidate="rntxtMultiDropRate" ErrorMessage="Please enter a Multi Drop rate or 0." Display="Dynamic"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel" style="width: 150px;">
                    High rate for multi-drop
                </td>
                <td class="formCellField">
                    <asp:CheckBox runat="server" ID="chkUseGreatestRateForPrimaryRate" Text="(The primary rate of the grouped order will be the highest within a zone.)"/>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <asp:RadioButton ID="optCopyTable" runat="server" Text="Copy rates from Tariff Table" GroupName="CreateTariff"
            Checked="true"></asp:RadioButton>
        <br />
        <div id="divCopyTable">
            <table style="margin-left: 50px">
                <tr>
                    <td class="formCellLabel" style="width: 150px;">
                        Tariff
                    </td>
                    <td class="formCellField">
                        <telerik:RadComboBox runat="server" ID="cboCopyFromTariff" Width="250" 
                        OnClientSelectedIndexChanged="cboCopyFromTariff_ClientSelectedIndexChanged" />
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Tariff Version
                    </td>
                    <td class="formCellField">
                        <telerik:RadComboBox runat="server" ID="cboCopyFromVersion" Width="250" 
                        OnClientSelectedIndexChanged="cboCopyFromVersion_ClientSelectedIndexChanged" 
                        OnClientItemsRequested="cboCopyFromVersion_ClientItemsRequested"/>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Tariff Table
                    </td>
                    <td class="formCellField">
                        <telerik:RadComboBox runat="server" ID="cboCopyFromTable" Width="250" 
                       OnClientItemsRequested="cboCopyFromTable_ClientItemsRequested"/>
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
        <asp:RadioButton ID="optEmptyTable" runat="server" Text="Empty Tariff Table" GroupName="CreateTariff">
        </asp:RadioButton>
        <br />
        <br />
        <asp:RadioButton ID="optImportTable" runat="server" Text="Import Tariff Table" GroupName="CreateTariff">
        </asp:RadioButton><div id="divImportTableMessage"  style="display: none;">Import instructions: Please copy and paste the rate table information into the box below. <br />The first column must contain the zone description. Subsequent columns must contain the rates, one column for each scale (in ascending scale value order).</div>
        <br />
        <asp:TextBox runat="server" id="txtImportRateTable" style="display: none;" TextMode="MultiLine" Width="700" Height="250"></asp:TextBox>
        <br />
        <asp:Label runat="server" ID="lblImportErrorMessage" ForeColor="Red"></asp:Label><div style="height: 10px;"></div>
        <br />
        <div class="buttonbar">
            <asp:Button ID="btnSave" runat="server" Text="Save" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CausesValidation="false" />
        </div>
    </fieldset>

    <script type="text/javascript">
        
        $(document).ready(function() {
            
            // Show the import rates text box
            $('input:radio[id*=optImportTable]').click(function () {
                $("#" + "<%=txtImportRateTable.ClientID %>").show();
                $('div[id*=divImportTableMessage]').show();
            });
            
            if($('input:radio[id*=optImportTable]')[0].checked) {
                $("#" + "<%=txtImportRateTable.ClientID %>").show();
            }
            
            // Hide the import rates text box when clicking the "Empty" radio button
            $('input:radio[id*=optEmptyTable]').click(function () {
                $("#" + "<%=txtImportRateTable.ClientID %>").hide();
                $('div[id*=divImportTableMessage]').hide();
                $('span[id*=lblImportErrorMessage]').hide();
            });
            
            // Hide the import rates text box when clicking the "Copy" radio button
            $('input:radio[id*=optCopyTable]').click(function () {
                $("#" + "<%=txtImportRateTable.ClientID %>").hide();
                $('div[id*=divImportTableMessage]').hide();
                $('span[id*=lblImportErrorMessage]').hide();
            });
        });
        
        //Trigger CopyFromTariff combo changed after initial page load
        //to cause child combos to load
        function pageLoad(sender, eventArgs) {
            if (!eventArgs.isPartialLoad)
                cboCopyFromTariff_ClientSelectedIndexChanged();
        }
        
        function cboCopyFromTariff_ClientSelectedIndexChanged(sender, eventArgs) {
            var cboCopyFromTariff = $find("<%= cboCopyFromTariff.ClientID %>");
            var cboCopyFromVersion = $find("<%= cboCopyFromVersion.ClientID %>");

            cboCopyFromVersion.clearSelection();
            cboCopyFromVersion.requestItems(cboCopyFromTariff.get_value(), false);
        }

        function cboCopyFromVersion_ClientSelectedIndexChanged(sender, eventArgs) {
            var cboCopyFromTable = $find("<%= cboCopyFromTable.ClientID %>");

            cboCopyFromTable.clearSelection();
            cboCopyFromTable.requestItems(eventArgs.get_item().get_value(), false);
        }

        function cboCopyFromVersion_ClientItemsRequested(sender, eventArgs) {
            var cboCopyFromVersion = $find("<%= cboCopyFromVersion.ClientID %>");

            var items = cboCopyFromVersion.get_items();

            if (items.get_count() > 0)
            {
                items.getItem(0).select();
                var defaultItem = cboCopyFromVersion.findItemByValue(<%= this.TariffVersionID %>);
                if (defaultItem != null)
                    defaultItem.select();
            }
        }

        function cboCopyFromTable_ClientItemsRequested(sender, eventArgs) {
            var cboCopyFromTable = $find("<%= cboCopyFromTable.ClientID %>");
            
            var items = sender.get_items();

            if (items.get_count() > 0)
            {
                items.getItem(0).select();
                var defaultItem = cboCopyFromTable.findItemByValue(<%= this.TariffTableID ?? 0 %>);
                if (defaultItem != null)
                    defaultItem.select();
            }
        }

        function UpdateTableDescription(sender, eventArgs) {
            var txtTableDescription = $get("<%= txtTableDescription.ClientID %>");
            var serviceLevel = $find("<%= cboServiceLevel.ClientID %>").get_text();
            var palletType = $find("<%= cboPalletType.ClientID %>").get_text();
            var goodsType =  $find("<%= cboGoodsType.ClientID %>").get_text();
            var collectionZone =  $find("<%= cboCollectionZone.ClientID %>").get_text();

            txtTableDescription.value = "";
            if (serviceLevel == "Any" && palletType == "Any" && goodsType == "Any" && collectionZone == "Any")
                txtTableDescription.value = "Default";
            else {
                if (collectionZone != "Any") 
                    txtTableDescription.value += collectionZone;
                                
                if (serviceLevel != "Any") {
                    if (txtTableDescription.value.length > 0)
                        txtTableDescription.value += " + ";
                    txtTableDescription.value += serviceLevel;
                }

                if (palletType != "Any") {
                    if (txtTableDescription.value.length > 0)
                        txtTableDescription.value += " + ";
                    txtTableDescription.value += palletType;
                }

                if (goodsType != "Any") {
                    if (txtTableDescription.value.length > 0)
                        txtTableDescription.value += " + ";
                    txtTableDescription.value += goodsType;
                }
                
                
            }
            txtTableDescription.value = txtTableDescription.value.substring(0, txtTableDescription.maxLength);
        }
        
        
        
    </script>

</asp:Content>
