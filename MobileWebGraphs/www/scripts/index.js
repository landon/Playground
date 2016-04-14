(function (globals) {
    "use strict";

    document.addEventListener('deviceready', onDeviceReady.bind(this), false);

    var canvas;
    var tool;
    var mouseDownTime;
    var mouseDownPoint;
    var state = "doing nothing";

    function onDeviceReady() {
        document.addEventListener('pause', onPause.bind(this), false);
        document.addEventListener('resume', onResume.bind(this), false);

        canvas = document.getElementById("mainCanvas");
        canvas.width = window.innerWidth;
        canvas.height = window.innerHeight;
        canvas.style.backgroundColor = 'rgba(247, 247, 247, 1.0)';
        paper.setup(canvas);
      
        tool = new paper.Tool();
        tool.activate();

        setInterval(function () {
            if (state === "adding vertex") {
                var currentTime = new Date().getTime();
                if (currentTime - mouseDownTime > 1000) {
                    state = "doing nothing";
                    var myCircle = new paper.Path.Circle(mouseDownPoint, 20);
                    myCircle.fillColor = 'black';
                    myCircle.isSelected = false;
                    paper.view.update();
                }
            };
        }, 100);

        tool.on("mousedown", doMouseDown);

        tool.onMouseDrag = function (event) {
            state = "doing nothing";
        };


        paper.view.draw();
    };

    function doMouseDown(event) {
        tool.on("mouseup", doMouseUp);
        tool.off("mousedown", doMouseDown);
        console.log("down: " + event.count);
        if (event.item) {
            event.item.isSelected = !event.item.isSelected;
            if (event.item.isSelected) {
                event.item.style.strokeColor = 'green';
                event.item.style.strokeWidth = 3;
            }
            else {
                event.item.style.strokeWidth = 0;
            }
        }
        else {
            mouseDownTime = new Date().getTime();
            mouseDownPoint = event.point;
            state = "adding vertex";
        }
    }

    function doMouseUp(event) {
        tool.off("mouseup", doMouseUp);
        tool.on("mousedown", doMouseDown);
        
        console.log("up: " + event.count);
        state = "doing nothing";
    }


    function onPause() {
        // TODO: This application has been suspended. Save application state here.
    };

    function onResume() {
        // TODO: This application has been reactivated. Restore application state here.
    };

    
} )(this);