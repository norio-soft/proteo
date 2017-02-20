<%@ Reference Page="~/job/wizard/wizard.aspx" %>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Job.Wizard.UserControls.pointTesterUC" Codebehind="pointTesterUC.ascx.cs" %>
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
							<span>Point Name</span>
						</td>
						<td>
						       <radc:radcombobox id="cboPoint" runat="server" enableloadondemand="true" itemrequesttimeout="500"
                                markfirstmatch="true" radcontrolsdir="~/RadControls/" AutoPostBack="true"
                                showmoreresultsbox="true" skin="WindowsXP" width="355px"></radc:radcombobox>
							<asp:RequiredFieldValidator ID="rfvPoint" Runat="server" ControlToValidate="cboPoint" ErrorMessage="Please enter the point description."><img src="../../images/error.png"  Title="Please enter the point description."></asp:RequiredFieldValidator>
						</td>
					</tr>
					
					<tr>
						<td width="100">
							<span>Town</span>
						</td>
						<td>
						    <telerik:radcombobox id="cboTown" runat="server" enableloadondemand="true" itemrequesttimeout="500"
                                markfirstmatch="true" radcontrolsdir="~/script/RadControls/"
                                showmoreresultsbox="true" skin="WindowsXP" width="355px" OnClientSelectedIndexChanging="handleChange"></telerik:radcombobox>
                                
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
        
        var values = "";
        var townId = 0;
        
        function handleChange(item)
        {
        alert("handling change");
            townId = item.Value;
            LoadPoints(null);
        }
        
        
        function LoadPoints(item)
        {
        alert("load points");
            pointCombo.ClearSelection();
            
            if (item == null)
            { 
                v = townId;
                pointCombo.RequestItems(v, false);
                //window.setTimeout(function(){pointCombo.ShowDropDown()}, 50);
            }    
            else
            {
                v = townId +";" + item.Text;
                pointCombo.RequestItems(v, false);
                
            }
        }
    </script>

