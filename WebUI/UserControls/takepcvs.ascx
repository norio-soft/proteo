<%@ Control Language="c#" Inherits="Orchestrator.WebUI.UserControls.TakePCVs" Codebehind="TakePCVs.ascx.cs" %>
<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>
<%@ Register TagPrefix="cc1" Namespace="Orchestrator.WebUI" Assembly="Orchestrator.WebUI.Dialog" %>

<cc1:Dialog ID="dlgScanDocument" runat="server" URL="/scan/wizard/ScanOrUpload.aspx" Height="550" Width="500" Mode="Normal" AutoPostBack="false" ReturnValueExpected="false" />

<asp:Label ID="lblConfirmation" runat="server" CssClass="Confirmation"></asp:Label>

<div style="width:100%; height:5px;"></div>
	
<asp:Panel ID="pnlControlArea" runat="server" Visible="false">
    <div style="margin-bottom:10px;">
        Attach PCVs to Jobs controlled by: <asp:DropDownList ID="cboControlAreas" runat="server" DataTextField="Description" DataValueField="ControlAreaId" AutoPostBack="True"></asp:DropDownList>
    </div>
</asp:Panel>
    
<div style="margin-bottom:20px;">
    <h3><asp:Label ID="lblJobs" runat="server" Text="Available Runs" /></h3>

    <asp:DataGrid ID="dgJobs" Runat="server" Width="100%" AutoGenerateColumns="False" AllowSorting="True" AllowPaging="False" cssclass="DataGridStyle" >
	    <Columns>
		    <asp:BoundColumn DataField="JobId" HeaderText="Run Id" SortExpression="JobId"></asp:BoundColumn>
		    <asp:BoundColumn DataField="LoadNo" HeaderText="Load Number" SortExpression="LoadNo"></asp:BoundColumn>
		    <asp:BoundColumn DataField="Dockets" HeaderText="Docket Number(s)" SortExpression="Dockets"></asp:BoundColumn>
		    <asp:BoundColumn DataField="Client" HeaderText="Client" SortExpression="Client"></asp:BoundColumn>
		    <asp:BoundColumn HeaderText="Delivery Point" DataField="DeliveryPoint" SortExpression="DeliveryPoint"></asp:BoundColumn>
		    <asp:BoundColumn HeaderText="Delivery Point Id" DataField="DeliveryPointId" Visible="false"></asp:BoundColumn>
		    <asp:TemplateColumn HeaderText="For Delivery At" SortExpression="BookedDateTime" ItemStyle-Wrap="False">
			    <ItemTemplate>
				    <%# DataBinder.Eval(Container.DataItem, "BookedDateTime", "{0:dd/MM/yy}") %> <%# (Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "IsAnyTime"))) ? "Anytime" : DataBinder.Eval(Container.DataItem, "BookedDateTime", "{0:HH:mm}") %>
			    </ItemTemplate>
		    </asp:TemplateColumn>
		    <asp:ButtonColumn HeaderText="Attach PCVs" CommandName="AttachPCVs" ButtonType="PushButton" Text="Attach to this Job" ></asp:ButtonColumn>
	    </Columns>
	    <AlternatingItemStyle CssClass="DataGridListItemAlt"></AlternatingItemStyle>
	    <ItemStyle CssClass="DataGridListItem" VerticalAlign="Top"></ItemStyle>
	    <HeaderStyle CssClass="DataGridListHead" Height="25px"></HeaderStyle>
	    <PagerStyle CssClass="DataGridListPagerStyle" Height="10"></PagerStyle>
    </asp:DataGrid>
    
    <asp:Panel ID="pnlAgreedPCVRedemptions" runat="server">
        <div style="float:left; width:500px;">
            <h3>Job Details</h3>
            <table style="width:100%;">
                <tr>
                    <td class="formCellLabel" style="width:27%;">JobID</td>
                    <td class="formCellField"><asp:Label ID="lblJobID" runat="server" /></td>
                </tr>
                <tr>
                    <td class="formCellLabel">Load Number</td>
                    <td class="formCellField"><asp:Label ID="lblLoadNo" runat="server" /></td>
                </tr>
                <tr>
                    <td class="formCellLabel" valign="top">Docket Number(s)</td>
                    <td class="formCellField"><asp:Label ID="lblDockets" runat="server" /></td>
                </tr>
                <tr>
                    <td class="formCellLabel">Client</td>
                    <td class="formCellField"><asp:Label ID="lblClient" runat="server" /></td>
                </tr>
                <tr>
                    <td class="formCellLabel">Delivery Point</td>
                    <td class="formCellField"><asp:Label ID="lblDeliveryPoint" runat="server" /></td>
                </tr>
                <tr>
                    <td class="formCellLabel">For Delivery At</td>
                    <td class="formCellField"><asp:Label ID="lblBookedDateTime" runat="server" /></td>
                </tr>
            </table>
        </div>
    
        <div style="float:left;">
            <div style="width:500px;">
                <h3>Agreed PCV Collections</h3>
                <asp:ListView ID="lvPCVRedemptionAgreed" runat="server" >
                    <LayoutTemplate>
                        <table cellpadding="0" cellspacing="0" style="width:100%;">
                            <thead>
                                <tr class="HeadingRow">
                                    <th>Voucher No</th>
                                    <th>Client Contact</th>
                                    <th>Agreed Date</th>
                                    <th>User</th>
                                    <th>Last Updated</th>
                                </tr>
                            </thead>
                            <tbody>
                                <asp:PlaceHolder ID="itemPlaceHolder" runat="server" />
                            </tbody>
                        </table>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <tr class="Row">
                            <td><%#((System.Data.DataRowView)Container.DataItem)["VoucherNo"].ToString()%></td>
                            <td><%#((System.Data.DataRowView)Container.DataItem)["ClientContact"].ToString()%></td>
                            <td><%#((DateTime)((System.Data.DataRowView)Container.DataItem)["AgreedDate"]).ToString("dd/MM/yy HH:mm")%></td>
                            <td><%#((System.Data.DataRowView)Container.DataItem)["User"].ToString()%></td>
                            <td><%#((DateTime)((System.Data.DataRowView)Container.DataItem)["LastUpdated"]).ToString("dd/MM/yy HH:mm") %></td>
                        </tr>
                    </ItemTemplate>
                    <EmptyDataTemplate>
                        <tr>
                            <td colspan="5">
                                There are currently no recorded agreements.
                            </td>
                        </tr>
                    </EmptyDataTemplate>
                </asp:ListView>
            </div>
        
            <div style="width:500px; margin-top:10px;">
                <fieldset>
                    <legend>Agreed Contact Details</legend>
                    <table style="width:100%;">
                        <tr>
                            <td class="formCellLabel" style="width:40%;">Client Contact</td>
                            <td class="formCellField">
                                <asp:TextBox ID="txtClientContact" runat="server" MaxLength="100" Width="136" ValidationGroup="grpRedemptionAgreedDetails" />
                                <asp:RequiredFieldValidator ID="rfvClientContact" runat="server" ControlToValidate="txtClientContact" ValidationGroup="grpRedemptionAgreedDetails" >
                                    <img src="/images/Error.gif" alt="Please supply a contact name." />
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Agreed Redemption Date</td>
                            <td class="formCellField">
                                <table cellpadding="0" cellspacing="0">
                                    <tr>
                                        <td>
                                            <telerik:RadDateInput ID="rdiSlotDate" runat="server" DateFormat="dd/MM/yy" Width="65" ValidationGroup="grpRedemptionAgreedDetails" />
                                            <asp:RequiredFieldValidator ID="rfvSlotDate" runat="server" ControlToValidate="rdiSlotDate" ValidationGroup="grpRedemptionAgreedDetails" >
                                                <img src="/images/Error.gif" alt="Please enter the agreed date." />
                                            </asp:RequiredFieldValidator>
                                        </td>
                                        <td>
                                            <telerik:RadDateInput ID="rdiSlotTime" runat="server" DateFormat="HH:mm" Width="45" ValidationGroup="grpRedemptionAgreedDetails" />
                                            <asp:RequiredFieldValidator ID="rfvSlotTime" runat="server" ControlToValidate="rdiSlotTime" ValidationGroup="grpRedemptionAgreedDetails" >
                                                <img src="../images/Error.gif" alt="Please enter the agreed time." />
                                            </asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </div>
        </div>
        
        <div class="clearDiv" style="margin-bottom:20px;"></div>
    </asp:Panel>
