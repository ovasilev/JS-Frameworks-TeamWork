/// <reference path="../libs/_references.js" />

window.viewModelsFactory = (function () {

    var apiUrl = "http://thalliumnewssite.apphb.com/api";

    var viewModels = Class.create({
        init: function () {
            this.persister = persister.getData(apiUrl);
        },
        getArticlesViewModel: function () {
            return this.persister.articles.all()
                .then(function (articles) {
                    for (var i = 0; i < articles.length; i++) {
                        articles[i].datePublished = articles[i].datePublished.substr(0, 10);
                    }

                    return new kendo.observable({
                        articles: articles
                    });
                });
        },
        getCategoryArticlesViewModel: function (id) {
            return this.persister.categories.single(id)
                .then(function (articles) {
                    return new kendo.observable({
                        articles: articles
                    });
                });
        },
        getNavigationViewModel: function () {
            var userStatus = this.persister.userStatus();

            if (userStatus == "admin") {
                return new kendo.observable({                    
                    navigationItems: [
                        { title: "Home", url: "#/home" },
                        { title: "Popular Articles", url: "#/popular" },
                        { title: "Contacts", url: "#/contacts" },
                        { title: "Admin Panel", url: "#/admin" },
                        { title: "Logout", url: "#/logout" }
                    ]
                });
            } else if (userStatus == "user") {
                return new kendo.observable({
                    navigationItems: [
                        { title: "Home", url: "#/home" },
                        { title: "Popular Articles", url: "#/popular" },
                        { title: "Contacts", url: "#/contacts" },
                        { title: "Logout", url: "#/logout" }
                    ]
                });
            } else {
                return new kendo.observable({
                    navigationItems: [
                        { title: "Home", url: "#/home" },
                        { title: "Popular Articles", url: "#/popular" },
                        { title: "Contacts", url: "#/contacts" },
                        { title: "Login/Register", url: "#/login" }
                    ]
                });
            }
        },
        getCategoriesViewModel: function () {
            return this.persister.categories.all()
                .then(function (categories) {
                    for (var i = 0; i < categories.length; i++) {
                        categories[i].url = "#/categories/" + categories[i].id + "/articles";
                    }

                    return new kendo.observable({
                        categories: categories
                    });
                });
        },
        getTagsViewModel: function () {
            return this.persister.tags.all()
                .then(function (tags) {
                    for (var i = 0; i < tags.length; i++) {
                        tags[i].url = "#/tags/" + tags[i].id + "/articles";
                    }

                    return new kendo.observable({
                        tags: tags
                    });
                });
        },
        getLoginViewModel: function (success) {
            var self = this;
            var viewModel = {
                username: "",
                displayName: "",
                password: "",
                login: function () {
                    self.persister.users.login(this.get("username"), this.get("password"))
                        .then(function () {
                            success();
                        });
                },
                register: function () {
                    self.persister.users.register(this.get("username"), this.get("displayName"), this.get("password"))
                        .then(function () {
                            success();
                        });
                }
            };
            return kendo.observable(viewModel);
        },
        getLogoutViewModel: function() {
            return this.persister.users.logout();
        }
    });

    return {
        get: function () {
            return new viewModels();
        }
    };
})();