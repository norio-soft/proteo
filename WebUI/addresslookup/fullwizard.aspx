<%@ Page Language="c#" Title="Address Lookup" MasterPageFile="~/WizardMasterPage.Master" Inherits="Orchestrator.WebUI.addresslookup.fullWizard" CodeBehind="fullWizard.aspx.cs" %>
 
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
    <base target="_self" />

    <script type="text/javascript" language="javascript">
        var AddressLine1 = '<%=Request["AddressLine1"]%>';
        var AddressLine2 = '<%=Request["AddressLine2"]%>';
        var AddressLine3 = '<%=Request["AddressLine3"]%>';
        var AddressLine4 = '<%=Request["AddressLine4"]%>';
        var AddressLine5 = '<%=Request["AddressLine5"]%>';
        var PostTown = '<%=Request["PostTown"]%>';
        var County = '<%=Request["County"]%>';
        var PostCode = '<%=Request["PostCode"]%>';
        var Longitude = '<%=Request["longitude"]%>';
        var Latitude = '<%=Request["latitude"]%>';
        var TrafficArea = '<%=Request["TrafficArea"]%>';
        var setPointRadius = '<%=Request["setPointRadius"]%>';

        var AddressLine1VAL = "<%=AddressLine1VAL%>";
        var AddressLine2VAL = "<%=AddressLine2VAL%>";
        var AddressLine3VAL = "<%=AddressLine3VAL%>";
        var AddressLine4VAL = "<%=AddressLine4VAL%>";
        var AddressLine5VAL = "<%=AddressLine5VAL%>";
        var PostTownVAL = "<%=PostTownVAL%>";
        var CountyVAL = "<%=CountyVAL%>";
        var PostCodeVAL = "<%=PostCodeVAL%>";
        var LongitudeVAL = "<%=LongitudeVAL%>";
        var LatitudeVAL = "<%=LatitudeVAL%>";
        var TrafficAreaVAL = "<%=TrafficAreaVAL%>";
        var SetPointRadiusVAL = "<%=SetPointRadiusVAL%>";

        var lblAddressLine1 = 'ctl00_lblAddressLine1';
        var lblAddressLine2 = 'ctl00_lblAddressLine2';
        var lblAddressLine3 = 'ctl00_lblAddressLine3';
//        var lblAddressLine4 = 'ctl00_lblAddressLine4';
//        var lblAddressLine5 = 'ctl00_lblAddressLine5';
        var lblPostTown = 'ctl00_lblPostTown';
        var lblCounty = 'ctl00_lblCounty';
        var lblPostCode = 'ctl00_lblPostCode';

        function setAddress() {

            window.opener.AddressLine1.value = AddressLine1VAL;
            window.opener.AddressLine2.value = AddressLine2VAL;
            window.opener.AddressLine3.value = AddressLine3VAL;

            window.opener.PostTown.value = PostTownVAL;
            window.opener.County.value = CountyVAL;
            window.opener.PostCode.value = PostCodeVAL;
            window.opener.longitude.value = LongitudeVAL;
            window.opener.latitude.value = LatitudeVAL;
            window.opener.TrafficArea.value = TrafficAreaVAL;
            window.opener.setPointRadius.value = SetPointRadiusVAL;


            //if (window.opener.AddressLine1 != null) 
                //window.opener.AddressLine1.innerText = AddressLine1VAL;
            
            //if (window.opener.AddressLine2 != null) 
                //window.opener.AddressLine2.innerText = AddressLine2VAL;

            //if (window.opener.AddressLine3 != null) 
                //window.opener.AddressLine3.innerText = AddressLine3VAL;
            
            //if (window.opener.PostTown != null) 
                //window.opener.PostTown.innerText = PostTownVAL;

            //if (window.opener.County != null) 
                //window.opener.County.innerText = CountyVAL;

            //if (window.opener.PostCode != null) 
                //window.opener.PostCode.innerText = PostCodeVAL;

            window.close();
        }

        function HidePage() {
            if (typeof (Page_ClientValidate) == 'function') {
                if (Page_ClientValidate()) {
                    document.getElementById("divMain").style.display = "none";
                    document.getElementById("inProgress").style.display = "block";
                }
            }
            else {
                document.getElementById("divMain").style.display = "none";
                document.getElementById("inProgress").style.display = "block";
            }
        }
				
    </script>
</asp:Content>


