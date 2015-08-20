declare function cytoscape(options);
declare function encode_ascii85(string);
declare function decode_ascii85(string);

module UI {
	export class GraphCanvas {
		private scape;
		private nextVertexId: number = 0;
		private nextEdgeId: number = 0;

		constructor(private element: HTMLElement) {
			this.scape = cytoscape({
				container: element,
				wheelSensitivity: 0.1,
				ready: function() {
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

		SetupStyles() {
			this.scape.style().selector("edge")
				.style('line-color', '#000000')
				.style('width', 5);

			this.scape.style().selector("edge:selected")
				.style('line-color', '#009900')
				.style('width', 5);

			this.scape.style().selector("node")
				.style('background-color', '#000000')
				.style('background-opacity', 0.47)
				.style('border-width', 0);


			this.scape.style().selector("node:selected")
				.style('background-color', '#000000')
				.style('background-opacity', 0.47)
				.style('border-color', '#009900')
				.style('border-opacity', 1.0)
				.style('border-width', 6)
				.style('border-style', 'solid')
		}

		SetupEventHandlers() {
			this.scape.on('taphold', (e) => this.OnTapHold(e));
			this.scape.on('taphold', 'node', (e) => this.OnTapHoldVertex(e));
			this.scape.on('taphold', 'edge', (e) => this.OnTapHoldEdge(e));
			this.scape.on('tap', 'node', (e) => this.OnTapVertex(e));
		}

		OnTapHold(e) {
			if (e.cyTarget == this.scape)
				this.AddVertex(e['cyPosition']['x'], e['cyPosition']['y']);
		}

		OnTapHoldVertex(e) {
			var target = e['cyTarget'];
			var addedEdge = false;
			this.scape.elements("node:selected")
				.each((i, v) => {
					var a = v.id();
					var b = target.id();
					if (a != b) {
						this.AddEdge(a, b);
						addedEdge = true;
					}
				});
			if (!addedEdge) {
				this.scape.remove(target);
			}
		}

		OnTapHoldEdge(e) {
			var target = e['cyTarget'];
			this.scape.remove(target);
		}

		OnTapVertex(e) {
		}

		AddVertex(x: number, y: number) {
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
		}

		AddEdge(v1: string, v2: string) {
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
		}

		Center(): void {
			this.scape.center();
		}
	}
}