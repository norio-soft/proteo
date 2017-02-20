<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="True" CodeBehind="AddUnattachedPCV.aspx.cs" Inherits="Orchestrator.WebUI.Pallet.AddUnattachedPCV" %>
<%@ Register TagPrefix="p1" TagName="Point" Src="~/UserControls/point.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="Orchestrator.WebUI" Assembly="Orchestrator.WebUI.Dialog" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Add Unattached PCV</h1></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <cc1:Dialog ID="dlgPCVAdded" runat="server" URL="/pcv/pcvcreatedconf.aspx" Height="125" Width="350" Mode="Modal" AutoPostBack="false" ReturnValueExpected="false" />
    <cc1:Dialog ID="dlgScanDocument" runat="server" URL="/scan/wizard/ScanOrUpload.aspx" Height="550" Width="500" Mode="Modal" AutoPostBack="false" ReturnValueExpected="false" />
    
    <div>
    
        <div style="float:left;">
            <div style="width:500px;">
                
                <fieldset>
                    <table>
                        <tr>
                            <td class="formCellLabel">Client</td>
                            <td class="formCellField">
                                <telerik:RadComboBox ID="rcbClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500" AutoPostBack="true" MarkFirstMatch="true" 
                                                     Height="300px" Overlay="true" ShowMoreResultsBox="false" Width="350px" AllowCustomText="True"
                                                     DataTextField="OrganisationName" DataValueField="IdentityID" />
                            </td>
                            <td>
                                <asp:RequiredFieldValidator ID="rfvClient" runat="server" ControlToValidate="rcbClient" Display="Dynamic" ValidationGroup="grpAddUnAttachedPCV" >
                                    <img src="/images/Error.gif" alt="Please select a client" />
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">PalletType</td>
                            <td class="formCellField">
                                <telerik:RadComboBox ID="rcbPalletType" runat="server"  Height="150px" Overlay="true" 
                                                     ShowMoreResultsBox="false" Width="350px" DataTextField="Description" DataValueField="PalletTypeID" />
                            </td>
                            <td>
                                <asp:RequiredFieldValidator ID="rfvPalletType" runat="server" ControlToValidate="rcbPalletType" Display="Dynamic" ValidationGroup="grpAddUnAttachedPCV" >
                                    <img src="/images/Error.gif" alt="Please select a PalletType." />
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Voucher No</td>
                            <td class="formCellField">
                                <telerik:RadNumericTextBox ID="rntVoucherNo" runat="server" Type="Number" MinValue="0"   NumberFormat-DecimalDigits="0" />
                            </td>
                            <td>
                                <asp:RequiredFieldValidator ID="rfvVoucherNo" runat="server" ControlToValidate="rntVoucherNo" Display="Dynamic" ValidationGroup="grpAddUnAttachedPCV">
                                    <img src="/images/Error.gif" alt="Please enter a PCV voucher number." />
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">No of Pallets</td>
                            <td class="formCellField">
                                <telerik:RadNumericTextBox ID="rntNoOfPallets" runat="server" Type="Number" MinValue="0"   NumberFormat-DecimalDigits="0" />
                            </td>
                            <td>
                                <asp:RequiredFieldValidator ID="rfvNoOfPallets" runat="server" ControlToValidate="rntNoOfPallets" Display="Dynamic" ValidationGroup="grpAddUnAttachedPCV" >
                                    <img src="/images/Error.gif" alt="Please enter the number of pallets." />
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Point Issued</td>
                            <td class="formCellField">
                                <p1:Point runat="server" ID="ucIssuedPoint" ShowFullAddress="true" CanClearPoint="true"
                                        CanUpdatePoint="true" ShowPointOwner="true" Visible="true" IsDepotVisible="false" />
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Date of Issue</td>
                            <td class="formCellField">
                                <telerik:RadDatePicker ID="rdiDate" runat="server" Width="100">
                                <DateInput runat="server"
                                DateFormat="dd/MM/yy">
                                </DateInput>
                                </telerik:RadDatePicker>
                            </td>
                            <td>
                                <asp:RequiredFieldValidator ID="rfvDate" runat="server" ControlToValidate="rdiDate" Display="Dynamic" ValidationGroup="grpAddUnAttachedPCV" >
                                    <img src="/images/Error.gif" alt="Please enter an issue date" />
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">No of Signings</td>
                            <td class="formCellField">
                                <telerik:RadComboBox ID="rcbNoOfSignings" runat="server" CausesValidation="false" AutoPostBack="false" Height="75px" 
                                                     Overlay="true" ShowMoreResultsBox="false" Width="350px">
                                    <Items>
                                        <telerik:RadComboBoxItem Text="1" Value="1" />
                                        <telerik:RadComboBoxItem Text="2" Value="2" />
                                        <telerik:RadComboBoxItem Text="3" Value="3" />
                                    </Items>
                                </telerik:RadComboBox>
                            </td>
                            <td>
                                <asp:RequiredFieldValidator ID="rfvNoOfSignings" runat="server" ControlToValidate="rcbNoOfSignings" Display="Dynamic" ValidationGroup="grpAddUnAttachedPCV" >
                                    <img src="/images/Error.gif" alt="Please select the number of times the PCV has been signed." />
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Reason for Issue</td>
                            <td class="formCellField">
                                <telerik:RadComboBox ID="rcbReasonForIssue" runat="server"   
                                                     EnableLoadOnDemand="true" ItemRequestTimeout="500" AutoPostBack="false" MarkFirstMatch="true" 
                                                     Height="75px" Overlay="true" ShowMoreResultsBox="false" Width="350px" AllowCustomText="True"
                                                     DataTextField="Value" DataValueField="Key" />
                            </td>
                            <td>
                                <asp:RequiredFieldValidator ID="rfvReasonForIssue" runat="server" ControlToValidate="rcbReasonForIssue" Display="Dynamic" ValidationGroup="grpAddUnAttachedPCV" >
                                    <img src="/images/Error.gif" alt="Please select a reason for issue." />
                                </asp:RequiredFieldValidator>
                            </td>
                        </tr>
                    </table>
                </fieldset>
            
                <div class="buttonBar">
                    <asp:Button ID="btnAdd" runat="server" Text="Add" CommandArgument="Add" CausesValidation="true" ValidationGroup="grpAddUnAttachedPCV" />
                    <asp:Button ID="btnAddAndReset" runat="server" Text="Add and Reset"  CommandArgument="Add_Reset" CausesValidation="true" ValidationGroup="grpAddUnAttachedPCV" />
                </div>
            
            </div>
        </div>
    
        <div style="float:left; margin-left:50px;">
        
            <h3>Last 10 unattached PCV's added</h3>
            
            <asp:Panel ID="pnlLastPCVs" runat="server" >
                <asp:ListView ID="lvLastPCVs" runat="server">
                    <LayoutTemplate>
                        <table cellpadding="0" cellspacing="0" style="width:500px;">
                            <thead class="HeadingRow">
                                <th>PCVID</th>
                                <th>Voucher No</th>
                                <th>Client</th>
                                <th>Point Issued</th>
                                <th>Is Scanned</th>
                                <th>User</th>
                            </thead>
                            <tbody>
                                <asp:PlaceHolder ID="itemPlaceHolder" runat="server" />
                            </tbody>
                        </table>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <tr class="Row">
                            <td><a href='javascript:loadScanUploadSF(<%# ((System.Data.DataRowView)Container.DataItem)["ScannedFormID"].ToString() %>)'><%# ((System.Data.DataRowView)Container.DataItem)["PCVID"].ToString() %></a></td>
                            <td><%# ((System.Data.DataRowView)Container.DataItem)["VoucherNo"].ToString() %></td>
                            <td><%# ((System.Data.DataRowView)Container.DataItem)["Client"].ToString() %></td>
                            <td><%# ((System.Data.DataRowView)Container.DataItem)["IssuedPoint"].ToString()%></td>
                            <td><%# ((System.Data.DataRowView)Container.DataItem)["PDFName"].ToString() == Orchestrator.Globals.Constants.NO_DOCUMENT_AVAILABLE ? "No" : "Yes" %></td>
                            <td><%# ((System.Data.DataRowView)Container.DataItem)["UserID"].ToString() %></td>
                        </tr>
                    </ItemTemplate>
                    <EmptyDataTemplate>
                        No Unattached PCV's have been added.
                    </EmptyDataTemplate>
                </asp:ListView>
            </asp:Panel>
        
        </div>
    
    </div>
    
    <telerik:RadAjaxManager ID="ramUnAttachedPCV" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="rcbClient" EventName="SelectedIndexChanged">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rcbPalletType" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="dlgScanDocument">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlLastPCVs" LoadingPanelID="ralpIntegratePoints" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    
    <telerik:RadAjaxLoadingPanel ID="ralpIntegratePoints" runat="server" Transparency="10">
        <table width='100%' cellpadding='20px;' height='10%'>
            <tr>
                <td align="center" valign="top">
                    <img src='/images/postbackLoading.gif' />
                </td>
            </tr>
        </table>
    </telerik:RadAjaxLoadingPanel>
    
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript" language="javascript">

            function loadPCVCreatedWindow(sfID, pcvID) 
            {
                var qs = "sfID=" + sfID + "&pcvID=" + pcvID;
                <%=dlgPCVAdded.ClientID %>_Open(qs);
            }
            
            function loadScanUploadSF(sfID)
            {
                var qs = "ScannedFormTypeId=1&ScannedFormId=" + sfID;
                <%=dlgScanDocument.ClientID %>_Open(qs);
            }
        
        </script>

        <asp:Literal ID="litInjectScript" runat="server" />
    </telerik:RadCodeBlock>
    
</asp:Content>