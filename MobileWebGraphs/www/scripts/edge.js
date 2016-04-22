WebGraphs.Edge = function (v1, v2, scope) {
    this.scope = scope;
    this.v1 = v1;
    this.v2 = v2;
    this.p1 = this.v1.shape.position;
    this.p2 = this.v2.shape.position;
    this.shape = new this.scope.Path.Line(this.p1, this.p2);
    this.shape.strokeColor = 'black';
    this.shape.isEdge = true;
}

WebGraphs.Edge.prototype.doFrame = function (event) {
    if (this.v1.shape.position.x != this.p1.x || this.v1.shape.position.y != this.p1.y ||
        this.v2.shape.position.x != this.p2.x || this.v2.shape.position.y != this.p2.y) {
        this.p1 = this.v1.shape.position;
        this.p2 = this.v2.shape.position;
        this.shape.remove();
        this.shape = new this.scope.Path.Line(this.p1, this.p2);
        this.shape.strokeColor = 'black';
        this.shape.isEdge = true;
    }
};