</div>

<div style="margin:10px 0px 10px 0px;">
    <asp:Panel ID="pnlPCVAvailable" runat="server">
        <h3>Available PCVs</h3>
        <asp:DataGrid ID="dgPCVs" runat="server" Width="100%" AutoGenerateColumns="False" AllowSorting="True" AllowPaging="False" >
            <Columns>
                <asp:BoundColumn DataField="PCVId" HeaderText="PCV Id" Visible="false"></asp:BoundColumn>
                <asp:TemplateColumn HeaderText="Voucher Number">
                    <ItemTemplate>
                        <a href='javascript:OpenPCVWindowForEdit(<%# DataBinder.Eval(Container.DataItem, "ScannedFormId") %>)'><%# DataBinder.Eval(Container.DataItem, "VoucherNo") %></a>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:BoundColumn DataField="DeliveryPoint" HeaderText="Issued At" SortExpression="DeliveryPoint"></asp:BoundColumn>
                <asp:BoundColumn DataField="DateOfIssue" HeaderText="Issued On" SortExpression="DateOfIssue" DataFormatString="{0:dd/MM/yy HH:mm}" ItemStyle-Wrap="False"></asp:BoundColumn>
                <asp:BoundColumn DataField="NoOfPalletsOutstanding" HeaderText="Pallets Outstanding" SortExpression="NoOfPalletsOutstanding" ItemStyle-HorizontalAlign="Right"></asp:BoundColumn>
                <asp:BoundColumn DataField="PCVStatus" HeaderText="Status" SortExpression="PCVStatus"></asp:BoundColumn>
                <asp:BoundColumn DataField="PCVRedemptionStatus" HeaderText="Redemption Status" SortExpression="PCVRedemptionStatus"></asp:BoundColumn>
                <asp:BoundColumn DataField="PCVReasonForIssuingStatus" HeaderText="Reason for Issuing" SortExpression="PCVReasonForIssuingStatus"></asp:BoundColumn>
                <asp:BoundColumn DataField="NoOfSignings" HeaderText="Number of Signing" SortExpression="NoOfSignings"></asp:BoundColumn>
                <asp:TemplateColumn HeaderText="Attach PCV">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkTakePCV" Runat="server"></asp:CheckBox>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Update Agreement Details">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkUpdatePCV" Runat="server"></asp:CheckBox>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
            <AlternatingItemStyle CssClass="DataGridListItemAlt"></AlternatingItemStyle>
            <ItemStyle CssClass="DataGridListItem" VerticalAlign="Top"></ItemStyle>
            <HeaderStyle CssClass="DataGridListHead" VerticalAlign="Top"></HeaderStyle>
            <PagerStyle CssClass="DataGridListPagerStyle" Height="10"></PagerStyle>
        </asp:DataGrid>

        <div style="margin-bottom:10px; margin-right:20px; float:right; ">
            Number of Pallets represented by selected PCVS:&nbsp;
            <asp:Label ID="lblPalletCount" runat="server" Font-Bold="true"></asp:Label>
        </div>
        
        <div class="clearDiv"></div>
    </asp:Panel>
