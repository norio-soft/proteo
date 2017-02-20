<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true"
    CodeBehind="EditTariff.aspx.cs" Inherits="Orchestrator.WebUI.Tariff.EditTariff" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <link rel="stylesheet" href="//da7xgjtj801h2.cloudfront.net/2015.1.429/styles/kendo.common.min.css" />
    <link rel="stylesheet" href="//da7xgjtj801h2.cloudfront.net/2015.1.429/styles/kendo.material.min.css" />

    <style type="text/css">
        #tariffRates .RadInput_Default .riTextBox,
        #tariffRates .RadInputMgr_Default
        {
            text-align: left;
            background-color: Transparent;
            border: 0 none;
            width: 60px;
            height: 90%;
            padding-right: 4px;
            padding-bottom: 0; /*Otherwise the padding messes with the bounding div in IE*/
            padding-top: 0; /*Otherwise the padding messes with the bounding div in IE*/
            font-family: Verdana,Geneva,sans-serif !important;
            font-size: 11px !important;
        }

        .rateTableFieldset {
            width: 956px;
        }

        .stackContainer {
            width: 950px;
            -moz-column-count: 4;
            -webkit-column-count: 4;
            column-count: 4;
            -moz-column-gap: 4px;
            -webkit-column-gap: 4px;
            column-gap: 4px;
            word-spacing: 0;
            font-size: 0;
        }

        .stackContainer > .rgMasterTable
        {
            display: inline-block;
            border-collapse: collapse !important;
            width: 230px;
            margin-bottom: -1px;
        }

        .stackContainer .rgHeader
        {
            width: 100px;
            max-width: 100px;
            height: auto;
            text-overflow: ellipsis;
            vertical-align: top;
            padding-top: 8px;
            padding-bottom: 8px;
        }

        .stackContainer .rgRow > td
        {
            border: none;
        }

        .rateRangeTable td
        {
            border: none;
        }
        
        .rateRangeNone
        {
            color: #a5a5a5;
            padding: 4px;
        }

        .editExtraRatesTrigger
        {
            cursor: pointer;
        }

        .editExtraTypeRates
        {
            margin: 10px;
        }

        .editExtraTypeRates .rgMasterTable
        {
            border-collapse: collapse !important;
        }

        .editExtraTypeRates .rgMasterTable .rgRow > th.rgHeader {
            width: 60px;
            padding: 1px 3px !important;
        }

        .editExtraTypeRates .rgMasterTable .rgRow > td {
            padding: 0;
        }

        .editExtraTypeRates span.extraTypeScaleValueRate
        {
            width: 80px;
        }

        .editExtraTypeRates span.extraTypeScaleValueRate.dirty
        {
            background-color: #ffa;
        }

        .editExtraTypeRates span.extraTypeScaleValueRate span
        {
            background-color: transparent !important;
            border: none !important;
            padding: 0 !important;
        }

        .editExtraTypeRates span.extraTypeScaleValueRate input
        {
            background-color: transparent !important;
            border: none !important;
            padding: 2px 0 !important;
        }

        .editExtraTypeRates .buttonbar
        {
            margin-top: 12px;
        }

        .mainTable
        {
            table-layout: fixed;
            width: 1px; /* Failure to specify a width stops the columns from having their set width? */
        }

            .mainTable td,
            .metricTable td,
            .cornerTable td
            {
                width: 75px;
                height: 32px;
                overflow: hidden;
                white-space: nowrap;
            }

            .mainTable tr:hover
            {
                background-color: #e0e0e0;
            }


        div#header-container,
        div#scroll,
        div#x-fake-scroll
        {
            width: 85%;
        }

        div#metric-container,
        div#scroll,
        div#y-fake-scroll
        {
            max-height: 400px;
        }

        div#header-container
        {
            overflow: hidden;
            padding: 0px 0px 0 0px;
        }

        div#top-corner-container
        {
            float: left;
            overflow: hidden;
        }

        div#metric-container
        {
            width: 82px;
            position: relative;
            overflow: hidden;
            float: left;
        }

        div#scroll
        {
            overflow: hidden;
            padding-left: 1px;
            float: left;
        }

        div#fake-scroll-container
        {
            width: 950px;
            overflow: hidden;
            position: relative;
            border-style: solid;
            border-width: 1px;
            border-color: black;
        }

        div#y-fake-scroll
        {
            overflow-y: scroll;
            overflow-x: hidden;
            background: transparent;
            position: absolute;
            right: 0;
        }

        div#x-fake-scroll
        {
            height: 25px;
            margin-left: 75px;
            margin-top: -23px;
            overflow-x: auto;
            overflow-y: hidden;
            margin-top: expression('0px'); /* IE 7 fix*/
            height: expression('17px'); /* IE 7 fix*/
        }
    </style>

    <h1>Edit Tariff
    </h1>
    
    <telerik:RadInputManager runat="server" ID="rimTariffRates">
        <telerik:NumericTextBoxSetting Type="Number" BehaviorID="TariffRates" DecimalDigits="2" SelectionOnFocus="SelectAll" EmptyMessage="None" runat="server">
            <TargetControls>
                <telerik:TargetInput ControlID="grdTariffRates" Enabled="true" />
            </TargetControls>
        </telerik:NumericTextBoxSetting>
    </telerik:RadInputManager>
    
    <fieldset runat="server" id="fsTariff" class="invisiableFieldset">
        <div style="float: left; padding-right: 40px">
            <fieldset>
                <legend>Tariff</legend>
                <table width="520px">
                    <tr>
                        <td class="formCellLabel">Tariff Description
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
                        <td class="formCellLabel" style="width: 150px">Is For Sub Contractors
                        </td>
                        <td class="formCellField">
                            <asp:CheckBox runat="server" ID="chkIsForSubContractor" Enabled="False" />
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel">Zone Type
                        </td>
                        <td class="formCellField">
                            <asp:Label runat="server" ID="lblZoneType"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel">Metric
                        </td>
                        <td class="formCellField">
                            <asp:Label runat="server" ID="lblMetric"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel">Multiply Rate By Order Metric Value
                        </td>
                        <td class="formCellField">
                            <asp:CheckBox runat="server" ID="chkMultiplyByOrderValue" />
                            <asp:Label runat="server" ID="lblAdditionalMultiplier" Text="Additional Multiplier" AssociatedControlID="txtAdditionalMultiplier" Enabled="false" />
                            <telerik:RadNumericTextBox runat="server" ID="txtAdditionalMultiplier" Type="Number" NumberFormat-DecimalDigits="3" MinValue="0" Enabled="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel">Grouped Order Rate Determination
                        </td>
                        <td class="formCellField">
                            <asp:RadioButton runat="server" ID="rbGroupedOrderUseHighestRated" GroupName="rbGroupedOrderRateDetermination" Text="Use highest-rated order for group rate" Checked="true" />
                            <div style="margin: 2px 0 4px 16px;">
                                <asp:CheckBox runat="server" ID="chkIgnoreAdditionalCollectsFromADeliveryPoint" Text="Ignore additional collections from a delivery point" />
                            </div>

                            <asp:RadioButton runat="server" ID="rbGroupedOrderRateIndividually" GroupName="rbGroupedOrderRateDetermination" Text="Rate orders individually" />
                            <div style="margin: 2px 0 4px 16px;">
                                <asp:CheckBox runat="server" ID="chkRateCombinedWherePointsMatch" Text="Rate orders as one (pro-rata) where points match" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel">Tariff Version
                        </td>
                        <td class="formCellField">
                            <telerik:RadComboBox runat="server" ID="cboTariffVersion" Width="250" AutoPostBack="True" CausesValidation="False"
                                EmptyMessage=" ">
                            </telerik:RadComboBox>
                            &nbsp;<asp:Button ID="btnNewVersion" runat="server" Height="24px" Width="80px" Text="New Version" ToolTip="Create a new version. The existing version must have a finish date before a new version can be created." CausesValidation="False" />
                        </td>
                    </tr>
                </table>
            </fieldset>
            <fieldset>
                <legend>Selected Tariff Version</legend>
                <table width="520px">
                    <tr>
                        <td style="width: 150px" class="formCellLabel">Version Description
                        </td>
                        <td class="formCellField">
                            <asp:TextBox ID="txtVersionDescription" CssClass="fieldInputBox" runat="server" Width="250"
                                MaxLength="50"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvVersionDescription" runat="server" ErrorMessage="Please enter a description for this tariff version"
                                ControlToValidate="txtVersionDescription" Display="Dynamic">
                                     <img src="/images/Error.png" height="16" width="16" title="Please enter a description for this tariff version" />
                            </asp:RequiredFieldValidator>
                            <asp:Button ID="btnDeleteTariffVersion" runat="server" Height="24px" Width="80px" Text="Delete" CausesValidation="False" Style="vertical-align: bottom" />
                            <ajaxToolkit:ConfirmButtonExtender ID="btnDeleteTariffVersionConfirmation" runat="server"
                                TargetControlID="btnDeleteTariffVersion"
                                ConfirmText="Are you sure you want to delete this Tarfiff Version?"
                                DisplayModalPopupID="mpeDeleteTariffVersionConfirmation" />
                            <ajaxToolkit:ModalPopupExtender ID="mpeDeleteTariffVersionConfirmation" runat="server" TargetControlID="btnDeleteTariffVersion" PopupControlID="pnlDeleteTariffVersionConfirmation" OkControlID="ButtonOk" CancelControlID="ButtonCancel" BackgroundCssClass="modalBackground" />
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel" style="width: 150px">Zone Map
                        </td>
                        <td class="formCellField">
                            <asp:Label runat="server" ID="lblZoneMap" />
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel" style="width: 150px">Scale
                        </td>
                        <td class="formCellField">
                            <asp:Label runat="server" ID="lblScale" />
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel">Start Date
                        </td>
                        <td>
                            <telerik:RadDatePicker runat="server" ID="dteStartDate" Width="100px" />
                            <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ErrorMessage="Please enter a start date for this tariff"
                                ControlToValidate="dteStartDate" Display="Dynamic">
                                     <img src="/images/Error.png" height="16" width="16" title="Please enter a start date for this tariff" />
                            </asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel">Finish Date
                        </td>
                        <td>
                            <telerik:RadDatePicker runat="server" ID="dteFinishDate" Width="100px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel" style="width: 150px">Tariff Table
                        </td>
                        <td class="formCellField">
                            <telerik:RadComboBox runat="server" ID="cboTariffTable" Width="250" AutoPostBack="true" CausesValidation="False"
                                EmptyMessage="&lt;Empty - Click New Table&gt;">
                            </telerik:RadComboBox>
                            &nbsp;
                        <asp:Button ID="btnNewTable" runat="server" Height="24px" Width="80px" Text="New Table" CausesValidation="False" />
                        </td>
                    </tr>
                </table>
            </fieldset>
            <fieldset>
                <legend>Selected Tariff Table</legend>
                <table width="520px">
                    <tr>
                        <td style="width: 150px" class="formCellLabel">Table Description
                        </td>
                        <td class="formCellField">
                            <asp:TextBox ID="txtTableDescription" CssClass="fieldInputBox" runat="server" Width="250"
                                MaxLength="50"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvTableDescription" runat="server" ErrorMessage="Please enter a description for this tariff table"
                                ControlToValidate="txtTableDescription" Display="Dynamic">
                                     <img src="/images/Error.png" height="16" width="16" title="Please enter a description for this tariff table" />
                            </asp:RequiredFieldValidator>
                            <asp:Button ID="btnDeleteTariffTable" runat="server" Height="24px" Width="80px" Text="Delete" CausesValidation="False" Style="vertical-align: bottom" />
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel">Collection Zone
                        </td>
                        <td class="formCellField">
                            <telerik:RadComboBox runat="server" ID="cboCollectionZone" Width="150" />
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel" style="width: 150px;">Service Level
                        </td>
                        <td class="formCellField">
                            <telerik:RadComboBox runat="server" ID="cboServiceLevel" Width="150" />
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel" style="width: 150px;">Pallet Type
                        </td>
                        <td class="formCellField">
                            <telerik:RadComboBox runat="server" ID="cboPalletType" Width="150" />
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel" style="width: 150px;">Goods Type
                        </td>
                        <td class="formCellField">
                            <telerik:RadComboBox runat="server" ID="cboGoodsType" Width="150" />

                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel" style="width: 150px;">Multi Collect Rate
                        </td>
                        <td class="formCellField">
                            <telerik:RadNumericTextBox runat="server" ID="rntxtMultiCollectRate" MinValue="0" MaxValue="999" Width="60px" Type="Currency" />
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel" style="width: 150px;">Multi Drop Rate
                        </td>
                        <td class="formCellField">
                            <telerik:RadNumericTextBox runat="server" ID="rntxtMultiDropRate" MinValue="0" MaxValue="999" Width="60px" Type="Currency" />
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel" style="width: 150px;">High rate for multi-drop
                        </td>
                        <td class="formCellField">
                            <table>
                                <tr>
                                    <td>
                                        <asp:CheckBox runat="server" ID="chkUseGreatestRateForPrimaryRate"></asp:CheckBox>
                                    </td>
                                    <td>
                                        <label for="<%=this.chkUseGreatestRateForPrimaryRate.ClientID %>">
                                            (The primary rate of the grouped order<br />
                                            will be the highest within a zone.)
                                        </label>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </fieldset>
            <div class="buttonbar">
                <asp:Button ID="btnSave" runat="server" Text="Save Changes" OnClientClick="btnSave_clientClick()" />
                <asp:Button ID="btnTariffList" runat="server" Text="Tariff List" CausesValidation="false" />
            </div>
        </div>

        <div>
            <fieldset class="rateTableFieldset">
                <legend>Selected Tariff Version Extra Rates</legend>
                <asp:Panel runat="server" ID="pnlExtraTypeRates" CssClass="stackContainer"></asp:Panel>
            </fieldset>
            
            <fieldset id="tariffRates" class="rateTableFieldset">
                <legend>Selected Tariff Table Rates</legend>

                <div id="fake-scroll-container">

                    <div id="top-corner-container">
                        <table runat="server" id="grdTariffRatesCorner" class="cornerTable" cellpadding="0" cellspacing="0"></table>
                    </div>

                    <div id="header-container">
                        <table runat="server" id="grdTariffRatesHeader" class="mainTable" cellpadding="0" cellspacing="0"></table>
                    </div>

                    <div id="metric-container">
                        <table runat="server" id="grdTariffRatesMetric" class="metricTable" cellpadding="0" cellspacing="0"></table>
                    </div>

                    <div id="scroll">
                        <table runat="server" id="grdTariffRates" class="mainTable" cellpadding="0" cellspacing="0"></table>
                    </div>

                    <div id="y-fake-scroll">
                        <div id="y-table-emulator" style="width: 40px;">
                            &nbsp;
                        </div>
                    </div>

                    <div id="x-fake-scroll">
                        <div id="x-table-emulator">
                            &nbsp;
                        </div>
                    </div>
                </div>

            </fieldset>

            <input type="hidden" runat="server" id="hidTariffRateChanges" />
            <asp:HiddenField runat="server" ID="hidExtraTypeRates" ClientIDMode="Static" />
            <asp:HiddenField runat="server" ID="hidExtraTypeRateChanges" ClientIDMode="Static" />
        </div>
    </fieldset>

    <asp:Panel ID="pnlDeleteTariffVersionConfirmation" runat="server" Style="width: 400px; background-color: white; border-width: 2px; border-color: black; border-style: solid; padding: 20px; color: Silver; display: none;">
        <h2>Confirmation Needed</h2>
        <p>
            Are you sure you want to delete this Tariff Version?
        </p>
        <div class="buttonBar">
            <asp:Button ID="ButtonOk" runat="server" Text="I am sure" />
            <asp:Button ID="ButtonCancel" runat="server" Text="Cancel" />
        </div>
    </asp:Panel>

    <telerik:RadToolTip runat="server" ID="radToolTip" EnableShadow="true" ShowEvent="FromCode" HideEvent="ManualClose" Width="250px"
        RelativeTo="Element" Position="Center" MouseTrailing="true" ShowCallout="false" ShowDelay="500" />

    <script src="//da7xgjtj801h2.cloudfront.net/2015.1.429/js/kendo.all.min.js"></script>
    <script src="//da7xgjtj801h2.cloudfront.net/2015.1.429/js/cultures/kendo.culture.en-GB.min.js"></script>

    <script src="../script/handlebars-1.0.rc.2.js"></script>
    <script src="../script/templates.js"></script>

    <script type="text/javascript">
        var YtableEmulator = document.getElementById('y-table-emulator');
        var XtableEmulator = document.getElementById('x-table-emulator');
        var table = document.getElementById('<%= grdTariffRates.ClientID %>');

        YtableEmulator.style.height = table.clientHeight == 0 ? "330px" : table.clientHeight + 39 + "px";
        XtableEmulator.style.width = table.clientWidth + 75 + "px";

        var scrollablePanel = document.getElementById('scroll');
        var headerContainer = document.getElementById('header-container');
        var metricContainer = document.getElementById('metric-container');
        var YfakeScrollablePanel = document.getElementById('y-fake-scroll');
        var XfakeScrollablePanel = document.getElementById('x-fake-scroll');

        scrollablePanel.onscroll = function (e) {
            XfakeScrollablePanel.scrollTop = scrollablePanel.scrollTop;
        }

        YfakeScrollablePanel.onscroll = function (e) {
            scrollablePanel.scrollTop = YfakeScrollablePanel.scrollTop;
            metricContainer.scrollTop = YfakeScrollablePanel.scrollTop;
        }

        XfakeScrollablePanel.onscroll = function (e) {
            scrollablePanel.scrollLeft = XfakeScrollablePanel.scrollLeft;
            headerContainer.scrollLeft = XfakeScrollablePanel.scrollLeft;
        }

        var extraTypesWithRates = JSON.parse($get('hidExtraTypeRates').value);

        $(function () {
            groupedOrderRateDeterminationChange($("#<%= rbGroupedOrderUseHighestRated.ClientID %>").prop("checked"));

            $("#<%= chkMultiplyByOrderValue.ClientID %>").click(function () {
                configureAdditionalMultiplierField(this.checked);
            });

            $("#<%= rbGroupedOrderUseHighestRated.ClientID %>").click(function () {
                groupedOrderRateDeterminationChange(true);
            });

            $("#<%= rbGroupedOrderRateIndividually.ClientID %>").click(function () {
                groupedOrderRateDeterminationChange(false);
            });

            initializeExtraRateTypeSummaries();

            var $document = $(document);

            $document.on('click', '.editExtraRatesTrigger', editExtraRatesTrigger_click);

            $document.on('change', '.extraTypeScaleValueRate', function () {
                $(this).addClass('dirty');
            });

            $document.on('focus', '.extraTypeScaleValueRate', function () {
                var element = this;
                setTimeout(function () { $(element).select(); });
            });

            $document.on('click', '#btnExtraTypeRatesUpdate', function (e) { e.preventDefault(); updateExtraTypeRates(); });
            $document.on('click', '#btnExtraTypeRatesClearAll', function (e) { e.preventDefault(); clearExtraTypeRates(); });
            $document.on('click', '#btnExtraTypeRatesCopyDown', function (e) { e.preventDefault(); copyDownExtraTypeRates(); });
        });

        function initializeExtraRateTypeSummaries() {
            extraTypesWithRates.forEach(function (etr) {
                displayExtraTypeRateSummary(etr);
            });
        }

        function displayExtraTypeRateSummary(extraTypeRates) {
            var scaleValueRates = extraTypeRates.scaleValueRates;

            var rateRanges = [];
            var fromScaleValue = null;
            var toScaleValue = null;
            var currentRate = null;

            var createRateRange = function (fromScaleValue, toScaleValue, rate) {
                var scaleRangeText = (toScaleValue && toScaleValue !== fromScaleValue) ? String.format("{0} to {1}", fromScaleValue, toScaleValue) : fromScaleValue.toString();

                return {
                    scales: scaleRangeText,
                    rate: rate.toFixed(2)
                };
            };

            scaleValueRates.forEach(function (svr) {
                if (fromScaleValue === null || svr.rate !== currentRate) {
                    if (fromScaleValue !== null && currentRate !== null) {
                        rateRanges.push(createRateRange(fromScaleValue, toScaleValue, currentRate));
                    }

                    fromScaleValue = svr.scaleValue;
                }

                toScaleValue = svr.scaleValue;
                currentRate = svr.rate;
            });

            if (currentRate !== null) {
                rateRanges.push(createRateRange(fromScaleValue, toScaleValue, currentRate));
            }

            var compiledTemplate = getTemplate('extratyperatesummary');
            var html = compiledTemplate({ rateRanges: rateRanges });

            $('.extraTypeRateSummary[data-extra-type-id=' + extraTypeRates.extraTypeID + ']').html(html);
        }

        function displayRateSummaryForExtraTypeID(extraTypeID) {
            var extraTypeRates = getExtraTypeRates(extraTypeID);
            displayExtraTypeRateSummary(extraTypeRates);
        }

        function editExtraRatesTrigger_click() {
            var tooltip = $find('<%= radToolTip.ClientID %>');
            tooltip.set_targetControl(this);

            var extraTypeID = $(this).parent().data('extraTypeId');
            var extraTypeRates = getExtraTypeRates(extraTypeID);

            var compiledTemplate = getTemplate('extratyperates');
            var html = compiledTemplate(extraTypeRates);

            tooltip.set_content(html);
            tooltip.show();

            var $tooltipContent = $(tooltip.get_contentElement());
            $tooltipContent.data('extraTypeId', extraTypeID);

            var $inputs = $tooltipContent.find('.extraTypeScaleValueRate');

            $inputs.kendoNumericTextBox({
                decimals: 2,
                min: 0,
                placeholder: 'None',
                spinners: false
            });

            $inputs.on('keypress', function (e) {
                if (e.which === 13) {
                    updateExtraTypeRates();
                }
            });
        }

        function getExtraTypeRates(extraTypeID) {
            return extraTypesWithRates.filter(function (i) {
                return i.extraTypeID === extraTypeID;
            })[0];
        }

        function updateExtraTypeRates() {
            var tooltip = $find('<%= radToolTip.ClientID %>');
            var $tooltipContent = $(tooltip.get_contentElement());
            var extraTypeID = $tooltipContent.data('extraTypeId');

            tooltip.hide();

            var $inputs = $tooltipContent.find('.extraTypeScaleValueRate');

            var $dirty = $inputs.filter(function () {
                return $(this).hasClass('dirty');
            })

            if ($dirty.length === 0)
                return;

            var scaleValueRates = getExtraTypeRates(extraTypeID).scaleValueRates;

            $dirty.each(function () {
                var $input = $(this);

                var scaleValueRate = scaleValueRates.filter(function (sv) {
                    return sv.scaleValue == $input.data('scaleValue');
                });
                
                if (scaleValueRate.length > 0) {
                    scaleValueRate[0].isDirty = true;
                    scaleValueRate[0].rate = parseFloat($input.val()) || null;
                }
            });

            displayRateSummaryForExtraTypeID(extraTypeID);
        };

        function clearExtraTypeRates() {
            var tooltip = $find('<%= radToolTip.ClientID %>');
            var $tooltipContent = $(tooltip.get_contentElement());
            var $inputs = $tooltipContent.find('.extraTypeScaleValueRate');

            $inputs.val(null).addClass('dirty');
        }

        function copyDownExtraTypeRates() {
            var tooltip = $find('<%= radToolTip.ClientID %>');
            var $tooltipContent = $(tooltip.get_contentElement());
            var $inputs = $tooltipContent.find('.extraTypeScaleValueRate');

            var previousRate = null;
            var foundNonEmptyRate = false;
            var foundEmptyRate = false;

            $inputs.each(function (idx, input) {
                var $input = $(input);
                var hasRate = !!$input.val();

                foundNonEmptyRate |= hasRate;
                foundEmptyRate |= !hasRate;
                
                if (previousRate && !hasRate) {
                    $input.val(previousRate).addClass('dirty');
                }

                previousRate = $input.val();
            });

            if (!foundEmptyRate) {
                alert('No empty rates found.  Clicking this button causes rates to be copied down to non-rated cells below, but there were no applicable cells to copy to.')
            }
            else if (!foundNonEmptyRate) {
                alert('No non-empty rates found.  Clicking this button causes rates to be copied down to non-rated cells below, but there were no rates to copy.')
            }
        }

        function radToolTip_clientShow(sender, eventArgs) {
            setTimeout(function () {
                sender.updateLocation();
            }, 500);
        }

        function btnSave_clientClick(sender, eventArgs) {
            var updatedExtraTypeRates = extraTypesWithRates.filter(function (et) {
                var updatedScaleValueRates = et.scaleValueRates.filter(function (svr) {
                    return svr.isDirty;
                });

                return updatedScaleValueRates.length > 0;
            });

            $get('hidExtraTypeRateChanges').value = JSON.stringify(updatedExtraTypeRates);
        };

        function groupedOrderRateDeterminationChange(useHighestRated) {
            var chkIgnoreAdditionalCollectsFromADeliveryPoint = $("#<%= chkIgnoreAdditionalCollectsFromADeliveryPoint.ClientID %>");
            var chkRateCombinedWherePointsMatch = $("#<%= chkRateCombinedWherePointsMatch.ClientID %>");

            if (useHighestRated) {
                chkRateCombinedWherePointsMatch.prop("checked", false);
            }
            else {
                chkIgnoreAdditionalCollectsFromADeliveryPoint.prop("checked", false);
            }

            chkIgnoreAdditionalCollectsFromADeliveryPoint.prop("disabled", !useHighestRated);
            chkRateCombinedWherePointsMatch.prop("disabled", useHighestRated);
        }

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

        function updateVersionDescription() {
            var tariffDesc = document.getElementById("<%= txtTariffDescription.ClientID %>").value;
            var startDate = $find("<%= dteStartDate.ClientID %>").get_value();

            if (tariffDesc.length > 0 && startDate.length > 0) {
                var _txtVersionDescription = document.getElementById("<%= txtVersionDescription.ClientID %>");
                _txtVersionDescription.value = tariffDesc + " - " + startDate;
            }
        }

        function TariffRate_onchange(input) {
            var hidTariffRateChanges = $get("<%= hidTariffRateChanges.ClientID %>");
            if (hidTariffRateChanges.value.length > 0)
                hidTariffRateChanges.value += ",";
            hidTariffRateChanges.value += input.name;
            input.parentElement.style.backgroundColor = "#ffa";
        }

    </script>

</asp:Content>
