<%@ Control Language="C#" AutoEventWireup="True" Inherits="Orchestrator.WebUI.Controls.Point" CodeBehind="point.ascx.cs" %>
<%@ Register Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI" TagPrefix="cc1" %>


<script language="javascript" src="/script/scripts.js" type="text/javascript"></script>

<asp:ScriptManagerProxy runat="server" ID="smProxyPoint">
    <Services>
        <asp:ServiceReference Path="/ws/combostreamers.asmx" />
    </Services>
</asp:ScriptManagerProxy>

<asp:UpdatePanel ID="upPoint" runat="server">
    <ContentTemplate>
        <cc1:Dialog ID="dlgAddUpdatePointGeofence" runat="server" Width="1034" Height="600" Mode="Normal" URL="/Point/Geofences/GeofenceManagement.aspx">
        </cc1:Dialog>

        <asp:Panel ID="pnlPoint" runat="server">
            <table width="100%">
                <tr>
                    <td style="white-space: nowrap;">
                        <asp:Label ID="lblOwner" runat="server"></asp:Label>
                    </td>
                </tr>
                <asp:Panel ID="pnlShowHelp" runat="server" Visible="false">
                    <tr>
                        <td colspan="2" style="font-family: Verdana; size: 10pt;">
                            <ul>
                                <li>To search for a point By <strong>Organisation Name</strong> just type into the box.</li>
                                <li>To Search for a point By <strong>Point Description</strong> please put a <strong>\</strong> at the start of your search.</li>
                                <li>Or you can combine the 2 (e.g.) SCA \Aylesford.</li>
                            </ul>
                        </td>
                    </tr>
                </asp:Panel>
                <tr>
                    <td>
                        <table cellspacing="0" cellpadding="0">
                            <tr>
                                <td>
                                    <div style="float: left;">Point Description and Address</div>
                                    <div style="float: right;">
                                        <asp:RequiredFieldValidator ID="rfvPoint" ValidationGroup="submit" runat="server" Display="Static" ControlToValidate="cboPoint" ErrorMessage="Please enter the point description.">
                                            <img id="Img3" src="/App_Themes/Orchestrator/img/MasterPage/icon-warning.png" runat="server" title="Please enter the point description." />
                                        </asp:RequiredFieldValidator>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <telerik:RadComboBox ID="cboPoint" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500" MarkFirstMatch="false" OnClientKeyPressing=""
                                        AutoPostBack="true" ShowMoreResultsBox="false" Width="340px" Height="300px" Overlay="true" AllowCustomText="True" HighlightTemplatedItems="true" CausesValidation="false">
                                        <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetPoints" />
                                    </telerik:RadComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td style="width: 340px;" nowrap="nowrap">
                        <div style="float: left">
                            <asp:LinkButton ID="lnkAlterPoint" runat="server" Text="<u>Alter Point</u> |" Style="font-size: 10px; text-decoration: none;" CausesValidation="false"></asp:LinkButton>
                            <asp:LinkButton ID="lnkPointGeography" runat="server" Visible="false" Text="<u>Alter Position</u> |" Style="font-size: 10px; text-decoration: none;" CausesValidation="false"></asp:LinkButton>
                            <asp:LinkButton ID="lnkNewPoint" runat="server" Text="<u>New Point</u> |" CausesValidation="false" Style="font-size: 10px; text-decoration: none;"></asp:LinkButton>
                            <asp:LinkButton ID="lnkClearSelectedPoint" runat="server" Text="Clear" Style="font-size: 10px;" CausesValidation="false" Visible="false"></asp:LinkButton>
                        </div>
                        <div id="divDepot" runat="server" style="vertical-align: top; text-align: right; float: right" visible="false">
                            <asp:Label runat="server" ID="lblDepot" />
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Panel ID="pnlCreateNewPoint" runat="server" Visible="False">
                            <fieldset>
                                <legend>Create a new Point</legend>
                                <div style="padding-top: 10px">
                                    You cannot create a new point with this name, as this client already has a point
										with this name.
                                </div>
                            </fieldset>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </asp:Panel>

        <asp:Panel ID="pnlNewPoint" runat="server" Visible="false">

            <div id="divDuplicateAddress" class="pointDuplicateAddress" runat="server" visible="false">
                <table>
                    <tr>
                        <td>
                            <h1>The below address(s) have been flagged as possible duplicates. 
                            </h1>
                            <br />
                            Click "Continue" to add the new address anyway.
                                <br />
                            Click "Go Back" to make further changes.                    
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div style="height: 325px; background: white; width: 100%; overflow: auto; padding: 0;">
                                <asp:ListView runat="server" ID="lvDuplicateAddress" DataKeyNames="PointId">
                                    <LayoutTemplate>
                                        <h1>Duplicate Addresses</h1>
                                        <table cellspacing="0">
                                            <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
                                        </table>
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td valign="middle" class="formCellField" style="width: 130px;">
                                                <asp:LinkButton runat="server" Text="Use this address" ToolTip="Click to select point." CommandName="select" CommandArgument='<%# Eval("PointId")%>' />
                                            </td>
                                            <td class="formCellField" style="width: 130px;"><%# Eval("OrganisationName")%></td>
                                            <td class="formCellField" style="width: 250px;"><%# Eval("Description") %></td>
                                            <td class="formCellField" style="width: 250px;"><%# Eval("Address") %></td>
                                        </tr>
                                        <tr style="border-bottom-width: 2; border-bottom-color: Black; border-bottom-style: inset;">
                                            <td colspan="3" valign="bottom">
                                                <br />
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <AlternatingItemTemplate>
                                        <tr style="background-color: InfoBackground;">
                                            <td valign="middle" class="formCellField" style="width: 130px;">
                                                <asp:LinkButton ID="HyperLink1" runat="server" Text="Use this address" ToolTip="Click to select point." CommandName="select" CommandArgument='<%# Eval("PointId")%>' />
                                            </td>
                                            <td class="formCellField" style="width: 130px;"><%# Eval("OrganisationName")%></td>
                                            <td class="formCellField" style="width: 250px;"><%# Eval("Description") %></td>
                                            <td class="formCellField" style="width: 250px;"><%# Eval("Address") %></td>
                                        </tr>
                                        <tr style="background-color: InfoBackground; border-bottom-width: 2; border-bottom-color: Black; border-bottom-style: inset;">
                                            <td colspan="3" valign="bottom">
                                                <br />
                                            </td>
                                        </tr>
                                    </AlternatingItemTemplate>
                                </asp:ListView>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Button ID="btnGoBack" runat="server" Text="Go Back" Visible="true" CausesValidation="false" />
                            <asp:Button ID="btnContinue" runat="server" Text="Continue" Visible="true" ValidationGroup="valDuplicatePoint" />
                        </td>
                    </tr>
                </table>
            </div>

            <fieldset>
                <table>
                    <tr>
                        <td class="formCellLabel">Organisation
                        </td>
                        <td class="formCellField">
                            <telerik:RadComboBox ID="cboNewPointOwner" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500" AllowCustomText="true" AutoPostBack="false" ShowMoreResultsBox="false" Width="230px" Height="300px" Overlay="true">
                                <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetPointOwner" />
                            </telerik:RadComboBox>
                            <asp:RequiredFieldValidator ID="rfvNewPointOwner" runat="server" ControlToValidate="cboNewPointOwner" ErrorMessage="Please enter the point owner." Display="dynamic" ValidationGroup="PointValidation">
                                <img id="Img4" runat="server" src="/App_Themes/Orchestrator/img/MasterPage/icon-warning.png" title="Please enter the point owner.">
                            </asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel" style="vertical-align:middle">Created By</td>
                        <td class="formCellField">
                            <asp:TextBox ID="txtCreatedBy" runat="server" ReadOnly="true" />
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel" style="vertical-align:middle">Creation Date</td>
                        <td class="formCellField">
                            <asp:TextBox ID="txtCreationDate" runat="server" ReadOnly="true" />
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel" style="vertical-align:middle">Last Modified By</td>
                        <td class="formCellField">
                            <asp:TextBox ID="txtLastModifiedBy" runat="server" ReadOnly="true" />
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel" style="vertical-align:middle">Last Modification Date</td>
                        <td class="formCellField">
                            <asp:TextBox ID="txtLastModificationDate" runat="server" ReadOnly="true" />
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel">Country
                        </td>
                        <td class="formCellField">
                            <telerik:RadComboBox ID="cboCountry" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500" AutoPostBack="false" MarkFirstMatch="true" AllowCustomText="false" ShowMoreResultsBox="false" Width="230px" Height="300px" Overlay="true">
                                <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetCountries" />
                            </telerik:RadComboBox>
                            <asp:RequiredFieldValidator ID="rfvCboCountry" runat="server" ControlToValidate="cboCountry" ErrorMessage="<img src='/App_Themes/Orchestrator/img/MasterPage/icon-warning.png' Title='Please select a country for this point.'>" EnableClientScript="True"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel">Post Code
                        </td>
                        <td class="formCellField">
                            <asp:TextBox ID="txtPostCode" runat="server"></asp:TextBox>
                            <asp:LinkButton ID="lnkLookUp" runat="server" Text="Find" Style="font-size: 10px;" ValidationGroup="Lookup"></asp:LinkButton>
                            <input type="hidden" id="hdnSetPointRadius" runat="server" name="hdnSetPointRadius" />
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel">Closest Town
                        </td>
                        <td class="formCellField">
                            <telerik:RadComboBox ID="cboClosestTown" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500" AutoPostBack="false" MarkFirstMatch="false" AllowCustomText="true" ShowMoreResultsBox="false" Width="230px" Height="300px" Overlay="true">
                                <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetClosestTown" />
                            </telerik:RadComboBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" Display="dynamic"
                                ControlToValidate="cboClosestTown" ValidationGroup="PointValidation" ErrorMessage="Please select the town.">
                                <img id="Img5" src="/App_Themes/Orchestrator/img/MasterPage/icon-warning.png" runat="server" title="Please select the town.">
                            </asp:RequiredFieldValidator>
                            <asp:CustomValidator ID="cfvClosestTown" runat="server" Display="dynamic" EnableClientScript="false"
                                ControlToValidate="cboClosestTown" ValidationGroup="PointValidation" ErrorMessage="Please select the town from the list.">
                                <img id="Img6" src="/App_Themes/Orchestrator/img/MasterPage/icon-warning.png" runat="server" title="Please select the town.">
                            </asp:CustomValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel">Description
                        </td>
                        <td class="formCellField">
                            <asp:TextBox Enabled="true" ID="txtDescription" runat="server" Width="230"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtDescription"
                                ErrorMessage="Please enter the point description." Display="dynamic" ValidationGroup="PointValidation">
                                <img id="Img7" src="/App_Themes/Orchestrator/img/MasterPage/icon-warning.png" runat="server" title="Please enter the point description.">
                            </asp:RequiredFieldValidator><asp:CustomValidator
                                runat="server" ID="cfvDescription" ErrorMessage="Please enter a description"
                                ValidationGroup="PointValidation" Display="dynamic">
                                <img id="Img8" src="/App_Themes/Orchestrator/img/MasterPage/icon-warning.png" runat="server" title="Please enter a description for this point" />
                            </asp:CustomValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel">Point Code
                        </td>
                        <td class="formCellField">
                            <asp:TextBox ID="txtPointCode" runat="server" Width="160" MaxLength="20"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel" width="100" nowrap="nowrap">Address Line 1
                        </td>
                        <td class="formCellField">
                            <asp:TextBox ID="txtAddressLine1" runat="server" Width="200"></asp:TextBox><asp:RequiredFieldValidator
                                ID="rfvAddressLine1" runat="server" ControlToValidate="txtAddressLine1" ValidationGroup="PointValidation"
                                Display="Dynamic">
                                <img id="Img9" runat="server" src="/App_Themes/Orchestrator/img/MasterPage/icon-warning.png" title="You must enter the first line of the address" />
                            </asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel">Address Line 2
                        </td>
                        <td class="formCellField">
                            <asp:TextBox ID="txtAddressLine2" runat="server" Width="200"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel">Address Line 3
                        </td>
                        <td class="formCellField">
                            <asp:TextBox ID="txtAddressLine3" runat="server" Width="200"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel">Post Town
                        </td>
                        <td class="formCellField">
                            <asp:TextBox ID="txtPostTown" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel">County/Province
                        </td>
                        <td class="formCellField">
                            <asp:TextBox ID="txtCounty" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel" valign="top">Phone Number
                        </td>
                        <td class="formCellField">
                            <asp:TextBox ID="txtPhoneNumber" runat="server" MaxLength="50" Wrap="true"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel">Traffic Area
                        </td>
                        <td class="formCellField">
                            <telerik:RadComboBox ID="cboTrafficArea" runat="server"></telerik:RadComboBox>
                            <asp:CustomValidator ValidationGroup="PointValidation" ControlToValidate="cboTrafficArea" runat="server" ID="cvTrafficArea" OnServerValidate="cboTrafficAreaValidator_ServerValidate">
                                <img id="imgTrafficAreaCustomValidatorError" runat="server" src="/App_Themes/Orchestrator/img/MasterPage/icon-warning.png" title="You must either select a traffic area or enter a postcode." />
                            </asp:CustomValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel" valign="top">Point Notes
                        </td>
                        <td class="formCellField">
                            <asp:TextBox ID="txtPointNotes" runat="server" MaxLength="1000" Rows="10"
                                Wrap="true" TextMode="MultiLine" Width="230"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="lblError" runat="server" Visible="false" CssClass="Error"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <div class="buttonbar">
                                <asp:Button ID="btnCreatePoint" runat="server" Text="Create Point" ValidationGroup="PointValidation" />
                                <asp:Button ID="btnAlterPoint" runat="server" Text="Alter Point" ValidationGroup="PointValidation" />
                                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CausesValidation="false" />
                                <asp:CustomValidator runat="server" ID="cfvCreatePoint" Display="Dynamic" ValidationGroup="PointSavedValidation" OnServerValidate="cfvCreatePoint_ServerValidate">
                                    <img id="img1" runat="server" src="~/images/error.png" title="You must complete the new point process by clicking the cancel or create point button." />
                                </asp:CustomValidator>
                                <asp:CustomValidator runat="server" ID="cfvAlterPoint" Display="Dynamic" OnServerValidate="cfvAlterPoint_ServerValidate" ValidationGroup="PointSavedValidation">
                                    <img id="img2" runat="server" src="~/images/error.png" title="You must complete the point alteration by clicking the cancel or alter point button." />
                                </asp:CustomValidator>
                            </div>
                        </td>
                    </tr>
                </table>
            </fieldset>

            <asp:HiddenField ID="hidLat" runat="server" />
            <asp:HiddenField ID="hidLon" runat="server" />

            <asp:Panel runat="server" ID="pnlRunPostCodeScripts" Visible="false">
                <script type="text/javascript">
					
                </script>
            </asp:Panel>
        </asp:Panel>

        <asp:Panel ID="pnlFullAddress" runat="server" Visible="false" Style="height: 80px; overflow: auto; border-bottom: solid 1pt silver; padding: 2px; color: black; background-color: white; width: 340px; text-align: left; font-size: 12px;">
            <asp:Label ID="lblFullAddress" runat="server" Visible="True" Text="Full Address" />
            <br />
        </asp:Panel>

        <asp:Panel ID="pnlAddressList" runat="server" Visible="false">
            <asp:ListBox ID="lstAddress" runat="server" Rows="10" Width="100%" AutoPostBack="true"></asp:ListBox>
            <div style="height: 22px; padding: 2px; color: #ffffff; background-color: #99BEDE; text-align: right;">
                <asp:Button ID="btnCancelList" runat="server" Text="Cancel" CausesValidation="false" />
            </div>
        </asp:Panel>

        <input id="inpCreateNewPointSelected" runat="server" type="hidden" value="" />

    </ContentTemplate>

