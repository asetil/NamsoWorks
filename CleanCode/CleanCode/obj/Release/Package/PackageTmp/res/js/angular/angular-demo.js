var demoApp = angular.module('demoApp', ['ngResource', 'ngRoute']);
demoApp.controller('demoCntrl', function ($scope, $filter) {
    $scope.colortext = "#dc143c";
    $scope.newcolortext = "green";
    $scope.customer = { name: "Osman", company: "Aware.co" };
    $scope.student = { name: "ali Veli", age: 17 };

    $scope.reverseMe = function (msg) {
        //msg = $filter('reverseText')(msg);
        alert($filter('reverseText')(msg));
    };

    $scope.showMessage = function () {
        alert("showMessage metodu parent scope'dan çağrıldı.");
    };

});

//Isolated scope
demoApp.directive('colorMe', function () {
    return {
        scope: {
            pattern: '@colorPattern',
            color: '@'
        },
        restrict: 'AE',
        replace: true,
        template: '<p style="background-color:{{color}};padding:5px 10px;">Grilen renk : {{color}}, pattern : {{pattern}}</p>',
    };
});

//Isolated scope = example
demoApp.directive('newColorMe', function () {
    return {
        scope: {
            color: '=',
            title: '@'
        },
        restrict: 'AE',
        replace: true,
        template: '<div class="mt20"><p>Directive isolated scope : </p>' +
        '<input type="text" class="form-control" ng-model="color" placeholder="Enter a color for isolated scope" />' +
        '<p class="mt10" style="background-color:{{color}};padding:10px;">I was colored by {{title}}</p></div>',
    };
});

//Isolation Scope & Example
demoApp.directive("reversable", function () {
    return {
        restrict: 'E',
        scope: {
            reverse: "&reversemethod"
        },
        template: '<input type="text" class="form-control" ng-model="message"/><button class="btn btn-success" ng-click="reverse({msg:message})">Terse Çevir</button>'
    };
});

//Transclude Example
demoApp.directive("transcludeMe", function () {
    return {
        restrict: 'E',
        transclude: true,
        template: '<div style="text-align:left;padding:0;">' +
        '<span style="color:#dc143c;">Bu içerik directive şablonundan geliyor.</span>' +
        '<span ng-transclude style="color:blue;">Transclude içeriği buraya gelecek</span>' +
        '</div>'
    };
});

demoApp.directive("compileDemo", function () {
    return {
        restrict: 'A',
        compile: function (element, attributes) {
            element.css("border", "1px solid #cccccc");
            var deneme = attributes.deneme; //accessing an attribute
            console.log(" attributes.deneme : ", deneme);

            //linkFunction is linked with each element with scope to get the element specific data.
            var linkFunction = function ($scope, element, attributes) {
                element.html("Student: " + $scope.student.name + ", Age: " + $scope.student.age);
                element.css("background-color", "#ff00ff");
                element.css("color", "#fff");
            }
            return linkFunction;
        }
    };
});

demoApp.directive("linkDemo", function () {
    return {
        restrict: 'AE',
        replace: true,
        template: '<p style="background-color:#000;color:{{colortext}}">Buraya Tıkla</p>',
        link: function (scope, elem, attrs) {
            elem.bind('click', function () {
                elem.css('background-color', '#f00');
                scope.$apply(function () {
                    scope.colortext = "blue";
                });
            });

            elem.bind('mouseover', function () {
                elem.css('cursor', 'pointer');
                elem.css('background-color', 'yellowgreen');
            });
        }
    };
});

demoApp.filter('reverseText', function () {
    return function (input) {
        return input.split("").reverse().join("");
    };
});