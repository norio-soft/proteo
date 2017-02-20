<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="True" CodeBehind="EditDepotPostCodes.aspx.cs" Inherits="Orchestrator.WebUI.Tariff.EditDepotPostCodes" %>

<asp:Content ContentPlaceHolderID="Header" runat="server"></asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Edit Depot Postcodes</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <fieldset runat="server" id="fsZoneMap" class="invisiableFieldset">
        <table>
            <tr>
                <td class="formCellLabel">
                    Network
                </td>
                <td class="formCellField">
                    <telerik:RadComboBox runat="server" ID="cboNetworks" AutoPostBack="true" />
                </td>
            </tr>
        </table>
        <table >
            <tr style="height: 20px; valign="top" >
                <td style="padding-left:10px; padding-right:10px; text-align:left" class="formCellLabel">
                    Select an Area
                </td>
                <td style="padding-left:10px; padding-right:10px; text-align:left" class="formCellLabel">
                    Select the Districts within an Area
                </td>
                <td style="padding-left:10px; padding-right:10px; text-align:left; width:178px;" class="formCellLabel">
                    Select the Depot that the Districts should be allocated to and click the Allocate button
                </td>
                <td style="padding-left:10px; padding-right:10px; text-align:left;" class="formCellLabel">
                    Districts allocated to Depots
                </td>
            </tr>
            <tr valign="top">
                <td style="padding-left:10px; padding-right:10px;">
                    <telerik:RadGrid runat="server" ID="grdRegions" Width="98px" Height="490px" AutoGenerateColumns="False">
                        <MasterTableView TableLayout="Fixed" DataKeyNames="PostcodeRegionID" Width="80">
                            <Columns>
                                <telerik:GridBoundColumn HeaderText="Area" DataField="Description" />
                            </Columns>
                        </MasterTableView>
                        <ClientSettings Scrolling-AllowScroll="true" Scrolling-UseStaticHeaders="true" Scrolling-SaveScrollPosition="true"
                            Selecting-AllowRowSelect="true" EnablePostBackOnRowClick="true">
                            <Selecting AllowRowSelect="True"></Selecting>
                            <ClientEvents />
                        </ClientSettings>
                    </telerik:RadGrid>
                </td>
                <td style="padding-left:10px; padding-right:10px;">
                    <telerik:RadGrid runat="server" ID="grdAreas" AllowMultiRowSelection="true" Width="218"
                        Height="490" AutoGenerateColumns="False">
                        <MasterTableView Width="200" GroupLoadMode="Server" TableLayout="Fixed" DataKeyNames="PostcodeAreaID">
                            <Columns>
                                <telerik:GridClientSelectColumn />
                                <telerik:GridBoundColumn HeaderText="District" DataField="Area" />
                                <telerik:GridBoundColumn HeaderText="Depot" DataField="Depot" />
                            </Columns>
                        </MasterTableView>
                        <ClientSettings Scrolling-AllowScroll="true" Scrolling-UseStaticHeaders="true" Scrolling-SaveScrollPosition="true"
                            Selecting-AllowRowSelect="true">
                            <ClientEvents />
                        </ClientSettings>
                    </telerik:RadGrid>
                </td>
                <td style="padding-left:10px; padding-right:10px;">
                    <telerik:RadGrid runat="server" ID="grdDepots" AllowMultiRowSelection="false" Width="100"
                        AutoGenerateColumns="False">
                        <MasterTableView TableLayout="Fixed" DataKeyNames="DepotID">
                            <Columns>
                                <telerik:GridBoundColumn HeaderText="Depot" DataField="Code" />
                            </Columns>
                        </MasterTableView>
                        <ClientSettings Selecting-AllowRowSelect="true">
                            <ClientEvents />
                        </ClientSettings>
                    </telerik:RadGrid>
                    <br />
                    <br />
                    <asp:Button runat="server" ID="btnAllocate" Text="Allocate" Width="100" Height="20" />
                </td>
                <td style="padding-left:10px; padding-right:10px;">
                    <table runat="server" id="grdDepotAreas" cellpadding="0" cellspacing="0" class="MasterTable_Orchestrator">
                    </table>
                </td>
            </tr>
        </table>
    </fieldset>
    <telerik:RadAjaxManager runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="grdRegions">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdAreas" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
</asp:Content>