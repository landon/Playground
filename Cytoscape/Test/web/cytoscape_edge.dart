part of cytoscape_library;

class CytoscapeEdge {
  JsObject _edge;

  String get id => _vertex.callMethod('id');
  bool get isSelected => _vertex.callMethod('selected');

  CytoscapeEdge(this._edge);
}
