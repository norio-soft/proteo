<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageDriverBenchmarks2.aspx.cs" MasterPageFile="~/default_tableless.Master" Inherits="Orchestrator.WebUI.CAN.ManageDriverBenchmarks2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>Manage Driver Benchmarks
    </h1>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Header" runat="server">
  <%--  <link rel="stylesheet" href="//netdna.bootstrapcdn.com/bootstrap/3.1.1/css/bootstrap.min.css">
    <link rel="stylesheet" href="//netdna.bootstrapcdn.com/bootstrap/3.1.1/css/bootstrap-theme.min.css">--%>
    <style>
        .error-message {
            font-weight:bold;
            color:#f00;
        }

        .faultListWrapper {
            margin-bottom:10px;
            padding:0
        }

       img.faultListMoveUp, img.faultListMoveDown, img.faultListDelete {
           width:16px;
           height:16px;
       }

       .listTable {
           margin: 0 auto;
           width:100%;
           border: 1px solid #363636;
           border-collapse: collapse;
       }
       .listTable thead tr {
           line-height: 28px;
           background-color: #000;
       }
       .listTable thead tr th {
           font-weight: normal;
           color: #fff;
           padding: 0 8px;
           border-right: 1px solid #363636;
           border-bottom: 1px solid #363636;
       }
       tr.odd {
           background-color: #f5f5f5;
       }
       td.centered {
           text-align:center;
       }

       .tableWrapper {
           width: 500px;
       }

       .profileAssignmentTable {
           width:100%;
       }

       /*inputs*/
       .faultTextBox {
           width:95%
       }
       .faultToggleCol, .faultActionCol {
           text-align:center;
       }
       .wideButtonDiv {
           padding:5px !important;
           border: none !important;
           background: none !important;
       }
       .wideButtonDiv > input {
           width:100%;
       }

       .buttonbar > table {
           width:100%
       }

       .floatR {
           float:right;
       }

       .profileAssignmentTable {
           width:700px;
       }

       .faultValidationWrapper ul { margin:0; }
       .faultValidationWrapper ul li { font-weight:bold; }
   </style>
      <script>

       function reallyDeleteBenchmark() { return confirm("Are you sure you want to delete this benchmark?\r\nThere is no way to undo this action."); }

       $(function () {
           $(".deleteProfileAction").on("click", reallyDeleteBenchmark);
       });

   </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="alert alert-success" role="alert" id="divSuccess" runat="server" visible="false">The Benchmarks have been updated successfully.</div>
    <telerik:RadMultiPage runat="server" RenderSelectedPageOnly="true" SelectedIndex="0" id="rmpBenchmarks">
         <!-- == Safety Check Profile List == -->
        <telerik:RadPageView id="rpBenchmarkList" runat="server">
            <div runat="server" Visible="false" id="deleteBenchmarkError">
                <p class="error-message">Unable to delete benchmark. Please ensure it is not currently assigned to any active drivers and not set to be the default. If the problem persists, please contact the support desk.</p>
            </div>

            <telerik:RadGrid runat="server" ID="rgBenchmarks" AutoGenerateColumns="false" AllowSorting="false">
                <MasterTableView DataKeyNames="DriverGradingBenchmarkID">
                    <Columns>
                        <telerik:GridBoundColumn DataField="Title" HeaderText="Title" HeaderStyle-Width="35%" />
                        <telerik:GridBoundColumn DataField="Description" HeaderText="Description" HeaderStyle-Width="35%" />
                        <telerik:GridTemplateColumn DataField="IsDefault" HeaderText="Is Default">
                            <ItemTemplate>
                                <asp:Label runat="server" Text='<%# Convert.ToBoolean(Eval("IsDefault")) == true ? "Yes" : "No" %>'></asp:Label>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="No of Drivers">
                            <ItemTemplate>
                                <%# GetDriverCount((Orchestrator.EF.DriverGradingBenchmark)Container.DataItem) %>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridButtonColumn ButtonType="LinkButton" CommandName="AssignDrivers" HeaderText="Assign" Text="Assign" HeaderStyle-Width="5%"  />
                        <telerik:GridButtonColumn ButtonType="LinkButton" CommandName="EditBenchmark" HeaderText="Edit" Text="Edit" HeaderStyle-Width="5%" />
                        <telerik:GridButtonColumn ButtonType="LinkButton" CommandName="DeleteBenchmark" HeaderText="Delete" Text="Delete" HeaderStyle-Width="5%" ButtonCssClass="deleteProfileAction"/>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
            <div class="buttonbar">
                <asp:Button ID="btnAddNew" runat="server" Text="Add New Benchmark" CssClass="buttonclass" />
            </div>  
        </telerik:RadPageView>

        <telerik:RadPageView runat="server" ID="rpvAddorEditBenchmark">
            <table>
             <tr>
                    <td>Title</td>
                    <td><asp:TextBox ID="txtTitle" runat="server" MaxLength="255" Width="300"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Description</td>
                    <td><asp:TextBox runat="server" ID="txtDescription" MaxLength="255" TextMode="MultiLine" Width="300"></asp:TextBox></td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkIsDefault" Text="Use as Default" />
                    </td>
                </tr>
            </table>
             <table class="table table-condensed">
                    <tr>
                        <th style="text-align: left;">Benchmark</th>
                        <th style="text-align: left;">Baseline</th>
                        <th style="text-align: left;">Target</th>
                    </tr>
                    <tr>
                        <td>Speeding Instances</td>
                        <td>
                            <telerik:RadNumericTextBox runat="server" ID="rnSpeedingBaseline" NumberFormat-DecimalDigits="0"></telerik:RadNumericTextBox>
                        </td>
                        <td>
                            <telerik:RadNumericTextBox runat="server" ID="rnSpeedingTarget" NumberFormat-DecimalDigits="0"></telerik:RadNumericTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>Over Rev Instances</td>
                        <td>
                            <telerik:RadNumericTextBox runat="server" ID="rnOverRevBaseline" NumberFormat-DecimalDigits="0"></telerik:RadNumericTextBox>
                        </td>
                        <td>
                            <telerik:RadNumericTextBox runat="server" ID="rnOverRevTarget" NumberFormat-DecimalDigits="0"></telerik:RadNumericTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>Harsh Braking Instances</td>
                        <td>
                            <telerik:RadNumericTextBox runat="server" ID="rnHarshBrakingBaseline" NumberFormat-DecimalDigits="0"></telerik:RadNumericTextBox>
                        </td>
                        <td>
                            <telerik:RadNumericTextBox runat="server" ID="rnHarshBrakingTarget" NumberFormat-DecimalDigits="0"></telerik:RadNumericTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>MPG</td>
                        <td>
                            <telerik:RadNumericTextBox runat="server" ID="rnMPGBaseline"></telerik:RadNumericTextBox>
                        </td>
                        <td>
                            <telerik:RadNumericTextBox runat="server" ID="rnMPGTarget"></telerik:RadNumericTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>Idling Percentage</td>
                        <td>
                            <telerik:RadNumericTextBox runat="server" ID="rnIdlingBaseline"></telerik:RadNumericTextBox>
                        </td>
                        <td>
                            <telerik:RadNumericTextBox runat="server" ID="rnIdlingTarget"></telerik:RadNumericTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>Grading Percentage</td>
                        <td>
                            <telerik:RadNumericTextBox runat="server" ID="rnGradingBaseline"></telerik:RadNumericTextBox>
                        </td>
                        <td>
                            <telerik:RadNumericTextBox runat="server" ID="rnGradingTarget"></telerik:RadNumericTextBox>
                        </td>
                    </tr>
                </table>
              <div class="buttonbar">
                <asp:Button ID="btnSaveChanges" runat="server" Text="Save Changes" CssClass="buttonclass" />
                  <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="buttonclass" />
            </div>  
        </telerik:RadPageView>
        <telerik:RadPageView ID="rpvAssignDrivers" runat="server">

            <table>
                <tr>
                    <td>
                        <p>
                        <asp:Label ID="lblDriversInBenchmark" Text="Drivers in this benchmark" runat="server"></asp:Label>
                            </p>
                        <telerik:RadListBox runat="server" ID="lbDriversInBenchmark" Width="282" EnableDragAndDrop="true"
                                AllowTransfer="true" TransferToID="lbDriversNotInBenchmark" AllowTransferDuplicates="false"
                                Height="400" MultipleSelect="true" AllowTransferOnDoubleClick="true" AutoPostBackOnTransfer="false"
                                SelectionMode="Multiple"  DataValueField="ResourceID" DataTextField="Description"/>
                    </td>
                    <td>
                
                <p>
                 <asp:Label ID="lblDriversNotInBenchmark" Text="Drivers Not in this benchmark" runat="server"></asp:Label>
                </p>
               <telerik:RadListBox runat="server" ID="lbDriversNotInBenchmark" Width="250" EnableDragAndDrop="true"
                                Height="400" SelectionMode="Multiple"  DataValueField="ResourceID" DataTextField="Description"/>
                        </td>
                    </tr>
            </table>
              <div class="buttonbar">
                <asp:Button ID="btnSaveAssignments" runat="server" Text="Save Changes" CssClass="buttonclass" />
                  <asp:Button ID="btnCancelAssignments" runat="server" Text="Cancel" CssClass="buttonclass" />
            </div>  
        </telerik:RadPageView>
        </telerik:RadMultiPage>

</asp:Content>

