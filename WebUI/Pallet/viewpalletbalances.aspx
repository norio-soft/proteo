<%@ Page language="c#" Inherits="Orchestrator.WebUI.Pallet.ViewPalletBalances" Codebehind="ViewPalletBalances.aspx.cs" MasterPageFile="/default_tableless.master" %>
<%@ Register TagPrefix="P1" Namespace="P1TP.Components.Web.UI" Assembly="P1TP.Components" %>
<%@ Register TagPrefix="cc1" Namespace="Orchestrator.WebUI" Assembly="Orchestrator.WebUI.Dialog" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Pallet Balances</h1></asp:Content>

<asp:Content ContentPlaceHolderID="Header" runat="server">
    <script type="text/javascript" language="javascript" src="/script/popAddress.js"></script>
    <script type="text/javascript" language="javascript" src="viewpalletbalances.aspx.js"></script>
    
    <script type="text/javascript" language="javascript">
    
        $(document).ready(function(){
            $("input[id*='rntPalletCount']").prop('disabled', true);
            $("input:checkbox").prop("checked", false);
        });
    
        function openTrailerAlterPalletBalance(resourceID, palletTypeID) {
            var qs = "resourceId=" + resourceID + "&palletTypeID=" + palletTypeID;
            <%=dlgAlterPalletBalance.ClientID %>_Open(qs);
        }
        
        function chkHeaderSelect(item)
        {
            var filterString = item.checked ? ":not(checked)" : ":checked";
            $("input:checkbox[id*='chkPalletTypeSelect']" + filterString).prop("checked", item.checked);
            
            var palletTextBoxes = $("input[id*='rntPalletCount']");
            var checkedLegs = $("input:checkbox[id*='chkPalletTypeSelect']:checked").length;
            var pointBalanceButtonBar = $('#bbCPR');
            
            if(item.checked)
                palletTextBoxes.removeAttr('disabled');
            else
                palletTextBoxes.prop('disabled', true);
                
            if(checkedLegs == 0)
                pointBalanceButtonBar.css("display", "none");
            else
                pointBalanceButtonBar.css("display", "");
        }
        
        function chkItemSelect(item)
        {
            var headerCheckBox = $("input:checkbox[id*='chkHeaderSelect']");
            var legs = $("input:checkbox[id*='chkPalletTypeSelect']").length;
            var checkedLegs = $("input:checkbox[id*='chkPalletTypeSelect']:checked").length;
            var pointBalanceButtonBar = $('#bbCPR');            
            
            if(!item.checked || checkedLegs == 0)
                headerCheckBox.prop("checked", false);
                
            if(legs == checkedLegs)
                headerCheckBox.prop("checked", true);
                
            var currentItem = $('#' + item.id);
            var currentPalletCount = currentItem.parent().parent().find("input[id*='rntPalletCount']");
            
            if(item.checked)
                currentPalletCount.removeAttr("disabled");
            else
                currentPalletCount.prop("disabled", true);
                
            if(checkedLegs == 0)
                pointBalanceButtonBar.css("display", "none");
            else
                pointBalanceButtonBar.css("display", "");
        }
        
        function openPHW(){
            var contents = "";
            var checkedLegs = $("input:checkbox[id*='chkPalletTypeSelect']:checked");
            var btnCreatePalletReturn = $('#' + "<%=btnCreatePalletReturn.ClientID %>");
            var selectedBalanceIsEmpty = false;
            
            btnCreatePalletReturn.prop('disabled', true);
            
            try
            {
                for(var i = 0; i < checkedLegs.length; i++)
                {
                    var pointId = "-1", palletTypeId = "-1", palletTypeDescription = "", selectedBalance = "-1";
                
                    var currentRow = $('#' + checkedLegs[i].id).parent().parent();
                    var pointControl = currentRow.find("input[id*='hdnPointId']");
                    var palletTypeControl = currentRow.find("input[id*='hdnPalletTypeId']");
                    var palletTypeDesControl = currentRow.find("input[id*='hdnPalletTypeDescription']");
                    
                    var sBControl = currentRow.find("input[id*='rntPalletCount']");
                    var palletCountControl = $find(sBControl[1].id);
                    
                    pointId = pointControl.val();
                    palletTypeId = palletTypeControl.val();
                    palletTypeDescription = palletTypeDesControl.val();
                    selectedBalance = palletCountControl.get_value();
                    
                    if(selectedBalance.length < 1)
                        selectedBalanceIsEmpty = true;
                    
                    if(contents.length > 0)
                        contents = contents + '|' + pointId + ',' + palletTypeId + ',' + palletTypeDescription + ',' + selectedBalance;
                    else
                        contents = pointId + ',' + palletTypeId + ',' + palletTypeDescription + ',' + selectedBalance;                
                }
            }
            catch(err)
            {
                btnCreatePalletReturn.removeAttr('disabled');
            }
            
            if(!selectedBalanceIsEmpty)
                PageMethods.GetSelectedPallets(contents, getSelectedPallets_Success, getSelectedPallets_Failure);
            else
            {
                alert("Please make sure all selected pallet types have a balance.");
                btnCreatePalletReturn.removeAttr('disabled');
            }
                
            return false;
        }
        
        function getSelectedPallets_Success(result)
        {
            if(result.toString().toLowerCase() == "complete")
            {
                var qs = "";
                <%=dlgPalletHandlingWizard.ClientID %>_Open(qs);
            }
            else
                alert(result.toString());
            
            $('#' + "<%=btnCreatePalletReturn.ClientID %>").removeAttr('disabled');
        }
        
        function getSelectedPallets_Failure(error)
        {
            alert(error.get_message());
            $('#' + "<%=btnCreatePalletReturn.ClientID %>").removeAttr('disabled');
        }
        
    </script>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <cc1:Dialog ID="dlgAlterPalletBalance" runat="server" URL="AlterPalletBalance.aspx" Height="400" Width="550" Mode="Normal" AutoPostBack="true" ReturnValueExpected="true" />
    <cc1:Dialog ID="dlgPalletHandlingWizard" runat="server" URL="PalletHandlingWizard.aspx" Height="660" Width="1135" Mode="Normal" AutoPostBack="true" ReturnValueExpected="true" />
    
	<h2>The number of pallets at various loctions.</h2>
	
	<div class="buttonbar" style="margin-bottom:10px;">
        <asp:Button ID="btnRefresh" runat="server" Text="Refresh Balances" />
    </div>
	  
    <telerik:RadTabStrip id="RadTabStrip1" Skin="Orchestrator" SkinsPath="~/RadControls/Tabstrip/Skins/" Width="800" runat="server" MultiPageID="mpTabs" SelectedIndex="0" >
        <Tabs>
             <telerik:RadTab AccessKey="c" Text="&lt;u&gt;C&lt;/u&gt;lient Pallet Balances" PageViewID="ClientPalletBalanceView"></telerik:RadTab>
             <telerik:RadTab AccessKey="p" Text="&lt;u&gt;P&lt;/u&gt;oint Pallet Balances" PageViewID="PointPalletBalanceView"></telerik:RadTab>
             <telerik:RadTab AccessKey="t" Text="&lt;u&gt;T&lt;/u&gt;railer Pallet Balances" PageViewID="TrailerPalletBalanceView"></telerik:RadTab>
        </Tabs>
    </telerik:RadTabStrip>
    
    <telerik:RadMultiPage ID="mpTabs" runat="server" SelectedIndex="0">
        <telerik:RadPageView ID="ClientPalletBalanceView" runat="server">
            <div class="whitespacepusher"></div>
            <telerik:RadGrid ID="grdClient" runat="server" Skin="Orchestrator" Width="800" AllowAutomaticInserts="false" EnableAJAX="True" AllowAutomaticDeletes="false" AutoGenerateColumns="false">
                <MasterTableView EditMode="InPlace" DataKeyNames="IdentityId, PalletTypeId, Balance, PaperPallets">
                    <Columns>
                        <telerik:GridTemplateColumn HeaderText="Client" ReadOnly="true">
                            <ItemTemplate>
                                <span onmouseover="" onmouseout="javascript:HidePoint();"><a href="AuditPalletBalance.aspx?cID=<%# DataBinder.Eval(Container.DataItem, "IdentityId") %>&ptID=<%# DataBinder.Eval(Container.DataItem, "PalletTypeID") %>"><%# DataBinder.Eval(Container.DataItem, "OrganisationName")%></a></span>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn DataField="PalletTypeDescription" HeaderText="Pallet Type" ReadOnly="true"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Balance" HeaderText="Pallet Balance" ReadOnly="true" ></telerik:GridBoundColumn>
                        <telerik:GridTemplateColumn HeaderText="Wooden Pallets">
                            <ItemTemplate>
                                <%# ((System.Data.DataRowView)Container.DataItem)["WoodPallets"].ToString()%>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <telerik:RadNumericTextBox ID="rntClientPalletBalance" runat="server" Type="Number" Value='<%# ((int)((System.Data.DataRowView)Container.DataItem)["WoodPallets"]) %>' NumberFormat-DecimalDigits="0" NegativeStyle-ForeColor="Red" ClientEvents-OnValueChanged="" />
                                <asp:HiddenField ID="hidWoodenPallets" runat="server" Value='<%# ((int)((System.Data.DataRowView)Container.DataItem)["WoodPallets"]) %>' />
                                <asp:CustomValidator ID="cfvClientPalletBalance" runat="server" ControlToValidate="rntClientPalletBalance" ClientValidationFunction="validatePalletBalance" Display="Dynamic" EnableClientScript="true" ValidateEmptyText="true">
                                    <img src="/images/Error.gif" alt="Please enter a value greater or equal to 0." />
                                </asp:CustomValidator>
                            </EditItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn DataField="PaperPallets" HeaderText="Paper Pallets" ReadOnly="true" ></telerik:GridBoundColumn>
                        <telerik:GridEditCommandColumn UniqueName="AlterClientPallets" UpdateText="Save Changes" EditText="Alter Balance"  ></telerik:GridEditCommandColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
        </telerik:RadPageView>
        
        <telerik:RadPageView ID="PointPalletBalanceView" runat="server">
            <div class="whitespacepusher"></div>  
            <div>
                <telerik:RadGrid ID="grdPoints" runat="server" Skin="Orchestrator" Width="800" AllowAutomaticInserts="false" EnableAJAX="True" AllowAutomaticDeletes="false" AutoGenerateColumns="false">
                    <MasterTableView EditMode="InPlace" DataKeyNames="PointID, PalletTypeId">
                        <Columns>
                            <telerik:GridTemplateColumn HeaderText="Location">
                                <ItemTemplate>
                                    <span onmouseover="" onmouseout="javascript:HidePoint();"><a href="AuditPalletBalance.aspx?pID=<%# DataBinder.Eval(Container.DataItem, "PointId") %>&ptID=<%# DataBinder.Eval(Container.DataItem, "PalletTypeID") %>"><%# DataBinder.Eval(Container.DataItem, "PointDescription") %></a></span>
                                    <asp:HiddenField ID="hdnPointId" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "PointId") %>' />
                                    <asp:HiddenField ID="hdnPalletTypeId" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "PalletTypeID") %>' />
                                    <asp:HiddenField ID="hdnPalletTypeDescription" runat="server" Value='<%# DataBinder.Eval(Container.DataItem, "PalletTypeDescription") %>' />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="PalletTypeDescription" HeaderText="Pallet Type" ReadOnly="true"></telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn>
                                <ItemTemplate>
                                    <%# ((System.Data.DataRowView)Container.DataItem)["Balance"].ToString() %>
                                    <asp:HiddenField ID="hdnPalletBalance" runat="server" Value='<%# ((System.Data.DataRowView)Container.DataItem)["Balance"].ToString() %>' />
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <telerik:RadNumericTextBox ID="rntPointPalletBalance" runat="server" Type="Number" Value='<%# ((int)((System.Data.DataRowView)Container.DataItem)["Balance"]) %>' NumberFormat-DecimalDigits="0" NegativeStyle-ForeColor="Red" ClientEvents-OnValueChanged="" />
                                    <asp:CustomValidator ID="cfvPointPalletBalance" runat="server" ControlToValidate="rntPointPalletBalance" ClientValidationFunction="validatePalletBalance" Display="Dynamic" EnableClientScript="true" ValidateEmptyText="true">
                                        <img src="/images/Error.gif" alt="Please enter a value greater or equal to 0." />
                                    </asp:CustomValidator>
                                </EditItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridEditCommandColumn UniqueName="AlterPointPallets" UpdateText="Save Changes" EditText="Alter Balance"  ></telerik:GridEditCommandColumn>
                            <telerik:GridTemplateColumn>
                                <HeaderTemplate>
                                    <asp:CheckBox ID="chkHeaderSelect" runat="server" AutoPostBack="false" onclick="javascript:chkHeaderSelect(this);" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkPalletTypeSelect" runat="server" AutoPostBack="false" onclick="javascript:chkItemSelect(this);" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn>
                                <ItemTemplate>
                                    <telerik:RadNumericTextBox ID="rntPalletCount" runat="server" Type="Number" MinValue="0" Width="20px" NumberFormat-DecimalDigits="0" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </div>              
            
            <div id="bbCPR" class="buttonbar" style="width:790px; display:none;">
                <asp:Button ID="btnCreatePalletReturn" runat="server" Text="Pallet Return Run" ValidationGroup="CreatePalletRun" OnClientClick="if(!openPHW()) return false;" />
            </div>

        </telerik:RadPageView>
        
        <telerik:RadPageView ID="TrailerPalletBalanceView" runat="server">
              <div class="whitespacepusher"></div> 
              <telerik:RadGrid ID="grdTrailers" runat="server" Skin="Orchestrator" Width="800" AllowAutomaticInserts="false" EnableAJAX="True" AllowAutomaticDeletes="false" AutoGenerateColumns="false">
                <MasterTableView AllowAutomaticUpdates="true" EditMode="InPlace" >
                    <Columns>
                        <telerik:GridTemplateColumn HeaderText="Trailer Ref">
                            <ItemTemplate>
                                <span onmouseover="" onmouseout="javascript:HidePoint();"><a href="AuditPalletBalance.aspx?tID=<%# DataBinder.Eval(Container.DataItem, "ResourceId") %>&ptID=<%# DataBinder.Eval(Container.DataItem, "PalletTypeID") %>"><%# DataBinder.Eval(Container.DataItem, "TrailerRef")%></a></span>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn DataField="Description" HeaderText="Location" ReadOnly="true"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="PalletTypeDescription" HeaderText="Pallet Type" ReadOnly="true"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Balance" HeaderText="Pallet Balance"></telerik:GridBoundColumn>
                        <telerik:GridTemplateColumn UniqueName="AlterTrailerPallets">
                            <ItemTemplate>
                                <a href='javascript:openTrailerAlterPalletBalance(<%# Eval("ResourceId") %>,<%# Eval("PalletTypeId") %>);'>Alter</a>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
        </telerik:RadPageView>
    </telerik:RadMultiPage>
    
    <div id="divPointAddress" style="z-index=5;">
		<table style="background-color: white; border-width=1px; border-style=solid;" cellpadding="2">
			<tr>
				<td><span id="spnPointAddress"></span></td>
			</tr>
		</table>
	</div>
</asp:Content>