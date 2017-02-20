<%@ Page Language="C#" AutoEventWireup="true" Inherits="help_releases" Codebehind="releases.aspx.cs" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<uc1:header id="Header1" runat="server" PageTitle="Releases" Title="Homepage" SubTitle="Release and Bug Fix History."></uc1:header>
<form runat="server" Id="Form1">
    <style>
        .BugId {color:blue; font-weight:bold;}
    </style>
  <table width="100%">      
      <tr valign="top">
        <td valign="top" width="50%">
            <table id="tblChanges" runat="server" width="100%" cellpadding="0" cellspacing="0">
                <tr>
	                <td class="myHeading" width="100%"><img id="Img7" src="../images/cornerLeftYellow.gif" alt="" border="0" /></td>
	                <td><img id="Img8" src="../images/corner_Right.gif" alt="" border="0" style="width:10px;" /></td>		
                </tr>
                <tr>
	                <td colspan="2">
		                <table cellspacing="2" class="greyBorder myTrafficDesk" width="100%" height=150>
			                <tr>
				                <td colspan="2" class="pageHeadingDefault myTitle" style="BORDER-RIGHT:medium none; BORDER-TOP:medium none; BORDER-LEFT:medium none; BORDER-BOTTOM:medium none">
					                Changes to the System (Version 1.0.0.33) 27 February 2006
				                </td>
			                </tr>
			                <tr height=100% align="left" valign="top">
				                <td align="left" valign="top" width="34"><img src="../images/ico_info.gif" /></td>
				                <td class="greyText" width="100%">
				                    <ul>
				                        <li>Rates are now enforced when updating a job. <b>(Tony)</b></li>
				                        <li>You can now Update Dockets on a Load at anytime. <b>(P1)</b></li>
				                        <li>There is now a refresh button on the Traffic Sheet and the Job Sheet <b>(JR)</b></li>
				                    </ul>
				                    <hr />
				                    V1.0.0.32 23 February 2006
				                    <ul>
				                        <li>Popup of Extras on Job Search results. <b>(Tony)</b></li>
				                        <li>Adding instructions to a job no longer ruins the leg times. <b>(Planners)</b></li>
				                        <li>Job linking from Purple Book to Traffic Sheet. <b>(Planners)</b></li>
				                    </ul>
				                    <hr />
				                    V1.0.0.31 21 February 2006
				                    <ul>
				                        <li>Batch Invoicing now has the facility to sort by collection point. <b>(Kevin)</b></li>
				                        <li>Job Charge is shown on the Export to CSV version of the Call In Sheet. <b>(Tony)</b></li>
				                        <li>Job Details page now uses more of the available space. <b>(Tony)</b></li>
				                        <li>Textbox size for Contact when adding an extra has been incresed. <b>(Tony)</b></li>
				                    </ul>
				                    <hr />
				                    V1.0.0.30 16 February 2006
				                    <ul>
				                        <li>List Rates page now has functioning filtering abilities. <b>(P1)</b></li>
				                        <li>Fixed Units are now supported by the system. <b>(Haullier)</b></li>
				                        <li>Following speed increase to job search queries, the search field choice has been recoded. <b>(P1)</b></li>
				                    </ul>
				                </td>					
			                </tr>
		                </table>
	                </td>		
                </tr>
            </table>
          </td>
          <td>&nbsp;</td>
          <td width="50%"> 
            <table id="tblBugs" runat="server" width="100%" cellpadding="0" cellspacing="0">
                <tr>
	                <td class="myHeading" width="100%"><img id="Img9" src="../images/cornerLeftYellow.gif" alt="" border="0" /></td>
	                <td><img id="Img10" src="../images/corner_Right.gif" alt="" border="0" style="width:10px;" /></td>		
                </tr>
                <tr valign="top">
	                <td colspan="2">
		                <table cellspacing="2" class="greyBorder myTrafficDesk" width="100%" height=150>
			                <tr>
				                <td colspan="2" class="pageHeadingDefault myTitle" style="BORDER-RIGHT:medium none; BORDER-TOP:medium none; BORDER-LEFT:medium none; BORDER-BOTTOM:medium none">					
					               Bugs Fixed (Version 1.0.0.33) 27 February 2006
				                </td>
			                </tr>
			                <tr height=100% align="left" valign="top">
				                <td align="left" valign="top" width="34"><img src="../images/ico_info.gif" /></td>
				                <td class="greyText" width="100%">
				                    <ul>
				                        <li><span class="BugId"> Bug Id 74: </span>Searching for Tesco no longer searched for jobs involving the Trailer 'esco'. <b>(Roy)</b></li>
				                    </ul>
				                    <hr />
				                    V1.0.0.32 22 February 2006
				                    <ul>
				                        <li><span class="BugId"> Bug Id 65: </span>Deleted Drivers Can be Planned</li>
			                        </ul>
				                    <hr />
				                    V1.0.0.31 21 February 2006
				                    <ul>
				                        <li>New grid applied to re-enable sorting of collection points and increase speed on Normal Invoice Preparation. <b>(Kevin)</b></li>
				                        <li>Duplicate Driver Details were being displayed in the Driver List <b>(J.Cooper)</b></li>
				                        <li>Weekend Sheet now shows appropriate data. <b>(Tony)</b></li>
				                    </ul>
				                    <hr />
				                    V1.0.0.30 16 February 2006
				                    <ul>
				                        <li>Trunking legs will now only use the resources selected for the new leg. <b>(Tony)</b></li>
				                        <li>One Liner Invoice will no longer repeat details. <b>(Kevin)</b></li>
				                        <li>Load Release Form now pulls the correct vehicle and the report is populated correctly. <b>(Tony)</b></li>
				                    </ul>
				                    
				                </td>					
			                </tr>
		                </table>
	                </td>		
                </tr>
            </table></td>
      </tr>
      
   </table>
    
    <div style="height:10px"></div>
</form>
<uc1:footer id="Footer1" runat="server"></uc1:footer>