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
            console.log('pasted: ', webgraph);

            //var vv = ascii85.decode('<~7n3OZ!*]F\'!!\",eJ-7\\P\\-lgI!!((q`!Ja7paU5!63_Ns7/-lb!<<-#/-,tU\'``c5IXHTlIXV>,&-3\\#!uDMtI\"$M~>');
            //var vvv = unpack(vv);
            //var wxx = QLZ.decompress(vvv);
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