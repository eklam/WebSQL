var app = angular.module('WebSQLApp', ['ngGrid']);
app.controller('ResultsGridCtrl', function ($scope) {
    //$scope.myData = [{ name: "Moroni", age: 50 },
    //                 { name: "Tiancum", age: 43 },
    //                 { name: "Jacob", age: 27 },
    //                 { name: "Nephi", age: 29 },
    //                 { name: "Enos", age: 34 }];
    $scope.myData = [];
    $scope.myCols = [];
    $scope.gridOptions = { data: 'myData', enableColumnResize: true, columnDefs: 'myCols' };

    
});