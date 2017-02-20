<%@ Page language="c#" Inherits="Orchestrator.WebUI.Job.resourcemovement" Codebehind="resourcemovement.aspx.cs" %>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Register TagPrefix="P1" Namespace="P1TP.Components.Web.UI" Assembly="P1TP.Components" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>
<%@ Register TagPrefix="cc2" Namespace="Orchestrator.WebUI.Pagers" Assembly="WebUI" %>
<%@ Register Assembly="RadCombobox.Net2" Namespace="Telerik.Web.UI" TagPrefix="radC" %>
<uc1:header id="Header1" runat="server" title="Resource Movement" SubTitle="Resource Movement Job"></uc1:header>
<style>
		.PageNumbers { FONT-SIZE: 10pt; FONT-FAMILY: Tahoma, Verdana, Arial; TEXT-DECORATION: none }
		.CurrentPage { FONT-WEIGHT: bold; FONT-SIZE: 10pt; COLOR: red; FONT-FAMILY: Tahoma, Verdana, Arial; TEXT-DECORATION: underline }
</style>
<form id="Form1" runat="server">
	<table>
		<tr bgcolor="#ece9d8">
			<td style="PADDING-RIGHT:10px;PADDING-LEFT:35px;VERTICAL-ALIGN:top;PADDING-TOP:10px"
				width="100%">
				<FIELDSET>
					<LEGEND>
						<strong>Start Point (From)</strong></LEGEND>
					<table border="0" cellpadding="0" cellspacing="0">
						<tr>
							<td>Organisation&nbsp;</td>
							<td>
                                <radC:RadComboBox ID="cboOrganisation" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                                    MarkFirstMatch="true" OnClientSelectedIndexChanging="handleChange" RadControlsDir="~/script/RadControls/"
                                    ShowMoreResultsBox="true" Skin="WindowsXP" Width="355px">
                                </radC:RadComboBox>
							</td>
						</tr>
						<tr>
							<td>Point&nbsp;</td>
							<td>
								  <radC:RadComboBox ID="cboPoint" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                                    MarkFirstMatch="true"  RadControlsDir="~/script/RadControls/"
                                    ShowMoreResultsBox="true" Skin="WindowsXP" Width="355px">
                                </radC:RadComboBox>
								<asp:RequiredFieldValidator id="rfvPoint" runat="server" ControlToValidate="cboPoint" Display="Dynamic" ErrorMessage="Please select a start point.">
									<img src="../images/error.gif" alt="Please select a start point." /></asp:RequiredFieldValidator>
							</td>
						</tr>
					</table>
				</FIELDSET>
			</td>
		</tr>
		<tr bgcolor="#ece9d8">
			<td style="PADDING-RIGHT:10px;PADDING-LEFT:35px;VERTICAL-ALIGN:top;PADDING-TOP:10px"
				width="100%">
				<FIELDSET>
					<LEGEND>
						<strong>End Point (To)</strong></LEGEND>
					<table border="0" cellpadding="0" cellspacing="0">
						<tr>
							<td>Organisation&nbsp;</td>
							<td>
							    <radC:RadComboBox ID="cboOrganisationDestination" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                                    MarkFirstMatch="true" OnClientSelectedIndexChanging="handleChangeDestination" RadControlsDir="~/script/RadControls/"
                                    ShowMoreResultsBox="true" Skin="WindowsXP" Width="355px">
                                </radC:RadComboBox>
							    
								
							</td>
						</tr>
						<tr>
							<td>Point&nbsp;</td>
							<td>
							    <radC:RadComboBox ID="cboPointDestination" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                                    MarkFirstMatch="true"  RadControlsDir="~/script/RadControls/"
                                    ShowMoreResultsBox="true" Skin="WindowsXP" Width="355px">
                                </radC:RadComboBox>
								<asp:RequiredFieldValidator id="Requiredfieldvalidator1" runat="server" ControlToValidate="cboPointDestination" Display="Dynamic"
									ErrorMessage="Please select a End point.">
									<img src="../images/error.gif" alt="Please select a End point." /></asp:RequiredFieldValidator>
							</td>
						</tr>
					</table>
				</FIELDSET>
			</td>
		</tr>
	</table>
	  <script language="javascript">
        var organisationCombo   = <%=cboOrganisation.ClientID %>;
        var pointCombo          = <%=cboPoint.ClientID %>
        
        var organisationComboD   = <%=cboOrganisationDestination.ClientID %>;
        var pointComboD          = <%=cboPointDestination.ClientID %>
        
        var values = "";
        var identityId = 0;
        var destinationidentityId = 0;
        
        function handleChange(item)
        {
            identityId = item.Value;
            LoadPoints(null);
        }
        
        function handleChangeDestination(item)
        {
            destinationidentityId = item.Value;
            LoadPointsDestination(null);
        }
        
        
        function LoadPoints(item)
        {
            pointCombo.ClearSelection();
            
            if (item == null)
            { 
                v = identityId;
                pointCombo.RequestItems(v, false);
                //window.setTimeout(function(){pointCombo.ShowDropDown()}, 50);
            }    
            else
            {
                pointCombo.RequestItems(item.Text, false);
                
            }
        }
        
        function LoadPointsDestination(item)
        {
            pointComboD.ClearSelection();
            
            if (item == null)
            { 
                v = destinationidentityId;
                pointComboD.RequestItems(v, false);
                //window.setTimeout(function(){pointCombo.ShowDropDown()}, 50);
            }    
            else
            {
                pointComboD.RequestItems(item.Text, false);
                
            }
        }
    </script>
	<script language="javascript" type="text/javascript">
