<%@ Page Language="c#" MasterPageFile="~/default_tableless.Master" Inherits="Orchestrator.WebUI.Point.addupdatepoint" CodeBehind="addupdatepoint.aspx.cs" %>

<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>
<%@ Register Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI" TagPrefix="cc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>Add New Point</h1>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="ContentPlaceHolder1" >
    <script language="javascript" type="text/javascript">

        function CountryOnClientSelectedIndexChanged(item)
        {
            var townCombo = $find("<%=cboTown.ClientID %>");
            townCombo.set_text("");
            townCombo.requestItems(item.get_value(),false);
        }

        function <%=this.cboTown.ClientID %>_pointRequesting(sender, eventArgs)
        {
            var countryCombo = $find("<%=cboCountry.ClientID %>");

            var context = eventArgs.get_context();
            context["FilterString"] = "countryid=" + countryCombo.get_value() + ";";
        }
        
        function cboClient_SelectedIndexChanged()
        {
            UpdateDescription();
        }
        
        function cboTown_SelectedIndexChanged()
        {
            UpdateDescription();
        }
        
        function UpdateDescription()
        {
            var clientCombo = $find("<%=cboClient.ClientID %>");
            var townCombo = $find("<%=cboTown.ClientID %>");
            var descriptionTextBox = $get("<%=txtDescription.ClientID %>");
            
            descriptionTextBox.value = clientCombo.get_text() + ' - ' + townCombo.get_text();
        }
            
        $(document).ready(function() {
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
            setPointRadius = $('input[id*=<%=hdnSetPointRadius.ClientID %>]')[0];
        });

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
            setPointRadius = $('input[id*=<%=hdnSetPointRadius.ClientID %>]')[0];

            var sURL = "../addresslookup/fullwizard.aspx?";
            
            // No need to pass all this info in the query string as fullwizard doesn't use it + when data contains apostrophe's it caused errors anyway.
            //sURL += "AddressLine1=" + AddressLine1.value;
            //sURL += "&AddressLine2=" + AddressLine2.value;
            //sURL += "&AddressLine3=" + AddressLine3.value;
            //sURL += "&PostTown=" + PostTown.value;
            //sURL += "&County=" + County.value;
            sURL += "PostCode=" + PostCode.value;
            //sURL += "&longitude=" + longitude.value;
            //sURL += "&latitude=" + latitude.value;
            //sURL += "&TrafficArea=" + TrafficArea.value;
            sURL += "&searchTown=" + searchTown.value;
            //sURL += "&setPointRadius=" + setPointRadius.value;

            window.open(sURL, "Checker", "toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=0,resizable=0,width=560,height=450");
        }
        
         function UpdatePosition(pointId) {
            var qs = "pointId=" + pointId;
            <%=dlgAddUpdatePointGeofence.ClientID %>_Open(qs);
        }

        function confirmPointRemoval() {
             var retVal = confirm("Are you sure you wish to remove this Point, as this action cannot be undone once confirmed?");
             return retVal;
         }
    </script>

     <cc1:Dialog ID="dlgAddUpdatePointGeofence" runat="server" Width="1034" Height="600" Mode="Normal" URL="/Point/Geofences/GeofenceManagement.aspx" AutoPostBack="true" ReturnValueExpected="true"></cc1:Dialog>
    <h2>Please enter the <%=Orchestrator.Globals.Configuration.FleetMetrikInstance ? "Location" : "Point" %> Details below</h2>
    
    <asp:Label ID="lblConfirmation" runat="server" Visible="false" CssClass="confirmation" Text="The new Point has been added successfully."></asp:Label>
        
    <fieldset>
        <legend>Organisation Details</legend>
        <telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
            MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" ShowMoreResultsBox="true"
            Skin="WindowsXP" Width="355px" OnClientSelectedIndexChanged="cboClient_SelectedIndexChanged" DataTextField="OrganisationName" DataValueField="IdentityId">
        </telerik:RadComboBox>
        <asp:RequiredFieldValidator ID="rfvClient" runat="server" ErrorMessage="Please select an organisation."
            ControlToValidate="cboClient" Display="Dynamic"></asp:RequiredFieldValidator>
    </fieldset>
        
    <fieldset>
        <legend runat="server"><%=Orchestrator.Globals.Configuration.FleetMetrikInstance ? "Location Details" : "Point Details" %></legend>
        <table>
            <tr>
                <td class="formCellLabel" style="vertical-align: middle">Created By</td>
                <td class="formCellField">
                    <asp:TextBox ID="txtCreatedBy" runat="server" ReadOnly="true" />
                </td>
            </tr>
            <tr>
                <td class="formCellLabel" style="vertical-align: middle">Creation Date</td>
                <td class="formCellField">
                    <asp:TextBox ID="txtCreationDate" runat="server" ReadOnly="true" />
                </td>
            </tr>
            <tr>
                <td class="formCellLabel" style="vertical-align: middle">Last Modified By</td>
                <td class="formCellField">
                    <asp:TextBox ID="txtLastModifiedBy" runat="server" ReadOnly="true" />
                </td>
            </tr>
            <tr>
                <td class="formCellLabel" style="vertical-align: middle">Last Modification Date</td>
                <td class="formCellField">
                    <asp:TextBox ID="txtLastModificationDate" runat="server" ReadOnly="true" />
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">Description</td>
                <td class="formCellField">
                    <asp:TextBox ID="txtDescription" runat="server" Width="250"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvDescription" runat="server" ControlToValidate="txtDescription"
                        ErrorMessage="<img src='../images/error.png' Title='Please enter the the point's description.'>"
                        EnableClientScript="True"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">Country</td>
                <td class="formCellField">
                    <telerik:RadComboBox ID="cboCountry" runat="server" EnableLoadOnDemand="false" ItemRequestTimeout="500"
                        AutoPostBack="true" MarkFirstMatch="true" RadControlsDir="~/script/RadControls/"
                        AllowCustomText="false" ShowMoreResultsBox="false" Width="355px"
                        Height="200px" Overlay="true" OnClientSelectedIndexChanged="CountryOnClientSelectedIndexChanged">
                    </telerik:RadComboBox>
                    <asp:RequiredFieldValidator ID="rfvCboCountry" runat="server" ControlToValidate="cboCountry"
                        ErrorMessage="<img src='../images/error.png' Title='Please select a country for this point.'>"
                        EnableClientScript="True"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">Closest Town</td>
                <td class="formCellField">
                    <telerik:RadComboBox ID="cboTown" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                        MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" ShowMoreResultsBox="true"
                        Width="355px" Height="200px" OnClientSelectedIndexChanged="cboTown_SelectedIndexChanged">
                    </telerik:RadComboBox>
                    <asp:RequiredFieldValidator ID="rfvTownId" runat="server" ControlToValidate="cboTown"
                        ErrorMessage="<img src='../images/error.png' Title='Please select the town closest to this point.'>"
                        EnableClientScript="True"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr runat="server" id="trPointCode">
                <td class="formCellLabel">Point Code</td>
                <td class="formCellField">
                    <asp:TextBox ID="txtPointCode" runat="server" MaxLength="20" Width="150"></asp:TextBox>
  
                 
                </td>
            </tr>
             <tr runat="server" id="trDelPerMatrix">
                <td class="formCellLabel">Delivery Period Matrix</td>
                <td class="formCellField">
                  <asp:DropDownList ID="cboDeliveryPeriod" runat="server" AppendDataBoundItems="true" > 
                      <asp:ListItem Text="--Select one--" Value="0"></asp:ListItem>
                  </asp:DropDownList>
                </td>
            </tr>
        </table>
    </fieldset>
    
    <fieldset>
        <legend>Address Details</legend>
        <p runat="server" id="pAddressText">Please note that all points need to have an address.</p>
        <uc1:infringementDisplay ID="infringementDisplay" runat="server" Visible="false"></uc1:infringementDisplay>
        <table>
            <tr>
                <td class="formCellLabel">Postal Code</td>
                <td class="formCellField">
                    <asp:TextBox ID="txtPostCode" runat="server"></asp:TextBox>
                    <asp:CustomValidator ID="Customvalidator1" runat="server"></asp:CustomValidator>
                    <asp:HyperLink ID="addressLink" runat="server" onclick="javascript:openChecker()" Text="Lookup Address" Target="_blank" />&nbsp;(enter postcode)<br />        
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">Address Line 1</td>
                <td class="formCellField">
                    <asp:TextBox ID="txtAddressLine1" runat="server" Width="200"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvAddressLine1" runat="server" ControlToValidate="txtAddressLine1"
                        ErrorMessage="<img src='../images/error.png' Title='Please enter the the first line of the Address.'>"
                        EnableClientScript="True"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">Address Line 2</td>
                <td class="formCellField">
                    <asp:TextBox ID="txtAddressLine2" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">Address Line 3</td>
                <td class="formCellField">
                    <asp:TextBox ID="txtAddressLine3" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">Post Town</td>
                <td class="formCellField">
                    <asp:TextBox ID="txtPostTown" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">County/Province</td>
                <td class="formCellField">
                    <asp:TextBox ID="txtCounty" runat="server"></asp:TextBox>
                </td>
            </tr>
           
            <tr>
                <td class="formCellLabel">Grid Reference</td>
                <td class="formCellField">
                    <input type="hidden" id="hdnSetPointRadius" runat="server" name="hdnSetPointRadius" />
                    <asp:TextBox ID="txtLongitude" runat="server" Width="120"></asp:TextBox>
                    <asp:TextBox ID="txtLatitude" runat="server" Width="120"></asp:TextBox>
                    <a href="javascript:UpdatePosition(<%=this.PointID %>);">Alter Position</a>        
                </td>
            </tr>
            <tr runat="server" id="trTrafficArea">
                <td class="formCellField"></td>
                <td class="formCellField">
                    <input type="hidden" id="hidTrafficArea" runat="server" name="hidTrafficArea">
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">
                    <asp:Label runat="server" ID="lblTrafficArea" Text="Traffic Area"></asp:Label></td>
                <td class="formCellField">
                    <asp:DropDownList ID="cboTrafficArea" runat="server" />
                    <asp:CustomValidator runat="server" ID="cvTrafficArea" OnServerValidate="cboTrafficAreaValidator_ServerValidate"
                        ControlToValidate="cboTrafficArea" ErrorMessage="You must specify either a postcode or a traffic area.">
                        <img id="imgTrafficAreaCustomValidatorError" runat="server" src="~/images/error.png" title="You must either select a traffic area or enter a postcode." />
                    </asp:CustomValidator>
                </td>
            </tr>
        </table>
    </fieldset>
    
    <fieldset>
        <legend><%=Orchestrator.Globals.Configuration.FleetMetrikInstance ? "Location Notes" : "Point Notes" %></legend>
        <p>These will appear in the Address Pop-up when you hover over the Organisation Name</p>
        <table>
            <tr>
                <td class="formCellField">
                    <asp:TextBox ID="txtPointNotes" runat="server" Width="400" MaxLength="1000" Rows="10" Wrap="true" TextMode="MultiLine"></asp:TextBox>
                </td>
                <td class="formCellField" style="width: 200px; text-align: justify; vertical-align: top;">
                    <br />
                    <b>NOTE</b><br />
                    Any Updates made to Point Notes will take approx 20 minutes to appear within pop-ups.
                </td> 
            </tr> 
        </table>
    </fieldset>

    <fieldset runat="server" id="fsRemoveIntegration">
        <legend>Remove Integration</legend>
        <p>To Remove an Integration, please select the relevant row and click the Cross Button</p>

        <telerik:RadGrid runat="server" ID="RemoveIntegrationGrid" AllowPaging="true" PageSize="5" AllowFilteringByColumn="false" AllowMultiRowSelection="false" AllowSorting="False" AutoGenerateColumns="false" >
                    <MasterTableView>
                        <Columns> 
                            <telerik:GridBoundColumn UniqueName="IntegrationPointID" DataField="IntegrationPointId" HeaderText="Id" Display="false"/>
                            <telerik:GridBoundColumn UniqueName="ExternalPointRef" DataField="ExternalPointRef" HeaderText="From System"/>
                            <telerik:GridBoundColumn UniqueName="ExternalPointDescription" DataField="ExternalPointDescription" HeaderText="Message" />
                            <telerik:GridBoundColumn UniqueName="PointID" HeaderText="Point ID" DataField="PointId" Display="false"/>
                            <telerik:GridBoundColumn UniqueName="Postcode" HeaderText="Post Code" DataField="PostCode" Display="true" />
                            <telerik:GridBoundColumn UniqueName="LastUpdateDate" HeaderText="Last Update Date" DataField="LastUpdateDate" Display="true" />
                            <telerik:GridBoundColumn UniqueName="LastUpdateUserID" HeaderText="Last Updated By" DataField="LastUpdateUserId" Display="true" />

                            <telerik:GridClientSelectColumn UniqueName="CheckboxSelectColumn" FooterText="CheckBoxSelect footer" HeaderText="Select"/>
                            <telerik:GridButtonColumn HeaderText="Remove Integration" HeaderStyle-Width="30" ItemStyle-HorizontalAlign="Center" ButtonType="ImageButton" CommandName="remove" ImageUrl="~/images/newmasterpage/icon-cross.png" ></telerik:GridButtonColumn>
                        </Columns>                 
                    </MasterTableView>
                    <ClientSettings EnablePostBackOnRowClick="True">  
                        <Selecting AllowRowSelect="True" />  
                    </ClientSettings>  
                    <PagerStyle Mode="NumericPages"></PagerStyle>
                </telerik:RadGrid>

    </fieldset>
        
    <div class="buttonbar">
        <asp:Button ID="btnRemove" runat="server" Width="75px" Text="Remove" OnClientClick="if(!confirmPointRemoval()) return false;" OnClick="btnRemove_Click" Visible="false" />
        <asp:Button ID="btnAdd" runat="server" Text="Add Point" OnClick="btnAdd_Click"></asp:Button>
        <telerik:RadCodeBlock runat="server">
            <input type="button" runat="server" id="btnList" onclick="location.href='listpoints.aspx?identityId=<%=IdentityId%>'" value="Return to List" />
            <input type="button" onclick="self.close();" value="Close" />
        </telerik:RadCodeBlock>
    </div>
        
</asp:Content>
