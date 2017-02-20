<%@ Page Language="C#" MasterPageFile="~/default_tableless.master" AutoEventWireup="True" Inherits="Orchestrator.WebUI.Point.Point_mergePoints" Title="Merge Point"  Codebehind="mergePoints.aspx.cs" %>
<%@ Register TagPrefix="uc" Namespace="Orchestrator.WebUI.Controls"  Assembly="WebUI" %>
<%@ Register TagPrefix="p1" TagName="Point" Src="~/UserControls/point.ascx" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Merge Points</h1></asp:Content>

<asp:Content ContentPlaceHolderID="Header" runat="server">
    <style type="text/css">
        .ConfirmationWindow
        {
            vertical-align:top;
            float:right;
            height: 20px;
            text-align: center;
            font: 12px Verdana, Arial, Helvetica, sans-serif;
            background:LightBlue;
            border:2,black;
            overflow:auto;
            opacity:0.6;
            filter:alpha(opacity=60);
        }
        
        .headoffice
        {
            display:none;
        }

    </style>
    <script language="javascript" type="text/javascript">
        var lastSelectedItem = null;
        var masterPointID = "";
        var PointIds = new Array();

        function giveWarning() {
            return confirm("WARNING! Merging points is an irreversible action. The selected 'Merge From' points will be irretrievably deleted; are you sure you would like to proceed?");
        }

        function MergePoints() {
            var checkboxs = $(":checkbox[id$=chkRow]:checked");
            if(checkboxs.length > 0)
                checkboxs.each(MergePoint);
            else    
                alert("No points selected to merge.")
        }

        function MergePoint() {
            var mergeFrom = this.parentNode.attributes["PointID"];
            var user = "<%=Page.User.Identity.Name %>";

            if (mergeFrom != "" && mergeFrom != null) {
                PointIds.push(mergeFrom.value);
                PageMethods.MergePoint(masterPointID, mergeFrom.value, user, MergePoint_Success, MergePoint_Failure);
            }

        }

        function MergePoint_Success(result) {
            try 
            {
                var parts = result.split(',');
                var confirmation = $get("<%=this.lblConfirmation.ClientID %>");
                confirmation.innerText = parts[1].toString();
                var div = $get('<%=this.divConfirmationWindow.ClientID %>');
                $(div).css("display", "");
                PointIds.pop(parts[0]);
                hideConfirmationWindow();
            }
            catch (e)
            {
                alert(e);
            }
        }

        function hideConfirmationWindow() {
            if (PointIds.length == 0) {
                var div = $get('<%=this.divConfirmationWindow.ClientID %>');
                $(div).css("display", "none");
                $get("<%=btnRefresh.ClientID %>").click();
            }
        }

        function MergePoint_Failure(error) {

            try 
            {
                var parts = result.toString();
                var confirmation = $get("<%=this.lblConfirmation.ClientID %>");
                confirmation.set_value(error.toString());
                var div = $get('<%=this.divConfirmationWindow.ClientID %>');
                $(div).css("display", "");  
            }
            catch (e)
            {
                
            }

        }

        function HandleSelectAll(chk) {
            $(":checkbox[id$=chkRow]:visible").each(function (chkIndex) { if (this.checked != chk.checked) this.click(); });
        }
        
        function checkMergeTo(rdo, chk, pointID) {
            if ($(rdo).prop("checked") == true) {
                masterPointID = pointID;

                // Enable all the checkboxes....
                $(':checkbox').prop("disabled", false);

                // always make sure that any hidden checkboxes remain unchecked and disabled
                $('span:hidden[pointid=+' + pointID + '] input:first-child')
                    .prop("checked", false)
                    .prop("disabled", true);

                // disable the checkbox for the merge to row.
                // Un-select the checkbox for the selected merge to row.
                $(chk)
                    .prop("checked", false)
                    .prop("disabled", true);
            }
        }


    </script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<telerik:RadAjaxManager runat="Server" ID="radAjaxManager">
    <AjaxSettings>
        <telerik:AjaxSetting AjaxControlID="repPoints">
            <UpdatedControls>
            <telerik:AjaxUpdatedControl ControlID="repPoints" LoadingPanelID="LoadingPanel1"/>
            </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="ucPoint" EventName="SelectedPointChanged">
            <UpdatedControls>
            <telerik:AjaxUpdatedControl ControlID="repPoints" LoadingPanelID="LoadingPanel1"/>
            </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="ucPoint">
            <UpdatedControls>
            <telerik:AjaxUpdatedControl ControlID="repPoints" LoadingPanelID="LoadingPanel1"/>
            </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="btnRefresh">
            <UpdatedControls>
            <telerik:AjaxUpdatedControl ControlID="repPoints" LoadingPanelID="LoadingPanel1"/>
            </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="pointDescriptionRadSlider">
            <UpdatedControls>
            <telerik:AjaxUpdatedControl ControlID="repPoints" LoadingPanelID="LoadingPanel1"/>
            </UpdatedControls>
        </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="addressLine1RadSlider">
            <UpdatedControls>
            <telerik:AjaxUpdatedControl ControlID="repPoints" LoadingPanelID="LoadingPanel1"/>
            </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="townRadSlider">
            <UpdatedControls>
            <telerik:AjaxUpdatedControl ControlID="repPoints" LoadingPanelID="LoadingPanel1"/>
            </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="btnMergePoints">
            <UpdatedControls>
            <telerik:AjaxUpdatedControl ControlID="repPoints" LoadingPanelID="LoadingPanel1"/>
            </UpdatedControls>
        </telerik:AjaxSetting>
    </AjaxSettings>
