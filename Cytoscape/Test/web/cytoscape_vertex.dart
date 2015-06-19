part of cytoscape_library;

class CytoscapeVertex {
  JsObject _vertex;

  String get id => _vertex.callMethod('id');
  num get x => _vertex.callMethod('position', ['x']);
  num get y => _vertex.callMethod('position', ['y']);
  bool get isSelected => _vertex.callMethod('selected');

  CytoscapeVertex(this._vertex);
}
