import 'dart:html';
import 'dart:js';

import 'cytoscape_library.dart';

Cytoscape cytoscape;
void main() {
  cytoscape = new Cytoscape(querySelector('#cy'), ready);
}

void ready(var data) {
  var v0 = cytoscape.AddVertex(10, 10);
  var v1 = cytoscape.AddVertex(10, 110);
  var v2 = cytoscape.AddVertex(110, 10);
  var v3 = cytoscape.AddVertex(110, 110);
  
  cytoscape.Center();
}
