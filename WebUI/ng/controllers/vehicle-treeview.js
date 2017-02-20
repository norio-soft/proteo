
'use strict';

angular.module('peApp').controller('VehicleTreeViewCtrl', ['$scope','apiService', function ($scope, apiService) {

    // ========== Helper functions =============
    var uniqueKey = 0;


    // unique key to disambiguate tree nodes (vehicles may appear in multiple groups)
    function addUniqueKey(node) {
        node.uniqKey = uniqueKey++;     
        if (node.children) {
            for (var i = 0; i < node.children.length; i++) {
                addUniqueKey(node.children[i], uniqueKey);
            }                            
        }
    }

    function getVehicleViews() {

        apiService.resourceViews.get({resourceType: 'vehicle'})
            .$promise.then(function (result) {
                addUniqueKey(result[0])
                $scope.mapData.vehicleViews = result[0];
            });
    };

    function selectVehicle(selectedVehicle) {
        // find the corresponding vehicle in the map/table and selected it
        var vehicle = _.findWhere($scope.mapData.vehicle.unfilteredVehicles, { resourceID: selectedVehicle.id });
        if (vehicle) {
            selectPin(vehicle)
        }
        else {
            $scope.mapOptions.zoomToSelectedRequested = false;
        }
        $scope.selection.selectedNode = selectedVehicle;
    }


    function getPin(pinId) {
        return _.find($scope.mapData.vehicle.vehicles, function (pin) {
            return pin.id === pinId;
        });
    }

     function selectPin(pin) {
        var zoomCoords = [pin];
        $scope.zoomToCoordinates(zoomCoords)
        $scope.mapOptions.zoom = 12;
        $scope.mapData.vehicle.selectedPinId = pin.id;
    }


    // ========== Watches ===========

    $scope.$watch('mapData.vehicle.selectedPinId', function (pinId) {

        // if a pin vehicle selected on the map or in the table
        // deselect the currently selected node in the treeview unless
        // it corresponds

        if (!$scope.selection.selectedNode) {
            return;
        }

        if (!pinId) {
            $scope.selection.selectedNode = undefined;
            return;
        }

        var selectedPin = getPin(pinId);

        if (selectedPin && selectedPin.resourceID != $scope.selection.selectedNode.id) {
            $scope.selection.selectedNode = undefined;
        }

    });


    // ========== Scope Functions ===========

    $scope.onSelected = function (node) {

        if (node.children && node.children.length > 0) {
            $scope.mapOptions.vehicle.vehicleViewFilter = node.children;
        }
        else {     
            if ($scope.mapOptions.vehicle.vehicleViewFilter.length > 0) {
                // need to set this kludgy bool so the the zoom which stops the zoom to bounding
                // box which happens because the vehicle view filter has been cleared happening after
                // the zoom to single vehicle we want to do next.
                $scope.mapOptions.zoomToSelectedRequested = true;
                $scope.mapOptions.vehicle.vehicleViewFilter = [];
            }
            selectVehicle(node);

        }
    };

    // ========== Initialisation ===========

    $scope.options = {
        dirSelectable: true,
        injectClasses: {
            "ul": "treeview-ul",
            "li": "treeview-li",
            "liSelected": "treeview-liSelected",
            "iExpanded": "treeview-iExpanded",
            "iCollapsed": "treeview-iCollapsed",
            "iLeaf": "treeview-iLeaf",
            "label": "treeview-label",
            "labelSelected": "treeview-labelSelected"
        },
        equality: function (node1, node2) {
            if (node1 && node2) {
                return node1.uniqKey === node2.uniqKey;
            }
            else {
                return false;
            }
            
        }
    };

    $scope.selection = {
        selectedNode: undefined
    }

    getVehicleViews();

  }]);
