<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="True" CodeBehind="EditZonePostCodes.aspx.cs" Inherits="Orchestrator.WebUI.Tariff.EditZonePostCodes" %>

<asp:Content ContentPlaceHolderID="Header" runat="server"></asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Edit Zone Postcodes</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <fieldset runat="server" id="fsZoneMap" class="invisiableFieldset">
        <br />
        <table>
            <tr>
                <td class="formCellLabel">
                    Zone Map
                </td>
                <td class="formCellField">
                    <asp:Label runat="server" ID="lblZoneMap" />
                </td>
            </tr>
        </table>
        <table >
            <tr style="height: 20px" valign="top" >
                <td style="padding-left:10px; padding-right:10px; text-align:left" class="formCellLabel">
                    Select an Area
                </td>
                <td style="padding-left:10px; padding-right:10px; text-align:left" class="formCellLabel">
                    Select the Districts within an Area
                </td>
                <td colspan="2" style="padding-left:10px; padding-right:10px; text-align:left; width:178px;" class="formCellLabel">
                    Select the Zone that the Districts should be allocated to and click the Allocate button
                </td>
                <td style="padding-left:10px; padding-right:10px; text-align:left;" class="formCellLabel">
                    Districts allocated to Zones
                </td>
            </tr>
            <tr valign="top">
                <td style="padding-left:10px; padding-right:10px;">
                    <telerik:RadGrid runat="server" ID="grdRegions" Width="98px" Height="560" AutoGenerateColumns="False">
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
                        Height="560" AutoGenerateColumns="False">
                        <MasterTableView Width="200" GroupLoadMode="Server" TableLayout="Fixed" DataKeyNames="PostcodeAreaID">
                            <Columns>
                                <telerik:GridClientSelectColumn />
                                <telerik:GridBoundColumn HeaderText="District" DataField="Area" />
                                <telerik:GridBoundColumn HeaderText="Zone" DataField="Zone" />
                            </Columns>
                        </MasterTableView>
                        <ClientSettings Scrolling-AllowScroll="true" Scrolling-UseStaticHeaders="true" Scrolling-SaveScrollPosition="true"
                            Selecting-AllowRowSelect="true">
                            <ClientEvents />
                        </ClientSettings>
                    </telerik:RadGrid>
                </td>
                <td style="padding-left:10px; padding-right:5px;">
                    <telerik:RadGrid runat="server" ID="grdZones" AllowMultiRowSelection="false" Width="100" Height="560"
                        AutoGenerateColumns="False">
                        <MasterTableView TableLayout="Fixed" DataKeyNames="ZoneID">
                            <Columns>
                                <telerik:GridBoundColumn HeaderText="Zone" DataField="Description" />
                            </Columns>
                        </MasterTableView>
                        <ClientSettings Scrolling-AllowScroll="true" Selecting-AllowRowSelect="true" Scrolling-UseStaticHeaders="true" Scrolling-SaveScrollPosition="true">
                            <ClientEvents />
                        </ClientSettings>
                    </telerik:RadGrid>
                </td>
                <td style="padding-left:5px; padding-right:10px; padding-top:25px;">
                    <asp:Button runat="server" ID="btnAllocate" Text="Allocate" Width="90" Height="20" />
                </td>
                <td style="padding-left:10px; padding-right:10px;">
                    <table runat="server" id="grdZoneAreas" cellpadding="0" cellspacing="0" class="MasterTable_Orchestrator">
                    </table>
                </td>
            </tr>
        </table>
        <div class="buttonbar">
            <asp:Button ID="btnZoneMapList" runat="server" Text="Zone Map List" CausesValidation="false" />
            <asp:Button ID="btnEditZoneMap" runat="server" Text="Edit Zone Map" CausesValidation="false" />
        </div>
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