/// <reference path="../libs/_references.js" />

window.viewModelsFactory = (function () {

    var apiUrl = "http://thalliumnewssite.apphb.com/api";

    var viewModels = Class.create({
        init: function () {
            this.persister = persister.getData(apiUrl);
        },
        getArticlesViewModel: function () {
            var promise = new RSVP.Promise(function (resolve, reject) {
                $.ajax({
                    url: "articles.js",
                    type: "GET",
                    dataType: "json",
                    success: function (data) {
                        resolve(new kendo.observable({
                            articles: data
                        }));
                    },
                    error: function (err) {
                        reject(err);
                    }
                });
            });

            return promise;

            //return this.persister.articles.all()
            //    .then(function(articles) {
            //        return new kendo.Observable({
            //            articles: articles
            //        });
            //    });
        },
        getNavigationViewModel: function () {
            var promise = new RSVP.Promise(function (resolve, reject) {
                $.ajax({
                    url: "menu.js",
                    type: "GET",
                    dataType: "json",
                    success: function (data) {
                        resolve(new kendo.observable({
                            navigationItems: data
                        }));
                    },
                    error: function (err) {
                        reject(err);
                    }
                });
            });

            return promise;
        },
        getCategoriesViewModel: function () {
            var promise = new RSVP.Promise(function (resolve, reject) {
                $.ajax({
                    url: "categories.js",
                    type: "GET",
                    dataType: "json",
                    success: function (data) {
                        resolve(new kendo.observable({
                            categories: data
                        }));
                    },
                    error: function (err) {
                        reject(err);
                    }
                });
            });

            return promise;
        },
        getTagsViewModel: function () {
            var promise = new RSVP.Promise(function (resolve, reject) {
                $.ajax({
                    url: "tags.js",
                    type: "GET",
                    dataType: "json",
                    success: function (data) {
                        resolve(new kendo.observable({
                            tags: data
                        }));
                    },
                    error: function (err) {
                        reject(err);
                    }
                });
            });

            return promise;
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
        }
    });

    return {
        get: function () {
            return new viewModels();
        }
    };
})();