WebGraphs.GraphCanvas = function (canvas, scope) {
    this.canvas = canvas;
    this.scope = scope;
    this.state = "doing nothing";
    this.vertices = [];
    this.edges = [];
    this.mouseDownTime = 0;
    this.regionPath = {};
    this.touchedVertex = {};
    this.mouseDownPoint = {};
    this.distanceDragged = 0;

    scope.setup(canvas);
};

WebGraphs.GraphCanvas.prototype.areAdjacent = function (v1, v2) {
    return this.edges.some(function (e) {
        return e.v1 == v1 && e.v2 == v2 || e.v1 == v2 && e.v2 == v1;
    });
};

WebGraphs.GraphCanvas.prototype.doFrame = function (event) {
    this.vertices.forEach(function (v) {
        v.doFrame(event);
    }, this);

    this.edges.forEach(function (e) {
        e.doFrame(event);
    }, this);
};

WebGraphs.GraphCanvas.prototype.tick = function () {
    switch (this.state) {
        case "doing nothing":
            break;
        case "touching canvas":
            var currentTime = new Date().getTime();
            if (currentTime - this.mouseDownTime > WebGraphs.VertexAddDelay) {
                var v = new WebGraphs.Vertex(this.mouseDownPoint, paper);
                this.vertices.push(v);
                this.scope.view.update();
                navigator.vibrate(50);
                this.state = "doing nothing";
            }
            break;
        case "touching vertex":
            var currentTime = new Date().getTime();
            if (currentTime - this.mouseDownTime > WebGraphs.EdgeAddDelay) {
                this.vertices.forEach(function (v) {
                    var added = false;
                    if (v.shape.selected && !this.areAdjacent(this.touchedVertex, v)) {
                        this.edges.push(new WebGraphs.Edge(this.touchedVertex, v, paper));
                        added = true;
                    }

                    if (added)
                        navigator.vibrate(50);
                }, this);

                this.scope.view.update();
                this.state = "doing nothing";
            }
            break;
        case "dragging vertex":
            break;
        case "dragging canvas":
            break;
    }
};

WebGraphs.GraphCanvas.prototype.doMouseDown = function (event) {
    this.mouseDownTime = new Date().getTime();
    this.mouseDownPoint = event.point;
    this.distanceDragged = 0;

    switch (this.state) {
        case "doing nothing":
            if (event.item && event.item.isVertex) {
                this.state = "touching vertex";
                this.touchedVertex = this.vertices.find(function(v) {
                    return v.shape == event.item;
                });
            }
            else {
                this.state = "touching canvas";
            }
            break;
        case "touching canvas":
            this.state = "doing nothing";
            break;
        case "touching vertex":
            this.state = "doing nothing";
            break;
        case "dragging vertex":
            this.state = "doing nothing";
            break;
        case "dragging canvas":
            this.state = "doing nothing";
            break;
    }
};

WebGraphs.GraphCanvas.prototype.doMouseUp = function (event) {
    switch (this.state) {
        case "doing nothing":
            break;
        case "touching canvas":
            this.state = "doing nothing";
            break;
        case "touching vertex":
            this.state = "doing nothing";
            break;
        case "dragging vertex":
            this.state = "doing nothing";
            break;
        case "dragging canvas":
            this.state = "doing nothing";
            var ctrlDown = this.scope.Key.isDown('control');
            this.vertices.forEach(function (v) {
                if (!ctrlDown)
                    v.shape.selected = false;
                if (this.regionPath.contains(v.shape.position))
                    v.shape.selected = !v.shape.selected;
            }, this);
            this.regionPath.remove();
            break;

    }
};

WebGraphs.GraphCanvas.prototype.doMouseDrag = function (event) {
    this.distanceDragged += event.delta.length;
    switch (this.state) {
        case "doing nothing":
            break;
        case "touching canvas":
            if (this.distanceDragged > 30) {
                this.state = "dragging canvas";
                this.regionPath = new this.scope.Path();
                this.regionPath.strokeColor = 'blue';
                this.regionPath.add(event.point);
            }
            break;
        case "touching vertex":
            if (this.distanceDragged > 30) {
                this.state = "dragging vertex";
            }
            break;
        case "dragging vertex":
            if (this.touchedVertex.shape.selected) {
                this.vertices.forEach(function (v) {
                    if (v.shape.selected) {
                        v.translate(event.delta);
                    }
                });
            }
            else {
                this.touchedVertex.translate(event.delta);
            }
            break;
        case "dragging canvas":
            this.regionPath.add(event.point);
            break;

    }
};

WebGraphs.GraphCanvas.prototype.deleteSelected = function () {
    var selectedVertices = this.vertices.filter(function (v) {
        return v.shape.selected;
    });

    selectedVertices.forEach(function (v) {
        v.shape.remove();
        this.vertices.remove(v);
        this.edges.removeAll(function (e) {
            var incident = e.v1 == v || e.v2 == v;
            if (incident)
                e.shape.remove();
            return incident;
        });
    }, this);
};