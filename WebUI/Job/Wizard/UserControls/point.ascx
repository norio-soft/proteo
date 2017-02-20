<%@ Reference Page="~/job/wizard/wizard.aspx" %>
<%@ Control Language="c#" Inherits="Orchestrator.WebUI.Job.Wizard.UserControls.Point" Codebehind="Point.ascx.cs" %>
<table width="100%" height="472" cellpadding="0" cellspacing="0" border="0">
	<tr height="56">
		<td bgcolor="white" align="right" style="PADDING-RIGHT:10px" >
			<table width="100%" cellpadding="0" cellspacing="0" align="right">
				<tr>
					<td width="100%" valign="top" align="left">
						<div style="PADDING-LEFT:20px; FONT-WEIGHT:bold; PADDING-TOP:5px"><STRONG>
							<asp:Label id=lblCollectDrop runat="server" Font-Size="Medium"></asp:Label></STRONG>
						</div>
						<div style="PADDING-LEFT:20px; FONT-WEIGHT:bold; PADDING-TOP:5px">
							Add <asp:Label ID="lblCollection" Runat="server">Collection</asp:Label><asp:Label ID="lblDelivery" Runat="server">Delivery</asp:Label> Point
						</div>
						<div style="PADDING-LEFT:35px;PADDING-TOP:2px">
							Please specify the next point to <asp:Label ID="lblCollectFrom" Runat="server">collect from</asp:Label><asp:Label ID="lblDeliverTo" Runat="server">deliver to</asp:Label>.
						</div>
					</td>
					<td align="right">
						<img src="../../images/p1logo.gif" width="50" height="50">			
					</td>
				</tr>
			</table>
		</td>
	</tr>	
	<tr height="2" bgcolor="#aca899" >
		<td colspan="2" height="3" style="BORDER-TOP:#aca899 1pt solid; BORDER-BOTTOM:white 1pt solid"></td>
	</tr>
	<tr height="305" bgcolor="#ece9d8">
		<td style="PADDING-RIGHT:10px;PADDING-LEFT:35px;VERTICAL-ALIGN:top; padding-top:10px;" width="100%">
			<div>
				<table width="100%" border="0" cellpadding="0" cellspacing="0">
				    <tr>
				        <td width="100">
				            <span>Point Owner</span>
				        </td>
				        <td>
				            <asp:Label ID="lblOwner" runat="server" visible="false" />
                            <telerik:radcombobox id="cboOwner" runat="server" enableloadondemand="true" itemrequesttimeout="500"
                                markfirstmatch="true" AllowCustomText="true" radcontrolsdir="~/script/RadControls/" AutoPostBack="false" ShowMoreResultsBox="false"
                                skin="WindowsXP" width="355px" OnClientSelectedIndexChanging="handleOwnerChange" Height="300px" Overlay="true"></telerik:radcombobox>
							<asp:RequiredFieldValidator ID="rfvOwner" Runat="server" ControlToValidate="cboOwner" ErrorMessage="Please enter the point owner."><img src="../../images/error.png"  Title="Please enter the point owner."></asp:RequiredFieldValidator>
							<asp:CustomValidator ID="cfvOwner" runat="server" ControlToValidate="cboOwner" ErrorMessage="This organisation name is already in use." EnableClientScript="false"><img src="../../images/error.png"  Title="This organisation name is already in use."></asp:CustomValidator>
				        </td>
				    </tr>
					<tr>
						<td width="100">
							<span>Point Name</span>
						</td>
						<td>
                            <telerik:radcombobox id="cboPoint" runat="server" enableloadondemand="true" itemrequesttimeout="500"
                                markfirstmatch="true" radcontrolsdir="~/script/RadControls/" AutoPostBack="true" ShowMoreResultsBox="false"
                                skin="WindowsXP" width="355px" Height="300px" Overlay="true"  AllowCustomText="True"></telerik:radcombobox>
							<asp:RequiredFieldValidator ID="rfvPoint" Runat="server" ControlToValidate="cboPoint" ErrorMessage="Please enter the point description."><img src="../../images/error.png"  Title="Please enter the point description."></asp:RequiredFieldValidator>
						</td>
					</tr>
					
					<tr>
						<td width="100">
							<span>Town</span>
						</td>
						<td>
						    <telerik:radcombobox id="cboTown" runat="server" enableloadondemand="true" itemrequesttimeout="500"
                                markfirstmatch="true" radcontrolsdir="~/script/RadControls/" AllowCustomText="false" ShowMoreResultsBox="false"
                                skin="WindowsXP" width="355px" OnClientSelectedIndexChanging="handleChange" Height="300px" Overlay="true"></telerik:radcombobox>
                                
							<asp:RequiredFieldValidator ID="rfvTown" Runat="server" ControlToValidate="cboTown" ErrorMessage="Please select the town."><img src="../../images/error.png"  Title="Please select the town."></asp:RequiredFieldValidator>
						</td>
					</tr>

					<tr>
						<td colspan="2">
							<asp:Panel id="pnlCreateNewPoint" Runat="server" Visible="False">
								<fieldset>
									<legend>Create a new Point</legend>
									<div style="padding-top:10px">You cannot create a new point with this name, as this client already has a point with this name.</div>
								</fieldset>
							</asp:Panel>
						</td>
					</tr>
					<tr>
						<td colspan="2">
							<div class="spacer" style="height:10px;"></div>
						</td>
					</tr>
				</table>
			</div>
		</td>
	</tr>
	<tr height="2" bgcolor="#aca899">
		<td colspan="2" height="3" style="BORDER-TOP:#aca899 1pt solid; BORDER-BOTTOM:white 1pt solid"></td>
	</tr>
	<tr height="46" bgcolor="#ece9d8">
		<td colspan="2" align="right" height="46" style="PADDING-RIGHT:10px">
			<asp:Button ID="btnBack" Runat="server" CausesValidation="False" Text="< Back"></asp:Button>
			<asp:button id="btnNext" runat="server" CausesValidation="True" Text="Next >"></asp:Button>
			&nbsp;&nbsp;
			<asp:Button ID="btnCancel" Runat="server" CausesValidation="False" Text="Cancel"></asp:Button>
		</td>
	</tr>
</table>

<script language="javascript">
        var townCombo           = <%=cboTown.ClientID %>;
        var pointCombo          = <%=cboPoint.ClientID %>;
        
        try{
        var owner               = <%=cboOwner.ClientID %>;
        }catch(e){}
        
        var values = "";
        var townId = 0;
        var identityId = <%=m_identityId %>;
        
        function handleOwnerChange(item)
        {
            identityId = item.Value;
            LoadPoints(null);
        }
        
        function handleChange(item)
        {
            townId = item.Value;
            if (pointCombo.get_value() != "")
                LoadPoints(null);
        }
        
        function <%=cboPoint.ClientID %>_Requesting(cbo)
        {
			if(owner != null)
			{
				var clientData = owner.get_value() + ";" + townId + ";" + <%=cboPoint.ClientID %>.GetText() + ";";
				cbo.ClientDataString = clientData;
			}
			else
			{
				var clientData = identityId + ";" + townId + ";" + <%=cboPoint.ClientID %>.GetText() + ";";
				cbo.ClientDataString = clientData;
			}
        }
        
        function LoadPoints(item)
        {
            pointCombo.ClearSelection();
            
            if (item == null)
            { 
                i = identityId;
                v = townId;
                pointCombo.RequestItems(i + ";" + v + ";", false);
            }    
            else
            {
                v = identityId + ";" + townId + ";" + item.Text;
                pointCombo.RequestItems(v, false);
            }
        }
    </script>

