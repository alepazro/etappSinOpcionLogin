var DeviceSelectorDir = angular.module('DeviceSelectorDir', []);

DeviceSelectorDir.directive('deviceselector',
   function () {
       return {
           restrict: 'E',
           scope: {
               fields: '=fields'
           },
           templateUrl: 'templates/deviceSelector.html',
           controller: function ($scope) {
               $scope.imgindex = 0

               $scope.setIndex = function (index) {
                   $scope.imgindex = index;
               }
           }
       };
   });
