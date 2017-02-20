<%@ Reference Page="~/organisation/addupdateorganisation.aspx" %>

<%@ Page Language="c#" Inherits="Orchestrator.WebUI.Organisation.addupdateorganisationlocation" CodeBehind="addupdateorganisationlocation.aspx.cs" MasterPageFile="~/default_tableless.master" Title="Add/Update Organisation Location" %>
<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Point" Src="~/UserControls/Point.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div style="background-color: White; padding: 3px;">
    <asp:label id="lblWhereAmI" style="padding-bottom: 5px" runat="server" visible="true"></asp:label>
</div>
<asp:validationsummary id="valSum" runat="server" showsummary="False" showmessagebox="True"></asp:validationsummary>
<br>
<asp:label id="lblConfirmation" runat="server" visible="false" cssclass="confirmation"
    text="The new location has been added successfully."></asp:label>
<asp:label id="lblReturnLink" style="padding-bottom: 5px" runat="server" visible="false"></asp:label>
<fieldset>
    <legend>General Information</legend>
    <table>
        <tr>
            <td class="formCellLabel">Location Name</td>
            <td>
                <asp:textbox id="txtLocationName" runat="server" width="250"></asp:textbox>
                <asp:requiredfieldvalidator id="rfvLocationName" runat="server" errormessage="Please enter the the name of the location."
                    controltovalidate="txtLocationName" display="Dynamic">
						<img src="../images/Error.gif" height="16" width="16" title="Please enter the the name of the location." /></asp:requiredfieldvalidator>
            </td>
        </tr>
        <tr>
            <td class="formCellLabel">Type</td>
            <td>
                <asp:dropdownlist id="cboType" runat="server" datatextfield="Description" datavaluefield="OrganisationLocationTypeID"></asp:dropdownlist>
            </td>
        </tr>
        <tr>
            <td class="formCellLabel">Title</td>
            <td>
                <asp:dropdownlist id="cboTitle" runat="server"></asp:dropdownlist>
            </td>
        </tr>
        <tr>
            <td class="formCellLabel">First Names</td>
            <td>
                <asp:textbox id="txtFirstNames" runat="server"></asp:textbox>
                <asp:requiredfieldvalidator id="rfvFirstnames" runat="server" enableclientscript="False"
                    errormessage="Please enter the First Name(s) for the Contact." controltovalidate="txtFirstNames"
                    display="Dynamic">
						<img src="../images/Error.gif" height="16" width="16" title="Please enter the First Name(s) for the Contact." /></asp:requiredfieldvalidator>
            </td>
        </tr>
        <tr>
            <td class="formCellLabel">Last Name</td>
            <td>
                <asp:textbox id="txtLastName" runat="server"></asp:textbox>
                <asp:requiredfieldvalidator id="rfvLastName" runat="server" enableclientscript="False"
                    errormessage="Please enter the Last Name for the Contact." controltovalidate="txtLastName"
                    display="Dynamic">
						<img src="../images/Error.gif" height="16" width="16" title="Please enter the Last Name for the Contact." /></asp:requiredfieldvalidator>
            </td>
        </tr>
        <tr>
            <td class="formCellLabel">Email Address</td>
            <td>
                <asp:textbox id="txtEmailAddress" runat="server" width="300"></asp:textbox>
            </td>
        </tr>
        <tr>
            <td class="formCellLabel">Telephone Number</td>
            <td>
                <asp:textbox id="txtTelephone" runat="server"></asp:textbox>
            </td>
        </tr>
        <tr>
            <td class="formCellLabel">Fax Number</td>
            <td>
                <asp:textbox id="txtFax" runat="server"></asp:textbox>
            </td>
        </tr>
    </table>
