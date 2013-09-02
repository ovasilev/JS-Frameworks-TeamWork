/// <reference path="_references.js" />

var requester = (function() {

    function getJson(url, headers) {
            var promise = new RSVP.Promise(function(resolve, reject) {
                $.ajax({                
                    url: url,
                    type: "GET",
                    dataType: "json",
                    headers: headers,
                    success: function(data) {
                        resolve(data);
                    },
                    error: function(err) {
                        reject(err);
                    }
                });
            });

            return promise;
    }
    
    function postJson(url, data, headers) {
            var promise = new RSVP.Promise(function (resolve, reject) {
                $.ajax({
                    url: url,
                    type: "POST",
                    dataType: "json",
                    contentType: "application/json",
                    data: JSON.stringify(data),
                    headers: headers,
                    success: function (data) {
                        resolve(data);
                    },
                    error: function (err) {
                        reject(err);
                    }
                });
            });

            return promise;
    }
    
    function putJson(url, data, headers) {
            var promise = new RSVP.Promise(function (resolve, reject) {
                $.ajax({
                    url: url,
                    type: "PUT",
                    dataType: "json",
                    contentType: "application/json",
                    data: JSON.stringify(data),
                    headers: headers,
                    success: function (data) {
                        resolve(data);
                    },
                    error: function (err) {
                        reject(err);
                    }
                });
            });

            return promise;
    }
    
    function deleteJson(url, headers) {
            var promise = new RSVP.Promise(function (resolve, reject) {
                $.ajax({
                    url: url,
                    type: "DELETE",
                    dataType: "json",
                    headers: headers,
                    success: function (data) {
                        resolve(data);
                    },
                    error: function (err) {
                        reject(err);
                    }
                });
            });

            return promise;
    }

    return {
        getJson: getJson,
        postJson: postJson,
        putJson: putJson,
        deleteJson: deleteJson
    };
})();