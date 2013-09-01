/// <reference path="../libs/_references.js" />

window.uiController = (function() {

    var controller = Class.create({
        init: function() {
            this.viewModels = viewModelsFactory.get();
            //this.views = viewsFactory.get("");
        },
        getArticlesView: function () {
            return this.viewModels.getArticlesViewModel()
                .then(function (articlesViewModel) {
                    var articlesView = new kendo.View("articles-template",
                        { model: articlesViewModel });

                    return articlesView;
                }, function (err) {
                    console.log(err);
                });
        },
        getNavigationView: function() {
            return this.viewModels.getNavigationViewModel()
                .then(function (navigationViewModel) {
                    var navigationView = new kendo.View("navigation-template",
                        { model: navigationViewModel });

                    return navigationView;
                }, function (err) {
                    console.log(err);
                });
        },
        getCategoriesView: function() {
            return this.viewModels.getCategoriesViewModel()
                .then(function(categoriesViewModel) {
                    var categoriesView = new kendo.View("categories-template",
                        { model: categoriesViewModel });

                    return categoriesView;
                }, function(err) {
                    console.log(err);
                });
        },
        getTagsView: function () {
            return this.viewModels.getTagsViewModel()
                .then(function (tagsViewModel) {
                    var tagsView = new kendo.View("tags-template",
                        { model: tagsViewModel });

                    return tagsView;
                }, function (err) {
                    console.log(err);
                });
        }
    });

    return {        
        get: function() {
            return new controller();
        }    
    };
})();