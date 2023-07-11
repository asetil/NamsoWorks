webmarketApp.controller('regionCtrl', function ($scope, regionService) {
    $scope.regions = [];
    $scope.orderProp = 'sortOrder';
    $scope.selectedRegion = { ID: 0 };

    regionService.getRegions()
        .then(function (result) {
            $scope.regions = result.districts;
            for (var i in $scope.regions) {
                var region = $scope.regions[i];
                if (region.ID == $scope.selectedRegion.ID) {
                    $scope.selectedRegion = region;
                    break;
                }
            }
        });

    $scope.saveSelected = function (rid) {
        $scope.selectedRegion = rid;
    }

    $scope.completeSelection = function () {
        if ($scope.selectedRegion == undefined) {
            aware.showMessage('Semt Seçimi Yapmadınız', 'Devam etmeden önce semt seçimi yapmalısınız!', 'fail', 'fa-minus-circle');
            return false;
        }
        regionService.saveRegion($scope.selectedRegion.ID)
        .then(function (result) {
            var message = "<i class='fa fa-check'></i> Semtiniz " + $scope.selectedRegion.Name + " olarak güncellendi.<br/> <span style='font-size:14px;color:#fff;'>Anasayfaya yönlendiriliyorsunuz..</span>";
            aware.showLoading(message, true);
            aware.delayedRefresh(1200, "/");
        });
    }
});

webmarketApp.controller('testController', function ($scope) {
    $scope.world = "dünya";
});