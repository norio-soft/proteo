<%@ Page Language="c#" Inherits="Orchestrator.WebUI.resource.vehicle.addupdatevehicle" CodeBehind="addupdatevehicle.aspx.cs" MasterPageFile="~/WizardMasterPage.master" Title="Vehicle Details" %>

<%@ Register TagPrefix="uc1" TagName="AuditHistory" Src="~/UserControls/resourceAuditHistory.ascx" %>
<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>
<%@ Register TagPrefix="cs" Namespace="Codesummit" Assembly="WebModalControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="height: 496px; width: 100%; overflow: auto;">
        <cs:WebModalWindowHelper ID="mwhelper" runat="server" ShowVersion="false"></cs:WebModalWindowHelper>
        
        <asp:Label ID="lblConfirmation" runat="server" Visible="false" CssClass="confirmation" Text="The new vehicle has been added successfully."></asp:Label>
        
        <fieldset>
            <legend><strong>Vehicle Details</strong></legend>
            <uc1:infringementDisplay ID="infringementDisplay" runat="server" Visible="false">
            </uc1:infringementDisplay>
            <table id="Table1" width="100%">
                <tr>
                    <td>
                        <table height="0">
                            <tr>
                                <td class="formCellLabel">Registration No.</td>
                                <td class="formCellField" width="159">
                                    <asp:TextBox CssClass="fieldInputBox" ID="txtRegistrationNo" runat="server" Width="152px" MaxLength="50"></asp:TextBox>
                                </td>
                                <td class="formCellField">
                                    <asp:RequiredFieldValidator ID="rfvRegistration" runat="server" ControlToValidate="txtRegistrationNo"
                                        Display="Dynamic" ErrorMessage="Please enter a Registration Number."><img src="../../images/Error.gif" height='16' width='14' title='Please enter a Registration Number.' alt="Error"/></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">Manufacturer</td>
                                <td class="formCellField">
                                    <asp:DropDownList ID="cboManufacturer" runat="server" AutoPostBack="True">
                                    </asp:DropDownList>
                                </td>
                                <td class="formCellField">
                                    <asp:RequiredFieldValidator ID="rfvManufacturer" runat="server" ControlToValidate="cboManufacturer"
                                        ErrorMessage="Please enter/select a Vehicle Manufacturer."><img src="../../images/Error.gif" height='16' width='14' title='Please enter a Manufacturer.' alt="Error"/></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">Model</td>
                                <td class="formCellField">
                                    <asp:DropDownList ID="cboModel" runat="server" Visible="False">
                                    </asp:DropDownList>
                                </td>
                                <td class="formCellField">
                                    <asp:RequiredFieldValidator ID="rfvModel" runat="server" ControlToValidate="cboModel"
                                        ErrorMessage="Please enter/select a Vehicle Model."><img src="../../images/Error.gif" height='16' width='14' title='Please enter a Model.' alt="Error"/></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">Chassis No</td>
                                <td class="formCellField">
                                    <asp:TextBox ID="txtChassisNo" CssClass="fieldInputBox" runat="server" Width="152px" MaxLength="50"></asp:TextBox>
                                </td>
                                <td class="formCellField"></td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">Cab Phone Number</td>
                                <td class="formCellField">
                                    <asp:TextBox ID="txtTelephoneNumber" CssClass="fieldInputBox" runat="server" Width="152px" MaxLength="50"></asp:TextBox>
                                </td>
                                <td class="formCellField"></td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">Class</td>
                                <td class="formCellField">
                                    <asp:DropDownList ID="cboClass" runat="server">
                                    </asp:DropDownList>
                                </td>
                                <td class="formCellField">
                                    <asp:RequiredFieldValidator ID="rfvClass" runat="server" ControlToValidate="cboClass"
                                        ErrorMessage="Please enter Class"><img src="../../images/Error.gif" height='16' width='14' title='Please enter a Class Number.' alt="Error"/></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr runat="server" id="trIsFixedUnit">
                                <td class="formCellLabel">Is Fixed Unit (Van/Rigid)</td>
                                <td class="formCellField">
                                    <asp:CheckBox ID="chkIsFixedUnit" runat="server" />
                                </td>
                                <td class="formCellField">&nbsp;</td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">MOT Expires On</td>
                                <td class="formCellField">
                                    <telerik:RadDatePicker ID="dteMOTExpiry" runat="server" ToolTip="The Date the Vehicle's MOT Expires"
                                        DisplayDateFormat="dd/MM/yyyy" dateformat="dd/MM/yyyy">
                                    </telerik:RadDatePicker>
                                </td>
                                <td class="formCellField">
                                    <asp:RequiredFieldValidator ID="rfvMOTExpiry" runat="server" ControlToValidate="dteMOTExpiry"
                                                                ErrorMessage="Please supply the date the vehicle's current MOT expires on"><img src="../../images/Error.gif" height='16' width='14' title='Please supply the date the current MOT expires on' alt="Error"/></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">Next Service Date</td>
                                <td class="formCellField">
                                    <telerik:RadDatePicker ID="dteServiceDate" runat="server" ToolTip="The Date the vehicle&amp;quote;s MOT Expires"
                                        DisplayDateFormat="dd/MM/yyyy" dateformat="dd/MM/yyyy">
                                    </telerik:RadDatePicker>
                                </td>
                                <td class="formCellField">
                                    <asp:RequiredFieldValidator ID="rfvServiceDate" runat="server" ControlToValidate="dteServiceDate"
                                                                ErrorMessage="Please supply the date this vehicle needs servicing on"><img src="../../images/Error.gif" height='16' width='14' title='Please supply the date this vehicle needs servicing on' alt="Error"/></asp:RequiredFieldValidator>&nbsp;&nbsp;<a
                                            href="VehicleService.aspx?vehicleId=<%= m_resourceId.ToString() %>" style="display: <%= m_isUpdate ? "" : "none" %>">Service History</a>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">Vehicle Type</td>
                                <td class="formCellField">
                                    <asp:DropDownList ID="cboVehicleType" runat="server" DataTextField="Description"
                                        DataValueField="VehicleTypeID">
                                    </asp:DropDownList>
                                </td>
                                <td class="formCellField">&nbsp;</td>
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
                            <tr>
                                <td class="formCellLabel">GPS Unit ID</td>
                                <td class="formCellField">
                                    <asp:TextBox ID="txtGPSUnitID" CssClass="fieldInputBox" runat="server" Width="250" />
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr runat="server" id="trThirdPartyIntergration">
                                <td class="formCellLabel">Third Party Integration ID</td>
                                <td class="formCellField">
                                    <asp:TextBox ID="txtThirdPartyIntegrationID" CssClass="fieldInputBox" runat="server" Width="250" MaxLength="19" />
                                    
                                </td>
                                    <td class="formCellField">
                                    <asp:RangeValidator ID="thirdPartyIntegrationIDRangeValidator" 
                                            ControlToValidate="txtThirdPartyIntegrationID" runat="server" 
                                            ErrorMessage="Not a valid ID. Please check with supplier of ID."
                                            SetFocusOnError="True" Type="Double"/>
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">Resource Grouping</td>
                                <td class="formCellField">
                                    <telerik:RadTreeView ID="trvResourceGrouping" runat="server" MultipleSelect="false" DataTextField="Text" 
                                                            DataFieldID="OrgUnitId" DataFieldParentID="ParentOrgUnitId" 
                                                            CheckBoxes="true"/>
                                 
                                </td>
                            </tr>
                            <tr id="ProfitabilityReportExclusionsRow" runat="server">
                                <td class="formCellLabel">Profitability Report</td>
                                <td class="formCellField">
                                    <a href="/administration/ProfitabilityReporting/JRProfitabilityManageVehicleExclusions.aspx?VehicleId=<%= m_resourceId.ToString() %>&rs=">Manage Profitability Report Vehicle Exclusions</a>
                                </td>
                                <td>&nbsp;</td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </fieldset>
        
        <fieldset runat="server" id="fsFinancialDetails">
            <legend>Financial Details</legend>
            <table id="Table3">
                <tr>
                    <td>
                        <table>
                            <tr>
                                <td class="formCellLabel" width="159">
                                    Nominal Code
                                </td>
                                <td class="formCellField">
                                    <asp:DropDownList runat="server" ID="cboNominalCode">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </fieldset>
        
        <fieldset id="fsKeyDetails" runat="server">
            <legend>Key Details</legend>
            <asp:DataGrid ID="dgdKeys" runat="server" Width="100%" AllowSorting="True" AutoGenerateColumns="False"
                OnSortCommand="dgdKeys_SortCommand" OnPageIndexChanged="dgdKeys_Page" PagerStyle-HorizontalAlign="Right"
                PagerStyle-Mode="NumericPages" PageSize="10" AllowPaging="True">
                <Columns>
                    <asp:TemplateColumn HeaderText="Name" SortExpression="VehicleKeyTypeDescription">
                        <ItemTemplate>
                            <a href='addupdatevehiclekey.aspx?resourceId=<%# DataBinder.Eval(Container.DataItem, "ResourceId")%>&vehicleKeyId=<%# DataBinder.Eval(Container.DataItem, "VehicleKeyId")%>&vehicleRegistrationNo=<%=vehicleRegistrationNo%>'>
                                <%#DataBinder.Eval(Container.DataItem, "VehicleKeyTypeDescription")%>
                            </a>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                    <asp:BoundColumn DataField="Serial" HeaderText="Serial" SortExpression="Serial">
                    </asp:BoundColumn>
                </Columns>
                <PagerStyle CssClass="DataGridListPagerStyle"></PagerStyle>
                <ItemStyle CssClass="DataGridListItem"></ItemStyle>
                <HeaderStyle CssClass="DataGridListHead"></HeaderStyle>
                <AlternatingItemStyle CssClass="DataGridListItemAlt"></AlternatingItemStyle>
            </asp:DataGrid>
            <br />
            <div class="buttonbar">
                <asp:Button ID="btnAddVehicleKey" runat="server" Text="Add Vehicle Key" CausesValidation="False">
                </asp:Button>
            </div>
        </fieldset>
        
        <fieldset runat="server" id="fsLastKnownLocation">
            <legend><strong>Last Known Location</strong></legend>
            <table border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td class="formCellLabel" width="159">
                        Organisation&nbsp;
                    </td>
                    <td class="formCellField">
                        <telerik:RadComboBox ID="cboOrganisation" runat="server" EnableLoadOnDemand="true"
                            ItemRequestTimeout="500" MarkFirstMatch="true" RadControlsDir="~/script/RadControls/"
                            OnClientSelectedIndexChanged="OrganisationSelectedIndexChanged" ShowMoreResultsBox="true" Skin="WindowsXP"
                            Width="355px" ZIndex="50" Height="200px" Overlay="true">
                        </telerik:RadComboBox>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Point&nbsp;
                    </td>
                    <td class="formCellField">
                        <telerik:RadComboBox ID="cboPoint" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                            MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" OnClientItemsRequesting="PointRequesting"
                            ShowMoreResultsBox="true" Skin="WindowsXP" Width="355px" ZIndex="50" Height="150px"
                            Overlay="true">
                        </telerik:RadComboBox>
                        <asp:RequiredFieldValidator ID="rfvPoint" runat="server" ControlToValidate="cboPoint"
                            Display="Dynamic" ErrorMessage="Please select a last known location."><img src="../../images/error.gif" alt="Please select a last known location." /></asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="cfvPoint" runat="server" ControlToValidate="cboPoint" EnableClientScript="false"
                            Display="Dynamic" ErrorMessage="Please select a last known location."><img src="../../images/error.gif" alt="Please select a last known location." /></asp:CustomValidator>
                    </td>
                </tr>
                <asp:Panel ID="pnlTrailerDetails" runat="server">
                    <tr>
                        <td class="formCellLabel">Trailer&nbsp;</td>
                        <td class="formCellField">
                            <asp:Label ID="lblTrailer" runat="server" />
                        </td>
                    </tr>
                </asp:Panel>
            </table>
        </fieldset>
        
        <fieldset runat="server" id="fsBelongsTo">
            <legend><strong>Belongs to</strong></legend>
            <table>
                <tr id="vehicleDepot" runat="server">
                    <td>
                        This vehicle's depot is&nbsp;&nbsp;
                        <asp:DropDownList ID="cboDepot" runat="server" />
                        <asp:RequiredFieldValidator ID="rfvDepot" runat="server" ControlToValidate="cboDepot"
                            ErrorMessage="Please select the vehicle's depot." Display="Dynamic"><img src="../../images/Error.gif" title="Please select the vehicle's depot." /></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>
                        This vehicle is a&nbsp;&nbsp;<asp:DropDownList ID="cboControlArea" runat="server" />
                        &nbsp;&nbsp;<asp:DropDownList ID="cboTrafficArea" runat="server" Width="140px" />
                        &nbsp;resource.
                    </td>
                </tr>
                <tr>
                    <td>
                        Dedicated to client&nbsp;&nbsp;
                        <telerik:RadComboBox ID="cboDedicatedToClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
						    AutoPostBack="false" MarkFirstMatch="true" CausesValidation="false" ShowMoreResultsBox="false"
                            OnClientItemsRequesting="cboClient_ItemsRequesting" Width="350px" Height="300px">
						    <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetClients" />
					    </telerik:RadComboBox>
                    </td>
                </tr>
            </table>
        </fieldset>
        
        <asp:Panel ID="pnlVehicleDeleted" runat="server" Height="40px" Visible="False">
            <fieldset>
                <legend><strong>Is Vehicle Deleted</strong></legend>
                <asp:CheckBox ID="chkDelete" runat="server" Text="This vehicle is deleted."></asp:CheckBox>
            </fieldset>
        </asp:Panel>
        
        <br />
        <div style="padding-top:10px;">
            <uc1:AuditHistory ID="audit" runat="server">
            </uc1:AuditHistory>
        </div>
    </div>
    
    <div class="buttonbar">
        <asp:Button ID="btnAdd" runat="server" Text="Add New" Width="75"></asp:Button>
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" Width="75" CausesValidation="false" />
    </div>

    <script language="javascript" type="text/javascript">
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

        function CancelAndGoToResourceGrouping() {
            var result = confirm("Are you sure you want to discard changes?");
            

            if ( result ) {
                window.open('../../CAN/AddUpdateOrgUnit.aspx', 'Resource Grouping', 'height=800,width=1600');
            }
            else {                
                return false;
            }

            return true;
        }

        function cboClient_ItemsRequesting(sender, eventArgs) {
            var context = eventArgs.get_context();
            context['TopItemText'] = '- none -';
        }

    </script>
</asp:Content>
