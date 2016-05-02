var app = angular.module('MobileWebGraphs', ['ionic']);

app.controller('TheController', function ($scope, $ionicPopup) {
    $scope.deleteSelected = function () {
        WebGraphs.TheGraphCanvas.deleteSelected();
    }

    $scope.openFolderPopup = function () {
        $scope.data = {};

        var pasteWebGraphPopup = $ionicPopup.show({
            template: '<input type="text" ng-model="data.webgraph">',
            title: 'paste webgraph',
            scope: $scope,
            buttons: [
              { text: 'cancel' },
              {
                  text: 'load',
                  type: 'button-positive',
                  onTap: function (e) {
                      if (!$scope.data.webgraph) {
                          e.preventDefault();
                      } else {
                          return $scope.data.webgraph;
                      }
                  }
              }
            ]
        });

        pasteWebGraphPopup.then(function (webgraph) {
            WebGraphs.TheGraphCanvas.buildGraphCanvasFromSilverlightFormat(webgraph);
        });
    }
});

angular.element(document).ready(function () {
    if (window.cordova) {
        console.log("Running in Cordova, will bootstrap AngularJS once 'deviceready' event fires.");
        document.addEventListener('deviceready', function () {
            console.log("Deviceready event has fired, bootstrapping AngularJS.");
            angular.bootstrap(document.body, ['MobileWebGraphs']);
        }, false);
    } else {
        console.log("Running in browser, bootstrapping AngularJS now.");
        angular.bootstrap(document.body, ['MobileWebGraphs']);
    }
});