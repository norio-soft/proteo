<%@ Page Language="C#" MasterPageFile="~/default_tableless.master" Inherits="Orchestrator.WebUI.Point.Point_mergePoints" Title="Merge Point" Codebehind="PointsMerge.aspx.cs" %>


<%@ Register TagPrefix="uc" Namespace="Orchestrator.WebUI.Controls" Assembly="WebUI" %>

<asp:content id="Content1" contentplaceholderid="ContentPlaceHolder1" runat="server">
<fieldset>
    <legend>Merge Points</legend>
		<div style="" >
            <div>   
            <div><strong>Organisation:</strong><br /><br />
                <telerik:RadComboBox ID="cboOrganisation" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500" AutoPostBack="true" 
                    MarkFirstMatch="true" RadControlsDir="~/script/RadControls/"   
                    ShowMoreResultsBox="true" Skin="WindowsXP" Width="200px" OnSelectedIndexChanged="cboOrganisation_SelectedIndexChanged">
                </telerik:RadComboBox>
            </div>
            <div>
                <strong>Address Points:</strong><br /><br />
                <div>
                <telerik:RadGrid ID="repPoints" runat="server" Width="100%" AllowAutomaticUpdates="True" AllowSorting="True" AutoGenerateColumns="False" EnableAJAXLoadingTemplate="True" LoadingTemplateTransparency="50" GridLines="None">
                 <ClientSettings>
                  <Selecting AllowRowSelect="True" />
                 </ClientSettings>
                  <MasterTableView Name="Points" AllowMultiColumnSorting="True">
                     <Columns>
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
                      <telerik:GridTemplateColumn HeaderText="Collect" UniqueName="Collect">
                        <ItemTemplate>
                            <asp:label ID="lblCollect" runat="server" text='<%# ((bool)Eval("Collect")) ? "Yes" : "No" %>'></asp:label>
                        </ItemTemplate>
                      </telerik:GridTemplateColumn>
                      <telerik:GridTemplateColumn HeaderText="Deliver" UniqueName="Deliver">
                        <ItemTemplate>
                            <asp:label ID="lblDeliver" runat="server" text='<%# ((bool)Eval("Collect")) ? "Yes" : "No" %>'></asp:label>
                        </ItemTemplate>
                      </telerik:GridTemplateColumn>
                      <telerik:GridTemplateColumn UniqueName="rdbReference" HeaderText="Merge To">
                            <ItemTemplate>
                                <uc:RdoBtnGrouper ID="btnRadioKeep" runat="server" GroupName="cboPalletTypeIsDefault" />
                            </ItemTemplate>
                          <HeaderStyle Width="3%" />
                        </telerik:GridTemplateColumn> 
                       <telerik:GridTemplateColumn UniqueName="chkReference" HeaderText="Merge From">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkRow" runat="server" />
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
                <div class="buttonbar">
                    <asp:Button ID="btnMergePoints" runat="server" Text="Merge Points" OnClick="btnMergePoints_Click" OnClientClick="if(!giveWarning()) {return false;} else { return true; }" />
                </div>
            </div>
        </div>
        <div style="clear: both"></div>
    </div>
</div>
<script language="javascript" type="text/javascript">

var masterPointID = "";

function giveWarning()
{
    return confirm("WARNING! Merging points is an irreversible action. The selected 'Merge From' points will be irretrievably deleted; are you sure you would like to proceed?");
}

function getSelectedRadioValue(pointID, grid, chkBox)
{
    masterPointID = pointID;
    var mtv = grid.MasterTableView;
    for (var rowIndex = 0; rowIndex < mtv.Rows.length; rowIndex++)
    {
        try
        {
            //var chkMergeFrom = mtv.Rows[rowIndex].Control.childNodes[10].childNodes[0];
            var chkMergeFrom = mtv.GetCellByColumnUniqueName(mtv.Rows[rowIndex] , "chkReference").childNodes[0];
            // If the checkbox has been found, and is selected - untick and disable the checkbox.
            if (chkMergeFrom != null)
                if(chkMergeFrom == chkBox)
                {
                    chkMergeFrom.checked = false;
                    chkMergeFrom.disabled = true;
                } else //enable the checkbox
                {
                    chkMergeFrom.disabled = false;
                }
        }
        catch (error)
        {
        }
    }
}
</script>
</asp:Content>
