<%@ Reference Page="~/job/wizard/wizard.aspx" %>
<%@ Control Language="c#" Inherits="Orchestrator.WebUI.Job.Wizard.UserControls.AddPoint" Codebehind="AddPoint.ascx.cs" %>
<script language="javascript" type="text/javascript">
<!--
	
	var win = null;
	
	function openChecker() 
	{
		var AddressLine1	= "<%=txtAddressLine1.ClientID%>";
		var AddressLine2	= "<%=txtAddressLine2.ClientID%>";
		var AddressLine3	= "<%=txtAddressLine3.ClientID%>";
		var PostTown		= "<%=txtPostTown.ClientID%>";
		var County			= "<%=txtCounty.ClientID%>";
		var PostCode		= "<%=txtPostCode.ClientID%>";
		var longitude		= "<%=txtLongitude.ClientID%>";
		var latitude		= "<%=txtLatitude.ClientID%>";
		var TrafficArea		= "<%=hidTrafficArea.ClientID%>";
		var searchCompany	= "<%=m_company%>";
		var searchTown = "<%=m_town%>";
		var setPointRadius = "<%=hdnSetPointRadius.ClientID %>";
	
		var sURL = "../../addresslookup/fullwizard.aspx?";
		sURL += "AddressLine1=" + AddressLine1;
		sURL += "&AddressLine2=" + AddressLine2;
		sURL += "&AddressLine3=" + AddressLine3;
		sURL += "&PostTown=" + PostTown;
		sURL += "&County=" + County;
		sURL += "&PostCode=" + PostCode;
		sURL += "&longitude=" + longitude;
		sURL += "&latitude=" + latitude;
		sURL +=	"&TrafficArea=" + TrafficArea;
		sURL += "&searchCompany=" + searchCompany;
		sURL += "&searchTown=" + searchTown;
		sURL += "&setPointRadius=" + setPointRadius;

		if (document.all)
		{
			if (win)
			{
				win.close();
				win = null;
			}
		}
		
		win = window.open(sURL, "Checker", "toolbar=0,location=0,directories=0,status=0,menubar=0,scrollbars=0,resizable=0,width=560,height=450");
		
		// Hide the validator
		var validator = document.getElementById('<%=rfvLongitude.ClientID%>');
		if (validator != null)
			validator.style.visibility = "hidden";
	}