<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table height="100%" cellspacing="0" cellpadding="0" width="100%">
        <tr height="99%">
            <td style="padding: 10px;" valign="top" width="99%">
                <div id="divMain">
                    <asp:Panel ID="pnlStart" runat="server">
                        <b><font color="red"><span id="lblError"></span></font></b>
                        <div id="pnl0">
                            <h1>
                                Search for an address or postcode?
                            </h1>
                            <h2>
                                What do you want to do?</h2>
                            <fieldset>
                                <asp:RadioButtonList ID="rbOption" runat="server" AutoPostBack="True">
                                    <asp:ListItem Value="1" Text="Find an address, I already know the postcode" Selected="True"></asp:ListItem>
                                    <asp:ListItem Value="2" Text="Find an address, I don't know the postcode"></asp:ListItem>
                                </asp:RadioButtonList>
                            </fieldset>
                        </div>
                    </asp:Panel>
                </div>
                <asp:Panel ID="pnlPostCodeOnly" runat="server" Visible="False">
                    <h1>
                        Please enter the postcode below and click on the next button</h1>
                    <h2>
                        What's the postcode?</h2>
                    <fieldset>
                        <table>
                            <tr>
                                <td class="formCellLabel">
                                    Post Code
                                </td>
                                <td class="formCellField">
                                    <asp:TextBox ID="txtPostCodeOnly" runat="server"></asp:TextBox>&nbsp;(e.g. WR2 or WR2
                                    6NJ)
                                </td>
                            </tr>
                        </table>
                    </fieldset>

                    <script language="javascript" defer>
                        <!--
                        $('input[id*=txtPostCodeOnly]')[0].focus();                        
                        //-->
                    </script>

                </asp:Panel>
                <asp:Panel ID="pnlAddressNoPostCode" runat="server" Visible="False">
                    <h1>
                        Please enter some information and click the next button</h1>
                    <h2>
                        You do not need to fill in all fields</h2>
                    <fieldset>
                        <table>
                            <tr>
                                <span id="spanCompanyName" runat="server">
                                    <td class="formCellLabel">
                                        What's the name of the company?
                                    </td>
                                    <td class="formCellField">
                                        <asp:TextBox ID="txtCompanyName" runat="server"></asp:TextBox>&nbsp;(e.g. Boots)
                                    </td>
                                </span>
                            </tr>
                            <tr>
                                <td class="formCellLabel">
                                    What's the name of the street?
                                </td>
                                <td class="formCellField">
                                    <asp:TextBox ID="txtStreetName" runat="server"></asp:TextBox>&nbsp;(e.g. High Street)
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">
                                    What's the name of the town or city?
                                </td>
                                <td class="formCellField">
                                    <asp:TextBox ID="txtTownName" runat="server"></asp:TextBox>&nbsp;(e.g. Manchester)
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                    <div class="infoPanel">
                        <b>Tip:</b>You don't need to supply every part of the address but the street and
                        town normally get the best results. If you're looking for an address in a London
                        Borough, just enter London in the town name.</div>
                </asp:Panel>
                <asp:Panel ID="pnlSearchResults" runat="server" Visible="False">
                    <h1>
                        Select an Item from the List below to get the address..</h1>
                    <asp:RequiredFieldValidator ID="rfvAddress" runat="server" ErrorMessage="Please select an address from the list displayed below. If no addresses are displayed, please press the back button to refine your address search, or press the Specify button to enter the address manually."
                        EnableClientScript="False" ControlToValidate="lstAddress" Display="Dynamic"><img src="../images/Error.gif" height='16' width='16' title='Please select an address from the list displayed below. If no addresses are displayed, please press the back button to refine your address search, or press the Specify button to enter the address manually.'></asp:RequiredFieldValidator>
                    <fieldset>
                        <asp:ListBox ID="lstAddress" runat="server" Rows="18" Width="100%"></asp:ListBox>
                    </fieldset>
                    <asp:Button CssClass="buttonClass" ID="btnSpecify" runat="server" CausesValidation="False"
                        Text="Specify"></asp:Button>
                </asp:Panel>
                <asp:Panel ID="pnlManual" runat="server" Visible="False">
                    <h1>
                        Specify the address below..</h1>
                    <fieldset>
                        <table>
                            <tr>
                                <td class="formCellLabel">
                                    Address Line 1
                                </td>
                                <td class="formCellField">
                                    <asp:TextBox ID="txtAddressLine1" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvAddressLine1" runat="server" ErrorMessage="Please supply the first line of the address."
                                        EnableClientScript="False" ControlToValidate="txtAddressLine1" Display="Dynamic">
											    <img src="../images/Error.gif" height='16' width='16' title='Please supply the first line of the address.'></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">
                                    Address Line 2
                                </td>
                                <td class="formCellField">
                                    <asp:TextBox ID="txtAddressLine2" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">
                                    Address Line 3
                                </td>
                                <td class="formCellField">
                                    <asp:TextBox ID="txtAddressLine3" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">
                                    Town
                                </td>
                                <td class="formCellField">
                                    <asp:TextBox ID="txtTown" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvTown" runat="server" ErrorMessage="Please supply the town."
                                        EnableClientScript="False" ControlToValidate="txtTown" Display="Dynamic">
											    <img src="../images/Error.gif" height='16' width='16' title='Please supply the town.'></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">
                                    County
                                </td>
                                <td class="formCellField">
                                    <asp:TextBox ID="txtCounty" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">
                                    Post Code
                                </td>
                                <td class="formCellField">
                                    <asp:TextBox ID="txtPostCode" runat="server"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">
                                    Traffic Area
                                </td>
                                <td class="formCellField">
                                    <asp:DropDownList ID="cboTrafficArea" runat="server">
                                    </asp:DropDownList>
                                    <asp:CustomValidator ID="cvTrafficAreaCustomValidator" ControlToValidate="cboTrafficArea"
                                        OnServerValidate="cboTrafficAreaValidator_ServerValidate" runat="server">
                                        <img id="imgTrafficAreaCustomValidatorError" runat="server" src="~/images/error.png"
                                            title="You must either select a traffic area or enter a postcode." />
                                    </asp:CustomValidator>
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                </asp:Panel>
                <asp:Panel ID="pnlAddress" runat="server" Visible="False">
                    <h1>
                        Confirmation of the address</h1>
                    <fieldset style="text-align: center;">
                        <asp:Label ID="lblAddress" runat="server"></asp:Label>
                    </fieldset>
                    <b>Grid Reference</b>
                    <asp:Label ID="lblGridReference" runat="server"></asp:Label>
                    <asp:Panel ID="pnlExistingAddresses" runat="server" Visible="false">
                        <br />
                        <br />
                        One or more existing points have been found that match this new points position,
                        please review the following and consider using one of these points instead.<br />
                        <asp:Repeater ID="repExistingAddresses" runat="server">
                            <HeaderTemplate>
                                <table border="0" cellpadding="1" cellspacing="2" width="100%">
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <li><span onmouseover="showHelpTipUrl(event, '../point/getPointAddresshtml.aspx?pointId=<%# Eval("PointId") %>');"
                                            onmouseout="hideHelpTip(this);">
                                            <%# Eval("Description") %></span></li>
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                <tr>
                                    <td align="right">
                                        <input type="button" value="Continue Anyway" onclick="javascript:document.all['<%=btnFinish.ClientID %>'].style.display='inline';" />
                                    </td>
                                </tr>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </asp:Panel>
                </asp:Panel>
                <div align="center" id="inProgress" style="display: none">
                    <table width="99%" cellspacing="2" cellpadding="0" border="0">
                        <tr valign="top">
                            <td align="left">
                                <h3>
                                    Please wait - retrieving addresses...</h3>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
        <tr height="1%">
            <td style="padding: 0 10px;">
                <div class="buttonbar">
                    <asp:Button ID="btnBack" runat="server" CausesValidation="False" Enabled="False"
                        Text="< Back" Width="75"></asp:Button>
                    <asp:Button ID="btnNext" runat="server" Text="Next >" Width="75"></asp:Button>
                    <input id="btnFinish" style="width: 75px" onclick="javascript:setAddress();" type="button" value="Finish" name="Button1" runat="server">
                    <input style="width: 75px" onclick="window.close();" type="button" value="Close">
                </div>
            </td>
        </tr>
    </table>

    <asp:Panel ID="pnlHideFinishButton" runat="server" Visible="false">

        <script language="javascript" type="text/javascript">
			    <!--
            if (document.all["<%=btnFinish.ClientID %>"] != null)
                document.all["<%=btnFinish.ClientID %>"].style.display = 'none';
			    //-->
        </script>

    </asp:Panel>
</asp:Content>