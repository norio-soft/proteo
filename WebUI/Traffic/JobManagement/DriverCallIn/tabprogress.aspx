<%@ Page language="c#" Inherits="Orchestrator.WebUI.Traffic.JobManagement.tabProgress" Codebehind="tabProgress.aspx.cs" MasterPageFile="~/WizardMasterPage.Master" Title="Haulier Enterprise" %>
<%@ Register TagPrefix="uc1" TagName="callInTabStrip" Src="~/UserControls/DriverCallInTabStrip.ascx" %>
<%@ Register assembly="Orchestrator.WebUI.Dialog" namespace="Orchestrator.WebUI" tagprefix="cc1" %>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
    <link id="lnkNS" runat="server" rel="stylesheet" type="text/css" href="/style/newStyles.css" />
    <link id="lnkNMP" runat="server" rel="stylesheet" type="text/css" href="/style/newMasterpage.css" />    
    
    <script language="javascript" src="/script/scripts.js" type="text/javascript" ></script>
    <script language="javascript" src="/script/popaddress.js" type="text/javascript" ></script>
    
    <script language="javascript" type="text/javascript">
	<!--
		window.focus();
	
	    function showDocketWindow(instructionId)
        {
            var qs = "instructionId=" + instructionId;
            <%=dlgUpdateDockets.ClientID %>_Open(qs);
        }
        
        function showDemurrageWindow(jobid, instructionid, extraid)
        {
            var qs = "jobid=" + jobid + "&instructionid=" + instructionid + "&extraid=" + extraid;
            <%=dlgAddExtra.ClientID %>_Open(qs);
        }
        
        function ShowLoadOrder(instructionID)
        {
            var qs = "instructionID=" + instructionID;
            <%=dlgLoadOrder.ClientID %>_Open(qs);
        }
        
        function viewOrder(orderID)
        {
            var qs = "oID=" + orderID;
            <%=dlgOrder.ClientID %>_Open(qs);
        }
        
        function viewPalletHandling(jobId)
        {
            var qs = "jobId=" + jobId;
            <%=dlgPalletHandling.ClientID %>_Open(qs);
        }
	//-->
	</script>    
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Progress</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
	
	<cc1:Dialog ID="dlgUpdateDockets" runat="server" Mode="Modal" URL="/job/updatedockets.aspx" Width="620" Height="320" Scrollbars="true" ReturnValueExpected="true" AutoPostBack="true" ></cc1:Dialog>
	<cc1:Dialog ID="dlgAddExtra" runat="server" Mode="Modal" URL="/groupage/addupdateextra.aspx" Width="400" Height="280" ReturnValueExpected="true" AutoPostBack="true" ></cc1:Dialog>
	<cc1:Dialog ID="dlgLoadOrder" runat="server" Mode="Modal" URL="/Groupage/LoadOrder.aspx" Width="800" Height="400" Scrollbars="true"></cc1:Dialog>
	<cc1:Dialog ID="dlgOrder" runat="server" Width="1180" Height="900" Mode="Normal" URL="/groupage/manageorder.aspx"></cc1:Dialog>
	<cc1:Dialog ID="dlgPalletHandling" runat="server" URL="/Traffic/JobManagement/AddUpdatePalletHandling.aspx" Width="1115" Height="750" AutoPostBack="true" ReturnValueExpected="true" Mode="Normal" />

	<table id="Table2" runat="server" width="100%" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td class="layoutContentMiddle" valign="top" align="left">
                <div class="layoutContentMiddleInner">
                
                    <div class="buttonbar" >
                        <table width="99%" border="0" cellpadding="0" cellspacing="2">
                            <tr>
                                <td><input type="button" style="width: 75px" value="Details" onclick="javascript:window.location='/job/job.aspx?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" /></td>
                                <td><input type="button" style="width: 75px" value="Coms" onclick="javascript:window.location='/traffic/JobManagement/driverCommunications.aspx?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" /></td>
                                <td><input type="button" style="width: 75px" value="Call-In" onclick="javascript:window.location='/traffic/JobManagement/DriverCallIn/CallIn.aspx?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" /></td>
                                <td><input type="button" style="width: 75px" value="PODs" onclick="javascript:window.location='/traffic/JobManagement/bookingInPODs.aspx?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" /></td>
                                <td><input type="button" style="width: 75px; display: <%= m_job.JobType == Orchestrator.eJobType.Groupage ? "none" : "" %>" value="Pricing" onclick="javascript:window.location='/traffic/JobManagement/pricing2.aspx?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" /></td>
                                <td width="100%" align="right"><iframe marginheight="0" marginwidth="0" frameborder="0" scrolling="no" width="360px" height="22px" src='/traffic/jobManagement/CallInSelector.aspx?JobId=<%=Request.QueryString["JobId"]%>&amp;csid=<%=this.CookieSessionID %>'></iframe></td>
                            </tr>
                        </table>
                    </div>
                    
                    <div>
                        <uc1:callInTabStrip id="CallInTabStrip1" runat="server" SelectedTab="0"></uc1:callInTabStrip>
                        <div style="padding-bottom:10px;"></div>
                    </div>
                    
                    <div>
                        <table cellpadding="1" cellspacing="0" width="100%">
                            <tr id="trEmptyPallets" runat="server" style="display:none;">
                                <td>
                                    <fieldset>
                                        <legend style="margin: 0px; padding: 0px;">Empty Pallets</legend>
                                        <table width="98%" border="0" cellpadding="1" cellspacing="2"> 
			                                <tbody>
				                                <tr>
					                                <td width="30%" valign="top">
						                                <p style="background-color: #363636; padding: 3px; color: white; margin-left: 0px; margin-top: 0px; margin-bottom: 10px;">&nbsp;Trailer Pallets&nbsp;</p>
						                                <div>
						                                    <input type="button" style="width:110px" class="buttonClass" value="Pallet Handling" onclick="javascript:viewPalletHandling(<%=m_jobId %>);" />
						                                </div>
					                                </td>
					                                <td width="70%" valign="top"> 
					                                    <asp:ListView ID="lvTrailerPallets" runat="server">
					                                        <LayoutTemplate>
					                                            <table cellpadding="0" cellspacing="0" style="width:100%;">
					                                                <thead class="HeadingRow">
					                                                    <tr>
					                                                        <td>Resource</td>
					                                                        <td>Pallet Type</td>
					                                                        <td>No of Pallets</td>
					                                                    </tr>
					                                                </thead>
					                                                <tbody>
					                                                    <asp:PlaceHolder ID="itemPlaceHolder" runat="server" />
					                                                </tbody>
					                                            </table>
					                                        </LayoutTemplate>
					                                        <ItemTemplate>
					                                            <tr class="Row">
					                                                <td><%# ((System.Data.DataRowView)Container.DataItem)["Resource"].ToString() %></td>
					                                                <td><%# ((System.Data.DataRowView)Container.DataItem)["PalletType"].ToString() %></td>
					                                                <td><%# ((System.Data.DataRowView)Container.DataItem)["NoOfPallets"].ToString() %></td>
					                                            </tr>
					                                        </ItemTemplate>
					                                        <EmptyDataTemplate>
					                                            There are currently no empty pallets awaiting action on this run.
					                                        </EmptyDataTemplate>
					                                    </asp:ListView>
		                                            </td>
                                                </tr>
                                            </tbody> 
                                        </table>
                                    </fieldset>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <fieldset style="padding: 0px 5px 5px 5px;">
                                        <legend style="margin: 0px; padding: 0px;">Collections</legend>
                                        <asp:label id="lblCollections" runat="server"></asp:label>
                                    </fieldset>
                                    <fieldset style="padding: 0px 5px 5px 5px;">
                                        <legend style="margin: 0px; padding: 0px;">Deliveries</legend>
                                        <asp:label id="lblDeliveries" runat="server"></asp:label>
                                    </fieldset>
                                    <fieldset style="padding: 0px 5px 5px 5px;">
                                        <legend style="margin: 0px; padding: 0px;">Pallet Handling</legend>
                                        <asp:Label id="lblLeavePallets" runat="server"></asp:Label>
                                        <asp:Label id="lblDeHirePallets" runat="server"></asp:Label>
                                    </fieldset>
                                </td>
                            </tr>
                        </table>
                    </div>
                    
                </div>
            </td>
        </tr>
    </table>
    
    <telerik:RadWindowManager ID="rmwtabProgress" runat="server" Modal="true" ShowContentDuringLoad="true" KeepInScreenBounds="true" VisibleStatusbar="false"></telerik:RadWindowManager>

</asp:Content>