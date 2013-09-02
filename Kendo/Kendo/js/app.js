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

    controller.getNavigationView()
           .then(function (view) {
               navigationLayout.showIn("#navigation", view);
           });

    controller.getCategoriesView()
        .then(function (view) {
            categoriesLayout.showIn("#categories", view);
        });

    controller.getTagsView()
        .then(function (view) {
            tagsLayout.showIn("#tags", view);
        });

    router.route("/home", function () {

        controller.getArticlesView()
            .then(function (view) {
                articlesLayout.showIn("#articles", view);
            });
    });

    router.route("/login", function () {
        var view = controller.getLoginView(function () {
            router.navigate("/home");
        });
        articlesLayout.showIn("#articles", view);
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


    $(function () {
        navigationLayout.render("#header");
        categoriesLayout.render("#categories-container");
        tagsLayout.render("#tags-container");
        articlesLayout.render("#content");
        router.start();
        //router.navigate("/home");
    });
})();