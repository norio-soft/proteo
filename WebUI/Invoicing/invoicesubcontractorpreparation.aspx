<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>

<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Register TagPrefix="componentart" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>
<%@ Register TagPrefix="cs" Namespace="Codesummit" Assembly="WebModalControls" %>
<%@ Page language="c#" Inherits="Orchestrator.WebUI.Invoicing.InvoiceSubContractorPreparation" Codebehind="InvoiceSubContractorPreparation.aspx.cs" %>
<uc1:header id="Header1" title="Invoice Sub Contractor Preparation" XMLPath="InvoicingContextMenu.xml"
	SubTitle="The Invoice Sub Contractor Preparation summary is below." ShowLeftMenu="false"
	runat="server"></uc1:header>
<FORM id="Form1" method="post" runat="server">
	<asp:Label id="lblJobCount" runat="server" Width="100%" Font-Size="Medium"></asp:Label>
	<asp:label id="lblDetails" runat="server" Width="100%"></asp:label>
	<uc1:infringementDisplay id="infringementDisplay" runat="server" Visible="false"></uc1:infringementDisplay>	
	<input type="hidden" id="hidJobCount" runat="server" value="0"/>
	<input type="hidden" id="hidJobTotal" runat="server" value="0"/>
	<input type="hidden" id="hidSelectedJobs" runat="server" value="" />
	
	<asp:panel id="pnlFilter" runat="server" visible="true">
		<TABLE id="Table3" width="100%">
			<TBODY>
				<TR>
					<TD>
						<FIELDSET>
							<P><LEGEND><STRONG>Client</STRONG></LEGEND></P>
							<P>
								<TABLE>
									<TR>
										<TD vAlign="top" width="180">
											<asp:Label id="lblClient" Text="Sub Contractor" Runat="server"></asp:Label></TD>
										<TD width="360">
										    <telerik:RadComboBox ID="cboSubContractor" runat="server" RadControlsDir="~/script/RadControls/" EnableLoadOnDemand="true" ShowMoreResultsBox="true" MarkFirstMatch="true" Skin="WindowsXP" ItemRequestTimeout="500" Width="355px" ></telerik:RadComboBox>
										<TD>
										<td width="20">
										    
										</td>
									</TR>
									<tr>
										<td>
											<asp:label id="lblInvoiceNumber" visible="false" runat="server" text="Sub-Contractor's Invoice Number"></asp:label> 
										</td>
										<td>
											<asp:textbox id="txtInvoiceNumber" runat="server" visible="false"></asp:textbox>
										</td>
										<td>
										    <asp:RequiredFieldValidator id="rfvAssign" runat="server" ErrorMessage="Please enter a invoice number to assign." ControlToValidate="txtInvoiceNumber" Display="Dynamic"><img src="../images/Error.gif" height='16' width='16' title='Please enter an invoice number to assign.'></asp:RequiredFieldValidator>
										</td>
									</tr>
									<tr>
										<td>
											<asp:label id="lblAssignUpdate" visible="true" runat="server" text=""></asp:label> 
										</td>
										<td>
											<nfvc:noformvalbutton id="btnAssign" visible="false" runat="server" text="Assign Invoice Number" ServerClick="btnAssign_Click"></nfvc:noformvalbutton>
										</td>
									</tr>
								</TABLE>
							</P>
						</FIELDSET>
					</TD>
			    </TR>
			</TBODY>
		</TABLE>
    </asp:panel>
    <asp:panel id="pnlDate" runat="server">
		<FIELDSET>
			<P><LEGEND><STRONG>Date</STRONG></LEGEND></P>
			<TABLE>
				<TR>
					<TD>Date From</TD>
					<TD><telerik:RadDateInput id="dteStartDate" runat="server" dateformat="dd/MM/yy"></telerik:RadDateInput></TD>
				</TR>
				<TR>
					<TD>Date To</TD>
					<TD><telerik:RadDateInput id="dteEndDate" runat="server" dateformat="dd/MM/yy"></telerik:RadDateInput></TD>
				</TR>
			</TABLE>
		</FIELDSET>
	</asp:panel>
	<DIV class="buttonbar">
		<nfvc:noformvalbutton id="btnFilter1" runat="server" text="Generate Jobs To Invoice" NoFormValList="rfvAssign"></nfvc:noformvalbutton>
		<nfvc:noformvalbutton id="btnCreate1" visible="False" runat="server" text="Create Invoice"></nfvc:noformvalbutton>
		<nfvc:noformvalbutton id="btnClear1" runat="server" text="Clear" ServerClick="btnClear_Click"></nfvc:noformvalbutton>
		<nfvc:noformvalbutton id="btnExport" visible="false" runat="server" text="Export to CSV"></nfvc:noformvalbutton>
	</DIV>
		
	<asp:panel id="pnlSubContractor" runat="server" Visible="False">
		<asp:label id="lblSubContractorNote" runat="server" visible="false" cssclass="confirmation" text=""></asp:label>

        <cs:webmodalanchor id="MyClientSideAnchor" title="Sub-Contract Job" runat="server" clientsidesupport="true"
            windowwidth="580" windowheight="532" scrolling="false" url="addupdatedriver.aspx" handledevent="onclick"
            linkedcontrolid="dvJobs"></cs:webmodalanchor>
		
		<asp:GridView ID="dvJobs" runat="server" AllowSorting="true" gridlines="vertical" autogeneratecolumns="false" cssclass="Grid" width="100%" visible="true">
            <headerstyle cssclass="HeadingRow" height="25" verticalalign="middle"/>
            <rowStyle height="20" cssclass="Row" />
            <AlternatingRowStyle height="20" backcolor="WhiteSmoke" />
            <SelectedRowStyle height="20" cssclass="SelectedRow" />
            <columns>
                <asp:TemplateField headerText="">
                    <HeaderTemplate>
                        <input id="chkAll" onclick="javascript:SelectAllCheckboxes(this);" runat="server" type="checkbox" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:checkbox id="chkSelect" runat="server"></asp:checkbox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField headertext="Job Id">
                    <ItemTemplate>
                        <input id="hidJobId" type=hidden value='<%# DataBinder.Eval(Container.DataItem, "JobId") %>' name=hidJobId runat="server" />
                        <a id="lnkJob" href="javascript:openDialogWithScrollbars('~/traffic/JobManagement/pricing2.aspx?wiz=true&jobId=<%# Eval("JobId").ToString() %>'+ getCSID(), '800','600');"><%# Eval("JobId").ToString() %></a>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="JobType" HeaderText="Job Text" />
                <asp:BoundField DataField="JobState" HeaderText="Job State" />
                <asp:TemplateField HeaderText="Rate">
                    <ItemTemplate>
                        <a id="lnkRate" runat="server" title="Change Rate"><%# ((decimal) Eval("Rate")).ToString("C") %></a>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="SubContractor" HeaderText="Sub Contractor" />
                <asp:TemplateField HeaderText="Received Invoice">
                    <ItemTemplate>
                        <%# Eval("InvoiceReceived").ToString().ToLower() == "true" ? "Yes" : "No" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Date Received">
                    <ItemTemplate>
                        <%# Eval("InvoiceReceived").ToString().ToLower() == "true" ? ((DateTime) Eval("DateReceived")).ToString("dd/MM/yy") : "No Date" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Invoice Number">
                    <ItemTemplate>
                        <%# Eval("InvoiceReceived").ToString().ToLower() == "true" ? Eval("ClientInvoiceNumber").ToString() : "No Invoice Number" %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="CompleteDate" HeaderText="Complete Date" DataFormatString="{0:dd/MM HH:mm}" HtmlEncode="False" />
                <asp:BoundField DataField="ChargeAmount" HeaderText="Charge Amount" DataFormatString="{0:C}" HtmlEncode="False" />
                <asp:BoundField DataField="Client" HeaderText="Customer" />
            </columns>
        </asp:GridView>
	</asp:panel>
	
	<DIV class="buttonbar">
		<nfvc:noformvalbutton id="btnFilter" runat="server" text="Generate Jobs To Invoice" NoFormValList="rfvAssign"></nfvc:noformvalbutton>
		<nfvc:noformvalbutton id="btnCreate" visible="False" runat="server" text="Create Invoice"></nfvc:noformvalbutton>
		<nfvc:noformvalbutton id="btnClear" runat="server" text="Clear"></nfvc:noformvalbutton>
    </DIV>
