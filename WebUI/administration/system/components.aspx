<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.SetUp.components" MasterPageFile="~/default_tableless.Master" Codebehind="components.aspx.cs" %>

<asp:Content ContentPlaceHolderID="Header" runat="server"></asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Setup Scaning Components</h1></asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>The links below allow you to set up a scanning machine</h2>
        <table width="100%" cellpadding="5px">
		    <tr>
			    <td valign="top" width="50%">
			        <a href="../../downloads/scanbutton.exe">
				        <span class="downloadPanel">
					        <span style="font-weight: bold;">Orchestrator Scanner Software</span><span class="orangeBold"></span><br>
					        <span>This is required to allow scanning machines to scan and upload (not to be used for Development or Testing)</span>
				        </span>
				    </a>
			    </td>
			    <td valign="top" width="50%">
			        <a href="../../downloads/Fujitsu fj-4120C2/Disk1/Setup.exe">
				        <span class="downloadPanel">
					        <span style="font-weight: bold;">Fujitsu fi-4120C2 Scanner Drivers</span><br />
					        <span>These are the scanner drivers for the Fujitsu fi-4120C2 series.</span>
				        </span>
				    </a>
			    </td>
		    </tr>
		    <tr>
			    <td valign="top" width="50%">
			        <a href="http://www.adobe.com/products/acrobat/readstep2.html" target="_blank">
				        <span class="downloadPanel">
					        <span style="font-weight: bold;">Adobe Reader</span><span class="orangeBold"></span><br>
					        <span>This is required to view any scanned POD or PCV</span>
				        </span>
				    </a>
			    </td>
			    <td valign="top" width="50%">
			        <a href="scanner.aspx" target="_blank">
			            <span class="downloadPanel">
					        <span style="font-weight: bold;">Scanner Settings</span><span class="orangeBold"></span><br>
					        <span>Configure Scanner</span>
				        </span>
				    </a>
			    </td>
		    </tr>
		    <tr>
			    <td valign="top" width="50%">
			        <a id="A1" href="~/downloads/Orchestrator.Vigo.WinApp.Deployment.msi" runat="server">
				        <span class="downloadPanel">
					        <span style="font-weight: bold;">Vigo Integration Application</span><span class="orangeBold"></span><br>
					        <span>This is required to integrate orders between Orchestrator and Vigo</span>
				        </span>
				    </a>
			    </td>
			    <td valign="top" width="50%">
			    </td>
		    </tr>		
	    </table>
</asp:Content>