<!--
	var storedIdentityId = "<%=m_organisationId%>";
	var storedStartTown = "<%=m_startTown%>";
	var storedStartTownId = "<%=m_startTownId%>";
	var storedPointId = "<%=m_pointId%>";
	
	var storedDestinationIdentityId	= "<%=m_destinationOrganisationId%>";
	var storedDestinationTown		= "<%=m_destinationTown%>";
	var storedDestinationTownId		= "<%=m_destinationTownId%>";
	var storeddestinationPointId	= "<%=m_destinationPointId%>";
	
	function ClientStateClient()
	{
		var state = new Object();
		
		state["ClientId"] = storedIdentityId;
		DbComboStateChanged("<%=cboPoint.ClientID%>");
		
		return state;
	}
	
	function ClientStateClientDestination()
	{
		var state = new Object();
		
		state["ClientId"] = storedDestinationIdentityId;
		DbComboStateChanged("<%=cboPointDestination.ClientID%>");
		
		return state;
	}

	function ClientStateStartTown()
	{
		var state = new Object();
		
		state["TownId"]		= storedStartTownId;
		DbComboStateChanged("<%=cboPoint.ClientID%>");

		return state;
	}
	
	function ClientStateStartTownDestination()
	{
		var state = new Object();
		
		state["TownId"]		= storedDestinationTownId;
		DbComboStateChanged("<%=cboPointDestination.ClientID%>");

		return state;
	}

	function ClientStatePoint()
	{
		var state = new Object();

		state["IdentityId"]	= storedIdentityId;
		state["TownId"]		= storedStartTownId;
		state["PointId"]	= storedPointId;

		return state;
	}
	
	function ClientStatePointDestination()
	{
		var state = new Object();

		state["IdentityId"]	= storedDestinationIdentityId;
		state["TownId"]		= storedDestinationTownId;
		state["PointId"]	= storedDestinationPointId;

		return state;
	}

	function StoreClient(Value, Text, SelectionType)
	{
		if (SelectionType == 2 ||SelectionType == 9 || SelectionType == 7 || SelectionType == 3)
		{
			storedIdentityId = Value;
		}
	}			
	
	function StoreClientDestination(Value, Text, SelectionType)
	{
		if (SelectionType == 2 ||SelectionType == 9 || SelectionType == 7 || SelectionType == 3)
		{
			storedDestinationIdentityId = Value;
		}
	}			

	function StoreStartTown(Value, Text, SelectionType) 
	{
		if (SelectionType == 2 ||SelectionType == 9 || SelectionType == 7 || SelectionType == 3)
		{
			storedStartTownId = Value;
		}
	}
	
	function StoreDestinationTown(Value, Text, SelectionType) 
	{
		if (SelectionType == 2 ||SelectionType == 9 || SelectionType == 7 || SelectionType == 3)
		{
			storedDestinationTownId = Value;
		}
	}

	function StorePoint(Value, Text, SelectionType) 
	{
		if (SelectionType == 2 ||SelectionType == 9 || SelectionType == 7 || SelectionType == 3)
		{
			storedStartPointId = Value;
		}
	}
	
	function StoreDestinationPoint(Value, Text, SelectionType) 
	{
		if (SelectionType == 2 ||SelectionType == 9 || SelectionType == 7 || SelectionType == 3)
		{
			storedDestinationPointId = Value;
		}
	}
	
	
//-->
	</script>
</form>
<uc1:footer id="Footer1" runat="server"></uc1:footer>
