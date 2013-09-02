var Class = (function () {
    function create(init, properties) {
        var newInstance = function () {
            init.apply(this, arguments);
        }

        newInstance.prototype = {};
        newInstance.prototype.init = init;

        for (var prop in properties) {
            newInstance.prototype[prop] = properties[prop];
        }

        return newInstance;
    }

    function copyParentPrototype(parentPrototype) {
        function F() { };

        F.prototype = parentPrototype;

        return new F();
    }

    //to check
    Function.prototype.extend = function (properties) {
        for (var prop in properties) {
            this.prototype[prop] = properties[prop];
        }
    }

    Function.prototype.inherit = function (parent) {

        var originalPrototype = this.prototype;

        this.prototype = copyParentPrototype(parent.prototype);

        this.prototype._super = parent.prototype;

        for (var prop in originalPrototype) {
            this.prototype[prop] = originalPrototype[prop];
        }
    }

    return {
        create: create
    }
}());
