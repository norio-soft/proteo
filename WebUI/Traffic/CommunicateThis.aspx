<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Traffic.CommunicateThis" Codebehind="CommunicateThis.aspx.cs" Title="Communicate This" MasterPageFile="~/WizardMasterPage.Master" %>
<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Communicate This</asp:Content>

<asp:Content ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
    <script language="javascript" src="/script/scripts.js" type="text/javascript"></script>

    <style>
        .nonMWFField
        {
            display: none;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <table id="Table2" width="100%" cellpadding="0" cellspacing="0" border="0" height="300">
        <tr>
            <td colspan="2" width="100%">
                <table cellspacing="2" width="100%" >
                    <tr height="250">
                        <td class="greyText" width="100%" valign="top" >
                            <div style="height: 520px; width: 100%; overflow: auto">
                                <uc1:infringementDisplay runat="server" ID="idErrors" Visible="false" />
                                 <asp:Panel ID="pnlConfirmation" runat="server" Visible="false">
                                    <div class="MessagePanel">
                                        <asp:Image ID="imgIcon" runat="server" ImageUrl="~/images/ico_info.gif" /><asp:Label ID="lblConfirmation" runat="server" Visible="false"></asp:Label>
                                    </div>
                                </asp:Panel>
                                <asp:Panel ID="pnlPCVS" runat="server" Visible="false">
                                    
					                <div style="font-family:tahoma; color:Red; width:98%; border:solid 1pt red;padding:3px"><asp:Label ID="lblPCVs" runat="server">Please Inform Driver There are PCV's To Take</asp:Label></div>
                                </asp:Panel>
                                <asp:panel ID="pnlForm" runat="server" Visible="true">
                                <fieldset>
                                    <table>
								        <tr valign="top">
								            <td class="formCellLabel">Number Used</td>
									        <td class="formCellInput">
										        <asp:placeholder id="plcNumbers" runat="server"></asp:placeholder>
									        </td>
								        </tr>
								        <tr>
								            <td class="formCellInput"></td>
										    <td class="formCellInput">
											    <asp:TextBox id="txtNumber" runat="server" enabled="false" OnChange="txtNumber_TextChanged"></asp:TextBox>
											    <asp:RequiredFieldValidator id="rfvNumber" runat="server" ControlToValidate="txtNumber" Display="Dynamic" ErrorMessage="Please supply a number to communicate over."><img src="../images/Error.gif" width="16" height="16" alt="Please supply a number to communicate over." ></asp:RequiredFieldValidator>
										    </td>
									    </tr>
								        <tr>
										    <td class="formCellLabel" nowrap="true">Communicate via</td>
										    <td class="formCellInput">
											    <asp:DropDownList id="cboCommunicationType" runat="server" DataValueField="key" DataTextField="value" OnChange="cboCommunicationTypeIndex_Changed"></asp:DropDownList>
											    <asp:RequiredFieldValidator id="rfvCommunicationType" runat="server" ControlToValidate="cboCommunicationType" InitialValue="" Display="Dynamic" ErrorMessage="Please supply a communication type."><img src="../images/Error.gif" width="16" height="16" alt="Please supply a communication type." ></asp:RequiredFieldValidator>
										    </td>
									    </tr>
									    <tr class="nonMWFField">
										    <td class="formCellLabel" valign="top">Comments</td>
										    <td class="formCellInput">
											    <asp:TextBox id="txtComments" runat="server" TextMode="MultiLine" MaxLength="1000" Height="80px" Width="100%"></asp:TextBox>
											    <asp:RequiredFieldValidator id="rfvComments" runat="server" ControlToValidate="txtComments" Display="Dynamic" ErrorMessage="Please supply the communication comments."><img src="../images/Error.gif" width="16" height="16" alt="Please supply the communication comments." ></asp:RequiredFieldValidator>
										    </td>
									    </tr>
									    <tr class="nonMWFField">
										    <td class="formCellLabel" valign="top">SMS Text</td>
										    <td class="formCellInput">
											    <asp:TextBox id="txtSMSText" runat="server" TextMode="MultiLine" MaxLength="160" Height="60px" Width="100%"></asp:TextBox>
											    <asp:RequiredFieldValidator id="rfvSMSText" runat="server" ControlToValidate="txtSMSText" Display="Dynamic" ErrorMessage="Please supply the SMS Text."><img src="../images/Error.gif" width="16" height="16" alt="Please supply the SMS Text." ></asp:RequiredFieldValidator>
										    </td>
									    </tr>
									    <tr class="nonMWFField">
										    <td class="formCellLabel" nowrap="nowrap">Status</td>
										    <td class="formCellInput">
											    <asp:DropDownList id="cboCommunicationStatus" runat="server"></asp:DropDownList>
											    <asp:RequiredFieldValidator id="rfvCommunicationStatus" runat="server" ControlToValidate="cboCommunicationStatus" InitialValue="" Display="Dynamic" ErrorMessage="Please choose a status for this comunication."><img src="../images/Error.gif" width="16" height="16" alt="Please supply a communication type." ></asp:RequiredFieldValidator>
										    </td>
									    </tr>
									    <tr runat="server" id="trLoadOrder" valign="top" class="nonMWFField" visible="false">
									        <td class="formCellLabel" valign="top">Load Order</td>
									        <td class="formCellInput">
									            <fieldset style="color:Black; font-size:11px;">
										            <div style="height:100px; overflow:auto;">
										                If the driver is performing a load they should ensure that the dockets are loaded in the following order:<br>
										                <asp:Label id="lblLoadOrder" runat="server"></asp:Label>
										            </div>
									            </fieldset>
									        </td>
									    </tr>
									    <tr valign="top">
									        <td valign="top" colspan="3" width="100%">
									            <asp:CheckBox ID="chkGiveResources" runat="server" Text="Give resources to" Checked="true" />
									            <asp:DropDownList ID="cboControlArea" runat="server" DataValueField="ControlAreaId" DataTextField="Description" AutoPostBack="True"></asp:DropDownList>
									            <asp:DropDownList ID="cboTrafficArea" runat="server" DataValueField="TrafficAreaId" DataTextField="Description"></asp:DropDownList>
									            <input type="hidden" id="hidLastInstructionInvolvedWith" runat="server" />
    									        
									        </td>    
									    </tr>
							        </table>
							    </fieldset>
                        		</asp:panel>
						     </div>
                            
                        </td>
                    </tr>
                 </table>
                </td>
               </tr>
        <tr>
            <td colspan="2">
                <div class="buttonbar">
                    <input type="button" value="Show Load Order" onclick="javascript:openResizableDialogWithScrollbars('LoadOrder.aspx?jobid=<%=int.Parse(Request.QueryString["jobId"]) %>', '700', '258')" />
                    <asp:Button ID="btnCommunicate" runat="server" Text="Communicate" Width="105"></asp:Button>
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" Width="75" CausesValidation="False" />
                </div>
            </td>
        </tr>
    </table>
    
    <asp:HiddenField ID="hdnValidateNumber" runat="server" EnableViewState="false" />
    <asp:HiddenField ID="hdnValidateSMSText" runat="server" EnableViewState="false" />
        
    <script type="text/javascript">
        $(function() {
            cboCommunicationTypeIndex_Changed();
		});

        function enableOtherTextBox()
	    {
	        var txtNumber = $get("<%=txtNumber.ClientID%>");
	        txtNumber.disabled = false;
	        txtNumber.focus();		    
	    }
    	
	    function setNumberUsed(e)
	    {			
	        var txtNumber = $get("<%=txtNumber.ClientID%>");
	        txtNumber.disabled = true;
	        txtNumber.value = e;

	        txtNumber_TextChanged();		    
	    }

	    function cboCommunicationTypeIndex_Changed() {
            //ValidatorEnable is part of ClientAPI for Validation.
	        var cboCommunicationType = $get("<%=cboCommunicationType.ClientID%>");

	        if (cboCommunicationType == null) {
	            return;
	        }

	        var selectedType = cboCommunicationType.options[cboCommunicationType.selectedIndex];
    		
		    var rfvNumber = $get("<%=rfvNumber.ClientID%>");
		    var rfvSMSText = $get("<%=rfvSMSText.ClientID%>");
	        var txtSMSText = $get("<%=txtSMSText.ClientID%>");

		    var hdnValidateNumber = $get("<%=hdnValidateNumber.ClientID%>");
		    var hdnValidateSMSText = $get("<%=hdnValidateSMSText.ClientID%>"); 
    		
		    switch (selectedType.value)
		    {
		        case "<%= (int)Orchestrator.eDriverCommunicationType.Phone %>":
		            txtSMSText.disabled = true;
		            $('.nonMWFField').show();
		            ValidatorEnable(rfvNumber, true);
		            ValidatorEnable(rfvSMSText, false);
		            updateControlValue(hdnValidateNumber, true);
		            updateControlValue(hdnValidateSMSText, false);
		            break;
		        case "<%= (int)Orchestrator.eDriverCommunicationType.Text %>":
				    txtSMSText.disabled = false;
				    $('.nonMWFField').show();
				    ValidatorEnable(rfvNumber, true);
				    ValidatorEnable(rfvSMSText, true);
				    updateControlValue(hdnValidateNumber, true);
				    updateControlValue(hdnValidateSMSText, true);
				    break;
		        case "<%= (int)Orchestrator.eDriverCommunicationType.InPerson %>":
				    txtSMSText.disabled = true;
				    $('.nonMWFField').show();
				    ValidatorEnable(rfvNumber, false);
				    ValidatorEnable(rfvSMSText, false);
				    updateControlValue(hdnValidateNumber, false);
				    updateControlValue(hdnValidateSMSText, false);
				    break;
		        case "<%= (int)Orchestrator.eDriverCommunicationType.Manifest %>":
				    txtSMSText.disabled = true;
				    $('.nonMWFField').show();
				    ValidatorEnable(rfvNumber, false);
				    ValidatorEnable(rfvSMSText, false);
				    updateControlValue(hdnValidateNumber, false);
				    updateControlValue(hdnValidateSMSText, false);
				    break;
		        case "<%= (int)Orchestrator.eDriverCommunicationType.ToughTouch %>":
		            txtSMSText.disabled = true;
		            $('.nonMWFField').hide();
		            ValidatorEnable(rfvNumber, false);
				    ValidatorEnable(rfvSMSText, false);
				    updateControlValue(hdnValidateNumber, false);
				    updateControlValue(hdnValidateSMSText, false);
				    break;
		    }
		}

		function txtNumber_TextChanged() {
		    var cboCommunicationType = $get("<%=cboCommunicationType.ClientID%>");
		    var rfvNumber = $get("<%=rfvNumber.ClientID%>");
		    var selectedType = cboCommunicationType.options[cboCommunicationType.selectedIndex];

		    var hdnValidateNumber = $get("<%=hdnValidateNumber.ClientID%>");

		    switch (selectedType.value) {
		        case "<%= (int)Orchestrator.eDriverCommunicationType.Phone %>":
		            ValidatorEnable(rfvNumber, false);
		            ValidatorEnable(rfvNumber, true);
		            updateControlValue(hdnValidateNumber, true);
		            break;
		    }
		}

		function updateControlValue(control, value) {
		    control.Value = value;
		}
    </script>

</asp:Content>