</telerik:RadAjaxManager>


<telerik:RadAjaxLoadingPanel Visible="true" ID="LoadingPanel1" IsSticky="false" runat="server">
    <img alt="Loading" src='<%= RadAjaxLoadingPanel.GetWebResourceUrl(Page, "Telerik.Web.UI.Skins.Default.Ajax.loading.gif") %>' />
</telerik:RadAjaxLoadingPanel>

    <div style="float: left; width: 400px; padding: 0 14px 0 0;">
        <h3>Point</h3>
            <p1:Point runat="server" ID="ucPoint" ShowFullAddress="true" CanUpdatePoint="false" CanClearPoint="true"
                ShowPointOwner="true" Visible="true" IsDepotVisible="true" CanCreateNewPoint="false" />

        <asp:Panel ID="Panel1" runat="server">
                <fieldset>
                <legend>Select the level of accuracy for each option</legend>
                <table>
                    <tr>
                        <td style=" text-align:center;" class="formCellLabel">
                            <h2>Description</h2>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <telerik:RadSlider ID="pointDescriptionRadSlider" AutoPostBack="true" runat="server" ItemType="item" Width="350px"
                                Height="70px" AnimationDuration="400" CssClass="ItemsSlider" ThumbsInteractionMode="Free"
                                 Value="9">
                                <Items>
                                    <telerik:RadSliderItem Value="0.1" />
                                    <telerik:RadSliderItem Value="0.15" />
                                    <telerik:RadSliderItem  Value="0.2" />
                                     <telerik:RadSliderItem Value="0.25" />
                                    <telerik:RadSliderItem Value="0.3" />
                                     <telerik:RadSliderItem  Value="0.35" />
                                    <telerik:RadSliderItem  Value="0.4" />
                                    <telerik:RadSliderItem  Value="0.45" />
                                    <telerik:RadSliderItem  Value="0.5" />
                                    <telerik:RadSliderItem Value="0.55" />
                                    <telerik:RadSliderItem  Value="0.6" />
                                    <telerik:RadSliderItem Value="0.65" />
                                    <telerik:RadSliderItem  Value="0.7" />
                                    <telerik:RadSliderItem  Value="0.75" />
                                    <telerik:RadSliderItem  Value="0.8" />
                                    <telerik:RadSliderItem  Value="0.85" />
                                    <telerik:RadSliderItem  Value="0.9" />
                                     <telerik:RadSliderItem  Value="0.95" />
                                    <telerik:RadSliderItem  Value="1" />
                                </Items>
                            </telerik:RadSlider>
                        </td>
                    </tr>
                    <tr>
                        <td style=" text-align:center;" class="formCellLabel">
                            <h2>Address Line 1</h2>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <telerik:RadSlider ID="addressLine1RadSlider" AutoPostBack="true" runat="server" ItemType="item" Width="350px"
                                Height="70px" AnimationDuration="400" CssClass="ItemsSlider" ThumbsInteractionMode="Free" Value="9">
                                <Items>
                                    <telerik:RadSliderItem Value="0.1" />
                                    <telerik:RadSliderItem Value="0.15" />
                                    <telerik:RadSliderItem  Value="0.2" />
                                     <telerik:RadSliderItem Value="0.25" />
                                    <telerik:RadSliderItem Value="0.3" />
                                     <telerik:RadSliderItem  Value="0.35" />
                                    <telerik:RadSliderItem  Value="0.4" />
                                    <telerik:RadSliderItem  Value="0.45" />
                                    <telerik:RadSliderItem  Value="0.5" />
                                    <telerik:RadSliderItem Value="0.55" />
                                    <telerik:RadSliderItem  Value="0.6" />
                                    <telerik:RadSliderItem Value="0.65" />
                                    <telerik:RadSliderItem  Value="0.7" />
                                    <telerik:RadSliderItem  Value="0.75" />
                                    <telerik:RadSliderItem  Value="0.8" />
                                    <telerik:RadSliderItem  Value="0.85" />
                                    <telerik:RadSliderItem  Value="0.9" />
                                     <telerik:RadSliderItem  Value="0.95" />
                                    <telerik:RadSliderItem  Value="1" />
                                </Items>
                            </telerik:RadSlider>
                        </td>
                    </tr>
                    <tr>
                        <td style=" text-align:center;" class="formCellLabel">
                            <h2>Town</h2>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <telerik:RadSlider ID="townRadSlider" runat="server" ItemType="item" Width="350px"
                                Height="70px" AnimationDuration="400" AutoPostBack="true" CssClass="ItemsSlider" ThumbsInteractionMode="Free" Value="9">
                                <Items>
                                    <telerik:RadSliderItem Value="0.1" />
                                    <telerik:RadSliderItem Value="0.15" />
                                    <telerik:RadSliderItem  Value="0.2" />
                                     <telerik:RadSliderItem Value="0.25" />
                                    <telerik:RadSliderItem Value="0.3" />
                                     <telerik:RadSliderItem  Value="0.35" />
                                    <telerik:RadSliderItem  Value="0.4" />
                                    <telerik:RadSliderItem  Value="0.45" />
                                    <telerik:RadSliderItem  Value="0.5" />
                                    <telerik:RadSliderItem Value="0.55" />
                                    <telerik:RadSliderItem  Value="0.6" />
                                    <telerik:RadSliderItem Value="0.65" />
                                    <telerik:RadSliderItem  Value="0.7" />
                                    <telerik:RadSliderItem  Value="0.75" />
                                    <telerik:RadSliderItem  Value="0.8" />
                                    <telerik:RadSliderItem  Value="0.85" />
                                    <telerik:RadSliderItem  Value="0.9" />
                                     <telerik:RadSliderItem  Value="0.95" />
                                    <telerik:RadSliderItem  Value="1" />
                                </Items>
                            </telerik:RadSlider>
                        </td>
                    </tr>
                    <tr>
                        <td style=" text-align:center;" class="formCellLabel">
                            <h2>Match on PostCode</h2>
                        </td>
                    </tr>
                    <tr>
                        <td style=" text-align:center;">    
                            <asp:CheckBox runat="Server" ID="chkMatchOnPostCode" Checked="false" />
                        </td>
                    </tr>
                </table>
                <div class="buttonbar">
                    <asp:Button ID="btnRefresh" runat="server" Text="Refresh"/>
                </div>
                </fieldset>
        </asp:Panel>
    </div>
   <table>
       <tr>
            <td>Key:</td>
           <td style=" background:silver; width:100;">
                <h1>Head Office</h1>
           </td>
       </tr>
   </table>
    <div style="float: left; width: 850px; padding: 0 0 0 15px; border-left: 1px dotted #CCC;">
        <h3>Address Points</h3>
        <div>
            <telerik:RadGrid ID="repPoints" runat="server" Width="100%" AllowAutomaticUpdates="True" AllowSorting="True" Skin="Office2007" 
            AutoGenerateColumns="False" EnableAJAXLoadingTemplate="True" LoadingTemplateTransparency="50" GridLines="None" EnableViewState="false">
                <ClientSettings>
                    <Selecting AllowRowSelect="True" />
                </ClientSettings>
                <MasterTableView Name="Points" AllowMultiColumnSorting="True">
                    <Columns>
                        <telerik:GridBoundColumn HeaderText="Organisation" DataField="OrganisationName" ReadOnly="True" UniqueName="OrganisationName">
                            <HeaderStyle Width="25%" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderText="Point Description" DataField="Description" ReadOnly="True" UniqueName="Description">
                            <HeaderStyle Width="25%" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderText="Address Line 1" DataField="AddressLine1" ReadOnly="True" UniqueName="AddressLine1">
                            <HeaderStyle Width="12%" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderText="Address Line 2" DataField="AddressLine2" ReadOnly="True" UniqueName="AddressLine2">
                            <HeaderStyle Width="12%" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderText="Address Line 3" DataField="AddressLine3" ReadOnly="True" UniqueName="AddressLine3">
                            <HeaderStyle Width="12%" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderText="Post Town" DataField="PostTown" SortExpression="PostTown" ReadOnly="True" UniqueName="PostTown">
                            <HeaderStyle Width="9%" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderText="County" DataField="County" SortExpression="County" ReadOnly="True" UniqueName="County">
                            <HeaderStyle Width="9%" />
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderText="Post Code" DataField="PostCode" ReadOnly="True" UniqueName="PostCode" >
                            <HeaderStyle Width="8%" />
                        </telerik:GridBoundColumn>
                        <telerik:GridTemplateColumn UniqueName="rdbReference" HeaderText="Merge To">
                            <ItemTemplate>
                                <uc:RdoBtnGrouper ID="btnRadioKeep" runat="server" GroupName="cboPalletTypeIsDefault" />
                            </ItemTemplate>
                            <HeaderStyle Width="3%" />
                        </telerik:GridTemplateColumn> 
                        <telerik:GridTemplateColumn UniqueName="chkReference" HeaderText="Merge From">
                            <HeaderTemplate>
                                <asp:Label Text="Merge From" runat="server" />
                                <input type="checkbox" onclick="javascript:HandleSelectAll(this);" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkRow" runat="server"  />
                            </ItemTemplate>
                            <HeaderStyle Width="3%" />
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn HeaderText="Address ID" DataField="PointID" UniqueName="PointID" ReadOnly="True" Visible="False" >
                        </telerik:GridBoundColumn>   
                    </Columns>                     
                    <ExpandCollapseColumn Visible="False">
                        <HeaderStyle Width="19px" />
                    </ExpandCollapseColumn>
                    <RowIndicatorColumn Visible="False">
                        <HeaderStyle Width="20px" />
                    </RowIndicatorColumn>
                </MasterTableView>
            </telerik:RadGrid>

            <div id="divConfirmationWindow" class="ConfirmationWindow" runat="server" style="display:none; width:100%;" >
                <asp:Label ID="lblConfirmation" runat="server" ForeColor="Black"></asp:Label>
            </div>
            <div class="buttonbar">
                <asp:Button ID="btnMergePoints" runat="server" Text="Merge Points" OnClientClick="if(!giveWarning()) {MergePoints(); return false;} else {MergePoints(); return false; }" />
            </div>

        </div>
    </div>
    <div class="clearDiv"></div> 
   
</asp:Content>