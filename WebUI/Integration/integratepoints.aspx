<%@ Page language="c#" MasterPageFile="~/default_tableless.master" Inherits="Orchestrator.WebUI.Integration.IntegratePoints" Title="Haulier Enterprise" Codebehind="IntegratePoints.aspx.cs" %>
<%@ Register TagPrefix="componentart" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register TagPrefix="P1" Namespace="P1TP.Components.Web.UI" Assembly="P1TP.Components" %>
<%@ Register TagPrefix="uc1" TagName="point" Src="~/usercontrols/point.ascx" %>

<asp:Content ContentPlaceHolderID="Header" runat="server">
</asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Points Awaiting Integration</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <h2>External points awaiting integration with Haulier Enterprise are displayed below</h2>

    <div style="width:100%;">
        <div style="float:left; width:49%;">
		    <fieldset>
			    <legend>External Points Awaiting Integration</legend>
                <telerik:RadGrid runat="server" ID="pointsToIntegrateGrid" AllowPaging="true" PageSize="20" AllowFilteringByColumn="false" AllowMultiRowSelection="false" AllowSorting="False" AutoGenerateColumns="false">
                    <MasterTableView>
                        <GroupByExpressions>
                            <telerik:GridGroupByExpression>
                                <GroupByFields>
                                    <telerik:GridGroupByField FieldName="IntegrationApplication" />
                                </GroupByFields>
                               <SelectFields>
                                    <telerik:GridGroupByField FieldName="IntegrationApplication" FieldAlias="Source"/>
                                </SelectFields>
                            </telerik:GridGroupByExpression>
                        </GroupByExpressions>
                        <Columns> 
                            <telerik:GridBoundColumn UniqueName="IntegrationPointId" DataField="IntegrationPointId" HeaderText="Id" Display="false"/>
                            <telerik:GridBoundColumn UniqueName="IntegrationApplication" DataField="IntegrationApplication" HeaderText="Source"/>
                            <telerik:GridBoundColumn UniqueName="PointType" DataField="PointType" HeaderText="Type" />
                            <telerik:GridBoundColumn UniqueName="ExternalPointRef" DataField="ExternalPointRef" HeaderText="Reference" />
                            <telerik:GridBoundColumn UniqueName="ExternalPointDescription" HeaderText="Description" DataField="ExternalPointDescription" />
                            <telerik:GridBoundColumn UniqueName="AddressLine1" HeaderText="AddressLine1" DataField="AddressLine1" Display="false"/>
                            <telerik:GridBoundColumn UniqueName="AddressLine2" HeaderText="AddressLine2" DataField="AddressLine2" Display="false" />
                            <telerik:GridBoundColumn UniqueName="AddressLine3" HeaderText="AddressLine3" DataField="AddressLine3" Display="false" />
                            <telerik:GridBoundColumn UniqueName="PostTown" HeaderText="PostTown" DataField="PostTown" Display="false" />
                            <telerik:GridBoundColumn UniqueName="County" HeaderText="County" DataField="County" Display="false" />
                            <telerik:GridBoundColumn UniqueName="PostCode" HeaderText="PostCode" DataField="PostCode" Display="true" />                                   
                        </Columns>                 
                    </MasterTableView>
                    <ClientSettings EnablePostBackOnRowClick="True">  
                        <Selecting AllowRowSelect="True" />  
                    </ClientSettings>  
                    <PagerStyle Mode="NumericPages"></PagerStyle>
                </telerik:RadGrid>
		    </fieldset>
	    </div>
    	
	    <div style="float:right; width:49%;">
		    <asp:Panel id="pnlIntegratePoint" runat="server">
		        <fieldset>
		            <legend>Integrate this Address (Point)</legend>
		            <asp:Label id="lblIntegratingPoint" runat="server"></asp:Label>
		        </fieldset>
		    </asp:Panel>
    				
            <fieldset>
                <legend>Integrate to an existing point</legend>
                <asp:Label ID="lblExistingPointValidation" runat="server" Text="" Visible="false" ForeColor="Red"></asp:Label>
                <uc1:point ID="existingPoint" runat="server" CanCreateNewPoint="false" ShowHelp="true" CanClearPoint="true" />
            </fieldset>

            <fieldset>
                <legend>Integrate to a new point</legend>
                    <asp:Label ID="lblNewPointValidation" runat="server" Text="" Visible="false" ForeColor="Red"></asp:Label>
                    <uc1:point ID="newPoint" runat="server" CanCreateNewPoint="true" ShowHelp="true" />
            </fieldset>
            <div id="RemoveIntegrationID">
            <fieldset class="RemoveIntegration" onchange="hideShowFieldSet()" id="fsRemoveIntegration">
                <legend>Remove Messages Awaiting Integration</legend>
                    <asp:Label ID="lblRemoveIntegration" runat="server" Text="" Visible="false" ForeColor="Red"></asp:Label>
                    <h2>Integration Messages</h2>
                    <p>Messages sent that are awaiting point integration, please note that these messages may not always be related to the point</p>
                <telerik:RadGrid runat="server" ID="RemoveIntegrationGrid" AllowPaging="true" PageSize="20" AllowFilteringByColumn="false" AllowMultiRowSelection="true" AllowSorting="False" AutoGenerateColumns="false" >
                    <MasterTableView>
                        <Columns> 
                            <telerik:GridBoundColumn UniqueName="ImportMessageID" DataField="ImportMessageID" HeaderText="Id" Display="false"/>
                            <telerik:GridBoundColumn UniqueName="FromSystem" DataField="FromSystem" HeaderText="From System"/>
                            
                            <telerik:GridBoundColumn UniqueName="Message" DataField="Message" HeaderText="Message" />
                            
                            <telerik:GridBoundColumn UniqueName="CreateDate" HeaderText="Create Date" DataField="CreateDate"/>
                            <telerik:GridBoundColumn UniqueName="CreateUserID" HeaderText="CreateUserID" DataField="CreateUserID" Display="false" />
                            <telerik:GridClientSelectColumn UniqueName="CheckboxSelectColumn" FooterText="CheckBoxSelect footer" />
                        </Columns>                 
                    </MasterTableView>
                    <ClientSettings EnablePostBackOnRowClick="True">  
                        <Selecting AllowRowSelect="True" />  
                    </ClientSettings>  
                    <PagerStyle Mode="NumericPages"></PagerStyle>
                </telerik:RadGrid>
            </fieldset>
            </div>
            <div class="buttonbar" style="margin-top:5px;">
                <asp:Button id="btnIntegratePoint" runat="server" CausesValidation="False" Text="Integrate Point"></asp:Button>
                <asp:Button ID="btnRemoveIntegration" runat="server" CausesValidation="false" Text="Remove Point" />
                <asp:Button id="btnCancel" runat="server" Text="Cancel" CausesValidation="False"></asp:Button>
            </div>
        </div>
        
        <div class="clearDiv"></div>
    </div>
      <script type="text/javascript">
         

          //$(function () {
          //    $('fieldset.RemoveIntegration').hide();
          //    $('input[name="other"]').click(function () {
          //        if (this.checked) {
          //            $('fieldset.RemoveIntegration').show();
          //            $('input[name="other"]').hide();
          //        } else {
          //            $('fieldset.RemoveIntegration').hide();
          //        }
          //    });
          //});

      </script>
</asp:Content>
