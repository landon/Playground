(function () {
    "use strict";

    document.addEventListener('deviceready', onDeviceReady.bind(this), false);

    var tool;
    var graphCanvas;

    function onDeviceReady() {
        document.addEventListener('pause', onPause.bind(this), false);
        document.addEventListener('resume', onResume.bind(this), false);
        
        var canvas = document.getElementById("mainCanvas");
        canvas.width = window.innerWidth;
        canvas.height = window.innerHeight;
        canvas.style.backgroundColor = 'rgba(247, 247, 247, 1.0)';

        graphCanvas = new WebGraphs.GraphCanvas(canvas, paper);
        WebGraphs.TheGraphCanvas = graphCanvas;

        tool = new paper.Tool();
        tool.on("mousedown", doMouseDown);
        tool.on("mousedrag", doMouseDrag);
        tool.activate();

        paper.view.onFrame = doFrame;
        setInterval(doTick, 10);
        paper.view.draw();
    };

    function doFrame(event) {
        graphCanvas.doFrame(event);
    }
    function doTick() {
        graphCanvas.tick();
    }
    function doMouseDrag(event) {
        graphCanvas.doMouseDrag(event);
    }
    function doMouseDown(event) {
        tool.on("mouseup", doMouseUp);
        tool.off("mousedown", doMouseDown);

        graphCanvas.doMouseDown(event);
    }
    function doMouseUp(event) {
        tool.off("mouseup", doMouseUp);
        tool.on("mousedown", doMouseDown);

        graphCanvas.doMouseUp(event);
    }

    function onPause() {
    };

    function onResume() {
    };

    
})();