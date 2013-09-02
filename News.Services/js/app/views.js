window.viewsFactory = (function () {

    var views = Class.create({
        init: function () {
            this.rootPath = "js/partials/";
            this.templates = {};
        },
        loadTemplate: function (name) {
            var self = this;
            var promise = new RSVP.Promise(function (resolve, reject) {
                if (self.templates[name]) {
                    resolve(self.templates[name]);
                } else {
                    $.ajax({
                        url: self.rootPath + name + ".html",
                        type: "GET",
                        success: function (templateHtml) {
                            self.templates[name] = templateHtml;
                            resolve(self.templates[name]);
                        },
                        error: function (err) {
                            reject(err);
                        }
                    });
                }
            });

            return promise;
        },
        sampleView: function () {
            return this.loadTemplate("sample-view");
        },
        getLoginView: function () {
            return this.loadTemplate("loginForm");
        }
    });

    return {
        get: function (rootPath) {
            return new views(rootPath);
        }
    };

})();