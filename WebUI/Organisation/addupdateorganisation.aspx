<%@ Page Language="c#" Inherits="Orchestrator.WebUI.Organisation.addupdateorganisation"
    MasterPageFile="~/default_tableless.master" CodeBehind="addupdateorganisation.aspx.cs"
    AutoEventWireup="True" %>

<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>
<%@ Register TagPrefix="uc" Namespace="Orchestrator.WebUI.Controls" Assembly="WebUI" %>
<%@ Register TagPrefix="dlg" Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">

    <script language="javascript" type="text/javascript">
	<!--
    var chkMustCaptureDebrief = null;

    $(document).ready(function () {
        
        AddressLine1 = $('input[id*=<%=tcAddressLine1.ClientID%>]')[0];
        AddressLine2 = $('input[id*=<%=tcAddressLine2.ClientID%>]')[0];
        AddressLine3 = $('input[id*=<%=tcAddressLine3.ClientID%>]')[0];
        PostTown = $('input[id*=<%=tcPostTown.ClientID%>]')[0];
        County = $('input[id*=<%=tcCounty.ClientID%>]')[0];
        PostCode = $('input[id*=<%=tcPostCode.ClientID%>]')[0];
        longitude = $('input[id*=<%=tcLongitude.ClientID%>]')[0];
        latitude = $('input[id*=<%=tcLatitude.ClientID%>]')[0];
        TrafficArea = $('input[id*=<%=tcTrafficArea.ClientID%>]')[0];
        searchCompany = $('input[id*=<%=tcOrganisationName.ClientID%>]')[0];
        searchTown = $('input[id*=<%=tcPostTown.ClientID%>]')[0];
        setPointRadius = $('input[id*=<%=hdnSetPointRadius.ClientID %>]')[0];
    });

    var AddressLine1 = null;
    var AddressLine2 = null;
    var AddressLine3 = null;
    var PostTown = null;
    var County = null;
    var PostCode = null;
    var longitude = null;
    var latitude = null;
    var TrafficArea = null;
    var searchCompany = null;
    var searchTown = null;
    var setPointRadius = null;

    function openChecker() {

        AddressLine1 = $('input[id*=<%=tcAddressLine1.ClientID%>]')[0];
		    AddressLine2 = $('input[id*=<%=tcAddressLine2.ClientID%>]')[0];
		    AddressLine3 = $('input[id*=<%=tcAddressLine3.ClientID%>]')[0];
		    PostTown = $('input[id*=<%=tcPostTown.ClientID%>]')[0];
		    County = $('input[id*=<%=tcCounty.ClientID%>]')[0];
		    PostCode = $('input[id*=<%=tcPostCode.ClientID%>]')[0];
		    longitude = $('input[id*=<%=tcLongitude.ClientID%>]')[0];
		    latitude = $('input[id*=<%=tcLatitude.ClientID%>]')[0];
		    TrafficArea = $('input[id*=<%=tcTrafficArea.ClientID%>]')[0];
		    searchCompany = $('input[id*=<%=tcOrganisationName.ClientID%>]')[0];
		    searchTown = $('input[id*=<%=tcPostTown.ClientID%>]')[0];
		    setPointRadius = $('input[id*=<%=hdnSetPointRadius.ClientID %>]')[0];

		    var sURL = "../addresslookup/fullwizard.aspx?";

		    sURL += "PostCode=" + PostCode.value;
		    sURL += "&searchCompany=" + searchCompany.value;
		    sURL += "&searchTown=" + searchTown.value;

		    window.open(sURL, "Checker", "toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=0,resizable=0,width=560,height=450");
    }

        function openJobDetails(jobId) {
            openDialogWithScrollbars('../Job/Job.aspx?wiz=true&jobId=' + jobId, '1220', '800');
        }

		function pageLoad() {

		    // --------------------------------
		    // START OF FUEL SURCHARGE SETTINGS
		    // --------------------------------

		    var rntFuelSurchargeOverride = $find("<%=rntFuelSurchargeOverride.ClientID%>");
		    var rntFuelSurchargeAdjustment = $find("<%=rntFuelSurchargeAdjustment.ClientID%>");

		    var strFuelSurchargeOverride = "<%=rdFuelSurchargeOverride.ClientID%>";
		    var strFuelSurchargeAdjustment = "<%=rdFuelSurchargeAdjustment.ClientID%>";

		    var rdFuelSurchargeOverride = $('input:radio[id=' + strFuelSurchargeOverride + ']')[0];
		    var rdFuelSurchargeAdjustment = $('input:radio[id=' + strFuelSurchargeAdjustment + ']')[0];

		    if (rdFuelSurchargeOverride.checked == true) {
		        rntFuelSurchargeOverride.enable();
		    } else {
		        rntFuelSurchargeOverride.disable();
		    }

		    if (rdFuelSurchargeAdjustment.checked == true) {
		        rntFuelSurchargeAdjustment.enable();
		    } else {
		        rntFuelSurchargeAdjustment.disable();
		    }

		    // --------------------------------
		    // END OF FUEL SURCHARGE SETTINGS
		    // --------------------------------

		    var chkSrc = $get("<%=tcAutoEmailInvoices.ClientID %>");
		    updateInvoiceEmailAddressVisibility(chkSrc);

		    var chkSusSrc = $get("<%=tcClientSuspended.ClientID %>");
			SuspendedChanged(chkSusSrc);

			var chkOnHoldSrc = $get("<%=tcClientOnHold.ClientID %>");
			OnHoldChanged(chkOnHoldSrc);

			$('#<%=tcAutoEmailInvoices.ClientID%>').click(function () {
			    updateInvoiceEmailAddressVisibility(this);
			});

			$('#<%=tcClientSuspended.ClientID%>').click(function () {
		        SuspendedChanged(this);
		    });

		    $('#<%=tcClientOnHold.ClientID%>').click(function () {
		        OnHoldChanged(this);
		    });
		}

		function updateInvoiceEmailAddressVisibility(src) {
		    if (src.checked) {
		        $('#<%=tcInvoiceEmailAddress.ClientID %>').show();
		        $('#<%=tcInvoiceEmailAddress.ClientID %>').focus();
		    }
		    else {
		        $('#<%=tcInvoiceEmailAddress.ClientID %>').hide();
		    }
        }

        function fuelSurchargeRadioOptionChecked(rd) {

            var rntFuelSurchargeOverride = $find("<%=rntFuelSurchargeOverride.ClientID%>");
            var rntFuelSurchargeAdjustment = $find("<%=rntFuelSurchargeAdjustment.ClientID%>");

            if (rd.value == 'rdFuelSurchargeOverride') {
                rntFuelSurchargeAdjustment.disable();
                rntFuelSurchargeOverride.enable();
            }

            if (rd.value == 'rdFuelSurchargeAdjustment') {
                rntFuelSurchargeAdjustment.enable();
                rntFuelSurchargeOverride.disable();
            }

            if (rd.value == 'rdFuelSurchargeStandard') {
                rntFuelSurchargeAdjustment.disable();
                rntFuelSurchargeOverride.disable();
            }
        }

        function CountryOnClientSelectedIndexChanged(item) {
            var townCombo = $find("<%=cboClosestTown.ClientID %>");
		    townCombo.set_text("");
		    townCombo.requestItems(item.get_value(), false);
		}

		function ClosestTownRequesting(sender, eventArgs) {
		    var countryCombo = $find("<%=cboCountry.ClientID %>");

		    var context = eventArgs.get_context();
		    context["CountryId"] = countryCombo.get_value();
		}

		function RemoveRelationship(relatedIdentityId, identityId, organisation) {
		    var response = confirm("Are you sure you want to remove " + organisation + " from the list of customers?\n Click OK to Remove or Cancel to go back.");
		    if (response) {
		        alert('make the call');
		    }
		}

		
        function subcontractAllocatedOrders() {
            Orchestrator.WebUI.Services.Allocation.SubcontractAllocatedOrders(
				parseInt("<%= m_identityId %>", 10),
				"<%= Page.User.Identity.Name %>",
				subcontractAllocatedOrders_Success,
				subcontractAllocatedOrders_Failure);
        }

        function subcontractAllocatedOrders_Success(result) {
            var res = Sys.Serialization.JavaScriptSerializer.deserialize(result);
            var orderCount = res.OrderCount;
            var businessRuleInfringements = res.BusinessRuleInfringements;

            var msg =
				orderCount == 0
				? (businessRuleInfringements ? "No allocated orders were subcontracted." : "There are no allocated orders to be subcontracted for this client.")
				: (orderCount + " order(s) have been subcontracted.");

            if (businessRuleInfringements) {
                msg += "\n\nSubcontracting the following orders was unsuccessful:\n\n" + businessRuleInfringements;
            }

            alert(msg);
        }

        function subcontractAllocatedOrders_Failure(error) {
            alert("Subcontracting orders failed.  Please contact Orchestrator support to resolve this problem.");
        }

        function SuspendedChanged(src) {
            if (src.checked) {
                $('#<%=tcClientSuspendedReason.ClientID %>').show();
            }
            else {
                $('#<%=tcClientSuspendedReason.ClientID %>').hide();
            }
        }

        function OnHoldChanged(src) {
            if (src.checked) {
                $('#<%=tcClientOnHoldReason.ClientID %>').show();
            }
            else {
                $('#<%=tcClientOnHoldReason.ClientID %>').hide();
            }
        }


        function toggleTariffType(isPerBusinessType) {
            $("div[id$=pnlTariffPerClient]").toggle(!isPerBusinessType);
            $("div[id$=pnlTariffPerBusinessType]").toggle(isPerBusinessType);
        }

        // The Combostreamers returns HTML which formats the Points in the drop-down. But when selected, the HTML itself will show.
        // Strip the HTML and other address information out leaving only the Point Name/Description.
        function Point_CombBoxClosing(sender, eventArgs) {

            try {
                var itemText = sender.get_selectedItem().get_text();

                if (itemText.indexOf('</td><td>') > 0) {
                    // remove any html characters from this. and Show the Point Name only
                    var pointName = itemText.split('</td><td>')[0];

                    pointName = pointName.replace(/&(lt|gt);/g, function (strMatch, p1) {
                        return (p1 == "lt") ? "<" : ">";
                    });
                    pointName = pointName.replace(/<\/?[^>]+(>|$)/g, "");
                    sender.set_text(pointName);
                }

            }
            catch (e) { }

        }
        //-->
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>Add/Update Client</h1>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManagerProxy ID="scriptManagerProxy" runat="server">
        <Services>
            <asp:ServiceReference Path="~/Services/Allocation.svc" />
        </Services>
    </asp:ScriptManagerProxy>
    <dlg:Dialog ID="dlgScan" URL="/scan/wizard/ScanOrUpload.aspx" Width="550" Height="550"
        AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true" OnDialogCallBack="dlgScan_DialogCallBack" />
    <div class="pagetitleTabs">
        <h1>
            <asp:Label ID="lblOrganisationName" runat="server">Add New Client</asp:Label></h1>
        <ajaxToolkit:TextBoxWatermarkExtender ID="tbwmVatNumber" runat="server" WatermarkText="NOT SET"
            WatermarkCssClass="watermarked" />
        <uc1:infringementDisplay ID="infringementDisplay" runat="server" Visible="false"></uc1:infringementDisplay>
        <telerik:RadTabStrip ID="RadTabStrip1" runat="server" Width="100%" Skin="Orchestrator"
            EnableEmbeddedSkins="false" CausesValidation="false" MultiPageID="RadMultiPage1"
            SelectedIndex="0">
            <Tabs>
                <telerik:RadTab AccessKey="d" Text="&lt;u&gt;D&lt;/u&gt;etails" PageViewID="DetailsView">
                </telerik:RadTab>
                <telerik:RadTab AccessKey="r" Text="&lt;u&gt;R&lt;/u&gt;eferences" PageViewID="ReferencesView">
                </telerik:RadTab>
                <telerik:RadTab AccessKey="l" Text="&lt;u&gt;L&lt;/u&gt;ocations" PageViewID="LocationsView">
                </telerik:RadTab>
                <telerik:RadTab AccessKey="s" Text="&lt;u&gt;S&lt;/u&gt;ettings" PageViewID="SettingsView">
                </telerik:RadTab>
                <telerik:RadTab AccessKey="g" Text="Lo&lt;u&gt;g&lt;/u&gt; Settings" Visible="false"
                    PageViewID="LogSettingsView">
                </telerik:RadTab>
                <telerik:RadTab AccessKey="r" Text="Re&lt;u&gt;p&lt;/u&gt;ort Settings" PageViewID="ReportSettingsView">
                </telerik:RadTab>
                <telerik:RadTab AccessKey="g" Text="&lt;u&gt;G&lt;/u&gt;roupage" PageViewID="GroupageView">
                </telerik:RadTab>
                <telerik:RadTab AccessKey="c" Text="&lt;u&gt;C&lt;/u&gt;lient Customers" PageViewID="ClientCustomersView">
                </telerik:RadTab>
                <telerik:RadTab AccessKey="t" Text="Con&lt;u&gt;t&lt;/u&gt;acts" PageViewID="ContactsView">
                </telerik:RadTab>
                <telerik:RadTab AccessKey="e" Text="&lt;u&gt;E&lt;/u&gt;xport Messages" PageViewID="ExportMessagesView">
                </telerik:RadTab>
            </Tabs>
        </telerik:RadTabStrip>
    </div>
    <asp:Panel ID="pnlConfirmation" runat="server">
        <div class="infoPanel">
            <asp:Image runat="server" ID="imgIcon" Visible="False" ImageUrl="~/images/ico_info.gif"
                alt="information" />
            <asp:Label ID="lblConfirmation" runat="server" Visible="false" CssClass="confirmation"></asp:Label>
        </div>
    </asp:Panel>
    <telerik:RadMultiPage ID="RadMultiPage1" runat="server" BackColor="White" SelectedIndex="0">
        <telerik:RadPageView ID="DetailsView" runat="server">
            <table cellspacing="0" cellpadding="0">
                <tr>
                    <td valign="top" width="50%">
                        <div class="formBlock" style="width: 450px;">
                            <h1>Details</h1>
                            <table>
                                <tr>
                                    <td class="formCellLabel" style="width: 120px;">Name
                                    </td>
                                    <td class="formCellField">
                                        <asp:TextBox ID="txtOrganisationName" CssClass="fieldInputBox" runat="server" Width="250"
                                            MaxLength="30"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvOrganisationName" runat="server" EnableClientScript="False"
                                            ErrorMessage="Please enter the Organisation Name." ControlToValidate="txtOrganisationName"
                                            Display="Dynamic">
											<img src="/images/Error.gif" height="16" width="16" title="Please enter the Organisation Name." alt="" />
                                        </asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formCellLabel">Display Name
                                    </td>
                                    <td class="formCellField">
                                        <asp:TextBox ID="txtOrganisationDisplayName" CssClass="fieldInputBox" runat="server"
                                            Width="250" ToolTip="This is the text that will appear as the company name on any invoices for this organisation."></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvOrganisationDisplayName" runat="server" EnableClientScript="False"
                                            ErrorMessage="Please enter the Organisation Display Name." ControlToValidate="txtOrganisationDisplayName"
                                            Display="Dynamic">
											<img src="/images/Error.gif" height="16" width="16" title="Please enter the Organisation Display Name." alt="" />
                                        </asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formCellLabel">Main Phone Number
                                    </td>
                                    <td class="formCellField">
                                        <asp:TextBox ID="txtTelephone" CssClass="fieldInputBox" runat="server" MaxLength="25"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formCellLabel">Fax Number
                                    </td>
                                    <td class="formCellField">
                                        <asp:TextBox ID="txtFaxNumber" CssClass="fieldInputBox" runat="server" MaxLength="25"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td>
                                        <asp:CheckBox ID="chkIsSubContractor" runat="server" Text="Can be used as a Sub-Contractor"></asp:CheckBox>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                    <td runat="server" id="tdFinancial">
                        <div class="formBlock" style="width: 450px;">
                            <h1>Financial</h1>
                            <table>
                                <tr>
                                    <td class="formCellLabel" style="width: 120px;">Account Code
                                    </td>
                                    <td class="formCellField">
                                        <asp:TextBox ID="txtAccountCode" CssClass="fieldInputBox" runat="server" MaxLength="6"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formCellLabel">VAT Number
                                    </td>
                                    <td class="formCellField">
                                        <asp:TextBox ID="txtVATNumber" CssClass="fieldInputBox" runat="server"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formCellLabel">Nominal Code
                                    </td>
                                    <td class="formCellField" valign="top">
                                        <telerik:RadComboBox ID="cboNominalCode" runat="server" HighlightTemplatedItems="true"
                                            Width="230px">
                                            <HeaderTemplate>
                                                <table style="width: 230px; text-align: left;">
                                                    <tr>
                                                        <td style="width: 80px;">Code
                                                        </td>
                                                        <td style="width: 150px;">Description
                                                        </td>
                                                    </tr>
                                                </table>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <table>
                                                    <tr>
                                                        <td style="width: 80px;">
                                                            <%#DataBinder.Eval(Container.DataItem, "NominalCode")%>
                                                        </td>
                                                        <td style="width: 150px;">
                                                            <%#DataBinder.Eval(Container.DataItem, "Description") %>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ItemTemplate>
                                        </telerik:RadComboBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formCellLabel">
                                        <asp:Label ID="lblCreditLimit" runat="server" Text="Credit Limit"></asp:Label>
                                    </td>
                                    <td class="formCellField">
                                        <telerik:RadNumericTextBox ID="txtCreditLimit" CssClass="fieldInputBox" runat="server"
                                            Type="Currency">
                                        </telerik:RadNumericTextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formCellLabel">
                                        <asp:Label ID="lblCostCentre" runat="server" Text="Cost Centre"></asp:Label>
                                    </td>
                                    <td class="formCellField">
                                        <asp:TextBox runat="server" CssClass="fieldInputBox" ID="txtCostCentre" MaxLength="20"></asp:TextBox><br />
                                        <asp:CustomValidator ID="cfvCostCentre" runat="server" ControlToValidate="txtCostCentre"
                                            ErrorMessage="Cost Centre must not exceed 20 characters in length." EnableClientScript="false"></asp:CustomValidator>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div class="formBlock" style="width: 450px; height: 262px">
                            <h1>Head Office Address</h1>
                            <asp:Panel ID="pnlAddress" runat="server">
                                <table>
                                    <tr>
                                        <td class="formCellLabel">Postal Code
                                        </td>
                                        <td class="formCellField">
                                            <table cellpadding="0" cellspacing="0">
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="txtPostCode" CssClass="fieldInputBox" runat="server" MaxLength="10"></asp:TextBox>
                                                        <asp:RequiredFieldValidator ID="rfvPostCode" runat="server" EnableClientScript="False"
                                                            ErrorMessage="Please enter the Post Code." ControlToValidate="txtPostCode" Display="Dynamic">
														<img src="/images/Error.gif" height="16" width="16" title="Please enter the Post Code." alt="" />
                                                        </asp:RequiredFieldValidator>
                                                    </td>
                                                    <td style="padding: 0 0 0 5px;">
                                                        <span>
                                                            <asp:LinkButton ID="lnkLookUp" runat="server" Text="Find" Style="font-size: 10px;" ValidationGroup="Lookup"></asp:LinkButton>
                                                        </span>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="formCellLabel" style="width: 120px;">Country
                                        </td>
                                        <td class="formCellField" colspan="2">
                                            <telerik:RadComboBox ID="cboCountry" runat="server" EnableLoadOnDemand="false" ItemRequestTimeout="500"
                                                AutoPostBack="true" MarkFirstMatch="true" AllowCustomText="false" OnClientSelectedIndexChanged="CountryOnClientSelectedIndexChanged"
                                                ShowMoreResultsBox="false" Width="250px" Height="200px" Overlay="true" />
                                            <asp:RequiredFieldValidator ID="rfvCountry" runat="server" ControlToValidate="cboCountry"
                                                ErrorMessage="<img src='/images/error.png' Title='Please select a country.'>"
                                                EnableClientScript="False"></asp:RequiredFieldValidator>
                                        </td>
                                        <td>&nbsp;
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="formCellLabel">Closest Town
                                        </td>
                                        <td class="formCellField" colspan="2">
                                            <telerik:RadComboBox ID="cboClosestTown" runat="server" EnableLoadOnDemand="true"
                                                ItemRequestTimeout="500" MarkFirstMatch="false" OnClientItemsRequesting="ClosestTownRequesting"
                                                ShowMoreResultsBox="false" Width="250px" Height="400px">
                                                <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetClosestTown" />
                                            </telerik:RadComboBox>
                                            <asp:RequiredFieldValidator ID="rfvTownId" runat="server" ControlToValidate="cboClosestTown"
                                                ErrorMessage="<img src='/images/error.png' Title='Please select the town closest to this point.'>"
                                                EnableClientScript="False"></asp:RequiredFieldValidator>
                                            <asp:CustomValidator ID="cfvTownId" runat="server" ControlToValidate="cboClosestTown"
                                                EnableClientScript="False" ErrorMessage="<img src='../images/error.png' Title='Please select the town closest to this point.'>"
                                                OnServerValidate="cfvTown_ServerValidate"></asp:CustomValidator>
                                        </td>
                                        <td>&nbsp;
                                        </td>
                                    </tr>

                                    <tr>
                                        <td class="formCellLabel">Address Line 1
                                        </td>
                                        <td class="formCellField">
                                            <asp:TextBox ID="txtAddressLine1" CssClass="fieldInputBox" runat="server" Width="250"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvAddressLine1" runat="server" EnableClientScript="False"
                                                ErrorMessage="Please enter the the first line of the Address." ControlToValidate="txtAddressLine1"
                                                Display="Dynamic">
											<img src="/images/Error.gif" height="16" width="16" title="Please enter the the first line of the Address." alt="" />
                                            </asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="formCellLabel">Address Line 2
                                        </td>
                                        <td class="formCellField">
                                            <asp:TextBox ID="txtAddressLine2" CssClass="fieldInputBox" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="formCellLabel">Address Line 3
                                        </td>
                                        <td class="formCellField">
                                            <asp:TextBox ID="txtAddressLine3" CssClass="fieldInputBox" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="formCellLabel">Post Town
                                        </td>
                                        <td class="formCellField">
                                            <asp:TextBox ID="txtPostTown" CssClass="fieldInputBox" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="formCellLabel">County/Province
                                        </td>
                                        <td class="formCellField">
                                            <asp:TextBox ID="txtCounty" CssClass="fieldInputBox" runat="server"></asp:TextBox>
                                        </td>
                                    </tr>

                                    <tr style="display: none;">
                                        <td class="formCellLabel">Grid Reference
                                        </td>
                                        <td class="formCellField">
                                            <asp:TextBox ID="txtLongitude" CssClass="fieldInputBox" runat="server" Text="0" Width="150"></asp:TextBox>
                                            <asp:TextBox ID="txtLatitude" CssClass="fieldInputBox" runat="server" Text="0" Width="150"></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td></td>
                                        <td>
                                            <input type="hidden" id="hdnSetPointRadius" runat="server" name="hdnSetPointRadius" />
                                            <input type="hidden" id="hidTrafficArea" runat="server" name="hidTrafficArea" value="0" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <asp:Panel ID="pnlAddressList" runat="server" Visible="false">
                                <asp:ListBox ID="lstAddress" runat="server" Rows="10" Width="100%" AutoPostBack="true"></asp:ListBox>
                                <div style="height: 22px; padding: 2px; color: #ffffff; background-color: #99BEDE; text-align: right;">
                                    <asp:Button ID="btnCancelList" runat="server" Text="Cancel" CausesValidation="false" />
                                </div>
                            </asp:Panel>
                        </div>
                    </td>
                    <td valign="top" width="50%" runat="server" id="tdClientCulture">
                        <!-- 
						divClientCulture is needed because we need an object that is always rendered 
						so that the radAjaxManager can uupdate it when it needs to. It was updating rcbClientCulture 
						but when it is not rendered it throws an error.
					-->
                        <div runat="server" id="divClientCulture">
                            <asp:Panel ID="pnlClientCulture" runat="server">
                                <div class="formBlock" style="width: 450px; height: 70px;">
                                    <h1>Client Culture</h1>
                                    <table>
                                        <tr>
                                            <td class="formCellLabel" style="width: 120px;">Culture:
                                            </td>
                                            <td class="formCellField">
                                                <telerik:RadComboBox ID="rcbClientCulture" runat="server" Width="250px" Height="300px" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </asp:Panel>
                        </div>
                        <div class="formBlock" style="width: 450px; height: 155px;">
                            <h1>Status</h1>
                            <table>
                                <tr>
                                    <td class="formCellField">
                                        <asp:CheckBox ID="chkSuspended" runat="server" Text="Suspended"></asp:CheckBox>
                                    </td>
                                    <td class="formCellField">
                                        <asp:TextBox ID="txtSuspendedReason" runat="server" Width="200"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formCellField">
                                        <asp:CheckBox ID="chkOnHold" runat="server" Text="Account On Hold"></asp:CheckBox>
                                    </td>
                                    <td class="formCellField">
                                        <asp:TextBox ID="txtOnHoldReason" runat="server" Width="200"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width: 50%; vertical-align: top;" class="formCellLabel">Credit Application Form
                                    </td>
                                    <td class="formCellField">
                                        <asp:CheckBox ID="chkCreditApplicationForm" runat="server" Enabled="false" />
                                        <asp:HyperLink ID="hlCreditApplicationForm" runat="server" Text="View" Target="_blank" Visible="false" />
                                        <asp:HyperLink ID="hlScanCreditApplicationForm" runat="server" Text="Scan" Visible="false" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width: 50%; vertical-align: top;" class="formCellLabel">Client T&Cs
                                    </td>
                                    <td class="formCellField">
                                        <asp:CheckBox ID="chkClientTnC" runat="server" Enabled="false" />
                                        <asp:HyperLink ID="hlClientTnC" runat="server" Text="View" Target="_blank" Visible="false" />
                                        <asp:HyperLink ID="hlScanClientTnC" runat="server" Text="Scan" Visible="false" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="width: 50%; vertical-align: top;" class="formCellLabel">Subcontractor T&Cs
                                    </td>
                                    <td class="formCellField">
                                        <asp:CheckBox ID="chkSubbyTnC" runat="server" Enabled="false" />
                                        <asp:HyperLink ID="hlSubbyTnC" runat="server" Text="View" Target="_blank" Visible="false" />
                                        <asp:HyperLink ID="hlScanSubbyTnC" runat="server" Text="Scan" Visible="false" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                </tr>
            </table>
        </telerik:RadPageView>
        <telerik:RadPageView ID="ReferencesView" runat="server">
            <div class="formBlock">
                <h1>Reference Capturing Information</h1>
                <p>
                    As
					<%=GetDescription.ToLower()%>s do not share common terminology you can alter the
					descriptions of two heavily used fields to aid order entry.
                </p>
                <div class="formBlock" style="float: left; width: 450px;">
                    <table>
                        <tr>
                            <td class="formCellLabel" style="width: 120px;">
                                <b>
                                    <asp:Label ID="lblLoadNumber" Text="Load Number" runat="server"></asp:Label></b>
                            </td>
                            <td class="formCellField">
                                <asp:TextBox ID="txtLoadNumberText" CssClass="fieldInputBox" runat="server" Text="Load Number"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvLoadNumberText" runat="server" ControlToValidate="txtLoadNumberText"
                                    EnableClientScript="False" ErrorMessage="Please enter the relevant description for this client.">
									<img src="/images/Error.gif" height="16" width="16" title="Please enter the relevant description for this client." alt="" />
                                </asp:RequiredFieldValidator>
                            </td>
                            <td>
                                <asp:RadioButton ID="rbUseAsDefaultLoadNo" GroupName="rbDefault" Text="Use as Default"
                                    runat="server" ToolTip="The field to display on the Traffic sheet"></asp:RadioButton>
                            </td>
                        </tr>
                    </table>
                    <p>
                        The Load Number is the
						<%=GetDescription.ToLower() %>'s job reference that can be used to tie a multi-drop
						job together. For example, Lever Faberge specify a Shipment Number, whereas Wisbech
						Roadways Ltd specify a Load Number. Please enter the relevant description for this
						<%=GetDescription.ToLower() %>.
                    </p>
                </div>
                <div class="formBlock" style="float: left; width: 450px;">
                    <table>
                        <tr>
                            <td class="formCellLabel" style="width: 120px;">
                                <b>
                                    <asp:Label ID="lblDocketNumber" Text="Docket Number" runat="server"></asp:Label></b>
                            </td>
                            <td class="formCellField">
                                <asp:TextBox ID="txtDocketNumberText" CssClass="fieldInputBox" runat="server" Text="Docket Number"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvDocketNumberText" runat="server" ControlToValidate="txtDocketNumberText"
                                    EnableClientScript="False" ErrorMessage="Please enter the relevant description for this client.">
									<img alt="" src="/images/Error.gif" height="16" width="16" title="Please enter the relevant description for this client." />
                                </asp:RequiredFieldValidator>
                            </td>
                            <td>
                                <asp:RadioButton ID="rbUseAsDefaultDocketNo" GroupName="rbDefault" Text="Use as Default"
                                    runat="server" ToolTip="The field to display on the Traffic sheet"></asp:RadioButton>
                            </td>
                        </tr>
                    </table>
                    <p>
                        The Docket Number is the
						<%=GetDescription.ToLower() %>
						's job reference that can be used to reference a single drop on a multi-drop job.
						For example, Lever Faberge specify a Load Number, whereas Wisbech Roadways Ltd specify
						a Docket Number. Please enter the relevant description for this
						<%=GetDescription.ToLower() %>.
                    </p>
                </div>
                <div class="clearDiv">
                </div>
                <table style="border-top: 1px solid #000; border-right: 1px solid #000; border-left: 1px solid #000; width: 100%;"
                    cellspacing="0">
                    <tr class="rgCommandRow">
                        <td class="rgCommandCell">
                            <asp:Button ID="btnAddReference" CssClass="buttonClassSmall" OnClick="btnAddReference_Click"
                                runat="server" Text="Add Reference" CausesValidation="False"></asp:Button>
                            <asp:CheckBox ID="chkShowDeletedReferences" runat="server" OnCheckedChanged="dgShowDeletedReferences_CheckedChanged"
                                Text="Show Deleted References" AutoPostBack="true" CausesValidation="False"></asp:CheckBox>
                        </td>
                    </tr>
                </table>
                <asp:Panel ID="pnlReferences" runat="server">
                    <asp:DataGrid ID="dgReferences" OnEditCommand="dgReferences_EditCommand" OnUpdateCommand="dgReferences_UpdateCommand"
                        OnCancelCommand="dgReferences_CancelCommand" OnItemCommand="dgReferences_ItemCommand"
                        OnItemDataBound="dgReferences_ItemDataBound" runat="server" CssClass="Grid" AutoGenerateColumns="False"
                        Width="100%">
                        <Columns>
                            <asp:TemplateColumn HeaderText="Description">
                                <ItemTemplate>
                                    <asp:Label runat="server" Text='<%# Bind("Description") %>' ID="lblReferenceDescription">
                                    </asp:Label>
                                    <input type="hidden" id="hidReferenceIdForEdit" runat="server" value='<%# Bind("OrganisationReferenceId") %>'
                                        name="hidReferenceIdForEdit" />
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtReferenceDescription" runat="server" Text='<%# Bind("Description")%>'>
                                    </asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvReferenceDescription" runat="server" Display="Dynamic"
                                        ControlToValidate="txtReferenceDescription" ErrorMessage="Please enter the reference description."
                                        EnableClientScript="False">
										<img src="/images/Error.gif" height="16" width="16" title="Please enter the reference description." alt="" />
                                    </asp:RequiredFieldValidator>
                                    <input type="hidden" id="hidReferenceIdForUpdate" runat="server" value='<%# Bind("OrganisationReferenceId") %>'
                                        name="hidReferenceIdForUpdate" />
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Captured As">
                                <ItemTemplate>
                                    <asp:Label runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "DataTypeDescription") %>'
                                        ID="lblReferenceDataType"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList ID="cboReferenceDataType" runat="server" DataSource='<%#ReferenceDataTypes%>'
                                        SelectedValue='<%# DataBinder.Eval(Container.DataItem, "DataTypeDescription")%>'>
                                    </asp:DropDownList>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Can Display on Invoice">
                                <ItemTemplate>
                                    <asp:Label runat="server" Text='<%# ((bool)DataBinder.Eval(Container.DataItem, "DisplayOnInvoice") == true) ? "Yes" : "No" %>'
                                        ID="lblCanDisplayOnInvoice"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:CheckBox runat="server" ID="chkCanDisplayOnInvoice" Checked='<%# (bool)DataBinder.Eval(Container.DataItem, "DisplayOnInvoice") == true %>' />
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Is Mandatory on Order">
                                <ItemTemplate>
                                    <asp:Label runat="server" Text='<%# ((bool)DataBinder.Eval(Container.DataItem, "MandatoryOnOrder") == true) ? "Yes" : "No" %>'
                                        ID="lblIsMandatoryOnOrder"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:CheckBox runat="server" ID="chkIsMandatoryOnOrder" Checked='<%# (bool)DataBinder.Eval(Container.DataItem, "MandatoryOnOrder") == true %>' />
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Reference Status">
                                <ItemTemplate>
                                    <asp:Label runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "StatusDescription")%>'
                                        ID="lblReferenceStatus"></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:DropDownList ID="cboReferenceStatus" runat="server" DataSource='<%#ReferenceStatus%>'
                                        SelectedValue='<%# DataBinder.Eval(Container.DataItem, "StatusDescription")%>'>
                                    </asp:DropDownList>
                                </EditItemTemplate>
                            </asp:TemplateColumn>
                            <asp:ButtonColumn Text="Move Down" ButtonType="PushButton" CommandName="MoveDown"></asp:ButtonColumn>
                            <asp:ButtonColumn Text="Move Up" ButtonType="PushButton" CommandName="MoveUp"></asp:ButtonColumn>
                            <asp:EditCommandColumn ButtonType="PushButton" UpdateText="Update" HeaderText="Edit Item"
                                CancelText="Cancel" EditText="Edit"></asp:EditCommandColumn>
                        </Columns>
                        <AlternatingItemStyle CssClass="DataGridListItemAlt"></AlternatingItemStyle>
                        <ItemStyle CssClass="DataGridListItem"></ItemStyle>
                        <HeaderStyle CssClass="DataGridListHead"></HeaderStyle>
                    </asp:DataGrid>
                </asp:Panel>
            </div>
        </telerik:RadPageView>
        <telerik:RadPageView ID="LocationsView" runat="server">
            <div class="formBlock">
                <h1>List of Locations</h1>
                <table style="border-top: 1px solid #000; border-right: 1px solid #000; border-left: 1px solid #000; width: 100%;"
                    cellspacing="0">
                    <tr class="rgCommandRow">
                        <td class="rgCommandCell">
                            <asp:Button ID="btnViewPoints" CssClass="buttonClassSmall" runat="server" OnClick="btnViewPoints_Click"
                                Text="View Points" CausesValidation="False"></asp:Button>
                            <asp:Button ID="btnAddLocation" CssClass="buttonClassSmall" OnClick="btnAddLocation_Click"
                                runat="server" Text="Add Location" CausesValidation="False"></asp:Button>
                        </td>
                    </tr>
                </table>
                <telerik:RadGrid ID="grdLocations" runat="server" AutoGenerateColumns="false" AllowPaging="true"
                    PageSize="20" Skin="Office2007">
                    <MasterTableView DataKeyNames="OrganisationLocationId, IdentityId, OrganisationName">
                        <Columns>
                            <telerik:GridHyperLinkColumn HeaderText="Name" SortExpression="OrganisationLocationName"
                                DataNavigateUrlFormatString="addupdateorganisationlocation.aspx?organisationLocationId={0}&identityId={1}&organisationName={2}"
                                DataNavigateUrlFields="OrganisationLocationId, IdentityId, OrganisationName"
                                DataTextField="OrganisationLocationName">
                            </telerik:GridHyperLinkColumn>
                            <telerik:GridBoundColumn DataField="OrganisationLocationType" HeaderText="Type">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="FullName" HeaderText="Contact">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="PostTown" HeaderText="Post Town">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="County" HeaderText="County">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="PostCode" HeaderText="PostCode">
                            </telerik:GridBoundColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </div>
        </telerik:RadPageView>
        <telerik:RadPageView ID="SettingsView" runat="server">
            <div style="width: 1000px; overflow: hidden;">
                <div style="float: left; width: 500px;">
                    <div class="formBlock">
                        <h1>Default Collection Point Settings</h1>
                        <table>
                            <tr>
                                <td class="formCellLabel" style="width: 120px;">Point
                                </td>
                                <td class="formCellField">
                                    <telerik:RadComboBox ID="cboDefaultCollectionPoint" runat="server" EnableLoadOnDemand="true"
                                        ItemRequestTimeout="500" MarkFirstMatch="true" AllowCustomText="false"
                                        Width="290px" Height="300px" Overlay="true" EnableVirtualScrolling="True">
                                    </telerik:RadComboBox>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div class="formBlock" >
                        <asp:Panel ID="pnlTariff" runat="server">
                            <h1>Tariff Settings</h1>
                            <div style="margin-bottom: 4px;">
                                <asp:RadioButton ID="rbTariffPerClient" runat="server" Text="Per Client" GroupName="rbTariff"
                                    Checked="true" onclick="toggleTariffType(false);" />
                                <asp:RadioButton ID="rbTariffPerBusinessType" runat="server" Text="Per Business Type"
                                    GroupName="rbTariff" onclick="toggleTariffType(true);" />
                            </div>
                            <asp:Panel ID="pnlTariffPerClient" runat="server">
                                <telerik:RadComboBox ID="cboTariff" runat="server" Width="290px" EnableLoadOnDemand="true" Filter="Contains">
                                    <WebServiceSettings Method="GetTariffs" Path="~/Tariff/Tariffs.asmx" />
                                </telerik:RadComboBox>
                            </asp:Panel>
                            <asp:Panel ID="pnlTariffPerBusinessType" runat="server" Style="display: none;">
                                <asp:ListView ID="lvBusinessTypeTariffs" runat="server" DataKeyNames="BusinessTypeID">
                                    <LayoutTemplate>
                                        <table>
                                            <asp:PlaceHolder ID="itemPlaceholder" runat="server"></asp:PlaceHolder>
                                        </table>
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td class="formCellLabel" style="width: 120px;">
                                                <%# Eval("Description") %>
                                                <input id="hidBusinessTypeID" runat="server" type="hidden" value='<%# Eval("BusinessTypeID") %>' />
                                            </td>
                                            <td class="formCellField">
                                                <telerik:RadComboBox ID="cboBusinessTypeTariff" runat="server" Width="290px" EnableLoadOnDemand="true">
                                                    <WebServiceSettings Method="GetTariffs" Path="~/Tariff/Tariffs.asmx" />
                                                </telerik:RadComboBox>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:ListView>
                            </asp:Panel>
                        </asp:Panel>
                    </div>
                    <div class="formBlock">
                        <asp:Panel ID="pnlPlanByDepot" runat="server">
                            <h1>Traffic Planning Options</h1>
                            <table>
                                <tr>
                                                        <td class="formCellField">
                        Default Control Area&nbsp;&nbsp;<telerik:radcombobox id="cboControlArea" runat="server"
                            skin="WindowsXP" radcontrolsdir="~/script/RadControls/"></telerik:radcombobox>
                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </div>
                     <div class="formBlock">
                        <asp:Panel ID="pnlExportSettings" runat="server" CssClass="exportSettings">
                            <h1>Export Settings</h1>
                            <table>
                                <tr>
                                    <td class="formCellField" colspan="2">
                                        <asp:CheckBox ID="chkEnableSubContractExport" runat="server" 
                                            Text="Enable Sub-Contractor Standard CSV Export" 
                                            OnCheckedChanged="chkEnableSubContractExport_CheckedChanged"
                                            Title="Export CSV file when sub-contracting. Note: CSV export only functions when subcontracting a whole run or 'per order' (not specific legs)."></asp:CheckBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <h1 class="subContractorPanelSubHeading">FTP Settings</h1>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formCellLabel">Host Name
                                    </td>
                                    <td class="formCellField">
                                        <asp:TextBox ID="txtFTPHostName" CssClass="fieldInputBox ftpInputBox" runat="server" MaxLength="70"></asp:TextBox>
                                    </td>
                                    <td>
                                        <asp:RequiredFieldValidator ID="rfvFTPHostname" runat="server" Tooltip="Please enter a ftp host name for csv export."
                                                                    EnableClientScript="False" ControlToValidate="txtFTPHostName" Display="Dynamic"
                                                                    Enabled="false"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formCellLabel">Port Number
                                    </td>
                                    <td class="formCellField">
                                        <telerik:RadNumericTextBox ID="txtFTPPortNumber" CssClass="fieldInputBox  ftpInputBox" runat="server"
                                                                   MaxValue="65535"
                                                                   MinValue="1" 
                                                                   NumberFormat-DecimalDigits="0"
                                                                   NumberFormat-GroupSeparator=""></telerik:RadNumericTextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formCellLabel">User Name
                                    </td>
                                    <td class="formCellField">
                                        <asp:TextBox ID="txtFTPUserName" CssClass="fieldInputBox ftpInputBox" runat="server" MaxLength="50"></asp:TextBox>
                                    </td>
                                     <td>
                                        <asp:RequiredFieldValidator ID="rfvFTPUserName" runat="server" Tooltip="Please enter a ftp user name for csv export."
                                                                    EnableClientScript="False" ControlToValidate="txtFTPUserName" Display="Dynamic"
                                                                    Enabled="false"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formCellLabel">Password
                                    </td>
                                    <td class="formCellField">
                                        <asp:TextBox ID="txtFTPPassword" CssClass="fieldInputBox ftpInputBox" runat="server" MaxLength="50"></asp:TextBox>
                                    </td>
                                     <td>
                                        <asp:RequiredFieldValidator ID="rfvFTPPassword" runat="server" Tooltip="Please enter a ftp password for csv export."
                                                                    EnableClientScript="False" ControlToValidate="txtFTPPassword" Display="Dynamic"
                                                                    Enabled="false"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formCellLabel">Directory
                                    </td>
                                    <td class="formCellField">
                                        <asp:TextBox ID="txtFTPDirectory" CssClass="fieldInputBox ftpInputBox" runat="server" MaxLength="50"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formCellLabel">Passive
                                    </td>
                                    <td class="formCellField">
                                        <asp:CheckBox ID="chkFTPPassive" CssClass="ftpInputBox" runat="server" Checked="true"/>
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </div>
                </div>
                <div style="float: left; width: 500px;">
                    <div class="formBlock">
                        <h1>Invoice Settings</h1>
                        <table style="width: 100%;">
                            <tr>
                                <td class="formCellLabel">Default Invoice Type
                                </td>
                                <td class="formCellField">
                                    <asp:DropDownList ID="cboInvoiceType" runat="server">
                                        <asp:ListItem Text="Normal" Value="Normal"></asp:ListItem>
                                        <asp:ListItem Text="Self Bill" Value="Self Bill"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">Payment Terms
                                </td>
                                <td class="formCellField">
                                    <asp:TextBox ID="txtPaymentTerms" CssClass="fieldInputBox" runat="server" Width="130"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">Auto Batch Settings
                                </td>
                                <td class="formCellField">
                                    <asp:DropDownList ID="cboInvoiceGroupType" runat="server">
                                        <asp:ListItem Text="By Order Count" Value="ByCount"></asp:ListItem>
                                        <asp:ListItem Text="By Value" Value="ByValue"></asp:ListItem>
                                        <asp:ListItem Text="None" Value="" Selected="True"></asp:ListItem>
                                    </asp:DropDownList>
                                    <telerik:RadNumericTextBox ID="rntInvoiceGroupValue" runat="server">
                                    </telerik:RadNumericTextBox>
                                </td>
                            </tr>
                        </table>


                        Fuel Surcharges
                        <table style="margin: 5px 0px 15px 20px; border: 1px solid" width="70%">
                            <tr>
                                <td>
                                    <asp:RadioButton ID="rdFuelSurchargeDisplayInclude" GroupName="rbFuelSurcharges" Text="Display in Details and Include in Totals"
                                        runat="server" Checked="true"></asp:RadioButton>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:RadioButton ID="rdFuelSurchargeHideInclude" GroupName="rbFuelSurcharges" Text="Hide in Details and Include in Totals"
                                        runat="server"></asp:RadioButton>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:RadioButton ID="rdFuelSurchargeNeither" GroupName="rbFuelSurcharges" Text="Hide in Details and Not Included in Totals"
                                        runat="server"></asp:RadioButton>
                                </td>

                            </tr>

                        </table>

                        <div style="margin: 3px;">
                            <table style="width: 100%;" cellspacing="5">


                                <tr>
                                    <%--<td class="formCellField">
										<asp:CheckBox ID="chkIncludeFuelSurcharge" runat="server" Text="Fuel Surcharge" AutoPostBack="True" Visible="false">
										</asp:CheckBox>
									</td>--%>
                                    <td class="formCellField">
                                        <asp:CheckBox ID="chkIncludeReferences" runat="server" Text="Non-Custom References" ToolTip="Checking this will show load number and docket number on invoices (but will not show any custom references defined on the References tab)"></asp:CheckBox>
                                    </td>
                                    <td class="formCellField">
                                        <asp:CheckBox ID="chkIncludeDemurrage" runat="server" Text="Demurrage" AutoPostBack="True"></asp:CheckBox>
                                    </td>
                                    <td class="formCellField">
                                        <asp:CheckBox ID="chkIncludeInvoiceRunId" runat="server" Text="Run Id" AutoPostBack="True" ToolTip="Checking this will show the Run Id on the invoice"></asp:CheckBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formCellField">
                                        <asp:CheckBox ID="chkIsExcludedFromInvoicing" runat="server" Text="Exclude from invoicing" />
                                    </td>
                                    <td class="formCellField">
                                        <asp:CheckBox ID="chkIncludeJobDetails" Checked="true" runat="server" Text="Job Details"
                                            AutoPostBack="True"></asp:CheckBox>
                                    </td>
                                    <td class="formCellField">
                                        <asp:CheckBox ID="chkIncludeInstructionNotes" runat="server" Text="Instruction Notes"></asp:CheckBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formCellField">
                                        <asp:CheckBox ID="chkIncludeServiceLevel" runat="server" Text="Service Level"></asp:CheckBox>
                                    </td>
                                    <td class="formCellField">
                                        <asp:CheckBox ID="chkIncludeJobExtras" runat="server" Text="Job Extras"></asp:CheckBox>
                                    </td>
                                    <td class="formCellField">
                                        <asp:CheckBox ID="chkShowGoodsRefusal" runat="server" Text="Goods Refusals"></asp:CheckBox>
                                    </td>
                                </tr>

                                <tr>

                                    <td class="formCellField">
                                        <asp:CheckBox ID="chkIncludeWeights" runat="server" Text="Weights" Checked="true"></asp:CheckBox>
                                    </td>
                                    <td class="formCellField">
                                        <asp:CheckBox ID="chkIncludePallets" runat="server" Text="Pallets" Checked="true"></asp:CheckBox>
                                    </td>
                                    <td class="formCellField">
                                        <asp:CheckBox ID="chkIncludePalletType" runat="server" Text="Pallet Type" Checked="false"></asp:CheckBox>
                                    </td>
                                </tr>


                            </table>
                        </div>
                        <table style="width: 100%;" cellspacing="0">
                            <tr>
                                <td class="formCellLabel" style="text-align: left !important; padding: 5px;">
                                    <asp:CheckBox ID="chkIncludeVatTotal" runat="server" Text="VAT Total" Checked="true"></asp:CheckBox>

                                </td>
                            </tr>

                            <tr>
                                <td class="formCellLabel" style="text-align: left !important; padding: 5px;">
                                    <asp:CheckBox ID="chkIncludeExtraDetails" runat="server" Text="Extra Details" AutoPostBack="True"></asp:CheckBox>
                                    &nbsp;
									<asp:DropDownList ID="cboDemurrageType" runat="server" Visible="false">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                                 <tr>
                                <td class="formCellLabel" style="text-align: left !important; padding: 5px;">
                                    <asp:CheckBox ID="chkIncludeSurcharges" runat="server" Text="Add Surcharges next to Service Levels"></asp:CheckBox>
                                </td>
                            </tr>
                        </table>
                        <table style="width: 100%;" cellspacing="0">
                            <tr>
                                <td class="formCellLabel" style="text-align: left !important; padding: 5px;">
                                    <asp:CheckBox ID="chkAutoEmailInvoices" runat="server" Text="Email Invoice when Approved to" />
                                    <asp:TextBox ID="txtInvoiceEmailAddress" CssClass="fieldInputBox" runat="server"
                                        Width="220" ToolTip="Email address to send invoices to." />
                                    <asp:CustomValidator ID="cfvInvoiceEmailAddress" ControlToValidate="txtInvoiceEmailAddress"
                                        runat="server" ToolTip="Please enter an invoice email address." Display="Dynamic"
                                        EnableClientScript="false">
										<img src="/images/Error.gif" height="16" width="16" title="Please enter an invoice email address." alt="" />
                                    </asp:CustomValidator>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel" style="text-align: left !important; padding: 5px;">
                                    <asp:CheckBox ID="chkInvoiceAttachCSV" runat="server" Text="Attach Invoice CSV To Email" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
                <div class="clearDiv">
                </div>
                <div class="formBlock" style="float: left; width: 468px; min-height: 200px;">
                    <h1>Job Settings</h1>
                    <table>
                        <tr>
                            <td class="formCellLabel" style="width: 220px;">Default Job Type
                            </td>
                            <td class="formCellField">
                                <asp:DropDownList ID="cboJobType" runat="server" />
                            </td>
                        </tr>
                        <tr style="display: none;">
                            <td class="formCellLabel">Must Capture Rates
                            </td>
                            <td class="formCellField">
                                <asp:CheckBox ID="chkMustCaptureRates" runat="server"></asp:CheckBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Must Capture Collection Call-In Information
                            </td>
                            <td class="formCellField">
                                <asp:CheckBox ID="chkMustCaptureCollectionDebrief" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Must Capture Delivery Call-In Information
                            </td>
                            <td class="formCellField">
                                <asp:CheckBox ID="chkMustCaptureDebrief" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Enable GPS Auto Call-In<br />
                                (overrides Pallet tracking/manual debriefs)
                            </td>
                            <td class="formCellField">
                                <asp:CheckBox ID="chkGPSAutoCallIn" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Default Geofence Radius (metres)
                            </td>
                            <td class="formCellField">
                                <asp:TextBox ID="txtDefaultRadius" CssClass="fieldInputBox" runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Export Return Orders
                            </td>
                            <td class="formCellField">
                                <asp:CheckBox ID="chkExportReturnOrders" runat="server" Checked="false" />
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Import Return Orders
                            </td>
                            <td class="formCellField">
                                <asp:CheckBox ID="chkImportReturnOrder" runat="server" Checked="false" />
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Palletforce Tracking Number From Order
                            </td>
                            <td class="formCellField">
                                <asp:CheckBox ID="chkPalletforceTrackingNoFromOrder" runat="server" Checked="false" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="formBlock" style="float: left; width: 468px; min-height: 200px;">
                    <h1>
                        <%=GetDescription%>
						Fuel Surcharge</h1>
                    <table style="width: 100%;">
                        <tr>
                            <td class="formCellLabel" style="width: 220px;">Use Standard Fuel Surcharge
                            </td>
                            <td class="formCellField">
                                <asp:RadioButton onclick="javascript:fuelSurchargeRadioOptionChecked(this);" GroupName="FuelSurcharge"
                                    runat="server" ID="rdFuelSurchargeStandard" />
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Adjust Standard Fuel Surcharge
                            </td>
                            <td class="formCellField">
                                <asp:RadioButton onclick="javascript:fuelSurchargeRadioOptionChecked(this);" GroupName="FuelSurcharge"
                                    runat="server" ID="rdFuelSurchargeAdjustment" />
                                <telerik:RadNumericTextBox Value="0.00" ID="rntFuelSurchargeAdjustment" runat="server"
                                    AutoPostBack="false" MaxValue="100.00" MinValue="-100.00" MaxLength="6" Width="50px"
                                    Culture="en-GB" />&nbsp;%
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Override Standard Fuel Surcharge
                            </td>
                            <td class="formCellField">
                                <asp:RadioButton onclick="javascript:fuelSurchargeRadioOptionChecked(this);" Checked="true"
                                    GroupName="FuelSurcharge" runat="server" ID="rdFuelSurchargeOverride" />
                                <telerik:RadNumericTextBox Value="0.00" ID="rntFuelSurchargeOverride" runat="server"
                                    AutoPostBack="false" MaxValue="100.00" MinValue="-100.00" MaxLength="6" Width="50px"
                                    Culture="en-GB" />&nbsp;%
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Fuel Surcharge Breakdown Type
                            </td>
                            <td class="formCellField">
                                <asp:DropDownList ID="cboFuelSurchargeBreakdownType" runat="server">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Fuel Surcharge will be applied to Extras
                            </td>
                            <td class="formCellField">
                                <asp:CheckBox ID="chkFuelSurchargeOnExtras" runat="server" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="clearDiv">
                </div>
                <div class="formBlock" style="float: left; width: 468px; min-height: 100px;">
                    <h1>Rate Tariff Card</h1>
                    <table>
                        <tr>
                            <td class="formCellLabel" style="width: 120px;">Notes
                            </td>
                            <td class="formCellField">
                                <asp:TextBox ID="txtRateTariffCard" runat="server" BackColor="#fffedb" BorderWidth="1px"
                                    BorderColor="#7f9db9" MaxLength="100" Rows="4" TextMode="MultiLine" Columns="47"
                                    Wrap="true" Visible="true" Font-Bold="true"></asp:TextBox>
                                <asp:CustomValidator ID="cfvRateTariffCard" runat="server" Display="Dynamic" ControlToValidate="txtRateTariffCard"
                                    ErrorMessage="Please keep the Rate Tariff rate description to less than 100 characters."
                                    EnableClientScript="false" OnServerValidate="ValidateRateTariffCard">
									<img src="/images/Error.gif" height="16" width="16" title="Please keep the Rate Tariff rate description to less than 100 characters." alt="" />
                                </asp:CustomValidator>
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="formBlock" style="float: left; width: 468px; min-height: 100px;">
                    <h1>KPI Settings</h1>
                    <table>
                        <tr>
                            <td class="formCellLabel" style="width: 120px;">Early Minutes
                            </td>
                            <td class="formCellField">
                                <asp:TextBox ID="txtEarlyMinutes" CssClass="fieldInputBox" runat="server" Width="18" />&nbsp;<span
                                    style="font-size: 11px;">Arrival Time Minus Early Minutes = Early Delivery</span>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Late Minutes
                            </td>
                            <td class="formCellField">
                                <asp:TextBox ID="txtLateMinutes" CssClass="fieldInputBox" runat="server" Width="18" />&nbsp;<span
                                    style="font-size: 11px;">Arrival Time Plus Late Minutes = Late Delivery</span>
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="clearDiv">
                </div>
                <fieldset style="display: none;">
                    <legend>
                        <%=GetDescription%>
						Demurrage Settings</legend>
                    <table>
                        <tr>
                            <td class="formCellLabel">Tipping Time Threshold (minutes)
                            </td>
                            <td class="formCellField">
                                <asp:TextBox ID="txtTippingThreshold" runat="server"></asp:TextBox>
                                <asp:CustomValidator ID="cfvTippingThreshold" runat="server" Display="Dynamic" ControlToValidate="txtTippingThreshold"
                                    ErrorMessage="Please enter the Tipping Threshold in minutes." EnableClientScript="False"
                                    OnServerValidate="ValidateTippingThreshold" ClientValidationFunction="ValidateTippingThreshold">
									<img src="/images/Error.gif" height="16" width="16" title="Please enter the Tipping Threshold in minutes." alt="" />
                                </asp:CustomValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Demurrage Charge Rate (£s per hour)
                            </td>
                            <td class="formCellField">
                                <asp:TextBox ID="txtDemmurageChargeRate" runat="server"></asp:TextBox>
                                <asp:CustomValidator ID="cfvDemmurageChargeRate" runat="server" Display="Dynamic"
                                    ControlToValidate="txtDemmurageChargeRate" ErrorMessage="Please enter the Demurrage Charge Rate in the correct format."
                                    EnableClientScript="False" OnServerValidate="ValidateDemurrageChargeRate" ClientValidationFunction="ValidateDemurrageChargeRate">
									<img src="/images/Error.gif" height="16" width="16" title="Please enter the Demurrage Charge Rate in the correct format." alt="" />
                                </asp:CustomValidator>
                            </td>
                        </tr>
                    </table>
                </fieldset>

                <div style="float: left; width: 500px;">
                    <div class="formBlock">
                        <h1>Pallet Configuration</h1>
                        <table style="display: none;">
                            <tr style="display: none;">
                                <td class="formCellLabel">Pallet Number Threshold
                                </td>
                                <td class="formCellField">
                                    <asp:TextBox ID="txtPalletNumberThreshold" runat="server"></asp:TextBox>
                                    <asp:CustomValidator ID="cfvPalletNumberThreshold" runat="server" Display="Dynamic"
                                        ControlToValidate="txtPalletNumberThreshold" ErrorMessage="Please enter the Pallet Number Threshold."
                                        EnableClientScript="False" OnServerValidate="ValidatePalletNumberThreshold" ClientValidationFunction="ValidatePalletNumberThreshold">
										<img src="../images/Error.gif" height="16" width="16" title="Please enter the Pallet Number Threshold." alt="Error"/>
                                    </asp:CustomValidator>
                                </td>
                            </tr>
                            <tr style="display: none;">
                                <td class="formCellLabel">Penalty Charge (pence per pallet)
                                </td>
                                <td class="formCellField">
                                    <asp:TextBox ID="txtPalletPenaltyCharge" runat="server"></asp:TextBox>
                                    <asp:CustomValidator ID="cfvPalletPenaltyCharge" runat="server" Display="Dynamic"
                                        ControlToValidate="txtPalletPenaltyCharge" ErrorMessage="Please enter the Penalty Charge in pence."
                                        EnableClientScript="False" OnServerValidate="ValidatePalletPenaltyCharge" ClientValidationFunction="ValidatePalletPenaltyCharge">
										<img src="../images/Error.gif" height="16" width="16" title="Please enter the Penalty Charge in pence." alt="Error"/>
                                    </asp:CustomValidator>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">CHEP Account Number
                                </td>
                                <td class="formCellField">
                                    <asp:TextBox ID="txtCHEPNumber" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                        </table>
                        <asp:GridView ID="gvPalletType" runat="server" CssClass="DataGridStyle" AutoGenerateColumns="false"
                            ShowFooter="false" OnDataBound="tcPalletType_DataBound" Width="100%">
                            <HeaderStyle CssClass="HeadingRowLite" />
                            <Columns>
                                <asp:BoundField HeaderText="Pallet Type" DataField="Description"></asp:BoundField>
                                <asp:TemplateField HeaderText="Active">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkPalletTypeIsActive" runat="server"></asp:CheckBox>
                                        <asp:HiddenField ID="hidPalletTypeId" runat="server"></asp:HiddenField>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Track">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkPalletTypeIsTracked" runat="server"></asp:CheckBox>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Default">
                                    <ItemTemplate>
                                        <uc:RdoBtnGrouper ID="cboPalletTypeIsDefault" runat="server" GroupName="cboPalletTypeIsDefault" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="No Pallets">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtPalletBalance" runat="server" Width="50px" Text='<%# ((System.Data.DataRowView)Container.DataItem)["Balance"].ToString() %>'></asp:TextBox>
                                        <asp:HiddenField ID="hidPalletBalance" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>

                    <div class="formBlock">
                        <h1>Linked Clients</h1>
                        <table id="tblLinkedClientSelector">
                            <tr>
                                <td>
                                    <telerik:RadComboBox ID="cboLinkedClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                                        MarkFirstMatch="true" Height="300px" Overlay="true" CausesValidation="False"
                                        ShowMoreResultsBox="false" Width="350px" AllowCustomText="True"
                                        OnClientItemsRequesting="cboLinkedClient_itemsRequesting" OnClientItemsRequested="cboLinkedClient_itemsRequested">
                                        <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetClients" />
                                    </telerik:RadComboBox>
                                </td>
                                <td>
                                    <asp:Button runat="server" Text="Add" UseSubmitBehavior="false" OnClientClick="btnLinkedClientAdd_Clicked(); return false;" />
                                </td>
                            </tr>
                        </table>

                        <asp:ListView ID="lvLinkedClients" runat="server" AutoGenerateColumns="false" ShowFooter="false">
                            <LayoutTemplate>
                                <table id="tblLinkedClients" class="DataGridStyle" border="1" style="width: 100%; border-collapse: collapse;">
                                    <tr runat="server" class="HeadingRowLite">
                                        <th runat="server">Client Name</th>
                                        <th runat="server">Remove</th>
                                    </tr>
                                    <tr runat="server" id="itemPlaceholder">
                                    </tr>
                                </table>
                            </LayoutTemplate>
                            <ItemTemplate>
                                <tr id="tblLinkedClientsRow_<%# Eval("IdentityID") %>">
                                    <td runat="server">
                                        <asp:Label ID="lblOrganisationName" runat="server" Text='<%# Eval("OrganisationName") %>' />
                                    </td>
                                    <td>
                                        <button style="float: right" type="button" onclick="removeLinkedClient(<%# Eval("IdentityID") %>)">Remove</button></td>
                                </tr>
                            </ItemTemplate>
                            <EmptyDataTemplate>
                                <table id="tblLinkedClients" class="DataGridStyle" border="1" style="width: 100%; border-collapse: collapse;">
                                    <tr runat="server" class="HeadingRowLite">
                                        <th runat="server">Client Name</th>
                                        <th runat="server">Remove</th>
                                    </tr>
                                </table>
                            </EmptyDataTemplate>
                        </asp:ListView>

                        <asp:HiddenField ID="LinkedClientsToAdd" Value="" runat="server" />
                        <asp:HiddenField ID="LinkedClientsToRemove" Value="" runat="server" />
                    </div>

                    <div class="formBlock" runat="server" id="divArrivalsBoard">
                        <h1>Arrivals Board Configuration</h1>
                        <table>
                            <tr>
                                <td class="formCellLabel">Arrivals Board Enabled</td>
                                <td class="formCellField">
                                    <asp:CheckBox ID="chkArrivalsBoardEnabled" runat="server" />
                                </td>
                            </tr>
                         </table>
                    </div>
                </div>
                
                <div style="float: left; width: 500px;">
                    <div class="formBlock">
                        <h1>Goods Type Configuration</h1>
                        <asp:GridView ID="gvGoodsType" runat="server" CssClass="DataGridStyle" AutoGenerateColumns="false"
                            ShowFooter="false" OnDataBound="tcGoodsType_DataBound" Width="100%">
                            <HeaderStyle CssClass="HeadingRowLite" />
                            <Columns>
                                <asp:BoundField HeaderText="Goods Type" DataField="Description"></asp:BoundField>
                                <asp:TemplateField HeaderText="Active">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkGoodsTypeIsActive" runat="server"></asp:CheckBox>
                                        <asp:HiddenField ID="hidGoodsTypeId" runat="server"></asp:HiddenField>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Default">
                                    <ItemTemplate>
                                        <uc:RdoBtnGrouper ID="cboGoodsTypeIsDefault" runat="server" GroupName="cboGoodsTypeIsDefault" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                    <table id="addOrderConfiguration" runat="server" cellpadding="0" cellspacing="0"
                        style="width: 100%;">
                        <tr>
                            <td>
                                <div class="formBlock" style="min-height: 187px;">
                                    <h1>Add Order Screen defaults</h1>
                                    <table>
                                        <tr>
                                            <td class="formCellLabel" style="width:105px;">Default Business Type
                                            </td>
                                            <td class="formCellField" style="vertical-align:top;">
                                                <asp:DropDownList ID="cboDefaultBusinessType" runat="server">
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                If this client uses the client portal to add orders and the default business type is not set, an arbitrary type will be used when adding orders from the client portal.
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="formCellLabel">Default Number of Pallets
                                            </td>
                                            <td colspan="2" class="formCellField">
                                                <asp:TextBox ID="txtDefaultNumberOfPallets" CssClass="fieldInputBox" runat="server"
                                                    MaxLength="3" Width="20" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="formCellLabel">Attach confirmation email as booking form
                                            </td>
                                            <td colspan="2" class="formCellField">
                                                <asp:CheckBox ID="chkAttachConfimationEmail" runat="server" Checked="false" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="3">
                                                <h1>Service Level Configuration </h1>
                                                <asp:GridView ID="gvServiceLevels" runat="server" CssClass="DataGridStyle" AutoGenerateColumns="false"
                                                    ShowFooter="false" OnDataBound="tcServiceLevel_DataBound" Width="100%">
                                                    <HeaderStyle CssClass="HeadingRowLite" />
                                                    <Columns>
                                                        <asp:BoundField HeaderText="Service Levels" DataField="Description"></asp:BoundField>
                                                        <asp:TemplateField HeaderText="Active">
                                                            <ItemTemplate>
                                                                <asp:CheckBox ID="chkServiceLevelEnabled" runat="server"></asp:CheckBox>
                                                                <asp:HiddenField ID="hidServiceLevelID" runat="server"></asp:HiddenField>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Default">
                                                            <ItemTemplate>
                                                                <uc:RdoBtnGrouper ID="cboServiceLevelIsDefault" runat="server" GroupName="cboServiceLevelIsDefault" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td colspan="3">
                                                <h1>Extra Type Configuration </h1>
                                                <asp:GridView ID="gvExtraTypes" runat="server" CssClass="DataGridStyle" AutoGenerateColumns="false"
                                                    ShowFooter="false" OnDataBound="gvExtraTypes_DataBound" Width="100%">
                                                    <HeaderStyle CssClass="HeadingRowLite" />
                                                    <Columns>
                                                        <asp:BoundField HeaderText="Extra Types" DataField="Description"></asp:BoundField>
                                                        <asp:TemplateField HeaderText="Enabled by Default">
                                                            <ItemTemplate>
                                                                <asp:CheckBox ID="chkExtraTypesEnabled" runat="server"></asp:CheckBox>
                                                                <asp:HiddenField ID="hidExtraTypeID" runat="server"></asp:HiddenField>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                        </tr>
                    </table>

                </div>
                <div class="clearDiv">
                </div>
                <div id="pnlAllocation" runat="server" style="float: left; width: 500px;">
                    <div class="formBlock">
                        <h1>Consortium Member Allocation Settings</h1>
                        <table>
                            <tr>
                                <td class="formCellLabel" style="width: 120px;">Allocation Rule Set
                                </td>
                                <td class="formCellField">
                                    <telerik:RadComboBox ID="cboAllocationRuleSet" runat="server" Width="250px" DataValueField="AllocationRuleSetID"
                                        DataTextField="Description" AppendDataBoundItems="true">
                                        <Items>
                                            <telerik:RadComboBoxItem Value="" Text="- none -" />
                                        </Items>
                                    </telerik:RadComboBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel" colspan="2" style="text-align: left !important; padding: 5px;">
                                    <asp:CheckBox ID="chkExcludeFromAutoSubcontracting" runat="server" Text="Exclude from Auto-Dispatch" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
                <div class="clearDiv">
                </div>
                <table id="pcvConfiguration" runat="server" style="width: 100%;" cellpadding="0"
                    cellspacing="0">
                    <tr>
                        <td>
                            <div class="formBlock">
                                <h1>PCV Redemption Status Text Configuration</h1>
                                <table>
                                    <tr>
                                        <td class="formCellLabel">PCV Requires Sending to Customer (Requires De-Hire)
                                        </td>
                                        <td class="formCellField">
                                            <asp:TextBox ID="txtRequiresDeHire" runat="server" MaxLength="100" Width="400" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="formCellLabel">PCV Posted to Customer (De-Hired)
                                        </td>
                                        <td class="formCellField">
                                            <asp:TextBox ID="txtDeHired" runat="server" MaxLength="100" Width="400" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </td>
                    </tr>
                </table>
            </div>
        </telerik:RadPageView>
        <telerik:RadPageView ID="LogSettingsView" runat="server">
            <fieldset>
                <legend>Delivery and Returns Log Production</legend>
                <table>
                    <tr>
                        <td>Log Generation Frequency
                        </td>
                        <td>
                            <asp:DropDownList ID="cboLogFrequency" runat="server" Width="100" OnSelectedIndexChanged="tcLogFrequency_SelectedIndexChanged"
                                AutoPostBack="True">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <asp:Panel ID="pnlDay" runat="server" Visible="True">
                        <tr>
                            <td>Deliver Log By (Day)
                            </td>
                            <td>
                                <asp:DropDownList ID="cboDay" runat="server" Width="100">
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </asp:Panel>
                    <tr>
                        <td>Deliver Log By (Time)
                        </td>
                        <td>
                            <table>
                                <tr>
                                    <td valign="top">
                                        <telerik:RadDateInput ID="dteDeliverLogBy" runat="server" DateFormat="HH:mm">
                                        </telerik:RadDateInput>
                                    </td>
                                    <td valign="top">
                                        <asp:RequiredFieldValidator ID="rfvDeliverLogBy" runat="server" EnableClientScript="False"
                                            ErrorMessage="Please enter the time to Deliver the Log By." ControlToValidate="dteDeliverLogBy"
                                            Display="Dynamic">
											<img src="../images/Error.gif" height="16" width="16" title="Please enter the time to Deliver the Log By." alt="Error"/></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>Display Warnings After (minutes)
                        </td>
                        <td>
                            <asp:TextBox ID="txtWarningElapsedTime" runat="server" Width="100"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvWarningElapsedTime" runat="server" EnableClientScript="False"
                                ErrorMessage="Please enter the time to Display Warnings After." ControlToValidate="txtWarningElapsedTime"
                                Display="Dynamic">
								<img src="../images/Error.gif" height="16" width="16" title="Please enter the time to Display Warnings After." alt="Error"/></asp:RequiredFieldValidator>
                            <asp:CustomValidator ID="cfvWarningElapsedTime" runat="server" EnableClientScript="False"
                                ErrorMessage="Please enter the time to Display Warnings After in minutes." ControlToValidate="txtWarningElapsedTime"
                                Display="Dynamic" ClientValidationFunction="ValidateLogWarningAfter" OnServerValidate="ValidateLogWarningAfter">
								<img src="../images/Error.gif" height="16" width="16" title="Please enter the time to Display Warnings After in minutes." alt="Error"/></asp:CustomValidator>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:TextBox ID="txtDefaultLog" Visible="False" runat="server" Width="100"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>Default Log Columns
                        </td>
                        <td align="center">Column(s) to exclude
                        </td>
                        <td></td>
                        <td align="center">Column(s) to include
                        </td>
                        <td align="center">Column ordering
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td>
                            <asp:ListBox ID="lbExcludedColumns" runat="server" Height="170" Width="150"></asp:ListBox>
                        </td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnAssignColumn" runat="server" OnClick="tcAssignColumn_Click" Text="Add >"
                                            Width="70" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnUnAssignColumn" runat="server" OnClick="tcUnassignColumn_Click"
                                            Text="Remove <" Width="70" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td>
                            <asp:ListBox ID="lbIncludedColumns" runat="server" Height="170" Width="150"></asp:ListBox>
                        </td>
                        <td>
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnMoveColumnUp" runat="server" OnClick="tcMoveColumnUp_Click" Text="Move Up"
                                            Width="90" />
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Button ID="btnMoveColumnDown" runat="server" Text="Move Down" OnClick="tcMoveColumnDown_Click"
                                            Width="90" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td>
                            <asp:Label ID="lblColumnError" Visible="False" CssClass="ControlErrorMessage" runat="server" />
                        </td>
                    </tr>
                </table>
            </fieldset>
        </telerik:RadPageView>
        <telerik:RadPageView ID="ReportSettingsView" runat="server">
            <div class="formBlock">
                <h1>Manifest Production</h1>
                <p>
                    Please supply the general resource instructions and the contact details that need
					to appear on a manifest report.
                </p>
                <div class="formBlock">
                    <h1>General Resource Instructions</h1>
                    <asp:DataGrid ID="dgMPGeneralResourceInstructions" runat="server" Width="100%" AllowSorting="False"
                        AutoGenerateColumns="False" AllowPaging="False" CssClass="Grid" ShowFooter="true"
                        OnItemCommand="dgMPGeneralResourceInstructions_ItemCommand" OnItemDataBound="dgMPGeneralResourceInstructions_ItemDataBound"
                        OnPreRender="dgMPGeneralResourceInstructions_PreRender" EnableViewState="false">
                        <HeaderStyle CssClass="HeadingRow" Height="20" VerticalAlign="middle" />
                        <ItemStyle Height="20" CssClass="Row" />
                        <AlternatingItemStyle Height="20" BackColor="WhiteSmoke" />
                        <SelectedItemStyle Height="20" CssClass="SelectedRow" />
                        <Columns>
                            <asp:TemplateColumn HeaderText="Instruction">
                                <ItemTemplate>
                                    <%#DataBinder.Eval(Container.DataItem, "Data") %>
                                    <asp:HiddenField ID="hidReportSetting" runat="server"></asp:HiddenField>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtInstruction" runat="server" Width="250px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvInstruction" runat="server" ControlToValidate="txtInstruction"
                                        Display="Dynamic" ErrorMessage="Please supply a value for this instruction."
                                        ValidationGroup="vgEditMPGeneralResourceInstruction"><img src="../images/Error.gif" height="16" width="16" title="Please supply a value for this instruction." alt="Error"/></asp:RequiredFieldValidator>
                                    <asp:HiddenField ID="hidReportSetting" runat="server"></asp:HiddenField>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtNewInstruction" runat="server" Width="250px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvNewInstruction" runat="server" ControlToValidate="txtNewInstruction"
                                        Display="Dynamic" ErrorMessage="Please supply a value for this instruction."
                                        ValidationGroup="vgAddMPGeneralResourceInstruction"><img src="../images/Error.gif" height="16" width="16" title="Please supply a value for this instruction." alt="Error"/></asp:RequiredFieldValidator>
                                </FooterTemplate>
                            </asp:TemplateColumn>
                            <asp:ButtonColumn ButtonType="PushButton" Text=" &#9660; " CausesValidation="false"
                                CommandName="Down"></asp:ButtonColumn>
                            <asp:ButtonColumn ButtonType="PushButton" Text=" &#9650; " CausesValidation="false"
                                CommandName="Up"></asp:ButtonColumn>
                            <asp:EditCommandColumn ButtonType="PushButton" UpdateText="Update" CancelText="Cancel"
                                EditText="Edit" HeaderText="Edit" ValidationGroup="vgEditMPGeneralResourceInstruction"></asp:EditCommandColumn>
                            <asp:TemplateColumn>
                                <ItemTemplate>
                                    <asp:Button ID="btnDelete" runat="server" CommandName="Delete" Text="Delete" />
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Button ID="btnInsert" runat="server" CommandName="Insert" Text="Add" ValidationGroup="vgAddMPGeneralResourceInstruction" />
                                </FooterTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                    </asp:DataGrid>
                </div>
                <div class="formBlock">
                    <h1>Contact Details</h1>
                    <asp:DataGrid ID="dgMPContactNumbers" runat="server" Width="100%" AllowSorting="False"
                        AutoGenerateColumns="False" AllowPaging="False" CssClass="Grid" ShowFooter="true"
                        OnItemCommand="dgMPContactNumbers_ItemCommand" OnItemDataBound="dgMPContactNumbers_ItemDataBound"
                        OnPreRender="dgMPContactNumbers_PreRender" EnableViewState="false">
                        <HeaderStyle CssClass="HeadingRow" Height="20" VerticalAlign="middle" />
                        <ItemStyle Height="20" CssClass="Row" />
                        <AlternatingItemStyle Height="20" BackColor="WhiteSmoke" />
                        <SelectedItemStyle Height="20" CssClass="SelectedRow" />
                        <Columns>
                            <asp:TemplateColumn HeaderText="Name">
                                <ItemTemplate>
                                    <%#((string)((System.Data.DataRowView)Container.DataItem)["Data"]).Split('|')[0] %>
                                    <asp:HiddenField ID="hidReportSetting" runat="server"></asp:HiddenField>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtContactNumberName" runat="server" Width="250px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvContactNumberName" runat="server" ControlToValidate="txtContactNumberName"
                                        Display="Dynamic" ErrorMessage="Please supply a value for this contact number name."
                                        ValidationGroup="vgEditMPContactNumber"><img src="../images/Error.gif" height="16" width="16" title="Please supply a value for this contact number name." alt="Error"/></asp:RequiredFieldValidator>
                                    <asp:HiddenField ID="hidReportSetting" runat="server"></asp:HiddenField>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtNewContactNumberName" runat="server" Width="250px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvNewContactNumberName" runat="server" ControlToValidate="txtNewContactNumberName"
                                        Display="Dynamic" ErrorMessage="Please supply a value for this contact number name."
                                        ValidationGroup="vgAddMPContactNumber"><img src="../images/Error.gif" height="16" width="16" title="Please supply a value for this contact number name." alt="Error" /></asp:RequiredFieldValidator>
                                </FooterTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Quickdial">
                                <ItemTemplate>
                                    <%#((string)((System.Data.DataRowView)Container.DataItem)["Data"]).Split('|')[1]%>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtContactNumberQuickdial" runat="server" Width="250px"></asp:TextBox>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtNewContactNumberQuickdial" runat="server" Width="250px"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateColumn>
                            <asp:TemplateColumn HeaderText="Number">
                                <ItemTemplate>
                                    <%#((string)((System.Data.DataRowView)Container.DataItem)["Data"]).Split('|')[2]%>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtContactNumber" runat="server" Width="250px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvContactNumber" runat="server" ControlToValidate="txtContactNumber"
                                        Display="Dynamic" ErrorMessage="Please supply a value for this contact number."
                                        ValidationGroup="vgEditMPContactNumber"><img src="../images/Error.gif" height="16" width="16" title="Please supply a value for this contact number." alt="Error"/></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtNewContactNumber" runat="server" Width="250px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvNewContactNumber" runat="server" ControlToValidate="txtNewContactNumber"
                                        Display="Dynamic" ErrorMessage="Please supply a value for this contact number."
                                        ValidationGroup="vgAddMPContactNumber"><img src="../images/Error.gif" height="16" width="16" title="Please supply a value for this contact number." alt="Error"/></asp:RequiredFieldValidator>
                                </FooterTemplate>
                            </asp:TemplateColumn>
                            <asp:ButtonColumn ButtonType="PushButton" Text=" &#9660; " CausesValidation="false"
                                CommandName="Down"></asp:ButtonColumn>
                            <asp:ButtonColumn ButtonType="PushButton" Text=" &#9650; " CausesValidation="false"
                                CommandName="Up"></asp:ButtonColumn>
                            <asp:EditCommandColumn ButtonType="PushButton" UpdateText="Update" CancelText="Cancel"
                                EditText="Edit" HeaderText="Edit" ValidationGroup="vgEditMPContactNumber"></asp:EditCommandColumn>
                            <asp:TemplateColumn>
                                <ItemTemplate>
                                    <asp:Button ID="btnDelete" runat="server" CommandName="Delete" Text="Delete" />
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:Button ID="btnInsert" runat="server" CommandName="Insert" Text="Add" ValidationGroup="vgAddMPContactNumber" />
                                </FooterTemplate>
                            </asp:TemplateColumn>
                        </Columns>
                    </asp:DataGrid>
                </div>
            </div>
        </telerik:RadPageView>
        <telerik:RadPageView ID="GroupageView" runat="server">
            <div class="formBlock" style="width: 600px;">
                <h1>Groupage settings</h1>
                <table>
                    <tr>
                        <td class="formCellLabel" style="width: 220px">Default Collection Point to Use When Planning Deliveries before Collections
                        </td>
                        <td class="formCellField">
                            <telerik:RadComboBox ID="cboPoint" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                                MarkFirstMatch="true" AllowCustomText="false" ShowMoreResultsBox="false" Width="355px"
                                Height="300px" Overlay="true" OnClientDropDownClosed="Point_CombBoxClosing">
                                <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetPoints" />
                            </telerik:RadComboBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel">Default Return Point for Orders Involved in an Attempted Delivery
                        </td>
                        <td class="formCellField">
                            <telerik:RadComboBox ID="cboDefaultAttemptedDeliveryReturnPoint" runat="server" EnableLoadOnDemand="true"
                                ItemRequestTimeout="500" MarkFirstMatch="true" AllowCustomText="false" ShowMoreResultsBox="false"
                                Width="355px" Height="70px" Overlay="true" OnClientDropDownClosed="Point_CombBoxClosing">
                                <WebServiceSettings Method="GetPoints" Path="/ws/combostreamers.asmx" />
                            </telerik:RadComboBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel">Default Column for Sorting Groupage Grid "Deliveries", "Collections" and "Find Order"
                        </td>
                        <td class="formCellField">
                            <telerik:RadComboBox ID="cboDefaultSort" runat="server" EnableLoadOnDemand="true"
                                ItemRequestTimeout="500" MarkFirstMatch="true" AllowCustomText="false" ShowMoreResultsBox="false"
                                Width="355px" Height="70px" Overlay="true">
                            </telerik:RadComboBox>
                        </td>
                    </tr>
                </table>
            </div>
        </telerik:RadPageView>
        <telerik:RadPageView ID="ClientCustomersView" runat="server">
            <div class="formBlock">
                <h1>List of Clients</h1>
                <telerik:RadGrid ID="grdClients" runat="server" EnableAJAX="true" Skin="Office2007"
                    AllowPaging="true" PageSize="20" AutoGenerateColumns="false">
                    <MasterTableView DataKeyNames="IdentityId, RelatedIdentityId">
                        <Columns>
                            <telerik:GridTemplateColumn HeaderText="Name">
                                <ItemTemplate>
                                    <a href='addupdateorganisation.aspx?identityId=<%# DataBinder.Eval(Container.DataItem,"IdentityId")%>&parentIdentityId=<%=m_identityId%>&parentOrganisationName=<%=m_organisationName%>'>
                                        <%#DataBinder.Eval(Container.DataItem, "OrganisationName")%>
                                    </a>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="MainTelNo" HeaderText="Telephone" SortExpression="MainTelNo">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="MainFaxNo" HeaderText="Fax" SortExpression="MainFaxNo">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="FullName" HeaderText="Contact" SortExpression="FullName">
                            </telerik:GridBoundColumn>
                            <telerik:GridButtonColumn ImageUrl="~/images/itxt_xButton.gif" CommandName="RemoveRelationShip"
                                ButtonType="ImageButton" UniqueName="imbRemoveRelationShip" ConfirmText="Are you sure you want to remove this customer ?"
                                HeaderStyle-Width="18">
                            </telerik:GridButtonColumn>
                        </Columns>
                    </MasterTableView>
                    <ClientSettings>
                        <Selecting AllowRowSelect="true" />
                    </ClientSettings>
                </telerik:RadGrid>
            </div>
        </telerik:RadPageView>
        <telerik:RadPageView ID="ContactsView" runat="server">
            <div class="formBlock">
                <h1>Organisation Contacts</h1>
                <telerik:RadGrid runat="server" ID="grdContacts" Skin="Office2007" EnableAJAX="false"
                    AutoGenerateColumns="false">
                    <MasterTableView CommandItemDisplay="Top" CommandItemStyle-Height="30" EditMode="EditForms"
                        DataKeyNames="IndividualId">
                        <EditFormSettings ColumnNumber="2">
                            <FormTableItemStyle Wrap="False" Width="100%"></FormTableItemStyle>
                            <FormMainTableStyle GridLines="Horizontal" CellSpacing="0" CellPadding="3" Width="100%" />
                            <FormTableStyle GridLines="None" CellSpacing="0" CellPadding="2" BackColor="white"
                                Width="100%" />
                            <FormTableAlternatingItemStyle Wrap="False"></FormTableAlternatingItemStyle>
                            <EditColumn ButtonType="PushButton" InsertText="Insert Contact" UpdateText="Update record"
                                UniqueName="EditCommandColumn1" CancelText="Cancel edit">
                            </EditColumn>
                            <FormTableButtonRowStyle BackColor="#FFFFE7" HorizontalAlign="Left"></FormTableButtonRowStyle>
                        </EditFormSettings>
                        <CommandItemTemplate>
                            <asp:Button ID="btnAddNewContact" CssClass="buttonClassSmall" CommandName="InitInsert"
                                runat="server" Text="Add New Contact" Visible='<%# ! IsContactGridInEditMode() %>'></asp:Button>
                        </CommandItemTemplate>
                        <Columns>
                            <telerik:GridEditCommandColumn>
                                <ItemStyle Width="50px" />
                            </telerik:GridEditCommandColumn>
                            <telerik:GridBoundColumn HeaderText="IndividualId" DataField="IndividualId" ReadOnly="True"
                                UniqueName="IndividualId" Display="False" />
                            <telerik:GridDropDownColumn UniqueName="Title" ListDataMember="Titles" ListValueField="Title"
                                ListTextField="Title" DataField="Title" HeaderText="Title" />
                            <telerik:GridBoundColumn UniqueName="FirstName" DataField="FirstName" HeaderText="First Names" />
                            <telerik:GridBoundColumn UniqueName="LastName" DataField="LastName" HeaderText="Last Name" />
                            <telerik:GridDropDownColumn UniqueName="IndividualContactType" ListDataMember="IndividualContactType"
                                ListValueField="IndividualContactType" ListTextField="IndividualContactType"
                                DataField="IndividualContactType" HeaderText="Contact Type" />
                            <telerik:GridBoundColumn UniqueName="Email" DataField="Email" HeaderText="Email"
                                EditFormColumnIndex="1" />
                            <telerik:GridBoundColumn UniqueName="Telephone" DataField="Telephone" HeaderText="Telephone"
                                EditFormColumnIndex="1" />
                            <telerik:GridBoundColumn UniqueName="Fax" DataField="Fax" HeaderText="Fax" EditFormColumnIndex="1" />
                            <telerik:GridBoundColumn UniqueName="MobilePhone" DataField="MobilePhone" HeaderText="Mobile"
                                EditFormColumnIndex="1" />
                            <telerik:GridBoundColumn UniqueName="PersonalMobile" DataField="PersonalMobile" HeaderText="Personal Mobile"
                                EditFormColumnIndex="1" />
                            <telerik:GridButtonColumn ImageUrl="~/images/itxt_xButton.gif" CommandName="Delete"
                                ButtonType="ImageButton" UniqueName="btnRemoveContact" ConfirmText="Are you sure you want to remove this Contact ?"
                                HeaderStyle-Width="18" />
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </div>
        </telerik:RadPageView>
        <telerik:RadPageView ID="ExportMessagesView" runat="server">
            <div class="formBlock exportMessagesTab">
                <h1>Export Messages <span runat="server" ID="exportMessagesShowingMessage">(showing 10 most recent)</span></h1>
                <telerik:RadGrid runat="server" ID="grdExportMessages" Skin="Office2007" EnableAJAX="false"
                    AutoGenerateColumns="false">
                    <MasterTableView CommandItemDisplay="Top" CommandItemStyle-Height="30"> 
                        <CommandItemSettings ShowAddNewRecordButton="false"  />
                         <CommandItemTemplate>
                            <div class="overlayedRefreshIcon">
                                <asp:Button ID="btnRefresh" runat="server" Text="Refresh" OnClick="btnRefresh_Click"/>
                            </div>
                                <table>
                                    <tr>
                                        <td class="formCellLabel">
                                            Date From
                                        </td>                                     
                                         <td class="formCellField">
                                            <telerik:RadDatePicker ID="dteExportMessagesStartDate" runat="server" ToolTip="Start date of messages to display.">
                                            <DateInput runat="server"
                                            DateFormat="dd/MM/yy">
                                            </DateInput>
                                            </telerik:RadDatePicker>
                                        <td class="formCellLabel">
                                            Date To
                                        </td>
                                        <td class="formCellField">
                                            <telerik:RadDatePicker ID="dteExportMessagesEndDate" runat="server"  ToolTip="End date of messages to display.">
                                            <DateInput Runat="server"
                                            DateFormat="dd/MM/yy">
                                            </DateInput>
                                            </telerik:RadDatePicker>
                                        </td>                             
                                    </tr>
                              </table>
                        </CommandItemTemplate>                   
                        <Columns>                        
                            <telerik:GridBoundColumn HeaderText="Message ID" DataField="MessageId" ReadOnly="True"
                                UniqueName="MessageId" />
                            <telerik:GridHyperLinkColumn HeaderText="Run ID" 
                                                          DataTextField="RunId"
                                                          DataNavigateUrlFields="RunId"
                                                          UniqueName="RunId"
                                                          DataNavigateUrlFormatString="javascript:openJobDetails({0})"></telerik:GridHyperLinkColumn>
                            <telerik:GridBoundColumn HeaderText="Order ID" DataField="OrderId" ReadOnly="True"
                                UniqueName="OrderId" />
                            <telerik:GridBoundColumn HeaderText="Filename" DataField="Filename" ReadOnly="True"
                                UniqueName="Filename" />
                            <telerik:GridBoundColumn HeaderText="Sent" DataField="Sent" ReadOnly="True"
                                UniqueName="Sent" />
                            <telerik:GridBoundColumn HeaderText="Status" DataField="Status" ReadOnly="True"
                                UniqueName="Status" />
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </div>
        </telerik:RadPageView>
    </telerik:RadMultiPage>
    <br />
    <div class="buttonbar">
        <asp:Button ID="btnAdd" runat="server" Text="Add" Width="75px" OnClick="btnAdd_Click" />
        <asp:Button ID="btnListClients" runat="server" Text="List Clients" OnClick="btnListClients_Click"
            CausesValidation="False" />
        <asp:Button ID="btnPromotetoClient" runat="server" Text="Promote to Client" OnClick="btnPromotetoClient_Click"
            CausesValidation="false" />
        <asp:Button ID="btnSubcontractAllocatedOrders" runat="server" Text="Subcontract Allocated Orders"
            CausesValidation="false" OnClientClick="subcontractAllocatedOrders(); return false;" />
    </div>
    <telerik:RadAjaxManager ID="ramOrganisation" runat="server" EnableAJAX="true">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="cboCountry">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="divClientCulture" />
                    <telerik:AjaxUpdatedControl ControlID="cboClosestTown" />
                    <telerik:AjaxUpdatedControl ControlID="addressLink" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="dgReferences">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="dgReferences" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
    </telerik:RadCodeBlock>

    <script type="text/javascript">
        var linkedClientsToAdd = [];
        var linkedClientsToRemove = [];

        function cboLinkedClient_itemsRequesting(sender, eventArgs) {
            try
            {
                var context = eventArgs.get_context();
	            context["DisplaySuspended"] = false;
            }
            catch (err) { }
        }

        function cboLinkedClient_itemsRequested(sender, eventArgs) {
            var thisClientItem = sender.findItemByValue(<%= m_identityId %>);

            if (thisClientItem !== null) {
                sender.get_items().remove(thisClientItem);
            }
        }

        function btnLinkedClientAdd_Clicked() {
            var combo = $find('<%= cboLinkedClient.ClientID %>');
            var item = combo.get_selectedItem();

            if (item === null) {
                return;
            }

            var client = { name: item.get_text(), id: item.get_value() };
            addLinkedClient(client);
            combo.clearSelection();
        }

        function addLinkedClient(client) {
            var listView = document.getElementById('tblLinkedClients');
            var table = listView.getElementsByTagName('tbody')[0];
            var tableRow = document.createElement('tr');
            tableRow.id = 'tblLinkedClientsRow_' + client.id;
            tableRow.innerHTML = '<td>' + client.name + '</td><td><button type=\'button\' style="float: right" onclick=\'removeLinkedClient(' + client.id + ')\'>Remove</button></td>';
            table.appendChild(tableRow);
            linkedClientsToAdd.push(client.id);
            var hidden = document.getElementById('<%= LinkedClientsToAdd.ClientID %>');
            hidden.value = linkedClientsToAdd;
        }

        function removeLinkedClient(clientId) {
            var listView = document.getElementById('tblLinkedClients');
            var table = listView.getElementsByTagName('tbody')[0];
            var row = document.getElementById('tblLinkedClientsRow_' + clientId);
            table.removeChild(row);
            linkedClientsToRemove.push(clientId);
            var hidden = document.getElementById('<%= LinkedClientsToRemove.ClientID %>');
            hidden.value = linkedClientsToRemove;
        }
    </script>

</asp:Content>
