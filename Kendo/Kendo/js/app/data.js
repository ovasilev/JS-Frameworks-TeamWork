/// <reference path="../libs/_references.js" />

window.persister = (function () {

    var displayName = localStorage.getItem("displayName");
    var sessionKey = localStorage.getItem("sessionKey");

    function saveUserData(userData) {
        localStorage.setItem("displayName", userData.displayName);
        localStorage.setItem("sessionKey", userData.sessionKey);
        displayName = userData.displayName;
        sessionKey = userData.sessionKey;
    }

    function removeUserData() {
        localStorage.removeItem("displayName");
        localStorage.removeItem("sessionKey");
        displayName = null;
        sessionKey = null;
    }

    var dataPersister = Class.create({
        init: function (apiUrl) {
            this.apiUrl = apiUrl;
            this.users = new usersPersister(this.apiUrl + "/users");
            this.articles = new articlesPersister(this.apiUrl + "/articles");
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

    return {
        getData: function (apiUrl) {
            return new dataPersister(apiUrl);
        }
    };
})();