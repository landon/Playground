WebGraphs.GraphCanvas = function (canvas, scope) {
    scope.setup(canvas);
    this.canvas = canvas;
    this.scope = scope;
    this.layers = [];
    this.newGraphLayer();
};

WebGraphs.GraphCanvas.prototype.newGraphLayer = function () {
    if (this.currentLayer)
        this.currentLayer.visible = false;
    var layer = new this.scope.Layer();
    layer.activate();
    this.currentLayer = layer;
    this.layers.push(layer);

    this.graph = new WebGraphs.Graph(this.scope);
    this.mouseDownTime = 0;
    this.regionPath = {};
    this.touchedVertex = {};
    this.mouseDownPoint = {};
    this.distanceDragged = 0;
    this.state = "doing nothing";

    layer.graph = this.graph;
}

WebGraphs.GraphCanvas.prototype.buildGraphCanvasFromSilverlightFormat = function (webgraph) {
    var wg = webgraph.replace('webgraph:', '');
    var packed = ascii85.decode('<~' + wg + '~>');
    var compressed = unpack(packed);
    var raw = QLZ.decompress(compressed);

    this.newGraphLayer();

    var dv = new DataView(raw.buffer);
    var offset = 0;
    var minx = 10000;
    var maxx = -10000;
    var miny = 10000;
    var maxy = -10000;
    var n = dv.getUint8(offset, true); offset += 1;
    for (var i = 0; i < n; i++) {
        var x = dv.getUint16(offset, true); offset += 2;
        var y = dv.getUint16(offset, true); offset += 2;
        var padding = dv.getUint16(offset, true); offset += 2;

        var labelLength = dv.getUint8(offset, true); offset += 1;
        offset += labelLength;
        var styleLength = dv.getUint8(offset, true); offset += 1;
        offset += styleLength;

        var p = new this.scope.Point(800 * x / 10000, 800 * y / 10000);
        if (p.x < minx)
            minx = p.x;
        if (p.y < miny)
            miny = p.y;
        if (p.x > maxx)
            maxx = p.x;
        if (p.y > maxy)
            maxy = p.y;
        this.graph.addVertex(p);
    }

    var e = dv.getUint16(offset, true); offset += 2;
    for (var i = 0; i < e; i++) {
        var i1 = dv.getUint8(offset, true); offset += 1;
        var i2 = dv.getUint8(offset, true); offset += 1;
        var multiplicity = dv.getUint8(offset, true); offset += 1;
        var orientation = dv.getUint8(offset, true); offset += 1;
        var thickness = dv.getUint16(offset, true) / 100; offset += 2;

        var styleLength = dv.getUint8(offset, true); offset += 1;
        offset += styleLength;
      
        this.graph.addEdge(this.graph.vertices[i1], this.graph.vertices[i2]);
    }

    this.graph.translate(new this.scope.Point(this.scope.view.center.x -  (minx + maxx) / 2, this.scope.view.center.y - (miny + maxy) / 2));
    this.scope.view.update();
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