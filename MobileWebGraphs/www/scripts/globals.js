﻿var WebGraphs = {};
WebGraphs.VertexAddDelay = 200;
WebGraphs.EdgeAddDelay = 500;

if (!Array.prototype.find) {
    Array.prototype.find = function (predicate) {
        if (this === null) {
            throw new TypeError('Array.prototype.find called on null or undefined');
        }
        if (typeof predicate !== 'function') {
            throw new TypeError('predicate must be a function');
        }
        var list = Object(this);
        var length = list.length >>> 0;
        var thisArg = arguments[1];
        var value;

        for (var i = 0; i < length; i++) {
            value = list[i];
            if (predicate.call(thisArg, value, i, list)) {
                return value;
            }
        }
        return undefined;
    };
}


// Mozilla 1.8 has support for indexOf, lastIndexOf, forEach, filter, map, some, every
// http://developer-test.mozilla.org/docs/Core_JavaScript_1.5_Reference:Objects:Array:lastIndexOf
if (!Array.prototype.indexOf) {
    Array.prototype.indexOf = function (obj, fromIndex) {
        if (fromIndex == null) {
            fromIndex = 0;
        } else if (fromIndex < 0) {
            fromIndex = Math.max(0, this.length + fromIndex);
        }
        for (var i = fromIndex; i < this.length; i++) {
            if (this[i] === obj)
                return i;
        }
        return -1;
    };
}

// http://developer-test.mozilla.org/docs/Core_JavaScript_1.5_Reference:Objects:Array:lastIndexOf
if (!Array.prototype.lastIndexOf) {
    Array.prototype.lastIndexOf = function (obj, fromIndex) {
        if (fromIndex == null) {
            fromIndex = this.length - 1;
        } else if (fromIndex < 0) {
            fromIndex = Math.max(0, this.length + fromIndex);
        }
        for (var i = fromIndex; i >= 0; i--) {
            if (this[i] === obj)
                return i;
        }
        return -1;
    };
}


// http://developer-test.mozilla.org/docs/Core_JavaScript_1.5_Reference:Objects:Array:forEach
if (!Array.prototype.forEach) {
    Array.prototype.forEach = function (f, obj) {
        var l = this.length;	// must be fixed during loop... see docs
        for (var i = 0; i < l; i++) {
            f.call(obj, this[i], i, this);
        }
    };
}

// http://developer-test.mozilla.org/docs/Core_JavaScript_1.5_Reference:Objects:Array:filter
if (!Array.prototype.filter) {
    Array.prototype.filter = function (f, obj) {
        var l = this.length;	// must be fixed during loop... see docs
        var res = [];
        for (var i = 0; i < l; i++) {
            if (f.call(obj, this[i], i, this)) {
                res.push(this[i]);
            }
        }
        return res;
    };
}

// http://developer-test.mozilla.org/docs/Core_JavaScript_1.5_Reference:Objects:Array:map
if (!Array.prototype.map) {
    Array.prototype.map = function (f, obj) {
        var l = this.length;	// must be fixed during loop... see docs
        var res = [];
        for (var i = 0; i < l; i++) {
            res.push(f.call(obj, this[i], i, this));
        }
        return res;
    };
}

// http://developer-test.mozilla.org/docs/Core_JavaScript_1.5_Reference:Objects:Array:some
if (!Array.prototype.some) {
    Array.prototype.some = function (f, obj) {
        var l = this.length;	// must be fixed during loop... see docs
        for (var i = 0; i < l; i++) {
            if (f.call(obj, this[i], i, this)) {
                return true;
            }
        }
        return false;
    };
}

// http://developer-test.mozilla.org/docs/Core_JavaScript_1.5_Reference:Objects:Array:every
if (!Array.prototype.every) {
    Array.prototype.every = function (f, obj) {
        var l = this.length;	// must be fixed during loop... see docs
        for (var i = 0; i < l; i++) {
            if (!f.call(obj, this[i], i, this)) {
                return false;
            }
        }
        return true;
    };
}

Array.prototype.contains = function (obj) {
    return this.indexOf(obj) != -1;
};

Array.prototype.copy = function (obj) {
    return this.concat();
};

Array.prototype.insertAt = function (obj, i) {
    this.splice(i, 0, obj);
};

Array.prototype.insertBefore = function (obj, obj2) {
    var i = this.indexOf(obj2);
    if (i == -1)
        this.push(obj);
    else
        this.splice(i, 0, obj);
};

Array.prototype.removeAt = function (i) {
    this.splice(i, 1);
};

Array.prototype.remove = function (obj) {
    var j = 0;
    for (var i = 0, l = this.length; i < l; i++) {
        if (this[i] !== obj) {
            this[j++] = this[i];
        }
    }
    this.length = j;
};

Array.prototype.removeAll = function (f) {
    var j = 0;
    for (var i = 0, l = this.length; i < l; i++) {
        if (!f(this[i])) {
            this[j++] = this[i];
        }
    }
    this.length = j;
};