<%@ Page language="c#" Inherits="Orchestrator.WebUI.addresslookup.addresslookup" Codebehind="addresslookup.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>addresslookup</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<style>
			BODY { FONT-SIZE: 8pt; COLOR: #000000; FONT-FAMILY: Tahoma, Verdana, Arial, sans-serif }
			TD { FONT-SIZE: 8pt; COLOR: #000000; FONT-FAMILY: Tahoma, Verdana, Arial, sans-serif }
			TH { FONT-SIZE: 8pt; COLOR: #000000; FONT-FAMILY: Tahoma, Verdana, Arial, sans-serif }
			H1 { FONT-WEIGHT: bold; FONT-SIZE: 10pt }
			H2 { FONT-WEIGHT: bold; FONT-SIZE: 10pt }
			H3 { FONT-WEIGHT: bold; FONT-SIZE: 10pt }
			H4 { FONT-WEIGHT: bold; FONT-SIZE: 10pt }
			H5 { FONT-WEIGHT: bold; FONT-SIZE: 10pt }
			.FormButton { BORDER-RIGHT: #4d4a46 1px solid; BORDER-TOP: #4d4a46 1px solid; FONT-SIZE: 8pt; BORDER-LEFT: #4d4a46 1px solid; WIDTH: 80px; CURSOR: hand; BORDER-BOTTOM: #4d4a46 1px solid; FONT-FAMILY: Verdana, Arial, Helvetica; BACKGROUND-COLOR: buttonface }
			.towns {width:100%; height:100%}
		</style>
		
	</HEAD>
	<body bgcolor="buttonface" leftmargin="0" topmargin="0" onload="onLoad();">
		<form id="Form1" method="post" runat="server">
			<table style="WIDTH: 100%; BORDER-BOTTOM: #000000 1px solid; HEIGHT: 50px; BACKGROUND-COLOR: white"
				cellspacing="0" cellpadding="4" border="0">
				<tr>
					<td>
						<span style="FONT-WEIGHT: bold; FONT-SIZE: 10pt"><span id="HelpHeader">Verify Town</span></span>
						<br>
						&nbsp;&nbsp;In order to be able to track resource (Driver/Vehicle/Trailer) we need to know which Post Town this point is nearest to.
					</td>
				</tr>
			</table>
			<table cellspacing="0" cellpadding="8" width="100%" border="0">
				<tr>
					<td>
						<div style="PADDING-RIGHT:4px; PADDING-LEFT:4px; PADDING-BOTTOM:4px; OVERFLOW:auto; BORDER-TOP-STYLE:inset; PADDING-TOP:4px; BORDER-RIGHT-STYLE:inset; BORDER-LEFT-STYLE:inset; HEIGHT:375px; BACKGROUND-COLOR:white; BORDER-BOTTOM-STYLE:inset">
							<asp:listbox id="lstTowns" runat="server" Visible="False" DataTextField="Description" DataValueField="TownId" Rows="10" CssClass="town" Width="100%" Height="100%" AutoPostBack="True" onselectedindexchanged="lstTowns_SelectedIndexChanged"></asp:listbox>	
						</div>
					</td>
				</tr>
				<tr>
					<td align="right">
						<input type="button" onclick="setNames();" value="OK" class="FormButton">&nbsp;<input id="closeButton" type="button" class="FormButton" value="Close" onclick="javascript:window.close();">
						<asp:textbox id="txtTown" runat="server" visible="false"></asp:textbox><asp:textbox id="txtCounty" runat="server" visible="false"></asp:textbox>
			<asp:label id="lblTownId" runat="server" visible="false"></asp:label>
					</td>
				</tr>
			</table>
			<script language="javascript">
		var TownId	 = <%=m_townId%>;
		var TownName = "<%=m_townName%>";
		var County	 = '<%=m_countyName%>';
		var postTown = '<%=Request["townNameCtl"]%>';
		var townId	 = '<%=Request["townIDCtl"]%>';
		
		var countyName = '<%=Request["countyNameCtl"]%>';
		var ctl1	 = '<%=Request["ctl1"]%>';
		
		
		function onLoad() {
			window.focus();
			
			if ("<%=m_oneRow%>"=="True")
			{
			  setNames();
			}
		}
		
		function setNames() 
		{
			window.opener.document.all[postTown].value = TownName;
			window.opener.document.all[townId].value = TownId;
			try
			{
				window.opener.document.all[countyName].value = County;	
				window.opener.document.all[ctl1].value = TownName;	
			}
			catch(err){}
			
			self.close();
		}
		
		</script>		
		</form>
	</body>
</HTML>
