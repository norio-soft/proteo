<%@ Page Language="C#" MasterPageFile="~/WizardMasterPage.Master" AutoEventWireup="true" CodeBehind="JobSheetFilter.aspx.cs" Inherits="Orchestrator.WebUI.Traffic.Filters.JobSheetFilter" Title="Run Sheet Filter" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
    
    <link rel="stylesheet" type="text/css" href="/style/styles.css" />
    <link rel="stylesheet" type="text/css" href="/style/helpTip.css" />
    <link rel="stylesheet" type="text/css" href="/style/newStyles.css" />
    <link rel="stylesheet" type="text/css" href="/style/newMasterpage.css" /> 
    
    <script language="javascript" src="/script/tooltippopups.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Change Filter</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="layoutContentMiddleInner">
        <fieldset>
            <table width="99%">
                <tr>
                    <td width="25%" class="formCellLabel">Control Area</td>
                    <td class="formCellField">
                        <asp:RadioButtonList id="cboControlArea" runat="server" DataValueField="ControlAreaId" DataTextField="Description" RepeatDirection="Horizontal" RepeatColumns="5" />
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">Traffic Area(s)</td>
                    <td class="formCellField checkboxListField">
                        <div class="overflowHandler" style="height:100px; width: 540px;">
                            <asp:CheckBoxList id="cboTrafficAreas" runat="server" DataValueField="TrafficAreaId" DataTextField="Description" RepeatDirection="Horizontal" RepeatColumns="5" />
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">Start Date</td>
                    <td class="formCellField">
                        <telerik:RadDatePicker ID="dteStartDate" runat="server">
                        <DateInput runat="server"
                        DisplayDateFormat="dd/MM/yy" 
                        DateFormat="dd/MM/yy">
                        </DateInput>
                        </telerik:RadDatePicker>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">End Date</td>
                    <td class="formCellField">
                        <telerik:RadDatePicker ID="dteEndDate" runat="server">
                        <DateInput runat="server"
                        DisplayDateFormat="dd/MM/yy" 
                        DateFormat="dd/MM/yy">
                        </DateInput>
                        </telerik:RadDatePicker>
                    </td>
                </tr>
                <tr>
                    <td class="formCellField">&nbsp;</td>
                    <td class="formCellField">By default the dates you see will cause loads to be shown from the start date until the end of the day before the end date, and deliveries between both dates.</td>
                </tr>
                <tr>
                    <td class="formCellLabel">Resource Depot</td>
                    <td class="formCellField">
                        <asp:DropDownList ID="cboDepot" runat="server" DataValueField="OrganisationLocationId" DataTextField="OrganisationLocationName" />
                    </td>
                </tr>
                <tr>
                    <td class="formCellField">&nbsp;</td>
                    <td class="formCellField">This will cause resources affiliated with the selected depot to drive the resource pool used when planning.</td>
                </tr>
                <tr>
                    <td class="formCellLabel">Available Filters</td>
                    <td class="formCellField">
                        <asp:DropDownList id="cboFilters" runat="server" AutoPostBack="true" DataValueField="FilterId" DataTextField="FilterName" />
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">Filter Name</td>
                    <td class="formCellField">
                        <asp:TextBox id="txtFilterName" runat="server" />
                        <asp:RequiredFieldValidator id="rfvFilterName" runat="server" ControlToValidate="txtFilterName" Display="Dynamic" ErrorMessage="You must provide a filter name when saving a filter."><img src="../../images/error.gif"  height="16" width="16" title="You must provide a filter name when saving a filter." /></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">Run State</td>
                    <td class="formCellField">
                        <asp:CheckBoxList id="chkJobStates" runat="server" RepeatDirection="Horizontal" RepeatColumns="3"></asp:CheckBoxList>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">Advanced</td>
                    <td class="formCellField" style="display:none;">
                        <table>
							
							<tr>
								<td valign="top">
									<asp:CheckBox id="chkOnlyShowJobsWithPCVs" runat="server" Text="Only Show Jobs with PCVs" Text-Align="Right"></asp:CheckBox>
								</td>
							</tr>
							<tr>
								<td valign="top">
									<asp:CheckBox id="chkOnlyShowJobsWithDemurrage" runat="server" Text="Only Show Jobs with Demurrage" Text-Align="Right"></asp:CheckBox>
								</td>
							</tr>
							<tr>
								<td valign="top" colspan="2">
									<asp:CheckBox id="chkOnlyShowJobsWithDemurrageAwaitingAcceptance" runat="server" Text="Only Show Jobs with Demurrage Awaiting Acceptance" Text-Align="Right"></asp:CheckBox>
								</td>
							</tr>
						</table>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">Business Type</td>
                    <td class="formCellField">
                    <input type="checkbox" id="chkSelectAllBusinessTypes" onclick="selectAllBusinessTypes(this);" checked='true' /><label for="chkSelectAllBusinessTypes">Select All</label>
                    <asp:CheckBoxList runat="server" id="cblBusinessType" runat="server" RepeatDirection="Horizontal" RepeatColumns="6"></asp:CheckBoxList> </td>
                </tr>
            </table>
        </fieldset>

        <div class="buttonbar">                				
	        <nfvc:NoFormValButton id="btnFilter" runat="server" NoFormValList="rfvFilterName" Text="Apply Filter"></nfvc:NoFormValButton>
            <nfvc:NoFormValButton id="btnSaveFilter" runat="server" NoFormValList="rfvStartDate,rfvEndDate" Text="Save Filter"></nfvc:NoFormValButton>
            <asp:Button ID="btnClose" runat="server" Text="Close" CausesValidation="False" />
        </div>
        
    </div>

    <script type="text/javascript" language="javascript">
        $(document).ready(function() {
            var chkSelectAllTrafficAreas = $(':checkbox[id*=chkSelectAllTrafficAreas]').click(
                function(index, ele) {
                    var checked = $(chkSelectAllTrafficAreas).prop("checked");
                    $(':checkbox[id*=cboTrafficAreas]').attr("checked", checked);
                }
            );
        });

        function selectAllBusinessTypes(sender) {
            $('input:checkbox[id*=cblBusinessType]').prop('checked', $(sender).prop('checked'));
        }
    </script>

</asp:Content>
