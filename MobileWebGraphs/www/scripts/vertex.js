WebGraphs.Vertex = function (position, scope) {
    this.scope = scope;
    this.shape = new scope.Path.Circle(position, 15);
    this.shape.fillColor = 'black';
    this.shape.isVertex = true;
}

WebGraphs.Vertex.prototype.translate = function (delta) {
    this.shape.translate(delta);
};

WebGraphs.Vertex.prototype.doFrame = function (event) {
};

