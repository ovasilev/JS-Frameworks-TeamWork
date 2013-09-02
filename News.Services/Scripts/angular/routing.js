angular.module('adminPanel', []).
  config(['$routeProvider', function ($routeProvider) {
      $routeProvider.
          when('/admin', {
              templateUrl: 'scripts/angular/partials/adminHome.html',
              controller: angularController.homeController
          }).
           when('/admin/articles/:articleId', {
               templateUrl: 'scripts/angular/partials/adminArticleEdit.html',
               controller: angularController.articleController.articleEdit
           }).
          when('/admin/articles', {
              templateUrl: 'scripts/angular/partials/adminArticles.html',
              controller: angularController.articleController.general
          }).
          when('/admin/users', {
              templateUrl: 'scripts/angular/partials/adminUsers.html',
              controller: angularController.userController
          }).
          otherwise({ redirectTo: '/' });
  }]);