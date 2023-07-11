osmanApp.controller('PhoneListCtrl', function ($scope, $filter) {
    $scope.phones = [
      {
          'name': 'Nexus S',
          'snippet': 'Fast just got faster with Nexus S.',
          'price': 1299
      },
      {
          'name': 'Motorola XOOM™ with Wi-Fi',
          'snippet': 'The Next, Next Generation tablet.',
          'price': 399
      },
      {
          'name': 'Samsung S4',
          'snippet': 'Bu samsung bi harika dostuumm..',
          'price': 950
      },
      {
          'name': 'Apple IPhone 6S',
          'snippet': 'Epılı batasıca, tahtaya gelesi, nomıssız.',
          'price': 2300
      },
       {
           'name': 'Nokia 3310',
           'snippet': 'Horasyus misali genzeriko timsali',
           'price': 240
       }
    ];

    $scope.orderProp = 'price';
    $scope.reverseThis = function(input) {
        return $filter('reverseText')(input);
    };
});

//İmplicit annotation
osmanApp.controller('dynamicController', function ($scope, $http) {
    $http.get('https://jsonplaceholder.typicode.com/posts').success(function (data) {
        $scope.posts = data;// jQuery.parseJSON(data.result);
    });
    $scope.orderProp = 'id';
});

//İnline array notation
osmanApp.controller('serviceCntrl', ['$scope', 'Data', 'GithubFactory', function ($scope, Data, GithubFactory) {
    $scope.data = Data;
    $scope.reversed = function () {
        return $scope.data.message.split("").reverse().join("");
    };

    $scope.search = function () {
        GithubFactory.search($scope.repoName).then(function (data) {
            $scope.results = data.items;
        });
    };


    //productService.getProductList()
    //       .then(function (productList) {
    //           $scope.productList = productList;
    //       }, function () {
    //           //Error message
    //       });

    ////???
    //$scope.$watch('cvList', function () {
    //    if ($scope.cvList) {
    //        $scope.canCreateCv = $scope.cvList.length < 5;
    //        $scope.cvExists = $scope.cvList.length > 0;
    //    } else {
    //        $scope.cvExists = false;
    //    }
    //    //showCampaignModal();
    //});
}]);

//$inject property annotation
var MyController = function ($scope) {
    $scope.customer = {
        name: 'Osman Sokuoğlu',
        address: '1600 Amphitheatre'
    };
};
MyController.$inject = ['$scope'];
osmanApp.controller('myController', MyController);


//for directive example
osmanApp.controller('choreCntrl', function ($scope, $filter) {
    $scope.reverseMe = function (msg) {
        //msg = $filter('reverseText')(msg);
        alert($filter('reverseText')(msg));
    };
});

//ng-view example
osmanApp.controller('ngViewCntr', function ($scope) {
    $scope.model = {
        message: "This is ng-view example output"
    };

    $scope.deneme = 1;
});

//ng-view example
osmanApp.controller('siteController', function ($scope,$sce) {
    $scope.trustedHtml = $sce.trustAsHtml("<span>Merhaba Dünya</span>");
});
