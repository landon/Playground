WebGraphs.Graph = function (scope) {
    this.scope = scope;
    this.vertices = [];
    this.edges = [];
};

WebGraphs.Graph.prototype.deserialize = function (rawWebgraph) {

}


WebGraphs.Graph.prototype.doFrame = function (event) {
    this.vertices.forEach(function (v) {
        v.doFrame(event);
    }, this);

    this.edges.forEach(function (e) {
        e.doFrame(event);
    }, this);
};

WebGraphs.Graph.prototype.addVertex = function (p) {
    var v = new WebGraphs.Vertex(p, this.scope);
    this.vertices.push(v);

    return true;
};

WebGraphs.Graph.prototype.doSelection = function (ctrlDown, regionPath) {
    this.vertices.forEach(function (v) {
        if (!ctrlDown)
            v.shape.selected = false;
        if (regionPath.contains(v.shape.position))
            v.shape.selected = !v.shape.selected;
    }, this);
};


WebGraphs.Graph.prototype.getSelectedVertices = function () {
    return this.vertices.filter(function (v) {
        return v.shape.selected;
    });
};
WebGraphs.Graph.prototype.deleteSelected = function () {
    this.getSelectedVertices().forEach(function (v) {
        this.removeVertex(v);
    }, this);
};

WebGraphs.Graph.prototype.translateSelected = function (delta) {
    this.getSelectedVertices().forEach(function (v) {
        v.translate(delta);
    }, this);
};


WebGraphs.Graph.prototype.findVertexByShape = function (s) {
    return this.vertices.find(function (v) {
        return v.shape == s;
    });
};

WebGraphs.Graph.prototype.areAdjacent = function (v1, v2) {
    return this.edges.some(function (e) {
        return e.v1 == v1 && e.v2 == v2 || e.v1 == v2 && e.v2 == v1;
    });
};

WebGraphs.Graph.prototype.removeVertex = function (v) {
    v.shape.remove();
    this.vertices.remove(v);
    this.edges.removeAll(function (e) {
        var incident = e.v1 == v || e.v2 == v;
        if (incident)
            e.shape.remove();
        return incident;
    });
};

WebGraphs.Graph.prototype.addEdge = function (v1, v2) {
    this.edges.push(new WebGraphs.Edge(v1, v2, this.scope));
};

WebGraphs.Graph.prototype.addEdgesToSelected = function (touchedVertex) {
    var added = false;
    this.vertices.forEach(function (v) {
        if (v.shape.selected && !this.areAdjacent(touchedVertex, v)) {
            this.addEdge(touchedVertex, v);
            added = true;
        }
    }, this);

    return added;
};
