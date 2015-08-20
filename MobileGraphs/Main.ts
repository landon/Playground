///<reference path="GraphCanvas.ts"/>

module UI {
    export class Main {
        constructor(private root: HTMLDocument) {
            window.onload = this.Initialize;
        }

        Initialize(): void {
            console.log("initializing");
            var element = document.getElementById('cy');

            new GraphCanvas(element);
        }
    }
}

new UI.Main(document);