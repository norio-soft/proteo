<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="instructionlist.aspx.cs" Inherits="Orchestrator.WebUI.mwf.instructionlist" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">
    <script src="../script/jquery-ui-1.9.2.min.js" type="text/javascript"></script>
    <script src="../script/jquery.blockUI-2.64.0.min.js" type="text/javascript"></script>
    <script src="../script/jquery.validationEngine.js" type="text/javascript"></script>
    <script src="../script/jquery.validationEngine-en.js" type="text/javascript"></script>
    <script src="../script/loading-panel.js" type="text/javascript"></script>
    <script src="instructionlist.aspx.js" type="text/javascript"></script>
    <link href="../style/validationEngine.jquery.css" rel="stylesheet" type="text/css" />

    <script type="text/javascript">
        $(document).ready(function () {
            var interval = document.aspnetForm.elements['ctl00$ContentPlaceHolder1$hfInterval'].value;

            if (interval > 0) {
                window.setTimeout('submitForm()', interval);
            }
        });

        function submitForm() {
            document.aspnetForm.submit();
        };        
    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1 runat="server" id="headerText">Instruction list</h1>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField ID="hfInterval" EnableViewState="true" Value="" runat="server" />

    <div id="wrapper">
        <div id="header">
            <asp:ValidationSummary runat="server" ID="validationSummary" DisplayMode="BulletList" />
            <asp:Label runat="server" ID="errorMessage" EnableViewState="false" CssClass="errorMessage" />
        </div>

        <div id="container">
            <div id="instructionlist">
                <h1>Instruction list</h1>

                <div id="FilterOptions" style="margin-bottom: 4px; display: none;">
                    <h3>
                        <a href="#">Filter options</a></h3>
                    <div>
                        <table class="filter" width="100%">
                            <tr>
                                <td class="formCellLabel">
                                    <label for="Driver">
                                        Worker</label>
                                </td>
                                <td valign="top">
                                    <telerik:RadComboBox ID="DriverPicker" runat="server" ClientIDMode="Static" DataValueField="Value"
                                        DataTextField="Text" AppendDataBoundItems="true" Width="200px" DropDownWidth="200px">
                                        <Items>
                                            <telerik:RadComboBoxItem Value="" Text="- all -" />
                                        </Items>
                                    </telerik:RadComboBox>
                                </td>
                                <td style="text-align: left;">
                                    <label for="Status">
                                        Status</label>
                                </td>
                                <td style="text-align: left;">
                                    <label for="CommunicationsStatus">
                                        Communication status</label>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">
                                    <label for="Vehicle">
                                        Vehicle</label>
                                </td>
                                <td valign="top">
                                    <telerik:RadComboBox ID="VehiclePicker" runat="server" ClientIDMode="Static" DataValueField="Value"
                                        DataTextField="Text" AppendDataBoundItems="true" Width="200px" DropDownWidth="200px">
                                        <Items>
                                            <telerik:RadComboBoxItem Value="" Text="- all -" />
                                        </Items>
                                    </telerik:RadComboBox>
                                </td>
                                <td rowspan="3" valign="top">
                                    <asp:CheckBoxList ID="StatusCheckboxList" runat="server" DataTextField="Text" DataValueField="Value">
                                    </asp:CheckBoxList>
                                </td>
                                <td rowspan="3" valign="top">
                                    <asp:CheckBoxList ID="CommunicationStatusCheckBoxList" runat="server" DataTextField="Text"
                                        DataValueField="Value">
                                    </asp:CheckBoxList>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">
                                    <label for="ArrivalFrom">
                                        Arrival Between</label>
                                </td>
                                <td style="vertical-align: top">
                                    <telerik:RadDatePicker ID="ArrivalFrom" runat="server" />
                                    <label>and</label>
                                    <telerik:RadDatePicker ID="ArrivalTo" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td valign="top" class="formCellLabel">
                                    <label for="txtSearchField">Search Text</label>
                                </td>
                                <td valign="top">
                                    <asp:TextBox ID="txtSearchField" runat="server" />
                                    <br />
                                    (The Run ID, Customer order and Customer ref will be searched)
                                </td>
                            </tr>
                            
                        </table>

                        <asp:Button ID="FilterButton" runat="server" Text="Find" CausesValidation="false" CssClass="buttonClass" />
                        <asp:Button ID="ResetButton" runat="server" Text="Reset" CausesValidation="false" CssClass="buttonClass" />
                        <div class="clearDiv">
                        </div>
                    </div>
                </div>

                <div id="instructions">
                    <telerik:RadGrid runat="server" ID="InstructionsGrid" AutoGenerateColumns="false" AllowSorting="true">
                        <MasterTableView PageSize="15" AllowPaging="false" DataKeyNames="ID, StatusID, CommunicationStatusID" ClientDataKeyNames="ID" CommandItemDisplay="Top">
                            <CommandItemTemplate>
                                <input type="button" id="CommunicateButton" class="buttonClassSmall" value="Communicate new and changed" />
                            </CommandItemTemplate>
                            <Columns>
                                <telerik:GridHyperLinkColumn HeaderText="Run ID" DataTextField="JobID" DataNavigateUrlFields="JobID"
                                    DataNavigateUrlFormatString="/job/job.aspx?jid={0}&csid=xx" SortExpression="JobID" />
                                <telerik:GridBoundColumn DataField="CustomerOrder" HeaderText="Customer order" />
                                <telerik:GridBoundColumn DataField="CustomerReference" HeaderText="Customer ref" />
                                <telerik:GridBoundColumn DataField="InstructionType" HeaderText="Type" />
                                <telerik:GridBoundColumn DataField="Location" HeaderText="Location" />
                               
                                <telerik:GridBoundColumn DataField="RunStatus" HeaderText="Run status" />
                                <telerik:GridBoundColumn DataField="Status" HeaderText="Instruction status" />
                                <telerik:GridBoundColumn DataField="Driver" HeaderText="Worker" />
                                <telerik:GridBoundColumn DataField="VehicleReg" HeaderText="Vehicle" />
                                <telerik:GridBoundColumn DataField="CommunicationStatus" HeaderText="Current status" />
                                <telerik:GridBoundColumn DataField="CommunicationDateTime" HeaderText="When" DataFormatString="{0:dd/MM/yyyy HH:mm}"/>
                                <telerik:GridBoundColumn DataField="DriveDateTime" HeaderText="Drive Time"  DataFormatString="{0:dd/MM/yyyy HH:mm}"/>
                                 <telerik:GridBoundColumn DataField="ArrivalTime" HeaderText="Arrival time" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                                <telerik:GridBoundColumn DataField="CompleteDateTime" HeaderText="Complete Time"  DataFormatString="{0:dd/MM/yyyy HH:mm}"/>
                                <telerik:GridTemplateColumn HeaderText="Signed by">
                                    <ItemTemplate>
                                        <a href="#" onclick="showSignatureTooltip(this, '<%# Eval("SignatureImage")%>', '<%# Eval("SignedBy") %>', '<%# Eval("SignedComment")%>', '<%# Eval("SignedDateTime") %>', '<%# Eval("SignatureLatitude") %>','<%# Eval("SignatureLongitude") %>'); return false;">
                                            <%# Eval("SignedBy") %></a>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Comments">
                                    <ItemTemplate>
                                        <a href="#" onclick="showSignatureTooltip(this, '<%# Eval("SignatureImage")%>', '<%# Eval("SignedBy") %>', '<%# Eval("SignedComment")%>', '<%# Eval("SignedDateTime") %>'); return false;">
                                            <%# Eval("SignedComment")%></a>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn UniqueName="Pod" Visible="false">
                                    <ItemTemplate>
                                        <a target="_blank" class="podOrderLink gridTrueLink" href="/reports/pod.ashx?oid=<%# Eval("OrderID") %>"><%# ((bool)Eval("OrderComplete")) && (Eval("OrderID") != null) ? "POD" : "" %></a>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </div>                
            </div>
        </div>
    </div>

    <script type="text/javascript">
        var tooltipId = "<%= RadToolTip1.ClientID %>";
    </script>

    <telerik:RadToolTip runat="server" ID="RadToolTip1" EnableShadow="true" HideEvent="ManualClose"
        ShowEvent="FromCode" Width="500px" RelativeTo="Mouse" Position="MiddleLeft" MouseTrailing="true"
        ShowCallout="false">
    </telerik:RadToolTip>
</asp:Content>
