<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/default_tableless.Master"  CodeBehind="SafetyCheckProfiles.aspx.cs" Inherits="Orchestrator.WebUI.Resource.SafetyChecks.Profiles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">
   <style>
        .error-message {
            font-weight:bold;
            color:#f00;
        }

        .faultListWrapper {
            margin-bottom:10px;
            padding:0
        }

       img.faultListMoveUp, img.faultListMoveDown, img.faultListDelete {
           width:16px;
           height:16px;
       }

       .listTable {
           margin: 0 auto;
           width:100%;
           border: 1px solid #363636;
           border-collapse: collapse;
       }
       .listTable thead tr {
           line-height: 28px;
           background-color: #000;
       }
       .listTable thead tr th {
           font-weight: normal;
           color: #fff;
           padding: 0 8px;
           border-right: 1px solid #363636;
           border-bottom: 1px solid #363636;
       }
       tr.odd {
           background-color: #f5f5f5;
       }
       td.centered {
           text-align:center;
       }

       .tableWrapper {
           width: 500px;
       }

       .profileAssignmentTable {
           width:100%;
       }

       /*inputs*/
       .faultTextBox {
           width:95%
       }
       .faultToggleCol, .faultActionCol {
           text-align:center;
       }
       .wideButtonDiv {
           padding:5px !important;
           border: none !important;
           background: none !important;
       }
       .wideButtonDiv > input {
           width:100%;
       }

       .buttonbar > table {
           width:100%
       }

       .floatR {
           float:right;
       }

       .profileAssignmentTable {
           width:700px;
       }

       .faultValidationWrapper ul { margin:0; }
       .faultValidationWrapper ul li { font-weight:bold; }
   </style>
   <script>

       function reallyDeleteProfile() { return confirm("Are you sure you want to delete this profile?\r\nThere is no way to undo this action."); }
       function reallyDeleteFault() { return confirm("Are you sure you want to delete this row?"); }

       $(function () {
           $(".deleteProfileAction").on("click", reallyDeleteProfile);
           $(".deleteFaultAction").on("click", reallyDeleteFault);
       });

   </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h2>Manage Safety Check Profiles</h2>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    

    <telerik:RadMultiPage runat="server" RenderSelectedPageOnly="true" SelectedIndex="0" id="rmpSafetyProfiles">
        
        <!-- == Safety Check Profile List == -->
        <telerik:RadPageView id="rpProfileList" runat="server">
            <div runat="server" Visible="false" id="deleteProfileError">
                <p class="error-message">Unable to delete profile. Please ensure it is not currently assigned to any active vehicles or trailers and try again. If the problem persists, please contact the support desk.</p>
            </div>

            <telerik:RadGrid runat="server" ID="rgProfiles" AutoGenerateColumns="false" AllowSorting="false">
                <MasterTableView DataKeyNames="ProfileId">
                    <Columns>
                        <telerik:GridBoundColumn DataField="ProfileTitle" HeaderText="Safety Check Profile Name" HeaderStyle-Width="85%" />
                        <telerik:GridButtonColumn ButtonType="LinkButton" CommandName="AssignProfile" HeaderText="Assign" Text="Assign" HeaderStyle-Width="5%" />
                        <telerik:GridButtonColumn ButtonType="LinkButton" CommandName="EditProfile" HeaderText="Edit" Text="Edit" HeaderStyle-Width="5%" />
                        <telerik:GridButtonColumn ButtonType="LinkButton" CommandName="DeleteProfile" HeaderText="Delete" Text="Delete" HeaderStyle-Width="5%" ButtonCssClass="deleteProfileAction" />
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
            <div class="buttonbar">
                <asp:Button ID="btnAddNew" runat="server" Text="Add New Profile..." CssClass="buttonclass" />
            </div>  
        </telerik:RadPageView>

        <!-- == Add/Edit Profile == -->
        <telerik:RadPageView id="rpAddEditProfile" runat="server">
            
            <div runat="server" Visible="false" id="saveProfileError">
                <p runat="server" class="error-message" id="saveProfileErrorMessage"></p>
            </div>
            
            <asp:ValidationSummary ID="ValidationSummary" runat="server" EnableClientScript="true"
                ShowSummary="true" HeaderText="Changes cannot be saved until the following are changed."
                CssClass="error" ValidationGroup="MinimumProfileDetails" />

            <table>
                <tr>
                    <td class="formCellLabel-Horizontal">Profile name</td>
                    <td class="formCellField-Horizontal">
                        <asp:TextBox id="txtProfileName" runat="server" Width="300px"></asp:TextBox>
                        <asp:RequiredFieldValidator id="profileNameRequired" runat="server" ControlToValidate="txtProfileName" ErrorMessage="Please enter a profile name" Display="None" ValidationGroup="MinimumProfileDetails"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel-Horizontal">Odometer reading required?</td>
                    <td class="formCellField-Horizontal">
                        <asp:CheckBox id="chkOdometer" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel-Horizontal">Signature required?</td>
                    <td class="formCellField-Horizontal">
                        <asp:CheckBox id="chkSignature" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel-Horizontal">Display at Logon?</td>
                    <td class="formCellField-Horizontal">
                        <asp:CheckBox id="chkAtLogon" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel-Horizontal">Display at Logoff?</td>
                    <td class="formCellField-Horizontal">
                        <asp:CheckBox id="chkAtLogoff" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel-Horizontal">Allow incomplete safety check (Not VOSA compliant)</td>
                    <td class="formCellField-Horizontal">
                        <asp:CheckBox id="chkVOSACompliant" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        
                        <div class="faultListWrapper">
                            
                            <p>
                                <asp:CustomValidator ID="atLeastOneFaultValidator" runat="server" OnServerValidate="atLeastOneFault_ServerValidate" ValidationGroup="MinimumProfileDetails" Display="None" ErrorMessage="A profile requires at least one fault." />
                            </p>

                            <table class="listTable">
                                <thead>
                                    <tr>
                                        <th width="4%">&nbsp;</th><%-- Move Up action--%>
                                        <th width="4%">&nbsp;</th><%-- Move Down action--%>
                                        <th width="78%">Item</th>
                                        <th style="display:none">Category</th><%-- Hidden on purpose; if you put it back, set width--%>
                                        <th width="10%">Discretionary</th>
                                        <th style="display:none">Highlight</th><%-- Hidden on purpose; if you put it back, set width--%>
                                        <th width="4%">&nbsp;</th><%-- Delete action--%>
                                    </tr>
                                </thead>

                                <asp:Repeater runat="server" id="repeaterFaultList">    
                                    <ItemTemplate>
                                        <tr>
                                            <td class="faultActionCol">
                                                <asp:ImageButton ID="imgBtnUp" runat="server" ImageUrl="/App_Themes/Orchestrator/img/masterpage/icon-arrow-up.png"
                                                    CausesValidation="false" CommandArgument='<%# ((Orchestrator.Entities.SafetyCheckProfileFault)Container.DataItem).FaultTypeId %>' 
                                                    CommandName="MoveUp" ToolTip="Move up" />
                                            </td>
                                            <td class="faultActionCol">
                                                <asp:ImageButton ID="imgBtnDown" runat="server" ImageUrl="/App_Themes/Orchestrator/img/masterpage/icon-arrow-down.png"
                                                    CausesValidation="false" CommandArgument='<%# ((Orchestrator.Entities.SafetyCheckProfileFault)Container.DataItem).FaultTypeId %>' 
                                                    CommandName="MoveDown" ToolTip="Move down" />
                                            </td>
                                            <td>
                                                <asp:TextBox runat="server" id="txtFaultTitle" CssClass="faultTextBox" Text="<%# ((Orchestrator.Entities.SafetyCheckProfileFault)Container.DataItem).FaultTitle %>" ToolTip="Title"></asp:TextBox>
                                                <asp:RequiredFieldValidator id="faultTitleVal" runat="server" ControlToValidate="txtFaultTitle" ErrorMessage="Please enter a fault name" Display="None" ValidationGroup="MinimumProfileDetails"></asp:RequiredFieldValidator>
                                            </td>
                                            <td style="display:none;"><%-- This is set to display none because spec doesnt accoutn for it but I have a feeling it will be needed --%>
                                                <asp:TextBox runat="server" id="txtFaultCategory" CssClass="faultTextBox" Text="<%# ((Orchestrator.Entities.SafetyCheckProfileFault)Container.DataItem).FaultCategory %>" ToolTip="Category"></asp:TextBox>
                                                <%-- <asp:RequiredFieldValidator id="faultCatVal" runat="server" ControlToValidate="txtFaultCategory" ErrorMessage="Please enter a fault category" Display="None" ValidationGroup="MinimumProfileDetails"></asp:RequiredFieldValidator> --%>
                                            </td>
                                            <td class="faultToggleCol">
                                                <asp:CheckBox runat="server" id="chkIsDiscretionary" CausesValidation="False" ToolTip="Should the fault allow discretionary passes?" Checked="<%# ((Orchestrator.Entities.SafetyCheckProfileFault)Container.DataItem).IsDiscretionaryQuestion %>"/>
                                            </td>
                                            <td style="display:none;" class="faultToggleCol">
                                                <asp:CheckBox runat="server" id="chkHighlight" CausesValidation="False" ToolTip="Highlight?" Checked="<%# ((Orchestrator.Entities.SafetyCheckProfileFault)Container.DataItem).Highlight %>"/>
                                            </td>
                                            <td class="faultActionCol">
                                                <asp:ImageButton ID="imgBtnDelete" runat="server" ImageUrl="/App_Themes/Orchestrator/img/masterpage/icon-cross-cirlce.png"
                                                    CausesValidation="false" CommandArgument='<%# ((Orchestrator.Entities.SafetyCheckProfileFault)Container.DataItem).FaultTypeId %>' 
                                                    CommandName="DeleteFault" ToolTip="Delete" CssClass="deleteFaultAction"/>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    
                                </asp:Repeater>
                                
                                <tfoot>
                                    <tr>
                                        <td colspan="7">
                                            <div class="buttonbar wideButtonDiv">
                                                <asp:Button id="btnAddNewFault" runat="server" Text="Add New Fault Item..." CssClass="buttonclass" CausesValidation="false" />
                                            </div>  
                                        </td>
                                    </tr>
                                </tfoot>

                            </table>

                        </div>

                    </td>
                </tr>
                <tr>
                    <td colspan="2"><hr/></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div class="buttonbar">
                            <table width="100%">
                                <tr>
                                    <td>
                                        <asp:Button id="btnCancel" runat="server" Text="Cancel" CssClass="buttonclass cancelChanges" CausesValidation="false" />
                                    </td>
                                    <td>
                                        <asp:Button ID="btnSaveChanges" runat="server" Text="Save" CssClass="buttonclass floatR" CausesValidation="false" ValidationGroup="MinimumProfileDetails" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                </tr>
            </table>

        </telerik:RadPageView>
        
        <!-- == Assign Profile == -->
        <telerik:RadPageView id="rpAssignProfiles" runat="server">
            <div runat="server" Visible="false" id="assignProfileError">
                <p class="error-message" id="assignProfileErrorMessage">There was an error assigning this profile to the selected vehicles, please try again. If the problem persists, please contact the support desk.</p>
            </div>
            
            <p>
                Please select the vehicle(s) you wish to assign/unassign from the profile: <strong><span runat="server" id="lblAssigningProfileTitle">PROFILE_NAME</span></strong>.<br/>
                Remember to save your assignment(s) when finished.
            </p>

            <table class="tableWrapper">
                <tr>
                    <td>
                        <table class="profileAssignmentTable">
                            <thead>
                                <th>Assigned Vehicle(s)</th>
                                <th>Unassigned Vehicle(s)</th>
                            </thead>    
                            <tr>
                                <td>
                                    <telerik:RadListBox runat="server" id="assignedVehiclesListBox" Width="100%" Height="500px" SelectionMode="Multiple" AllowTransfer="true" TransferToID="unassignedVehiclesListBox" EnableDragAndDrop="true">
                                    </telerik:RadListBox>
                                </td>
                                <td>
                                    <telerik:RadListBox runat="server" id="unassignedVehiclesListBox" Width="100%" Height="500px" SelectionMode="Multiple" >
                                    </telerik:RadListBox>
                                </td>
                            </tr>

                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <div class="buttonbar">
                            <table>
                                <tr>
                                    <td>
                                        <asp:Button id="btnCancelAssignments" runat="server" Text="Cancel Changes" CssClass="buttonclass cancelChanges" CausesValidation="false" />
                                    </td>
                                    <td>
                                        <asp:Button ID="btnSaveAssignments" runat="server" Text="Save Assignments" CssClass="buttonclass floatR" CausesValidation="false"  />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                </tr>
            </table>
        </telerik:RadPageView>

    </telerik:RadMultiPage>
    
    

</asp:Content>