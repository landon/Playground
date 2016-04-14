WebGraphs.GraphCanvas = function (canvas, scope) {
    this.canvas = canvas;
    this.scope = scope;
    this.state = "doing nothing";
    this.vertices = [];
    this.mouseDownTime = 0;
    this.regionPath = {};
    this.draggedItem = {};
    this.mouseDownPoint = {};

    scope.setup(canvas);
}

WebGraphs.GraphCanvas.prototype.tick = function () {
    switch (this.state) {
        case "doing nothing":
            break;
        case "adding vertex":
            var currentTime = new Date().getTime();
            if (currentTime - this.mouseDownTime > 200) {
                this.state = "doing nothing";
                var v = new WebGraphs.Vertex(this.mouseDownPoint, paper);
                this.vertices.push(v);
                this.scope.view.update();
            }
            break;
        case "dragging item":
            break;
        case "dragging selected items":
            break;
        case "drawing region":
            break;
    }
};

WebGraphs.GraphCanvas.prototype.doMouseDrag = function (event) {
    switch (this.state) {
        case "doing nothing":
            break;
        case "adding vertex":
            this.state = "drawing region";
            this.regionPath = new this.scope.Path();
            this.regionPath.strokeColor = 'blue';
            this.regionPath.add(event.point);
            break;
        case "dragging item":
            this.draggedItem.translate(event.delta);
            break;
        case "dragging selected items":
            this.vertices.forEach(function (v) {
                if (v.shape.selected) {
                    v.translate(event.delta);
                }
            });
            break;
        case "drawing region":
            this.regionPath.add(event.point);
            break;

    }
};

WebGraphs.GraphCanvas.prototype.doMouseDown = function (event) {
    switch (this.state) {
        case "doing nothing":
            if (event.item && event.item.isVertex) {
                if (event.item.selected) {
                    this.state = "dragging selected items";
                }
                else {
                    this.state = "dragging item";
                    this.draggedItem = event.item;
                }
            }
            else {
                this.mouseDownTime = new Date().getTime();
                this.mouseDownPoint = event.point;
                this.state = "adding vertex";
            }
            break;
        case "adding vertex":
            this.state = "doing nothing";
            break;
        case "dragging item":
            this.state = "doing nothing";
            break;
        case "dragging selected items":
            this.state = "doing nothing";
            break;
        case "drawing region":
            this.state = "doing nothing";
            break;
    }
};

WebGraphs.GraphCanvas.prototype.doMouseUp = function (event) {
    switch (this.state) {
        case "adding vertex":
            this.state = "doing nothing";
            break;
        case "dragging item":
            this.state = "doing nothing";
            break;
        case "dragging selected items":
            this.state = "doing nothing";
            break;
        case "drawing region":
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
}