</div>
	
<div class="buttonbar">
	<nfvc:NoFormValButton ID="btnGenerateRedemptionForm" Runat="server" Text="Redemption Form"></nfvc:NoFormValButton>
	<nfvc:NoFormValButton ID="btnTakeOnJob" Runat="server" Text="Update PCVs" ValidationGroup="grpRedemptionAgreedDetails" CausesValidation="true" ></nfvc:NoFormValButton>
	<asp:Button ID="btnRefresh" runat="server" Text="Refresh" CausesValidation="false" />
</div>
		
<uc1:ReportViewer id="reportViewer" runat="server" Visible="False" ViewerWidth="100%" ViewerHeight="800"></uc1:ReportViewer>

<script language="javascript" type="text/javascript">
<!--
    var lastHighlightedRow = "";
    var lastHighlightedRowColour = "";
    
    function rdi_OnClientDateChanged(sender, eventArgs)
    {
        CheckContactDetails();
    }
    
    function CheckContactDetails(){
        var txtClientContact = $("input:text[id*='txtClientContact']");
        var rdiSD = $("input[id*='rdiSlotDate']");
        var rdiST = $("input[id*='rdiSlotTime']");
        
        if(txtClientContact.length > 0 && rdiSD.length > 2 && rdiST.length > 2)
        {
            var rdiSlotDate = $find(rdiSD[1].id);
            var rdiSlotTime = $find(rdiST[1].id);
            
            var rfvSlotDate = $("span[id*='rfvSlotDate']");
            var rfvSlotTime = $("span[id*='rfvSlotTime']");
            var rfvClientContact = $("span[id*='rfvClientContact']");
            
            if( txtClientContact.val() == "" && rdiSlotDate.get_value() == "" && rdiSlotTime.get_value() == "" )
            { 
                ValidatorEnable(rfvClientContact[0], false);
                ValidatorEnable(rfvSlotDate[0], false);
                ValidatorEnable(rfvSlotTime[0], false);            
            }
            else
            {
                ValidatorEnable(rfvClientContact[0], true);
                ValidatorEnable(rfvSlotDate[0], true);
                ValidatorEnable(rfvSlotTime[0], true);
            }
        }
    }

    function HighlightRow(row) {
        var rowElement;

        if (lastHighlightedRow != "") {
            rowElement = document.getElementById(lastHighlightedRow);
            rowElement.style.backgroundColor = lastHighlightedRowColour;
        }

        rowElement = document.getElementById(row);
        lastHighlightedRow = row;
        lastHighlightedRowColour = rowElement.style.backgroundColor;
        rowElement.style.backgroundColor = 'yellow';
    }

    function HandleCheckboxClick(checkBox, palletCount) {
        var lblPalletCount = document.getElementById("<%=lblPalletCount.ClientID%>");

        if (lblPalletCount != null) {
            var currentPalletCount = parseInt(lblPalletCount.innerText);

            if (checkBox.checked)
                currentPalletCount = currentPalletCount + palletCount;
            else
                currentPalletCount = currentPalletCount - palletCount;

            lblPalletCount.innerText = currentPalletCount;
        }
    }

    function OpenPCVWindowForEdit(scannedFormId) {
        var qs = "ScannedFormTypeId=1&ScannedFormId=" + scannedFormId;
        <%=dlgScanDocument.ClientID %>_Open(qs);
    }
//-->
</script>