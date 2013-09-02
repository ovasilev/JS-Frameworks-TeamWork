/// <reference path="../angular.js" />
/// <reference path="../angular.min.js" />
var angularController = (function () {
    //d033e22ae348aeb5660fc2140aec35850c4da997
    var root = '/api/';

    var articleController = (function () {
        function general($scope, $http) {
            $scope.pageManager = new PageManager(5);
            $scope.categoryFilterData;
            $scope.orderByData;
            $scope.pageSizes = [5, 10, 20, 50];
            console.log(window.location.href);

            $http.get(root + 'categories').success(function (cats) {
                $scope.categoryFilterData = cats;
            });

            $http.get(root + 'articles').success(function (s) {
                //s = s.sort(function (x, y) {
                //    return x['datePublished'].localeCompare(y['datePublished']) * -1;
                //});

                $scope.pageManager.loadData(s);
               
                if (s.length > 0) {
                    $scope.orderByData = [
                        { display: 'Title', prop: 'title' },
                        { display: 'Date Created', prop: 'datePublished' },
                        { display: 'Author', prop: 'author' },
                        { display: 'ID', prop: 'id' },
                    ];
                }
            });


            $scope.filterArticles = function () {
                $http.get(root + 'articles').success(function (s) {
                    var data = s;

                    if ($scope.query) {
                        data = _.filter(data, function (a) {
                            return a.title.match(new RegExp($scope.query)) ||
                                a.author.match(new RegExp($scope.query)) ||
                                a.category.match(new RegExp($scope.query));
                        });
                    }

                    if ($scope.categoryFilter) {
                        data = _.filter(data, function (c) { return c.category.name == $scope.categoryFilter });
                    }

                    if ($scope.orderByFilter) {
                        console.log($scope.orderByFilter);

                        var filter = $scope.orderByFilter;
                        if (parseInt(_.first(data)[filter]) && !Date.parse(_.first(data)[filter])) {
                            if ($scope.orderByOption == 'desc') {
                                data = data.sort(function (x, y) { return y[filter] - x[filter] });
                            } else {
                                data = data.sort(function (x, y) { return x[filter] - y[filter] });
                            }
                        } else {
                            if ($scope.orderByOption == 'desc') {
                                data = data.sort(function (x, y) {
                                    return x[filter].localeCompare(y[filter]) * -1});
                            } else {
                                data = data.sort(function (x, y) {
                                    return x[filter].localeCompare(y[filter])});
                            }
                        }
                    }

                    if ($scope.pageSize) {
                        $scope.pageManager.count = parseInt($scope.pageSize);
                    }

                    $scope.pageSize = null;
                    $scope.orderByFilter = null;
                    $scope.query = null;
                    $scope.categoryFilter = null;
                    $scope.orderByOption = null;

                    $scope.pageManager.loadData(data);
                });
            }

            $scope.deleteArticle = function (id) {
                if (!confirm("Are You Sure You Want To Delete This Article ?")) {
                    return;
                }

                console.log(_.filter($scope.pageManager.data, function (a) { return a.id == id; }))

                $scope.pageManager.loadData(
                    _.filter($scope.pageManager.data, function (a) { return a.id != id; }));

            }
        }

        function articleEdit($scope, $http, $routeParams) {
            $scope.article;
            $scope.textVal;
            $scope.selectedComment;
            $scope.editedCommentContent;
            $scope.tagsValue;

            $http.get(root + 'categories').success(function (cats) {
                $scope.articleCategories = cats;
            });

            $http({
                method: 'GET', url: root+ 'articles/' + $routeParams.articleId,
            }).success(function (data) {
                $scope.article = data;
                $scope.textVal = $scope.article.content;
                $scope.articleCategory = $scope.article.category;
                $scope.tagsValue = $scope.article.tags.join();
            });

            $scope.commentEditor = function (id) {
                $scope.selectedComment =
                    _.find($scope.article.comments, function (c) { return c.id == id });

                $scope.editedCommentContent = $scope.selectedComment.content;
                console.log($scope.editedCommentContent);
                $('#comment-editor').css('display', 'inline-block');
                $('#comment-box table').css('display', 'none');
            }

            $scope.cancelCommentEditor = function () {
                $('#comment-editor').css('display', 'none');
                $('#comment-box table').css('display', 'inline-block');
            }

            $scope.acceptCommentEditor = function () {
                $scope.selectedComment.content = $scope.editedCommentContent;
                console.log($scope.article);
                $('#comment-editor').css('display', 'none');
                $('#comment-box table').css('display', 'inline-block');
            }

            $scope.deleteComment = function (id) {
                if (!confirm("Are You Sure You Want To Delete This Comment ?")) {
                    return;
                }

                $scope.article.comments =
                    _.filter($scope.article.comments,function (c) { return c.id != id; });
            }

            $scope.addCategory = function () {
                $scope.articleCategories.push($scope.newCategory);
                $scope.newCategory = '';
            }

            $scope.addTag = function () {
                if ($scope.newTag != '') {
                    $scope.article.tags.push($scope.newTag);
                    $scope.newTag = '';
                }
            }

            $scope.removeTag = function (tag) {
                $scope.article.tags = _.filter($scope.article.tags, function (t) { return t != tag; });
            }

            $scope.saveChanges = function () {
                $scope.article.category = $scope.articleCategory;
                $scope.article.content = $scope.textVal;
                console.log($scope.article);
                $http({
                    method: 'PUT', url: root + 'articles',
                    headers: { 'X-sessionKey': '2gFkjkyZXZSgKzquoKJYCEegyqQdfmQICmiMzMMBJvfyLysrYj' }, data: $scope.article
                }).success(function (data) {
                    console.log('Updated')
                });
                //window.location.reload();

            }

            $scope.cancelChanges = function () {
               
                window.location.reload();
            }
        }

        return {
            general: general,
            articleEdit : articleEdit
        }
    }());

    function userController($scope, $http) {
        $scope.pageManager = new PageManager(5);
        $scope.userFilter;
        $scope.orderByData;
        $scope.pageSizes = [5, 10, 20, 50];
        $scope.userRoles = ['Admin', 'User'];

        $http({
            method: 'GET', url: root + 'users',
            headers: { 'X-sessionKey': '2gFkjkyZXZSgKzquoKJYCEegyqQdfmQICmiMzMMBJvfyLysrYj' }
        }).success(function (data) {
            $scope.data = data;
            $scope.pageManager.loadData(data);
            $scope.pageManager.load();
        });

        $scope.updateUser = function (id) {
            var user = _.find($scope.pageManager.pageData, function (u) { return u.id == id });
            console.log(user);
        }

        $scope.deleteUser = function (id) {
        };

        $scope.filterUsers = function () {
            $http({
                method: 'GET', url: root + 'users',
                headers: { 'X-sessionKey': '2gFkjkyZXZSgKzquoKJYCEegyqQdfmQICmiMzMMBJvfyLysrYj' }
            }).success(function (s) {
                var data = s;

                if ($scope.query) {
                    data = _.filter(data, function (a) {
                        return a.userName.match(new RegExp($scope.query)) ||
                            a.displayName.match(new RegExp($scope.query))
                    });
                }

                if ($scope.userFilter) {
                    switch($scope.userFilter)
                    {
                        case 'admin': {
                            data = _.filter(data, function (u) { return u.isAdmin == 'True' ? true : false });
                        } break;
                        case 'logged': {
                            data = _.filter(data, function (u) { return u.sessionKey ? true : false });
                        } break;
                    }
                }

                if ($scope.orderByFilter) {
                    var filter = $scope.orderByFilter;
                    if (parseInt(_.first(data)[filter])) {
                        if ($scope.orderByOption == 'desc') {
                            data = data.sort(function (x, y) { return y[filter] - x[filter] });
                        } else {
                            data = data.sort(function (x, y) { return x[filter] - y[filter] });
                        }
                    } else {
                        if ($scope.orderByOption == 'desc') {
                            data = data.sort(function (x, y) {
                                return x[filter].localeCompare(y[filter]) * -1
                            });
                        } else {
                            data = data.sort(function (x, y) {
                                return x[filter].localeCompare(y[filter])
                            });
                        }
                    }
                }

                if ($scope.pageSize) {
                    $scope.pageManager.count = parseInt($scope.pageSize);
                }

                $scope.pageManager.loadData(data);
            });
        }
    }

    function homeController($scope, $http) {
        $scope.article = {};
        $scope.article.tags = [];
        $scope.textVal;
        $scope.articleCategory;
        $scope.tagsValue;

        $http.get(root + 'categories').success(function (data) {
            $scope.articleCategories = data;
        });

        $scope.addCategory = function () {
            $http({
                method: 'POST', url: root + 'categories',
                headers: { 'X-sessionKey': '2gFkjkyZXZSgKzquoKJYCEegyqQdfmQICmiMzMMBJvfyLysrYj' }, data: $scope.newCategory
            }).success(function (s) {
                $scope.articleCategories.push(s);
                $scope.newCategory = '';
            });
        }

        $scope.addTag = function () {
            if ($scope.newTag != '') {
                $scope.article.tags.push($scope.newTag);
                $scope.newTag = '';
            }
        }

        $scope.removeTag = function (tag) {
            $scope.article.tags = _.filter($scope.article.tags, function (t) { return t != tag; });
        }

        $scope.saveChanges = function () {
            $scope.article.category = _.find($scope.articleCategories, function (cat) {
                return $scope.articleCategory.name == cat.name
            });
            console.log($scope.articleCategory);
            console.log($scope.articleCategories);
            $scope.article.content = $scope.textVal;
            $scope.article.title = $scope.titleValue;
            console.log($scope.article);
            $http({
                method: 'POST', url: root + 'articles',
                headers: { 'X-sessionKey': '2gFkjkyZXZSgKzquoKJYCEegyqQdfmQICmiMzMMBJvfyLysrYj' }, data: $scope.article
            }).success(function (s) {
                window.location = window.location.href.replace(/#.*$/, '#/admin/articles');
            });
        }

        $scope.cancelChanges = function () {
            window.location.reload();
        }
    }

    return {
        articleController: articleController,
        homeController: homeController,
        userController: userController
    }
}());