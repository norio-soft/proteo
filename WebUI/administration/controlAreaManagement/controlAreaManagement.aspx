<%@ Page Language="C#" MasterPageFile="~/default_tableless_angular.Master" AutoEventWireup="true" CodeBehind="controlAreaManagement.aspx.cs" Inherits="Orchestrator.WebUI.administration.ControlAreaManagement" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>Control Area Management</h1>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">


    <%: Scripts.Render("~/bundles/jquery") %>
    <%: Scripts.Render("~/bundles/angular") %>
    <%: Scripts.Render("~/bundles/corelibs") %>

    <script src="controlAreaManagement.js" type="text/javascript"></script>
    <script src="edit-control-area-modal.js" type="text/javascript"></script> 
    <script src="add-customer-modal.js" type="text/javascript"></script> 

    <asp:Literal runat="server" ID="BaseUrlScriptTag"></asp:Literal>

     <%: Scripts.Render("~/bundles/proteo.shared.api") %>
     <%: Scripts.Render("~/bundles/proteo.shared.forms") %>      
          
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid he-ang-page" ng-app="controlAreaManagementApp" >
        <div class="row" ng-controller="ControlAreaManagementCtrl">
    
            <div class="col-sm-6">
                <label for="control-area-select">Control Areas:</label>
                <table class="table bordered-table control-area-table" id="control-area-select">
                    <thead>
                        <tr>
                            <th>Description</th>
                            <th>Code</th>
                            <th>No of Assigned Customers</th>
                        </tr>
                    </thead>       
                    <tbody>
                        <tr ng-repeat="controlArea in controlAreas" 
                            ng-class="{'selected-row' : controlArea.isSelected}"
                            ng-click="selectControlArea(controlArea)">
                            <td>
                                {{controlArea.description}}                     
                            </td>
                            <td>

                                {{controlArea.controlAreaCode}}                        
                            </td>
                            <td>
                                {{controlArea.customerCount}}                        
                            </td>
                        </tr>
                        <tr style="height:auto">
                        </tr>
                    </tbody>
                </table>
                <div class="btn btn-primary" ng-click="addControlArea()" >Add</div>
                <div class="btn btn-primary" ng-click="editControlArea()" ng-disabled="editControlAreaDisabled()">Edit</div>
            </div>

             <div class="col-sm-6">
                <label for="customer-select">Customers:</label>
                <table class="table bordered-table control-area-table" id="customer-select">       
                    <tbody>
                        <tr ng-repeat="customer in customers"  
                            ng-class="{'selected-row' : customer.isSelected}"
                            ng-click="selectCustomer(customer)">
                            <td>
                                {{customer.organisationName}}
                            </td>
                        </tr>
                        <tr style="height:auto">
                        </tr>
                    </tbody>
                </table>
                <div class="btn btn-primary" ng-click="addCustomer()">Add</div>
                <div class="btn btn-primary" 
                        ng-really-click="removeCustomer()"
                        ng-really-message="Remove customer from control area?"
                        ng-disabled="removeCustomerDisabled()">
                    Remove
                </div>
            </div>
                      
        </div>  
    </div>


</asp:Content>