</fieldset>
<br>

	
<asp:Panel runat="server" ID="pnlAddress">
<fieldset>
    <legend>Address</legend>
    <uc1:infringementDisplay ID="infringementDisplay" runat="server" Visible="false">
    </uc1:infringementDisplay>
    <table>
     <tr>
            <td class="formCellLabel">Post Code</td>
            <td>
                <asp:textbox id="txtPostCode" runat="server"></asp:textbox>
                <asp:LinkButton ID="lnkLookUp" runat="server" Text="Find" Style="font-size: 10px;" ValidationGroup="Lookup"></asp:LinkButton>
            </td>
        </tr>

        <tr>
            <td class="formCellLabel">Country</td>
            <td colspan="2">
                <telerik:RadComboBox ID="cboCountry" runat="server" EnableLoadOnDemand="false" ItemRequestTimeout="500"
                    AutoPostBack="true" MarkFirstMatch="true" RadControlsDir="~/script/RadControls/"
                    AllowCustomText="false" ShowMoreResultsBox="false" Skin="WindowsXP" Width="355px"
                    Height="200px" Overlay="true" OnClientSelectedIndexChanged="CountryOnClientSelectedIndexChanged" />
                <asp:requiredfieldvalidator id="rfvCountry" runat="server" controltovalidate="cboCountry"
                    errormessage="<img src='../images/error.png' Title='Please select a country.'>"
                    enableclientscript="False"></asp:requiredfieldvalidator>
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="formCellLabel">Closest Town</td>
            <td>
                <telerik:RadComboBox ID="cboClosestTown" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                    MarkFirstMatch="false" RadControlsDir="~/script/RadControls/" ShowMoreResultsBox="false"
                    Skin="WindowsXP" Width="355px" Height="400px" OnClientItemsRequesting="ClosestTownRequesting">
                    <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetClosestTown" />
                </telerik:RadComboBox>
                <asp:requiredfieldvalidator id="rfvTownId" runat="server" controltovalidate="cboClosestTown"
                    errormessage="<img src='../images/error.png' Title='Please select the town closest to this point.'>"
                    enableclientscript="False"></asp:requiredfieldvalidator>
                <asp:customvalidator id="cfvTownId" runat="server" controltovalidate="cboClosestTown"
                    enableclientscript="False" errormessage="<img src='../images/error.png' Title='Please select the town closest to this point.'>"
                    onservervalidate="cfvTown_ServerValidate"></asp:customvalidator>
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="formCellLabel">Address Line 1</td>
            <td>
                <input id="hidOrganisationName" type="hidden" runat="server">
                <asp:textbox id="txtAddressLine1" runat="server" width="250"></asp:textbox>
                <asp:requiredfieldvalidator id="rfvAddressLine1" runat="server" errormessage="Please enter the the first line of the Address."
                    controltovalidate="txtAddressLine1" display="Dynamic">
						<img src="../images/Error.gif" height="16" width="16" title="Please enter the the first line of the Address." /></asp:requiredfieldvalidator>
            </td>
        </tr>
        <tr>
            <td class="formCellLabel">Address Line 2</td>
            <td>
                <asp:textbox id="txtAddressLine2" runat="server"></asp:textbox>
            </td>
        </tr>
        <tr>
            <td class="formCellLabel">Address Line 3</td>
            <td>
                <asp:textbox id="txtAddressLine3" runat="server"></asp:textbox>
            </td>
        </tr>
        <tr>
            <td class="formCellLabel">Post Town</td>
            <td>
                <asp:textbox id="txtPostTown" runat="server"></asp:textbox>
            </td>
        </tr>
        <tr>
            <td class="formCellLabel">County</td>
            <td>
                <asp:textbox id="txtCounty" runat="server"></asp:textbox>
            </td>
        </tr>
       
        <tr>
            <td class="formCellLabel">Traffic Area</td>
            <td>
                <asp:dropdownlist id="cboTrafficArea" runat="server" />
                <asp:customvalidator runat="server" id="cvTrafficArea" onservervalidate="cboTrafficAreaValidator_ServerValidate"
                    controltovalidate="cboTrafficArea" errormessage="You must specify either a postcode or a traffic area.">
                            <img id="imgTrafficAreaCustomValidatorError" runat="server" src="~/images/error.png" title="You must either select a traffic area or enter a postcode." /></asp:customvalidator>
            </td>
        </tr>
        <tr>
            <td class="formCellLabel">Grid Reference</td>
            <td>
                <input type="hidden" ID="hdnSetPointRadius" runat="server" name="hdnSetPointRadius" />
                <asp:textbox id="txtLongitude" runat="server" text="0" width="150"></asp:textbox>
                <asp:textbox id="txtLatitude" runat="server" text="0" width="150"></asp:textbox>
                <input type="hidden" id="hidTrafficArea" runat="server" name="hidTrafficArea"/>
            </td>
        </tr>
    </table>
</fieldset>
</asp:Panel>
<asp:Panel ID="pnlAddressList" runat="server" Visible="false">
				<asp:ListBox ID="lstAddress" runat="server" Rows="10" Width="100%" AutoPostBack="true">
				</asp:ListBox>
				<div style="height: 22px; padding: 2px; color: #ffffff;
					background-color: #99BEDE; text-align: right;">
					<asp:Button ID="btnCancelList" runat="server" Text="Cancel" CausesValidation="false" />
				</div>
			</asp:Panel>

<div class="buttonbar">
    <asp:button id="btnAdd" runat="server" text="Add" width="75px" onclick="btnAdd_Click"></asp:button>
</div>

<script language="javascript" type="text/javascript" >

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
        
 </script>
 
 </asp:Content>