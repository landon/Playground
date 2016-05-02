WebGraphs.GraphCanvas = function (canvas, scope) {
    this.canvas = canvas;
    this.scope = scope;
    this.state = "doing nothing";
    this.graph = new WebGraphs.Graph(scope);
    this.mouseDownTime = 0;
    this.regionPath = {};
    this.touchedVertex = {};
    this.mouseDownPoint = {};
    this.distanceDragged = 0;
    scope.setup(canvas);
    this.layer = new this.scope.Layer();
};

WebGraphs.GraphCanvas.prototype.buildGraphCanvasFromSilverlightFormat = function (webgraph) {
    var wg = webgraph.replace('webgraph:', '');
    var packed = ascii85.decode('<~' + wg + '~>');
    var compressed = unpack(packed);
    var raw = QLZ.decompress(compressed);

    this.layer.visible = false;
    this.layer = new this.scope.Layer();
}

WebGraphs.GraphCanvas.prototype.doFrame = function (event) {
    this.graph.doFrame(event);
};

WebGraphs.GraphCanvas.prototype.tick = function () {
    switch (this.state) {
        case "doing nothing":
            break;
        case "touching canvas":
            var currentTime = new Date().getTime();
            if (currentTime - this.mouseDownTime > WebGraphs.VertexAddDelay) {
                if (this.graph.addVertex(this.mouseDownPoint)) {
                    this.scope.view.update();
                    navigator.vibrate(50);
                }
                this.state = "doing nothing";
            }
            break;
        case "touching vertex":
            var currentTime = new Date().getTime();
            if (currentTime - this.mouseDownTime > WebGraphs.EdgeAddDelay) {
                if (this.graph.addEdgesToSelected(this.touchedVertex)) {
                    this.scope.view.update();
                    navigator.vibrate(50);
                }
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
                this.touchedVertex = this.graph.findVertexByShape(event.item);
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
            this.graph.doSelection(ctrlDown, this.regionPath);
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
            if (this.touchedVertex.shape.selected)
                this.graph.translateSelected(event.delta);
            else
                this.touchedVertex.translate(event.delta);
            break;
        case "dragging canvas":
            this.regionPath.add(event.point);
            break;

    }
};