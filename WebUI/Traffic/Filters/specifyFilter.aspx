<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Traffic.Filters.specifyFilter" Codebehind="specifyFilter.aspx.cs" %>

<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Register TagPrefix="cs" Namespace="Codesummit" Assembly="WebModalControls" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>

<!doctype html>
<html lang="en">

    <head id="Head1" runat="server">
        <meta charset="utf-8" />
		
        <title>Change Filter</title>
        <base target="_self" />
        <script src="//ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js"></script>
        <script>window.jQuery || document.write('<script src="/script/jquery-1.10.2.min.js">\x3C/script>')</script>
        <script src="/script/jquery-migrate-1.2.1.js"></script>
        <script src="/script/show-modal-dialog.js"></script>
        <link rel="stylesheet" type="text/css" href="/style/styles.css" />
        <link rel="stylesheet" type="text/css" href="/style/helpTip.css" />
        <link rel="stylesheet" type="text/css" href="/style/newStyles.css" />
        <link rel="stylesheet" type="text/css" href="/style/newMasterpage.css" />    

        <style>
            .inProgressSubStatesRow
            {
                display: none;
            }
        </style>
    </head>
    <body style="margin: 0px; padding: 0px;">
        <form runat="server" Id="Form1" style="margin: 0px; padding: 0px;">
            <asp:ScriptManager runat="server"></asp:ScriptManager>
            <table id="Table2" cellpadding="0" cellspacing="0" border="0" style="width: 674px;">
                <tr>
                   <!-- <td class="layoutWizzardHeaderOuter">
                        <div class="layoutWizzardHeaderInner">
                            <p>&nbsp;|&nbsp;</p><asp:Label ID="lblWizardTitle" runat="server">Job sheet filter </asp:Label><div class="clearDiv"></div>
                        </div>
                    </td> -->
                </tr>
                <tr>
                    <td class="layoutContentTop">&nbsp;</td>
                </tr>
                <tr>
                    <td class="layoutContentMiddle" valign="top" align="left">
                        <div class="layoutContentMiddleInner">
                            <fieldset>
                                <table>
                                    <tr>
                                        <td width="100" class="formCellLabel">Control Area</td>
                                        <td class="formCellField" colspan="4">
                                            <asp:RadioButtonList id="cboControlArea" runat="server" DataValueField="ControlAreaId" DataTextField="Description" RepeatDirection="Horizontal" RepeatColumns="5" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="formCellLabel">Traffic Area(s)</td>
                                        <td class="formCellField checkboxListField" colspan="4" style="width: 550px !important;">
                                            <div runat="server" id="divTrafficAreas">
                                                <asp:CheckBox runat="server" ID="chkSelectAllTrafficAreas" Text="Select all Traffic Areas"  />
                                                <asp:CheckBoxList id="cboTrafficAreas" runat="server" DataValueField="TrafficAreaId" DataTextField="Description" RepeatDirection="Horizontal" RepeatColumns="5" />
                                            </div>
                                        </td>
                                    </tr>
                                    <tr align="left">
                                        <td class="formCellLabel">Start Date</td>
                                        <td class="formCellField" style="width:150px;" >
                                            <telerik:RadDatePicker id="dteStartDate" runat="server">
                                            <DateInput DateFormat="dd/MM/yy">
                                            </DateInput>
                                            </telerik:RadDatePicker>
                                        </td>
                                        <td class="formCellLabel" style="width:50px;">End Date</td>
                                        <td class="formCellField" width="100%" >
                                            <telerik:RadDatePicker id="dteEndDate" runat="server" dateformat="dd/MM/yy">
                                            <DateInput DateFormat="dd/MM/yy">
                                            </DateInput>
                                            </telerik:RadDatePicker>
                                        </td>
                                    </tr>
                                    
                                    <tr>
                                        <td class="formCellField">&nbsp;</td>
                                        <td class="formCellField" colspan="4"><div style="width:400px;">By default the dates you see will cause loads to be shown from the start date until the end of the day before the end date, and deliveries between both dates.</div></td>
                                    </tr>
                                    <tr>
                                        <td class="formCellLabel">Resource Depot</td>
                                        <td class="formCellField" colspan="4">
                                            <asp:DropDownList ID="cboDepot" runat="server" DataValueField="OrganisationLocationId" DataTextField="OrganisationLocationName" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="formCellField">&nbsp;</td>
                                        <td class="formCellField" colspan="4"><div style="width:400px;">This will cause resources affiliated with the selected depot to drive the resource pool used when planning.</div></td>
                                    </tr>
                                    <tr>
                                        <td class="formCellLabel">Available Filters</td>
                                        <td class="formCellField" colspan="4">
                                            <asp:DropDownList id="cboFilters" runat="server" AutoPostBack="true" DataValueField="FilterId" DataTextField="FilterName" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="formCellLabel">Filter Name</td>
                                        <td class="formCellField" colspan="4">
                                            <asp:TextBox id="txtFilterName" runat="server" />
                                            <asp:RequiredFieldValidator id="rfvFilterName" runat="server" ControlToValidate="txtFilterName" Display="Dynamic" ErrorMessage="You must provide a filter name when saving a filter."><img src="../../images/error.gif"  height="16" width="16" title="You must provide a filter name when saving a filter." /></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr valign="top" style="display:none;">
                                        <td class="formCellLabel">Job States</td>
                                        <td class="formCellField" colspan="4">
                                            <asp:CheckBox ID="chkShowPlanned" Text="Show Planned Jobs" runat="server" />
                                            &nbsp;
                                            <asp:CheckBox ID="chkShowStockMovementJobs" Text="Show Stock Movement Jobs" runat="server" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="formCellLabel">Collapse Runs</td>
                                        <td class="formCellField" colspan="4"><asp:CheckBox ID="chkCollapseRuns" runat="server" /></td>
                                    </tr>
                                    <tr>
                                        <td class="formCellLabel">Business Types</td>
                                        <td class="formCellField" colspan="4">
                                        <input type="checkbox" id="chkSelectAllBuisinessType" onclick="selectAllBusinessTypes(this);" checked='true' /><label for="chkSelectAllBuisinessType">Select All</label>
                                        <asp:CheckBoxList runat="server" id="cblBusinessType" RepeatDirection="Horizontal" RepeatColumns="6"></asp:CheckBoxList> </td>
                                    </tr>
                                    <tr>
                                        <td class="formCellLabel">Leg States</td>
                                        <td class="formCellField" colspan="4">
                                            <asp:CheckBoxList runat="server" ID="cblInstructionStates" RepeatDirection="Horizontal"></asp:CheckBoxList>

                                            <table class="inProgressSubStatesRow">
                                                <tr>
                                                    <td>
                                                        In Progress Status: 
                                                    </td>
                                                    <td>
                                                        <asp:CheckBoxList runat="server" ID="cblInProgressSubStates" RepeatDirection="Horizontal" DataValueField="key" DataTextField="value"></asp:CheckBoxList>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="formCellLabel">Planning Category </td>
                                        <td class="formCellField" colspan="4"><asp:CheckBoxList runat="server" ID="cblPlanningCategory" RepeatDirection="Horizontal" RepeatColumns="5" DataTextField="DisplayShort" DataValueField="ID"></asp:CheckBoxList></td>
                                    </tr>
                                    <tr>
                                        <td class="formCellLabel">Instruction Types</td>
                                        <td class="formCellField" colspan="4">
                                            <asp:CheckBox runat="server" ID="chkSelectAllInstructionTypes" Text="Select All" onclick="selectAllInstructionTypes(this);"/>
                                            <asp:CheckBoxList runat="server" ID="cblInstructionType" RepeatDirection="Horizontal" RepeatColumns="5" DataTextField="DisplayShort" DataValueField="ID"/>
                                        </td>
                                    </tr>
                                     <tr>
                                        <td class="formCellLabel">MWF Comms Status</td>
                                        <td class="formCellField" colspan="4">
                                            <asp:CheckBox runat="server" ID="chkSelectAllMWFCommsStates" Text="Select All" onclick="selectAllMWFCommsStatus(this);"/>
                                            <asp:CheckBoxList runat="server" ID="cblMWFCommsStates" RepeatDirection="Horizontal" RepeatColumns="5" DataTextField="Value" DataValueField="Key" OnDataBound="cblMWFCommsStatus_DataBound"></asp:CheckBoxList>
                                        </td>
                                    </tr>
                                    <tr style="display: none;">
                                        <td class="formCellLabel">MWF Instruction Status</td>
                                        <td class="formCellField" colspan="4">
                                            <asp:CheckBox runat="server" ID="chkSelectAllMWFInstructionStates" Text="Select All" onclick="selectAllMWFInstructionStates(this);"/>
                                            <asp:CheckBoxList runat="server" ID="cblMWFInstructionStates" RepeatDirection="Horizontal" RepeatColumns="5" DataTextField="Value" DataValueField="Key" OnDataBound="cblMWFInstructionStates_DataBound"></asp:CheckBoxList>
                                        </td>
                                    </tr>
                                    <tr style="display:none;">
                                        <td class="formCellLabel">Default Sorting</td>
                                        <td class="formCellField" colspan="4"><asp:CheckBox ID="chkSortbyEvent" runat="server" Text="Sort by event" />(Sort by deliveries then collections)</td>
                                    </tr>
                                </table>
                            </fieldset>
                            <cs:WebModalWindowHelper ID="mwhelper" runat="server" ShowVersion="false" />
                            <div class="buttonbar">
	                            <nfvc:NoFormValButton id="btnFilter" runat="server" NoFormValList="rfvFilterName" Text="Apply Filter"></nfvc:NoFormValButton>
	                            <nfvc:NoFormValButton id="btnSaveFilter" runat="server" NoFormValList="rfvStartDate,rfvEndDate" Text="Save Filter"></nfvc:NoFormValButton>
                                <asp:Button ID="btnClose" runat="server" Text="Close" CausesValidation="False" />
                            </div>                            
                         </div>
                    </td>
                </tr>
            </table>
            <script type="text/javascript">

                $(document).ready(function () {
                    $(":checkbox[id*='chkSelectAllTrafficAreas']").click(function() {
                        var checkedStatus = this.checked;
                        $(":checkbox[id*='cboTrafficAreas']").each(function() {
                            this.checked = checkedStatus;
                        });
                    });

                    $('.inProgressInstructionState > :checkbox').click(displayInProgressSubStates);
                    displayInProgressSubStates();
                });

                function displayInProgressSubStates () {
                    var inProgressInstructionStateCheckBox = $('.inProgressInstructionState > :checkbox');
                    var displayInProgressSubStates = inProgressInstructionStateCheckBox.prop('checked');
                    var inProgressSubStatesRow = $('.inProgressSubStatesRow');

                    if (displayInProgressSubStates) {
                        inProgressSubStatesRow.slideDown();
                    } else {
                        inProgressSubStatesRow.slideUp();
                    }
                }

                function selectAllBusinessTypes(sender) {
                    $('input:checkbox[id*=cblBusinessType]').prop('checked', sender.checked);
                }

                // ****************** MWF/Non-MWF Instruction Type **************************

                function selectAllInstructionTypes(sender) {
                    $(":checkbox[id*='cblInstructionType']").prop('checked', sender.checked);
                }

                function onInstructionTypeChecked() {
                    allInCheckboxListChecked($(":checkbox[id*='cblInstructionType']"), $(":checkbox[id*='chkSelectAllInstructionTypes']"));
                }

                // ****************** MWF Comms Status **************************
                function selectAllMWFCommsStatus(sender) {
                    $(":checkbox[id*='cblMWFCommsStates']").prop('checked', sender.checked);
                }
                
                function selectAllMWFInstructionStates(sender) {
                    $(":checkbox[id*='cblMWFInstructionStates']").prop('checked', sender.checked);
                }
                
                function onInProgressStateChecked() {
                    allInCheckboxListChecked($(":checkbox[id*='cblMWFCommsStates']"), $(":checkbox[id*='chkSelectAllMWFCommsStates']"));
                    var cbl = $(":checkbox[id*='cblMWFCommsStates']");
                    cbl.each(function () {
                        var test = this;
                    });
                }

                // ******************  **************************
                function onMWFCommsStatusChecked() {
                    allInCheckboxListChecked($(":checkbox[id*='cblMWFCommsStates']"), $(":checkbox[id*='chkSelectAllMWFCommsStates']"));
                }
                
                function onMWFInstructionStatusChecked() {
                    allInCheckboxListChecked($(":checkbox[id*='cblMWFInstructionStates']"), $(":checkbox[id*='chkSelectAllMWFInstructionStates']"));
                }

                // ****************** Update Select All CheckBox **************************
                function allInCheckboxListChecked(chkboxlist, chkbox) {
                    var allChecked = true;
                    chkboxlist.each(function () {
                        if (!this.checked)
                            allChecked = false;
                    });
                    chkbox[0].checked = allChecked;
                }
                
            </script>
        </form>
    </body>
</html>