</asp:UpdatePanel>

<telerik:RadCodeBlock ID="RadCodeblock1" runat="server">

    <script type="text/javascript" language="javascript">
	<!--
    function <%=this.ClientID %>_checkAddedNewPoint() {
			var retVal = true;
			
			var createNewPointButtonSelected = $get("<%=inpCreateNewPointSelected.ClientID %>");
		    if(createNewPointButtonSelected != null && createNewPointButtonSelected.value == "True")
		        retVal = false;
		
		    return retVal;
		}
		//-->
    </script>

    <script language="javascript" type="text/javascript">
		
        var <%=this.ClientID %>_pointCombo;
		
        var <%=this.ClientID %>_values = "";
        var <%=this.ClientID %>_townId = 0;
		
        var <%=this.ClientID %>_identityId ;
		
        Sys.Application.add_load(<%=this.ClientID %>_controlLoad);

        function <%=this.ClientID %>_hideDuplicateAddressWindow()
        {
            var div = $get('<%=this.divDuplicateAddress.ClientID %>');
            $(div).css("display", "none");  
        }
		
        function <%=this.ClientID %>_controlLoad(sender)
        {
		 
            <%=this.ClientID %>_pointCombo = $find("<%=cboPoint.ClientID %>");
		    if (<%=this.ClientID %>_pointCombo != null)
		        <%=this.ClientID %>_identityId = <%=this.ClientID %>_pointCombo.get_value();
			   
            if ($find("<%=cboNewPointOwner.ClientID %>") != null)
		    {
		        var input = $find("<%=cboNewPointOwner.ClientID %>").get_inputDomElement();
			    if(input.disabled == false)
			        input.focus();
			}
			
            try{
                if (<%=this.ClientID %>_pointCombo != null)
			        <%=this.ClientID %>_pointCombo.add_itemsRequesting(<%=this.ClientID %>_itemsRequesting);
        }
        catch(err){}
				
    }
		
    function <%=this.ClientID %>_newPointOwner_SelectedIndexChanged(sender, eventargs)
        {
            <%=this.ClientID %>_AutoNamePointDescription();
		}
		
		function <%=this.ClientID %>_itemsRequesting(sender, eventArgs)
        {
            try
            {
                if (<%=ClientUserOrganisationIdentityID %> > 0)
			    {
			        var context = eventArgs.get_context();
			        context["IdentityID"] = <%=ClientUserOrganisationIdentityID %>;
            }
        }
        catch(err){}
    }
		
    function <%=this.ClientID %>_cboClosestTown_Requesting(sender, eventargs)
        {
            var context = eventargs.get_context();
            context["CountryId"] = $find("<%=this.cboCountry.ClientID %>").get_value();
        }
		
        function <%=this.cboCountry.ClientID %>_SeleectedIndexChange(sender, eventargs)
        {
            var combo = $find("<%=cboClosestTown.ClientID %>");
		    var lnkLookUp = $('a[id*=lnkLookUp]');
			
		    combo.set_text("");      
		    combo.clearItems();    

		    if (sender._selectedItem._properties._data.value > 1)
		        lnkLookUp.hide();
		    else
		        lnkLookUp.show();
		}
		
		function <%=this.ClientID %>_updater(identityId, pointId, organisationName)
        {
			
            var oManager = GetRadWindowManager();
			   
            var oWnd = oManager.GetWindowByName("singleton");
		 
            var url = '<%=this.ResolveUrl("~/point/addupdatepoint.aspx") %>' + "?identityId=" + identityId + "&pointId=" + pointId + "&organisationName=" + organisationName;
			var callBack = "<%=this.ClientID %>" + "_clientClose";
				
		    oWnd.SetUrl(url);
		    oWnd.SetTitle("Update Point");
		    oWnd.SetSize(900, 600);

		    oWnd.add_close(callBack);
		    oWnd.Show();
			
		    var loc = window.location.href;
					
		    if (loc.indexOf("updateOrder.aspx") > -1)
		    {
		        var parentPage = GetRadWindow();
				
		        if(parentPage != null)
		            parentPage.Maximize();
		    }
		}
		
		function GetRadWindow()
		{
		    var oWindow = null;
		    if (window.radWindow) oWindow = window.radWindow; //Will work in Moz in all cases, including clasic dialog
		    else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;//IE (and Moz az well) 
		    return oWindow;
		}
			
		// Fires when the alter point window is closed (wired up in the _updater method)
		function <%=this.ClientID %>_clientClose()
		{
		    //alert("_clientClose");
		
		    var combo = $find("<%=cboPoint.ClientID %>");
		    var url = "~/point/getLowerPointAddress.aspx?pointID=";
		    var ids = combo.get_value().split(",");
		    var orgIdentityId = ids[0];
		    var pointId = ids[1];

		    if(pointId == null)
		    {
		        url += "-1";
		        $get("<%=lblFullAddress.ClientID %>").innerHTML = "";
            }
            else
            {
                url = "~/point/getLowerPointAddress.aspx?pointID=" + pointId;
                url = url.replace("~", webserver);
		
                var xmlRequest = new XMLHttpRequest();
		
                xmlRequest.open("POST", url, false);
                xmlRequest.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                try
                {
                    xmlRequest.send(null);
                    $get("<%=lblFullAddress.ClientID %>").innerHTML = xmlRequest.responseText;
                }
                catch(e){
                    //alert("Catch exception - clientClose");
                }
            }
        }
		
        function <%=this.ClientID %>_setTownID(item)
        {
            //<%=this.ClientID %>_townId = <%=this.ClientID %>_townCombo.get_value();
		}
	   
	   
        function <%=this.ClientID %>_stopRequesting(sender, eventargs)
        {
            //alert("_stopRequesting");
            if(sender.get_text().length < 3)
                //return false if the number of the typed characters is less than 3
                eventargs.set_cancel(true);
            else
                eventargs.set_cancel(false);

        }

        function <%=this.ClientID %>_AutoNamePointDescription()
        {
            var cboNewPointOwner = $find('<%=this.cboNewPointOwner.ClientID %>');
		    var cboClosestTown = $find('<%=this.cboClosestTown.ClientID %>');
		    $get('<%=this.txtDescription.ClientID %>').value = cboNewPointOwner.get_text() + ' - ' + cboClosestTown.get_text();
		}
	 
		function <%=this.ClientID %>_cboClosestTown_SelectedIndexChanged(sender, eventargs)
        {   
            if (sender._selectedItem._properties._data.value > 0)
            {
                <%=this.ClientID %>_AutoNamePointDescription();
			    var el = $get("<%=txtPostTown.ClientID %>");
			    var townName = sender.get_text();
			    if (townName.indexOf("(") > 0)
			    {
			        townName = townName.substr(0, townName.indexOf("(") - 1);
			    }
			    el.value = townName;
			} 
        }

        function <%=this.ClientID %>_cboClosestTown_Blur(sender, eventargs)
        {   
            setTimeout("<%=this.ClientID %>_ClosestTownBlur()", 500);
		}

		function <%=this.ClientID %>_ClosestTownBlur() {
            var cboClosestTown = $find('ctl00_ContentPlaceHolder1_ucOrder_ucDeliveryPoint_cboClosestTown');
		
            try {
                if (cboClosestTown._value == null || cboClosestTown._value < 1)
                {
                    cboClosestTown.set_text('');
                    <%=this.ClientID %>_AutoNamePointDescription(); 
				} 
            } catch (err) {}
        }

        var <%=this.ClientID %>_focusTimeout = -1;
        var <%=this.ClientID %>_focusControl = null;
		
        function <%=this.ClientID %>_GiveFocus()
        {
            clearTimeout(<%=this.ClientID %>_focusTimeout);
		    <%=this.ClientID %>_focusControl.focus();
		    <%=this.ClientID %>_focusControl = null;
		}

		function <%=this.ClientID %>_SetFocus(controlID)
        {
            <%=this.ClientID %>_focusControl = null;
		    if (controlID != "")
		    {
		        var control = $get(controlID);
		        if (control != null)
		        {
		            <%=this.ClientID %>_focusControl = control;
				    <%=this.ClientID %>_focusTimeout = setTimeout("<%=this.ClientID %>_GiveFocus()", 500);
				}
            }
        }

        function <%=this.ClientID %>_UpdateGeography(pointId) {
            var qs = "pointId=" + pointId;
            <%=dlgAddUpdatePointGeofence.ClientID %>_Open(qs);
		}
	
		function <%=this.ClientID %>_cboPoint_ClientDropDownClosed(sender, eventArgs){
			
            try
            {
                var itemText = sender.get_selectedItem().get_text();
			
                if (itemText.indexOf('</td><td>')> 0)
                {
                    // remove any html characters from this. and Show the Point Name only
                    var pointName = itemText.split('</td><td>')[0];
			
                    pointName = pointName.replace(/&(lt|gt);/g, function (strMatch, p1){
                        return (p1 == "lt")? "<" : ">";
                    });
                    pointName = pointName.replace(/<\/?[^>]+(>|$)/g, "");
                    sender.set_text(pointName);
                }
			
            }
            catch(e){}
		   
        }

        // function to set the focus on callback needs to be re-evaluated
	
    </script>

</telerik:RadCodeBlock>
