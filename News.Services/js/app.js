/// <reference path="libs/_references.js" />
(function () {

    var router = new kendo.Router();
    var navigationLayout = new kendo.Layout('<div id="navigation"/>');
    var categoriesLayout = new kendo.Layout('<div id="categories"/>');
    var tagsLayout = new kendo.Layout('<div id="tags"/>');
    var articlesLayout = new kendo.Layout('<div id="articles"/>');

    //var viewModels = viewModelsFactory.get();
    //var views = viewsFactory.get("");
    var controller = uiController.get();
    
    controller.getCategoriesView()
        .then(function (view) {
            categoriesLayout.showIn("#categories", view);
        });

    controller.getTagsView()
        .then(function (view) {
            tagsLayout.showIn("#tags", view);
        });
    
    

    router.route("/home", function () {
        var navigationView = controller.getNavigationView();
        navigationLayout.showIn("#navigation", navigationView);

        controller.getArticlesView()
            .then(function (articlesView) {
                articlesLayout.showIn("#articles", articlesView);
            });
    });

    router.route("/login", function () {
        var navigationView = controller.getNavigationView();
        navigationLayout.showIn("#navigation", navigationView);

        var loginView = controller.getLoginView(function () {
            router.navigate("/home");
        });
        articlesLayout.showIn("#articles", loginView);
        $(document).ready(function () {
            $("#tabs").kendoTabStrip({
                animation: {
                    open: {
                        effects: "fadeIn"
                    }
                }
            });
        });
    });

    router.route("/categories/:id", function(id) {
        var navigationView = controller.getNavigationView();
        navigationLayout.showIn("#navigation", navigationView);
        
        controller.getCategoryArticlesView(id)
            .then(function (articlesView) {
                articlesLayout.showIn("#articles", articlesView);
            });
    });

    router.route("/logout", function() {
        var navigationView = controller.getNavigationView();
        navigationLayout.showIn("#navigation", navigationView);

        controller.getLogoutView()
            .then(function() {
                router.navigate("/home");
            });
    });

    $(function () {
        navigationLayout.render("#header");
        categoriesLayout.render("#categories-container");
        tagsLayout.render("#tags-container");
        articlesLayout.render("#content");
        router.start();
        //router.navigate("/home");
    });
})();