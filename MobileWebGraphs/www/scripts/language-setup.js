(function () {
    if (typeof Object.create !== 'function') {
        Object.create = function (o) {
            var F = function () { };
            F.prototype = o;
            return new F();
        };
    }

    if (typeof Function.prototype.method !== 'function') {
        Function.prototype.method = function (name, func) {
            if (!this.prototype[name]) {
                this.prototype[name] = func;
                return this;
            }
        };
    }
})();