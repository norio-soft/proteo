<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/default_tableless.master" Inherits="Orchestrator.WebUI.Traffic.Manifest" Codebehind="Manifest.aspx.cs" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>
<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <asp:Label id="lblSubTitle" runat="server" Text="Produce a manifest using this page."></asp:Label>
    
    	<fieldset>
	    	<legend><strong>Produce a Manifest</strong></legend>

	    	<table width="100%" border="0" cellpadding="1" cellspacing="0">
	    	    <tr>
	    	        <td width="45%">Resource:
	    	            <telerik:RadComboBox ID="cboResource" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                            MarkFirstMatch="true" AllowCustomText="False"
                            ShowMoreResultsBox="false" Width="355px" Height="300px" OnClientSelectedIndexChanged="OnClientSelectedIndexChanged">
                        </telerik:RadComboBox>
	    	        </td>
	    	        <td width="5%"></td>
	    	        <td width="50%">Sub-Contractor:
                        <telerik:RadComboBox ID="cboSubContractor" runat="server" 
                            EnableLoadOnDemand="true" ItemRequestTimeout="500"
                            MarkFirstMatch="true" AllowCustomText="False"
                            ShowMoreResultsBox="false" Width="355px" Height="300px" OnClientSelectedIndexChanged="OnClientSelectedIndexChanged">
                        </telerik:RadComboBox>
                    </td>
	    	    </tr>
	    	    <tr>
                    <td colspan="2">
                        <br />
                        <table border="0" cellpadding="0" cellspacing="2">
                            <tr>
                                <td>Display legs planned to start after:</td>
	                            <td valign="top"><telerik:RadDateInput id="dteStartDate" runat="server" Width="60px" dateformat="dd/MM/yy"></telerik:RadDateInput><%--</td>
	                            <td valign="top">--%> <telerik:RadDateInput id="dteStartTime" runat="server" Width="45px" dateformat="HH:mm"></telerik:RadDateInput></td>
	                            <td valign="top">
		                            <asp:RequiredFieldValidator id="rfvStartDate" runat="server" ErrorMessage="Please enter a valid start date" ControlToValidate="dteStartDate" Display="Dynamic" EnableClientScript="True"><img src="<%= Page.ResolveUrl("~/images/Error.gif") %>" height="16" width="16" alt="Please enter a valid start date" /></asp:RequiredFieldValidator>
		                            <asp:RequiredFieldValidator id="rfvStartTime" runat="server" ErrorMessage="Please enter a valid start time" ControlToValidate="dteStartTime" Display="Dynamic" EnableClientScript="True"><img src="<%= Page.ResolveUrl("~/images/Error.gif") %>" height="16" width="16" alt="Please enter a valid start time" /></asp:RequiredFieldValidator>
	                            </td>
                            </tr>
                            <tr>
                                <td>Add additional blank rows:</td>
                                <td valign="top" colspan="3"><asp:TextBox id="txtExtraRowCount" runat="server" Text="5" Width="40px"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td>Special Instructions:</td>
                                <td valign="top" colspan="5">
                                    <asp:TextBox id="txtSpecialInstructions" 
                                        runat="server" MaxLength="240" Width="700px"></asp:TextBox></td>
	    	                </tr>
                        </table>
                    </td>
                </tr>
                <tr>
	    	        <td valign="top">
	    	            <asp:CheckBox id="chkIncludePlannedWork" runat="server" Text="Include planned work" Text-Align="Right" checked="true" />
	    	            <asp:CheckBox id="chkIncludeFullAddresses" runat="server" Text="Show Full Addresses" Text-Align="Right" checked="true" />
	    	        </td>
	    	    </tr>
	    	</table>
	    	<div class="buttonbar">
	    	    <nfvc:NoFormValButton id="btnGetJobs" runat="server" Text="Get Jobs" />
	    	    <asp:Button id="btnReset" runat="server" Text="Reset" CausesValidation="False" />
	    	</div>
	    	
	    	<asp:Panel id="pnlMatchedJobs" runat="server" Visible="false">
	    	    <asp:Label id="lblLegCount" runat="server"></asp:Label>
		        <asp:GridView ID="gvManifest" runat="server" AllowSorting="false" GridLines="vertical" AutoGenerateColumns="false" CssClass="Grid" Width="100%">
                    <HeaderStyle CssClass="HeadingRow" Height="25" VerticalAlign="middle"/>
                    <RowStyle Height="20" CssClass="Row" />
                    <AlternatingRowStyle Height="20" BackColor="WhiteSmoke" />
                    <SelectedRowStyle Height="20" CssClass="SelectedRow" />
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <input id="chkAll" onclick="javascript: SelectAllCheckboxes(this);" runat="server" type="checkbox" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox id="chkSelect" runat="server"></asp:CheckBox>
                                <asp:HiddenField id="hidJobId" runat="server"></asp:HiddenField>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Job Id">
                            <ItemTemplate>
                                <a id="lnkItem" runat="server"><%# ((System.Data.DataRowView)Container.DataItem)["JobID"].ToString()%></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Customer">
                            <ItemTemplate>
                                <asp:Label id="lblCustomer" runat="server"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Start From">
                            <ItemTemplate>
                                <asp:Label id="lblStartFrom" runat="server"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Start At">
                            <ItemTemplate>
                                <asp:Label id="lblStartAt" runat="server"></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Destination" HeaderText="Destination" />
                        <asp:BoundField DataField="ArriveAtDisplay" HeaderText="Arrive At"/>
                        <asp:BoundField DataField="Items" HeaderText="Items" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="Weight" HeaderText="Weight" ItemStyle-HorizontalAlign="Right" DataFormatString="{0:F0}" HtmlEncode="false" />
                        <asp:BoundField DataField="Reference" HeaderText="Reference" ItemStyle-HorizontalAlign="Right" />
                        <asp:BoundField DataField="Comments" HeaderText="Comments" />
                        <asp:BoundField DataField="DriverName" HeaderText="Driver" />
                        <asp:BoundField DataField="RegNo" HeaderText="Vehicle" />
                        <asp:BoundField DataField="TrailerRef" HeaderText="Trailer" />
                        
                    </Columns>
                </asp:GridView>
                <div class="buttonbar">
	    	        <nfvc:NoFormValButton id="btnDisplayManifest" runat="server" Text="Display Manifest" CausesValidation="False" />
                    <asp:Button id="btnProducePILs" runat="server" Text="Produce PILs" CausesValidation="False" />
	    	    </div>
	    	</asp:Panel>

		    <uc1:ReportViewer id="reportViewer" runat="server" Visible="False"></uc1:ReportViewer>
		    
            <telerik:RadWindowManager ID="rmwAdmin" runat="server" Modal="true" ShowContentDuringLoad="true" KeepInScreenBounds="true" VisibleStatusbar="false"></telerik:RadWindowManager>
        </fieldset>
    
    <script language="javascript" type="text/javascript">
    <!--
    function SelectAllCheckboxes(spanChk) {
        var oItem = spanChk.children;
        var theBox = (spanChk.type == "checkbox") ? spanChk : spanChk.children.item[0];
        xState = theBox.checked;

        elm = theBox.form.elements;

        for (i = 0; i < elm.length; i++)
            if (elm[i].type == "checkbox" && elm[i].id != theBox.id) {
                if (elm[i].name.indexOf("chkSelect") > -1) {
                    if (elm[i].checked != xState)
                        elm[i].click();
                }
            }
    }

    function viewOrderProfile(orderID) {
        var oManager = GetRadWindowManager();
        var url = "/Groupage/ManageOrder.aspx?wiz=true&oID=" + orderID;

        oManager.SetTitle("Add/Update Order");
        oManager.SetHeight(900);
        oManager.SetWidth(1180);
        oManager.Open(url, null);
    }

    function viewJobDetails(jobID) {
        var url = "../job/job.aspx?jobId=" + jobID + getCSID();
        window.open(url);
    }

    function OnClientSelectedIndexChanged(sender, eventArgs) {
        document.getElementById("<%= txtSpecialInstructions.ClientID %>").value = "";
        }
        //-->
    </script>
</asp:Content>