<%@ Page Language="C#" MasterPageFile="~/default_tableless_angular.Master" AutoEventWireup="true" CodeBehind="interSiteChargingReportVariables.aspx.cs" Inherits="Orchestrator.WebUI.administration.intersite.interSiteChargingReportVariables" %>

<%@ Import Namespace="System.Web.Optimization" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">

    <h1>Inter-site Charging Report Variables</h1>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">

    <%: Scripts.Render("~/bundles/jquery") %>
    <%: Scripts.Render("~/bundles/angular") %>
    <%: Scripts.Render("~/bundles/corelibs") %>


    <script src="interSiteChargingReportVariables.js" type="text/javascript"></script>

    <asp:Literal runat="server" ID="BaseUrlScriptTag"></asp:Literal>

    <%: Scripts.Render("~/bundles/proteo.shared.api") %>
    <%: Scripts.Render("~/bundles/proteo.shared.forms") %>   

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-fluid he-ang-page" style="padding-right: 21px" ng-app="interSiteReportVariablesApp">
        <toast></toast>
        <div class="row" ng-controller="InterSiteReportVariablesCtrl">
            <div class="col-sm-6">
            <div class="row">
                    <form name="currentInterSiteForm" role="form">
                        <div class="form-horizontal col-md-6">
                            <h4 class="form-sub-header">Current Inter-site charges:</h4>
                            <hr class="form-sep" />

                            <div class="form-group">
                                <label class="control-label col-sm-3">0-20</label>
                                <div class="col-sm-9" pr-show-errors>
                                    <div class="input-group">
                                        <input
                                            type="text"
                                            ng-currency
                                            currency-symbol="£"
                                            class="form-control rounded"
                                            ng-model="currentInterSiteCharge.miles1"
                                            required
                                            name="miles1"
                                            min="0" />
                                        <span class="input-group-addon input-group-addon-remove-border">
                                            <pr-validator form-control="currentInterSiteForm.miles1" field-name="(0-20)" only-when-touched>
                                        </span>
                                    </div>
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="control-label col-sm-3">21-40</label>
                                <div class="col-sm-9" pr-show-errors>
                                    <div class="input-group">
                                        <input
                                            type="text"
                                            ng-currency
                                            currency-symbol="£"
                                            class="form-control rounded"
                                            ng-model="currentInterSiteCharge.miles2"
                                            required
                                            name="miles2"
                                            min="0" />
                                        <span class="input-group-addon input-group-addon-remove-border">
                                            <pr-validator form-control="currentInterSiteForm.miles2" field-name="(21-40)" only-when-touched>
                                        </span>
                                    </div>
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="control-label col-sm-3">41-60</label>
                                <div class="col-sm-9" pr-show-errors>
                                    <div class="input-group">
                                        <input
                                            type="text"
                                            ng-currency
                                            currency-symbol="£"
                                            class="form-control rounded"
                                            ng-model="currentInterSiteCharge.miles3"
                                            required
                                            name="miles3"
                                            min="0" />
                                        <span class="input-group-addon input-group-addon-remove-border">
                                            <pr-validator form-control="currentInterSiteForm.miles3" field-name="(41-60)" only-when-touched>
                                        </span>
                                    </div>
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="control-label col-sm-3">61-100</label>
                                <div class="col-sm-9" pr-show-errors>
                                    <div class="input-group">
                                        <input
                                            type="text"
                                            ng-currency
                                            currency-symbol="£"
                                            class="form-control rounded"
                                            ng-model="currentInterSiteCharge.miles4"
                                            required
                                            name="miles4"
                                            min="0" />
                                        <span class="input-group-addon input-group-addon-remove-border">
                                            <pr-validator form-control="currentInterSiteForm.miles4" field-name="(61-100)" only-when-touched>
                                        </span>
                                    </div>
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="control-label col-sm-3">101-150</label>
                                <div class="col-sm-9" pr-show-errors>
                                    <div class="input-group">
                                        <input
                                            type="text"
                                            ng-currency
                                            currency-symbol="£"
                                            class="form-control rounded"
                                            ng-model="currentInterSiteCharge.miles5"
                                            required
                                            name="miles5"
                                            min="0" />
                                        <span class="input-group-addon input-group-addon-remove-border">
                                            <pr-validator form-control="currentInterSiteForm.miles5" field-name="(101-150)" only-when-touched>
                                        </span>
                                    </div>
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="control-label col-sm-3">151-200</label>
                                <div class="col-sm-9" pr-show-errors>
                                    <div class="input-group">
                                        <input
                                            type="text"
                                            ng-currency
                                            currency-symbol="£"
                                            class="form-control rounded"
                                            ng-model="currentInterSiteCharge.miles6"
                                            required
                                            name="miles6"
                                            min="0" />
                                        <span class="input-group-addon input-group-addon-remove-border">
                                            <pr-validator form-control="currentInterSiteForm.miles6" field-name="(151-200)" only-when-touched>
                                        </span>
                                    </div>
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="control-label col-sm-3">Valid from</label>
                                <div class="col-sm-9" pr-show-errors>
                                    <div class="input-group">
                                        <input type="text"
                                            class="form-control rounded"
                                            ng-model="currentInterSiteCharge.validFrom"
                                            close-text="Close"
                                            name="validFrom"
                                            ng-disabled="true" />
                                        <span class="input-group-addon input-group-addon-remove-border">
                                            <pr-validator form-control="currentInterSiteForm.validFrom" field-name="Valid From" only-when-touched></pr-validator>
                                        </span>
                                    </div>

                                </div>
                            </div>

                            <div class="form-group">
                                <div class="btn btn-primary pull-right" ng-click="addCurrentInterSiteCharge()">Update</div>
                            </div>
                        </div>
                    </form>


                    <form name="futureInterSiteForm">
                        <div class="form-horizontal col-md-6">
                            <h4 class="form-sub-header">Future Inter-site charges:</h4>
                            <hr class="form-sep" />

                            <div ng-show="futureInterSiteCharge">

                                <div class="form-group">
                                    <label class="control-label col-sm-3">0-20</label>
                                    <div class="col-sm-9" pr-show-errors>
                                        <div class="input-group">
                                            <input
                                                type="text"
                                                ng-currency
                                                currency-symbol="£"
                                                class="form-control rounded"
                                                ng-model="futureInterSiteCharge.miles1"
                                                required
                                                name="futureMiles1"
                                                min="0" />
                                            <span class="input-group-addon input-group-addon-remove-border">
                                                <pr-validator form-control="futureInterSiteForm.futureMiles1" field-name="(0-20)" only-when-touched>
                                            </span>
                                        </div>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <label class="control-label col-sm-3">21-40</label>
                                    <div class="col-sm-9" pr-show-errors>
                                        <div class="input-group">
                                            <input
                                                type="text"
                                                ng-currency
                                                currency-symbol="£"
                                                class="form-control rounded"
                                                ng-model="futureInterSiteCharge.miles2"
                                                required
                                                name="miles2"
                                                min="0" />
                                            <span class="input-group-addon input-group-addon-remove-border">
                                                <pr-validator form-control="futureInterSiteForm.miles2" field-name="(21-40)" only-when-touched>
                                            </span>
                                        </div>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <label class="control-label col-sm-3">41-60</label>
                                    <div class="col-sm-9" pr-show-errors>
                                        <div class="input-group">
                                            <input
                                                type="text"
                                                ng-currency
                                                currency-symbol="£"
                                                class="form-control rounded"
                                                ng-model="futureInterSiteCharge.miles3"
                                                required
                                                name="miles3"
                                                min="0" />
                                            <span class="input-group-addon input-group-addon-remove-border">
                                                <pr-validator form-control="futureInterSiteForm.miles3" field-name="(41-60)" only-when-touched>
                                            </span>
                                        </div>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <label class="control-label col-sm-3">61-100</label>
                                    <div class="col-sm-9" pr-show-errors>
                                        <div class="input-group">
                                            <input
                                                type="text"
                                                ng-currency
                                                currency-symbol="£"
                                                class="form-control rounded"
                                                ng-model="futureInterSiteCharge.miles4"
                                                required
                                                name="miles4"
                                                min="0" />
                                            <span class="input-group-addon input-group-addon-remove-border">
                                                <pr-validator form-control="futureInterSiteForm.miles4" field-name="(61-100)" only-when-touched>
                                            </span>
                                        </div>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <label class="control-label col-sm-3">101-150</label>
                                    <div class="col-sm-9" pr-show-errors>
                                        <div class="input-group">
                                            <input
                                                type="text"
                                                ng-currency
                                                currency-symbol="£"
                                                class="form-control rounded"
                                                ng-model="futureInterSiteCharge.miles5"
                                                required
                                                name="miles5"
                                                min="0" />
                                            <span class="input-group-addon input-group-addon-remove-border">
                                                <pr-validator form-control="futureInterSiteForm.miles5" field-name="(101-150)" only-when-touched>
                                            </span>
                                        </div>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <label class="control-label col-sm-3">151-200</label>
                                    <div class="col-sm-9" pr-show-errors>
                                        <div class="input-group">
                                            <input
                                                type="text"
                                                ng-currency
                                                currency-symbol="£"
                                                class="form-control rounded"
                                                ng-model="futureInterSiteCharge.miles6"
                                                required
                                                name="miles6"
                                                min="0" />
                                            <span class="input-group-addon input-group-addon-remove-border">
                                                <pr-validator form-control="futureInterSiteForm.miles6" field-name="(151-200)" only-when-touched>
                                            </span>
                                        </div>
                                    </div>
                                </div>

                                <div class="form-group">
                                    <label class="control-label col-sm-3">Valid from</label>
                                    <div class="col-sm-9" pr-show-errors>
                                        <div class="input-group">
                                            <input type="text"
                                                name="validFrom"
                                                class="form-control rounded"
                                                ng-model="futureInterSiteCharge.validFrom"
                                                datepicker-popup="dd/MM/yy"
                                                close-text="Close"
                                                pr-format-date
                                                pr-valid-date
                                                is-open="futureValidFromOpened"
                                                required
                                                min-date="tommorrow" />
                                            <span class="input-group-btn">
                                                <button type="button" class="btn btn-default" ng-click="openFutureValidFrom($event)">
                                                    <i class="glyphicon glyphicon-calendar"></i>
                                                </button>
                                            </span>
                                            <span class="input-group-addon input-group-addon-remove-border">
                                                <pr-validator form-control="futureInterSiteForm.validFrom" field-name="Valid From" only-when-touched></pr-validator>
                                            </span>
                                        </div>

                                    </div>
                                </div>
                            </div>

                            <div class="form-group">
                                <div class="btn btn-primary pull-right" ng-click="addFutureInterSiteCharge()" ng-show="futureInterSiteCharge">Update</div>
                                <div class="btn btn-primary pull-right" ng-click="populateFutureInterSiteCharge()" ng-show="!futureInterSiteCharge">Add</div>
                            </div>

                        </div>
                    </form>
            </div>
            </div>
            <div class="col-sm-6">
            <div class="row">
                <div class="form-horizontal">
                    <h4 class="form-sub-header">Manage Control Area Costs</h4>
                    <hr class="form-sep" />

                    <div class="form-group">
                        <label class="control-label col-sm-3">Select control area</label>
                        <div class="col-sm-6">
                            <select class="form-control"
                                name="controlAreaDropdown"
                                ng-model="selectedControlArea"
                                ng-options="controlArea.description for controlArea in controlAreas"
                                ng-change="filterChanged()">
                            </select>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row" ng-show="showInterSiteCosts">

                    <form name="currentControlAreaForm">
                        <div class="form-horizontal col-sm-6">
                            <h4 class="form-sub-header">Current Control Area Costs</h4>
                            <hr class="form-sep" />

                            <div class="form-group">
                                <label class="control-label col-sm-3">Fuel Cost Per Mile</label>
                                <div class="col-sm-9" pr-show-errors>
                                    <div class="input-group">
                                        <input
                                            type="text"
                                            ng-currency
                                            currency-symbol="£"
                                            class="form-control rounded"
                                            ng-model="currentControlAreaInterSiteCost.fuelCostPerMile"
                                            required
                                            name="fuelCostPerMile"
                                            min="0" />
                                        <span class="input-group-addon input-group-addon-remove-border">
                                            <pr-validator form-control="currentControlAreaForm.fuelCostPerMile" field-name="Fuel Cost Per Mile" only-when-touched></pr-validator>
                                        </span>
                                    </div>
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="control-label col-sm-3">Driver Cost Per Hour</label>
                                <div class="col-sm-9" pr-show-errors>
                                    <div class="input-group">
                                        <input
                                            type="text"
                                            ng-currency
                                            currency-symbol="£"
                                            class="form-control rounded"
                                            ng-model="currentControlAreaInterSiteCost.driverCostPerHour" 
                                            required
                                            name="driverCostPerHour"
                                            min="0"
                                            />
                                        <span class="input-group-addon input-group-addon-remove-border">
                                            <pr-validator form-control="currentControlAreaForm.driverCostPerHour" field-name="Driver cost Per Hour" only-when-touched></pr-validator>
                                        </span>
                                    </div>
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="control-label col-sm-3">Unit Cost Per Hour</label>
                                <div class="col-sm-9" pr-show-errors>
                                    <div class="input-group">
                                        <input
                                            type="text"
                                            ng-currency
                                            currency-symbol="£"
                                            class="form-control rounded"
                                            ng-model="currentControlAreaInterSiteCost.unitCostPerHour" 
                                            required
                                            name="unitCostPerHour"
                                            min="0"
                                            />
                                        <span class="input-group-addon input-group-addon-remove-border">
                                            <pr-validator form-control="currentControlAreaForm.unitCostPerHour" field-name="Unit Cost Per Hour" only-when-touched></pr-validator>
                                        </span>
                                    </div>
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="control-label col-sm-3">Trailer Cost Per Hour</label>
                                <div class="col-sm-9" pr-show-errors>
                                    <div class="input-group">
                                        <input
                                            type="text"
                                            ng-currency
                                            currency-symbol="£"
                                            class="form-control rounded"
                                            ng-model="currentControlAreaInterSiteCost.trailerCostPerHour" 
                                            required
                                            name="trailerCostPerHour"
                                            min="0"
                                            />
                                        <span class="input-group-addon input-group-addon-remove-border">
                                            <pr-validator form-control="currentControlAreaForm.trailerCostPerHour" field-name="Trailer Cost Per Hour" only-when-touched></pr-validator>
                                        </span>
                                    </div>
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="control-label col-sm-3">Valid from</label>
                                <div class="col-sm-9" pr-show-errors>
                                    <div class="input-group">
                                        <input type="text"
                                            class="form-control rounded"
                                            ng-model="currentControlAreaInterSiteCost.validFrom"
                                            close-text="Close"
                                            name="validFrom"
                                            ng-disabled="true" />
                                        <span class="input-group-addon input-group-addon-remove-border">
                                            <pr-validator form-control="currentControlAreaForm.validFrom" field-name="Valid From" only-when-touched></pr-validator>
                                        </span>
                                    </div>

                                </div>
                            </div>
                            <div class="form-group">
                                <div class="btn btn-primary pull-right" ng-click="addCurrentControlAreaInterSiteCost()">Update</div>
                            </div>
                        </div>
                                                    
                        </form>


                    <div class="form-horizontal col-md-6" >
                <form name="futureInterSiteControlAreaForm">
                    
                        <h4 class="form-sub-header">Future Control Area Costs</h4>
                        <hr class="form-sep" />

                        <div ng-show="futureControlAreaInterSiteCost">
                            <div class="form-group">
                                <label class="control-label col-sm-3">Fuel Cost Per Mile</label>
                                <div class="col-sm-9" pr-show-errors>
                                    <div class="input-group">
                                        <input
                                            type="text"
                                            ng-currency
                                            currency-symbol="£"
                                            class="form-control rounded"
                                            ng-model="futureControlAreaInterSiteCost.fuelCostPerMile"
                                            required
                                            name="futureFuelCostPerMile"
                                            min="0" />
                                        <span class="input-group-addon input-group-addon-remove-border">
                                            <pr-validator form-control="futureInterSiteControlAreaForm.futureFuelCostPerMile" field-name="Fuel Cost Per Mile" only-when-touched></pr-validator>
                                        </span>
                                    </div>
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="control-label col-sm-3">Driver Cost Per Hour</label>
                                <div class="col-sm-9" pr-show-errors>
                                    <div class="input-group">
                                        <input
                                            type="text"
                                            ng-currency
                                            currency-symbol="£"
                                            class="form-control rounded"
                                            ng-model="futureControlAreaInterSiteCost.driverCostPerHour" 
                                            required
                                            name="futureDriverCostPerHour"
                                            min="0"
                                            />
                                        <span class="input-group-addon input-group-addon-remove-border">
                                            <pr-validator form-control="futureInterSiteControlAreaForm.futureDriverCostPerHour" field-name="Driver Cost Per Hour" only-when-touched></pr-validator>
                                        </span>
                                    </div>
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="control-label col-sm-3">Unit Cost Per Hour</label>
                                <div class="col-sm-9" pr-show-errors>
                                    <div class="input-group">
                                        <input
                                            type="text"
                                            ng-currency
                                            currency-symbol="£"
                                            class="form-control rounded"
                                            ng-model="futureControlAreaInterSiteCost.unitCostPerHour" 
                                            required
                                            name="futureUnitCostPerHour"
                                            min="0"
                                            />
                                        <span class="input-group-addon input-group-addon-remove-border">
                                            <pr-validator form-control="futureInterSiteControlAreaForm.futureUnitCostPerHour" field-name="Unit Cost Per Hour" only-when-touched></pr-validator>
                                        </span>
                                    </div>
                                </div>
                            </div>

                            <div class="form-group">
                                <label class="control-label col-sm-3">Trailer Cost Per Hour</label>
                                <div class="col-sm-9" pr-show-errors>
                                    <div class="input-group">
                                        <input
                                            type="text"
                                            ng-currency
                                            currency-symbol="£"
                                            class="form-control rounded"
                                            ng-model="futureControlAreaInterSiteCost.trailerCostPerHour" 
                                            required
                                            name="futureTrailerCostPerHour"
                                            min="0"
                                            />
                                        <span class="input-group-addon input-group-addon-remove-border">
                                            <pr-validator form-control="futureInterSiteControlAreaForm.futureTrailerCostPerHour" field-name="Trailer Cost Per Hour" only-when-touched></pr-validator>
                                        </span>
                                    </div>
                                </div>
                            </div>

                                <div class="form-group">
                                    <label class="control-label col-sm-3">Valid from</label>
                                    <div class="col-sm-9" pr-show-errors>
                                        <div class="input-group">
                                            <input type="text"
                                                class="form-control rounded"
                                                ng-model="futureControlAreaInterSiteCost.validFrom"
                                                datepicker-popup="dd/MM/yy"
                                                close-text="Close"
                                                pr-format-date
                                                pr-valid-date
                                                is-open="futureControlAreaValidFromOpened"
                                                name="futureValidFrom"
                                                min-date="tommorrow" />
                                            <span class="input-group-btn">
                                                <button type="button" class="btn btn-default" ng-click="openFutureControlAreaValidFrom($event)">
                                                    <i class="glyphicon glyphicon-calendar"></i>
                                                </button>
                                            </span>
                                            <span class="input-group-addon input-group-addon-remove-border">
                                                <pr-validator form-control="futureInterSiteControlAreaForm.futureValidFrom" field-name="Valid From" only-when-touched></pr-validator>
                                            </span>
                                        </div>

                                    </div>
                                </div>

                        </div>

                    
                            <div class="form-group">
                                <div class="btn btn-primary pull-right" ng-click="addFutureControlAreaInterSiteCost()" ng-show="futureControlAreaInterSiteCost">Update</div>
                                <div class="btn btn-primary pull-right" ng-click="populateFutureControlAreaIntersiteCost()" ng-show="!futureControlAreaInterSiteCost">Add</div>
                            </div>
                    </form>
                    </div>


                
                
                

            </div>
                </div>
        </div>
    </div>
</asp:Content>