//-->
</script>
<table width="100%" height="452" cellpadding="0" cellspacing="0" border="0" id="tblMain">
	<tr height="56">
		<td bgcolor="white" align="right" style="PADDING-RIGHT:10px">
			<table width="100%" cellpadding="0" cellspacing="0" align="right">
				<tr>
					<td width="100%" valign="top" align="left">
						<div style="PADDING-LEFT:20px; FONT-WEIGHT:bold; PADDING-TOP:5px">
							Adding a New Point
						</div>
						<div style="PADDING-LEFT:35px;PADDING-TOP:2px">
							Adding a new point for
							<%=m_company%>
							at
							<%=m_town%>
							.
						</div>
					</td>
					<td align="right">
						<img src="../../images/p1logo.gif" width="50" height="50">
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr height="2" bgcolor="#aca899">
		<td colspan="2" height="3" style="BORDER-TOP:#aca899 1pt solid; BORDER-BOTTOM:white 1pt solid"></td>
	</tr>
	<tr height="305" bgcolor="#ece9d8">
		<td style="PADDING-RIGHT:10px;PADDING-LEFT:35px;VERTICAL-ALIGN:top;PADDING-TOP:10px"
			width="100%">
			<div>
				<table width="100%" border="0" cellpadding="0" cellspacing="0">
					<tr>
						<td width="100">
							<span><b>Company</b></span>
						</td>
						<td>
							<%=m_company%>
						</td>
					</tr>
					<tr>
						<td>
							<span><b>Town</b></span>
						</td>
						<td>
							<%=m_town%>
						</td>
					</tr>
					<tr>
						<td>
							<span><b>Description</b></span>
						</td>
						<td>
							<%=m_description%>
						</td>
					</tr>
					<tr>
						<td colspan="2">
							<div class="spacer" style="HEIGHT:10px"></div>
						</td>
					</tr>
					<tr>
						<td colspan="2">
							<a href="javascript:openChecker()">Set Address for this Point</a>
							<asp:RequiredFieldValidator id="rfvLongitude" Runat="server" ControlToValidate="txtLongitude" ErrorMessage="Please select an address.">
								<img src="../../images/error.png" Title="Please select an address."></asp:RequiredFieldValidator>
							<asp:textbox id="txtAddressLine1" style="DISPLAY: none" runat="server"></asp:textbox>
							<asp:textbox id="txtAddressLine2" style="DISPLAY: none" runat="server"></asp:textbox>
							<asp:textbox id="txtAddressLine3" style="DISPLAY: none" runat="server"></asp:textbox>
							<asp:textbox id="txtPostTown" style="DISPLAY: none" runat="server"></asp:textbox>
							<asp:textbox id="txtCounty" style="DISPLAY: none" runat="server"></asp:textbox>
							<asp:textbox id="txtPostCode" style="DISPLAY: none" runat="server"></asp:textbox>
							<asp:textbox id="txtLongitude" style="DISPLAY: none" runat="server"></asp:textbox>
							<asp:textbox id="txtLatitude" style="DISPLAY: none" runat="server"></asp:textbox>
							<input type="hidden" id="hidTrafficArea" runat="server" NAME="hidTrafficArea"/>
							<input type="hidden" id="hdnSetPointRadius" runat="server" name="hdnSetPointRadius"/>
						</td>
					</tr>
					<tr>
						<td colspan="2">
							<div class="spacer" style="HEIGHT:10px"></div>
						</td>
					</tr>
					<TR>
						<td width="100">
							<span><b>Address</b></span>
						</td>
						<TD>
							<asp:Label id="lblAddressLine1" runat="server">No Address Set</asp:Label></TD>
					</TR>
					<TR>
						<td></td>
						<TD><asp:Label id="lblAddressLine2" runat="server"></asp:Label></TD>
					</TR>
					<TR>
						<td></td>
						<TD><asp:Label id="lblAddressLine3" runat="server"></asp:Label></TD>
					</TR>
					<TR>
						<td></td>
						<TD><asp:Label id="lblPostTown" runat="server"></asp:Label></TD>
					</TR>
					<TR>
						<td></td>
						<TD><asp:Label id="lblCounty" runat="server"></asp:Label></TD>
					</TR>
					<TR>
						<td></td>
						<TD><asp:Label id="lblPostCode" runat="server"></asp:Label></TD>
					</TR>
					<tr>
					    <td valign="top"><b>Point Notes</b></td>
					    <td>
					        <asp:textbox id="txtPointNotes" runat="server" width="375" MaxLength="1000" Rows="10" Wrap="true"   TextMode="MultiLine"></asp:textbox>
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
			<asp:Button id="btnNext" runat="server" CausesValidation="True" Text="Next >"></asp:Button>
			&nbsp;&nbsp;
			<asp:Button ID="btnCancel" Runat="server" CausesValidation="False" Text="Cancel"></asp:Button>
		</td>
	</tr>
</table>
<script language="javascript" type="text/javascript">
<!--
	function HidePage()
	{
		if (typeof(Page_ClientValidate) == 'function')
		{
			if (Page_ClientValidate())
			{
	            document.getElementById("tblMain").style.display = "none";
		        document.getElementById("tblRotate").style.display = "";
			}
		}
		else
		{
	        document.getElementById("tblMain").style.display = "none";
		    document.getElementById("tblRotate").style.display = "";
		}
	}
//-->
</script>
<table width="100%" height="452" cellpadding="0" cellspacing="0" border="0" id="tblRotate" style="display: none">
	<tr height="56">
		<td bgcolor="white" align="right" style="PADDING-RIGHT:10px" >
			<table width="100%" cellpadding="0" cellspacing="0" align="right">
				<tr>
					<td width="100%" valign="top">
						<div style="PADDING-LEFT:20px; FONT-WEIGHT:bold; PADDING-TOP:5px">
							Processing
						</div>
						<div style="PADDING-LEFT:35px;PADDING-TOP:2px">
							Your actions are being processed, please wait.
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
		<td style="PADDING-RIGHT:10px;PADDING-LEFT:35px;VERTICAL-ALIGN:top; padding-top:10px;" width="100%"></td>
	</tr>
	<tr height="2" bgcolor="#aca899">
		<td colspan="2" height="3" style="BORDER-TOP:#aca899 1pt solid; BORDER-BOTTOM:white 1pt solid"></td>
	</tr>
	<tr height="46" bgcolor="#ece9d8">
		<td colspan="2" align="right" height="46" style="PADDING-RIGHT:10px"></td>
	</tr>
</table>