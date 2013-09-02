/// <reference path="../libs/_references.js" />

window.persister = (function () {

    var displayName = localStorage.getItem("displayName");
    var sessionKey = localStorage.getItem("sessionKey");
    var isAdmin = localStorage.getItem("isAdmin");

    function saveUserData(userData) {
        localStorage.setItem("displayName", userData.displayName);
        localStorage.setItem("sessionKey", userData.sessionKey);
        localStorage.setItem("isAdmin", userData.isAdmin);
        displayName = userData.displayName;
        sessionKey = userData.sessionKey;
        isAdmin = userData.isAdmin;
    }

    function removeUserData() {
        localStorage.removeItem("displayName");
        localStorage.removeItem("sessionKey");
        localStorage.removeItem("isAdmin");
        displayName = null;
        sessionKey = null;
        isAdmin = null;
    }

    var dataPersister = Class.create({
        init: function (apiUrl) {
            this.apiUrl = apiUrl;
            this.users = new usersPersister(this.apiUrl + "/users");
            this.articles = new articlesPersister(this.apiUrl + "/articles");
            this.categories = new cateogiesPersister(this.apiUrl + "/categories");
            this.tags = new tagsPersister(this.apiUrl + "/tags");
        },
        userStatus: function() {
            if (isAdmin == "true") {
                return "admin";
            } else if (isAdmin == "false") {
                return "user";
            } else {
                return "guest";
            }
        }
    });

    var usersPersister = Class.create({
        init: function (apiUrl) {
            this.apiUrl = apiUrl;
        },
        login: function (username, password) {
            var url = this.apiUrl + "/login";
            var userData = {
                username: username,
                authCode: CryptoJS.SHA1(password).toString()
            };

            return requester.postJson(url, userData)
                .then(function (data) {
                    saveUserData(data);
                    return data;
                });
        },
        register: function (username, displayName, password) {
            var url = this.apiUrl + "/register";
            var userData = {
                username: username,
                displayName: displayName,
                authCode: CryptoJS.SHA1(password).toString()
            };

            return requester.postJson(url, userData)
                .then(function (data) {
                    saveUserData(data);
                    return data;
                });
        },
        logout: function () {
            var url = this.apiUrl + "/logout";

            return requester.getJson(url).then(function () {
                removeUserData();
            });
        }
    });

    var articlesPersister = Class.create({
        init: function (apiUrl) {
            this.apiUrl = apiUrl;
        },
        all: function () {
            var url = this.apiUrl;

            return requester.getJson(url);
        },
        single: function (id) {
            var url = this.apiUrl + "/" + id;

            return requester.getJson(url);
        },
        read: function (id) {
            var url = this.apiUrl + "/" + id + "/read";

            return requester.putJson(url);
        }
    });
    
    var cateogiesPersister = Class.create({
        init: function (apiUrl) {
            this.apiUrl = apiUrl;
        },
        all: function () {
            var url = this.apiUrl;

            return requester.getJson(url);
        },
        single: function (id) {
            var url = this.apiUrl + "/" +  + "/articles";

            return requester.getJson(url);
        }
    });
    
    var tagsPersister = Class.create({
        init: function (apiUrl) {
            this.apiUrl = apiUrl;
        },
        all: function () {
            var url = this.apiUrl;

            return requester.getJson(url);
        },
        single: function (id) {
            var url = this.apiUrl + "/" + id + "/articles";

            return requester.getJson(url);
        }
    });

    return {
        getData: function (apiUrl) {
            return new dataPersister(apiUrl);
        }
    };
})();