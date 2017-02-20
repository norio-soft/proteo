<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Traffic.changePlannedTimes" Codebehind="changePlannedTimes.aspx.cs" MasterPageFile="~/WizardMasterPage.Master" %>

<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>

<asp:Content ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
    <script type="text/javascript" src="/script/scripts.js"></script>
    <script type="text/javascript" src="/script/tooltippopups.js"></script>
    <script type="text/javascript" src="/script/jquery.fixedheader.js"></script>
    <script type="text/javascript" src="/script/jquery.blockUI-2.64.0.min.js"></script>

    <script type="text/javascript">
        var jobID = parseInt('<%= this.JobID %>', 10);
    </script>

    <script type="text/javascript" src="changePlannedTimes.aspx.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="margin-bottom:2px;">
        <uc1:infringementDisplay ID="infrigementDisplay" Visible="false" runat="server" />
        <table  id="PlannedTimes"  cellpadding="0" cellspacing="0" style="border-top: 0px;">
            <thead class="HeadingRow">
                <tr>
                    <th>From</th>
                    <th>Start</th>
                    <th>To</th>
                    <th>Finish</th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater ID="repLegs" runat="server" ItemType="Orchestrator.Entities.LegView">
                    <ItemTemplate>
                        <tr runat="server" id="legRow">
                            <td valign="top" width="30%">
                                <div onmouseover="ShowPointToolTip(this, <%# Item.StartLegPoint.Point.PointId %>);" onmouseout="closeToolTip();"><b><%# Item.StartLegPoint.Point.Description %></b></div>
                                <input type="hidden" id="hidInstructionID" runat="server" value='<%# Item.InstructionID %>' />
                            </td>
                            <td valign="top" width="20%">
                                <table border="0" cellpadding="0" cellspacing="0" style="width:180px;">
                                    <tr>
                                        <td>
                                            <telerik:RadDatePicker Width="100" ID="dteSDate" runat="server" >
                                                <DateInput runat="server" DateFormat="dd/MM/yy" DisplayDateFormat="dd/MM/yy">

                                                </DateInput>
                                            </telerik:RadDatePicker>
                                        </td>
                                        <td>
                                            <telerik:RadTimePicker Width="65" ID="dteSTime" runat="server">
                                                <DateInput runat="server" 
                                                DateFormat="HH:mm" DisplayDateFormat="HH:mm">
                                                    </DateInput>
                                            </telerik:RadTimePicker>
                                        </td>
                                        <td width="20">
	                                        <asp:RequiredFieldValidator id="rfvStartDate" runat="server" ControlToValidate="dteSDate" Display="Dynamic"
		                                        ErrorMessage="Please enter a Start Date.">
		                                        <img src="../images/error.gif" title="Please enter a Start Date." alt="Start date required" />
                                            </asp:RequiredFieldValidator>
		                                </td>
		                                <td width="20">
	                                        <asp:RequiredFieldValidator id="rfvStartTime" runat="server" ControlToValidate="dteSTime" Display="Dynamic"
		                                        ErrorMessage="Please enter a Start Time.">
		                                        <img src="../images/error.gif" title="Please enter a Start Time." alt="Start time required" />
                                            </asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td valign="top" width="30%">
                                <div onmouseover="ShowPointToolTip(this, <%# Item.EndLegPoint.Point.PointId %>);" onmouseout="closeToolTip();" ><b><%# Item.EndLegPoint.Point.Description %></b></div>
                                <input type="hidden" id="hidEndLegPointID" runat="server" value='<%# Item.EndLegPoint.Point.PointId %>' />
                            </td>
                            <td valign="top" width="20%">
                                <table border="0" cellpadding="0" cellspacing="0" style="width:180px;">
                                    <tr>
                                        <td>
                                            <telerik:RadDatePicker Width="100" ID="dteEDate" runat="server"> 
                                                <DateInput runat="server" DateFormat="dd/MM/yy" DisplayDateFormat="dd/MM/yy">
                                                </DateInput>
                                                </telerik:RadDatePicker>
                                        </td>
                                        <td>
                                            <telerik:RadTimePicker Width="65" ID="dteETime" runat="server" DateFormat="HH:mm" DisplayDateFormat="HH:mm"></telerik:RadTimePicker>
                                        </td>

                                        <td width="20">
	                                        <asp:RequiredFieldValidator id="rfvEndDate" runat="server" ControlToValidate="dteEDate" Display="Dynamic"
		                                        ErrorMessage="Please enter an End Date.">
		                                        <img src="../images/error.gif" title="Please enter an End Date." alt="End date required" />
                                            </asp:RequiredFieldValidator>
		                                </td>
		                                <td width="20">
	                                        <asp:RequiredFieldValidator id="rfvEndTime" runat="server" ControlToValidate="dteETime" Display="Dynamic"
		                                        ErrorMessage="Please enter an End Time.">
		                                        <img src="../images/error.gif" title="Please enter an End Time." alt="End time required" />
	                                        </asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>
    </div>
    
    <div class="buttonbar">
        <asp:Button ID="btnConfirm" runat="server" Text="Confirm" Width="75"></asp:Button>
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" Width="75" CausesValidation="False" />
    </div>
    
    <div id="divPointAddress" style="z-index: 5; display: none; background-color: Wheat; padding: 2px 2px 2px 2px;">
        <table style="background-color: white; border: solid 1pt black;" cellpadding="2">
            <tr>
                <td>
                    <span id="spnPointAddress"></span>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>