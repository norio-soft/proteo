<%@ Page language="c#" MasterPageFile="~/default_tableless.master" Inherits="Orchestrator.WebUI.Resource.Driver.EnterDriverStartTimes" Codebehind="EnterDriverStartTimes.aspx.cs" Title="Haulier Enterprise" %>

<%@ Register TagPrefix="cs" Namespace="Codesummit" Assembly="WebModalControls" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div>
        <asp:Label EnableViewState="false" Text="The start time has been successfully added." id="lblConfirmation" runat="server" cssclass="confirmation" visible="false"></asp:Label>
        <fieldset><legend><strong>Driver start times</strong></legend>
            <table>
                <tr>
                    <td>Select driver</td>
                    <td>
                        <telerik:RadComboBox ID="cboDriver" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500" MarkFirstMatch="true" RadControlsDir="~/script/RadControls/"
                            ShowMoreResultsBox="true" Skin="WindowsXP" Width="355px" DataTextField="Description" DataValueField="ResourceId">
                        </telerik:RadComboBox>
                    </td>
                    <td>
                        <asp:RequiredFieldValidator id="rfvDriver" runat="server" Display="Dynamic" ControlToValidate="cboDriver" ErrorMessage="Please supply a driver"><img src="/images/Error.gif" height="16" width="16" title="Please supply a driver" /></asp:RequiredFieldValidator>
                        <asp:CustomValidator id="cfvDriver" runat="server" Display="Dynamic" ControlToValidate="cboDriver" EnableClientScript="False" ErrorMessage="Please supply a driver"><img src="/images/Error.gif" height="16" width="16" title="Please supply a driver" /></asp:CustomValidator>
                    </td>
                </tr>
                <tr>
                    <td>Date</td>
                    <td><telerik:RadDatePicker ID="rdiDate" runat="server" ToolTip="The date to enter the start time for." Width="100">
                    <DateInput runat="server"
                    DateFormat="dd/MM/yy">
                    </DateInput>
                    </telerik:RadDatePicker></td>
                    <td><asp:RequiredFieldValidator id="rfvDate" runat="server" Display="Dynamic" ControlToValidate="rdiDate" ErrorMessage="Please supply a date"><img src="/images/Error.gif" height="16" width="16" title="Please supply a date" /></asp:RequiredFieldValidator></td>
                </tr>
                <tr>
                    <td>Start time</td>
                    <td colspan="2">
                        <table border="0" cellpadding="0" cellspacing="0">
                            <tr>
                                <td><telerik:RadTimePicker ID="rdiStartTime" runat="server" ToolTip="The start time to display on the Traffic Sheet" Width="100">
                                <DateInput runat="server"
                                DateFormat="HH:mm">
                                </DateInput>
                                </telerik:RadTimePicker></td>
                                <td><asp:RequiredFieldValidator id="rfvStartTime" runat="server" Display="Dynamic" ControlToValidate="rdiStartTime" ErrorMessage="Please supply a start time"><img src="/images/Error.gif" height="16" width="16" title="Please supply a start time" /></asp:RequiredFieldValidator></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>Notes</td>
                    <td colspan="2">
                        <table border="0" cellpadding="0" cellspacing="0">
                            <tr>
                                <td><asp:TextBox id="txtNotes" runat="server" TextMode="MultiLine" Width="350px" Rows="3"></asp:TextBox></td>
                            </tr>
                        </table>
                    </td>
                </tr>	
            </table>
        </fieldset>
    </div>
    
    <div class="buttonbar">
       <asp:Button id="btnSet" runat="server" Text="Set Start Time"></asp:Button>
        <asp:Button ID="btnClose" OnClientClick="redirect()" runat="server" Text="Close" Width="75" CausesValidation="false" />

    </div>
    <script type="text/javascript">

          function redirect()
          {
            if (window.opener.location.pathname=="/traffic/TSResource.aspx") {
                window.close();
            }
          }
    </script>
           
</asp:Content>