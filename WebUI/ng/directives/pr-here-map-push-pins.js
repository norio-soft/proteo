
'use strict';

angular.module('peApp').directive('prHereMapPushPins', ['$compile', '$interpolate', function ($compile, $interpolate) {
    return {
        template: '<div ng-transclude></div>',
        restrict: 'EA',
        transclude: true,
        scope: {
            defaultOptions: '=?',
            pins: '=',
            collectionId: '@',
            liveUpdate: '=?',
            selectedPinId: '=',
            trackedPinId: '='
        },
        require: '^prHereMap',
        compile: function (tElem, tAttrs) {
            return function link(scope, element, attrs, mapCtrl, linker) {

                // ============== Variables ======================

                var pinStates = {};
                var liveUpdate = (scope.liveUpdate === true);
                var oldSelectedPinId = null;
                var oldTrackedPinId = null;

                // ============ Helper Functions ===============


                function getPin(pinId) {
                    return _.find(scope.pins, function (pin) {
                        return pin.id === pinId;
                    });
                }

                function createPinElement(pin) {

                        pinStates[pin.id] = {
                            scope: scope.$new(),
                        }                      

                        pinStates[pin.id].scope.pin = pin;
                        pinStates[pin.id].scope.mapOptions = mapCtrl.mapOptions;

                        linker(pinStates[pin.id].scope, function (clone) {
                            // clone the transcluded element, passing in the new scope. 
                            pinStates[pin.id].element = clone.filter('#pin-template');
                        });
                                
                }

                function getPlaceholder(pin) {
                    var pinId = getPinID(pin)
                    return '<div class="pin-placeholder" id="' + pinId + '"></div>';

                }
             

                function getPinID(pin) {
                    if (!pin) {
                        debugger;
                    }
                    return scope.collectionId + '-' + pin.id;
                }

                function mergeOptions(dstOptions, srcOptions) {
                    if (!dstOptions && !srcOptions) {
                        return {};
                    }

                    if (!dstOptions) {
                        return angular.copy(srcOptions);
                    }

                    if (!srcOptions) {
                        return dstOptions;
                    }

                    var copy = angular.copy(srcOptions);

                    return angular.extend(copy, dstOptions);

                }

                // Gets the pins from the here maps objects array into
                // an array
                function getPinObjectArray() {

                    var objects = mapCtrl.map.getObjects()
                    return  _.filter(objects, function (object) {
                        return (object.entityType == 'pin' && object.collectionId == scope.collectionId) 
                    });
                }

                function onDomIconAttach(clonedElement, domIcon, domMarker) {
                    $(clonedElement).empty;
                    $(clonedElement).append(pinStates[domMarker.id].element);
                    domMarker.setZIndex(1);

                }

                function onDomIconDetach(clonedElement, domIcon, domMarker) {
                    if (pinStates[domMarker.id]) {
                        pinStates[domMarker.id].element.detach();
                    }
                }

                function createHereMapMarker(pin) {
                    var icon = new H.map.DomIcon(getPlaceholder(pin),
                        {
                            onAttach: onDomIconAttach,
                            onDetach: onDomIconDetach
                    });
                    return new H.map.DomMarker(new H.geo.Point(pin.lat, pin.lng), { icon: icon});
                }


                //redraws pins, adding, modifying or removing the associated here maps objects
                function updatePins() {

                    var pinObjects = getPinObjectArray();

                    // check for pins that have been deleted
                    pinObjects.forEach(function (pinObject, index, pins) {

                        var existingPin = _.findWhere(scope.pins, { id: pinObject.id });

                        // pin object no longer exists in our collection of pins, so delete it
                        if (!existingPin) {
                            if (!pinStates[pinObject.id]) {
                                debugger;
                            }

                            pinStates[pinObject.id].scope.$destroy();
                            pinStates[pinObject.id].element.remove();
                            mapCtrl.map.removeObjects([pinObject]);
                            delete pinStates[pinObject.id];

                            if (scope.selectedPinId === pinObject.id) {
                                scope.selectedPinId = null;
                            }

                            if (scope.trackedPinId === pinObject.id) {
                                scope.trackedPinId = null;
                            }
                        }

                      

                    });

                    // check for pins that have been added/updated
                    scope.pins.forEach(function (pin, index, pins) {

                        var existingPinObject = _.findWhere(pinObjects, { id: pin.id });

                        // corresponding entity pin doesn't exist, so add it
                        if (!existingPinObject) {
                            createPinElement(pin);
                            var newPin = createHereMapMarker(pin)
                            newPin.id = pin.id;
                            newPin.entityType = 'pin';
                            newPin.collectionId = pin.collectionId = scope.collectionId;                          
                            // dptodo: rethink "options" maybe not relevant anymore
                            pin.options = newPin.options = mergeOptions(pin.options, scope.defaultOptions);
                            pinStates[pin.id].scope.pin = pin;
                            mapCtrl.map.addObject(newPin);


                        }
                        else {

                            if (scope.selectedPinId === pin.id) {
                                pin.isSelected = true;
                            }

                            if (scope.trackedPinId === pin.id) {
                                pin.isTracked = true;
                            }

                            // otherwise update it
                            var position = existingPinObject.getPosition();
                            pin.options = mergeOptions(pin.options, scope.defaultOptions);
                            pin.options = mergeOptions(pin.options, existingPinObject.options)


                            if (position.lat != pin.lat ||
                                position.lng != pin.lng) {
                                existingPinObject.setPosition(new H.geo.Point(pin.lat, pin.lng));

                            }
                            
                            // update the pin data
                            pinStates[pin.id].scope.pin = pin;
                      

                        }
                    });

                }

              

                // ============== maps event handlers =============


                // ============== Watches ======================

                if (liveUpdate) {
                    scope.$watch('pins', updatePins, true);
                }
                else {
                    scope.$watchCollection('pins', updatePins);
                }


                scope.$on('$destroy', function () {
                    mapCtrl.map.removeObjects(mapCtrl.map.getObjects());

                    for (var pinStateId in pinStates) {
                        if (pinStates.hasOwnProperty(pinStateId)) {
                            pinStates[pinStateId].scope.$destroy();
                            pinStates[pinStateId].element.remove();
                        }
                    }


                });

                scope.$on('pinSelectedEmitUp', function (event, pin) {

                    if (scope.selectedPinId && scope.selectedPinId !== pin.id) {

                        var selectedPin = getPin(scope.selectedPinId)
                        if (selectedPin)
                            selectedPin.isSelected = false;
                    }

                    scope.selectedPinId = pin.id;
                    oldSelectedPinId = pin.id;

                    //scope.trackedPinId = pin.id;

                });

                scope.$watch('selectedPinId', function (selectedPinId) {

                    if (oldSelectedPinId) {
                        var oldSelectedPin = getPin(oldSelectedPinId);
                        if (oldSelectedPin)
                            oldSelectedPin.isSelected = false;
                    }

                    if (selectedPinId) {                  
                        var newSelectedPin = getPin(selectedPinId);
                        if (newSelectedPin) {
                            newSelectedPin.isSelected = true;
                            oldSelectedPinId = selectedPinId;
                        }
                    }

                });

                scope.$on('pinTrackedEmitUp', function (event, pin) {

                    if (pin.isTracked) {
                        if (scope.trackedPinId && scope.trackedPinId !== pin.id) {

                            var trackedPin = getPin(scope.trackedPinId)
                            if (trackedPin)
                                trackedPin.isTracked = false;
                        }

                        scope.trackedPinId = pin.id;
                        oldTrackedPinId = pin.id;

                    }
                    else if (pin.id === scope.trackedPinId)
                    {
                        scope.trackedPinId = null;
                        oldTrackedPinId = null;
                    }

                });

                scope.$watch('trackedPinId', function (trackedPinId) {

                    if (oldTrackedPinId) {
                        var oldTrackedPin = getPin(oldTrackedPinId);
                        if (oldTrackedPin)
                            oldTrackedPin.isTracked = false;
                    }

                    if (trackedPinId) {
                        var newTrackedPin = getPin(trackedPinId);
                        if (newTrackedPin) {
                            newTrackedPin.isTracked = true;
                            oldTrackedPinId = trackedPinId;
                        }
                    }

                });

                // ============ Initialization ===============
                updatePins();

            }
        }
    }

}]);


