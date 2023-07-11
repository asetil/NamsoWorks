var osmanApp = angular.module('osmanApp', ['ngResource', 'ngRoute']);

osmanApp.config(function ($routeProvider) {
    $routeProvider.when('/', {
        template: '{{model.message}}',
        controller:"ngViewCntr"
    }).when('/home222', {
        template: 'Evine hoşgeldin!'
    }).otherwise( {
        redirectTo:'/home'
    });
});


var zippyApp = angular.module('zippyApp', ['ngResource', 'ngRoute']);
zippyApp.directive("zippy", function () {
    return {
        restrict: "E",
        transclude: true,
        scope: {
            title:"@"
        },
        template: '<div><h3 ng-click="toggleContent()">{{title}}</h3>' +
            '<div ng-show="isContentVisible" ng-transclude></div></div>',
        link: function (scope) {
            scope.isContentVisible = false;
            scope.toogleContent= function() {
                scope.isContentVisible = !scope.isContentVisible;
            }
        }
    };
});