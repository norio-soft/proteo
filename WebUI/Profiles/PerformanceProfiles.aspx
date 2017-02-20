<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true"
    CodeBehind="PerformanceProfiles.aspx.cs" Inherits="Orchestrator.WebUI.Profiles.PerformanceProfile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">
   
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
 <h2>
        Manage Performance Profiles</h2>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="mpProfile">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="mpProfile" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    <telerik:RadMultiPage runat="server" RenderSelectedPageOnly="true" SelectedIndex="0" id="rmpProfiles">
        <telerik:RadPageView runat="server">
          <telerik:RadGrid runat="server" ID="rgProfiles" AutoGenerateColumns="false" AllowSorting="true">
            <MasterTableView DataKeyNames="ProfileID">
                <Columns>
                    <telerik:GridButtonColumn ButtonType="ImageButton" ImageUrl="/app_themes/fleetmetrik/img/masterpage/icon-pencil.png" CommandName="Edit" HeaderStyle-Width="20" />
                    <telerik:GridButtonColumn ButtonType="LinkButton" DataTextField="Title" CommandName="Edit" HeaderText="Title" />
                    <telerik:GridCheckBoxColumn DataField="IsDefault" HeaderText="Is Default" HeaderStyle-Width="80" />
                    <telerik:GridBoundColumn DataField="Make" HeaderText="Make" />
                    <telerik:GridBoundColumn DataField="Model" HeaderText="Model" />
                    <telerik:GridBoundColumn DataField="Purpose" HeaderText="Purpose" />
                    <telerik:GridButtonColumn ButtonType="LinkButton" DataTextField="NumberOfVehicles" HeaderText="# of Vehicles" CommandName="Vehicles" />
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
          <div class="buttonbar">
                        <asp:Button ID="btnAddNew" runat="server" Text="Add New" CssClass="buttonclass" />
                    </div>  
        </telerik:RadPageView>
        <telerik:RadPageView runat="server">
            <telerik:RadTabStrip ID="RadTabStrip1" runat="server" MultiPageID="rmpPerformance" CausesValidation="false">
                <Tabs>
                    <telerik:RadTab PageViewID="rpvPerformance" runat="server" Text="Performance" Selected="true" />
                    <telerik:RadTab PageViewID="rpvRev" runat="server" Text="Rev Bands (rpm)" />
                    <telerik:RadTab PageViewID="rpvSpeed" runat="server" Text="Speed Bands (kph)" />
                </Tabs>
            </telerik:RadTabStrip>
            <telerik:RadMultiPage runat="server" ID="rmpPerformance">
                <telerik:RadPageView runat="server" ID="rpvPerformance">
                    <asp:ValidationSummary ID="ValidationSummary1" runat="server" EnableClientScript="true"
                        ShowSummary="true" HeaderText="Changes cannot be saved until the following are changed."
                        CssClass="error" />
                    <table>
                        <tr>
                            <td class="formCellLabel-Horizontal">
                                Title
                            </td>
                            <td class="formCellField-Horizontal">
                                <asp:TextBox ID="txtTitle" runat="server"></asp:TextBox><asp:RequiredFieldValidator
                                    runat="server" ControlToValidate="txtTitle" ErrorMessage="Please enter a title"
                                    Display="None"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel-Horizontal">
                                Purpose
                            </td>
                            <td class="formCellField-Horizontal">
                                <asp:TextBox ID="txtPurpose" runat="server"></asp:TextBox><asp:RequiredFieldValidator
                                    ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtPurpose" ErrorMessage="Please enter a purpose for this profile"
                                    Display="None"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel-Horizontal">
                                Make
                            </td>
                            <td class="formCellField-Horizontal">
                                <asp:TextBox ID="txtMake" runat="server"></asp:TextBox><asp:RequiredFieldValidator
                                    ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtMake" ErrorMessage="Please enter a Make"
                                    Display="None"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel-Horizontal">
                                Model
                            </td>
                            <td class="formCellField-Horizontal">
                                <asp:TextBox ID="txtModel" runat="server"></asp:TextBox><asp:RequiredFieldValidator
                                    ID="RequiredFieldValidator3" runat="server" ControlToValidate="txtModel" ErrorMessage="Please enter a Model"
                                    Display="None"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel-Horizontal">
                                Use as Default?
                            </td>
                            <td class="formCellField-Horizontal">
                                <asp:CheckBox ID="chkIsDefault" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel-Horizontal">
                                Free Stopping Time (secs)
                            </td>
                            <td class="formCellField-Horizontal">
                                <telerik:RadNumericTextBox runat="server" ID="txtFreeStoppingTime" NumberFormat-DecimalDigits="0"
                                    MinValue="0">
                                </telerik:RadNumericTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel-Horizontal">
                                Free Idling Time (secs)
                            </td>
                            <td class="formCellField-Horizontal">
                                <telerik:RadNumericTextBox runat="server" ID="txtFreeIdlingTime" NumberFormat-DecimalDigits="0"
                                    MinValue="0">
                                </telerik:RadNumericTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel-Horizontal">
                                Economy Band Start (rpm)
                            </td>
                            <td class="formCellField-Horizontal">
                                <telerik:RadNumericTextBox runat="server" ID="txtEconomyBandStart" NumberFormat-DecimalDigits="0"
                                    MinValue="0">
                                </telerik:RadNumericTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel-Horizontal">
                                Economy Band End (rpm)
                            </td>
                            <td class="formCellField-Horizontal">
                                <telerik:RadNumericTextBox runat="server" ID="txtEconomyBandEnd" NumberFormat-DecimalDigits="0"
                                    MinValue="0">
                                </telerik:RadNumericTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel-Horizontal">
                                Free Speeding Time (secs)
                            </td>
                            <td class="formCellField-Horizontal">
                                <telerik:RadNumericTextBox runat="server" ID="txtFreeSpeedingTime" NumberFormat-DecimalDigits="0"
                                    MinValue="0">
                                </telerik:RadNumericTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel-Horizontal">
                                Speeding Limit (kph)
                            </td>
                            <td class="formCellField-Horizontal">
                                <telerik:RadNumericTextBox runat="server" ID="txtSpeedingLimit" NumberFormat-DecimalDigits="0"
                                    MinValue="0" MaxValue="150">
                                </telerik:RadNumericTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel-Horizontal">
                                Over Rev Warning (rpm)
                            </td>
                            <td class="formCellField-Horizontal">
                                <telerik:RadNumericTextBox runat="server" ID="txtOverRevWarning" NumberFormat-DecimalDigits="0"
                                    MinValue="0">
                                </telerik:RadNumericTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel-Horizontal">
                                Over Rev Limit (rpm)
                            </td>
                            <td class="formCellField-Horizontal">
                                <telerik:RadNumericTextBox runat="server" ID="txtOverRevLimit" NumberFormat-DecimalDigits="0"
                                    MinValue="0">
                                </telerik:RadNumericTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel-Horizontal">
                                Deceleration Cut Off (kph)
                            </td>
                            <td class="formCellField-Horizontal">
                                <telerik:RadNumericTextBox runat="server" ID="txtDecelerationCutOff" NumberFormat-DecimalDigits="0"
                                    MinValue="0">
                                </telerik:RadNumericTextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel-Horizontal">
                                Deceleration Warning (kph/s)
                            </td>
                            <td class="formCellField-Horizontal">
                                <telerik:RadNumericTextBox runat="server" ID="txtDecelerationWarning" NumberFormat-DecimalDigits="0"
                                    MinValue="0">
                                </telerik:RadNumericTextBox>
                            </td>
                        </tr>
                    </table>
                  
                </telerik:RadPageView>
                <telerik:RadPageView runat="server" ID="rpvRevBands">
                    <table style="margin:5px;">
                        <tr>
                            <td class="formCellLabel-Horizontal">1</td>
                            <td class="formCellField-Horizontal"><telerik:RadNumericTextBox runat="server" MinValue="0" ID="txtRevBand1" NumberFormat-DecimalDigits="0"></telerik:RadNumericTextBox></td>
                        </tr>
                        <tr>
                            <td class="formCellLabel-Horizontal">2</td>
                            <td class="formCellField-Horizontal"><telerik:RadNumericTextBox runat="server" MinValue="0" ID="txtRevBand2" NumberFormat-DecimalDigits="0"></telerik:RadNumericTextBox></td>
                        </tr>
                        <tr>
                            <td class="formCellLabel-Horizontal">3</td>
                            <td class="formCellField-Horizontal"><telerik:RadNumericTextBox runat="server" MinValue="0" ID="txtRevBand3" NumberFormat-DecimalDigits="0"></telerik:RadNumericTextBox></td>
                        </tr>
                        <tr>
                            <td class="formCellLabel-Horizontal">4</td>
                            <td class="formCellField-Horizontal"><telerik:RadNumericTextBox runat="server" MinValue="0" ID="txtRevBand4" NumberFormat-DecimalDigits="0"></telerik:RadNumericTextBox></td>
                        </tr>
                        <tr>
                            <td class="formCellLabel-Horizontal">5</td>
                            <td class="formCellField-Horizontal"><telerik:RadNumericTextBox runat="server" MinValue="0" ID="txtRevBand5" NumberFormat-DecimalDigits="0"></telerik:RadNumericTextBox></td>
                        </tr>
                        <tr>
                            <td class="formCellLabel-Horizontal">6</td>
                            <td class="formCellField-Horizontal"><telerik:RadNumericTextBox runat="server" MinValue="0" ID="txtRevBand6" NumberFormat-DecimalDigits="0"></telerik:RadNumericTextBox></td>
                        </tr>
                        <tr>
                            <td class="formCellLabel-Horizontal">7</td>
                            <td class="formCellField-Horizontal"><telerik:RadNumericTextBox runat="server" MinValue="0" ID="txtRevBand7" NumberFormat-DecimalDigits="0"></telerik:RadNumericTextBox></td>
                        </tr>
                        <tr>
                            <td class="formCellLabel-Horizontal">8</td>
                            <td class="formCellField-Horizontal"><telerik:RadNumericTextBox runat="server" MinValue="0" ID="txtRevBand8" NumberFormat-DecimalDigits="0"></telerik:RadNumericTextBox></td>
                        </tr>
                        <tr>
                            <td class="formCellLabel-Horizontal">9</td>
                            <td class="formCellField-Horizontal"><telerik:RadNumericTextBox runat="server" MinValue="0" ID="txtRevBand9" NumberFormat-DecimalDigits="0"></telerik:RadNumericTextBox></td>
                        </tr>
                        <tr>
                            <td class="formCellLabel-Horizontal">10</td>
                            <td class="formCellField-Horizontal"><telerik:RadNumericTextBox runat="server" MinValue="0" ID="txtRevBand10" NumberFormat-DecimalDigits="0"></telerik:RadNumericTextBox></td>
                        </tr>
                    </table>
                </telerik:RadPageView>
                <telerik:RadPageView runat="server" ID="rpvSpeedBands">
                <table>
                        <tr>
                            <td class="formCellLabel-Horizontal">1</td>
                            <td class="formCellField-Horizontal"><telerik:RadNumericTextBox runat="server" MinValue="0" ID="txtSpeedBand1" NumberFormat-DecimalDigits="0"></telerik:RadNumericTextBox></td>
                        </tr>
                        <tr>
                            <td class="formCellLabel-Horizontal">2</td>
                            <td class="formCellField-Horizontal"><telerik:RadNumericTextBox runat="server" MinValue="0" ID="txtSpeedBand2" NumberFormat-DecimalDigits="0"></telerik:RadNumericTextBox></td>
                        </tr>
                        <tr>
                            <td class="formCellLabel-Horizontal">3</td>
                            <td class="formCellField-Horizontal"><telerik:RadNumericTextBox runat="server" MinValue="0" ID="txtSpeedBand3" NumberFormat-DecimalDigits="0"></telerik:RadNumericTextBox></td>
                        </tr>
                        <tr>
                            <td class="formCellLabel-Horizontal">4</td>
                            <td class="formCellField-Horizontal"><telerik:RadNumericTextBox runat="server" MinValue="0" ID="txtSpeedBand4" NumberFormat-DecimalDigits="0"></telerik:RadNumericTextBox></td>
                        </tr>
                        <tr>
                            <td class="formCellLabel-Horizontal">5</td>
                            <td class="formCellField-Horizontal"><telerik:RadNumericTextBox runat="server" MinValue="0" ID="txtSpeedBand5" NumberFormat-DecimalDigits="0"></telerik:RadNumericTextBox></td>
                        </tr>
                        <tr>
                            <td class="formCellLabel-Horizontal">6</td>
                            <td class="formCellField-Horizontal"><telerik:RadNumericTextBox runat="server" MinValue="0" ID="txtSpeedBand6" NumberFormat-DecimalDigits="0"></telerik:RadNumericTextBox></td>
                        </tr>
                        <tr>
                            <td class="formCellLabel-Horizontal">7</td>
                            <td class="formCellField-Horizontal"><telerik:RadNumericTextBox runat="server" MinValue="0" ID="txtSpeedBand7" NumberFormat-DecimalDigits="0"></telerik:RadNumericTextBox></td>
                        </tr>
                        <tr>
                            <td class="formCellLabel-Horizontal">8</td>
                            <td class="formCellField-Horizontal"><telerik:RadNumericTextBox runat="server" MinValue="0" ID="txtSpeedBand8" NumberFormat-DecimalDigits="0"></telerik:RadNumericTextBox></td>
                        </tr>
                        <tr>
                            <td class="formCellLabel-Horizontal">9</td>
                            <td class="formCellField-Horizontal"><telerik:RadNumericTextBox runat="server" MinValue="0" ID="txtSpeedBand9" NumberFormat-DecimalDigits="0"></telerik:RadNumericTextBox></td>
                        </tr>
                        <tr>
                            <td class="formCellLabel-Horizontal">10</td>
                            <td class="formCellField-Horizontal"><telerik:RadNumericTextBox runat="server" MinValue="0" ID="txtSpeedBand10" NumberFormat-DecimalDigits="0"></telerik:RadNumericTextBox></td>
                        </tr>
                    </table>
                </telerik:RadPageView>
            </telerik:RadMultiPage>
            <div class="buttonbar">
                <asp:Button runat="server" ID="btnSave" Text="Save" CssClass="buttonclass" />
                <asp:Button runat="server" ID="btnCopy" Text="Create a Copy" CssClass="buttonclass" />
                <asp:Button runat="server" ID="btnCancel" Text="Cancel" CssClass="buttonclass" CausesValidation="false" />
            </div>
        </telerik:RadPageView>
        <telerik:RadPageView runat="server">
      
             <table style="border:none;">
            <tr>
                <th>
                    Vehicles In profile
                </th>
                <th>
                    Vehicles
                </th>
            </tr>
            <tr>
                <td><telerik:RadListBox runat="server" ID="lstVehiclesInProfile" SelectionMode="Multiple" Width="200px" Height="400px" AllowTransfer="true" TransferToID="lstVehicles" EnableDragAndDrop="true"></telerik:RadListBox></td>
                <td><telerik:RadListBox runat="server" ID="lstVehicles" Width="200px" Height="400px" SelectionMode="Multiple" ></telerik:RadListBox></td>
            </tr>

        </table>
        <div class="buttonbar">
            <asp:Button ID="btnSaveVehicles" runat="server" Text="Save" CssClass="buttonclass" />
            <asp:Button ID="btnCancelVehicles" runat="server" Text="Cancel" CssClass="buttonclass" CausesValidation="false" />
        </div>
        </telerik:RadPageView>
    </telerik:RadMultiPage>
</asp:Content>
