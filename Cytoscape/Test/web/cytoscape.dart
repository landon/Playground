part of cytoscape_library;

class Cytoscape {
  JsObject _cytoscape;
  int _verticesAdded = 0;
  int _edgesAdded = 0;
  var _ready;

  Cytoscape(Element element, ready) {
    _ready = ready;

    var style = new JsObject(context['cytoscape']['stylesheet']);
    var nodeSelectedStyle = style.callMethod('selector', ['node:selected']);
    nodeSelectedStyle.callMethod('css', [new JsObject.jsify({
        'background-color': '#000',
        'text-outline-color': '#000'
      })]);

    var edgeStyle = style.callMethod('selector', ['edge']);
    edgeStyle.callMethod('css', [new JsObject.jsify({
        'width': 4,
        'line-color': '#ddd',
      })]);

    _cytoscape = new JsObject(context['cytoscape'], [new JsObject.jsify({
        "container": element,
        "ready": _preReady,
        "style": style
      })]);
  }

  CytoscapeVertex AddVertex(num x, num y) {
    var id = _nextVertexId();

    _cytoscape.callMethod('add', [new JsObject.jsify({
        "group": "nodes",
        "data": {
          "id": id
        },
        "position": {
          "x": x,
          "y": y
        }
      })]);

    return new CytoscapeVertex(_cytoscape.callMethod('getElementById', [id]));
  }

  CytoscapeEdge AddEdge(CytoscapeVertex v1, CytoscapeVertex v2) {
    var edgeId = _nextEdgeId();

    _cytoscape.callMethod('add', [new JsObject.jsify({
        "group": "edges",
        "data": {
          "id": edgeId,
          "source": v1.id,
          "target": v2.id
        }
      })]);

    return new CytoscapeEdge(_cytoscape.callMethod('getElementById', [edgeId]));
  }

  dynamic GetVertices() => _cytoscape.callMethod('nodes');
  dynamic GetSelectedVertices() => GetVertices().callMethod('filter', [(i, v) => new CytoscapeVertex(v).isSelected]);

  void Center() {
    _cytoscape.callMethod('center');
  }

  String _nextVertexId() {
    return "v" + (_verticesAdded++).toString();
  }

  String _nextEdgeId() {
    return "e" + (_edgesAdded++).toString();
  }

  void _preReady(var data) {
    _cytoscape.callMethod('on', ['taphold', _onTapHold]);
    _cytoscape.callMethod('on', ['tap', 'node', _onTapVertex]);

    _ready(data);
  }

  void _onTapHold(var e) {
    AddVertex(e['cyPosition']['x'], e['cyPosition']['y']);
  }

  void _onTapVertex(var e) {
    var target = new CytoscapeVertex(e['cyTarget']);

    var selected = GetSelectedVertices();
    selected.callMethod('each', [(i, v) => AddEdge(target, new CytoscapeVertex(v))]);
  }
}
