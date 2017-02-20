'use strict';

angular.module('peApp').directive('prInlineEditFocus', ['$timeout', function ($timeout) {
    return function(scope, element, attrs) {
        scope.$watch('editing', 
          function (newValue) { 
              $timeout(function() {
                  newValue && element[0].focus();
              }, 0, false);
          });
    };    
}]);