<uc1:footer id="Footer1" runat="server"></uc1:footer>
</FORM>
<script language="javascript">
<!--
	var message = "You have selected "
	var message1 = " job(s), and the total amount is £"
	var hidSelectedJobs = document.getElementById('<%=hidSelectedJobs.ClientID %>');
    var hidJobCount = document.getElementById('<%=hidJobCount.ClientID %>');
	var hidJobTotal = document.getElementById('<%=hidJobTotal.ClientID %>');
	
     //Window Code
    function _showModalDialog(url, width, height, windowTitle)
    {
        MyClientSideAnchor.WindowHeight= height + "px";
        MyClientSideAnchor.WindowWidth= width + "px";
        
        MyClientSideAnchor.URL = url;
        MyClientSideAnchor.Title = windowTitle;
        
        var outputData = "";
        
        if (MyClientSideAnchor.Open() == true)
        {
            outputData = unescape(MyClientSideAnchor.OutputData);
        }

        return outputData;	        
    }

    function openSubContractWindow(jobId, lastUpdateDate, chkSelectId, lnkRateId)
    {
        var url = "../Traffic/SubContract.aspx?jobId=" + jobId + "&lastUpdateDate=" + lastUpdateDate + "&isUpdate=true";

        var outputData = _showModalDialog(url, 400, 320, 'Sub-Contract Job');
        
        if (outputData != "")
        {
            var lnkRate = document.getElementById(lnkRateId);
            var chkSelect = document.getElementById(chkSelectId);

            var oldRateStr = lnkRate.innerHTML.substr(1); // strip off the £ symbol
            var oldRate = parseFloat(oldRateStr);
            
            var prefix = "<subContract+rate=\"";
            var suffix = "\"+/>";
            var newRate = parseFloat(outputData.substr(prefix.length, outputData.length - prefix.length - suffix.length));
            
            if (newRate.toFixed)
            {
                lnkRate.innerHTML = "£" + newRate.toFixed(2);
            }
            else
            {
                // Number.toFixed requires JavaScript 1.5... degrade gracefully if this is not present
                lnkRate.innerHTML = "£" + newRate;
            }
                        
            if (chkSelect.checked == true)
            {
                hidJobTotal.value = parseFloat(hidJobTotal.value) + newRate - oldRate;
                updateDetailsLabel();
            }
        }
    }

    function SelectAllCheckboxes(spanChk)
    {
        var oItem = spanChk.children;
        var theBox= (spanChk.type=="checkbox") ? spanChk : spanChk.children.item[0];
        xState=theBox.checked;

        elm=theBox.form.elements;

        for(i=0;i<elm.length;i++)
            if(elm[i].type=="checkbox" && elm[i].id!=theBox.id)
            {
                if (elm[i].name.indexOf("dvJobs")> -1)
                {
                    //elm[i].click();
                    if(elm[i].checked!=xState)
                        elm[i].click();
                    //elm[i].checked=xState;
                }
            }
    }
 
    function GetCheckedItems(jobId, lnkRateId, checkBoxElement)
    {
        var count = 0; 
		var total = 0;

        var rateStr = document.getElementById(lnkRateId).innerHTML.substr(1); // strip off the £ symbol
        var rate = parseFloat(rateStr);
        
        // Check the current CheckBox that we have just dealt with 
        if (checkBoxElement.checked)
        {
            // We've selected this item
            hidJobCount.value++;
	        hidJobTotal.value = parseFloat(hidJobTotal.value) + rate;
            hidSelectedJobs.value = hidSelectedJobs.value + jobId + ",";
        }
        else
        {
            // We've deselected this item
            hidJobCount.value--;
			hidJobTotal.value = parseFloat(hidJobTotal.value) - rate;
			
		
			if (hidSelectedJobs.value.substr(0, (new String(jobId).length) + 1) == jobId + ",")
			{
			    if (hidSelectedJobs.value == (jobId + ","))
			    {
			        hidSelectedJobs.value = "";
			    }
			    else
			    {
			        hidSelectedJobs.value = hidSelectedJobs.value.substr((new String(jobId).length) + 1);
			    }
		    }
			else
			{
			    var location = hidSelectedJobs.value.indexOf("," + jobId + ",");
                hidSelectedJobs.value = hidSelectedJobs.value.substr(0, location + 1) + hidSelectedJobs.value.substr(location + ("," + jobId + ",").length);
			}
        }

        updateDetailsLabel()
        
        return true;
    }

    function updateDetailsLabel()
    {
        hidJobTotal.value  = Math.round(hidJobTotal.value * 100)/100;

		var text = message + hidJobCount.value + message1 + hidJobTotal.value;
	
		setLabelText("lblDetails", text);        
    }
    
    function setLabelText(ID, Text)
  	{
  	  	document.getElementById(ID).innerHTML = Text;
  	}
	-->
</script>