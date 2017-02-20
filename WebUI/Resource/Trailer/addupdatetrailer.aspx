<%@ Page Language="c#" MasterPageFile="~/WizardMasterPage.master" Inherits="Orchestrator.WebUI.Resource.Trailer.addupdatetrailer" CodeBehind="addupdatetrailer.aspx.cs" Title="Haulier Enterprise" %>

<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>
<%@ Register TagPrefix="cs" Namespace="Codesummit" Assembly="WebModalControls" %>

<asp:Content ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Trailer Details</asp:Content>

<asp:Content ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <object classid="clsid:5220cb21-c88d-11cf-b347-00aa00a28331" height="0px">
        <param name="LPKPath" value="<%=Page.ResolveUrl("~/CAB/ctSchedule7.LPK")%>">
    </object>
    
    <div style="height: 410; width: 100%; overflow: auto;">
        <asp:ValidationSummary ID="vsAddUpdateTrailer" runat="server" ShowMessageBox="True" ShowSummary="False" ></asp:ValidationSummary>
        <br />
        <asp:Label ID="lblConfirmation" runat="server" Visible="false" CssClass="confirmation" Text="The new Trailer has been added successfully."></asp:Label>
        <uc1:infringementDisplay id="infringementDisplay" runat="server" visible="false"></uc1:infringementDisplay>
        
        <fieldset>
            <legend><strong>Trailer Details</strong></legend>
            <table>
                <tr>
                    <td class="formCellLabel" style="width: 120px;">Trailer Ref</td>
                    <td class="formCellField">
                        <asp:TextBox ID="txtTrailerRef" CssClass="fieldInputBox" runat="server" MaxLength="50" ToolTip="50 Characters Maximum"></asp:TextBox>
                       

                    </td>
                    <td class="formCellField">
                        <asp:RequiredFieldValidator ID="rfvTrailerRef" runat="server" Text="<img src='Images/Error.gif' height='16' width='16' title='Field is Required' alt='Field is Required'>"
                            Display="Dynamic" ControlToValidate="txtTrailerRef" ErrorMessage="Please enter a Trailer Ref."><img src="../../images/Error.gif" height='16' width='16' title='Please enter a Trailer Ref.' alt='Field is Required'></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">Trailer Manufacturer</td>
                    <td class="formCellField">
                        <asp:DropDownList ID="cboTrailerManufacturer" runat="server">
                        </asp:DropDownList>
                    </td>
                    <td class="formCellField">
                        <asp:RequiredFieldValidator ID="rfvTrailerManufacturer" runat="server" Text="<img src='Images/Error.gif' height='16' width='16' title='Field is Required' alt='Field is Required'>"
                            Display="Dynamic" InitialValue="" ControlToValidate="cboTrailerManufacturer"
                            ErrorMessage="Please enter/select  a Trailer Manufacturer."><img src="../../images/Error.gif" height='16' width='16' title='Please enter/select a Trailer Manufacturer.' alt='Field is Required'></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">Trailer Type</td>
                    <td class="formCellField">
                        <asp:DropDownList ID="cboTrailerType" runat="server">
                        </asp:DropDownList>
                    </td>
                    <td class="formCellField">
                        <asp:RequiredFieldValidator ID="rfvTrailerType" runat="server" Text="<img src='Images/Error.gif' height='16' width='16' title='Field is Required' alt='Field is Required'>"
                            Display="Dynamic" InitialValue="" ControlToValidate="cboTrailerType" ErrorMessage="Please enter/select a Trailer Type."><img src="../../images/Error.gif" height='16' width='16' title='Please enter/select a Trailer Type.' alt='Field is Required'></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">Trailer Description</td>
                    <td class="formCellField">
                        <asp:DropDownList ID="cboTrailerDescription" runat="server" />
                    </td>
                    <td class="formCellField">
                        <asp:RequiredFieldValidator ID="rfvTrailerDescription" runat="server" Text="<img src='Images/Error.gif' height='16' width='16' title='Field is Required' alt='Field is Required'>"
                            Display="Dynamic" InitialValue="" ControlToValidate="cboTrailerDescription" ErrorMessage="Please enter/select a Trailer Description"><img src="../../images/Error.gif" height='16' width='16' title='Please enter/select a Trailer Description.' alt='Field is Required'></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">GPS Unit ID</td>
                    <td class="formCellField">
                        <asp:TextBox ID="txtGPSUnitID" CssClass="fieldInputBox" runat="server" Width="250"></asp:TextBox>
                    </td>
                    <td class="formCellField">
                    </td>
                </tr>
                <tr>
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
            </table>
        </fieldset>
        <fieldset>
            <legend><strong>Last Known Location</strong></legend>
            <table border="0" cellpadding="0" cellspacing="0">
                <tr>
                    <td class="formCellLabel" style="width: 120px;">Organisation&nbsp;</td>
                    <td class="formCellField">
                        <telerik:radcombobox id="cboOrganisation" runat="server" enableloadondemand="true" itemrequesttimeout="500"
                            markfirstmatch="true" onclientselectedindexchanged="OrganisationSelectedIndexChanged" radcontrolsdir="~/script/RadControls/"
                            showmoreresultsbox="true" skin="WindowsXP" width="355px" overlay="true" zindex="50"
                            height="200px">
                        </telerik:radcombobox>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">Point&nbsp;</td>
                    <td class="formCellField">
                        <telerik:radcombobox id="cboPoint" runat="server" enableloadondemand="true" itemrequesttimeout="500"
                            markfirstmatch="true" radcontrolsdir="~/script/RadControls/" showmoreresultsbox="true"
                            skin="WindowsXP" width="355px" onclientitemsrequesting="PointRequesting" overlay="true"
                            zindex="50" height="150px">
                        </telerik:radcombobox>
                        <asp:RequiredFieldValidator ID="rfvPoint" runat="server" ControlToValidate="cboPoint"
                            Display="Dynamic" ErrorMessage="Please select a last known location."><img src="../../images/error.gif" alt="Please select a last known location." /></asp:RequiredFieldValidator>
                        <asp:CustomValidator ID="cfvPoint" runat="server" ControlToValidate="cboPoint" EnableClientScript="false"
                            Display="Dynamic" ErrorMessage="Please select a last known location."><img src="../../images/error.gif" alt="Please select a last known location." /></asp:CustomValidator>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">Vehicle&nbsp;</td>
                    <td class="formCellField">
                        <asp:Label ID="lblVehicle" runat="server" />
                    </td>
                </tr>
            </table>
        </fieldset>
        <fieldset>
            <legend><strong>Belongs to</strong></legend>
            <table>
                <tr>
                    <td>
                        This trailer is a&nbsp;&nbsp;<asp:DropDownList ID="cboControlArea" runat="server" />
                        &nbsp;&nbsp;<asp:DropDownList ID="cboTrafficArea" runat="server" />
                        &nbsp;&nbsp;resource.
                    </td>
                </tr>
            </table>
        </fieldset>
        <asp:Panel ID="pnlTrailerDeleted" runat="server" Visible="False">
            <fieldset>
                <legend><strong>Is Trailer Deleted</strong></legend>
                <asp:CheckBox ID="chkDelete" runat="server" Text="This trailer is deleted."></asp:CheckBox>
            </fieldset>
        </asp:Panel>
    </div>
                        
    <div class="buttonbar">
        <asp:Button ID="btnAdd" runat="server" Text="Add New" Width="75"></asp:Button>
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" Width="75" CausesValidation="false" />
    </div>
    
    <cs:WebModalWindowHelper ID="mwhelper" runat="server" ShowVersion="false">
        </cs:WebModalWindowHelper>
                       
    <div style="height: 10px;">
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

	    GetScheduleData();
    	
	    function GetScheduleData()
	    {
		    var scheduleControl = document.getElementById('ctSchedule');
		    var success = false;
    		
		    if (scheduleControl != null)
		    {
			    var urlControl = document.getElementById('hidResourceScheduleURL');
    			
			    if (urlControl != null)
			    {
				    var url = urlControl.value;
    				
				    if (url != "")
				    {
					    success = scheduleControl.ReadFile(url, 0);
				    }
			    }
		    }
    		
		    if (document.getElementById('divScheduleHolder') != null)
		    {
			    if (success)
			    {
				    document.getElementById('divScheduleHolder').style.display = "inline";
			    }
			    else
			    {
				    document.getElementById('divScheduleHolder').style.display = "none";
			    }
		    }
	    }
    </script>

    <script for="ctSchedule" event="FirstDraw()" language="javascript">
	    var scheduleControl = document.getElementById("ctSchedule");
    	
	    if (scheduleControl != null)
	    {
		    scheduleControl.TipsType = 2;
		    scheduleControl.TipsDelay = 0;
	    }
    </script>

</asp:Content>
