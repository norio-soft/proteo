<%@ Page Language="C#" AutoEventWireup="true" Inherits="Resource_managedrivertype" MasterPageFile="~/WizardMasterPage.master" Title="Manage Driver Type" Codebehind="managedrivertype.aspx.cs" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
    <script language="javascript" type="text/javascript">
        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow; //Will work in Moz in all cases, including clasic dialog
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow; //IE (and Moz az well) 
            return oWindow;
        }

        function CloseOnReload() {
            GetRadWindow().Close();
        }

        function RefreshParentPage() {
            GetRadWindow().BrowserWindow.location.reload();
            GetRadWindow().Close();
        }
 
    </script>
</asp:Content>

<asp:Content ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Manage Driver Type</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .control-label {
            width: 80px;
            display: inline-block;
        }
    </style>
    
    <div>
        <asp:Label ID="lblError" runat="server" CssClass="Error" Visible="false" style="font-size:10pt; color:Red;"></asp:Label>
        <table>
            <tr>
                <td style="font-weight: bold">Driver Type Description</td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="txtDescription" runat="server" Width="300" MaxLength="100"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvDescription" runat="server" ControlToValidate="txtDescription" Text="The Driver Type Cannot be empty"><img runat="server" src="~/images/error.png" title="The driver type description cannot be empty." /> </asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td style="font-weight: bold">Standard Working Days</td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" AssociatedControlID="chkMonday" CssClass="control-label">Monday</asp:Label>
                    <asp:CheckBox ID="chkMonday" runat="server"></asp:CheckBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" AssociatedControlID="chkTuesday" CssClass="control-label">Tuesday</asp:Label>
                    <asp:CheckBox ID="chkTuesday" runat="server"></asp:CheckBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" AssociatedControlID="chkWednesday" CssClass="control-label">Wednesday</asp:Label>
                    <asp:CheckBox ID="chkWednesday" runat="server"></asp:CheckBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" AssociatedControlID="chkThursday" CssClass="control-label">Thursday</asp:Label>
                    <asp:CheckBox ID="chkThursday" runat="server"></asp:CheckBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" AssociatedControlID="chkFriday" CssClass="control-label">Friday</asp:Label>
                    <asp:CheckBox ID="chkFriday" runat="server"></asp:CheckBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" AssociatedControlID="chkSaturday" CssClass="control-label">Saturday</asp:Label>
                    <asp:CheckBox ID="chkSaturday" runat="server"></asp:CheckBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" AssociatedControlID="chkSunday" CssClass="control-label">Sunday</asp:Label>
                    <asp:CheckBox ID="chkSunday" runat="server"></asp:CheckBox>
                </td>
            </tr>
            <tr>
                <td style="font-weight: bold; padding-top: 15px;">Standard Working Hours</td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" AssociatedControlID="rtpStartTime" CssClass="control-label">Start Time</asp:Label>
                    <telerik:RadTimePicker ID="rtpStartTime" runat="server" Width="100">
                        <DateInput runat="server" DateFormat="HH:mm"></DateInput>
                    </telerik:RadTimePicker>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorStartTime" runat="server" ControlToValidate="rtpStartTime" Text="Enter a valid Start Time"><img runat="server" src="~/images/error.png" /> </asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td style="padding-bottom: 15px;">
                    <asp:Label runat="server" AssociatedControlID="rtpFinishTime" CssClass="control-label">Finish Time</asp:Label>
                    <telerik:RadTimePicker ID="rtpFinishTime" runat="server" Width="100">
                        <DateInput runat="server" DateFormat="HH:mm"></DateInput>
                    </telerik:RadTimePicker>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidatorFinishTime" runat="server" ControlToValidate="rtpFinishTime" Text="Enter a valid Finish Time"><img runat="server" src="~/images/error.png" /> </asp:RequiredFieldValidator>
                </td>
            </tr>
        </table>
    </div>
    
    <div class="buttonbar">
        <asp:Button ID="btnOK" runat="server" Text="OK" style="width:75px;" />
        <input type="button" onclick="javascript:CloseOnReload();" value="Cancel" style="width:75px" />
    </div>
    
    <asp:label ID="InjectScript" runat="server"></asp:label>
</asp:Content>