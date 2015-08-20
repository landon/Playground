var UI;
(function (UI) {
    var GraphCanvas = (function () {
        function GraphCanvas(element) {
            this.element = element;
            this.nextVertexId = 0;
            this.nextEdgeId = 0;
            this.scape = cytoscape({
                container: element,
                wheelSensitivity: 0.1,
                ready: function () {
                    console.log("cytoscape ready");
                }
            });

            var encoded = encode_ascii85('This is a test!');
            var decoded = decode_ascii85(encoded);
            console.log(encoded);
            console.log(decoded);

            //this.scape.userPanningEnabled(false);
            this.scape.userZoomingEnabled(true);
            this.scape.boxSelectionEnabled(true);

            this.SetupStyles();
            this.SetupEventHandlers();
        }
        GraphCanvas.prototype.SetupStyles = function () {
            this.scape.style().selector("edge").style('line-color', '#000000').style('width', 5);

            this.scape.style().selector("edge:selected").style('line-color', '#009900').style('width', 5);

            this.scape.style().selector("node").style('background-color', '#000000').style('background-opacity', 0.47).style('border-width', 0);

            this.scape.style().selector("node:selected").style('background-color', '#000000').style('background-opacity', 0.47).style('border-color', '#009900').style('border-opacity', 1.0).style('border-width', 6).style('border-style', 'solid');
        };

        GraphCanvas.prototype.SetupEventHandlers = function () {
            var _this = this;
            this.scape.on('taphold', function (e) {
                return _this.OnTapHold(e);
            });
            this.scape.on('taphold', 'node', function (e) {
                return _this.OnTapHoldVertex(e);
            });
            this.scape.on('taphold', 'edge', function (e) {
                return _this.OnTapHoldEdge(e);
            });
            this.scape.on('tap', 'node', function (e) {
                return _this.OnTapVertex(e);
            });
        };

        GraphCanvas.prototype.OnTapHold = function (e) {
            if (e.cyTarget == this.scape)
                this.AddVertex(e['cyPosition']['x'], e['cyPosition']['y']);
        };

        GraphCanvas.prototype.OnTapHoldVertex = function (e) {
            var _this = this;
            var target = e['cyTarget'];
            var addedEdge = false;
            this.scape.elements("node:selected").each(function (i, v) {
                var a = v.id();
                var b = target.id();
                if (a != b) {
                    _this.AddEdge(a, b);
                    addedEdge = true;
                }
            });
            if (!addedEdge) {
                this.scape.remove(target);
            }
        };

        GraphCanvas.prototype.OnTapHoldEdge = function (e) {
            var target = e['cyTarget'];
            this.scape.remove(target);
        };

        GraphCanvas.prototype.OnTapVertex = function (e) {
        };

        GraphCanvas.prototype.AddVertex = function (x, y) {
            var id = "v" + (this.nextVertexId++);

            this.scape.add({
                "group": "nodes",
                "data": {
                    "id": id
                },
                "position": {
                    "x": x,
                    "y": y
                }
            });
        };

        GraphCanvas.prototype.AddEdge = function (v1, v2) {
            console.log(v1);
            console.log(v2);

            var id = "e" + (this.nextEdgeId++);

            this.scape.add({
                "group": "edges",
                "data": {
                    "id": id,
                    "source": v1,
                    "target": v2
                }
            });
        };

        GraphCanvas.prototype.Center = function () {
            this.scape.center();
        };
        return GraphCanvas;
    })();
    UI.GraphCanvas = GraphCanvas;
})(UI || (UI = {}));
///<reference path="GraphCanvas.ts"/>
var UI;
(function (UI) {
    var Main = (function () {
        function Main(root) {
            this.root = root;
            window.onload = this.Initialize;
        }
        Main.prototype.Initialize = function () {
            console.log("initializing");
            var element = document.getElementById('cy');

            new UI.GraphCanvas(element);
        };
        return Main;
    })();
    UI.Main = Main;
})(UI || (UI = {}));

new UI.Main(document);
