var app = angular.module('MobileWebGraphs', []);

app.controller('TheController', function ($scope) {
    $scope.deleteSelected = function () {
        WebGraphs.TheGraphCanvas.deleteSelected();
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