osmanApp.directive("firstDirective", function () {
    var result = {
        restrict: "E", //Sadece html elemanı (tag) olarak kullanılabilir.
        template: "Merhaba Dünya, bu ilk directivim, yuppi.."
    };
    return result;
});

osmanApp.directive('directiveScope', function () {
    return {
        restrict: "EA",
        scope: {},  // use a new isolated scope rather then global scobe of the controller
        template: 'Name: {{customer.name}} Address: {{customer.address}}'
    };
});

//Isolated scope
osmanApp.directive('colorMe', function () {
    return {
        scope: {
            pattern:'@colorPattern',
            color: '@'
        },
        restrict: 'AE',
        replace: true,
        template: '<p style="background-color:{{color}};">I was colored by {{pattern}}</p>',
    };
});

//Isolated scope @ example
osmanApp.directive('newColorMe', function () {
    return {
        scope: {
            color: '=',
            title:'@'
        },
        restrict: 'AE',
        replace: true,
        template: '<div><input type="text" ng-model="color" placeholder="Enter a color for isolated scope" />' +
                    '<p style="background-color:{{color}}">I was colored by {{title}}</p></div>',
    };
});

//& Example
osmanApp.directive("reversable", function () {
    return {
        restrict: 'E',
        scope: {
            reverse: "&reversemethod"
        },
        template: '<input type="text" ng-model="message"/>{{message}}<button class="btn btn-success" ng-click="reverse({msg:message})">Terse Çevir</button>'
    };
});

//Link Function
osmanApp.directive("linkExample", function () {
    return {
        restrict: 'AE',
        replace: true,
        template: '<p style="background-color:{{color}}">Hello World</p>',
        link: function (scope, elem, attrs) {
            elem.bind('click', function () {
                elem.css('background-color', 'white');
                scope.$apply(function () {
                    scope.color = "white";
                });
            });
            elem.bind('mouseover', function () {
                elem.css('cursor', 'pointer');
            });
        }
    };
});

//Transclude Example
osmanApp.directive("asetil", function () {
    return {
        scope: {},
        restrict: 'E',
        transclude: true,
        template: '<div style="background:#eee;padding:20px;">This is a panel componenent <span ng-transclude style="color:red;">Osman</p></div>'
    };
});

osmanApp.directive('myDraggable', ['$document', function ($document) {
    return function (scope, element, attr) {
        var startX = 0, startY = 0, x = 0, y = 0;

        element.css({
            position: 'relative',
            border: '1px solid red',
            backgroundColor: 'lightgrey',
            cursor: 'pointer'
        });

        element.on('mousedown', function (event) {
            // Prevent default dragging of selected content
            event.preventDefault();
            startX = event.pageX - x;
            startY = event.pageY - y;
            $document.on('mousemove', mousemove);
            $document.on('mouseup', mouseup);
        });

        function mousemove(event) {
            y = event.pageY - startY;
            x = event.pageX - startX;
            element.css({
                top: y + 'px',
                left: x + 'px'
            });
        }

        function mouseup() {
            $document.off('mousemove', mousemove);
            $document.off('mouseup', mouseup);
        }
    };
}]);





//Controller Usage
osmanApp.directive('outerDirective', function () {
    return {
        scope: {},
        restrict: 'AE',
        controller: function ($scope, $compile, $http) {
            // $scope is the appropriate scope for the directive
            this.addChild = function (nestedDirective) { // this refers to the controller
                console.log("Directive Controller Example Output :");
                console.info('Got the message from nested directive:',nestedDirective.message);
            };
        }
    };
});

osmanApp.directive('innerDirective', function () {
    return {
        scope: {},
        restrict: 'AE',
        require: '^outerDirective',
        link: function (scope, elem, attrs, controllerInstance) {
            //the fourth argument is the controller instance you require
            scope.message = "Hi, Parent directive";
            controllerInstance.addChild(scope);
        }
    };
});