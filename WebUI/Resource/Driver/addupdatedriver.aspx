<%@ Register Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI" TagPrefix="cc1" %>
<%@ Page Language="c#" MasterPageFile="~/WizardMasterPage.master" Title="Driver Details"
    Inherits="Orchestrator.WebUI.Resource.Driver.AddUpdateDriver" CodeBehind="AddUpdateDriver.aspx.cs" %>

<asp:content id="Titlebar1" ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
    Configure the Driver Details
</asp:content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script language="javascript" type="text/javascript">

	<!--
        var AddressLine1 = null;
        var AddressLine2 = null;
        var AddressLine3 = null;
        var PostTown = null;
        var County = null;
        var PostCode = null;
        var longitude = null;
        var latitude = null;
        var TrafficArea = null;
        var searchTown = null;
        var searchStreet = null;
        var searchCompany = null;
        var setPointRadius = null;

        function openChecker() {

            AddressLine1 = $('input[id*=<%=txtAddressLine1.ClientID%>]')[0];
            AddressLine2 = $('input[id*=<%=txtAddressLine2.ClientID%>]')[0];
            AddressLine3 = $('input[id*=<%=txtAddressLine3.ClientID%>]')[0];
            PostTown = $('input[id*=<%=txtPostTown.ClientID%>]')[0];
            County = $('input[id*=<%=txtCounty.ClientID%>]')[0];
            PostCode = $('input[id*=<%=txtPostCode.ClientID%>]')[0];
            longitude = $('input[id*=<%=txtLongitude.ClientID%>]')[0];
            latitude = $('input[id*=<%=txtLatitude.ClientID%>]')[0];
            TrafficArea = $('input[id*=<%=hidTrafficArea.ClientID%>]')[0];
            searchTown = $('input[id*=<%=txtPostTown.ClientID%>]')[0];
            searchStreet = $('input[id*=<%=txtAddressLine1.ClientID%>]')[0];
            searchCompany = "";
            setPointRadius = $('input[id*=<%=hdnSetPointRadius.ClientID %>]')[0];

            var sURL = "../../addresslookup/fullwizard.aspx?";
	        
	        sURL += "PostCode=" + PostCode.value;
	        sURL += "&searchCompany=" + searchCompany.value;
	        sURL += "&searchTown=" + searchTown.value;

	        window.open(sURL, "Checker", "toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=0,resizable=0,width=560,height=450");
	    }
	//-->
    </script>
    
    <asp:Label ID="lblConfirmation" runat="server" Text="The new driver has been added successfully." CssClass="confirmation" Visible="false">The new driver has been added successfully.</asp:Label>
    <div style="height: 610px; overflow-y: scroll; padding: 0 10px 0 0;">
        <fieldset>
            <legend>Personal Details</legend>
            <table>
                <tr valign="top">
                    <td>
                        <table>
                            <tr>
                                <td class="formCellLabel">Title</td>
                                <td class="formCellField">
                                    <asp:DropDownList ID="cboTitle" runat="server">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">
                                    First Names
                                </td>
                                <td class="formCellField">
                                    <asp:TextBox ID="txtFirstNames" runat="server" MaxLength="100"></asp:TextBox>
                                </td>
                                <td class="formCellField">
                                    <asp:RequiredFieldValidator ID="rfvFirstnames" runat="server" ErrorMessage="Please enter the First Name(s) for the driver."
                                        ControlToValidate="txtFirstNames" Display="Dynamic"><img src="../../images/Error.gif" height='16' width='14' title='Please enter the First Name(s) for the driver.' alt="Error"/></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">
                                    Last Names
                                </td>
                                <td class="formCellField">
                                    <asp:TextBox ID="txtLastName" runat="server" MaxLength="100"></asp:TextBox>
                                </td>
                                <td class="formCellField">
                                    <asp:RequiredFieldValidator ID="rfvLastName" runat="server" ErrorMessage="Please enter the Last Name for the driver."
                                        ControlToValidate="txtLastName" Display="Dynamic"><img src="../../images/Error.gif" height='16' width='14' title='Please enter the Last Name for the driver.' /></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr id="trPasscode" runat="server">
                                <td class="formCellLabel">
                                    Passcode
                                </td>
                                <td class="formCellField">
                                    <asp:TextBox ID="txtPasscode" runat="server" MaxLength="4"></asp:TextBox>
                                </td>
                                <td class="formCellField">
                                    <asp:RegularExpressionValidator ID="revPasscode" ValidationExpression="^\d{1,8}$" runat="server" ErrorMessage="Please enter a passcode that is numeric and a maximum of 4 digits. "
                                        ControlToValidate="txtPasscode" Display="Dynamic" CssClass="validationErrorIcon"><img src="../../images/Error.gif" height="16" width="14" title="Please enter a passcode that is numeric and a maximum of 4 digits." alt="Please enter a passcode that is numeric and a maximum of 4 digits."/></asp:RegularExpressionValidator>
                                </td>
                            </tr>
                            <tr id="trDriverType" runat="server">
                                <td class="formCellLabel">
                                    Type of Driver
                                </td>
                                <td class="formCellField">
                                    <asp:DropDownList ID="cboDriverType" runat="server" Width="160">
                                    </asp:DropDownList>
                                </td>
                                <td class="formCellField">
                                    <asp:RequiredFieldValidator ID="rfvDriverType" runat="server" ControlToValidate="cboDriverType"
                                        ErrorMessage="Please select the driver type." Display="Dynamic"><img src="../../images/Error.gif" title="Please select the driver type." alt='Error'/></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr id="trDOB" runat="server">
                                <td class="formCellLabel">
                                    Date Of Birth
                                </td>
                                <td class="formCellField">
                                    <telerik:RadDateInput ID="dteDOB" runat="server" DisplayDateFormat="dd/MM/yyyy" dateformat="dd/MM/yyyy" ToolTip="The Driver's Date of Birth">
                                    </telerik:RadDateInput>
                                </td>
                                <td class="formCellField">
                                    <asp:RequiredFieldValidator ID="rfvDOB" runat="server" ErrorMessage="Please enter the Date Of Birth for the driver."
                                        ControlToValidate="dteDOB" Display="Dynamic"><img src="../../images/Error.gif" height='16' width='14' title='Please enter the Date Of Birth for the driver.' alt='Error'/></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr id="trStartDate" runat="server">
                                <td class="formCellLabel">Start Date
                                </td>
                                <td class="formCellField">
                                    <telerik:RadDateInput ID="dteSD" runat="server" DisplayDateFormat="dd/MM/yyyy" DateFormat="dd/MM/yyyy" ToolTip="The Driver's Start Date">
                                    </telerik:RadDateInput>
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td>
                        <table height="58">
                            <tr id="trHomePhone" runat="server">
                                <td class="formCellLabel">
                                    Home Phone
                                </td>
                                <td class="formCellField">
                                    <asp:TextBox ID="txtTelephone" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">
                                    Mobile Phone
                                </td>
                                <td class="formCellField">
                                    <asp:TextBox ID="txtMobilePhone" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr id="trPersonalMobile" runat="server">
                                <td class="formCellLabel">
                                    Personal Mobile
                                </td>
                                <td class="formCellField">
                                    <asp:TextBox ID="txtPersonalMobile" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">
                                    Digital Tacho Card Id
                                </td>
                                <td class="formCellField">
                                    <asp:TextBox ID="txtDigitalTachoCardId" runat="server" MaxLength="255"></asp:TextBox>
                                </td>
                            </tr>
                            <tr id="trPayrollNumber" runat="server">
                                <td class="formCellLabel">
                                    Payroll No
                                </td>
                                <td class="formCellField">
                                    <asp:TextBox ID="txtPayrollNo" runat="server" MaxLength="100"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellField" colspan="2">
                                    <asp:CheckBox ID="chkAgencyDriver" runat="server" Text="This driver is an agency driver" onchange="chkAgencyChanged()"/>
                                </td>
                            </tr>
                            <tr id="trAgency" style="display: none">
                                <td class="formCellLabel">
                                    Agency
                                </td>
                                <td class="formCellField" colspan="3">
                                    <telerik:RadComboBox ID="cboAgency" runat="server" AllowCustomText="true" >

                                    </telerik:RadComboBox>
                                </td>
                                <td class="formCellField">
                                    <asp:CustomValidator ID="cvAgency" runat="server" ErrorMessage="Please enter an agency"
                                        ControlToValidate="cboAgency" Display="Dynamic" ClientValidationFunction="validateAgency" ValidateEmptyText="true"><img src="../../images/Error.gif" height="16" width="14" title="Please enter an agency." alt="Error" /></asp:CustomValidator>
                                </td> 
                            </tr>
                            <tr id="trPrefCommunication" runat="server">
                                <td class="formCellLabel">
                                    Preferred Communication Type
                                </td>
                                <td class="formCellField" >
                                    <asp:RadioButtonList ID="rblDefaultCommunicationType" runat="server">
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr ID="telematicsOption" runat="server" visible="false">
                                <td class="formCellLabel">Telematics Solution</td>
                                <td class="formCellField">
                                    <asp:DropDownList ID="cboTelematicsSolution" runat="server" DataTextField="Description"
                                        DataValueField="TelematicsSolutionID">
                                    </asp:DropDownList>
                                </td>
                                <td class="formCellField">
                                    <asp:RequiredFieldValidator ID="telematicsRequiredFieldValidator" runat="server" ControlToValidate="cboTelematicsSolution"
                                        ErrorMessage="Please select a telematics solution."><img src="../../images/Error.gif" height='16' width='14' title='Please select a telematics solution.' alt="Error"/></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </fieldset>
        
        <fieldset id="fsAddress" runat="server">
            <legend>Address</legend>
            <table>
                <tr>
                    <td class="formCellLabel" nowrap="nowrap">
                        Closest Town
                    </td>
                    <td class="formCellField" colspan="3">
                        <telerik:radcombobox id="cboTown" runat="server" enableloadondemand="true" itemrequesttimeout="500"
                            markfirstmatch="true" radcontrolsdir="~/script/RadControls/" showmoreresultsbox="true"
                            skin="WindowsXP" width="355px" overlay="true" zindex="50" height="200px" AllowCustomText="False">
                        </telerik:radcombobox>
                    </td>
                    <td class="formCellField">
                        <asp:RequiredFieldValidator ID="rfvClosestTown" runat="server" ErrorMessage="Please select the Closest Town."
                            ControlToValidate="cboTown" Display="Dynamic"><img src="../../images/Error.gif" height='16' width='14' title='Please select the Closest Town' alt="Error"/></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel" nowrap="nowrap">
                        Address Line 1
                    </td>
                    <td class="formCellField">
                        <asp:TextBox ID="txtAddressLine1" runat="server" autocomplete="off"></asp:TextBox>
                    </td>
                    <td class="formCellField">
                        <asp:RequiredFieldValidator ID="rfvAddressLine1" runat="server" ErrorMessage="Please enter the the first line of the Address."
                            ControlToValidate="txtAddressLine1" Display="Dynamic"><img src="../../images/Error.gif" height='16' width='14' title='Please enter the the first line of the Address.' alt="Error"/></asp:RequiredFieldValidator>
                    </td>
                    <td rowspan="8">
                        <a href="javascript:openChecker()">Find Address</a>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Address Line 2
                    </td>
                    <td class="formCellField">
                        <asp:TextBox ID="txtAddressLine2" runat="server" autocomplete="off"></asp:TextBox>
                    </td>
                    <td class="formCellField">
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Address Line 3
                    </td>
                    <td class="formCellField">
                        <asp:TextBox ID="txtAddressLine3" runat="server" autocomplete="off"></asp:TextBox>
                    </td>
                    <td class="formCellField">
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Post Town
                    </td>
                    <td class="formCellField">
                        <asp:TextBox ID="txtPostTown" runat="server" autocomplete="off"></asp:TextBox>
                    </td>
                    <td class="formCellField">
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        County
                    </td>
                    <td class="formCellField">
                        <asp:TextBox ID="txtCounty" runat="server" autocomplete="off"></asp:TextBox>
                    </td>
                    <td class="formCellField">
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Post Code
                    </td>
                    <td class="formCellField">
                        <asp:TextBox ID="txtPostCode" runat="server" autocomplete="off"></asp:TextBox>
                    </td>
                    <td class="formCellField">
                        <asp:RequiredFieldValidator ID="rfvPostCode" runat="server" ErrorMessage="Please enter the Post Code."
                            ControlToValidate="txtPostCode" Display="Dynamic"><img src="../../images/Error.gif" height='16' width='14' title='Please enter the Post Code.' alt="Error"/></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="formCellField">
                    </td>
                    <td class="formCellField">
                        <input type="hidden" id="hidTrafficArea" runat="server" name="hidTrafficArea" />
                    </td>
                </tr>
                <tr style="display: none;">
                    <td class="formCellLabel">
                        Grid Reference
                    </td>
                    <td class="formCellField">
                    <input type="hidden" id="hdnSetPointRadius" runat="server" name="hdnSetPointRadius" />
                        <asp:TextBox ID="txtLatitude" runat="server"></asp:TextBox>&nbsp;<asp:TextBox ID="txtLongitude"
                            runat="server"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </fieldset>
        
        <fieldset>
            <legend>Usual Vehicle</legend>
            <table>
                <tr>
                    <td class="formCellLabel">
                        Please select the Usual Vehicle for this driver
                    </td>
                    <td class="formCellField">
                        <asp:DropDownList ID="cboVehicle" runat="server">
                        </asp:DropDownList>
                    </td>
                    <td class="formCellField">
                        <asp:RequiredFieldValidator ID="rfvUsualVehicle" runat="server" ErrorMessage="Please enter the Usual Vehicle for this driver."
                            ControlToValidate="cboVehicle" Display="Dynamic" Height="18px"><img src="../../images/Error.gif" height='16' width='14' title='Please enter the Usual Vehicle for this driver.' alt="Error"/></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="formCellField">
                        <asp:HyperLink ID="hypAddNewVehicle" runat="server" Visible="false" NavigateUrl="../Vehicle/addupdatevehicle.aspx">Add New Vehicle</asp:HyperLink>
                    </td>
                </tr>
            </table>
        </fieldset>

        <fieldset id="fsResourceGrouping" runat="server">
            <legend>Resource Grouping</legend>
            <table border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td class="formCellLabel">Resource Grouping</td>
                    <td class="formCellField">
                        <telerik:RadTreeView ID="trvResourceGrouping" runat="server" MultipleSelect="false" DataTextField="Text"
                            DataFieldID="OrgUnitId" DataFieldParentID="ParentOrgUnitId"
                            CheckBoxes="true" />

                    </td>
                </tr>
            </table>
        </fieldset>
        
        <fieldset id="fsLKL" runat="server">
            <legend>Last Known Location</legend>
            <table border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td class="formCellLabel">Organisation</td>
                    <td class="formCellField">
                        <telerik:radcombobox id="cboOrganisation" runat="server" enableloadondemand="true"
                            itemrequesttimeout="500" markfirstmatch="true" onclientselectedindexchanged="OrganisationSelectedIndexChanged"
                            radcontrolsdir="~/script/RadControls/" showmoreresultsbox="true" skin="WindowsXP"
                            width="355px" overlay="true" zindex="50" height="75px">
                        </telerik:radcombobox>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">Point</td>
                    <td class="formCellField">
                        <telerik:radcombobox id="cboPoint" runat="server" enableloadondemand="true" itemrequesttimeout="500"
                            markfirstmatch="true" radcontrolsdir="~/script/RadControls/" showmoreresultsbox="true"
                            skin="WindowsXP" width="355px" onclientitemsrequesting="PointRequesting" overlay="true"
                            zindex="50" height="75px">
                        </telerik:radcombobox>
                        <asp:RequiredFieldValidator ID="rfvPoint" runat="server" ControlToValidate="cboPoint"
                            Display="Dynamic" ErrorMessage="Please select a last known location."><img src="../../images/error.gif" alt="Please select a last known location." /></asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="cfvPoint" runat="server" ControlToValidate="cboPoint" EnableClientScript="false"
                            Display="Dynamic" ErrorMessage="Please select a last known location."><img src="../../images/error.gif" alt="Please select a last known location. " /></asp:CustomValidator>
                    </td>
                </tr>
            </table>
        </fieldset>
        
        <fieldset id="fsBelongsTo" runat="server">
            <legend>Belongs to</legend>
            <table>
                <tr>
                    <td class="formCellField">This driver's depot is
                        <telerik:radcombobox id="cboDepot" runat="server" skin="WindowsXP" radcontrolsdir="~/script/RadControls/">
                        </telerik:radcombobox>
                        <asp:RequiredFieldValidator ID="rfvDepot" runat="server" ControlToValidate="cboDepot" InitialValue="--- [ Please Select ] ---"
                            ErrorMessage="Please select the driver's depot." Display="Dynamic"><img src="../../images/Error.gif" title="Please select the driver's depot." /></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="formCellField">
                        This driver is a&nbsp;&nbsp;<telerik:radcombobox id="cboControlArea" runat="server"
                            skin="WindowsXP" radcontrolsdir="~/script/RadControls/"></telerik:radcombobox>&nbsp;&nbsp;<telerik:radcombobox
                                id="cboTrafficArea" runat="server" skin="WindowsXP" radcontrolsdir="~/script/RadControls/"></telerik:radcombobox>&nbsp;resource.
                    </td>
                </tr>
                <tr>
                    <td class="formCellField">
                        This drivers planner is <telerik:RadComboBox ID="cboDriverPlanner" Skin="WindowsXP" InitialValue="--- [ Please Select ] ---" radcontrolsdir="~/script/RadControls/" runat="server"></telerik:RadComboBox>
                    </td>
                </tr>
            </table>
        </fieldset>
        
        <asp:Panel ID="pnlDriverDeleted" runat="server" Visible="False">
            <fieldset>
                <legend>Is Driver Deleted</legend>
                <asp:CheckBox ID="chkDelete" runat="server" Text="This driver is deleted."></asp:CheckBox>
            </fieldset>
        </asp:Panel>
    
    </div>
                               
    <div class="buttonbar">
        <asp:Button ID="btnAdd" runat="server" Text="Add" Width="75"></asp:Button>
        <asp:Button ID="btnCancel" OnClientClick="javascript:window.close();" runat="server" Text="Cancel" Width="75" CausesValidation="false" />
    </div>

    <script language="javascript" type="text/javascript">
    <!--
        function OrganisationSelectedIndexChanged(item) {
            var pointCombo = $find("<%=cboPoint.ClientID %>");
            pointCombo.set_text("");
            pointCombo.requestItems(item.get_value(), false);
        }

        function PointRequesting(sender, eventArgs) {
            var organisationCombo = $find("<%=cboOrganisation.ClientID %>");

            var context = eventArgs.get_context();
            context["FilterString"] = organisationCombo.get_value() + ";" + sender.get_text();
        }

        var agencyChkBox = $(<%=chkAgencyDriver.ClientID%>)[0];


        var agencyRow = $(trAgency);
        if(agencyChkBox.checked){
                agencyRow.css('display', 'table-row');
        }

        function chkAgencyChanged(sender, eventArgs) {
            var agencyChkBox = $(<%=chkAgencyDriver.ClientID%>)[0];
            var agencyRow = $(trAgency);
            if(agencyChkBox.checked){
                agencyRow.css('display', 'table-row');
            }
            else {
                agencyRow.css('display', 'none');
            }
  
        }

        function validateAgency(oSrc, args) {
            var agencyChkBox = $(<%=chkAgencyDriver.ClientID%>)[0];
            if(agencyChkBox.checked){
                args.IsValid = args.Value.length > 0;
            }
            
        }
    //-->
    </script>
</asp:Content>
